namespace AdventOfCode2025.Util;

public class CollectionUtils
{
  //https://stackoverflow.com/a/57058345/5503076 :goat:
  //also not combinations not permutations -> if you give it ([0,1], 0, 2) you'll get [0], [1], [0,1] but not [1,0]
  //also note not sorted in order of set size
  
  /// <summary>
  /// https://stackoverflow.com/a/57058345/5503076 :goat:
  /// note: returns combinations not permutations -> if you give it ([0,1], 0, 2) you'll get [0], [1], [0,1] but not [1,0]
  /// also note: they are not sorted in order of set size or any other particular way, if you need that, post-process it
  /// </summary>
  /// <param name="source"></param>
  /// <param name="minCombinationSize"></param>
  /// <param name="maxCombinationSize"></param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  public static IEnumerable<T[]> GetAllCombinations<T>(IEnumerable<T> source, int minCombinationSize = 0, int maxCombinationSize = 0) 
  {
    ArgumentNullException.ThrowIfNull(source);

    var data = source.ToArray();
    if(maxCombinationSize == 0) maxCombinationSize = data.Length;

    return Enumerable
      .Range(minCombinationSize, 1 << (maxCombinationSize))
      .Select(index => data
        .Where((v, i) => (index & (1 << i)) != 0)
        .ToArray());
  }
}