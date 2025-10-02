namespace FastPayment.Core;

public static class BankStatementParser {
  public static IEnumerable<BankTransaction> Parse(string[] lines, bool skipHeader) {
    List<BankTransaction> result = [];

    // Skip headers if there are any.
    IEnumerable<string> data = skipHeader ? lines.Skip(1) : lines;

    foreach ((Index index, string line) in data.Index()) {
      string[] split = line.Split(",");
      if (split.Length != 13) {
        Console.WriteLine(
            $"Line on row {index.Value} should have 13 columns but have {split.Length}");
        continue;
      }

      BankTransaction value = new() {
        OperationDate = DateTime.Parse(split[0]),
        CurrencyDate = DateTime.Parse(split[1]),
        TransactionType = GetTransactionType(split[2]),
        Amount = decimal.Parse(split[3]),
        Currency = split[4],
        BalanceAfterTransaction = decimal.Parse(split[5]),
        TransactionDescription = split[6],
        UnnamedProperty1 = split[7],
        UnnamedProperty2 = split[8],
        UnnamedProperty3 = split[9]
      };

      result.Add(value);
    }
    return result;
  }

  private static BankTransactionType GetTransactionType(string v) {
    throw new NotImplementedException();
  }
}