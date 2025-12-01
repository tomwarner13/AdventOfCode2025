using System.Text;
using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day14;

public class Day14Problems : Problems
{
  protected override string TestInput => @"p=0,4 v=3,-3
p=6,3 v=-1,-3
p=10,3 v=-1,2
p=2,0 v=2,-1
p=0,0 v=1,3
p=3,0 v=-2,-2
p=7,6 v=-1,-3
p=3,0 v=-1,-2
p=9,3 v=2,3
p=7,3 v=-1,2
p=2,4 v=2,-3
p=9,5 v=-3,-3";

  protected override int Day => 14;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var xBound = isTestInput ? 11 : 101;
    var yBound = isTestInput ? 7 : 103;
    const int steps = 100;

    var totalTopRight = 0;
    var totalTopLeft = 0;
    var totalBotRight = 0;
    var totalBotLeft = 0;
    
    var xHalf = xBound / 2;
    var yHalf = yBound / 2;

    foreach (var line in input)
    {
      var guardInit = StringUtils.ExtractIntsFromString(line, true).ToArray();
      var xDest = (guardInit[0] + (guardInit[2] * steps)) % xBound;
      var yDest = (guardInit[1] + (guardInit[3] * steps)) % yBound;

      if(xDest < 0) xDest += xBound;
      if(yDest < 0) yDest += yBound;
      
      if (xDest < xHalf)
      {
        if (yDest < yHalf)
        {
          totalTopLeft++;
        }
        else if (yDest > yHalf)
        {
          totalBotLeft++;
        }
      }
      else if (xDest > xHalf)
      {
        if (yDest < yHalf)
        {
          totalTopRight++;
        }
        else if (yDest > yHalf)
        {
          totalBotRight++;
        }
      }
    }
    return (totalTopLeft * totalBotLeft * totalTopRight * totalBotRight).ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var xBound = isTestInput ? 11 : 101;
    var yBound = isTestInput ? 7 : 103;
    var permutations = xBound * yBound;

    var interestingFramePrinter = new StringBuilder();
    var guards = 
      input.Select(line => StringUtils.ExtractIntsFromString(line, true).ToArray())
        .Select(guardInit => 
          new GuardInfo
          {
            Start = new GridPoint(guardInit[0], guardInit[1]), 
            Velocity = new GridPoint(guardInit[2], guardInit[3])
          }).ToList();

    for (var frame = 0; frame < permutations; frame++)
    {
      var guardsInFrame = GenerateOneFrame(guards, frame, xBound, yBound);
      if(IsInterestingFrame(guardsInFrame, 20))
        PrintFrame(frame, guardsInFrame, xBound, yBound, ref interestingFramePrinter);
    }

    return interestingFramePrinter.ToString();
  }

  private static HashSet<GridPoint> GenerateOneFrame(IEnumerable<GuardInfo> guards, int frame, int xBound, int yBound)
  {
    var guardPositions = new HashSet<GridPoint>();
    foreach (var guard in guards)
    {
      var xDest = MathUtils.PositiveMod(guard.Start.X + (guard.Velocity.X * frame), xBound);
      var yDest = MathUtils.PositiveMod(guard.Start.Y + (guard.Velocity.Y * frame), yBound);
      
      guardPositions.Add(new GridPoint(xDest, yDest));
    }
    return guardPositions;
  }

  private static bool IsInterestingFrame(HashSet<GridPoint> frame, int percentageThreshold)
  {
    var totalOccupied = frame.Count;
    var totalWithAllNeighbors = 0;

    foreach (var guard in frame)
    {
      if (GridPoint.ExtendedDirections.All(direction => IsANeighbor(guard, direction, ref frame)))
        totalWithAllNeighbors++;
    }

    if (totalWithAllNeighbors == 0) return false;
    return (totalOccupied * 100) / totalWithAllNeighbors > percentageThreshold;
  }

  private static bool IsANeighbor(GridPoint guard, GridPoint direction, ref HashSet<GridPoint> frame)
  {
    return frame.Contains(guard + direction);
  }

  private static void PrintFrame(int frame, HashSet<GridPoint> occupiedPositions, int xBound, int yBound,
    ref StringBuilder printer)
  {
    printer.AppendLine();
    printer.AppendLine($"FRAME {frame}");
    for (var y = 0; y < yBound; y++)
    {
      for (var x = 0; x < xBound; x++)
      {
        printer.Append(occupiedPositions.Contains(new GridPoint(x, y)) ? '#' : '.');
      }
      printer.AppendLine();
    }
  }

  private struct GuardInfo
  {
    public GridPoint Start;
    public GridPoint Velocity;
  };
}