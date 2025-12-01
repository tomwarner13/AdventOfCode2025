using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day11;

public class Day11Problems : Problems
{
  protected override string TestInput => @"125 17";

  protected override int Day => 11;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var resultsCache = new Dictionary<LongPoint, long>();

    var line = input[0];
    var startNumbers = StringUtils.ExtractIntsFromString(line);
    var total = 0L;
    const int blinks = 25;

    foreach (var startNumber in startNumbers)
    { 
      total += BlinkForNumber(startNumber, blinks, ref resultsCache);
    }

    return total.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var resultsCache = new Dictionary<LongPoint, long>();

    var line = input[0];
    var startNumbers = StringUtils.ExtractIntsFromString(line);
    var total = 0L;
    const int blinks = 75;

    foreach (var startNumber in startNumbers)
    { 
      total += BlinkForNumber(startNumber, blinks, ref resultsCache);
    }

    return total.ToString();
  }

  private static long BlinkForNumber(long curNumber, int blinksRemaining, ref Dictionary<LongPoint, long> resultsCache)
  {
    if (blinksRemaining == 0) return 1;
    
    var curIndex = new LongPoint(curNumber, blinksRemaining);
    if(resultsCache.TryGetValue(curIndex, out var answer)) return answer;

    if (curNumber == 0)
    {
      var zeroResult = BlinkForNumber(1, blinksRemaining - 1, ref resultsCache);
      resultsCache[curIndex] = zeroResult;
      return zeroResult;
    }
    
    var strNum = curNumber.ToString();
    if (strNum.Length % 2 == 0) //split by digits
    {
      var firstHalf = strNum[..(strNum.Length / 2)];
      var secondHalf = strNum[(strNum.Length / 2)..];
      var firstNum = int.Parse(firstHalf);
      var secondNum = int.Parse(secondHalf);
      
      var splitResult = BlinkForNumber(firstNum, blinksRemaining - 1, ref resultsCache) + 
                        BlinkForNumber(secondNum, blinksRemaining - 1, ref resultsCache);
      resultsCache[curIndex] = splitResult;
      return splitResult;
    }
    
    var defaultResult = BlinkForNumber(curNumber * 2024, blinksRemaining - 1, ref resultsCache);
    resultsCache[curIndex] = defaultResult;
    return defaultResult;
  }
}