using System.Text;
using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day2;

public class Day2Problems : Problems
{
  protected override string TestInput => @"7 6 4 2 1
1 2 7 8 9
9 7 6 2 1
1 3 2 4 5
8 6 4 4 1
1 3 6 7 9
8 9 8 7 6 5 4";

  protected override int Day => 2;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var safeReports = 0;
    foreach (var line in input)
    {
      var report = StringUtils.ExtractIntsFromString(line).ToArray();
      if(IsReportSafe(report)) safeReports++;
    }
    return safeReports.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var safeReports = 0;
    foreach (var line in input)
    {
      var report = StringUtils.ExtractIntsFromString(line).ToArray();
      if (IsReportSafeWithDampener(report))
        safeReports++;
    }
    return safeReports.ToString();
  }

  private static bool IsReportSafe(int[] report)
  {
    var isIncreasing = report[0] < report[1];

    if (isIncreasing)
    {
      for (var i = 1; i < report.Length; i++)
      {
        var curNum = report[i];
        var lastNum = report[i - 1];
        var diff = curNum - lastNum;

        if (diff is < 1 or > 3)
          return false;
      }
    }
    else
    {
      for (var i = 1; i < report.Length; i++)
      {
        var curNum = report[i];
        var lastNum = report[i - 1];
        var diff = lastNum - curNum;

        if (diff is < 1 or > 3)
          return false;
      }
    }

    return true;
  }
  
  private static bool IsReportSafeWithDampener(int[] report)
  {
    var isIncreasing = report[0] < report[1];

    if (isIncreasing)
    {
      for (var i = 1; i < report.Length; i++)
      {
        var curNum = report[i];
        var lastNum = report[i - 1];
        var diff = curNum - lastNum;

        if (diff is < 1 or > 3)
        {
          var missingLastNum = RemoveAtIndex(report, i - 1);
          var missingCurNum = RemoveAtIndex(report, i);
          //also attempt removing the first number! changing the move order may turn a report safe
          var missingFirstNum = RemoveAtIndex(report, 0);

          return IsReportSafe(missingLastNum) || IsReportSafe(missingCurNum) || IsReportSafe(missingFirstNum);
        }
      }
    }
    else
    {
      for (var i = 1; i < report.Length; i++)
      {
        var curNum = report[i];
        var lastNum = report[i - 1];
        var diff = lastNum - curNum;

        if (diff is < 1 or > 3)
        {
          var missingLastNum = RemoveAtIndex(report, i - 1);
          var missingCurNum = RemoveAtIndex(report, i);
          var missingFirstNum = RemoveAtIndex(report, 0);
          
          return IsReportSafe(missingLastNum) || IsReportSafe(missingCurNum) || IsReportSafe(missingFirstNum);
        }
      }
    }

    return true;
  }

  private static int[] RemoveAtIndex(int[] values, int index)
  {
    var list = values.ToList();
    list.RemoveAt(index);
    return list.ToArray();
  }
}