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
    /// Cyberpunk 2077 Inspired Sidebar Painter
    /// 
    /// DISTINCT FEATURES:
    /// - Dark background with neon accent colors
    /// - Glitch effects and scan lines
    /// - Angular/tech-inspired shapes
    /// - Neon glow effects on selection/hover
    /// - Monospace/tech font styling
    /// - Circuit board patterns
    /// - Animated scan line simulation
    /// 
    /// Selection: Neon glow border with glitch effect
    /// Hover: Scan line animation with color shift
    /// Expand Icon: Tech chevron with glow
    /// Accordion: Terminal-style tree lines
    /// </summary>
    public sealed class CyberpunkSideBarPainter : ISideBarPainter
    {
        public string Name => "Cyberpunk";

        private bool _disposed = false;
        private Font _headerFont;
        private Font _itemFont;
        private Font _childFont;
        private Font _badgeFont;

        // Cyberpunk colors
        private readonly Color _bgColor = Color.FromArgb(15, 15, 20);
        private readonly Color _neonCyan = Color.FromArgb(0, 255, 255);
        private readonly Color _neonMagenta = Color.FromArgb(255, 0, 128);
        private readonly Color _neonYellow = Color.FromArgb(255, 255, 0);
        private readonly Color _textColor = Color.FromArgb(200, 200, 210);
        private readonly Color _dimText = Color.FromArgb(100, 100, 120);

        // Glitch effect state
        private static readonly Random _glitchRandom = new Random();

        #region Main Paint Method

        public void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            EnsureFonts();

            // Dark cyberpunk background
            PaintCyberpunkBackground(g, bounds, context);

            int padding = 12;
            int currentY = bounds.Top + padding;

            // Toggle button
            if (context.ShowToggleButton)
            {
                Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                PaintToggleButton(g, toggleRect, context);
                currentY += context.ItemHeight + 16;
            }

            // Menu items
            PaintMenuItems(g, bounds, context, ref currentY);
        }

        private void EnsureFonts()
        {
            // Use monospace/tech fonts for cyberpunk feel
            _headerFont ??= BeepFontManager.GetCachedFont("Consolas", 9f, FontStyle.Bold);
            _itemFont ??= BeepFontManager.GetCachedFont("Consolas", 10f, FontStyle.Regular);
            _childFont ??= BeepFontManager.GetCachedFont("Consolas", 9f, FontStyle.Regular);
            _badgeFont ??= BeepFontManager.GetCachedFont("Consolas", 8f, FontStyle.Bold);
        }

        #endregion

        #region Background Painting

        private void PaintCyberpunkBackground(Graphics g, Rectangle bounds, ISideBarPainterContext context)
        {
            // Deep dark background
            Color bgColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(Math.Min(30, (int)context.Theme.SideMenuBackColor.R), Math.Min(30, (int)context.Theme.SideMenuBackColor.G), Math.Min(35, (int)context.Theme.SideMenuBackColor.B))
                : _bgColor;

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Scan lines effect
            PaintScanLines(g, bounds);

            // Circuit pattern (subtle)
            PaintCircuitPattern(g, bounds, context);

            // Neon border on right edge
            Color neonColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : _neonCyan;

            // Glow effect
            for (int i = 4; i >= 0; i--)
            {
                using (var pen = new Pen(Color.FromArgb(30 - i * 5, neonColor), i + 1))
                {
                    g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
                }
            }

            // Sharp neon line
            using (var pen = new Pen(neonColor, 2f))
            {
                g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }
        }

        private void PaintScanLines(Graphics g, Rectangle bounds)
        {
            // Subtle horizontal scan lines
            using (var pen = new Pen(Color.FromArgb(10, 255, 255, 255), 1f))
            {
                for (int y = bounds.Top; y < bounds.Bottom; y += 3)
                {
                    g.DrawLine(pen, bounds.Left, y, bounds.Right, y);
                }
            }
        }

        private void PaintCircuitPattern(Graphics g, Rectangle bounds, ISideBarPainterContext context)
        {
            // Subtle circuit board pattern
            Color circuitColor = Color.FromArgb(15, context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : _neonCyan);

            using (var pen = new Pen(circuitColor, 1f))
            {
                // Draw some angular lines
                int spacing = 40;
                for (int x = bounds.Left; x < bounds.Right; x += spacing)
                {
                    int y1 = bounds.Top + (x % 80);
                    int y2 = y1 + 20;
                    if (y2 < bounds.Bottom)
                    {
                        g.DrawLine(pen, x, y1, x, y2);
                        g.DrawLine(pen, x, y2, x + 10, y2 + 10);
                    }
                }
            }
        }

        #endregion

        #region Toggle Button

        public void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            Color neonColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : _neonCyan;

            // Angular cyberpunk button shape
            using (var path = CreateAngularPath(toggleRect, 6))
            {
                // Dark fill
                using (var brush = new SolidBrush(Color.FromArgb(40, 40, 50)))
                {
                    g.FillPath(brush, path);
                }

                // Neon border with glow
                for (int i = 3; i >= 0; i--)
                {
                    using (var pen = new Pen(Color.FromArgb(40 - i * 10, neonColor), i + 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }

                using (var pen = new Pen(neonColor, 1.5f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Hamburger icon with neon color
            int iconSize = Math.Min(18, toggleRect.Height - 14);
            Rectangle iconRect = new Rectangle(
                toggleRect.X + (toggleRect.Width - iconSize) / 2,
                toggleRect.Y + (toggleRect.Height - iconSize) / 2,
                iconSize, iconSize);

            try
            {
                StyledImagePainter.PaintWithTint(g, iconRect, TheTechIdea.Beep.Icons.Svgs.Menu, neonColor);
            }
            catch
            {
                DrawHamburgerFallback(g, iconRect, neonColor);
            }
        }

        #endregion

        #region Menu Items

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;

            int padding = 12;
            int itemIndex = 0;

            foreach (var item in context.Items)
            {
                // Section header
                if (context.SectionHeaders != null)
                {
                    var sectionHeader = context.SectionHeaders.FirstOrDefault(h => h.BeforeIndex == itemIndex);
                    if (!string.IsNullOrEmpty(sectionHeader.HeaderText))
                    {
                        Rectangle headerRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, 22);
                        PaintSectionHeader(g, headerRect, sectionHeader.HeaderText, context);
                        currentY += 26;
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

                PaintMenuItem(g, item, itemRect, context);

                // Badge
                if (context.ItemBadges != null && context.ItemBadges.TryGetValue(item, out string badgeText))
                {
                    Color badgeColor = context.ItemBadgeColors != null && context.ItemBadgeColors.TryGetValue(item, out Color bc) ? bc : _neonMagenta;
                    PaintBadge(g, itemRect, badgeText, badgeColor, context);
                }

                currentY += context.ItemHeight + 4;

                // Divider
                if (context.DividerPositions != null && context.DividerPositions.Contains(itemIndex))
                {
                    Rectangle dividerRect = new Rectangle(bounds.Left, currentY, bounds.Width, 8);
                    PaintDivider(g, dividerRect, context);
                    currentY += 12;
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
            Color neonColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : _neonCyan;

            int iconSize = 20;
            int iconPadding = 10;
            int x = itemRect.X + iconPadding;

            // Draw icon
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);

                Color iconColor = item == context.SelectedItem ? neonColor : _textColor;

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
                Color textColor = item == context.SelectedItem ? neonColor : _textColor;

                // Glitch effect on selected item text
                if (item == context.SelectedItem && _glitchRandom.Next(100) < 5)
                {
                    // Occasional glitch offset
                    x += _glitchRandom.Next(-2, 3);
                }

                int expandIconSpace = (item.Children != null && item.Children.Count > 0) ? 28 : 8;
                Rectangle textRect = new Rectangle(x, itemRect.Y, Math.Max(0, itemRect.Right - x - expandIconSpace), itemRect.Height);

                using (var brush = new SolidBrush(textColor))
                {
                    var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                    g.DrawString(item.Text, _itemFont, brush, textRect, format);
                }

                // Expand icon
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

                currentY += context.ChildItemHeight + 2;

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
            Color neonColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : _neonCyan;

            int iconSize = 16;
            int iconPadding = 8;
            int x = childRect.X + iconPadding;

            // Icon
            if (!string.IsNullOrEmpty(childItem.ImagePath))
            {
                Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);

                Color iconColor = childItem == context.SelectedItem ? neonColor : _dimText;

                try
                {
                    StyledImagePainter.PaintWithTint(g, iconRect, childItem.ImagePath, iconColor);
                }
                catch { }

                x += iconSize + iconPadding;
            }

            // Text
            Color textColor = childItem == context.SelectedItem ? neonColor : _dimText;

            int expandIconSpace = (childItem.Children != null && childItem.Children.Count > 0) ? 22 : 4;
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
                Rectangle expandRect = new Rectangle(childRect.Right - expandSize - 6, childRect.Y + (childRect.Height - expandSize) / 2, expandSize, expandSize);
                bool isExpanded = context.ExpandedState.TryGetValue(childItem, out bool exp) && exp;
                PaintExpandIcon(g, expandRect, isExpanded, childItem, context);
            }
        }

        #endregion

        #region State Painting (DISTINCT - Cyberpunk Neon)

        public void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color neonColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : _neonCyan;

            // Cyberpunk selection: neon glow border with dark fill
            using (var path = CreateAngularPath(itemRect, 4))
            {
                // Dark fill with subtle gradient
                using (var brush = new LinearGradientBrush(itemRect, Color.FromArgb(50, neonColor), Color.FromArgb(20, neonColor), 0f))
                {
                    g.FillPath(brush, path);
                }

                // Neon glow effect (multiple layers)
                for (int i = 4; i >= 0; i--)
                {
                    using (var pen = new Pen(Color.FromArgb(50 - i * 10, neonColor), i + 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }

                // Sharp neon border
                using (var pen = new Pen(neonColor, 1.5f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Left accent bar
            Rectangle accentBar = new Rectangle(itemRect.X, itemRect.Y + 4, 3, itemRect.Height - 8);
            using (var brush = new SolidBrush(neonColor))
            {
                g.FillRectangle(brush, accentBar);
            }

            // Glow on accent bar
            using (var brush = new SolidBrush(Color.FromArgb(100, neonColor)))
            {
                g.FillRectangle(brush, itemRect.X - 2, itemRect.Y + 4, 7, itemRect.Height - 8);
            }
        }

        public void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color neonColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : _neonCyan;

            // Cyberpunk hover: scan line effect with subtle glow
            using (var path = CreateAngularPath(itemRect, 4))
            {
                // Very subtle fill
                using (var brush = new SolidBrush(Color.FromArgb(20, neonColor)))
                {
                    g.FillPath(brush, path);
                }

                // Scan line effect
                using (var pen = new Pen(Color.FromArgb(30, neonColor), 1f))
                {
                    for (int y = itemRect.Top; y < itemRect.Bottom; y += 2)
                    {
                        g.DrawLine(pen, itemRect.Left, y, itemRect.Right, y);
                    }
                }

                // Subtle border
                using (var pen = new Pen(Color.FromArgb(80, neonColor), 1f))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        public void PaintPressed(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color neonColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : _neonMagenta; // Different color for pressed

            // Cyberpunk pressed: inverted colors, glitch effect
            using (var path = CreateAngularPath(itemRect, 4))
            {
                // Brighter fill
                using (var brush = new SolidBrush(Color.FromArgb(60, neonColor)))
                {
                    g.FillPath(brush, path);
                }

                // Glitch offset lines
                using (var pen = new Pen(Color.FromArgb(100, _neonMagenta), 1f))
                {
                    int glitchOffset = _glitchRandom.Next(-3, 4);
                    g.DrawLine(pen, itemRect.Left + glitchOffset, itemRect.Top + 2, itemRect.Right + glitchOffset, itemRect.Top + 2);
                    g.DrawLine(pen, itemRect.Left - glitchOffset, itemRect.Bottom - 2, itemRect.Right - glitchOffset, itemRect.Bottom - 2);
                }

                // Border
                using (var pen = new Pen(neonColor, 2f))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        public void PaintDisabled(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Cyberpunk disabled: static/noise effect
            using (var brush = new SolidBrush(Color.FromArgb(30, 50, 50, 60)))
            {
                g.FillRectangle(brush, itemRect);
            }

            // Static noise
            using (var brush = new SolidBrush(Color.FromArgb(20, 100, 100, 100)))
            {
                for (int i = 0; i < 20; i++)
                {
                    int x = itemRect.X + _glitchRandom.Next(itemRect.Width);
                    int y = itemRect.Y + _glitchRandom.Next(itemRect.Height);
                    g.FillRectangle(brush, x, y, 2, 1);
                }
            }
        }

        #endregion

        #region Expand Icon (DISTINCT - Tech Chevron with Glow)

        public void PaintExpandIcon(Graphics g, Rectangle iconRect, bool isExpanded, SimpleItem item, ISideBarPainterContext context)
        {
            Color neonColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : _neonCyan;

            // Tech chevron with glow
            int cx = iconRect.X + iconRect.Width / 2;
            int cy = iconRect.Y + iconRect.Height / 2;
            int size = iconRect.Width / 3;

            // Glow effect
            using (var glowPen = new Pen(Color.FromArgb(60, neonColor), 3f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                if (isExpanded)
                {
                    g.DrawLine(glowPen, cx - size, cy - size / 2, cx, cy + size / 2);
                    g.DrawLine(glowPen, cx, cy + size / 2, cx + size, cy - size / 2);
                }
                else
                {
                    g.DrawLine(glowPen, cx - size / 2, cy - size, cx + size / 2, cy);
                    g.DrawLine(glowPen, cx + size / 2, cy, cx - size / 2, cy + size);
                }
            }

            // Sharp chevron
            using (var pen = new Pen(neonColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
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

        #endregion

        #region Accordion Connector (DISTINCT - Terminal Tree Lines)

        public void PaintAccordionConnector(Graphics g, SimpleItem parent, Rectangle parentRect, SimpleItem child, Rectangle childRect, int indentLevel, ISideBarPainterContext context)
        {
            Color neonColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : _neonCyan;

            int indent = context.IndentationWidth * indentLevel;
            int lineX = childRect.X - indent / 2;

            // Terminal-style tree connector
            using (var pen = new Pen(Color.FromArgb(80, neonColor), 1f))
            {
                // Vertical line
                g.DrawLine(pen, lineX, parentRect.Bottom, lineX, childRect.Y + childRect.Height / 2);

                // Horizontal line to child
                g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X - 4, childRect.Y + childRect.Height / 2);
            }

            // Small neon dot at connection
            int dotSize = 4;
            using (var brush = new SolidBrush(neonColor))
            {
                g.FillEllipse(brush, lineX - dotSize / 2, childRect.Y + childRect.Height / 2 - dotSize / 2, dotSize, dotSize);
            }

            // Glow on dot
            using (var brush = new SolidBrush(Color.FromArgb(60, neonColor)))
            {
                g.FillEllipse(brush, lineX - dotSize, childRect.Y + childRect.Height / 2 - dotSize, dotSize * 2, dotSize * 2);
            }
        }

        #endregion

        #region Badge, Section Header, Divider (DISTINCT)

        public void PaintBadge(Graphics g, Rectangle itemRect, string badgeText, Color badgeColor, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(badgeText)) return;

            var textSize = g.MeasureString(badgeText, _badgeFont);
            int badgeWidth = Math.Max(20, (int)textSize.Width + 10);
            int badgeHeight = 18;
            int badgeX = itemRect.Right - badgeWidth - 28;
            int badgeY = itemRect.Y + (itemRect.Height - badgeHeight) / 2;

            Rectangle badgeRect = new Rectangle(badgeX, badgeY, badgeWidth, badgeHeight);

            // Angular cyberpunk badge
            using (var path = CreateAngularPath(badgeRect, 3))
            {
                // Fill
                using (var brush = new SolidBrush(Color.FromArgb(200, badgeColor)))
                {
                    g.FillPath(brush, path);
                }

                // Glow border
                using (var pen = new Pen(Color.FromArgb(150, badgeColor), 2f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Text
            using (var brush = new SolidBrush(Color.Black))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(badgeText, _badgeFont, brush, badgeRect, format);
            }
        }

        public void PaintSectionHeader(Graphics g, Rectangle headerRect, string headerText, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(headerText)) return;

            Color neonColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : _neonCyan;

            // Terminal-style header: "// SECTION_NAME"
            string displayText = $"// {headerText.ToUpperInvariant()}";

            using (var brush = new SolidBrush(Color.FromArgb(150, neonColor)))
            {
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(displayText, _headerFont, brush, headerRect, format);
            }

            // Underline with glow
            int lineY = headerRect.Bottom - 2;
            using (var pen = new Pen(Color.FromArgb(60, neonColor), 1f))
            {
                g.DrawLine(pen, headerRect.X, lineY, headerRect.Right, lineY);
            }
        }

        public void PaintDivider(Graphics g, Rectangle dividerRect, ISideBarPainterContext context)
        {
            Color neonColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : _neonCyan;

            int y = dividerRect.Y + dividerRect.Height / 2;
            int padding = 16;

            // Cyberpunk divider: dashed neon line
            using (var pen = new Pen(Color.FromArgb(80, neonColor), 1f) { DashStyle = DashStyle.Dash, DashPattern = new float[] { 4, 4 } })
            {
                g.DrawLine(pen, dividerRect.X + padding, y, dividerRect.Right - padding, y);
            }

            // Center decoration
            int cx = dividerRect.X + dividerRect.Width / 2;
            using (var brush = new SolidBrush(neonColor))
            {
                g.FillRectangle(brush, cx - 3, y - 1, 6, 2);
            }
        }

        #endregion

        #region Helper Methods

        private GraphicsPath CreateAngularPath(Rectangle rect, int cutSize)
        {
            // Angular/tech shape with cut corners
            var path = new GraphicsPath();

            // Top-left cut
            path.AddLine(rect.Left + cutSize, rect.Top, rect.Right - cutSize, rect.Top);
            // Top-right cut
            path.AddLine(rect.Right - cutSize, rect.Top, rect.Right, rect.Top + cutSize);
            path.AddLine(rect.Right, rect.Top + cutSize, rect.Right, rect.Bottom - cutSize);
            // Bottom-right cut
            path.AddLine(rect.Right, rect.Bottom - cutSize, rect.Right - cutSize, rect.Bottom);
            path.AddLine(rect.Right - cutSize, rect.Bottom, rect.Left + cutSize, rect.Bottom);
            // Bottom-left cut
            path.AddLine(rect.Left + cutSize, rect.Bottom, rect.Left, rect.Bottom - cutSize);
            path.AddLine(rect.Left, rect.Bottom - cutSize, rect.Left, rect.Top + cutSize);
            // Back to start
            path.AddLine(rect.Left, rect.Top + cutSize, rect.Left + cutSize, rect.Top);

            path.CloseFigure();
            return path;
        }

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
            int gap = 3;
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

