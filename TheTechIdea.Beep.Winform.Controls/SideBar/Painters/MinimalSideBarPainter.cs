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
    public sealed class MinimalSideBarPainter : BaseSideBarPainter
    {
        // We rely on BaseSideBarPainter.PaintMenuItemIcon which uses the cached BeepImage approach
        public override string Name => "Minimal";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            
            // Minimal clean white/off-white
            Color backColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.SideMenuBackColor 
                : Color.White;
            
            using (var brush = new SolidBrush(backColor)) 
            { 
                g.FillRectangle(brush, bounds); 
            }
            
            // Very subtle border
            Color borderColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(30, context.Theme.BorderColor)
                : Color.FromArgb(30, 0, 0, 0);
            
            using (var pen = new Pen(borderColor, 1f)) 
            { 
                g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom); 
            }
            
            int padding = 20;
            int currentY = bounds.Top + padding;
            
            if (context.ShowToggleButton) 
            { 
                Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight); 
                PaintToggleButton(g, toggleRect, context); 
                currentY += context.ItemHeight + 16; 
            }
            
            PaintMenuItems(g, bounds, context, ref currentY);
        }

        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            // Minimal black button
            Color buttonColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(32, 32, 32);
            
            using (var brush = new SolidBrush(buttonColor)) 
            { 
                g.FillRectangle(brush, toggleRect); 
            }
            
            // White icon - use shared DrawHamburgerIcon for consistent motif and fallback
            Color iconColor = Color.White;
            int iconW = Math.Min(22, Math.Max(12, toggleRect.Width - 12));
            int iconH = Math.Min(14, Math.Max(10, toggleRect.Height - 8));
            var iconRect = new Rectangle(toggleRect.X + (toggleRect.Width - iconW) / 2, toggleRect.Y + (toggleRect.Height - iconH) / 2, iconW, iconH);
            DrawHamburgerIcon(g, iconRect, iconColor);
        }

        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Minimal subtle selection - very light background
            Color selectionColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(5, context.Theme.PrimaryColor)
                : Color.FromArgb(248, 248, 248);
            
            using (var brush = new SolidBrush(selectionColor)) 
            { 
                g.FillRectangle(brush, itemRect); 
            }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Minimal hover - barely visible
            Color hoverColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(3, context.Theme.PrimaryColor.R, context.Theme.PrimaryColor.G, context.Theme.PrimaryColor.B)
                : Color.FromArgb(252, 252, 252);
            
            using (var brush = new SolidBrush(hoverColor)) 
            { 
                g.FillRectangle(brush, itemRect); 
            }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;

            int padding = 20;
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

                int x = itemRect.X;

                // Draw icon using cached BeepImage via the base painter helper
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, item, iconRect, context);
                    x += iconSize + iconPadding;
                }

                // Draw text - minimal sans-serif
                if (!context.IsCollapsed)
                {
                    Color textColor = context.UseThemeColors && context.Theme != null
                        ? (item == context.SelectedItem ? Color.Black : context.Theme.SideMenuForeColor)
                        : (item == context.SelectedItem ? Color.Black : Color.FromArgb(100, 100, 100));

                    var font = BeepFontManager.GetCachedFont(context.Theme.SideMenuTextFont.FontFamily, context.Theme.SideMenuTextFont.FontSize, item == context.SelectedItem ? FontStyle.Bold : FontStyle.Regular);
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

                // Draw expand/collapse - simple plus/minus
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(itemRect.Right - expandIconSize - 8, itemRect.Y + (itemRect.Height - expandIconSize) / 2, expandIconSize, expandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];

                    Color iconColor = context.UseThemeColors && context.Theme != null
                        ? context.Theme.SideMenuForeColor
                        : Color.FromArgb(150, 150, 150);

                    if (context.UseExpandCollapseIcon && !string.IsNullOrEmpty(context.ExpandIconPath) && !string.IsNullOrEmpty(context.CollapseIconPath))
                    {
                        // Use an image or svg icon if provided
                        var iconPath = isExpanded ? context.CollapseIconPath : context.ExpandIconPath;
                        // Use helper to centralize try/catch and fallback drawing
                        PaintSvgWithFallback(g, expandRect, iconPath, context.Theme != null && context.UseThemeColors ? iconColor : (Color?)null, true);

                        // previous try/catch block preserved as fallback if the helper isn't appropriate
                        if (false)
                        {
                            // Fallback to line-based drawing if the icon fails to render
                            using (var pen = new Pen(iconColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                            {
                                int cx = expandRect.X + expandRect.Width / 2;
                                int cy = expandRect.Y + expandRect.Height / 2;
                                g.DrawLine(pen, cx - 5, cy, cx + 5, cy);
                                if (!isExpanded)
                                {
                                    g.DrawLine(pen, cx, cy - 5, cx, cy + 5);
                                }
                            }
                        }
                        else
                        {
                            using (var pen = new Pen(iconColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                            {
                                int cx = expandRect.X + expandRect.Width / 2;
                                int cy = expandRect.Y + expandRect.Height / 2;

                                // Horizontal line (always)
                                g.DrawLine(pen, cx - 5, cy, cx + 5, cy);

                                // Vertical line (only when collapsed - making a plus)
                                if (!isExpanded)
                                {
                                    g.DrawLine(pen, cx, cy - 5, cx, cy + 5);
                                }
                            }
                        }
                    }

                    currentY += context.ItemHeight + 8;

                    if (item.Children != null && item.Children.Count > 0 &&
                        context.ExpandedState.ContainsKey(item) && context.ExpandedState[item])
                    {
                        PaintChildItems(g, bounds, context, item, ref currentY, 1);
                    }
                }
            }
        }

        private void PaintChildItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, SimpleItem parentItem, ref int currentY, int indentLevel)
        {
            if (parentItem.Children == null || parentItem.Children.Count == 0 || context.IsCollapsed) return;
            
            int padding = 8;
            int indent = context.IndentationWidth * indentLevel;
            int iconSize = GetTopLevelIconSize(context);
            int childIconSize = GetChildIconSize(context);
            int expandIconSize = GetExpandIconSize(context);
            int childExpandIconSize = GetChildExpandIconSize(context);
            int iconPadding = GetIconPadding(context);

            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                
                int x = childRect.X;
                
                // No connector lines in minimal Style - clean
                
                // Draw icon using cached BeepImage via the base painter helper
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, child, iconRect, context);
                    x += iconSize + iconPadding;
                }
                
                // Draw text
                Color textColor = context.UseThemeColors && context.Theme != null 
                    ? (child == context.SelectedItem ? Color.Black : Color.FromArgb(180, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B))
                    : (child == context.SelectedItem ? Color.Black : Color.FromArgb(120, 120, 120));
                
                var font = BeepFontManager.GetCachedFont("Arial", 11f, FontStyle.Regular); 
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
                
                // Draw expand/collapse for nested children
                if (child.Children != null && child.Children.Count > 0)
                {
                    Rectangle expandRect = new Rectangle(childRect.Right - childExpandIconSize - 8, childRect.Y + (childRect.Height - childExpandIconSize) / 2, childExpandIconSize, childExpandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(child) && context.ExpandedState[child];
                    
                    Color iconColor = context.UseThemeColors && context.Theme != null 
                        ? context.Theme.SideMenuForeColor 
                        : Color.FromArgb(150, 150, 150);
                    
                    if (context.UseExpandCollapseIcon && !string.IsNullOrEmpty(context.ExpandIconPath) && !string.IsNullOrEmpty(context.CollapseIconPath))
                    {
                        var iconPath = isExpanded ? context.CollapseIconPath : context.ExpandIconPath;
                        PaintSvgWithFallback(g, expandRect, iconPath, context.Theme != null && context.UseThemeColors ? iconColor : (Color?)null, true);
                        if (false)
                        {
                            using (var pen = new Pen(iconColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                            {
                                int cx = expandRect.X + expandRect.Width / 2;
                                int cy = expandRect.Y + expandRect.Height / 2;
                                g.DrawLine(pen, cx - 4, cy, cx + 4, cy);
                                if (!isExpanded) g.DrawLine(pen, cx, cy - 4, cx, cy + 4);
                            }
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(iconColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                        {
                            int cx = expandRect.X + expandRect.Width / 2;
                            int cy = expandRect.Y + expandRect.Height / 2;
                            
                            // Horizontal line
                            g.DrawLine(pen, cx - 4, cy, cx + 4, cy);
                            
                            // Vertical line when collapsed
                            if (!isExpanded) 
                            { 
                                g.DrawLine(pen, cx, cy - 4, cx, cy + 4);
                            }
                        }
                    }
                }
                
                currentY += context.ChildItemHeight + 4;
                
                if (child.Children != null && child.Children.Count > 0 && 
                    context.ExpandedState.ContainsKey(child) && context.ExpandedState[child]) 
                { 
                    PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1); 
                }
            }
        }
    }
}
