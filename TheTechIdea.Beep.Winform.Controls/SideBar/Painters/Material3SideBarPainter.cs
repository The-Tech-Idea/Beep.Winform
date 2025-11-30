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
    /// Material Design 3 Sidebar Painter
    /// 
    /// DISTINCT FEATURES:
    /// - Tonal color system with primary container colors
    /// - Large rounded pills (28px radius) for selection
    /// - 8% opacity overlay for hover states
    /// - Roboto font family
    /// - Subtle connector lines for accordion
    /// - Material 3 elevation and surface colors
    /// 
    /// Selection: Rounded pill with tonal surface color
    /// Hover: 8% primary color overlay
    /// Expand Icon: Animated chevron with smooth rotation
    /// Accordion: Smooth slide with connector lines
    /// </summary>
    public sealed class Material3SideBarPainter : BaseSideBarPainter
    {
        // Use StyledImagePainter for caching/tinting and better performance
        public override string Name => "Material3";

        // Material 3 specific fonts
        private Font _headerFont;
        private Font _itemFont;
        private Font _childFont;
        private Font _badgeFont;

        private void EnsureFonts()
        {
            _headerFont ??= BeepFontManager.GetCachedFont("Roboto", 11f, FontStyle.Bold);
            _itemFont ??= BeepFontManager.GetCachedFont("Roboto", 14f, FontStyle.Bold);
            _childFont ??= BeepFontManager.GetCachedFont("Roboto", 12f, FontStyle.Regular);
            _badgeFont ??= BeepFontManager.GetCachedFont("Roboto", 9f, FontStyle.Bold);
        }

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            EnsureFonts();
            
            // Material 3 surface color
            Color backColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.SideMenuBackColor 
                : Color.FromArgb(255, 251, 254);
            
            using (var brush = new SolidBrush(backColor)) 
            { 
                g.FillRectangle(brush, bounds); 
            }
            
            // Material 3 outline color
            Color outlineColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.BorderColor 
                : Color.FromArgb(121, 116, 126);
            
            using (var pen = new Pen(outlineColor, 1f)) 
            { 
                g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom); 
            }
            
            int padding = 16;
            int currentY = bounds.Top + padding;
            
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
            // Material 3 primary color (purple)
            Color buttonColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(103, 80, 164);
            
            //using (var path = CreateRoundedPath(toggleRect, 28)) 
            //using (var brush = new SolidBrush(buttonColor)) 
            //{ 
            //    g.FillPath(brush, path); 
            //}
            
            // Hamburger icon using embedded SVG for crisp rendering
            string hamburger = TheTechIdea.Beep.Icons.Svgs.Menu;
            int iconSize = Math.Min(toggleRect.Height - 12, 22);
            var iconRect = new Rectangle(
                toggleRect.X + (toggleRect.Width - iconSize) / 2,
                toggleRect.Y + (toggleRect.Height - iconSize) / 2,
                iconSize,
                iconSize);

            // Use white hamburger icon for contrast on Material primary toggle
            Color iconColor = Color.White;
            DrawHamburgerIcon(g, iconRect, iconColor);
        }

        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Material 3 primary container
            Color selectionColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(234, 221, 255);
            
            using (var path = CreateRoundedPath(itemRect, 28)) 
            using (var brush = new SolidBrush(selectionColor)) 
            { 
                g.FillPath(brush, path); 
            }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Material 3 hover state overlay
            Color hoverColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(8, context.Theme.PrimaryColor.R, context.Theme.PrimaryColor.G, context.Theme.PrimaryColor.B)
                : Color.FromArgb(8, 103, 80, 164);
            
            using (var path = CreateRoundedPath(itemRect, 28)) 
            using (var brush = new SolidBrush(hoverColor)) 
            { 
                g.FillPath(brush, path); 
            }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;
            
            int padding = 16;
            int iconSize = GetTopLevelIconSize(context); // use BaseSideBarPainter helper - top-level icon size
            int childIconSize = GetChildIconSize(context); // helper for child icon size
            int expandIconSize = GetExpandIconSize(context);
            int childExpandIconSize = GetChildExpandIconSize(context);
            int iconPadding = GetIconPadding(context);
            
            foreach (var item in context.Items)
            {
                Rectangle itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                
                if (item == context.HoveredItem) PaintHover(g, itemRect, context);
                if (item == context.SelectedItem) PaintSelection(g, itemRect, context);
                
                int x = itemRect.X + iconPadding;
                
                // Draw icon using ImagePainter
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, item, iconRect, context);
                    x += iconSize + iconPadding;
                }
                
                // Draw text
                    if (!context.IsCollapsed)
                    {
                    // Material 3 on-surface or on-primary-container
                    Color textColor = context.UseThemeColors && context.Theme != null 
                        ? (item == context.SelectedItem ? Color.FromArgb(73, 69, 79) : context.Theme.SideMenuForeColor)
                        : (item == context.SelectedItem ? Color.FromArgb(73, 69, 79) : Color.FromArgb(28, 27, 31));
                    
                    var font = BeepFontManager.GetCachedFont("Roboto", 14f, FontStyle.Bold);
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
                
                // Draw expand/collapse icon
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(itemRect.Right - expandIconSize - 8, itemRect.Y + (itemRect.Height - expandIconSize) / 2, expandIconSize, expandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    
                    Color chevronColor = context.UseThemeColors && context.Theme != null 
                        ? context.Theme.SideMenuForeColor 
                        : Color.FromArgb(73, 69, 79);
                    
                    if (context.UseExpandCollapseIcon && !string.IsNullOrEmpty(context.ExpandIconPath) && !string.IsNullOrEmpty(context.CollapseIconPath))
                    {
                        var iconPath = isExpanded ? context.CollapseIconPath : context.ExpandIconPath;
                        try
                        {
                            // tint expand/collapse icon to match painter chevron color
                            if (context.Theme != null && context.UseThemeColors) StyledImagePainter.PaintWithTint(g, expandRect, iconPath, chevronColor);
                            else StyledImagePainter.Paint(g, expandRect, iconPath);
                        }
                        catch
                        {
                            using (var pen = new Pen(chevronColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                            {
                                int cx = expandRect.X + expandRect.Width / 2;
                                int cy = expandRect.Y + expandRect.Height / 2;
                                
                                if (isExpanded) 
                                { 
                                    g.DrawLine(pen, cx - 5, cy - 2, cx, cy + 3);
                                    g.DrawLine(pen, cx, cy + 3, cx + 5, cy - 2);
                                }
                                else 
                                { 
                                    g.DrawLine(pen, cx - 2, cy - 5, cx + 3, cy);
                                    g.DrawLine(pen, cx + 3, cy, cx - 2, cy + 5);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(chevronColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                        {
                            int cx = expandRect.X + expandRect.Width / 2;
                            int cy = expandRect.Y + expandRect.Height / 2;
                            
                            if (isExpanded) 
                            { 
                                g.DrawLine(pen, cx - 5, cy - 2, cx, cy + 3);
                                g.DrawLine(pen, cx, cy + 3, cx + 5, cy - 2);
                            }
                            else 
                            { 
                                g.DrawLine(pen, cx - 2, cy - 5, cx + 3, cy);
                                g.DrawLine(pen, cx + 3, cy, cx - 2, cy + 5);
                            }
                        }
                    }
                }
                
                currentY += context.ItemHeight + 4;
                
                if (item.Children != null && item.Children.Count > 0 && 
                    context.ExpandedState.ContainsKey(item) && context.ExpandedState[item]) 
                { 
                    PaintChildItems(g, bounds, context, item, ref currentY, 1); 
                }
            }
        }

        private void PaintChildItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, SimpleItem parentItem, ref int currentY, int indentLevel)
        {
            int iconSize = GetTopLevelIconSize(context);
            int childIconSize = GetChildIconSize(context);
            int expandIconSize = GetExpandIconSize(context);
            int childExpandIconSize = GetChildExpandIconSize(context);
            int iconPadding = GetIconPadding(context);

            if (parentItem.Children == null || parentItem.Children.Count == 0 || context.IsCollapsed) return;
            
            int padding = 8;
            int indent = context.IndentationWidth * indentLevel;
          
            
            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                
                int x = childRect.X + iconPadding;
                
                // Draw connector line
                Color lineColor = context.UseThemeColors && context.Theme != null 
                    ? Color.FromArgb(50, context.Theme.SideMenuForeColor)
                    : Color.FromArgb(50, 121, 116, 126);
                
                using (var pen = new Pen(lineColor, 1f)) 
                { 
                    int lineX = bounds.Left + (indent / 2);
                    g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2);
                }
                
                // Draw icon using ImagePainter
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, child, iconRect, context);
                    x += childIconSize + iconPadding;
                }
                
                // Draw text
                Color textColor = context.UseThemeColors && context.Theme != null 
                    ? (child == context.SelectedItem ? Color.FromArgb(73, 69, 79) : Color.FromArgb(180, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B))
                    : (child == context.SelectedItem ? Color.FromArgb(73, 69, 79) : Color.FromArgb(99, 91, 103));
                
                var font = BeepFontManager.GetCachedFont("Roboto", 12f, FontStyle.Regular); 
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
                
                // Draw expand/collapse icon for nested children
                if (child.Children != null && child.Children.Count > 0)
                {
                    Rectangle expandRect = new Rectangle(childRect.Right - childExpandIconSize - 8, childRect.Y + (childRect.Height - childExpandIconSize) / 2, childExpandIconSize, childExpandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(child) && context.ExpandedState[child];
                    
                    Color chevronColor = context.UseThemeColors && context.Theme != null 
                        ? context.Theme.SideMenuForeColor 
                        : Color.FromArgb(99, 91, 103);
                    
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
                            using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
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
                    }
                    else
                    {
                        using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
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
                }
                
                currentY += context.ChildItemHeight + 2;
                
                if (child.Children != null && child.Children.Count > 0 && 
                    context.ExpandedState.ContainsKey(child) && context.ExpandedState[child]) 
                { 
                    PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1); 
                }
            }
        }

        #region Material 3 Distinct Implementations

        /// <summary>
        /// Material 3 pressed state: darker tonal surface
        /// </summary>
        public override void PaintPressed(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color pressedColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(20, context.Theme.PrimaryColor)
                : Color.FromArgb(20, 103, 80, 164);

            using (var path = CreateRoundedPath(itemRect, 28))
            using (var brush = new SolidBrush(pressedColor))
            {
                g.FillPath(brush, path);
            }
        }

        /// <summary>
        /// Material 3 disabled state: reduced opacity
        /// </summary>
        public override void PaintDisabled(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            using (var brush = new SolidBrush(Color.FromArgb(38, 28, 27, 31)))
            {
                g.FillRectangle(brush, itemRect);
            }
        }

        /// <summary>
        /// Material 3 expand icon: smooth animated chevron
        /// </summary>
        public override void PaintExpandIcon(Graphics g, Rectangle iconRect, bool isExpanded, SimpleItem item, ISideBarPainterContext context)
        {
            Color iconColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.SideMenuForeColor
                : Color.FromArgb(73, 69, 79);

            // Try SVG icons first (Material Design icons)
            string iconPath = isExpanded
                ? (context.CollapseIconPath ?? TheTechIdea.Beep.Icons.SvgsUI.ChevronDown)
                : (context.ExpandIconPath ?? TheTechIdea.Beep.Icons.SvgsUI.ChevronRight);

            try
            {
                StyledImagePainter.PaintWithTint(g, iconRect, iconPath, iconColor);
            }
            catch
            {
                // Fallback: Material-style smooth chevron
                using (var pen = new Pen(iconColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
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

        /// <summary>
        /// Material 3 accordion connector: subtle dotted line
        /// </summary>
        public override void PaintAccordionConnector(Graphics g, SimpleItem parent, Rectangle parentRect, SimpleItem child, Rectangle childRect, int indentLevel, ISideBarPainterContext context)
        {
            Color lineColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(50, context.Theme.SideMenuForeColor)
                : Color.FromArgb(50, 121, 116, 126);

            int indent = context.IndentationWidth * indentLevel;
            int lineX = childRect.X - indent / 2;

            using (var pen = new Pen(lineColor, 1f))
            {
                // Horizontal line to child
                g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2);
            }
        }

        /// <summary>
        /// Material 3 badge: pill-shaped with primary color
        /// </summary>
        public override void PaintBadge(Graphics g, Rectangle itemRect, string badgeText, Color badgeColor, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(badgeText)) return;

            EnsureFonts();
            var textSize = g.MeasureString(badgeText, _badgeFont);
            int badgeWidth = Math.Max(20, (int)textSize.Width + 12);
            int badgeHeight = 20;
            int badgeX = itemRect.Right - badgeWidth - 28;
            int badgeY = itemRect.Y + (itemRect.Height - badgeHeight) / 2;

            Rectangle badgeRect = new Rectangle(badgeX, badgeY, badgeWidth, badgeHeight);

            // Material 3 pill badge
            using (var path = CreateRoundedPath(badgeRect, badgeHeight / 2))
            using (var brush = new SolidBrush(badgeColor))
            {
                g.FillPath(brush, path);
            }

            // White text on badge
            using (var brush = new SolidBrush(Color.White))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(badgeText, _badgeFont, brush, badgeRect, format);
            }
        }

        /// <summary>
        /// Material 3 section header: uppercase with subtle line
        /// </summary>
        public override void PaintSectionHeader(Graphics g, Rectangle headerRect, string headerText, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(headerText)) return;

            EnsureFonts();
            Color textColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(150, context.Theme.SideMenuForeColor)
                : Color.FromArgb(99, 91, 103);

            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(headerText.ToUpperInvariant(), _headerFont, brush, headerRect, format);
            }
        }

        /// <summary>
        /// Material 3 divider: subtle horizontal line
        /// </summary>
        public override void PaintDivider(Graphics g, Rectangle dividerRect, ISideBarPainterContext context)
        {
            Color lineColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(40, context.Theme.BorderColor)
                : Color.FromArgb(40, 121, 116, 126);

            int y = dividerRect.Y + dividerRect.Height / 2;
            int padding = 16;

            using (var pen = new Pen(lineColor, 1f))
            {
                g.DrawLine(pen, dividerRect.X + padding, y, dividerRect.Right - padding, y);
            }
        }

        #endregion
    }
}
