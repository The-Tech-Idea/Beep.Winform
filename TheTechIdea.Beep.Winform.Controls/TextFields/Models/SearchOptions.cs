using System;
using System.Text.RegularExpressions;

namespace TheTechIdea.Beep.Winform.Controls.TextFields.Models
{
    /// <summary>
    /// Configuration options for text search operations
    /// </summary>
    public class SearchOptions
    {
        /// <summary>
        /// The search pattern or text to find
        /// </summary>
        public string SearchText { get; set; } = string.Empty;

        /// <summary>
        /// The replacement text (for replace operations)
        /// </summary>
        public string ReplaceText { get; set; } = string.Empty;

        /// <summary>
        /// Enable case-sensitive search
        /// </summary>
        public bool CaseSensitive { get; set; } = false;

        /// <summary>
        /// Match whole words only
        /// </summary>
        public bool WholeWord { get; set; } = false;

        /// <summary>
        /// Treat search text as regular expression
        /// </summary>
        public bool UseRegex { get; set; } = false;

        /// <summary>
        /// Search direction
        /// </summary>
        public SearchDirection Direction { get; set; } = SearchDirection.Forward;

        /// <summary>
        /// Wrap around when reaching end/beginning of text
        /// </summary>
        public bool WrapAround { get; set; } = true;

        /// <summary>
        /// Highlight all matches in the text
        /// </summary>
        public bool HighlightAllMatches { get; set; } = true;

        /// <summary>
        /// Enable incremental search (search as you type)
        /// </summary>
        public bool IncrementalSearch { get; set; } = true;

        /// <summary>
        /// Search within selection only
        /// </summary>
        public bool SearchInSelection { get; set; } = false;

        /// <summary>
        /// Gets the RegexOptions based on current settings
        /// </summary>
        public RegexOptions GetRegexOptions()
        {
            var options = RegexOptions.None;
            
            if (!CaseSensitive)
                options |= RegexOptions.IgnoreCase;
            
            return options;
        }

        /// <summary>
        /// Gets the StringComparison based on current settings
        /// </summary>
        public StringComparison GetStringComparison()
        {
            return CaseSensitive 
                ? StringComparison.Ordinal 
                : StringComparison.OrdinalIgnoreCase;
        }

        /// <summary>
        /// Creates a copy of this options object
        /// </summary>
        public SearchOptions Clone()
        {
            return new SearchOptions
            {
                SearchText = SearchText,
                ReplaceText = ReplaceText,
                CaseSensitive = CaseSensitive,
                WholeWord = WholeWord,
                UseRegex = UseRegex,
                Direction = Direction,
                WrapAround = WrapAround,
                HighlightAllMatches = HighlightAllMatches,
                IncrementalSearch = IncrementalSearch,
                SearchInSelection = SearchInSelection
            };
        }
    }

    /// <summary>
    /// Search direction enumeration
    /// </summary>
    public enum SearchDirection
    {
        /// <summary>Search forward from current position</summary>
        Forward,
        /// <summary>Search backward from current position</summary>
        Backward
    }
}

