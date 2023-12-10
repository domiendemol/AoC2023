namespace AoC2023;

public class Day9
{
	public void Run()
	{
		List<string> lines = File.ReadAllText("input/day9.txt").Trim().Split('\n').Where(s => s.Length > 0).ToList();

		Console.WriteLine($"PART 1: {lines.Sum(line => Extrapolate(line))}");
		Console.WriteLine($"PART 2: {lines.Sum(line => ExtrapolateLeft(line))}");
	}

	int Extrapolate(string line)
	{
		List<List<int>> grid = BuildDiffsGrid(line);

		for (int i = grid.Count-2; i >=0; i--)
		{
			grid[i].Add(grid[i][grid[i].Count - 1] + grid[i + 1][grid[i + 1].Count -1]);
		}
		return grid[0][grid[0].Count-1];
	}

	int ExtrapolateLeft(string line)
	{
		List<List<int>> grid = BuildDiffsGrid(line);

		for (int i = grid.Count-2; i >=0; i--)
		{
			grid[i].Insert(0, grid[i][0] - grid[i + 1][0]);
		}
		return grid[0][0];
	}

	List<List<int>> BuildDiffsGrid(string line)
	{
		List<List<int>> grid = new List<List<int>>();
		string[] nrs = line.Split(' ');
		grid.Add(nrs.Select(nr => Int32.Parse(nr)).ToList());

		int count = 1;
		while (true)
		{
			List<int> diffs = new List<int>();
			grid.Add(diffs);
			for (int i = 0; i < grid[count-1].Count-1; i++)
			{
				diffs.Add(grid[count-1][i+1] - grid[count-1][i]);
			}

			if (diffs.Count(i => i == 0) == diffs.Count) break;
			count++;
		}
		return grid;
	}
}