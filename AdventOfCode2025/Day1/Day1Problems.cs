namespace AdventOfCode2025.Day1;

using Util;

public class Day1Problems : Problems
{
  protected override int Day => 1;
  
  protected override string TestInput => @"L68
L30
R48
L5
R60
L55
L1
L99
R14
L82";

  public override string Problem1(string[] input, bool isTestInput)
  {
    var zeroesEncountered = 0;
    var currentPos = 50;

    foreach (var line in input)
    {
      var sign = line[0] == 'L' ?  -1 : 1;
      var clicks = StringUtils.ExtractIntsFromString(line).First();
      var movement = clicks * sign;
      
      currentPos = DoMovement(movement, currentPos);
      D(currentPos);
      if(currentPos == 0) zeroesEncountered++;
    }
    
    return zeroesEncountered.ToString();
  }

  private static int DoMovement(int movement, int currentPos)
  {
    var rawResult = currentPos + movement;
    if (rawResult >= 0)
    {
      return rawResult % 100;
    }
    while (rawResult < 0) rawResult += 100;
    return rawResult;
  }

  public override string Problem2(string[] input, bool isTestInput)
  {
    var zeroesEncountered = 0;
    var currentPos = 50;

    foreach (var line in input)
    {
      var sign = line[0] == 'L' ?  -1 : 1;
      var clicks = StringUtils.ExtractIntsFromString(line).First();
      var movement = clicks * sign;
      
      D(line);
      var result = DoMovementAndCountPasses(movement, currentPos);
      D($"pos: {result.finalPos}, zs: {result.zeroesPassed}");
      currentPos = result.finalPos;
      zeroesEncountered += result.zeroesPassed;
    }
    
    return zeroesEncountered.ToString();
  }
  
  private static (int finalPos, int zeroesPassed) DoMovementAndCountPasses(int movement, int currentPos)
  {
    var rawResult = currentPos + movement;
    switch (rawResult)
    {
      case 0:
        return (rawResult, 1);
      case > 0:
        return (rawResult % 100, rawResult / 100);
    }

    var  zeroesPassed = currentPos == 0 ? -1 : 0;
    while (rawResult < 0)
    {
      rawResult += 100;
      zeroesPassed++;
    }
    if(rawResult == 0) zeroesPassed++;
    return (rawResult, zeroesPassed);
  }
}