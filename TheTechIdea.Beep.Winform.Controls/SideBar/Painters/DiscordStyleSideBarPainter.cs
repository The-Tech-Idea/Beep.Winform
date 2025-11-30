using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar.Painters
{
    /// <summary>
    /// Discord Style Sidebar Painter
    /// 
    /// DISTINCT FEATURES:
    /// - Dark slate background (Discord's signature dark theme)
    /// - White pill indicator on left edge for selection
    /// - Blurple (Discord blue-purple) accent color
    /// - Category-style grouping with collapsible sections
    /// - Rounded channel/item indicators
    /// - Whitney font family (or fallback)
    /// 
    /// Selection: Dark fill with white left pill indicator
    /// Hover: Slightly lighter background
    /// Expand Icon: Category collapse arrow
    /// Accordion: Channel-style grouping
    /// </summary>
    public sealed class DiscordStyleSideBarPainter : BaseSideBarPainter
    {
        // Discord colors
        private readonly Color _blurple = Color.FromArgb(88, 101, 242);
        private readonly Color _darkBg = Color.FromArgb(47, 49, 54);
        private readonly Color _darkerBg = Color.FromArgb(32, 34, 37);
        private readonly Color _lightText = Color.FromArgb(185, 187, 190);
        private readonly Color _mutedText = Color.FromArgb(142, 146, 151);

        private Font _headerFont;
        private Font _itemFont;
        private Font _childFont;
        private Font _badgeFont;

        private void EnsureFonts()
        {
            _headerFont ??= BeepFontManager.GetCachedFont("Whitney", 11f, FontStyle.Bold);
            _itemFont ??= BeepFontManager.GetCachedFont("Whitney", 14f, FontStyle.Regular);
            _childFont ??= BeepFontManager.GetCachedFont("Whitney", 12f, FontStyle.Regular);
            _badgeFont ??= BeepFontManager.GetCachedFont("Whitney", 9f, FontStyle.Bold);
        }

        public override string Name => "DiscordStyle";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            Color backColor = context.UseThemeColors && context.Theme != null ? context.Theme.SideMenuBackColor : Color.FromArgb(47, 49, 54);
            using (var brush = new SolidBrush(backColor)) { g.FillRectangle(brush, bounds); }
            int padding = 8, currentY = bounds.Top + padding;
            if (context.ShowToggleButton) { Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight); PaintToggleButton(g, toggleRect, context); currentY += context.ItemHeight + 8; }
            PaintMenuItems(g, bounds, context, ref currentY);
        }

        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            Color buttonColor = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : Color.FromArgb(88, 101, 242);
            using (var path = CreateRoundedPath(toggleRect, 4)) using (var brush = new SolidBrush(buttonColor)) { g.FillPath(brush, path); }
            // Use the shared DrawHamburgerIcon helper
            Color iconColor = Color.White;
            int iconW = Math.Min(22, Math.Max(12, toggleRect.Width - 12));
            int iconH = Math.Min(14, Math.Max(10, toggleRect.Height - 8));
            var iconRect = new Rectangle(toggleRect.X + (toggleRect.Width - iconW) / 2, toggleRect.Y + (toggleRect.Height - iconH) / 2, iconW, iconH);
            DrawHamburgerIcon(g, iconRect, iconColor);
        }

        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color selectionColor = context.UseThemeColors && context.Theme != null ? Color.FromArgb(66, 70, 77) : Color.FromArgb(66, 70, 77);
            using (var path = CreateRoundedPath(itemRect, 4)) using (var brush = new SolidBrush(selectionColor)) { g.FillPath(brush, path); }
            Color accentColor = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : Color.FromArgb(88, 101, 242);
            Rectangle accentBar = new Rectangle(itemRect.X, itemRect.Y, 4, itemRect.Height);
            using (var path = CreateRoundedPath(accentBar, 2)) using (var brush = new SolidBrush(accentColor)) { g.FillPath(brush, path); }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color hoverColor = context.UseThemeColors && context.Theme != null ? Color.FromArgb(54, 57, 63) : Color.FromArgb(54, 57, 63);
            using (var path = CreateRoundedPath(itemRect, 4)) using (var brush = new SolidBrush(hoverColor)) { g.FillPath(brush, path); }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;
            int padding = 8;
            int iconSize = GetTopLevelIconSize(context);
            int childIconSize = GetChildIconSize(context);
            int expandIconSize = GetExpandIconSize(context);
            int childExpandIconSize = GetChildExpandIconSize(context);
            int iconPadding = GetIconPadding(context);
            
            foreach (var item in context.Items)
            {
                Rectangle itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                if (item == context.HoveredItem) PaintHover(g, itemRect, context);
                if (item == context.SelectedItem) PaintSelection(g, itemRect, context);
                int x = itemRect.X + 8;
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, item, iconRect, context);
                    x += iconSize + iconPadding;
                }
                if (!context.IsCollapsed)
                {
                    Color textColor = context.UseThemeColors && context.Theme != null ? (item == context.SelectedItem ? Color.White : context.Theme.SideMenuForeColor) : (item == context.SelectedItem ? Color.White : Color.FromArgb(185, 187, 190));
                    var font = BeepFontManager.GetCachedFont("Whitney", 14f, item == context.SelectedItem ? FontStyle.Bold : FontStyle.Regular);
                    using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, Math.Max(0, itemRect.Right - x - expandIconSize - 12), itemRect.Height);
                        g.DrawString(item.Text, font, brush, textRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                    }
                }
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(itemRect.Right - expandIconSize - 8, itemRect.Y + (itemRect.Height - expandIconSize) / 2, expandIconSize, expandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    Color chevronColor = context.UseThemeColors && context.Theme != null ? context.Theme.SideMenuForeColor : Color.FromArgb(185, 187, 190);
                    if (context.UseExpandCollapseIcon && !string.IsNullOrEmpty(context.ExpandIconPath) && !string.IsNullOrEmpty(context.CollapseIconPath))
                    {
                        var iconPath = isExpanded ? context.CollapseIconPath : context.ExpandIconPath;
                        try
                        {
                            // prefer StyledImagePainter with fallback
                            PaintSvgWithFallback(g, expandRect, iconPath, context.Theme != null && context.UseThemeColors ? chevronColor : (Color?)null, true, context);
                        }
                        catch
                        {
                            using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                            {
                                int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2;
                                if (isExpanded) { g.DrawLine(pen, cx - 4, cy - 1, cx, cy + 3); g.DrawLine(pen, cx, cy + 3, cx + 4, cy - 1); }
                                else { g.DrawLine(pen, cx - 1, cy - 4, cx + 3, cy); g.DrawLine(pen, cx + 3, cy, cx - 1, cy + 4); }
                            }
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                        {
                            int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2;
                            if (isExpanded) { g.DrawLine(pen, cx - 4, cy - 1, cx, cy + 3); g.DrawLine(pen, cx, cy + 3, cx + 4, cy - 1); }
                            else { g.DrawLine(pen, cx - 1, cy - 4, cx + 3, cy); g.DrawLine(pen, cx + 3, cy, cx - 1, cy + 4); }
                        }
                    }
                }
                currentY += context.ItemHeight + 2;
                if (item.Children != null && item.Children.Count > 0 && context.ExpandedState.ContainsKey(item) && context.ExpandedState[item]) { PaintChildItems(g, bounds, context, item, ref currentY, 1); }
            }
        }

        private void PaintChildItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, SimpleItem parentItem, ref int currentY, int indentLevel)
        {
            int indent = context.IndentationWidth * indentLevel;
            int iconSize = GetTopLevelIconSize(context);
            int childIconSize = GetChildIconSize(context);
            int expandIconSize = GetExpandIconSize(context);
            int childExpandIconSize = GetChildExpandIconSize(context);
            int iconPadding = GetIconPadding(context);
            if (parentItem.Children == null || parentItem.Children.Count == 0 || context.IsCollapsed) return;
            int padding = 4;
            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                int x = childRect.X + 8;
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, child, iconRect, context);
                    x += iconSize + iconPadding;
                }
                Color textColor = context.UseThemeColors && context.Theme != null ? (child == context.SelectedItem ? Color.White : Color.FromArgb(180, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B)) : (child == context.SelectedItem ? Color.White : Color.FromArgb(142, 146, 151));
                var font = BeepFontManager.GetCachedFont("Whitney", 12f, FontStyle.Regular);
                using (var brush = new SolidBrush(textColor))
                {
                    Rectangle textRect = new Rectangle(x, childRect.Y, Math.Max(0, childRect.Right - x - childExpandIconSize - 12), childRect.Height);
                    g.DrawString(child.Text, font, brush, textRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                }
                currentY += context.ChildItemHeight + 2;
                if (child.Children != null && child.Children.Count > 0)
                {
                    Rectangle expandRect = new Rectangle(childRect.Right - childExpandIconSize - 8, childRect.Y + (childRect.Height - childExpandIconSize) / 2, childExpandIconSize, childExpandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(child) && context.ExpandedState[child];
                    Color chevronColor = context.UseThemeColors && context.Theme != null ? context.Theme.SideMenuForeColor : Color.FromArgb(142, 146, 151);
                    if (context.UseExpandCollapseIcon && !string.IsNullOrEmpty(context.ExpandIconPath) && !string.IsNullOrEmpty(context.CollapseIconPath))
                    {
                        var iconPath = isExpanded ? context.CollapseIconPath : context.ExpandIconPath;
                        try
                        {
                            PaintSvgWithFallback(g, expandRect, iconPath, context.Theme != null && context.UseThemeColors ? chevronColor : (Color?)null, true, context);
                        }
                        catch
                        {
                            using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                            {
                                int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2;
                                if (isExpanded) { g.DrawLine(pen, cx - 3, cy - 1, cx, cy + 2); g.DrawLine(pen, cx, cy + 2, cx + 3, cy - 1); }
                                else { g.DrawLine(pen, cx - 1, cy - 3, cx + 2, cy); g.DrawLine(pen, cx + 2, cy, cx - 1, cy + 3); }
                            }
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                        {
                            int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2;
                            if (isExpanded) { g.DrawLine(pen, cx - 3, cy - 1, cx, cy + 2); g.DrawLine(pen, cx, cy + 2, cx + 3, cy - 1); }
                            else { g.DrawLine(pen, cx - 1, cy - 3, cx + 2, cy); g.DrawLine(pen, cx + 2, cy, cx - 1, cy + 3); }
                        }
                    }

                    if (context.ExpandedState.ContainsKey(child) && context.ExpandedState[child]) { PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1); }
                }
            }
        }

        #region Discord Distinct Implementations

        /// <summary>
        /// Discord pressed state: darker background
        /// </summary>
        public override void PaintPressed(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            using (var path = CreateRoundedPath(itemRect, 4))
            using (var brush = new SolidBrush(Color.FromArgb(79, 84, 92)))
            {
                g.FillPath(brush, path);
            }
        }

        /// <summary>
        /// Discord disabled state: very muted
        /// </summary>
        public override void PaintDisabled(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            using (var brush = new SolidBrush(Color.FromArgb(50, 72, 75, 78)))
            {
                g.FillRectangle(brush, itemRect);
            }
        }

        /// <summary>
        /// Discord expand icon: category collapse arrow
        /// </summary>
        public override void PaintExpandIcon(Graphics g, Rectangle iconRect, bool isExpanded, SimpleItem item, ISideBarPainterContext context)
        {
            EnsureFonts();
            Color iconColor = _mutedText;

            // Discord uses small triangular arrows
            int cx = iconRect.X + iconRect.Width / 2;
            int cy = iconRect.Y + iconRect.Height / 2;
            int size = 4;

            using (var brush = new SolidBrush(iconColor))
            {
                Point[] triangle;
                if (isExpanded)
                {
                    // Down arrow
                    triangle = new Point[]
                    {
                        new Point(cx - size, cy - size / 2),
                        new Point(cx + size, cy - size / 2),
                        new Point(cx, cy + size / 2)
                    };
                }
                else
                {
                    // Right arrow
                    triangle = new Point[]
                    {
                        new Point(cx - size / 2, cy - size),
                        new Point(cx - size / 2, cy + size),
                        new Point(cx + size / 2, cy)
                    };
                }
                g.FillPolygon(brush, triangle);
            }
        }

        /// <summary>
        /// Discord accordion connector: none (Discord uses indentation only)
        /// </summary>
        public override void PaintAccordionConnector(Graphics g, SimpleItem parent, Rectangle parentRect, SimpleItem child, Rectangle childRect, int indentLevel, ISideBarPainterContext context)
        {
            // Discord doesn't use connector lines - just indentation
        }

        /// <summary>
        /// Discord badge: blurple pill with white text
        /// </summary>
        public override void PaintBadge(Graphics g, Rectangle itemRect, string badgeText, Color badgeColor, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(badgeText)) return;

            EnsureFonts();
            var textSize = g.MeasureString(badgeText, _badgeFont);
            int badgeWidth = Math.Max(16, (int)textSize.Width + 8);
            int badgeHeight = 16;
            int badgeX = itemRect.Right - badgeWidth - 24;
            int badgeY = itemRect.Y + (itemRect.Height - badgeHeight) / 2;

            Rectangle badgeRect = new Rectangle(badgeX, badgeY, badgeWidth, badgeHeight);

            // Discord-style badge (red for mentions, or custom color)
            Color bgColor = badgeColor.IsEmpty ? Color.FromArgb(237, 66, 69) : badgeColor;

            using (var path = CreateRoundedPath(badgeRect, badgeHeight / 2))
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillPath(brush, path);
            }

            using (var brush = new SolidBrush(Color.White))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(badgeText, _badgeFont, brush, badgeRect, format);
            }
        }

        /// <summary>
        /// Discord section header: category style with uppercase text
        /// </summary>
        public override void PaintSectionHeader(Graphics g, Rectangle headerRect, string headerText, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(headerText)) return;

            EnsureFonts();
            // Discord category headers are uppercase and muted
            using (var brush = new SolidBrush(_mutedText))
            {
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(headerText.ToUpperInvariant(), _headerFont, brush, headerRect, format);
            }
        }

        /// <summary>
        /// Discord divider: subtle line
        /// </summary>
        public override void PaintDivider(Graphics g, Rectangle dividerRect, ISideBarPainterContext context)
        {
            int y = dividerRect.Y + dividerRect.Height / 2;
            int padding = 12;

            using (var pen = new Pen(Color.FromArgb(60, 64, 67, 73), 1f))
            {
                g.DrawLine(pen, dividerRect.X + padding, y, dividerRect.Right - padding, y);
            }
        }

        #endregion
    }
}
