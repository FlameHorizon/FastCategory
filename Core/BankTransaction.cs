
namespace FastPayment.Core;

public class BankTransaction {
    public DateTime OperationDate { get; set; }
    public DateTime CurrencyDate { get; set; }
    public BankTransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public decimal BalanceAfterTransaction { get; set; }
    public string TransactionDescription { get; set; }
    public string UnnamedProperty1 { get; set; }
    public string UnnamedProperty2 { get; set; }
    public string UnnamedProperty3 { get; set; }
}