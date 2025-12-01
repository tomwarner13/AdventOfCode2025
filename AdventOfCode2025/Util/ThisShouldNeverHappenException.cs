using System.Runtime.Serialization;

namespace AdventOfCode2025.Util;

/// <summary>
/// Use to mark code paths that the compiler wants to cover but that should be unreachable at runtime
/// </summary>
[Serializable]
public class ThisShouldNeverHappenException : Exception
{
  public ThisShouldNeverHappenException()
  { }

  public ThisShouldNeverHappenException(string message) : base(message) { }

  public ThisShouldNeverHappenException(string message, Exception inner) : base(message, inner) { }

  protected ThisShouldNeverHappenException(SerializationInfo info, StreamingContext context) { }
}