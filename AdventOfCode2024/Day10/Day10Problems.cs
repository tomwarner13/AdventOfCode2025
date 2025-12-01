using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day10;

public class Day10Problems : Problems
{
  protected override string TestInput => @"89010123
78121874
87430965
96549874
45678903
32019012
01329801
10456732";

  protected override int Day => 10;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var trailMap = new int[input.Length][];
    var trailHeads = new List<GridPoint>();

    for (var y = 0; y < input.Length; y++)
    {
      trailMap[y] = new int[input[y].Length];
      for (var x = 0; x < input[y].Length; x++)
      {
        var curElevation = int.Parse(input[y][x].ToString());
        trailMap[y][x] = curElevation;
        if(curElevation == 0)
          trailHeads.Add(new GridPoint(x, y));
      }
    }

    var totalTrails = 0;

    foreach (var head in trailHeads)
    {
      totalTrails += GetUniqueDestinations(head, ref trailMap).Count;
    }
    
    return totalTrails.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var trailMap = new int[input.Length][];
    var trailHeads = new List<GridPoint>();

    for (var y = 0; y < input.Length; y++)
    {
      trailMap[y] = new int[input[y].Length];
      for (var x = 0; x < input[y].Length; x++)
      {
        var curElevation = int.Parse(input[y][x].ToString());
        trailMap[y][x] = curElevation;
        if(curElevation == 0)
          trailHeads.Add(new GridPoint(x, y));
      }
    }

    var totalTrails = 0;

    foreach (var head in trailHeads)
    {
      totalTrails += GetUniquePathCount(head, ref trailMap);
    }
    
    return totalTrails.ToString();
  }

  private static HashSet<GridPoint> GetUniqueDestinations(GridPoint trail, ref int[][] trailMap, int prevElevation = -1)
  {
    if (!trail.IsInBounds(trailMap[0].Length, trailMap.Length)) return new HashSet<GridPoint>();
    
    var curElevation = trailMap[trail.Y][trail.X];
    if (curElevation - 1 != prevElevation) return new HashSet<GridPoint>();
    if (curElevation == 9) return new HashSet<GridPoint> { trail };

    var result =  GetUniqueDestinations(trail + GridPoint.Up, ref trailMap, curElevation);
    result.UnionWith(GetUniqueDestinations(trail + GridPoint.Down, ref trailMap, curElevation));
    result.UnionWith(GetUniqueDestinations(trail + GridPoint.Left, ref trailMap, curElevation));
    result.UnionWith(GetUniqueDestinations(trail + GridPoint.Right, ref trailMap, curElevation));
    
    return result;
  }
  
  
  private static int GetUniquePathCount(GridPoint trail, ref int[][] trailMap, int prevElevation = -1)
  {
    if (!trail.IsInBounds(trailMap[0].Length, trailMap.Length)) return 0;
    
    var curElevation = trailMap[trail.Y][trail.X];
    if (curElevation - 1 != prevElevation) return 0;
    if (curElevation == 9) return 1;

    return GetUniquePathCount(trail + GridPoint.Up, ref trailMap, curElevation) +
    GetUniquePathCount(trail + GridPoint.Down, ref trailMap, curElevation) +
    GetUniquePathCount(trail + GridPoint.Left, ref trailMap, curElevation) +
    GetUniquePathCount(trail + GridPoint.Right, ref trailMap, curElevation);
  }
}