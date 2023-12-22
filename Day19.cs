using System.Numerics;
using System.Text.RegularExpressions;

namespace AoC2023;

public class Day19
{
	private static List<Workflow> _workflows;
	private static BigInteger _part2;
	
	public void Run(List<string> input)
	{
		// parse workflows
		_workflows = input.Where(line => line.Contains(':')).Select(line => new Workflow(line)).ToList();
		
		Console.WriteLine($"PART 1: {Part1(input)}");
		
		// we will have to build a graph and find ranges that end up in A
		Part2();
		Console.WriteLine($"PART 2: {_part2.ToString()}");
	}

	void Part2()
	{
		Queue<Node> queue = new Queue<Node>();
		Workflow start = _workflows.First(w => w.name.Equals("in"));
		queue.Enqueue(new Node(start, new Dictionary<char, (int min, int max)>()));

		List<Rule> rulesToInvert = new List<Rule>();
		
		while (queue.Count > 0)
		{
			Node node = queue.Dequeue();
			rulesToInvert.Clear();
			foreach (Rule rule in node.workflow.rules)
			{
				Dictionary<char, (int, int)> mins = rule.GetNewMinimums(node.xmas, false);
				foreach (Rule ruleToInvert in rulesToInvert) mins = ruleToInvert.GetNewMinimums(mins, true);
				if (rule.result == "A")
				{
					_part2 = BigInteger.Add(_part2, GetCombinations(mins)); 
					// Console.WriteLine($"{node.workflow.name} - {mins['x']} * {mins['m']} * {mins['a']} * {mins['s']} => {GetCombinations(mins)}");
				}
				else if (rule.result != "R")
				{
					queue.Enqueue(new Node(_workflows.First(w => w.name.Equals(rule.result)), mins));
				}
				rulesToInvert.Add(rule);
			}
		}
	}

	BigInteger GetCombinations(Dictionary<char, (int, int)> mins)
	{
		BigInteger total = new BigInteger(mins['x'].Item2 + 1 - mins['x'].Item1);
		total = BigInteger.Multiply(total, (mins['m'].Item2 + 1 - mins['m'].Item1));
		total = BigInteger.Multiply(total, (mins['a'].Item2 + 1 - mins['a'].Item1));
		return BigInteger.Multiply(total, (mins['s'].Item2 + 1 - mins['s'].Item1));
	}

	long Part1(List<string> input)
	{
		// parse parts
		List<Part> parts = input.Select(line => Regex.Matches(line, @"(?<name>[a-z]+)=(?<value>[0-9]+)"))
			.Where(m => m.Count > 0).Select(m => new Part(m)).ToList();
		Console.WriteLine("");
		
		// run parts through workflow in
		Workflow wf = _workflows.First(w => w.name.Equals("in"));
		return parts.Where(p => wf.Execute(p)).Sum(p => p.xmas.Values.Sum());
	}

	class Workflow
	{
		public string name;
		public List<Rule> rules = new List<Rule>();
		public Workflow(string line)
		{
			Match m = Regex.Match(line, @"(?<name>[a-z]+){(?<conds>[^}]+)}");
			name = m.Groups["name"].Value;
			string[] rulesStrings = m.Groups["conds"].Value.Split(',');
			for (int i = 0; i < rulesStrings.Length; i++)
			{
				// split by :
				string[] split = rulesStrings[i].Split(':');
				rules.Add(new Rule(split[0], split[split.Length-1]));
			}
		}

		public bool Execute(Part part)
		{
			string result;
			foreach (Rule rule in rules)
			{
				result = rule.TryMatch(part);
				if (result.Length > 0)
				{
					// recursively
					if (result.Equals("A")) return true;
					if (result.Equals("R")) return false;
					return _workflows.First(w => w.name.Equals(result)).Execute(part);
				}
			}
			return false; // will never get here as there is always an 'end' rule/jump
		}
	}

	class Part
	{
		public Dictionary<char, int> xmas = new Dictionary<char, int>();
		
		public Part(MatchCollection matchCollection)
		{
			matchCollection.ToList().ForEach(m => xmas[m.Groups["name"].Value[0]] = Int32.Parse(m.Groups["value"].Value)); 
		}
	}

	class Rule
	{
		public char category;
		public int target;
		public int comparator; // 1 or -1
		public string result;

		public Rule(string condition, string result)
		{
			if (condition == result) {
				category = '/';
			}
			else {
				Match m = Regex.Match(condition, @"(?<name>[a-z]+)(?<comp>[\<\>])(?<target>[0-9]+)");
				category = m.Groups["name"].Value[0];
				comparator = m.Groups["comp"].Value == ">" ? 1 : -1;
				target = Int32.Parse(m.Groups["target"].Value);
			}
			this.result = result;
		}

		public bool HasCondition() => category != '/';

		public string TryMatch(Part part)
		{
			if (!HasCondition()) return result;
			if (part.xmas.TryGetValue(category, out int value) && value.CompareTo(target) * comparator >= 1) return result;
			return "";
		}

		public Dictionary<char, (int, int)> GetNewMinimums(Dictionary<char, (int, int)> input, bool inverse)
		{
			if (category == '/') return input;
			Dictionary<char, (int, int)> mins = new Dictionary<char, (int, int)>
			{
				['x'] = input['x'],
				['m'] = input['m'],
				['a'] = input['a'],
				['s'] = input['s']
			};
			if (inverse)
				mins[category] = comparator == -1 ? (Math.Max(mins[category].Item1, target), mins[category].Item2) : (mins[category].Item1, Math.Min(mins[category].Item2, target));
			else
				mins[category] = comparator == 1 ? (Math.Max(mins[category].Item1, target+1), mins[category].Item2) : (mins[category].Item1, Math.Min(mins[category].Item2, target-1));
			return mins;
		}
	}

	class Node
	{
		public Workflow workflow;
		public Dictionary<char, (int min, int max)> xmas = new Dictionary<char, (int, int)>();

		public Node(Workflow workflow, Dictionary<char, (int min, int max)> mins)
		{
			this.workflow = workflow;
			this.xmas = mins;
			if (mins.Count == 0)
			{
				xmas['x'] = (1, 4000);
				xmas['m'] = (1, 4000);
				xmas['a'] = (1, 4000);
				xmas['s'] = (1, 4000);
			}
		}
	}
}