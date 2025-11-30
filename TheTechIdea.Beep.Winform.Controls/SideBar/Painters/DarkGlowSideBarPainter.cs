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
    /// Dark Glow Style Sidebar Painter
    /// 
    /// DISTINCT FEATURES:
    /// - Deep dark background (gaming/creative aesthetic)
    /// - Neon glow effects on selection and hover
    /// - Gradient fills with luminous highlights
    /// - Glowing accent colors (blue by default)
    /// - Inter font family
    /// - Subtle glow propagation to children
    /// 
    /// Selection: Gradient fill with neon glow effect
    /// Hover: Subtle outer glow
    /// Expand Icon: Glowing chevron
    /// Accordion: Glow propagates to children
    /// </summary>
    public sealed class DarkGlowSideBarPainter : BaseSideBarPainter
    {
        private Font _headerFont;
        private Font _itemFont;
        private Font _childFont;
        private Font _badgeFont;

        private void EnsureFonts()
        {
            _headerFont ??= BeepFontManager.GetCachedFont("Inter", 10f, FontStyle.Bold);
            _itemFont ??= BeepFontManager.GetCachedFont("Inter", 13f, FontStyle.Regular);
            _childFont ??= BeepFontManager.GetCachedFont("Inter", 12f, FontStyle.Regular);
            _badgeFont ??= BeepFontManager.GetCachedFont("Inter", 8f, FontStyle.Bold);
        }

        public override string Name => "DarkGlow";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            Color backColor = context.UseThemeColors && context.Theme != null ? context.Theme.SideMenuBackColor : Color.FromArgb(17, 24, 39);
            using (var brush = new SolidBrush(backColor)) { g.FillRectangle(brush, bounds); }
            Color borderColor = context.UseThemeColors && context.Theme != null ? context.Theme.BorderColor : Color.FromArgb(31, 41, 55);
            using (var pen = new Pen(borderColor, 1f)) { g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom); }
            int padding = 12, currentY = bounds.Top + padding;
            if (context.ShowToggleButton) { Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight); PaintToggleButton(g, toggleRect, context); currentY += context.ItemHeight + 12; }
            PaintMenuItems(g, bounds, context, ref currentY);
        }

        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            Color buttonColor = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : Color.FromArgb(59, 130, 246);
            using (var path = CreateRoundedPath(toggleRect, 8)) using (var brush = new LinearGradientBrush(toggleRect, buttonColor, Color.FromArgb(buttonColor.R - 20, buttonColor.G - 20, buttonColor.B), LinearGradientMode.Vertical)) { g.FillPath(brush, path); }
            using (var path = CreateRoundedPath(new Rectangle(toggleRect.X + 1, toggleRect.Y + 1, toggleRect.Width - 2, toggleRect.Height / 2), 7)) using (var glowBrush = new SolidBrush(Color.FromArgb(60, 255, 255, 255))) { g.FillPath(glowBrush, path); }
            Color iconColor = Color.White;
            int iconW = Math.Min(22, Math.Max(12, toggleRect.Width - 12));
            int iconH = Math.Min(14, Math.Max(10, toggleRect.Height - 8));
            var iconRect = new Rectangle(toggleRect.X + (toggleRect.Width - iconW) / 2, toggleRect.Y + (toggleRect.Height - iconH) / 2, iconW, iconH);
            DrawHamburgerIcon(g, iconRect, iconColor);
        }

        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color selectionColor1 = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : Color.FromArgb(59, 130, 246);
            Color selectionColor2 = context.UseThemeColors && context.Theme != null ? Color.FromArgb(selectionColor1.R - 30, selectionColor1.G - 30, selectionColor1.B) : Color.FromArgb(37, 99, 235);
            using (var path = CreateRoundedPath(itemRect, 8)) using (var brush = new LinearGradientBrush(itemRect, selectionColor1, selectionColor2, LinearGradientMode.Vertical)) { g.FillPath(brush, path); }
            using (var glowPath = CreateRoundedPath(new Rectangle(itemRect.X, itemRect.Y - 2, itemRect.Width, 6), 4)) using (var glowBrush = new SolidBrush(Color.FromArgb(40, selectionColor1))) { g.FillPath(glowBrush, glowPath); }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color hoverColor = context.UseThemeColors && context.Theme != null ? Color.FromArgb(31, 41, 55) : Color.FromArgb(31, 41, 55);
            using (var path = CreateRoundedPath(itemRect, 8)) using (var brush = new SolidBrush(hoverColor)) { g.FillPath(brush, path); }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;
            int padding = 12;
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
                int x = itemRect.X + 10;
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, item, iconRect, context);
                    x += iconSize + iconPadding;
                }
                if (!context.IsCollapsed)
                {
                    Color textColor = context.UseThemeColors && context.Theme != null ? (item == context.SelectedItem ? Color.White : context.Theme.SideMenuForeColor) : (item == context.SelectedItem ? Color.White : Color.FromArgb(209, 213, 219));
                    var font = BeepFontManager.GetCachedFont("Inter", 13f, item == context.SelectedItem ? FontStyle.Bold : FontStyle.Regular);
                    using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, Math.Max(0, itemRect.Right - x - expandIconSize - 12), itemRect.Height);
                        g.DrawString(item.Text, font, brush, textRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                    }
                }
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(itemRect.Right - expandIconSize, itemRect.Y + (itemRect.Height - expandIconSize) / 2, expandIconSize, expandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    Color chevronColor = context.UseThemeColors && context.Theme != null ? (item == context.SelectedItem ? Color.White : context.Theme.SideMenuForeColor) : (item == context.SelectedItem ? Color.White : Color.FromArgb(156, 163, 184));
                    if (context.UseExpandCollapseIcon && !string.IsNullOrEmpty(context.ExpandIconPath) && !string.IsNullOrEmpty(context.CollapseIconPath))
                    {
                        var iconPath = isExpanded ? context.CollapseIconPath : context.ExpandIconPath;
                        try
                        {
                            if (context.Theme != null && context.UseThemeColors) StyledImagePainter.PaintWithTint(g, expandRect, iconPath, chevronColor);
                            else StyledImagePainter.Paint(g, expandRect, iconPath);
                        }
                        catch
                        {
                            using (var pen = new Pen(chevronColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                            {
                                int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2;
                                if (isExpanded) { g.DrawLine(pen, cx - 4, cy - 2, cx, cy + 2); g.DrawLine(pen, cx, cy + 2, cx + 4, cy - 2); }
                                else { g.DrawLine(pen, cx - 2, cy - 4, cx + 2, cy); g.DrawLine(pen, cx + 2, cy, cx - 2, cy + 4); }
                            }
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(chevronColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                        {
                            int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2; 
                            if (isExpanded) { g.DrawLine(pen, cx - 4, cy - 2, cx, cy + 2); g.DrawLine(pen, cx, cy + 2, cx + 4, cy - 2); }
                            else { g.DrawLine(pen, cx - 2, cy - 4, cx + 2, cy); g.DrawLine(pen, cx + 2, cy, cx - 2, cy + 4); }
                        }
                    }
                }
                currentY += context.ItemHeight + 4;
                if (item.Children != null && item.Children.Count > 0 && context.ExpandedState.ContainsKey(item) && context.ExpandedState[item]) { PaintChildItems(g, bounds, context, item, ref currentY, 1); }
            }
        }

        private void PaintChildItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, SimpleItem parentItem, ref int currentY, int indentLevel)
        {
            if (parentItem.Children == null || parentItem.Children.Count == 0 || context.IsCollapsed) return;
            int padding = 6, indent = context.IndentationWidth * indentLevel, iconSize = 18, iconPadding = 10;
            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                int x = childRect.X + 10;
                Color lineColor = context.UseThemeColors && context.Theme != null ? Color.FromArgb(50, context.Theme.SideMenuForeColor) : Color.FromArgb(50, 75, 85, 99);
                using (var pen = new Pen(lineColor, 1f)) { int lineX = bounds.Left + (indent / 2); g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2); }
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, child, iconRect, context);
                    x += iconSize + iconPadding;
                }
                Color textColor = context.UseThemeColors && context.Theme != null ? (child == context.SelectedItem ? Color.White : Color.FromArgb(180, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B)) : (child == context.SelectedItem ? Color.White : Color.FromArgb(156, 163, 175));
                var font = BeepFontManager.GetCachedFont("Inter", 12f, FontStyle.Regular);
                using (var brush = new SolidBrush(textColor))
                {
                    Rectangle textRect = new Rectangle(x, childRect.Y, childRect.Right - x - 8, childRect.Height);
                    g.DrawString(child.Text, font, brush, textRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                }
                currentY += context.ChildItemHeight + 3;
                if (child.Children != null && child.Children.Count > 0 && context.ExpandedState.ContainsKey(child) && context.ExpandedState[child]) { PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1); }
            }
        }

        #region DarkGlow Distinct Implementations

        /// <summary>
        /// DarkGlow pressed state: intense glow
        /// </summary>
        public override void PaintPressed(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color glowColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : Color.FromArgb(59, 130, 246);

            // Intense glow background
            using (var path = CreateRoundedPath(itemRect, 8))
            {
                using (var brush = new SolidBrush(Color.FromArgb(80, glowColor)))
                {
                    g.FillPath(brush, path);
                }

                // Outer glow effect
                for (int i = 3; i >= 0; i--)
                {
                    Rectangle glowRect = new Rectangle(itemRect.X - i, itemRect.Y - i, itemRect.Width + i * 2, itemRect.Height + i * 2);
                    using (var glowPath = CreateRoundedPath(glowRect, 8 + i))
                    using (var pen = new Pen(Color.FromArgb(40 - i * 10, glowColor), 1f))
                    {
                        g.DrawPath(pen, glowPath);
                    }
                }
            }
        }

        /// <summary>
        /// DarkGlow disabled state: no glow, dimmed
        /// </summary>
        public override void PaintDisabled(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            using (var brush = new SolidBrush(Color.FromArgb(30, 40, 50, 60)))
            {
                g.FillRectangle(brush, itemRect);
            }
        }

        /// <summary>
        /// DarkGlow expand icon: glowing chevron
        /// </summary>
        public override void PaintExpandIcon(Graphics g, Rectangle iconRect, bool isExpanded, SimpleItem item, ISideBarPainterContext context)
        {
            EnsureFonts();
            Color glowColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : Color.FromArgb(59, 130, 246);

            Color iconColor = Color.FromArgb(156, 163, 184);

            int cx = iconRect.X + iconRect.Width / 2;
            int cy = iconRect.Y + iconRect.Height / 2;
            int size = iconRect.Width / 3;

            // Glow effect
            using (var glowPen = new Pen(Color.FromArgb(40, glowColor), 4f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
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

            // Main chevron
            using (var pen = new Pen(iconColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
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

        /// <summary>
        /// DarkGlow accordion connector: glowing line
        /// </summary>
        public override void PaintAccordionConnector(Graphics g, SimpleItem parent, Rectangle parentRect, SimpleItem child, Rectangle childRect, int indentLevel, ISideBarPainterContext context)
        {
            Color glowColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(30, context.Theme.PrimaryColor)
                : Color.FromArgb(30, 59, 130, 246);

            Color lineColor = Color.FromArgb(50, 75, 85, 99);

            int indent = context.IndentationWidth * indentLevel;
            int lineX = childRect.X - indent / 2;

            // Glow behind line
            using (var pen = new Pen(glowColor, 3f))
            {
                g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2);
            }

            // Main line
            using (var pen = new Pen(lineColor, 1f))
            {
                g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2);
            }
        }

        /// <summary>
        /// DarkGlow badge: glowing pill
        /// </summary>
        public override void PaintBadge(Graphics g, Rectangle itemRect, string badgeText, Color badgeColor, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(badgeText)) return;

            EnsureFonts();
            var textSize = g.MeasureString(badgeText, _badgeFont);
            int badgeWidth = Math.Max(18, (int)textSize.Width + 10);
            int badgeHeight = 18;
            int badgeX = itemRect.Right - badgeWidth - 24;
            int badgeY = itemRect.Y + (itemRect.Height - badgeHeight) / 2;

            Rectangle badgeRect = new Rectangle(badgeX, badgeY, badgeWidth, badgeHeight);

            // Glow effect
            Rectangle glowRect = new Rectangle(badgeRect.X - 2, badgeRect.Y - 2, badgeRect.Width + 4, badgeRect.Height + 4);
            using (var glowPath = CreateRoundedPath(glowRect, (badgeHeight + 4) / 2))
            using (var glowBrush = new SolidBrush(Color.FromArgb(60, badgeColor)))
            {
                g.FillPath(glowBrush, glowPath);
            }

            // Badge fill
            using (var path = CreateRoundedPath(badgeRect, badgeHeight / 2))
            using (var brush = new SolidBrush(badgeColor))
            {
                g.FillPath(brush, path);
            }

            // Text
            using (var brush = new SolidBrush(Color.White))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(badgeText, _badgeFont, brush, badgeRect, format);
            }
        }

        /// <summary>
        /// DarkGlow section header: glowing text
        /// </summary>
        public override void PaintSectionHeader(Graphics g, Rectangle headerRect, string headerText, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(headerText)) return;

            EnsureFonts();
            Color glowColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : Color.FromArgb(59, 130, 246);

            // Glow effect on text
            using (var glowBrush = new SolidBrush(Color.FromArgb(40, glowColor)))
            {
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                // Draw glow (offset)
                Rectangle glowRect = new Rectangle(headerRect.X + 1, headerRect.Y + 1, headerRect.Width, headerRect.Height);
                g.DrawString(headerText.ToUpperInvariant(), _headerFont, glowBrush, glowRect, format);
            }

            // Main text
            using (var brush = new SolidBrush(Color.FromArgb(156, 163, 175)))
            {
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(headerText.ToUpperInvariant(), _headerFont, brush, headerRect, format);
            }
        }

        /// <summary>
        /// DarkGlow divider: glowing line
        /// </summary>
        public override void PaintDivider(Graphics g, Rectangle dividerRect, ISideBarPainterContext context)
        {
            Color glowColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : Color.FromArgb(59, 130, 246);

            int y = dividerRect.Y + dividerRect.Height / 2;
            int padding = 16;

            // Glow
            using (var pen = new Pen(Color.FromArgb(30, glowColor), 3f))
            {
                g.DrawLine(pen, dividerRect.X + padding, y, dividerRect.Right - padding, y);
            }

            // Main line
            using (var pen = new Pen(Color.FromArgb(50, 31, 41, 55), 1f))
            {
                g.DrawLine(pen, dividerRect.X + padding, y, dividerRect.Right - padding, y);
            }
        }

        #endregion
    }
}
