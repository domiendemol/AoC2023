using System.ComponentModel.Design;
using System.Diagnostics;

namespace AoC2023;

public class Day21
{
	private Vector2Int[] _directions = new[] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };

	private char[,] grid;
	
	// BFs
	private bool[,] visited;
	private Vector2Int[,] prevs;
	
	public void Run(List<string> input)
	{
		grid = new char[input.Count, input[0].Length];
		Vector2Int start = new Vector2Int(0,0);
		for (int i = 0; i < input.Count; i++) {
			for (int j = 0; j < grid.GetLength(1); j++) {
				grid[i, j] = input[i][j];
				if (grid[i, j] == 'S') start = new Vector2Int(i,j);
			}
		}
		// strategy: calculated shortest path for all positions within 64 dist, using modified BFS
		int[,] dists = BFS(start, 64);
		
		int part1 = 0;
		for (int i = 0; i < input.Count; i++) {
			for (int j = 0; j < grid.GetLength(1); j++) {
				// if it can be reached with 64 steps or a lower amount divisible by 2, then it's good
				// if uneven, it's never possible to reach it with 64 since we only do wind direction changes
				if (dists[i, j] <= 64 && dists[i,j] % 2 == 0) part1++;
			}
		}
		Console.WriteLine($"PART 1: {part1}");
		
		// PART 2: can we brute force it by using dicts instead of a fixed grid??
		
		// option 2 is to math calculate it, 26501365 steps = k-l grid of grids
		// we can calculate shortest paths for all places in our original grid
		// but then what? we only need to know if shortest path is even or uneven
		// --> outer edges of first grid are 65 steps away, so will always be uneven
		// so we should be able to figure out in which grids we will end up
		// oooh there's a line through the middle!!
		// we can use manhattan distance to count the grids too!

		int steps = 26501365;
		int width = grid.GetLength(0);
		int half = width / 2;
		bool even = (steps % 2 == 0);
		
		int nrBlocks = ((steps-half)/width); // 202300
		long evenBlocks = 0;
		long unevenBlocks = 1;
		for (int a = 1; a < nrBlocks; a++) // layers
		{
			if (a % 2 == 0) unevenBlocks += 4 * a;
			else evenBlocks += 4 * a;
		}
		dists = BFS(start, 0);
		long reachableWithUnevenDist = CountCells(even, dists);
		long reachableWithEvenDist = CountCells(!even, dists);
		
		// end cells
		dists = BFS(new Vector2Int(-1,half), width);
		long reachableEndGrid1 = CountCells(false, dists);
		dists = BFS(new Vector2Int(width,half), width);
		long reachableEndGrid2 = CountCells(false, dists);
		dists = BFS(new Vector2Int(half,-1), width);
		long reachableEndGrid3 = CountCells(false, dists);
		dists = BFS(new Vector2Int(half,width), width);
		long reachableEndGrid4 = CountCells(false, dists);
		// end grids outside of our square
		dists = BFS(new Vector2Int(0,-1), half);
		long reachableOutsideGrid1 = CountCells(false, dists);
		dists = BFS(new Vector2Int(width-1,width), half);
		long reachableOutsideGrid2 = CountCells(false, dists);
		dists = BFS(new Vector2Int(width,0), half);
		long reachableOutsideGrid3 = CountCells(false, dists);
		dists = BFS(new Vector2Int(0,width), half);
		long reachableOutsideGrid4 = CountCells(false, dists);
		// similar ones, but inside
		dists = BFS(new Vector2Int(0,-1), width+half);
		long reachableEndInsideGrid1 = CountCells(true, dists);
		dists = BFS(new Vector2Int(width-1,width), width+half);
		long reachableEndInsideGrid2 = CountCells(true, dists);
		dists = BFS(new Vector2Int(width,0), width+half);
		long reachableEndInsideGrid3 = CountCells(true, dists);
		dists = BFS(new Vector2Int(0,width), width+half);
		long reachableEndInsideGrid4 = CountCells(true, dists);

		long result = 0;
		result += (evenBlocks * reachableWithEvenDist) + (unevenBlocks * reachableWithUnevenDist);
		result += reachableEndGrid1 + reachableEndGrid2 + reachableEndGrid3 + reachableEndGrid4;
		result += nrBlocks * (reachableOutsideGrid1 + reachableOutsideGrid2 + reachableOutsideGrid3 + reachableOutsideGrid4);
		result += (nrBlocks - 1) * (reachableEndInsideGrid1 + reachableEndInsideGrid2 + reachableEndInsideGrid3 + reachableEndInsideGrid4);
		
		// now the special cases
		// the edges: calculate 3 more types
		// - the 4 corners
		// - tiles outside who are only used a little (in the corner)
		// - tiles inside who have a little corner cut off
		// https://imgur.com/tq8bDre
		
		Console.WriteLine($"PART 2: {result}");
	}
	int[,] BFS(Vector2Int start, int depth)
	{
		prevs = new Vector2Int[grid.GetLength(0), grid.GetLength(1)];
		int [,] dists = new int[grid.GetLength(0), grid.GetLength(1)];
		visited = new bool[grid.GetLength(0), grid.GetLength(1)];
		
		for (int i = 0; i < grid.GetLength(0); i++) {
			for (int j = 0; j < grid.GetLength(0); j++) {
				dists[i,j] = Int32.MaxValue;
			}
		}

		Queue<Vector2Int> queue = new Queue<Vector2Int>();
		if (start.x >= 0 && start.x < dists.GetLength(0)
			&& start.y >= 0 && start.y < dists.GetLength(1)) 
			dists[start.x, start.y] = 0;
		queue.Enqueue(start);

		while (queue.Count > 0)
		{
			Vector2Int pos = queue.Dequeue();
			int currDist = 0;
			if (pos.x >= 0 && pos.x < dists.GetLength(0) && pos.y >= 0 && pos.y < dists.GetLength(1)) 
				currDist = dists[pos.x, pos.y];
			// evaluate all 4 directions
			foreach (Vector2Int direction in _directions)
			{
				Vector2Int newPos = pos + direction;
				
				if (newPos.x < 0 || newPos.x >= grid.GetLength(0)) continue; 
				if (newPos.y < 0 || newPos.y >= grid.GetLength(1)) continue;
				if (visited[newPos.x, newPos.y]) continue;
				if (grid[newPos.x, newPos.y] == '#') continue;

				visited[newPos.x, newPos.y] = true;
				dists[newPos.x, newPos.y] = currDist + 1;
				prevs[newPos.x, newPos.y] = pos;

				if (depth != 0 && dists[newPos.x, newPos.y] == depth) continue;
				
				queue.Enqueue(newPos);
			}
		}

		return dists;
	}

	long CountCells(bool even, int[,] dists)
	{
		long result = 0;
		for (int i = 0; i < dists.GetLength(0); i++) {
			for (int j = 0; j < dists.GetLength(1); j++)
			{
				if (dists[i,j] == Int32.MaxValue) continue;
				if (even && dists[i,j] % 2 == 0) result++;
				else if (!even && dists[i, j] % 2 != 0) result++;
			}
		}

		return result;
	}
	
}