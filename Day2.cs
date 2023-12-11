using System.Text.RegularExpressions;

namespace AoC2023;

public class Day2
{
    public void Run(List<string> lines)
    {
        int part1Sum = 0;
        int part2Sum = 0;
        foreach (string line in lines)
        {
            string id = Regex.Match(line, @"Game (?<id>[0-9]+):").Groups["id"].Value;

            var matches = Regex.Matches(line, @" (?<nr>[0-9]+) (?<color>(red|blue|green))");
            int maxRed = matches.Max(m => m.Groups["color"].Value.Equals("red") ? Int32.Parse(m.Groups["nr"].Value) : 0);
            int maxGreen = matches.Max(m => m.Groups["color"].Value.Equals("green") ? Int32.Parse(m.Groups["nr"].Value) : 0);
            int maxBlue = matches.Max(m => m.Groups["color"].Value.Equals("blue") ? Int32.Parse(m.Groups["nr"].Value) : 0);

            // 12 red cubes, 13 green cubes, and 14 blue cubes
            if (maxRed <= 12 && maxGreen <= 13 && maxBlue <= 14) part1Sum += Int32.Parse(id);
            part2Sum += maxRed * maxGreen * maxBlue;
        }
        
        Console.WriteLine("PART 1: " + part1Sum);
        Console.WriteLine("PART 2: " + part2Sum);
    }
}