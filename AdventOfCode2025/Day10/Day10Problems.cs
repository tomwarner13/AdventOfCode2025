namespace AdventOfCode2025.Day10;

using System.Text.RegularExpressions;
using Util;
//using Z3;

public partial class Day10Problems : Problems
{
  protected override int Day => 10;

  protected override string TestInput => """
                                         [.#..#..###] (0,1,3,6,7) (0,1,2,3,8) (1,4,5,9) (0,4,6,8) (4,8,9) (2,4,7) (4,9) (1,8) { 24,27,12,16,41,8,22,29,35,19 }
                                         [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
                                         [...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
                                         [.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
                                         """;
  
  private int _cacheHits = 0;

  public override string Problem1(string[] input, bool isTestInput)
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

      //COPYPASTE THIS AND HAVE IT RETURN FALSE IF NO RESULTS AGAINST INDICATOR, ITS YOUR GOOD ILLEGAL STATE CHECKER
      //try each iteration of final buttons: 1, then 2, then 3, .... add to solution and continue when one found
      foreach (var set in CollectionUtils.GetAllCombinations(finalButtons, 1).OrderBy(s => s.Length))
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

  public override string Problem2(string[] input, bool isTestInput)
  {
    //input parsing: get all buttons and all joltage requirements

    var totalButtonPresses = 0;
    
    foreach (var line in input)
    {
      D($"starting line << {line} >> .....", force: true);
      var resultCache = new Dictionary<string, int?>();
      var constraintCache = new Dictionary<string, JoltageConstraint>();
      var illegalStatesCache = new Dictionary<string, bool>();
      var buttonCombosCache = new Dictionary<string, HashSet<int>>();
      var rawButtonWires = ButtonWiringRegex().Matches(line).Select(m => m.Groups[1].Value).ToArray();

      var buttonCombos = rawButtonWires
        .Select(w => w.Split(',').Select(int.Parse).ToArray()).ToArray();
      
      var rawJoltages = JoltageRegex().Matches(line).First().Groups[1].Value;
      var targetJoltages = rawJoltages.Split(',').Select(int.Parse).ToArray();

      var sortedButtonCombos = buttonCombos
        .OrderByDescending(bc => bc.Length)
        .ThenByDescending(bc => string.Join(",", bc))
        .ToArray();
      
      var minPresses = 
        GetLowestCombination(targetJoltages, sortedButtonCombos, ref resultCache, 
          ref constraintCache, ref illegalStatesCache, ref buttonCombosCache, isTopLevel: true);
      D();
      D(line);
      D(minPresses);
      D();

      if (minPresses == null)
        throw new ThisShouldNeverHappenException($"null minPresses: << {line} >>");

      totalButtonPresses += minPresses.Value;
      D();
      
      D($"finished line << {line} >> min: {minPresses.Value} total: {totalButtonPresses}", true, true);
    }

    return totalButtonPresses.ToString();
  }

  [GeneratedRegex(@"\[(.*)\]", RegexOptions.Compiled)]
  private static partial Regex IndicatorLightRegex();
  
  [GeneratedRegex(@"\(([\d\,]*)\)", RegexOptions.Compiled)]
  private static partial Regex ButtonWiringRegex();
  
  
  [GeneratedRegex(@"\{([\d\,]*)\}", RegexOptions.Compiled)]
  private static partial Regex JoltageRegex();
  
  
  //check combinations by starting with the one with the largest size (then highest max) at max and iterating down from there through others?
  /*
(3)->max 7  (1,3)->max 5 (2)-> max 4 (2,3)-> max 4 (0,2)-> max 3 (0,1)-> max 3 {3,5,4,7} -> min 7
(0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
(0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
   */

