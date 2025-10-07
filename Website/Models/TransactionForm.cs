using System.ComponentModel.DataAnnotations;
using Website.Models.Validation;

using MudBlazor;

namespace Website.Models;

public class TransactionForm {

  /// <summary>
  /// Date when transaction occured.
  /// </summary>
  [Required]
  public DateTime? Date { get; set; } = DateTime.Now;

  /// <summary>
  /// Type of the transaction. Either "Expense" or "Income".
  /// </summary>
  [Required]
  public string TransactionType { get; set; } = "";

  /// <summary>
  /// Total amount of the transaction.
  /// </summary>
  [Required]
  [NotValue(0)]
  public decimal TotalAmount { get; set; }

  /// <summary>
  /// Person or entity receiving the payment.
  /// </summary>
  [Required]
  public string Receiver { get; set; } = string.Empty;

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
  [Required]
  public string Name { get; set; } = string.Empty;

  [Required]
  public decimal Amount { get; set; }

  /// <summary>
  /// Reference to the MudAutocomplete component for this category.
  /// Note: This should not be a part of the actual data model. It's included here for convenience in the UI layer.
  /// </summary>
  public MudAutocomplete<string>? Ref { get; set; }
}