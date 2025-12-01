using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day4;

public class Day4Problems : Problems
{
  protected override string TestInput => @"MMMSXXMASM
MSAMXMSMSA
AMXSXMAAMM
MSAMASMSMX
XMASAMXAMM
XXAMMXXAMA
SMSMSASXSS
SAXAMASAAA
MAMMMXMMMM
MXMXAXMASX";

  protected override int Day => 4;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var total = 0;
    var directions = new GridPoint[]
    {
      new(-1, -1),
      new(-1, 0),
      new(-1, 1),
      new(0, -1),
      new(0, 1),
      new(1, -1),
      new(1, 0),
      new(1, 1)
    };

    for (var y = 0; y < input.Length; y++)
    {
      for (var x = 0; x < input[y].Length; x++)
      {
        if (input[y][x] == 'X')
        {
          foreach (var direction in directions)
          {
            if (IsValidStringInDirection(input, direction, new GridPoint(x, y), "MAS")) total++;
          }
        }
      }
    }
    
    return total.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {    
    var total = 0;

    for (var y = 1; y < input.Length -1; y++) //don't even check on edges
    {
      for (var x = 1; x < input[y].Length - 1; x++)
      {
        if (input[y][x] == 'A')
        {
          if ((input[y - 1][x - 1] == 'M' && input[y + 1][x + 1] == 'S') ||
              (input[y - 1][x - 1] == 'S' && input[y + 1][x + 1] == 'M'))
          {
            if ((input[y + 1][x - 1] == 'M' && input[y - 1][x + 1] == 'S') ||
                (input[y + 1][x - 1] == 'S' && input[y - 1][x + 1] == 'M'))
            {
              total++;
            }
          }
        }
      }
    }
    
    return total.ToString();
  }

  private static bool IsValidStringInDirection(string[] input, GridPoint direction, GridPoint startingLoc, string searchStr)
  {
    var x = startingLoc.X + direction.X;
    var y = startingLoc.Y + direction.Y;

    //check bounds
    if (x < 0 || x >= input[0].Length || y < 0 || y >= input.Length) return false;
    
    var checkChar = searchStr[0];
    var curChar = input[y][x];
    
    if (curChar != checkChar) return false;

    if (searchStr.Length == 1) return true;
    
    return IsValidStringInDirection(input, direction, new GridPoint(x, y), searchStr[1..]);
  }
}