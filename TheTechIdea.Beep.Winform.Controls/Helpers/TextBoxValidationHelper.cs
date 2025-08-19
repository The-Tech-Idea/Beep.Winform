using System;
using System.Globalization;
using System.Text.RegularExpressions;
using TheTechIdea.Beep.Vis.Modules; // Use the existing TextBoxMaskFormat

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    /// <summary>
    /// Handles text validation, input masking, and character filtering
    /// </summary>
    public class TextBoxValidationHelper
    {
        #region "Fields"
        
        private readonly IBeepTextBox _textBox;
        private TextBoxMaskFormat _maskFormat = TextBoxMaskFormat.None;
        private string _customMask = string.Empty;
        private string _dateFormat = "MM/dd/yyyy";
        private string _timeFormat = "HH:mm:ss";
        private string _dateTimeFormat = "MM/dd/yyyy HH:mm:ss";
        private bool _onlyDigits = false;
        private bool _onlyCharacters = false;
        private bool _isApplyingMask = false;
        
        #endregion
        
        #region "Properties"
        
        public TextBoxMaskFormat MaskFormat
        {
            get => _maskFormat;
            set
            {
                _maskFormat = value;
                _isApplyingMask = (value != TextBoxMaskFormat.None);
            }
        }
        
        public string CustomMask
        {
            get => _customMask;
            set
            {
                _customMask = value;
                if (!string.IsNullOrEmpty(value))
                {
                    _isApplyingMask = true;
                    _maskFormat = TextBoxMaskFormat.Custom;
                }
                else
                {
                    _isApplyingMask = false;
                    _maskFormat = TextBoxMaskFormat.None;
                }
            }
        }
        
        public string DateFormat
        {
            get => _dateFormat;
            set => _dateFormat = value;
        }
        
        public string TimeFormat
        {
            get => _timeFormat;
            set => _timeFormat = value;
        }
        
        public string DateTimeFormat
        {
            get => _dateTimeFormat;
            set => _dateTimeFormat = value;
        }
        
        public bool OnlyDigits
        {
            get => _onlyDigits;
            set => _onlyDigits = value;
        }
        
        public bool OnlyCharacters
        {
            get => _onlyCharacters;
            set => _onlyCharacters = value;
        }
        
        public bool IsApplyingMask => _isApplyingMask;
        
        #endregion
        
        #region "Constructor"
        
        public TextBoxValidationHelper(IBeepTextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
        }
        
        #endregion
        
        #region "Input Validation"
        
        /// <summary>
        /// Validates input text based on current mask and validation settings
        /// </summary>
        public bool ValidateInput(string input)
        {
            if (string.IsNullOrEmpty(input)) return true;

            // If mask is applied, validate each character using the mask helper
            if (_maskFormat != TextBoxMaskFormat.None && _isApplyingMask)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    char c = input[i];
                    int position = _textBox.SelectionStart + i; // Calculate position where character will be inserted

                    if (!IsValidMaskCharacter(c, position))
                    {
                        return false;
                    }
                }
                return true;
            }

            // Fallback to original validation for non-masked input
            foreach (char c in input)
            {
                if (!ValidateCharacter(c))
                    return false;
            }
            return true;
        }
        
        /// <summary>
        /// Validates a single character based on current settings
        /// </summary>
        public bool ValidateCharacter(char c)
        {
            // Allow control characters
            if (char.IsControl(c))
            {
                // Allow newlines in multiline mode
                if (_textBox.Multiline && (c == '\r' || c == '\n'))
                {
                    return true;
                }
                // Allow other control characters like backspace, delete, etc.
                return c == '\b' || c == '\t' || c == '\r' || c == '\n';
            }
            
            if (!_isApplyingMask) return true; // If mask is not applied, allow all characters
            if (_maskFormat == TextBoxMaskFormat.None) return true; // If no mask format is set, allow all characters
            
            // Priority 1: Check mask format validation if mask is applied
            if (_maskFormat != TextBoxMaskFormat.None && _isApplyingMask)
            {
                return IsValidMaskCharacter(c, _textBox.SelectionStart);
            }

            // Priority 2: Check OnlyDigits/OnlyCharacters validation (legacy behavior)
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string groupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;

            if (_onlyDigits)
            {
                return char.IsDigit(c) ||
                       c.ToString() == decimalSeparator ||
                       c.ToString() == groupSeparator;
            }
            else if (_onlyCharacters)
            {
                return char.IsLetter(c);
            }

            // Priority 3: No validation - allow all characters
            return true;
        }
        
        #endregion
        
        #region "Mask Validation"
        
        /// <summary>
        /// Validates a character against the current mask format
        /// </summary>
        private bool IsValidMaskCharacter(char c, int position)
        {
            switch (_maskFormat)
            {
                case TextBoxMaskFormat.None:
                    return true;
                    
                case TextBoxMaskFormat.Numeric:
                    return char.IsDigit(c) || c == '.' || c == '-' || c == '+';
                    
                case TextBoxMaskFormat.Decimal:
                    return char.IsDigit(c) || c == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
                    
                case TextBoxMaskFormat.Currency:
                    return char.IsDigit(c) || c == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0] ||
                           c == CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator[0];
                    
                case TextBoxMaskFormat.Percentage:
                    return char.IsDigit(c) || c == '.' || c == '%';
                    
                case TextBoxMaskFormat.PhoneNumber:
                    return char.IsDigit(c) || c == '(' || c == ')' || c == '-' || c == ' ' || c == '+';
                    
                case TextBoxMaskFormat.SocialSecurityNumber:
                    return char.IsDigit(c) || c == '-';
                    
                case TextBoxMaskFormat.ZipCode:
                    return char.IsDigit(c) || c == '-';
                    
                case TextBoxMaskFormat.Date:
                    return char.IsDigit(c) || c == '/' || c == '-' || c == '.';
                    
                case TextBoxMaskFormat.Time:
                    return char.IsDigit(c) || c == ':' || c == ' ' || char.ToUpper(c) == 'A' || char.ToUpper(c) == 'M' || char.ToUpper(c) == 'P';
                    
                case TextBoxMaskFormat.DateTime:
                    return char.IsDigit(c) || c == '/' || c == '-' || c == '.' || c == ':' || c == ' ' || 
                           char.ToUpper(c) == 'A' || char.ToUpper(c) == 'M' || char.ToUpper(c) == 'P';
                    
                case TextBoxMaskFormat.CreditCard:
                    return char.IsDigit(c) || c == '-' || c == ' ';
                    
                case TextBoxMaskFormat.IPAddress:
                    return char.IsDigit(c) || c == '.';
                    
                case TextBoxMaskFormat.Email:
                    return char.IsLetterOrDigit(c) || c == '@' || c == '.' || c == '-' || c == '_';
                    
                case TextBoxMaskFormat.Custom:
                    return ValidateCustomMask(c, position);
                    
                default:
                    return true;
            }
        }
        
        /// <summary>
        /// Validates character against custom mask pattern
        /// </summary>
        private bool ValidateCustomMask(char c, int position)
        {
            if (string.IsNullOrEmpty(_customMask) || position >= _customMask.Length)
                return true;
                
            char maskChar = _customMask[position];
            
            switch (maskChar)
            {
                case '0': // Required digit
                    return char.IsDigit(c);
                case '9': // Optional digit
                    return char.IsDigit(c) || c == ' ';
                case '#': // Optional digit or + or -
                    return char.IsDigit(c) || c == '+' || c == '-' || c == ' ';
                case 'L': // Required letter
                    return char.IsLetter(c);
                case '?': // Optional letter
                    return char.IsLetter(c) || c == ' ';
                case '&': // Required character
                    return !char.IsControl(c);
                case 'C': // Optional character
                    return !char.IsControl(c) || c == ' ';
                case 'A': // Required alphanumeric
                    return char.IsLetterOrDigit(c);
                case 'a': // Optional alphanumeric
                    return char.IsLetterOrDigit(c) || c == ' ';
                case '.': // Decimal separator placeholder
                    return c == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
                case ',': // Thousands separator placeholder
                    return c == CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator[0];
                case '$': // Currency symbol placeholder
                    return c == CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol[0];
                case '<': // Convert to lowercase
                    return char.IsLetter(c);
                case '>': // Convert to uppercase
                    return char.IsLetter(c);
                case '|': // Password character
                    return true;
                case '\\': // Literal next character
                    if (position + 1 < _customMask.Length)
                        return c == _customMask[position + 1];
                    return true;
                default: // Literal character
                    return c == maskChar;
            }
        }
        
        #endregion
        
        #region "Text Formatting"
        
        /// <summary>
        /// Applies mask formatting to the current text
        /// </summary>
        public string ApplyMaskFormat(string text)
        {
            if (!_isApplyingMask || _maskFormat == TextBoxMaskFormat.None)
                return text;
                
            switch (_maskFormat)
            {
                case TextBoxMaskFormat.Currency:
                    return ApplyCurrencyFormat(text);
                case TextBoxMaskFormat.Percentage:
                    return ApplyPercentageFormat(text);
                case TextBoxMaskFormat.Date:
                    return ApplyDateFormat(text);
                case TextBoxMaskFormat.Time:
                    return ApplyTimeFormat(text);
                case TextBoxMaskFormat.DateTime:
                    return ApplyDateTimeFormat(text);
                case TextBoxMaskFormat.PhoneNumber:
                    return ApplyPhoneNumberFormat(text);
                case TextBoxMaskFormat.SocialSecurityNumber:
                    return ApplySSNFormat(text);
                case TextBoxMaskFormat.ZipCode:
                    return ApplyZipCodeFormat(text);
                case TextBoxMaskFormat.CreditCard:
                    return ApplyCreditCardFormat(text);
                case TextBoxMaskFormat.IPAddress:
                    return ApplyIPAddressFormat(text);
                case TextBoxMaskFormat.Custom:
                    return ApplyCustomFormat(text);
                default:
                    return text;
            }
        }
        
        private string ApplyCurrencyFormat(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            
            // Remove non-numeric characters except decimal point
            string numericOnly = Regex.Replace(text, @"[^\d\.]", "");
            
            if (decimal.TryParse(numericOnly, out decimal value))
            {
                return value.ToString("C", CultureInfo.CurrentCulture);
            }
            
            return text;
        }
        
        private string ApplyPercentageFormat(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            
            string numericOnly = Regex.Replace(text, @"[^\d\.]", "");
            
            if (decimal.TryParse(numericOnly, out decimal value))
            {
                return (value / 100).ToString("P", CultureInfo.CurrentCulture);
            }
            
            return text;
        }
        
        private string ApplyDateFormat(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            
            if (DateTime.TryParse(text, out DateTime date))
            {
                return date.ToString(_dateFormat);
            }
            
            return text;
        }
        
        private string ApplyTimeFormat(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            
            if (DateTime.TryParse(text, out DateTime time))
            {
                return time.ToString(_timeFormat);
            }
            
            return text;
        }
        
        private string ApplyDateTimeFormat(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            
            if (DateTime.TryParse(text, out DateTime dateTime))
            {
                return dateTime.ToString(_dateTimeFormat);
            }
            
            return text;
        }
        
        private string ApplyPhoneNumberFormat(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            
            string digitsOnly = Regex.Replace(text, @"[^\d]", "");
            
            if (digitsOnly.Length == 10)
            {
                return $"({digitsOnly.Substring(0, 3)}) {digitsOnly.Substring(3, 3)}-{digitsOnly.Substring(6, 4)}";
            }
            else if (digitsOnly.Length == 11 && digitsOnly.StartsWith("1"))
            {
                return $"1 ({digitsOnly.Substring(1, 3)}) {digitsOnly.Substring(4, 3)}-{digitsOnly.Substring(7, 4)}";
            }
            
            return text;
        }
        
        private string ApplySSNFormat(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            
            string digitsOnly = Regex.Replace(text, @"[^\d]", "");
            
            if (digitsOnly.Length == 9)
            {
                return $"{digitsOnly.Substring(0, 3)}-{digitsOnly.Substring(3, 2)}-{digitsOnly.Substring(5, 4)}";
            }
            
            return text;
        }
        
        private string ApplyZipCodeFormat(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            
            string digitsOnly = Regex.Replace(text, @"[^\d]", "");
            
            if (digitsOnly.Length == 9)
            {
                return $"{digitsOnly.Substring(0, 5)}-{digitsOnly.Substring(5, 4)}";
            }
            
            return text;
        }
        
        private string ApplyCreditCardFormat(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            
            string digitsOnly = Regex.Replace(text, @"[^\d]", "");
            
            if (digitsOnly.Length >= 13)
            {
                // Format as groups of 4 digits
                string formatted = "";
                for (int i = 0; i < digitsOnly.Length; i += 4)
                {
                    if (i > 0) formatted += " ";
                    formatted += digitsOnly.Substring(i, Math.Min(4, digitsOnly.Length - i));
                }
                return formatted;
            }
            
            return text;
        }
        
        private string ApplyIPAddressFormat(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            
            var parts = text.Split('.');
            if (parts.Length == 4)
            {
                var validParts = new List<string>();
                foreach (var part in parts)
                {
                    if (int.TryParse(part, out int value) && value >= 0 && value <= 255)
                    {
                        validParts.Add(value.ToString());
                    }
                    else
                    {
                        validParts.Add(part); // Keep original if invalid
                    }
                }
                return string.Join(".", validParts);
            }
            
            return text;
        }
        
        private string ApplyCustomFormat(string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(_customMask))
                return text;
                
            // This is a simplified custom mask implementation
            // A full implementation would be more complex
            return text;
        }
        
        #endregion
        
        #region "Data Validation"
        
        /// <summary>
        /// Validates the complete text data according to the current mask format
        /// </summary>
        public bool ValidateData(out string message)
        {
            message = string.Empty;
            
            if (_maskFormat == TextBoxMaskFormat.None)
                return true;
                
            string text = _textBox.Text;
            
            switch (_maskFormat)
            {
                case TextBoxMaskFormat.Email:
                    return ValidateEmail(text, out message);
                case TextBoxMaskFormat.Date:
                    return ValidateDate(text, out message);
                case TextBoxMaskFormat.Time:
                    return ValidateTime(text, out message);
                case TextBoxMaskFormat.DateTime:
                    return ValidateDateTime(text, out message);
                case TextBoxMaskFormat.IPAddress:
                    return ValidateIPAddress(text, out message);
                case TextBoxMaskFormat.Currency:
                    return ValidateCurrency(text, out message);
                case TextBoxMaskFormat.PhoneNumber:
                    return ValidatePhone(text, out message);
                default:
                    return true;
            }
        }
        
        private bool ValidateEmail(string email, out string message)
        {
            message = string.Empty;
            
            if (string.IsNullOrEmpty(email))
                return true; // Allow empty
                
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
            {
                message = "Invalid email format";
                return false;
            }
            
            return true;
        }
        
        private bool ValidateDate(string date, out string message)
        {
            message = string.Empty;
            
            if (string.IsNullOrEmpty(date))
                return true;
                
            if (!DateTime.TryParseExact(date, _dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                message = $"Invalid date format. Expected: {_dateFormat}";
                return false;
            }
            
            return true;
        }
        
        private bool ValidateTime(string time, out string message)
        {
            message = string.Empty;
            
            if (string.IsNullOrEmpty(time))
                return true;
                
            if (!DateTime.TryParseExact(time, _timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                message = $"Invalid time format. Expected: {_timeFormat}";
                return false;
            }
            
            return true;
        }
        
        private bool ValidateDateTime(string dateTime, out string message)
        {
            message = string.Empty;
            
            if (string.IsNullOrEmpty(dateTime))
                return true;
                
            if (!DateTime.TryParseExact(dateTime, _dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                message = $"Invalid date/time format. Expected: {_dateTimeFormat}";
                return false;
            }
            
            return true;
        }
        
        private bool ValidateIPAddress(string ip, out string message)
        {
            message = string.Empty;
            
            if (string.IsNullOrEmpty(ip))
                return true;
                
            if (!System.Net.IPAddress.TryParse(ip, out _))
            {
                message = "Invalid IP address format";
                return false;
            }
            
            return true;
        }
        
        private bool ValidateCurrency(string currency, out string message)
        {
            message = string.Empty;
            
            if (string.IsNullOrEmpty(currency))
                return true;
                
            if (!decimal.TryParse(currency, NumberStyles.Currency, CultureInfo.CurrentCulture, out _))
            {
                message = "Invalid currency format";
                return false;
            }
            
            return true;
        }
        
        private bool ValidatePhone(string phone, out string message)
        {
            message = string.Empty;
            
            if (string.IsNullOrEmpty(phone))
                return true;
                
            string digitsOnly = Regex.Replace(phone, @"[^\d]", "");
            
            if (digitsOnly.Length != 10 && digitsOnly.Length != 11)
            {
                message = "Phone number must be 10 or 11 digits";
                return false;
            }
            
            return true;
        }
        
        #endregion
    }
}