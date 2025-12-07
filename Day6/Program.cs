using AoCToolbox;

Console.WriteLine("-- Day6: Trash Compactor --");

var input = File.ReadAllText("input.txt").Trim();

long total = Part1(input);

Console.WriteLine($"Part1: {total}");

total = Part2(input);

Console.WriteLine($"Part2: {total}");

static long Part1(string input)
{
    var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    var operatorLine = lines[^1];

    var columns = new List<Column>();
    var columnCount = operatorLine.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

    for (int col = 0; col < columnCount; col++)
    {
        var numbers = new List<int>();
        for (int row = 0; row < lines.Length - 1; row++) 
        {
            var values = lines[row].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (col < values.Length)
            {
                numbers.Add(int.Parse(values[col]));
            }
        }

        var operators = operatorLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        char op = operators[col][0];

        columns.Add(new Column(numbers, op));
    }

    long total = 0;

    foreach (var column in columns)
    {
        total += column.Operator == '+'
            ? column.Numbers.Sum()
            : column.Numbers.Multiply();

    }

    return total;
}

static long Part2(string input)
{
    var lines = input.Split('\n');
    var operatorLine = lines[^1];
    var numberLines = lines[..^1];

    var problemRanges = new List<(int start, int end, char op)>();
    int i = 0;
    while (i < operatorLine.Length)
    {
        while (i < operatorLine.Length && operatorLine[i] == ' ')
            i++;
        
        if (i >= operatorLine.Length) break;
        
        char op = operatorLine[i];
        int operatorPos = i;
        
        int start = operatorPos;
        while (start > 0 && operatorLine[start - 1] == ' ')
            start--;
        
        int problemStart = start;
        while (problemStart > 0 && operatorLine[problemStart - 1] == ' ')
            problemStart--;

        problemRanges.Add((operatorPos, operatorPos, op));
        i++;
    }


    var problems = new List<Column>();
    
    var operatorPositions = new List<(int pos, char op)>();
    for (int pos = 0; pos < operatorLine.Length; pos++)
    {
        if (operatorLine[pos] != ' ')
        {
            operatorPositions.Add((pos, operatorLine[pos]));
        }
    }

    foreach (var (opPos, op) in operatorPositions)
    {
        var numbers = new List<int>();

        int leftBound = opPos;
        int rightBound = opPos;

        foreach (var line in numberLines)
        {
            for (int p = opPos; p >= 0 && p < line.Length && line[p] != ' '; p--)
            {
                leftBound = Math.Min(leftBound, p);
            }
            for (int p = opPos; p < line.Length && line[p] != ' '; p++)
            {
                rightBound = Math.Max(rightBound, p);
            }
        }

        for (int col = rightBound; col >= leftBound; col--)
        {
            string digitString = "";
            foreach (var line in numberLines)
            {
                if (col < line.Length && char.IsDigit(line[col]))
                {
                    digitString += line[col];
                }
            }
            
            if (!string.IsNullOrEmpty(digitString))
            {
                numbers.Add(int.Parse(digitString));
            }
        }
        
        problems.Add(new Column(numbers, op));
    }

    long total = 0;

    foreach (var problem in problems)
    {
        long result = problem.Operator == '+'
            ? problem.Numbers.Sum()
            : problem.Numbers.Select(n => (long)n).Multiply();
        total += result;
    }

    return total;
}

record Column(List<int> Numbers, char Operator);
