using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Neon-lit futuristic dystopian style.
    /// 
    /// Cyberpunk Color Palette (synced with CyberpunkTheme):
    /// - Background: #0A0A14 (10, 10, 20) - Deep dark background
    /// - Foreground: #00FFFF (0, 255, 255) - Neon cyan
    /// - Border: #00FFFF (0, 255, 255) - Neon cyan
    /// - Accent 1: #FF0064 (255, 0, 100) - Neon magenta/red
    /// - Accent 2: #FF00FF (255, 0, 255) - Neon magenta
    /// - Accent 3: #FF9600 (255, 150, 0) - Neon orange
    /// 
    /// Features:
    /// - Scan line overlay (cyan tint, every 4 pixels)
    /// - Random glitch effect (5% chance)
    /// - Multi-layer neon glow on buttons (3 layers)
    /// - Hexagon button shapes
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class CyberpunkFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Cyberpunk, owner.UseThemeColors ? owner.CurrentTheme : null);
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
            
            // Cyberpunk: Flat dark background (#0A0A14)
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillPath(brush, path);
            }
            
            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Scan line overlay for digital screen effect (neon cyan #00FFFF)
            using (var scanPen = new Pen(Color.FromArgb(15, 0, 255, 255), 1))
            {
                for (int y = 0; y < owner.ClientRectangle.Height; y += 4)
                {
                    g.DrawLine(scanPen, 0, y, owner.ClientRectangle.Width, y);
                }
            }
            
            // Glitch effect - random offset rectangles (5% chance)
            var random = new Random(owner.ClientRectangle.GetHashCode());
            if (random.Next(100) < 5)
            {
                var glitchRect = new Rectangle(
                    random.Next(owner.ClientRectangle.Width / 2),
                    random.Next(owner.ClientRectangle.Height),
                    random.Next(50, 150),
                    2);
                using (var glitchBrush = new SolidBrush(Color.FromArgb(80, 0, 255, 255)))
                {
                    g.FillRectangle(glitchBrush, glitchRect);
                }
            }
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            
            // Cyberpunk: Flat caption with neon accent line
            using var capBrush = new SolidBrush(metrics.CaptionColor);
            g.FillRectangle(capBrush, captionRect);
            
            // Neon accent line at bottom of caption
            using (var neonPen = new Pen(metrics.BorderColor, 2))
            {
                g.DrawLine(neonPen, 0, captionRect.Bottom - 1, captionRect.Width, captionRect.Bottom - 1);
            }

            // Paint Cyberpunk neon hexagon buttons
            PaintCyberpunkNeonButtons(g, owner, captionRect, metrics);

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom neon buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }

        /// <summary>
        /// Paint Cyberpunk-Style neon hexagon buttons with multi-layer glow
        /// </summary>
        private void PaintCyberpunkNeonButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Close button: Red neon hexagon
            PaintNeonHexagonButton(g, closeRect, Color.FromArgb(255, 0, 100), "close");

            // Maximize button: Cyan neon hexagon
            PaintNeonHexagonButton(g, maxRect, Color.FromArgb(0, 255, 255), "maximize");

            // Minimize button: Magenta neon hexagon
            PaintNeonHexagonButton(g, minRect, Color.FromArgb(255, 0, 255), "minimize");

            // Theme button (if shown)
            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintNeonHexagonButton(g, themeRect, Color.FromArgb(0, 255, 150), "theme");
            }

            // Style button (if shown)
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintNeonHexagonButton(g, styleRect, Color.FromArgb(255, 200, 0), "Style");
            }
        }

        /// <summary>
        /// Paint a single neon hexagon button with multi-layer glow effect
        /// </summary>
        private void PaintNeonHexagonButton(Graphics g, Rectangle buttonRect, Color neonColor, string buttonType)
        {
            int centerX = buttonRect.X + buttonRect.Width / 2;
            int centerY = buttonRect.Y + buttonRect.Height / 2;
            int size = 18; // Hexagon size

            // Create hexagon path
            var hexPath = CreateHexagonPath(centerX, centerY, size);

            // Multi-layer neon glow (3 layers)
            // Layer 1: Outer glow (blur 12)
            using (var glowPen1 = new Pen(Color.FromArgb(30, neonColor), 12))
            {
                g.DrawPath(glowPen1, hexPath);
            }

            // Layer 2: Mid glow (blur 6)
            using (var glowPen2 = new Pen(Color.FromArgb(60, neonColor), 6))
            {
                g.DrawPath(glowPen2, hexPath);
            }

            // Layer 3: Inner glow (blur 3)
            using (var glowPen3 = new Pen(Color.FromArgb(100, neonColor), 3))
            {
                g.DrawPath(glowPen3, hexPath);
            }

            // Fill hexagon with dark background
            using (var fillBrush = new SolidBrush(Color.FromArgb(255, 10, 10, 20)))
            {
                g.FillPath(fillBrush, hexPath);
            }

            // Core neon outline (2px solid)
            using (var corePen = new Pen(neonColor, 2))
            {
                g.DrawPath(corePen, hexPath);
            }

            // Draw icon in neon color
            using (var iconPen = new Pen(neonColor, 2f))
            {
                int iconSize = 8;

                switch (buttonType)
                {
                    case "close":
                        // X icon
                        g.DrawLine(iconPen, centerX - iconSize / 2, centerY - iconSize / 2,
                            centerX + iconSize / 2, centerY + iconSize / 2);
                        g.DrawLine(iconPen, centerX + iconSize / 2, centerY - iconSize / 2,
                            centerX - iconSize / 2, centerY + iconSize / 2);
                        break;

                    case "maximize":
                        // Square icon
                        g.DrawRectangle(iconPen, centerX - iconSize / 2, centerY - iconSize / 2, iconSize, iconSize);
                        break;

                    case "minimize":
                        // Horizontal line icon
                        g.DrawLine(iconPen, centerX - iconSize / 2, centerY, centerX + iconSize / 2, centerY);
                        break;

                    case "theme":
                        // Palette/hexagon icon
                        var smallHex = CreateHexagonPath(centerX, centerY, iconSize / 2);
                        g.DrawPath(iconPen, smallHex);
                        break;

                    case "Style":
                        // Brush/paint icon (triangle)
                        var points = new PointF[]
                        {
                            new PointF(centerX, centerY - iconSize/2),
                            new PointF(centerX - iconSize/2, centerY + iconSize/2),
                            new PointF(centerX + iconSize/2, centerY + iconSize/2)
                        };
                        g.DrawPolygon(iconPen, points);
                        break;
                }
            }

            hexPath.Dispose();
        }

        /// <summary>
        /// Create a hexagon path centered at the given point
        /// </summary>
        private GraphicsPath CreateHexagonPath(int centerX, int centerY, int size)
        {
            var path = new GraphicsPath();
            var points = new PointF[6];

            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i; // 60 degrees per side
                points[i] = new PointF(
                    centerX + (float)(size * Math.Cos(angle)),
                    centerY + (float)(size * Math.Sin(angle))
                );
            }

            path.AddPolygon(points);
            path.CloseFigure();
            return path;
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
             using var path = owner.BorderShape;
            
            // Cyberpunk: Multi-layer neon glow on border
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Outer glow layers (3 layers for intense neon effect)
            var glowColor = metrics.BorderColor;
            
            // Layer 1: Outermost glow (blur 30)
            using (var glowPen1 = new Pen(Color.FromArgb(20, glowColor), 30))
            {
                g.DrawPath(glowPen1, path);
            }
            
            // Layer 2: Mid glow (blur 15)
            using (var glowPen2 = new Pen(Color.FromArgb(40, glowColor), 15))
            {
                g.DrawPath(glowPen2, path);
            }
            
            // Layer 3: Inner glow (blur 8)
            using (var glowPen3 = new Pen(Color.FromArgb(60, glowColor), 8))
            {
                g.DrawPath(glowPen3, path);
            }
            
            // Core border (solid neon)
            using (var corePen = new Pen(glowColor, 2))
            {
                g.DrawPath(corePen, path);
            }
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Cyberpunk: Colored glow shadow
            var metrics = GetMetrics(owner);
            return new ShadowEffect
            {
                Color = Color.FromArgb(40, metrics.BorderColor), // Colored shadow for glow
                Blur = 25, // Large blur for neon glow
                OffsetX = 0,
                OffsetY = 8,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(2); // Sharp with minimal rounding
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.High;
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
            
            owner._hits.Clear();
            
            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                owner.CurrentLayout = layout;
                return;
            }
            
            int captionHeight = Math.Max(metrics.CaptionHeight, (int)(owner.Font.Height * metrics.FontHeightMultiplier));
            
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            owner._hits.Register("caption", layout.CaptionRect, HitAreaType.Drag);
            
            int buttonWidth = metrics.ButtonWidth;
            int buttonX = owner.ClientSize.Width - buttonWidth;
            
            if (owner.ShowCloseButton)
            {
                layout.CloseButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            if (owner.ShowMinMaxButtons)
            {
                layout.MaximizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
                
                layout.MinimizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
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
            
            // Custom action button (only if ShowCustomActionButton is true)
            if (owner.ShowCustomActionButton)
            {
                layout.CustomActionButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
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
