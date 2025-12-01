using System.Text;
using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day15;

public class Day15Problems : Problems
{
  protected override string TestInput => @"##########
#..O..O.O#
#......O.#
#.OO..O.O#
#..O@..O.#
#O#..O...#
#O..O..O.#
#.OO.O.OO#
#....O...#
##########

<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
>^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^";

  protected override int Day => 15;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var robotPos = new GridPoint(0, 0);
    var walls = new HashSet<GridPoint>();
    var boxes = new HashSet<GridPoint>();

    var parsingMap = true;
    var y = 0;

    while (parsingMap)
    {
      var line = input[y];
      if (!string.IsNullOrWhiteSpace(line))
      {
        for (var x = 0; x < line.Length; x++)
        {
          switch (line[x])
          {
            case '#': walls.Add(new GridPoint(x, y)); break;
            case 'O': boxes.Add(new GridPoint(x, y)); break;
            case '@': robotPos = new GridPoint(x, y); break;
          }
        }
      }
      else
      {
        parsingMap = false;
      }
      
      y++;
    }

    while (y < input.Length)
    {
      var line = input[y];
      foreach (var c in line)
      {
        var dir = c switch
        {
          '^' => GridPoint.Up,
          'v' => GridPoint.Down,
          '>' => GridPoint.Right,
          '<' => GridPoint.Left,
          _ => throw new ThisShouldNeverHappenException()
        };
        AttemptOneMove(ref robotPos, ref walls, ref boxes, dir);
      }

      y++;
    }

    var total = 0;
    foreach (var box in boxes)
    {
      total += box.X + (box.Y * 100);
    }
    
    return total.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {    
    var robotPos = new GridPoint(0, 0);
    var walls = new HashSet<GridPoint>();
    var boxes = new HashSet<GridPoint>();
    var printer = new StringBuilder();
    var xBound = input[0].Length * 2;

    var parsingMap = true;
    var y = 0;

    while (parsingMap)
    {
      var line = input[y];
      if (!string.IsNullOrWhiteSpace(line))
      {
        for (var x = 0; x < line.Length; x++)
        {
          switch (line[x])
          {
            case '#': 
              walls.Add(new GridPoint(2 * x, y)); 
              walls.Add(new GridPoint((2 * x) + 1, y)); 
              break;
            case 'O': 
              boxes.Add(new GridPoint(2 * x, y)); 
              break;
            case '@': 
              robotPos = new GridPoint(2 * x, y); 
              break;
          }
        }
      }
      else
      {
        parsingMap = false;
      }
      
      y++;
    }

    var yBound = y - 1;
    var lastMove = '.';
    var frame = 0;
    var maxFrames = 100;
    while (y < input.Length)
    {
      var line = input[y];
      foreach (var c in line)
      {
        var dir = c switch
        {
          '^' => GridPoint.Up,
          'v' => GridPoint.Down,
          '>' => GridPoint.Right,
          '<' => GridPoint.Left,
          _ => throw new ThisShouldNeverHappenException()
        };
        if(isTestInput && frame < maxFrames) PrintFrame(frame, lastMove, walls, boxes, robotPos, xBound, yBound, ref printer);
        AttemptOneMoveWithDoubleWideBoxes(ref robotPos, ref walls, ref boxes, dir);
        frame++;
        lastMove = c;
      }

      y++;
    }
    if(isTestInput) PrintFrame(frame, lastMove, walls, boxes, robotPos, xBound, yBound, ref printer);

    var total = 0;
    foreach (var box in boxes)
    {
      total += box.X + (box.Y * 100);
    }
    
    printer.AppendLine(total.ToString());
    
    return printer.ToString();
  }

  private static void AttemptOneMove(ref GridPoint robot, ref HashSet<GridPoint> walls, ref HashSet<GridPoint> boxes,
    GridPoint direction)
  {
    var attemptedPosition = robot + direction;

    if (walls.Contains(attemptedPosition))
    {
      //no-op
    }
    else if (boxes.Contains(attemptedPosition))
    {
      //try moving boxes
      var attemptedBoxMove = attemptedPosition + direction;
      while (boxes.Contains(attemptedBoxMove))
        attemptedBoxMove += direction;

      if (!walls.Contains(attemptedBoxMove))
      {
        boxes.Remove(attemptedPosition);
        boxes.Add(attemptedBoxMove);
        robot = attemptedPosition;
      }
    }
    else
    {
      robot = attemptedPosition;
    }
  }
  
