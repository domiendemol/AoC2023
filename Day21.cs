using System.ComponentModel.Design;

namespace AoC2023;

public class Day21
{
	private char[,] grid;
	private int[,] depths;
	
	public void Run(List<string> input)
	{
		
		// BFS/DFS to test all possbilities
		grid = new char[input.Count, input[0].Length];
		depths = new int[input.Count, input[0].Length];
		Vector2Int start = new Vector2Int(0,0);
		for (int i = 0; i < input.Count; i++) {
			for (int j = 0; j < input[0].Length; j++) {
				grid[i, j] = input[i][j];
				depths[i,j] = Int32.MaxValue;
				if (grid[i, j] == 'S') start = new Vector2Int(i,j);
			}
		}

		DFS(start, 0);
		
		// count 0's
		
		// print
		int part1 = 0;
		for (int i = 0; i < input.Count; i++) {
			for (int j = 0; j < input[0].Length; j++) {
				if (grid[i, j] == 'O') part1++;
			}
		}
		Console.WriteLine($"PART 1: {part1}");
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