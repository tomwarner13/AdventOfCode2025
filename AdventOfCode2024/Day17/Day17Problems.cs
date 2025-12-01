using System.Text;
using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day17;

public class Day17Problems : Problems
{
  protected override string TestInput => @"Register A: 729
Register B: 0
Register C: 0

Program: 0,1,5,4,3,0";

  protected override int Day => 17;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var registerA = StringUtils.ExtractIntsFromString(input[0]).First();
    var registerB = StringUtils.ExtractIntsFromString(input[1]).First();
    var registerC = StringUtils.ExtractIntsFromString(input[2]).First();
    
    var instructions = StringUtils.ExtractLongsFromString(input[4]).ToArray();
    
    var compy = new ChronospatialComputer(registerA, registerB, registerC, instructions);
    var output = compy.Operate();
    return string.Join(',', output.Select(n => n.ToString()));
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    if (isTestInput)
      input = new[]
      {
        "0",
        "0",
        "0",
        "",
        "0,3,5,4,3,0"
      };
    
    var instructions = StringUtils.ExtractLongsFromString(input[4]).ToArray();
    
    var printer = new StringBuilder();
    
    var compy = new ChronospatialComputer(0, 0, 0, instructions);
    var output = compy.FindLowestQuineInput();
    //printer.AppendLine(string.Join(',', output.Select(n => n.ToString())));
    printer.AppendLine(output.ToString());
    return printer.ToString();
  }

  private class ChronospatialComputer
  {
    public long RegisterA { get; set; }
    public long RegisterB { get; set; }
    public long RegisterC { get; set; }

    private long[] _input;
    private long _pointer;
    private List<long> _output = new();
    private StringBuilder? _printer = null;

    public ChronospatialComputer(long a, long b, long c, long[] input, StringBuilder? printer = null)
    {
      RegisterA = a;
      RegisterB = b;
      RegisterC = c;
      _input = input;
      _pointer = 0;
      _printer = printer;
    }

    public long FindLowestQuineInput()
    {
      var result = FindLowestQuine(new List<long>());
      return GetLongFromOctalList(result ?? new List<long>(), _input.Length);
    }

    private List<long>? FindLowestQuine(List<long> knownValues)
    {
      for (var testDigit = 0; testDigit < 8; testDigit++)
      {
        var testValues = new List<long>(knownValues) { testDigit };

        var inputLong = GetLongFromOctalList(testValues, _input.Length);
        var inputResult = OperateAndCleanup(inputLong);
        if(inputResult.SequenceEqual(_input)) 
          return testValues;
        if(inputResult.Length != _input.Length) continue;
        
        //check if digits at significance are correct, if so, recurse
        var shouldRecurse = true;
        for (var i = 1; i <= testValues.Count; i++)
        {
          var lastIndexToCheck = inputResult.Length - i;
          if (inputResult[lastIndexToCheck] != _input[lastIndexToCheck])
          {
            shouldRecurse = false;
            break;
          }
        }

        if (shouldRecurse)
        {
          var recursionResult = FindLowestQuine(testValues);
          if(recursionResult != null) 
            return recursionResult;
        }
      }

      return null;
    }

    private void Reset()
    {
      RegisterA = 0;
      RegisterB = 0;
      RegisterC = 0;
      _pointer = 0;
      _output.Clear();
    }

    public List<long> Operate()
    {
      _printer?.AppendLine($"init: A: {RegisterA} B: {RegisterB} C: {RegisterC}");
      while (_pointer < _input.Length)
      {     
        var opcode = _input[_pointer];
        var rawOperand = _input[_pointer + 1]; //could this throw? probably lol
        Iterate(opcode, rawOperand);
      }
      
      //halt when trying to read opcode past end of program
      return _output;
    }

    private long[] OperateAndCleanup(long startingA)
    {
      RegisterA = startingA;
      Operate();
      var result = _output.ToArray();
      Reset();
      return result;
    }

    private void Iterate(long opcode, long rawOperand)
    {
      switch (opcode)
      {
        case 0:
          Adv(rawOperand);
          break;
        case 1:
          Bxl(rawOperand);
          break;
        case 2:
          Bst(rawOperand);
          break;
        case 3:
          Jnz(rawOperand);
          break;
        case 4:
          Bxc(rawOperand);
          break;
        case 5:
          Out(rawOperand);
          break;
        case 6:
          Bdv(rawOperand);
          break;
        case 7:
          Cdv(rawOperand);
          break;
        default:
          throw new ThisShouldNeverHappenException($"opcode out of range {opcode}");
      }
    }

    private void Adv(long rawOperand)
    {
      RegisterA = (long)(RegisterA / Math.Pow(2, ComboOperand(rawOperand)));
      _pointer += 2;
      PrintState("adv", rawOperand, ComboOperand(rawOperand));
    }
    
    private void Bxl(long rawOperand)
    {
      RegisterB ^= rawOperand;
      _pointer += 2;
      PrintState("bxl", rawOperand);
    }
    
    private void Bst(long rawOperand)
    {
      RegisterB = ComboOperand(rawOperand) % 8;
      _pointer += 2;
      PrintState("bst", rawOperand, ComboOperand(rawOperand));
    }
    
    private void Jnz(long rawOperand)
    {
      if (RegisterA == 0)
      {
        _pointer += 2;
        PrintState("jnz", rawOperand);
        return;
      }

      _pointer = rawOperand;
      PrintState("jnz", rawOperand);
    }
    
    private void Bxc(long rawOperand)
    {
      RegisterB ^= RegisterC;
      _pointer += 2;
      PrintState("bxc", rawOperand);
    }
    
    private void Out(long rawOperand)
    {
      _output.Add((long)(ComboOperand(rawOperand) % 8));
      _pointer += 2;
      PrintState("out", rawOperand, ComboOperand(rawOperand));
    }
    
    private void Bdv(long rawOperand)
    {
      RegisterB = (long)(RegisterA / Math.Pow(2, ComboOperand(rawOperand)));
      _pointer += 2;
      PrintState("bdv", rawOperand, ComboOperand(rawOperand));
    }
    
    private void Cdv(long rawOperand)
    {
      RegisterC = (long)(RegisterA / Math.Pow(2, ComboOperand(rawOperand)));
      _pointer += 2;
      PrintState("cdv", rawOperand, ComboOperand(rawOperand));
    }

    private long ComboOperand(long rawOperand)
    {
      if (rawOperand is < 0 or > 6)
        throw new ThisShouldNeverHappenException($"operand out of range: {rawOperand}");
      
      if (rawOperand <= 3)
        return rawOperand;

      if (rawOperand == 4)
        return RegisterA;
      
      if (rawOperand == 5)
        return RegisterB;
      
      if (rawOperand == 6)
        return RegisterC;
      
      throw new ThisShouldNeverHappenException($"operand impossibly out of range: {rawOperand}");
    }

    private void PrintState(string op, long rawOperand, long? operand = null)
    {
      _printer?.AppendLine($"Ptr: {_pointer} A: {RegisterA} B: {RegisterB} C: {RegisterC} LastOp: {op}: Raw: {rawOperand} Operand: {operand ?? rawOperand}");
    }

    //i'm doing leftpad without calling a dependency here, Node devs devastated
    private static long GetLongFromOctalList(List<long> octalValues, long numDigits)
    {
      var result = 0L;
      numDigits--;
      foreach (var octalValue in octalValues)
      {
        result += octalValue * (long)Math.Pow(8, numDigits);
        numDigits--;
      }

      return result;
    }
  }
}