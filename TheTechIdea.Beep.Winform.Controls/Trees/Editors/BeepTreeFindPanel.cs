using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Editors
{
    /// <summary>
    /// A find/search panel for BeepTree that allows searching through node text.
    /// Appears when Ctrl+F is pressed.
    /// </summary>
    public class BeepTreeFindPanel : Panel
    {
        private TextBox _searchTextBox;
        private Button _findNextButton;
        private Button _findPreviousButton;
        private Button _closeButton;
        private Label _resultLabel;
        private CheckBox _matchCaseCheckBox;
        private CheckBox _wholeWordCheckBox;

        private BeepTree _owner;
        private List<SimpleItem> _matches;
        private int _currentMatchIndex = -1;

        /// <summary>
        /// Occurs when the find panel requests to find the next match.
        /// </summary>
        public event EventHandler<FindEventArgs> FindNext;

        /// <summary>
        /// Occurs when the find panel requests to find the previous match.
        /// </summary>
        public event EventHandler<FindEventArgs> FindPrevious;

        /// <summary>
        /// Occurs when the find panel is closed.
        /// </summary>
        public event EventHandler Closed;

        public string SearchText => _searchTextBox?.Text ?? string.Empty;
        public bool MatchCase => _matchCaseCheckBox?.Checked ?? false;
        public bool WholeWord => _wholeWordCheckBox?.Checked ?? false;

        public BeepTreeFindPanel(BeepTree owner)
        {
            _owner = owner;
            _matches = new List<SimpleItem>();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Height = 32;
            this.Dock = DockStyle.Top;
            this.BackColor = SystemColors.Control;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Padding = new Padding(2);

            // Search text box
            _searchTextBox = new TextBox
            {
                Width = 200,
                Height = 22,
                Top = 4,
                Left = 4,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            _searchTextBox.KeyDown += SearchTextBox_KeyDown;
            _searchTextBox.TextChanged += SearchTextBox_TextChanged;
            this.Controls.Add(_searchTextBox);

            // Find next button
            _findNextButton = new Button
            {
                Text = "Next",
                Width = 50,
                Height = 22,
                Top = 4,
                Left = 208,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            _findNextButton.Click += (s, e) => OnFindNext();
            this.Controls.Add(_findNextButton);

            // Find previous button
            _findPreviousButton = new Button
            {
                Text = "Prev",
                Width = 50,
                Height = 22,
                Top = 4,
                Left = 262,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            _findPreviousButton.Click += (s, e) => OnFindPrevious();
            this.Controls.Add(_findPreviousButton);

            // Result label
            _resultLabel = new Label
            {
                Text = "",
                Width = 100,
                Height = 22,
                Top = 6,
                Left = 316,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            this.Controls.Add(_resultLabel);

            // Match case checkbox
            _matchCaseCheckBox = new CheckBox
            {
                Text = "Match case",
                Width = 80,
                Height = 22,
                Top = 4,
                Left = 420,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            this.Controls.Add(_matchCaseCheckBox);

            // Whole word checkbox
            _wholeWordCheckBox = new CheckBox
            {
                Text = "Whole word",
                Width = 80,
                Height = 22,
                Top = 4,
                Left = 505,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            this.Controls.Add(_wholeWordCheckBox);

            // Close button
            _closeButton = new Button
            {
                Text = "X",
                Width = 24,
                Height = 22,
                Top = 4,
                Left = this.Width - 28,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            _closeButton.Click += (s, e) => Close();
            this.Controls.Add(_closeButton);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_closeButton != null)
            {
                _closeButton.Left = this.Width - 28;
            }
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnFindNext();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Close();
                e.Handled = true;
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            _currentMatchIndex = -1;
            _matches.Clear();
            UpdateResultLabel();
        }

        private void OnFindNext()
        {
            if (string.IsNullOrEmpty(SearchText))
                return;

            FindNext?.Invoke(this, new FindEventArgs(SearchText, MatchCase, WholeWord));
        }

        private void OnFindPrevious()
        {
            if (string.IsNullOrEmpty(SearchText))
                return;

            FindPrevious?.Invoke(this, new FindEventArgs(SearchText, MatchCase, WholeWord));
        }

        /// <summary>
        /// Updates the search results display.
        /// </summary>
        public void UpdateResults(List<SimpleItem> matches, int currentIndex)
        {
            _matches = matches ?? new List<SimpleItem>();
            _currentMatchIndex = currentIndex;
            UpdateResultLabel();
        }

        private void UpdateResultLabel()
        {
            if (_matches.Count == 0)
            {
                _resultLabel.Text = string.IsNullOrEmpty(SearchText) ? "" : "No matches";
            }
            else
            {
                _resultLabel.Text = $"{_currentMatchIndex + 1} of {_matches.Count}";
            }
        }

        /// <summary>
        /// Shows the find panel and focuses the search box.
        /// </summary>
        public void ShowPanel()
        {
            this.Visible = true;
            _searchTextBox?.Focus();
            _searchTextBox?.SelectAll();
        }

        /// <summary>
        /// Hides the find panel.
        /// </summary>
        public void Close()
        {
            this.Visible = false;
            _searchTextBox.Text = string.Empty;
            _matches.Clear();
            _currentMatchIndex = -1;
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Event arguments for find operations.
    /// </summary>
    public class FindEventArgs : EventArgs
    {
        public string SearchText { get; }
        public bool MatchCase { get; }
        public bool WholeWord { get; }

        public FindEventArgs(string searchText, bool matchCase, bool wholeWord)
        {
            SearchText = searchText;
            MatchCase = matchCase;
            WholeWord = wholeWord;
        }
    }
}
