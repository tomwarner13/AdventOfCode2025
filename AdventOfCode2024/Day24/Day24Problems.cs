using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day24;

public class Day24Problems : Problems
{
  protected override string TestInput => @"x00: 1
x01: 0
x02: 1
x03: 1
x04: 0
y00: 1
y01: 1
y02: 1
y03: 1
y04: 1

ntg XOR fgs -> mjb
y02 OR x01 -> tnw
kwq OR kpj -> z05
x00 OR x03 -> fst
tgd XOR rvg -> z01
vdt OR tnw -> bfw
bfw AND frj -> z10
ffh OR nrd -> bqk
y00 AND y03 -> djm
y03 OR y00 -> psh
bqk OR frj -> z08
tnw OR fst -> frj
gnj AND tgd -> z11
bfw XOR mjb -> z00
x03 OR x00 -> vdt
gnj AND wpb -> z02
x04 AND y00 -> kjc
djm OR pbm -> qhw
nrd AND vdt -> hwm
kjc AND fst -> rvg
y04 OR y02 -> fgs
y01 AND x02 -> pbm
ntg OR kjc -> kwq
psh XOR fgs -> tgd
qhw XOR tgd -> z09
pbm OR djm -> kpj
x03 XOR y03 -> ffh
x00 XOR y04 -> ntg
bfw OR bqk -> z06
nrd XOR fgs -> wpb
frj XOR qhw -> z04
bqk OR frj -> z07
y03 OR x01 -> nrd
hwm AND bqk -> z03
tgd XOR rvg -> z12
tnw OR pbm -> gnj";

  protected override int Day => 24;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    DebugMode = isTestInput;
    var knownWireStatus = new Dictionary<string, bool>();
    var allGates = new Dictionary<string, List<Gate>>();
    var zGates = new Dictionary<string, bool>();

    var parsingDefaults = true;

    foreach (var line in input)
    {
      if (parsingDefaults)
      {
        if(string.IsNullOrWhiteSpace(line)) parsingDefaults = false;
        else
        {
          var parts = line.Split(": ");
          knownWireStatus[parts[0]] = parts[1] == "1";
        }
      }
      else
      {
        var data = StringUtils.ExtractAlphanumericsFromString(line).ToArray();
        var gateType = GetGateType(data[1]);
        var gateDest = data[3];
        var gate = new Gate(gateType, gateDest);
        
        if(knownWireStatus.TryGetValue(data[0], out var value1)) gate.UpdateValueAndTryResolve(value1);
        if(knownWireStatus.TryGetValue(data[2], out var value2)) gate.UpdateValueAndTryResolve(value2);
        
        if(allGates.TryGetValue(data[0], out var gates1)) gates1.Add(gate);
        else allGates.Add(data[0], [gate]);
        if(allGates.TryGetValue(data[2], out var gates2)) gates2.Add(gate);
        else allGates.Add(data[2], [gate]);
        
        var gatesToTry = new Queue<Gate>();
        if(gate.HasResolved) gatesToTry.Enqueue(gate);
        while (gatesToTry.TryDequeue(out var currentGate))
        {
          knownWireStatus[currentGate.OutputWire] = currentGate.Output;
          if(currentGate.OutputWire.Contains('z')) zGates[currentGate.OutputWire] = currentGate.Output;
          
          if (allGates.TryGetValue(currentGate.OutputWire, out var nextGates))
          {
            foreach (var gateToTry in nextGates.Where(g => !knownWireStatus.ContainsKey(g.OutputWire)))
            {
              if (gateToTry.UpdateValueAndTryResolve(currentGate.Output))
              {
                D($"resolved {currentGate.OutputWire} to {gateToTry.OutputWire}");
                gatesToTry.Enqueue(gateToTry);
              }
              else
              {
                D($"did not resolve gateToTry {currentGate.OutputWire} to {gateToTry.OutputWire}");
              }
            }
          }
          else
          {
            D($"did not resolve {currentGate.OutputWire}");
          }
        }
      }
    }

    var pow = 0;
    var result = 0L;
    foreach (var zGate in zGates.OrderBy(g => g.Key))
    {
      result += zGate.Value ? (long)Math.Pow(2, pow) : 0;
      pow++;
    }
    
