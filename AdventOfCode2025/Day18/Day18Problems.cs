using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day18;

public class Day18Problems : Problems
{
  protected override string TestInput => @"5,4
4,2
4,5
3,0
2,1
6,3
2,4
1,5
0,6
3,3
2,6
5,1
1,2
5,5
2,5
6,5
1,4
0,4
6,4
1,1
6,1
1,0
0,5
1,6
2,0";

  protected override int Day => 18;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var boundarySize = isTestInput ? 7 : 71;
    var maxBytes = isTestInput ? 12 : 1024;
    var occupiedSpaces = new HashSet<GridPoint>();

    for (var i = 0; i < maxBytes; i++)
    {
      var coords = StringUtils.ExtractIntsFromString(input[i]).ToArray();
      occupiedSpaces.Add(new GridPoint(coords[0], coords[1]));
    }
    
    //it's ya boi dijkstra back again after finding the shortest path from day 16 to here
    
    var startPoint = new GridPoint(0, 0);
    var endPoint = new GridPoint(boundarySize - 1, boundarySize - 1);
    var knownByDistance = new Dictionary<GridPoint, int>
    {
      [startPoint] = 0
    };
    var visitedPoints = new HashSet<GridPoint>();
    var currentPoint = startPoint;

    while (visitedPoints.Count + occupiedSpaces.Count < boundarySize * boundarySize)
    {
      var currentDistance = knownByDistance[currentPoint];

      if (currentPoint == endPoint) return currentDistance.ToString();

      var possiblePoints = 
        GridPoint.CardinalDirections.Select(p => currentPoint + p);
      
      var distanceToPoint = currentDistance + 1;

      foreach (var point in possiblePoints
                 .Where(p => 
                   p.IsInBounds(boundarySize, boundarySize) && !occupiedSpaces.Contains(p) && !visitedPoints.Contains(p)))
      {
        if (!knownByDistance.TryGetValue(point, out var knownDistance) || knownDistance > distanceToPoint)
        {
          knownByDistance[point] = distanceToPoint;
        }
      }
      
      visitedPoints.Add(currentPoint);
      knownByDistance.Remove(currentPoint);

      currentPoint = knownByDistance.MinBy(r => r.Value).Key;
    }
    
    throw new ThisShouldNeverHappenException("no route");
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var boundarySize = isTestInput ? 7 : 71;
    var startBytes = isTestInput ? 12 : 1024;
    var occupiedSpaces = new HashSet<GridPoint>();

    for (var i = 0; i < startBytes; i++)
    {
      var coords = StringUtils.ExtractIntsFromString(input[i]).ToArray();
      occupiedSpaces.Add(new GridPoint(coords[0], coords[1]));
    }

    var currentBestRoute = FindPathIfExists(occupiedSpaces, boundarySize);
    
    for (var i = startBytes; i < input.Length; i++)
    {
      var coords = StringUtils.ExtractIntsFromString(input[i]).ToArray();
      var point = new GridPoint(coords[0], coords[1]);
      occupiedSpaces.Add(point);
      if (currentBestRoute.Contains(point))
      {
        var newBestRoute = FindPathIfExists(occupiedSpaces, boundarySize);
        if (newBestRoute.Count == 0) return $"{point.X},{point.Y}";
        currentBestRoute = newBestRoute;
      }
    }

    throw new ThisShouldNeverHappenException("all bytes processed !?!?!");
  }

  private static HashSet<GridPoint> FindPathIfExists(HashSet<GridPoint> occupiedSpaces, int bounds)
  {
    var startPoint = new GridPoint(0, 0);
    var endPoint = new GridPoint(bounds - 1, bounds - 1);
    var knownByRoute = new Dictionary<GridPoint, HashSet<GridPoint>>
    {
      [startPoint] = new(){ startPoint }
    };
    var visitedPoints = new HashSet<GridPoint>();
    var currentPoint = startPoint;

    while (visitedPoints.Count + occupiedSpaces.Count < bounds * bounds)
    {
      var currentRoute = knownByRoute[currentPoint];
      var currentDistance = currentRoute.Count;

      if (currentPoint == endPoint) return currentRoute;

      var possiblePoints = 
        GridPoint.CardinalDirections.Select(p => currentPoint + p);
      
      var distanceToPoint = currentDistance + 1;

      foreach (var point in possiblePoints
                 .Where(p => 
                   p.IsInBounds(bounds, bounds) && !occupiedSpaces.Contains(p) && !visitedPoints.Contains(p)))
      {
        if (!knownByRoute.TryGetValue(point, out var knownRoute) || knownRoute.Count > distanceToPoint)
        {
          knownByRoute[point] = new HashSet<GridPoint>(currentRoute) { point };
        }
      }
      
      visitedPoints.Add(currentPoint);
      knownByRoute.Remove(currentPoint);
      
      if(knownByRoute.Any())
        currentPoint = knownByRoute.MinBy(r => r.Value.Count).Key;
      else
        return new HashSet<GridPoint>();
      
    }

    return new HashSet<GridPoint>();
  }
}