using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day23;

public class Day23Problems : Problems
{
  protected override string TestInput => @"kh-tc
qp-kh
de-cg
ka-co
yn-aq
qp-ub
cg-tb
vc-aq
tb-ka
wh-tc
yn-cg
kh-ub
ta-co
de-co
tc-td
tb-wq
wh-td
ta-ka
td-qp
aq-cg
wq-ub
ub-vc
de-ta
wq-aq
wq-vc
wh-yn
ka-de
kh-ta
co-tc
wh-qp
tb-vc
td-yn";

  protected override int Day => 23;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var pointsWithTs = new HashSet<string>();
    var map = new Dictionary<string, HashSet<string>>();
    var uniqueResults = new HashSet<string>();

    foreach (var line in input)
    {
      var points = line.Split('-');
      for (var i = 0; i < 2; i++ )
      {
        var point = points[i];
        var other = points[i == 0 ? 1 : 0];
        
        if(point.StartsWith('t')) pointsWithTs.Add(point);
        if (map.TryGetValue(point, out var connections)) connections.Add(other);
        else map[point] = [other];
      }
    }

    foreach (var tPoint in pointsWithTs)
    {
      foreach (var firstDegreeConnection in map[tPoint])
      {
        foreach (var secondDegreeConnection in map[firstDegreeConnection].Where(c => c != tPoint))
        {
          //check if this one connects back to the start
          if(map[secondDegreeConnection].Contains(tPoint)) 
            uniqueResults.Add(MakeSortedKey(tPoint, firstDegreeConnection, secondDegreeConnection));
        }
      }
    }

    return uniqueResults.Count.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    DebugMode = isTestInput;
    var map = new Dictionary<string, HashSet<string>>();
    
    foreach (var line in input)
    {
      var points = line.Split('-');
      for (var i = 0; i < 2; i++ )
      {
        var point = points[i];
        var other = points[i == 0 ? 1 : 0];
        
        if (map.TryGetValue(point, out var connections)) connections.Add(other);
        else map[point] = [other];
      }
    }
    
    HashSet<string> largestLanParty = [];

    foreach (var comp in map
               .OrderBy(c => c.Key))
    {
      var party = GetLargestParty(comp.Key, [], ref map);
      if (party.Count > largestLanParty.Count)
      {
        var pwd = isTestInput ? MakeIntoPassword(party) : string.Empty;
        D($"found largest party: {pwd}");
        largestLanParty = party;
      }
    }
    
    return MakeIntoPassword(largestLanParty);
  }

  private static string MakeSortedKey(string p1, string p2, string p3)
  {
    var arr = new[] { p1, p2, p3 };
    Array.Sort(arr);
    return string.Join("|", arr);
  }

  //quick-and-dirty pruning to avoid combinatorial explosion: only considering neighbors in lower order
  //if we call this with visited: {A, B, C} and D as start,
  //it should check if D has conns to all 3, then if so, recurse with {A, B, C, D} to each of D's higher-order neighbors
  private static HashSet<string> GetLargestParty(string startNode, HashSet<string> visitedNodes,
    ref Dictionary<string, HashSet<string>> map)
  {
    var neighbors = map[startNode];
    if (visitedNodes.All(n => neighbors.Contains(n)))
    {
      var newVisited = new HashSet<string>(visitedNodes) { startNode };
      var bestResult = newVisited;

      foreach (var neighbor in neighbors
                 .Where(n => string.CompareOrdinal(n, startNode) > 0
                             && !newVisited.Contains(n)))
      {
        var testResult = GetLargestParty(neighbor, newVisited, ref map);
        if(testResult.Count > bestResult.Count) bestResult = testResult;
      }
      
      return bestResult;
    }

    return [];
  }

  private static string MakeIntoPassword(HashSet<string> party)
  {
    var partyList = party.ToList();
    partyList.Sort();
    return string.Join(',', partyList);
  }
}