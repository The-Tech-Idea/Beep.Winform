using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Material Design Paper style with elevation shadows and layered depth (synced with PaperTheme).
    /// 
    /// Paper Color Palette (synced with PaperTheme):
    /// - Background: #FAFAFA (250, 250, 250) - Off-white paper
    /// - Foreground: #212121 (33, 33, 33) - Dark gray text
    /// - Border: #E0E0E0 (224, 224, 224) - Light gray border
    /// - Hover: #F5F5F5 (245, 245, 245) - Lighter hover
    /// - Selected: #EEEEEE (238, 238, 238) - Medium gray selected
    /// 
    /// Features:
    /// - Subtle material texture (noise pattern)
    /// - Material elevation line at top
    /// - Elevated surface with subtle gradient for caption
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class PaperFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Paper, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);

            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            // This prevents semi-transparent overlays from accumulating on repaint
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;

            // Material Paper: subtle texture with elevation effect
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
            
            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Add subtle material texture (noise pattern)
            Random rand = new Random(owner.ClientRectangle.GetHashCode());
            using (var textureBrush = new SolidBrush(Color.FromArgb(3, 255, 255, 255)))
            {
                for (int i = 0; i < 50; i++)
                {
                    int x = rand.Next(owner.ClientRectangle.Width);
                    int y = rand.Next(owner.ClientRectangle.Height);
                    g.FillRectangle(textureBrush, x, y, 1, 1);
                }
            }
            
            // Material elevation line at top (subtle white, 1px)
            using (var linePen = new Pen(Color.FromArgb(40, 255, 255, 255), 1))
            {
                g.DrawLine(linePen, 0, 0, owner.ClientRectangle.Width, 0);
            }
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // CRITICAL: Set compositing mode for semi-transparent overlays
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Material Paper caption: elevated surface with subtle gradient
            using (var capBrush = new LinearGradientBrush(captionRect, 
                Color.FromArgb(10, 255, 255, 255), 
                Color.FromArgb(5, 0, 0, 0), 
                LinearGradientMode.Vertical))
            {
                var blend = new ColorBlend(3);
                blend.Colors = new[] { 
                    Color.FromArgb(8, 255, 255, 255), 
                    metrics.CaptionColor, 
                    Color.FromArgb(3, 0, 0, 0) 
                };
                blend.Positions = new[] { 0f, 0.5f, 1f };
                capBrush.InterpolationColors = blend;
                
                g.FillRectangle(capBrush, captionRect);
            }
            
            // Material elevation separator line (bottom of caption)
            using (var linePen = new Pen(Color.FromArgb(20, 0, 0, 0), 1))
            {
                g.DrawLine(linePen, 0, captionRect.Bottom - 1, captionRect.Width, captionRect.Bottom - 1);
            }
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;

            // Paint Material Paper textured torn-edge buttons (ENHANCED UNIQUE SKIN)
            PaintPaperTexturedButtons(g, owner, captionRect, metrics);

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom paper texture buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }
        
        /// <summary>
        /// Paint Material Design Paper textured buttons with torn edges (ENHANCED UNIQUE SKIN)
        /// Features: paper texture noise, torn edge effect, ink bleed, fiber pattern
        /// </summary>
        private void PaintPaperTexturedButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            int buttonSize = 20;
            int padding = (captionRect.Height - buttonSize) / 2;
            
            // Close button: Red with paper texture
            PaintPaperTexturedButton(g, closeRect, Color.FromArgb(232, 17, 35), padding, buttonSize, "close");
            
            // Maximize button: Green with paper texture
            PaintPaperTexturedButton(g, maxRect, Color.FromArgb(16, 124, 16), padding, buttonSize, "maximize");
            
            // Minimize button: Blue with paper texture
            PaintPaperTexturedButton(g, minRect, Color.FromArgb(0, 120, 215), padding, buttonSize, "minimize");
            
            // Theme/Style buttons if shown
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintPaperTexturedButton(g, styleRect, Color.FromArgb(135, 100, 184), padding, buttonSize, "Style");
            }

            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintPaperTexturedButton(g, themeRect, Color.FromArgb(247, 99, 12), padding, buttonSize, "theme");
            }
        }
        
        private void PaintPaperTexturedButton(Graphics g, Rectangle buttonRect, Color baseColor, int padding, int size, string buttonType)
        {
            int centerX = buttonRect.X + buttonRect.Width / 2;
            int centerY = buttonRect.Y + buttonRect.Height / 2;
            int radius = size / 2;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Paper texture background
            DrawPaperTexture(g, new Rectangle(centerX - radius, centerY - radius, size, size));

            // Torn circle path (jagged edges)
            using (var tornPath = CreateTornCirclePath(centerX, centerY, radius))
            {
                // Ink bleed effect (blurred outer rings)
                for (int i = 3; i > 0; i--)
                {
                    using (var bleedBrush = new SolidBrush(Color.FromArgb(20 * i, baseColor)))
                    using (var bleedPath = CreateTornCirclePath(centerX, centerY, radius + i))
                    {
                        g.FillPath(bleedBrush, bleedPath);
                    }
                }

                // Main button fill
                using (var paperBrush = new SolidBrush(baseColor))
                {
                    g.FillPath(paperBrush, tornPath);
                }

                // Double border (material design rings)
                using (var outerBorderPen = new Pen(ControlPaint.Dark(baseColor, 0.3f), 2))
                {
                    g.DrawPath(outerBorderPen, tornPath);
                }
                
                using (var innerBorderPath = CreateTornCirclePath(centerX, centerY, radius - 2))
                using (var innerBorderPen = new Pen(ControlPaint.Light(baseColor, 0.2f), 1))
                {
                    g.DrawPath(innerBorderPen, innerBorderPath);
                }
            }

            // Fiber pattern (random short lines on button surface)
            DrawFiberPattern(g, new Rectangle(centerX - radius, centerY - radius, size, size));

            // Draw icon
            using (var iconPen = new Pen(Color.White, 1.5f))
            {
                int iconSize = 7;

                switch (buttonType)
                {
                    case "close":
                        g.DrawLine(iconPen, centerX - iconSize / 2, centerY - iconSize / 2,
                            centerX + iconSize / 2, centerY + iconSize / 2);
                        g.DrawLine(iconPen, centerX + iconSize / 2, centerY - iconSize / 2,
                            centerX - iconSize / 2, centerY + iconSize / 2);
                        break;
                    case "maximize":
                        g.DrawRectangle(iconPen, centerX - iconSize / 2, centerY - iconSize / 2, iconSize, iconSize);
                        break;
                    case "minimize":
                        g.DrawLine(iconPen, centerX - iconSize / 2, centerY, centerX + iconSize / 2, centerY);
                        break;
                    case "Style":
                        // Palette icon
                        g.DrawEllipse(iconPen, centerX - iconSize / 2, centerY - iconSize / 2, iconSize, iconSize);
                        g.FillEllipse(Brushes.White, centerX - 1, centerY - 1, 2, 2);
                        break;
                    case "theme":
                        // Paper stack icon
                        g.DrawRectangle(iconPen, centerX - iconSize / 2, centerY - iconSize / 2 + 1, iconSize, iconSize - 2);
                        g.DrawLine(iconPen, centerX - iconSize / 2 + 1, centerY - iconSize / 2, 
                            centerX + iconSize / 2 + 1, centerY - iconSize / 2);
                        break;
                }
            }
        }

        /// <summary>
        /// Create torn circle path with jagged edges
        /// </summary>
        private GraphicsPath CreateTornCirclePath(int centerX, int centerY, int radius)
        {
            var path = new GraphicsPath();
            var points = new PointF[36];
            var random = new Random(radius); // Consistent seed for stable jitter

            for (int i = 0; i < 36; i++)
            {
                double angle = (Math.PI * 2 / 36 * i);
                float jitter = (float)(random.NextDouble() * 1.5 - 0.75); // ±0.75 jitter
                float r = radius + jitter;

                points[i] = new PointF(
                    centerX + (float)(r * Math.Cos(angle)),
                    centerY + (float)(r * Math.Sin(angle))
                );
            }

            path.AddPolygon(points);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Draw paper texture noise (subtle grain)
        /// </summary>
        private void DrawPaperTexture(Graphics g, Rectangle rect)
        {
            var random = new Random(rect.GetHashCode());
            using (var noisePen = new Pen(Color.FromArgb(8, 100, 80, 60), 1))
            {
                for (int i = 0; i < 50; i++)
                {
                    int x = rect.X + random.Next(rect.Width);
                    int y = rect.Y + random.Next(rect.Height);
                    g.DrawRectangle(noisePen, x, y, 1, 1);
                }
            }
        }

        /// <summary>
        /// Draw fiber pattern (random short lines simulating paper fibers)
        /// </summary>
        private void DrawFiberPattern(Graphics g, Rectangle rect)
        {
            var random = new Random(rect.GetHashCode() + 1);
            using (var fiberPen = new Pen(Color.FromArgb(40, 255, 255, 255), 1))
            {
                for (int i = 0; i < 12; i++)
                {
                    int x = rect.X + random.Next(rect.Width);
                    int y = rect.Y + random.Next(rect.Height);
                    int length = random.Next(2, 6);
                    double angle = random.NextDouble() * Math.PI * 2;

                    int x2 = x + (int)(Math.Cos(angle) * length);
                    int y2 = y + (int)(Math.Sin(angle) * length);

                    g.DrawLine(fiberPen, x, y, x2, y2);
                }
            }
        }
        
        /// <summary>
        /// Paint Material Design Paper double-border circle buttons (ORIGINAL - KEPT FOR COMPATIBILITY)
        /// Concentric ring buttons with material elevation shadows
        /// </summary>
        private void PaintPaperCircleButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            int circleDiameter = 22; // Circle button size
            int circleY = (captionRect.Height - circleDiameter) / 2;
            
            // Close button: Red double-border circle with X
            int cx = closeRect.X + closeRect.Width / 2;
            int cy = circleY + circleDiameter / 2;
            
            // Outer ring (2px, darker red)
            using (var outerPen = new Pen(Color.FromArgb(180, 50, 50), 2))
            {
                g.DrawEllipse(outerPen, cx - circleDiameter/2, cy - circleDiameter/2, circleDiameter, circleDiameter);
            }
            
            // Inner ring (1px, lighter red, inset by 3px)
            using (var innerPen = new Pen(Color.FromArgb(220, 80, 80), 1))
            {
                int innerSize = circleDiameter - 6;
                g.DrawEllipse(innerPen, cx - innerSize/2, cy - innerSize/2, innerSize, innerSize);
            }
            
            // X icon (8px, centered)
            using (var iconPen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.5f))
            {
                int iconSize = 8;
                g.DrawLine(iconPen, cx - iconSize/2, cy - iconSize/2, cx + iconSize/2, cy + iconSize/2);
                g.DrawLine(iconPen, cx + iconSize/2, cy - iconSize/2, cx - iconSize/2, cy + iconSize/2);
            }
            
            // Maximize button: Gray double-border circle with square
            int mx = maxRect.X + maxRect.Width / 2;
            int my = circleY + circleDiameter / 2;
            
            // Outer ring (2px, dark gray)
            using (var outerPen = new Pen(Color.FromArgb(100, 100, 100), 2))
            {
                g.DrawEllipse(outerPen, mx - circleDiameter/2, my - circleDiameter/2, circleDiameter, circleDiameter);
            }
            
            // Inner ring (1px, lighter gray)
            using (var innerPen = new Pen(Color.FromArgb(140, 140, 140), 1))
            {
                int innerSize = circleDiameter - 6;
                g.DrawEllipse(innerPen, mx - innerSize/2, my - innerSize/2, innerSize, innerSize);
            }
            
            // Square icon (7px)
            using (var iconPen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.5f))
            {
                int sqSize = 7;
                g.DrawRectangle(iconPen, mx - sqSize/2, my - sqSize/2, sqSize, sqSize);
            }
            
            // Minimize button: Gray double-border circle with line
            int mnx = minRect.X + minRect.Width / 2;
            int mny = circleY + circleDiameter / 2;
            
            // Outer ring (2px, dark gray)
            using (var outerPen = new Pen(Color.FromArgb(100, 100, 100), 2))
            {
                g.DrawEllipse(outerPen, mnx - circleDiameter/2, mny - circleDiameter/2, circleDiameter, circleDiameter);
            }
            
            // Inner ring (1px, lighter gray)
            using (var innerPen = new Pen(Color.FromArgb(140, 140, 140), 1))
            {
                int innerSize = circleDiameter - 6;
                g.DrawEllipse(innerPen, mnx - innerSize/2, mny - innerSize/2, innerSize, innerSize);
            }
            
            // Line icon (8px horizontal)
            using (var iconPen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.5f))
            {
                int lineSize = 8;
                g.DrawLine(iconPen, mnx - lineSize/2, mny, mnx + lineSize/2, mny);
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
             using var path = owner.BorderShape; 
            // Material Paper: subtle elevated border (1px with alpha)
            using var pen = new Pen(Color.FromArgb(40, metrics.BorderColor), 1);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Material Design Paper: LAYERED elevation shadow (multiple blur layers for depth)
            // This creates the "floating paper" effect characteristic of Material Design
            return new ShadowEffect
            {
                Color = Color.FromArgb(50, 0, 0, 0), // Deeper shadow for elevation
                Blur = 12, // Wider blur for Material elevation (not flat like Arc)
                OffsetY = 6, // Larger offset for elevated paper effect
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(7); // Material Paper smooth corners (not 4px like Arc)
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.High; // Crisp but smooth
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
