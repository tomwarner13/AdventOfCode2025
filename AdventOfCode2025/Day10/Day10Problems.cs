namespace AdventOfCode2025.Day10;

using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Util;

public partial class Day10Problems : Problems
{
  protected override int Day => 10;

  protected override string TestInput => """
                                         [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
                                         [...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
                                         [.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
                                         """;
  
  private int _cacheHits = 0;
  private readonly MemoryCache _cache = new(new MemoryCacheOptions
  {
    SizeLimit = 1000000,
  });

  private static MemoryCacheEntryOptions _defaultSizeOptions = new()
  {
    Size = 1
  };

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
    var totalButtonPresses = 0;
    
    foreach (var line in input)
    {
      D($"starting line << {line} >> .....", force: true);
      
      var rawButtonWires = ButtonWiringRegex().Matches(line).Select(m => m.Groups[1].Value).ToArray();

      
      var rawJoltages = JoltageRegex().Matches(line).First().Groups[1].Value;
      var targetJoltages = rawJoltages.Split(',').Select(int.Parse).ToArray();
      
      
      var buttonCombos = rawButtonWires
        .Select(w => w.Split(',').Select(int.Parse).ToArray())
        .Select(b => DayTenProblemUtilities.ButtonToInt(b, targetJoltages.Length))
        .ToArray();

      var minPresses = GetLowestCombinationWithHalfSplits(targetJoltages, buttonCombos);
      
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

  //https://aoc.winslowjosiah.com/solutions/2025/day/10/
  public int? GetLowestCombinationWithHalfSplits(int[] targetJoltages, int[] buttons)
  {
    if (targetJoltages.All(i => i == 0)) return 0;
      
    var indicatorLightState = DayTenProblemUtilities.GetIndicatorLightState(targetJoltages);
    var buttonCombosCacheKey = "button_combos | " + string.Join(",", buttons);
    var buttonCombos = _cache.GetOrCreate(buttonCombosCacheKey,
      _ => DayTenProblemUtilities.GetAllButtonCombinations(buttons), _defaultSizeOptions)!;

    var plausibleButtonCombos =
      buttonCombos.Where(b => CanReachTargetStateCached(indicatorLightState, b));

    int? bestAnswer = null;

    foreach (var combo in plausibleButtonCombos)
    {
      var pressesToEven = combo.Length;
      var subtractedTarget = DayTenProblemUtilities.SubtractButtonCombo(targetJoltages, combo);
      if (subtractedTarget.All(i => i == 0))
      {
        if ((bestAnswer ?? int.MaxValue) > pressesToEven) bestAnswer = pressesToEven;
      }
      if(subtractedTarget.Any(i => i < 0)) continue;
      
      DayTenProblemUtilities.DivideArrayByTwo(subtractedTarget);
      var halfPressAnswer = GetLowestCombinationWithHalfSplits(subtractedTarget, buttons);
      if (halfPressAnswer != null)
      {
        var totalAnswer = pressesToEven + (halfPressAnswer.Value * 2);
        if(totalAnswer < (bestAnswer ?? int.MaxValue)) bestAnswer = totalAnswer;
      }
    }
    
    return bestAnswer;
  }

  private bool CanReachTargetStateCached(int indicatorLightState, int[] buttonCombo)
  {
    var cacheKey = $"target_state | {indicatorLightState} {string.Join(',', buttonCombo)}";
    return  _cache.GetOrCreate(cacheKey,
      _ => DayTenProblemUtilities.CanReachTargetState(indicatorLightState, buttonCombo), _defaultSizeOptions);
  }

  #region museum of failure
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
    
    //now try for illegal states situation
    var indicatorLightState = JoltageConstraint.GetIndicatorLightState(targetJoltages);
    var illegalStateCacheKey = $"{indicatorLightState} || {sortedPlausiblesCacheKey}";
    if (illegalStatesCache.TryGetValue(illegalStateCacheKey, out var isLegalState))
    {
      if (!isLegalState) return null;
    }
    else
    {
      var isLegalStateByIndicators = 
        JoltageConstraint.IsLegalStateByIndicators
          (indicatorLightState, targetJoltages.Length, sortedPlausiblesAsSets, sortedPlausiblesCacheKey, ref buttonCombosCache);
      illegalStatesCache[illegalStateCacheKey] = isLegalStateByIndicators;
      if (!isLegalStateByIndicators) return null;
    }
    
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
  
  #endregion
}