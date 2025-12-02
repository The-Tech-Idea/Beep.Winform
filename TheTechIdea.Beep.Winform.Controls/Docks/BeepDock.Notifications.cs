using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Docks;


namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDock - Badge and Notification System
    /// </summary>
    public partial class BeepDock
    {
        private readonly Dictionary<string, DockItemNotification> _notifications = new Dictionary<string, DockItemNotification>();
        private readonly Dictionary<string, DockItemProgress> _progressIndicators = new Dictionary<string, DockItemProgress>();

        /// <summary>
        /// Sets a badge count for an item
        /// </summary>
        public void SetBadgeCount(SimpleItem item, int count)
        {
            if (item == null) return;

            var state = _itemStates.FirstOrDefault(s => s.Item == item);
            if (state != null)
            {
                state.BadgeCount = Math.Max(0, count);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets the badge count for an item
        /// </summary>
        public int GetBadgeCount(SimpleItem item)
        {
            if (item == null) return 0;

            var state = _itemStates.FirstOrDefault(s => s.Item == item);
            return state?.BadgeCount ?? 0;
        }

        /// <summary>
        /// Sets a notification for an item
        /// </summary>
        public void SetNotification(SimpleItem item, string title, string message, NotificationType type = NotificationType.Info)
        {
            if (item == null) return;

            var notification = new DockItemNotification
            {
                Title = title,
                Message = message,
                Type = type,
                Timestamp = DateTime.Now
            };

            _notifications[item.Text] = notification;

            // Also set badge count
            SetBadgeCount(item, GetBadgeCount(item) + 1);

            // Trigger attention animation
            TriggerAttentionAnimation(item);

            OnNotificationAdded(item, notification);
        }

        /// <summary>
        /// Clears notification for an item
        /// </summary>
        public void ClearNotification(SimpleItem item)
        {
            if (item == null) return;

            _notifications.Remove(item.Text);
            SetBadgeCount(item, 0);

            Invalidate();
        }

        /// <summary>
        /// Gets notification for an item
        /// </summary>
        public DockItemNotification GetNotification(SimpleItem item)
        {
            if (item == null) return null;

            _notifications.TryGetValue(item.Text, out var notification);
            return notification;
        }

        /// <summary>
        /// Sets a progress indicator for an item
        /// </summary>
        public void SetProgress(SimpleItem item, float percentage, string status = null)
        {
            if (item == null) return;

            percentage = Math.Max(0, Math.Min(100, percentage));

            var progress = new DockItemProgress
            {
                Percentage = percentage,
                Status = status,
                IsIndeterminate = false
            };

            _progressIndicators[item.Text] = progress;
            Invalidate();

            OnProgressUpdated(item, percentage);
        }

        /// <summary>
        /// Sets an indeterminate progress indicator
        /// </summary>
        public void SetIndeterminateProgress(SimpleItem item, string status = null)
        {
            if (item == null) return;

            var progress = new DockItemProgress
            {
                Percentage = 0,
                Status = status,
                IsIndeterminate = true
            };

            _progressIndicators[item.Text] = progress;
            Invalidate();
        }

        /// <summary>
        /// Clears progress indicator for an item
        /// </summary>
        public void ClearProgress(SimpleItem item)
        {
            if (item == null) return;

            _progressIndicators.Remove(item.Text);
            Invalidate();
        }

        /// <summary>
        /// Gets progress indicator for an item
        /// </summary>
        public DockItemProgress GetProgress(SimpleItem item)
        {
            if (item == null) return null;

            _progressIndicators.TryGetValue(item.Text, out var progress);
            return progress;
        }

        /// <summary>
        /// Triggers an attention animation for an item (bounce, pulse, etc.)
        /// </summary>
        public void TriggerAttentionAnimation(SimpleItem item, int bounceCount = 3)
        {
            if (item == null) return;

            var state = _itemStates.FirstOrDefault(s => s.Item == item);
            if (state != null)
            {
                // Start bounce animation
                StartBounceAnimation(state, bounceCount);
            }
        }

        private void StartBounceAnimation(DockItemState state, int bounceCount)
        {
            // Animation will be handled by the animation timer
            // Set a flag or counter in the state
            state.CurrentRotation = 0;
            // Could add animation queue here
        }

        /// <summary>
        /// Paints badge for an item
        /// </summary>
        private void PaintBadge(Graphics g, DockItemState itemState, Rectangle itemBounds)
        {
            if (!_config.ShowBadges || itemState.BadgeCount <= 0)
                return;

            string text = itemState.BadgeCount > 99 ? "99+" : itemState.BadgeCount.ToString();

            int badgeHeight = Math.Max(18, itemBounds.Height / 4);
            int minWidth = badgeHeight;

            using (var font = new Font("Segoe UI", badgeHeight * 0.55f, FontStyle.Bold))
            {
                var textSize = g.MeasureString(text, font);
                int badgeWidth = Math.Max(minWidth, (int)textSize.Width + 8);

                var badgeBounds = new Rectangle(
                    itemBounds.Right - badgeWidth / 2,
                    itemBounds.Top - badgeHeight / 2,
                    badgeWidth,
                    badgeHeight
                );

                // Check if there's a notification to determine badge color
                var notification = _notifications.Values.FirstOrDefault();
                Color badgeColor = GetBadgeColor(notification?.Type ?? NotificationType.Info);

                using (var path = CreateRoundedRectPath(badgeBounds, badgeHeight / 2))
                {
                    // Badge background
                    using (var brush = new SolidBrush(badgeColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Badge border
                    using (var pen = new Pen(Color.White, 2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }

                // Badge text
                using (var textBrush = new SolidBrush(Color.White))
                {
                    var textFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(text, font, textBrush, badgeBounds, textFormat);
                }
            }
        }

        /// <summary>
        /// Paints progress indicator for an item
        /// </summary>
        private void PaintProgressIndicator(Graphics g, DockItemState itemState, Rectangle itemBounds)
        {
            if (!_progressIndicators.TryGetValue(itemState.Item.Text, out var progress))
                return;

            int thickness = 3;
            var progressBounds = new Rectangle(
                itemBounds.X - thickness / 2,
                itemBounds.Y - thickness / 2,
                itemBounds.Width + thickness,
                itemBounds.Height + thickness
            );

            var progressColor = _currentTheme?.AccentColor ?? Color.FromArgb(0, 122, 255);

            if (progress.IsIndeterminate)
            {
                // Indeterminate progress - spinning arc
                PaintIndeterminateProgress(g, progressBounds, thickness, progressColor);
            }
            else
            {
                // Determinate progress - partial circle
                PaintDeterminateProgress(g, progressBounds, thickness, progressColor, progress.Percentage);
            }
        }

        private void PaintDeterminateProgress(Graphics g, Rectangle bounds, int thickness, Color color, float percentage)
        {
            using (var pen = new Pen(Color.FromArgb(50, color), thickness))
            {
                g.DrawEllipse(pen, bounds);
            }

            if (percentage > 0)
            {
                float sweepAngle = (percentage / 100f) * 360f;

                using (var pen = new Pen(color, thickness))
                {
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    g.DrawArc(pen, bounds, -90, sweepAngle);
                }
            }
        }

        private void PaintIndeterminateProgress(Graphics g, Rectangle bounds, int thickness, Color color)
        {
            // Animated spinning arc
            float rotation = (Environment.TickCount % 2000) / 2000f * 360f;
            float sweepAngle = 90f;

            using (var pen = new Pen(color, thickness))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                g.DrawArc(pen, bounds, rotation - 90, sweepAngle);
            }

            // Keep animating
            Invalidate();
        }

        private Color GetBadgeColor(NotificationType type)
        {
            return type switch
            {
                NotificationType.Error => Color.FromArgb(255, 69, 58),
                NotificationType.Warning => Color.FromArgb(255, 159, 10),
                NotificationType.Success => Color.FromArgb(52, 199, 89),
                NotificationType.Info => Color.FromArgb(0, 122, 255),
                _ => Color.FromArgb(0, 122, 255)
            };
        }

        private System.Drawing.Drawing2D.GraphicsPath CreateRoundedRectPath(Rectangle bounds, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
            var arc = new Rectangle(bounds.X, bounds.Y, diameter, diameter);

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }

        #region Events

        /// <summary>
        /// Occurs when a notification is added
        /// </summary>
        public event EventHandler<DockNotificationEventArgs>? NotificationAdded;

        /// <summary>
        /// Occurs when progress is updated
        /// </summary>
        public event EventHandler<DockProgressEventArgs>? ProgressUpdated;

        protected virtual void OnNotificationAdded(SimpleItem item, DockItemNotification notification)
        {
            NotificationAdded?.Invoke(this, new DockNotificationEventArgs(item, notification));
        }

        protected virtual void OnProgressUpdated(SimpleItem item, float percentage)
        {
            ProgressUpdated?.Invoke(this, new DockProgressEventArgs(item, percentage));
        }

        #endregion
    }

    #region Notification Types

    /// <summary>
    /// Represents a notification for a dock item
    /// </summary>
    public class DockItemNotification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Represents a progress indicator for a dock item
    /// </summary>
    public class DockItemProgress
    {
        public float Percentage { get; set; }
        public string Status { get; set; }
        public bool IsIndeterminate { get; set; }
    }

    /// <summary>
    /// Notification types
    /// </summary>
    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }

    public class DockNotificationEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public DockItemNotification Notification { get; }

        public DockNotificationEventArgs(SimpleItem item, DockItemNotification notification)
        {
            Item = item;
            Notification = notification;
        }
    }

    public class DockProgressEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public float Percentage { get; }

        public DockProgressEventArgs(SimpleItem item, float percentage)
        {
            Item = item;
            Percentage = percentage;
        }
    }

    #endregion
}

