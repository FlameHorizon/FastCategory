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
    _sb.AppendLine(dt.ToString("yyyy-MM-dd"));
    return this;
  }

  public QifBuilder WithTotalCost(decimal value) {
    _sb.AppendLine("T-" + value.ToString());
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
    _sb.AppendLine("$-" + value.ToString());
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
}