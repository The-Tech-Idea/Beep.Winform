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
            return owner.MetricsFor(FormStyle.Paper);
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

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            // The custom action slot, drawn on the same terms as its neighbours. Painters that
            // draw their own button row must draw this one too: only two painters route through
            // PaintBuiltInCaptionElements, and not one painter invoked this region, so the slot
            // was laid out and hit-tested but never painted in 33 of 37 skins.
            if (owner.ShowCustomActionButton && owner.CurrentLayout.CustomActionButtonRect.Width > 0)
            {
                owner.CustomActionButton?.OnPaint?.Invoke(g, owner.CurrentLayout.CustomActionButtonRect);
            }

            if (owner.ShowProfileButton && owner.CurrentLayout.ProfileButtonRect.Width > 0)
            {
                owner.ProfileButton?.OnPaint?.Invoke(g, owner.CurrentLayout.ProfileButtonRect);
            }

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
            
            // Check hover states
            bool closeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea(FormHitAreaNames.Close)) ?? false;
            bool maxHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea(FormHitAreaNames.Maximize)) ?? false;
            bool minHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea(FormHitAreaNames.Minimize)) ?? false;
            bool themeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea(FormHitAreaNames.Theme)) ?? false;
            bool styleHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea(FormHitAreaNames.Style)) ?? false;
            
            // Close button: Red with paper texture
            PaintPaperTexturedButton(g, closeRect, Color.FromArgb(232, 17, 35), padding, buttonSize, FormHitAreaNames.Close, closeHovered);
            
            // Maximize button: Green with paper texture
            PaintPaperTexturedButton(g, maxRect, Color.FromArgb(16, 124, 16), padding, buttonSize, FormHitAreaNames.Maximize, maxHovered);
            
            // Minimize button: Blue with paper texture
            PaintPaperTexturedButton(g, minRect, Color.FromArgb(0, 120, 215), padding, buttonSize, FormHitAreaNames.Minimize, minHovered);
            
            // Theme/Style buttons if shown
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintPaperTexturedButton(g, styleRect, Color.FromArgb(135, 100, 184), padding, buttonSize, FormHitAreaNames.Style, styleHovered);
            }

            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintPaperTexturedButton(g, themeRect, Color.FromArgb(247, 99, 12), padding, buttonSize, FormHitAreaNames.Theme, themeHovered);
            }
        }
        
        private void PaintPaperTexturedButton(Graphics g, Rectangle buttonRect, Color baseColor, int padding, int size, string buttonType, bool isHovered = false)
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
                // Hover: darker/more intense ink bleed
                int bleedOpacity = isHovered ? 30 : 20;
                for (int i = 3; i > 0; i--)
                {
                    using (var bleedBrush = new SolidBrush(Color.FromArgb(bleedOpacity * i, baseColor)))
                    using (var bleedPath = CreateTornCirclePath(centerX, centerY, radius + i))
                    {
                        g.FillPath(bleedBrush, bleedPath);
                    }
                }

                // Main button fill
                // Hover: slightly darkened fill to simulate ink saturation
                Color fillColor = isHovered ? ShiftLuminance(baseColor, -0.1f) : baseColor;
                using (var paperBrush = new SolidBrush(fillColor))
                {
                    g.FillPath(paperBrush, tornPath);
                }

                // Double border (material design rings)
                // Hover: thicker outer border
                float outerWidth = isHovered ? 2.5f : 2f;
                using (var outerBorderPen = new Pen(ShiftLuminance(baseColor, -0.3f), outerWidth))
                {
                    g.DrawPath(outerBorderPen, tornPath);
                }
                
                using (var innerBorderPath = CreateTornCirclePath(centerX, centerY, radius - 2))
                using (var innerBorderPen = new Pen(ShiftLuminance(baseColor, 0.2f), 1))
                {
                    g.DrawPath(innerBorderPen, innerBorderPath);
                }
            }

            // Fiber pattern (random short lines on button surface)
            DrawFiberPattern(g, new Rectangle(centerX - radius, centerY - radius, size, size));

            // Draw icon
            // Hover: slightly thicker icon
            float iconThickness = isHovered ? 2f : 1.5f;
            using (var iconPen = new Pen(Color.White, iconThickness))
            {
                int iconSize = 7;

                switch (buttonType)
                {
                    case FormHitAreaNames.Close:
                        g.DrawLine(iconPen, centerX - iconSize / 2, centerY - iconSize / 2,
                            centerX + iconSize / 2, centerY + iconSize / 2);
                        g.DrawLine(iconPen, centerX + iconSize / 2, centerY - iconSize / 2,
                            centerX - iconSize / 2, centerY + iconSize / 2);
                        break;
                    case FormHitAreaNames.Maximize:
                        g.DrawRectangle(iconPen, centerX - iconSize / 2, centerY - iconSize / 2, iconSize, iconSize);
                        break;
                    case FormHitAreaNames.Minimize:
                        g.DrawLine(iconPen, centerX - iconSize / 2, centerY, centerX + iconSize / 2, centerY);
                        break;
                    case FormHitAreaNames.Style:
                        // Palette icon
                        g.DrawEllipse(iconPen, centerX - iconSize / 2, centerY - iconSize / 2, iconSize, iconSize);
                        g.FillEllipse(Brushes.White, centerX - 1, centerY - 1, 2, 2);
                        break;
                    case FormHitAreaNames.Theme:
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
        
        // PaintPaperCircleButtons was here, documented as
        // "(ORIGINAL - KEPT FOR COMPATIBILITY)" - the superseded predecessor of
        // PaintPaperTexturedButtons, which is what PaintCaption actually calls.
        //
        // Deleted: it is a PRIVATE method, so nothing outside this file could ever have depended
        // on it, and there is no back-compatibility burden on this library's internals. A second
        // implementation kept beside the live one is the thing the no-legacy-paths rule exists to
        // prevent - it compiles, it looks maintained, and it is never reached.


        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
             var path = owner.BorderShape; // Do NOT dispose - path is cached and owned by BeepiFormPro 
            // Material Paper: subtle elevated border (1px with alpha)
            using var pen = new Pen(Color.FromArgb(40, metrics.BorderColor), 1);
            FormPainterRenderHelper.ApplyFormChromeOutlinePenAlignment(pen);
            
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
            // A `g.Clip = new Region(path); g.Clip = originalClip;` pair stood here. The
            // second line discarded the first before anything drew under it, so the clip
            // never took effect - and the Region was never disposed, leaking a GDI handle
            // on every paint. 31 painters carried the identical pair.

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

        private static Color ShiftLuminance(Color color, float amount)
        {
            float h, s, l;
            ColorToHsl(color, out h, out s, out l);
            l = Math.Max(0, Math.Min(1, l + amount));
            return ColorFromHsl(h, s, l);
        }
        private static void ColorToHsl(Color c, out float h, out float s, out float l)
        {
            float r = c.R / 255f, g = c.G / 255f, b = c.B / 255f;
            float min = Math.Min(r, Math.Min(g, b)), max = Math.Max(r, Math.Max(g, b));
            l = (max + min) / 2f;
            if (max == min) { h = s = 0; }
            else
            {
                float d = max - min;
                s = l > 0.5f ? d / (2f - max - min) : d / (max + min);
                if (max == r) h = (g - b) / d + (g < b ? 6 : 0);
                else if (max == g) h = (b - r) / d + 2;
                else h = (r - g) / d + 4;
                h /= 6f;
            }
        }
        private static Color ColorFromHsl(float h, float s, float l)
        {
            float r, g, b;
            if (s == 0) { r = g = b = l; }
            else
            {
                float q = l < 0.5f ? l * (1f + s) : l + s - l * s;
                float p = 2f * l - q;
                r = Hue2Rgb(p, q, h + 1f / 3f);
                g = Hue2Rgb(p, q, h);
                b = Hue2Rgb(p, q, h - 1f / 3f);
            }
            return Color.FromArgb(255, (int)Math.Round(r * 255), (int)Math.Round(g * 255), (int)Math.Round(b * 255));
        }
        private static float Hue2Rgb(float p, float q, float t)
        {
            if (t < 0) t += 1f; if (t > 1) t -= 1f;
            if (t < 1f / 6f) return p + (q - p) * 6f * t;
            if (t < 1f / 2f) return q;
            if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
            return p;
        }

        public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
        {
            // Migrated to the shared cascade (stage 05). Its layout method was verified against the
            // shape established by six hand-migrated painters before this change: metrics-driven
            // caption height, the metrics.ButtonWidth cascade, metrics-based icon and title, and no
            // corner-safe or DPI work of its own. The drawing in this file is untouched.
            var metrics = GetMetrics(owner);
            int captionHeight = Math.Max(metrics.CaptionHeight, (int)(owner.Font.Height * metrics.FontHeightMultiplier));

            if (!FormPainterLayoutHelper.TryBuildStandardRightAlignedCaptionLayout(
                owner,
                captionHeight,
                metrics.ButtonWidth,
                metrics.SearchBoxWidth,
                metrics.SearchBoxPadding,
                includeCustomAction: true,
                out var layout,
                out int titleRightBoundary))
            {
                owner.CurrentLayout = layout;
                return;
            }

            owner._hits.Register("caption", layout.CaptionRect, HitAreaType.Drag);

            int iconY = (captionHeight - metrics.IconSize) / 2;
            layout.IconRect = new Rectangle(metrics.IconLeftPadding, iconY, metrics.IconSize, metrics.IconSize);
            if (owner.ShowIcon && owner.Icon != null)
            {
                owner._hits.Register("icon", layout.IconRect, HitAreaType.Icon);
            }

            int titleX = layout.IconRect.Right + metrics.TitleLeftPadding;
            layout.TitleRect = new Rectangle(titleX, 0,
                titleRightBoundary - titleX - metrics.ButtonSpacing, captionHeight);

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
