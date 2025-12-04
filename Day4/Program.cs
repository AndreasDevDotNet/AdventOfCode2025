Console.WriteLine("--- Dayt 4: Printing Department ---");

var paperGrid = File.ReadAllLines("Input.txt");

var forkliftRolls = 0;

for (int r = 0; r < paperGrid.Length; r++)
{
    for (int c = 0; c < paperGrid[r].Length; c++)
    {
        if(paperGrid[r][c] == '@')
        {
            if (CanBeAccessedByForklift(r, c, paperGrid))
            {
                forkliftRolls++;
            }
        }
    }
}

Console.WriteLine($"Part 1: {forkliftRolls}");

// Part 2: Keep removing rolls until no more can be accessed
var removedRolls = new HashSet<(int row, int col)>();
bool foundNewRolls = true;

while (foundNewRolls)
{
    foundNewRolls = false;
    var rollsToRemove = new List<(int row, int col)>();
    
    for (int r = 0; r < paperGrid.Length; r++)
    {
        for (int c = 0; c < paperGrid[r].Length; c++)
        {
            if (paperGrid[r][c] == '@' && !removedRolls.Contains((r, c)))
            {
                if (CanBeAccessedByForklift(r, c, paperGrid, removedRolls))
                {
                    rollsToRemove.Add((r, c));
                    foundNewRolls = true;
                }
            }
        }
    }
    
    foreach (var roll in rollsToRemove)
    {
        removedRolls.Add(roll);
    }
}

Console.WriteLine($"Part 2: {removedRolls.Count}");

static bool CanBeAccessedByForklift(int row, int col, string[] grid, HashSet<(int row, int col)>? removedRolls = null)
{
    int numberOfRolls = 0;

    foreach (var dir in GetAdjecentPositions(row, col, grid))
    {
        int newRow = row + dir.dr;
        int newCol = col + dir.dc;
        if (newRow >= 0 && newRow < grid.Length &&
            newCol >= 0 && newCol < grid[newRow].Length)
        {
            if (removedRolls != null && removedRolls.Contains((newRow, newCol)))
            {
                continue;
            }
            if (grid[newRow][newCol] == '@')
            {
                numberOfRolls++;
            }
        }
    }

    return numberOfRolls < 4;
}

static List<(int dr, int dc)> GetAdjecentPositions(int row, int col, string[] grid)
{
    var directions = new List<(int dr, int dc)>
    {
        (-1, -1), // Up left
        (-1, 1),  // Up right
        (1, -1),  // Down left
        (1, 1),   // Down right
        (-1, 0), // Up
        (1, 0),  // Down
        (0, -1), // Left
        (0, 1)   // Right
    };

    return directions;
}