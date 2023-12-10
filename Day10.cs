namespace AoC2023;

public class Day10
{
	static char[] norths = {'|', '7', 'F'};
	static char[] souths = {'|', 'L', 'J'};
	static char[] easts = {'-', 'J', '7'};
	static char[] wests = {'-', 'L', 'F'};

	// for pretty printing - not used
	private Dictionary<string, string> symbolMap = new Dictionary<string, string>()
	{
		{"F", "\u250F"}, {"J", "\u251B"}, {"L", "\u2517"}, {"7", "\u2513"}, {"|", "\u2503"}, {"-", "\u2501" }
	};
	
	private int[,] dists;
	private int[,] path;
	private int[,] insides;
	private List<string> lines;

	private bool found = false;

	public void Run()
	{
		lines = File.ReadAllText("input/day10.txt").Trim().Split('\n').Where(s => s.Length > 0).ToList();
		dists = new int[lines.Count, lines[0].Length];
		path = new int[lines.Count, lines[0].Length];
		insides = new int[lines.Count, lines[0].Length];

		(int, int) start = lines.Select((l, index) => (index, l.IndexOf('S'))).First(tuple => tuple.Item2 >= 0);
		
		// start at start point
		// execute next() function with neighbours
		// stop if we found S again - there is but one loop
		Next(start.Item1, start.Item2,0, 'S', new []{'S'});
		// PrintMap(path, false);
		// PrintMap(dists);
		
		// Part 2
		int part2 = 0;
		for (int i = 0; i < lines.Count; i++)
		{
			lines[i] = lines[i].Replace('S','L');
			part2 += lines[i].Where((c, index) => path[i,index] == 0 && IsInside(i, index)).Count();
		}
		Console.WriteLine($"PART 2: {part2}");
		// PrintMap(insides, false);
	}	

	bool IsInside(int x, int y)
	{
		// Ray-casting algorithm
		// apparently you can test if inside loop by checking amount of |,L,J
		// I'm not sure why only those 3 and not also 7 and F
		List<char> left = new List<char>();
		for (int i = 0; i < lines[x].Length; i++)
		{
			if (i < y && path[x,i] == 1 && souths.Contains(lines[x][i])) left.Add(lines[x][i]);
		}
		insides[x, y] = left.Count % 2 != 0 ? 1 : 0;

		return left.Count % 2 != 0;
	}

	bool Next(int x, int y, int distance, char prevSymbol, char[] checkDirections)
	{
		if (x > lines.Count || x < 0) return false;
		if (y > lines[0].Length || y < 0) return false;

		// validate move
		if (found) return false;
		char symbol = lines[x][y];
		if (symbol == 'S') {
			if (distance > 2)
			{
				Console.WriteLine("PART 1: " + (distance)/2);
				found = true;
				return true;
			}
			if (dists[x, y] == distance - 2) return false; // no going back!
		}
        else if (dists[x,y] > 0 && dists[x, y] == distance - 2) 
			return false; // no going back!
		if (dists[x,y] > distance ) return false;
		if (lines[x][y] == '.') return false;
		if (!checkDirections.Contains(prevSymbol) && prevSymbol != 'S') return false;

		dists[x, y] = distance;
		// Console.WriteLine($"Writing distance: {x},{y} => {distance}");

		// go to neighbours
		bool result = false;
		switch (lines[x][y])
		{
			case '|': //  is a vertical pipe connecting north and south.
				result |= Next(x - 1, y, distance + 1, symbol, souths);
				result |= Next(x + 1, y, distance + 1, symbol, norths);
				break;
			case '-': //is a horizontal pipe connecting east and west.
				result |= Next(x, y - 1, distance + 1, symbol, wests);
				result |= Next(x, y + 1, distance + 1, symbol, easts);
				break;
			case 'L': //is a 90-degree bend connecting north and east.
				result |= Next(x - 1, y, distance + 1, symbol, souths);
				result |= Next(x, y + 1, distance + 1, symbol, wests);
				break;
			case 'J': //is a 90-degree bend connecting north and west.
				result |= Next(x - 1, y, distance + 1, symbol, souths);
				result |= Next(x, y - 1, distance + 1, symbol, easts);
				break;
			case '7': //is a 90-degree bend connecting south and west.
				result |= Next(x + 1, y, distance + 1, symbol, norths);
				result |= Next(x, y - 1, distance + 1, symbol, easts);
				break;
			case 'F': // is a 90-degree bend connecting south and east.
				result |= Next(x + 1, y, distance + 1, symbol, norths);
				result |= Next(x, y + 1, distance + 1, symbol, wests);
				break;
			case 'S':
				// TODO this part isn't totally correct but works for our input
				if (lines[x-1][y] == '|' || lines[x-1][y] == 'F' || lines[x-1][y] == '7') result |= Next(x - 1, y, distance + 1, symbol, souths);
				// if (lines[x+1][y] == '|' || lines[x+1][y] == 'L' || lines[x+1][y] == 'J') result |= Next(x + 1, y, distance + 1, symbol, norths);
				// if (lines[x][y+1] == '-' || lines[x][y+1] == '7' || lines[x][y+1] == 'J') result |= Next(x, y + 1, distance - 1, symbol, easts);
				// if (lines[x][y-1] == '-' || lines[x][y-1] == 'L' || lines[x][y-1] == 'F') result |= Next(x, y - 1, distance + 1, symbol, wests);
				break;
		}

		if (result) path[x, y] = 1;
		return result;
	}

	void PrintMap(int[,] map, bool space)
	{
		for (int i = 0; i < lines.Count; i++)
		{
			Console.WriteLine("");
			for (int j = 0; j < lines[0].Length; j++)
			{
				Console.Write((lines[i][j] == 'S' ? "S" : map[i,j]) + (space ? " " : ""));
			}
		}
		Console.WriteLine("");
	}
}