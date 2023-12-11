
namespace AoC2023;

public class Day11
{
	private List<string> _lines;
	private List<Galaxy> _galaxies = new List<Galaxy>();

	public void Run()
	{
		_lines = File.ReadAllText("input/day11.txt").Trim().Split('\n').Where(s => s.Length > 0).ToList();

		Part1();
		Part2();
	}

	void Part1()
	{
		_galaxies = new List<Galaxy>();
		BuildGalaxies();
		ExtendUniverse(1);
		Console.WriteLine($"PART 1: {CalculateDistances()}");
	}
	
	void Part2()
	{
		_galaxies = new List<Galaxy>();
		BuildGalaxies();
		ExtendUniverse(1000000-1);
		Console.WriteLine($"PART 2: {CalculateDistances()}");
	}

	long CalculateDistances()
	{
		long sum = 0;
		for (int i = 0; i < _galaxies.Count; i++)
		{
			for (int j = i+1; j < _galaxies.Count; j++)
			{
				sum += Math.Abs(_galaxies[i].Pos.x - _galaxies[j].Pos.x) + Math.Abs(_galaxies[i].Pos.y - _galaxies[j].Pos.y);
			}
		}
		return sum;
	}
	
	void ExtendUniverse(int amount)
	{
		// find empty rows
		for (int i = 0; i < _lines.Count; i++)
		{
			if (!_lines[i].Contains('#'))
			{
				_galaxies.Where(g => g.StartPos.x > i).ToList().ForEach(g => g.ShiftDown(amount));
			}
		}
		// find empty columns
		for (int i = 0; i < _lines[0].Length; i++)
		{
			if (_lines.Count(l => l[i] == '.') == _lines.Count)
			{
				_galaxies.Where(g => g.StartPos.y > i).ToList().ForEach(g => g.ShiftRight(amount));
			}
		}
	}
	
	void BuildGalaxies()
	{
		for (int i = 0; i < _lines.Count; i++){
			for (int j = 0; j < _lines[i].Length; j++){
				if (_lines[i][j] == '#') _galaxies.Add(new Galaxy(new Vector2Int(i,j)));
			}
		}
	}

	class Galaxy
	{
		private Vector2Int _pos; // row,col
		private Vector2Int _startPos; // row,col
		
		public Vector2Int Pos { get => _pos; set => _pos = value; }
		public Vector2Int StartPos => _startPos;

		public Galaxy(Vector2Int pos)
		{
			this._pos = pos;
			this._startPos = pos;
		}

		public void ShiftDown(int amount) => _pos += new Vector2Int(amount, 0);
		public void ShiftRight(int amount) => _pos += new Vector2Int(0, amount);
	}
}