    return result.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    if (isTestInput) throw new NotImplementedException("test");
    DebugMode = true;
    var knownWireStatus = new Dictionary<string, bool>();
    var allGates = new Dictionary<string, List<Gate>>();
    var zGates = new Dictionary<string, bool>();
    var primaryLines = new LineInfo[90];
    var secondaryLines = new Dictionary<string, List<LineInfo>>();
    var printedLines = new HashSet<string>();
    var unprintedLines = new HashSet<string>();

    var parsingDefaults = true;
    var defaultXVal = true;
    var defaultYVal = false;
    var expectedZZeroVal = defaultXVal ^ defaultYVal;
    var expectedZUpVal = defaultXVal || defaultYVal;
    //known good Zs: 0-11
    
    foreach (var line in input)
    {
      if (parsingDefaults)
      {
        if(string.IsNullOrWhiteSpace(line)) parsingDefaults = false;
        else
        {
          var parts = line.Split(": ");
          knownWireStatus[parts[0]] = parts[0].Contains('x') ? defaultXVal : defaultYVal;
        }
      }
      else
      {
        var data = StringUtils.ExtractAlphanumericsFromString(line).ToArray();
        var gateType = GetGateType(data[1]);
        var gateDest = data[3];
        var gate = new Gate(gateType, gateDest);
        var lineInfo = new LineInfo(line);
        unprintedLines.Add(lineInfo.ToString());
        
        if(lineInfo.IsPrimary)
          primaryLines[lineInfo.GetPrecedence()] = lineInfo;
        else
        {
          if(secondaryLines.TryGetValue(data[0], out var sec1)) sec1.Add(lineInfo);
          else secondaryLines.Add(data[0], [lineInfo]);
          if(secondaryLines.TryGetValue(data[2], out var sec2)) sec2.Add(lineInfo);
          else secondaryLines.Add(data[2], [lineInfo]);
        }
        
        if(knownWireStatus.TryGetValue(data[0], out var value1)) gate.UpdateValueAndTryResolve(value1);
        if(knownWireStatus.TryGetValue(data[2], out var value2)) gate.UpdateValueAndTryResolve(value2);
        
        if(allGates.TryGetValue(data[0], out var gates1)) gates1.Add(gate);
        else allGates.Add(data[0], [gate]);
        if(allGates.TryGetValue(data[2], out var gates2)) gates2.Add(gate);
        else allGates.Add(data[2], [gate]);
        
        var gatesToTry = new Queue<Gate>();
        if(gate.HasResolved) gatesToTry.Enqueue(gate);
        while (gatesToTry.TryDequeue(out var currentGate))
        {
          knownWireStatus[currentGate.OutputWire] = currentGate.Output;
          if(currentGate.OutputWire.Contains('z')) zGates[currentGate.OutputWire] = currentGate.Output;
          
          if (allGates.TryGetValue(currentGate.OutputWire, out var nextGates))
          {
            foreach (var gateToTry in nextGates.Where(g => !knownWireStatus.ContainsKey(g.OutputWire)))
            {
              if (gateToTry.UpdateValueAndTryResolve(currentGate.Output))
              {
                D($"resolved {currentGate.OutputWire} to {gateToTry.OutputWire}");
                gatesToTry.Enqueue(gateToTry);
              }
              else
              {
                D($"did not resolve gateToTry {currentGate.OutputWire} to {gateToTry.OutputWire}");
              }
            }
          }
          else
          {
            D($"did not resolve {currentGate.OutputWire}");
          }
        }
      }
    }
    
    //here's the plan: as we parse input, building in-order set of the primary inputs (AND then XOR each)
    //output each one, mark its outputs as available for printing
    //go through the available-for-printing set in order (alphabetically by output gate?) and print those
    //when done, go to next primary input
    foreach (var primaryLine in primaryLines)
    {
      D(primaryLine.ToString());
      printedLines.Add(primaryLine.ToString());
      unprintedLines.Remove(primaryLine.ToString());
      var output = primaryLine.OutputWire;
      var outputLines = new Queue<LineInfo>();
      foreach (var connection in secondaryLines.GetValueOrDefault(output) ?? [])
      {
        if(!printedLines.Contains(connection.ToString()) 
           && connection.ResolveInput(output)) outputLines.Enqueue(connection);
      }

      while (outputLines.TryDequeue(out var printableLine))
      {
        D(printableLine.ToString());
        printedLines.Add(printableLine.ToString());
        unprintedLines.Remove(printableLine.ToString());
        var newOutput = printableLine.OutputWire;
        foreach (var connection in secondaryLines.GetValueOrDefault(newOutput) ?? [])
        {
          if(!printedLines.Contains(connection.ToString()) 
             && connection.ResolveInput(newOutput)) outputLines.Enqueue(connection);
        }
      }
    }
    
