using AoCToolbox;

Console.WriteLine("-- Day7: Laboratories --");

var manifoldGrid = File.ReadAllLines("input.txt").ToList();

long total = SimulateTachyonBeam(manifoldGrid);

Console.WriteLine($"Part1: {total}");

total = SimulateQuantumTachyonBeam(manifoldGrid);

Console.WriteLine($"Part2: {total}");

int SimulateTachyonBeam(List<string> manifoldGrid)
{
    int splits = 0;

    var startCol = manifoldGrid[0].IndexOf('S');
    
    var activeBeams = new HashSet<(int row, int col)> { (-1, startCol) };
    
    while (activeBeams.Count > 0)
    {
        var nextBeams = new HashSet<(int row, int col)>();
        
        foreach (var beam in activeBeams)
        {
            int nextRow = beam.row + 1;
            int col = beam.col;
            
            if (nextRow >= manifoldGrid.Count)
                continue;
            
            char cell = manifoldGrid[nextRow][col];
            
            if (cell == '^')
            {
                splits++;
                
                if (col - 1 >= 0)
                    nextBeams.Add((nextRow, col - 1));
                if (col + 1 < manifoldGrid[nextRow].Length)
                    nextBeams.Add((nextRow, col + 1));
            }
            else
            {
                nextBeams.Add((nextRow, col));
            }
        }
        
        activeBeams = nextBeams;
    }

    return splits;
}

long SimulateQuantumTachyonBeam(List<string> manifoldGrid)
{
    var startCol = manifoldGrid[0].IndexOf('S');
    
    var memo = new Dictionary<(int row, int col), long>();
    
    return CountPathsFrom(-1, startCol, manifoldGrid, memo);
}

long CountPathsFrom(int row, int col, List<string> manifoldGrid, Dictionary<(int row, int col), long> memo)
{
    if (memo.TryGetValue((row, col), out long cachedResult))
        return cachedResult;
    
    int nextRow = row + 1;
    
    if (nextRow >= manifoldGrid.Count)
        return 1;
    
    if (col < 0 || col >= manifoldGrid[nextRow].Length)
        return 0;
    
    char cell = manifoldGrid[nextRow][col];
    
    long pathCount;
    
    if (cell == '^')
    {
        long leftPaths = CountPathsFrom(nextRow, col - 1, manifoldGrid, memo);
        long rightPaths = CountPathsFrom(nextRow, col + 1, manifoldGrid, memo);
        pathCount = leftPaths + rightPaths;
    }
    else
    {
        pathCount = CountPathsFrom(nextRow, col, manifoldGrid, memo);
    }
    
    memo[(row, col)] = pathCount;
    return pathCount;
}
