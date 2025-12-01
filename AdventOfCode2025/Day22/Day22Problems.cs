using AdventOfCode2025.Util;

namespace AdventOfCode2025.Day22;

public class Day22Problems : Problems
{
  protected override string TestInput => @"123
10
100
2025";

  protected override int Day => 22;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    DebugMode = isTestInput;
    var acc = 0L;

    foreach (var line in input)
    {
      var seed = long.Parse(line);
      var secret = seed;

      for (var i = 1; i <= 2000; i++)
      {
        secret = Mix(secret, secret * 64);
        secret = Prune(secret);
        
        secret = Mix(secret, secret / 32);
        secret = Prune(secret);
        
        secret = Mix(secret, secret * 2048);
        secret = Prune(secret);
      }
        
      D($"{seed} : {secret}");
      acc += secret;
    }
    
    return acc.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    //130321 possible changes to check, no better idea than brute-force
    //EDIT there is a better idea: when building the dictionary of change info per seed,
    //you can total up the value of each changeInfo from there (across all lines), and then just take the 
    //max of that. and that way you're only even bothering to keep track of the change permutations that have
    //at least one price associated (many don't) and you're only looping through things once. i'm not rewriting
    //this code though, it got the answer for me in like 25 seconds lmao
    //you could also use bitshifts n shit to track queued changes and avoid allocating string keys and stuff.
    //next year ¯\_(ツ)_/¯
    DebugMode = isTestInput;

    if (isTestInput)
    {
      input = ["1", "2", "3", "2025"];
    }
    
    var bestBananas = 0;
    var bestKnownChange = Array.Empty<int>();

    var priceInfo = new List<Dictionary<string, int>>();
    
    foreach (var line in input)
    {
      var seed = long.Parse(line);
      priceInfo.Add(GetChangeLookupBySeed(seed));
    }

    foreach (var change in GetAllPossibleChanges())
    {
      var currentBestPrice = 0;
      var isPossibleWinner = true;
      for (var currentLine = 0; isPossibleWinner && currentLine < priceInfo.Count; currentLine++)
      {
        currentBestPrice += GetBestPriceIfAny(change, priceInfo[currentLine]);
        if(currentBestPrice + (9 * (priceInfo.Count - currentLine -1)) < bestBananas) 
          isPossibleWinner = false;
      }

      if (currentBestPrice > bestBananas)
      {
        D($"best change found: price {currentBestPrice}, change: {change[0]}|{change[1]}|{change[2]}|{change[3]}");
        bestBananas = currentBestPrice;
        bestKnownChange = change;
      }
    }
    
    D($"best known change: {bestKnownChange[0]}|{bestKnownChange[1]}|{bestKnownChange[2]}|{bestKnownChange[3]}");
    return bestBananas.ToString();
  }

  private static long Mix(long secret, long mixin)
    => secret ^ mixin;

  private static long Prune(long secret)
    => secret % 16777216;

  private static Dictionary<string, int> GetChangeLookupBySeed(long seed)
  {
    var secret = seed;
    var lineInfo = new Dictionary<string, int>();
    var changeQueue = new ChangeQueue();
      
    var firstPrice = (int)seed % 10;
    var lastPrice = firstPrice;

    for (var i = 1; i <= 2000; i++)
    {
      secret = Mix(secret, secret * 64);
      secret = Prune(secret);
        
      secret = Mix(secret, secret / 32);
      secret = Prune(secret);
        
      secret = Mix(secret, secret * 2048);
      secret = Prune(secret);

      var price = (int)secret % 10;
      var change = price - lastPrice;
      
      changeQueue.AddItem(change);

      if (changeQueue.IsFull)
      {
        var changeKey = changeQueue.ToString();
        if(!lineInfo.ContainsKey(changeKey)) lineInfo.Add(changeQueue.ToString(), price);
      }
      lastPrice = price;
    }
    
    return lineInfo;
  }
  
  private static int GetBestPriceIfAny(int[] change, Dictionary<string, int> lineInfo)
  {
    var changeKey = GetChangeKey(change);
    return lineInfo.GetValueOrDefault(changeKey, 0);
  }

  private static IEnumerable<int[]> GetAllPossibleChanges()
  {
    for (var i = -9; i <= 9; i++)
    {
      for (var j = -9; j <= 9; j++)
      {
        for (var k = -9; k <= 9; k++)
        {
          for (var l = -9; l <= 9; l++)
          {
            yield return [i, j, k, l];
          }
        }
      }
    }
  }

  private static string GetChangeKey(int[] changes) 
    => $"{changes[0]}|{changes[1]}|{changes[2]}|{changes[3]}";

  private class ChangeQueue
  {
    private readonly Queue<int> _queue = new();
    private const int MaxSize = 4;

    public void AddItem(int change)
    {
      _queue.Enqueue(change);
      if(_queue.Count > MaxSize) _queue.Dequeue();
    }
    
    public bool IsFull => _queue.Count == MaxSize;
    
    public override string ToString() =>
      string.Join("|", _queue);
  }
}