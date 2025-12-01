#!/bin/sh

#TODO REFACTOR ME TO HANDLE STRINGBUILDERS IN PARAMS
#MAYBE ALSO BETTER TEST INPUT HANDLING -- PUT IT AS A STRING[] IN A SEPARATE REGION OR FILE?

DAY=$1

cat << EOF
using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day$1
{
  public class Day$1Problems : Problems
  {
    public override string TestInput => @"";

    public override int Day => $1;

    public override string Problem1(string[] input)
    {
      throw new NotImplementedException();
    }

    public override string Problem2(string[] input)
    {    
      throw new NotImplementedException();
    }
  }
}
EOF