  private static void AttemptOneMoveWithDoubleWideBoxes(ref GridPoint robot, ref HashSet<GridPoint> walls, 
    ref HashSet<GridPoint> boxes, GridPoint direction)
  {
    var attemptedPosition = robot + direction;

    if (walls.Contains(attemptedPosition))
    {
      //no-op
      return;
    }

    var blockedByBoxRight = boxes.TryGetValue(attemptedPosition, out var rightBox);
    var blockedByBoxLeft = boxes.TryGetValue(attemptedPosition + GridPoint.Left, out var leftBox);
    
    if (blockedByBoxRight || blockedByBoxLeft)
    {
      var canMoveBoxes = true;
      var firstBox = blockedByBoxRight ? rightBox : leftBox;
      var boxesToMove = new List<GridPoint> { firstBox };
      
      //try moving boxes
      if (direction == GridPoint.Right) //check for box or wall 2 right
      {
        var stillBoxes = true;
        var currentBox = firstBox;
        while (stillBoxes)
        {
          var attemptedBoxMove = currentBox + (direction * 2);
          if (walls.Contains(attemptedBoxMove))
          {
            stillBoxes = false;
            canMoveBoxes = false;
          }
          else if(boxes.Contains(attemptedBoxMove))
          {
            boxesToMove.Add(attemptedBoxMove);
            currentBox = attemptedBoxMove;
          }
          else
          {
            stillBoxes = false;
          }
        }
      }
      else if (direction == GridPoint.Left) // check for wall 1 left or box 2 left
      {
        var stillBoxes = true;
        var currentBox = firstBox;
        while (stillBoxes)
        {
          var attemptedBoxMove = currentBox + direction;
          if (walls.Contains(attemptedBoxMove))
          {
            stillBoxes = false;
            canMoveBoxes = false;
          }
          else if(boxes.TryGetValue(attemptedBoxMove + direction, out var nextBox))
          {
            boxesToMove.Add(nextBox);
            currentBox = nextBox;
          }
          else
          {
            stillBoxes = false;
          }
        }
      }
      else
      {
        var currentLayer = new HashSet<GridPoint> { firstBox };
        while (canMoveBoxes && currentLayer.Any())
        {
          var nextLayer = new HashSet<GridPoint>();
          foreach (var box in currentLayer)
          {
            if (walls.Contains(box + direction) || walls.Contains(box + direction + GridPoint.Right))
            {
              canMoveBoxes = false;
              break;
            }
            else
            {
              var prospectiveBoxes = new List<GridPoint>
              {
                box + direction + GridPoint.Left,
                box + direction,
                box + direction + GridPoint.Right,
              };
              foreach (var pBox in prospectiveBoxes)
              {
                if(boxes.Contains(pBox))
                  nextLayer.Add(pBox);
              }
            }
          }
          boxesToMove.AddRange(nextLayer);
          currentLayer = nextLayer;
        }
      }

      if (canMoveBoxes)
      {
        var boxesToAdd = new List<GridPoint>(); //process all adds after removes, to avoid accidentally removing new adds
        foreach (var box in boxesToMove)
        {
          boxes.Remove(box);
          boxesToAdd.Add(box + direction);
        }
        boxes.UnionWith(boxesToAdd);
        robot = attemptedPosition;
      }
    }
    else
    {
      robot = attemptedPosition;
    }
  }
  
  private static void PrintFrame(int frame, char lastMove, HashSet<GridPoint> walls, HashSet<GridPoint> boxes,
    GridPoint robot, int xBound, int yBound, ref StringBuilder printer)
  {
    printer.AppendLine();
    printer.AppendLine($"FRAME {frame}");
    printer.AppendLine($"LAST {lastMove}");
    var hasErrors = false;
    for (var y = 0; y < yBound; y++)
    {
      var lastWasBox = false;
      for (var x = 0; x < xBound; x++)
      {
        var toPrint = lastWasBox ? ']' : '.';
        var pos = new GridPoint(x, y);
        var isEmpty = true;
        if (walls.Contains(pos))
        {
          if(toPrint != '.') hasErrors = true;
          toPrint = '#';
          lastWasBox = false;
          isEmpty = false;
        }
        if (boxes.Contains(pos))
        {
          if(toPrint != '.') hasErrors = true;
          lastWasBox = true;
          toPrint = '[';
          isEmpty = false;
        }
        if (robot == pos)
        {
          if(toPrint != '.') hasErrors = true;
          toPrint = '@';
          lastWasBox = false;
          isEmpty = false;
        }

        if (isEmpty) lastWasBox = false;

        printer.Append(toPrint);
      }
      printer.AppendLine();
    }
    if(hasErrors) printer.AppendLine("!!!!!!! ERROR !!!!!!");
  }
}