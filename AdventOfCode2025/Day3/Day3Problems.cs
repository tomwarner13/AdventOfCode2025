namespace AdventOfCode2025.Day3;

using Util;

public class Day3Problems : Problems
{
  protected override int Day => 3;

  protected override string TestInput => @"987654321111111
811111111111119
234234234234278
818181911112111";

  public override string Problem1(string[] input, bool isTestInput)
  {
    DebugMode = isTestInput;
    var joltageSum = 0L;

    foreach (var line in input)
    {
      var numbers = line.Select(c => int.Parse(c.ToString())).ToArray();
      var joltageRating = GetJoltageRating(numbers);
      D(joltageRating);
      joltageSum += joltageRating;
    }
    
    return joltageSum.ToString();
  }

  private static int GetJoltageRating(int[] numbers)
  {
    var firstIndex = 0;
    var firstDigit = 0;
    for (var i = 0; i < numbers.Length - 1; i++)
    {
      if (numbers[i] > firstDigit)
      {
        firstDigit = numbers[i];
        firstIndex = i;
      }
    }
    
    var remaining = numbers[(firstIndex + 1)..];

    var secondDigit = remaining.Max();

    return (10 * firstDigit) + secondDigit;
  }
  
  public override string Problem2(string[] input, bool isTestInput)
  {
    DebugMode = isTestInput;
    var joltageSum = 0L;

    foreach (var line in input)
    {
      var numbers = line.Select(c => int.Parse(c.ToString())).ToArray();
      var joltageRating = GetJoltageRatingRecursive(numbers, 12);
      D(joltageRating);
      joltageSum += joltageRating;
    }
    
    return joltageSum.ToString();
  }

  private static long GetJoltageRatingRecursive(int[] numbers, int placesRemaining)
  {
    if (placesRemaining == 0) return 0;
    
    var remainingSignificance = placesRemaining - 1;
    
    var firstIndex = 0;
    var firstDigit = 0;
    for (var i = 0; i < numbers.Length - remainingSignificance; i++)
    {
      if (numbers[i] > firstDigit)
      {
        firstDigit = numbers[i];
        firstIndex = i;
      }
    }
    
    var remaining = numbers[(firstIndex + 1)..];
    var secondDigit = GetJoltageRatingRecursive(remaining, remainingSignificance);
    
    return ((long)Math.Pow(10, remainingSignificance) * firstDigit) + secondDigit;
  }
}