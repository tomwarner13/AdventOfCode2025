namespace Tests.Day10;

using AdventOfCode2025.Day10;
using AdventOfCode2025.Util;

public class DayTenProblemUtilitiesTests
{
  [Test]
  public void ButtonToIntTest()
  {
    var b1 = DayTenProblemUtilities.ButtonToInt([3], 4);
    var b2 = DayTenProblemUtilities.ButtonToInt([3], 10);
    var b3 = DayTenProblemUtilities.ButtonToInt([0,1,2,3], 4);
    using (Assert.EnterMultipleScope())
    {
      Assert.That(b1, Is.EqualTo(1));
      Assert.That(b2, Is.EqualTo(64));
      Assert.That(b3, Is.EqualTo(15));
    }
  }
  
  [Test]
  public void IntToButtonTest()
  {
    var b1 = DayTenProblemUtilities.IntToButton(1, 4);
    var b2 = DayTenProblemUtilities.IntToButton(64, 10);
    var b3 = DayTenProblemUtilities.IntToButton(15, 4);
    using (Assert.EnterMultipleScope())
    {
      Assert.That(b1, Is.EquivalentTo([3]));
      Assert.That(b2, Is.EquivalentTo([3]));
      Assert.That(b3, Is.EquivalentTo([0,1,2,3]));
    }
  }

  [Test]
  public void SubtractButtonComboTest()
  {
    int[] targetJoltages = [6, 3, 7, 4, 10];
    var buttons = new[]
    {
      DayTenProblemUtilities.ButtonToInt([0], targetJoltages.Length),
      DayTenProblemUtilities.ButtonToInt([1, 3], targetJoltages.Length),
      DayTenProblemUtilities.ButtonToInt([3, 4], targetJoltages.Length)
    };

    var result = DayTenProblemUtilities.SubtractButtonCombo(targetJoltages, buttons);
    
    Assert.That(result, Is.EquivalentTo([5, 2, 7, 2, 9]));
  }

  [Test]
  public void DivideArrayTest()
  {
    var t1 = new[] { 10, 4, 0, 100, 8 };
    var t2 = new[] { 10, 4, 7, 100, 8 };
    DayTenProblemUtilities.DivideArrayByTwo(t1);
    
    using (Assert.EnterMultipleScope())
    {
      Assert.That(t1, Is.EquivalentTo([5, 2, 0, 50, 4]));
      Assert.Throws<ThisShouldNeverHappenException>(
        () => DayTenProblemUtilities.DivideArrayByTwo(t2));
    }
  }
}