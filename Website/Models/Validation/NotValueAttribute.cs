using System;
using System.ComponentModel.DataAnnotations;

namespace Website.Models.Validation;

public class NotValueAttribute : ValidationAttribute {
  public object DisallowedValue { get; }

  public NotValueAttribute(object disallowedValue) : base("This value is not allowed.") {
    DisallowedValue = disallowedValue;
  }

  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
    if (value == null)
      return ValidationResult.Success;

    try {
      // Convert both values to decimal for numeric comparisons
      if (IsNumericType(value) && IsNumericType(DisallowedValue)) {
        var numericValue = Convert.ToDecimal(value);
        var disallowed = Convert.ToDecimal(DisallowedValue);

        if (numericValue == disallowed)
          return new ValidationResult(ErrorMessage, [validationContext.MemberName!]);
      }
      else if (value.Equals(DisallowedValue)) {
        return new ValidationResult(ErrorMessage, [validationContext.MemberName!]);
      }
    }
    catch {
      // Ignore conversion errors — treat as valid
    }

    return ValidationResult.Success;
  }

  private static bool IsNumericType(object obj) {
    switch (Type.GetTypeCode(obj.GetType())) {
      case TypeCode.Byte:
      case TypeCode.SByte:
      case TypeCode.UInt16:
      case TypeCode.UInt32:
      case TypeCode.UInt64:
      case TypeCode.Int16:
      case TypeCode.Int32:
      case TypeCode.Int64:
      case TypeCode.Decimal:
      case TypeCode.Double:
      case TypeCode.Single:
        return true;
      default:
        return false;
    }
  }
}