namespace AdventOfCode2025.Day8;

using Util;

public class Day8Problems : Problems
{
  protected override int Day => 8;
  
  protected override string TestInput => """
                                         162,817,812
                                         57,618,57
                                         906,360,560
                                         592,479,940
                                         352,342,300
                                         466,668,158
                                         542,29,236
                                         431,825,988
                                         739,650,466
                                         52,470,668
                                         216,146,977
                                         819,987,18
                                         117,168,530
                                         805,96,715
                                         346,949,466
                                         970,615,88
                                         941,993,340
                                         862,61,35
                                         984,92,344
                                         425,690,689
                                         """;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    throw new NotImplementedException("skipping, TODO deleteme");
    
    var maxTrackedJunctions = isTestInput ? 10 : 1000;
    
    var boxen = new SortedSet<JunctionBox>();
    var connectionsByMagnitude = new SortedDictionary<double, (JunctionBox low,  JunctionBox high)>();

    foreach (var line in input)
    {
      var coords = StringUtils.ExtractIntsFromString(line).ToArray();
      var box = new JunctionBox(new(coords[0], coords[1], coords[2]));
      D(box);
      boxen.Add(box);
    }
    
    Dbe(() =>
    {
      return '\n' + string.Join('\n', boxen.Select(b => b.ToString()).ToArray());
    });
    
    //for each box, search up the set to find all neighbors and add to tracked connections
    //stop when (nextMag - curMag) > connectionsByMagnitude.Max()
    for (var i = 0; i < boxen.Count; i++)
    {
      var box = boxen.ElementAt(i);
      var magnitudeCapHit = false;

      for (var higherBoxIndex = i + 1; higherBoxIndex < boxen.Count && !magnitudeCapHit; higherBoxIndex++)
      {
        var higherBox = boxen.ElementAt(higherBoxIndex);
        
        var relativeMagnitude = JunctionBox.MeasureMagnitude(box.Coordinates, higherBox.Coordinates);

        if (connectionsByMagnitude.Count < maxTrackedJunctions)
        {
          connectionsByMagnitude.Add(relativeMagnitude, (box, higherBox));
        }
        else
        {
          var currentMaxRelativeMagnitude = connectionsByMagnitude.Last().Key;
          if (relativeMagnitude < currentMaxRelativeMagnitude)
          {
            connectionsByMagnitude.Add(relativeMagnitude, (box, higherBox));
            connectionsByMagnitude.Remove(currentMaxRelativeMagnitude);
          }
          else
          {
            if(higherBox.AbsoluteMagnitude - box.AbsoluteMagnitude > currentMaxRelativeMagnitude) magnitudeCapHit = true;
          }
        }
      }
    }
    
    Dbe(() =>
    {
      return '\n' + string.Join('\n', connectionsByMagnitude
        .Select(conn => $"{conn.Key}: {conn.Value.low} -> {conn.Value.high}")
        .ToArray());
    });
    
    //finally, build circuits
    //probably a little suboptimal but like, it has a HashSet in it, how slow can it be?

    var circuits = new List<HashSet<JunctionBox>>();

    foreach (var connection in connectionsByMagnitude)
    {
      var existingCircuits = circuits
        .Where(c => c.Contains(connection.Value.low) || c.Contains(connection.Value.high)).ToList();

      if (existingCircuits.Count > 0)
      {
        var firstCircuit = existingCircuits.First();
        
        firstCircuit.Add(connection.Value.low);
        firstCircuit.Add(connection.Value.high);

        if (existingCircuits.Count > 1)
        {
          foreach (var dupeCircuit in existingCircuits[1..])
          {
            firstCircuit.UnionWith(dupeCircuit);
            circuits.Remove(dupeCircuit);
          }
        }
      }
      else
      {
        circuits.Add([connection.Value.low, connection.Value.high]);
      }
    }
    
    var topThreeCircuits = circuits.OrderByDescending(c => c.Count).Take(3).ToArray();

    foreach (var circuit in topThreeCircuits)
    {
      Dbe(() => $"circuit of size: {circuit.Count} :::: {string.Join("  ^  ", circuit)}");
    }
    