  private int? GetLowestCombination(int[] targetJoltages, int[][] sortedButtons,
    ref Dictionary<string, int?> comboCache, ref Dictionary<string, JoltageConstraint> constraintCache,
    ref Dictionary<string, bool> illegalStatesCache, ref Dictionary<string, HashSet<int>> buttonCombosCache,
  bool isTopLevel = false)
  {
    var cacheKey = 
      $"{string.Join(',', targetJoltages)} -- {string.Join('|', sortedButtons.Select(b => string.Join('^', b)))}";
    if (comboCache.TryGetValue(cacheKey, out var combination))
    {
      _cacheHits++;
      return combination;
    }

    if (sortedButtons.Length == 0)
    {
      //comboCache[cacheKey] = null;
      return null;
    }
    
    var availableIndexes = new HashSet<int>();
    var plausibleButtons = 
      sortedButtons.Where(b => b.All(i => targetJoltages[i] > 0)).ToList();
    
    foreach (var buttonCombo in plausibleButtons)
    {
      foreach (var b in buttonCombo) availableIndexes.Add(b);
    }

    //if none of the remaining buttons can work, stop eval
    if (targetJoltages.Where((t, i) => t > 0 && !availableIndexes.Contains(i)).Any())
    {
      //comboCache[cacheKey] = null;
      return null;
    }
    
    var sortedPlausibles = plausibleButtons
      .OrderBy(b => string.Join(',', b)).ToArray();
    var sortedPlausiblesCacheKey = string.Join('|', sortedPlausibles.Select(b => string.Join('^', b)));
    var sortedPlausiblesAsSets = sortedPlausibles.Select(b => b.ToHashSet()).ToArray();
    
    //get constraints if any for each index, check against them too
    for (var checkIndex = 0; checkIndex < targetJoltages.Length; checkIndex++)
    {
      if(targetJoltages[checkIndex] == 0) continue;

      var constraintCacheKey = $"[{checkIndex} -> {sortedPlausiblesCacheKey}]";
      if (!constraintCache.TryGetValue(constraintCacheKey, out var joltageConstraint))
      {
        joltageConstraint = new(checkIndex);
        joltageConstraint.BuildDependents(sortedPlausiblesAsSets);
        constraintCache[constraintCacheKey] = joltageConstraint;
      }

      if (!joltageConstraint.IsLegalState(targetJoltages))
      {
        //comboCache[cacheKey] = null;
        return null;
      }
    }
    
    //now try for illegal states situation
    var indicatorLightState = JoltageConstraint.GetIndicatorLightState(targetJoltages);
    var illegalStateCacheKey = $"{indicatorLightState} || {sortedPlausiblesCacheKey}";
    if (illegalStatesCache.ContainsKey(illegalStateCacheKey) && !illegalStatesCache[illegalStateCacheKey]) return null;
    
    var isLegalStateByIndicators = 
      JoltageConstraint.IsLegalStateByIndicators
        (indicatorLightState, targetJoltages.Length, sortedPlausiblesAsSets, sortedPlausiblesCacheKey, ref buttonCombosCache);
    illegalStatesCache[illegalStateCacheKey] = isLegalStateByIndicators;
    if (!isLegalStateByIndicators) return null;
    
    var bestAnswer = (int?)null;
    var firstButton = sortedButtons[0];
    
    //find target minimum answer
    var highestJoltageTarget = targetJoltages.Max();
    var totalJoltageTarget = targetJoltages.Sum();
    var highestButtonCombo = firstButton.Length;
    
    var minimumPressTarget = (int)Math.Ceiling((double)totalJoltageTarget / highestButtonCombo);
    
    var targetBestAnswer = Math.Max(highestJoltageTarget, minimumPressTarget);
    if (isTopLevel) D($"target best {targetBestAnswer}", force: true);
    if (targetBestAnswer > bestAnswer)
    {
      //comboCache[cacheKey] = null;
      return null;
    }
    
    //track best found answer
    //recurse till we find target min if any
    var maxFirstButtonPresses = targetJoltages.Where((_, i) =>
      firstButton.Contains(i)).Min();

    for (var pressesToAttempt = maxFirstButtonPresses; pressesToAttempt >= 0; pressesToAttempt--)
    {
      if (isTopLevel)
      {
        D($"attempting {pressesToAttempt} presses of button {string.Join('^', firstButton)}", force: true);
        D($"cache hits: {_cacheHits}");
      }
      var nextTargetJoltages = new int[targetJoltages.Length];
      for (var i = 0; i < targetJoltages.Length; i++)
      {
        if (pressesToAttempt > 0 && firstButton.Contains(i))
        {
          nextTargetJoltages[i] = targetJoltages[i] - pressesToAttempt;
        }
        else
        {
          nextTargetJoltages[i] = targetJoltages[i];
        }
      }
      
      if (nextTargetJoltages.All(n => n == 0))
      {
        comboCache[cacheKey] = pressesToAttempt;
        return pressesToAttempt;
      }
      
      var bestPressesRemaining = 
        GetLowestCombination(nextTargetJoltages, sortedButtons[1..],
          ref comboCache, ref constraintCache, ref illegalStatesCache, ref buttonCombosCache);
      if (bestPressesRemaining != null)
      {
        var answer = bestPressesRemaining.Value + pressesToAttempt;
        if (answer == targetBestAnswer)
        {
          comboCache[cacheKey] = answer;
          return answer;
        }

        if (answer < (bestAnswer ?? int.MaxValue))
        {
          if(isTopLevel) D($"new best answer: {answer}", force: true);
          bestAnswer = answer;
        }
      }
    }
    
    if(bestAnswer != null) comboCache[cacheKey] = bestAnswer;
    return bestAnswer;
  }
}