namespace AdventOfCode2025.Day10;

using System.Text.RegularExpressions;
using Util;

public partial class Day10Problems : Problems
{
  protected override int Day => 10;

  protected override string TestInput => """
                                         [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
                                         [...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
                                         [.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
                                         """;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var solution = 0L;

    foreach (var line in input)
    {
      var rawIndicatorLights = IndicatorLightRegex().Matches(line).First().Groups[1].Value;
      D(rawIndicatorLights);

      var rawButtonWires = ButtonWiringRegex().Matches(line).Select(m => m.Groups[1].Value).ToArray();
      
      D(string.Join(" | ", rawButtonWires));
      
      var buttonCombos = rawButtonWires.Select(w => w.Split(',').Select(int.Parse));

      var requiredIndicatorStateInt = 0;
      for (var i = 0; i < rawIndicatorLights.Length; i++)
      {
        if (rawIndicatorLights[rawIndicatorLights.Length - 1 - i] == '#') requiredIndicatorStateInt += 1 << i;
      }

      var buttonsToTry = 0;
      var rangeWidth = rawIndicatorLights.Length - 1;
      var finalButtons = 
        buttonCombos.Select(buttonCombo 
            => buttonCombo.Sum(button => 1 << (rangeWidth - button)))
          .ToList();

      //try each iteration of final buttons: 1, then 2, then 3, .... add to solution and continue when one found
      foreach (var set in GetAllCombinations(finalButtons, 1).OrderBy(s => s.Length))
      {
        var testVal = set.Aggregate(0, (current, button) => current ^ button);
        if (testVal != requiredIndicatorStateInt) continue;
        D($"adding {set.Length} to solution, permutation was {string.Join(',', set)}");
        solution += set.Length;
        break;
      }
    }

    return solution.ToString();
  }
  
  //https://stackoverflow.com/a/57058345/5503076 :goat:
  //TODO move me to a util class
  //also not combinations not permutations -> if you give it ([0,1], 0, 2) you'll get [0], [1], [0,1] but not [1,0]
  //also note not sorted in order of set size
  public static IEnumerable<T[]> GetAllCombinations<T>(IEnumerable<T> source, int minCombinationSize = 0, int maxCombinationSize = 0) 
  {
    ArgumentNullException.ThrowIfNull(source);

    var data = source.ToArray();
    if(maxCombinationSize == 0) maxCombinationSize = data.Length;

    return Enumerable
      .Range(minCombinationSize, 1 << (maxCombinationSize))
      .Select(index => data
        .Where((v, i) => (index & (1 << i)) != 0)
        .ToArray());
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    throw new NotImplementedException();
  }

  [GeneratedRegex(@"\[(.*)\]", RegexOptions.Compiled)]
  private static partial Regex IndicatorLightRegex();
  
  [GeneratedRegex(@"\(([\d\,]*)\)", RegexOptions.Compiled)]
  private static partial Regex ButtonWiringRegex();
}