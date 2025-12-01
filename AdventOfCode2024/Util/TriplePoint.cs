namespace AdventOfCode2024.Util;

public struct TriplePoint : IEquatable<TriplePoint>
{
  public readonly int X;
  public readonly int Y;
  public readonly int Z;

  public TriplePoint(int x, int y, int z)
  {
    X = x;
    Y = y;
    Z = z;
  }

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
    return new TriplePoint(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
  }

  public static TriplePoint operator -(TriplePoint a, TriplePoint b)
  {
    return new TriplePoint(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
  }

  public override string ToString() => $"{X}:{Y}:{Z}";
}