using FastPayment.Core;

namespace Tests;

public class UnitTests {
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
  public void Search_Returns_ResultsWhichStartsWithPhrase() {
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
      "Baterie: Inne",
    };

    var se = new CategorySearchEngine(searchSpace);

    IEnumerable<string> actual = se.Search(phrase);

    List<string> expected = new() {
      "Inne wydatki: Baterie",
      "Inne wydatki: Bizuteria"
    };

    Assert.Equal(expected, actual);
  }

  [Fact]
  public void Search_Should_NotIncludeCandidatesWithoutSubcategory_IfSubcategoryWasDefinedInInput() {
    string phrase = "Inne:B";
    List<string> searchSpace = new() {
      "Inne wydatki: Art. papiernicze",
      "Inne wydatki: Baterie",
      "Inne wydatki: Bizuteria",
      "Inne zakupy",
    };

    var se = new CategorySearchEngine(searchSpace);

    IEnumerable<string> actual = se.Search(phrase);

    List<string> expected = new() {
      "Inne wydatki: Baterie",
      "Inne wydatki: Bizuteria"
    };

    Assert.Equal(expected, actual);
  }

  [Theory]
  [InlineData("inne wydatki: b")]
  [InlineData("I:b")]
  [InlineData("inne:b")]
  public void Search_IsCaseSensitive(string phrase) {
    List<string> searchSpace = new() {
      "Inne wydatki: Art. papiernicze",
      "Inne wydatki: Baterie",
      "Inne wydatki: Bizuteria",
      "Baterie: Inne"
    };

    var se = new CategorySearchEngine(searchSpace);

    IEnumerable<string> actual = se.Search(phrase, StringComparison.OrdinalIgnoreCase);

    List<string> expected = new() {
      "Inne wydatki: Baterie",
      "Inne wydatki: Bizuteria"
    };

    Assert.Equal(expected, actual);
  }

  [Fact]
  public void Search_ShouldNot_SearchByCategoryIfPhraseDoesNotProvideIt() {
    string phrase = ":Praca";

    List<string> searchSpace = [
      "Art. Papiernicze",
      "Praca:Komputer",
      "Wynagordzenie:Praca"
    ];

    var se = new CategorySearchEngine(searchSpace);
    IEnumerable<string> actual = se.Search(phrase, StringComparison.OrdinalIgnoreCase);

    List<string> expected = [
      "Wynagordzenie:Praca"
    ];

    Assert.Equal(expected, actual);
  }

  [Fact]
  public void QifBuilder_Builds_CorrectQifString() {
    // Builder should already contain default header
    // which will be used most often by me.
    // This is the assumption which I've made myself
    // to make life easier.
    var builder = new QifBuilder();

    const string expected =
"""
!Account
NWspólne
TInvoice
D[PLN]

""";

    string actual = builder.Build();

    Assert.Equal(expected, actual);
  }

  [Fact]
  public void QifBuilder_Builds_WithAdditionalSetup() {
    var builder = new QifBuilder();
    string actual = builder
      .StartTransaction()
      .WithDate(DateTime.Parse("2025-08-29"))
      .WithTotalCost(136.04m)
      .WithPayee("Biedronka")
      .WithSplit("Jedzenie:Sok")
      .WithSplitAmountCost(5.49m)
      .WithSplit("Jedzenie:Dom")
      .WithSplitAmountCost(129.55m)
      .WithSplit("Jedzenie:Inne")
      .WithSplitAmountCost(1.00m)
      .EndTransaction()
      .Build();

    Assert.Contains("D2025-08-29", actual);
    Assert.Contains("T-136.04", actual);
    Assert.Contains("PBiedronka", actual);
    Assert.Contains("SJedzenie:Sok", actual);
    Assert.Contains("$-5.49", actual);
    Assert.Contains("SJedzenie:Dom", actual);
    Assert.Contains("$-129.55", actual);
    Assert.Contains("SJedzenie:Inne", actual);
    Assert.Contains("$-1.00", actual);
    Assert.EndsWith("^", actual);
  }

  [Fact]
  public void QifBuilder_Builds_QifString_WhenPaymentIsDeposit() {
    var builder = new QifBuilder();
    var p = new Payment(
      date: DateTime.Now,
      transactionType: TransactionTypes.Deposit,
      totalAmount: 100.00m,
      payee: "Bank",
      categories: [new(Name: "Wynagrodzenie", Subcategory: "Praca", Amount: 100.00m)],
      notes: ""
    );

    string actual = builder.AddTransaction(p).Build();

    Assert.Contains("T100.00", actual);
    Assert.Contains("PBank", actual);
    Assert.Contains("SWynagrodzenie:Praca", actual);
    Assert.Contains("$100.00", actual);
  }
}