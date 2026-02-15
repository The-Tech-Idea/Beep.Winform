using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    /// <summary>
    /// History panel for viewing dismissed notifications
    /// Shows a list of past notifications with filtering and search
    /// </summary>
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Notification history panel with filtering and search")]
    public class BeepNotificationHistory : BaseControl
    {
        #region Private Fields
        private readonly List<NotificationHistoryItem> _history;
        private readonly Panel _headerPanel;
        private readonly Label _titleLabel;
        private readonly TextBox _searchBox;
        private readonly ComboBox _filterCombo;
        private readonly Button _clearButton;
        private readonly Panel _listPanel;
        private readonly VScrollBar _scrollBar;
        private int _scrollOffset = 0;
        private int _itemHeight = 60;
        private int _maxHistorySize = 100;
        private NotificationType? _filterType = null;
        private Font _textFont;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of BeepNotificationHistory
        /// </summary>
        public BeepNotificationHistory()
        {
            _history = new List<NotificationHistoryItem>();

            // BaseControl configuration
            ControlStyle = BeepControlStyle.Material3;
            IsRounded = true;
            BorderRadius = DpiScalingHelper.ScaleValue(8, this);
            ShowShadow = true;
            ShadowOffset = DpiScalingHelper.ScaleValue(2, this);
            ShowAllBorders = true;
            BorderThickness = 1;
            MinimumSize = new Size(DpiScalingHelper.ScaleValue(300, this), DpiScalingHelper.ScaleValue(400, this));
            Size = new Size(DpiScalingHelper.ScaleValue(350, this), DpiScalingHelper.ScaleValue(500, this));

            // Header panel
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = DpiScalingHelper.ScaleValue(100, this),
                BackColor = Color.Transparent
            };

            _titleLabel = new Label
            {
                Text = "Notification History",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(DpiScalingHelper.ScaleValue(12, this), DpiScalingHelper.ScaleValue(12, this)),
                AutoSize = true
            };

            _searchBox = new TextBox
            {
                PlaceholderText = "Search notifications...",
                Location = new Point(DpiScalingHelper.ScaleValue(12, this), DpiScalingHelper.ScaleValue(40, this)),
                Width = DpiScalingHelper.ScaleValue(200, this),
                Height = DpiScalingHelper.ScaleValue(24, this)
            };
            _searchBox.TextChanged += SearchBox_TextChanged;

            _filterCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(DpiScalingHelper.ScaleValue(220, this), DpiScalingHelper.ScaleValue(40, this)),
                Width = DpiScalingHelper.ScaleValue(100, this),
                Height = DpiScalingHelper.ScaleValue(24, this)
            };
            _filterCombo.Items.Add("All");
            _filterCombo.Items.Add("Info");
            _filterCombo.Items.Add("Success");
            _filterCombo.Items.Add("Warning");
            _filterCombo.Items.Add("Error");
            _filterCombo.Items.Add("System");
            _filterCombo.SelectedIndex = 0;
            _filterCombo.SelectedIndexChanged += FilterCombo_SelectedIndexChanged;

            _clearButton = new Button
            {
                Text = "Clear",
                Location = new Point(DpiScalingHelper.ScaleValue(12, this), DpiScalingHelper.ScaleValue(70, this)),
                Width = DpiScalingHelper.ScaleValue(80, this),
                Height = DpiScalingHelper.ScaleValue(24, this)
            };
            _clearButton.Click += ClearButton_Click;

            _headerPanel.Controls.Add(_titleLabel);
            _headerPanel.Controls.Add(_searchBox);
            _headerPanel.Controls.Add(_filterCombo);
            _headerPanel.Controls.Add(_clearButton);

            // List panel
            _listPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = false
            };
            _listPanel.Paint += ListPanel_Paint;
            _listPanel.MouseWheel += ListPanel_MouseWheel;
            _listPanel.Click += ListPanel_Click;

            // Scroll bar
            _scrollBar = new VScrollBar
            {
                Dock = DockStyle.Right,
                Visible = false
            };
            _scrollBar.Scroll += ScrollBar_Scroll;

            // Add controls
            this.Controls.Add(_listPanel);
            this.Controls.Add(_scrollBar);
            this.Controls.Add(_headerPanel);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Maximum number of notifications to keep in history
        /// </summary>
        [Category("Behavior")]
        [Description("Maximum number of notifications to store")]
        [DefaultValue(100)]
        public int MaxHistorySize
        {
            get => _maxHistorySize;
            set => _maxHistorySize = Math.Max(10, value);
        }

        /// <summary>
        /// Height of each history item in pixels
        /// </summary>
        [Category("Appearance")]
        [Description("Height of each notification item")]
        [DefaultValue(60)]
        public int ItemHeight
        {
            get => _itemHeight;
            set
            {
                _itemHeight = Math.Max(40, value);
                UpdateScrollBar();
                _listPanel.Invalidate();
            }
        }

        /// <summary>
        /// Number of items in history
        /// </summary>
        [Browsable(false)]
        public int Count => _history.Count;
        #endregion

        #region Public Methods
        /// <summary>
        /// Add a notification to history
        /// </summary>
        public void AddNotification(NotificationData data)
        {
            if (data == null)
                return;

            var item = new NotificationHistoryItem
            {
                Data = data,
                DismissedAt = DateTime.Now
            };

            _history.Insert(0, item); // Add to top

            // Trim history if needed
            while (_history.Count > _maxHistorySize)
            {
                _history.RemoveAt(_history.Count - 1);
            }

            UpdateScrollBar();
            _listPanel.Invalidate();
        }

        /// <summary>
        /// Clear all history
        /// </summary>
        public void ClearHistory()
        {
            _history.Clear();
            _scrollOffset = 0;
            UpdateScrollBar();
            _listPanel.Invalidate();
        }

        /// <summary>
        /// Get filtered history items
        /// </summary>
        public List<NotificationHistoryItem> GetFilteredItems()
        {
            var query = _history.AsEnumerable();

            // Apply type filter
            if (_filterType.HasValue)
            {
                query = query.Where(h => h.Data.Type == _filterType.Value);
            }

            // Apply search filter
            var searchText = _searchBox?.Text?.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(h =>
                    h.Data.Title?.ToLower().Contains(searchText) == true ||
                    h.Data.Message?.ToLower().Contains(searchText) == true);
            }

            return query.ToList();
        }
        #endregion

        #region Event Handlers
        private void SearchBox_TextChanged(object? sender, EventArgs e)
        {
            _scrollOffset = 0;
            UpdateScrollBar();
            _listPanel.Invalidate();
        }

        private void FilterCombo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            _filterType = _filterCombo.SelectedIndex switch
            {
                1 => NotificationType.Info,
                2 => NotificationType.Success,
                3 => NotificationType.Warning,
                4 => NotificationType.Error,
                5 => NotificationType.System,
                _ => null
            };

            _scrollOffset = 0;
            UpdateScrollBar();
            _listPanel.Invalidate();
        }

        private void ClearButton_Click(object? sender, EventArgs e)
        {
            ClearHistory();
        }

        private void ScrollBar_Scroll(object? sender, ScrollEventArgs e)
        {
            _scrollOffset = e.NewValue;
            _listPanel.Invalidate();
        }

        private void ListPanel_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (!_scrollBar.Visible)
                return;

            int scrollDelta = DpiScalingHelper.ScaleValue(20, this);
            int delta = e.Delta / 120 * scrollDelta;
            _scrollOffset = Math.Max(0, Math.Min(_scrollBar.Maximum - _scrollBar.LargeChange + 1, _scrollOffset - delta));
            _scrollBar.Value = _scrollOffset;
            _listPanel.Invalidate();
        }

        private void ListPanel_Click(object? sender, EventArgs e)
        {
            // Could implement click to view details or re-show notification
        }

        private void ListPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var items = GetFilteredItems();
            if (items.Count == 0)
            {
                DrawEmptyMessage(g);
                return;
            }

            int y = -_scrollOffset;
            foreach (var item in items)
            {
                if (y + _itemHeight < 0)
                {
                    y += _itemHeight;
                    continue;
                }

                if (y > _listPanel.Height)
                    break;

                DrawHistoryItem(g, item, new Rectangle(0, y, _listPanel.Width - (_scrollBar.Visible ? _scrollBar.Width : 0), _itemHeight));
                y += _itemHeight;
            }
        }
        #endregion

        #region Drawing Methods
        private void DrawHistoryItem(Graphics g, NotificationHistoryItem item, Rectangle bounds)
        {
            // Background
            var bgColor = item.Data.Type switch
            {
                NotificationType.Success => Color.FromArgb(20, 76, 175, 80),
                NotificationType.Warning => Color.FromArgb(20, 255, 152, 0),
                NotificationType.Error => Color.FromArgb(20, 244, 67, 54),
                NotificationType.Info => Color.FromArgb(20, 33, 150, 243),
                NotificationType.System => Color.FromArgb(20, 158, 158, 158),
                _ => Color.FromArgb(20, 128, 128, 128)
            };

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Border bottom
            using (var pen = new Pen(Color.FromArgb(30, 0, 0, 0)))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }

            int labelLeft = DpiScalingHelper.ScaleValue(12, this);
            int titleTop = DpiScalingHelper.ScaleValue(8, this);
            int titleW = bounds.Width - DpiScalingHelper.ScaleValue(100, this);
            int titleH = DpiScalingHelper.ScaleValue(16, this);
            int msgTop = DpiScalingHelper.ScaleValue(26, this);
            int msgH = DpiScalingHelper.ScaleValue(20, this);

            // Title
            Font titleFont = _textFont != null ? new Font(_textFont, FontStyle.Bold) : new Font("Segoe UI", 9, FontStyle.Bold);
            try
            {
                using (var titleBrush = new SolidBrush(this.ForeColor))
                {
                    var titleRect = new Rectangle(bounds.X + labelLeft, bounds.Y + titleTop, titleW, titleH);
                    g.DrawString(item.Data.Title ?? "Notification", titleFont, titleBrush, titleRect);
                }
            }
            finally
            {
                if (titleFont != _textFont) titleFont?.Dispose();
            }

            // Message
            Font messageFont = _textFont != null ? new Font(_textFont.FontFamily, Math.Max(7f, _textFont.Size - 2f)) : new Font("Segoe UI", 8);
            try
            {
                using (var messageBrush = new SolidBrush(Color.FromArgb(180, this.ForeColor)))
                {
                    var messageRect = new Rectangle(bounds.X + labelLeft, bounds.Y + msgTop, titleW, msgH);
                    g.DrawString(item.Data.Message ?? "", messageFont, messageBrush, messageRect);
                }
            }
            finally
            {
                if (messageFont != _textFont) messageFont?.Dispose();
            }

            // Timestamp
            var timeAgo = GetTimeAgo(item.DismissedAt);
            Font timeFont = _textFont != null ? new Font(_textFont.FontFamily, Math.Max(6f, _textFont.Size - 3f)) : new Font("Segoe UI", 7);
            try
            {
                using (var timeBrush = new SolidBrush(Color.FromArgb(120, this.ForeColor)))
                {
                    var timeSize = g.MeasureString(timeAgo, timeFont);
                    g.DrawString(timeAgo, timeFont, timeBrush, bounds.Right - timeSize.Width - labelLeft, bounds.Y + titleTop);
                }
            }
            finally
            {
                if (timeFont != _textFont) timeFont?.Dispose();
            }
        }

        private void DrawEmptyMessage(Graphics g)
        {
            var message = "No notifications in history";
            Font emptyFont = _textFont != null ? new Font(_textFont, FontStyle.Italic) : new Font("Segoe UI", 10, FontStyle.Italic);
            try
            {
            using (var brush = new SolidBrush(Color.FromArgb(120, this.ForeColor)))
            {
                var size = g.MeasureString(message, emptyFont);
                var x = (_listPanel.Width - size.Width) / 2;
                var y = (_listPanel.Height - size.Height) / 2;
                g.DrawString(message, emptyFont, brush, x, y);
            }
            }
            finally
            {
                if (emptyFont != _textFont) emptyFont?.Dispose();
            }
        }

        private string GetTimeAgo(DateTime time)
        {
            var span = DateTime.Now - time;

            if (span.TotalMinutes < 1)
                return "Just now";
            if (span.TotalMinutes < 60)
                return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24)
                return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays < 7)
                return $"{(int)span.TotalDays}d ago";

            return time.ToString("MMM d");
        }
        #endregion

        #region Helper Methods
        private void UpdateScrollBar()
        {
            var items = GetFilteredItems();
            int totalHeight = items.Count * _itemHeight;
            int visibleHeight = _listPanel.Height;

            if (totalHeight > visibleHeight)
            {
                _scrollBar.Visible = true;
                _scrollBar.Maximum = totalHeight;
                _scrollBar.LargeChange = visibleHeight;
                _scrollBar.SmallChange = _itemHeight;
            }
            else
            {
                _scrollBar.Visible = false;
                _scrollOffset = 0;
            }
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme != null)
                _textFont = BeepFontManager.ToFont(_currentTheme.BodyMedium);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _itemHeight = DpiScalingHelper.ScaleValue(60, this);
            BorderRadius = DpiScalingHelper.ScaleValue(8, this);
            ShadowOffset = DpiScalingHelper.ScaleValue(2, this);
            MinimumSize = DpiScalingHelper.ScaleSize(new Size(300, 400), this);
            Size = DpiScalingHelper.ScaleSize(new Size(350, 500), this);
            UpdateScrollBar();
            Invalidate();
        }

        /// <summary>
        /// Cleanup resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _textFont?.Dispose();
                _headerPanel?.Dispose();
                _listPanel?.Dispose();
                _scrollBar?.Dispose();
                _titleLabel?.Dispose();
                _searchBox?.Dispose();
                _filterCombo?.Dispose();
                _clearButton?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }

    /// <summary>
    /// Represents a notification in history
    /// </summary>
    public class NotificationHistoryItem
    {
        /// <summary>The notification data</summary>
        public NotificationData Data { get; set; } = new NotificationData();
        
        /// <summary>When the notification was dismissed</summary>
        public DateTime DismissedAt { get; set; }
        
        /// <summary>Was the notification acted upon</summary>
        public bool WasActioned { get; set; }
    }
}
