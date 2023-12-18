using System.Text.RegularExpressions;

namespace AoC2023;

public class Day18
{
	private static Dictionary<char, Vector2Long> _dirs = new Dictionary<char, Vector2Long>()
	{
		{ 'R', new Vector2Long(0,1) },
		{ 'D', new Vector2Long(1, 0) },
		{ 'L', new Vector2Long(0,-1) },
		{ 'U', new Vector2Long(-1,0) },
	};
	private static char[] _dirChars = new []{'R','D','L','U'};

	public void Run(List<string> input)
	{
		List<Instruction> instructions = input.Select(line => Regex.Match(line, @"(?<dir>[ULDR]) (?<nr>[0-9]+) \(#(?<color>[a-z0-9]+)\)"))
			.Select(m => new Instruction(m.Groups["dir"].Value, m.Groups["nr"].Value, m.Groups["color"].Value)).ToList();
		
		long part1 = GetArea(instructions);
		Console.WriteLine($"PART 1: {part1}");

		instructions.ForEach(instruction => instruction.Swap());
		long part2 = GetArea(instructions);
		Console.WriteLine($"PART 2: {part2}");
	}

	// other (more brute) approach would be to do a flood fill but would be pretty slow for part 2
	private long GetArea(List<Instruction> instructions)
	{
		// build walls, store pos per wall, don't use grid (ignore color)
		Vector2Long curr = new Vector2Long(0, 0);
		List<Vector2Long> corners = new List<Vector2Long>();
		corners.AddRange(instructions.Select(instr => curr += (instr.steps * _dirs[instr.dir])));

		int trench = instructions.Sum(i => i.steps);

		// shoelace to get area (using middle points of the corners, so not correct yet as our trench is a meter wide)
		long A = Math.Abs(corners.Select((v, i) => (v, i)).Sum(v => v.Item1.x * (corners[(v.Item2 + 1) % corners.Count].y - corners[(v.Item2 == 0 ? corners.Count : v.Item2) - 1].y)) / 2);

		// next up Pick's theorem
		// A = i + b/2 -1
		// so i = A - b/2 + 1 gives us the actual internal integer points of the polygon
		// end result is the internal integer count (i) + points on polygon line (our trench)
		long i = A - trench / 2 + 1;
		return i + trench;
	}

	class Instruction
	{
		public char dir;
		public int steps;
		public string color;

		public Instruction(string dir, string steps, string color)
		{
			this.dir = dir[0];
			this.steps = Int32.Parse(steps);
			this.color = color;
		}

		public void Swap()
		{
			steps = Convert.ToInt32(color.Substring(0, 5), 16);
			dir = _dirChars[Int16.Parse(color.Substring(5, 1))];
		}
	}
}