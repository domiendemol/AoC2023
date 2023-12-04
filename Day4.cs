using System.Text.RegularExpressions;

namespace AoC2023;

public class Day4
{
	private long[] _cards;
	
	public void Run()
	{
		List<string> lines = File.ReadAllText("day4.txt").Trim().Split('\n').Where(s => s.Length > 0).ToList();

		int answer1 = lines.Sum(l => GetCardTotal(l, true));
		Console.WriteLine($"PART 1: {answer1}");

		_cards = new long[lines.Count];
		for (int i=0; i<lines.Count; i++)
		{
			_cards[i] += 1;
			CalculateNextCards(lines, i);
		}

		Console.WriteLine($"PART 2: {_cards.Sum()}");
	}

	void CalculateNextCards(List<string> lines, int index)
	{
		long cards = _cards[index];
		int wins = GetCardTotal(lines[index], false);
		for (int i = 1; i <= wins; i++)
		{
			if (index + i < _cards.Length) _cards[index + i] += cards;
		}			
	}
	
	int GetCardTotal(string line, bool power)
	{
		int count = 0;
		string[] parts = line.Split(":")[1].Split("|");
		HashSet<int> winningNumbers = new HashSet<int>();
		
		var matches = Regex.Matches(parts[0], @"(?<nr>[0-9]+ )");
		foreach (Match match in matches) {
			winningNumbers.Add(Int32.Parse(match.Value));
		}
		
		var matches2 = Regex.Matches(parts[1], @"(?<nr>[0-9]+)");
		foreach (Match match in matches2) {
			if (winningNumbers.Contains(Int32.Parse(match.Value))) count++;
		}

		return count == 0 ? 0 : (power ? (int) Math.Pow(2, count-1) : count);
	}
}