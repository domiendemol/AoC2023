namespace AoC2023;

public class Day10
{
	static char[] norths = {'|', '7', 'F'};
	static char[] souths = {'|', 'L', 'J'};
	static char[] easts = {'-', 'J', '7'};
	static char[] wests = {'-', 'L', 'F'};

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
		// recursive next() function
		// execute next() function with neighbours
		// stop if we found S again - there is but one loop
		Next(start.Item1, start.Item2,0, 'S', new []{'S'});
		// PrintMap(path);
		// PrintMap(dists);
		
		// Part 2
		int part2 = 0;
		for (int i = 0; i < lines.Count; i++)
		{
			part2 += lines[i].Where((c, index) => path[i,index] == 0 && IsInside(i, index)).Count();
		}
		Console.WriteLine($"PART 2: {part2}");
		// PrintMap(insides);
	}	

	bool IsInside(int x, int y)
	{
		// if (lines[x][y] == 'S') lines[x] = lines[x].Replace('S','7');
		(int, int) hor = GetHorCrossings(x, y);
		int left = hor.Item1;
		int right = hor.Item2;
		
		(int, int) ver = GetVerCrossings(x, y);
		int up = ver.Item1;
		int down = ver.Item2;

		if (left + right < 2) return false;
		if (left == 0 || right == 0) return false;
		if (up + down < 2) return false;
		if (up == 0 || down == 0) return false;

		if (left + right > 2 && (left + right) % 2 == 0 && left == right) return false; 
		if (up + down > 2 && (up + down) % 2 == 0 && up == down) return false;

		insides[x, y] = 1;
		return true;
	}

	(int,int) GetHorCrossings(int x, int y)
	{
		List<char> left = new List<char>();
		List<char> right = new List<char>();
		for (int i = 0; i < lines[x].Length; i++)
		{
			if (i < y && path[x,i] == 1 && (norths.Contains(lines[x][i]) || souths.Contains(lines[x][i]))) left.Add(lines[x][i]);
			if (i > y && path[x,i] == 1 && (norths.Contains(lines[x][i]) || souths.Contains(lines[x][i]))) right.Add(lines[x][i]);
		}
		int l = left.Count(c => c == '|') + GetDoubleCrossings(left);
		int r = right.Count(c => c == '|') + GetDoubleCrossings(right);
		return (l,r);
	}
	
	(int,int) GetVerCrossings(int x, int y)
	{
		List<char> up = new List<char>();
		List<char> down = new List<char>();
		for (int i = 0; i < lines.Count; i++)
		{
			if (i < x && path[i,y] == 1 && (wests.Contains(lines[i][y]) || easts.Contains(lines[i][y]))) up.Add(lines[i][y]);
			if (i > x && path[i,y] == 1 && (wests.Contains(lines[i][y]) || easts.Contains(lines[i][y]))) down.Add(lines[i][y]);
		}
		int u = up.Count(c => c == '-') + GetDoubleCrossings(up);
		int d = down.Count(c => c == '-') + GetDoubleCrossings(down);
		return (u, d);
	}

	int GetDoubleCrossings(List<char> crossings)
	{
		// L 7
		// J F
		int dc = (int) Math.Floor((crossings.Count(c => c == 'L') + crossings.Count(c => c == '7')) / 2f);
		dc += (int) Math.Floor((crossings.Count(c => c == 'J') + crossings.Count(c => c == 'F')) / 2f);
		return dc;
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

	void PrintMap(int[,] map)
	{
		for (int i = 0; i < lines.Count; i++)
		{
			Console.WriteLine("");
			for (int j = 0; j < lines[0].Length; j++)
			{
				Console.Write((lines[i][j] == 'S' ? "S" : map[i,j]) + " ");
			}
		}
		Console.WriteLine("");
	}
}