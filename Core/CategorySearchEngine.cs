namespace FastPayment.Core;

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
    string? inputSubcategory = null;

    if (inputSplit.Length == 2) {
      inputSubcategory = inputSplit[1].Trim();
    }

    List<string> result = [];
    foreach (var el in _searchSpace) {
      string[] categorySplit = el.Split(":");
      string candidateCategory = categorySplit[0].Trim();
      string? candidateSubcategory = null;

      if (inputSplit.Length == 2 && categorySplit.Length == 2) {

        candidateSubcategory = categorySplit[1].Trim();
      }

      // We can't compare subcategories against each other 
      // if one is null.
      if (inputSubcategory is null || candidateSubcategory is null) {
        if (candidateCategory.StartsWith(inputCategory, comparisonType)
          && string.IsNullOrEmpty(inputCategory) == false) {
          result.Add(el);
        }
      }
      else {
        if (candidateCategory.StartsWith(inputCategory, comparisonType)
          && candidateSubcategory.StartsWith(inputSubcategory, comparisonType)) {
          result.Add(el);
        }
      }
    }

    return result;
  }
}