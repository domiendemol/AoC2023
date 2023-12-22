using System.Diagnostics;

namespace AoC2023;

public class Day20
{
    public enum Type
    {
        Broadcaster,
        FlipFlop,
        Conjunction,
        Untyped
    }
    
    Dictionary<string, Module> _modules = new Dictionary<string, Module>();
    private static Queue<Signal> queue = new Queue<Signal>();
    private static long _highPulses;
    private static long _lowPulses;
    private static long _buttonCount;

    public void Run(List<string> input)
    {
        // parse modules
        input.Select(line => new Module(line)).ToList().ForEach(m => _modules[m.name]=m);
        _modules["output"] = new Module("output -> output"); // for test input
        _modules["rx"] = new Module("rx -> rx");

        foreach (Module mod in _modules.Values) {
            foreach (string target in mod.targets) {
                _modules[target].inputs[mod] = false;
            }
        }
        
        // broadcaster
        for (int i = 0; i < 10000; i++)
        {
            _buttonCount = i+1;
            PushTheButton(_modules["broadcaster"], _modules);
        }
        Console.WriteLine($"PART 1: {_highPulses * _lowPulses}");
        
        //TODO for part 2:
        // result 241,823,802,412,393
        // trace back from rx to find node with multiple inputs (nx,sp,cc,jq)
        // creat a dict with the lowest cycle length, when they are HIGH!
        // do LCM on the results
    }

    void PushTheButton(Module broadcaster, Dictionary<string, Module> modules)
    {
        queue.Enqueue(new Signal(){source = null, high = false, target = broadcaster});
        while (queue.Count > 0)
        {
            Signal signal = queue.Dequeue();
            signal.target.HandleSignal(signal.source, signal.high, modules);
        }
    }

    public class Module
    {
        public Type type;
        public string name;
        public List<string> targets;
        
        private bool state = false;
        public Dictionary<Module?, bool> inputs = new Dictionary<Module?, bool>();


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
                default:
                    type = Type.Untyped;
                    break;
            }

            name = parts[0].Trim().Replace("&", "").Replace("%", "");
            targets = parts[1].Trim().Split(", ").ToList();
        }

        public void HandleSignal(Module? source, bool high, Dictionary<string, Module> modules)
        {
            // increase counter
            if (high) _highPulses++;
            else _lowPulses++;
            // Console.WriteLine($"{(source == null ? "button" : source.name)} -{high}-> {this.name}");
            
            // TODO print out the steps to get to rx high, try and spot a pattern or sos
            if (name.Equals("rx"))
                //if (high)
                    //Console.WriteLine($"BUTTON {_buttonCount}");
                if (!high)
                    Console.WriteLine($"FOUND it! {_buttonCount}");
            
            switch (type)
            {
                case Type.Broadcaster: 
                    targets.ForEach(target => queue.Enqueue(new Signal(){high=high, source=this, target=modules[target]}));
                    break;
                case Type.FlipFlop:
                    if (!high)
                    {
                        state = !state;
                        targets.ForEach(target => queue.Enqueue(new Signal(){high=state, source=this, target=modules[target]}));
                    }
                    break;
                case Type.Conjunction:
                    //inputs.TryGetValue(source, out bool prev);
                    inputs[source] = high;
                    if (name.Equals("nx") && inputs.Values.Count(b => b) != inputs.Count) Console.WriteLine($"FOUND nx! {_buttonCount}");
                    if (name.Equals("sp") && inputs.Values.Count(b => b) != inputs.Count) Console.WriteLine($"FOUND sp! {_buttonCount}");
                    if (name.Equals("cc") && inputs.Values.Count(b => b) != inputs.Count) Console.WriteLine($"FOUND cc! {_buttonCount}");
                    if (name.Equals("jq") && inputs.Values.Count(b => b) != inputs.Count) Console.WriteLine($"FOUND jq! {_buttonCount}");
                    targets.ForEach(target => queue.Enqueue(new Signal(){high=inputs.Values.Count(b => b) != inputs.Count, source=this, target=modules[target]}));
                    break;
                default:
                    return;
            }
        }
    }

    private struct Signal
    {
        public bool high;
        public Module source;
        public Module target;
    }
}