using FastPayment.Core;

namespace Tests;

public class UnitTest1 {
  [Fact]
  public void Check_If_Ctor_Works() {
    var p = new Payment(
      date: DateTime.Parse("2025-01-01 14:00:00"),
      transactionType: TransactionTypes.Widthdrawl,
      totalAmount: 100.0m,
      payee: "Maciej Piotrowski",
      categories: [
        new Category(
          Name: "Samochód",
          Subcategory: "Paliwo",
          Amount: 100.0m)
      ],
      notes: "None"
    );

    Assert.Equal(DateTime.Parse("2025-01-01 14:00:00"), p.Date);
    Assert.Equal(TransactionTypes.Widthdrawl, p.TransactionType);
    Assert.Equal(100.0m, p.TotalAmount);
    Assert.Equal("Maciej Piotrowski", p.Payee);
    Assert.Equal(new Category("Samochód", "Paliwo", 100.0m), p.Categories.First());
    Assert.Equal("None", p.Notes);
  }

  [Fact]
  public void ThrowIf_SumOfAllCategories_IsNotEqual_ToTotalAmount() {

    Action act = () => _ = new Payment(
      date: DateTime.Parse("2025-01-01 14:00:00"),
      transactionType: TransactionTypes.Widthdrawl,
      totalAmount: 100.0m,
      payee: "Maciej Piotrowski",
      categories: [
        new Category(
          Name: "Samochód",
          Subcategory: "Paliwo",
          Amount: 110.0m)
      ],
      notes: "None"
    );

    Assert.Throws<ArgumentException>(act);
  }

  [Fact]
  public void Search_Returns_ClosestMatch() {
    List<string> searchSpace = new() { "Samochód" };
    var se = new CategorySearchEngine(searchSpace);

    IEnumerable<string> actual = se.Search("Sam");

    Assert.Single(actual);
    Assert.Equal("Samochód", actual.First());
  }

  [Fact]
  public void Search_Returns_ClosestMatch_2() {
    List<string> searchSpace = new() { "Datek", "Dziecko", "Działka" };
    var se = new CategorySearchEngine(searchSpace);

    IEnumerable<string> actual = se.Search("Dat");

    Assert.Equal(new List<string> { "Datek" }, actual);
  }

  [Theory]
  [InlineData("Inne wydatki: B")]
  [InlineData("I:B")]
  [InlineData("Inne:B")]
  public void Search_SupportCategory_AndSubcategory(string phrase) {
    List<string> searchSpace = new() {
      "Inne wydatki: Art. papiernicze",
      "Inne wydatki: Baterie",
      "Inne wydatki: Bizuteria",
      "Baterie: Inne"
    };

    var se = new CategorySearchEngine(searchSpace);

    IEnumerable<string> actual = se.Search(phrase);

    List<string> expected = new() {
      "Inne wydatki: Baterie",
      "Inne wydatki: Bizuteria"
    };

    Assert.Equal(expected, actual);
  }
}