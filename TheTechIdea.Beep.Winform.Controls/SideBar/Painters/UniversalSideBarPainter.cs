using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.SideBar.Painters
{
    /// <summary>
    /// Universal SideBar Painter - Uses BeepStyling/StyleColors for all BeepControlStyle values.
    /// 
    /// This single painter handles the majority of styles by leveraging the centralized
    /// color definitions in StyleColors. Style-specific visual tweaks (corner radius,
    /// selection shape, border style) are determined by style category.
    /// 
    /// For styles requiring unique visual effects (Cyberpunk, Glassmorphism, Neumorphism),
    /// use the dedicated specialized painters instead.
    /// </summary>
    public sealed class UniversalSideBarPainter : BaseSideBarPainter
    {
        public override string Name => "Universal";

        // Fonts
        private Font _headerFont;
        private Font _itemFont;
        private Font _childFont;
        private Font _badgeFont;

        #region Color Helpers

        private Color GetBackColor(ISideBarPainterContext ctx)
        {
            if (ctx.UseThemeColors && ctx.Theme != null)
                return ctx.Theme.SideMenuBackColor;
            return StyleColors.GetBackground(ctx.ControlStyle);
        }

        private Color GetForeColor(ISideBarPainterContext ctx)
        {
            if (ctx.UseThemeColors && ctx.Theme != null)
                return ctx.Theme.SideMenuForeColor;
            return StyleColors.GetForeground(ctx.ControlStyle);
        }

        private Color GetPrimaryColor(ISideBarPainterContext ctx)
        {
            if (ctx.UseThemeColors && ctx.Theme != null)
                return ctx.Theme.PrimaryColor;
            return StyleColors.GetPrimary(ctx.ControlStyle);
        }

        private Color GetSecondaryColor(ISideBarPainterContext ctx)
        {
            if (ctx.UseThemeColors && ctx.Theme != null)
                return ctx.Theme.SecondaryColor;
            return StyleColors.GetSecondary(ctx.ControlStyle);
        }

        private Color GetBorderColor(ISideBarPainterContext ctx)
        {
            if (ctx.UseThemeColors && ctx.Theme != null)
                return ctx.Theme.BorderColor;
            return StyleColors.GetBorder(ctx.ControlStyle);
        }

        private Color GetAccentColor(ISideBarPainterContext ctx)
        {
            if (ctx.UseThemeColors && ctx.Theme != null)
                return ctx.Theme.AccentColor;
            return StyleColors.GetPrimary(ctx.ControlStyle);
        }

        #endregion

        #region Style-Specific Visual Parameters

        /// <summary>
        /// Get corner radius based on style category
        /// </summary>
        private int GetCornerRadius(BeepControlStyle style) => style switch
        {
            // Sharp corners (0)
            BeepControlStyle.Metro or
            BeepControlStyle.Metro2 or
            BeepControlStyle.NeoBrutalist or
            BeepControlStyle.Brutalist or
            BeepControlStyle.Terminal or
            BeepControlStyle.HighContrast => 0,

            // Large pill shape (20-28)
            BeepControlStyle.iOS15 or
            BeepControlStyle.MacOSBigSur or
            BeepControlStyle.PillRail or
            BeepControlStyle.Apple => 20,

            BeepControlStyle.Material3 or
            BeepControlStyle.MaterialYou => 28,

            // Medium rounded (12-16)
            BeepControlStyle.Fluent2 or
            BeepControlStyle.Windows11Mica or
            BeepControlStyle.ChakraUI or
            BeepControlStyle.Bootstrap => 12,

            // Small rounded (4-8)
            BeepControlStyle.AntDesign or
            BeepControlStyle.TailwindCard or
            BeepControlStyle.NotionMinimal or
            BeepControlStyle.VercelClean or
            BeepControlStyle.Minimal => 6,

            // Default
            _ => 8
        };

        /// <summary>
        /// Check if style uses dark theme
        /// </summary>
        private bool IsDarkStyle(BeepControlStyle style) => style switch
        {
            BeepControlStyle.DarkGlow or
            BeepControlStyle.DiscordStyle or
            BeepControlStyle.Cyberpunk or
            BeepControlStyle.Dracula or
            BeepControlStyle.OneDark or
            BeepControlStyle.Nord or
            BeepControlStyle.Tokyo or
            BeepControlStyle.GruvBox or
            BeepControlStyle.Metro2 or
            BeepControlStyle.Terminal or
            BeepControlStyle.Gaming or
            BeepControlStyle.ArcLinux or
            BeepControlStyle.Ubuntu or
            BeepControlStyle.Neon or
            BeepControlStyle.Effect or
            BeepControlStyle.Holographic => true,
            _ => false
        };

        /// <summary>
        /// Check if style uses left accent bar for selection
        /// </summary>
        private bool UsesAccentBar(BeepControlStyle style) => style switch
        {
            BeepControlStyle.DiscordStyle or
            BeepControlStyle.Metro or
            BeepControlStyle.Metro2 or
            BeepControlStyle.Fluent or
            BeepControlStyle.Fluent2 or
            BeepControlStyle.Windows11Mica => true,
            _ => false
        };

        /// <summary>
        /// Check if style uses bold borders
        /// </summary>
        private bool UsesBoldBorder(BeepControlStyle style) => style switch
        {
            BeepControlStyle.NeoBrutalist or
            BeepControlStyle.Brutalist or
            BeepControlStyle.Cartoon => true,
            _ => false
        };

        /// <summary>
        /// Get font family for style
        /// </summary>
        private string GetFontFamily(BeepControlStyle style) => style switch
        {
            BeepControlStyle.Material3 or BeepControlStyle.MaterialYou => "Roboto",
            BeepControlStyle.iOS15 or BeepControlStyle.MacOSBigSur or BeepControlStyle.Apple => "SF Pro Display",
            BeepControlStyle.Fluent2 or BeepControlStyle.Windows11Mica => "Segoe UI Variable",
            BeepControlStyle.Terminal => "Cascadia Code",
            BeepControlStyle.DiscordStyle => "Whitney",
            BeepControlStyle.Ubuntu => "Ubuntu",
            _ => "Segoe UI"
        };

        #endregion

        #region Font Management

        private void EnsureFonts(BeepControlStyle style)
        {
            string fontFamily = GetFontFamily(style);
            _headerFont ??= BeepFontManager.GetCachedFont(fontFamily, 9f, FontStyle.Bold);
            _itemFont ??= BeepFontManager.GetCachedFont(fontFamily, 11f, FontStyle.Regular);
            _childFont ??= BeepFontManager.GetCachedFont(fontFamily, 10f, FontStyle.Regular);
            _badgeFont ??= BeepFontManager.GetCachedFont(fontFamily, 8f, FontStyle.Bold);
        }

        #endregion

        #region Main Paint Method

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            var style = context.ControlStyle;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            EnsureFonts(style);

            // Paint background
            PaintBackground(g, bounds, context);

            // Paint border/edge
            PaintEdge(g, bounds, context);

            int padding = 12;
            int currentY = bounds.Top + padding;

            // Paint toggle button if visible
            if (context.ShowToggleButton)
            {
                Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                PaintToggleButton(g, toggleRect, context);
                currentY += context.ItemHeight + 8;
            }

            // Paint menu items
            PaintMenuItems(g, bounds, context, ref currentY);
        }

        private void PaintBackground(Graphics g, Rectangle bounds, ISideBarPainterContext context)
        {
            Color backColor = GetBackColor(context);

            using (var brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Add subtle gradient overlay for some styles
            var style = context.ControlStyle;
            if (style == BeepControlStyle.GradientModern || style == BeepControlStyle.Holographic)
            {
                Color gradientEnd = Color.FromArgb(30, GetPrimaryColor(context));
                using (var gradientBrush = new LinearGradientBrush(bounds, Color.Transparent, gradientEnd, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(gradientBrush, bounds);
                }
            }
        }

        private void PaintEdge(Graphics g, Rectangle bounds, ISideBarPainterContext context)
        {
            Color borderColor = GetBorderColor(context);
            float borderWidth = UsesBoldBorder(context.ControlStyle) ? 3f : 1f;

            using (var pen = new Pen(borderColor, borderWidth))
            {
                g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }
        }

        #endregion

        #region Toggle Button

        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            Color iconColor = GetForeColor(context);
            int iconSize = Math.Min(toggleRect.Height - 12, 20);
            var iconRect = new Rectangle(
                toggleRect.X + (toggleRect.Width - iconSize) / 2,
                toggleRect.Y + (toggleRect.Height - iconSize) / 2,
                iconSize,
                iconSize);

            DrawHamburgerIcon(g, iconRect, iconColor);
        }

        #endregion

        #region Selection & Hover

        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            var style = context.ControlStyle;
            Color selectionColor = GetPrimaryColor(context);
            int radius = GetCornerRadius(style);

            if (UsesAccentBar(style))
            {
                // Left accent bar style (Discord, Metro, Fluent)
                Rectangle barRect = new Rectangle(itemRect.Left, itemRect.Top + 4, 3, itemRect.Height - 8);
                using (var brush = new SolidBrush(selectionColor))
                {
                    g.FillRectangle(brush, barRect);
                }

                // Subtle background
                Color bgColor = Color.FromArgb(20, selectionColor);
                using (var bgBrush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(bgBrush, itemRect);
                }
            }
            else
            {
                // Filled selection with corner radius
                Color fillColor = IsDarkStyle(style)
                    ? Color.FromArgb(40, selectionColor)
                    : Color.FromArgb(25, selectionColor);

                using (var path = CreateRoundedPath(itemRect, radius))
                using (var brush = new SolidBrush(fillColor))
                {
                    g.FillPath(brush, path);
                }

                // Border for some styles
                if (style == BeepControlStyle.NeoBrutalist || style == BeepControlStyle.Brutalist)
                {
                    using (var path = CreateRoundedPath(itemRect, radius))
                    using (var pen = new Pen(Color.Black, 2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            var style = context.ControlStyle;
            Color primaryColor = GetPrimaryColor(context);
            int radius = GetCornerRadius(style);

            int alpha = IsDarkStyle(style) ? 15 : 8;
            Color hoverColor = Color.FromArgb(alpha, primaryColor);

            using (var path = CreateRoundedPath(itemRect, radius))
            using (var brush = new SolidBrush(hoverColor))
            {
                g.FillPath(brush, path);
            }
        }

        public override void PaintPressed(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            var style = context.ControlStyle;
            Color primaryColor = GetPrimaryColor(context);
            int radius = GetCornerRadius(style);

            int alpha = IsDarkStyle(style) ? 25 : 15;
            Color pressedColor = Color.FromArgb(alpha, primaryColor);

            using (var path = CreateRoundedPath(itemRect, radius))
            using (var brush = new SolidBrush(pressedColor))
            {
                g.FillPath(brush, path);
            }
        }

        public override void PaintDisabled(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color disabledColor = Color.FromArgb(50, 128, 128, 128);
            int radius = GetCornerRadius(context.ControlStyle);

            using (var path = CreateRoundedPath(itemRect, radius))
            using (var brush = new SolidBrush(disabledColor))
            {
                g.FillPath(brush, path);
            }
        }

        #endregion

        #region Menu Items

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;

            int padding = 12;
            int iconSize = GetTopLevelIconSize(context);
            int itemIndex = 0;

            foreach (var item in context.Items)
            {
                // Section header
                if (context.SectionHeaders != null)
                {
                    var sectionHeader = context.SectionHeaders.Find(h => h.BeforeIndex == itemIndex);
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

                // Paint item content
                PaintItemContent(g, itemRect, item, context, iconSize, false);

                // Paint expand icon if has children
                if (item.Children != null && item.Children.Count > 0)
                {
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    Rectangle expandRect = new Rectangle(itemRect.Right - 24, itemRect.Top + (itemRect.Height - 16) / 2, 16, 16);
                    PaintExpandIcon(g, expandRect, isExpanded, item, context);
                }

                // Paint badge if exists
                if (context.ItemBadges != null && context.ItemBadges.ContainsKey(item))
                {
                    string badgeText = context.ItemBadges[item];
                    Color badgeColor = context.ItemBadgeColors.ContainsKey(item) ? context.ItemBadgeColors[item] : GetPrimaryColor(context);
                    PaintBadge(g, itemRect, badgeText, badgeColor, context);
                }

                currentY += context.ItemHeight + 2;

                // Paint children if expanded
                if (item.Children != null && item.Children.Count > 0)
                {
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    if (isExpanded)
                    {
                        PaintChildItems(g, bounds, context, item, ref currentY, 1);
                    }
                }

                // Divider
                if (context.DividerPositions != null && context.DividerPositions.Contains(itemIndex))
                {
                    Rectangle dividerRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, 1);
                    PaintDivider(g, dividerRect, context);
                    currentY += 8;
                }

                itemIndex++;
            }
        }

        private void PaintChildItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, SimpleItem parent, ref int currentY, int indentLevel)
        {
            if (parent.Children == null) return;

            int padding = 12;
            int indent = context.IndentationWidth * indentLevel;
            int childIconSize = GetChildIconSize(context);

            // Calculate parent rect for connector drawing
            Rectangle parentRect = new Rectangle(bounds.Left + padding, currentY - context.ItemHeight - 2, bounds.Width - padding * 2, context.ItemHeight);

            foreach (var child in parent.Children)
            {
                Rectangle childRect = new Rectangle(
                    bounds.Left + padding + indent,
                    currentY,
                    bounds.Width - padding * 2 - indent,
                    context.ChildItemHeight);

                // Paint connector line
                PaintAccordionConnector(g, parent, parentRect, child, childRect, indentLevel, context);

                // Paint states
                if (!child.IsEnabled)
                {
                    PaintDisabled(g, childRect, context);
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
                PaintItemContent(g, childRect, child, context, childIconSize, true);

                currentY += context.ChildItemHeight + 2;

                // Recursive children
                if (child.Children != null && child.Children.Count > 0)
                {
                    bool isExpanded = context.ExpandedState.ContainsKey(child) && context.ExpandedState[child];
                    if (isExpanded)
                    {
                        PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1);
                    }
                }
            }
        }

        private void PaintItemContent(Graphics g, Rectangle itemRect, SimpleItem item, ISideBarPainterContext context, int iconSize, bool isChild)
        {
            Color foreColor = GetForeColor(context);
            if (!item.IsEnabled)
                foreColor = Color.FromArgb(100, foreColor);
            else if (item == context.SelectedItem)
                foreColor = IsDarkStyle(context.ControlStyle) ? Color.White : GetPrimaryColor(context);

            int iconPadding = 8;
            int textX = itemRect.Left + iconPadding;

            // Paint icon
            if (!string.IsNullOrEmpty(item.ImagePath) || !string.IsNullOrEmpty(context.DefaultImagePath))
            {
                Rectangle iconRect = new Rectangle(itemRect.Left + iconPadding, itemRect.Top + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                string imagePath = !string.IsNullOrEmpty(item.ImagePath) ? item.ImagePath : context.DefaultImagePath;

                if (context.UseThemeColors && context.Theme != null)
                {
                    StyledImagePainter.PaintWithTint(g, iconRect, imagePath, foreColor);
                }
                else
                {
                    StyledImagePainter.Paint(g, iconRect, imagePath);
                }
                textX = iconRect.Right + 10;
            }

            // Paint text
            if (!context.IsCollapsed || isChild)
            {
                Font font = isChild ? _childFont : _itemFont;
                Rectangle textRect = new Rectangle(textX, itemRect.Top, itemRect.Right - textX - 30, itemRect.Height);

                using (var brush = new SolidBrush(foreColor))
                using (var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter })
                {
                    g.DrawString(item.Text ?? "", font, brush, textRect, sf);
                }
            }
        }

        #endregion

        #region Expand Icon, Badge, Section Header, Divider

        public override void PaintExpandIcon(Graphics g, Rectangle iconRect, bool isExpanded, SimpleItem item, ISideBarPainterContext context)
        {
            Color iconColor = Color.FromArgb(150, GetForeColor(context));

            using (var pen = new Pen(iconColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int cx = iconRect.X + iconRect.Width / 2;
                int cy = iconRect.Y + iconRect.Height / 2;
                int size = 4;

                if (isExpanded)
                {
                    // Down chevron
                    g.DrawLine(pen, cx - size, cy - 2, cx, cy + 2);
                    g.DrawLine(pen, cx, cy + 2, cx + size, cy - 2);
                }
                else
                {
                    // Right chevron
                    g.DrawLine(pen, cx - 2, cy - size, cx + 2, cy);
                    g.DrawLine(pen, cx + 2, cy, cx - 2, cy + size);
                }
            }
        }

        public override void PaintBadge(Graphics g, Rectangle itemRect, string badgeText, Color badgeColor, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(badgeText)) return;

            int radius = GetCornerRadius(context.ControlStyle);
            radius = Math.Min(radius, 9);

            // Calculate badge rect
            var textSize = g.MeasureString(badgeText, _badgeFont);
            int badgeWidth = Math.Max(18, (int)textSize.Width + 8);
            int badgeHeight = 16;
            int badgeX = itemRect.Right - badgeWidth - 8;
            int badgeY = itemRect.Y + (itemRect.Height - badgeHeight) / 2;
            Rectangle badgeRect = new Rectangle(badgeX, badgeY, badgeWidth, badgeHeight);

            using (var path = CreateRoundedPath(badgeRect, radius))
            using (var brush = new SolidBrush(badgeColor))
            {
                g.FillPath(brush, path);
            }

            using (var brush = new SolidBrush(Color.White))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(badgeText, _badgeFont, brush, badgeRect, sf);
            }
        }

        public override void PaintSectionHeader(Graphics g, Rectangle headerRect, string headerText, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(headerText)) return;

            Color headerColor = Color.FromArgb(150, GetForeColor(context));

            using (var brush = new SolidBrush(headerColor))
            using (var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(headerText.ToUpperInvariant(), _headerFont, brush, headerRect, sf);
            }
        }

        public override void PaintDivider(Graphics g, Rectangle dividerRect, ISideBarPainterContext context)
        {
            Color dividerColor = Color.FromArgb(50, GetBorderColor(context));

            using (var pen = new Pen(dividerColor, 1f))
            {
                int y = dividerRect.Top + dividerRect.Height / 2;
                g.DrawLine(pen, dividerRect.Left, y, dividerRect.Right, y);
            }
        }

        public override void PaintAccordionConnector(Graphics g, SimpleItem parent, Rectangle parentRect, SimpleItem child, Rectangle childRect, int indentLevel, ISideBarPainterContext context)
        {
            Color connectorColor = Color.FromArgb(60, GetBorderColor(context));

            int indent = context.IndentationWidth * indentLevel;
            int lineX = childRect.X - indent / 2;

            using (var pen = new Pen(connectorColor, 1f))
            {
                // Vertical line from parent
                g.DrawLine(pen, lineX, parentRect.Bottom, lineX, childRect.Y + childRect.Height / 2);
                // Horizontal line to child
                g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2);
            }
        }

        #endregion
    }
}

