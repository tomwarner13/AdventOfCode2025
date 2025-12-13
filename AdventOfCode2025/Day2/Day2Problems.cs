namespace AdventOfCode2025.Day2;

using Util;

public class Day2Problems : Problems
{
  protected override int Day => 2;

  protected override string TestInput => 
    "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124";

  public override string Problem1(string[] input, bool isTestInput)
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

  public override string Problem2(string[] input, bool isTestInput)
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

      var nextInvalid = GetNextInvalidIdAnyPattern(min - 1); //inclusive!
      while (nextInvalid <= max)
      {
        D(nextInvalid);
        invalidIds += nextInvalid;
        nextInvalid = GetNextInvalidIdAnyPattern(nextInvalid);
      }
    }
    
    return invalidIds.ToString();
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

  private static long GetNextInvalidIdAnyPattern(long input)
  {
    if (input < 11) return 11; //the first possible ID

    var inputStr = input.ToString();
    if (inputStr.All(c => c == '9')) //if all 9s, add one to roll over to correct number of digits
    {
      input += 1;
      inputStr = input.ToString();
    }

    var possibleNextIds = new List<long>();

    var inputLen = inputStr.Length;
    for (var segments = 2; segments <= inputLen; segments++)
    {
      if (inputLen % segments == 0)
      {
        possibleNextIds.Add(GenerateInvalidAtMaskLength(input, segments));
      }
    }

    if (possibleNextIds.Count == 0) throw new ThisShouldNeverHappenException($"no next IDs! input: {input}");
    return possibleNextIds.Min();
  }

  private static long GenerateInvalidAtMaskLength(long input, int segments)
  {
    var inputStr = input.ToString();
    var mask = inputStr[..(inputStr.Length / segments)];
    
    var maskApplied = string.Concat(Enumerable.Repeat(mask, segments));
    var maskAppliedLong = long.Parse(maskApplied);
    if (maskAppliedLong > input) return maskAppliedLong;

    var maskPlus = long.Parse(mask) + 1;
    maskApplied = string.Concat(Enumerable.Repeat(maskPlus, segments));
    maskAppliedLong = long.Parse(maskApplied);
    if (maskAppliedLong > input) return maskAppliedLong;

    throw new ThisShouldNeverHappenException($"mAL: {maskAppliedLong} <= input: {inputStr}");
  }
}