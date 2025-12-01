using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day19;

public class Day19Problems : Problems
{
  protected override string TestInput => @"r, wr, b, g, bwu, rb, gb, br

brwrr
bggr
gbbr
rrbgbr
ubwu
bwurrg
brgr
bbrgwb";

  protected override int Day => 19;
  
  private readonly Dictionary<string, long> _arrangementCache = new();

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var towelDesigns = new HashSet<string>();
    var maxDesignLength = 0;

    foreach (var design in input[0].Split(", "))
    {
      towelDesigns.Add(design);
      if(design.Length > maxDesignLength)
        maxDesignLength = design.Length;
    }

    var totalPossiblePatterns = 0;

    for (var i = 2; i < input.Length; i++)
    {
      if (IsPatternPossible(input[i], towelDesigns, maxDesignLength)) totalPossiblePatterns++;
    }
    
    return totalPossiblePatterns.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var towelDesigns = new HashSet<string>();
    var maxDesignLength = 0;

    foreach (var design in input[0].Split(", "))
    {
      towelDesigns.Add(design);
      if(design.Length > maxDesignLength)
        maxDesignLength = design.Length;
    }

    var totalPossibleArrangements = 0L;
    _arrangementCache.Clear();

    for (var i = 2; i < input.Length; i++)
    {
      totalPossibleArrangements += GetAllArrangements(input[i], towelDesigns, maxDesignLength);
    }
    
    return totalPossibleArrangements.ToString();
  }

  private static bool IsPatternPossible(string pattern, HashSet<string> towelDesigns, int maxDesignLength, int startIndex = 0)
  {
    if (startIndex == pattern.Length) return true;
    
    for (var testIndex = 1; 
         testIndex <= maxDesignLength && startIndex + testIndex <= pattern.Length; 
         testIndex++)
    {
      var subStr = pattern.Substring(startIndex, testIndex);
      
      if(towelDesigns.Contains(subStr) &&
        IsPatternPossible(pattern, towelDesigns, maxDesignLength, startIndex + testIndex)) return true;
    }

    return false;
  }
  
  private long GetAllArrangements(string remainingPattern, HashSet<string> towelDesigns, int maxDesignLength)
  {
    if (remainingPattern.Length == 0) return 1;

    if(_arrangementCache.TryGetValue(remainingPattern, out var total)) return total;
    
    var arrangements = 0L;
    for (var testIndex = 1; 
         testIndex <= maxDesignLength && testIndex <= remainingPattern.Length; 
         testIndex++)
    {
      var subStr = remainingPattern[..testIndex];
      
      if(towelDesigns.Contains(subStr))
        arrangements += GetAllArrangements(remainingPattern[testIndex..], towelDesigns, maxDesignLength);
    }

    _arrangementCache[remainingPattern] = arrangements;
    return arrangements;
  }
}