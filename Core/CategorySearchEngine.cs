using System.Diagnostics;

namespace Core;

public class CategorySearchEngine {
  private readonly IEnumerable<string> _searchSpace;

  public CategorySearchEngine(IEnumerable<string> searchSpace) {

    ArgumentNullException.ThrowIfNull(searchSpace);
    _searchSpace = searchSpace;
  }

  /// <summary>
  /// Searches the search space for categories and subcategories
  /// that start with the input string. Search is case-sensitive.
  /// </summary>
  /// <param name="input">String which might contains category and subcategory separated by colon.</param>
  /// <returns>Return all possible matches which starts with category or subcategory.</returns>
  public IEnumerable<string> Search(string input) {
    return Search(input, StringComparison.Ordinal);
  }

  /// <summary>
  /// Searches the search space for categories and subcategories
  /// that start with the input string using the specified comparison type.
  /// </summary>
  /// <param name="input">String which might contains category and subcategory separated by colon.</param>
  /// <param name="comparisonType">Specifies how string should be compared</param>
  /// <returns>Return all possible matches which starts with category or subcategory.</returns>
  public IEnumerable<string> Search(string input, StringComparison comparisonType) {
    string[] inputSplit = input.Split(":");
    string inputCategory = inputSplit[0].Trim();
    string? inputSubcategory = inputSplit.Length == 2 ? inputSplit[1].Trim() : null;

    List<string> result = [];
    foreach (var el in _searchSpace) {
      string[] candidateSplit = el.Split(":");
      string candidateCategory = candidateSplit[0].Trim();
      string? candidateSubcategory = candidateSplit.Length == 2 ? candidateSplit[1].Trim() : null;

      // Category should be always available. Sub., sometimes.
      Debug.Assert(
        string.IsNullOrEmpty(candidateCategory) == false,
        "Category for candidate should never be empty.");

      // It is up to input to define how are we searching.
      // Search for category and subcategory if input and candidate has both.
      // It is implicitly said that candidate always has category.
      if (string.IsNullOrEmpty(inputCategory) == false
        && string.IsNullOrEmpty(inputSubcategory) == false) {

        if (string.IsNullOrEmpty(candidateSubcategory) == false) {
          if (candidateCategory.StartsWith(inputCategory, comparisonType)
            && candidateSubcategory.StartsWith(inputSubcategory, comparisonType)) {
            result.Add(el);
          }
          else {
            continue;
          }
        }
      }
      // Search by category only.
      else if (string.IsNullOrEmpty(inputCategory) == false) {
        if (candidateCategory.StartsWith(inputCategory, comparisonType)) {
          result.Add(el);
        }
      }
      // Search by subcategory only, if possible.
      else if (string.IsNullOrEmpty(inputSubcategory) == false
        && string.IsNullOrEmpty(candidateSubcategory) == false) {
        // Seach by just using sub.
        if (candidateSubcategory.StartsWith(inputSubcategory, comparisonType)) {
          result.Add(el);
        }
      }
    }

    return result;
  }
}