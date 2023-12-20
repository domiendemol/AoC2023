namespace AoC2023;

public class Day20
{
    enum Type
    {
        Broadcaster,
        FlipFlop,
        Conjunction,
        Button
    }
    public void Run(List<string> input)
    {
        // parse modules
        List<Module> modules = input.Select(line => new Module(line)).ToList();
        // TODO probably store in a dict for fast access
        Console.WriteLine("'");
    }

    class Module
    {
        public Type type;
        public string name;
        public List<string> targets;
        public Module(string input)
        {
            string[] parts = input.Split("->");
            switch (parts[0][0])
            {
                case 'b':
                    type = Type.Broadcaster; break;
                case '%':
                    type = Type.FlipFlop; break;
                case '&':
                    type = Type.Conjunction; break;
            }

            name = parts[0].Trim().Replace("&", "").Replace("%", "");
            targets = parts[1].Trim().Split(", ").ToList();
        }
    }
}