using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day5;

public class Day5Problems : Problems
{
  protected override string TestInput => @"47|53
97|13
97|61
97|47
75|29
61|13
75|53
29|13
97|29
53|29
61|53
97|53
61|29
47|13
75|47
97|75
47|61
75|61
47|29
75|13
53|13

75,47,61,53,29
97,61,53,29,13
75,29,13
75,97,47,61,53
61,13,29
97,13,75,29,47";

  protected override int Day => 5;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var accumulator = 0;
    var rules = new Dictionary<int, List<int>>();
    var parsingRules = true;

    foreach (var line in input)
    {
      if (parsingRules)
      {
        if (string.IsNullOrWhiteSpace(line))
        {
          parsingRules = false;
        }
        else
        {
          var ints = StringUtils.ExtractIntsFromString(line).ToArray();

          if (rules.ContainsKey(ints[1]))
          {
            rules[ints[1]].Add(ints[0]);
          }
          else
          {
            rules.Add(ints[1], new List<int> { ints[0] });
          }
        }
      }
      else
      {
        var ints = StringUtils.ExtractIntsFromString(line).ToArray();
        accumulator += ReturnMiddleNumberIfRulesValidate(ints, rules);
      }
    }
    
    return accumulator.ToString();
  }
  
  protected override string Problem2(string[] input, bool isTestInput)
  {    
    var accumulator = 0;
    var rules = new Dictionary<int, List<int>>();
    var parsingRules = true;

    foreach (var line in input)
    {
      if (parsingRules)
      {
        if (string.IsNullOrWhiteSpace(line))
        {
          parsingRules = false;
        }
        else
        {
          var ints = StringUtils.ExtractIntsFromString(line).ToArray();

          if (rules.ContainsKey(ints[1]))
          {
            rules[ints[1]].Add(ints[0]);
          }
          else
          {
            rules.Add(ints[1], new List<int> { ints[0] });
          }
        }
      }
      else
      {
        var ints = StringUtils.ExtractIntsFromString(line).ToArray();
        accumulator += ReturnMiddleNumberOrAdjustRules(ints, rules);
      }
    }
    
    return accumulator.ToString();
  }

  private static int ReturnMiddleNumberIfRulesValidate(int[] ints, Dictionary<int, List<int>> rules)
  {
    var invalidNums = new HashSet<int>();

    foreach (var num in ints)
    {
      if (invalidNums.Contains(num))
      {
        return 0;
      }

      if(rules.TryGetValue(num, out var rule)) invalidNums.UnionWith(rule);
    }
    
    return ints[ints.Length / 2];
  }
  
  private static int ReturnMiddleNumberOrAdjustRules(int[] ints, Dictionary<int, List<int>> rules, bool isFirstAttempt = true)
  {
    var invalidNums = new HashSet<int>();
    var index = 0;

    foreach (var num in ints)
    {
      if (invalidNums.Contains(num))
      {
        //swap current index with index before
        (ints[index], ints[index - 1]) = (ints[index - 1], ints[index]);
        return ReturnMiddleNumberOrAdjustRules(ints, rules, false);
      }

      if(rules.TryGetValue(num, out var rule)) invalidNums.UnionWith(rule);
      index++;
    }
    
    return isFirstAttempt ? 0 : ints[ints.Length / 2];
  }
}