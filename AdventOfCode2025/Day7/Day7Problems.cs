namespace AdventOfCode2025.Day7;

using Util;

public class Day7Problems : Problems
{
  protected override int Day => 7;

  protected override string TestInput => """
                                         .......S.......
                                         ...............
                                         .......^.......
                                         ...............
                                         ......^.^......
                                         ...............
                                         .....^.^.^.....
                                         ...............
                                         ....^.^...^....
                                         ...............
                                         ...^.^...^.^...
                                         ...............
                                         ..^...^.....^..
                                         ...............
                                         .^.^.^.^.^...^.
                                         ...............
                                         """;

  public override string Problem1(string[] input, bool isTestInput)
  {
    var beams = new HashSet<int>();
    var splitters = new HashSet<GridPoint>();
    
    StringUtils.ReadInputGrid(input, (c, x, y) =>
    {
      switch (c)
      {
        case 'S':
          // ReSharper disable once AccessToModifiedClosure - it's the whole point lol
          beams.Add(x);
          break;
        case '^':
          splitters.Add(new(x, y));
          break;
      }
    });
    
    var encounteredSplitters = new HashSet<GridPoint>();

    for (var row = 1; row < input.Length; row++)
    {
      var newBeams = new HashSet<int>();
      foreach (var beam in beams)
      {
        var pos = new GridPoint(beam, row);
        if (splitters.Contains(pos))
        {
          encounteredSplitters.Add(pos);
          if(beam > 0) newBeams.Add(beam - 1);
          if (beam < input[0].Length - 1) newBeams.Add(beam + 1);
        }
        else
        {
          newBeams.Add(beam);
        }
      }

      beams = newBeams;
      if(DebugMode) D(string.Join(',', beams.OrderBy(beam => beam)));
    }

    return encounteredSplitters.Count.ToString();
  }

  public override string Problem2(string[] input, bool isTestInput)
  {    
    var beamCounter = new Dictionary<int, long>();
    var splitters = new HashSet<GridPoint>();
    
    StringUtils.ReadInputGrid(input, (c, x, y) =>
    {
      switch (c)
      {
        case 'S':
          // ReSharper disable once AccessToModifiedClosure - it's the whole point lol
          beamCounter[x] = 1;
          break;
        case '^':
          splitters.Add(new(x, y));
          break;
      }
    });
    
    for (var row = 1; row < input.Length; row++)
    {
      var newBeamCounter = new Dictionary<int, long>();
      foreach (var beam in beamCounter)
      {
        var pos = new GridPoint(beam.Key, row);
        var currentTotalTimelines = beamCounter[beam.Key];
        
        if (splitters.Contains(pos))
        {
          if (beam.Key > 0)
          {
            if(newBeamCounter.TryGetValue(beam.Key - 1, out _)) newBeamCounter[beam.Key - 1] += currentTotalTimelines;
            else newBeamCounter[beam.Key - 1] = currentTotalTimelines;
          }

          if (beam.Key < input[0].Length - 1)
          {
            if(newBeamCounter.TryGetValue(beam.Key + 1, out _)) newBeamCounter[beam.Key + 1] += currentTotalTimelines;
            else newBeamCounter[beam.Key + 1] = currentTotalTimelines;
          }
        }
        else
        {
          if(newBeamCounter.TryGetValue(beam.Key, out var _)) newBeamCounter[beam.Key] += currentTotalTimelines;
          else newBeamCounter[beam.Key] = currentTotalTimelines;
        }
      }

      beamCounter = newBeamCounter;
      // ReSharper disable once AccessToModifiedClosure
      Dbe( () => string.Join(',', beamCounter
          .OrderBy(b => b.Key).Select(b => $"{b.Key}:{b.Value}")));
    }

    return beamCounter.Select(b => b.Value).Sum().ToString();
  }
}