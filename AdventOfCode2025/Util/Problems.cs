namespace AdventOfCode2025.Util;

using System.Diagnostics;

public abstract class Problems
{
  //TODO refactor to be an array of strings (each can be split to lines on \n) and live in a partial class in its own file
  protected abstract string TestInput { get; }
  
  private string[] _testInputLines = [];

  private string[] ReadTestInput()
  {
    if(_testInputLines.Length > 0) return _testInputLines;
    
    _testInputLines = TestInput.Split('\n');
    return _testInputLines;
  }

  private string FullInputFilePath => $"Day{Day}\\D{Day}.txt";
  
  private string[] _fullInputLines = [];

  private string[] ReadFullInput()
  {
    if(_fullInputLines.Length > 0) return _fullInputLines;
    
    _fullInputLines = File.ReadAllLines(FullInputFilePath);
    return _fullInputLines;
  }

  protected abstract int Day { get; }
  protected bool DebugMode;

  protected abstract string Problem1(string[] input, bool isTestInput);
  protected abstract string Problem2(string[] input, bool isTestInput);

  public string Problem1TestInput()
  {
    var lines = ReadTestInput();
    DebugMode = true;
    
    var sw = Stopwatch.StartNew();
    var result = Problem1(lines, true);
    sw.Stop();
    Console.WriteLine($"Elapsed: {sw.Elapsed:c}");
    return result;
  }

  public string Problem2TestInput()
  {
    var lines = ReadTestInput();
    DebugMode = true;
    
    var sw = Stopwatch.StartNew();
    var result = Problem2(lines, true);
    sw.Stop();
    Console.WriteLine($"Elapsed: {sw.Elapsed:c}");
    return result;
  }

  public string Problem1FullInput()
  {
    var lines = ReadFullInput();
    DebugMode = false;
    var sw = Stopwatch.StartNew();
    var result = Problem1(lines, false);
    sw.Stop();
    Console.WriteLine($"Elapsed: {sw.Elapsed:c}");
    return result;
  }

  public string Problem2FullInput()
  {
    var lines = ReadFullInput();
    DebugMode = false;
    
    var sw = Stopwatch.StartNew();
    var result = Problem2(lines, false);
    sw.Stop();
    Console.WriteLine($"Elapsed: {sw.Elapsed:c}");
    return result;
  }

  /// <summary>
  /// D for Debug - with no params, just a writeline
  /// </summary>
  protected void D()
  {
    if (DebugMode) Console.WriteLine();
  }

  /// <summary>
  /// Debug Expensive - pass in an operation that only should be executed in debug mode
  /// </summary>
  /// <param name="expensiveOp"></param>
  /// <param name="appendNewLine"></param>
  protected void Dbe(Func<string> expensiveOp, bool appendNewLine = true)
  {
    if (!DebugMode) return;
    var result = expensiveOp();
    D(result, appendNewLine);
  }
  
  /// <summary>
  /// D for Debug
  /// </summary>
  /// <param name="message"></param>
  /// <param name="appendNewLine"></param>
  protected void D(string message, bool appendNewLine = true)
  {
    if (!DebugMode) return;
    if(appendNewLine) Console.WriteLine($"[{DateTime.Now:O}] {message}");
    else Console.Write(message);
  }
  
  /// <summary>
  /// D for Debug, will ToString() obj if exists or print nUlL if not
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="appendNewLine"></param>
  protected void D(object? obj, bool appendNewLine = true)
  {
    if (!DebugMode) return;
    if(appendNewLine) Console.WriteLine($"[{DateTime.Now:O}] {obj?.ToString() ?? "nUlL"}");
    else Console.Write(obj?.ToString() ?? "nUlL");
  }
}