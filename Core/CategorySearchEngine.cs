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
    List<string> result = [];
    foreach (var el in _searchSpace) {
      string[] inputSplit = input.Split(":");

      string inputCategory = inputSplit[0].Trim();;
      string? inputSubcategory = null;

      if (inputSplit.Count() == 2)
      {
        inputSubcategory = inputSplit[1].Trim();
      }
      
      string[] categorySplit = el.Split(":");
      string candidateCategory = categorySplit[0].Trim();
      string? candidateSubcategory = null;

      if (inputSplit.Count() == 2)
      {
        candidateSubcategory = categorySplit[1].Trim();        
      }

      // We can't compare subcategories against each other 
      // if one is null.
      if (inputSubcategory is null || candidateSubcategory is null)
      {
        if (candidateCategory.StartsWith(inputCategory))
        {
          result.Add(el);
        }
      }
      else
      {
        if (candidateCategory.StartsWith(inputCategory) 
          && candidateSubcategory.StartsWith(inputSubcategory))
        {
          result.Add(el);
        }
      }
    }

    return result;
  }
}