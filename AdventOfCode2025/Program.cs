// ReSharper disable RedundantUsingDirective
using AdventOfCode2025.Day1;
using AdventOfCode2025.Day10;
using AdventOfCode2025.Day11;
using AdventOfCode2025.Day12;
using AdventOfCode2025.Day2;
using AdventOfCode2025.Day3;
using AdventOfCode2025.Day4;
using AdventOfCode2025.Day5;
using AdventOfCode2025.Day6;
using AdventOfCode2025.Day7;
using AdventOfCode2025.Day8;
using AdventOfCode2025.Day9;
using AdventOfCode2025.Util;

var problems = new Day4Problems();
DoAllProblems(problems);
return;


void DoAllProblems(Problems probs)
{
  TryPrintResult(probs.Problem1TestInput, "Problem 1 Test Input");
  TryPrintResult(probs.Problem1FullInput, "Problem 1 Full Input");
  TryPrintResult(probs.Problem2TestInput, "Problem 2 Test Input");
  TryPrintResult(probs.Problem2FullInput, "Problem 2 Full Input");
}

void TryPrintResult(Func<string> attempter, string description)
{
  try
  {
    var result = attempter();

    Console.WriteLine($"{description}:");
    Console.WriteLine(result);
  }
  catch (NotImplementedException)
  {
    Console.WriteLine($"{description} not implemented");
  }
  catch(Exception e)
  {
    Console.WriteLine($"{description} failed:");
    Console.WriteLine(e);
  }

  Console.WriteLine();
}