namespace AdventOfCode2025.Day10;

public class JoltageConstraint(int targetIndex)
{
  public readonly int TargetIndex = targetIndex;

  public readonly List<HashSet<int>> DependentButtonSets = [];

  //have to figure out how to build this too
  
  /*
   * example: [5,5,1], [1,2], [[0,1],[0,2],[1]] (should return 0 if TargetIndex 0, 5 if 1, 1 if 2)
   * DependentButtonSets will in this case be [[1], [2]]
   *
   * harder target: [7,5,8,3], [1,2], TargetIndex: 0
   * dependentButtonSets -> [1,2] [2,3], should return 1
   */
  public int GetMaximumLegalPresses(int[] currentRemainingState, HashSet<int> button) //do i care about otherButtons here? are they only used to build constraints ifne?
  {
    if (button.Contains(TargetIndex)) return currentRemainingState[TargetIndex];

    var maxAvailable = currentRemainingState[TargetIndex]; //5, 7
    
    //find max presses such that DependentButtonSets still has remaining total at least MaxAvailable
    var setsRemaining = 0;
    var tempState = new int[currentRemainingState.Length];
    Array.Copy(currentRemainingState, tempState, currentRemainingState.Length);
    foreach (var dep in DependentButtonSets)
    {
      var maxSets = tempState.Where((t, i) => dep.Contains(i)).Min();
      setsRemaining += maxSets;
      for (var i = 0; i < tempState.Length; i++)
      {
        if (dep.Contains(i)) tempState[i] -= maxSets;
      }
    }
    
    //now figure out how many sets Button knocks out per press
    //this maybe requires min not total? MaxSets at this point for 5,5,1 example should be 1
    var maxJoltagesAllowed = setsRemaining - maxAvailable;
    var knockedOutPerPress =
      DependentButtonSets.Where(button.IsSupersetOf).Count();

    return maxJoltagesAllowed / knockedOutPerPress;
  }
}