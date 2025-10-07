using System.Text;

namespace Core;

public class QifBuilder {
  private const string _header =
"""
!Account
N#AccountName#
TInvoice
D[PLN]
""";

  private readonly StringBuilder _sb = new();

  private readonly string _source = "";

  public QifBuilder(string source) {
    _source = source;
  }

  public string Build() {
    return _header.Replace("#AccountName#", _source) + Environment.NewLine + _sb.ToString().TrimEnd();
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

  public QifBuilder WithTransferDetails(string from, string to, decimal amount) {
    _sb.AppendLine($"P{amount.ToString("F2")} PLN {from} -> {amount.ToString("F2")} PLN {to}");

    // Always pick external account.
    if (from == _source) {
      _sb.AppendLine($"L[{to}]");
    }
    else {
      _sb.AppendLine($"L[{from}]");
    }
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
      _sb.AppendLine("M" + value);
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
    else if (payment.TransactionType == MMEXTransactionTypes.Transfer) {
      if (payment.TotalAmount > 0) {
        // Deposit - we are receiving transfer.
        WithTotalDeposit(payment.TotalAmount);
        WithTransferDetails(payment.Payee, _source, payment.TotalAmount);
      }
      else {
        // Withdrawal - we are sending money somewhere.
        WithTotalCost(-payment.TotalAmount);
        WithTransferDetails(_source, payment.Payee, -payment.TotalAmount);
      }

      WithNote(payment.Notes);
      EndTransaction();
      return this;
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