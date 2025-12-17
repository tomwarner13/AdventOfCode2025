namespace AdventOfCode2025.Day10;

using Util;

public static class DayTenProblemUtilities
{
  public static int ButtonToInt(IEnumerable<int> button, int rangeWidth)
  {
    return button.Sum(i => 1 << (rangeWidth - i - 1));
  }
  
  public static int[] IntToButton(int buttonInt, int rangeWidth)
  {
    var result = new List<int>();

    for (var i = rangeWidth - 1; i >= 0; i--)
    {
      var matchAtIndex = (buttonInt & (1 << (rangeWidth - i - 1))) > 0;
      if(matchAtIndex) result.Add(i);
    }
    
    return result.ToArray();
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

  public static IEnumerable<int[]> GetAllButtonCombinations(int[] buttons)
    => CollectionUtils.GetAllCombinations(buttons, 1);

  public static bool CanReachTargetState(int targetIndicatorState, int[] buttonCombo)
  {
    var testVal = buttonCombo.Aggregate(0, (current, button) => current ^ button);
    return testVal == targetIndicatorState;
  }

  public static int[] SubtractButtonCombo(int[] targetJoltages, IEnumerable<int> buttonCombo)
  {
    //for each button, decrement every index in tj it matches
    var result = new int[targetJoltages.Length];
    Array.Copy(targetJoltages, result, targetJoltages.Length);
    foreach (var button in buttonCombo)
    {
      var toIndices = IntToButton(button, result.Length);
      foreach (var i in toIndices) result[i]--;
    }

    return result;
  }

  public static void DivideArrayByTwo(int[] target)
  {
    for (var i = 0; i < target.Length; i++)
    {
      if (target[i] % 2 != 0)
      {
        throw new ThisShouldNeverHappenException($"attempted to halve uneven array! array was [{string.Join(',', target)}]");
      }
      target[i] /= 2;
    }
  }
}