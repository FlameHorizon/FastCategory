namespace FastPayment.Core;

public class BankTransaction {
    public DateTime OperationDate { get; set; }
    public DateTime CurrencyDate { get; set; }
    public BankTransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal BalanceAfterTransaction { get; set; }
    public string TransactionDescription { get; set; } = string.Empty;
    public string UnnamedProperty1 { get; set; } = string.Empty;
    public string UnnamedProperty2 { get; set; } = string.Empty;
    public string UnnamedProperty3 { get; set; } = string.Empty;

    // @@NOTE: Different information depeneding on the context
    // will be available at different times. If information is not available
    // null will be returned.

    public string? GetTitle() {
        string source = TransactionType switch {
            BankTransactionType.MobileWebPayment or
            BankTransactionType.AtmWithdrawal or
            BankTransactionType.AtmWithdrawalMobileCode or
            BankTransactionType.CardPayment => TransactionDescription,

            BankTransactionType.IncomingPhoneTransfer or
            BankTransactionType.OutgoingAccountTransfer or
            BankTransactionType.ForeignCurrencyTransfer or 
            BankTransactionType.StandingOrder => UnnamedProperty2,

            BankTransactionType.AccountTransfer or
            BankTransactionType.VariableOrder => UnnamedProperty3,

            BankTransactionType.Commission => "",

            _ => throw new InvalidOperationException(
                $"This transaction type '{Enum.GetName(TransactionType)}' does not support " +
                $"'{nameof(GetTitle)}' method.")
        };

        if (TransactionType == BankTransactionType.Commission) {
            return TransactionDescription;
        }

        return GetTextAfter(source, "Tytuł: ");
    }

    public string? GetAddress() {
        if (TransactionType == BankTransactionType.MobileWebPayment) {
            return GetTextAfter(UnnamedProperty2, "Adres: ");
        }
        else if (TransactionType == BankTransactionType.VariableOrder) {
            return GetTextAfter(UnnamedProperty2, "Adres odbiorcy:");
        }
        else if (TransactionType == BankTransactionType.AtmWithdrawalMobileCode) {
            return GetTextBetween(UnnamedProperty2, "Adres: ", " Miasto:");
        }
        else {
            return GetTextBetween(UnnamedProperty1, "Adres: ", " Miasto:");
        }
    }

    public string? GetCityName() {
        string source = TransactionType switch {
            BankTransactionType.AtmWithdrawalMobileCode => UnnamedProperty2,
            _ => UnnamedProperty1
        };
        return GetTextBetween(source, "Miasto: ", " Kraj:");
    }

    public string? GetCountryName() {
        string source = TransactionType switch {
            BankTransactionType.AtmWithdrawalMobileCode => UnnamedProperty2,
            _ => UnnamedProperty1
        };
        return GetTextAfter(source, "Kraj: ");
    }

    public DateTime? GetOperationDateTime() {
        string? value = GetTextAfter(UnnamedProperty2, "Data wykonania operacji: ");
        if (value is null) {
            return null;
        }

        return DateTime.Parse(value);
    }

    public string? GetRecieverBankAccount() {
        return GetTextAfter(TransactionDescription, "Rachunek odbiorcy: ");
    }

    public string? GetRecieverName() {
        return GetTextAfter(UnnamedProperty1, "Nazwa odbiorcy: ");
    }

    public string? GetSenderBankAccount() {
        return GetTextAfter(TransactionDescription, "Rachunek nadawcy: ");
    }

    public string? GetSenderName() {
        return GetTextAfter(UnnamedProperty1, "Nazwa nadawcy: ");
    }

    public string? GetSenderAddress() {
        return GetTextAfter(UnnamedProperty2, "Adres nadawcy: ");
    }

    /// <summary>
    /// Returns string after first occurence of a given value from a given input.
    /// </summary>
    /// <param name="input">String whithin which search will be done.</param>
    /// <param name="value">String which will be used as separator.</param>
    /// <returns>String after first occurence of value. Trimmed.</returns>
    private static string? GetTextAfter(string input, string value) {
        int len = value.Length;
        int index = input.IndexOf(value);
        if (index == -1) {
            return null;
        }

        string result = input.Substring(index + len).Trim();
        return result;
    }

    private static string? GetTextBetween(string input, string after, string before) {
        int afterLen = after.Length;
        int beforeLen = before.Length;

        int indexAfter = input.IndexOf(after);
        if (indexAfter == -1) {
            return null;
        }

        int indexBefore = input.IndexOf(before);
        if (indexBefore == -1) {
            return null;
        }

        string result = input[(indexAfter + afterLen)..indexBefore];
        return result;
    }
}