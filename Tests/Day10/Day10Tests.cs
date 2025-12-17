namespace Tests.Day10;

using AdventOfCode2025.Day10;

public class Day10Tests
{
  private Day10Problems _problems;
  
  [OneTimeSetUp]
  public void Setup()
  {
    _problems = new();
  }

  [Test]
  public void GetLowestCombinationTest_Basic()
  {
    var targetJoltages = new[] { 5, 5, 1 };
    var buttons = new int[][]
    {
      [0, 1],
      [0, 2],
      [1, 2],
      [1]
    }
      .Select(b => DayTenProblemUtilities.ButtonToInt(b, targetJoltages.Length))
      .ToArray();
    
    var expectedAnswer = 6;

    var result = _problems.GetLowestCombinationWithHalfSplits(targetJoltages, buttons);
    
    Assert.That(result, Is.EqualTo(expectedAnswer));
  }
  
  [Test]
  public void GetLowestCombinationTest_BlogExample()
  {
    var targetJoltages = new[] { 3, 5, 4, 7 };
    var buttons = new int[][]
      {
        [3],
        [1, 3],
        [2],
        [2, 3],
        [0, 2],
        [0, 1]
      }
      .Select(b => DayTenProblemUtilities.ButtonToInt(b, targetJoltages.Length))
      .ToArray();
    
    var expectedAnswer = 10;

    var result = _problems.GetLowestCombinationWithHalfSplits(targetJoltages, buttons);
    
    Assert.That(result, Is.EqualTo(expectedAnswer));
  }
  
  [Test]
  public void GetLowestCombinationTest_BlogExample_Part()
  {
    var targetJoltages = new[] { 1, 2, 1, 2 };
    var buttons = new int[][]
      {
        [3],
        [1, 3],
        [2],
        [2, 3],
        [0, 2],
        [0, 1]
      }
      .Select(b => DayTenProblemUtilities.ButtonToInt(b, targetJoltages.Length))
      .ToArray();
    
    var expectedAnswer = 3;

    var result = _problems.GetLowestCombinationWithHalfSplits(targetJoltages, buttons);
    
    Assert.That(result, Is.EqualTo(expectedAnswer));
  }
  
  [Test]
  public void GetLowestCombinationTest_HardExample()
  {
    var targetJoltages = new[] { 72,69,79,61,83,33,69,84,61,86 };
    var buttons = new int[][]
      {
        [1,4,5,9],
        [2,4,7],
        [2,3,5,6,9],
        [0,1,2,3,8],
        [0,1,2,4,6,7,9],
        [1,2,3,4,7,9],
        [4,9],
        [1,8],
        [4,8,9],
        [0,1,3,6,7],
        [0,4,6,8],
        [0,5,6,8,9],
        [0,2,3,7,8]
      }
      .Select(b => DayTenProblemUtilities.ButtonToInt(b, targetJoltages.Length))
      .ToArray();
    
    var expectedAnswer = 143;

    var result = _problems.GetLowestCombinationWithHalfSplits(targetJoltages, buttons);
    
    Assert.That(result, Is.EqualTo(expectedAnswer));
  }

  [Test]
  public void Problem2_TestInput()
  {
    var result = _problems.Problem2TestInput();
    Assert.That(result, Is.EqualTo("33"));
  }

  [Test]
  public void BuildDependentsTest()
  {
    var jc = new JoltageConstraint(0);
    jc.BuildDependents(
    new List<HashSet<int>>
    {
      new() { 0, 1, 2, 3 },
      new() { 0, 1 },
      new() { 0, 2, 3 },
      new() { 0, 3, 4 },
      new() { 0, 1, 2, 3, 4 },
      new() { 0, 1, 2, 3, 4, 5 }
    });
    
    Assert.That(jc.DependentButtonSets, Has.Count.EqualTo(3));
  }

  [Test]
  public void IsLegalStateTest()
  {
    var jc = new  JoltageConstraint(1);
    jc.DependentButtonSets.Add([0]);
    jc.DependentButtonSets.Add([2]);
    
    Assert.That(jc.IsLegalState([1, 2, 1]), Is.True);
    Assert.That(jc.IsLegalState([1, 3, 1]), Is.False);
  }
  
  [Test]
  public void IsLegalStateTest_Harder()
  {
    var jc = new  JoltageConstraint(0);
    jc.DependentButtonSets.Add([1]);
    
    Assert.That(jc.IsLegalState([10, 10, 7]), Is.True);
    Assert.That(jc.IsLegalState([10, 9, 7]), Is.False);
  }
  
  [Test]
  public void IsLegalStateTest_Harder2()
  {
    var jc = new  JoltageConstraint(0);    
    jc.BuildDependents(
      new List<HashSet<int>>
      {
        new() { 0, 1, 2 },
        new() { 0, 1 },
        new() { 0, 2 }
      });
    
      using (Assert.EnterMultipleScope())
      {
          Assert.That(jc.DependentButtonSets, Has.Count.EqualTo(2));

          Assert.That(jc.IsLegalState([10, 5, 5]), Is.True);
          Assert.That(jc.IsLegalState([10, 0, 12]), Is.True);
          Assert.That(jc.IsLegalState([10, 8, 1]), Is.False);
      }
  }
    
