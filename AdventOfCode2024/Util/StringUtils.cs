namespace AdventOfCode2024.Util;

public static class StringUtils
{
  public static IEnumerable<int> ExtractIntsFromString(string input, bool includeNegative = false)
  {
    var regex = includeNegative ? RegexUtils.BasicDigitNegativeRegex : RegexUtils.BasicDigitRegex;
    var matches = regex.Matches(input);
    return matches.Select(s => int.Parse(s.ToString()));
  }
  
  public static IEnumerable<string> ExtractWordsFromString(string input)
  {
    var matches = RegexUtils.BasicLetterRegex.Matches(input);
    return matches.Select(s => s.ToString());
  }
  
  public static IEnumerable<long> ExtractLongsFromString(string input, bool includeNegative = false)
  {
    var regex = includeNegative ? RegexUtils.BasicDigitNegativeRegex : RegexUtils.BasicDigitRegex;
    var matches = regex.Matches(input);
    return matches.Select(s => long.Parse(s.ToString()));
  }
  
  public static IEnumerable<string> ExtractAlphanumericsFromString(string input)
  {
    var matches = RegexUtils.BasicWordRegex.Matches(input);
    return matches.Select(s => s.ToString());
  }
  
  /// <summary>
  /// pass in (c, x, y)
  /// </summary>
  /// <param name="input"></param>
  /// <param name="reader">c, x, y</param>
  public static void ReadInputGrid(string[] input, Action<char, int, int> reader)
  {
    for (var y = 0; y < input.Length; y++)
    {
      for (var x = 0; x < input[0].Length; x++)
      {
        var c = input[y][x];
        reader(c, x, y);
      }
    }
  }
  
  /// <summary>
  /// pass in (c, x, y)
  /// </summary>
  /// <param name="input">will be string.split on newlines</param>
  /// <param name="reader">c, x, y</param>
  public static void ReadInputGrid(string input, Action<char, int, int> reader)
    => ReadInputGrid(input.Split('\n'), reader);
}