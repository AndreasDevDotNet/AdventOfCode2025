Console.WriteLine("-- Day 2: Gift Shop --");

var invalidIdRanges = File.ReadAllText("input.txt")
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .Select(range =>
    {
        var parts = range.Split('-', StringSplitOptions.RemoveEmptyEntries);
        return (Start: long.Parse(parts[0]), End: long.Parse(parts[1]));
    })
    .ToList();

Console.WriteLine($"Part 1 (sum of invalid IDs): {CalculatePart1(invalidIdRanges)}");
Console.WriteLine($"Part 2 (sum of invalid IDs, repeated blocks >= 2): {CalculatePart2(invalidIdRanges)}");

static long CalculatePart1(List<(long Start, long End)> invalidIdRanges)
{
    var powersOf10 = new long[20];
    powersOf10[0] = 1;
    for (int i = 1; i < powersOf10.Length; i++)
        powersOf10[i] = powersOf10[i - 1] * 10;

    HashSet<long> invalidIds = new(); // ensure uniqueness if ranges ever overlap

    for (int h = 1; h <= 9; h++) // total length 2h up to 18 digits (fits in long)
    {
        long multiplierPlusOne = powersOf10[h] + 1; // 10^h + 1
        long minBaseByLength = powersOf10[h - 1];
        long maxBaseByLength = powersOf10[h] - 1;

        foreach (var (start, end) in invalidIdRanges)
        {
            long baseMin = (start + multiplierPlusOne - 1) / multiplierPlusOne; // ceil
            long baseMax = end / multiplierPlusOne;

            if (baseMax < minBaseByLength || baseMin > maxBaseByLength)
                continue;

            if (baseMin < minBaseByLength) baseMin = minBaseByLength;
            if (baseMax > maxBaseByLength) baseMax = maxBaseByLength;

            for (long baseVal = baseMin; baseVal <= baseMax; baseVal++)
            {
                long repeated = baseVal * multiplierPlusOne;
                if (repeated >= start && repeated <= end)
                {
                    invalidIds.Add(repeated);
                }
            }
        }
    }

    return invalidIds.Sum();
}

static long CalculatePart2(List<(long Start, long End)> invalidIdRanges)
{
    // Precompute powers of 10 up to 19 digits (long max is 9,223,372,036,854,775,807 ~ 19 digits)
    var powersOf10 = new long[20];
    powersOf10[0] = 1;
    for (int i = 1; i < powersOf10.Length; i++)
        powersOf10[i] = powersOf10[i - 1] * 10;

    HashSet<long> invalidIds = new();

    // m = number of digits in base (no leading zero), k = repetition count (>=2)
    for (int m = 1; m <= 9; m++)
    {
        long minBaseByLength = powersOf10[m - 1];
        long maxBaseByLength = powersOf10[m] - 1;

        // Determine max k such that total digits m*k fits in 19 (safe for long)
        int maxK = Math.Min(19 / m, 19); // cap at 19 digits
        for (int k = 2; k <= maxK; k++)
        {
            int totalDigits = m * k;
            if (totalDigits > 19) break;

            // multiplier = (10^(m*k) - 1) / (10^m - 1)
            // This is the number formed by k blocks of m digits, all 1s in block terms.
            long numerator = powersOf10[totalDigits] - 1;
            long denominator = powersOf10[m] - 1;
            long multiplier = numerator / denominator; // exact division

            foreach (var (start, end) in invalidIdRanges)
            {
                // base range by the numeric constraints
                long baseMin = (start + multiplier - 1) / multiplier; // ceil(start / multiplier)
                long baseMax = end / multiplier;

                if (baseMax < minBaseByLength || baseMin > maxBaseByLength)
                    continue;

                if (baseMin < minBaseByLength) baseMin = minBaseByLength;
                if (baseMax > maxBaseByLength) baseMax = maxBaseByLength;

                for (long baseVal = baseMin; baseVal <= baseMax; baseVal++)
                {
                    long repeated = baseVal * multiplier;
                    if (repeated >= start && repeated <= end)
                    {
                        invalidIds.Add(repeated);
                    }
                }
            }
        }
    }

    return invalidIds.Sum();
}