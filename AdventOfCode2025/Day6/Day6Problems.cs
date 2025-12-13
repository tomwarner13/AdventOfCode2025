namespace AdventOfCode2025.Day6;

using Util;

public class Day6Problems : Problems
{
  protected override int Day => 6;
  
  protected override string TestInput => @"123 328  51 64 
 45 64  387 23 
  6 98  215 314
*   +   *   +  ";

  public override string Problem1(string[] input, bool isTestInput)
  {
    var columns = input.Last()
      .Split(' ', StringSplitOptions.RemoveEmptyEntries)
      .Select(o => new MathColumn(o[0])).ToArray();

    foreach (var line in input[..^1])
    {
      var numbers = StringUtils.ExtractLongsFromString(line).ToArray();

      for (var i = 0; i < numbers.Length; i++)
      {
        columns[i].Operate(numbers[i]);
        D($"column {i}: current total {columns[i].Total}");
      }
    }
    
    return columns.Select(c => c.Total).Sum().ToString();
  }

  public override string Problem2(string[] input, bool isTestInput)
  {
    var currentColumn = new MathColumn('+');
    var total = 0L;
    var opStr = input.Last();

    for (var x = 0; x < input[0].Length; x++)
    {
      var testOp = opStr[x];
      if (testOp != ' ') //resolve and create new column 
      {
        D($"adding {currentColumn.Total} to {total}");
        total += currentColumn.Total;
        currentColumn = new(testOp);
      }

      var numberAcc = string.Empty;
      for(var y = 0; y < input.Length - 1; y++)
      {
        var c = input[y][x];
        if (c != ' ') numberAcc += c;
      }

      if (numberAcc != string.Empty)
      {
        D(numberAcc);
        currentColumn.Operate(long.Parse(numberAcc));
      }
    }
    
    //resolve last column since it doesn't bump to a new one
    D($"adding {currentColumn.Total} to {total}");
    total += currentColumn.Total;
    
    return total.ToString();
  }

  private class MathColumn
  {
    public long Total { get; private set; }
    private readonly char _operator;
    private readonly Action<long> _operation;

    public MathColumn(char operatorChar)
    {
      switch (operatorChar)
      {
        case '+':
          _operation = n => { Total += n; };
          Total = 0;
          break;
        case '*':
          _operation = n => { Total *= n; };
          Total = 1;
          break;
        default:
          throw new ThisShouldNeverHappenException($"wtf op {operatorChar}");
      }
      _operator = operatorChar;
    }

    public void Operate(long number)
    {
      _operation(number);
    }
    
    public override string ToString() => $"{_operator} {Total}";
  }
}