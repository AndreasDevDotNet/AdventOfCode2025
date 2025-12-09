using AoCToolbox;
using System.Diagnostics;

Console.WriteLine("-- Day 9: Movie Theater --");

var sw = Stopwatch.StartNew();

var rectangleCorners = File.ReadAllLines("input.txt")
    .Select(line => line.ParseInts())
    .Select(parts => (x: parts[0], y: parts[1]))
    .ToList();

long maxArea = 0;

for (int r1 = 0; r1 < rectangleCorners.Count; r1++)
{
    for (int r2 = r1 + 1; r2 < rectangleCorners.Count; r2++)
    {
        var corner1 = rectangleCorners[r1];
        var corner2 = rectangleCorners[r2];
        
        if (corner1.x != corner2.x && corner1.y != corner2.y)
        {
            long width = Math.Abs(corner2.x - corner1.x) + 1;
            long height = Math.Abs(corner2.y - corner1.y) + 1;
            long area = width * height;

            maxArea = Math.Max(maxArea, area);
        }
    }
}

Console.WriteLine($"Part 1: {maxArea} (took {sw.ElapsedMilliseconds}ms)");

sw.Restart();

var redTileSet = rectangleCorners.ToHashSet();
int minY = rectangleCorners.Min(p => p.y);
int maxY = rectangleCorners.Max(p => p.y);

// For each row, compute which X ranges contain green/red tiles
var validXRangesPerY = new Dictionary<int, List<(int start, int end)>>();

//Console.WriteLine($"Computing valid ranges for rows {minY} to {maxY}...");

for (int y = minY; y <= maxY; y++)
{
    if (y % 1000 == 0)
    {
        //Console.WriteLine($"Processing row {y}/{maxY}...");
    }
    
    var ranges = new List<(int start, int end)>();
    
    // Find all X coordinates where polygon edges cross this scanline
    var intersections = new List<int>();
    
    for (int i = 0; i < rectangleCorners.Count; i++)
    {
        var p1 = rectangleCorners[i];
        var p2 = rectangleCorners[(i + 1) % rectangleCorners.Count];
        
        // Add horizontal edge segments on this row
        if (p1.y == y && p2.y == y)
        {
            int minX = Math.Min(p1.x, p2.x);
            int maxX = Math.Max(p1.x, p2.x);
            ranges.Add((minX, maxX));
        }
        // Find vertical/diagonal edge crossings
        else if ((p1.y <= y && p2.y > y) || (p1.y > y && p2.y <= y))
        {
            // Calculate X where edge crosses this Y
            int intersectX = p1.x + (int)Math.Round((double)(y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y));
            intersections.Add(intersectX);
        }
    }
    
    // Sort intersections and create ranges for polygon interior
    intersections.Sort();
    for (int i = 0; i < intersections.Count - 1; i += 2)
    {
        ranges.Add((intersections[i], intersections[i + 1]));
    }
    
    // Merge overlapping ranges
    if (ranges.Count > 0)
    {
        ranges.Sort();
        var merged = new List<(int start, int end)>();
        var current = ranges[0];
        
        for (int i = 1; i < ranges.Count; i++)
        {
            if (ranges[i].start <= current.end + 1)
            {
                current = (current.start, Math.Max(current.end, ranges[i].end));
            }
            else
            {
                merged.Add(current);
                current = ranges[i];
            }
        }
        merged.Add(current);
        
        validXRangesPerY[y] = merged;
    }
}

//Console.WriteLine($"Precomputation complete (took {sw.ElapsedMilliseconds}ms)");

sw.Restart();
long maxArea2 = 0;

for (int r1 = 0; r1 < rectangleCorners.Count; r1++)
{
    if (r1 % 100 == 0)
    {
        //Console.WriteLine($"Checking from tile {r1}/{rectangleCorners.Count}... (best: {maxArea2})");
    }
    
    for (int r2 = r1 + 1; r2 < rectangleCorners.Count; r2++)
    {
        var corner1 = rectangleCorners[r1];
        var corner2 = rectangleCorners[r2];
        
        if (corner1.x != corner2.x && corner1.y != corner2.y)
        {
            int minX = Math.Min(corner1.x, corner2.x);
            int maxX = Math.Max(corner1.x, corner2.x);
            int minYRect = Math.Min(corner1.y, corner2.y);
            int maxYRect = Math.Max(corner1.y, corner2.y);
            
            // Check if [minX, maxX] is covered on all rows [minYRect, maxYRect]
            bool allRowsValid = true;
            
            for (int y = minYRect; y <= maxYRect && allRowsValid; y++)
            {
                if (!validXRangesPerY.ContainsKey(y))
                {
                    allRowsValid = false;
                    break;
                }
                
                bool rowCoversRange = false;
                foreach (var (start, end) in validXRangesPerY[y])
                {
                    if (start <= minX && end >= maxX)
                    {
                        rowCoversRange = true;
                        break;
                    }
                }
                
                if (!rowCoversRange)
                {
                    allRowsValid = false;
                }
            }
            
            if (allRowsValid)
            {
                long width = maxX - minX + 1;
                long height = maxYRect - minYRect + 1;
                long area = width * height;
                maxArea2 = Math.Max(maxArea2, area);
            }
        }
    }
}

Console.WriteLine($"Part 2: {maxArea2} (took {sw.Elapsed.Seconds}s)");

