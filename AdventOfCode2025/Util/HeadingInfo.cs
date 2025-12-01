namespace AdventOfCode2025.Util;

public struct HeadingInfo : IEquatable<HeadingInfo>
{
  public readonly Direction Direction;
  public readonly GridPoint Position;

  public HeadingInfo(Direction direction, GridPoint position)
  {
    Direction = direction;
    Position = position;
  }

  public bool Equals(HeadingInfo other)
  {
    return Direction == other.Direction && Position == other.Position;
  }

  public override bool Equals(object? obj)
  {
    return obj is HeadingInfo other && Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(Direction.GetHashCode(), Position.GetHashCode());
  }

  public static bool operator ==(HeadingInfo a, HeadingInfo b)
  {
    return a.Equals(b);
  }
    
  public static bool operator !=(HeadingInfo a, HeadingInfo b)
  {
    return !a.Equals(b);
  }
    
  public override string ToString() => $"{Position}:{Direction}";
}