using System.Text.RegularExpressions;

Console.WriteLine("-- Day 10 Factory --");

var machines = File.ReadAllLines("input.txt")
    .Select(line => ParseMachine(line))
    .ToList();

int totalPresses = machines.Sum(machine => SolveMinimumIndicatorPresses(machine));
Console.WriteLine($"Part 1: {totalPresses}");

long totalPresses2 = machines.Sum(machine => SolveMinimumJoltageLevelsPresses(machine));
Console.WriteLine($"Part 2: {totalPresses2}");

int SolveMinimumIndicatorPresses(Machine machine)
{
    int numLights = machine.Indicators.Count;
    int numButtons = machine.Buttons.Count;
    
    // Create augmented matrix for Gaussian elimination over GF(2)
    bool[,] matrix = new bool[numLights, numButtons + 1];
    
    // Fill matrix: matrix[light][button] = true if button toggles that light
    for (int button = 0; button < numButtons; button++)
    {
        foreach (int light in machine.Buttons[button])
        {
            matrix[light, button] = true;
        }
    }
    
    // Set target state (last column)
    for (int light = 0; light < numLights; light++)
    {
        matrix[light, numButtons] = machine.Indicators[light];
    }
    
    // Gaussian elimination over GF(2) - convert to reduced row echelon form
    int[] pivotCol = new int[numLights];
    Array.Fill(pivotCol, -1);
    int currentRow = 0;
    
    for (int col = 0; col < numButtons && currentRow < numLights; col++)
    {
        // Find pivot row
        int pivotRow = -1;
        for (int row = currentRow; row < numLights; row++)
        {
            if (matrix[row, col])
            {
                pivotRow = row;
                break;
            }
        }
        
        if (pivotRow == -1) continue;
        
        // Swap rows if needed
        if (pivotRow != currentRow)
        {
            for (int c = 0; c <= numButtons; c++)
            {
                (matrix[currentRow, c], matrix[pivotRow, c]) = (matrix[pivotRow, c], matrix[currentRow, c]);
            }
        }
        
        // Mark which column is the pivot for this row
        pivotCol[currentRow] = col;
        
        // Eliminate ALL other rows (both above and below)
        for (int row = 0; row < numLights; row++)
        {
            if (row != currentRow && matrix[row, col])
            {
                for (int c = 0; c <= numButtons; c++)
                {
                    matrix[row, c] ^= matrix[currentRow, c];
                }
            }
        }
        
        currentRow++;
    }
    
    // Check for inconsistency
    for (int row = 0; row < numLights; row++)
    {
        if (pivotCol[row] == -1 && matrix[row, numButtons])
        {
            throw new InvalidOperationException("No solution exists");
        }
    }
    
    // Find free variables (columns without pivots)
    List<int> freeVars = new();
    HashSet<int> pivotCols = new(pivotCol.Where(p => p != -1));
    for (int col = 0; col < numButtons; col++)
    {
        if (!pivotCols.Contains(col))
        {
            freeVars.Add(col);
        }
    }
    
    // Try all combinations of free variables to find minimum
    int minPresses = int.MaxValue;
    int numCombinations = 1 << freeVars.Count; // 2^n combinations
    
    for (int combo = 0; combo < numCombinations; combo++)
    {
        bool[] buttonPresses = new bool[numButtons];
        
        // Set free variables according to current combination
        for (int i = 0; i < freeVars.Count; i++)
        {
            buttonPresses[freeVars[i]] = ((combo >> i) & 1) == 1;
        }
        
        // Calculate dependent variables (pivot columns)
        for (int row = 0; row < numLights; row++)
        {
            if (pivotCol[row] != -1)
            {
                bool value = matrix[row, numButtons];
                // XOR with all free variables in this row
                for (int col = 0; col < numButtons; col++)
                {
                    if (matrix[row, col] && col != pivotCol[row])
                    {
                        value ^= buttonPresses[col];
                    }
                }
                buttonPresses[pivotCol[row]] = value;
            }
        }
        
        int presses = buttonPresses.Count(b => b);
        minPresses = Math.Min(minPresses, presses);
    }
    
    return minPresses;
}

