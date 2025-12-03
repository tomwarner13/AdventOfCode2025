#!/bin/sh

#MAYBE ALSO BETTER TEST INPUT HANDLING -- PUT IT AS A STRING[] IN A SEPARATE REGION OR FILE?

cat << EOF
namespace AdventOfCode2025.Day$1;

using Util;

public class Day$1Problems : Problems
{
  protected override int Day => $1;
  
  protected override string TestInput => @"";

  protected override string Problem1(string[] input, bool isTestInput)
  {
    throw new NotImplementedException();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    throw new NotImplementedException();
  }
}
EOF
