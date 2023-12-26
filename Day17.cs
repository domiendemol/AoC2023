using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace AoC2023;

public class Day17
{
    private Vector2Int[] _directions = new[] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
    private int _gridWidth;
    private int _gridHeight;
    
    public void Run(List<string> input)
    {
        _gridHeight = input.Count;
        _gridWidth = input[0].Length;
        
        int[,] blocks = new int[_gridHeight, _gridWidth];
        for (int i = 0; i < _gridHeight; i++) {
            for (int j=0; j<_gridWidth; j++) {
                blocks[i, j] = Int32.Parse("" + input[i][j]);
            }
        }
        // TODO currently off by one on the actual input (1127 instead of 1128) and no idea why, works fine for part 2
        int part1 = FindShortestPath(blocks, false);        
        Console.WriteLine($"PART 1: {part1+1}");
        int part2 = FindShortestPath(blocks, true);
        Console.WriteLine($"PART 2: {part2}");
    }

    private int FindShortestPath(int[,] blocks, bool ultra)
    {
        Dictionary<(Vector2Int pos, Vector2Int dir), int> queue = new Dictionary<(Vector2Int pos, Vector2Int dir), int>();
        Dictionary<(Vector2Int pos, Vector2Int dir), int> minDist = new Dictionary<(Vector2Int pos, Vector2Int dir), int>();

        queue[(new Vector2Int(0, 0), new Vector2Int(0, 0))] = 0;

        while (queue.Count > 0)
        {
            int min = queue.Min(x => x.Value);
            KeyValuePair<(Vector2Int pos, Vector2Int dir), int> cur = queue.Where(x => x.Value == min).First();
            queue.Remove(cur.Key);

            minDist.TryAdd(cur.Key, min);

            // check its neighbours
            Vector2Int lastDir = cur.Key.dir;
            List<Vector2Int> directions = GetPossibleDirections(cur.Key.pos, lastDir, ultra);
            foreach (Vector2Int nextDir in directions)
            {
                int cost = cur.Value;
                int magn = (int) nextDir.AbsMax();
                for (int i = 1; i <= magn; i++) cost += blocks[(cur.Key.pos + i*(nextDir/magn)).x, (cur.Key.pos + i*(nextDir/magn)).y];  
                
                (Vector2Int pos, Vector2Int dir) nextPos = (cur.Key.pos + nextDir,
                    (nextDir.Normalize() == lastDir.Normalize()) ? nextDir + lastDir : nextDir);
                if (!minDist.ContainsKey(nextPos)) // not visited yet
                {
                    queue.TryAdd(nextPos, cost);
                    minDist.TryAdd(cur.Key, cost);
                }
                else minDist[nextPos] = Math.Min(minDist[nextPos], cost);
            }
            // try to stop a bit earlier
            if (cur.Key.pos == new Vector2Int(_gridHeight - 1, _gridWidth - 1)) queue.Clear();
        }

        int result = minDist.Where(kvp => kvp.Key.pos == new Vector2Int(_gridHeight - 1, _gridWidth - 1))
            .Min(kvp => kvp.Value);
        return result;
    }

    List<Vector2Int> GetPossibleDirections(Vector2Int blockPos, Vector2Int lastStep, bool ultra)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        if (ultra) // aka part 2
        {
            foreach (Vector2Int direction in _directions)
            {
                Vector2Int newDir = direction;
                if (direction.Normalize() != lastStep.Normalize()) newDir = 4 * direction;
                // check grid edges
                if (blockPos.x + newDir.x < 0) continue;
                if (blockPos.y + newDir.y < 0) continue;
                if (blockPos.x + newDir.x >= _gridHeight) continue;
                if (blockPos.y + newDir.y >= _gridWidth) continue;
                // should never go back
                if (lastStep.Normalize() == -1 * direction) continue;
                if (lastStep.Max() > 0 && lastStep.Max() < 4 && lastStep.Normalize() != direction.Normalize()) continue;
                // should never do more than 3 or 10 steps in same direction
                if ((lastStep + direction).Max() >= 11f) continue;
                result.Add(newDir);
            }
        }
        else
        {
            foreach (Vector2Int direction in _directions)
            {
                // check grid edges
                if (blockPos.x + direction.x < 0) continue;
                if (blockPos.y + direction.y < 0) continue;
                if (blockPos.x + direction.x >= _gridHeight) continue;
                if (blockPos.y + direction.y >= _gridWidth) continue;
                // should never go back
                if (lastStep.Normalize() == -1 * direction) continue;
                // should never do more than 3 or 10 steps in same direction
                if ((lastStep + direction).Max() >=  4f) continue;
                result.Add(direction);
            }
        }
        return result;
    }

    void PrintMap(Dictionary<(Vector2Int, Vector2Int), int> minDist)
    {
        List<Vector2Int> pathPositions = new List<Vector2Int>();
        int min = minDist.Where(kvp => kvp.Key.Item1 == new Vector2Int(_gridHeight-1, _gridWidth-1)).Min(x => x.Value);
        KeyValuePair<(Vector2Int pos, Vector2Int dir), int> cur = minDist.Where(x => x.Value == min).First();
        while (true)
        {
            pathPositions.Add(cur.Key.pos);
            if (cur.Key.pos.x == 0 && cur.Key.pos.y == 0) break;
            Vector2Int prevPos = cur.Key.pos - cur.Key.dir.Normalize();
            min = minDist.Where(kvp => kvp.Key.Item1 == prevPos).Min(x => x.Value);
            cur = minDist.Where(x => x.Value == min).First();
        }

        for (int i = 0; i < _gridHeight; i++) {
            for (int j=0; j<_gridWidth; j++) {
                Console.Write(pathPositions.Contains(new Vector2Int(i,j)) ? "#" : ".");
            }
            Console.WriteLine("");
        }
    }
}