    return (topThreeCircuits[0].Count * topThreeCircuits[1].Count * topThreeCircuits[2].Count).ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {    
    var maxTrackedJunctions = isTestInput ? 50 : 7500; //these two chosen by highly scientific W.A.G method
    var neighborsToCheckUp = isTestInput ? 20 : 300;
    
    var boxen = new SortedSet<JunctionBox>();
    var connectionsByMagnitude = new SortedDictionary<double, (JunctionBox low,  JunctionBox high)>();

    foreach (var line in input)
    {
      var coords = StringUtils.ExtractIntsFromString(line).ToArray();
      var box = new JunctionBox(new(coords[0], coords[1], coords[2]));
      D(box);
      boxen.Add(box);
    }
    
    //for each box, search up the set to find all neighbors and add to tracked connections
    //stop when (nextMag - curMag) > connectionsByMagnitude.Max()
    var skipBoxIndices = new HashSet<int>();
    
    for (var targetNeighborIncrement = 1; targetNeighborIncrement <= neighborsToCheckUp; targetNeighborIncrement++)
    {
      for (var i = 0; i < boxen.Count; i++)
      {
        if(skipBoxIndices.Contains(i) || i + targetNeighborIncrement >= boxen.Count) continue;
        
        var box = boxen.ElementAt(i);
        
        var higherBox = boxen.ElementAt(i + targetNeighborIncrement);

        var relativeMagnitude = JunctionBox.MeasureMagnitude(box.Coordinates, higherBox.Coordinates);

        if (connectionsByMagnitude.Count < maxTrackedJunctions)
        {
          connectionsByMagnitude.Add(relativeMagnitude, (box, higherBox));
        }
        else
        {
          var currentMaxRelativeMagnitude = connectionsByMagnitude.Last().Key;
          if (relativeMagnitude < currentMaxRelativeMagnitude)
          {
            connectionsByMagnitude.Add(relativeMagnitude, (box, higherBox));
            connectionsByMagnitude.Remove(currentMaxRelativeMagnitude);
          }
          else if (higherBox.AbsoluteMagnitude - box.AbsoluteMagnitude > currentMaxRelativeMagnitude)
          {
            skipBoxIndices
              .Add(i); //all further neighbors are already over the max magnitude, no point ever trying this one again
          }
        }
      }
    }

    Dbe(() =>
    {
      return '\n' + string.Join('\n', connectionsByMagnitude
        .Select(conn => $"{conn.Key}: {conn.Value.low} -> {conn.Value.high}")
        .ToArray());
    });
    
    //finally, build circuits
    //probably a little suboptimal but like, it has a HashSet in it, how slow can it be?

    var circuits = new List<HashSet<JunctionBox>>();

    foreach (var connection in connectionsByMagnitude)
    {
      var existingCircuits = circuits
        .Where(c => c.Contains(connection.Value.low) || c.Contains(connection.Value.high)).ToList();

      if (existingCircuits.Count > 0)
      {
        var firstCircuit = existingCircuits.First();
        
        firstCircuit.Add(connection.Value.low);
        firstCircuit.Add(connection.Value.high);

        if (existingCircuits.Count > 1)
        {
          foreach (var dupeCircuit in existingCircuits[1..])
          {
            firstCircuit.UnionWith(dupeCircuit);
            circuits.Remove(dupeCircuit);
          }
        }

        if (circuits.Count == 1) //we did it
        {
          return (connection.Value.low.Coordinates.X * connection.Value.high.Coordinates.X).ToString();
        }
      }
      else
      {
        circuits.Add([connection.Value.low, connection.Value.high]);
      }
    }
    
    var topCircuit = circuits.OrderByDescending(c => c.Count).First();
    throw new ThisShouldNeverHappenException($"failed to find solution, finished with {circuits.Count} circuits, largest size {topCircuit.Count}");
  }

  private class JunctionBox : IComparable<JunctionBox>, IEquatable<JunctionBox>
  {
    public readonly TriplePoint Coordinates;
    public readonly double AbsoluteMagnitude;

    public JunctionBox(TriplePoint coordinates)
    {
      Coordinates = coordinates;
      AbsoluteMagnitude = MeasureMagnitude(TriplePoint.Zero, coordinates);
    }

    public int CompareTo(JunctionBox? other)
    {
      ArgumentNullException.ThrowIfNull(other);
      
      var result = AbsoluteMagnitude.CompareTo(other.AbsoluteMagnitude);
      
      if(result != 0) return result;
      
      result = Coordinates.X.CompareTo(other.Coordinates.X);
      if(result != 0) return result;
      
      result = Coordinates.Y.CompareTo(other.Coordinates.Y);
      return result != 0 ? result : Coordinates.Z.CompareTo(other.Coordinates.Z);
    }

    public bool Equals(JunctionBox? other)
    {
      return other?.Coordinates == Coordinates;
    }

    public static double MeasureMagnitude(TriplePoint c1, TriplePoint c2)
    {
      return Math.Sqrt(
        Math.Pow(c2.X - c1.X, 2) + Math.Pow(c2.Y - c1.Y, 2) + Math.Pow(c2.Z - c1.Z, 2));
    }
    
    public override bool Equals(object? obj)
    {
      return obj is JunctionBox other && Equals(other);
    }

    public override int GetHashCode()
    {
      return Coordinates.GetHashCode();
    }

    public static bool operator ==(JunctionBox a, JunctionBox b)
    {
      return a.Equals(b);
    }

    public static bool operator !=(JunctionBox a, JunctionBox b)
    {
      return !a.Equals(b);
    }

    public static JunctionBox operator +(JunctionBox a, JunctionBox b)
    {
      return new(a.Coordinates + b.Coordinates);
    }

    public static JunctionBox operator -(JunctionBox a, JunctionBox b)
    {
      return new(a.Coordinates - b.Coordinates);
    }

    public override string ToString()
      => $"{Coordinates} | {AbsoluteMagnitude}";
  }
}