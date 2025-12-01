namespace AdventOfCode2024.Util;

public abstract class Problems
{
  //TODO refactor to be an array of strings (each can be split to lines on \n) and live in a partial class in its own file
  protected abstract string TestInput { get; }

  private string FullInputFilePath => $"Day{Day}\\D{Day}.txt";

  protected abstract int Day { get; }
  protected bool DebugMode = false;

  protected abstract string Problem1(string[] input, bool isTestInput);
  protected abstract string Problem2(string[] input, bool isTestInput);

  public string Problem1TestInput()
  {
    var lines = TestInput.Split('\n').Select(s => s.Trim()).ToArray();
    return Problem1(lines, true);
  }

  public string Problem2TestInput()
  {
    var lines = TestInput.Split('\n').Select(s => s.Trim()).ToArray();
    return Problem2(lines, true);
  }

  public string Problem1FullInput()
  {
    var lines = File.ReadAllLines(FullInputFilePath);
    return Problem1(lines, false);
  }

  public string Problem2FullInput()
  {
    var lines = File.ReadAllLines(FullInputFilePath);
    return Problem2(lines, false);
  }

  /// <summary>
  /// D for Debug
  /// </summary>
  /// <param name="message"></param>
  /// <param name="appendNewLine"></param>
  protected void D()
  {
    if (DebugMode) Console.WriteLine();
  }
  
  /// <summary>
  /// D for Debug
  /// </summary>
  /// <param name="message"></param>
  /// <param name="appendNewLine"></param>
  protected void D(string message, bool appendNewLine = true)
  {
    if (!DebugMode) return;
    if(appendNewLine) Console.WriteLine(message);
    else Console.Write(message);
  }
  
  /// <summary>
  /// D for Debug
  /// </summary>
  /// <param name="message"></param>
  /// <param name="appendNewLine"></param>
  protected void D(object? obj, bool appendNewLine = true)
  {
    if (!DebugMode) return;
    if(appendNewLine) Console.WriteLine(obj?.ToString() ?? "nUlL");
    else Console.Write(obj?.ToString() ?? "nUlL");
  }
}