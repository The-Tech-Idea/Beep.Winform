using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Abstract base class for notification painters
    /// Provides common painting functionality and delegates to specific implementations
    /// </summary>
    public abstract class NotificationPainterBase : INotificationPainter
    {
        /// <summary>
        /// Gets colors for notification type using NotificationThemeHelpers
        /// </summary>
        protected (Color BackColor, Color ForeColor, Color BorderColor, Color IconColor) GetColorsForType(
            NotificationType type,
            NotificationRenderOptions options)
        {
            return NotificationThemeHelpers.GetColorsForType(
                type,
                null, // Theme can be passed if needed
                options.CustomBackColor,
                options.CustomForeColor,
                options.CustomBorderColor,
                options.CustomIconColor
            );
        }

        /// <summary>
        /// Creates a rounded rectangle path
        /// </summary>
        protected GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = radius * 2;
            int x = bounds.X;
            int y = bounds.Y;
            int width = bounds.Width;
            int height = bounds.Height;

            path.AddArc(x, y, diameter, diameter, 180, 90); // Top-left
            path.AddArc(x + width - diameter, y, diameter, diameter, 270, 90); // Top-right
            path.AddArc(x + width - diameter, y + height - diameter, diameter, diameter, 0, 90); // Bottom-right
            path.AddArc(x, y + height - diameter, diameter, diameter, 90, 90); // Bottom-left
            path.CloseFigure();

            return path;
        }

        public virtual void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            using (var brush = new SolidBrush(colors.BackColor))
            {
                if (data.Layout == NotificationLayout.Banner)
                {
                    g.FillRectangle(brush, bounds);
                }
                else
                {
                    using (var path = CreateRoundedPath(bounds, NotificationStyleHelpers.GetRecommendedBorderRadius(BeepControlStyle.Material3)))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
        }

        public virtual void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            if (iconRect.IsEmpty || string.IsNullOrEmpty(data.IconPath))
                return;

            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            var iconPath = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath
                : NotificationData.GetDefaultIconForType(data.Type);

            var iconColor = data.IconTint ?? colors.IconColor;

            StyledImagePainter.PaintWithTint(g, iconRect, iconPath, iconColor, 1f, 0);
        }

        public virtual void PaintTitle(Graphics g, Rectangle titleRect, string title, NotificationData data)
        {
            if (titleRect.IsEmpty || string.IsNullOrEmpty(title))
                return;

            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            var titleFont = new Font(data.Layout == NotificationLayout.Prominent ? "Segoe UI" : "Segoe UI", 
                data.Layout == NotificationLayout.Prominent ? 14 : 10, FontStyle.Bold);

            TextRenderer.DrawText(g, title, titleFont, titleRect, colors.ForeColor,
                TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak);
        }

        public virtual void PaintMessage(Graphics g, Rectangle messageRect, string message, NotificationData data)
        {
            if (messageRect.IsEmpty || string.IsNullOrEmpty(message))
                return;

            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            var messageColor = Color.FromArgb(180, colors.ForeColor);

            TextRenderer.DrawText(g, message, SystemFonts.DefaultFont, messageRect, messageColor,
                TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak);
        }

        public virtual void PaintActions(Graphics g, Rectangle actionsRect, NotificationAction[] actions, NotificationData data)
        {
            if (actionsRect.IsEmpty || actions == null || actions.Length == 0)
                return;

            int buttonWidth = (actionsRect.Width - (12 * (actions.Length - 1))) / actions.Length;
            int x = actionsRect.X;

            foreach (var action in actions)
            {
                var buttonRect = new Rectangle(x, actionsRect.Y, buttonWidth, actionsRect.Height);
                PaintActionButton(g, buttonRect, action, data);
                x += buttonWidth + 12;
            }
        }

        protected virtual void PaintActionButton(Graphics g, Rectangle rect, NotificationAction action, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            var buttonColor = action.CustomColor ?? (action.IsPrimary ? colors.ForeColor : Color.FromArgb(150, colors.ForeColor));
            var isHovered = rect.Contains(Cursor.Position);

            if (isHovered)
            {
                using (var brush = new SolidBrush(Color.FromArgb(30, colors.ForeColor)))
                {
                    g.FillRectangle(brush, rect);
                }
            }

            using (var pen = new Pen(buttonColor, 1))
            {
                g.DrawRectangle(pen, rect);
            }

            TextRenderer.DrawText(g, action.Text, SystemFonts.DefaultFont, rect, buttonColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        public virtual void PaintCloseButton(Graphics g, Rectangle closeButtonRect, bool isHovered, NotificationData data)
        {
            if (closeButtonRect.IsEmpty)
                return;

            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            var closeColor = isHovered ? Color.FromArgb(200, colors.ForeColor) : colors.ForeColor;

            using (var pen = new Pen(closeColor, 2))
            {
                int padding = 6;
                g.DrawLine(pen,
                    closeButtonRect.X + padding, closeButtonRect.Y + padding,
                    closeButtonRect.Right - padding, closeButtonRect.Bottom - padding);
                g.DrawLine(pen,
                    closeButtonRect.Right - padding, closeButtonRect.Y + padding,
                    closeButtonRect.X + padding, closeButtonRect.Bottom - padding);
            }
        }

        public virtual void PaintProgressBar(Graphics g, Rectangle progressBarRect, float progress, NotificationData data)
        {
            if (progressBarRect.IsEmpty)
                return;

            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));

            // Background
            using (var brush = new SolidBrush(Color.FromArgb(50, colors.ForeColor)))
            {
                g.FillRectangle(brush, progressBarRect);
            }

            // Progress
            int progressWidth = (int)(progressBarRect.Width * (progress / 100f));
            if (progressWidth > 0)
            {
                var progressRect = new Rectangle(progressBarRect.X, progressBarRect.Y, progressWidth, progressBarRect.Height);
                using (var brush = new SolidBrush(colors.IconColor))
                {
                    g.FillRectangle(brush, progressRect);
                }
            }
        }

        /// <summary>
        /// Creates render options from notification data
        /// </summary>
        protected NotificationRenderOptions CreateRenderOptions(NotificationData data)
        {
            return new NotificationRenderOptions
            {
                Type = data.Type,
                Layout = data.Layout,
                Priority = data.Priority,
                CustomBackColor = data.CustomBackColor,
                CustomForeColor = data.CustomForeColor,
                CustomBorderColor = null,
                CustomIconColor = data.IconTint,
                BorderRadius = NotificationStyleHelpers.GetRecommendedBorderRadius(BeepControlStyle.Material3),
                Padding = NotificationStyleHelpers.GetRecommendedPadding(data.Layout),
                Spacing = NotificationStyleHelpers.GetRecommendedSpacing(data.Layout),
                IconSize = NotificationStyleHelpers.GetRecommendedIconSize(data.Layout),
                ShowCloseButton = data.ShowCloseButton,
                ShowProgressBar = data.ShowProgressBar
            };
        }
    }
}
