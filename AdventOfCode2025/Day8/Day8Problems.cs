using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day8;

public class Day8Problems : Problems
{
  protected override string TestInput => @"............
........0...
.....0......
.......0....
....0.......
......A.....
............
............
........A...
.........A..
............
............";

  protected override int Day => 8;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var antennaeByFrequency = new Dictionary<char, List<GridPoint>>();
    var antinodes = new HashSet<GridPoint>();
    var bounds = new GridPoint(input[0].Length, input.Length);

    for (var y = 0; y < input.Length; y++)
    {
      for (var x = 0; x < input.Length; x++)
      {
        var checkChar = input[y][x];
        switch (checkChar)
        {
          case '.':
            //no-op
            break;
          default:
            if (antennaeByFrequency.ContainsKey(checkChar))
            {
              antennaeByFrequency[checkChar].Add(new GridPoint(x, y));
            }
            else
            {
              antennaeByFrequency.Add(checkChar, new List<GridPoint> { new (x, y) });
            }
            break;
        }
      }
    }

    foreach (var frequency in antennaeByFrequency)
    {
      for (var i = 0; i < frequency.Value.Count; i++)
      {
        for (var j = i + 1; j < frequency.Value.Count; j++)
        {
          var p1 = frequency.Value[i];
          var p2 = frequency.Value[j];
          
          var diff = p2 - p1;

          var a1 = p1 - diff;
          var a2 = p2 + diff;
          
          if(a1.IsInBounds(bounds)) antinodes.Add(a1);
          if(a2.IsInBounds(bounds)) antinodes.Add(a2);
        }
      }
    }
    
    return antinodes.Count.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {    var antennaeByFrequency = new Dictionary<char, List<GridPoint>>();
    var antinodes = new HashSet<GridPoint>();
    var bounds = new GridPoint(input[0].Length, input.Length);

    for (var y = 0; y < input.Length; y++)
    {
      for (var x = 0; x < input.Length; x++)
      {
        var checkChar = input[y][x];
        switch (checkChar)
        {
          case '.':
            //no-op
            break;
          default:
            if (antennaeByFrequency.ContainsKey(checkChar))
            {
              antennaeByFrequency[checkChar].Add(new GridPoint(x, y));
            }
            else
            {
              antennaeByFrequency.Add(checkChar, new List<GridPoint> { new (x, y) });
            }
            break;
        }
      }
    }

    foreach (var frequency in antennaeByFrequency)
    {
      for (var i = 0; i < frequency.Value.Count; i++)
      {
        for (var j = i + 1; j < frequency.Value.Count; j++)
        {
          var p1 = frequency.Value[i];
          var p2 = frequency.Value[j];
          
          antinodes.Add(p1);
          antinodes.Add(p2);
          
          var diff = p2 - p1;

          var isInBounds = true;
          var curPoint1 = p1 - diff;

          while (isInBounds)
          {
            if (curPoint1.IsInBounds(bounds))
            {
              antinodes.Add(curPoint1);
              curPoint1 -= diff;
            }
            else
            {
              isInBounds = false;
            }
          }

          isInBounds = true;
          var curPoint2 = p2 + diff;

          while (isInBounds)
          {
            if (curPoint2.IsInBounds(bounds))
            {
              antinodes.Add(curPoint2);
              curPoint2 += diff;
            }
            else
            {
              isInBounds = false;
            }
          }
        }
      }
    }
    
    return antinodes.Count.ToString();
  }
}