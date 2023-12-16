namespace AoC2023;

public class Day14
{
	enum Direction {NORTH, WEST, SOUTH, EAST}

	private Dictionary<int, int> hashMap = new();

	public void Run(List<string> input)
	{
		char[,] map = Utils.ToCharArray(input);
	
		// PART 1
		map = Move(map, Direction.NORTH);
		Console.WriteLine($"PART 1: {GetLoad(map)}");
			
		// PART 2
		map = Utils.ToCharArray(input);
		for (int i = 0; i < 1000000000; i++)
		{
			map = Cycle(map);
			int hash = GetHash(map);
			if (hashMap.ContainsKey(hash))
			{
				// Console.WriteLine($"CYCLE FOUND {i} => {hashMap[hash]}"); // 0.1.2 - 9, length 7
				int cycle = i - hashMap[hash];
				int mapIndex = ((1000000000 - hashMap[hash]) % cycle) + hashMap[hash];
				
				// cycle/pattern found, now calculate the actual output after those moves
				// could have stored the cycle outputs, then this step wasn't necessary but decided to do a hash
				// because I assumed the cycle would be much larger than what it is. Not changing anymore :p
				map = Utils.ToCharArray(input);
				for (int j = 0; j < mapIndex; j++){
					map = Cycle(map);
				}
				Console.WriteLine($"PART 2: {GetLoad(map)}");
				return;
			}
			else hashMap[hash] = i; // store hash/index
		}
	}

	char[,] Cycle(char[,] map)
	{
		map = Move(map, Direction.NORTH);
		map = Move(map, Direction.WEST);
		map = Move(map, Direction.SOUTH);
		map = Move(map, Direction.EAST);
		return map;
	}

	int GetHash(char[,] map)
	{
		int hash = 0;
		for (int i = 0; i < map.GetLength(0); i++)
		{
			for (int j = 0; j < map.GetLength(1); j++)
			{
				if (map[i, j] == 'O') hash += (101 * i) + j;
			}
		}
		return hash;
	}

	char[,] Move(char[,] map, Direction direction)
	{
		// int rowEnd = rowStart == 0 
		bool done = false;
		while (!done)
		{
			done = !SingleMove(map, direction);
		}
		return map;
	}

	bool SingleMove(char[,] map, Direction direction)
	{
		int moves = 0;
		if (direction == Direction.NORTH)
		{
			// starting at the top, move all rolling rocks upwards
			for (int i = 1; i < map.GetLength(0); i++) {
				for (int j = 0; j < map.GetLength(1); j++) {
					if (map[i, j] == 'O' && map[i - 1, j] == '.') {
						map[i, j] = '.';
						map[i - 1, j] = 'O';
						moves++;
					}
				}
			}
		}
		else if (direction == Direction.SOUTH)
		{
			// starting at the bottom, move all rolling rocks downwards
			for (int i =map.GetLength(0) -2; i>=0; i--) {
				for (int j = 0; j < map.GetLength(1); j++) {
					if (map[i, j] == 'O' && map[i + 1, j] == '.') {
						map[i, j] = '.';
						map[i + 1, j] = 'O';
						moves++;
					}
				}
			}	
		}
		else if (direction == Direction.EAST)
		{
			for (int i = map.GetLength(1) - 2; i >= 0; i--) {
				for (int j = 0; j < map.GetLength(0); j++) {
					if (map[j, i] == 'O' && map[j, i + 1] == '.') {
						map[j, i] = '.';
						map[j, i + 1] = 'O';
						moves++;
					}
				}
			}			
		}
		else if (direction == Direction.WEST)
		{
			for (int i = 1; i < map.GetLength(1); i++) {
				for (int j = 0; j < map.GetLength(0); j++) {
					if (map[j, i] == 'O' && map[j, i - 1] == '.') {
						map[j, i] = '.';
						map[j, i - 1] = 'O';
						moves++;
					}
				}
			}			
		}
		return moves > 0;
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
}