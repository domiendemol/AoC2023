using System.Text.RegularExpressions;

namespace AoC2023;

public class Day5
{
    public void Run(List<string> lines)
    {
        List<long> seeds = Regex.Matches(lines[0], @"(?<nr> [0-9]+)").Select(m => Int64.Parse(m.Value)).ToList();
        List<Map> maps = BuildMaps(lines);
        
        // for every seed, go over all maps and find the end location
        long answer1 = seeds.Min(s => GetLocation(maps, s));
        Console.WriteLine($"PART 1: {answer1}");

        bool found = false;
        long location = 0;
        while (!found)
        {
            long seed = GetSeed(maps, location++);
            // test if seed in input seeds array
            for (int i = 0; i < seeds.Count-1 && !found; i += 2)
            {
                if (seed >= seeds[i] && seed <= seeds[i] + seeds[i + 1]) found = true;
            }
        }
        Console.WriteLine($"PART 2: {location-1}");
    }

    // reverse lookup, from location to seed
    long GetSeed(List<Map> maps, long location) 
    {
        long input = location;
        foreach (Map map in Enumerable.Reverse(maps)) input = GetMapInput(map, input);
        return input;
    }
    
    long GetLocation(List<Map> maps, long seed, long range) 
    {
        return Enumerable.Range(0, (int)range).Min(i => GetLocation(maps, seed + i));
    }
    
    long GetLocation(List<Map> maps, long seed)
    {
        long input = seed;
        foreach (Map map in maps) input = GetMapOutput(map, input);
        return input;
    }

    // Reverse
    long GetMapInput(Map map, long input)
    {
        foreach (Transform transform in map.transforms)
        {
            if (input >= transform.destinationStart && input < transform.destinationStart + transform.length) {
                input = transform.sourceStart + (input - transform.destinationStart);
                break;
            }
        }
        return input;
    }
    
    long GetMapOutput(Map map, long input)
    {
        foreach (Transform transform in map.transforms)
        {
            if (input >= transform.sourceStart && input < transform.sourceStart + transform.length)
            {
                input = transform.destinationStart + (input - transform.sourceStart);
                break;
            }
        }
        return input;
    }

    List<Map> BuildMaps(List<string> lines)
    {
        Map map = new Map();
        List<Map> maps = new List<Map>();
        for (int i = 1; i < lines.Count; i++)
        {
            if (lines[i].Contains("map"))
            {
                map = new Map();
                maps.Add(map);
            }
            else
            {
                var matches = Regex.Matches(lines[i], @"(?<nr>[0-9]+)");
                map.transforms.Add(new Transform(Int64.Parse(matches[0].Value), Int64.Parse(matches[1].Value), Int64.Parse(matches[2].Value)));
            }
        }
        return maps;
    }

    struct Map
    {
        public List<Transform> transforms;
        
        public Map() => transforms = new List<Transform>();
    }

    struct Transform
    {
        public long sourceStart;
        public long destinationStart;
        public long length;

        public Transform(long destinationStart, long sourceStart, long length)
        {
            this.sourceStart = sourceStart;
            this.destinationStart = destinationStart;
            this.length = length;
        }
    }
}