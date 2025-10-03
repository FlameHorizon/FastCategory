namespace FastPayment.Core;

public enum BankTransactionType {
  AtmWithdrawal,                // Wypłata z bankomatu
  StandingOrder,                // Zlecenie stałe
  CardPayment,                  // Płatność kartą
  AccountTransfer,              // Przelew na konto
  IncomingPhoneTransfer,        // Przelew na telefon przychodz. zew.
  MobileWebPayment,             // Płatność web - kod mobilny
  Commission,                   // Prowizja
  VariableOrder,                // Zlecenie zmienne
  OutgoingAccountTransfer,      // Przelew z rachunku
  ForeignCurrencyTransfer,      // Przelew zagraniczny i walutowy
  AtmWithdrawalMobileCode       // Wypłata w bankomacie - kod mobilny
}