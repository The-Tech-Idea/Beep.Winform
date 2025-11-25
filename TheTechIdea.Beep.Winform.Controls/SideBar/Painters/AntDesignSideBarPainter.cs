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
    /// AntDesign style painter: Dark sidebar with Ant Design color cues and clear affordances.
    /// </summary>
    public sealed class AntDesignSideBarPainter : BaseSideBarPainter
    {
        // Switched to using BeepImage cache via context.GetCachedIcon to avoid per-paint ImagePainter allocations
        public override string Name => "AntDesign";

        /// <summary>Paint the entire sidebar for Ant Design style</summary>
        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;

            // Ant Design dark sidebar background
            Color backColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.SideMenuBackColor
                : Color.FromArgb(0, 21, 41);

            using (var brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Ant Design border
            Color borderColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BorderColor
                : Color.FromArgb(255, 255, 255, 8);

            using (var pen = new Pen(borderColor, 1f))
            {
                g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }

            int padding = 8;
            int currentY = bounds.Top + padding;

            if (context.ShowToggleButton)
            {
                Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                PaintToggleButton(g, toggleRect, context);
                currentY += context.ItemHeight + 8;
            }

            PaintMenuItems(g, bounds, context, ref currentY);
        }

        /// <summary>Paint the toggle button used in this style</summary>
        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            // Ant Design primary blue
            Color buttonColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : Color.FromArgb(24, 144, 255);

            using (var path = CreateRoundedPath(toggleRect, 2))
            using (var brush = new SolidBrush(buttonColor))
            {
                g.FillPath(brush, path);
            }

            // White icon - use shared hamburger drawing for consistent look and fallback
            Color iconColor = Color.White;
            int iconW = Math.Min(22, Math.Max(12, toggleRect.Width - 12));
            int iconH = Math.Min(14, Math.Max(10, toggleRect.Height - 8));
            var iconRect = new Rectangle(toggleRect.X + (toggleRect.Width - iconW) / 2, toggleRect.Y + (toggleRect.Height - iconH) / 2, iconW, iconH);
            DrawHamburgerIcon(g, iconRect, iconColor);
        }

        /// <summary>Paint selection highlight for an item</summary>
        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Ant Design primary color with light background
            Color selectionColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(230, 247, 255)
                : Color.FromArgb(230, 247, 255);

            // For dark theme, use darker blue
            if (context.Theme != null && context.UseThemeColors)
            {
                selectionColor = Color.FromArgb(24, 144, 255, 20);
            }
            else
            {
                selectionColor = Color.FromArgb(24, 144, 255, 15);
            }

            using (var brush = new SolidBrush(selectionColor))
            {
                g.FillRectangle(brush, itemRect);
            }

            // Ant Design left accent bar
            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : Color.FromArgb(24, 144, 255);

            Rectangle accentBar = new Rectangle(itemRect.X, itemRect.Y, 3, itemRect.Height);
            using (var brush = new SolidBrush(accentColor))
            {
                g.FillRectangle(brush, accentBar);
            }
        }

        /// <summary>Paint hover state for an item</summary>
        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Ant Design hover state
            Color hoverColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(15, context.Theme.PrimaryColor.R, context.Theme.PrimaryColor.G, context.Theme.PrimaryColor.B)
                : Color.FromArgb(255, 255, 255, 8);

            using (var brush = new SolidBrush(hoverColor))
            {
                g.FillRectangle(brush, itemRect);
            }
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

                int x = itemRect.X + (item == context.SelectedItem ? 8 : 4);

                // Draw icon using StyledImagePainter
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, item, iconRect, context);
                    x += iconSize + iconPadding;
                }

                // Draw text with Ant Design typography
                if (!context.IsCollapsed)
                {
                    Color textColor = context.UseThemeColors && context.Theme != null
                        ? (item == context.SelectedItem ? Color.FromArgb(24, 144, 255) : context.Theme.SideMenuForeColor)
                        : (item == context.SelectedItem ? Color.FromArgb(24, 144, 255) : Color.FromArgb(255, 255, 255, 175));

                    var font = BeepFontManager.GetCachedFont("Microsoft YaHei UI", 14f, FontStyle.Regular);
                    using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, Math.Max(0, itemRect.Right - x - expandIconSize - 12), itemRect.Height);
                        StringFormat format = new StringFormat
                        {
                            Alignment = StringAlignment.Near,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter
                        };

                        g.DrawString(item.Text, font, brush, textRect, format);
                    }
                }

                // Draw expand/collapse chevron
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(itemRect.Right - expandIconSize - 8, itemRect.Y + (itemRect.Height - expandIconSize) / 2, expandIconSize, expandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];

                    Color chevronColor = context.UseThemeColors && context.Theme != null
                        ? context.Theme.SideMenuForeColor
                        : Color.FromArgb(255, 255, 255, 135);

                    if (context.UseExpandCollapseIcon && !string.IsNullOrEmpty(context.ExpandIconPath) && !string.IsNullOrEmpty(context.CollapseIconPath))
                    {
                        var iconPath = isExpanded ? context.CollapseIconPath : context.ExpandIconPath;
                        try
                        {
                            // Use centralized cached rendering first, then fallback to painter if needed
                            PaintSvgWithFallback(g, expandRect, iconPath, context.Theme != null && context.UseThemeColors ? chevronColor : (Color?)null, true, context);
                        }
                        catch
                        {
                            using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                            {
                                int cx = expandRect.X + expandRect.Width / 2;
                                int cy = expandRect.Y + expandRect.Height / 2;

                                if (isExpanded)
                                {
                                    g.DrawLine(pen, cx - 4, cy - 1, cx, cy + 3);
                                    g.DrawLine(pen, cx, cy + 3, cx + 4, cy - 1);
                                }
                                else
                                {
                                    g.DrawLine(pen, cx - 1, cy - 4, cx + 3, cy);
                                    g.DrawLine(pen, cx + 3, cy, cx - 1, cy + 4);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                        {
                            int cx = expandRect.X + expandRect.Width / 2;
                            int cy = expandRect.Y + expandRect.Height / 2;

                            if (isExpanded)
                            {
                                g.DrawLine(pen, cx - 4, cy - 1, cx, cy + 3);
                                g.DrawLine(pen, cx, cy + 3, cx + 4, cy - 1);
                            }
                            else
                            {
                                g.DrawLine(pen, cx - 1, cy - 4, cx + 3, cy);
                                g.DrawLine(pen, cx + 3, cy, cx - 1, cy + 4);
                            }
                        }
                    }
                }

                currentY += context.ItemHeight + 2;

                if (item.Children != null && item.Children.Count > 0 &&
                    context.ExpandedState.ContainsKey(item) && context.ExpandedState[item])
                {
                    PaintChildItems(g, bounds, context, item, ref currentY, 1);
                }
            }
        }

        private void PaintChildItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, SimpleItem parentItem, ref int currentY, int indentLevel)
        {
            if (parentItem.Children == null || parentItem.Children.Count == 0 || context.IsCollapsed) return;

            int padding = 4;
            int indent = context.IndentationWidth * indentLevel;
            int iconSize = 16;
            int iconPadding = 8;
            int childExpandIconSize = GetChildExpandIconSize(context);

            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);

                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);

                int x = childRect.X + (child == context.SelectedItem ? 8 : 4);

                // Draw connector line
                Color lineColor = context.UseThemeColors && context.Theme != null
                    ? Color.FromArgb(40, context.Theme.SideMenuForeColor)
                    : Color.FromArgb(255, 255, 255, 30);

                using (var pen = new Pen(lineColor, 1f))
                {
                    int lineX = bounds.Left + (indent / 2);
                    g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2);
                }

                // Draw icon using cached BeepImage via base helper
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, child, iconRect, context);
                    x += iconSize + iconPadding;
                }

                // Draw text
                Color textColor = context.UseThemeColors && context.Theme != null
                    ? (child == context.SelectedItem ? Color.FromArgb(24, 144, 255) : Color.FromArgb(180, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B))
                    : (child == context.SelectedItem ? Color.FromArgb(24, 144, 255) : Color.FromArgb(255, 255, 255, 145));

                var font = BeepFontManager.GetCachedFont("Microsoft YaHei UI", 12f, FontStyle.Regular);
                using (var brush = new SolidBrush(textColor))
                {
                    Rectangle textRect = new Rectangle(x, childRect.Y, Math.Max(0, childRect.Right - x - childExpandIconSize - 12), childRect.Height);
                    StringFormat format = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };

                    g.DrawString(child.Text, font, brush, textRect, format);
                }

                // Draw expand/collapse chevron for nested children
                if (child.Children != null && child.Children.Count > 0)
                {
                    Rectangle expandRect = new Rectangle(childRect.Right - childExpandIconSize - 8, childRect.Y + (childRect.Height - childExpandIconSize) / 2, childExpandIconSize, childExpandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(child) && context.ExpandedState[child];

                    Color chevronColor = context.UseThemeColors && context.Theme != null
                        ? context.Theme.SideMenuForeColor
                        : Color.FromArgb(255, 255, 255, 135);

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
                                int cx = expandRect.X + expandRect.Width / 2;
                                int cy = expandRect.Y + expandRect.Height / 2;

                                if (isExpanded)
                                {
                                    g.DrawLine(pen, cx - 3, cy - 1, cx, cy + 2);
                                    g.DrawLine(pen, cx, cy + 2, cx + 3, cy - 1);
                                }
                                else
                                {
                                    g.DrawLine(pen, cx - 1, cy - 3, cx + 2, cy);
                                    g.DrawLine(pen, cx + 2, cy, cx - 1, cy + 3);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                        {
                            int cx = expandRect.X + expandRect.Width / 2;
                            int cy = expandRect.Y + expandRect.Height / 2;

                            if (isExpanded)
                            {
                                g.DrawLine(pen, cx - 3, cy - 1, cx, cy + 2);
                                g.DrawLine(pen, cx, cy + 2, cx + 3, cy - 1);
                            }
                            else
                            {
                                g.DrawLine(pen, cx - 1, cy - 3, cx + 2, cy);
                                g.DrawLine(pen, cx + 2, cy, cx - 1, cy + 3);
                            }
                        }
                    }

                    currentY += context.ChildItemHeight + 2;

                    if (child.Children != null && child.Children.Count > 0 &&
                        context.ExpandedState.ContainsKey(child) && context.ExpandedState[child])
                    {
                        PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1);
                    }
                }
            }
        }
    }
}