    D();
    D("UNPRINTED LINES: -------------");
    foreach (var line in unprintedLines)
    {
      D(line);
    }

    var pow = 0;
    var result = 0L;
    foreach (var zGate in zGates.OrderBy(g => g.Key))
    {
      if(zGate.Value != (pow == 0 ? expectedZZeroVal : expectedZUpVal)) 
        throw new InvalidOperationException($"found unexpected value at Z {zGate.Key}");
      
      result += zGate.Value ? (long)Math.Pow(2, pow) : 0;
      pow++;
    }
    
    return result.ToString();
  }

  private static GateType GetGateType(string input)
    => input switch
    {
      "AND" => GateType.And,
      "OR" => GateType.Or,
      "XOR" => GateType.Xor,
      _ => throw new ArgumentException($"Unknown gate type: {input}"),
    };

  private class Gate(GateType gateType, string outputWire)
  {
    private bool? _val1;
    private bool? _val2;
    private bool _output;
    public bool HasResolved { get; private set; }

    public bool Output
    {
      get
      {
        if (HasResolved) return _output;
        throw new ThisShouldNeverHappenException("tried to read unresolved gate");
      }
    }

    public readonly string OutputWire = outputWire;

    public bool UpdateValueAndTryResolve(bool inputValue)
    {
      if (_val1 == null)
      {
        _val1 = inputValue;
        return false;
      }

      if (_val2 == null)
      {
        _val2 = inputValue;
        Resolve();
        return true;
      }

      throw new ThisShouldNeverHappenException("can't update already resolved gate");
    }

    private void Resolve()
    {
      _output = gateType switch
      {
        GateType.Or => _val1!.Value || _val2!.Value,
        GateType.And => _val1!.Value && _val2!.Value,
        GateType.Xor => _val1!.Value ^ _val2!.Value,
        _ => throw new ThisShouldNeverHappenException()
      };
      HasResolved = true;
    }
  }
    
  private class LineInfo
  {
    public readonly string Input1;
    public readonly string Input2;
    public readonly GateType Operation;
    public readonly string OutputWire;
    public readonly bool IsPrimary;
    public readonly bool IsOutput;
    private readonly string _stringOutput;
    public bool Input1Printed { get; private set; }
    public bool Input2Printed { get; private set; }

    public LineInfo(string line)
    {
      var data = StringUtils.ExtractAlphanumericsFromString(line).ToArray();
      Operation = GetGateType(data[1]);
      Input1 = data[0];
      Input2 = data[2];
      OutputWire = data[3];
      IsPrimary = data[0].StartsWith('x') || data[0].StartsWith('y');
      IsOutput = data[3].StartsWith('z');
      var primarySuffix = IsPrimary ? " [PRIM]" : "";
      var outputSuffix = IsOutput ? " [OUT]" : "";
      _stringOutput = $"{Input1} {data[1]} {Input2} -> {OutputWire}{primarySuffix}{outputSuffix} ";
    }

    public bool ResolveInput(string input)
    {
      if(input == Input1) Input1Printed = true;
      else if (input == Input2) Input2Printed = true;
      else throw new ThisShouldNeverHappenException($"can't resolve {input} with {Input1} or {Input2}");
      
      return Input1Printed && Input2Printed;
    }

    public override string ToString()
      => _stringOutput;

    public int GetPrecedence()
    {
      if(!IsPrimary) throw new ThisShouldNeverHappenException("can't precedence secondary");
      var index = int.Parse(Input1[1..]);
      var baseI = index * 2;
      return Operation == GateType.Xor ? baseI : baseI + 1;
    }
  }

  private enum GateType
  {
    Or,
    And,
    Xor
  }
}