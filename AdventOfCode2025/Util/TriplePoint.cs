namespace AdventOfCode2025.Util;

public readonly struct TriplePoint(int x, int y, int z) : IEquatable<TriplePoint>
{
  public readonly int X = x;
  public readonly int Y = y;
  public readonly int Z = z;

  public static TriplePoint Zero => new(0, 0, 0);
  
  public bool Equals(TriplePoint other)
  {
    return X == other.X && Y == other.Y && Z == other.Z;
  }

  public override bool Equals(object? obj)
  {
    return obj is TriplePoint other && Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(X, Y, Z);
  }

  public static bool operator ==(TriplePoint a, TriplePoint b)
  {
    return a.Equals(b);
  }

  public static bool operator !=(TriplePoint a, TriplePoint b)
  {
    return !a.Equals(b);
  }

  public static TriplePoint operator +(TriplePoint a, TriplePoint b)
  {
    return new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
  }

  public static TriplePoint operator -(TriplePoint a, TriplePoint b)
  {
    return new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
  }

  public override string ToString() => $"{X}:{Y}:{Z}";
}