using System.Runtime.InteropServices;
using System.Threading.Tasks.Dataflow;

namespace AoC2023;

public class Day23
{	
    private Vector2Int[] _directions = new[] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
    private char[,] _grid;
    
    public void Run(List<string> input)
    {
        _grid = new char[input.Count,input[0].Length];
        for (int i = 0; i < input.Count; i++) {
            for (int j = 0; j < _grid.GetLength(1); j++) {
                _grid[i, j] = input[i][j];
            }
        }
        
        // Part 1: simple BFS based on grid (without building a real graph)
        // Console.WriteLine($"PART 1: {RawSlowBFS(true)}");
        // Same algorithm works for part 2 but is way too slow
        // Console.WriteLine($"PART 2: {RawSlowBFS(false)}");
        
        // We need to build a proper graph and compress it first
        // Input contains very little actual intersections, most of it is just very large paths
        // we can compress those
        // Ended up implementing both BFS and DFS solutions :/
        // Both work now - DFS is clearly faster (less overhead creating objects in memory)

        // PART 1
        Dictionary<Vector2Int, Node> nodes = BuildGraph(true);
        CompressGraph(nodes);
        int part1 = FindMaxPathDFS(nodes[new Vector2Int(0,1)], 0, new bool[_grid.GetLength(0), _grid.GetLength(1)], _grid.GetLength(0)-1);
        Console.WriteLine($"PART 1: {part1}");
        // int part1 = FindMaxPath(nodes);
        // Console.WriteLine($"PART 1 BFS: {part1}");

        // PART 2
        nodes = BuildGraph(false);
        CompressGraph(nodes);
        int part2 = FindMaxPathDFS(nodes[new Vector2Int(0,1)], 0, new bool[_grid.GetLength(0), _grid.GetLength(1)], _grid.GetLength(0)-1);
        Console.WriteLine($"PART 2: {part2}");
        // int part2bfs = FindMaxPath(nodes);
        // Console.WriteLine($"PART 2 BFS: {part2bfs}");
    }

    int FindMaxPathDFS(Node node, int depth, bool[,] visited, int xEnd)
    {
        if (node.x == xEnd) {
            return depth;
        }
        
        visited[node.x, node.y] = true;
        
        int max = 0;
        foreach (Edge edge in node.edges)
        {
            if (visited[edge.target.x, edge.target.y]) continue;
            max = Math.Max(max, FindMaxPathDFS(edge.target, depth + edge.weight, visited, xEnd));
        }
        
        visited[node.x, node.y] = false;
        return max;
    }
    
    int FindMaxPath(Dictionary<Vector2Int, Node> nodes)
    {
        Queue<(Node node, int depth, int[] visited)> queue = new Queue<(Node node, int depth, int[] visited)>();
        queue.Enqueue((nodes[new Vector2Int(0,1)], 0,  new int[1]));

        int max = 0;
        
        while (queue.Count > 0)
        {
            (Node node, int depth, int[] visited) tuple = queue.Dequeue();
            tuple.visited[tuple.visited.Length-1] = new Vector2Int(tuple.node.x, tuple.node.y).GetHashCode();
            
            if (tuple.node.x == _grid.GetLength(0) - 1 && tuple.node.y ==  _grid.GetLength(1) - 2) {
                max = Math.Max(max, tuple.depth);
                //Console.WriteLine(tuple.depth);
                continue;
            }

            foreach (Edge edge in tuple.node.edges)
            {
                if (tuple.visited.Contains(new Vector2Int(edge.target.x, edge.target.y).GetHashCode())) continue;
                int [] visited = new int[tuple.visited.Length+1];
                Array.Copy(tuple.visited, visited, tuple.visited.Length);
                queue.Enqueue((edge.target, tuple.depth + edge.weight, visited));
            }
        }

        return max;
    }
    
    Dictionary<Vector2Int, Node> BuildGraph(bool followSlopes)
    {
        Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();

        bool[,] visited = new bool[_grid.GetLength(0), _grid.GetLength(1)];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(0,1));

        while (queue.Count > 0)
        {
            Vector2Int pos = queue.Dequeue();
            nodes.TryGetValue(pos, out Node node);
            if (node == null) node = new Node(pos.x, pos.y);
            nodes[pos] = node;
            
            visited[pos.x, pos.y] = true;
            
            foreach (Vector2Int direction in _directions)
            {
                Vector2Int newPos = pos + direction;
				
                if (newPos.x < 0 || newPos.x >= _grid.GetLength(0)) continue; 
                if (newPos.y < 0 || newPos.y >= _grid.GetLength(1)) continue;
                if (visited[newPos.x, newPos.y]) continue;
               
                if (_grid[newPos.x, newPos.y] == '#') continue;

                int weight = 1;
                if (followSlopes)
                {
                    if (_grid[newPos.x, newPos.y] == '>') {
                        weight = 2;
                        newPos += new Vector2Int(0, 1);
                    }
                    else if (_grid[newPos.x, newPos.y] == '<') {
                        weight = 2;
                        newPos += new Vector2Int(0, -1);
                    }
                    else if (_grid[newPos.x, newPos.y] == '^') {
                        weight = 2;
                        newPos += new Vector2Int(-1, 0);
                    }
                    else if (_grid[newPos.x, newPos.y] == 'v') {
                        weight = 2;
                        newPos += new Vector2Int(1, 0);
                    }
                }
                
                nodes.TryGetValue(newPos, out Node next);
                if (next == null)
                {
                    next = new Node(newPos.x, newPos.y);
                    nodes[newPos] = next;
                }

                if (node == next) continue;
                node.edges.Add(new Edge(node, next, weight));
                if (weight == 1) next.edges.Add(new Edge(next, node, weight));
                if (!visited[newPos.x, newPos.y]) queue.Enqueue(newPos);
            }
        }

