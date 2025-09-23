using Fastenshtein;

namespace FastPayment.Core;

public class CategorySearchEngine {
  private readonly IEnumerable<string> _searchSpace;

  public CategorySearchEngine(IEnumerable<string> searchSpace) {

    if (searchSpace is null) {
      throw new ArgumentNullException(nameof(searchSpace));
    }
    _searchSpace = searchSpace;
  }

  public IEnumerable<string> Search(string input) {
    var lev = new Levenshtein(input);

    var distances = new List<(string Word, int Dist)>();
    foreach (var el in _searchSpace) {
      int dist = lev.DistanceFrom(el);

      Console.WriteLine($"Distance between '{el}' and '{input}' is {dist}");

      distances.Add((el, dist));
    }

    IEnumerable<string> result = distances
      .OrderBy(x => x.Dist)
      .Select(x => x.Word);

    return result;
  }
}