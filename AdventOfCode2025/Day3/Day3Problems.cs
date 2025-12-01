using System.Text.RegularExpressions;
using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day3;

public class Day3Problems : Problems
{
  private static readonly Regex MultiplierRegex = new(@"mul\(\d+,\d+\)", RegexOptions.Compiled);
  private static readonly Regex InstructionRegex = new(@"mul\(\d+,\d+\)|do\(\)|don't\(\)", RegexOptions.Compiled);
  
  protected override string TestInput => @"xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";

  protected override int Day => 3;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var total = 0;
    foreach (var line in input)
    {
      var multipliers = MultiplierRegex.Matches(line);
      foreach (Match match in multipliers)
      {
        var nums = StringUtils.ExtractIntsFromString(match.Value).ToArray();
        total += nums[0] * nums[1];
      }
    }
    return total.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    //test input for problem 2 is different, annoyingly enough
    if (isTestInput)
    {
      input = new [] { "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))" };
    }
    
    var total = 0;
    var instructionsOn = true;
    foreach (var line in input)
    {
      var instructions = InstructionRegex.Matches(line);
      foreach (Match match in instructions)
      {
        var stringVal = match.Value;
        if (stringVal == "don't()")
        {
          instructionsOn = false;
        }
        else if (stringVal == "do()")
        {
          instructionsOn = true;
        }
        else
        {
          if (instructionsOn)
          {
            var nums = StringUtils.ExtractIntsFromString(match.Value).ToArray();
            total += nums[0] * nums[1];
          }
        }
      }
    }
    return total.ToString();
  }
}