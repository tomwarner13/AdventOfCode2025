using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day21;

public partial class Day21Problems : Problems
{
  protected override string TestInput => @"029A
980A
179A
456A
379A";

  protected override int Day => 21;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    DebugMode = false; //isTestInput;

    var total = 0;
    var numPadRobot = RobotNumPadController.Numeric;
    var directionalRobot = RobotNumPadController.Directional;
    
    foreach (var line in input)
    {
      D(line);
      var directions = numPadRobot.ProcessInput(line);
      D(directions);
      directions = directionalRobot.ProcessInput(directions);
      D(directions);
      directions = directionalRobot.ProcessInput(directions);
      D(directions);

      var addToTotal =
        StringUtils.ExtractIntsFromString(line).First() * directions.Length;
      D(directions.Length);
      D(addToTotal);
      D();
      total += addToTotal;
    }
    
    return total.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var total = 0L;
    
    var numPadRobot = RobotNumPadController.Numeric;
    var directionalRobot = RobotNumPadController.Directional;
    var cache = new Dictionary<string, long>();
    
    foreach (var line in input)
    {
      D(line);
      var baseDirections = numPadRobot.ProcessInput(line);
      
      var resultWithCache 
        = directionalRobot.CalcPressCount(baseDirections, 25, ref cache) 
          * StringUtils.ExtractLongsFromString(line).First();
      D(resultWithCache);
      total += resultWithCache;
    }
    
    D(total);
    return total.ToString();
  }

  private partial class RobotNumPadController
  {
    private GridPoint _locationPointer;
    private GridPoint _panicPoint;
    private readonly Dictionary<char, GridPoint> keyLookup = new();
    private readonly Dictionary<GridPoint, char> posLookup = new();
    
    private RobotNumPadController(string layout)
    {
      StringUtils.ReadInputGrid(layout.Split('\n'), (c, x, y) =>
        {
          var pt = new GridPoint(x, y);
          switch (c)
          {
            case 'A':
              _locationPointer = pt;
              break;
            case '!':
              _panicPoint = pt;
              break;
          }
          keyLookup[c] = pt;
          posLookup[pt] = c;
        });
    }
    
    public static RobotNumPadController Numeric => new(
      "789\n456\n123\n!0A"
      );
    
    public static RobotNumPadController Directional => new(
      "!^A\n<v>"
    );

    public string FollowInput(string input)
    {
      var output = new StringBuilder();

      foreach (var c in input)
      {
        switch (c)
        {
          case '>':
            _locationPointer.PlusEquals(GridPoint.Right);
            break;
          case '<':
            _locationPointer.PlusEquals(GridPoint.Left);
            break;
          case '^':
            _locationPointer.PlusEquals(GridPoint.Up);
            break;
          case 'v':
            _locationPointer.PlusEquals(GridPoint.Down);
            break;
          case 'A':
            output.Append(posLookup[_locationPointer]);
            break;
        }
      }
      
      Reset();
      return output.ToString();
    }

    public string ProcessInput(string input)
    {
      var sb = new StringBuilder();

      foreach (var c in input)
      {
        var dest = keyLookup[c];
        bool? avoidingPanic = null;
        while (_locationPointer != dest)
        {
          //move
          if (_locationPointer == _panicPoint) throw new ThisShouldNeverHappenException("panic!!");
          if (avoidingPanic == null)
          {
            avoidingPanic = 
              (_locationPointer.Y == _panicPoint.Y && dest.X == 0) //this would bring us left to pp
               || (_locationPointer.X == 0 && dest.Y == _panicPoint.Y); //this would bring us vert to pp
          }

          if (avoidingPanic ?? throw new ThisShouldNeverHappenException("waaaat"))
          {
            //right -> vert -> left
            if (dest.X > _locationPointer.X) //right
            {
              sb.Append('>');
              _locationPointer.PlusEquals(GridPoint.Right);
            }
            else if(dest.Y > _locationPointer.Y) //down
            {
              sb.Append('v');
              _locationPointer.PlusEquals(GridPoint.Down);
            }
            else if(dest.Y < _locationPointer.Y) //up
            {
              sb.Append('^');
              _locationPointer.PlusEquals(GridPoint.Up);
            }
            else if(dest.X < _locationPointer.X) //left
            {
              sb.Append('<');
              _locationPointer.PlusEquals(GridPoint.Left);
            }
            else
            {
              throw new ThisShouldNeverHappenException("math no worky!!");
            }
          }
          else
          {
            //left -> vert -> right
            if(dest.X < _locationPointer.X) //left
            {
              sb.Append('<');
              _locationPointer.PlusEquals(GridPoint.Left);
            }
            else if(dest.Y > _locationPointer.Y) //down
            {
              sb.Append('v');
              _locationPointer.PlusEquals(GridPoint.Down);
            }
            else if(dest.Y < _locationPointer.Y) //up
            {
              sb.Append('^');
              _locationPointer.PlusEquals(GridPoint.Up);
            }
            else if (dest.X > _locationPointer.X) //right
            {
              sb.Append('>');
              _locationPointer.PlusEquals(GridPoint.Right);
            }
            else
            {
              throw new ThisShouldNeverHappenException("math no worky!!");
            }
          }
        }
        sb.Append('A');
      }
      Reset();
      
      return sb.ToString();
    }

    public long CalcPressCount(string input, int iterationsRemaining, ref Dictionary<string, long> cache)
    {
      var cacheKey = $"{input}:{iterationsRemaining}";
      if(cache.TryGetValue(cacheKey, out var cachedResult)) return cachedResult;
      if(iterationsRemaining == 1) return ProcessInput(input).Length;
      
      var nextInput = ProcessInput(input);
      // break up by As, so "<Av<AA>>^A" -> [<A v<A A >>^A]
      var segments = SegmentRegex().Matches(nextInput);
      var result = 0L;

      foreach (Match segment in segments)
      {
        result += CalcPressCount(segment.ToString(), iterationsRemaining - 1, ref cache);
      }

      cache[cacheKey] = result;
      return result;
    }
    
    private void Reset() => _locationPointer = keyLookup['A'];
    
    [GeneratedRegex("[^A]*A")]
    private static partial Regex SegmentRegex();
    }
}