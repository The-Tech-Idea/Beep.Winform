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
    /// Neumorphism (Soft UI) Style Sidebar Painter
    /// 
    /// DISTINCT FEATURES:
    /// - Soft, extruded shadows creating depth illusion
    /// - Same-color background with light/dark shadows
    /// - Raised elements appear to pop out
    /// - Pressed elements appear inset/concave
    /// - Monochromatic color scheme
    /// - Soft, rounded corners
    /// 
    /// Selection: Inset/concave shadow (pressed look)
    /// Hover: Raised/convex shadow (elevated look)
    /// Expand Icon: Soft plus/minus with shadow
    /// Accordion: Soft indentation with depth
    /// </summary>
    public sealed class NeumorphicSideBarPainter : ISideBarPainter
    {
        public string Name => "Neumorphic";

        private bool _disposed = false;
        private Font _headerFont;
        private Font _itemFont;
        private Font _childFont;
        private Font _badgeFont;

        // Neumorphism shadow colors (calculated from base color)
        private Color _baseColor;
        private Color _lightShadow;
        private Color _darkShadow;
        private int _shadowOffset = 4;
        private int _shadowBlur = 8;

        #region Main Paint Method

        public void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Initialize fonts and calculate shadow colors
            EnsureFonts();
            CalculateShadowColors(context);

            // Neumorphism background - flat, same color as elements
            PaintNeumorphicBackground(g, bounds, context);

            int padding = 16;
            int currentY = bounds.Top + padding;

            // Toggle button
            if (context.ShowToggleButton)
            {
                Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                PaintToggleButton(g, toggleRect, context);
                currentY += context.ItemHeight + 20;
            }

            // Menu items
            PaintMenuItems(g, bounds, context, ref currentY);
        }

        private void EnsureFonts()
        {
            _headerFont ??= BeepFontManager.GetCachedFont("Segoe UI", 9f, FontStyle.Bold);
            _itemFont ??= BeepFontManager.GetCachedFont("Segoe UI Semibold", 11f, FontStyle.Regular);
            _childFont ??= BeepFontManager.GetCachedFont("Segoe UI", 10f, FontStyle.Regular);
            _badgeFont ??= BeepFontManager.GetCachedFont("Segoe UI", 8f, FontStyle.Bold);
        }

        private void CalculateShadowColors(ISideBarPainterContext context)
        {
            // Base color from theme or default soft gray
            _baseColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.SideMenuBackColor
                : Color.FromArgb(230, 230, 235);

            // Calculate light and dark shadows based on base color
            int lightR = Math.Min(255, _baseColor.R + 25);
            int lightG = Math.Min(255, _baseColor.G + 25);
            int lightB = Math.Min(255, _baseColor.B + 25);
            _lightShadow = Color.FromArgb(lightR, lightG, lightB);

            int darkR = Math.Max(0, _baseColor.R - 30);
            int darkG = Math.Max(0, _baseColor.G - 30);
            int darkB = Math.Max(0, _baseColor.B - 30);
            _darkShadow = Color.FromArgb(darkR, darkG, darkB);
        }

        #endregion

        #region Background Painting

        private void PaintNeumorphicBackground(Graphics g, Rectangle bounds, ISideBarPainterContext context)
        {
            // Flat background - the key to neumorphism
            using (var brush = new SolidBrush(_baseColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Subtle inner shadow on right edge (rail effect)
            if (context.EnableRailShadow)
            {
                int shadowWidth = 6;
                Rectangle shadowRect = new Rectangle(bounds.Right - shadowWidth, bounds.Top, shadowWidth, bounds.Height);
                using (var shadowBrush = new LinearGradientBrush(
                    shadowRect,
                    Color.FromArgb(0, _darkShadow),
                    Color.FromArgb(40, _darkShadow),
                    LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(shadowBrush, shadowRect);
                }
            }
        }

        #endregion

        #region Toggle Button

        public void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            // Neumorphic raised button
            PaintNeumorphicRaised(g, toggleRect, 20);

            // Hamburger icon
            Color iconColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.SideMenuForeColor
                : Color.FromArgb(100, 100, 110);

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

        #region Neumorphic Effect Helpers

        private void PaintNeumorphicRaised(Graphics g, Rectangle rect, int radius)
        {
            // Raised/convex neumorphic effect
            // Light shadow (top-left) and dark shadow (bottom-right)

            using (var path = CreateRoundedPath(rect, radius))
            {
                // Dark shadow (bottom-right) - offset down and right
                Rectangle darkShadowRect = new Rectangle(rect.X + _shadowOffset, rect.Y + _shadowOffset, rect.Width, rect.Height);
                using (var darkPath = CreateRoundedPath(darkShadowRect, radius))
                using (var darkBrush = new SolidBrush(Color.FromArgb(60, _darkShadow)))
                {
                    g.FillPath(darkBrush, darkPath);
                }

                // Light shadow (top-left) - offset up and left
                Rectangle lightShadowRect = new Rectangle(rect.X - _shadowOffset / 2, rect.Y - _shadowOffset / 2, rect.Width, rect.Height);
                using (var lightPath = CreateRoundedPath(lightShadowRect, radius))
                using (var lightBrush = new SolidBrush(Color.FromArgb(80, _lightShadow)))
                {
                    g.FillPath(lightBrush, lightPath);
                }

                // Main surface
                using (var brush = new SolidBrush(_baseColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private void PaintNeumorphicInset(Graphics g, Rectangle rect, int radius)
        {
            // Inset/concave neumorphic effect
            // Light shadow (bottom-right) and dark shadow (top-left) - reversed!

            using (var path = CreateRoundedPath(rect, radius))
            {
                // Fill with slightly darker base
                using (var brush = new SolidBrush(Color.FromArgb(_baseColor.R - 8, _baseColor.G - 8, _baseColor.B - 8)))
                {
                    g.FillPath(brush, path);
                }

                // Inner dark shadow (top-left)
                Rectangle innerRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4);
                using (var innerPath = CreateRoundedPath(innerRect, radius - 2))
                {
                    // Gradient from dark (top-left) to transparent
                    using (var shadowBrush = new LinearGradientBrush(
                        innerRect,
                        Color.FromArgb(50, _darkShadow),
                        Color.FromArgb(0, _darkShadow),
                        135f))
                    {
                        g.FillPath(shadowBrush, innerPath);
                    }
                }

                // Inner light highlight (bottom-right)
                using (var highlightBrush = new LinearGradientBrush(
                    innerRect,
                    Color.FromArgb(0, _lightShadow),
                    Color.FromArgb(40, _lightShadow),
                    135f))
                {
                    using (var innerPath = CreateRoundedPath(innerRect, radius - 2))
                    {
                        g.FillPath(highlightBrush, innerPath);
                    }
                }
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
                // Section header
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

                // Paint content
                PaintMenuItem(g, item, itemRect, context);

                // Badge
                if (context.ItemBadges != null && context.ItemBadges.TryGetValue(item, out string badgeText))
                {
                    Color badgeColor = context.ItemBadgeColors != null && context.ItemBadgeColors.TryGetValue(item, out Color bc) ? bc : Color.FromArgb(200, 100, 100);
                    PaintBadge(g, itemRect, badgeText, badgeColor, context);
                }

                currentY += context.ItemHeight + 8; // More spacing for neumorphism

                // Divider
                if (context.DividerPositions != null && context.DividerPositions.Contains(itemIndex))
                {
                    Rectangle dividerRect = new Rectangle(bounds.Left, currentY, bounds.Width, 8);
                    PaintDivider(g, dividerRect, context);
                    currentY += 16;
                }

                // Children
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
            int iconPadding = 14;
            int x = itemRect.X + iconPadding;

            // Draw icon
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);

                Color iconColor = context.UseThemeColors && context.Theme != null
                    ? (item == context.SelectedItem ? context.Theme.PrimaryColor : context.Theme.SideMenuForeColor)
                    : (item == context.SelectedItem ? Color.FromArgb(100, 120, 180) : Color.FromArgb(100, 100, 110));

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
                    : (item == context.SelectedItem ? Color.FromArgb(80, 100, 150) : Color.FromArgb(80, 80, 90));

                int expandIconSpace = (item.Children != null && item.Children.Count > 0) ? 32 : 8;
                Rectangle textRect = new Rectangle(x, itemRect.Y, Math.Max(0, itemRect.Right - x - expandIconSpace), itemRect.Height);

                using (var brush = new SolidBrush(textColor))
                {
                    var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                    g.DrawString(item.Text, _itemFont, brush, textRect, format);
                }

                // Expand icon
                if (item.Children != null && item.Children.Count > 0)
                {
                    int expandSize = 18;
                    Rectangle expandRect = new Rectangle(itemRect.Right - expandSize - 10, itemRect.Y + (itemRect.Height - expandSize) / 2, expandSize, expandSize);
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

                // Connector
                PaintAccordionConnector(g, parentItem, parentRect, child, childRect, indentLevel, context);

                // States
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

                PaintChildItem(g, child, childRect, context, indentLevel);

                currentY += context.ChildItemHeight + 6;

                // Nested
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
            int iconPadding = 10;
            int x = childRect.X + iconPadding;

            // Icon
            if (!string.IsNullOrEmpty(childItem.ImagePath))
            {
                Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);

                Color iconColor = context.UseThemeColors && context.Theme != null
                    ? Color.FromArgb(160, context.Theme.SideMenuForeColor)
                    : Color.FromArgb(120, 120, 130);

                try
                {
                    StyledImagePainter.PaintWithTint(g, iconRect, childItem.ImagePath, iconColor);
                }
                catch { }

                x += iconSize + iconPadding;
            }

            // Text
            Color textColor = context.UseThemeColors && context.Theme != null
                ? (childItem == context.SelectedItem ? context.Theme.PrimaryColor : Color.FromArgb(160, context.Theme.SideMenuForeColor))
                : (childItem == context.SelectedItem ? Color.FromArgb(80, 100, 150) : Color.FromArgb(100, 100, 110));

            int expandIconSpace = (childItem.Children != null && childItem.Children.Count > 0) ? 26 : 4;
            Rectangle textRect = new Rectangle(x, childRect.Y, Math.Max(0, childRect.Right - x - expandIconSpace), childRect.Height);

            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                g.DrawString(childItem.Text, _childFont, brush, textRect, format);
            }

            // Nested expand icon
            if (childItem.Children != null && childItem.Children.Count > 0)
            {
                int expandSize = 14;
                Rectangle expandRect = new Rectangle(childRect.Right - expandSize - 8, childRect.Y + (childRect.Height - expandSize) / 2, expandSize, expandSize);
                bool isExpanded = context.ExpandedState.TryGetValue(childItem, out bool exp) && exp;
                PaintExpandIcon(g, expandRect, isExpanded, childItem, context);
            }
        }

        #endregion

        #region State Painting (DISTINCT - Neumorphic)

        public void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Neumorphic selection: INSET (pressed/concave) appearance
            PaintNeumorphicInset(g, itemRect, 14);

            // Accent indicator on left
            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : Color.FromArgb(100, 120, 180);

            Rectangle accentBar = new Rectangle(itemRect.X + 4, itemRect.Y + 8, 3, itemRect.Height - 16);
            using (var path = CreateRoundedPath(accentBar, 2))
            using (var brush = new SolidBrush(accentColor))
            {
                g.FillPath(brush, path);
            }
        }

        public void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Neumorphic hover: RAISED (convex) appearance
            PaintNeumorphicRaised(g, itemRect, 14);
        }

        public void PaintPressed(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Neumorphic pressed: deeper INSET
            using (var path = CreateRoundedPath(itemRect, 14))
            {
                // Darker fill
                using (var brush = new SolidBrush(Color.FromArgb(_baseColor.R - 15, _baseColor.G - 15, _baseColor.B - 15)))
                {
                    g.FillPath(brush, path);
                }

                // Stronger inner shadow
                Rectangle innerRect = new Rectangle(itemRect.X + 3, itemRect.Y + 3, itemRect.Width - 6, itemRect.Height - 6);
                using (var shadowBrush = new LinearGradientBrush(innerRect, Color.FromArgb(70, _darkShadow), Color.FromArgb(0, _darkShadow), 135f))
                using (var innerPath = CreateRoundedPath(innerRect, 12))
                {
                    g.FillPath(shadowBrush, innerPath);
                }
            }
        }

        public void PaintDisabled(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Neumorphic disabled: flat, no shadows
            using (var brush = new SolidBrush(Color.FromArgb(200, _baseColor)))
            {
                g.FillRectangle(brush, itemRect);
            }
        }

        #endregion

        #region Expand Icon (DISTINCT - Soft Plus/Minus)

        public void PaintExpandIcon(Graphics g, Rectangle iconRect, bool isExpanded, SimpleItem item, ISideBarPainterContext context)
        {
            // Neumorphic expand icon: soft plus/minus with subtle shadow
            Color iconColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.SideMenuForeColor
                : Color.FromArgb(120, 120, 130);

            // Draw soft circular background
            using (var bgBrush = new SolidBrush(Color.FromArgb(30, _darkShadow)))
            {
                g.FillEllipse(bgBrush, iconRect.X + 1, iconRect.Y + 1, iconRect.Width - 2, iconRect.Height - 2);
            }

            // Draw plus or minus
            int cx = iconRect.X + iconRect.Width / 2;
            int cy = iconRect.Y + iconRect.Height / 2;
            int lineLength = iconRect.Width / 3;

            using (var pen = new Pen(iconColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                // Horizontal line (always)
                g.DrawLine(pen, cx - lineLength, cy, cx + lineLength, cy);

                // Vertical line (only when collapsed - making a plus)
                if (!isExpanded)
                {
                    g.DrawLine(pen, cx, cy - lineLength, cx, cy + lineLength);
                }
            }
        }

        #endregion

        #region Accordion Connector (DISTINCT - Soft Indentation)

        public void PaintAccordionConnector(Graphics g, SimpleItem parent, Rectangle parentRect, SimpleItem child, Rectangle childRect, int indentLevel, ISideBarPainterContext context)
        {
            // Neumorphic connector: soft inset channel
            int indent = context.IndentationWidth * indentLevel;
            int channelX = childRect.X - indent / 2 - 2;
            int channelWidth = 4;

            // Draw soft inset channel
            Rectangle channelRect = new Rectangle(channelX, childRect.Y, channelWidth, childRect.Height);

            using (var shadowBrush = new LinearGradientBrush(
                channelRect,
                Color.FromArgb(30, _darkShadow),
                Color.FromArgb(0, _darkShadow),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(shadowBrush, channelRect);
            }

            // Small dot at child level
            int dotSize = 6;
            int dotX = channelX + channelWidth / 2 - dotSize / 2;
            int dotY = childRect.Y + childRect.Height / 2 - dotSize / 2;

            using (var dotBrush = new SolidBrush(Color.FromArgb(80, _darkShadow)))
            {
                g.FillEllipse(dotBrush, dotX, dotY, dotSize, dotSize);
            }
        }

        #endregion

        #region Badge, Section Header, Divider (DISTINCT)

        public void PaintBadge(Graphics g, Rectangle itemRect, string badgeText, Color badgeColor, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(badgeText)) return;

            var textSize = g.MeasureString(badgeText, _badgeFont);
            int badgeWidth = Math.Max(20, (int)textSize.Width + 10);
            int badgeHeight = 20;
            int badgeX = itemRect.Right - badgeWidth - 32;
            int badgeY = itemRect.Y + (itemRect.Height - badgeHeight) / 2;

            Rectangle badgeRect = new Rectangle(badgeX, badgeY, badgeWidth, badgeHeight);

            // Neumorphic raised badge
            using (var path = CreateRoundedPath(badgeRect, badgeHeight / 2))
            {
                // Shadow
                Rectangle shadowRect = new Rectangle(badgeRect.X + 2, badgeRect.Y + 2, badgeRect.Width, badgeRect.Height);
                using (var shadowPath = CreateRoundedPath(shadowRect, badgeHeight / 2))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(40, _darkShadow)))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }

                // Badge fill
                using (var brush = new SolidBrush(badgeColor))
                {
                    g.FillPath(brush, path);
                }
            }

            // Text
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
                : Color.FromArgb(110, 110, 120);

            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(headerText.ToUpperInvariant(), _headerFont, brush, headerRect, format);
            }
        }

        public void PaintDivider(Graphics g, Rectangle dividerRect, ISideBarPainterContext context)
        {
            int y = dividerRect.Y + dividerRect.Height / 2;
            int padding = 24;

            // Neumorphic divider: inset line
            using (var darkPen = new Pen(Color.FromArgb(40, _darkShadow), 1f))
            {
                g.DrawLine(darkPen, dividerRect.X + padding, y, dividerRect.Right - padding, y);
            }

            using (var lightPen = new Pen(Color.FromArgb(60, _lightShadow), 1f))
            {
                g.DrawLine(lightPen, dividerRect.X + padding, y + 1, dividerRect.Right - padding, y + 1);
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
                _disposed = true;
            }
        }

        #endregion
    }
}

