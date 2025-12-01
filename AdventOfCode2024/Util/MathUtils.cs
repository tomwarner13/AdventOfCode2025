namespace AdventOfCode2024.Util;

public class MathUtils
{
  public static int PositiveMod(int num, int mod)
  {
    var answer = num % mod;
    return answer > 0 ? answer : answer + mod;
  }
  
  public static long PositiveMod(long num, long mod)
  {
    var answer = num % mod;
    return answer > 0 ? answer : answer + mod;
  }
}