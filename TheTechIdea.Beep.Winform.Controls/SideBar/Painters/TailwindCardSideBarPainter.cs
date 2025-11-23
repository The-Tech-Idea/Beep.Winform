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
    /// Tailwind card style painter; uses Tailwind color tokens and card-shaped selections
    /// </summary>
    public sealed class TailwindCardSideBarPainter : BaseSideBarPainter
    {
<<<<<<< HEAD
=======
        private static readonly ImagePainter _imagePainter = new ImagePainter();
        /// <summary>Style name identifier</summary>
>>>>>>> bdb7ce0d65c735a56e2837a4b1bdc571b4d72341
        public override string Name => "TailwindCard";

        /// <summary>
        /// Paints the entire sidebar area using Tailwind card visual style.
        /// </summary>
        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            
            // Tailwind slate background
            Color backColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.SideMenuBackColor 
                : Color.FromArgb(248, 250, 252);
            
            using (var brush = new SolidBrush(backColor)) 
            { 
                g.FillRectangle(brush, bounds); 
            }
            
            // Tailwind border
            Color borderColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.BorderColor 
                : Color.FromArgb(226, 232, 240);
            
            using (var pen = new Pen(borderColor, 1f)) 
            { 
                g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom); 
            }
            
            int padding = 12;
            int currentY = bounds.Top + padding;
            
            if (context.ShowToggleButton) 
            { 
                Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight); 
                PaintToggleButton(g, toggleRect, context); 
                currentY += context.ItemHeight + 12; 
            }
            
            PaintMenuItems(g, bounds, context, ref currentY);
        }

        /// <summary>Paint the toggle button used in the sidebar top area</summary>
        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            // Tailwind blue-600
            Color buttonColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(37, 99, 235);
            
            using (var path = CreateRoundedPath(toggleRect, 8)) 
            using (var brush = new SolidBrush(buttonColor)) 
            { 
                g.FillPath(brush, path); 
            }
            
            // Tailwind shadow-sm
            using (var shadowPath = CreateRoundedPath(new Rectangle(toggleRect.X + 1, toggleRect.Y + 2, toggleRect.Width, toggleRect.Height), 8))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
            {
                g.FillPath(shadowBrush, shadowPath);
            }
            
            using (var path = CreateRoundedPath(toggleRect, 8)) 
            using (var brush = new SolidBrush(buttonColor)) 
            { 
                g.FillPath(brush, path); 
            }
            
            Color iconColor = Color.White;
            using (var pen = new Pen(iconColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                int centerX = toggleRect.X + toggleRect.Width / 2;
                int centerY = toggleRect.Y + toggleRect.Height / 2;
                int lineWidth = 16;
                
                g.DrawLine(pen, centerX - lineWidth / 2, centerY - 5, centerX + lineWidth / 2, centerY - 5);
                g.DrawLine(pen, centerX - lineWidth / 2, centerY, centerX + lineWidth / 2, centerY);
                g.DrawLine(pen, centerX - lineWidth / 2, centerY + 5, centerX + lineWidth / 2, centerY + 5);
            }
        }

        /// <summary>Paint item selection background and highlight</summary>
        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Tailwind card-Style selection with shadow
            Color selectionColor = context.UseThemeColors && context.Theme != null 
                ? Color.White
                : Color.White;
            
            // Draw shadow first
            Rectangle shadowRect = new Rectangle(itemRect.X + 2, itemRect.Y + 2, itemRect.Width, itemRect.Height);
            using (var shadowPath = CreateRoundedPath(shadowRect, 8))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
            {
                g.FillPath(shadowBrush, shadowPath);
            }
            
            // Draw card
            using (var path = CreateRoundedPath(itemRect, 8)) 
            using (var brush = new SolidBrush(selectionColor)) 
            { 
                g.FillPath(brush, path); 
            }
            
            // Draw border
            Color borderColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(226, 232, 240)
                : Color.FromArgb(226, 232, 240);
            
            using (var path = CreateRoundedPath(itemRect, 8))
            using (var pen = new Pen(borderColor, 1f))
            {
                g.DrawPath(pen, path);
            }
            
            // Left accent
            Color accentColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(37, 99, 235);
            
            Rectangle accentBar = new Rectangle(itemRect.X + 2, itemRect.Y + 6, 3, itemRect.Height - 12);
            using (var path = CreateRoundedPath(accentBar, 2))
            using (var brush = new SolidBrush(accentColor)) 
            { 
                g.FillPath(brush, path); 
            }
        }

        /// <summary>Paint item hover state</summary>
        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Tailwind hover state - slate-100
            Color hoverColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(241, 245, 249)
                : Color.FromArgb(241, 245, 249);
            
            using (var path = CreateRoundedPath(itemRect, 8)) 
            using (var brush = new SolidBrush(hoverColor)) 
            { 
                g.FillPath(brush, path); 
            }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;
            
            int padding = 12;
            int iconSize = 20;
            int iconPadding = 12;
            
            foreach (var item in context.Items)
            {
                Rectangle itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                
                if (item == context.HoveredItem) PaintHover(g, itemRect, context);
                if (item == context.SelectedItem) PaintSelection(g, itemRect, context);
                
                int x = itemRect.X + 10;
                
                // Draw icon using ImagePainter
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
<<<<<<< HEAD
                    Color defaultTint = Color.FromArgb(71, 85, 105);
                    Color iconTint = GetEffectiveColor(context, context.Theme?.SideMenuForeColor ?? defaultTint, defaultTint);
                    if (context.Theme != null && item == context.SelectedItem && context.UseThemeColors) iconTint = Color.FromArgb(15, 23, 42);
                    if (context.Theme != null && context.UseThemeColors) StyledImagePainter.PaintWithTint(g, iconRect, item.ImagePath, iconTint);
                    else StyledImagePainter.Paint(g, iconRect, item.ImagePath);
=======
                    _imagePainter.ImagePath = GetIconPath(item, context);
                    
                    if (context.Theme != null && context.UseThemeColors) 
                    { 
                        _imagePainter.CurrentTheme = context.Theme; 
                        _imagePainter.ApplyThemeOnImage = true; 
                        _imagePainter.ImageEmbededin = ImageEmbededin.SideBar; 
                    }
                    
                    _imagePainter.DrawImage(g, iconRect);
>>>>>>> bdb7ce0d65c735a56e2837a4b1bdc571b4d72341
                    x += iconSize + iconPadding;
                }
                
                // Draw text - Inter font (Tailwind default)
                if (!context.IsCollapsed)
                {
                    Color textColor = context.UseThemeColors && context.Theme != null 
                        ? (item == context.SelectedItem ? Color.FromArgb(15, 23, 42) : context.Theme.SideMenuForeColor)
                        : (item == context.SelectedItem ? Color.FromArgb(15, 23, 42) : Color.FromArgb(71, 85, 105));
                    
                    using (var font = new Font("Inter", 14f, item == context.SelectedItem ? FontStyle.Bold : FontStyle.Regular)) 
                    using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, itemRect.Right - x - 10, itemRect.Height);
                        StringFormat format = new StringFormat 
                        { 
                            Alignment = StringAlignment.Near, 
                            LineAlignment = StringAlignment.Center, 
                            Trimming = StringTrimming.EllipsisCharacter 
                        };
                        
                        g.DrawString(item.Text, font, brush, textRect, format);
                    }
                }
                
                // Draw expand/collapse icon
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(itemRect.Right - 22, itemRect.Y + (itemRect.Height - 16) / 2, 16, 16);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    
                    Color chevronColor = context.UseThemeColors && context.Theme != null 
                        ? context.Theme.SideMenuForeColor 
                        : Color.FromArgb(148, 163, 184);
                    
                    using (var pen = new Pen(chevronColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                    {
                        int cx = expandRect.X + expandRect.Width / 2;
                        int cy = expandRect.Y + expandRect.Height / 2;
                        
                        if (isExpanded) 
                        { 
                            g.DrawLine(pen, cx - 4, cy - 2, cx, cy + 2);
                            g.DrawLine(pen, cx, cy + 2, cx + 4, cy - 2);
                        }
                        else 
                        { 
                            g.DrawLine(pen, cx - 2, cy - 4, cx + 2, cy);
                            g.DrawLine(pen, cx + 2, cy, cx - 2, cy + 4);
                        }
                    }
                }
                
                currentY += context.ItemHeight + 6;
                
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
            
            int padding = 8;
            int indent = context.IndentationWidth * indentLevel;
            int iconSize = 18;
            int iconPadding = 10;
            
            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                
                int x = childRect.X + 10;
                
                // Draw connector line - Tailwind Style
                Color lineColor = context.UseThemeColors && context.Theme != null 
                    ? Color.FromArgb(40, context.Theme.SideMenuForeColor)
                    : Color.FromArgb(40, 203, 213, 225);
                
                using (var pen = new Pen(lineColor, 1f)) 
                { 
                    int lineX = bounds.Left + (indent / 2);
                    g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2);
                }
                
                // Draw icon using ImagePainter
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
<<<<<<< HEAD
                    Color defaultTint = Color.FromArgb(100, 116, 139);
                    Color iconTint = GetEffectiveColor(context, context.Theme?.SideMenuForeColor ?? defaultTint, defaultTint);
                    if (context.Theme != null && child == context.SelectedItem && context.UseThemeColors) iconTint = Color.FromArgb(15, 23, 42);
                    if (context.Theme != null && context.UseThemeColors) StyledImagePainter.PaintWithTint(g, iconRect, child.ImagePath, iconTint);
                    else StyledImagePainter.Paint(g, iconRect, child.ImagePath);
