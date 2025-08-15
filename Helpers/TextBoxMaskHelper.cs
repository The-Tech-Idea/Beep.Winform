using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    /// <summary>
    /// Static helper class for handling text box masking, formatting, and validation for BeepSimpleTextBox
    /// </summary>
    public static class TextBoxMaskHelper
    {
        #region "Format Templates"

        // Default format strings for various mask types
        private static readonly string _currencyFormat = "C2";
        private static readonly string _percentageFormat = "P2";
        private static readonly string _decimalFormat = "N2";
        private static readonly string _monthYearFormat = "MM/yyyy";
        private static readonly string _dateFormat = "MM/dd/yyyy";
        private static readonly string _timeFormat = "HH:mm:ss";
        private static readonly string _dateTimeFormat = "MM/dd/yyyy HH:mm:ss";

        #endregion

        #region "Mask Generation"

        /// <summary>
        /// Generates a mask pattern based on the specified format type
        /// </summary>
        public static string GenerateMask(TextBoxMaskFormat maskFormat, string customMask = "", CultureInfo culture = null)
        {
            culture ??= CultureInfo.CurrentCulture;

            return maskFormat switch
            {
                TextBoxMaskFormat.Currency => "$#,##0.00",
                TextBoxMaskFormat.CurrencyWithoutSymbol => "#,##0.00",
                TextBoxMaskFormat.Percentage => "##0.00%",
                TextBoxMaskFormat.Decimal => "#,##0.00",
                TextBoxMaskFormat.CreditCard => "0000-0000-0000-0000",
                TextBoxMaskFormat.IPAddress => "000.000.000.000",
                TextBoxMaskFormat.PhoneNumber => "(000) 000-0000",
                TextBoxMaskFormat.SocialSecurityNumber => "000-00-0000",
                TextBoxMaskFormat.ZipCode => "00000-0000",
                TextBoxMaskFormat.Email => "",
                TextBoxMaskFormat.URL => "",
                TextBoxMaskFormat.Date => ConvertDateFormatToMask(culture.DateTimeFormat.ShortDatePattern),
                TextBoxMaskFormat.Time => "00:00:00",
                TextBoxMaskFormat.DateTime => ConvertDateFormatToMask($"{culture.DateTimeFormat.ShortDatePattern} HH:mm:ss"),
                TextBoxMaskFormat.TimeSpan => "00:00:00",
                TextBoxMaskFormat.Hexadecimal => "0x########",
                TextBoxMaskFormat.Custom => customMask,
                TextBoxMaskFormat.Alphanumeric => "",
                TextBoxMaskFormat.Numeric => "########",
                TextBoxMaskFormat.Password => "",
                TextBoxMaskFormat.Year => "0000",
                TextBoxMaskFormat.MonthYear => "00/0000",
                TextBoxMaskFormat.None => "",
                _ => ""
            };
        }

        /// <summary>
        /// Converts a date format string to a mask pattern
        /// </summary>
        private static string ConvertDateFormatToMask(string dateFormat)
        {
            return Regex.Replace(dateFormat, @"[dMyHhmsf]+", match => new string('0', match.Length));
        }

        /// <summary>
        /// Gets placeholder text for the specified mask format
        /// </summary>
        public static string GetPlaceholderText(TextBoxMaskFormat maskFormat, string customMask = "", CultureInfo culture = null)
        {
            culture ??= CultureInfo.CurrentCulture;

            return maskFormat switch
            {
                TextBoxMaskFormat.Currency => "$0.00",
                TextBoxMaskFormat.CurrencyWithoutSymbol => "0.00",
                TextBoxMaskFormat.Percentage => "0.00%",
                TextBoxMaskFormat.Decimal => "0.00",
                TextBoxMaskFormat.CreditCard => "1234-5678-9012-3456",
                TextBoxMaskFormat.IPAddress => "192.168.1.1",
                TextBoxMaskFormat.PhoneNumber => "(555) 123-4567",
                TextBoxMaskFormat.SocialSecurityNumber => "123-45-6789",
                TextBoxMaskFormat.ZipCode => "12345-6789",
                TextBoxMaskFormat.Email => "example@domain.com",
                TextBoxMaskFormat.URL => "https://www.example.com",
                TextBoxMaskFormat.Date => culture.DateTimeFormat.ShortDatePattern.ToUpper(),
                TextBoxMaskFormat.Time => "HH:MM:SS",
                TextBoxMaskFormat.DateTime => $"{culture.DateTimeFormat.ShortDatePattern.ToUpper()} HH:MM:SS",
                TextBoxMaskFormat.TimeSpan => "HH:MM:SS",
                TextBoxMaskFormat.Hexadecimal => "0xABCDEF",
                TextBoxMaskFormat.Custom => customMask,
                TextBoxMaskFormat.Alphanumeric => "ABC123",
                TextBoxMaskFormat.Numeric => "123456",
                TextBoxMaskFormat.Password => "••••••••",
                TextBoxMaskFormat.Year => "YYYY",
                TextBoxMaskFormat.MonthYear => "MM/YYYY",
                TextBoxMaskFormat.None => "",
                _ => ""
            };
        }

        #endregion

        #region "Apply Mask Methods - BeepSimpleTextBox Integration"

        /// <summary>
        /// Applies mask formatting to a BeepSimpleTextBox control
        /// </summary>
        public static void ApplyMaskToControl(TextBoxMaskFormat maskFormat, string customMask, string dateFormat, string timeFormat, ref string text, ref string placeholderText, bool isApplyingMask)
        {
            if (maskFormat == TextBoxMaskFormat.None || !isApplyingMask)
                return;

            try
            {
                // Set placeholder text if not already set
                if (string.IsNullOrEmpty(placeholderText))
                {
                    placeholderText = GetPlaceholderText(maskFormat, customMask);
                }

                // Apply formatting to existing text
                if (!string.IsNullOrEmpty(text))
                {
                    string formattedText = FormatTextForMask(text, maskFormat, customMask, dateFormat, timeFormat);
                    if (formattedText != text)
                    {
                        text = formattedText;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Mask formatting error: {ex.Message}");
            }
        }

        /// <summary>
        /// Formats text according to the specified mask format with custom date/time formats
        /// </summary>
        private static string FormatTextForMask(string input, TextBoxMaskFormat maskFormat, string customMask, string dateFormat, string timeFormat)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return maskFormat switch
            {
                TextBoxMaskFormat.Currency => FormatCurrency(input, CultureInfo.CurrentCulture),
                TextBoxMaskFormat.CurrencyWithoutSymbol => FormatCurrencyWithoutSymbol(input, CultureInfo.CurrentCulture),
                TextBoxMaskFormat.Percentage => FormatPercentage(input, CultureInfo.CurrentCulture),
                TextBoxMaskFormat.Decimal => FormatDecimal(input, CultureInfo.CurrentCulture),
                TextBoxMaskFormat.CreditCard => FormatCreditCard(input),
                TextBoxMaskFormat.PhoneNumber => FormatPhoneNumber(input),
                TextBoxMaskFormat.SocialSecurityNumber => FormatSSN(input),
                TextBoxMaskFormat.ZipCode => FormatZipCode(input),
                TextBoxMaskFormat.Date => FormatDateWithCustomFormat(input, dateFormat),
                TextBoxMaskFormat.Time => FormatTimeWithCustomFormat(input, timeFormat),
                TextBoxMaskFormat.DateTime => FormatDateTimeWithCustomFormat(input, dateFormat, timeFormat),
                TextBoxMaskFormat.Hexadecimal => FormatHexadecimal(input),
                TextBoxMaskFormat.Custom => FormatCustom(input, customMask),
                TextBoxMaskFormat.Year => FormatYear(input),
                TextBoxMaskFormat.MonthYear => FormatMonthYear(input),
                _ => input
            };
        }

        /// <summary>
        /// Validates if a character is allowed for the current mask at the given position
        /// </summary>
        public static bool IsValidMaskCharacter(char character, int position, string currentText, TextBoxMaskFormat maskFormat, string customMask, string dateFormat, string timeFormat)
        {
            if (maskFormat == TextBoxMaskFormat.None) return true;
            if (char.IsControl(character)) return true; // Allow control characters

            return maskFormat switch
            {
                TextBoxMaskFormat.Currency => char.IsDigit(character) || character == '.' || character == '$' || character == ',',
                TextBoxMaskFormat.CurrencyWithoutSymbol => char.IsDigit(character) || character == '.' || character == ',',
                TextBoxMaskFormat.Percentage => char.IsDigit(character) || character == '.' || character == '%',
                TextBoxMaskFormat.Decimal => char.IsDigit(character) || character == '.' || character == '-' || character == ',',
                TextBoxMaskFormat.CreditCard => char.IsDigit(character) || character == '-' || character == ' ',
                TextBoxMaskFormat.IPAddress => char.IsDigit(character) || character == '.',
                TextBoxMaskFormat.PhoneNumber => char.IsDigit(character) || character == '(' || character == ')' || character == '-' || character == ' ',
                TextBoxMaskFormat.SocialSecurityNumber => char.IsDigit(character) || character == '-',
                TextBoxMaskFormat.ZipCode => char.IsDigit(character) || character == '-',
                TextBoxMaskFormat.Email => char.IsLetterOrDigit(character) || character == '@' || character == '.' || character == '-' || character == '_',
                TextBoxMaskFormat.URL => char.IsLetterOrDigit(character) || "://.?&=-_#%+".Contains(character),
                TextBoxMaskFormat.Date => ValidateDateCharacterWithFormat(character, position, dateFormat),
                TextBoxMaskFormat.Time => char.IsDigit(character) || character == ':',
                TextBoxMaskFormat.DateTime => ValidateDateTimeCharacterWithFormat(character, position, dateFormat, timeFormat),
                TextBoxMaskFormat.Hexadecimal => char.IsDigit(character) || (character >= 'A' && character <= 'F') || (character >= 'a' && character <= 'f') || character == 'x',
                TextBoxMaskFormat.Custom => ValidateCustomCharacter(character, position, customMask),
                TextBoxMaskFormat.Alphanumeric => char.IsLetterOrDigit(character),
                TextBoxMaskFormat.Numeric => char.IsDigit(character) || character == '.' || character == '-',
                TextBoxMaskFormat.Password => true, // Allow any character for password
                TextBoxMaskFormat.Year => char.IsDigit(character),
                TextBoxMaskFormat.MonthYear => char.IsDigit(character) || character == '/',
                _ => true
            };
        }

        /// <summary>
        /// Validates the text against the mask format
        /// </summary>
        public static bool ValidateMaskText(string text, TextBoxMaskFormat maskFormat, out string errorMessage, string customMask = "")
        {
            errorMessage = "";

            if (maskFormat == TextBoxMaskFormat.None)
                return true;

            if (string.IsNullOrEmpty(text))
            {
                errorMessage = "This field is required.";
                return false;
            }

            try
            {
                switch (maskFormat)
                {
                    case TextBoxMaskFormat.Currency:
                    case TextBoxMaskFormat.CurrencyWithoutSymbol:
                        return decimal.TryParse(text.Replace("$", "").Replace(",", ""), out _) ||
                               (errorMessage = "Invalid currency format.") == null;

                    case TextBoxMaskFormat.Email:
                        var emailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
                        return emailRegex.IsMatch(text) || (errorMessage = "Invalid email format.") == null;

                    case TextBoxMaskFormat.Date:
                        return DateTime.TryParse(text, out _) || (errorMessage = "Invalid date format.") == null;

                    case TextBoxMaskFormat.Time:
                        return TimeSpan.TryParse(text, out _) || (errorMessage = "Invalid time format.") == null;

                    case TextBoxMaskFormat.PhoneNumber:
                        string digits = Regex.Replace(text, @"[^\d]", "");
                        return (digits.Length == 10 || digits.Length == 11) ||
                               (errorMessage = "Invalid phone number format.") == null;

                    case TextBoxMaskFormat.Year:
                        return (int.TryParse(text, out int year) && year >= 1900 && year <= 2100) ||
                               (errorMessage = "Invalid year format.") == null;

                    case TextBoxMaskFormat.MonthYear:
                        return DateTime.TryParseExact(text, "MM/yyyy", null, DateTimeStyles.None, out _) ||
                               (errorMessage = "Invalid month/year format.") == null;

                    default:
                        return true;
                }
            }
            catch (Exception)
            {
                errorMessage = "Invalid format.";
                return false;
            }
        }

        #endregion

        #region "Character Validation"

        /// <summary>
        /// Validates if a character is allowed for the specified mask format at the given position
        /// </summary>
        public static bool IsValidCharacterForMask(char character, TextBoxMaskFormat maskFormat, int position, string currentText, string mask)
        {
            // Allow control characters (backspace, delete, etc.)
            if (char.IsControl(character)) return true;

            return maskFormat switch
            {
                TextBoxMaskFormat.Currency => IsValidCurrencyChar(character, position, currentText),
                TextBoxMaskFormat.CurrencyWithoutSymbol => IsValidCurrencyWithoutSymbolChar(character, position, currentText),
                TextBoxMaskFormat.Percentage => IsValidPercentageChar(character, position, currentText),
                TextBoxMaskFormat.Decimal => IsValidDecimalChar(character, position, currentText),
                TextBoxMaskFormat.CreditCard => IsValidCreditCardChar(character, position, currentText),
                TextBoxMaskFormat.IPAddress => IsValidIPChar(character, position, currentText),
                TextBoxMaskFormat.PhoneNumber => IsValidPhoneChar(character, position, currentText),
                TextBoxMaskFormat.SocialSecurityNumber => IsValidSSNChar(character, position, currentText),
                TextBoxMaskFormat.ZipCode => IsValidZipChar(character, position, currentText),
                TextBoxMaskFormat.Email => IsValidEmailChar(character),
                TextBoxMaskFormat.URL => IsValidUrlChar(character),
                TextBoxMaskFormat.Date => IsValidDateChar(character, position, mask),
                TextBoxMaskFormat.Time => IsValidTimeChar(character, position, currentText),
                TextBoxMaskFormat.DateTime => IsValidDateTimeChar(character, position, mask),
                TextBoxMaskFormat.TimeSpan => IsValidTimeChar(character, position, currentText),
                TextBoxMaskFormat.Hexadecimal => IsValidHexChar(character),
                TextBoxMaskFormat.Custom => IsValidCustomChar(character, position, mask),
                TextBoxMaskFormat.Alphanumeric => char.IsLetterOrDigit(character),
                TextBoxMaskFormat.Numeric => char.IsDigit(character),
                TextBoxMaskFormat.Password => true, // Allow any character for password
                TextBoxMaskFormat.Year => char.IsDigit(character),
                TextBoxMaskFormat.MonthYear => char.IsDigit(character) || character == '/',
                TextBoxMaskFormat.None => true,
                _ => true
            };
        }

        #region "Character Validation Helpers"

        private static bool IsValidCurrencyChar(char c, int position, string currentText)
        {
            return char.IsDigit(c) || c == '.' || c == ',' || c == '$';
        }

        private static bool IsValidCurrencyWithoutSymbolChar(char c, int position, string currentText)
        {
            return char.IsDigit(c) || c == '.' || c == ',';
        }

        private static bool IsValidPercentageChar(char c, int position, string currentText)
        {
            return char.IsDigit(c) || c == '.' || c == '%';
        }

        private static bool IsValidDecimalChar(char c, int position, string currentText)
        {
            return char.IsDigit(c) || c == '.' || c == ',' || c == '-';
        }

        private static bool IsValidCreditCardChar(char c, int position, string currentText)
        {
            return char.IsDigit(c) || c == '-' || c == ' ';
        }

        private static bool IsValidIPChar(char c, int position, string currentText)
        {
            return char.IsDigit(c) || c == '.';
        }

        private static bool IsValidPhoneChar(char c, int position, string currentText)
        {
            return char.IsDigit(c) || c == '(' || c == ')' || c == '-' || c == ' ';
        }

        private static bool IsValidSSNChar(char c, int position, string currentText)
        {
            return char.IsDigit(c) || c == '-';
        }

        private static bool IsValidZipChar(char c, int position, string currentText)
        {
            return char.IsDigit(c) || c == '-';
        }

        private static bool IsValidEmailChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '@' || c == '.' || c == '-' || c == '_' || c == '+';
        }

        private static bool IsValidUrlChar(char c)
        {
            return char.IsLetterOrDigit(c) || "://.?&=-_#%+".Contains(c);
        }

        private static bool IsValidDateChar(char c, int position, string mask)
        {
            if (position >= mask.Length) return false;
            char maskChar = mask[position];

            return maskChar switch
            {
                '0' => char.IsDigit(c),
                '/' or '-' or ':' or ' ' => c == maskChar,
                _ => true
            };
        }

        private static bool IsValidTimeChar(char c, int position, string currentText)
        {
            return char.IsDigit(c) || c == ':';
        }

        private static bool IsValidDateTimeChar(char c, int position, string mask)
        {
            return IsValidDateChar(c, position, mask);
        }

        private static bool IsValidHexChar(char c)
        {
            return char.IsDigit(c) || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f') || c == 'x';
        }

        private static bool IsValidCustomChar(char c, int position, string mask)
        {
            if (position >= mask.Length) return false;
            char maskChar = mask[position];

            return maskChar switch
            {
                '0' => char.IsDigit(c),
                'A' => char.IsLetter(c),
                'a' => char.IsLetter(c),
                '9' => char.IsLetterOrDigit(c),
                _ => c == maskChar // Literal character
            };
        }

        #endregion

        #endregion

        #region "Text Formatting"

        /// <summary>
        /// Formats text according to the specified mask format
        /// </summary>
        public static string FormatText(string input, TextBoxMaskFormat maskFormat, string customMask = "", CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(input)) return input;

            culture ??= CultureInfo.CurrentCulture;

            try
            {
                return maskFormat switch
                {
                    TextBoxMaskFormat.Currency => FormatCurrency(input, culture),
                    TextBoxMaskFormat.CurrencyWithoutSymbol => FormatCurrencyWithoutSymbol(input, culture),
                    TextBoxMaskFormat.Percentage => FormatPercentage(input, culture),
                    TextBoxMaskFormat.Decimal => FormatDecimal(input, culture),
                    TextBoxMaskFormat.CreditCard => FormatCreditCard(input),
                    TextBoxMaskFormat.PhoneNumber => FormatPhoneNumber(input),
                    TextBoxMaskFormat.SocialSecurityNumber => FormatSSN(input),
                    TextBoxMaskFormat.ZipCode => FormatZipCode(input),
                    TextBoxMaskFormat.Hexadecimal => FormatHexadecimal(input),
                    TextBoxMaskFormat.Custom => FormatCustom(input, customMask),
                    TextBoxMaskFormat.Year => FormatYear(input),
                    TextBoxMaskFormat.MonthYear => FormatMonthYear(input),
                    _ => input
                };
            }
            catch
            {
                return input; // Return original if formatting fails
            }
        }

        #region "Format Helpers"

        private static string FormatCurrency(string input, CultureInfo culture)
        {
            if (decimal.TryParse(input.Replace("$", "").Replace(",", ""), out decimal value))
            {
                return value.ToString("C", culture);
            }
            return input;
        }

        private static string FormatCurrencyWithoutSymbol(string input, CultureInfo culture)
        {
            if (decimal.TryParse(input.Replace(",", ""), out decimal value))
            {
                return value.ToString("N", culture);
            }
            return input;
        }

        private static string FormatPercentage(string input, CultureInfo culture)
        {
            if (decimal.TryParse(input.Replace("%", ""), out decimal value))
            {
                return (value / 100).ToString("P", culture);
            }
            return input;
        }

        private static string FormatDecimal(string input, CultureInfo culture)
        {
            if (decimal.TryParse(input, out decimal value))
            {
                return value.ToString("N", culture);
            }
            return input;
        }

        private static string FormatCreditCard(string input)
        {
            string digits = Regex.Replace(input, @"[^\d]", "");
            if (digits.Length <= 4) return digits;
            if (digits.Length <= 8) return $"{digits.Substring(0, 4)}-{digits.Substring(4)}";
            if (digits.Length <= 12) return $"{digits.Substring(0, 4)}-{digits.Substring(4, 4)}-{digits.Substring(8)}";
            return $"{digits.Substring(0, 4)}-{digits.Substring(4, 4)}-{digits.Substring(8, 4)}-{digits.Substring(12)}";
        }

        private static string FormatPhoneNumber(string input)
        {
            string digits = Regex.Replace(input, @"[^\d]", "");
            if (digits.Length <= 3) return digits;
            if (digits.Length <= 6) return $"({digits.Substring(0, 3)}) {digits.Substring(3)}";
            return $"({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6)}";
        }

        private static string FormatSSN(string input)
        {
            string digits = Regex.Replace(input, @"[^\d]", "");
            if (digits.Length <= 3) return digits;
            if (digits.Length <= 5) return $"{digits.Substring(0, 3)}-{digits.Substring(3)}";
            return $"{digits.Substring(0, 3)}-{digits.Substring(3, 2)}-{digits.Substring(5)}";
        }

        private static string FormatZipCode(string input)
        {
            string digits = Regex.Replace(input, @"[^\d]", "");
            if (digits.Length <= 5) return digits;
            return $"{digits.Substring(0, 5)}-{digits.Substring(5)}";
        }

        private static string FormatHexadecimal(string input)
        {
            string hex = input.Replace("0x", "").Replace("0X", "");
            return $"0x{hex.ToUpper()}";
        }

        private static string FormatYear(string input)
        {
            string digits = Regex.Replace(input, @"[^\d]", "");
            if (digits.Length <= 4) return digits;
            return digits.Substring(0, 4);
        }

        private static string FormatMonthYear(string input)
        {
            string digits = Regex.Replace(input, @"[^\d]", "");
            if (digits.Length <= 2) return digits;
            if (digits.Length <= 6) return $"{digits.Substring(0, 2)}/{digits.Substring(2)}";
            return $"{digits.Substring(0, 2)}/{digits.Substring(2, 4)}";
        }

        private static string FormatCustom(string input, string mask)
        {
            if (string.IsNullOrEmpty(mask)) return input;

            StringBuilder result = new StringBuilder();
            int inputIndex = 0;

            foreach (char maskChar in mask)
            {
                if (inputIndex >= input.Length) break;

                switch (maskChar)
                {
                    case '0': // Digit
                        if (char.IsDigit(input[inputIndex]))
                        {
                            result.Append(input[inputIndex]);
                            inputIndex++;
                        }
                        else
                        {
                            return input; // Invalid format
                        }
                        break;

                    case 'A': // Letter (uppercase)
                        if (char.IsLetter(input[inputIndex]))
                        {
                            result.Append(char.ToUpper(input[inputIndex]));
                            inputIndex++;
                        }
                        else
                        {
                            return input; // Invalid format
                        }
                        break;

                    case 'a': // Letter (lowercase)
                        if (char.IsLetter(input[inputIndex]))
                        {
                            result.Append(char.ToLower(input[inputIndex]));
                            inputIndex++;
                        }
                        else
                        {
                            return input; // Invalid format
                        }
                        break;

                    case '9': // Letter or digit
                        if (char.IsLetterOrDigit(input[inputIndex]))
                        {
                            result.Append(input[inputIndex]);
                            inputIndex++;
                        }
                        else
                        {
                            return input; // Invalid format
                        }
                        break;

                    default: // Literal character
                        result.Append(maskChar);
                        if (input[inputIndex] == maskChar)
                        {
                            inputIndex++;
                        }
                        break;
                }
            }

            return result.ToString();
        }

        #endregion

        #region "Custom Date/Time Format Helpers"

        private static string FormatDateWithCustomFormat(string input, string dateFormat)
        {
            if (DateTime.TryParse(input, out DateTime date))
            {
                return date.ToString(dateFormat);
            }
            return input;
        }

        private static string FormatTimeWithCustomFormat(string input, string timeFormat)
        {
            if (TimeSpan.TryParse(input, out TimeSpan time))
            {
                return time.ToString(timeFormat);
            }
            return input;
        }

        private static string FormatDateTimeWithCustomFormat(string input, string dateFormat, string timeFormat)
        {
            if (DateTime.TryParse(input, out DateTime dateTime))
            {
                return dateTime.ToString($"{dateFormat} {timeFormat}");
            }
            return input;
        }

        private static bool ValidateDateCharacterWithFormat(char character, int position, string dateFormat)
        {
            if (position >= dateFormat.Length) return false;
            char formatChar = dateFormat[position];

            return formatChar switch
            {
                'M' or 'd' or 'y' => char.IsDigit(character),
                '/' or '-' or '.' or ' ' => character == formatChar,
                _ => true
            };
        }

        private static bool ValidateDateTimeCharacterWithFormat(char character, int position, string dateFormat, string timeFormat)
        {
            string fullFormat = $"{dateFormat} {timeFormat}";
            if (position >= fullFormat.Length) return false;

            char formatChar = fullFormat[position];
            return formatChar switch
            {
                'M' or 'd' or 'y' or 'H' or 'm' or 's' => char.IsDigit(character),
                '/' or '-' or '.' or ':' or ' ' => character == formatChar,
                _ => true
            };
        }

        private static bool ValidateCustomCharacter(char character, int position, string customMask)
        {
            if (position >= customMask.Length) return false;
            char maskChar = customMask[position];

            return maskChar switch
            {
                '0' => char.IsDigit(character),
                'A' => char.IsLetter(character),
                'a' => char.IsLetter(character),
                '9' => char.IsLetterOrDigit(character),
                _ => character == maskChar // Literal character
            };
        }

        #endregion

        #endregion

        #region "Validation"

        /// <summary>
        /// Validates text against the specified mask format
        /// </summary>
        public static bool ValidateText(string text, TextBoxMaskFormat maskFormat, out string errorMessage, string customMask = "")
        {
            errorMessage = "";

            if (string.IsNullOrWhiteSpace(text))
            {
                errorMessage = "This field is required.";
                return false;
            }

            return maskFormat switch
            {
                TextBoxMaskFormat.Currency => ValidateCurrency(text, out errorMessage),
                TextBoxMaskFormat.CurrencyWithoutSymbol => ValidateCurrencyWithoutSymbol(text, out errorMessage),
                TextBoxMaskFormat.Percentage => ValidatePercentage(text, out errorMessage),
                TextBoxMaskFormat.Decimal => ValidateDecimal(text, out errorMessage),
                TextBoxMaskFormat.CreditCard => ValidateCreditCard(text, out errorMessage),
                TextBoxMaskFormat.IPAddress => ValidateIPAddress(text, out errorMessage),
                TextBoxMaskFormat.PhoneNumber => ValidatePhoneNumber(text, out errorMessage),
                TextBoxMaskFormat.SocialSecurityNumber => ValidateSSN(text, out errorMessage),
                TextBoxMaskFormat.ZipCode => ValidateZipCode(text, out errorMessage),
                TextBoxMaskFormat.Email => ValidateEmail(text, out errorMessage),
                TextBoxMaskFormat.URL => ValidateUrl(text, out errorMessage),
                TextBoxMaskFormat.Date => ValidateDate(text, out errorMessage),
                TextBoxMaskFormat.Time => ValidateTime(text, out errorMessage),
                TextBoxMaskFormat.DateTime => ValidateDateTime(text, out errorMessage),
                TextBoxMaskFormat.TimeSpan => ValidateTimeSpan(text, out errorMessage),
                TextBoxMaskFormat.Hexadecimal => ValidateHexadecimal(text, out errorMessage),
                TextBoxMaskFormat.Custom => ValidateCustom(text, customMask, out errorMessage),
                TextBoxMaskFormat.Alphanumeric => ValidateAlphanumeric(text, out errorMessage),
                TextBoxMaskFormat.Numeric => ValidateNumeric(text, out errorMessage),
                TextBoxMaskFormat.Password => ValidatePassword(text, out errorMessage),
                TextBoxMaskFormat.Year => ValidateYear(text, out errorMessage),
                TextBoxMaskFormat.MonthYear => ValidateMonthYear(text, out errorMessage),
                TextBoxMaskFormat.None => true,
                _ => true
            };
        }

        #region "Validation Helpers"

        private static bool ValidateCurrency(string text, out string errorMessage)
        {
            errorMessage = "";
            if (decimal.TryParse(text.Replace("$", "").Replace(",", ""), out _))
                return true;

            errorMessage = "Invalid currency format.";
            return false;
        }

        private static bool ValidateCurrencyWithoutSymbol(string text, out string errorMessage)
        {
            errorMessage = "";
            if (decimal.TryParse(text.Replace(",", ""), out _))
                return true;

            errorMessage = "Invalid currency format.";
            return false;
        }

        private static bool ValidatePercentage(string text, out string errorMessage)
        {
            errorMessage = "";
            if (decimal.TryParse(text.Replace("%", ""), out _))
                return true;

            errorMessage = "Invalid percentage format.";
            return false;
        }

        private static bool ValidateDecimal(string text, out string errorMessage)
        {
            errorMessage = "";
            if (decimal.TryParse(text, out _))
                return true;

            errorMessage = "Invalid decimal format.";
            return false;
        }

        private static bool ValidateCreditCard(string text, out string errorMessage)
        {
            errorMessage = "";
            string digits = Regex.Replace(text, @"[^\d]", "");

            if (digits.Length >= 13 && digits.Length <= 19)
            {
                // Luhn algorithm validation
                int sum = 0;
                bool alternate = false;

                for (int i = digits.Length - 1; i >= 0; i--)
                {
                    int digit = int.Parse(digits[i].ToString());

                    if (alternate)
                    {
                        digit *= 2;
                        if (digit > 9) digit -= 9;
                    }

                    sum += digit;
                    alternate = !alternate;
                }

                if (sum % 10 == 0) return true;
            }

            errorMessage = "Invalid credit card number.";
            return false;
        }

        private static bool ValidateIPAddress(string text, out string errorMessage)
        {
            errorMessage = "";
            if (IPAddress.TryParse(text, out _))
                return true;

            errorMessage = "Invalid IP address format.";
            return false;
        }

        private static bool ValidatePhoneNumber(string text, out string errorMessage)
        {
            errorMessage = "";
            string digits = Regex.Replace(text, @"[^\d]", "");

            if (digits.Length == 10 || digits.Length == 11)
                return true;

            errorMessage = "Invalid phone number format.";
            return false;
        }

        private static bool ValidateSSN(string text, out string errorMessage)
        {
            errorMessage = "";
            string digits = Regex.Replace(text, @"[^\d]", "");

            if (digits.Length == 9)
                return true;

            errorMessage = "Invalid Social Security Number format.";
            return false;
        }

        private static bool ValidateZipCode(string text, out string errorMessage)
        {
            errorMessage = "";
            string digits = Regex.Replace(text, @"[^\d]", "");

            if (digits.Length == 5 || digits.Length == 9)
                return true;

            errorMessage = "Invalid ZIP code format.";
            return false;
        }

        private static bool ValidateEmail(string text, out string errorMessage)
        {
            errorMessage = "";
            var emailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");

            if (emailRegex.IsMatch(text))
                return true;

            errorMessage = "Invalid email address format.";
            return false;
        }

        private static bool ValidateUrl(string text, out string errorMessage)
        {
            errorMessage = "";
            if (Uri.TryCreate(text, UriKind.Absolute, out _))
                return true;

            errorMessage = "Invalid URL format.";
            return false;
        }

        private static bool ValidateDate(string text, out string errorMessage)
        {
            errorMessage = "";
            if (DateTime.TryParse(text, out _))
                return true;

            errorMessage = "Invalid date format.";
            return false;
        }

        private static bool ValidateTime(string text, out string errorMessage)
        {
            errorMessage = "";
            if (TimeSpan.TryParse(text, out _))
                return true;

            errorMessage = "Invalid time format.";
            return false;
        }

        private static bool ValidateDateTime(string text, out string errorMessage)
        {
            errorMessage = "";
            if (DateTime.TryParse(text, out _))
                return true;

            errorMessage = "Invalid date/time format.";
            return false;
        }

        private static bool ValidateTimeSpan(string text, out string errorMessage)
        {
            errorMessage = "";
            if (TimeSpan.TryParse(text, out _))
                return true;

            errorMessage = "Invalid time span format.";
            return false;
        }

        private static bool ValidateHexadecimal(string text, out string errorMessage)
        {
            errorMessage = "";
            string hex = text.Replace("0x", "").Replace("0X", "");
            var hexRegex = new Regex(@"^[0-9A-Fa-f]+$");

            if (hexRegex.IsMatch(hex))
                return true;

            errorMessage = "Invalid hexadecimal format.";
            return false;
        }

        private static bool ValidateCustom(string text, string mask, out string errorMessage)
        {
            errorMessage = "";

            if (string.IsNullOrEmpty(mask))
                return true;

            if (text.Length != mask.Length)
            {
                errorMessage = "Text length does not match mask.";
                return false;
            }

            for (int i = 0; i < mask.Length; i++)
            {
                char maskChar = mask[i];
                char textChar = text[i];

                bool isValid = maskChar switch
                {
                    '0' => char.IsDigit(textChar),
                    'A' => char.IsLetter(textChar),
                    'a' => char.IsLetter(textChar),
                    '9' => char.IsLetterOrDigit(textChar),
                    _ => textChar == maskChar
                };

                if (!isValid)
                {
                    errorMessage = $"Invalid character at position {i + 1}.";
                    return false;
                }
            }

            return true;
        }

        private static bool ValidateAlphanumeric(string text, out string errorMessage)
        {
            errorMessage = "";
            if (text.All(char.IsLetterOrDigit))
                return true;

            errorMessage = "Only letters and numbers are allowed.";
            return false;
        }

        private static bool ValidateNumeric(string text, out string errorMessage)
        {
            errorMessage = "";
            if (decimal.TryParse(text, out _))
                return true;

            errorMessage = "Invalid numeric format.";
            return false;
        }

        private static bool ValidatePassword(string text, out string errorMessage)
        {
            errorMessage = "";

            if (text.Length < 8)
            {
                errorMessage = "Password must be at least 8 characters long.";
                return false;
            }

            return true;
        }

        private static bool ValidateYear(string text, out string errorMessage)
        {
            errorMessage = "";

            if (int.TryParse(text, out int year) && year >= 1900 && year <= 2100)
                return true;

            errorMessage = "Invalid year format. Must be between 1900 and 2100.";
            return false;
        }

        private static bool ValidateMonthYear(string text, out string errorMessage)
        {
            errorMessage = "";

            if (DateTime.TryParseExact(text, "MM/yyyy", null, DateTimeStyles.None, out _))
                return true;

            errorMessage = "Invalid month/year format. Use MM/YYYY.";
            return false;
        }

        #endregion

        #endregion
    }
}