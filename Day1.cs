namespace AoC2023;

public class Day1
{
    string[] words = {"one","two","three","four","five","six","seven","eight","nine"};

    public void Run(List<string> lines)
    {
        // part 1
        int sum = 0;
        foreach (string line in lines) {
            IEnumerable<char> chars = line.ToCharArray().Where(c => c >= '0' & c <= '9');
            sum += ((chars.First() - '0') * 10) + (chars.Last() - '0');
        }
            
        Console.WriteLine("PART 1:" + sum);

        // part 2
        sum = 0;
        foreach (string line in lines) {
            sum += FindFirstDigit(line) * 10 + FindLastDigit(line);
        }

        Console.WriteLine("PART 2:" + sum);
    }

    int FindFirstDigit(string str)
    {
        int dIndex = 666;
        for (int i = 0; i < str.Length && dIndex == 666; i++)
        {
            if (str[i] >= '0' && str[i] <= '9') dIndex = i;
        }

        int wValue = FindFirstWord(str, out int wIndex);
        return dIndex < wIndex ? str[dIndex] - '0' : wValue;
    }
    
    int FindLastDigit(string str)
    {
        int dIndex = -1;
        for (int i = str.Length-1; i >= 0 && dIndex == -1; i--)
        {
            if (str[i] >= '0' && str[i] <= '9') dIndex = i;
        }

        int wValue = FindLastWord(str, out int wIndex);
        return dIndex > wIndex ? str[dIndex] - '0' : wValue;
    }
    
    int FindFirstWord(string str, out int index)
    {
        int digitIndex = 0;
        index = 666;
        foreach (string word in words)
        {
            int wIndex = str.IndexOf(word);
            if (wIndex > -1 && wIndex < index) {
                index = wIndex;
                digitIndex = Array.IndexOf(words, word);
            }
        }
        return index == 666 ? 0 : digitIndex+1;
    }
    
    int FindLastWord(string str, out int index)
    {
        int digitIndex = 0;
        index = -1;
        foreach (string word in words)
        {
            int wIndex = str.LastIndexOf(word);
            if (wIndex > -1 && wIndex > index) {
                index = wIndex;
                digitIndex = Array.IndexOf(words, word);
            }
        }
        return index == -1 ? 0 : digitIndex+1;
    }
}