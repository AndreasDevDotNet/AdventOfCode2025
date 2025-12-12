using AoCToolbox;

Console.WriteLine("-- Day 12: Christmas Tree Farm --");

var sections = File.ReadAllText("input.txt").SplitByDoubleNewline();

var shapes = (from shapeSection in sections[..^1] 
              let shape = shapeSection.Count(c => c == '#') 
              select shape).ToList();

var regions = sections.Last().SplitByNewline().Select(line => 
    {
        var parts = line.ParseInts();
        var dimensions = parts.Skip(2).ToArray();
        return (Width: parts[0], Height: parts[1], Dimensions: dimensions);
    }).ToList();

var numFittableRegions = 0;
foreach (var region in regions)
{
    var areaInRegion = Enumerable.Zip(region.Dimensions, shapes).Sum(p => p.First * p.Second);
    if (areaInRegion <= region.Width * region.Height)
    {
        numFittableRegions++;
    }
}

Console.WriteLine($"Part 1: {numFittableRegions}");

