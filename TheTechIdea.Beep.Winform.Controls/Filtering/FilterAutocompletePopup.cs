using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Autocomplete popup for filter value suggestions
    /// Displays smart suggestions with icons, match counts, and keyboard navigation
    /// </summary>
    public class FilterAutocompletePopup : Form
    {
        private ListBox _suggestionList;
        private List<FilterSuggestion> _suggestions = new List<FilterSuggestion>();
        private IBeepTheme _theme;
        private const int ItemHeight = 36;
        private const int MaxVisibleItems = 8;
        
        /// <summary>
        /// Event fired when a suggestion is selected
        /// </summary>
        public event EventHandler<FilterSuggestion> SuggestionSelected;
        
        /// <summary>
        /// Initializes the autocomplete popup
        /// </summary>
        public FilterAutocompletePopup(IBeepTheme theme = null)
        {
            _theme = theme;
            
            InitializeForm();
            InitializeListBox();
        }
        
        private void InitializeForm()
        {
            // Form setup
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            Size = new Size(320, ItemHeight * Math.Min(MaxVisibleItems, 5) + 4);
            BackColor = Color.White;
            Padding = new Padding(2);
            
            // Make it look like a dropdown
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.OptimizedDoubleBuffer, true);
        }
        
        private void InitializeListBox()
        {
            _suggestionList = new ListBox
            {
                Dock = DockStyle.Fill,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = ItemHeight,
                BorderStyle = BorderStyle.None,
                IntegralHeight = false
            };
            
            _suggestionList.DrawItem += OnDrawItem;
            _suggestionList.SelectedIndexChanged += OnSelectedIndexChanged;
            _suggestionList.MouseClick += OnMouseClick;
            _suggestionList.KeyDown += OnKeyDown;
            
            Controls.Add(_suggestionList);
        }
        
        /// <summary>
        /// Shows suggestions at the specified location
        /// </summary>
        public void ShowSuggestions(List<FilterSuggestion> suggestions, Point screenLocation)
        {
            if (suggestions == null || suggestions.Count == 0)
            {
                Hide();
                return;
            }
            
            _suggestions = suggestions;
            _suggestionList.Items.Clear();
            
            foreach (var suggestion in suggestions)
            {
                _suggestionList.Items.Add(suggestion);
            }
            
            // Adjust size based on item count
            int visibleItems = Math.Min(suggestions.Count, MaxVisibleItems);
            Height = (ItemHeight * visibleItems) + 4;
            
            // Position popup
            Location = AdjustLocationToScreen(screenLocation);
            
            // Show and focus
            Show();
            _suggestionList.Focus();
            
            // Select first item
            if (_suggestionList.Items.Count > 0)
            {
                _suggestionList.SelectedIndex = 0;
            }
        }
        
        /// <summary>
        /// Adjusts location to keep popup on screen
        /// </summary>
        private Point AdjustLocationToScreen(Point location)
        {
            var screen = Screen.FromPoint(location);
            
            // Adjust horizontal position
            if (location.X + Width > screen.WorkingArea.Right)
            {
                location.X = screen.WorkingArea.Right - Width - 8;
            }
            if (location.X < screen.WorkingArea.Left)
            {
                location.X = screen.WorkingArea.Left + 8;
            }
            
            // Adjust vertical position
            if (location.Y + Height > screen.WorkingArea.Bottom)
            {
                // Show above instead of below
                location.Y = location.Y - Height - 24;
            }
            if (location.Y < screen.WorkingArea.Top)
            {
                location.Y = screen.WorkingArea.Top + 8;
            }
            
            return location;
        }
        
        /// <summary>
        /// Draws each suggestion item
        /// </summary>
        private void OnDrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _suggestions.Count)
                return;
            
            var suggestion = _suggestions[e.Index];
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            // Background
            var isSelected = e.State.HasFlag(DrawItemState.Selected);
            var bgColor = isSelected 
                ? _theme?.ButtonHoverBackColor ?? Color.FromArgb(240, 245, 255)
                : _theme?.BackColor ?? Color.White;
            
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, e.Bounds);
            }
            
            // Icon (emoji or drawn icon)
            int iconSize = 20;
            var iconBounds = new Rectangle(e.Bounds.X + 8, e.Bounds.Y + (e.Bounds.Height - iconSize) / 2, iconSize, iconSize);
            
            if (!string.IsNullOrEmpty(suggestion.Icon))
            {
                using (var font = new Font("Segoe UI Emoji", 12f))
                using (var brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
                {
                    g.DrawString(suggestion.Icon, font, brush, iconBounds.X, iconBounds.Y);
                }
            }
            
            // Display text
            int textX = iconBounds.Right + 8;
            using (var font = new Font("Segoe UI", 9.5f))
            using (var brush = new SolidBrush(_theme?.ForeColor ?? Color.FromArgb(40, 40, 40)))
            {
                var textRect = new RectangleF(textX, e.Bounds.Y + 6, e.Bounds.Width - textX - 60, 18);
                var format = new StringFormat
                {
                    Trimming = StringTrimming.EllipsisCharacter,
                    LineAlignment = StringAlignment.Near
                };
                
                g.DrawString(suggestion.DisplayText, font, brush, textRect, format);
            }
            
            // Match count (if available)
            if (suggestion.MatchCount > 0)
            {
                string countText = $"({suggestion.MatchCount})";
                using (var font = new Font("Segoe UI", 8.5f))
                using (var brush = new SolidBrush(Color.FromArgb(130, 130, 130)))
                {
                    var textSize = g.MeasureString(countText, font);
                    g.DrawString(countText, font, brush, 
                        e.Bounds.Right - textSize.Width - 12, 
                        e.Bounds.Y + 11);
                }
            }
            
            // Type badge
            if (!string.IsNullOrEmpty(suggestion.Description))
            {
                using (var font = new Font("Segoe UI", 7.5f))
                using (var brush = new SolidBrush(Color.FromArgb(160, 160, 160)))
                {
                    g.DrawString(suggestion.Description, font, brush, textX, e.Bounds.Y + 20);
                }
            }
            
            // Focus rectangle
            if (isSelected)
            {
                var focusRect = e.Bounds;
                focusRect.Inflate(-1, -1);
                using (var pen = new Pen(_theme?.AccentColor ?? Color.FromArgb(0, 120, 212), 1f))
                {
                    g.DrawRectangle(pen, focusRect);
                }
            }
        }
        
        /// <summary>
        /// Handles selection change
        /// </summary>
        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            // Just repaint - actual selection happens on click or enter
        }
        
        /// <summary>
        /// Handles mouse click on suggestion
        /// </summary>
        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (_suggestionList.SelectedIndex >= 0 && _suggestionList.SelectedIndex < _suggestions.Count)
            {
                SelectSuggestion(_suggestions[_suggestionList.SelectedIndex]);
            }
        }
        
        /// <summary>
        /// Handles keyboard input
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (_suggestionList.SelectedIndex >= 0 && _suggestionList.SelectedIndex < _suggestions.Count)
                    {
                        SelectSuggestion(_suggestions[_suggestionList.SelectedIndex]);
                        e.Handled = true;
                    }
                    break;
                
                case Keys.Escape:
                    Hide();
                    e.Handled = true;
                    break;
                
                case Keys.Up:
                case Keys.Down:
                    // Let listbox handle it
                    break;
                
                case Keys.Tab:
                    // Tab selects current and closes
                    if (_suggestionList.SelectedIndex >= 0 && _suggestionList.SelectedIndex < _suggestions.Count)
                    {
                        SelectSuggestion(_suggestions[_suggestionList.SelectedIndex]);
                    }
                    e.Handled = true;
                    break;
            }
        }
        
        /// <summary>
        /// Selects a suggestion and raises event
        /// </summary>
        private void SelectSuggestion(FilterSuggestion suggestion)
        {
            SuggestionSelected?.Invoke(this, suggestion);
            Hide();
        }
        
        /// <summary>
        /// Paints the popup border
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            var g = e.Graphics;
            
            // Draw shadow
            for (int i = 3; i > 0; i--)
            {
                var shadowRect = ClientRectangle;
                shadowRect.Inflate(i, i);
                
                int alpha = 20 - (i * 5);
                using (var pen = new Pen(Color.FromArgb(alpha, 0, 0, 0)))
                {
                    g.DrawRectangle(pen, shadowRect);
                }
            }
            
            // Draw border
            using (var pen = new Pen(_theme?.BorderColor ?? Color.FromArgb(200, 200, 200), 1f))
            {
                var borderRect = ClientRectangle;
                borderRect.Inflate(-1, -1);
                g.DrawRectangle(pen, borderRect);
            }
        }
        
        /// <summary>
        /// Handles focus loss
        /// </summary>
        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            Hide();
        }
        
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000008; // WS_EX_TOPMOST
                return cp;
            }
        }
    }
}

