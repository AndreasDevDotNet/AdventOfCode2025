using AoCToolbox;

Console.WriteLine("-- Day8: Playground --");

var junctionBoxes = File.ReadAllLines("input.txt").Select(x => ParseJuctionBox(x)).ToList();

var pairs = new List<(int index1, int index2, double distance)>();
for (int i = 0; i < junctionBoxes.Count; i++)
{
    for (int j = i + 1; j < junctionBoxes.Count; j++)
    {
        var distance = Vector3D.EuclideanDistance(junctionBoxes[i], junctionBoxes[j]);
        pairs.Add((i, j, distance));
    }
}

pairs = pairs.OrderBy(p => p.distance).ToList();

var parent = Enumerable.Range(0, junctionBoxes.Count).ToArray();
var circuitSize = Enumerable.Repeat(1, junctionBoxes.Count).ToArray();

int Find(int x)
{
    if (parent[x] != x)
        parent[x] = Find(parent[x]); 
    return parent[x];
}

bool Union(int x, int y)
{
    int rootX = Find(x);
    int rootY = Find(y);

    if (rootX != rootY)
    {
        if (circuitSize[rootX] < circuitSize[rootY])
            (rootX, rootY) = (rootY, rootX);

        parent[rootY] = rootX;
        circuitSize[rootX] += circuitSize[rootY];
        return true; // Return true if a merge happened
    }
    return false; // Already in same circuit
}

int CountCircuits()
{
    return junctionBoxes.Select((_, i) => Find(i))
        .Distinct()
        .Count();
}

// Part 1: Connect the 1000 closest pairs
int pairsToConnect = 1000;
for (int idx = 0; idx < pairsToConnect && idx < pairs.Count; idx++)
{
    var (i, j, _) = pairs[idx];
    Union(i, j);
}

var circuits = junctionBoxes.Select((_, i) => Find(i))
    .GroupBy(root => root)
    .Select(g => circuitSize[g.Key])
    .OrderByDescending(size => size)
    .ToList();

var part1 = circuits.Take(3).Aggregate(1, (acc, size) => acc * size);
Console.WriteLine($"Part 1: {part1}");

// Part 2: Continue until all in one circuit
int lastConnectionIndex = -1;
for (int idx = pairsToConnect; idx < pairs.Count; idx++)
{
    var (i, j, _) = pairs[idx];
    
    if (Union(i, j))
    {
        lastConnectionIndex = idx;
        
        // Check if we now have only one circuit
        if (CountCircuits() == 1)
        {
            break;
        }
    }
}

if (lastConnectionIndex >= 0)
{
    var (i, j, _) = pairs[lastConnectionIndex];
    var part2 = (long)junctionBoxes[i].X * (long)junctionBoxes[j].X;
    Console.WriteLine($"Part 2: {part2}");
    Console.WriteLine($"Last connection was between box {i} at {junctionBoxes[i]} and box {j} at {junctionBoxes[j]}");
}

Vector3D ParseJuctionBox(string val)
{
    var coords = val.ParseInts();
    return new Vector3D(coords[0], coords[1], coords[2]);
}