using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;
using ActivityItem = TheTechIdea.Beep.Winform.Controls.Widgets.Models.ActivityItem;
using ActivityType = TheTechIdea.Beep.Winform.Controls.Widgets.Models.ActivityType;
using BaseImage = TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Social
{
    /// <summary>
    /// ActivityStream - Activity/timeline display painter with enhanced functionality
    /// Displays user activities in a chronological timeline format
    /// </summary>
    internal sealed class ActivityStreamPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public ActivityStreamPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            // Base on Owner.DrawingRect to align with BaseControl's layout
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -4, -4);
            
            // Title area
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                ctx.HeaderRect = new Rectangle(
                    ctx.DrawingRect.Left + pad, 
                    ctx.DrawingRect.Top + pad, 
                    ctx.DrawingRect.Width - pad * 2, 
                    24
                );
            }
            
            // Activity content area with scrollable region
            int contentTop = ctx.HeaderRect.IsEmpty ? ctx.DrawingRect.Top + pad : ctx.HeaderRect.Bottom + 8;
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - contentTop - pad
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.ApplyThemeOnImage = true;

            // Draw title with activity indicator
            if (!string.IsNullOrEmpty(ctx.Title) && !ctx.HeaderRect.IsEmpty)
            {
                // Determine activities first so count can be shown in header
                var activitiesForHeader = ctx.ActivityItems;

                DrawActivityHeader(g, ctx.HeaderRect, ctx.Title, ctx.AccentColor, activitiesForHeader.Count);
            }
            
            // Draw activity items
            var activities = ctx.ActivityItems;
            if (activities == null || activities.Count == 0)
            {
                activities = CreateSampleActivities();
            }
            
            DrawActivityTimeline(g, ctx.ContentRect, activities);
        }

        private void DrawActivityHeader(Graphics g, Rectangle rect, string title, Color accentColor, int activityCount)
        {
            // Activity icon
            var iconRect = new Rectangle(rect.X, rect.Y + 2, 20, 20);
            _imagePainter.DrawSvg(g, "activity", iconRect, accentColor, 0.9f);

            // Title text
            var titleRect = new Rectangle(iconRect.Right + 8, rect.Y, rect.Width - iconRect.Width - 8, rect.Height);
            using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(title, titleFont, titleBrush, titleRect, format);

            // Activity count indicator
            if (activityCount > 0)
            {
                string countText = $"({activityCount})";
                using var countFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                using var countBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                var countSize = TextUtils.MeasureText(g,countText, countFont);
                g.DrawString(countText, countFont, countBrush, rect.Right - countSize.Width, rect.Y + 4);
            }
        }

        private void DrawActivityTimeline(Graphics g, Rectangle rect, List<ActivityItem> activities)
        {
            if (!activities.Any())
            {
                DrawEmptyState(g, rect);
                return;
            }

            int itemHeight = 50;
            int maxVisibleItems = Math.Max(1, rect.Height / itemHeight);
            int timelineX = rect.X + 30; // Space for timeline line

            // Draw timeline vertical line
            using var timelinePen = new Pen(Color.FromArgb(60, Color.Gray), 2);
            g.DrawLine(timelinePen, timelineX, rect.Y, timelineX, rect.Bottom);

            // Draw activity items
            for (int i = 0; i < Math.Min(activities.Count, maxVisibleItems); i++)
            {
                int y = rect.Y + i * itemHeight;
                var itemRect = new Rectangle(rect.X, y, rect.Width, itemHeight - 4);
                DrawActivityItem(g, itemRect, activities[i], timelineX);
            }

            // Draw "show more" indicator if there are more items
            if (activities.Count > maxVisibleItems)
            {
                var moreRect = new Rectangle(rect.X, rect.Bottom - 20, rect.Width, 20);
                using var moreFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Italic);
                using var moreBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString($"and {activities.Count - maxVisibleItems} more activities...", 
                    moreFont, moreBrush, moreRect, format);
            }
        }

        private void DrawActivityItem(Graphics g, Rectangle rect, ActivityItem activity, int timelineX)
        {
            // Timeline dot
            var dotRect = new Rectangle(timelineX - 6, rect.Y + 12, 12, 12);
            Color activityColor = GetActivityColor(activity.Type);
            using var dotBrush = new SolidBrush(activityColor);
            g.FillEllipse(dotBrush, dotRect);

            // Activity icon in dot
            var iconRect = new Rectangle(dotRect.X + 2, dotRect.Y + 2, 8, 8);
            _imagePainter.DrawSvg(g, GetActivityIcon(activity.Type), iconRect, Color.White, 0.8f);

            // Activity content area
            var contentRect = new Rectangle(timelineX + 16, rect.Y, rect.Width - (timelineX + 16 - rect.X), rect.Height);

            // User avatar (if available)
            if (!string.IsNullOrEmpty(activity.UserAvatar))
            {
                var avatarRect = new Rectangle(contentRect.X, contentRect.Y + 2, 24, 24);
                DrawUserAvatar(g, avatarRect, activity.UserAvatar, activity.UserName);
                contentRect = new Rectangle(avatarRect.Right + 8, contentRect.Y, 
                    contentRect.Width - avatarRect.Width - 8, contentRect.Height);
            }

            // Activity text
            using var activityFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var activityBrush = new SolidBrush(Color.FromArgb(160, Color.Black));
            string activityText = FormatActivityText(activity);
            g.DrawString(activityText, activityFont, activityBrush, contentRect.X, contentRect.Y + 2);

            // Timestamp
            using var timeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var timeBrush = new SolidBrush(Color.FromArgb(100, Color.Black));
            string timeText = FormatTimeAgo(activity.Timestamp);
            var timeSize = TextUtils.MeasureText(g,timeText, timeFont);
            g.DrawString(timeText, timeFont, timeBrush, 
                rect.Right - timeSize.Width, contentRect.Y + 2);

            // Activity details (if any)
            if (!string.IsNullOrEmpty(activity.Details))
            {
                using var detailFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var detailBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                g.DrawString(activity.Details, detailFont, detailBrush, 
                    contentRect.X, contentRect.Y + 18);
            }
        }

        private void DrawUserAvatar(Graphics g, Rectangle rect, string avatarPath, string userName)
        {
            // Draw avatar background
            using var avatarBrush = new SolidBrush(Color.FromArgb(220, 220, 220));
            g.FillEllipse(avatarBrush, rect);

            // Try to load and draw avatar image
            if (!string.IsNullOrEmpty(avatarPath))
            {
                try
                {
                    float radius = Math.Min(rect.Width, rect.Height) / 2f;
                    float cx = rect.X + rect.Width / 2f;
                    float cy = rect.Y + rect.Height / 2f;
                    StyledImagePainter.PaintInCircle(g, cx, cy, radius, avatarPath);
                }
                catch
                {
                    _imagePainter.ImagePath = avatarPath;
                    _imagePainter.ClipShape = TheTechIdea.Beep.Vis.Modules.ImageClipShape.Circle;
                    _imagePainter.DrawImage(g, avatarPath, rect);
                }
            }
            else
            {
                // Draw initials as fallback
                string initials = GetUserInitials(userName);
                using var initialFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
                using var initialBrush = new SolidBrush(Color.FromArgb(100, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(initials, initialFont, initialBrush, rect, format);
            }
        }

        private void DrawEmptyState(Graphics g, Rectangle rect)
        {
            // Empty state illustration
            var illustrationRect = new Rectangle(
                rect.X + rect.Width / 2 - 32, 
                rect.Y + rect.Height / 2 - 40, 
                64, 64
            );
            _imagePainter.DrawSvg(g, "activity-empty", illustrationRect, Color.FromArgb(150, Color.Gray), 0.6f);

            // Empty state text
            using var emptyFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
            using var emptyBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            var textRect = new Rectangle(rect.X, illustrationRect.Bottom + 16, rect.Width, 40);
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("No recent activity", emptyFont, emptyBrush, textRect, format);

            using var subFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var subBrush = new SolidBrush(Color.FromArgb(100, Color.Black));
            var subTextRect = new Rectangle(rect.X, textRect.Bottom, rect.Width, 20);
            g.DrawString("Activities will appear here when users interact with the system", 
                subFont, subBrush, subTextRect, format);
        }

        private List<ActivityItem> CreateSampleActivities()
        {
            return new List<ActivityItem>
            {
                new ActivityItem 
                { 
                    Type = ActivityType.Login, 
                    UserName = "John Doe", 
                    Action = "logged in",
                    Timestamp = DateTime.Now.AddMinutes(-5),
                    Details = "from Windows desktop"
                },
                new ActivityItem 
                { 
                    Type = ActivityType.Create, 
                    UserName = "Jane Smith", 
                    Action = "created new report",
                    Target = "Sales Analysis Q4",
                    Timestamp = DateTime.Now.AddMinutes(-15)
                },
                new ActivityItem 
                { 
                    Type = ActivityType.Update, 
                    UserName = "Mike Johnson", 
                    Action = "updated dashboard",
                    Target = "Marketing Metrics",
                    Timestamp = DateTime.Now.AddHours(-2)
                }
            };
        }

        private Color GetActivityColor(ActivityType type)
        {
            return type switch
            {
                ActivityType.Login => Color.FromArgb(76, 175, 80),
                ActivityType.Logout => Color.FromArgb(156, 39, 176),
                ActivityType.Create => Color.FromArgb(33, 150, 243),
                ActivityType.Update => Color.FromArgb(255, 152, 0),
                ActivityType.Delete => Color.FromArgb(244, 67, 54),
                ActivityType.View => Color.FromArgb(96, 125, 139),
                _ => Color.FromArgb(158, 158, 158)
            };
        }

        private string GetActivityIcon(ActivityType type)
        {
            return type switch
            {
                ActivityType.Login => "login",
                ActivityType.Logout => "logout", 
                ActivityType.Create => "plus",
                ActivityType.Update => "edit",
                ActivityType.Delete => "trash",
                ActivityType.View => "eye",
                _ => "activity"
            };
        }

        private string FormatActivityText(ActivityItem activity)
        {
            string text = $"{activity.UserName} {activity.Action}";
            if (!string.IsNullOrEmpty(activity.Target))
                text += $" \"{activity.Target}\"";
            return text;
        }

        private string FormatTimeAgo(DateTime timestamp)
        {
            var timeSpan = DateTime.Now - timestamp;
            
            if (timeSpan.TotalMinutes < 1)
                return "just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes}m ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours}h ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays}d ago";
            
            return timestamp.ToString("MMM dd");
        }

        private string GetUserInitials(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return "?";
            
            var parts = userName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return userName.Substring(0, Math.Min(2, userName.Length)).ToUpper();
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw activity status indicator
            var activities = ctx.ActivityItems;
            
            if (activities.Any(a => (DateTime.Now - a.Timestamp).TotalMinutes < 5))
            {
                // Draw "live" indicator for recent activity
                var liveRect = new Rectangle(ctx.DrawingRect.Right - 40, ctx.DrawingRect.Top + 8, 32, 12);
                using var liveBrush = new SolidBrush(Color.FromArgb(100, Color.Green));
                using var livePath = CreateRoundedPath(liveRect, 6);
                g.FillPath(liveBrush, livePath);

                using var liveFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Bold);
                using var liveTextBrush = new SolidBrush(Color.White);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("LIVE", liveFont, liveTextBrush, liveRect, format);
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }

    // ActivityItem and ActivityType are now in Widgets.Models namespace
}