=======
                    _imagePainter.ImagePath = GetIconPath(child, context);
                    
                    if (context.Theme != null && context.UseThemeColors) 
                    { 
                        _imagePainter.CurrentTheme = context.Theme; 
                        _imagePainter.ApplyThemeOnImage = true; 
                        _imagePainter.ImageEmbededin = ImageEmbededin.SideBar; 
                    }
                    
                    _imagePainter.DrawImage(g, iconRect);
>>>>>>> bdb7ce0d65c735a56e2837a4b1bdc571b4d72341
                    x += iconSize + iconPadding;
                }
                
                // Draw text
                Color textColor = context.UseThemeColors && context.Theme != null 
                    ? (child == context.SelectedItem ? Color.FromArgb(15, 23, 42) : Color.FromArgb(180, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B))
                    : (child == context.SelectedItem ? Color.FromArgb(15, 23, 42) : Color.FromArgb(100, 116, 139));
                
                using (var font = new Font("Inter", 13f, FontStyle.Regular)) 
                using (var brush = new SolidBrush(textColor))
                {
                    Rectangle textRect = new Rectangle(x, childRect.Y, childRect.Right - x - 8, childRect.Height);
                    StringFormat format = new StringFormat 
                    { 
                        Alignment = StringAlignment.Near, 
                        LineAlignment = StringAlignment.Center, 
                        Trimming = StringTrimming.EllipsisCharacter 
                    };
                    
                    g.DrawString(child.Text, font, brush, textRect, format);
                }
                
                // Draw expand/collapse icon for nested children
                if (child.Children != null && child.Children.Count > 0)
                {
                    Rectangle expandRect = new Rectangle(childRect.Right - 18, childRect.Y + (childRect.Height - 14) / 2, 14, 14);
                    bool isExpanded = context.ExpandedState.ContainsKey(child) && context.ExpandedState[child];
                    
                    Color chevronColor = context.UseThemeColors && context.Theme != null 
                        ? context.Theme.SideMenuForeColor 
                        : Color.FromArgb(148, 163, 184);
                    
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
                
                currentY += context.ChildItemHeight + 3;
                
                if (child.Children != null && child.Children.Count > 0 && 
                    context.ExpandedState.ContainsKey(child) && context.ExpandedState[child]) 
                { 
                    PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1); 
                }
            }
        }
    }
}
