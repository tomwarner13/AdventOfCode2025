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
}