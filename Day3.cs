using System.Text.RegularExpressions;

namespace AoC2023;

public class Day3
{
    private Dictionary<Vector2Int, List<int>> _gears = new Dictionary<Vector2Int, List<int>>();
    private bool _gearFound;
    private Vector2Int _lastGear;
    
    public void Run(List<string> lines)
    {
        int sum = 0;
        for (int i=0; i<lines.Count; i++)
        {
            string line = lines[i];
            MatchCollection matches = Regex.Matches(line, @"(?<id>[0-9]+)");
            foreach (Match match in matches)
            {
                _gearFound = false;
                if (IsAdjacentToSymbol(lines, i, match.Index, match.Length))
                {
                    if (_gearFound)
                    {
                        if (_gears.ContainsKey(_lastGear)) _gears[_lastGear].Add(Int32.Parse(match.Value));
                        else _gears[_lastGear] = new List<int>{int.Parse(match.Value)};
                    }
                    sum += Int32.Parse(match.Value);
                }
            }
        }
        
        Console.WriteLine($"PART 1: {sum}");

        int p2Sum = 0;
        IEnumerable<KeyValuePair<Vector2Int, List<int>>> doubles = _gears.Where(kvp => kvp.Value.Count == 2);
        foreach (KeyValuePair<Vector2Int,List<int>> keyValuePair in doubles)
        {
            int m = 1;
            foreach (int i in keyValuePair.Value) m *= i;
            p2Sum += m;
        }
        
        Console.WriteLine($"PART 2: {p2Sum}");
    }

    bool IsAdjacentToSymbol(List<string> input, int line, int index, int length)
    {
        // brute force, for every char: left, right, up, down, left-up, left-down, right-up, right-down
        bool adjacent = false;
        for (int i = 0; i < length && !adjacent; i++)
        {
            if (IsSymbol(input, line-1, index+i-1) || IsSymbol(input, line+1, index+i-1)) adjacent = true;
            if (IsSymbol(input, line-1, index+i+1) || IsSymbol(input, line+1, index+i+1)) adjacent = true;
            if (IsSymbol(input, line, index+i-1) || IsSymbol(input, line, index+i+1)) adjacent = true;
            if (IsSymbol(input, line-1, index+i) || IsSymbol(input, line+1, index+i)) adjacent = true;
        }
        return adjacent;
    }

    bool IsSymbol(List<string> input, int line, int index)
    {
        if (line < 0 || line >= input.Count) return false;
        if (index < 0 || index >= input[line].Length) return false;
        if ((input[line][index] < '0' || input[line][index] > '9') && input[line][index] != '.')
        {
            if (input[line][index] == '*')
            {
                _gearFound = true;
                _lastGear = new Vector2Int(line, index);
            }
            return true;
        }
        return false;
    }
}