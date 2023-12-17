using System.Collections;

namespace AoC2023;

public class Day17a
{
    private Vector2Int[] _directions = new[] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
    private int _gridWidth;
    private int _gridHeight;
    
    public void Run(List<string> input)
    {
        _gridHeight = input.Count;
        _gridWidth = input[0].Length;
        Block[,] blocks = new Block[_gridHeight, _gridWidth];
        // List<Block> nodeList = new List<Block>();
        for (int i = 0; i < _gridHeight; i++) {
            for (int j=0; j<_gridWidth; j++) {
                // nodeList.Add(new Block(Int32.Parse(""+input[i][j]), (i,j)));
                blocks[i,j] = (new Block(Int32.Parse(""+input[i][j]), new Vector2Int(i,j)));
            }
        }
        // TODO don't create a block list 
        // but have a dict (with a hash consisting of pos/lastStep)
        // don't create all blocks but just check in dijkstra method: if one don't exist, we create it

        List<Block> queue = new List<Block>();
        Block start = blocks[0, 0];
        start.totalHeatLoss = 0;
        queue.Add(start);

        // TODO don't use a queue, but a normal sorted list
        
        while (queue.Count > 0)
        {
            queue.Sort();
            Block block = queue.First();
            queue.RemoveAt(0); 
            
            // check its neighbours
            Vector2Int lastStep = block.lastStep;
            List<Vector2Int> directions = GetPossibleDirections(block.pos, lastStep);
            // int minHeat = directions.Min(dir => blocks[dir.x, dir.y].heatLoss);
            // foreach (Vector2Int nextDir in directions.Where(dir => blocks[dir.x, dir.y].heatLoss == minHeat))
            foreach (Vector2Int nextDir in directions)
            {
                // TODO do I need to set some values already here?
                // set heatloss
                Block next = blocks[block.pos.x + nextDir.x, block.pos.y + nextDir.y];
                if (!next.visited && next.heatLoss + block.totalHeatLoss < next.totalHeatLoss)
                {
                    next.totalHeatLoss = next.heatLoss + block.totalHeatLoss;
                    next.lastStep = (nextDir.Normalize() == block.lastStep.Normalize()) ? nextDir+block.lastStep : nextDir;
                    
                    // optionally store previous block
                    next.previous = block;
                    if (!next.visited) queue.Add(next);
                }
                //queue.Enqueue(next);
            }

            block.visited = true;
            Console.WriteLine(block.pos + " - " + block.totalHeatLoss);
        }
        Console.WriteLine(blocks[_gridHeight-1, _gridWidth - 1].totalHeatLoss);
        PrintMap(blocks);
        // pathfinding, use grid or build nodes?
// store visited 

        // dijkstra
    }

    List<Vector2Int> GetPossibleDirections(Vector2Int blockPos, Vector2Int lastStep)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (Vector2Int direction in _directions)
        {
            // check grid edges
            if (blockPos.x + direction.x < 0) continue;
            if (blockPos.y + direction.y < 0) continue;
            if (blockPos.x + direction.x >= _gridHeight) continue;
            if (blockPos.y + direction.y >= _gridWidth) continue;
            // should never go back
            if (lastStep.Normalize() == -1 * direction) continue;
            // should never do more than 3 steps in same direction
            if ((lastStep + direction).Magnitude() == 4f) continue;
            result.Add(direction);
        }
        return result;
    }

    void PrintMap(Block[,] blocks)
    {
        List<Vector2Int> pathPositions = new List<Vector2Int>();
        Block block = blocks[_gridHeight - 1, _gridWidth - 1];
        while (true)
        {
            pathPositions.Add(block.pos);
            if (block.previous == null) break;
            block = block.previous;
        }

        for (int i = 0; i < _gridHeight; i++) {
            for (int j=0; j<_gridWidth; j++) {
                // nodeList.Add(new Block(Int32.Parse(""+input[i][j]), (i,j)));
                Console.Write(pathPositions.Contains(new Vector2Int(i,j)) ? "#" : ".");
            }
            Console.WriteLine("");
        }
    }
 
    class Block: IComparable<Block>
    {
        public int heatLoss;
        public Vector2Int pos;
        public int totalHeatLoss = Int32.MaxValue;
        public Vector2Int lastStep;
        public bool visited = false;
        public Block previous;
        
        public Block(int heatLoss, Vector2Int pos)
        {
            this.heatLoss = heatLoss;
            this.pos = pos;
        }

        public int CompareTo(Block? other)
        {
            //int result = totalHeatLoss.CompareTo(other.totalHeatLoss);
            // if (result == 0) result = lastStep.Magnitude().CompareTo(other.lastStep.Magnitude());
            int result = (totalHeatLoss + lastStep.Magnitude()).CompareTo(other.totalHeatLoss + other.lastStep.Magnitude());
            return result;
        }
    }
}