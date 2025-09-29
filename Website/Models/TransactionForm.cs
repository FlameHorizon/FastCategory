using MudBlazor;

namespace Website.Models;

public class TransactionForm {

  /// <summary>
  /// Date when transaction occured.
  /// </summary>
  public DateTime? Date { get; set; } = DateTime.Now;

  /// <summary>
  /// Type of the transaction. Either "Expense" or "Income".
  /// </summary>
  public string TransactionType { get; set; } = "Expense";

  /// <summary>
  /// Total amount of the transaction.
  /// </summary>
  public decimal TotalAmount { get; set; }

  /// <summary>
  /// Person or entity receiving the payment.
  /// </summary>
  public string Payee { get; set; } = string.Empty;

  /// <summary>
  /// Categories associated with the transaction.
  /// </summary>
  public List<CategoryAmount> Categories { get; set; } = [];

  /// <summary>
  /// Additional notes about the transaction.
  /// </summary>
  public string Notes { get; set; } = string.Empty;
}

public class CategoryAmount {
  public string Name { get; set; } = string.Empty;
  public decimal Amount { get; set; }

  /// <summary>
  /// Reference to the MudAutocomplete component for this category.
  /// Note: This should not be a part of the actual data model. It's included here for convenience in the UI layer.
  /// </summary>
  public MudAutocomplete<string>? Ref { get; set; }
}