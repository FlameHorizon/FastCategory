namespace FastPayment.Core;

public class Payment {

  public DateTime Date { get; }
  public TransactionTypes TransactionType { get; }
  public decimal TotalAmount { get; }
  public string Payee { get; }
  public IEnumerable<Category> Categories { get; }
  public string Notes { get; }

  public Payment(
    DateTime date,
    TransactionTypes transactionType,
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

    if (categories.Sum(x => x.Amount) != totalAmount) {
      throw new ArgumentException(
        "Sum of all categories must be equal to total amount.");
    }
  }
}