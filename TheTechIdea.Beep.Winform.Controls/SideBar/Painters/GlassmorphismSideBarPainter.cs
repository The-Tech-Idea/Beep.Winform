using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.SideBar.Painters
{
    /// <summary>
    /// Glassmorphism Style Sidebar Painter
    /// 
    /// DISTINCT FEATURES:
    /// - Frosted glass effect with blur simulation
    /// - Semi-transparent backgrounds with white/light overlays
    /// - Soft shadows and glows
    /// - Glass-like selection pills with inner highlight
    /// - Subtle gradient overlays
    /// - Modern, ethereal aesthetic
    /// 
    /// Selection: Frosted glass pill with inner glow
    /// Hover: Increased frosting/opacity
    /// Expand Icon: Translucent chevron with glow
    /// Accordion: Stacked glass panels
    /// </summary>
    public sealed class GlassmorphismSideBarPainter : ISideBarPainter
    {
        public string Name => "Glassmorphism";

        private bool _disposed = false;
        private Font _headerFont;
        private Font _itemFont;
        private Font _childFont;
        private Font _badgeFont;

        #region Main Paint Method

        public void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Initialize fonts
            EnsureFonts();

            // Glassmorphism background - gradient with frosted overlay
            PaintGlassBackground(g, bounds, context);

            // Draw rail shadow on right edge
            if (context.EnableRailShadow)
            {
                PaintRailShadow(g, bounds, context);
            }

            int padding = 16;
            int currentY = bounds.Top + padding;

            // Toggle button
            if (context.ShowToggleButton)
            {
                Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                PaintToggleButton(g, toggleRect, context);
                currentY += context.ItemHeight + 16;
            }

            // Section headers and menu items
            PaintMenuItems(g, bounds, context, ref currentY);
        }

        private void EnsureFonts()
        {
            _headerFont ??= BeepFontManager.GetCachedFont("Segoe UI", 9f, FontStyle.Bold);
            _itemFont ??= BeepFontManager.GetCachedFont("Segoe UI", 11f, FontStyle.Regular);
            _childFont ??= BeepFontManager.GetCachedFont("Segoe UI", 10f, FontStyle.Regular);
            _badgeFont ??= BeepFontManager.GetCachedFont("Segoe UI", 8f, FontStyle.Bold);
        }

        #endregion

        #region Background Painting

        private void PaintGlassBackground(Graphics g, Rectangle bounds, ISideBarPainterContext context)
        {
            // Base gradient (subtle blue to purple)
            Color gradientStart = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(240, context.Theme.SideMenuBackColor)
                : Color.FromArgb(240, 230, 235, 250);

            Color gradientEnd = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(230, context.Theme.PrimaryColor.R / 4 + 200, context.Theme.PrimaryColor.G / 4 + 200, context.Theme.PrimaryColor.B / 4 + 220)
                : Color.FromArgb(230, 220, 225, 245);

            using (var gradientBrush = new LinearGradientBrush(bounds, gradientStart, gradientEnd, 135f))
            {
                g.FillRectangle(gradientBrush, bounds);
            }

            // Frosted glass overlay (white with transparency)
            using (var frostBrush = new SolidBrush(Color.FromArgb(60, 255, 255, 255)))
            {
                g.FillRectangle(frostBrush, bounds);
            }

            // Subtle noise texture simulation (small dots)
            PaintNoiseTexture(g, bounds);

            // Glass border on right
            Color borderColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(100, context.Theme.BorderColor)
                : Color.FromArgb(100, 200, 200, 220);

            using (var pen = new Pen(borderColor, 1f))
            {
                g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }
        }

        private void PaintNoiseTexture(Graphics g, Rectangle bounds)
        {
            // Simulate frosted glass noise with subtle dots
            var random = new Random(42); // Fixed seed for consistency
            using (var brush = new SolidBrush(Color.FromArgb(8, 255, 255, 255)))
            {
                for (int i = 0; i < bounds.Width * bounds.Height / 200; i++)
                {
                    int x = bounds.Left + random.Next(bounds.Width);
                    int y = bounds.Top + random.Next(bounds.Height);
                    g.FillRectangle(brush, x, y, 1, 1);
                }
            }
        }

        private void PaintRailShadow(Graphics g, Rectangle bounds, ISideBarPainterContext context)
        {
            // Soft shadow on right edge
            int shadowWidth = 8;
            Rectangle shadowRect = new Rectangle(bounds.Right - shadowWidth, bounds.Top, shadowWidth, bounds.Height);

            using (var shadowBrush = new LinearGradientBrush(
                shadowRect,
                Color.FromArgb(0, 0, 0, 0),
                Color.FromArgb(30, 0, 0, 0),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(shadowBrush, shadowRect);
            }
        }

        #endregion

        #region Toggle Button

        public void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            // Glass pill toggle button
            using (var path = CreateRoundedPath(toggleRect, toggleRect.Height / 2))
            {
                // Glass background
                using (var brush = new SolidBrush(Color.FromArgb(80, 255, 255, 255)))
                {
                    g.FillPath(brush, path);
                }

                // Inner highlight (top edge)
                using (var highlightPen = new Pen(Color.FromArgb(120, 255, 255, 255), 1f))
                {
                    g.DrawArc(highlightPen, toggleRect.X, toggleRect.Y, toggleRect.Width, toggleRect.Height, 180, 180);
                }

                // Border
                using (var borderPen = new Pen(Color.FromArgb(60, 100, 100, 120), 1f))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            // Hamburger icon
            Color iconColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.SideMenuForeColor
                : Color.FromArgb(80, 80, 100);

            int iconSize = Math.Min(20, toggleRect.Height - 12);
            Rectangle iconRect = new Rectangle(
                toggleRect.X + (toggleRect.Width - iconSize) / 2,
                toggleRect.Y + (toggleRect.Height - iconSize) / 2,
                iconSize, iconSize);

            try
            {
                StyledImagePainter.PaintWithTint(g, iconRect, TheTechIdea.Beep.Icons.Svgs.Menu, iconColor);
            }
            catch
            {
                DrawHamburgerFallback(g, iconRect, iconColor);
            }
        }

        #endregion

        #region Menu Items

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;

            int padding = 16;
            int itemIndex = 0;

            foreach (var item in context.Items)
            {
                // Check for section header before this item
                if (context.SectionHeaders != null)
                {
                    var sectionHeader = context.SectionHeaders.FirstOrDefault(h => h.BeforeIndex == itemIndex);
                    if (!string.IsNullOrEmpty(sectionHeader.HeaderText))
                    {
                        Rectangle headerRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, 24);
                        PaintSectionHeader(g, headerRect, sectionHeader.HeaderText, context);
                        currentY += 28;
                    }
                }

                Rectangle itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);

                // Paint states
                if (!item.IsEnabled)
                {
                    PaintDisabled(g, itemRect, context);
                }
                else if (item == context.PressedItem)
                {
                    PaintPressed(g, itemRect, context);
                }
                else if (item == context.SelectedItem)
                {
                    PaintSelection(g, itemRect, context);
                }
                else if (item == context.HoveredItem)
                {
                    PaintHover(g, itemRect, context);
                }

                // Paint menu item content
                PaintMenuItem(g, item, itemRect, context);

                // Paint badge if present
                if (context.ItemBadges != null && context.ItemBadges.TryGetValue(item, out string badgeText))
                {
                    Color badgeColor = context.ItemBadgeColors != null && context.ItemBadgeColors.TryGetValue(item, out Color bc) ? bc : Color.FromArgb(220, 80, 80);
                    PaintBadge(g, itemRect, badgeText, badgeColor, context);
                }

                currentY += context.ItemHeight + 4;

                // Check for divider after this item
                if (context.DividerPositions != null && context.DividerPositions.Contains(itemIndex))
                {
                    Rectangle dividerRect = new Rectangle(bounds.Left, currentY, bounds.Width, 8);
                    PaintDivider(g, dividerRect, context);
                    currentY += 12;
                }

                // Paint children if expanded
                if (item.Children != null && item.Children.Count > 0 &&
                    context.ExpandedState.TryGetValue(item, out bool isExpanded) && isExpanded)
                {
                    PaintChildItems(g, bounds, context, item, itemRect, ref currentY, 1);
                }

                itemIndex++;
            }
        }

        public void PaintMenuItem(Graphics g, SimpleItem item, Rectangle itemRect, ISideBarPainterContext context)
        {
            int iconSize = 22;
            int iconPadding = 12;
            int x = itemRect.X + iconPadding;

            // Draw icon
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);

                Color iconColor = context.UseThemeColors && context.Theme != null
                    ? (item == context.SelectedItem ? context.Theme.PrimaryColor : context.Theme.SideMenuForeColor)
                    : (item == context.SelectedItem ? Color.FromArgb(100, 100, 200) : Color.FromArgb(80, 80, 100));

                try
                {
                    StyledImagePainter.PaintWithTint(g, iconRect, item.ImagePath, iconColor);
                }
                catch { }

                x += iconSize + iconPadding;
            }

            // Draw text
            if (!context.IsCollapsed)
            {
                Color textColor = context.UseThemeColors && context.Theme != null
                    ? (item == context.SelectedItem ? context.Theme.PrimaryColor : context.Theme.SideMenuForeColor)
                    : (item == context.SelectedItem ? Color.FromArgb(60, 60, 120) : Color.FromArgb(60, 60, 80));

                int expandIconSpace = (item.Children != null && item.Children.Count > 0) ? 28 : 8;
                Rectangle textRect = new Rectangle(x, itemRect.Y, Math.Max(0, itemRect.Right - x - expandIconSpace), itemRect.Height);

                using (var brush = new SolidBrush(textColor))
                {
                    var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                    g.DrawString(item.Text, _itemFont, brush, textRect, format);
                }

                // Draw expand icon
                if (item.Children != null && item.Children.Count > 0)
                {
                    int expandSize = 16;
                    Rectangle expandRect = new Rectangle(itemRect.Right - expandSize - 8, itemRect.Y + (itemRect.Height - expandSize) / 2, expandSize, expandSize);
                    bool isExpanded = context.ExpandedState.TryGetValue(item, out bool exp) && exp;
                    PaintExpandIcon(g, expandRect, isExpanded, item, context);
                }
            }
        }

        private void PaintChildItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, SimpleItem parentItem, Rectangle parentRect, ref int currentY, int indentLevel)
        {
            if (parentItem.Children == null || parentItem.Children.Count == 0 || context.IsCollapsed) return;

            int padding = 8;
            int indent = context.IndentationWidth * indentLevel;

            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);

                // Draw glass connector
                PaintAccordionConnector(g, parentItem, parentRect, child, childRect, indentLevel, context);

                // Paint states
                if (!child.IsEnabled)
                {
                    PaintDisabled(g, childRect, context);
                }
                else if (child == context.PressedItem)
                {
                    PaintPressed(g, childRect, context);
                }
                else if (child == context.SelectedItem)
                {
                    PaintSelection(g, childRect, context);
                }
                else if (child == context.HoveredItem)
                {
                    PaintHover(g, childRect, context);
                }

                // Paint child content
                PaintChildItem(g, child, childRect, context, indentLevel);

                currentY += context.ChildItemHeight + 2;

                // Nested children
                if (child.Children != null && child.Children.Count > 0 &&
                    context.ExpandedState.TryGetValue(child, out bool isExpanded) && isExpanded)
                {
                    PaintChildItems(g, bounds, context, child, childRect, ref currentY, indentLevel + 1);
                }
            }
        }

        public void PaintChildItem(Graphics g, SimpleItem childItem, Rectangle childRect, ISideBarPainterContext context, int indentLevel)
        {
            int iconSize = 18;
            int iconPadding = 8;
            int x = childRect.X + iconPadding;

            // Draw icon
            if (!string.IsNullOrEmpty(childItem.ImagePath))
            {
                Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);

                Color iconColor = context.UseThemeColors && context.Theme != null
                    ? Color.FromArgb(180, context.Theme.SideMenuForeColor)
                    : Color.FromArgb(100, 100, 120);

                try
                {
                    StyledImagePainter.PaintWithTint(g, iconRect, childItem.ImagePath, iconColor);
                }
                catch { }

                x += iconSize + iconPadding;
            }

            // Draw text
            Color textColor = context.UseThemeColors && context.Theme != null
                ? (childItem == context.SelectedItem ? context.Theme.PrimaryColor : Color.FromArgb(180, context.Theme.SideMenuForeColor))
                : (childItem == context.SelectedItem ? Color.FromArgb(80, 80, 150) : Color.FromArgb(100, 100, 120));

            int expandIconSpace = (childItem.Children != null && childItem.Children.Count > 0) ? 24 : 4;
            Rectangle textRect = new Rectangle(x, childRect.Y, Math.Max(0, childRect.Right - x - expandIconSpace), childRect.Height);

            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                g.DrawString(childItem.Text, _childFont, brush, textRect, format);
            }

            // Draw expand icon for nested children
            if (childItem.Children != null && childItem.Children.Count > 0)
            {
                int expandSize = 14;
                Rectangle expandRect = new Rectangle(childRect.Right - expandSize - 6, childRect.Y + (childRect.Height - expandSize) / 2, expandSize, expandSize);
                bool isExpanded = context.ExpandedState.TryGetValue(childItem, out bool exp) && exp;
                PaintExpandIcon(g, expandRect, isExpanded, childItem, context);
            }
        }

        #endregion

        #region State Painting (DISTINCT)

        public void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Glassmorphism selection: frosted glass pill with inner glow
            using (var path = CreateRoundedPath(itemRect, 12))
            {
                // Glass fill
                Color glassColor = context.UseThemeColors && context.Theme != null
                    ? Color.FromArgb(120, context.Theme.PrimaryColor.R / 2 + 100, context.Theme.PrimaryColor.G / 2 + 100, context.Theme.PrimaryColor.B / 2 + 150)
                    : Color.FromArgb(120, 180, 180, 220);

                using (var brush = new SolidBrush(glassColor))
                {
                    g.FillPath(brush, path);
                }

                // Inner highlight (top)
                Rectangle highlightRect = new Rectangle(itemRect.X + 2, itemRect.Y + 2, itemRect.Width - 4, itemRect.Height / 2);
                using (var highlightBrush = new LinearGradientBrush(highlightRect, Color.FromArgb(80, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), LinearGradientMode.Vertical))
                {
                    using (var highlightPath = CreateRoundedPath(highlightRect, 10))
                    {
                        g.FillPath(highlightBrush, highlightPath);
                    }
                }

                // Glass border
                using (var borderPen = new Pen(Color.FromArgb(100, 255, 255, 255), 1f))
                {
                    g.DrawPath(borderPen, path);
                }
            }
        }

        public void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Glassmorphism hover: subtle frosted overlay
            using (var path = CreateRoundedPath(itemRect, 10))
            {
                using (var brush = new SolidBrush(Color.FromArgb(60, 255, 255, 255)))
                {
                    g.FillPath(brush, path);
                }

                // Subtle border
                using (var borderPen = new Pen(Color.FromArgb(40, 200, 200, 220), 1f))
                {
                    g.DrawPath(borderPen, path);
                }
            }
        }

        public void PaintPressed(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Glassmorphism pressed: inset glass effect
            using (var path = CreateRoundedPath(itemRect, 10))
            {
                // Darker glass
                using (var brush = new SolidBrush(Color.FromArgb(100, 150, 150, 180)))
                {
                    g.FillPath(brush, path);
                }

                // Inner shadow (simulated)
                Rectangle shadowRect = new Rectangle(itemRect.X + 2, itemRect.Y + 2, itemRect.Width - 4, itemRect.Height - 4);
                using (var shadowPath = CreateRoundedPath(shadowRect, 8))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }
            }
        }

        public void PaintDisabled(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Glassmorphism disabled: very faint, desaturated
            using (var brush = new SolidBrush(Color.FromArgb(40, 200, 200, 200)))
            {
                g.FillRectangle(brush, itemRect);
            }
        }

        #endregion

        #region Expand Icon (DISTINCT)

        public void PaintExpandIcon(Graphics g, Rectangle iconRect, bool isExpanded, SimpleItem item, ISideBarPainterContext context)
        {
            // Glassmorphism expand icon: translucent chevron with subtle glow
            Color iconColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(180, context.Theme.SideMenuForeColor)
                : Color.FromArgb(120, 120, 140);

            // Try SVG first
            string iconPath = isExpanded
                ? (context.CollapseIconPath ?? TheTechIdea.Beep.Icons.SvgsUI.ChevronDown)
                : (context.ExpandIconPath ?? TheTechIdea.Beep.Icons.SvgsUI.ChevronRight);

            try
            {
                StyledImagePainter.PaintWithTint(g, iconRect, iconPath, iconColor);
            }
            catch
            {
                // Fallback: draw glass chevron
                using (var pen = new Pen(iconColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    int cx = iconRect.X + iconRect.Width / 2;
                    int cy = iconRect.Y + iconRect.Height / 2;
                    int size = iconRect.Width / 3;

                    if (isExpanded)
                    {
                        g.DrawLine(pen, cx - size, cy - size / 2, cx, cy + size / 2);
                        g.DrawLine(pen, cx, cy + size / 2, cx + size, cy - size / 2);
                    }
                    else
                    {
                        g.DrawLine(pen, cx - size / 2, cy - size, cx + size / 2, cy);
                        g.DrawLine(pen, cx + size / 2, cy, cx - size / 2, cy + size);
                    }
                }
            }
        }

        #endregion

        #region Accordion Connector (DISTINCT)

        public void PaintAccordionConnector(Graphics g, SimpleItem parent, Rectangle parentRect, SimpleItem child, Rectangle childRect, int indentLevel, ISideBarPainterContext context)
        {
            // Glassmorphism connector: subtle gradient line
            int indent = context.IndentationWidth * indentLevel;
            int lineX = childRect.X - indent / 2;

            Color lineColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(60, context.Theme.PrimaryColor)
                : Color.FromArgb(60, 150, 150, 200);

            using (var pen = new Pen(lineColor, 1f))
            {
                // Horizontal line to child
                g.DrawLine(pen, lineX + 4, childRect.Y + childRect.Height / 2, childRect.X - 4, childRect.Y + childRect.Height / 2);

                // Small glass dot at connection point
                int dotSize = 4;
                using (var dotBrush = new SolidBrush(Color.FromArgb(100, 180, 180, 220)))
                {
                    g.FillEllipse(dotBrush, lineX, childRect.Y + childRect.Height / 2 - dotSize / 2, dotSize, dotSize);
                }
            }
        }

        #endregion

        #region Badge, Section Header, Divider (DISTINCT)

        public void PaintBadge(Graphics g, Rectangle itemRect, string badgeText, Color badgeColor, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(badgeText)) return;

            var textSize = g.MeasureString(badgeText, _badgeFont);
            int badgeWidth = Math.Max(18, (int)textSize.Width + 10);
            int badgeHeight = 18;
            int badgeX = itemRect.Right - badgeWidth - 28; // Leave space for expand icon
            int badgeY = itemRect.Y + (itemRect.Height - badgeHeight) / 2;

            Rectangle badgeRect = new Rectangle(badgeX, badgeY, badgeWidth, badgeHeight);

            // Glass badge
            using (var path = CreateRoundedPath(badgeRect, badgeHeight / 2))
            {
                // Glass background with badge color tint
                using (var brush = new SolidBrush(Color.FromArgb(180, badgeColor)))
                {
                    g.FillPath(brush, path);
                }

                // Inner highlight
                using (var highlightBrush = new SolidBrush(Color.FromArgb(60, 255, 255, 255)))
                {
                    Rectangle highlightRect = new Rectangle(badgeRect.X + 1, badgeRect.Y + 1, badgeRect.Width - 2, badgeRect.Height / 2);
                    using (var highlightPath = CreateRoundedPath(highlightRect, (badgeHeight - 2) / 2))
                    {
                        g.FillPath(highlightBrush, highlightPath);
                    }
                }
            }

            // Badge text
            using (var brush = new SolidBrush(Color.White))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(badgeText, _badgeFont, brush, badgeRect, format);
            }
        }

        public void PaintSectionHeader(Graphics g, Rectangle headerRect, string headerText, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(headerText)) return;

            Color textColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(140, context.Theme.SideMenuForeColor)
                : Color.FromArgb(100, 100, 120);

            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(headerText.ToUpperInvariant(), _headerFont, brush, headerRect, format);
            }

            // Subtle glass line after header
            int lineY = headerRect.Bottom - 2;
            using (var pen = new Pen(Color.FromArgb(40, 200, 200, 220), 1f))
            {
                g.DrawLine(pen, headerRect.X, lineY, headerRect.Right, lineY);
            }
        }

        public void PaintDivider(Graphics g, Rectangle dividerRect, ISideBarPainterContext context)
        {
            int y = dividerRect.Y + dividerRect.Height / 2;
            int padding = 20;

            // Glass divider line
            using (var pen = new Pen(Color.FromArgb(50, 200, 200, 220), 1f))
            {
                g.DrawLine(pen, dividerRect.X + padding, y, dividerRect.Right - padding, y);
            }

            // Subtle highlight below
            using (var pen = new Pen(Color.FromArgb(30, 255, 255, 255), 1f))
            {
                g.DrawLine(pen, dividerRect.X + padding, y + 1, dividerRect.Right - padding, y + 1);
            }
        }

        #endregion

        #region Helper Methods

        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void DrawHamburgerFallback(Graphics g, Rectangle iconRect, Color color)
        {
            int barHeight = 2;
            int gap = 4;
            int w = iconRect.Width - 4;
            int x = iconRect.X + 2;
            int y = iconRect.Y + (iconRect.Height - (3 * barHeight + 2 * gap)) / 2;

            using (var brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, x, y, w, barHeight);
                g.FillRectangle(brush, x, y + barHeight + gap, w, barHeight);
                g.FillRectangle(brush, x, y + 2 * (barHeight + gap), w, barHeight);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (!_disposed)
            {
                // Fonts are cached by BeepFontManager, don't dispose them
                _disposed = true;
            }
        }

        #endregion
    }
}

