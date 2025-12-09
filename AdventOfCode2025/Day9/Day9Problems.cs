namespace AdventOfCode2025.Day9;

using Util;

public class Day9Problems : Problems
{
  protected override int Day => 9;
  
  protected override string TestInput => @"7,1
11,1
11,7
9,7
9,5
2,5
2,3
7,3";

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var xBound = 0;
    var redTiles = input
      .Select(l =>
      {
        var coords = StringUtils.ExtractIntsFromString(l).ToArray();
        if(coords[0] > xBound) xBound = coords[0];
        return new GridPoint(coords[0], coords[1]);
      }).Order().ToArray();

    var maxArea = 0L;
    var yBound = redTiles.Last().Y;
    
    for (var i = 0; i < redTiles.Length; i++)
    {
      var remainingSearchArea = (1L + yBound - redTiles[i].Y) * (xBound + 1L);
      if (remainingSearchArea < maxArea) return maxArea.ToString();

      var lowerTile = redTiles[i];
      for (var upperIndex = redTiles.Length - 1; upperIndex > i; upperIndex--)
      {
        var upperTile = redTiles[upperIndex];
        var remainingStripeArea = (1L + upperTile.Y - lowerTile.Y) * (xBound + 1L);
        if(remainingStripeArea < maxArea) continue;
        
        var area = (1L + Math.Abs(upperTile.Y - lowerTile.Y)) * (1L + Math.Abs(upperTile.X - lowerTile.X));
        if (area > maxArea)
        {
          D($"new max area: {area} | {lowerTile} -> {upperTile}");
          maxArea = area;
        }
      }
    }

    //return maxArea.ToString(); //answer: 4777816465
    
    throw new ThisShouldNeverHappenException($"loop terminated without answer, max area: {maxArea}");
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    throw new NotImplementedException();
  }
}