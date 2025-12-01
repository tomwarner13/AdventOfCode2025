using System.Net;
using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day12;

public class Day12Problems : Problems
{
  protected override string TestInput => @"RRRRIICCFF
RRRRIICCCF
VVRRRCCFFF
VVRCCCJFFF
VVVVCJJCFE
VVIVCCJJEE
VVIIICJJEE
MIIIIIJJEE
MIIISIJEEE
MMMISSJEEE";

  protected override int Day => 12;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var visitedPoints = new HashSet<GridPoint>();
    var totalPrice = 0L;

    for (var y = 0; y < input.Length; y++)
    {
      for (var x = 0; x < input[y].Length; x++)
      {
        var point = new GridPoint(x, y);
        if (!visitedPoints.Contains(point))
        {
          var region = CalculateRegion(input[y][x], point, ref input, ref visitedPoints);
          totalPrice += region.perimeter * region.area;
        }
      }
    }
    
    return totalPrice.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var visitedPoints = new HashSet<GridPoint>();
    var totalPrice = 0L;

    for (var y = 0; y < input.Length; y++)
    {
      for (var x = 0; x < input[y].Length; x++)
      {
        var point = new GridPoint(x, y);
        if (!visitedPoints.Contains(point))
        {
          var seed = input[y][x];
          var pointsInRegion  = GetUniquePointsInRegion(seed, point, ref input, ref visitedPoints);
          
          var orderedPoints = pointsInRegion.OrderBy(p => p.Y).ThenBy(p => p.X).ToList();

          var sides = CountSidesOfRegion(orderedPoints);
          totalPrice += sides * pointsInRegion.Count;
        }
      }
    }
    
    return totalPrice.ToString();
  }

  private static (int perimeter, int area) CalculateRegion(char seed, GridPoint point, ref string[] input,
    ref HashSet<GridPoint> visitedPoints)
  {
    if (!point.IsInBounds(input[0].Length, input.Length) || input[point.Y][point.X] != seed) return (1, 0);
    
    if (!visitedPoints.Add(point)) return (0, 0);

    var perimeter = 0;
    var area = 1;

    foreach (var dir in GridPoint.CardinalDirections)
    {
      var result = CalculateRegion(seed, point + dir, ref input, ref visitedPoints);
      perimeter += result.perimeter;
      area += result.area;
    }
    
    return (perimeter, area);
  }

  private static List<GridPoint> GetUniquePointsInRegion(char seed, GridPoint point, ref string[] input,
    ref HashSet<GridPoint> visitedPoints)
  {
    if (!point.IsInBounds(input[0].Length, input.Length) || input[point.Y][point.X] != seed)
      return new List<GridPoint>();

    if (!visitedPoints.Add(point)) return new List<GridPoint>();

    var result = new List<GridPoint>{ point };
    foreach (var dir in GridPoint.CardinalDirections)
    {
      result.AddRange(GetUniquePointsInRegion(seed, point + dir, ref input, ref visitedPoints));
    }

    return result;
  }

  private static int CountSidesOfRegion(List<GridPoint> orderedRegion)
  {
    var sidesByPoint = orderedRegion.ToDictionary(p => p, p => new HashSet<GridPoint>());
    var uniqueSides = 0;

    foreach (var point in orderedRegion)
    {
      foreach(var verticalDir in new List<GridPoint>{GridPoint.Up, GridPoint.Down})
      {
        var neighbor = point + verticalDir;
        if (!sidesByPoint.ContainsKey(neighbor))
        {
          //potential side, check if the point to the left is a neighbor and has a corresponding side
          sidesByPoint[point].Add(verticalDir);
          var leftNeighbor = point + GridPoint.Left;
          if (!sidesByPoint.ContainsKey(leftNeighbor) || !sidesByPoint[leftNeighbor].Contains(verticalDir))
            uniqueSides++;
        }
      }
      
      foreach(var horizontalDir in new List<GridPoint>{GridPoint.Left, GridPoint.Right})
      {
        var neighbor = point + horizontalDir;
        if (!sidesByPoint.ContainsKey(neighbor))
        {
          //potential side, check if the point above is a neighbor and has a corresponding side
          sidesByPoint[point].Add(horizontalDir);
          var upNeighbor = point + GridPoint.Up;
          if (!sidesByPoint.ContainsKey(upNeighbor) || !sidesByPoint[upNeighbor].Contains(horizontalDir))
            uniqueSides++;
        }
      }
    }
    
    return uniqueSides;
  }
}