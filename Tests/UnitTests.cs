using System.Reflection;
using System.Text;

using FastPayment.Core;

namespace Tests;

public class UnitTests {
  [Fact]
  public void Check_If_Ctor_Works() {
    var p = new Payment(
      date: DateTime.Parse("2025-01-01 14:00:00"),
      transactionType: MMEXTransactionTypes.Widthdrawl,
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
    Assert.Equal(MMEXTransactionTypes.Widthdrawl, p.TransactionType);
    Assert.Equal(100.0m, p.TotalAmount);
    Assert.Equal("Maciej Piotrowski", p.Payee);
    Assert.Equal(new Category("Samochód", "Paliwo", 100.0m), p.Categories.First());
    Assert.Equal("None", p.Notes);
  }

  [Fact]
  public void ThrowIf_SumOfAllCategories_IsNotEqual_ToTotalAmount() {

    Action act = () => _ = new Payment(
      date: DateTime.Parse("2025-01-01 14:00:00"),
      transactionType: MMEXTransactionTypes.Widthdrawl,
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
      transactionType: MMEXTransactionTypes.Deposit,
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

  [Fact]
  public void Parser_Should_ParseFirst10Columns() {
    const string input = """
"2025-10-01","2025-10-01","Wypłata z bankomatu","-100.00","PLN","+100.00","Tytuł: <Some title> ","Lokalizacja: Adres: <Street Name> Miasto: <City Name> Kraj: <Country name>","Data wykonania operacji: 2025-10-01 02:00","Oryginalna kwota operacji: 100.00","Numer karty: <Card number>","",""
""";

    IEnumerable<BankTransaction> actual = BankStatementParser.Parse([input], skipHeader: false);
    Assert.Single(actual);

    BankTransaction t = actual.First();
    Assert.Equal(DateTime.Parse("2025-10-01"), t.OperationDate);
    Assert.Equal(DateTime.Parse("2025-10-01"), t.CurrencyDate);
    Assert.Equal(BankTransactionType.AtmWithdrawal, t.TransactionType);
    Assert.Equal(-100.00m, t.Amount);
    Assert.Equal("PLN", t.Currency);
    Assert.Equal(100.00m, t.BalanceAfterTransaction);
    Assert.Equal("Tytuł: <Some title>", t.TransactionDescription);
    Assert.Equal("Lokalizacja: Adres: <Street Name> Miasto: <City Name> Kraj: <Country name>", t.UnnamedProperty1);
    Assert.Equal("Data wykonania operacji: 2025-10-01 02:00", t.UnnamedProperty2);
    Assert.Equal("Oryginalna kwota operacji: 100.00", t.UnnamedProperty3);
  }

  [Fact]
  public void BankTransaction_Should_RetriveAdditionalInformation_FromItsContent_ForAtmWithdrawl() {
    const string input = """
"2025-10-01","2025-10-01","Wypłata z bankomatu","-100.00","PLN","+100.00","Tytuł: <Some title> ","Lokalizacja: Adres: <Street Name> Miasto: <City Name> Kraj: <Country name>","Data wykonania operacji: 2025-10-01 02:00","Oryginalna kwota operacji: 100.00","Numer karty: <Card number>","",""
""";

    IEnumerable<BankTransaction> actual = BankStatementParser.Parse([input], skipHeader: false);
    Assert.Single(actual);

    BankTransaction t = actual.First();
    Assert.Equal("<Some title>", t.GetTitle());
    Assert.Equal("<Street Name>", t.GetAddress());
    Assert.Equal("<City Name>", t.GetCityName());
    Assert.Equal("<Country name>", t.GetCountryName());
    Assert.Equal(DateTime.Parse("2025-10-01 02:00:00 AM"), t.GetOperationDateTime());
  }

  [Fact]
  public void BankTransaction_Should_RetriveAdditionalInformation_FromItsContent_ForStandingOrder() {
    const string input = """
"2025-10-01","2025-10-01","Zlecenie stałe","-100.00","PLN","+100.00","Rachunek odbiorcy: <number>","Nazwa odbiorcy: <name>","Tytuł: <title>","","","",""
""";

    IEnumerable<BankTransaction> actual = BankStatementParser.Parse([input], skipHeader: false);
    Assert.Single(actual);

    BankTransaction t = actual.First();
    Assert.Equal("<number>", t.GetRecieverBankAccount());
    Assert.Equal("<name>", t.GetRecieverName());
    Assert.Equal("<title>", t.GetTitle());
  }

  [Fact]
  public void BankTransaction_Should_RetriveAdditionalInformation_FromItsContent_ForCardPayment() {
    const string input = """
"2025-10-01","2025-10-01","Płatność kartą","-100.00","PLN","+100.00","Tytuł:  <title>","Lokalizacja: Adres: <address> Miasto: <city> Kraj: <country>","Data wykonania operacji: 2025-10-01 02:00","Oryginalna kwota operacji: 100.00","Numer karty: <number>","",""
""";

    IEnumerable<BankTransaction> actual = BankStatementParser.Parse([input], skipHeader: false);
    Assert.Single(actual);

    BankTransaction t = actual.First();
    Assert.Equal("<title>", t.GetTitle());
    Assert.Equal("<address>", t.GetAddress());
    Assert.Equal("<city>", t.GetCityName());
    Assert.Equal("<country>", t.GetCountryName());
    Assert.Equal(DateTime.Parse("2025-10-01 02:00:00 AM"), t.GetOperationDateTime());
  }

  [Fact]
  public void BankTransaction_Should_RetriveAdditionalInformation_FromItsContent_ForAccountTransfer() {
    const string input = """
"2025-10-01","2025-10-01","Przelew na konto","+100.00","PLN","+100.0","Rachunek nadawcy: <number>","Nazwa nadawcy: <name>","Adres nadawcy: <address>","Tytuł: <title>  ","Referencje własne zleceniodawcy: <number>","",""
""";

    IEnumerable<BankTransaction> actual = BankStatementParser.Parse([input], skipHeader: false);
    Assert.Single(actual);

    BankTransaction t = actual.First();
    Assert.Equal("<number>", t.GetSenderBankAccount());
    Assert.Equal("<name>", t.GetSenderName());
    Assert.Equal("<address>", t.GetSenderAddress());
    Assert.Equal("<title>", t.GetTitle());
  }

  [Fact]
  public void BankTransaction_Should_RetriveAdditionalInformation_FromItsContent_ForIncomingPhoneTransfer() {
    const string input = """
"2025-10-01","2025-10-01","Przelew na telefon przychodz. zew.","-100.00","PLN","+100.00","Rachunek odbiorcy: <number>","Nazwa odbiorcy: <name>","Tytuł: <title>","","","",""
""";

    IEnumerable<BankTransaction> actual = BankStatementParser.Parse([input], skipHeader: false);
    Assert.Single(actual);

    BankTransaction t = actual.First();
    Assert.Equal("<number>", t.GetRecieverBankAccount());
    Assert.Equal("<name>", t.GetRecieverName());
    Assert.Equal("<title>", t.GetTitle());
  }

  [Fact]
  public void BankTransaction_Should_RetriveAdditionalInformation_FromItsContent_ForCommision() {
    const string input = """
"2025-10-01","2025-10-01","Prowizja","-100.00","PLN","+100.00","<title>","","","","","",""
""";

    IEnumerable<BankTransaction> actual = BankStatementParser.Parse([input], skipHeader: false);
    Assert.Single(actual);

    BankTransaction t = actual.First();
    Assert.Equal("<title>", t.GetTitle());
  }

  [Fact]
  public void BankTransaction_Should_RetriveAdditionalInformation_FromItsContent_ForMobileWebPayment() {
    const string input = """
"2025-10-01","2025-10-01","Płatność web - kod mobilny","-100.00","PLN","+100.00","Tytuł: <title>","Numer telefonu: <number>","Lokalizacja: Adres: <address>","'Operacja: <number>","Numer referencyjny: <number>","",""
""";

    IEnumerable<BankTransaction> actual = BankStatementParser.Parse([input], skipHeader: false);
    Assert.Single(actual);

    BankTransaction t = actual.First();
    Assert.Equal("<title>", t.GetTitle());
    Assert.Equal("<address>", t.GetAddress());
  }

  [Fact]
  public void BankTransaction_Should_RetriveAdditionalInformation_FromItsContent_ForVariableOrder() {
    const string input = """
"2025-10-01","2025-10-01","Zlecenie zmienne","-100.00","PLN","+100.00","Rachunek odbiorcy: <number>","Nazwa odbiorcy: <name>","Adres odbiorcy: <address>","Tytuł: <title>","","",""
""";

    IEnumerable<BankTransaction> actual = BankStatementParser.Parse([input], skipHeader: false);
    Assert.Single(actual);

    BankTransaction t = actual.First();
    Assert.Equal("<number>", t.GetRecieverBankAccount());
    Assert.Equal("<name>", t.GetRecieverName());
    Assert.Equal("<address>", t.GetAddress());
  }

  [Fact]
  public void BankTransaction_Should_RetriveAdditionalInformation_FromItsContent_ForOutgoingAccountTransfer() {
    const string input = """
"2025-10-01","2025-10-01","Przelew z rachunku","-100.00","PLN","+100.00","Rachunek odbiorcy: <number>","Nazwa odbiorcy: <name>","Tytuł: <title>","Referencje własne zleceniodawcy: <number>","","",""
""";

    IEnumerable<BankTransaction> actual = BankStatementParser.Parse([input], skipHeader: false);
    Assert.Single(actual);

    BankTransaction t = actual.First();
    Assert.Equal("<number>", t.GetRecieverBankAccount());
    Assert.Equal("<name>", t.GetRecieverName());
    Assert.Equal("<title>", t.GetTitle());
  }

  [Fact]
  public void BankTransaction_Should_RetriveAdditionalInformation_FromItsContent_ForForeignCurrencyTransfer() {
    const string input = """
"2025-10-01","2025-10-01","Przelew zagraniczny i walutowy","-100.00","PLN","+100.00","Rachunek odbiorcy: <number>","Nazwa odbiorcy: <name>","Tytuł: <title>","Oryginalna kwota przelewu: <number> <currency>","Kurs przewalutowania: <number>","Opłaty za przelew: <text>","Numer referencyjny przelewu: <text>"
""";

    IEnumerable<BankTransaction> actual = BankStatementParser.Parse([input], skipHeader: false);
    Assert.Single(actual);

    BankTransaction t = actual.First();
    Assert.Equal("<number>", t.GetRecieverBankAccount());
    Assert.Equal("<name>", t.GetRecieverName());
    Assert.Equal("<title>", t.GetTitle());
  }

  [Fact]
  public void BankTransaction_Should_RetriveAdditionalInformation_FromItsContent_ForAtmWithdrawalMobileCode() {
    const string input = """
"2025-10-10","2025-10-10","Wypłata w bankomacie - kod mobilny","-100.00","PLN","+100.00","Tytuł: <title>","Numer telefonu: <number>","Lokalizacja: Adres: <address> Miasto: <city> Kraj: <country>","Bankomat: <name>","'Operacja: <number>","Numer referencyjny: <number>",""
""";

    IEnumerable<BankTransaction> actual = BankStatementParser.Parse([input], skipHeader: false);
    Assert.Single(actual);

    BankTransaction t = actual.First();
    Assert.Equal("<title>", t.GetTitle());
    Assert.Equal("<address>", t.GetAddress());
    Assert.Equal("<city>", t.GetCityName());
    Assert.Equal("<country>", t.GetCountryName());
  }

  [Fact]
  public void ParserAndBankTransaction_SmokeTest() {
    // Requried to get 1250 encoding page.
    // See: https://stackoverflow.com/a/47017180
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    // Just throw some random bank statement and see if it is capable of processing it.
    string path = Path.Combine("test_data", "1.csv");
    IEnumerable<string> data = File.ReadLines(path, Encoding.GetEncoding(1250));

    // If call throws, test automatically failes.
    IEnumerable<BankTransaction> items = BankStatementParser.Parse(data.ToArray(), skipHeader: true);

    // Here I'm just calling every possible method on BankStatement to make sure it does not crash.
    foreach (var item in items) {
      Type t = item.GetType();
      MethodInfo[] methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public);

      foreach (var method in methods) {
        if (method.GetParameters().Length == 0) {
          method.Invoke(item, null);
        }
      }
    }
  }
}