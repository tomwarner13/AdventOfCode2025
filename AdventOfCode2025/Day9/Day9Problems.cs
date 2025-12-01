using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day9;

public class Day9Problems : Problems
{
  protected override string TestInput => @"2333133121414131402";

  protected override int Day => 9;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var diskLayout = new List<long>();

    for (var i = 0; i < input[0].Length; i++)
    {
      var c = input[0][i];

      int fileBlock;
      if (i % 2 == 1)
      {
        fileBlock = -1;
      }
      else
      {
        fileBlock = i / 2;
      }

      var amountToAdd = int.Parse(c.ToString());
      for (var j = 0; j < amountToAdd; j++)
      {
        diskLayout.Add(fileBlock);
      }
    }

    var currentBlock = 0;
    var lastBlock = diskLayout.Count - 1;

    while (currentBlock < lastBlock)
    {
      if(diskLayout[currentBlock] != -1) currentBlock++;
      else
      {
        while (diskLayout[lastBlock] == -1)
        {
          lastBlock--;
        }
        if(currentBlock < lastBlock) //avoid the double-swap bug!
          (diskLayout[currentBlock], diskLayout[lastBlock]) = (diskLayout[lastBlock], diskLayout[currentBlock]);
        currentBlock++;
        lastBlock--;
      }
    }

    long checkSum = 0;
    currentBlock = 0;

    while (diskLayout[currentBlock] != -1)
    {
      checkSum += diskLayout[currentBlock] * currentBlock;
      currentBlock++;
    }

    var firstFoundFreeSpace = currentBlock;
    
    while (currentBlock < diskLayout.Count)
    {
      if (diskLayout[currentBlock] != -1)
      {
        throw new InvalidOperationException($"first free: {firstFoundFreeSpace} | found {diskLayout[currentBlock]} at position {currentBlock}");
      }
      currentBlock++;
    }
    
    return checkSum.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var diskLayout = new List<int>();

    for (var i = 0; i < input[0].Length; i++)
    {
      var c = input[0][i];

      int fileBlock;
      if (i % 2 == 1)
      {
        fileBlock = -1;
      }
      else
      {
        fileBlock = i / 2;
      }

      var amountToAdd = int.Parse(c.ToString());
      for (var j = 0; j < amountToAdd; j++)
      {
        diskLayout.Add(fileBlock);
      }
    }
    
    var filePointer = diskLayout.Count - 1;

    while (filePointer >= 0)
    {
      if (diskLayout[filePointer] != -1)
      {
        var blockEndPostion = filePointer;
        var blockId = diskLayout[filePointer];

        while (filePointer >= 0 && diskLayout[filePointer] == blockId)
        {
          filePointer--;
        }
        
        var blockStart = filePointer + 1;
        var blockLength = blockEndPostion - filePointer;
        
        //start at zero, search through layout for a space blockLength long
        var searchPos = 0;
        var gapStart = 0;

        while (searchPos < blockStart)
        {
          if (diskLayout[searchPos] != -1)
          {
            gapStart = searchPos;
          }
          else
          {
            if (searchPos - gapStart == blockLength)
            {
              //gap found, move whole file
              for (var i = gapStart + 1; i <= gapStart + blockLength; i++)
              {
                diskLayout[i] = blockId;
              }

              for (var i = blockStart; i < blockStart + blockLength; i++)
              {
                diskLayout[i] = -1;
              }

              searchPos = blockStart;
            }
          }
          
          searchPos++;
        }
      }
      else
      {
        filePointer--;
      }
    }
    
    
    long checkSum = 0;

    for(var i = 0; i < diskLayout.Count; i++)
    {
      if(diskLayout[i] != -1) checkSum += diskLayout[i] * i;
    }
    
    return checkSum.ToString();
  }
}