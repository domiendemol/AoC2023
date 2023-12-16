namespace AoC2023;

public class Day16
{
    enum Direction {NORTH, WEST, SOUTH, EAST}

    private Direction[] _slashTransform = new[] { Direction.EAST, Direction.SOUTH, Direction.WEST, Direction.NORTH };
    private Direction[] _backslashTransform = new[] { Direction.WEST, Direction.NORTH, Direction.EAST, Direction.SOUTH };

    public void Run(List<string> input)
    {
        char[,] grid = Utils.ToCharArray(input);

        int part1 = GetTotalEnergy(grid, (0, 0, Direction.EAST));
        Console.WriteLine($"PART 1: {part1}");
        
        // PART 2
        int max = 0;
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            int engr = GetTotalEnergy(grid, (0, i, Direction.SOUTH));
            max = Math.Max(engr, max);
            engr = GetTotalEnergy(grid, (grid.GetLength(1)-1, i, Direction.NORTH));
            max = Math.Max(engr, max);
        }
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            int engr = GetTotalEnergy(grid, (i, 0, Direction.EAST));
            max = Math.Max(engr, max);
            engr = GetTotalEnergy(grid, (i, grid.GetLength(1)-1, Direction.WEST));
            max = Math.Max(engr, max);
        }
        
        Console.WriteLine($"PART 2: {max}");
    }

    int GetTotalEnergy(char[,] grid, (int, int, Direction) start)
    {
        bool[,] energized = new bool[grid.GetLength(0),grid.GetLength(1)];
        List<Direction>[,] dirCache = new List<Direction>[grid.GetLength(0),grid.GetLength(1)];
        for (int i = 0; i < dirCache.GetLength(0); i++) {
            for (int j = 0; j < dirCache.GetLength(1); j++) {
                dirCache[i, j] = new List<Direction>();
            }
        }

        Queue<(int, int, Direction)> queue = new Queue<(int, int, Direction)>();
        queue.Enqueue(start);
        
        // follow beam(s)
        while (queue.Count > 0) {
            FollowBeam(queue.Dequeue(), grid, energized, queue, dirCache);
        }
        int sum = 0;
        for (int i = 0; i < energized.GetLength(0); i++) {
            for (int j = 0; j < energized.GetLength(1); j++) {
                if (energized[i, j]) sum++;
            }
        }
        return sum;
    }

    void FollowBeam((int, int, Direction) next, char[,] grid, bool[,] energized, Queue<(int, int, Direction)> queue, List<Direction>[,] dirCache)
    {
        if (!ValidateBoundaries(next, grid)) return;
        if (dirCache[next.Item1, next.Item2].Contains(next.Item3)) return;
        
        if (grid[next.Item1, next.Item2] == '.')
        {
            queue.Enqueue(GetNextPos(next));
        }
        else if (grid[next.Item1, next.Item2] == '|')
        {
            if (next.Item3 == Direction.NORTH || next.Item3 == Direction.SOUTH)
            {
                queue.Enqueue(GetNextPos(next));
            }
            else
            {
                queue.Enqueue(GetNextPos((next.Item1, next.Item2, Direction.NORTH)));
                queue.Enqueue(GetNextPos((next.Item1, next.Item2, Direction.SOUTH)));
            }
        }
        else if (grid[next.Item1, next.Item2] == '-')
        {
            if (next.Item3 == Direction.EAST || next.Item3 == Direction.WEST)
            {
                queue.Enqueue(GetNextPos(next));
            }
            else
            {
                queue.Enqueue(GetNextPos((next.Item1, next.Item2, Direction.EAST)));
                queue.Enqueue(GetNextPos((next.Item1, next.Item2, Direction.WEST)));
            }
        }
        else if (grid[next.Item1, next.Item2] == '/' || grid[next.Item1, next.Item2] == '\\')
        {
            Direction nextDir = grid[next.Item1, next.Item2] == '/' ? _slashTransform[(int) next.Item3] : _backslashTransform[(int) next.Item3];
            queue.Enqueue(GetNextPos((next.Item1, next.Item2, nextDir)));
        }
        
        energized[next.Item1, next.Item2] = true;
        dirCache[next.Item1, next.Item2].Add(next.Item3);
    }

    (int, int, Direction) GetNextPos((int, int, Direction direction) current)
    {
        switch (current.Item3)
        {
            case Direction.NORTH:
                return (current.Item1 - 1, current.Item2, current.Item3);
            case Direction.SOUTH:
                return (current.Item1 + 1, current.Item2, current.Item3);
            case Direction.EAST:
                return (current.Item1, current.Item2 + 1, current.Item3);
            case Direction.WEST:
                return (current.Item1, current.Item2 - 1, current.Item3);
            default:
                return current;
        }
    }
    
    
    bool ValidateBoundaries((int, int, Direction) next, char[,] grid)
    {
        if (next.Item1 < 0 || next.Item2 < 0) return false;
        if (next.Item1 >= grid.GetLength(0) || next.Item2 >= grid.GetLength(1)) return false;
        return true;
    }
    
}