  [Test]
  public void IsLegalStateTest_Harder3()
  {
    var jc = new  JoltageConstraint(0);    
    jc.BuildDependents(
      new List<HashSet<int>>
      {
        new() { 0, 1, 2, 3, 4, 5 },
        new() { 0, 5 },
        new() { 0, 1, 2, 3 },
        new() { 0, 1, 2 },
        new() { 0, 1 },
        new() { 0, 1, 2, 3, 4 },
      });
    
    using (Assert.EnterMultipleScope())
    {
      Assert.That(jc.DependentButtonSets, Has.Count.EqualTo(2));

      Assert.That(jc.IsLegalState([10, 0, 47, 23, 17, 9]), Is.False);
      Assert.That(jc.IsLegalState([10, 6, 47, 23, 17, 5]), Is.True);
    }
  }
  
  [Test]
  public void IsLegalStateTest_Harder4()
  {
    var jc = new  JoltageConstraint(6);    
    jc.BuildDependents(
      new List<HashSet<int>>
      {
        new() { 6, 1, 2 },
        new() { 6, 2, 3 },
      });
    
    using (Assert.EnterMultipleScope())
    {
      Assert.That(jc.DependentButtonSets, Has.Count.EqualTo(2));

      Assert.That(jc.IsLegalState([0, 1, 1, 1, 0, 0, 1]), Is.True);
      Assert.That(jc.IsLegalState([0, 1, 1, 1, 0, 0, 2]), Is.True);
      Assert.That(jc.IsLegalState([0, 1, 1, 1, 0, 0, 3]), Is.False);
      Assert.That(jc.IsLegalState([0, 1, 0, 1, 0, 0, 1]), Is.False);
    }
  }

  [Test]
  public void IsLegalStateByIndicatorsTest()
  {
    var targetJoltages = new[] { 1, 1, 1 };
    var buttons = new HashSet<int>[]
    {
      [0, 1],
      [1, 2],
    };
    var bcCache = new Dictionary<string, HashSet<int>>();

    var indicatorLightState = JoltageConstraint.GetIndicatorLightState(targetJoltages);
    var isLegal = 
      JoltageConstraint.IsLegalStateByIndicators(indicatorLightState, targetJoltages.Length,
        buttons, "", ref bcCache);
    Assert.That(isLegal, Is.False);
  }
  
  [Test]
  public void IsLegalStateByIndicatorsTest_Harder1()
  {
    var targetJoltages = new[] { 5, 5, 1 };
    var buttons = new HashSet<int>[]
    {
      [0, 1],
      [1, 2],
    };
    var bcCache = new Dictionary<string, HashSet<int>>();

    var indicatorLightState = JoltageConstraint.GetIndicatorLightState(targetJoltages);
    var isLegal = JoltageConstraint.IsLegalStateByIndicators(indicatorLightState, targetJoltages.Length, 
      buttons, "", ref bcCache);
    Assert.That(isLegal, Is.False);
  }
  
  [Test]
  public void IsLegalStateByIndicatorsTest_Harder2()
  {
    var targetJoltages = new[] { 11, 17, 1 };
    var buttons = new HashSet<int>[]
    {
      [0, 1],
      [1, 2],
      [0, 2],
      [1]
    };

    var indicatorLightState = JoltageConstraint.GetIndicatorLightState(targetJoltages);
    
    var bcCache = new Dictionary<string, HashSet<int>>();
    var isLegal = 
      JoltageConstraint.IsLegalStateByIndicators(indicatorLightState, targetJoltages.Length,
        buttons, "", ref bcCache);
    Assert.That(isLegal, Is.True);
  }
  
  //(3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
  [Test]
  public void IsLegalStateByIndicatorsTest_Harder3()
  {
    var targetJoltages = new[] { 3, 5, 4, 7 };
    var buttons = new HashSet<int>[]
    {
      [3],
      [1, 3],
      [2],
      [2, 3],
      [0, 2],
      [0, 1]
    };

    var indicatorLightState = JoltageConstraint.GetIndicatorLightState(targetJoltages);
    
    var bcCache = new Dictionary<string, HashSet<int>>();
    var isLegal = 
      JoltageConstraint.IsLegalStateByIndicators(indicatorLightState, targetJoltages.Length,
        buttons, "", ref bcCache);
    Assert.That(isLegal, Is.True);
  }
  
  [Test]
  public void IsLegalStateByIndicatorsTest_Harder4()
  {
    var targetJoltages = new[] { 24,27,12,16,41,8,22,29,35,19 };
    var buttons = new HashSet<int>[]
    {
      [0,1,3,6,7],
      [0,1,2,3,8],
      [1,4,5,9],
      [0,4,6,8],
      [4,8,9],
      [2,4,7],
      [4,9],
      [1,8]
    };

    var indicatorLightState = JoltageConstraint.GetIndicatorLightState(targetJoltages);
    
    var bcCache = new Dictionary<string, HashSet<int>>();
    var isLegal = 
      JoltageConstraint.IsLegalStateByIndicators(indicatorLightState, targetJoltages.Length,
        buttons, "", ref bcCache);
    Assert.That(isLegal, Is.False);
  }
}