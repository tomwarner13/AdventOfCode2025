namespace Tests;

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
  public void Problem2_TestInput()
  {
    var result = _problems.Problem2TestInput();
    Assert.That(result, Is.EqualTo("33"));
  }

  [Test]
  public void Problem2_HardInput()
  {
    const string hardInput = 
      "[..#...###.] (1,4,5,9) (2,4,7) (2,3,5,6,9) (0,1,2,3,8) (0,1,2,4,6,7,9) (1,2,3,4,7,9) (4,9) (1,8) (4,8,9) (0,1,3,6,7) (0,4,6,8) (0,5,6,8,9) (0,2,3,7,8) {72,69,79,61,83,33,69,84,61,86}";

    Assert.That(int.Parse(_problems.Problem2([hardInput], false)), Is.GreaterThan(0));
  }

  [Test]
  public void JoltageConstraintCheckerTest()
  {
    var jc = new JoltageConstraint(0);
    jc.DependentButtonSets.Add([1]);
    jc.DependentButtonSets.Add([2]);

    var result = jc.GetMaximumLegalPresses([5, 5, 1], [1,2]);
    Assert.That(result, Is.EqualTo(0));
  }
  
  [Test]
  public void JoltageConstraintCheckerTest_Harder()
  {
    var jc = new JoltageConstraint(0);
    jc.DependentButtonSets.Add([1, 2]);
    jc.DependentButtonSets.Add([2, 3]);

    var result = jc.GetMaximumLegalPresses([7, 5, 8, 3], [1,2]);
    Assert.That(result, Is.EqualTo(1));
    
    var result2 = jc.GetMaximumLegalPresses([7, 5, 7, 3], [1,2]); 
    Assert.That(result2, Is.EqualTo(0));//this fails now, probably handling overlaps bad
  }
}