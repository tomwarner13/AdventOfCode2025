namespace AdventOfCode2024.Util;

public struct LongPoint : IEquatable<LongPoint>
{
  public readonly long X;
  public readonly long Y;

  public LongPoint(long x, long y)
  {
    X = x;
    Y = y;
  }
  
  /// <summary>
  /// Checks that the point is >= (0,0) and lt (bounds)
  /// </summary>
  /// <param name="bounds"></param>
  /// <returns></returns>
  public bool IsInBounds(LongPoint bounds) 
    => X >= 0 && Y >= 0 && X < bounds.X && Y < bounds.Y;
  
  /// <summary>
  /// Checks that the point is >= (0,0) and lt (bounds)
  /// </summary>
  /// <param name="x"></param>
  /// <param name="y"></param>
  /// <returns></returns>
  public bool IsInBounds(int x, long y) 
    => X >= 0 && Y >= 0 && X < x && Y < y;

  public bool Equals(LongPoint other)
  {
    return X == other.X && Y == other.Y;
  }

  public override bool Equals(object? obj)
  {
    return obj is LongPoint other && Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(X, Y);
  }

  public static bool operator ==(LongPoint a, LongPoint b)
  {
    return a.Equals(b);
  }

  public static bool operator !=(LongPoint a, LongPoint b)
  {
    return !a.Equals(b);
  }

  public static LongPoint operator +(LongPoint a, LongPoint b)
  {
    return new LongPoint(a.X + b.X, a.Y + b.Y);
  }

  public static LongPoint operator -(LongPoint a, LongPoint b)
  {
    return new LongPoint(a.X - b.X, a.Y - b.Y);
  }

  public override string ToString() => $"{X}:{Y}";

  public static readonly LongPoint Up = new(0, -1);
  public static readonly LongPoint Left = new(-1, 0);
  public static readonly LongPoint Down = new(0, 1);
  public static readonly LongPoint Right = new(1, 0);
  public static readonly LongPoint UpRight = new(1, -1);
  public static readonly LongPoint UpLeft = new(-1, -1);
  public static readonly LongPoint DownRight = new(1, 1);
  public static readonly LongPoint DownLeft = new(-1, 1);
  

  public static List<LongPoint> CardinalDirections => new()
  {
    Up,
    Left,
    Down,
    Right
  };
  
  public static List<LongPoint> ExtendedDirections => new()
  {
    Up,
    Left,
    Down,
    Right,
    UpRight,
    UpLeft,
    DownRight,
    DownLeft
  };
}