using System.Text.RegularExpressions;

namespace AoC2023;

public class Day6
{
    public void Run()
    {
        List<string> lines = File.ReadAllText("input/day6.txt").Trim().Split('\n').Where(s => s.Length > 0).ToList();
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
        }
        Console.WriteLine(total);
        return total;
    }
    long GetDistance(long total, long button) => ((total - button) * button);
}