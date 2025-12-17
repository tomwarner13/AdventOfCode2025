namespace AdventOfCode2025.Day11;

using Util;

public class Day11Problems : Problems
{
  protected override int Day => 11;
  
  protected override string TestInput => """
                                         aaa: you hhh
                                         you: bbb ccc
                                         bbb: ddd eee
                                         ccc: ddd eee fff
                                         ddd: ggg
                                         eee: out
                                         fff: out
                                         ggg: out
                                         hhh: ccc fff iii
                                         iii: out
                                         """;

  public override string Problem1(string[] input, bool isTestInput)
  {
    var startingNode = string.Empty;
    Dictionary<string, HashSet<string>> nodeMaps = new();
    HashSet<string> outNodes = [];

    foreach (var line in input)
    {
      var spl = line.Split(':');
      var nodeAddress = spl[0];

      if (spl[1].Contains("out"))
      {
        outNodes.Add(nodeAddress);
        continue;
      }
      
      HashSet<string> connections;
      if (spl[0].Contains("you"))
      {
        startingNode = nodeAddress;
      }
      
      connections = StringUtils.ExtractWordsFromString(spl[1])
        .Where(c => c != "you")
        .ToHashSet();
      
      nodeMaps[nodeAddress] = connections;
    }

    var result = GetConnectionsToOutput(startingNode, ref nodeMaps, ref outNodes, outNodes);
    
    return result.ToString();
  }

  public override string Problem2(string[] input, bool isTestInput)
  {
    throw new NotImplementedException();
  }

  private static int GetConnectionsToOutput(string currentAddress, ref Dictionary<string, HashSet<string>> nodeMaps,
    ref HashSet<string> outNodes, HashSet<string> visitedNodes)
  {
    if (outNodes.Contains(currentAddress)) return 1;

    if (visitedNodes.Contains(currentAddress)) return 0; //cut out cycles

    var totalRoutes = 0;
    var destinations = nodeMaps[currentAddress];
    var newVisitedNodes = new HashSet<string> { currentAddress };
    newVisitedNodes.UnionWith(visitedNodes);

    foreach (var node in destinations)
    {
      totalRoutes += GetConnectionsToOutput(node, ref nodeMaps, ref outNodes, newVisitedNodes);
    }

    return totalRoutes;
  }
}