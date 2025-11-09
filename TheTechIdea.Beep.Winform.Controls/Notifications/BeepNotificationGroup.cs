using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

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
        #region Private Fields
        private readonly List<NotificationData> _notifications;
        private bool _isExpanded = false;
        private string _groupKey = string.Empty;
        private string _groupTitle = "Notifications";
        private NotificationType _groupType;
        private Rectangle _headerRect;
        private Rectangle _badgeRect;
        private Rectangle _expandButtonRect;
        private Rectangle _contentRect;
        private const int HEADER_HEIGHT = 60;
        private const int ITEM_HEIGHT = 40;
        private const int BADGE_SIZE = 24;
        private const int PADDING = 12;
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
            BorderRadius = 8;
            ShowShadow = true;
            ShadowOffset = 4;
            ShowAllBorders = true;
            BorderThickness = 1;
            MinimumSize = new Size(280, HEADER_HEIGHT);
            MaximumSize = new Size(420, 600);
            Size = new Size(350, HEADER_HEIGHT);

            // Mouse events
            MouseClick += BeepNotificationGroup_MouseClick;
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
                    UpdateSize();
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
            Invalidate();
        }

        /// <summary>
        /// Toggle expand/collapse state
        /// </summary>
        public void ToggleExpand()
        {
            IsExpanded = !IsExpanded;
        }
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
        #endregion

        #region Private Methods
        private void UpdateSize()
        {
            int newHeight = HEADER_HEIGHT;

            if (_isExpanded && _notifications.Count > 0)
            {
                newHeight += _notifications.Count * ITEM_HEIGHT + PADDING;
            }

            this.Height = Math.Min(newHeight, MaximumSize.Height);
        }

        private void RecalculateLayout()
        {
            var contentArea = DrawingRect;

            // Header area (always visible)
            _headerRect = new Rectangle(
                contentArea.X,
                contentArea.Y,
                contentArea.Width,
                HEADER_HEIGHT
            );

            // Badge (count indicator)
            _badgeRect = new Rectangle(
                _headerRect.Right - BADGE_SIZE - PADDING,
                _headerRect.Y + (_headerRect.Height - BADGE_SIZE) / 2,
                BADGE_SIZE,
                BADGE_SIZE
            );

            // Expand button
            _expandButtonRect = new Rectangle(
                _badgeRect.Left - 30,
                _headerRect.Y + (_headerRect.Height - 24) / 2,
                24,
                24
            );

            // Content area (visible when expanded)
            if (_isExpanded)
            {
                _contentRect = new Rectangle(
                    contentArea.X,
                    _headerRect.Bottom,
                    contentArea.Width,
                    contentArea.Height - HEADER_HEIGHT
                );
            }
            else
            {
                _contentRect = Rectangle.Empty;
            }
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Draw the grouped notification content
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            RecalculateLayout();

            // Draw header
            DrawHeader(g);

            // Draw expanded content if needed
            if (_isExpanded && _notifications.Count > 0)
            {
                DrawExpandedContent(g);
            }
        }

        private void DrawHeader(Graphics g)
        {
            // Background color based on type
            var (bgColor, borderColor, textColor) = GetColorsForType(_groupType);

            // Header background
            using (var brush = new SolidBrush(Color.FromArgb(30, bgColor)))
            {
                g.FillRectangle(brush, _headerRect);
            }

            // Group icon
            var iconPath = NotificationData.GetDefaultIconForType(_groupType);
            var iconRect = new Rectangle(_headerRect.X + PADDING, _headerRect.Y + 18, 24, 24);
            
            // Icon rendering - simple colored circle as fallback
            using (var iconBrush = new SolidBrush(bgColor))
            {
                g.FillEllipse(iconBrush, iconRect);
            }

            // Group title
            using (var titleFont = new Font("Segoe UI", 10, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(this.ForeColor))
            {
                var titleRect = new Rectangle(
                    iconRect.Right + 8,
                    _headerRect.Y + 12,
                    _expandButtonRect.Left - iconRect.Right - 16,
                    20
                );
                g.DrawString(_groupTitle ?? "Notifications", titleFont, titleBrush, titleRect);
            }

            // Subtitle (most recent message preview)
            if (_notifications.Count > 0)
            {
                var latestMessage = _notifications[_notifications.Count - 1].Message;
                using (var subtitleFont = new Font("Segoe UI", 8))
                using (var subtitleBrush = new SolidBrush(Color.FromArgb(160, this.ForeColor)))
                {
                    var subtitleRect = new Rectangle(
                        iconRect.Right + 8,
                        _headerRect.Y + 34,
                        _expandButtonRect.Left - iconRect.Right - 16,
                        16
                    );
                    
                    var format = new StringFormat
                    {
                        Trimming = StringTrimming.EllipsisCharacter,
                        FormatFlags = StringFormatFlags.NoWrap
                    };
                    
                    g.DrawString(latestMessage ?? "", subtitleFont, subtitleBrush, subtitleRect, format);
                }
            }

            // Expand/collapse button
            DrawExpandButton(g);

            // Count badge
            DrawBadge(g, bgColor);
        }

        private void DrawExpandButton(Graphics g)
        {
            var isHovering = _expandButtonRect.Contains(PointToClient(Cursor.Position));
            var buttonColor = isHovering ? Color.FromArgb(200, this.ForeColor) : Color.FromArgb(120, this.ForeColor);

            using (var pen = new Pen(buttonColor, 2))
            {
                // Draw chevron (down when collapsed, up when expanded)
                var centerX = _expandButtonRect.X + _expandButtonRect.Width / 2;
                var centerY = _expandButtonRect.Y + _expandButtonRect.Height / 2;

                if (_isExpanded)
                {
                    // Up chevron
                    g.DrawLine(pen, centerX - 6, centerY + 3, centerX, centerY - 3);
                    g.DrawLine(pen, centerX, centerY - 3, centerX + 6, centerY + 3);
                }
                else
                {
                    // Down chevron
                    g.DrawLine(pen, centerX - 6, centerY - 3, centerX, centerY + 3);
                    g.DrawLine(pen, centerX, centerY + 3, centerX + 6, centerY - 3);
                }
            }
        }

        private void DrawBadge(Graphics g, Color bgColor)
        {
            // Badge background
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillEllipse(brush, _badgeRect);
            }

            // Badge border
            using (var pen = new Pen(Color.White, 2))
            {
                g.DrawEllipse(pen, _badgeRect);
            }

            // Count text
            var countText = _notifications.Count > 99 ? "99+" : _notifications.Count.ToString();
            using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.White))
            {
                var size = g.MeasureString(countText, font);
                var x = _badgeRect.X + (_badgeRect.Width - size.Width) / 2;
                var y = _badgeRect.Y + (_badgeRect.Height - size.Height) / 2;
                g.DrawString(countText, font, brush, x, y);
            }
        }

        private void DrawExpandedContent(Graphics g)
        {
            if (_contentRect.IsEmpty)
                return;

            int y = _contentRect.Y + PADDING / 2;

            for (int i = 0; i < _notifications.Count && y < _contentRect.Bottom; i++)
            {
                var notification = _notifications[i];
                var itemRect = new Rectangle(_contentRect.X + PADDING, y, _contentRect.Width - PADDING * 2, ITEM_HEIGHT);

                DrawNotificationItem(g, notification, itemRect, i);
                y += ITEM_HEIGHT;
            }
        }

        private void DrawNotificationItem(Graphics g, NotificationData notification, Rectangle bounds, int index)
        {
            // Hover effect
            var mousePos = PointToClient(Cursor.Position);
            var isHovering = bounds.Contains(mousePos);

            if (isHovering)
            {
                using (var brush = new SolidBrush(Color.FromArgb(20, this.ForeColor)))
                {
                    g.FillRectangle(brush, bounds);
                }
            }

            // Title
            using (var font = new Font("Segoe UI", 8, FontStyle.Bold))
            using (var brush = new SolidBrush(this.ForeColor))
            {
                var textRect = new Rectangle(bounds.X + 4, bounds.Y + 4, bounds.Width - 60, 16);
                g.DrawString(notification.Title ?? "", font, brush, textRect, new StringFormat
                {
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                });
            }

            // Message
            using (var font = new Font("Segoe UI", 7))
            using (var brush = new SolidBrush(Color.FromArgb(160, this.ForeColor)))
            {
                var textRect = new Rectangle(bounds.X + 4, bounds.Y + 22, bounds.Width - 60, 14);
                g.DrawString(notification.Message ?? "", font, brush, textRect, new StringFormat
                {
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                });
            }

            // Timestamp
            var timeAgo = GetTimeAgo(notification.Timestamp);
            using (var font = new Font("Segoe UI", 7))
            using (var brush = new SolidBrush(Color.FromArgb(100, this.ForeColor)))
            {
                var size = g.MeasureString(timeAgo, font);
                g.DrawString(timeAgo, font, brush, bounds.Right - size.Width - 4, bounds.Y + 4);
            }

            // Separator line
            using (var pen = new Pen(Color.FromArgb(30, this.ForeColor)))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }
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

        private (Color bgColor, Color borderColor, Color textColor) GetColorsForType(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => (Color.FromArgb(76, 175, 80), Color.FromArgb(56, 142, 60), Color.White),
                NotificationType.Warning => (Color.FromArgb(255, 152, 0), Color.FromArgb(245, 124, 0), Color.White),
                NotificationType.Error => (Color.FromArgb(244, 67, 54), Color.FromArgb(211, 47, 47), Color.White),
                NotificationType.Info => (Color.FromArgb(33, 150, 243), Color.FromArgb(25, 118, 210), Color.White),
                NotificationType.System => (Color.FromArgb(158, 158, 158), Color.FromArgb(117, 117, 117), Color.White),
                _ => (Color.FromArgb(128, 128, 128), Color.FromArgb(96, 96, 96), Color.White)
            };
        }
        #endregion

        #region Event Handlers
        private void BeepNotificationGroup_MouseClick(object? sender, MouseEventArgs e)
        {
            // Check if expand button clicked
            if (_expandButtonRect.Contains(e.Location))
            {
                ToggleExpand();
                return;
            }

            // Check if a notification item clicked (when expanded)
            if (_isExpanded && _contentRect.Contains(e.Location))
            {
                int itemIndex = (e.Y - _contentRect.Y - PADDING / 2) / ITEM_HEIGHT;
                if (itemIndex >= 0 && itemIndex < _notifications.Count)
                {
                    var notification = _notifications[itemIndex];
                    NotificationClicked?.Invoke(this, new NotificationEventArgs
                    {
                        Notification = notification
                    });
                }
                return;
            }

            // Header clicked - toggle expand
            if (_headerRect.Contains(e.Location))
            {
                ToggleExpand();
            }
        }

        /// <summary>
        /// Handle size changes
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RecalculateLayout();
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
