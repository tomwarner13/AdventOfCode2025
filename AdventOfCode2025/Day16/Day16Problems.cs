using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day16;

public class Day16Problems : Problems
{
  protected override string TestInput => @"###############
#.......#....E#
#.#.###.#.###.#
#.....#.#...#.#
#.###.#####.#.#
#.#.#.......#.#
#.#.#####.###.#
#...........#.#
###.#.#####.#.#
#...#.....#.#.#
#.#.#.###.#.#.#
#.....#...#.#.#
#.###.#.#.#.#.#
#S..#.....#...#
###############";

  protected override int Day => 16;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var startPoint = new HeadingInfo();
    var endPoint = new GridPoint();
    var allRoutes = new Dictionary<HeadingInfo, int>();

    for (var y = 0; y < input.Length; y++)
    {
      for (var x = 0; x < input.Length; x++)
      {
        var c = input[y][x];
        if (c == '#')
        {
          //no-op
        }
        else
        {
          var point = new GridPoint(x, y);
          if (c == 'E')
          {
            endPoint = point;            
            allRoutes.Add(new HeadingInfo(Direction.Up, point), int.MaxValue);
            allRoutes.Add(new HeadingInfo(Direction.Down, point), int.MaxValue);
            allRoutes.Add(new HeadingInfo(Direction.Left, point), int.MaxValue);
            allRoutes.Add(new HeadingInfo(Direction.Right, point), int.MaxValue);
          }
          else if (c == 'S')
          {
            startPoint = new HeadingInfo(Direction.Right, point);
            allRoutes[startPoint] = 0;
            allRoutes.Add(new HeadingInfo(Direction.Up, point), 1000);
            allRoutes.Add(new HeadingInfo(Direction.Down, point), 1000);
            allRoutes.Add(new HeadingInfo(Direction.Left, point), 2000);
          }
          else
          {
            AddToAllRoutes(point, ref allRoutes);
          }
        }
      }
    }
    
    //bae: come over
    //dijkstra: but there are so many possible paths and i dont know which one is shortest
    //bae: my parents arent home
    //dijkstra:

    var currentState = startPoint;

    while (allRoutes[currentState] < int.MaxValue)
    {
      var curDist = allRoutes[currentState];
      if(currentState.Position == endPoint)
        return curDist.ToString();
      
      //get possible connections
      var possibleTurns = new HeadingInfo[]
      {
        new (currentState.Direction.TurnLeft(), currentState.Position),
        new (currentState.Direction.TurnRight(), currentState.Position),
      };
      var possibleMove = new HeadingInfo(currentState.Direction,
        currentState.Position + GridPoint.GetDirectionPoint(currentState.Direction));

      foreach (var turn in possibleTurns.Where(t => allRoutes.ContainsKey(t)))
      {
        var knownDistanceToTurn = allRoutes[turn];
        var newDistanceToDest = curDist + 1000;
        if(newDistanceToDest < knownDistanceToTurn)
          allRoutes[turn] = newDistanceToDest;
      }

      if (allRoutes.TryGetValue(possibleMove, out var knownDistanceToDest))
      {
        var newDistanceToDest = curDist + 1;
        if(newDistanceToDest < knownDistanceToDest)
          allRoutes[possibleMove] = newDistanceToDest;
      }
      
      allRoutes.Remove(currentState);
      
      //find minimum next dest, set to current move
      currentState = allRoutes.MinBy(r => r.Value).Key;
    }
    
    throw new ThisShouldNeverHappenException("no route found!");
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var startPoint = new HeadingInfo();
    var endPoint = new GridPoint();
    var allRoutes = new Dictionary<HeadingInfo, RouteTracker>();

