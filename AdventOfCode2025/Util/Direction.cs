namespace AdventOfCode2025.Util;

public enum Direction
{
  Up,
  Down,
  Left,
  Right
};

public static class DirectionExtensions
{
  public static Direction TurnLeft(this Direction direction)
    => direction switch
    {
      Direction.Up => Direction.Left,
      Direction.Left => Direction.Down,
      Direction.Down => Direction.Right,
      Direction.Right => Direction.Up,
      _ => throw new ThisShouldNeverHappenException(nameof(direction))
    };
  
  public static Direction TurnRight(this Direction direction)
    => direction switch
    {
      Direction.Up => Direction.Right,
      Direction.Left => Direction.Up,
      Direction.Down => Direction.Left,
      Direction.Right => Direction.Down,
      _ => throw new ThisShouldNeverHappenException(nameof(direction))
    };
  
  public static Direction TurnAround(this Direction direction)
    => direction switch
    {
      Direction.Up => Direction.Down,
      Direction.Left => Direction.Right,
      Direction.Down => Direction.Up,
      Direction.Right => Direction.Left,
      _ => throw new ThisShouldNeverHappenException(nameof(direction))
    };
}