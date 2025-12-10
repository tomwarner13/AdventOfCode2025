namespace AdventOfCode2025.Day9;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Util;
public class Day9Problems : Problems
{
  protected override int Day => 9;
  
  protected override string TestInput => @"7,1
11,1
11,7
9,7
9,5
2,5
2,3
7,3";

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var xBound = 0;
    var redTiles = input
      .Select(l =>
      {
        var coords = StringUtils.ExtractIntsFromString(l).ToArray();
        if(coords[0] > xBound) xBound = coords[0];
        return new GridPoint(coords[0], coords[1]);
      }).Order().ToArray();

    var maxArea = 0L;
    var yBound = redTiles.Last().Y;
    
    for (var i = 0; i < redTiles.Length; i++)
    {
      var remainingSearchArea = (1L + yBound - redTiles[i].Y) * (xBound + 1L);
      if (remainingSearchArea < maxArea) return maxArea.ToString();

      var lowerTile = redTiles[i];
      for (var upperIndex = redTiles.Length - 1; upperIndex > i; upperIndex--)
      {
        var upperTile = redTiles[upperIndex];
        var remainingStripeArea = (1L + upperTile.Y - lowerTile.Y) * (xBound + 1L);
        if(remainingStripeArea < maxArea) continue;
        
        var area = (1L + Math.Abs(upperTile.Y - lowerTile.Y)) * (1L + Math.Abs(upperTile.X - lowerTile.X));
        if (area > maxArea)
        {
          D($"new max area: {area} | {lowerTile} -> {upperTile}");
          maxArea = area;
        }
      }
    }

    return maxArea.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var xBound = 0;
    var yBound = 0;
    var redCorners = input
      .Select(l =>
      {
        var coords = StringUtils.ExtractIntsFromString(l).ToArray();
        if(coords[0] > xBound) xBound = coords[0];
        if(coords[1] > yBound) yBound = coords[1];
        return new GridPoint(coords[0], coords[1]);
      }).ToArray();
    
    //increase the grid size for drawing/calculation purposes
    xBound += 2;
    yBound += 2;
    
    D($"bounds: {xBound}, {yBound}");
    
    //var map = new BitArray(xBound * yBound);
    var boundaryLines = new HashSet<GridPoint>();
    var lowestPoint = new GridPoint(xBound, yBound);
    
    for(var i = 0; i < redCorners.Length; i++)
    {
      var point = redCorners[i];
      if(lowestPoint.X > point.X) lowestPoint = point;
      var nextPoint = i == redCorners.Length - 1 ? redCorners[0] : redCorners[i + 1];
      DrawLine(point, nextPoint, ref boundaryLines);
    }
    
    D($"lowest point: {lowestPoint}");
    PrintGrid(boundaryLines, xBound, yBound);
    
    var perimeter = DrawOuterPerimeter(lowestPoint + GridPoint.Left, ref boundaryLines);
    D("Perimeter:");
    PrintGrid(perimeter, xBound, yBound);
    
    //recurse through rectangles, check all edges if in outer perimeter, return area of biggest
    
    var redTiles = redCorners.Order().ToArray();
    var maxArea = 0L;
    
    for (var i = 0; i < redTiles.Length; i++)
    {
      var remainingSearchArea = (1L + yBound - redTiles[i].Y) * (xBound + 1L);
      if (remainingSearchArea < maxArea) return maxArea.ToString();

      var lowerTile = redTiles[i];
      for (var upperIndex = redTiles.Length - 1; upperIndex > i; upperIndex--)
      {
        var upperTile = redTiles[upperIndex];
        var remainingStripeArea = (1L + upperTile.Y - lowerTile.Y) * (xBound + 1L);
        if(remainingStripeArea < maxArea) continue;
        
        var area = (1L + Math.Abs(upperTile.Y - lowerTile.Y)) * (1L + Math.Abs(upperTile.X - lowerTile.X));
        if (area > maxArea)
        {
          D($"new potential max area: {area} | {lowerTile} -> {upperTile}");
          
          //check rectangle against grid of invalid spaces
          if (!GenerateRectanglePerimeter(lowerTile, upperTile).Any(p => perimeter.Contains(p)))
          {
            D($"new max area: {area} | {lowerTile} -> {upperTile}");
            maxArea = area;
          }
        }
      }
    }
    
