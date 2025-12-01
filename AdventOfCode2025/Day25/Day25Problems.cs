using System.Text;
using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day25;

public class Day25Problems : Problems
{
  protected override string TestInput => @"#####
.####
.####
.####
.#.#.
.#...
.....

#####
##.##
.#.##
...##
...#.
...#.
.....

.....
#....
#....
#...#
#.#.#
#.###
#####

.....
.....
#.#..
###..
###.#
###.#
#####

.....
.....
.....
#....
#.#..
#.#.#
#####";

  protected override int Day => 25;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var locks = new List<int[]>();
    var keys = new List<int[]>();

    for (var baseIndex = 0; baseIndex < input.Length; baseIndex += 8)
    {
      //check if lock or key
      if (input[baseIndex][0] == '#') //lock
      {
        var buildingLock = new int[5];
        Array.Fill(buildingLock, 5);
        for (var height = 0; height <= 5; height++)
        {
          for (var pin = 0; pin < 5; pin++)
          {
            if(input[baseIndex + height + 1][pin] == '.')
              buildingLock[pin] = Math.Min(buildingLock[pin], height);
          }
        }
        locks.Add(buildingLock);
      }
      else //key
      {
        var buildingKey = new int[5];
        Array.Fill(buildingKey, 5);
        for (var height = 0; height <= 5; height++)
        {
          for (var pin = 0; pin < 5; pin++)
          {
            if(input[baseIndex + 5 - height][pin] == '.')
              buildingKey[pin] = Math.Min(buildingKey[pin], height);
          }
        }
        keys.Add(buildingKey); //check if i work on keys
      }
    }

    var totalMatches = 0;
    foreach (var key in keys)
      foreach (var loch in locks) //lock is a keyword lmao
        if(CompareLockAndKey(key, loch)) totalMatches++;
    
    return totalMatches.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    return "Merry Christmas!! Not having to do a Problem 2 today is a present in itself";
  }

  private static bool CompareLockAndKey(int[] testLock, int[] testKey)
  {
    for(var i = 0; i < 5; i++)
      if(testLock[i] + testKey[i] > 5) return false;
    return true;
  }
}