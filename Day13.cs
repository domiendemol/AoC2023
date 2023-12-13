using System.Runtime.InteropServices;
using System.Text;

namespace AoC2023;

public class Day13
{
    List<(int, int)> _part1Results = new List<(int, int)>();

    public void Run(List<string> input)
    {
        var blocks = SplitInput(input);
      
        long part1 = 0;
        foreach (List<string> block in blocks)
        {
            int hor = GetHorizontalLineCount(block, 0);
            int ver = GetHorizontalLineCount(RotateBy90(block), 0);
            _part1Results.Add((hor, ver));
            part1 += (100 * hor) + ver;
        }
        Console.WriteLine($"PART 1: {part1}");
        Console.WriteLine($"PART 2: {GetSmudgedTotal(blocks)}");
    }

    private long GetSmudgedTotal(List<List<string>> blocks)
    {
        long total = 0;
        for (int i = 0; i < blocks.Count; i++)
        {
            List<string> block = blocks[i];
            bool smudgeFound = false;
            for (int j = 0; j < block.Count && !smudgeFound; j++)
            {
                for (int k = 0; k < block[j].Length && !smudgeFound; k++)
                {
                    // switch pos, calculate lines
                    block[j] = ReplaceAtIndex(block[j], k, block[j][k] == '.' ? '#' : '.');
                    int hor = GetHorizontalLineCount(block, _part1Results[i].Item1);
                    int ver = GetHorizontalLineCount(RotateBy90(block), _part1Results[i].Item2);
                    if (hor != 0 && hor != _part1Results[i].Item1)
                    {
                        total += (100 * hor);
                        smudgeFound = true;
                    }

                    if (ver != 0 && ver != _part1Results[i].Item2 && !smudgeFound)
                    {
                        total += ver;
                        smudgeFound = true;
                    }

                    // switch back
                    block[j] = ReplaceAtIndex(block[j], k, block[j][k] == '.' ? '#' : '.');
                }
            }

            if (!smudgeFound) total += (100 * _part1Results[i].Item1) + _part1Results[i].Item2;
        }

        return total;
    }

    int GetHorizontalLineCount(List<string> block, int previousVal)
    {
        Stack<string> stack = new Stack<string>();
        int duplicates = 0;
        
        int i = 0;
        while (duplicates == 0 && i<block.Count)
        {
            if (stack.Count > 0 && stack.Peek().Equals(block[i]))
            {
                // duplicate found, loop back to count how many we have
                Stack<string> tStack = CloneStack(stack);
                int mirrored = 1;
                tStack.Pop();
                for (int j = 1; j < i && i+j < block.Count && tStack.Count > 0; j++) {
                    if (tStack.Pop().Equals(block[i + j])) mirrored++;
                }

                if ((mirrored == i || mirrored+i == block.Count) && (previousVal != i)) {
                    duplicates = i; // from beginning or to end of block => (y)
                }
            }
            stack.Push(block[i++]);
        }

        return duplicates;
    }
    
    List<string> RotateBy90(List<string> block) 
    {
        char[,] tempShape = new char[block[0].Length,block.Count];

        // rotate values
        for(int j = 0; j < block.Count; j++) {
            for(int i = 0; i < block[j].Length; i++) {
                tempShape[i,j] = block[j][i];
            }
        }

        // convert char[,] back to list of strings
        List<string> result = new List<string>();
        for(int i = 0; i < block[0].Length; i++) {
            string s = "";
            for(int j = 0; j < block.Count; j++) {
                s += tempShape[i, j];
            }
            result.Add(s);
        }

        return result;
    }
    
    List<List<string>> SplitInput(List<string> input)
    {
        List<List<string>> result = new List<List<string>>();
        List<string> list = new List<string>();
        int prevLength = 0;
        foreach (string s in input)
        {
            if (s.Length > 0)
            {
                if (s.Length != prevLength) {
                    list = new List<string>();
                    result.Add(list);
                }
                list.Add(s);   
            }
            prevLength = s.Length;
        }

        return result;
    }
    
    Stack<T> CloneStack<T>(Stack<T> original)
    {
        var arr = new T[original.Count];
        original.CopyTo(arr, 0);
        Array.Reverse(arr);
        return new Stack<T>(arr);
    }
    
    // TODO move to a utils class
    string ReplaceAtIndex(string text, int index, char c)
    {
        var stringBuilder = new StringBuilder(text);
        stringBuilder[index] = c;
        return stringBuilder.ToString();
    }
}