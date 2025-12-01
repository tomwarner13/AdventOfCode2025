using System.Text.RegularExpressions;

namespace AdventOfCode2024.Util;

public static partial class RegexUtils
{
  public static readonly Regex BasicDigitRegex = NumberRegex();
  public static readonly Regex BasicDigitNegativeRegex = NegativeNumberRegex();
  public static readonly Regex BasicWordRegex = WordRegex();
  public static readonly Regex BasicLetterRegex = LetterRegex();

  [GeneratedRegex("[A-Za-z]+", RegexOptions.Compiled)]
  private static partial Regex LetterRegex();
  
  [GeneratedRegex("\\w+", RegexOptions.Compiled)]
  private static partial Regex WordRegex();
  
  [GeneratedRegex("[0-9\\-]+", RegexOptions.Compiled)]
  private static partial Regex NegativeNumberRegex();
  
  [GeneratedRegex("\\d+", RegexOptions.Compiled)]
  private static partial Regex NumberRegex();
}