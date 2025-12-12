using AoCToolbox;

Console.WriteLine("-- Day 12: Christmas Tree Farm --");

var inputShapes = File.ReadAllText("input-shapes.txt").SplitByDoubleNewline().Select(x => x.SplitByNewline()).ToList(); 
var inputRegions = File.ReadAllLines("input-regions.txt").ToList();

// Parse shapes and precompute variants
var shapes = new List<Shape>();
foreach (var shapeLines in inputShapes)
{
    var indexLine = shapeLines[0];
    var index = int.Parse(indexLine.TrimEnd(':'));
    var pattern = shapeLines.Skip(1).ToList();
    shapes.Add(new Shape(index, pattern));
}

// Precompute all variants for each shape
var shapeVariants = shapes.Select(s => s.GetAllVariants()).ToList();

// Parse regions and check each one
int fittableRegions = 0;
foreach (var regionLine in inputRegions)
{
    var parts = regionLine.Split(':');
    var dimensions = parts[0].Trim().Split('x');
    int width = int.Parse(dimensions[0]);
    int height = int.Parse(dimensions[1]);
    int regionArea = width * height;
    
    var quantities = parts[1].Trim().Split(' ').Select(int.Parse).ToArray();
    
    var presents = new List<int>();
    for (int i = 0; i < quantities.Length; i++)
    {
        for (int j = 0; j < quantities[i]; j++)
        {
            presents.Add(i);
        }
    }
    
    // Early area check: Calculate total area of all shapes
    int totalShapesArea = presents.Sum(p => shapeVariants[p][0].Cells.Count);
    
    if (totalShapesArea > regionArea)
    {
        // Impossible to fit - skip expensive backtracking
        continue;
    }
    
    // Sort presents by size (larger first) for better pruning
    presents = presents.OrderByDescending(p => shapeVariants[p][0].Cells.Count).ToList();
    
    var grid = new bool[height * width];
    
    if (CanFitPresents(grid, width, height, presents, shapeVariants, 0))
    {
        fittableRegions++;
    }
}

Console.WriteLine($"Part 1: {fittableRegions}");

bool CanFitPresents(bool[] grid, int width, int height, List<int> presents, List<List<ShapeVariant>> allVariants, int presentIndex)
{
    if (presentIndex == presents.Count)
        return true;
    
    int shapeIndex = presents[presentIndex];
    var variants = allVariants[shapeIndex];
    
    foreach (var variant in variants)
    {
        // Skip variants that obviously won't fit
        if (variant.Width > width || variant.Height > height)
            continue;
            
        int maxY = height - variant.Height;
        int maxX = width - variant.Width;
        
        for (int y = 0; y <= maxY; y++)
        {
            for (int x = 0; x <= maxX; x++)
            {
                if (CanPlaceFast(grid, width, variant, x, y))
                {
                    PlaceFast(grid, width, variant, x, y, true);
                    if (CanFitPresents(grid, width, height, presents, allVariants, presentIndex + 1))
                    {
                        PlaceFast(grid, width, variant, x, y, false);
                        return true;
                    }
                    PlaceFast(grid, width, variant, x, y, false);
                }
            }
        }
    }
    
    return false;
}

bool CanPlaceFast(bool[] grid, int width, ShapeVariant variant, int startX, int startY)
{
    foreach (var (dx, dy) in variant.Cells)
    {
        int index = (startY + dy) * width + (startX + dx);
        if (grid[index])
            return false;
    }
    return true;
}

void PlaceFast(bool[] grid, int width, ShapeVariant variant, int startX, int startY, bool occupied)
{
    foreach (var (dx, dy) in variant.Cells)
    {
        int index = (startY + dy) * width + (startX + dx);
        grid[index] = occupied;
    }
}

record Shape(int Index, List<string> Pattern)
{
    private List<ShapeVariant>? _cachedVariants;
    
    public List<ShapeVariant> GetAllVariants()
    {
        if (_cachedVariants != null)
            return _cachedVariants;
            
        var variants = new HashSet<ShapeVariant>(new ShapeVariantComparer());
        var current = ParsePattern(Pattern);
        
        for (int rotation = 0; rotation < 4; rotation++)
        {
            variants.Add(current);
            variants.Add(Flip(current));
            current = Rotate(current);
        }
        
        _cachedVariants = variants.ToList();
        return _cachedVariants;
    }
    
    private ShapeVariant ParsePattern(List<string> pattern)
    {
        var cells = new List<(int x, int y)>();
        for (int y = 0; y < pattern.Count; y++)
        {
            for (int x = 0; x < pattern[y].Length; x++)
            {
                if (pattern[y][x] == '#')
                {
                    cells.Add((x, y));
                }
            }
        }
        return Normalize(cells);
    }
    
    private ShapeVariant Rotate(ShapeVariant variant)
    {
        var rotated = variant.Cells.Select(c => (-c.y, c.x)).ToList();
        return Normalize(rotated);
    }
    
    private ShapeVariant Flip(ShapeVariant variant)
    {
        var flipped = variant.Cells.Select(c => (-c.x, c.y)).ToList();
        return Normalize(flipped);
    }
    
    private ShapeVariant Normalize(List<(int x, int y)> cells)
    {
        int minX = cells.Min(c => c.x);
        int minY = cells.Min(c => c.y);
        var normalized = cells.Select(c => (c.x - minX, c.y - minY)).OrderBy(c => c.Item2).ThenBy(c => c.Item1).ToList();
        int width = normalized.Max(c => c.Item1) + 1;
        int height = normalized.Max(c => c.Item2) + 1;
        return new ShapeVariant(normalized, width, height);
    }
}

record ShapeVariant(List<(int x, int y)> Cells, int Width, int Height);

class ShapeVariantComparer : IEqualityComparer<ShapeVariant>
{
    public bool Equals(ShapeVariant? x, ShapeVariant? y)
    {
        if (x == null || y == null) return false;
        if (x.Cells.Count != y.Cells.Count) return false;
        return x.Cells.SequenceEqual(y.Cells);
    }
    
    public int GetHashCode(ShapeVariant obj)
    {
        var hash = new HashCode();
        foreach (var cell in obj.Cells)
        {
            hash.Add(cell);
        }
        return hash.ToHashCode();
    }
}

