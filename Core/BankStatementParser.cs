using Core;

public static class BankStatementParser {
  public static IEnumerable<BankTransaction> Parse(string[] lines, bool skipHeader) {
    List<BankTransaction> result = [];

    // Skip headers if there are any.
    IEnumerable<string> data = skipHeader ? lines.Skip(1) : lines;

    foreach ((Index index, string line) in data.Index()) {
      string[] split = line.Split("\",\"", StringSplitOptions.TrimEntries);
      if (split.Length != 13) {
        Console.WriteLine(
            $"Line on row {index.Value} should have 13 columns but have {split.Length}");
        continue;
      }

      BankTransaction value = new() {
        OperationDate = DateTime.Parse(split[0].Trim('"')),
        CurrencyDate = DateTime.Parse(split[1].Trim('"')),
        TransactionType = GetTransactionType(split[2].Trim('"')),
        Amount = decimal.Parse(split[3].Trim('"')),
        Currency = split[4].Trim('"'),
        BalanceAfterTransaction = decimal.Parse(split[5].Trim('"')),
        TransactionDescription = split[6].Trim('"'),
        UnnamedProperty1 = split[7].Trim('"'),
        UnnamedProperty2 = split[8].Trim('"'),
        UnnamedProperty3 = split[9].Trim('"')
      };

      result.Add(value);
    }
    return result;
  }

  private static BankTransactionType GetTransactionType(string value) {
    return value switch {
      "Wypłata z bankomatu" => BankTransactionType.AtmWithdrawal,
      "Zlecenie stałe" => BankTransactionType.StandingOrder,
      "Płatność kartą" => BankTransactionType.CardPayment,
      "Przelew na konto" => BankTransactionType.AccountTransfer,
      "Przelew na telefon przychodz. zew." => BankTransactionType.IncomingPhoneTransfer,
      "Płatność web - kod mobilny" => BankTransactionType.MobileWebPayment,
      "Prowizja" => BankTransactionType.Commission,
      "Zlecenie zmienne" => BankTransactionType.VariableOrder,
      "Przelew z rachunku" => BankTransactionType.OutgoingAccountTransfer,
      "Przelew zagraniczny i walutowy" => BankTransactionType.ForeignCurrencyTransfer,
      "Wypłata w bankomacie - kod mobilny" => BankTransactionType.AtmWithdrawalMobileCode,
      _ => throw new ArgumentOutOfRangeException($"Given value '{value}' is not mapped to enum {nameof(BankTransactionType)}.")
    };
  }
}