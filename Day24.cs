namespace AoC2023;

public class Day24
{
    public void Run(List<string> input)
    {
        List<Line> lines = input.Select(line => new Line(line.Split('@'))).ToList();
        
        // PART 1
        int part1 = 0;
        for (int i = 0; i < lines.Count; i++) {
            for (int j = i+1; j < lines.Count; j++) {
                if (lines[i].Intersects(lines[j], 200000000000000, 400000000000000)) part1++;
            }
        }
        Console.WriteLine($"PART 1: {part1}");
    }

    class Line
    {
        public (long x, long y, long z) polong;
        public (long x, long y, long z) slope;
        public float a;
        public float b;
        
        public Line(string[] values)
        {
            string[] p = values[0].Split(',');
            polong = (Int64.Parse(p[0]), Int64.Parse(p[1]), Int64.Parse(p[2]));
            string[] s = values[1].Split(',');
            slope = (Int64.Parse(s[0]), Int64.Parse(s[1]), Int64.Parse(s[2]));
            a = (float) slope.y/slope.x;
            b = polong.y - polong.x * a;
        }

        public bool Intersects(Line other, float min, float max)
        {
            (float x, float y) intersection = Intersection(other);
            // Console.WriteLine(intersection);
            if (float.IsInfinity(intersection.x)) return false;
            if (intersection.x > polong.x && slope.x < 0) return false;
            if (intersection.x > other.polong.x && other.slope.x < 0) return false;
            if (intersection.x < polong.x && slope.x > 0) return false;
            if (intersection.x < other.polong.x && other.slope.x > 0) return false;
            if (intersection.x < min || intersection.x > max) return false;
            if (intersection.y < min || intersection.y > max) return false;
            return true;
        }
        
        public (float, float) Intersection(Line other)
        {
            float x = (other.b - b) / (a - other.a);
            float y = a * x + b;
            return (x, y);
        }
    }
}