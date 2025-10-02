namespace FastPayment.Core;

// @@TODO: Rename to Transaction?
public class Payment {

  public DateTime Date { get; }
  public MMEXTransactionTypes TransactionType { get; }
  public decimal TotalAmount { get; }
  public string Payee { get; }
  public IEnumerable<Category> Categories { get; }
  public string Notes { get; }

  public Payment(
    DateTime date,
    MMEXTransactionTypes transactionType,
    decimal totalAmount,
    string payee,
    IEnumerable<Category> categories,
    string notes) {
    Date = date;
    TransactionType = transactionType;
    TotalAmount = totalAmount;
    Payee = payee;
    Categories = categories;
    Notes = notes;

    // Validation

    decimal sum = categories.Sum(x => x.Amount);
    if (sum != totalAmount) {
      throw new ArgumentException(
        $"Sum of all categories '{sum}' must be equal to total amount '{totalAmount}'.");
    }
  }
}