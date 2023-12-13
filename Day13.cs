using System.Runtime.InteropServices;

namespace AoC2023;

public class Day13
{
    public void Run(List<string> input)
    {
        var blocks = SplitInput(input);
        long part1 = 0;
        foreach (List<string> block in blocks)
        {
            Console.WriteLine(block.Count + " " + GetHorizontalLineCount(block) + " " + GetHorizontalLineCount(RotateBy90(block)));
            part1 += 100 * GetHorizontalLineCount(block);
            part1 += GetHorizontalLineCount(RotateBy90(block));
            
        }
        Console.WriteLine($"PART 1: {part1}");
    }

    int GetHorizontalLineCount(List<string> block)
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

                if (mirrored == i || mirrored+i == block.Count) duplicates = i; // from beginning or to end of block => (y)
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
}