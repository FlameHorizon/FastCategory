using System.ComponentModel.DataAnnotations;

namespace Website.Models.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class EnumContainsAttribute : ValidationAttribute {
    private readonly Type _enumType;

    public EnumContainsAttribute(Type enumType) {
        if (enumType.IsEnum == false) {
            throw new ArgumentException("Type must be an enum.", nameof(enumType));
        }

        _enumType = enumType;
        ErrorMessage = $"Value must be one of: {string.Join(", ", Enum.GetNames(enumType))}.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
        if (value is null) {
            return ValidationResult.Success;
        }

        if (value is string strValue) {
            if (Enum.GetNames(_enumType).Any(n => string.Equals(n, strValue, StringComparison.OrdinalIgnoreCase))) {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage, [validationContext.MemberName ?? "Unknown MemberName"]);
        }

        return new ValidationResult("Invalid type for EnumContainsAttribute.", [validationContext.MemberName ?? "Unknown MemberName"]);
    }
}
