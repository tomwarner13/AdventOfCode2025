namespace AdventOfCode2025.Day10;

using System.Text;
using Util;

public class JoltageConstraint(int targetIndex)
{
  public readonly int TargetIndex = targetIndex;

  public readonly List<HashSet<int>> DependentButtonSets = [];

  public void BuildDependents(IEnumerable<HashSet<int>> allButtonSets)
  {
    foreach (var set in allButtonSets.Where(set => set.Contains(TargetIndex)))
    {
      var copiedSet = new HashSet<int>(set);
      copiedSet.Remove(TargetIndex);
      var shouldAddThisSet = true;
      foreach (var dep in DependentButtonSets)
      {
        if (dep.IsSubsetOf(copiedSet))
        {
          shouldAddThisSet = false;
          break;
        }
        if (dep.IsSupersetOf(copiedSet))
        {
          shouldAddThisSet = false;
          dep.IntersectWith(copiedSet);
        }
      }
      if(shouldAddThisSet) DependentButtonSets.Add(copiedSet);
    }
  }

  public bool IsLegalState(int[] currentRemainingState)
  {
    //add up all possible buttons that can contribute
    //check if they are higher than value at index

    var valueAtIndex = currentRemainingState[TargetIndex];

    if (valueAtIndex == 0) return true;

    var totalPressableSets = 
      DependentButtonSets.Sum(dep => 
        currentRemainingState.Where((t, i) => dep.Contains(i)).Min());

    return totalPressableSets >= valueAtIndex;
  }
  
  /*
   * example: [5,5,1], [1,2], [[0,1],[0,2],[1]] (should return 0 if TargetIndex 0, 5 if 1, 1 if 2)
   * DependentButtonSets will in this case be [[1], [2]]
   *
   * harder target: [7,5,8,3], [1,2], TargetIndex: 0
   * dependentButtonSets -> [1,2] [2,3], should return 1
   *
   * also working from assumption now that if this number returns too high (say, over button's otherwise max presses) we'll min that somewhere else?
   */
  public int GetMaximumLegalPresses(int[] currentRemainingState, HashSet<int> button) //do i care about otherButtons here? are they only used to build constraints ifne?
  {
    if (button.Contains(TargetIndex)) return currentRemainingState[TargetIndex];

    var maxAvailable = currentRemainingState[TargetIndex]; //5, 7
    var setsRemaining = 0;
    var knockedOutPerPress = 0;
    var tempState = new int[currentRemainingState.Length];
    Array.Copy(currentRemainingState, tempState, currentRemainingState.Length);
    
    foreach (var dep in DependentButtonSets)
    {
      var maxSets = tempState.Where((t, i) => dep.Contains(i)).Min();
      setsRemaining += maxSets;

      if (dep.Overlaps(button) && button.All(bi => tempState[bi] > 0))
      {
        knockedOutPerPress++;
      }

      for (var i = 0; i < tempState.Length; i++)
      {
        if (dep.Contains(i)) tempState[i] -= maxSets;
      }
    }
    
    //new idea: go through all deps, try and get a total of how many presses available before running out of TempState
    //add that to any presses left by button?
    
    //or, maybe, go through deps, different handling for no-overlap (add a free press per press), some-overlap (figure this out), all-overlap (calculate at end?)
    
    //IF THERE WAS REMAINDER FROM TEMPSTATE apply test button to that remainder first - basically get count of "free presses" to add to total
    var freePresses = tempState.Where((t, i) => button.Contains(i)).Min();
    var buttonMaxPresses = currentRemainingState.Where((t, i) => button.Contains(i)).Min();
    
    var maxJoltagesAllowed = setsRemaining - maxAvailable;

    //var knockedOutPerPress =
    //DependentButtonSets.Count(d => button.Overlaps(d)); //this may need IsSuperSetOf, IsSubsetOf, both?
    return knockedOutPerPress == 0 ? 
      buttonMaxPresses : 
      Math.Min(freePresses + (maxJoltagesAllowed / knockedOutPerPress), buttonMaxPresses);
  }
  
  public static int GetIndicatorLightState(int[] targetJoltages)
  {
    var requiredIndicatorStateInt = 0;
    
    for (var i = 0; i < targetJoltages.Length; i++)
    {
      if (targetJoltages[targetJoltages.Length - 1 - i] % 2 != 0)
      {
        requiredIndicatorStateInt += 1 << i;
      }
    }

    return requiredIndicatorStateInt;
  }

  //make sure to hide this behind some mf backing cache
  public static bool IsLegalStateByIndicators(int indicatorLightState, int rangeWidth, 
    IEnumerable<HashSet<int>> buttonCombos, string buttonCombosCacheKey, ref Dictionary<string, HashSet<int>> buttonCombosCache)
  {
    var resultsByButtons =
      GetAllAvailableResultsByButton(rangeWidth, buttonCombos, buttonCombosCacheKey, ref buttonCombosCache);
    
    return resultsByButtons.Contains(indicatorLightState);
  }

  private static HashSet<int> GetAllAvailableResultsByButton(int rangeWidth, IEnumerable<HashSet<int>> buttonCombos,
    string buttonCombosCacheKey, ref Dictionary<string, HashSet<int>> buttonCombosCache)
  {
    if (buttonCombosCache.TryGetValue(buttonCombosCacheKey, out var result)) return result;
    
    var finalButtons = 
      buttonCombos.Select(buttonCombo 
          => buttonCombo.Sum(button => 1 << (rangeWidth - button - 1)))
        .ToList();

    var calculatedResult =  CollectionUtils.GetAllCombinations(finalButtons, 1)
      .OrderBy(s => s.Length)
      .Select(set => set.Aggregate(0, (current, button) => current ^ button))
      .ToHashSet(); 
    
    buttonCombosCache[buttonCombosCacheKey] = calculatedResult;
    return calculatedResult;
  }
}