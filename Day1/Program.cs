using AoCToolbox;

Console.WriteLine("--- Day1: Secret Entrance ---");

var turns = File.ReadAllLines("input.txt");

int numberOfTimeOnZero = 0;
int current = 50; // dial starts at 50
int numberOfTimesZeroIsPassed = 0;

foreach (var turn in turns)
{
    if (string.IsNullOrWhiteSpace(turn)) continue;

    char dir = turn[0];
    int clicks = int.Parse(turn[1..]); // support multi-digit

    // Count how many clicks in this move land exactly on 0.
    // For R: hits at k ≡ -current (mod 100); first positive hit is fR = (100 - current) % 100, with 0 -> 100.
    // For L: hits at k ≡  current (mod 100); first positive hit is fL = current % 100, with 0 -> 100.
    int firstHit = dir == 'R'
        ? (100 - (current % 100)) % 100
        : (current % 100);

    if (firstHit == 0) firstHit = 100; // first positive click that lands on 0

    int zerosThisTurn = clicks >= firstHit ? 1 + (clicks - firstHit) / 100 : 0;
    numberOfTimesZeroIsPassed += zerosThisTurn;

    // Apply the rotation to update the dial position
    int move = clicks % 100;
    if (dir == 'R')
    {
        current = (current + move) % 100;
    }
    else if (dir == 'L')
    {
        current = (current - move) % 100;
        if (current < 0) current += 100;
    }
    else
    {
        throw new InvalidOperationException($"Invalid direction '{dir}' in input '{turn}'.");
    }

    // Part 1: end-of-rotation landing on 0
    if (current == 0)
    {
        numberOfTimeOnZero++;
    }
}

Console.WriteLine($"Part 1 (end positions at 0): {numberOfTimeOnZero}");
Console.WriteLine($"Part 2 (method 0x434C49434B): {numberOfTimesZeroIsPassed}");

