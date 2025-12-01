using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day7;

public class Day7Problems : Problems
{
  protected override string TestInput => @"190: 10 19
3267: 81 40 27
83: 17 5
156: 15 6
7290: 6 8 6 15
161011: 16 10 13
192: 17 8 14
21037: 9 7 18 13
292: 11 6 16 20";


  protected override int Day => 7;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    long totalValidLines = 0;
    var operatorPermutationsCache = new Dictionary<GridPoint, string[]>();

    foreach (var line in input)
    {
      var numbers = StringUtils.ExtractLongsFromString(line).ToArray();

      var testSolution = numbers[0];
      var parameters = numbers[1..];

      if (TestParametersWithOperators(testSolution, parameters, ref operatorPermutationsCache))
      {
        totalValidLines += testSolution;
      }
    }

    return totalValidLines.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    long totalValidLines = 0;
    var operatorPermutationsCache = new Dictionary<GridPoint, string[]>();
    var concatenatorPermutationsCache = new Dictionary<GridPoint, string[]>();

    foreach (var line in input)
    {
      var numbers = StringUtils.ExtractLongsFromString(line).ToArray();

      var testSolution = numbers[0];
      var parameters = numbers[1..];

      if (TestParametersWithConcats(testSolution, parameters, ref operatorPermutationsCache, ref concatenatorPermutationsCache))
      {
        totalValidLines += testSolution;
        if(isTestInput) Console.WriteLine(testSolution);
      }
    }

    return totalValidLines.ToString();
  }

  private static bool TestParametersWithOperators(long testSolution, long[] parameters,
    ref Dictionary<GridPoint, string[]> operatorPermutations)
  {
    if(parameters.Length == 1) return testSolution == parameters[0];
    
    var testNumberOfMultipliers = 0;
    var allSolutionsAreGreater = false;
    var totalPossibleMultipliers = parameters.Length - 1;

    while (testNumberOfMultipliers <= totalPossibleMultipliers && !allSolutionsAreGreater)
    {
      var permutations = GetPermutations(parameters.Length - 1, testNumberOfMultipliers, ref operatorPermutations);
      var areAllGreater = true;

      foreach (var permutation in permutations)
      {
        var result = TestOnePermutation(parameters, permutation);
        if (result == testSolution) return true;
        if (result < testSolution) areAllGreater = false;
      }
      
      allSolutionsAreGreater = areAllGreater;
      testNumberOfMultipliers++;
    }
    
    return false;
  }

  private static long TestOnePermutation(long[] parameters, string permutation)
  {
    long acc = parameters[0];
    for (var i = 0; i < parameters.Length - 1; i++)
    {
      var operation = permutation[i];
      switch (operation)
      {
        case '+':
          acc += parameters[i + 1];
          break;
        case '*':
          acc *= parameters[i + 1];
          break;
      }
    }
    return acc;
  }

  private static bool TestParametersWithConcats(long testSolution, long[] parameters,
    ref Dictionary<GridPoint, string[]> operatorPermutations, ref Dictionary<GridPoint, string[]> concatenatorPermutations)
  {
    for (var totalConcatsToTry = 0; totalConcatsToTry < parameters.Length; totalConcatsToTry++)
    {
      var concatPermutations = GetConcatenatorPermutations(parameters.Length - 1, totalConcatsToTry, ref concatenatorPermutations, ref operatorPermutations);
      foreach (var concatPermutation in concatPermutations)
      {
        var result = ConcatOnePermutation(parameters, concatPermutation);
        if (result == testSolution) return true;
      }
    }

    return false;
  }

  private static long ConcatOnePermutation(long[] parameters, string permutation)
  {
    var acc = parameters[0];
    for (var i = 0; i < parameters.Length - 1; i++)
    {
      var operation = permutation[i];
      switch (operation)
      {
        case '+':
          acc += parameters[i + 1];
          break;
        case '*':
          acc *= parameters[i + 1];
          break;
        case '|':
          acc = long.Parse(acc +  parameters[i + 1].ToString());
          break;
      }
    }
    return acc;
  }

  private static IEnumerable<string> GetPermutations(int length, int totalMultiplications,
    ref Dictionary<GridPoint, string[]> operatorPermutations)
  {
    if(length < totalMultiplications) throw new ArgumentException($"oops, asked to include {totalMultiplications} in length {length}");
    
    var lookupPoint = new GridPoint(length, totalMultiplications);

    if (operatorPermutations.TryGetValue(lookupPoint, out var permutations))
      return permutations;

    if (totalMultiplications == 0)
    {
      operatorPermutations[lookupPoint] = new string[] { new('+', length) };
      return operatorPermutations[lookupPoint];
    }
    
    if (totalMultiplications == length)
    {
      operatorPermutations[lookupPoint] = new string[] { new('*', length) };
      return operatorPermutations[lookupPoint];
    }

    var results = new List<string>();

    foreach (var permutation in GetPermutations(length - 1, totalMultiplications, ref operatorPermutations))
    {
      results.Add('+' + permutation);
    }
    
    foreach (var permutation in GetPermutations(length - 1, totalMultiplications - 1, ref operatorPermutations))
    {
      results.Add('*' + permutation);
    }
    
    var resultArray = results.ToArray();
    operatorPermutations[lookupPoint] = resultArray;
    return resultArray;
  }

  private static IEnumerable<string> GetConcatenatorPermutations(int length, int totalConcats,
    ref Dictionary<GridPoint, string[]> concatPermutations, ref Dictionary<GridPoint, string[]> operatorPermutations)
  {
    if(length < totalConcats) throw new ArgumentException($"oops, asked to include {totalConcats} cats in length {length}");
    
    var lookupPoint = new GridPoint(length, totalConcats);
    
    if (concatPermutations.TryGetValue(lookupPoint, out var permutations))
      return permutations;
    
    if (totalConcats == 0)
    {
      var concatsToAdd = new List<string>();
      for (var totalMults = 0; totalMults <= length; totalMults++)
      {
        concatsToAdd.AddRange(GetPermutations(length, totalMults, ref operatorPermutations));
      }
      var concatsArr = concatsToAdd.ToArray();
      
      concatPermutations[lookupPoint] = concatsArr;
      return concatsArr;
    }
    
    if (totalConcats == length)
    {
      concatPermutations[lookupPoint] = new string[] { new('|', length) };
      return concatPermutations[lookupPoint];
    }
    
    var results = new List<string>();

    foreach (var permutation in GetConcatenatorPermutations(length - 1, totalConcats, ref concatPermutations, ref operatorPermutations))
    {
      results.Add('+' + permutation);
      results.Add('*' + permutation);
    }
    
    foreach (var permutation in GetConcatenatorPermutations(length - 1, totalConcats - 1, ref concatPermutations, ref operatorPermutations))
    {
      results.Add('|' + permutation);
    }
    
    var resultArray = results.ToArray();
    concatPermutations[lookupPoint] = resultArray;
    return resultArray;
  }
}