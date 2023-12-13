using System.Text;

namespace AoC2023;

public static class Utils
{
    public static string ReplaceAtIndex(this string text, int index, char c)
    {
        var stringBuilder = new StringBuilder(text);
        stringBuilder[index] = c;
        return stringBuilder.ToString();
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
}