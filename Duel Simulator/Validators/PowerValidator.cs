using System;
using System.Globalization;
using System.Windows.Controls;

namespace AgentFire.Gwent.DuelSimulator.Validators
{
    public sealed class PowerValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strValue = Convert.ToString(value);

            if (string.IsNullOrEmpty(strValue))
            {
                return new ValidationResult(true, null);
            }

            return int.TryParse(strValue, out int i) ? (i <= 0 ? new ValidationResult(false, "More than zero, please.") : new ValidationResult(true, null)) : new ValidationResult(false, $"A number, please.");
        }
    }
}
