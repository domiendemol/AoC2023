namespace AoC2023;

public class Day14
{
	public void Run(List<string> input)
	{
		char[,] map = ToCharArray(input);

		bool done = false;
		while (!done)
		{
			int moves = 0;
			// starting at the top, move all rolling rocks upwards
			for (int i = 1; i < map.GetLength(0); i++) {
				for (int j = 0; j < map.GetLength(1); j++) {
					if (map[i, j] == 'O' && map[i - 1, j] == '.')
					{
						// move
						map[i, j] = '.';
						map[i - 1, j] = 'O';
						moves++;
					}
				}
			}
			if (moves == 0) done = true;
		}

		PrintMap(map);
		Console.WriteLine($"PART 1: {GetLoad(map)}");
	}

	long GetLoad(char[,] map)
	{
		long total = 0;
		for (int i = 0; i < map.GetLength(0); i++) {
			for (int j = 0; j < map.GetLength(1); j++)
			{
				if (map[i, j] == 'O') total += map.GetLength(1) - i;
			}
		}
		return total;
	}
	
	void PrintMap(char[,] map)
	{
		for (int i = 0; i < map.GetLength(0); i++) {
			for (int j = 0; j < map.GetLength(1); j++) {
				Console.Write(map[i, j]);
			}
			Console.WriteLine("");
		}
	}
	
	
	// is there a better way?
	char[,] ToCharArray(List<string> input)
	{
		char[,] tempShape = new char[input[0].Length,input.Count];
		for(int j = 0; j < input.Count; j++) {
			for(int i = 0; i < input[j].Length; i++) {
				tempShape[i,j] = input[i][j];
			}
		}
		return tempShape;
	}
}