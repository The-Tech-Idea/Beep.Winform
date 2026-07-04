using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    /// <summary>
    /// Grouped notification control that displays multiple similar notifications as a stack
    /// Shows count badge and expands to show individual notifications
    /// </summary>
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Grouped notification control with expand/collapse")]
    public class BeepNotificationGroup : BaseControl
    {
        // Phase 3 — child controls replace all manual paint. The header strip
        // is a BeepPanel holding three BeepLabels; per-item rows are BeepPanels
        // inside _itemsHost (see RebuildItems). No DrawHeader / DrawBadge /
        // DrawExpandedContent / DrawContent remain.
        private BeepPanel _headerPanel;
        private BeepLabel _titleLabel;
        private BeepLabel _countBadgeLabel;
        private BeepLabel _expandHintLabel;

        // Item host — Dock=Fill below the header. Each item is its own
        // BeepPanel (Dock=Top) holding BeepLabel children (title / message /
        // timestamp + type-tinted background).
        private BeepPanel _itemsHost;
        private readonly List<BeepPanel> _itemPanels = new();
        #region Private Fields
        private readonly List<NotificationData> _notifications;
        private bool _isExpanded = false;
        private string _groupKey = string.Empty;
        private string _groupTitle = "Notifications";
        private NotificationType _groupType;
        // Phase 3: header chrome is composed of child controls; these rect
        // fields are now dead and only kept for reference. No active code
        // writes to or reads from them anymore.
        // private Rectangle _headerRect;
        // private Rectangle _badgeRect;
        // private Rectangle _expandButtonRect;
        // private Rectangle _contentRect;
        private int _minTouchTargetWidth = 44;
        private bool _popupOpen => _isExpanded;

        private int HeaderHeight => DpiScalingHelper.ScaleValue(60, this);
        private int ItemHeight => DpiScalingHelper.ScaleValue(40, this);
        private int BadgeSize => DpiScalingHelper.ScaleValue(24, this);
        private int PaddingValue => DpiScalingHelper.ScaleValue(12, this);
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of BeepNotificationGroup
        /// </summary>
        public BeepNotificationGroup()
        {
            _notifications = new List<NotificationData>();

            // BaseControl configuration
            ControlStyle = BeepControlStyle.Material3;
            IsRounded = true;
            BorderRadius = DpiScalingHelper.ScaleValue(8, this);
            ShowShadow = true;
            ShadowOffset = DpiScalingHelper.ScaleValue(4, this);
            ShowAllBorders = true;
            BorderThickness = 1;
            MinimumSize = new Size(DpiScalingHelper.ScaleValue(280, this), DpiScalingHelper.ScaleValue(60, this));
            MaximumSize = new Size(DpiScalingHelper.ScaleValue(420, this), DpiScalingHelper.ScaleValue(600, this));
            Size = new Size(DpiScalingHelper.ScaleValue(350, this), DpiScalingHelper.ScaleValue(60, this));

            TabStop = true;
            base.AccessibleRole = AccessibleRole.Grouping;

            // Mouse events
            MouseClick += BeepNotificationGroup_MouseClick;

            // Phase 3 / header chrome → child controls. The header is a
            // BeepPanel host (Dock=Top) carrying a BeepLabel title + count
            // badge + chevron hint. Items stack as their own BeepPanel rows
            // inside _itemsHost (see RebuildItems).
            // ── Header panel (Dock=Top) with title + count + chevron
            _headerPanel = new BeepPanel
            {
                Dock = DockStyle.Top, Height = HeaderHeight, UseThemeColors = true,
                Padding = new Padding(DpiScalingHelper.ScaleValue(12,this), DpiScalingHelper.ScaleValue(8,this), DpiScalingHelper.ScaleValue(12,this), DpiScalingHelper.ScaleValue(8,this))
            };
            _titleLabel        = new BeepLabel { Dock = DockStyle.Fill,  AutoEllipsis = true, UseThemeColors = true, Text = "Notifications" };
            _expandHintLabel   = new BeepLabel { Dock = DockStyle.Right, Width = DpiScalingHelper.ScaleValue(16,this), TextAlign = ContentAlignment.MiddleCenter, Text = "\u25BE", UseThemeColors = true };
            _countBadgeLabel   = new BeepLabel { Dock = DockStyle.Right, Width = DpiScalingHelper.ScaleValue(28,this), TextAlign = ContentAlignment.MiddleCenter, UseThemeColors = true };
            _headerPanel.Controls.Add(_titleLabel);
            _headerPanel.Controls.Add(_countBadgeLabel);
            _headerPanel.Controls.Add(_expandHintLabel);
            this.Controls.Add(_headerPanel);

            // ── Items host (Dock=Fill below header)
            _itemsHost = new BeepPanel { Dock = DockStyle.Fill, BackColor = Color.Transparent, UseThemeColors = true, Visible = false };
            this.Controls.Add(_itemsHost);
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Group key for identifying similar notifications
        /// </summary>
        [Category("Data")]
        [Description("Unique key for grouping notifications")]
        public string GroupKey
        {
            get => _groupKey;
            set
            {
                _groupKey = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Display title for the group
        /// </summary>
        [Category("Appearance")]
        [Description("Title displayed for the notification group")]
        public string GroupTitle
        {
            get => _groupTitle;
            set
            {
                _groupTitle = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Type of notifications in this group
        /// </summary>
        [Category("Appearance")]
        [Description("Notification type for the group")]
        public NotificationType GroupType
        {
            get => _groupType;
            set
            {
                _groupType = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Whether the group is expanded to show individual notifications
        /// </summary>
        [Browsable(false)]
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    if (value) OnPopupOpened(EventArgs.Empty);
                    else OnPopupClosed(EventArgs.Empty);
                    UpdateSize();
                    UpdateHeaderText();    // Phase 3: flip the chevron hint glyph
                    UpdateAccessibility();
                    UpdateToolTip();
                    RebuildItems();        // Phase 3: itemsHost visible/height toggles
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Number of notifications in the group
        /// </summary>
        [Browsable(false)]
        public int Count => _notifications.Count;

        /// <summary>
        /// All notifications in the group
        /// </summary>
        [Browsable(false)]
        public IReadOnlyList<NotificationData> Notifications => _notifications.AsReadOnly();

        /// <summary>
        /// Minimum touch target width in pixels
        /// </summary>
        [Category("Layout")]
        [Description("Minimum touch target width in pixels")]
        [DefaultValue(44)]
        public int MinTouchTargetWidth
        {
            get => _minTouchTargetWidth;
            set { _minTouchTargetWidth = Math.Max(32, value); Invalidate(); }
        }

        /// <summary>
        /// Whether the group popup is currently open
        /// </summary>
        [Browsable(false)]
        public bool IsPopupOpen => _popupOpen;
        #endregion

        #region Public Methods
        /// <summary>
        /// Add a notification to the group
        /// </summary>
        public void AddNotification(NotificationData data)
        {
            if (data == null)
                return;

            _notifications.Add(data);

            // Update group properties from first notification
            if (_notifications.Count == 1)
            {
                _groupType = data.Type;
                _groupTitle = data.Source ?? data.Title ?? "Notifications";
            }

            UpdateSize();
            UpdateHeaderText();    // Phase 3: refresh count badge + title
            UpdateAccessibility();
            UpdateToolTip();
            RebuildItems();        // Phase 3: rebuild item rows (add new one)
            Invalidate();
        }

        /// <summary>
        /// Remove a notification from the group
        /// </summary>
        public void RemoveNotification(string notificationId)
        {
            var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification != null)
            {
                _notifications.Remove(notification);
                UpdateSize();
                UpdateHeaderText();
                UpdateAccessibility();
                UpdateToolTip();
                RebuildItems();        // Phase 3: rebuild item rows (drop last)
                Invalidate();
            }
        }

        /// <summary>
        /// Clear all notifications from the group
        /// </summary>
        public void Clear()
        {
            _notifications.Clear();
            _isExpanded = false;
            UpdateSize();
            UpdateHeaderText();
            UpdateAccessibility();
            UpdateToolTip();
            RebuildItems();        // Phase 3: empty the items host
            Invalidate();
        }

        /// <summary>
        /// Toggle expand/collapse state
        /// </summary>
        public void ToggleExpand()
        {
            IsExpanded = !IsExpanded;
        }

        /// <summary>
        /// Close the child popup (collapse the group)
        /// </summary>
        public void CloseChildPopup()
        {
            if (_popupOpen)
            {
                IsExpanded = false;
                OnPopupClosed(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raise PopupOpened event
        /// </summary>
        protected virtual void OnPopupOpened(EventArgs e) => PopupOpened?.Invoke(this, e);

        /// <summary>
        /// Raise PopupClosed event
        /// </summary>
        protected virtual void OnPopupClosed(EventArgs e) => PopupClosed?.Invoke(this, e);
        #endregion

        #region Events
        /// <summary>
        /// Raised when a notification in the group is clicked
        /// </summary>
        public event EventHandler<NotificationEventArgs>? NotificationClicked;

        /// <summary>
        /// Raised when the group is dismissed
        /// </summary>
        public event EventHandler<NotificationEventArgs>? GroupDismissed;

        /// <summary>
        /// Raised when the group popup is opened
        /// </summary>
        public event EventHandler? PopupOpened;

        /// <summary>
        /// Raised when the group popup is closed
        /// </summary>
        public event EventHandler? PopupClosed;
        #endregion

        #region Private Methods
        private void UpdateSize()
        {
            int newHeight = HeaderHeight;

            if (_isExpanded && _notifications.Count > 0)
            {
                newHeight += _notifications.Count * ItemHeight + PaddingValue;
            }

            this.Height = Math.Min(newHeight, MaximumSize.Height);
        }

        /// <summary>
        /// Pushes the current <see cref="_groupType"/>, <see cref="_groupTitle"/>,
        /// count, and expand state into the header child labels.
        /// </summary>
        private void UpdateHeaderText()
        {
            if (_titleLabel == null || _countBadgeLabel == null || _expandHintLabel == null) return;

            // Phase 3 / G26: if notifications have different types, show
            // "Mixed" instead of the dominant type's name.
            var titles = _notifications
                .Where(n => n != null)
                .Select(n => string.IsNullOrEmpty(n.Title) ? null : n.Title)
                .ToList();
            var distinctTitles = titles.Where(t => t != null).Distinct().ToList();
            string resolvedTitle;
            if (distinctTitles.Count == 1) resolvedTitle = distinctTitles[0];
            else if (distinctTitles.Count == 0) resolvedTitle = _groupTitle ?? "Notifications";
            else resolvedTitle = distinctTitles.Count + " notification types";     // mixed

            _titleLabel.Text = resolvedTitle;

            // Count badge text — cap at "99+" so the badge stays small.
            var n = _notifications.Count;
            _countBadgeLabel.Text = n > 99 ? "99+" : n.ToString();

            _expandHintLabel.Text = _isExpanded ? "\u25C0" : "\u25B6";      // ◀ / ▶
        }
        #endregion

        /// <summary>
        /// (Re)populate <c>_itemsHost</c> with one BeepPanel per notification.
        /// Each row is Dock=Top, Height=ItemHeight, type-tinted background
        /// + three BeepLabel children (title, message, right-aligned timestamp).
        /// Called from <see cref="IsExpanded"/> setter, <see cref="AddNotification"/>,
        /// <see cref="RemoveNotification"/>, and <see cref="Clear"/>.
        /// </summary>
        private void RebuildItems()
        {
            if (_itemsHost == null) return;

            bool wantExpanded = _isExpanded && _notifications.Count > 0;

            _itemsHost.SuspendLayout();
            try
            {
                // Trim rows that no longer match the live notification list.
                while (_itemPanels.Count > _notifications.Count)
                {
                    int last = _itemPanels.Count - 1;
                    if (last >= 0 && last < _itemsHost.Controls.Count)
                    {
                        var stale = _itemPanels[last];
                        _itemsHost.Controls.RemoveAt(last);
                        stale.Dispose();
                    }
                    _itemPanels.RemoveAt(last);
                }

                // Update existing rows; create new ones as needed.
                for (int i = 0; i < _notifications.Count; i++)
                {
                    var notification = _notifications[i];
                    if (i < _itemPanels.Count)
                    {
                        UpdateItemRow(_itemPanels[i], notification);
                    }
                    else
                    {
                        var panel = CreateItemRow(notification);
                        _itemsHost.Controls.Add(panel);
                        _itemPanels.Add(panel);
                    }
                }
            }
            finally
            {
                _itemsHost.ResumeLayout();
            }

            _itemsHost.Visible = wantExpanded;
            int totalRows = _itemPanels.Count * ItemHeight + PaddingValue;
            _itemsHost.Height = wantExpanded ? totalRows : 0;
        }

        /// <summary>
        /// Build a single item row: outer BeepPanel (type-tinted) with three
        /// inner BeepLabel children (timestamp top-right, title, message).
        /// </summary>
        private BeepPanel CreateItemRow(NotificationData notification)
        {
            var row = new BeepPanel
            {
                Dock = DockStyle.Top,
                Height = ItemHeight,
                UseThemeColors = false,        // explicit BackColor below
                Padding = new Padding(
                    DpiScalingHelper.ScaleValue(8, this),
                    DpiScalingHelper.ScaleValue(4, this),
                    DpiScalingHelper.ScaleValue(8, this),
                    DpiScalingHelper.ScaleValue(4, this)),
                Cursor = Cursors.Hand
            };
            row.BackColor = ResolveTypeTint(notification.Type, BeepThemesManager.CurrentTheme);

            // Row click → NotificationClicked for this notification. Captured
            // via a local copy so the closure binds the right instance.
            var capture = notification;
            row.Click += (s, e) => NotificationClicked?.Invoke(this,
                new NotificationEventArgs { Notification = capture });

            var inner = new BeepPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            row.Controls.Add(inner);

            // Timestamp: top, single line, right-aligned
            inner.Controls.Add(new BeepLabel
            {
                Dock = DockStyle.Top,
                Height = DpiScalingHelper.ScaleValue(14, this),
                TextAlign = ContentAlignment.TopRight,
                AutoSize = false,
                AutoEllipsis = true,
                UseThemeColors = true,
                ForeColor = Color.FromArgb(100, this.ForeColor),
                TabStop = false,
                Tag = "_time"
            });
            // Title: top, single line, bold
            inner.Controls.Add(new BeepLabel
            {
                Dock = DockStyle.Top,
                Height = DpiScalingHelper.ScaleValue(18, this),
                AutoEllipsis = true,
                UseThemeColors = true,
                TabStop = false,
                Tag = "_title"
            });
            // Message: fill remaining — multi-line truncated
            inner.Controls.Add(new BeepLabel
            {
                Dock = DockStyle.Fill,
                AutoEllipsis = true,
                UseThemeColors = true,
                ForeColor = Color.FromArgb(160, this.ForeColor),
                TabStop = false,
                Tag = "_msg"
            });

            UpdateItemRow(row, notification);
            return row;
        }

        /// <summary>
        /// Re-apply text + colors to an existing row without rebuilding it.
        /// </summary>
        private void UpdateItemRow(BeepPanel row, NotificationData notification)
        {
            row.BackColor = ResolveTypeTint(notification.Type, BeepThemesManager.CurrentTheme);

            BeepLabel title = null, msg = null, ts = null;
            foreach (Control c in row.Controls)
            {
                if (c is BeepPanel inner)
                {
                    foreach (Control ic in inner.Controls)
                    {
                        if (ic.Tag is string tag)
                        {
                            if (tag == "_title") title = (BeepLabel)ic;
                            else if (tag == "_msg") msg = (BeepLabel)ic;
                            else if (tag == "_time") ts = (BeepLabel)ic;
                        }
                    }
                }
            }

            if (title != null) title.Text = notification.Title ?? string.Empty;
            if (msg   != null) msg.Text   = notification.Message ?? string.Empty;
            if (ts    != null) ts.Text    = GetTimeAgo(notification.Timestamp);
        }

        /// <summary>
        /// Returns the tinted background colour for a notification type, sourced
        /// from <see cref="NotificationThemeHelpers"/>. 12% alpha of the type's
        /// accent colour — soft highlight without overwhelming the chrome.
        /// </summary>
        private Color ResolveTypeTint(NotificationType type, IBeepTheme theme)
        {
            var colors = NotificationThemeHelpers.GetColorsForType(
                type, theme, null, null, null, null);

            int a = (int)(colors.IconColor.A * 0.12f);
            return Color.FromArgb(
                Math.Max(8, Math.Min(64, a)),
                colors.IconColor);
        }

        private string GetTimeAgo(DateTime time)
        {
            var span = DateTime.Now - time;

            if (span.TotalMinutes < 1)
                return "now";
            if (span.TotalMinutes < 60)
                return $"{(int)span.TotalMinutes}m";
            if (span.TotalHours < 24)
                return $"{(int)span.TotalHours}h";

            return $"{(int)span.TotalDays}d";
        }

        #region Event Handlers
        private void BeepNotificationGroup_MouseClick(object? sender, MouseEventArgs e)
        {
            // Phase 3 final: item rows own their own Click handlers (per
            // CreateItemRow above), so this form-level handler only fires for
            // clicks on the header strip / chevron / empty space — those all
            // mean the same thing: toggle expand / collapse.
            ToggleExpand();
        }

        /// <summary>
        /// Handle size changes
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            UpdateHeaderText();    // Phase 3: theme-coloured text on the labels
            UpdateAccessibility();
            UpdateToolTip();
            RebuildItems();        // Phase 3: re-tint item rows with new theme
            Invalidate();
        }

        /// <summary>
        /// Phase 3 / accessibility — refresh <see cref="AccessibleName"/> and
        /// <see cref="AccessibleDescription"/> from the current group state so
        /// screen readers announce the group's title + count + expand state.
        /// Called from <c>ApplyTheme</c> and any state mutation that changes the
        /// count or expand flag.
        /// </summary>
        private void UpdateAccessibility()
        {
            // AccessibleName: prefer the typed title text resolved by
            // UpdateHeaderText; fall back to a generic label.
            var name = _titleLabel?.Text;
            if (string.IsNullOrEmpty(name))
                name = _notifications.Count == 0 ? "Notification group" : $"Notifications ({_notifications.Count})";

            AccessibleName = name;
            AccessibleDescription = _isExpanded
                ? "Expanded; press Enter to collapse."
                : $"Collapsed; press Enter to expand. {_notifications.Count} notification(s).";
        }

        /// <summary>
        /// Phase 3 / tooltip — every Beep control surfaces <see cref="BaseControl.ToolTipText"/>
        /// through the central ToolTipManager. We just set the property on
        /// <c>this</c>; the centralized manager picks it up. Tooltip text
        /// describes how to interact (toggle / collapse) and current count.
        /// </summary>
        private void UpdateToolTip()
        {
            // BaseControl.ToolTipText on `this` (a BaseControl) is the right hook.
            ToolTipText = _isExpanded
                ? $"Notification group (expanded, {_notifications.Count} items). Press Enter to collapse."
                : $"Notification group ({_notifications.Count} items). Press Enter to expand.";
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            BorderRadius = DpiScalingHelper.ScaleValue(8, this);
            ShadowOffset = DpiScalingHelper.ScaleValue(4, this);
            MinimumSize = DpiScalingHelper.ScaleSize(new Size(280, 60), this);
            MaximumSize = DpiScalingHelper.ScaleSize(new Size(420, 600), this);
            Invalidate();
        }

        /// <summary>
        /// Cleanup resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _notifications.Clear();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
