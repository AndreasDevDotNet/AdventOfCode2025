
Console.WriteLine("-- Day 11: Reactor --");

var graph = ParseGraph(File.ReadAllText("input.txt"));
Dictionary<(string, string, HashSet<string>, bool, bool), long> memo = [];


Console.WriteLine($"Part 1: {CountPaths(graph, "you", "out", new HashSet<string>(), true, true)}");
Console.WriteLine($"Part 2: {CountPaths(graph, "svr", "out", new HashSet<string>())}");

long CountPaths(Dictionary<string, List<string>> graph, string current, string target, HashSet<string> seen, bool dac = false, bool fft = false)
{
    if (memo.TryGetValue((current, target, seen, dac, fft), out long result))
    {
        return result;
    }

    if (current == "dac")
        dac = true;
    if (current == "fft")
        fft = true;

    if (current == target)
    {
        if (dac && fft)
        {
            return 1;
        }
        return 0;
    }

    if(seen.Contains(current))
    {
        return 0;
    }

    seen.Add(current);

    long count = 0;
    foreach(var neighbor in graph[current])
    {
        if(!seen.Contains(neighbor))
        {
            count += CountPaths(graph, neighbor, target, seen, dac, fft);
        }
    }

    seen.Remove(current);
    memo[(current, target, seen, dac, fft)] = count;
    return count;
}


Dictionary<string, List<string>> ParseGraph(string input)
{
    Dictionary<string, List<string>> graph = [];

    var lines = input.Split('\n');

    foreach (var line in lines)
    {
        var parts = line.Replace("\r", "").Split(' ');
        graph[parts[0].Split(':')[0]] = parts[1..parts.Length].ToList();
        foreach (var p in parts[1..parts.Length])
        {
            if (!graph.ContainsKey(p))
            {
                graph[p] = [];
            }
        }
    }
    return graph;
}