using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    /// <summary>
    /// Base class for BeepSideBar painters with common helper methods
    /// </summary>
    public abstract class BaseSideBarPainter : ISideBarPainter
    {
        // Reusable ImagePainter instance - DO NOT create new instances in loops!
        private static readonly ImagePainter _sharedImagePainter = new ImagePainter();

        public abstract string Name { get; }

        public abstract void Paint(ISideBarPainterContext context);

        public virtual void PaintMenuItem(Graphics g, SimpleItem item, Rectangle itemRect, ISideBarPainterContext context)
        {
            int padding = 8;
            int iconSize = 24;
            int x = itemRect.X + padding;

            // Draw icon if available
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                PaintMenuItemIcon(g, item, iconRect, context);
                x += iconSize + padding;
            }

            // Draw text if not collapsed
            if (!context.IsCollapsed)
            {
                var textRect = new Rectangle(x, itemRect.Y, itemRect.Right - x - padding, itemRect.Height);
                PaintMenuItemText(g, item, textRect, context);
            }
        }

        public abstract void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context);

        public abstract void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context);

        public abstract void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context);

        public virtual void PaintChildItem(Graphics g, SimpleItem childItem, Rectangle childRect, ISideBarPainterContext context, int indentLevel)
        {
            int padding = 4;
            int iconSize = 18; // Smaller for children
            int indent = context.IndentationWidth * indentLevel;
            int x = childRect.X + indent + padding;

            // Draw connector line
            PaintConnectorLine(g, childRect, context, indent);

            // Draw icon if available
            if (!string.IsNullOrEmpty(childItem.ImagePath))
            {
                var iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                PaintMenuItemIcon(g, childItem, iconRect, context);
                x += iconSize + padding;
            }

            // Draw text if not collapsed
            if (!context.IsCollapsed)
            {
                var textRect = new Rectangle(x, childRect.Y, childRect.Right - x - padding, childRect.Height);
                PaintChildItemText(g, childItem, textRect, context);
            }
        }

        /// <summary>
        /// Helper method to paint menu item icon using ImagePainter with theme support
        /// REUSES shared ImagePainter instance - do NOT create new instances!
        /// </summary>
        protected virtual void PaintMenuItemIcon(Graphics g, SimpleItem item, Rectangle iconRect, ISideBarPainterContext context)
        {
            try
            {
                string iconPath = GetIconPath(item, context);
                if (!string.IsNullOrEmpty(iconPath))
                {
                    if (context.Theme != null && context.UseThemeColors)
                    {
                        StyledImagePainter.PaintWithTint(g, iconRect, iconPath, context.Theme.SideMenuForeColor);
                    }
                    else
                    {
                        StyledImagePainter.Paint(g, iconRect, iconPath);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Icon drawing error: {ex.Message}");
                // Fallback: draw placeholder
                using (var pen = new Pen(Color.Gray, 1f))
                {
                    g.DrawRectangle(pen, iconRect);
                }
            }
        }

        /// <summary>
        /// Get effective icon path for an item, falling back to context.DefaultImagePath if item.ImagePath is empty.
        /// </summary>
        protected virtual string GetIconPath(SimpleItem item, ISideBarPainterContext context)
        {
            // 1) Explicit: item.ImagePath
            if (!string.IsNullOrEmpty(item?.ImagePath)) return item.ImagePath;

            // 2) Per-item heuristic: check MenuID or Name and map to known svgs
            var heur = TryGetHeuristicIconByMenuOrName(item);
            if (!string.IsNullOrEmpty(heur)) return heur;

            // 3) Per-style heuristic: choose default icon depending on ControlStyle
            if (context != null)
            {
                var style = context.ControlStyle;
                var styleIcon = GetStyleDefaultIcon(style);
                if (!string.IsNullOrEmpty(styleIcon)) return styleIcon;
            }

            // 4) Global default from context
            if (context?.DefaultImagePath != null && !string.IsNullOrEmpty(context.DefaultImagePath)) return context.DefaultImagePath;

            // 5) Fallback to empty
            return string.Empty;
        }

        /// <summary>
        /// Map some common names or menu IDs to sensible default icons
        /// This is intentionally simple — add more mappings as needed.
        /// </summary>
        protected virtual string TryGetHeuristicIconByMenuOrName(SimpleItem item)
        {
            if (item == null) return null;
            string key = (item.MenuID ?? item.Name ?? item.Text ?? string.Empty).ToLowerInvariant();
            if (string.IsNullOrEmpty(key)) return null;

            switch (key)
            {
                case string k when k.Contains("dashboard") || k.Contains("home"):
                    return TheTechIdea.Beep.Icons.Svgs.NavDashboard; // default dashboard
                case string k when k.Contains("settings") || k.Contains("gear") || k.Contains("config"):
                    return TheTechIdea.Beep.Icons.Svgs.Settings;
                case string k when k.Contains("profile") || k.Contains("avatar"):
                    return TheTechIdea.Beep.Icons.Svgs.Cat; // playful default profile/avatar icon
                case string k when k.Contains("inbox") || k.Contains("mail") || k.Contains("messages"):
                    return TheTechIdea.Beep.Icons.Svgs.Mail;
                case string k when k.Contains("calendar"):
                    return TheTechIdea.Beep.Icons.Svgs.Calendar;
                case string k when k.Contains("search") || k.Contains("find"):
                    return TheTechIdea.Beep.Icons.Svgs.Search;
                case string k when k.Contains("users") || k.Contains("members") || k.Contains("team"):
                    return TheTechIdea.Beep.Icons.Svgs.Person;
                case string k when k.Contains("tasks") || k.Contains("todo"):
                    return TheTechIdea.Beep.Icons.Svgs.Check;
                case string k when k.Contains("reports") || k.Contains("analytics"):
                    return TheTechIdea.Beep.Icons.Svgs.NavDashboard;
                case string k when k.Contains("notification") || k.Contains("notifications") || k.Contains("bell"):
                    return TheTechIdea.Beep.Icons.SvgsUI.Bell;
                case string k when k.Contains("bill") || k.Contains("billing") || k.Contains("payment") || k.Contains("invoice"):
                    return TheTechIdea.Beep.Icons.SvgsUI.CreditCard;
                case string k when k.Contains("notifications") || k.Contains("alerts") || k.Contains("alarms"):
                    return TheTechIdea.Beep.Icons.Svgs.InfoAlarm;
                case string k when k.Contains("help") || k.Contains("support") || k.Contains("faq"):
                    return TheTechIdea.Beep.Icons.SvgsUI.HelpCircle;
                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// Returns a default icon for the entire painter style (e.g. PillRail uses small round icons)
        /// </summary>
        protected virtual string GetStyleDefaultIcon(TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle style)
        {
            switch (style)
            {
                case TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.PillRail:
                    return TheTechIdea.Beep.Icons.Svgs.NavDashboard; // placeholder for small nav style
                case TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.NotionMinimal:
                case TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.Minimal:
                    return TheTechIdea.Beep.Icons.Svgs.Menu; // ultra minimal
                case TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.FinSet:
                    return TheTechIdea.Beep.Icons.Svgs.NavDashboard; // FinSet prefers more detailed icons
                case TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.AntDesign:
                    return TheTechIdea.Beep.Icons.Svgs.NavDashboard;
                case TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.StripeDashboard:
                    return TheTechIdea.Beep.Icons.Svgs.NavDashboard;
          
                case TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.DiscordStyle:
                    return TheTechIdea.Beep.Icons.Svgs.NavUser;
                default:
                    return TheTechIdea.Beep.Icons.Svgs.Menu;
            }
        }

        /// <summary>
        /// Helper method to paint menu item text with theme colors
        /// Uses TextRenderer for better text quality and automatic ellipsis
        /// </summary>
        protected virtual void PaintMenuItemText(Graphics g, SimpleItem item, Rectangle textRect, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(item.Text) || textRect.Width <= 0 || textRect.Height <= 0)
                return;

            // Use theme colors if available
            Color textColor;
            if (context.Theme != null && context.UseThemeColors)
            {
                textColor = item.IsEnabled ?
                    context.Theme.SideMenuForeColor :
                    context.Theme.DisabledForeColor;
            }
            else
            {
                textColor = item.IsEnabled ? Color.FromArgb(240, 240, 240) : Color.Gray;
            }

            // Use theme font if available (cached)
            Font textFont = context.Theme != null ?
               BeepThemesManager.ToFont(context.Theme.GetAnswerFont()) :
                BeepFontManager.GetCachedFont("Segoe UI", 9f);

            // Use TextRenderer for better quality
            var textFormat = System.Windows.Forms.TextFormatFlags.Left |
                            System.Windows.Forms.TextFormatFlags.VerticalCenter |
                            System.Windows.Forms.TextFormatFlags.EndEllipsis |
                            System.Windows.Forms.TextFormatFlags.NoPrefix |
                            System.Windows.Forms.TextFormatFlags.SingleLine;

            System.Windows.Forms.TextRenderer.DrawText(g, item.Text, textFont, textRect, textColor, textFormat);
        }

        /// <summary>
        /// Helper method to paint child item text (smaller font)
        /// Uses TextRenderer for better text quality and automatic ellipsis
        /// </summary>
        protected virtual void PaintChildItemText(Graphics g, SimpleItem childItem, Rectangle textRect, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(childItem.Text) || textRect.Width <= 0 || textRect.Height <= 0)
                return;

            // Use theme colors if available
            Color textColor;
            if (context.Theme != null && context.UseThemeColors)
            {
                textColor = childItem.IsEnabled ?
                    context.Theme.SideMenuForeColor :
                    context.Theme.DisabledForeColor;
            }
            else
            {
                textColor = childItem.IsEnabled ? Color.FromArgb(220, 220, 220) : Color.Gray;
            }

            // Use smaller font for children (cached)
            Font textFont = context.Theme != null ?
                BeepThemesManager.ToFont(context.Theme.GetAnswerFont()) :
                BeepFontManager.GetCachedFont("Segoe UI", 8.5f);

            // Make it slightly smaller
            if (textFont.Size > 7)
            {
                textFont = BeepFontManager.GetCachedFont(textFont.FontFamily.Name, textFont.Size - 0.5f, textFont.Style);
            }

            // Use TextRenderer for better quality
            var textFormat = System.Windows.Forms.TextFormatFlags.Left |
                            System.Windows.Forms.TextFormatFlags.VerticalCenter |
                            System.Windows.Forms.TextFormatFlags.EndEllipsis |
                            System.Windows.Forms.TextFormatFlags.NoPrefix |
                            System.Windows.Forms.TextFormatFlags.SingleLine;

            System.Windows.Forms.TextRenderer.DrawText(g, childItem.Text, textFont, textRect, textColor, textFormat);
        }

        /// <summary>
        /// Helper method to draw connector line from parent to child
        /// </summary>
        protected virtual void PaintConnectorLine(Graphics g, Rectangle childRect, ISideBarPainterContext context, int indent)
        {
            Color lineColor = context.Theme != null && context.UseThemeColors ?
                Color.FromArgb(50, context.Theme.SideMenuForeColor) :
                Color.FromArgb(50, 180, 180, 180);

            using (var pen = new Pen(lineColor, 1f))
            {
                int lineX = childRect.X + (indent / 2);
                int lineY = childRect.Y + childRect.Height / 2;

                // Draw horizontal line to child
                g.DrawLine(pen, lineX, lineY, childRect.X + indent, lineY);
            }
        }

        /// <summary>
        /// Helper method to create rounded rectangle path
        /// </summary>
        protected GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Helper method to draw expand/collapse icon
        /// </summary>
        protected virtual void PaintExpandCollapseIcon(Graphics g, Rectangle iconRect, bool isExpanded, ISideBarPainterContext context)
        {
            Color iconColor = context.Theme != null && context.UseThemeColors ?
                context.Theme.SideMenuForeColor :
                Color.FromArgb(200, 200, 200);

            using (var pen = new Pen(iconColor, 2f))
            {
                int centerX = iconRect.X + iconRect.Width / 2;
                int centerY = iconRect.Y + iconRect.Height / 2;

                // Draw horizontal line
                g.DrawLine(pen, centerX - 4, centerY, centerX + 4, centerY);

                // Draw vertical line if not expanded (making a plus)
                if (!isExpanded)
                {
                    g.DrawLine(pen, centerX, centerY - 4, centerX, centerY + 4);
                }
            }
        }

        /// <summary>
        /// Paints an SVG given a path and optional tint, catching exceptions and falling back to a simple chevron.
        /// This centralizes try/catch so that painters don't cause unhandled exceptions that could freeze the UI.
        /// </summary>
        protected void PaintSvgWithFallback(Graphics g, Rectangle rect, string svgPath, Color? tint, bool isChevronDown = false, ISideBarPainterContext context = null)
        {
            try
            {
                // Prefer StyledImagePainter (it handles caching internally)
                if (!string.IsNullOrEmpty(svgPath))
                {
                    if (tint.HasValue)
                        StyledImagePainter.PaintWithTint(g, rect, svgPath, tint.Value);
                    else
                        StyledImagePainter.Paint(g, rect, svgPath);
                    return;
                }
                return;
            }
            catch
            {
                // fallback: chevron-like drawing
                using (var pen = new Pen(Color.Gray, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    int cx = rect.X + rect.Width / 2, cy = rect.Y + rect.Height / 2;
                    if (isChevronDown)
                    {
                        g.DrawLine(pen, cx - 3, cy - 1, cx, cy + 2);
                        g.DrawLine(pen, cx, cy + 2, cx + 3, cy - 1);
                    }
                    else
                    {
                        g.DrawLine(pen, cx - 2, cy - 2, cx + 2, cy);
                        g.DrawLine(pen, cx + 2, cy, cx - 2, cy + 2);
                    }
                }
            }
        }

        /// <summary>
        /// Helper method to get effective color based on UseThemeColors setting
        /// </summary>
        protected Color GetEffectiveColor(ISideBarPainterContext context, Color themeColor, Color fallbackColor)
        {
            return context.UseThemeColors && context.Theme != null ? themeColor : fallbackColor;
        }

        /// <summary>
        /// Calculate minimum required size for a sidebar item based on text and icon
        /// Helps prevent text clipping by ensuring adequate space
        /// Sidebar is always vertical: icon left, text right
        /// </summary>
        protected static Size CalculateMinimumItemSize(Graphics g, SimpleItem item,
            string fontFamily = "Segoe UI", float fontSize = 9f, int iconSize = 24, int padding = 12)
        {
            if (item == null)
                return new Size(200, 44); // Default minimum

            bool hasIcon = !string.IsNullOrEmpty(item.ImagePath);
            bool hasText = !string.IsNullOrEmpty(item.Text);

            if (!hasIcon && !hasText)
                return new Size(200, 44); // Default minimum

            Size textSize = Size.Empty;
        
            if (hasText)
            {
                var font = BeepFontManager.GetCachedFont(fontFamily, fontSize, FontStyle.Regular);
                textSize = System.Windows.Forms.TextRenderer.MeasureText(g, item.Text, font,
                    new Size(int.MaxValue, int.MaxValue),
                    System.Windows.Forms.TextFormatFlags.NoPadding |
                    System.Windows.Forms.TextFormatFlags.SingleLine);
            }

            // Sidebar: icon left, text right, side by side
            int width, height;

            if (hasIcon && hasText)
            {
                // Both icon and text
                width = iconSize + textSize.Width + padding * 3; // icon + spacing + text + padding
                height = Math.Max(iconSize, textSize.Height) + padding * 2;
            }
            else if (hasIcon)
            {
                // Only icon
                width = iconSize + padding * 2;
                height = iconSize + padding * 2;
            }
            else
            {
                // Only text
                width = textSize.Width + padding * 2;
                height = textSize.Height + padding * 2;
            }

            return new Size(Math.Max(width, 200), Math.Max(height, 44));
        }

        /// <summary>
        /// Helper methods for consistent sizing and spacing across SideBar painters
        /// Ensures parity across painters by computing sizes based on context heights.
        /// </summary>
        protected int GetTopLevelIconSize(ISideBarPainterContext context)
        {
            return Math.Max(12, Math.Min(24, context.ItemHeight - 8));
        }

        protected int GetChildIconSize(ISideBarPainterContext context)
        {
            return Math.Max(10, Math.Min(20, context.ChildItemHeight - 6));
        }

        protected int GetExpandIconSize(ISideBarPainterContext context)
        {
            return Math.Max(12, Math.Min(18, context.ItemHeight - 8));
        }

        protected int GetChildExpandIconSize(ISideBarPainterContext context)
        {
            return Math.Max(10, Math.Min(16, context.ChildItemHeight - 6));
        }

        protected int GetIconPadding(ISideBarPainterContext context)
        {
            return 12;
        }

        /// <summary>
        /// Draw a crisp hamburger (menu) icon using StyledImagePainter if available
        /// Falls back to 3 rounded bars if embedded SVG can't be used
        /// </summary>
        protected void DrawHamburgerIcon(Graphics g, Rectangle iconRect, Color color)
        {
            string svg = TheTechIdea.Beep.Icons.Svgs.Menu;
            try
            {
                if (!string.IsNullOrEmpty(svg))
                {
                    StyledImagePainter.PaintWithTint(g, iconRect, svg, color);
                    return;
                }
            }
            catch { }

            // fallback: 3 filled rounded rectangles
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int barHeight = Math.Max(2, iconRect.Height / 8);
            int gap = barHeight * 2;
            int w = iconRect.Width - 6; // slight inset
            int x = iconRect.X + 3;
            int y = iconRect.Y + (iconRect.Height - (3 * barHeight + 2 * gap)) / 2;
            using (var brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, new Rectangle(x, y, w, barHeight));
                g.FillRectangle(brush, new Rectangle(x, y + barHeight + gap, w, barHeight));
                g.FillRectangle(brush, new Rectangle(x, y + 2 * (barHeight + gap), w, barHeight));
            }
        }
    }
}
