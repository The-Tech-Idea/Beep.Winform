using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
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

            // Store bounds so PaintArrow() can access them
            _lastPaintBounds = bounds;

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
        /// Paint background using BeepStyling system with GraphicsPath support
        /// Fully integrated with all 20+ BeepControlStyle designs
        /// </summary>
        public override void PaintBackground(Graphics g, Rectangle bounds, 
            ToolTipConfig config, IBeepTheme theme)
        {
            var beepStyle = ToolTipStyleAdapter.GetBeepControlStyle(config);
            
            // Use ToolTipThemeHelpers for consistent theme color management
            var useThemeColors = config.UseBeepThemeColors && theme != null;
            var colors = ToolTipThemeHelpers.GetThemeColors(
                theme, 
                config.Type, 
                useThemeColors,
                config.BackColor,
                config.ForeColor,
                config.BorderColor);

            // Get corner radius from Style
            int radius = StyleBorders.GetRadius(beepStyle);

            using (var path = CreateRoundedRectangle(bounds, radius))
            {
                // Priority 1: Custom background color (from config)
                if (config.BackColor.HasValue)
                {
                    using (var brush = new SolidBrush(config.BackColor.Value))
                    {
                        g.FillPath(brush, path);
                    }
                    return;
                }

                // Priority 2: Use BeepStyling with theme colors from ToolTipThemeHelpers
                if (useThemeColors && theme != null)
                {
                    var savedTheme = BeepStyling.CurrentTheme;
                    var savedUseTheme = BeepStyling.UseThemeColors;
                    var savedStyle = BeepStyling.CurrentControlStyle;
                    
                    try
                    {
                        BeepStyling.CurrentTheme = theme;
                        BeepStyling.UseThemeColors = true;
                        BeepStyling.SetControlStyle(beepStyle);
                        
                        // Use BeepStyling.PaintStyleBackground for GraphicsPath-based rendering
                        BeepStyling.PaintStyleBackground(g, path, beepStyle, true);
                    }
                    finally
                    {
                        BeepStyling.CurrentTheme = savedTheme;
                        BeepStyling.UseThemeColors = savedUseTheme;
                        BeepStyling.SetControlStyle(savedStyle);
                    }
                }
                else
                {
                    // Priority 3: Use theme colors from ToolTipThemeHelpers (even without BeepStyling)
                    using (var brush = new SolidBrush(colors.backColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
        }

        #endregion

        #region Border Painting

        /// <summary>
        /// Paint border using BeepStyling BorderPainters system
        /// Supports all BeepControlStyle border designs
        /// </summary>
        public override void PaintBorder(Graphics g, Rectangle bounds, 
            ToolTipConfig config, IBeepTheme theme)
        {
            var beepStyle = ToolTipStyleAdapter.GetBeepControlStyle(config);
            var colors = ToolTipStyleAdapter.GetColors(config, theme);

            int radius = StyleBorders.GetRadius(beepStyle);

            using (var path = CreateRoundedRectangle(bounds, radius))
            {
                if (config.BorderColor.HasValue)
                {
                    // Custom border color specified
                    int borderWidth = (int)StyleBorders.GetBorderWidth(beepStyle);
                    using (var pen = new Pen(config.BorderColor.Value, borderWidth))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Use BeepStyling.PaintStyleBorder for consistent border rendering
                    var savedStyle = BeepStyling.CurrentControlStyle;
                    var savedTheme = BeepStyling.CurrentTheme;
                    var savedUseTheme = BeepStyling.UseThemeColors;
                    
                    try
                    {
                        BeepStyling.SetControlStyle(beepStyle);
                        
                        if (config.UseBeepThemeColors && theme != null)
                        {
                            BeepStyling.CurrentTheme = theme;
                            BeepStyling.UseThemeColors = true;
                        }
                        
                        // Paint border using BeepStyling
                        BeepStyling.PaintStyleBorder(g, path, false, beepStyle);
                    }
                    finally
                    {
                        BeepStyling.SetControlStyle(savedStyle);
                        BeepStyling.CurrentTheme = savedTheme;
                        BeepStyling.UseThemeColors = savedUseTheme;
                    }
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
        /// Paint arrow/pointer towards target element.
        /// Delegates to <see cref="ToolTipArrowPainter"/> for DPI-aware, style-aware rendering.
        /// </summary>
        public override void PaintArrow(Graphics g, Point position, ToolTipPlacement placement,
            ToolTipConfig config, IBeepTheme theme)
        {
            if (!config.ShowArrow || config.ArrowStyle == ToolTipArrowStyle.Hidden)
                return;

            var colors    = ToolTipStyleAdapter.GetColors(config, theme);
            Color fill    = config.BackColor ?? colors.background;
            Color border  = config.BorderColor ?? colors.border;

            // `position` is the pre-calculated arrow tip point from ToolTipHelpers.
            // We re-delegate to the bounds-based ToolTipArrowPainter which accounts for
            // ArrowOffset and ArrowStyle stored on the config, so we derive bounds from position.
            // Re-use the bounds rect supplied via the new Paint() overload that stores it in
            // _lastPaintBounds below.  Fall back to estimating from position.
            var bounds = _lastPaintBounds.IsEmpty
                ? new Rectangle(position.X - 80, position.Y - 30, 160, 30)
                : _lastPaintBounds;

            ToolTipArrowPainter.DrawArrow(
                g, bounds, placement,
                config.ArrowStyle,
                config.ArrowSize > 0 ? config.ArrowSize : DefaultArrowSize,
                config.ArrowOffset,
                fill, border);
        }

        // Stores the tooltip bounds so PaintArrow can access them.
        private Rectangle _lastPaintBounds;

        #endregion

        /// <summary>
        /// Paint content including icons, title, and text
        /// </summary>
        public override void PaintContent(Graphics g, Rectangle bounds, 
            ToolTipConfig config, IBeepTheme theme)
        {
            // If ContentItems is populated, use it as the source of truth
            if (config.ContentItems != null && config.ContentItems.Count > 0)
            {
                PaintContentItems(g, bounds, config, theme);
                return;
            }

            var contentRect = GetContentRectangle(bounds, config);
            var colors = ToolTipStyleAdapter.GetColors(config, theme);
            Color foreColor = config.ForeColor ?? colors.foreground;

            int currentX = contentRect.X;
            int currentY = contentRect.Y;
            int contentWidth = contentRect.Width;

            // Draw close button if closable
            if (config.Closable)
            {
                int btnSize = 16;
                var closeRect = new Rectangle(contentRect.Right - btnSize, contentRect.Y, btnSize, btnSize);
                DrawCloseButton(g, closeRect, foreColor);
                contentWidth -= (btnSize + 4);
            }

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
                var titleSize = TextUtils.MeasureText(g,config.Title, titleFont, contentWidth);
                currentY += (int)Math.Ceiling(titleSize.Height) + DefaultTitleSpacing;
            }

            // Paint text (with optional markup rendering)
            if (!string.IsNullOrEmpty(config.Text))
            {
                var textFont = GetTextFont(config);
                var textRect = new Rectangle(currentX, currentY, contentWidth, 
                    contentRect.Bottom - currentY);

                if (config.UseMarkup)
                {
                    // Parse and render markup spans
                    DrawMarkupSpans(g, textRect, config.Text, textFont, foreColor, theme);
                }
                else
                {
                    using (var brush = new SolidBrush(foreColor))
                    {
                        var format = new StringFormat
                        {
                            Alignment = config.TextHAlign,
                            LineAlignment = config.TextVAlign,
                            Trimming = StringTrimming.EllipsisWord
                        };

                        g.DrawString(config.Text, textFont, brush, textRect, format);
                    }
                }
            }

            // Paint keyboard shortcut badges in the footer
            if (config.Shortcuts != null && config.Shortcuts.Count > 0)
            {
                var badgeSize = ShortcutBadgePainter.MeasureShortcuts(g, config.Shortcuts);
                if (!badgeSize.IsEmpty)
                {
                    // Right-align in the content area
                    int bx = contentRect.Right - badgeSize.Width;
                    int by = contentRect.Bottom - badgeSize.Height;
                    ShortcutBadgePainter.DrawShortcuts(g, config.Shortcuts, new Point(bx, by), theme);
                }
            }
        }

        #endregion

        #region Rich Content Items Rendering

        /// <summary>
        /// Renders ToolTipContentItem[] sections (Header / Body / Divider / Footer)
        /// as the source of truth when populated on config.
        /// </summary>
        private void PaintContentItems(Graphics g, Rectangle bounds,
            ToolTipConfig config, IBeepTheme theme)
        {
            var contentRect = GetContentRectangle(bounds, config);
            var colors = ToolTipStyleAdapter.GetColors(config, theme);
            Color foreColor = config.ForeColor ?? colors.foreground;

            int y = contentRect.Y;
            int spacing = DefaultTitleSpacing;

            // Draw close button if closable
            int availableWidth = contentRect.Width;
            if (config.Closable)
            {
                int btnSize = 16;
                var closeRect = new Rectangle(contentRect.Right - btnSize, contentRect.Y, btnSize, btnSize);
                DrawCloseButton(g, closeRect, foreColor);
                availableWidth -= (btnSize + 4);
            }

            foreach (var item in config.ContentItems)
            {
                if (y >= contentRect.Bottom) break;
                int remainingHeight = contentRect.Bottom - y;

                switch (item.Section)
                {
                    case ToolTipSection.Header:
                        {
                            var headerFont = GetTitleFont(config);
                            int iconSize = 0;
                            int textX = contentRect.X;

                            // Icon on left
                            if (!string.IsNullOrEmpty(item.IconPath))
                            {
                                iconSize = DefaultIconSize;
                                var iconRect = new Rectangle(contentRect.X, y, iconSize, iconSize);
                                try
                                {
                                    var beepStyle = ToolTipStyleAdapter.GetBeepControlStyle(config);
                                    using (var path = CreateRoundedRectangle(iconRect, Math.Min(4, iconRect.Width / 4)))
                                    {
                                        StyledImagePainter.PaintWithTint(g, path, item.IconPath,
                                            foreColor, 0.8f, 4);
                                    }
                                }
                                catch { /* fallback: skip icon */ }
                                textX += iconSize + DefaultIconMargin;
                            }

                            // Title text
                            Color itemColor = item.ForeColor ?? foreColor;
                            FontStyle fs = FontStyle.Bold;
                            if (item.IsItalic) fs |= FontStyle.Italic;
                            var font = item.Font ?? headerFont;

                            var headerRect = new Rectangle(textX, y, availableWidth - iconSize, Math.Min(remainingHeight, 24));
                            using (var brush = new SolidBrush(itemColor))
                            {
                                var format = new StringFormat { LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                                g.DrawString(item.Text ?? "", font, brush, headerRect, format);
                            }
                            y += Math.Max(iconSize, headerRect.Height) + spacing;
                        }
                        break;

                    case ToolTipSection.Body:
                        {
                            var bodyFont = GetTextFont(config);
                            Color itemColor = item.ForeColor ?? foreColor;
                            FontStyle fs = FontStyle.Regular;
                            if (item.IsBold) fs |= FontStyle.Bold;
                            if (item.IsItalic) fs |= FontStyle.Italic;

                            Font font = item.Font ?? bodyFont;
                            if (fs != FontStyle.Regular && item.Font == null)
                            {
                                font = new Font(bodyFont.FontFamily, bodyFont.Size, fs);
                            }

                            var bodyRect = new Rectangle(contentRect.X, y, availableWidth, remainingHeight);

                            if (item.IsCode)
                            {
                                // Tinted code background
                                var codeRect = new Rectangle(contentRect.X, y, availableWidth, (int)(bodyFont.GetHeight(g) * 1.4f));
                                using (var codeBrush = new SolidBrush(Color.FromArgb(30, foreColor)))
                                    g.FillRectangle(codeBrush, codeRect);
                                var monoFont = new Font("Consolas", bodyFont.Size, fs);
                                using (var brush = new SolidBrush(itemColor))
                                    g.DrawString(item.Text ?? "", monoFont, brush, codeRect);
                                y += codeRect.Height + spacing;
                            }
                            else if (item.IsLink)
                            {
                                var linkFont = new Font(bodyFont.FontFamily, bodyFont.Size, FontStyle.Underline);
                                Color linkColor = theme?.AccentColor ?? Color.DodgerBlue;
                                using (var brush = new SolidBrush(linkColor))
                                    g.DrawString(item.Text ?? "", linkFont, brush, bodyRect);
                                y += (int)(linkFont.GetHeight(g) * 1.2f) + spacing;
                            }
                            else
                            {
                                var measured = TextUtils.MeasureText(g, item.Text ?? "", font, availableWidth);
                                using (var brush = new SolidBrush(itemColor))
                                {
                                    var format = new StringFormat
                                    {
                                        Alignment = config.TextHAlign,
                                        LineAlignment = config.TextVAlign,
                                        Trimming = StringTrimming.EllipsisWord
                                    };
                                    g.DrawString(item.Text ?? "", font, brush, bodyRect, format);
                                }
                                y += (int)Math.Ceiling(measured.Height) + spacing;
                            }
                        }
                        break;

                    case ToolTipSection.Divider:
                        {
                            var dividerRect = new Rectangle(contentRect.X, y + spacing / 2, availableWidth, 1);
                            DrawDivider(g, dividerRect, foreColor, theme);
                            y += spacing;
                        }
                        break;

                    case ToolTipSection.Footer:
                        {
                            var footerFont = GetTextFont(config);
                            Color itemColor = item.ForeColor ?? Color.FromArgb(160, foreColor);
                            var footerRect = new Rectangle(contentRect.X, y, availableWidth, remainingHeight);
                            using (var brush = new SolidBrush(itemColor))
                            {
                                var format = new StringFormat { Trimming = StringTrimming.EllipsisCharacter };
                                g.DrawString(item.Text ?? "", footerFont, brush, footerRect, format);
                            }
                            y += (int)(footerFont.GetHeight(g)) + spacing;
                        }
                        break;
                }
            }
        }

        #endregion

        #region Close Button & Divider Drawing

        /// <summary>
        /// Draws a × close glyph in the specified rectangle.
        /// Color is foreground at 60% alpha; hover state managed by CustomToolTip form.
        /// </summary>
        private void DrawCloseButton(Graphics g, Rectangle rect, Color foreColor)
        {
            Color btnColor = Color.FromArgb(153, foreColor); // 60% alpha
            using (var pen = new Pen(btnColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                int inset = 3;
                g.DrawLine(pen, rect.X + inset, rect.Y + inset, rect.Right - inset, rect.Bottom - inset);
                g.DrawLine(pen, rect.Right - inset, rect.Y + inset, rect.X + inset, rect.Bottom - inset);
            }
        }

        /// <summary>
        /// Draws a 1px horizontal divider line with 30% alpha border color.
        /// </summary>
        private void DrawDivider(Graphics g, Rectangle rect, Color foreColor, IBeepTheme theme)
        {
            Color dividerColor = theme != null
                ? Color.FromArgb(77, theme.BorderColor) // 30% alpha
                : Color.FromArgb(77, foreColor);
            using (var pen = new Pen(dividerColor, 1f))
            {
                g.DrawLine(pen, rect.X, rect.Y, rect.Right, rect.Y);
            }
        }

        #endregion

        #region Markup Span Rendering

        /// <summary>
        /// Renders parsed markup spans (bold, italic, code, link) inline within the given rectangle.
        /// </summary>
        private void DrawMarkupSpans(Graphics g, RectangleF rect, string markup,
            Font baseFont, Color foreColor, IBeepTheme theme)
        {
            var spans = ToolTipMarkupParser.Parse(markup);
            if (spans.Count == 0) return;

            float x = rect.X;
            float y = rect.Y;
            float lineHeight = baseFont.GetHeight(g);
            float maxWidth = rect.Width;

            foreach (var span in spans)
            {
                FontStyle fs = span.GetFontStyle();
                Font spanFont = span.Kind == SpanKind.Code
                    ? new Font("Consolas", baseFont.Size, fs)
                    : new Font(baseFont.FontFamily, baseFont.Size, fs);

                Color spanColor = span.Kind == SpanKind.Link
                    ? (theme?.AccentColor ?? Color.DodgerBlue)
                    : foreColor;

                var measured = g.MeasureString(span.Text, spanFont);

                // Word-wrap: if the span exceeds remaining width, move to next line
                if (x + measured.Width > rect.Right && x > rect.X)
                {
                    x = rect.X;
                    y += lineHeight;
                    if (y > rect.Bottom) break;
                }

                // Draw code background
                if (span.HasBackground)
                {
                    var codeBg = new RectangleF(x - 2, y, measured.Width + 4, lineHeight);
                    using (var bgBrush = new SolidBrush(Color.FromArgb(30, foreColor)))
                        g.FillRectangle(bgBrush, codeBg);
                }

                using (var brush = new SolidBrush(spanColor))
                    g.DrawString(span.Text, spanFont, brush, x, y);

                x += measured.Width;
            }
        }

        #endregion

        #region Icon Painting

        /// <summary>
        /// Paint icon using StyledImagePainter from BeepStyling system
        /// Fully integrated with BeepStyling cache and theme support
        /// </summary>
        private void PaintIcon(Graphics g, Rectangle iconRect, ToolTipConfig config, IBeepTheme theme)
        {
            try
            {
                var beepStyle = ToolTipStyleAdapter.GetBeepControlStyle(config);
                int cornerRadius = StyleBorders.GetRadius(beepStyle);

                // Determine icon source and paint using StyledImagePainter
                if (config.Icon != null)
                {
                    // Direct image - use ImagePainter for consistency
                    ImagePainter iconPainter = new ImagePainter();
                    iconPainter.Image = config.Icon;
                    iconPainter.CurrentTheme = theme;
                    iconPainter.ApplyThemeOnImage = config.ApplyThemeOnImage;
                    iconPainter.ScaleMode = ImageScaleMode.KeepAspectRatio;
                    iconPainter.Alignment = ContentAlignment.MiddleCenter;
                    iconPainter.DrawImage(g, iconRect);
                }
                else if (!string.IsNullOrEmpty(config.IconPath))
                {
                    // Use StyledImagePainter with path-based icon and rounded corners
                    using (var path = CreateRoundedRectangle(iconRect, Math.Min(cornerRadius, iconRect.Width / 4)))
                    {
                        if (config.ApplyThemeOnImage && theme != null)
                        {
                            // Apply theme tint to icon
                            var colors = ToolTipStyleAdapter.GetColors(config, theme);
                            StyledImagePainter.PaintWithTint(g, path, config.IconPath, 
                                colors.foreground, 0.8f, cornerRadius);
                        }
                        else
                        {
                            // Paint icon without tint
                            StyledImagePainter.Paint(g, path, config.IconPath, beepStyle);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(config.ImagePath))
                {
                    // Use StyledImagePainter for image with rounded corners
                    using (var path = CreateRoundedRectangle(iconRect, Math.Min(cornerRadius, iconRect.Width / 4)))
                    {
                        StyledImagePainter.Paint(g, path, config.ImagePath, beepStyle);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BeepStyledToolTipPainter] Error painting icon: {ex.Message}");
                
                // Fallback: draw placeholder icon
                var colors = ToolTipStyleAdapter.GetColors(config, theme);
                using (var brush = new SolidBrush(Color.FromArgb(50, colors.foreground)))
                {
                    g.FillEllipse(brush, iconRect);
                }
            }
        }

        #endregion
    }
}
