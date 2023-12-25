using System.Text.RegularExpressions;

namespace AoC2023;

public class Day25
{
    List<Node> _nodes = new List<Node>();
    Random _random = new Random(); 
    
    public void Run(List<string> input)
    {
        // After some googling, we'll use Karger's Contraction algorithm (not optimized - needs multiple attempts)
        // https://en.wikipedia.org/wiki/Karger%27s_algorithm
        while (true)
        {
            _nodes.Clear();
            input.Where(line => line.Length > 0).Select(l => Regex.Matches(l, @"([a-z]+)")).ToList()
                .ForEach(m => CreateNode(m[0].Value, m.Skip(1).Select(match => match.Value).ToList()));
            
            int cuts = FindMinCut();
            if (cuts == 3) break;
            // else Console.WriteLine($"Found {cuts} cuts :'(");
        }

        int part1 = (_nodes[0].name.Length / 3) * (_nodes[1].name.Length / 3);
        Console.WriteLine($"PART 1: {part1}");
    }

    int FindMinCut()
    {
        // keep contracting 2 random nodes until we only have 2 left: return edges between those 2
        while (_nodes.Count > 2) {
            CombineRandom();
        }
        return _nodes[0].neighbours.Count;
    }

    void CombineRandom()
    {
        Node node = _nodes[_random.Next(0, _nodes.Count)];
        Node neighbour = node.neighbours[_random.Next(0, node.neighbours.Count)];
        // combine: 
        // node name + neighbour name
        node.name += neighbour.name;
        // add neighbour neighbours (except node) to node neighbours
        foreach (Node neighbNeighb in neighbour.neighbours) {
            if (neighbNeighb.Equals(node)) continue;
            neighbNeighb.neighbours.Remove(neighbour);
            neighbNeighb.neighbours.Add(node);
            node.neighbours.Add(neighbNeighb);
        }
        // remove orig neighbour
        node.neighbours.RemoveAll(n => n.Equals(neighbour)); // can be multiple
        _nodes.Remove(neighbour);
    }

    Node CreateNode(string name, List<string> neighbourNames)
    {
        Node node = _nodes.Find(nd => nd.name.Equals(name));
        if (node == null)
        {
            node = new Node(name);
            _nodes.Add(node);
        }

        foreach (string neighbourName in neighbourNames)
        {
            Node neighbour = _nodes.Find(nd => nd.name.Equals(neighbourName));
            if (neighbour == null) {
                neighbour = new Node(neighbourName);
                _nodes.Add(neighbour);
            }
            neighbour.neighbours.Add(node);
            node.neighbours.Add(neighbour);            
        }

        return node;
    }

    class Node
    {
        public string name;
        public List<Node> neighbours = new List<Node>();

        public Node(string name)
        {
            this.name = name;
        }
    }

}