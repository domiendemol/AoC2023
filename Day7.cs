using System.Text.RegularExpressions;

namespace AoC2023;

public class Day7
{
    static char[] _labels = new []{'A','K','Q','J','T','9','8','7','6','5','4','3','2'};

    public enum Type { FIVE, FOUR, FULLHOUSE, THREE, DOUBLE, PAIR, HIGHCARD}

    public void Run(List<string> lines)
    {
        Console.WriteLine($"PART 1: {GetTotal(lines, false)}");
        _labels = new []{'A','K','Q','T','9','8','7','6','5','4','3','2','J'};
        Console.WriteLine($"PART 2: {GetTotal(lines, true)}");
    }

    int GetTotal(List<string> lines, bool joker)
    {
        List<Hand> hands = lines.Select(line => new Hand(line, joker)).ToList();
        hands.Sort();
        // hands.ForEach(hand => Console.WriteLine($"{hand.cards} - {hand.type} - {hand.bid}"));
        return hands.Select((hand, index) => hand.bid * (index + 1)).Sum();
    }

    public class Hand : IComparable<Hand>
    {
        public string cards;
        public int bid; 
        public Type type;
        int[] counts = new int[13];

        public Hand(string line, bool joker)
        {
            string[] lines = line.Split(" ");
            cards = lines[0];
            bid = Int32.Parse(lines[1]);
            foreach (char c in cards) counts[Array.IndexOf(_labels, c)]++;

            if (joker)
            {
                // find highest non-joker and replace with joker
                int max = 0;
                int maxIndex = 0;
                for (int i = 0; i < counts.Length - 1; i++)
                {
                    if (counts[i] > max)
                    {
                        max = counts[i];
                        maxIndex = i;
                    }
                }
                counts[maxIndex] += counts[12]; // convert jokers to that one
                counts[12] = 0;
            }
            DetermineType();
        }

        void DetermineType()
        {
            if (counts.Max() == 5)
            {
                type = Type.FIVE;
            }
            else if (counts.Max() == 4)
            {
                type = Type.FOUR;
            }
            else if (counts.Max() == 3)
            {
                type = (counts.Contains(2)) ? Type.FULLHOUSE : Type.THREE;
            }
            else if (counts.Max() == 2)
            {
                type = (counts.Count(i => i == 2) == 2) ? Type.DOUBLE : Type.PAIR;
            }
            else
            {
                type = Type.HIGHCARD;
            }
        }
        
        public int CompareTo(Hand other)
        {
            int result = -type.CompareTo(other.type);
            if (result == 0)
            {
                for (int i=0; i<cards.Length; i++)
                {
                    if (cards[i] != other.cards[i]) return -Array.IndexOf(_labels, cards[i]).CompareTo(Array.IndexOf(_labels, other.cards[i]));
                }
            }
            return result;
        }
    }
}