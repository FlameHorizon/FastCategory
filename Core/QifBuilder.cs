using System.Text;

namespace FastPayment.Core;

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

  public QifBuilder WithSplitAmount(decimal value) {
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

  public QifBuilder AddTransaction(Payment payment) {
    StartTransaction();
    WithDate(payment.Date);
    WithTotalCost(payment.TotalAmount);
    WithPayee(payment.Payee);

    if (payment.Categories != null) {
      foreach (var cat in payment.Categories) {
        WithSplit(cat.Name + ":" + cat.Subcategory);
        WithSplitAmount(cat.Amount);
      }
    }

    EndTransaction();
    return this;
  }
}