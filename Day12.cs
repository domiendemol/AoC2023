using System.Text;
using System.Text.RegularExpressions;

namespace AoC2023;

public class Day12
{
	private Dictionary<string, long> _cache = new Dictionary<string, long>();
	
	public void Run(List<string> input)
	{
		long part1 = input.Select(line => Unfold(line, 1)).Sum(t => CountCombos(t.Item1, 0, 0, t.Item2));
		Console.WriteLine($"PART 1: {part1}");
		
		// PART 2
		long part2 = input.Select(line => Unfold(line, 5)).Sum(t => CountCombos(t.Item1, 0, 0, t.Item2));
		Console.WriteLine($"PART 2: {part2}");
	}

	(string, List<int>) Unfold(string input, int amount)
	{
		string part1 = "";
		List<int> part2 = new List<int>();
		var parts = input.Split(' ');
		for (int i = 0; i < amount; i++)
		{
			part1 += parts[0] + (i == amount-1 ? "" : "?");
		}
		for (int i = 0; i < amount; i++)
		{
			part2.AddRange(Regex.Matches(parts[1], @"[0-9]+").Select(m => Int32.Parse(m.Value)));
		}
		return (part1, part2);
	}

	long CountCombos(string input, int index, int groupIndex, List<int> counts)
	{
		if (groupIndex == counts.Count) return input.LastIndexOf('#') < index ? 1 : 0;
		if (index >= input.Length) return 0;
		if (groupIndex >= counts.Count && input.Substring(index).Contains('#')) return 0;

		if (_cache.ContainsKey((input.Substring(index) + Implode(groupIndex, counts)))) 
			return _cache[(input.Substring(index) + Implode(groupIndex, counts))];
		
		// optimization: more dots than possible, bail out!
		if (input.Count(c => c == '.') > input.Length - counts.Sum())
		{
			//_cache[(input.Substring(index) + Implode(groupIndex, counts))] = 0;
			return 0;
		}
		
		long result = 0; 
		if (input[index] == '#')
		{
			int hashCount = 1;
			for (int i = 1; i < counts[groupIndex] && index + i < input.Length; i++) {
				if (input[index+i] != '.') hashCount++;
			}

			if (hashCount == counts[groupIndex] && (index == 0 || input[index-1] == '.'))
			{
				if (index + hashCount == input.Length) // end of input
					return CountCombos(input, index+hashCount+1, groupIndex+1, counts);
				if (input[index + hashCount] == '#') 
					return 0;
				input = ReplaceAtIndex(input, index + hashCount, '.');
				// group found, cut it and move further
				return CountCombos(input, index+hashCount+1, groupIndex+1, counts);
			}
			else
			{
				return 0; //CountCombos(input, index+hashCount, groupIndex, counts);
			}
		} 
		else if (input[index] == '?')
		{
			string s1 = ReplaceAtIndex(input, index, '.');
			result += CountCombos(s1, index+1, groupIndex, counts);
			string s2 = ReplaceAtIndex(input, index, '#');
			result += CountCombos(s2, index, groupIndex, counts);
		}
		else
		{
			result += CountCombos(input, index+1, groupIndex, counts);
		}
		
		_cache[(input.Substring(index) + Implode(groupIndex, counts))] = result;
		return result;
	}

	private static string Implode(int groupIndex, List<int> counts)
	{
		return string.Join("-", counts.Skip(groupIndex));
	}

	public static long Factorial(int n)
	{
		if (n < 0) throw new ArgumentException("Input should be a non-negative integer.");

		long result = 1;
		for (int i = 2; i <= n; i++) {
			result *= i;
		}

		return result;
	}
	
	string ReplaceAtIndex(string text, int index, char c)
	{
		var stringBuilder = new StringBuilder(text);
		stringBuilder[index] = c;
		return stringBuilder.ToString();
	}
}