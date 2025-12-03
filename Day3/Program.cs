using AoCToolbox;

Console.WriteLine("-- Day 3: Lobby --");

var batteryBanks = File.ReadAllLines("Input.txt");

Console.WriteLine($"Part 1: {Part1(batteryBanks)}");
Console.WriteLine($"Part 2: {Part2(batteryBanks)}");

static long Part1(string[] batteryBanks)
{
    long totalJoltage = 0;

    foreach (var batteryBank in batteryBanks)
    {
        var banks = batteryBank.Select(c => c - '0').ToList();

        var largestNumber = banks.GetLargestTwoDigitsInOrder();

        totalJoltage += largestNumber;
    }

    return totalJoltage;
}

static long Part2(string[] batteryBanks)
{
    long totalJoltage = 0;
    foreach (var batteryBank in batteryBanks)
    {
        var banks = batteryBank.Select(c => c - '0').ToList();
        var largestNumber = banks.GetLargestNDigitsInOrder(12);
        totalJoltage += largestNumber;
    }
    return totalJoltage;
}

