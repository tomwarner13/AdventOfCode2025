using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day1;

public class Day1Problems : Problems
{
  protected override string TestInput => @"3   4
4   3
2   5
1   3
3   9
3   3";

  protected override int Day => 1;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var distanceSum = 0;
    var firstLocs = new List<int>();
    var secondLocs = new List<int>();

    foreach (var line in input)
    {
      var ints = StringUtils.ExtractIntsFromString(line).ToArray();
      firstLocs.Add(ints[0]);
      secondLocs.Add(ints[1]);
    }
    
    firstLocs.Sort();
    secondLocs.Sort();

    for (var i = 0; i < firstLocs.Count; i++)
    {
      distanceSum += Math.Abs(firstLocs[i] - secondLocs[i]);
    }
    return distanceSum.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var similarity = 0;
    var firstLocs = new List<int>();
    var secondLocsByCount = new Dictionary<int, int>();

    foreach (var line in input)
    {
      var ints = StringUtils.ExtractIntsFromString(line).ToArray();
      firstLocs.Add(ints[0]);
      var secondLoc = ints[1];
      if (secondLocsByCount.ContainsKey(secondLoc))
      {
        secondLocsByCount[secondLoc]++;
      }
      else
      {
        secondLocsByCount[secondLoc] = 1;
      }
    }

    foreach (var loc in firstLocs)
    {
      if (secondLocsByCount.ContainsKey(loc))
      {
        similarity += secondLocsByCount[loc] * loc;
      }
    }

    return similarity.ToString();
  }
}