using AoCToolbox;

Console.WriteLine("-- Day 5: Cafeteria --");

var input = File.ReadAllText("input.txt").SplitByDoubleNewline();

var freshIngredientRanges = input[0].SplitByNewline().Select(line => Range<long>.Parse(line)).ToList();
var ingredients = input[1].SplitByNewline().Select(line => long.Parse(line)).ToList();

var freshIngredients = new HashSet<long>();

foreach (var range in freshIngredientRanges)
{
    var freshIngredientsInRange = ingredients.Where(ing => range.Contains(ing));
    foreach (var ing in freshIngredientsInRange)
    {
        freshIngredients.Add(ing);
    }
}

Console.WriteLine($"Part 1: {freshIngredients.Count}");

freshIngredientRanges.Sort((a, b) => a.Min.CompareTo(b.Min));

var mergedRanges = new List<Range<long>>();
var current = freshIngredientRanges[0];

for (int i = 1; i < freshIngredientRanges.Count; i++)
{
    if (freshIngredientRanges[i].Min <= current.Max + 1)
    {
        // Overlapping or adjacent ranges - merge them
        current = new Range<long>(current.Min, Math.Max(current.Max, freshIngredientRanges[i].Max));
    }
    else
    {
        // Non-overlapping - save current and start new
        mergedRanges.Add(current);
        current = freshIngredientRanges[i];
    }
}
mergedRanges.Add(current);

// Calculate total count from merged ranges using the Length property
long totalFreshIds = mergedRanges.Sum(r => r.Length);

Console.WriteLine($"Part 2: {totalFreshIds}");
