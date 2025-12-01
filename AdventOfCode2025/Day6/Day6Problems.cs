using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day6;

public class Day6Problems : Problems
{
  protected override string TestInput => @"....#.....
.........#
..........
..#.......
.......#..
..........
.#..^.....
........#.
#.........
......#...";

  protected override int Day => 6;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var visitedPoints = new HashSet<GridPoint>();
    var obstacles = new HashSet<GridPoint>();
    var startingPos = new GridPoint(0, 0);

    for (var y = 0; y < input.Length; y++)
    {
      for (var x = 0; x < input[0].Length; x++)
      {
        var checkChar = input[y][x];

        switch (checkChar)
        {
          case '#':
            obstacles.Add(new GridPoint(x, y));
            break;
          case '^':
            startingPos = new GridPoint(x, y);
            break;
          case '.':
          default:
            break;
        }
      }
    }

    visitedPoints.Add(startingPos);
    var currentPos = startingPos;
    var currentDir = Direction.Up;
    var bounds = new GridPoint(input[0].Length - 1, input.Length - 1);
    var isInBounds = true;

    while (isInBounds)
    {
      var nextPos = MakeOneMove(currentDir, currentPos, obstacles);
      
      currentPos = nextPos.Position;
      currentDir = nextPos.Direction;
      
      if(IsInBounds(currentPos, bounds))
        visitedPoints.Add(currentPos);
      else
        isInBounds = false;
    }
    
    return visitedPoints.Count.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var obstacles = new HashSet<GridPoint>();
    var startingPos = new GridPoint(0, 0);
    var numberOfLoopsFound = 0;
    var bounds = new GridPoint(input[0].Length - 1, input.Length - 1);

    for (var y = 0; y < input.Length; y++)
    {
      for (var x = 0; x < input[0].Length; x++)
      {
        var checkChar = input[y][x];

        switch (checkChar)
        {
          case '#':
            obstacles.Add(new GridPoint(x, y));
            break;
          case '^':
            startingPos = new GridPoint(x, y);
            break;
          case '.':
          default:
            break;
        }
      }
    }

    var startingInfo = new HeadingInfo(Direction.Up, startingPos);
    
    //we'll only attempt placing obstacles on points visited in the obstacle-free track -- otherwise they'll never be reached
    var visitedPoints = new HashSet<GridPoint>();
    var isInBounds = true;
    var currentPos = startingPos;
    var currentDir = Direction.Up;
    
    while (isInBounds)
    {
      var nextPos = MakeOneMove(currentDir, currentPos, obstacles);
      
      currentPos = nextPos.Position;
      currentDir = nextPos.Direction;
      
      if(IsInBounds(currentPos, bounds))
        visitedPoints.Add(currentPos);
      else
        isInBounds = false;
    }

    foreach (var testPos in visitedPoints.Where(p => p != startingPos))
    {
      var extraObstacle = obstacles.ToHashSet();
      extraObstacle.Add(testPos);

      if (CheckIfLoops(startingInfo, extraObstacle, bounds)) numberOfLoopsFound++;
    }
    
    return numberOfLoopsFound.ToString();
  }

  private static HeadingInfo MakeOneMove(Direction dir, GridPoint loc,
    HashSet<GridPoint> obstacles)
  {
    var nextPos = dir switch
    {
      Direction.Up => new GridPoint(loc.X, loc.Y - 1),
      Direction.Right => new GridPoint(loc.X + 1, loc.Y),
      Direction.Down => new GridPoint(loc.X, loc.Y + 1),
      Direction.Left => new GridPoint(loc.X - 1, loc.Y),
      _ => throw new ThisShouldNeverHappenException()
    };

    if (obstacles.Contains(nextPos)) // turn right instead
    {
      var nextDir = dir switch
      {
        Direction.Up => Direction.Right,
        Direction.Right => Direction.Down,
        Direction.Down => Direction.Left,
        Direction.Left => Direction.Up,
        _ => throw new ThisShouldNeverHappenException()
      };

      return new HeadingInfo(nextDir, loc);
    }
    
    return new HeadingInfo(dir, nextPos);
  }

  private static bool IsInBounds(GridPoint current, GridPoint bounds)
  {
    return current.X >= 0 && current.Y >= 0 & current.X <= bounds.X && current.Y <= bounds.Y;
  }

  private static bool CheckIfLoops(HeadingInfo start, HashSet<GridPoint> obstacles, GridPoint bounds)
  {
    var recordedHeadings = new HashSet<HeadingInfo> { start };
    var currentHeading = start;

    while (true)
    {
      var newHeading = MakeOneMove(currentHeading.Direction, currentHeading.Position, obstacles);
      
      if (!IsInBounds(newHeading.Position, bounds))
      {
        return false;
      }
      
      if (recordedHeadings.Contains(newHeading))
      {
        return true;
      }
      
      currentHeading = newHeading;
      recordedHeadings.Add(currentHeading);
    }
  }
}