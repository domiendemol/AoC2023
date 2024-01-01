using System.Text.RegularExpressions;

namespace AoC2023;

public class Day22
{
    private int[,,] _grid = new int[400,400,400];
    private Dictionary<int, Block> _blockDict = new Dictionary<int, Block>();

    public void Run(List<string> input)
    {
        // parse
        List<Block> blocks = input.Select((line, idx) => new Block(idx+1, Regex.Matches(line, @"[0-9]+").Select(m => m.Value).ToList())).ToList();
        blocks.Sort();
        
        // have them fall down
        blocks.ForEach(block => _blockDict[block.id] = block);

        blocks.ForEach(block => block.Fall(_grid));
        
        // find blocks we can disintegrate
        int part1 = 0;
        foreach (Block block in blocks)
        {
            // find out which blocks it supports - can be disintegrated if a supported block is supported by more than 1 block
            List<int> supportedBlocks = block.FindSupportedBlocks(_grid);
            bool canBeRemoved = true;
            foreach (int id in supportedBlocks)
            {
                Block suppBlock = _blockDict[id];
                if (suppBlock.GetSupportingBlockCount(_grid, null) == 1) canBeRemoved = false;
            }

            if (canBeRemoved) part1++;
        }
        Console.WriteLine($"PART 1: {part1}");
        
        // PART 2: for each block, count how many would fall
        int part2 = blocks.Sum(CountAllDisintegratingBlocks);
        Console.WriteLine($"PART 2: {part2}");
    }

    void MarkDisintegrated(Block block, List<int> disintegratedIds)
    {
        List<int> suppBlocks = block.FindSupportedBlocks(_grid);

        // go through tree and add all children recursively to disintegratedIds list
        foreach (int blockId in suppBlocks)
        {
            if (!disintegratedIds.Contains(blockId)) disintegratedIds.Add(blockId);
            MarkDisintegrated(_blockDict[blockId], disintegratedIds);
        }
    }

    // BFS 
    int CountAllDisintegratingBlocks(Block block)
    {
        List<int> disintegratedIds = new List<int>() { block.id };
        MarkDisintegrated(block, disintegratedIds);

        List<Block> toCheckBlocks = disintegratedIds.Select(id => _blockDict[id]).ToList();
        toCheckBlocks.Sort(); // important, go through them from low to high
        
        disintegratedIds = new List<int>() { block.id };
        toCheckBlocks.Remove(block);
        
        foreach (Block toCheckBlock in toCheckBlocks)
        {
            if (toCheckBlock.GetSupportingBlockCount(_grid, disintegratedIds) == 0) // this one does not fall as it's supported by at least another not falling block
            {
                disintegratedIds.Add(toCheckBlock.id);
            }
        }

        disintegratedIds.Remove(block.id);
        Console.WriteLine($"{block.id} -> {disintegratedIds.Count}");

        return disintegratedIds.Count;
    }
    
    
    class Block : IComparable<Block>
    {
        public int id;
        public (int x, int y, int z) start;
        public (int x, int y, int z) end;
        public int length;

        public Block(int id, List<string> coords)
        {
            this.id = id;
            start = (Int32.Parse(coords[0]), Int32.Parse(coords[1]), Int32.Parse(coords[2]));
            end = (Int32.Parse(coords[3]), Int32.Parse(coords[4]), Int32.Parse(coords[5]));
            length = Math.Max(end.x - start.x, Math.Max(end.y - start.y, end.z - start.z))+1;
        }

        public void Fall(int[,,] grid)
        {
            bool floating = IsFloating(grid);
            while (floating && start.z > 1)
            {
                start = (start.x, start.y, start.z - 1);
                end = (end.x, end.y, end.z - 1);
                floating = IsFloating(grid);
            }

            for (int i = 0; i < length; i++) {
                (int x, int y, int z) c = GetCell(i); // TODO check if cell in grid below it is free or ground
                grid[c.x, c.y, c.z] = id;
            }
        }

        public List<int> FindSupportedBlocks(int[,,] grid)
        {
            List<int> supported = new List<int>();
            for (int i = 0; i < length; i++)
            {
                (int x, int y, int z) c = GetCell(i); 
                if (grid[c.x, c.y, c.z+1] != 0 && grid[c.x, c.y, c.z+1] != id) supported.Add(grid[c.x, c.y, c.z+1]);
            }

            return supported.Distinct().ToList();
        }

        public int GetSupportingBlockCount(int[,,] grid, List<int> disintegrated)
        {
            List<int> supporting = new List<int>();
            for (int i = 0; i < length; i++)
            {
                (int x, int y, int z) c = GetCell(i);
                if (grid[c.x, c.y, c.z - 1] != 0 && grid[c.x, c.y, c.z - 1] != id && 
                    (disintegrated == null || !disintegrated.Contains(grid[c.x, c.y, c.z - 1]))) supporting.Add(grid[c.x, c.y, c.z - 1]);
            }

            return supporting.Distinct().Count();
        }
        
        bool IsFloating(int[,,] grid)
        {
            for (int i = 0; i < length; i++)
            {
                (int x, int y, int z) c = GetCell(i); 
                if (grid[c.x, c.y, c.z-1] != 0 && grid[c.x, c.y, c.z-1] != id) return false;
            }

            return true;
        }

        (int x, int y, int z) GetCell(int index)
        {
            if (index == 0) return start;
            if (index >= length) return end;
            if (start.x != end.x) return (start.x + index, start.y, start.z);
            if (start.y != end.y) return (start.x, start.y + index, start.z);
            if (start.z != end.z) return (start.x, start.y, start.z + index);

            return start;
        }
        
        public int CompareTo(Block? other)
        {
            return start.z.CompareTo(other.start.z);
        }
    }
}