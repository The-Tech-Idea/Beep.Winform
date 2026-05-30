using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepCard
    {
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            try
            {
                DrawContentCore(g);
            }
            catch (Exception ex)
            {
                // Prevent design-time crashes by drawing error indicator
                System.Diagnostics.Debug.WriteLine($"BeepCard.DrawContent error: {ex.Message}");
                if (DesignMode)
                {
                    DrawDesignTimeError(g, ex.Message);
                }
            }
        }

        private void DrawDesignTimeError(Graphics g, string message)
        {
            var rect = ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Draw red border
            using (var pen = new Pen(Color.Red, 2))
            {
                g.DrawRectangle(pen, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
            }

            // Draw error text
            using (var brush = new SolidBrush(Color.Red))
            using (var font = new Font("Arial", 8))
            {
                g.DrawString("Card Error", font, brush, rect.X + 4, rect.Y + 4);
                if (!string.IsNullOrEmpty(message) && rect.Height > 30)
                {
                    g.DrawString(message.Substring(0, Math.Min(50, message.Length)), font, brush, rect.X + 4, rect.Y + 18);
                }
            }
        }

        private void DrawContentCore(Graphics g)
        {
            // IMPORTANT: Do NOT call base.DrawContent(g) — this control handles all painting itself
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            UpdateDrawingRect();
            
            // Check if layout needs recalculation
            bool needsRecalculation = !_layoutCacheValid || 
                                     _cachedDrawingRect != DrawingRect ||
                                     _cachedDrawingRect.Size != DrawingRect.Size;
            
            if (needsRecalculation)
            {
                // Build layout context (fonts guaranteed non-null by field initializers + ApplyTheme)
                _layoutContext = BuildLayoutContext();
                
                // Let painter adjust layout with current fonts
                _painter?.Initialize(this, _currentTheme, 
                    _titleFont ?? SystemFonts.DefaultFont,
                    _bodyFont ?? SystemFonts.DefaultFont,
                    _captionFont ?? SystemFonts.DefaultFont);
                _layoutContext = _painter?.AdjustLayout(DrawingRect, _layoutContext) ?? _layoutContext;
                
                // Cache the layout
                _layoutCacheValid = true;
                _cachedDrawingRect = DrawingRect;
            }

            UpdateAuxiliaryHitAreas(_layoutContext);

            // Draw background
            if (UseThemeColors && _currentTheme != null)
            {
                _painter?.DrawBackground(g, _layoutContext);
            }
            else
            {
                BeepStyling.PaintStyleBackground(g, DrawingRect, ControlStyle);
            }

            DrawAccentBar(g);

            if (_isLoading)
            {
                DrawLoadingSkeleton(g);
            }
            else
            {
                // Paint image using StyledImagePainter
                if (_layoutContext.ShowImage && _layoutContext.ImageRect != Rectangle.Empty)
                {
                    string pathToPaint = GetImagePath();
                    if (!string.IsNullOrEmpty(pathToPaint))
                    {
                        try
                        {
                            if (Enabled)
                            {
                                StyledImagePainter.Paint(g, _layoutContext.ImageRect, pathToPaint);
                            }
                            else
                            {
                                StyledImagePainter.PaintDisabled(g, _layoutContext.ImageRect, pathToPaint, BackColor);
                            }
                        }
                        catch
                        {
                            // Swallow painting errors to prevent designer crashes
                        }
                    }
                }

                // Paint header text using GDI+ with anti-aliasing
                if (!string.IsNullOrEmpty(headerText) && _layoutContext.HeaderRect != Rectangle.Empty)
                {
                    var headerColor = _currentTheme?.CardHeaderStyle.TextColor ?? ForeColor;
                    var headerFont = _headerFont ?? _titleFont ?? _bodyFont ?? SystemFonts.DefaultFont;

                    DrawAntiAliasedText(g, headerText, headerFont, _layoutContext.HeaderRect, headerColor,
                        StringAlignment.Near, StringAlignment.Center);
                }

                // Paint paragraph text using GDI+ with anti-aliasing
                if (!string.IsNullOrEmpty(paragraphText) && _layoutContext.ParagraphRect != Rectangle.Empty)
                {
                    var paragraphColor = _currentTheme?.CardTextForeColor ?? ForeColor;
                    var paragraphFont = _paragraphFont ?? _bodyFont ?? _captionFont ?? SystemFonts.DefaultFont;

                    DrawAntiAliasedText(g, paragraphText, paragraphFont, _layoutContext.ParagraphRect, paragraphColor,
                        StringAlignment.Near, StringAlignment.Near);
                }

                // Paint primary button with enhanced styling
                if (_layoutContext.ShowButton && _layoutContext.ButtonRect != Rectangle.Empty)
                {
                    bool isHovered = IsButtonHovered(_layoutContext.ButtonRect);
                    PaintEnhancedButton(g, _layoutContext.ButtonRect, buttonText, _accentColor, isHovered, true);
                }

                // Paint secondary button with enhanced styling
                if (_layoutContext.ShowSecondaryButton && _layoutContext.SecondaryButtonRect != Rectangle.Empty)
                {
                    bool isHovered = IsButtonHovered(_layoutContext.SecondaryButtonRect);
                    var secondaryColor = _currentTheme?.CardBackColor ?? Color.Gray;
                    PaintEnhancedButton(g, _layoutContext.SecondaryButtonRect, secondaryButtonText, secondaryColor, isHovered, false);
                }

                // Draw foreground accents (badges, ratings, etc.)
                _painter?.DrawForegroundAccents(g, _layoutContext);
            }

            DrawAuxiliaryIcons(g);
            DrawRippleOverlay(g);
            DrawFocusRing(g);

            // Register hit areas for interaction
            RefreshHitAreas(_layoutContext);
        }

        /// <summary>
        /// Gets the hover lift offset based on hover progress
        /// </summary>
        private int GetHoverLiftOffset()
        {
            if (_interactionManager == null || _interactionManager.HoverProgress <= 0.001f)
                return 0;
            
            int maxLift = Scale(3);
            return (int)(maxLift * _interactionManager.HoverProgress);
        }

        /// <summary>
        /// Draws card shadow/elevation
        /// </summary>
        private void DrawCardShadow(Graphics g)
        {
            int shadowDepth = Scale(ShadowOffset);
            if (shadowDepth <= 0) return;

            int hoverLift = GetHoverLiftOffset();
            int shadowIntensity = (int)(20 + (_interactionManager?.HoverProgress ?? 0f) * 15);
            
            for (int i = 3; i > 0; i--)
            {
                int spread = i * 2 + hoverLift;
                int alpha = shadowIntensity * i / 3;
                var shadowRect = new Rectangle(
                    DrawingRect.X + shadowDepth - spread,
                    DrawingRect.Y + shadowDepth - spread + hoverLift,
                    DrawingRect.Width + spread * 2,
                    DrawingRect.Height + spread * 2
                );
                
                using (var shadowPath = CardRenderingHelpers.CreateRoundedPath(shadowRect, BorderRadius + i))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(alpha, Color.Black)))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }
            }
        }
        /// <summary>
        /// Draws anti-aliased text using GDI+ with proper formatting
        /// </summary>
        private void DrawAntiAliasedText(Graphics g, string text, Font font, Rectangle rect, Color color, 
            StringAlignment hAlign, StringAlignment vAlign)
        {
            if (string.IsNullOrEmpty(text) || rect.IsEmpty || font == null) return;

            try
            {
                using (var brush = new SolidBrush(color))
                using (var format = new StringFormat())
                {
                    format.Alignment = hAlign;
                    format.LineAlignment = vAlign;
                    format.Trimming = StringTrimming.EllipsisCharacter;
                    format.FormatFlags = StringFormatFlags.LineLimit;
                    
                    g.DrawString(text, font, brush, rect, format);
                }
            }
            catch (ArgumentException)
            {
                // GDI+ "Parameter is not valid" - font may be disposed or graphics invalid
                // Degrade gracefully rather than crashing the paint cycle
            }
        }

        /// <summary>
        /// Paints an enhanced button with gradient, shadow, and hover effects
        /// </summary>
        private void PaintEnhancedButton(Graphics g, Rectangle rect, string text, Color backColor, bool isHovered, bool isPrimary)
        {
            if (rect == Rectangle.Empty || string.IsNullOrEmpty(text))
                return;

            // Adjust color for hover with more dramatic effect
            Color btnColor = isHovered ? ShiftLuminance(backColor, 0.15f) : backColor;
            Color btnColorLight = isHovered ? ShiftLuminance(backColor, 0.25f) : ShiftLuminance(backColor, 0.1f);

            // Draw button shadow
            if (isHovered || isPrimary)
            {
                int shadowOffset = isHovered ? Scale(3) : Scale(2);
                int shadowSpread = isHovered ? Scale(4) : Scale(2);
                using (var shadowPath = CardRenderingHelpers.CreateRoundedPath(
                    new Rectangle(rect.X + shadowOffset - shadowSpread, rect.Y + shadowOffset - shadowSpread, 
                                 rect.Width + shadowSpread * 2, rect.Height + shadowSpread * 2), 
                    BorderRadius))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(isHovered ? 40 : 25, Color.Black)))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }
            }

            // Draw button background with gradient
            if (IsRounded && BorderRadius > 0)
            {
                using (var path = CreateRoundedRectanglePath(rect, BorderRadius))
                {
                    // Gradient fill
                    using (var gradientBrush = new LinearGradientBrush(
                        rect, btnColorLight, btnColor, LinearGradientMode.Vertical))
                    {
                        g.FillPath(gradientBrush, path);
                    }
                    
                    // Highlight at top for 3D effect
                    using (var highlightPen = new Pen(Color.FromArgb(60, Color.White), 1f))
                    {
                        var highlightPath = (GraphicsPath)path.Clone();
                        var highlightBounds = highlightPath.GetBounds();
                        highlightPath.Transform(new Matrix(1, 0, 0, 0.5f, 0, highlightBounds.Y));
                        g.DrawPath(highlightPen, highlightPath);
                    }
                }
            }
            else
            {
                using (var gradientBrush = new LinearGradientBrush(
                    rect, btnColorLight, btnColor, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(gradientBrush, rect);
                }
            }

            // Draw button border
            if (BorderThickness > 0)
            {
                Color borderColor = isHovered ? 
                    ShiftLuminance(backColor, -0.2f) : 
                    Color.FromArgb(100, ShiftLuminance(backColor, -0.3f));
                    
                using (var pen = new Pen(borderColor, BorderThickness))
                {
                    if (IsRounded && BorderRadius > 0)
                    {
                        using (var path = CreateRoundedRectanglePath(rect, BorderRadius))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                    else
                    {
                        g.DrawRectangle(pen, rect);
                    }
                }
            }

            // Draw button text with anti-aliasing
            var textColor = isPrimary ? (Color.White) : (_currentTheme?.CardTitleForeColor ?? Color.White);
            if (!isPrimary && backColor.GetBrightness() > 0.6f)
            {
                textColor = _currentTheme?.CardTextForeColor ?? Color.Black;
            }
            
            var textFont = _bodyFont ?? _captionFont ?? _headerFont ?? SystemFonts.DefaultFont;
            
            using (var format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                
                using (var brush = new SolidBrush(textColor))
                {
                    g.DrawString(text, textFont, brush, rect, format);
                }
            }
        }

        // Create rounded rectangle path for buttons
        private GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);

            // Top left
            path.AddArc(arc, 180, 90);
            // Top right
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            // Bottom right
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            // Bottom left
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private int Scale(int value) => DpiScalingHelper.ScaleValue(value, this);

        private void DrawAccentBar(Graphics g)
        {
            if (_accentBarHeight <= 0) return;
            int stripHeight = Scale(_accentBarHeight);
            if (stripHeight <= 0) return;

            var accentRect = new Rectangle(DrawingRect.X, DrawingRect.Y, DrawingRect.Width, Math.Min(stripHeight, DrawingRect.Height));
            using var brush = new SolidBrush(_accentColor);
            g.FillRectangle(brush, accentRect);
        }

        private void DrawFocusRing(Graphics g)
        {
            if (!Focused) return;

            float thickness = DpiScalingHelper.ScaleValue(2f, this);
            Color ringColor = Color.FromArgb(160, _currentTheme?.AccentColor ?? _accentColor);
            CardRenderingHelpers.DrawFocusRing(g, DrawingRect, BorderRadius, ringColor, thickness);
        }

        private void DrawRippleOverlay(Graphics g)
        {
            if (_interactionManager == null || _interactionManager.RippleAlpha <= 0 || _interactionManager.RippleRadius <= 0f)
            {
                return;
            }

            Color rippleColor = _currentTheme?.AccentColor ?? _accentColor;
            CardRenderingHelpers.DrawRippleOverlay(
                g,
                DrawingRect,
                BorderRadius,
                _interactionManager.RippleCenter,
                _interactionManager.RippleRadius,
                _interactionManager.RippleAlpha,
                rippleColor);
        }

        private void DrawLoadingSkeleton(Graphics g)
        {
            var surface = _currentTheme?.CardBackColor ?? BackColor;
            var shimmerTint = _currentTheme?.CardTitleForeColor ?? ForeColor;
            var baseColor = Color.FromArgb(60, surface);
            var shimmerColor = Color.FromArgb(130, shimmerTint);

            CardRenderingHelpers.DrawShimmerSkeleton(g, DrawingRect, BorderRadius, _loadingShimmerPhase, baseColor, shimmerColor);

            int pad = Scale(16);
            int lineHeight = Scale(12);
            int lineGap = Scale(8);
            int iconSize = Scale(48);

            Rectangle iconRect = new Rectangle(DrawingRect.X + pad, DrawingRect.Y + pad, iconSize, iconSize);
            Rectangle titleRect = new Rectangle(iconRect.Right + pad, DrawingRect.Y + pad, Math.Max(60, DrawingRect.Width - iconSize - (pad * 3)), lineHeight + Scale(2));
            Rectangle line1Rect = new Rectangle(DrawingRect.X + pad, titleRect.Bottom + lineGap, Math.Max(60, DrawingRect.Width - (pad * 2)), lineHeight);
            Rectangle line2Rect = new Rectangle(DrawingRect.X + pad, line1Rect.Bottom + lineGap, Math.Max(40, (int)(DrawingRect.Width * 0.65f)), lineHeight);
            Rectangle btnRect = new Rectangle(DrawingRect.X + pad, DrawingRect.Bottom - pad - Scale(30), Scale(92), Scale(24));

            using var placeholderBrush = new SolidBrush(Color.FromArgb(85, shimmerTint));
            using var smallRadiusPath = CardRenderingHelpers.CreateRoundedPath(titleRect, Scale(4));
            using var line1Path = CardRenderingHelpers.CreateRoundedPath(line1Rect, Scale(4));
            using var line2Path = CardRenderingHelpers.CreateRoundedPath(line2Rect, Scale(4));
            using var btnPath = CardRenderingHelpers.CreateRoundedPath(btnRect, Scale(6));
            g.FillEllipse(placeholderBrush, iconRect);
            g.FillPath(placeholderBrush, smallRadiusPath);
            g.FillPath(placeholderBrush, line1Path);
            g.FillPath(placeholderBrush, line2Path);
            g.FillPath(placeholderBrush, btnPath);
        }

        private void DrawAuxiliaryIcons(Graphics g)
        {
            Color iconTint = _currentTheme?.CardTitleForeColor ?? ForeColor;

            if (_showSelectionCheckbox && !_selectionRect.IsEmpty)
            {
                float opacity = _isSelected ? 1f : 0.55f;
                StyledImagePainter.PaintWithTint(g, _selectionRect, SvgsUI.CircleCheck, iconTint, opacity, Scale(4));
            }

            if (!string.IsNullOrWhiteSpace(_contextMenuIcon) && !_contextMenuRect.IsEmpty)
            {
                StyledImagePainter.PaintWithTint(g, _contextMenuRect, _contextMenuIcon, iconTint, 0.85f, Scale(4));
            }

            if (_isCollapsible && !_collapseRect.IsEmpty)
            {
                string chevron = _isExpanded ? SvgsUI.CircleChevronUp : SvgsUI.ChevronDown;
                StyledImagePainter.PaintWithTint(g, _collapseRect, chevron, iconTint, 0.85f, Scale(4));
            }
        }

        private void UpdateAuxiliaryHitAreas(LayoutContext ctx)
        {
            int margin = Scale(8);
            int iconSize = Scale(18);

            _selectionRect = Rectangle.Empty;
            _contextMenuRect = Rectangle.Empty;
            _collapseRect = Rectangle.Empty;

            if (ctx == null || ctx.DrawingRect == Rectangle.Empty)
            {
                return;
            }

            if (_showSelectionCheckbox)
            {
                _selectionRect = new Rectangle(
                    ctx.DrawingRect.X + margin,
                    ctx.DrawingRect.Y + margin,
                    iconSize,
                    iconSize);
            }

            if (!string.IsNullOrWhiteSpace(_contextMenuIcon))
            {
                _contextMenuRect = new Rectangle(
                    ctx.DrawingRect.Right - margin - iconSize,
                    ctx.DrawingRect.Y + margin,
                    iconSize,
                    iconSize);
            }

            if (_isCollapsible)
            {
                _collapseRect = new Rectangle(
                    ctx.DrawingRect.Right - margin - iconSize,
                    ctx.DrawingRect.Bottom - margin - iconSize,
                    iconSize,
                    iconSize);
            }
        }

        // Check if a button is hovered
        private bool IsButtonHovered(Rectangle buttonRect)
        {
            if (string.IsNullOrEmpty(_hoveredArea))
                return false;

            return _hoveredArea == "Button" && buttonRect == _layoutContext.ButtonRect ||
                   _hoveredArea == "SecondaryButton" && buttonRect == _layoutContext.SecondaryButtonRect;
        }
    }
}
