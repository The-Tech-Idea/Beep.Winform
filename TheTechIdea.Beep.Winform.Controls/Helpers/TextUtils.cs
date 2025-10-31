using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class TextUtils
    {
       #region "Caching Infrastructure"

        // Thread-safe cache for text measurements
        private static readonly ConcurrentDictionary<TextMeasurementKey, SizeF> _measurementCache = new();
        private static readonly int MaxCacheSize = 1000;
        private static int _cacheHits = 0;
        private static int _cacheMisses = 0;

        // String pooling for common strings to reduce memory allocations
        private static readonly ConcurrentDictionary<string, string> _stringPool = new();
        
        // Cache key structure for text measurements
        private readonly struct TextMeasurementKey : IEquatable<TextMeasurementKey>
        {
            public readonly string Text;
            public readonly string FontFamily;
            public readonly float FontSize;
            public readonly FontStyle FontStyle;
            public readonly int MaxWidth;

            public TextMeasurementKey(string text, Font font, int maxWidth)
            {
                Text = text;
                FontFamily = font.FontFamily.Name;
                FontSize = font.Size;
                FontStyle = font.Style;
                MaxWidth = maxWidth;
            }

            public bool Equals(TextMeasurementKey other)
            {
                return Text == other.Text &&
                       FontFamily == other.FontFamily &&
                       Math.Abs(FontSize - other.FontSize) < 0.01f &&
                       FontStyle == other.FontStyle &&
                       MaxWidth == other.MaxWidth;
            }

            public override bool Equals(object obj) => obj is TextMeasurementKey key && Equals(key);

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + (Text?.GetHashCode() ?? 0);
                    hash = hash * 31 + (FontFamily?.GetHashCode() ?? 0);
                    hash = hash * 31 + FontSize.GetHashCode();
                    hash = hash * 31 + FontStyle.GetHashCode();
                    hash = hash * 31 + MaxWidth.GetHashCode();
                    return hash;
                }
            }
        }

        /// <summary>
        /// Clears the text measurement cache. Call this when memory pressure is high or theme changes.
        /// </summary>
        public static void ClearCache()
        {
            _measurementCache.Clear();
            _stringPool.Clear();
            _cacheHits = 0;
            _cacheMisses = 0;
        }

        /// <summary>
        /// Gets cache statistics for monitoring performance
        /// </summary>
        public static (int hits, int misses, double hitRate, int cacheSize) GetCacheStats()
        {
            int total = _cacheHits + _cacheMisses;
            double hitRate = total > 0 ? (double)_cacheHits / total : 0;
            return (_cacheHits, _cacheMisses, hitRate, _measurementCache.Count);
        }

        /// <summary>
        /// Returns a pooled string instance to reduce memory allocations
        /// </summary>
        private static string GetPooledString(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length > 100) // Don't pool very long strings
                return input;
            
            return _stringPool.GetOrAdd(input, input);
        }

        #endregion

        #region "String Manipulation Utilities"
    
        public static string TruncateWithEllipsis(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input) || maxLength <= 0)
                return string.Empty;
            if (input.Length <= maxLength)
                return input;
            if (maxLength <= 3)
                return new string('.', maxLength);
            return string.Concat(input.AsSpan(0, maxLength - 3), "...");
        }

        public static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            if (input.Length == 1)
                return input.ToUpperInvariant();
            
            Span<char> chars = stackalloc char[input.Length];
            input.AsSpan().CopyTo(chars);
            chars[0] = char.ToUpperInvariant(chars[0]);
            return new string(chars);
        }

        public static string ToTitleCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        public static string RepeatString(string input, int count)
        {
            if (string.IsNullOrEmpty(input) || count <= 0)
                return string.Empty;
            if (count == 1)
                return input;
            
            return string.Create(input.Length * count, (input, count), (span, state) =>
            {
                var (str, cnt) = state;
                for (int i = 0; i < cnt; i++)
                {
                    str.AsSpan().CopyTo(span.Slice(i * str.Length));
                }
            });
        }

        public static bool IsPalindrome(string input)
        {
            if (string.IsNullOrEmpty(input))
                return true;
            
            ReadOnlySpan<char> span = input.AsSpan();
            int left = 0;
            int right = span.Length - 1;
            
            while (left < right)
            {
                if (span[left] != span[right])
                    return false;
                left++;
                right--;
            }
            return true;
        }

        public static string ReverseString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            
            return string.Create(input.Length, input, (span, str) =>
            {
                str.AsSpan().CopyTo(span);
                span.Reverse();
            });
        }

        public static int CountVowels(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;
            
            ReadOnlySpan<char> vowels = stackalloc char[] { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U' };
            int count = 0;
            
            foreach (char c in input.AsSpan())
            {
                if (vowels.Contains(c))
                    count++;
            }
            return count;
        }

        public static int CountConsonants(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;
            
            ReadOnlySpan<char> vowels = stackalloc char[] { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U' };
            int count = 0;
            
            foreach (char c in input.AsSpan())
            {
                if (char.IsLetter(c) && !vowels.Contains(c))
                    count++;
            }
            return count;
        }

        public static string RemoveWhitespace(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            
            return string.Create(input.Length, input, (span, str) =>
            {
                int writeIndex = 0;
                foreach (char c in str.AsSpan())
                {
                    if (!char.IsWhiteSpace(c))
                        span[writeIndex++] = c;
                }
                span = span.Slice(0, writeIndex);
            });
        }

        public static string NormalizeWhitespace(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            
            var parts = input.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", parts);
        }

        public static string PadLeft(string input, int totalWidth, char paddingChar = ' ')
        {
            input ??= string.Empty;
            if (totalWidth <= input.Length)
                return input;
            
            return string.Create(totalWidth, (input, paddingChar), (span, state) =>
            {
                var (str, pad) = state;
                int padCount = span.Length - str.Length;
                span.Slice(0, padCount).Fill(pad);
                str.AsSpan().CopyTo(span.Slice(padCount));
            });
        }

        public static string PadRight(string input, int totalWidth, char paddingChar = ' ')
        {
            input ??= string.Empty;
            if (totalWidth <= input.Length)
                return input;
            
            return string.Create(totalWidth, (input, paddingChar), (span, state) =>
            {
                var (str, pad) = state;
                str.AsSpan().CopyTo(span);
                span.Slice(str.Length).Fill(pad);
            });
        }

        public static string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            
            var sb = new StringBuilder(input.Length + 10);
            bool lastWasUpper = false;
            
            foreach (char c in input)
            {
                if (char.IsUpper(c))
                {
                    if (sb.Length > 0 && !lastWasUpper)
                        sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                    lastWasUpper = true;
                }
                else
                {
                    sb.Append(c);
                    lastWasUpper = false;
                }
            }
            return sb.ToString();
        }

        public static string ToKebabCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            
            var sb = new StringBuilder(input.Length + 10);
            bool lastWasUpper = false;
            
            foreach (char c in input)
            {
                if (char.IsUpper(c))
                {
                    if (sb.Length > 0 && !lastWasUpper)
                        sb.Append('-');
                    sb.Append(char.ToLowerInvariant(c));
                    lastWasUpper = true;
                }
                else
                {
                    sb.Append(c);
                    lastWasUpper = false;
                }
            }
            return sb.ToString();
        }

        public static string ToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            
            var words = input.Split(new char[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder(input.Length);
            
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    if (i == 0)
                    {
                        sb.Append(char.ToLowerInvariant(words[i][0]));
                        if (words[i].Length > 1)
                            sb.Append(words[i].Substring(1).ToLowerInvariant());
                    }
                    else
                    {
                        sb.Append(char.ToUpperInvariant(words[i][0]));
                        if (words[i].Length > 1)
                            sb.Append(words[i].Substring(1).ToLowerInvariant());
                    }
                }
            }
            return sb.ToString();
        }

        public static string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            
            var words = input.Split(new char[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder(input.Length);
            
            foreach (var word in words)
            {
                if (word.Length > 0)
                {
                    sb.Append(char.ToUpperInvariant(word[0]));
                    if (word.Length > 1)
                        sb.Append(word.Substring(1).ToLowerInvariant());
                }
            }
            return sb.ToString();
        }

        public static string RemovePunctuation(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            
            return string.Create(input.Length, input, (span, str) =>
            {
                int writeIndex = 0;
                foreach (char c in str.AsSpan())
                {
                    if (!char.IsPunctuation(c))
                        span[writeIndex++] = c;
                }
                span = span.Slice(0, writeIndex);
            });
        }

        public static string EscapeForCsv(string input)
        {
            if (input == null)
                return string.Empty;
            
            // Check if input contains any CSV special characters
            bool needsEscaping = false;
            foreach (char c in input)
            {
                if (c == ',' || c == '"' || c == '\n' || c == '\r')
                {
                    needsEscaping = true;
                    break;
                }
            }
            
            if (needsEscaping)
            {
                return $"\"{input.Replace("\"", "\"\"")}\"";
            }
            return input;
        }

        public static string UnescapeFromCsv(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            
            var span = input.AsSpan().Trim();
            if (span.Length >= 2 && span[0] == '"' && span[^1] == '"')
            {
                return span.Slice(1, span.Length - 2).ToString().Replace("\"\"", "\"");
            }
            return input;
        }

        #endregion

        #region "String Validation Utilities"
        
        public static bool IsValidEmail(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(input);
                return addr.Address == input;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidUrl(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            return Uri.TryCreate(input, UriKind.Absolute, out Uri uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static bool IsNumeric(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            return double.TryParse(input, out _);
        }

        public static bool IsAlphabetic(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            
            foreach (char c in input.AsSpan())
            {
                if (!char.IsLetter(c))
                    return false;
            }
            return true;
        }

        public static bool IsAlphanumeric(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            
            foreach (char c in input.AsSpan())
            {
                if (!char.IsLetterOrDigit(c))
                    return false;
            }
            return true;
        }

        #endregion

        #region "Text Rendering Utilities and Measurement using TextRenderer"
        public static int GetFontHeightSafe(Font font, Control context)
        {
            try
            {
                if (font == null)
                    return context?.Font?.Height ?? SystemFonts.DefaultFont.Height;
                // Use TextRenderer which is resilient and device-aware
                var sz = TextRenderer.MeasureText("Ag", font, new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
                return Math.Max(1, sz.Height);
            }
            catch
            {
                try { return context?.Font?.Height ?? SystemFonts.DefaultFont.Height; } catch { return 12; }
            }
        }

        /// <summary>
        /// Measures text with caching for improved performance
        /// </summary>
        public static SizeF MeasureText(string text, Font font, int maxWidth = int.MaxValue)
        {
            if (string.IsNullOrEmpty(text) || font == null)
                return SizeF.Empty;

            var key = new TextMeasurementKey(text, font, maxWidth);
            
            if (_measurementCache.TryGetValue(key, out var cachedSize))
            {
                System.Threading.Interlocked.Increment(ref _cacheHits);
                return cachedSize;
            }

            System.Threading.Interlocked.Increment(ref _cacheMisses);
            
            var proposedSize = new Size(maxWidth, int.MaxValue);
            var size = TextRenderer.MeasureText(text, font, proposedSize, TextFormatFlags.WordBreak);

            // Evict oldest entries if cache is too large
            if (_measurementCache.Count >= MaxCacheSize)
            {
                var keysToRemove = _measurementCache.Keys.Take(MaxCacheSize / 4).ToList();
                foreach (var k in keysToRemove)
                {
                    _measurementCache.TryRemove(k, out _);
                }
            }

            _measurementCache.TryAdd(key, size);
            return size;
        }

        /// <summary>
        /// Measures text with Graphics context and caching
        /// </summary>
        public static SizeF MeasureText(Graphics g, string text, Font font, int maxWidth = int.MaxValue)
        {
            if (string.IsNullOrEmpty(text) || font == null || g == null)
                return SizeF.Empty;

            var key = new TextMeasurementKey(text, font, maxWidth);
            
            if (_measurementCache.TryGetValue(key, out var cachedSize))
            {
                System.Threading.Interlocked.Increment(ref _cacheHits);
                return cachedSize;
            }

            System.Threading.Interlocked.Increment(ref _cacheMisses);
            
            var proposedSize = new Size(maxWidth, int.MaxValue);
            var size = TextRenderer.MeasureText(g, text, font, proposedSize, TextFormatFlags.WordBreak);

            // Evict oldest entries if cache is too large
            if (_measurementCache.Count >= MaxCacheSize)
            {
                var keysToRemove = _measurementCache.Keys.Take(MaxCacheSize / 4).ToList();
                foreach (var k in keysToRemove)
                {
                    _measurementCache.TryRemove(k, out _);
                }
            }

            _measurementCache.TryAdd(key, size);
            return size;
        }

        /// <summary>
        /// Draws text using TextRenderer (hardware accelerated)
        /// </summary>
        public static void DrawText(Graphics g, string text, Font font, Rectangle layoutRect, Color foreColor, TextFormatFlags flags)
        {
            if (g == null || string.IsNullOrEmpty(text) || font == null)
                return;
            
            TextRenderer.DrawText(g, text, font, layoutRect, foreColor, flags);
        }
        public static void DrawText(Graphics g, string text, Font font, Point location, Color foreColor, TextFormatFlags flags)
        {
            if (g == null || string.IsNullOrEmpty(text) || font == null)
                return;
            
            TextRenderer.DrawText(g, text, font, location, foreColor, flags);
        }
        public static void DrawText(Graphics g, string text, Font font, Rectangle layoutRect, Color foreColor)
        {
            DrawText(g, text, font, layoutRect, foreColor, TextFormatFlags.WordBreak);
        }
        public static void DrawText(Graphics g, string text, Font font, Point location, Color foreColor)
        {
            DrawText(g, text, font, location, foreColor, TextFormatFlags.WordBreak);
        }
        public static void DrawText(Graphics g, string text, Font font, Rectangle layoutRect)
        {
            DrawText(g, text, font, layoutRect, SystemColors.ControlText, TextFormatFlags.WordBreak);
        }
        public static void DrawText(Graphics g, string text, Font font, Point location)
        {
            DrawText(g, text, font, location, SystemColors.ControlText, TextFormatFlags.WordBreak);
        }

        /// <summary>
        /// Batch measure multiple texts - optimized for bulk operations
        /// </summary>
        public static Dictionary<string, SizeF> MeasureTextBatch(IEnumerable<string> texts, Font font, int maxWidth = int.MaxValue)
        {
            if (texts == null || font == null)
                return new Dictionary<string, SizeF>();

            var results = new Dictionary<string, SizeF>();
            var proposedSize = new Size(maxWidth, int.MaxValue);

            foreach (var text in texts)
            {
                if (string.IsNullOrEmpty(text))
                    continue;

                var key = new TextMeasurementKey(text, font, maxWidth);
                
                if (_measurementCache.TryGetValue(key, out var cachedSize))
                {
                    System.Threading.Interlocked.Increment(ref _cacheHits);
                    results[text] = cachedSize;
                }
                else
                {
                    System.Threading.Interlocked.Increment(ref _cacheMisses);
                    var size = TextRenderer.MeasureText(text, font, proposedSize, TextFormatFlags.WordBreak);
                    _measurementCache.TryAdd(key, size);
                    results[text] = size;
                }
            }

            return results;
        }

        /// <summary>
        /// Pre-warms the cache with common text measurements
        /// Call this during application startup or theme changes
        /// </summary>
        public static void PreWarmCache(Font font, string[] commonTexts, int[] commonWidths)
        {
            if (font == null || commonTexts == null || commonTexts.Length == 0)
                return;

            commonWidths ??= new[] { 100, 200, 300, 500, 1000 };

            foreach (var text in commonTexts)
            {
                foreach (var width in commonWidths)
                {
                    _ = MeasureText(text, font, width);
                }
            }
        }

        #endregion
    }
}
