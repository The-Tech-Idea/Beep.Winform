using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Painters
{
    /// <summary>
    /// Modern tooltip painter that integrates with BeepStyling system
    /// Supports all 20+ BeepControlStyle designs with unified rendering
    /// Uses BackgroundPainter, BorderPainter, ShadowPainter, and ImagePainter from BeepStyling
    /// </summary>
    public class BeepStyledToolTipPainter : ToolTipPainterBase
    {
        #region Main Paint Method

        /// <summary>
        /// Paint complete tooltip with all components
        /// </summary>
        public override void Paint(Graphics g, Rectangle bounds, ToolTipConfig config, 
            ToolTipPlacement placement, IBeepTheme theme)
        {
            if (config == null || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            // Set high quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Paint in order: Shadow -> Background -> Border -> Arrow -> Content
            if (config.ShowShadow || config.EnableShadow)
            {
                PaintShadow(g, bounds, config);
            }

            PaintBackground(g, bounds, config, theme);
            PaintBorder(g, bounds, config, theme);

            if (config.ShowArrow)
            {
                var arrowPos = ToolTipHelpers.CalculateArrowPosition(bounds, placement, DefaultArrowSize);
                PaintArrow(g, arrowPos, placement, config, theme);
            }

            PaintContent(g, bounds, config, theme);
        }

        #endregion

        #region Background Painting

        /// <summary>
        /// Paint background using BeepStyling BackgroundPainters
        /// </summary>
        public override void PaintBackground(Graphics g, Rectangle bounds, 
            ToolTipConfig config, IBeepTheme theme)
        {
            var beepStyle = ToolTipStyleAdapter.GetBeepControlStyle(config);
            var colors = ToolTipStyleAdapter.GetColors(config, theme);

            // Get corner radius from style
            int radius = StyleBorders.GetRadius(beepStyle);

            using (var path = CreateRoundedRectangle(bounds, radius))
            {
                // Apply custom background color if specified
                if (config.BackColor.HasValue)
                {
                    using (var brush = new SolidBrush(config.BackColor.Value))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else if (config.UseBeepThemeColors && theme != null)
                {
                    // Use BeepStyling system with theme colors
                    var savedTheme = BeepStyling.CurrentTheme;
                    var savedUseTheme = BeepStyling.UseThemeColors;
                    
                    try
                    {
                        BeepStyling.CurrentTheme = theme;
                        BeepStyling.UseThemeColors = true;
                        BeepStyling.PaintStyleBackground(g, bounds, beepStyle);
                    }
                    finally
                    {
                        BeepStyling.CurrentTheme = savedTheme;
                        BeepStyling.UseThemeColors = savedUseTheme;
                    }
                }
                else
                {
                    // Use BeepStyling without theme override
                    var savedStyle = BeepStyling.CurrentControlStyle;
                    
                    try
                    {
                        BeepStyling.SetControlStyle(beepStyle);
                        BeepStyling.PaintStyleBackground(g, bounds, beepStyle);
                    }
                    finally
                    {
                        BeepStyling.SetControlStyle(savedStyle);
                    }
                }
            }
        }

        #endregion

        #region Border Painting

        /// <summary>
        /// Paint border using BeepStyling BorderPainters
        /// </summary>
        public override void PaintBorder(Graphics g, Rectangle bounds, 
            ToolTipConfig config, IBeepTheme theme)
        {
            var beepStyle = ToolTipStyleAdapter.GetBeepControlStyle(config);
            var colors = ToolTipStyleAdapter.GetColors(config, theme);

            int radius = StyleBorders.GetRadius(beepStyle);
            int borderWidth = (int)StyleBorders.GetBorderWidth(beepStyle);

            using (var path = CreateRoundedRectangle(bounds, radius))
            {
                Color borderColor = config.BorderColor ?? colors.border;

                using (var pen = new Pen(borderColor, borderWidth))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        #endregion

        #region Shadow Painting

        /// <summary>
        /// Paint shadow using BeepStyling ShadowPainters
        /// </summary>
        public override void PaintShadow(Graphics g, Rectangle bounds, ToolTipConfig config)
        {
            if (!config.ShowShadow && !config.EnableShadow)
                return;

            var beepStyle = ToolTipStyleAdapter.GetBeepControlStyle(config);

            if (!StyleShadows.HasShadow(beepStyle))
                return;

            // Use BeepStyling shadow painters
            var savedStyle = BeepStyling.CurrentControlStyle;
            
            try
            {
                BeepStyling.SetControlStyle(beepStyle);
                
                // BeepStyling.PaintStyleBackground already handles shadows internally
                // But for tooltips, we may want explicit control
                int blur = StyleShadows.GetShadowBlur(beepStyle);
                int offsetY = StyleShadows.GetShadowOffsetY(beepStyle);
                int offsetX = StyleShadows.GetShadowOffsetX(beepStyle);
                Color shadowColor = StyleShadows.GetShadowColor(beepStyle);

                // Create shadow bounds
                Rectangle shadowBounds = new Rectangle(
                    bounds.X + offsetX,
                    bounds.Y + offsetY,
                    bounds.Width,
                    bounds.Height
                );

                int radius = StyleBorders.GetRadius(beepStyle);

                // Simple shadow with blur simulation
                for (int i = blur; i > 0; i--)
                {
                    int alpha = (int)(shadowColor.A * (i / (float)blur) * 0.3f);
                    Color blurColor = Color.FromArgb(alpha, shadowColor);

                    using (var shadowBrush = new SolidBrush(blurColor))
                    using (var path = CreateRoundedRectangle(shadowBounds, radius))
                    {
                        g.FillPath(shadowBrush, path);
                    }

                    shadowBounds.Inflate(1, 1);
                }
            }
            finally
            {
                BeepStyling.SetControlStyle(savedStyle);
            }
        }

        #endregion

        #region Arrow Painting

        /// <summary>
        /// Paint arrow/pointer towards target element
        /// </summary>
        public override void PaintArrow(Graphics g, Point position, ToolTipPlacement placement, 
            ToolTipConfig config, IBeepTheme theme)
        {
            if (!config.ShowArrow)
                return;

            var colors = ToolTipStyleAdapter.GetColors(config, theme);
            Color arrowColor = config.BackColor ?? colors.background;

            using (var path = CreateArrowPath(position, placement, DefaultArrowSize))
            {
                // Fill arrow with background color
                using (var brush = new SolidBrush(arrowColor))
                {
                    g.FillPath(brush, path);
                }

                // Draw arrow border to match tooltip border
                if (config.BorderColor.HasValue || colors.border != Color.Transparent)
                {
                    Color borderColor = config.BorderColor ?? colors.border;
                    var beepStyle = ToolTipStyleAdapter.GetBeepControlStyle(config);
                    int borderWidth = (int)StyleBorders.GetBorderWidth(beepStyle);

                    using (var pen = new Pen(borderColor, borderWidth))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        #endregion

        #region Content Painting

        /// <summary>
        /// Paint content including icons, title, and text
        /// </summary>
        public override void PaintContent(Graphics g, Rectangle bounds, 
            ToolTipConfig config, IBeepTheme theme)
        {
            var contentRect = GetContentRectangle(bounds);
            var colors = ToolTipStyleAdapter.GetColors(config, theme);
            Color foreColor = config.ForeColor ?? colors.foreground;

            int currentX = contentRect.X;
            int currentY = contentRect.Y;
            int contentWidth = contentRect.Width;

            // Paint icon if present
            if (HasIcon(config))
            {
                var iconRect = new Rectangle(currentX, currentY, DefaultIconSize, DefaultIconSize);
                PaintIcon(g, iconRect, config, theme);
                currentX += DefaultIconSize + DefaultIconMargin;
                contentWidth -= (DefaultIconSize + DefaultIconMargin);
            }

            // Paint title
            if (!string.IsNullOrEmpty(config.Title))
            {
                var titleFont = GetTitleFont(config);
                var titleRect = new Rectangle(currentX, currentY, contentWidth, contentRect.Height);
                
                using (var brush = new SolidBrush(foreColor))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Near,
                        Trimming = StringTrimming.EllipsisCharacter
                    };

                    g.DrawString(config.Title, titleFont, brush, titleRect, format);
                }

                // Measure title height for text positioning
                var titleSize = g.MeasureString(config.Title, titleFont, contentWidth);
                currentY += (int)Math.Ceiling(titleSize.Height) + DefaultTitleSpacing;
            }

            // Paint text
            if (!string.IsNullOrEmpty(config.Text))
            {
                var textFont = GetTextFont(config);
                var textRect = new Rectangle(currentX, currentY, contentWidth, 
                    contentRect.Bottom - currentY);

                using (var brush = new SolidBrush(foreColor))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Near,
                        Trimming = StringTrimming.EllipsisWord
                    };

                    g.DrawString(config.Text, textFont, brush, textRect, format);
                }
            }
        }

        #endregion

        #region Icon Painting

        /// <summary>
        /// Paint icon using ImagePainter from BeepStyling cache
        /// </summary>
        private void PaintIcon(Graphics g, Rectangle iconRect, ToolTipConfig config, IBeepTheme theme)
        {
            ImagePainter iconPainter = null;
            string cacheKey = null;

            try
            {
                // Determine icon source
                if (config.Icon != null)
                {
                    // Use provided image directly
                    iconPainter = new ImagePainter();
                    iconPainter.Image = config.Icon;
                }
                else if (!string.IsNullOrEmpty(config.IconPath))
                {
                    // Use cached ImagePainter for path-based icons
                    cacheKey = $"tooltip_icon_{config.IconPath}";
                    
                    if (!BeepStyling.ImageCachedPainters.TryGetValue(cacheKey, out iconPainter))
                    {
                        iconPainter = new ImagePainter(config.IconPath);
                        BeepStyling.ImageCachedPainters[cacheKey] = iconPainter;
                    }
                }
                else if (!string.IsNullOrEmpty(config.ImagePath))
                {
                    // Use cached ImagePainter for image path
                    cacheKey = $"tooltip_image_{config.ImagePath}";
                    
                    if (!BeepStyling.ImageCachedPainters.TryGetValue(cacheKey, out iconPainter))
                    {
                        iconPainter = new ImagePainter(config.ImagePath);
                        BeepStyling.ImageCachedPainters[cacheKey] = iconPainter;
                    }
                }

                if (iconPainter != null && iconPainter.HasImage)
                {
                    // Configure painter
                    iconPainter.CurrentTheme = theme;
                    iconPainter.ApplyThemeOnImage = config.ApplyThemeOnImage;
                    iconPainter.ScaleMode = ImageScaleMode.KeepAspectRatio;
                    iconPainter.Alignment = ContentAlignment.MiddleCenter;

                    // Draw icon
                    iconPainter.DrawImage(g, iconRect);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BeepStyledToolTipPainter] Error painting icon: {ex.Message}");
            }
        }

        #endregion
    }
}
