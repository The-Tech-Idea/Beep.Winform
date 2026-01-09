using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.TextFields.Helpers
{
    /// <summary>
    /// Advanced DevExpress-Style autocomplete with fuzzy matching and popularity scoring
    /// </summary>
    public class SmartAutoCompleteHelper
    {
        #region "Smart AutoComplete Engine"
        
        /// <summary>
        /// Advanced fuzzy matching autocomplete like DevExpress
        /// </summary>
        public class SmartAutoComplete
        {
            private readonly Dictionary<string, int> _popularityScores = new();
            private readonly List<string> _suggestions = new();
            
            public void AddSuggestion(string suggestion, int popularity = 1)
            {
                if (!_suggestions.Contains(suggestion))
                {
                    _suggestions.Add(suggestion);
                    _popularityScores[suggestion] = popularity;
                }
                else
                {
                    _popularityScores[suggestion] = _popularityScores.GetValueOrDefault(suggestion, 0) + popularity;
                }
            }
            
            public List<string> GetSuggestions(string input, int maxResults = 10)
            {
                if (string.IsNullOrEmpty(input)) return new List<string>();
                
                var results = new List<(string suggestion, int score)>();
                
                foreach (var suggestion in _suggestions)
                {
                    int score = CalculateMatchScore(input, suggestion);
                    if (score > 0)
                    {
                        results.Add((suggestion, score));
                    }
                }
                
                return results
                    .OrderByDescending(x => x.score)
                    .ThenByDescending(x => _popularityScores.GetValueOrDefault(x.suggestion, 0))
                    .ThenBy(x => x.suggestion.Length)
                    .Take(maxResults)
                    .Select(x => x.suggestion)
                    .ToList();
            }
            
            private int CalculateMatchScore(string input, string suggestion)
            {
                if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(suggestion))
                    return 0;
                
                input = input.ToLowerInvariant();
                suggestion = suggestion.ToLowerInvariant();
                
                // Exact match gets highest score
                if (suggestion == input) return 1000;
                
                // Starts with gets high score
                if (suggestion.StartsWith(input)) return 800;
                
                // Contains gets medium score
                if (suggestion.Contains(input)) return 600;
                
                // Fuzzy match using Levenshtein distance
                int distance = LevenshteinDistance(input, suggestion);
                if (distance <= 2 && input.Length > 2) // Allow 2 character differences
                {
                    return 400 - distance * 100;
                }
                
                // Subsequence match (characters appear in order)
                if (IsSubsequence(input, suggestion))
                {
                    return 200;
                }
                
                return 0;
            }
            
            private int LevenshteinDistance(string s1, string s2)
            {
                int[,] matrix = new int[s1.Length + 1, s2.Length + 1];
                
                for (int i = 0; i <= s1.Length; i++)
                    matrix[i, 0] = i;
                for (int j = 0; j <= s2.Length; j++)
                    matrix[0, j] = j;
                
                for (int i = 1; i <= s1.Length; i++)
                {
                    for (int j = 1; j <= s2.Length; j++)
                    {
                        int cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                        matrix[i, j] = Math.Min(Math.Min(
                            matrix[i - 1, j] + 1,
                            matrix[i, j - 1] + 1),
                            matrix[i - 1, j - 1] + cost);
                    }
                }
                
                return matrix[s1.Length, s2.Length];
            }
            
            private bool IsSubsequence(string s, string t)
            {
                int sIndex = 0;
                for (int tIndex = 0; tIndex < t.Length && sIndex < s.Length; tIndex++)
                {
                    if (s[sIndex] == t[tIndex])
                        sIndex++;
                }
                return sIndex == s.Length;
            }
        }
        
        #endregion
        
        #region "Fields"
        
        private readonly Control _parentControl;
        private BeepPopupListForm _autoCompletePopup;
        private List<string> _autoCompleteList = new List<string>();
        private bool _isAutoCompleteVisible = false;
        private System.Windows.Forms.Timer _autoCompleteTimer;  // Use full namespace
        private string _lastAutoCompleteSearch = "";
        private int _autoCompleteDelay = 300; // milliseconds
        private SmartAutoComplete _smartAutoComplete = new SmartAutoComplete();
        
        // Configuration properties
        public int AutoCompleteMinimumLength { get; set; } = 1;
        public int AutoCompleteMaxSuggestions { get; set; } = 10;
        public bool AutoCompleteCaseSensitive { get; set; } = false;
        public AutoCompleteMode AutoCompleteMode { get; set; } = AutoCompleteMode.None;
        public AutoCompleteSource AutoCompleteSource { get; set; } = AutoCompleteSource.None;
        public AutoCompleteStringCollection AutoCompleteCustomSource { get; set; }
        
        #endregion
        
        #region "Events"
        
        public event EventHandler<string> AutoCompleteItemSelected;
        public event EventHandler<string> AutoCompleteSearching;
        public event EventHandler AutoCompleteShown;
        public event EventHandler AutoCompleteHidden;
        
        #endregion
        
        #region "Constructor"
        
        public SmartAutoCompleteHelper(Control parentControl)
        {
            _parentControl = parentControl ?? throw new ArgumentNullException(nameof(parentControl));
            InitializeAutoComplete();
        }
        
        #endregion
        
        #region "Initialization"
        
        private void InitializeAutoComplete()
        {
            // Initialize autocomplete timer
            _autoCompleteTimer = new System.Windows.Forms.Timer();  // Use full namespace
            _autoCompleteTimer.Interval = _autoCompleteDelay;
            _autoCompleteTimer.Tick += AutoCompleteTimer_Tick;

            // Initialize autocomplete custom source if needed
            InitializeAutoCompleteCustomSource();
        }
        
        private void InitializeAutoCompleteCustomSource()
        {
            if (AutoCompleteCustomSource == null)
            {
                AutoCompleteCustomSource = new AutoCompleteStringCollection();
            }

            // Populate from AutoCompleteList if it has items
            if (_autoCompleteList?.Count > 0)
            {
                AutoCompleteCustomSource.Clear();
                AutoCompleteCustomSource.AddRange(_autoCompleteList.ToArray());
            }
        }
        
        #endregion
        
        #region "Public Methods"
        
        /// <summary>
        /// Add items to smart autocomplete with popularity scoring
        /// </summary>
        public void AddSmartAutoCompleteItem(string item, int popularity = 1)
        {
            _smartAutoComplete.AddSuggestion(item, popularity);
        }
        
        /// <summary>
        /// Add multiple items to smart autocomplete
        /// </summary>
        public void AddSmartAutoCompleteItems(IEnumerable<string> items, int popularity = 1)
        {
            foreach (string item in items)
            {
                _smartAutoComplete.AddSuggestion(item, popularity);
            }
        }
        
        public void AddAutoCompleteItem(string item)
        {
            if (string.IsNullOrEmpty(item)) return;

            if (!_autoCompleteList.Contains(item))
            {
                _autoCompleteList.Add(item);
                AutoCompleteCustomSource?.Add(item);
            }
        }

        public void AddAutoCompleteItems(IEnumerable<string> items)
        {
            if (items == null) return;

            foreach (string item in items)
            {
                AddAutoCompleteItem(item);
            }
        }

        public void ClearAutoCompleteItems()
        {
            _autoCompleteList.Clear();
            AutoCompleteCustomSource?.Clear();
        }
        
        /// <summary>
        /// Get autocomplete suggestions without showing popup (for testing)
        /// </summary>
        public List<string> GetAutoCompleteSuggestionsPreview(string input)
        {
            return GetSmartAutoCompleteSuggestions(input);
        }
        
        /// <summary>
        /// Trigger autocomplete with smart matching
        /// </summary>
        public void TriggerSmartAutoComplete(string currentText)
        {
            if (AutoCompleteMode == AutoCompleteMode.None) return;

            _autoCompleteTimer.Stop();

            if (string.IsNullOrEmpty(currentText))
            {
                HideAutoCompletePopup();
                return;
            }

            if (currentText != _lastAutoCompleteSearch)
            {
                if (currentText.Length >= AutoCompleteMinimumLength)
                {
                    _autoCompleteTimer.Start();
                }
                else
                {
                    HideAutoCompletePopup();
                }
            }
        }
        
        #endregion
        
        #region "Autocomplete Logic"
        
        private void AutoCompleteTimer_Tick(object sender, EventArgs e)
        {
            _autoCompleteTimer.Stop();

            if (AutoCompleteMode != AutoCompleteMode.None)
            {
                ShowAutoCompletePopup();
            }
        }
        
        private void ShowAutoCompletePopup()
        {
            if (_isAutoCompleteVisible) return;

            string searchText = GetCurrentWord();
            if (string.IsNullOrEmpty(searchText) || searchText.Length < AutoCompleteMinimumLength)
            {
                HideAutoCompletePopup();
                return;
            }

            // Get suggestions
            var suggestions = GetAutoCompleteSuggestions(searchText);
            if (suggestions.Count == 0)
            {
                HideAutoCompletePopup();
                return;
            }

            // Create or update popup
            if (_autoCompletePopup == null)
            {
                _autoCompletePopup = new BeepPopupListForm();
                _autoCompletePopup.AutoClose = true;
                _autoCompletePopup.ShowInTaskbar = false;
                _autoCompletePopup.SelectedItemChanged += AutoCompletePopup_SelectedItemChanged;
                _autoCompletePopup.FormClosed += AutoCompletePopup_FormClosed;
            }

            // Populate suggestions
            _autoCompletePopup.ListItems.Clear();
            foreach (string suggestion in suggestions)
            {
                _autoCompletePopup.ListItems.Add(new SimpleItem { Text = suggestion });
            }

            // Calculate popup position
            Point popupLocation = CalculateAutoCompletePopupLocation();

            // Set popup size
            int popupWidth = Math.Max(_parentControl.Width, 200);
            int popupHeight = Math.Min(suggestions.Count * 25 + 10, 200);
            _autoCompletePopup.Size = new Size(popupWidth, popupHeight);

            // Show popup
            _autoCompletePopup.ShowPopup(_parentControl, popupLocation);
            _isAutoCompleteVisible = true;
            _lastAutoCompleteSearch = searchText;

            // Raise events
            AutoCompleteShown?.Invoke(this, EventArgs.Empty);
            AutoCompleteSearching?.Invoke(this, searchText);
        }
        
        private void HideAutoCompletePopup()
        {
            if (!_isAutoCompleteVisible) return;

            _autoCompletePopup?.CloseCascade();
            _isAutoCompleteVisible = false;
            _lastAutoCompleteSearch = "";

            // Raise event
            AutoCompleteHidden?.Invoke(this, EventArgs.Empty);
        }
        
        private Point CalculateAutoCompletePopupLocation()
        {
            Point screenLocation = _parentControl.PointToScreen(new Point(0, _parentControl.Height));

            // Check if popup would go off screen and adjust
            Screen currentScreen = Screen.FromControl(_parentControl);
            if (screenLocation.Y + 200 > currentScreen.WorkingArea.Bottom)
            {
                // Show above the textbox instead
                screenLocation.Y = _parentControl.PointToScreen(new Point(0, -200)).Y;
            }

            return screenLocation;
        }
        
        private string GetCurrentWord()
        {
            if (!(_parentControl is IBeepTextBox textBox))
                return "";

            string text = textBox.Text ?? "";
            int caretPosition = textBox.SelectionStart;
            
            if (string.IsNullOrEmpty(text) || caretPosition < 0)
                return "";

            // Find word boundaries
            int start = caretPosition;
            int end = caretPosition;

            // Find start of word
            while (start > 0 && IsWordCharacter(text[start - 1]))
            {
                start--;
            }

            // Find end of word
            while (end < text.Length && IsWordCharacter(text[end]))
            {
                end++;
            }

            if (start < end)
            {
                return text.Substring(start, end - start);
            }

            return "";
        }

        private bool IsWordCharacter(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || c == '-';
        }
        
        #endregion
        
        #region "Suggestion Generation"
        
        private List<string> GetAutoCompleteSuggestions(string searchText)
        {
            var suggestions = new List<string>();

            if (AutoCompleteSource == AutoCompleteSource.None || string.IsNullOrEmpty(searchText))
                return suggestions;

            try
            {
                switch (AutoCompleteSource)
                {
                    case AutoCompleteSource.CustomSource:
                        if (AutoCompleteCustomSource != null)
                        {
                            // Use smart autocomplete for better matching
                            suggestions = GetSmartAutoCompleteSuggestions(searchText);
                        }
                        break;

                    case AutoCompleteSource.FileSystem:
                        suggestions = GetFileSystemSuggestions(searchText);
                        break;

                    case AutoCompleteSource.HistoryList:
                        suggestions = GetHistorySuggestions(searchText);
                        break;

                    case AutoCompleteSource.RecentlyUsedList:
                        suggestions = GetRecentlyUsedSuggestions(searchText);
                        break;

                    case AutoCompleteSource.AllUrl:
                    case AutoCompleteSource.AllSystemSources:
                        // Use smart suggestions for these too
                        suggestions = GetSmartAutoCompleteSuggestions(searchText);
                        break;
                }

                // Apply mode-specific filtering
                if (AutoCompleteMode == AutoCompleteMode.SuggestAppend || AutoCompleteMode == AutoCompleteMode.Suggest)
                {
                    suggestions = suggestions.Take(AutoCompleteMaxSuggestions).ToList();
                }

                return suggestions;
            }
            catch (Exception)
            {
                return suggestions;
            }
        }
        
        private List<string> GetSmartAutoCompleteSuggestions(string searchText)
        {
            // Add frequently used items to smart autocomplete
            foreach (string item in _autoCompleteList)
            {
                _smartAutoComplete.AddSuggestion(item);
            }
            
            if (AutoCompleteCustomSource != null)
            {
                foreach (string item in AutoCompleteCustomSource)
                {
                    _smartAutoComplete.AddSuggestion(item);
                }
            }
            
            return _smartAutoComplete.GetSuggestions(searchText, AutoCompleteMaxSuggestions);
        }
        
        private List<string> GetSuggestionsFromCustomSource(string searchText)
        {
            var suggestions = new List<string>();

            if (AutoCompleteCustomSource == null) return suggestions;

            var comparison = AutoCompleteCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            foreach (string item in AutoCompleteCustomSource)
            {
                if (item.IndexOf(searchText, comparison) >= 0)
                {
                    suggestions.Add(item);
                }
            }

            return suggestions.OrderBy(s => s.IndexOf(searchText, comparison))
                            .ThenBy(s => s.Length)
                            .ToList();
        }

        private List<string> GetFileSystemSuggestions(string searchText)
        {
            var suggestions = new List<string>();

            try
            {
                if (Directory.Exists(searchText))
                {
                    var directories = Directory.GetDirectories(searchText)
                                               .Take(AutoCompleteMaxSuggestions / 2);
                    var files = Directory.GetFiles(searchText)
                                        .Take(AutoCompleteMaxSuggestions / 2);

                    suggestions.AddRange(directories);
                    suggestions.AddRange(files);
                }
                else
                {
                    string directory = Path.GetDirectoryName(searchText) ?? "";
                    string fileName = Path.GetFileName(searchText);

                    if (Directory.Exists(directory))
                    {
                        var matches = Directory.GetFileSystemEntries(directory, fileName + "*")
                                               .Take(AutoCompleteMaxSuggestions);
                        suggestions.AddRange(matches);
                    }
                }
            }
            catch (Exception)
            {
                // Ignore filesystem errors
            }

            return suggestions;
        }

        private List<string> GetHistorySuggestions(string searchText)
        {
            // This would typically come from a history storage mechanism
            // For now, return from custom source
            return GetSuggestionsFromCustomSource(searchText);
        }

        private List<string> GetRecentlyUsedSuggestions(string searchText)
        {
            // This would typically come from a recently used items storage
            // For now, return from custom source
            return GetSuggestionsFromCustomSource(searchText);
        }
        
        #endregion
        
        #region "Event Handlers"
        
        private void AutoCompletePopup_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                string selectedText = e.SelectedItem.Text;

                // Apply the selected suggestion based on the autocomplete mode
                ApplyAutoCompleteSuggestion(selectedText);

                // Hide the popup
                HideAutoCompletePopup();

                // Raise event
                AutoCompleteItemSelected?.Invoke(this, selectedText);
            }
        }

        private void AutoCompletePopup_FormClosed(object sender, FormClosedEventArgs e)
        {
            _isAutoCompleteVisible = false;
        }

        private void ApplyAutoCompleteSuggestion(string suggestion)
        {
            if (string.IsNullOrEmpty(suggestion)) return;
            if (!(_parentControl is IBeepTextBox textBox)) return;

            switch (AutoCompleteMode)
            {
                case AutoCompleteMode.Append:
                case AutoCompleteMode.SuggestAppend:
                    // Replace the current text with the suggestion
                    textBox.Text = suggestion;
                    break;

                case AutoCompleteMode.Suggest:
                    // Replace the current text with the suggestion
                    textBox.Text = suggestion;
                    break;
            }
        }
        
        #endregion
        
        #region "Cleanup"
        
        public void Dispose()
        {
            _autoCompleteTimer?.Dispose();
            
            if (_autoCompletePopup != null)
            {
                _autoCompletePopup.SelectedItemChanged -= AutoCompletePopup_SelectedItemChanged;
                _autoCompletePopup.FormClosed -= AutoCompletePopup_FormClosed;
                _autoCompletePopup.Dispose();
            }
        }
        
        #endregion
    }
}