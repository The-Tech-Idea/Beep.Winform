using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.TextFields.Helpers;
using TheTechIdea.Beep.Winform.Controls.TextFields.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Search and Find/Replace functionality for BeepTextBox
    /// </summary>
    public partial class BeepTextBox
    {
        #region Search Fields

        private TextBoxSearchHelper _searchHelper;
        private bool _searchHighlightEnabled = true;

        #endregion

        #region Search Properties

        /// <summary>
        /// Gets the search helper for advanced search operations
        /// </summary>
        [Browsable(false)]
        public TextBoxSearchHelper SearchHelper
        {
            get
            {
                if (_searchHelper == null)
                {
                    _searchHelper = new TextBoxSearchHelper(this);
                    _searchHelper.SearchCompleted += SearchHelper_SearchCompleted;
                    _searchHelper.MatchNavigated += SearchHelper_MatchNavigated;
                    _searchHelper.TextReplaced += SearchHelper_TextReplaced;
                }
                return _searchHelper;
            }
        }

        /// <summary>
        /// Enable search match highlighting
        /// </summary>
        [Browsable(true)]
        [Category("Search")]
        [DefaultValue(true)]
        [Description("Enable highlighting of search matches.")]
        public bool SearchHighlightEnabled
        {
            get => _searchHighlightEnabled;
            set
            {
                _searchHighlightEnabled = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Color for highlighting search matches
        /// </summary>
        [Browsable(true)]
        [Category("Search")]
        [Description("Background color for search match highlighting.")]
        public Color SearchMatchHighlightColor
        {
            get => SearchHelper.MatchHighlightColor;
            set
            {
                SearchHelper.MatchHighlightColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Color for highlighting current search match
        /// </summary>
        [Browsable(true)]
        [Category("Search")]
        [Description("Background color for current search match.")]
        public Color SearchCurrentMatchColor
        {
            get => SearchHelper.CurrentMatchHighlightColor;
            set
            {
                SearchHelper.CurrentMatchHighlightColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Whether a search is currently active
        /// </summary>
        [Browsable(false)]
        public bool IsSearchActive => SearchHelper.IsSearchActive;

        /// <summary>
        /// Current search result
        /// </summary>
        [Browsable(false)]
        public SearchResult SearchResult => SearchHelper.Result;

        /// <summary>
        /// Search history
        /// </summary>
        [Browsable(false)]
        public System.Collections.Generic.IReadOnlyList<string> SearchHistory => SearchHelper.SearchHistory;

        #endregion

        #region Search Events

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

        private void SearchHelper_SearchCompleted(object sender, SearchEventArgs e)
        {
            SearchCompleted?.Invoke(this, e);
            Invalidate();
        }

        private void SearchHelper_MatchNavigated(object sender, SearchEventArgs e)
        {
            MatchNavigated?.Invoke(this, e);
        }

        private void SearchHelper_TextReplaced(object sender, ReplaceEventArgs e)
        {
            TextReplaced?.Invoke(this, e);
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// Search for text in the textbox
        /// </summary>
        /// <param name="searchText">Text to search for</param>
        /// <returns>Search result with matches</returns>
        public SearchResult Search(string searchText)
        {
            return SearchHelper.Search(searchText);
        }

        /// <summary>
        /// Search with custom options
        /// </summary>
        /// <param name="options">Search options</param>
        /// <returns>Search result with matches</returns>
        public SearchResult Search(SearchOptions options)
        {
            return SearchHelper.Search(options);
        }

        /// <summary>
        /// Find and select the next match
        /// </summary>
        /// <returns>The match found, or null if none</returns>
        public SearchMatch FindNext()
        {
            return SearchHelper.FindNext();
        }

        /// <summary>
        /// Find and select the previous match
        /// </summary>
        /// <returns>The match found, or null if none</returns>
        public SearchMatch FindPrevious()
        {
            return SearchHelper.FindPrevious();
        }

        /// <summary>
        /// Find next occurrence of the specified text
        /// </summary>
        public SearchMatch FindNext(string searchText)
        {
            if (SearchHelper.Options.SearchText != searchText)
            {
                SearchHelper.Search(searchText);
            }
            return SearchHelper.FindNext();
        }

        /// <summary>
        /// Replace the current match
        /// </summary>
        /// <param name="replaceText">Replacement text</param>
        /// <returns>True if replaced successfully</returns>
        public bool ReplaceCurrent(string replaceText)
        {
            return SearchHelper.ReplaceCurrent(replaceText);
        }

        /// <summary>
        /// Replace all matches
        /// </summary>
        /// <param name="searchText">Text to search for</param>
        /// <param name="replaceText">Replacement text</param>
        /// <returns>Number of replacements made</returns>
        public int ReplaceAll(string searchText, string replaceText)
        {
            if (SearchHelper.Options.SearchText != searchText)
            {
                SearchHelper.Search(searchText);
            }
            return SearchHelper.ReplaceAll(replaceText);
        }

        /// <summary>
        /// Clear current search and highlighting
        /// </summary>
        public void ClearSearch()
        {
            SearchHelper.ClearSearch();
            Invalidate();
        }

        /// <summary>
        /// Go to a specific match by index
        /// </summary>
        /// <param name="index">Match index (0-based)</param>
        public void GoToMatch(int index)
        {
            SearchHelper.GoToMatch(index);
        }

        /// <summary>
        /// Get the match at the specified position
        /// </summary>
        public SearchMatch GetMatchAtPosition(int position)
        {
            return SearchHelper.GetMatchAtPosition(position);
        }

        /// <summary>
        /// Show find dialog (can be overridden for custom UI)
        /// </summary>
        public virtual void ShowFindDialog()
        {
            // Default implementation - can be overridden
            string searchText = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter text to find:",
                "Find",
                SearchHelper.Options.SearchText);

            if (!string.IsNullOrEmpty(searchText))
            {
                Search(searchText);
                FindNext();
            }
        }

        /// <summary>
        /// Show find and replace dialog (can be overridden for custom UI)
        /// </summary>
        public virtual void ShowFindReplaceDialog()
        {
            // Default implementation - can be overridden
            ShowFindDialog();
        }

        #endregion

        #region Search Keyboard Shortcuts

        /// <summary>
        /// Handle search-related keyboard shortcuts
        /// </summary>
        private bool HandleSearchKeyDown(KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.F:
                        ShowFindDialog();
                        e.Handled = true;
                        return true;

                    case Keys.H:
                        ShowFindReplaceDialog();
                        e.Handled = true;
                        return true;

                    case Keys.G:
                        // Go to line (for multiline)
                        if (_multiline)
                        {
                            // Could implement GoToLine dialog
                        }
                        e.Handled = true;
                        return true;
                }
            }

            switch (e.KeyCode)
            {
                case Keys.F3:
                    if (e.Shift)
                        FindPrevious();
                    else
                        FindNext();
                    e.Handled = true;
                    return true;

                case Keys.Escape:
                    if (IsSearchActive)
                    {
                        ClearSearch();
                        e.Handled = true;
                        return true;
                    }
                    break;
            }

            return false;
        }

        #endregion

        #region Search Drawing

        /// <summary>
        /// Draw search match highlights
        /// </summary>
        private void DrawSearchHighlights(Graphics g, Rectangle textRect)
        {
            if (!_searchHighlightEnabled || !IsSearchActive)
                return;

            var highlights = SearchHelper.GetMatchHighlightRects(
                g, 
                textRect, 
                _textFont,
                _helper?.Scrolling?.ScrollOffsetX ?? 0);

            foreach (var (rect, isCurrent) in highlights)
            {
                Color highlightColor = isCurrent 
                    ? SearchHelper.CurrentMatchHighlightColor 
                    : SearchHelper.MatchHighlightColor;

                using (var brush = new SolidBrush(Color.FromArgb(128, highlightColor)))
                {
                    g.FillRectangle(brush, rect);
                }

                if (isCurrent)
                {
                    using (var pen = new Pen(highlightColor, 1))
                    {
                        g.DrawRectangle(pen, rect);
                    }
                }
            }
        }

        #endregion
    }
}

