using System.Text.RegularExpressions;

namespace AoC2023;

public class Day6
{
	/*
	 * Is actually a simple quadratic formula, can be solved much faster this way
	 * Alternatively, the bruteforce way can be optimized more (knowing that it's quadratic). The function is symmetric -
	 * the beginning/end part of the for loop can be cut if we find the button time associated with the current record
	 */
	public void Run(List<string> lines)
	{
		List<int> times = Regex.Matches(lines[0], @"(?<nr> [0-9]+)").Select(m => Int32.Parse(m.Value)).ToList();
		List<int> distances = Regex.Matches(lines[1], @"(?<nr> [0-9]+)").Select(m => Int32.Parse(m.Value)).ToList();

		long answer1 = 1;
		times.Select((t, index) => GetWins(t, distances[index])).ToList().ForEach(w => answer1 *= w);
		Console.WriteLine($"PART 1: {answer1}");
        
		List<long> times2 = Regex.Matches(lines[0].Replace(" ", ""), @"(?<nr>[0-9]+)").Select(m => Int64.Parse(m.Value)).ToList();
		List<long> distances2 = Regex.Matches(lines[1].Replace(" ", ""), @"(?<nr>[0-9]+)").Select(m => Int64.Parse(m.Value)).ToList();
		long answer2 = 1;
		times2.Select((t, index) => GetWins(t, distances2[index])).ToList().ForEach(w => answer2 *= w);
		Console.WriteLine($"PART 2: {answer2}");
	}

	long GetWins(long time, long maxDist)
	{
		long total = 0;
		for (long i = 1; i < time; i++) {
			if (GetDistance(time, i) > maxDist) total++;
			else if (total > 0) break;
		}
		return total;
	}
	long GetDistance(long total, long button) => ((total - button) * button);
}