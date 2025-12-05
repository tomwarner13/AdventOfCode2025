namespace AdventOfCode2025.Day5;

using Util;

public class Day5Problems : Problems
{
  protected override int Day => 5;
  
  protected override string TestInput => @"3-5
10-14
16-20
12-18

1
5
8
11
17
32";

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var firstRange = StringUtils.ExtractLongsFromString(input[0]).ToArray();
    var searchTreeRoot = new TreeNode(firstRange[0], firstRange[1]);
    D($"root node {searchTreeRoot.MinRange} to {searchTreeRoot.MaxRange}");
    var freshIngredients = 0L;

    foreach (var line in input[1..])
    {
      var numbers = StringUtils.ExtractLongsFromString(line).ToArray();
      switch (numbers.Length)
      {
        case 0:
          //no-op
          break;
        case 1:
          if (searchTreeRoot.CheckNumber(numbers[0]))
          {
            D($"detected fresh {numbers[0]}");
            freshIngredients++;
          }
          break;
          case 2:
            var newNode = new TreeNode(numbers[0], numbers[1]);
            D($"adding node {newNode.MinRange} to {newNode.MaxRange}");
            searchTreeRoot.AddChild(newNode);
            break;
          default:
          throw new ThisShouldNeverHappenException($"input weird {line}");
      }
    }

    return freshIngredients.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var firstRange = StringUtils.ExtractLongsFromString(input[0]).ToArray();
    var searchTreeRoot = new TreeNode(firstRange[0], firstRange[1]);
    D($"root node {searchTreeRoot.MinRange} to {searchTreeRoot.MaxRange}");
    
    var freshRangeTotal = 0L;

    foreach (var line in input[1..])
    {
      var numbers = StringUtils.ExtractLongsFromString(line).ToArray();
      switch (numbers.Length)
      {
        case 0:
        case 1:
          //no-op
          continue;
        case 2:
          var newNode = new TreeNode(numbers[0], numbers[1]);
          D($"adding node {newNode.MinRange} to {newNode.MaxRange}");
          searchTreeRoot.AddChild(newNode);
          D($"new node range: {newNode.MinRange} to {newNode.MaxRange}");
          break;
        default:
          throw new ThisShouldNeverHappenException($"input weird {line}");
      }
    }

    var result = searchTreeRoot.TotalFreshRange();
    return result.ToString();
  }

  private class TreeNode
  {
    public long MinRange { get; private set; }
    public long MaxRange { get; private set; }

    public long TotalFreshRange()
    {
      return
        ((MaxRange - MinRange) + 1)
        + (LeftChild?.TotalFreshRange() ?? 0)
        + (RightChild?.TotalFreshRange() ?? 0);
    }
    
    public TreeNode? LeftChild { get; private set; }
    public TreeNode? RightChild { get; private set; }

    public TreeNode(long minRange, long maxRange)
    {
      MinRange = minRange;
      MaxRange = maxRange;
    }

    public void AddChild(TreeNode child)
    {
      ArgumentNullException.ThrowIfNull(child);

      if (child.MinRange < MinRange) //add left
      {
        if (child.MaxRange > MaxRange) //overlap, edit this node rather than add a child
        {
          MinRange = child.MinRange;
          MaxRange = child.MaxRange;
          
          ModifyMinRangeRecursive(child.MinRange);
          ModifyMaxRangeRecursive(child.MaxRange);
        }
        else
        {
          //edit child range so no overlap
          if(child.MaxRange >= MinRange) child.MaxRange = MinRange - 1;
          
          if (LeftChild == null) LeftChild = child;
          else LeftChild.AddChild(child);
        }
      }
      else //add right
      {
        if (child.MaxRange <= MaxRange) return; //if inner overlap, do nothing
        
        //edit child range so no overlap
        if(child.MinRange <= MaxRange) child.MinRange = MaxRange + 1;
        
        if(RightChild == null) RightChild = child;
        else RightChild.AddChild(child);
      }
    }

    public void ModifyMinRangeRecursive(long newMin)
    {
      if (MinRange < newMin && MaxRange >= newMin) //edit self, kill right children
      {
        MaxRange = newMin -1;
        RightChild = null;
      }
      
      var left = LeftChild;
      if (left == null) return;
      
      if(left.MinRange >= newMin) //kill this entire child
      {
        while (LeftChild != null && LeftChild.MinRange >= newMin)
        {
          LeftChild = LeftChild.LeftChild;
        }
        
        LeftChild?.ModifyMinRangeRecursive(newMin);
      }
      else
      {
        left.RightChild?.ModifyMinRangeRecursive(newMin);
      }
    }
    
    public void ModifyMaxRangeRecursive(long newMax)
    {      
      if (MaxRange > newMax && MinRange <= newMax) //edit self, kill left children
      {
        MinRange = newMax + 1;
        LeftChild = null;
      }
      
      var right = RightChild;
      
      if (right == null) return;
      
      if(right.MaxRange <= newMax) //kill this entire child, recursively
      {
        while (RightChild != null && RightChild.MaxRange <= newMax)
        {
          RightChild = RightChild.RightChild;
        }

        RightChild?.ModifyMaxRangeRecursive(newMax);
      }
      else
      {
        right.LeftChild?.ModifyMaxRangeRecursive(newMax);
      }
    }

    public bool CheckNumber(long input)
    {
      if (input < MinRange)
      {
        return LeftChild != null && LeftChild.CheckNumber(input);
      }
      if (input > MaxRange)
      {
        return RightChild != null && RightChild.CheckNumber(input);
      }

      return true;
    }

    public override string ToString() => $"{MinRange} - {MaxRange}";
  }
}