using System.Text;

namespace Core;

public class QifBuilder {
  private const string _header =
"""
!Account
NWspólne
TInvoice
D[PLN]
""";

  private readonly StringBuilder _sb = new();

  public QifBuilder() {
  }

  public string Build() {
    return _header + Environment.NewLine + _sb.ToString().TrimEnd();
  }

  public QifBuilder WithDate(DateTime dt) {
    _sb.AppendLine("D" + dt.ToString("yyyy-MM-dd"));
    return this;
  }

  public QifBuilder WithTotalCost(decimal value) {
    _sb.AppendLine("T-" + value.ToString("F2"));
    return this;
  }

  public QifBuilder WithPayee(string value) {
    _sb.AppendLine("P" + value);
    return this;
  }

  public QifBuilder WithSplit(string value) {
    _sb.AppendLine("S" + value);
    return this;
  }

  public QifBuilder WithSplitAmountCost(decimal value) {
    _sb.AppendLine("$-" + value.ToString("F2"));
    return this;
  }

  public QifBuilder StartTransaction() {
    _sb.AppendLine("^");
    return this;
  }

  public QifBuilder EndTransaction() {
    _sb.AppendLine("^");
    return this;
  }

  public QifBuilder WithTotalDeposit(decimal value) {
    _sb.AppendLine("T" + value.ToString("F2"));
    return this;
  }

  public QifBuilder WithSplitAmountDeposit(decimal value) {
    _sb.AppendLine("$" + value.ToString("F2"));
    return this;
  }


  /// <summary>
  /// Add notes to the transaction. Supports multiline notes.
  /// </summary>
  /// <param name="text"></param>
  /// <returns>New instance of <c>QifBuilder</c> with note.</returns>
  public QifBuilder WithNote(string text) {
    string[] split = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    foreach (var value in split) {
      _sb.AppendLine("N" + value);
    }
    return this;
  }

  public QifBuilder AddTransaction(Payment payment) {
    StartTransaction();
    WithDate(payment.Date);

    if (payment.TransactionType == MMEXTransactionTypes.Widthdrawl) {
      WithTotalCost(payment.TotalAmount);
    }
    else if (payment.TransactionType == MMEXTransactionTypes.Deposit) {
      WithTotalDeposit(payment.TotalAmount);
    }
    else {
      throw new NotSupportedException(
        $"'{Enum.GetName(payment.TransactionType)}' is not supported transaction type.");
    }

    WithPayee(payment.Payee);

    if (payment.Categories != null) {
      foreach (var cat in payment.Categories) {
        // Some payments will not have subcategory.
        if (string.IsNullOrEmpty(cat.Subcategory)) {
          WithSplit(cat.Name);
        }
        else {
          WithSplit(cat.Name + ":" + cat.Subcategory);
        }

        if (payment.TransactionType == MMEXTransactionTypes.Widthdrawl) {
          WithSplitAmountCost(cat.Amount);
        }
        else if (payment.TransactionType == MMEXTransactionTypes.Deposit) {
          WithSplitAmountDeposit(cat.Amount);
        }
      }
    }

    WithNote(payment.Notes);

    EndTransaction();
    return this;
  }
}