    for (var y = 0; y < input.Length; y++)
    {
      for (var x = 0; x < input.Length; x++)
      {
        var c = input[y][x];
        if (c == '#')
        {
          //no-op
        }
        else
        {
          var point = new GridPoint(x, y);
          if (c == 'E')
          {
            endPoint = point;
            AddToAllRoutes(point, ref allRoutes);
          }
          else if (c == 'S')
          {
            startPoint = new HeadingInfo(Direction.Right, point);
            allRoutes[startPoint] = new RouteTracker(0, point);
            allRoutes.Add(new HeadingInfo(Direction.Up, point), new RouteTracker(1000, point));
            allRoutes.Add(new HeadingInfo(Direction.Down, point), new RouteTracker(1000, point));
            allRoutes.Add(new HeadingInfo(Direction.Left, point), new RouteTracker(2000, point));
          }
          else
          {
            AddToAllRoutes(point, ref allRoutes);
          }
        }
      }
    }

    var currentState = startPoint;
    var distToEnd = int.MaxValue;
    var routesToEnd = new HashSet<GridPoint>();

    while (allRoutes[currentState].Cost <= distToEnd)
    {
      var curRoute = allRoutes[currentState];
      var curDist = curRoute.Cost;
      if (currentState.Position == endPoint)
      {
        distToEnd = curDist;
        routesToEnd.UnionWith(curRoute.Route);
      }
      
      //get possible connections
      var possibleTurns = new HeadingInfo[]
      {
        new (currentState.Direction.TurnLeft(), currentState.Position),
        new (currentState.Direction.TurnRight(), currentState.Position),
      };

      foreach (var turn in possibleTurns.Where(t => allRoutes.ContainsKey(t)))
      {
        var knownRouteToTurn = allRoutes[turn];
        var newDistanceToDest = curDist + 1000;
        if (newDistanceToDest < knownRouteToTurn.Cost)
        {
          allRoutes[turn].Cost = newDistanceToDest;
          allRoutes[turn].Route = new HashSet<GridPoint>(curRoute.Route);
        }
        else if (newDistanceToDest == knownRouteToTurn.Cost)
        {
          knownRouteToTurn.Route.UnionWith(curRoute.Route);
        }
      }

      var possibleMove = new HeadingInfo(currentState.Direction,
        currentState.Position + GridPoint.GetDirectionPoint(currentState.Direction));
      if (allRoutes.TryGetValue(possibleMove, out var knownRouteToDest))
      {
        var newDistanceToDest = curDist + 1;
        if (newDistanceToDest < knownRouteToDest.Cost)
        {
          knownRouteToDest.Cost = newDistanceToDest;
          knownRouteToDest.Route = new HashSet<GridPoint>(curRoute.Route) { possibleMove.Position };
        }
        else if (newDistanceToDest == knownRouteToDest.Cost)
        {
          knownRouteToDest.Route.UnionWith(curRoute.Route);
          knownRouteToDest.Route.Add(possibleMove.Position);
        }
      }
      
      allRoutes.Remove(currentState);
      
      //find minimum next dest, set to current move
      currentState = allRoutes.MinBy(r => r.Value.Cost).Key;
    }

    return routesToEnd.Count.ToString();
  }

  private static void AddToAllRoutes(GridPoint point, ref Dictionary<HeadingInfo, int> allRoutes)
  {
    foreach (var dir in Enum.GetValues(typeof(Direction)).Cast<Direction>())
    {
      allRoutes.Add(new HeadingInfo(dir, point), int.MaxValue);
    }
  }
  
  private static void AddToAllRoutes(GridPoint point, ref Dictionary<HeadingInfo, RouteTracker> allRoutes)
  {
    foreach (var dir in Enum.GetValues(typeof(Direction)).Cast<Direction>())
    {
      allRoutes.Add(new HeadingInfo(dir, point), new RouteTracker(int.MaxValue, point));
    }
  }

  private class RouteTracker
  {
    public int Cost;
    public HashSet<GridPoint> Route;

    public RouteTracker(int cost, GridPoint point)
    {
      Cost = cost;
      Route = new HashSet<GridPoint> { point };
    }
  }
}