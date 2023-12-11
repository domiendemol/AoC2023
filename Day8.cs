using System.Text.RegularExpressions;

namespace AoC2023;

public class Day8
{
	private string _instructions;
	Dictionary<string, Node> _map = new Dictionary<string, Node>();

	public void Run(List<string> lines)
	{
		_instructions = lines[0];
		lines.Skip(1).
			Select(l => Regex.Matches(l, @"([A-Z][A-Z][A-Z])")).ToList().
			ForEach(m => _map[m[0].Value] = new Node(m[1].Value, m[2].Value));

		Console.WriteLine($"PART 1: {Part1()}");
		Console.WriteLine($"PART 2: {Part2()}");
	}

	int Part1()
	{
		int counter = 0;
		string currNode = "AAA";
		while (true)
		{
			char instr = _instructions[counter++ % _instructions.Length];
			currNode = (instr == 'L') ? _map[currNode].left : _map[currNode].right;
			if (currNode.Equals("ZZZ")) break;
		}
		return counter;
	}

	long Part2()
	{
		List<string> currNodes = _map.Keys.ToList().Where(node => node.EndsWith("A")).ToList();
		List<long> cycles = new List<long>();
		for (int i = 0; i < currNodes.Count; i++)
		{
			int counter = 0;
			bool allDone = false;
			string node = currNodes[i];
			while (!allDone)
			{
				char instr = _instructions[counter++ % _instructions.Length];
				string next = (instr == 'L') ? _map[node].left : _map[node].right;
				node = next;
				if (next.EndsWith("Z"))
				{
					allDone = true;
					cycles.Add(counter);
					// Console.WriteLine($"{currNodes[i]} - {counter}");
				}
			}
		}
		return cycles.Aggregate((x,y) => LCM(x, y));
	}
	
	long Part2Bruteforce() // never saw the end - took too long
	{
		long counter = 0;
		List<string> currNodes = _map.Keys.ToList().Where(node => node.EndsWith("A")).ToList();
		bool allDone = false;
		while (!allDone)
		{
			char instr = _instructions[(int)(counter++ % _instructions.Length)];
			for (int i=0; i<currNodes.Count; i++)
			{
				string node = currNodes[i];
				string next = (instr == 'L') ? _map[node].left : _map[node].right;
				
				currNodes[i] = next;
			}
			if (currNodes.Count(n => n.EndsWith("Z")) == currNodes.Count) allDone = true;
		}
		return counter;
	}
	
	static long GCF(long a, long b)
	{
		while (b != 0)
		{
			long temp = b;
			b = a % b;
			a = temp;
		}
		return a;
	}

	static long LCM(long a, long b)
	{
		return (a / GCF(a, b)) * b;
	}
	
	
	struct Node
	{
		public string left;
		public string right;
		public int cycle;

		public Node(string left, string right)
		{
			this.left = left;
			this.right = right;
		}
	}
}