    return maxArea.ToString();
  }

  private static void DrawLine(GridPoint p1, GridPoint p2, ref HashSet<GridPoint> boundaries)
  {
    try
    {
      if (p1.X == p2.X) //draw vertical
      {
        if (p1.Y > p2.Y) //down
        {
          for (var y = p1.Y; y >= p2.Y; y--)
          {
            boundaries.Add(new(p1.X, y));
          }
        }
        else //up
        {
          for (var y = p1.Y; y <= p2.Y; y++)
          {
            boundaries.Add(new(p1.X, y));
          }
        }
      }
      else if (p1.Y == p2.Y) //draw horizontal
      {
        if (p1.X > p2.X) //left
        {
          for (var x = p1.X; x >= p2.X; x--)
          {
            boundaries.Add(new(x, p1.Y));
          }
        }
        else //up
        {
          for (var x = p1.X; x <= p2.X; x++)
          {
            boundaries.Add(new(x, p1.Y));
          }
        }
      }
      else
      {
        throw new ThisShouldNeverHappenException($"line not straight! {p1} -> {p2}");
      }
    }
    catch(Exception e)
    { 
      //grid[0] = true;
    }
  }

  private static HashSet<GridPoint> DrawOuterPerimeter(GridPoint start, ref HashSet<GridPoint> boundaries)
  {
    var pointsToCheck = new HashSet<GridPoint>() { start };
    var outerPerimeter = new HashSet<GridPoint>();

    while (pointsToCheck.Any())
    {
      var currentPoint = pointsToCheck.First();
      if (boundaries.Contains(currentPoint))
      {
        throw new ThisShouldNeverHappenException($"how did {currentPoint} get here wtf");
      }

      if (!outerPerimeter.Contains(currentPoint))
      {
        var isOuterPerimeter = false;
        var checkableNeighbors = new List<GridPoint>();
        foreach (var neighbor in currentPoint.GetExtendedNeighbors())
        {
          if (boundaries.Contains(neighbor))
          {
            isOuterPerimeter = true;
          }
          else if (!outerPerimeter.Contains(neighbor))
          {
            if (boundaries.Contains(neighbor))
            {
              throw new ThisShouldNeverHappenException($"tried to add {neighbor} wtf");
            }
              
            checkableNeighbors.Add(neighbor);
          }
        }

        if (isOuterPerimeter)
        {
          outerPerimeter.Add(currentPoint);
          pointsToCheck.UnionWith(checkableNeighbors);
        }
      }

      pointsToCheck.Remove(currentPoint);
    }

    return outerPerimeter;
  }

  # region failed BitArray helpers
  private static BitArray FloodFill(ref BitArray map, int xBound, int yBound)
  {
    var result = new BitArray(map.Length);
    var pointsToCheck = new HashSet<GridPoint> { new(0,0) };

    do
    {
      var currentPoint = pointsToCheck.First();
      pointsToCheck.Remove(currentPoint);
      if (GetPointAt(ref map, currentPoint, xBound))
      {
        //we reached an edge, leave this one be
      }
      else if (!GetPointAt(ref result, currentPoint, xBound))
      {
        SetPointAt(ref result, currentPoint, xBound, true);
      }
      
      var neighbors = GridPoint.ExtendedDirections
        .Select(n => currentPoint + n)
        .Where(n => n.IsInBounds(xBound, yBound));

      foreach (var n in neighbors)
      {
        if (!GetPointAt(ref result, n, xBound) && !GetPointAt(ref map, currentPoint, xBound)) pointsToCheck.Add(n);
      }
    } while (pointsToCheck.Count != 0);

    return result;
  }

  private static void SetPointAt(ref BitArray map, GridPoint point, int xBound, bool value)
    => map[(point.Y * xBound) + point.X] = value;
  
  private static bool GetPointAt(ref BitArray map, GridPoint point, int xBound)
    => map[(point.Y * xBound) + point.X];
  
  private void PrintGrid(BitArray grid, int xBound)
  {
    if (!DebugMode) return;
    D();

    var sb = new StringBuilder();

    for (var i = 0; i < grid.Length; i++)
    {
      sb.Append(grid[i] ? '#' : '.');
      if (i % xBound == xBound - 1)
      {
        D(sb.ToString());
        sb.Clear();
      }
    }
  }
  
  #endregion
  
  private void PrintGrid(HashSet<GridPoint> points, int xBound, int yBound)
  {
    if (!DebugMode) return;
    D();

    var sb = new StringBuilder();

    for (var y = 0; y < yBound; y++)
    {
      for (var x = 0; x < xBound; x++)
      {
        sb.Append(points.Contains(new(x, y)) ? '#' : '.');
      }

      D(sb.ToString());
      sb.Clear();
    }
  }

  private static IEnumerable<GridPoint> GenerateRectanglePerimeter(GridPoint p1, GridPoint p2)
  {
    //special cases: lines
    if (p1.X == p2.X) //horizontal
    {
      var points = new[] { p1, p2 };
      points.Sort();

      for (var y = p1.Y; y <= p2.Y; y++)
      {
        yield return new(p1.X, y);
      }
    }
    else if (p1.Y == p2.Y) //vertical
    {
      var points = new[] { p1, p2 };
      points.Sort();

      for (var x = p1.X; x <= p2.X; x++)
      {
        yield return new(x, p1.Y);
      }
    }
    else
    {
      var points = new[] { p1, p2, new GridPoint(p1.X, p2.Y), new GridPoint(p2.X, p1.Y) };
      points.Sort();
      //0:0|4:0|0:4|4:4
      var topLeft = points[0];
      var topRight = points[1];
      var bottomLeft = points[2];
      var bottomRight = points[3];
      
      //iterate in a box lol
      for (var x = topLeft.X; x < topRight.X; x++)
      {
        yield return new(x, topLeft.Y);
      }
      
      for (var y = topRight.Y; y < bottomRight.Y; y++)
      {
        yield return new(topRight.X, y);
      }
      
      for (var x = bottomRight.X; x > bottomLeft.X; x--)
      {
        yield return new(x, bottomRight.Y);
      }
      
      for (var y = bottomLeft.Y; y > topLeft.Y; y--)
      {
        yield return new(bottomLeft.X, y);
      }
    }
  }
}