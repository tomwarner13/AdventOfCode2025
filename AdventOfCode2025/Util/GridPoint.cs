namespace AdventOfCode2025.Util;

public struct GridPoint : IEquatable<GridPoint>, IComparable<GridPoint>
{
  public int X;
  public int Y;

  public GridPoint(int x, int y)
  {
    X = x;
    Y = y;
  }
  
  /// <summary>
  /// Checks that the point is >= (0,0) and lt (bounds)
  /// </summary>
  /// <param name="bounds"></param>
  /// <returns></returns>
  public bool IsInBounds(GridPoint bounds) 
    => X >= 0 && Y >= 0 && X < bounds.X && Y < bounds.Y;
  
  /// <summary>
  /// Checks that the point is >= (0,0) and lt (bounds)
  /// </summary>
  /// <param name="x"></param>
  /// <param name="y"></param>
  /// <returns></returns>
  public bool IsInBounds(int x, int y) 
    => X >= 0 && Y >= 0 && X < x && Y < y;

  public bool Equals(GridPoint other)
  {
    return X == other.X && Y == other.Y;
  }

  /// <summary>
  /// Compare by Y then by X
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
  public int CompareTo(GridPoint other)
  {
    var vertCompare = Y.CompareTo(other.Y);
    return vertCompare != 0 ? vertCompare : X.CompareTo(other.X);
  }

  public override bool Equals(object? obj)
  {
    return obj is GridPoint other && Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(X, Y);
  }

  public static bool operator ==(GridPoint a, GridPoint b)
  {
    return a.Equals(b);
  }

  public static bool operator !=(GridPoint a, GridPoint b)
  {
    return !a.Equals(b);
  }

  public static GridPoint operator +(GridPoint a, GridPoint b)
  {
    return new GridPoint(a.X + b.X, a.Y + b.Y);
  }

  public static GridPoint operator -(GridPoint a, GridPoint b)
  {
    return new GridPoint(a.X - b.X, a.Y - b.Y);
  }
  
  public static GridPoint operator *(GridPoint a, int b)
  {
    return new GridPoint(a.X * b, a.Y * b);
  }
  
  public static GridPoint operator /(GridPoint a, int b)
  {
    return new GridPoint(a.X / b, a.Y / b);
  }

  public void PlusEquals(GridPoint other)
  {
    X += other.X;
    Y += other.Y;
  }
  
  public void MinusEquals(GridPoint other)
  {
    X -= other.X;
    Y -= other.Y;
  }

  public static int MeasureStepwiseDistance(GridPoint a, GridPoint b)
  {
    return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
  }
  
  public int MeasureStepwiseDistance(GridPoint other)
  {
    return MeasureStepwiseDistance(this, other);
  }

  public IEnumerable<GridPoint> GetCardinalNeighbors()
  {
    foreach (var direction in CardinalDirections)
    {
      yield return this + direction;
    }
  }
  
  public IEnumerable<GridPoint> GetExtendedNeighbors()
  {
    foreach (var direction in ExtendedDirections)
    {
      yield return this + direction;
    }
  }

  public override string ToString() => $"{X}:{Y}";

  public static readonly GridPoint Up = new(0, -1);
  public static readonly GridPoint Left = new(-1, 0);
  public static readonly GridPoint Down = new(0, 1);
  public static readonly GridPoint Right = new(1, 0);
  public static readonly GridPoint UpRight = new(1, -1);
  public static readonly GridPoint UpLeft = new(-1, -1);
  public static readonly GridPoint DownRight = new(1, 1);
  public static readonly GridPoint DownLeft = new(-1, 1);
  

  public static List<GridPoint> CardinalDirections => new()
  {
    Up,
    Left,
    Down,
    Right
  };
  
  public static List<GridPoint> ExtendedDirections => new()
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

  public static GridPoint GetDirectionPoint(Direction direction) => direction switch
  {
    Direction.Up => Up,
    Direction.Down => Down,
    Direction.Right => Right,
    Direction.Left => Left,
    _ => throw new ThisShouldNeverHappenException(nameof(direction))
  };
}
