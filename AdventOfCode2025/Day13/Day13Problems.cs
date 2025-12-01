using AdventOfCode2025.Util;
using Newtonsoft.Json.Linq;

namespace AdventOfCode2025.Day13;

public class Day13Problems : Problems
{
  protected override string TestInput => @"Button A: X+94, Y+34
Button B: X+22, Y+67
Prize: X=8400, Y=5400

Button A: X+26, Y+66
Button B: X+67, Y+21
Prize: X=12748, Y=12176

Button A: X+17, Y+86
Button B: X+84, Y+37
Prize: X=7870, Y=6450

Button A: X+69, Y+23
Button B: X+27, Y+71
Prize: X=18641, Y=10279";

  protected override int Day => 13;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var totalWinningsCost = 0L;
    const int maxMoves = 100;

    for (var i = 0; i < input.Length; i += 4)
    {
      var aInput = StringUtils.ExtractIntsFromString(input[i]).ToArray();
      var bInput = StringUtils.ExtractIntsFromString(input[i + 1]).ToArray();
      var pInput = StringUtils.ExtractIntsFromString(input[i + 2]).ToArray();
      
      var aX = aInput[0];
      var aY = aInput[1];
      var bX = bInput[0];
      var bY = bInput[1];
      var pX = pInput[0];
      var pY = pInput[1];
      
      totalWinningsCost += GetLowestTotalCost(aX, aY, bX, bY, pX, pY, maxMoves) ?? 0;
    }
    
    return totalWinningsCost.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var totalWinningsCost = 0L; //test response should be 875318608908
    const long clawCompensator = 10000000000000;

    for (var i = 0; i < input.Length; i += 4)
    {
      var aInput = StringUtils.ExtractIntsFromString(input[i]).ToArray();
      var bInput = StringUtils.ExtractIntsFromString(input[i + 1]).ToArray();
      var pInput = StringUtils.ExtractIntsFromString(input[i + 2]).ToArray();
      
      var aX = aInput[0];
      var aY = aInput[1];
      var bX = bInput[0];
      var bY = bInput[1];
      var pX = pInput[0];
      var pY = pInput[1];
      
      totalWinningsCost += GetLowestTotalCostUsingLinearSubstitution(aX, aY, bX, bY, pX, pY, clawCompensator) ?? 0;
    }
    
    return totalWinningsCost.ToString();
  }

  private static long? GetLowestTotalCostUsingLinearSubstitution(int aX, int aY, int bX, int bY, int pXraw, int pYraw,
    long clawOffset = 0)
  {
    var pX = pXraw + clawOffset;
    var pY = pYraw + clawOffset;

    var possibleSolutionForB = ((pY * aX) - (pX * aY)) / ((bY * aX) - (bX * aY)); //math is crazy lmao rip
    var possibleSolutionForA = (pX - (possibleSolutionForB * bX)) / aX;

    var xDist = possibleSolutionForA * aX + possibleSolutionForB * bX;
    var yDist = possibleSolutionForA * aY + possibleSolutionForB * bY;
    if (xDist == pX && yDist == pY)
    {
      return (possibleSolutionForA * 3) + possibleSolutionForB;
    }

    return null;
  }

  private static long? GetLowestTotalCost(int aX, int aY, int bX, int bY, int pXraw, int pYraw, int maxMoves = 0, long clawOffset = 0)
  {
    long aTotal = aX + aY;
    long bTotal = bX + bY;

    var pX = pXraw + clawOffset;
    var pY = pYraw + clawOffset;
    
    var pTotal = pX + pY;

    var extendedEuclideanResult = ExtendedEuclidean(aTotal, bTotal);
    
    //is a solution even possible?
    if(pTotal % extendedEuclideanResult.gcd != 0) return null;
    
    //find a working solution point that's possibly wildly out of bounds
    var pointX = extendedEuclideanResult.x * pTotal / extendedEuclideanResult.gcd;
    
    var solutionA = pointX % (bTotal / extendedEuclideanResult.gcd);
    var solutionB = (pTotal - (aTotal * solutionA))/bTotal;
    
    long? lowestTotalCost = null;

    while (solutionB >= 0)
    {
      //validate solution
      var validSolution = maxMoves > 0 ?
        solutionA >= 0 && solutionA <= maxMoves && solutionB <= maxMoves :
        solutionA >= 0;
      
      if (validSolution)
      {
        var xDist = solutionA * aX + solutionB * bX;
        var yDist = solutionA * aY + solutionB * bY;
        if (xDist == pX && yDist == pY)
        {
          var cost = (solutionA * 3) + solutionB;
          if(lowestTotalCost == null || cost < lowestTotalCost)
            lowestTotalCost = cost;
        }
      }

      solutionA += bTotal / extendedEuclideanResult.gcd;
      solutionB -= aTotal / extendedEuclideanResult.gcd;
    }
    
    return lowestTotalCost;
  }

  private static (long gcd, long x, long y) ExtendedEuclidean(long a, long b)
  {
    if (a == 0) return (b, 0, 1);
    
    var recursiveResult = ExtendedEuclidean(b % a, a);
    
    var x = recursiveResult.y - (b / a) * recursiveResult.x;
    var y = recursiveResult.x;
 
    return (recursiveResult.gcd, x, y);
  }
}