long SolveMinimumJoltageLevelsPresses(Machine machine)
{
    int numCounters = machine.Joltage.Count;
    int numButtons = machine.Buttons.Count;
    
    // Create augmented matrix for Gaussian elimination
    double[,] matrix = new double[numCounters, numButtons + 1];
    
    // Fill matrix
    for (int button = 0; button < numButtons; button++)
    {
        foreach (int counter in machine.Buttons[button])
        {
            matrix[counter, button] = 1;
        }
    }
    
    // Set target state (last column)
    for (int counter = 0; counter < numCounters; counter++)
    {
        matrix[counter, numButtons] = machine.Joltage[counter];
    }
    
    // Gaussian elimination to reduced row echelon form
    int[] pivotCol = new int[numCounters];
    Array.Fill(pivotCol, -1);
    int currentRow = 0;
    
    const double EPSILON = 1e-10;
    
    for (int col = 0; col < numButtons && currentRow < numCounters; col++)
    {
        // Find pivot row (largest absolute value for numerical stability)
        int pivotRow = -1;
        double maxVal = 0;
        for (int row = currentRow; row < numCounters; row++)
        {
            if (Math.Abs(matrix[row, col]) > maxVal)
            {
                maxVal = Math.Abs(matrix[row, col]);
                pivotRow = row;
            }
        }
        
        if (maxVal < EPSILON) continue;
        
        // Swap rows if needed
        if (pivotRow != currentRow)
        {
            for (int c = 0; c <= numButtons; c++)
            {
                (matrix[currentRow, c], matrix[pivotRow, c]) = (matrix[pivotRow, c], matrix[currentRow, c]);
            }
        }
        
        pivotCol[currentRow] = col;
        double pivot = matrix[currentRow, col];
        
        // Normalize pivot row
        for (int c = 0; c <= numButtons; c++)
        {
            matrix[currentRow, c] /= pivot;
        }
        
        // Eliminate other rows
        for (int row = 0; row < numCounters; row++)
        {
            if (row != currentRow && Math.Abs(matrix[row, col]) > EPSILON)
            {
                double factor = matrix[row, col];
                for (int c = 0; c <= numButtons; c++)
                {
                    matrix[row, c] -= factor * matrix[currentRow, c];
                }
            }
        }
        
        currentRow++;
    }
    
    // Find free variables
    List<int> freeVars = new();
    HashSet<int> pivotCols = new(pivotCol.Where(p => p != -1));
    for (int col = 0; col < numButtons; col++)
    {
        if (!pivotCols.Contains(col))
        {
            freeVars.Add(col);
        }
    }
    
    // Maximum value any free variable can take
    long maxJoltage = machine.Joltage.Max();
    
    // Try all combinations of free variables (each can be 0 to maxJoltage)
    long minPresses = long.MaxValue;
    
    void EnumerateFreeVars(int freeVarIndex, long[] buttonPresses)
    {
        if (freeVarIndex == freeVars.Count)
        {
            // Calculate dependent variables from free variables
            long[] solution = (long[])buttonPresses.Clone();
            
            for (int row = 0; row < numCounters; row++)
            {
                if (pivotCol[row] != -1)
                {
                    double value = matrix[row, numButtons];
                    for (int col = 0; col < numButtons; col++)
                    {
                        if (col != pivotCol[row])
                        {
                            value -= matrix[row, col] * solution[col];
                        }
                    }
                    
                    long intValue = (long)Math.Round(value);
                    solution[pivotCol[row]] = intValue;
                    
                    // Check if solution is valid (non-negative and close to expected value)
                    if (intValue < 0 || Math.Abs(value - intValue) > 0.5)
                    {
                        return;
                    }
                }
            }
            
            // Verify the solution actually works
            long[] verify = new long[numCounters];
            for (int button = 0; button < numButtons; button++)
            {
                foreach (int counter in machine.Buttons[button])
                {
                    verify[counter] += solution[button];
                }
            }
            
            bool valid = true;
            for (int i = 0; i < numCounters; i++)
            {
                if (verify[i] != machine.Joltage[i])
                {
                    valid = false;
                    break;
                }
            }
            
            if (valid)
            {
                long totalPresses = solution.Sum();
                minPresses = Math.Min(minPresses, totalPresses);
            }
            return;
        }
        
        // Try all values for this free variable
        int freeVar = freeVars[freeVarIndex];
        for (long val = 0; val <= maxJoltage; val++)
        {
            buttonPresses[freeVar] = val;
            EnumerateFreeVars(freeVarIndex + 1, buttonPresses);
        }
    }
    
    EnumerateFreeVars(0, new long[numButtons]);
    
    return minPresses;
}

Machine ParseMachine(string line)
{
    // Extract indicators [.##.]
    var indicatorsMatch = Regex.Match(line, @"\[([.#]+)\]");
    var indicators = indicatorsMatch.Groups[1].Value
        .Select(c => c == '#')
        .ToList();

    // Extract buttons (1,3) (2) etc.
    var buttonMatches = Regex.Matches(line, @"\(([0-9,]+)\)");
    var buttons = buttonMatches
        .Select(m => m.Groups[1].Value.Split(',').Select(int.Parse).ToList())
        .ToList();

    // Extract joltage {3,5,4,7}
    var joltageMatch = Regex.Match(line, @"\{([0-9,]+)\}");
    var joltage = joltageMatch.Groups[1].Value
        .Split(',')
        .Select(int.Parse)
        .ToList();

    return new Machine
    {
        Indicators = indicators,
        Buttons = buttons,
        Joltage = joltage
    };
}

class Machine
{
    public List<bool> Indicators { get; set; }
    public List<List<int>> Buttons { get; set; } = new();
    public List<int> Joltage { get; set; } = new();
}
