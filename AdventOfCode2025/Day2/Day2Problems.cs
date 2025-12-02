using System.Text;
using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day2;

public class Day2Problems : Problems
{
  protected override string TestInput => 
    "824824821-824824827,11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124";

  protected override int Day => 2;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    DebugMode = isTestInput;
    var invalidIds = 0L;
    
    var inputRanges = input[0].Split(',');
    foreach (var range in inputRanges)
    {
      D(range);
      var ends = StringUtils.ExtractLongsFromString(range).ToArray();
      var min = ends[0];
      var max = ends[1];

      var nextInvalid = GetNextInvalidId(min - 1); //inclusive!
      while (nextInvalid <= max)
      {
        D(nextInvalid);
        invalidIds += nextInvalid;
        nextInvalid = GetNextInvalidId(nextInvalid);
      }
    }
    
    return invalidIds.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    throw new NotImplementedException();
  }

  private static long GetNextInvalidId(long input)
  {
    var idStr = input.ToString();
    if (idStr.Length % 2 != 0)
    {
      var zeroes = idStr.Length / 2;
      var numPart = "1".PadRight(zeroes + 1, '0'); //padding includes first char
      return long.Parse(numPart + numPart);
    }

    var firstHalf = long.Parse(idStr[..(idStr.Length / 2)]);
    var secondHalf = long.Parse(idStr[(idStr.Length / 2)..]);

    return long.Parse(firstHalf > secondHalf ? 
      $"{firstHalf}{firstHalf}" : 
      $"{firstHalf + 1}{firstHalf + 1}");
  }
}