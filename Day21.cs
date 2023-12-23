using System.ComponentModel.Design;

namespace AoC2023;

public class Day21
{
	private Vector2Int[] _directions = new[] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };

	private char[,] grid;
	
	// DFS
	private int[,] depths;
	
	// BFs
	private bool[,] visited;
	private Vector2Int[,] prevs;
	private int[,] dists;
	
	public void Run(List<string> input)
	{
		
		// BFS/DFS to test all possbilities
		grid = new char[input.Count, input[0].Length];
		depths = new int[input.Count, input[0].Length];
		prevs = new Vector2Int[input.Count, input[0].Length];
		dists = new int[input.Count, input[0].Length];
		visited = new bool[input.Count, input[0].Length];
		Vector2Int start = new Vector2Int(0,0);
		for (int i = 0; i < input.Count; i++) {
			for (int j = 0; j < input[0].Length; j++) {
				grid[i, j] = input[i][j];
				depths[i,j] = Int32.MaxValue;
				if (grid[i, j] == 'S') start = new Vector2Int(i,j);
			}
		}

		BFS(start, 64);
		//DFS(start, 64);
		
		// count 0's
		
		// print
		int part1 = 0;
		for (int i = 0; i < input.Count; i++) {
			for (int j = 0; j < input[0].Length; j++) {
				if (dists[i, j] <= 64 && dists[i,j] % 2 == 0) part1++;
			}
		}
		Console.WriteLine($"PART 1: {part1}");
		
		/*
		for (int i = 0; i < input.Count; i++) {
			for (int j = 0; j < input[0].Length; j++) {
				Console.Write((dists[i, j] <= 6 && dists[i,j] % 2 == 0) ? 'O' : grid[i,j]);
			}
			Console.WriteLine();
		}
		*/
		
		// new strategy: calculated shortest path for all positions within 64 dist
		// loop them: if shortest dist == even, yes it can be reached with 64 steps!
		
	}
	void BFS(Vector2Int start, int depth)
	{
		for (int i = 0; i < grid.GetLength(0); i++) {
			for (int j = 0; j < grid.GetLength(0); j++) {
				dists[i,j] = Int32.MaxValue;
			}
		}

		Queue<Vector2Int> queue = new Queue<Vector2Int>();
		dists[start.x, start.y] = 0;
		queue.Enqueue(start);

		while (queue.Count > 0)
		{
			Vector2Int pos = queue.Dequeue();
			
			// evaluate all 4 directions
			foreach (Vector2Int direction in _directions)
			{
				Vector2Int newPos = pos + direction;
				
				if (newPos.x < 0 || newPos.x >= grid.GetLength(0)) continue; 
				if (newPos.y < 0 || newPos.y >= grid.GetLength(1)) continue;
				if (visited[newPos.x, newPos.y]) continue;
				if (grid[newPos.x, newPos.y] == '#') continue;

				visited[newPos.x, newPos.y] = true;
				dists[newPos.x, newPos.y] = dists[pos.x, pos.y] + 1;
				prevs[newPos.x, newPos.y] = pos;

				if (dists[newPos.x, newPos.y] == depth) continue;
				
				queue.Enqueue(newPos);
			}
		}
	}
	

	void DFS(Vector2Int pos, int depth)
	{
		if (pos.x < 0 || pos.x >= grid.GetLength(0)) return;
		if (pos.y < 0 || pos.y >= grid.GetLength(1)) return;
		if (grid[pos.x, pos.y] == '#') return;
		
		if (depth >= 64)
		{
			grid[pos.x, pos.y] = 'O';
			return;
		}

		if (depths[pos.x, pos.y] < depth) return;
		depths[pos.x, pos.y] = Math.Min(depths[pos.x, pos.y], depth);


		// evaluate all 4 directions
		DFS(pos + new Vector2Int(0,1), depth+1);
		DFS(pos + new Vector2Int(0,-1),depth+1);
		DFS(pos + new Vector2Int(1,0), depth+1);
		DFS(pos + new Vector2Int(-1,0),depth+1);
		return;
	}
}