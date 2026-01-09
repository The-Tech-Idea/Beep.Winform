using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Clean Scandinavian minimalist design (synced with NordicTheme).
    /// 
    /// Nordic Color Palette (synced with NordicTheme):
    /// - Background: #ECEFF4 (236, 239, 244) - Very light blue-gray base
    /// - Foreground: #2E3440 (46, 52, 64) - Dark blue-gray text
    /// - Border: #D8DEE9 (216, 222, 233) - Light blue-gray border
    /// - Hover: #E5E9F0 (229, 233, 240) - Very light blue-gray hover
    /// - Selected: #88C0D0 (136, 192, 208) - Nordic cyan selected
    /// 
    /// Features:
    /// - Clean minimalist design
    /// - Very subtle gradient (5% variation)
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class NordicFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Nordic, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            
            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            // This prevents semi-transparent overlays from accumulating on repaint
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            
            // Nordic: Clean with very subtle gradient (5% variation)
            var lightColor = ControlPaint.Light(metrics.BackgroundColor, 0.05f);
            using (var gradBrush = new LinearGradientBrush(
                owner.ClientRectangle,
                lightColor,
                metrics.BackgroundColor,
                LinearGradientMode.Vertical))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(gradBrush, path);
            }
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            
            using var capBrush = new SolidBrush(metrics.CaptionColor);
            g.FillRectangle(capBrush, captionRect);

            // Paint Nordic Viking rune buttons (ENHANCED UNIQUE SKIN)
            PaintNordicRuneButtons(g, owner, captionRect, metrics);

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom Nordic rune buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }

        /// <summary>
        /// Paint Nordic Viking rune buttons (ENHANCED UNIQUE SKIN)
        /// Features: Viking rune patterns, wood grain texture, natural aesthetic
        /// </summary>
        private void PaintNordicRuneButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;

            int buttonSize = 20;
            int padding = (captionRect.Height - buttonSize) / 2;

            // Close button: Red with rune pattern
            PaintRuneButton(g, closeRect, Color.FromArgb(180, 50, 50), padding, buttonSize, "close");

            // Maximize button: Green with rune pattern
            PaintRuneButton(g, maxRect, Color.FromArgb(80, 140, 90), padding, buttonSize, "maximize");

            // Minimize button: Blue with rune pattern
            PaintRuneButton(g, minRect, Color.FromArgb(70, 110, 140), padding, buttonSize, "minimize");

            // Theme/Style buttons if shown
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintRuneButton(g, styleRect, Color.FromArgb(120, 90, 130), padding, buttonSize, "Style");
            }

            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintRuneButton(g, themeRect, Color.FromArgb(160, 120, 70), padding, buttonSize, "theme");
            }
        }

        private void PaintRuneButton(Graphics g, Rectangle buttonRect, Color baseColor, int padding, int size, string buttonType)
        {
            int centerX = buttonRect.X + buttonRect.Width / 2;
            int centerY = buttonRect.Y + buttonRect.Height / 2;
            var rect = new Rectangle(centerX - size / 2, centerY - size / 2, size, size);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Wood grain texture background
            DrawWoodGrain(g, rect, baseColor);

            // Viking rune pattern border
            DrawRunePattern(g, rect, baseColor);

            // Minimalist rectangle with subtle rounding
            using (var path = CreateRoundedRectanglePath(rect, new CornerRadius(3)))
            {
                // Subtle gradient fill (natural color variation)
                using (var gradientBrush = new LinearGradientBrush(rect,
                    ControlPaint.Light(baseColor, 0.08f),
                    ControlPaint.Dark(baseColor, 0.05f),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(gradientBrush, path);
                }

                // Natural border (1px, slightly darker)
                using (var borderPen = new Pen(ControlPaint.Dark(baseColor, 0.15f), 1))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            // Draw icon
            using (var iconPen = new Pen(Color.FromArgb(230, 230, 220), 1.5f))
            {
                int iconSize = 7;
                int iconCenterX = rect.X + rect.Width / 2;
                int iconCenterY = rect.Y + rect.Height / 2;

                switch (buttonType)
                {
                    case "close":
                        g.DrawLine(iconPen, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2,
                            iconCenterX + iconSize / 2, iconCenterY + iconSize / 2);
                        g.DrawLine(iconPen, iconCenterX + iconSize / 2, iconCenterY - iconSize / 2,
                            iconCenterX - iconSize / 2, iconCenterY + iconSize / 2);
                        break;
                    case "maximize":
                        g.DrawRectangle(iconPen, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2, iconSize, iconSize);
                        break;
                    case "minimize":
                        g.DrawLine(iconPen, iconCenterX - iconSize / 2, iconCenterY, iconCenterX + iconSize / 2, iconCenterY);
                        break;
                    case "Style":
                        // Tree icon (Nordic nature)
                        g.DrawLine(iconPen, iconCenterX, iconCenterY - iconSize / 2, iconCenterX, iconCenterY + iconSize / 2);
                        g.DrawLine(iconPen, iconCenterX - iconSize / 3, iconCenterY - iconSize / 4,
                            iconCenterX + iconSize / 3, iconCenterY - iconSize / 4);
                        g.DrawLine(iconPen, iconCenterX - iconSize / 4, iconCenterY,
                            iconCenterX + iconSize / 4, iconCenterY);
                        break;
                    case "theme":
                        // Mountain icon (Nordic landscape)
                        var points = new PointF[] {
                            new PointF(iconCenterX - iconSize / 2, iconCenterY + iconSize / 3),
                            new PointF(iconCenterX, iconCenterY - iconSize / 2),
                            new PointF(iconCenterX + iconSize / 2, iconCenterY + iconSize / 3)
                        };
                        g.DrawPolygon(iconPen, points);
                        break;
                }
            }
        }

        /// <summary>
        /// Draw wood grain texture (Nordic natural aesthetic)
        /// </summary>
        private void DrawWoodGrain(Graphics g, Rectangle rect, Color baseColor)
        {
            var random = new Random(rect.GetHashCode());
            
            // Draw horizontal grain lines
            using (var grainPen = new Pen(Color.FromArgb(15, 80, 60, 40), 1))
            {
                for (int i = 0; i < 8; i++)
                {
                    int y = rect.Y + random.Next(rect.Height);
                    int offset = random.Next(-2, 3);
                    
                    var points = new PointF[5];
                    for (int j = 0; j < 5; j++)
                    {
                        float x = rect.X + (rect.Width * j / 4f);
                        float yPos = y + (float)(Math.Sin(j * 1.5) * 1.5) + offset;
                        points[j] = new PointF(x, yPos);
                    }
                    g.DrawCurve(grainPen, points, 0.3f);
                }
            }
        }

        /// <summary>
        /// Draw Viking rune pattern (Nordic heritage)
        /// </summary>
        private void DrawRunePattern(Graphics g, Rectangle rect, Color baseColor)
        {
            using (var runePen = new Pen(Color.FromArgb(25, 100, 80, 60), 1))
            {
                // Simple angular rune-like marks at corners
                // Top-left rune mark
                g.DrawLine(runePen, rect.X, rect.Y + 2, rect.X + 3, rect.Y);
                g.DrawLine(runePen, rect.X, rect.Y + 4, rect.X + 3, rect.Y + 2);
                
                // Top-right rune mark
                g.DrawLine(runePen, rect.Right - 3, rect.Y, rect.Right, rect.Y + 2);
                g.DrawLine(runePen, rect.Right - 3, rect.Y + 2, rect.Right, rect.Y + 4);
                
                // Bottom-left rune mark
                g.DrawLine(runePen, rect.X, rect.Bottom - 4, rect.X + 3, rect.Bottom - 2);
                g.DrawLine(runePen, rect.X, rect.Bottom - 2, rect.X + 3, rect.Bottom);
                
                // Bottom-right rune mark
                g.DrawLine(runePen, rect.Right - 3, rect.Bottom - 2, rect.Right, rect.Bottom - 4);
                g.DrawLine(runePen, rect.Right - 3, rect.Bottom, rect.Right, rect.Bottom - 2);
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
           using var path = owner.BorderShape;  
            // Nordic: Thin, clean border - minimalist
            using var pen = new Pen(metrics.BorderColor, 1);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Nordic: Very soft, subtle shadow
            return new ShadowEffect
            {
                Color = Color.FromArgb(15, 0, 0, 0), // Very low opacity
                Blur = 10, // Medium blur
                OffsetX = 0,
                OffsetY = 4,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(8); // Medium, soft rounding
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.Ultra; // Ultra smooth for clean aesthetic
        }

        public bool SupportsAnimations => false;

        public void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            var originalClip = g.Clip;
            var shadow = GetShadowEffect(owner);
            var radius = GetCornerRadius(owner);

            if (!shadow.Inner)
            {
                DrawShadow(g, rect, shadow, radius);
            }

            PaintBackground(g, owner);

            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            g.Clip = new Region(path);
            g.Clip = originalClip;

            PaintBorders(g, owner);
            if (owner.ShowCaptionBar)
            {
                PaintCaption(g, owner, owner.CurrentLayout.CaptionRect);
            }

            g.Clip = originalClip;
        }

        private void DrawShadow(Graphics g, Rectangle rect, ShadowEffect shadow, CornerRadius radius)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            var shadowRect = new Rectangle(
                rect.X + shadow.OffsetX - shadow.Blur,
                rect.Y + shadow.OffsetY - shadow.Blur,
                rect.Width + shadow.Blur * 2,
                rect.Height + shadow.Blur * 2);
            if (shadowRect.Width <= 0 || shadowRect.Height <= 0) return;
            using var brush = new SolidBrush(shadow.Color);
            using var path = CreateRoundedRectanglePath(shadowRect, radius);
            g.FillPath(brush, path);
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, CornerRadius radius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(new Rectangle(rect.X, rect.Y, Math.Max(1, rect.Width), Math.Max(1, rect.Height)));
                return path;
            }
            int maxRadius = Math.Min(rect.Width, rect.Height) / 2;
            int tl = Math.Max(0, Math.Min(radius.TopLeft, maxRadius));
            int tr = Math.Max(0, Math.Min(radius.TopRight, maxRadius));
            int br = Math.Max(0, Math.Min(radius.BottomRight, maxRadius));
            int bl = Math.Max(0, Math.Min(radius.BottomLeft, maxRadius));
            if (tl == 0 && tr == 0 && br == 0 && bl == 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            if (tl > 0) path.AddArc(rect.X, rect.Y, tl * 2, tl * 2, 180, 90); else path.AddLine(rect.X, rect.Y, rect.X, rect.Y);
            if (tr > 0) path.AddArc(rect.Right - tr * 2, rect.Y, tr * 2, tr * 2, 270, 90); else path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);
            if (br > 0) path.AddArc(rect.Right - br * 2, rect.Bottom - br * 2, br * 2, br * 2, 0, 90); else path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);
            if (bl > 0) path.AddArc(rect.X, rect.Bottom - bl * 2, bl * 2, bl * 2, 90, 90); else path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom);
            path.CloseFigure();
            return path;
        }

        public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
        {
            var layout = new PainterLayoutInfo();
            var metrics = GetMetrics(owner);
            
            int captionHeight = Math.Max(metrics.CaptionHeight, (int)(owner.Font.Height * metrics.FontHeightMultiplier));
            // NOTE: _hits.Clear() is handled by EnsureLayoutCalculated - do not call here
            
            // Check if caption bar should be hidden
            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                owner.CurrentLayout = layout;
                return;
            }
            
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            owner._hits.Register("caption", layout.CaptionRect, HitAreaType.Drag);
            
            int buttonWidth = metrics.ButtonWidth;
            int buttonX = owner.ClientSize.Width - buttonWidth;
            
            if (owner.ShowCloseButton)
            {
                layout.CloseButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.Register("close", layout.CloseButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            if (owner.ShowMinMaxButtons)
            {
                layout.MaximizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.Register("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
                
                layout.MinimizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.Register("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Style button (if shown)
            if (owner.ShowStyleButton)
            {
                layout.StyleButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("Style", layout.StyleButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Theme button (if shown)
            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Custom action button (fallback)
            // Custom action button (only if ShowCustomActionButton is true)
            if (owner.ShowCustomActionButton)
            {
                layout.CustomActionButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Search box (between title and buttons)
            int searchBoxWidth = 200;
            int searchBoxPadding = 8;
            if (owner.ShowSearchBox)
            {
                layout.SearchBoxRect = new Rectangle(buttonX - searchBoxWidth - searchBoxPadding, searchBoxPadding / 2, 
                    searchBoxWidth, captionHeight - searchBoxPadding);
                owner._hits.RegisterHitArea("search", layout.SearchBoxRect, HitAreaType.TextBox);
                buttonX -= searchBoxWidth + searchBoxPadding;
            }
            else
            {
                layout.SearchBoxRect = Rectangle.Empty;
            }
            
            int iconX = metrics.IconLeftPadding;
            int iconY = (captionHeight - metrics.IconSize) / 2;
            layout.IconRect = new Rectangle(iconX, iconY, metrics.IconSize, metrics.IconSize);
            if (owner.ShowIcon && owner.Icon != null)
            {
                owner._hits.Register("icon", layout.IconRect, HitAreaType.Icon);
            }
            
            int titleX = layout.IconRect.Right + metrics.TitleLeftPadding;
            int titleWidth = buttonX - titleX - metrics.ButtonSpacing;
            layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
            
            owner.CurrentLayout = layout;
        }

        public void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            var outer = new Rectangle(0, 0, owner.Width, owner.Height);
            using var path = CreateRoundedRectanglePath(outer, radius);
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, borderThickness))
            {
                Alignment = PenAlignment.Inset
            };
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }
    }
}
