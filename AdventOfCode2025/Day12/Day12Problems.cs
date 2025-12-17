namespace AdventOfCode2025.Day12;

using Util;

public class Day12Problems : Problems
{
  protected override int Day => 12;
  
  protected override string TestInput => "can we get a new guy to make AoC next year?";

  // https://www.reddit.com/r/adventofcode/comments/1pkjynl/2025_day_12_day_12_solutions/
  public override string Problem1(string[] input, bool isTestInput)
  {
    var regionsFitting = 0;

    foreach (var line in input)
    {
      var ints = StringUtils.ExtractIntsFromString(line).ToArray();
      
      if(ints.Length < 3) continue;
      
      var area = ints[0] * ints[1];
      
      var totalShapesRequired = ints[2..].Sum();
      
      regionsFitting += area >= (totalShapesRequired * 8) ? 1 : 0;
    }
    
    return regionsFitting.ToString();
  }

  public override string Problem2(string[] input, bool isTestInput)
  {
    throw new NotImplementedException();
  }
}