        return nodes;
    }

    void CompressGraph(Dictionary<Vector2Int, Node> nodes)
    {
        // Console.WriteLine($"Compression, starting with {nodes.Count} nodes");
        bool found = true;
        while (found)
        {
            found = false;
            List<Node> singles = nodes.Values.Where(node => node.edges.Count == 2).ToList();
            foreach (Node node in singles)
            {
                if (node.edges.Count != 2) continue;
                Edge left = node.edges.ToArray()[0];
                Edge right = node.edges.ToArray()[1];

                Node leftNode = left.target;
                Node rightNode = right.target;
                Edge leftEdge = leftNode.GetEdge(node);
                Edge rightEdge = rightNode.GetEdge(node);
                if (leftEdge == null || rightEdge == null) continue;

                int t = leftEdge.weight;
                leftEdge.target = rightNode;
                leftEdge.weight += rightEdge.weight;
                rightEdge.target = leftNode;
                rightEdge.weight += t;
                nodes.Remove(new Vector2Int(node.x, node.y));
                found = true;
            }
        }
        
        // Console.WriteLine($"Compression, ending with {nodes.Count} nodes");
    }

    int RawSlowBFS(bool followSlopes)
    {
        Queue<(Vector2Int pos, int depth, bool[,] visited)> queue = new Queue<(Vector2Int pos, int depth, bool[,] visited)>();
        queue.Enqueue((new Vector2Int(0,1), 0, new bool[_grid.GetLength(0),_grid.GetLength(1)]));

        int max = 0;
        
        while (queue.Count > 0)
        {
            (Vector2Int pos, int depth, bool[,] visited) node = queue.Dequeue();
            node.visited[node.pos.x, node.pos.y] = true;

            if (node.pos == new Vector2Int(_grid.GetLength(0) - 1, _grid.GetLength(1) - 2)) {
                max = Math.Max(max, node.depth);
                continue;
            }
            
            // evaluate all 4 directions
            foreach (Vector2Int direction in _directions)
            {
                Vector2Int newPos = node.pos + direction;
				
                if (newPos.x < 0 || newPos.x >= _grid.GetLength(0)) continue; 
                if (newPos.y < 0 || newPos.y >= _grid.GetLength(1)) continue;
                if (node.visited[newPos.x, newPos.y]) continue;
                
                bool[,] visited = new bool[_grid.GetLength(0),_grid.GetLength(1)];
                Array.Copy(node.visited, visited, _grid.GetLength(0)*_grid.GetLength(1));
                
                if (_grid[newPos.x, newPos.y] == '#') continue;
                else if (_grid[newPos.x, newPos.y] == '.' || !followSlopes)
                {
                    queue.Enqueue((newPos, node.depth+1, visited));
                }
                else if (_grid[newPos.x, newPos.y] == '>' && !visited[newPos.x, newPos.y+1])
                {
                    visited[newPos.x, newPos.y] = true;
                    queue.Enqueue((newPos+new Vector2Int(0,1), node.depth+2, visited));
                }
                else if (_grid[newPos.x, newPos.y] == '<' && !visited[newPos.x, newPos.y-1])
                {
                    visited[newPos.x, newPos.y] = true;
                    queue.Enqueue((newPos+new Vector2Int(0,-1), node.depth+2, visited));
                }
                else if (_grid[newPos.x, newPos.y] == '^' && !visited[newPos.x-1, newPos.y])
                {
                    visited[newPos.x, newPos.y] = true;
                    queue.Enqueue((newPos+new Vector2Int(-1,0), node.depth+2, visited));
                }
                else if (_grid[newPos.x, newPos.y] == 'v' && !visited[newPos.x+1, newPos.y])
                {
                    visited[newPos.x, newPos.y] = true;
                    queue.Enqueue((newPos+new Vector2Int(1,0), node.depth+2, visited));
                }
            }
        }

        return max;
    }

    class Node
    {
        public int x;
        public int y;
        public HashSet<Edge> edges = new HashSet<Edge>();

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Edge GetEdge(Node node)
        {
            return edges.FirstOrDefault(edge => edge.target == node);
        }

        public override string ToString()
        {
            return x + "," + y;
        }
    }

    class Edge
    {
        public Node source;
        public Node target;
        public int weight;

        public Edge(Node source, Node target, int weight)
        {
            this.source = source;
            this.target = target;
            this.weight = weight;
        }
    }
}