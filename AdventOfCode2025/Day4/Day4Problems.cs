namespace AdventOfCode2025.Day4;

using Util;

public class Day4Problems : Problems
{
  protected override int Day => 4;
  
  protected override string TestInput => @"..@@.@@@@.
@@@.@.@.@@
@@@@@.@.@@
@.@@@@..@.
@@.@@@@.@@
.@@@@@@@.@
.@.@.@.@@@
@.@@@.@@@@
.@@@@@@@@.
@.@.@@@.@.";

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var totalAccessibleRolls = 0L;
    var bounds = new GridPoint(input[0].Length, input.Length);

    for (var y = 0; y < input.Length; y++)
    {
      for (var x = 0; x < input[0].Length; x++)
      {
        var checkChar = input[y][x];
        switch (checkChar)
        {
          case '.':
            //no-op
            break;
          default:
            var curPoint = new GridPoint(x, y);
            var neighboringRolls = 0;
            foreach (var dir in GridPoint.ExtendedDirections)
            {
              var neighbor = curPoint + dir;
              if (neighbor.IsInBounds(bounds) &&
                  input[neighbor.Y][neighbor.X] == '@')
              {
                neighboringRolls++;
              }
            }

            if (neighboringRolls < 4)
            {
              totalAccessibleRolls++;
              D(curPoint);
            }
            break;
        }
      }
    }
    return totalAccessibleRolls.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    throw new NotImplementedException();
  }
}