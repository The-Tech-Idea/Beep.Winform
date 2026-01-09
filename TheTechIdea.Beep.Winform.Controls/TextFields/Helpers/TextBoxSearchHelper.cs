using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using TheTechIdea.Beep.Winform.Controls.TextFields.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.TextFields.Helpers
{
    /// <summary>
    /// Helper class for search and replace operations in BeepTextBox
    /// </summary>
    public class TextBoxSearchHelper : IDisposable
    {
        #region Fields

        private readonly BeepTextBox _textBox;
        private SearchOptions _currentOptions;
        private SearchResult _currentResult;
        private List<string> _searchHistory;
        private const int MaxSearchHistory = 20;
        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Current search options
        /// </summary>
        public SearchOptions Options
        {
            get => _currentOptions;
            set => _currentOptions = value ?? new SearchOptions();
        }

        /// <summary>
        /// Current search result
        /// </summary>
        public SearchResult Result => _currentResult;

        /// <summary>
        /// Search history (most recent first)
        /// </summary>
        public IReadOnlyList<string> SearchHistory => _searchHistory.AsReadOnly();

        /// <summary>
        /// Whether a search is currently active
        /// </summary>
        public bool IsSearchActive => _currentResult?.HasMatches == true;

        /// <summary>
        /// Highlight color for matches
        /// </summary>
        public Color MatchHighlightColor { get; set; } = Color.FromArgb(255, 255, 0); // Yellow

        /// <summary>
        /// Highlight color for current match
        /// </summary>
        public Color CurrentMatchHighlightColor { get; set; } = Color.FromArgb(255, 165, 0); // Orange

        #endregion

        #region Events

        /// <summary>
        /// Fired when a search is started
        /// </summary>
        public event EventHandler<SearchEventArgs> SearchStarted;

        /// <summary>
        /// Fired when a search is completed
        /// </summary>
        public event EventHandler<SearchEventArgs> SearchCompleted;

        /// <summary>
        /// Fired when navigating to a match
        /// </summary>
        public event EventHandler<SearchEventArgs> MatchNavigated;

        /// <summary>
        /// Fired when text is replaced
        /// </summary>
        public event EventHandler<ReplaceEventArgs> TextReplaced;

        #endregion

        #region Constructor

        public TextBoxSearchHelper(BeepTextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
            _currentOptions = new SearchOptions();
            _currentResult = new SearchResult();
            _searchHistory = new List<string>();
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// Perform a search with the current options
        /// </summary>
        public SearchResult Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                ClearSearch();
                return _currentResult;
            }

            _currentOptions.SearchText = searchText;
            return PerformSearch();
        }

        /// <summary>
        /// Perform a search with custom options
        /// </summary>
        public SearchResult Search(SearchOptions options)
        {
            if (options == null || string.IsNullOrEmpty(options.SearchText))
            {
                ClearSearch();
                return _currentResult;
            }

            _currentOptions = options.Clone();
            return PerformSearch();
        }

        /// <summary>
        /// Internal search implementation
        /// </summary>
        private SearchResult PerformSearch()
        {
            var stopwatch = Stopwatch.StartNew();
            
            _currentResult = new SearchResult
            {
                SearchText = _currentOptions.SearchText
            };

            SearchStarted?.Invoke(this, new SearchEventArgs(_currentOptions, _currentResult));

            string text = _textBox.Text ?? string.Empty;
            
            if (string.IsNullOrEmpty(text))
            {
                stopwatch.Stop();
                _currentResult.SearchDuration = stopwatch.Elapsed;
                SearchCompleted?.Invoke(this, new SearchEventArgs(_currentOptions, _currentResult));
                return _currentResult;
            }

            try
            {
                List<SearchMatch> matches;

                if (_currentOptions.UseRegex)
                {
                    matches = FindRegexMatches(text);
                }
                else
                {
                    matches = FindTextMatches(text);
                }

                _currentResult.Matches = matches;

                // Set current match based on caret position
                if (matches.Count > 0)
                {
                    int caretPos = _textBox.SelectionStart;
                    _currentResult.CurrentMatchIndex = _currentResult.FindMatchNearPosition(
                        caretPos, 
                        _currentOptions.Direction
                    );

                    // Add to search history
                    AddToSearchHistory(_currentOptions.SearchText);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
#endif
            }

            stopwatch.Stop();
            _currentResult.SearchDuration = stopwatch.Elapsed;

            SearchCompleted?.Invoke(this, new SearchEventArgs(_currentOptions, _currentResult));

            return _currentResult;
        }

        /// <summary>
        /// Find matches using plain text search
        /// </summary>
        private List<SearchMatch> FindTextMatches(string text)
        {
            var matches = new List<SearchMatch>();
            string searchText = _currentOptions.SearchText;
            var comparison = _currentOptions.GetStringComparison();

            int startIndex = 0;
            int lineNumber = 0;
            int lineStartIndex = 0;

            while (startIndex < text.Length)
            {
                int index;

                if (_currentOptions.CaseSensitive)
                {
                    index = text.IndexOf(searchText, startIndex, StringComparison.Ordinal);
                }
                else
                {
                    index = text.IndexOf(searchText, startIndex, StringComparison.OrdinalIgnoreCase);
                }

                if (index < 0) break;

                // Check whole word if required
                if (_currentOptions.WholeWord && !IsWholeWord(text, index, searchText.Length))
                {
                    startIndex = index + 1;
                    continue;
                }

                // Calculate line number and column
                while (lineStartIndex < index)
                {
                    int nextNewLine = text.IndexOf('\n', lineStartIndex);
                    if (nextNewLine < 0 || nextNewLine >= index) break;
                    lineNumber++;
                    lineStartIndex = nextNewLine + 1;
                }

                var match = new SearchMatch
                {
                    StartIndex = index,
                    Length = searchText.Length,
                    MatchedText = text.Substring(index, searchText.Length),
                    LineNumber = lineNumber,
                    ColumnNumber = index - lineStartIndex
                };

                matches.Add(match);
                startIndex = index + 1; // Allow overlapping matches: index + searchText.Length for non-overlapping
            }

            return matches;
        }

        /// <summary>
        /// Find matches using regular expressions
        /// </summary>
        private List<SearchMatch> FindRegexMatches(string text)
        {
            var matches = new List<SearchMatch>();
            
            try
            {
                string pattern = _currentOptions.SearchText;
                
                if (_currentOptions.WholeWord)
                {
                    pattern = $@"\b{Regex.Escape(pattern)}\b";
                }

                var regex = new Regex(pattern, _currentOptions.GetRegexOptions());
                var regexMatches = regex.Matches(text);

                int lineNumber = 0;
                int lineStartIndex = 0;

                foreach (Match m in regexMatches)
                {
                    // Calculate line number
                    while (lineStartIndex < m.Index)
                    {
                        int nextNewLine = text.IndexOf('\n', lineStartIndex);
                        if (nextNewLine < 0 || nextNewLine >= m.Index) break;
                        lineNumber++;
                        lineStartIndex = nextNewLine + 1;
                    }

                    var match = new SearchMatch
                    {
                        StartIndex = m.Index,
                        Length = m.Length,
                        MatchedText = m.Value,
                        LineNumber = lineNumber,
                        ColumnNumber = m.Index - lineStartIndex
                    };

                    matches.Add(match);
                }
            }
            catch (ArgumentException ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Invalid regex: {ex.Message}");
#endif
            }

            return matches;
        }

        /// <summary>
        /// Check if match is a whole word
        /// </summary>
        private bool IsWholeWord(string text, int index, int length)
        {
            // Check character before
            if (index > 0)
            {
                char before = text[index - 1];
                if (char.IsLetterOrDigit(before) || before == '_')
                    return false;
            }

            // Check character after
            int afterIndex = index + length;
            if (afterIndex < text.Length)
            {
                char after = text[afterIndex];
                if (char.IsLetterOrDigit(after) || after == '_')
                    return false;
            }

            return true;
        }

        #endregion

        #region Navigation Methods

        /// <summary>
        /// Find next match
        /// </summary>
        public SearchMatch FindNext()
        {
            if (!_currentResult.HasMatches)
            {
                if (!string.IsNullOrEmpty(_currentOptions.SearchText))
                {
                    PerformSearch();
                }
                if (!_currentResult.HasMatches) return null;
            }

            if (_currentOptions.WrapAround)
            {
                _currentResult.MoveNextWrap();
            }
            else
            {
                if (!_currentResult.MoveNext())
                    return null;
            }

            NavigateToCurrentMatch();
            return _currentResult.CurrentMatch;
        }

        /// <summary>
        /// Find previous match
        /// </summary>
        public SearchMatch FindPrevious()
        {
            if (!_currentResult.HasMatches)
            {
                if (!string.IsNullOrEmpty(_currentOptions.SearchText))
                {
                    _currentOptions.Direction = SearchDirection.Backward;
                    PerformSearch();
                }
                if (!_currentResult.HasMatches) return null;
            }

            if (_currentOptions.WrapAround)
            {
                _currentResult.MovePreviousWrap();
            }
            else
            {
                if (!_currentResult.MovePrevious())
                    return null;
            }

            NavigateToCurrentMatch();
            return _currentResult.CurrentMatch;
        }

        /// <summary>
        /// Navigate to a specific match index
        /// </summary>
        public void GoToMatch(int index)
        {
            if (!_currentResult.HasMatches) return;
            
            if (index >= 0 && index < _currentResult.TotalMatches)
            {
                _currentResult.CurrentMatchIndex = index;
                NavigateToCurrentMatch();
            }
        }

        /// <summary>
        /// Navigate to the current match in the textbox
        /// </summary>
        private void NavigateToCurrentMatch()
        {
            var match = _currentResult.CurrentMatch;
            if (match == null) return;

            _textBox.SelectionStart = match.StartIndex;
            _textBox.SelectionLength = match.Length;
            _textBox.ScrollToCaret();
            _textBox.Invalidate();

            MatchNavigated?.Invoke(this, new SearchEventArgs(_currentOptions, _currentResult));
        }

        #endregion

        #region Replace Methods

        /// <summary>
        /// Replace the current match
        /// </summary>
        public bool ReplaceCurrent(string replaceText)
        {
            var match = _currentResult.CurrentMatch;
            if (match == null) return false;

            return ReplaceMatch(match, replaceText);
        }

        /// <summary>
        /// Replace a specific match
        /// </summary>
        public bool ReplaceMatch(SearchMatch match, string replaceText)
        {
            if (match == null || _textBox.ReadOnly) return false;

            string text = _textBox.Text ?? string.Empty;
            
            if (match.StartIndex < 0 || match.StartIndex + match.Length > text.Length)
                return false;

            string oldText = match.MatchedText;
            
            // Perform replacement
            string newText = text.Substring(0, match.StartIndex) + 
                             replaceText + 
                             text.Substring(match.StartIndex + match.Length);

            _textBox.Text = newText;
            _textBox.SelectionStart = match.StartIndex + replaceText.Length;
            _textBox.SelectionLength = 0;

            TextReplaced?.Invoke(this, new ReplaceEventArgs(match, oldText, replaceText));

            // Re-search to update matches
            PerformSearch();

            return true;
        }

        /// <summary>
        /// Replace all matches
        /// </summary>
        public int ReplaceAll(string replaceText)
        {
            if (!_currentResult.HasMatches || _textBox.ReadOnly) return 0;

            string text = _textBox.Text ?? string.Empty;
            int replacementCount = 0;
            int offset = 0;

            // Replace from end to start to maintain correct indices
            var sortedMatches = new List<SearchMatch>(_currentResult.Matches);
            sortedMatches.Sort((a, b) => b.StartIndex.CompareTo(a.StartIndex));

            foreach (var match in sortedMatches)
            {
                int adjustedIndex = match.StartIndex;
                
                if (adjustedIndex >= 0 && adjustedIndex + match.Length <= text.Length)
                {
                    text = text.Substring(0, adjustedIndex) + 
                           replaceText + 
                           text.Substring(adjustedIndex + match.Length);
                    
                    replacementCount++;
                }
            }

            _textBox.Text = text;

            TextReplaced?.Invoke(this, new ReplaceEventArgs(null, _currentOptions.SearchText, replaceText)
            {
                ReplacementCount = replacementCount
            });

            ClearSearch();

            return replacementCount;
        }

        #endregion

        #region History Methods

        /// <summary>
        /// Add search text to history
        /// </summary>
        private void AddToSearchHistory(string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) return;

            // Remove if already exists
            _searchHistory.Remove(searchText);

            // Add to beginning
            _searchHistory.Insert(0, searchText);

            // Trim to max size
            while (_searchHistory.Count > MaxSearchHistory)
            {
                _searchHistory.RemoveAt(_searchHistory.Count - 1);
            }
        }

        /// <summary>
        /// Clear search history
        /// </summary>
        public void ClearHistory()
        {
            _searchHistory.Clear();
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Clear current search and results
        /// </summary>
        public void ClearSearch()
        {
            _currentResult.Clear();
            _textBox.Invalidate();
        }

        /// <summary>
        /// Check if a position is within any match
        /// </summary>
        public SearchMatch GetMatchAtPosition(int position)
        {
            if (!_currentResult.HasMatches) return null;

            foreach (var match in _currentResult.Matches)
            {
                if (match.ContainsPosition(position))
                    return match;
            }

            return null;
        }

        /// <summary>
        /// Get highlight rectangles for drawing matches
        /// </summary>
        public List<(Rectangle Rect, bool IsCurrent)> GetMatchHighlightRects(
            Graphics g, 
            Rectangle textRect, 
            Font font,
            int scrollOffset = 0)
        {
            var highlights = new List<(Rectangle, bool)>();
            
            if (!_currentResult.HasMatches || !_currentOptions.HighlightAllMatches)
                return highlights;

            string text = _textBox.Text ?? string.Empty;
            
            for (int i = 0; i < _currentResult.Matches.Count; i++)
            {
                var match = _currentResult.Matches[i];
                bool isCurrent = i == _currentResult.CurrentMatchIndex;

                try
                {
                    // Calculate position of match
                    string beforeText = text.Substring(0, match.StartIndex);
                    var beforeSize = TextRenderer.MeasureText(g, beforeText, font, 
                        new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                    
                    var matchSize = TextRenderer.MeasureText(g, match.MatchedText, font,
                        new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);

                    var rect = new Rectangle(
                        textRect.X + beforeSize.Width - scrollOffset,
                        textRect.Y,
                        matchSize.Width,
                        matchSize.Height
                    );

                    // Only add if visible
                    if (rect.Right > textRect.Left && rect.Left < textRect.Right)
                    {
                        highlights.Add((rect, isCurrent));
                    }
                }
                catch
                {
                    // Skip if measurement fails
                }
            }

            return highlights;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_disposed) return;
            
            _disposed = true;
            _searchHistory.Clear();
            _currentResult?.Clear();
        }

        #endregion
    }
}

