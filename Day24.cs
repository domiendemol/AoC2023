using System.Diagnostics;

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
        
        // PART 2
        (long x, long y, long z) pos1 = lines[0].point;
        (long x, long y, long z) pos2 = lines[1].point;
        foreach (int i in Range(500))
        {
            foreach (int j in Range(500))
            {
                // check if we can find intersection with first 2 or 3 lines
                // take line0 and line1: find line that crosses them with velocity (i,j,k)
                // => get start pos (x,y) and velocities t,s
                // use our solver
                // x0 + t*xv0 = x + ti
                // y0 + t*yv0 = y + tj
                // x1 + s*xv1 = x + si
                // y1 + s*yv1 = y + sj
                // or written differently:
                // x +     t*(i-xv0)           = x0
                //     y + t*(j-yv0)           = y0
                // x +               s*(i-xv1) = x1
                //     y +           s*(j-yv1) = y1
                // do same for line0 and line2
                // can be in 2 dimensions, and then check if Z matches too
                long posX, posY, t, s, u;
                double[,] eqMatrix = new double[4,5];
                eqMatrix[0,0] = 1;
                eqMatrix[0,2] = i-lines[0].slope.x;
                eqMatrix[0,4] = lines[0].point.x;
                eqMatrix[1,1] = 1;
                eqMatrix[1,2] = j-lines[0].slope.y;
                eqMatrix[1,4] = lines[0].point.y;
                eqMatrix[2,0] = 1;
                eqMatrix[2,3] = i-lines[1].slope.x;
                eqMatrix[2,4] = lines[1].point.x;
                eqMatrix[3,1] = 1;
                eqMatrix[3,3] = j-lines[1].slope.y;
                eqMatrix[3,4] = lines[1].point.y;
                if (LinearEquationSolver.Solve(ref eqMatrix))
                {
                    //Console.WriteLine($"1 - {eqMatrix[0, 4]}, {eqMatrix[1, 4]}");
                    posX = (long)Math.Round(eqMatrix[0, 4]);
                    posY = (long)Math.Round(eqMatrix[1, 4]);
                    t = (long)Math.Round(eqMatrix[2, 4]);
                    s = (long)Math.Round(eqMatrix[3, 4]);
                }
                else continue;
                
                // do same for line0 and line2
                // must be same pos
                eqMatrix = new double[4,5];
                eqMatrix[0,0] = 1;
                eqMatrix[0,2] = i-lines[0].slope.x;
                eqMatrix[0,4] = lines[0].point.x;
                eqMatrix[1,1] = 1;
                eqMatrix[1,2] = j-lines[0].slope.y;
                eqMatrix[1,4] = lines[0].point.y;
                eqMatrix[2,0] = 1;
                eqMatrix[2,3] = i-lines[2].slope.x;
                eqMatrix[2,4] = lines[2].point.x;
                eqMatrix[3,1] = 1;
                eqMatrix[3,3] = j-lines[2].slope.y;
                eqMatrix[3,4] = lines[2].point.y;
                if (LinearEquationSolver.Solve(ref eqMatrix) && 
                    (long)Math.Round(eqMatrix[0, 4]) == posX && (long)Math.Round(eqMatrix[1, 4]) == posY)
                {
                    //Console.WriteLine($"2 - {eqMatrix[0, 4]}, {eqMatrix[1, 4]}");
                    u = (long)Math.Round(eqMatrix[3, 4]);
                }
                else continue;

                foreach (int k in Range(500))
                {
                    // check z
                    // find Z start points
                    // use line 0 intersection to go back to first Z start point
                    double intersectZ0 = lines[0].point.z + t * lines[0].slope.z;
                    double start0 = intersectZ0 - t * k;
                    
                    double intersectZ1 = lines[1].point.z + s * lines[1].slope.z;
                    double start1 = intersectZ1 - s * k;
                    
                    double intersectZ2 = lines[2].point.z + u * lines[2].slope.z;
                    double start2 = intersectZ2 - u * k;
 
                    // If they don't align, keep searching
                    if (start0 == start1 && start1 == start2)
                    {
                        // Console.WriteLine($"pos: {posX}, {posY}, {start0}");   
                        // Console.WriteLine($"vel: {i}, {j}, {k}");   
                        Console.WriteLine($"PART 2: {((long)posX+(long)posY+(long)start0).ToString("0." + new string('#', 339))}");
                        return;
                    }
                }

            }
        }
    }
    
    // 0, -1, 1, -2, 2, -3, 3...
    public IEnumerable<int> Range(int max)
    {
        int i = 0;
        yield return i;
        while (i < max)
        {
            if (i >= 0) i++;
            i *= -1;
            yield return i;
        }
    }

    class Line
    {
        public (long x, long y, long z) point;
        public (long x, long y, long z) slope;
        public float a;
        public float b;
        
        public Line(string[] values)
        {
            string[] p = values[0].Split(',');
            point = (Int64.Parse(p[0]), Int64.Parse(p[1]), Int64.Parse(p[2]));
            string[] s = values[1].Split(',');
            slope = (Int64.Parse(s[0]), Int64.Parse(s[1]), Int64.Parse(s[2]));
            a = (float) slope.y/slope.x;
            b = point.y - point.x * a;
        }

        public Line((long x, long y, long z) pos1, (long x, long y, long z) pos2)
        {
            point = pos1;
            slope = (pos2.x - pos1.x, pos2.y - pos1.y,pos2.z - pos1.z);
            a = (float) slope.y/slope.x;
            b = point.y - point.x * a;
        }

        public bool Intersects(Line other, float min, float max)
        {
            (float x, float y) intersection = Intersection(other);
            // Console.WriteLine(intersection);
            if (float.IsInfinity(intersection.x)) return false;
            if (intersection.x > point.x && slope.x < 0) return false;
            if (intersection.x > other.point.x && other.slope.x < 0) return false;
            if (intersection.x < point.x && slope.x > 0) return false;
            if (intersection.x < other.point.x && other.slope.x > 0) return false;
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
    
    // Gaussian Elimination based on https://www.codeproject.com/tips/388179/linear-equation-solver-gaussian-elimination-csharp
    public static class LinearEquationSolver
    {
        /// <summary>Computes the solution of a linear equation system.</summary>
        /// <param name="M">
        /// The system of linear equations as an augmented matrix[row, col] where (rows == cols + 1).
        /// It will contain the solution in "row canonical form" if the function returns "true".
        /// </param>
        /// <returns>Returns whether the matrix has a unique solution or not.</returns>
        public static bool Solve(ref double[,] M)
        {
            int rowCount = M.GetLength(0);
            if (M == null || M.Length != rowCount * (rowCount + 1))
                throw new ArgumentException("The algorithm must be provided with a (n x n+1) matrix.");
            // pivoting
            for (int col = 0; col + 1 < rowCount; col++)
            {
                if (M[col, col] == 0) // check for zero coefficients
                {
                    // find non-zero coefficient
                    int swapRow = 0; // col + 1;
                    for (; swapRow < rowCount; swapRow++)
                        if (M[swapRow, col] != 0)
                            break;
                    if (swapRow < rowCount && M[swapRow, col] != 0) 
                    {
                        // found non-zeo coefficient, swap it with the above
                        double[] tmp = new double[rowCount + 1];
                        for (int i = 0; i < rowCount + 1; i++)
                        {
                            tmp[i] = M[swapRow, i];
                            M[swapRow, i] = M[col, i];
                            M[col, i] = tmp[i];
                        }
                    }
                    else return false; // no, then the matrix has no unique solution
                }
            }

            // elimination
            for (int sourceRow = 0; sourceRow + 1 < rowCount; sourceRow++)
            {
                for (int destRow = sourceRow + 1; destRow < rowCount; destRow++)
                {
                    double df = M[sourceRow, sourceRow];
                    double sf = M[destRow, sourceRow];
                    for (int i = 0; i < rowCount + 1; i++) {
                        M[destRow, i] = M[destRow, i] * df - M[sourceRow, i] * sf;
                    }
                }
            }

            // back-insertion
            for (int row = rowCount - 1; row >= 0; row--)
            {
                double f = M[row,row];
                if (f == 0) return false;

                for (int i = 0; i < rowCount + 1; i++) M[row, i] /= f;
                for (int destRow = 0; destRow < row; destRow++) {
                    M[destRow, rowCount] -= M[destRow, row] * M[row, rowCount]; M[destRow, row] = 0;
                }
            }
            return true;
            // TODO return something else
        }
    }
}