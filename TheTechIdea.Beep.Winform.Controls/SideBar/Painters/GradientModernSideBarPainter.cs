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
    public sealed class GradientModernSideBarPainter : BaseSideBarPainter
    {
        // Use the context's cached BeepImage to reduce parsing and GC churn
        public override string Name => "GradientModern";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            
            // Gradient background - Modern purple to blue
            Color backColor1 = context.UseThemeColors && context.Theme != null ? context.Theme.SideMenuBackColor : Color.FromArgb(139, 92, 246);
            Color backColor2 = context.UseThemeColors && context.Theme != null ? Color.FromArgb(backColor1.R - 50, backColor1.G, backColor1.B + 50) : Color.FromArgb(59, 130, 246);
            
            using (var brush = new LinearGradientBrush(bounds, backColor1, backColor2, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, bounds);
            }
            
            // Add subtle overlay texture
            Random rand = new Random(54321);
            for (int i = 0; i < 150; i++)
            {
                int x = rand.Next(bounds.Left, bounds.Right);
                int y = rand.Next(bounds.Top, bounds.Bottom);
                Color noiseColor = Color.FromArgb(10, 255, 255, 255);
                using (var brush = new SolidBrush(noiseColor)) { g.FillRectangle(brush, x, y, 1, 1); }
            }
            
            int padding = 12, currentY = bounds.Top + padding + 8;
            if (context.ShowToggleButton) 
            { 
                Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight); 
                PaintToggleButton(g, toggleRect, context); 
                currentY += context.ItemHeight + 12; 
            }
            PaintMenuItems(g, bounds, context, ref currentY);
        }

        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            Color buttonColor1 = context.UseThemeColors && context.Theme != null ? Color.FromArgb(255, 255, 255, 200) : Color.FromArgb(255, 255, 255, 180);
            Color buttonColor2 = context.UseThemeColors && context.Theme != null ? Color.FromArgb(255, 255, 255, 150) : Color.FromArgb(255, 255, 255, 130);
            
            using (var path = CreateRoundedPath(toggleRect, 12))
            using (var brush = new LinearGradientBrush(toggleRect, buttonColor1, buttonColor2, LinearGradientMode.Vertical))
            {
                g.FillPath(brush, path);
            }
            
            // Inner glow
            using (var path = CreateRoundedPath(new Rectangle(toggleRect.X + 2, toggleRect.Y + 2, toggleRect.Width - 4, toggleRect.Height / 2), 10))
            using (var glowBrush = new SolidBrush(Color.FromArgb(80, 255, 255, 255)))
            {
                g.FillPath(glowBrush, path);
            }
            
            Color iconColor = Color.FromArgb(139, 92, 246);
            int iconW = Math.Min(22, Math.Max(12, toggleRect.Width - 12));
            int iconH = Math.Min(14, Math.Max(10, toggleRect.Height - 8));
            var iconRect = new Rectangle(toggleRect.X + (toggleRect.Width - iconW) / 2, toggleRect.Y + (toggleRect.Height - iconH) / 2, iconW, iconH);
            DrawHamburgerIcon(g, iconRect, iconColor);
        }

        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color selectionColor1 = Color.FromArgb(255, 255, 255, 220);
            Color selectionColor2 = Color.FromArgb(255, 255, 255, 180);
            
            using (var path = CreateRoundedPath(itemRect, 12))
            using (var brush = new LinearGradientBrush(itemRect, selectionColor1, selectionColor2, LinearGradientMode.Vertical))
            {
                g.FillPath(brush, path);
            }
            
            // Top highlight
            Rectangle highlightRect = new Rectangle(itemRect.X + 3, itemRect.Y + 2, itemRect.Width - 6, itemRect.Height / 3);
            using (var path = CreateRoundedPath(highlightRect, 10))
            using (var highlightBrush = new SolidBrush(Color.FromArgb(50, 255, 255, 255)))
            {
                g.FillPath(highlightBrush, path);
            }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color hoverColor1 = Color.FromArgb(255, 255, 255, 30);
            Color hoverColor2 = Color.FromArgb(255, 255, 255, 10);
            
            using (var path = CreateRoundedPath(itemRect, 12))
            using (var brush = new LinearGradientBrush(itemRect, hoverColor1, hoverColor2, LinearGradientMode.Vertical))
            {
                g.FillPath(brush, path);
            }
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
                int x = itemRect.X + 12;
                
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, item, iconRect, context);
                    x += iconSize + iconPadding;
                }
                
                if (!context.IsCollapsed)
                {
                    Color textColor = context.UseThemeColors && context.Theme != null 
                        ? (item == context.SelectedItem ? Color.FromArgb(139, 92, 246) : Color.White)
                        : (item == context.SelectedItem ? Color.FromArgb(139, 92, 246) : Color.White);
                    
                    var font = BeepFontManager.GetCachedFont("Inter", 14f, item == context.SelectedItem ? FontStyle.Bold : FontStyle.Regular); 
                    using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, itemRect.Right - x - expandIconSize - 12, itemRect.Height);
                        g.DrawString(item.Text, font, brush, textRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                    }
                }
                
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(itemRect.Right - expandIconSize - 8, itemRect.Y + (itemRect.Height - expandIconSize) / 2, expandIconSize, expandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    Color chevronColor = item == context.SelectedItem ? Color.FromArgb(139, 92, 246) : Color.White;
                    if (context.UseExpandCollapseIcon && !string.IsNullOrEmpty(context.ExpandIconPath) && !string.IsNullOrEmpty(context.CollapseIconPath))
                    {
                        var iconPath = isExpanded ? context.CollapseIconPath : context.ExpandIconPath;
                        try
                        {
                            PaintSvgWithFallback(g, expandRect, iconPath, context.Theme != null && context.UseThemeColors ? chevronColor : (Color?)null, true, context);
                        }
                        catch
                        {
                            using (var pen = new Pen(chevronColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                            {
                                int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2;
                                if (isExpanded) { g.DrawLine(pen, cx - 5, cy - 2, cx, cy + 3); g.DrawLine(pen, cx, cy + 3, cx + 5, cy - 2); }
                                else { g.DrawLine(pen, cx - 2, cy - 5, cx + 3, cy); g.DrawLine(pen, cx + 3, cy, cx - 2, cy + 5); }
                            }
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(chevronColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                        {
                            int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2;
                            if (isExpanded) { g.DrawLine(pen, cx - 5, cy - 2, cx, cy + 3); g.DrawLine(pen, cx, cy + 3, cx + 5, cy - 2); }
                            else { g.DrawLine(pen, cx - 2, cy - 5, cx + 3, cy); g.DrawLine(pen, cx + 3, cy, cx - 2, cy + 5); }
                        }
                    }
                }
                
                currentY += context.ItemHeight + 6;
                if (item.Children != null && item.Children.Count > 0 && context.ExpandedState.ContainsKey(item) && context.ExpandedState[item]) { PaintChildItems(g, bounds, context, item, ref currentY, 1); }
            }
        }

        private void PaintChildItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, SimpleItem parentItem, ref int currentY, int indentLevel)
        {
           
            int childIconSize = GetChildIconSize(context);
            int expandIconSize = GetExpandIconSize(context);
            int childExpandIconSize = GetChildExpandIconSize(context);
            int iconPadding = GetIconPadding(context);
            if (parentItem.Children == null || parentItem.Children.Count == 0 || context.IsCollapsed) return;
            int padding = 8, indent = context.IndentationWidth * indentLevel, iconSize = childIconSize;
            
            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                int x = childRect.X + 10;
                
                Color lineColor = Color.FromArgb(60, 255, 255, 255);
                using (var pen = new Pen(lineColor, 1f)) { int lineX = bounds.Left + (indent / 2); g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2); }
                
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, child, iconRect, context);
                    x += iconSize + iconPadding;
                }
                
                Color textColor = child == context.SelectedItem ? Color.FromArgb(139, 92, 246) : Color.FromArgb(240, 240, 240);
                var font = BeepFontManager.GetCachedFont("Inter", 12f, FontStyle.Regular);
                using (var brush = new SolidBrush(textColor))
                {
                    Rectangle textRect = new Rectangle(x, childRect.Y, Math.Max(0, childRect.Right - x - childExpandIconSize - 12), childRect.Height);
                    g.DrawString(child.Text, font, brush, textRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                }
                
                currentY += context.ChildItemHeight + 4;
                if (child.Children != null && child.Children.Count > 0 && context.ExpandedState.ContainsKey(child) && context.ExpandedState[child]) { PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1); }
            }
        }
    }
}
