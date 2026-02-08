using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// 80s/90s retro computing nostalgic aesthetic.
    /// 
    /// Features:
    /// - CRT scan line effect (horizontal lines every 3 pixels)
    /// - Dithered pattern overlay (50% hatch pattern)
    /// - Win95-style 3D beveled borders
    /// - Pixel-perfect rendering (no anti-aliasing)
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class RetroFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Retro, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            
            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            // This prevents semi-transparent overlays from accumulating on repaint
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            
            // Retro: Flat background with optional dithered pattern
            g.SmoothingMode = SmoothingMode.None; // Pixel-perfect
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
            
            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;
            
            // CRT scan lines effect (every 3 pixels for retro monitor aesthetic)
            using (var scanLinePen = new Pen(Color.FromArgb(10, 0, 0, 0), 1))
            {
                for (int y = 0; y < owner.ClientRectangle.Height; y += 3)
                {
                    g.DrawLine(scanLinePen, 0, y, owner.ClientRectangle.Width, y);
                }
            }
            
            // Dithered pattern for retro texture (classic 50% hatch pattern)
            using (var hatchBrush = new HatchBrush(HatchStyle.Percent50, 
                Color.FromArgb(8, 0, 0, 0), 
                Color.Transparent))
            {
                g.FillRectangle(hatchBrush, owner.ClientRectangle);
            }
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            
            // Retro: Solid caption with 3D bevel effect (Win95 Style)
            g.SmoothingMode = SmoothingMode.None;
            using var capBrush = new SolidBrush(metrics.CaptionColor);
            g.FillRectangle(capBrush, captionRect);
            
            // 3D Bevel: Light edge on top/left, dark edge on bottom/right
            var lightEdge = ControlPaint.Light(metrics.CaptionColor);
            var darkEdge = ControlPaint.Dark(metrics.CaptionColor);
            
            // Top highlight
            using (var lightPen = new Pen(lightEdge, 2))
            {
                g.DrawLine(lightPen, captionRect.Left, captionRect.Top, captionRect.Right, captionRect.Top);
                g.DrawLine(lightPen, captionRect.Left, captionRect.Top, captionRect.Left, captionRect.Bottom);
            }
            
            // Bottom/Right shadow
            using (var darkPen = new Pen(darkEdge, 2))
            {
                g.DrawLine(darkPen, captionRect.Left, captionRect.Bottom - 1, captionRect.Right, captionRect.Bottom - 1);
                g.DrawLine(darkPen, captionRect.Right - 1, captionRect.Top, captionRect.Right - 1, captionRect.Bottom);
            }

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // Paint Retro 3D buttons (UNIQUE SKIN)
            PaintRetroButtons(g, owner, captionRect, metrics);
            
            // Only paint the icon (if not handled by custom buttons, but we need to ensure icon is drawn)
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }

        /// <summary>
        /// Paint Retro 3D buttons (Win95 Style)
        /// Features: 3D bevel borders, pixel fonts, classic gray
        /// </summary>
        private void PaintRetroButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            // Standard Windows 95 button size logic
            int buttonSize = Math.Min(captionRect.Height - 4, 16); 
            int padding = (captionRect.Height - buttonSize) / 2;
            
            // Check hover states
            bool closeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("close")) ?? false;
            bool maxHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("maximize")) ?? false;
            bool minHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("minimize")) ?? false;
            bool themeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("theme")) ?? false;
            bool styleHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("Style")) ?? false;
            
            // Retro gray base (Classic Win95 gray: 192, 192, 192)
            Color retroGray = Color.FromArgb(192, 192, 192);

            // Close button: X
            PaintRetroButton(g, closeRect, retroGray, "close", closeHovered);
            
            // Maximize button: Square
            PaintRetroButton(g, maxRect, retroGray, "maximize", maxHovered);
            
            // Minimize button: Underline
            PaintRetroButton(g, minRect, retroGray, "minimize", minHovered);
            
            // Theme/Style buttons if shown
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintRetroButton(g, styleRect, retroGray, "Style", styleHovered);
            }
            
            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintRetroButton(g, themeRect, retroGray, "theme", themeHovered);
            }
        }
        
        private void PaintRetroButton(Graphics g, Rectangle rect, Color baseColor, string buttonType, bool isHovered)
        {
            // Adjust rect for pixel crispness
            // In Retro mode, we want pixel perfection, so use the rect as is but ensure it's small enough to look like a button
            // If rect is too big, shrink it to look like a standard Win95 caption button
            int size = Math.Min(rect.Width, rect.Height);
             // Center it
            int x = rect.X + (rect.Width - size) / 2;
            int y = rect.Y + (rect.Height - size) / 2;
            var btnRect = new Rectangle(x, y, size, size);
            
            // Background
            using (var brush = new SolidBrush(baseColor))
            {
                g.FillRectangle(brush, btnRect);
            }
            
            // 3D Bevel Effect
            // Top/Left: Light (White)
            // Bottom/Right: Dark (Black/DarkGray)
            
            Color light = Color.White;
            Color shadow = Color.Black;
            Color darkShadow = Color.Gray;
            
            if (isHovered)
            {
                // Hover effect: Simulate "pushed" or just "highlighted"
                 using (var brush = new SolidBrush(Color.FromArgb(220, 220, 220)))
                {
                    g.FillRectangle(brush, btnRect);
                }
            }
            
            // Draw 3D Bevel (Raised)
             using (var pen = new Pen(light, 1))
            {
                g.DrawLine(pen, btnRect.Left, btnRect.Top, btnRect.Right - 1, btnRect.Top);
                g.DrawLine(pen, btnRect.Left, btnRect.Top, btnRect.Left, btnRect.Bottom - 1);
            }
            
            using (var pen = new Pen(shadow, 1))
            {
                g.DrawLine(pen, btnRect.Left, btnRect.Bottom - 1, btnRect.Right - 1, btnRect.Bottom - 1);
                g.DrawLine(pen, btnRect.Right - 1, btnRect.Top, btnRect.Right - 1, btnRect.Bottom - 1);
            }
            
            using (var pen = new Pen(darkShadow, 1))
            {
                g.DrawLine(pen, btnRect.Left + 1, btnRect.Bottom - 2, btnRect.Right - 2, btnRect.Bottom - 2);
                g.DrawLine(pen, btnRect.Right - 2, btnRect.Top + 1, btnRect.Right - 2, btnRect.Bottom - 2);
            }
            
            // Icon (Black pixel art)
            using (var iconPen = new Pen(Color.Black, 1)) // 1px black
            {
                
                 // Standard size for pixel art icons
                int iconSize = 7;
                int cx = btnRect.X + btnRect.Width / 2;
                int cy = btnRect.Y + btnRect.Height / 2;
                
                switch (buttonType)
                {
                    case "close":
                        // X
                        g.DrawLine(iconPen, cx - 3, cy - 3, cx + 2, cy + 2);
                        g.DrawLine(iconPen, cx - 3, cy - 2, cx + 1, cy + 2); // Thicken
                        g.DrawLine(iconPen, cx + 2, cy - 3, cx - 3, cy + 2);
                        g.DrawLine(iconPen, cx + 1, cy - 3, cx - 3, cy + 1); // Thicken
                        break;
                    case "maximize":
                        // Square
                        g.DrawRectangle(iconPen, cx - 4, cy - 4, 7, 7);
                        g.DrawLine(iconPen, cx - 4, cy - 5, cx + 3, cy - 5); // Thicken top
                        break;
                    case "minimize":
                        // Line
                        g.DrawLine(iconPen, cx - 3, cy + 2, cx + 2, cy + 2);
                        g.DrawLine(iconPen, cx - 3, cy + 3, cx + 2, cy + 3); // Thicken
                        break;
                    case "Style":
                        // ?
                        g.DrawLine(iconPen, cx, cy - 3, cx, cy + 3);
                        g.DrawLine(iconPen, cx - 2, cy, cx + 2, cy);
                        break;
                    case "theme":
                         // T
                        g.DrawLine(iconPen, cx - 3, cy - 3, cx + 3, cy - 3);
                        g.DrawLine(iconPen, cx, cy - 3, cx, cy + 3);
                        break;
                }
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);

            // Retro: Multi-line 3D border (inset/outset Style)
            g.SmoothingMode = SmoothingMode.None;

            var rect = owner.BorderShape.GetBounds();

            var lightColor = ControlPaint.Light(metrics.BorderColor);
            var darkColor = ControlPaint.Dark(metrics.BorderColor);
            
            // Outer border - raised effect
            // Light on top/left
            using (var lightPen = new Pen(lightColor, 2))
            {
                g.DrawLine(lightPen, rect.Left, rect.Top, rect.Right - 1, rect.Top);
                g.DrawLine(lightPen, rect.Left, rect.Top, rect.Left, rect.Bottom - 1);
            }
            
            // Dark on bottom/right
            using (var darkPen = new Pen(darkColor, 2))
            {
                g.DrawLine(darkPen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
                g.DrawLine(darkPen, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom);
            }
            
            // Inner border line (classic Win95 double border)
            var innerRect = new RectangleF(rect.X + 3, rect.Y + 3, rect.Width - 6, rect.Height - 6);
            using (var borderPen = new Pen(metrics.BorderColor, 1))
            {
                g.DrawRectangle(borderPen, innerRect);
            }
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Retro: Minimal shadow with slight blur
            return new ShadowEffect
            {
                Color = Color.FromArgb(40, 0, 0, 0),
                Blur = 3, // Small blur for retro look
                OffsetX = 4,
                OffsetY = 4,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(0); // Boxy, no rounding
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.None;
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
            
            // NOTE: _hits.Clear() is handled by EnsureLayoutCalculated - do not call here
            
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
