using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// macOS-Style form painter with traffic light buttons and subtle borders (synced with MacOSTheme).
    /// 
    /// macOS Color Palette (synced with MacOSTheme):
    /// - Background: #FFFFFF (255, 255, 255) - Pure white base
    /// - Foreground: #000000 (0, 0, 0) - Black text
    /// - Border: #D1D1D6 (209, 209, 214) - Light gray border
    /// - Hover: #F2F2F7 (242, 242, 247) - Very light gray hover
    /// - Selected: #007AFF (0, 122, 255) - macOS blue selected
    /// 
    /// Features:
    /// - Gentle inner highlights/shading (top highlight + bottom shade)
    /// - Traffic light buttons (red, yellow, green)
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class MacOSFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultForCached(FormStyle.MacOS, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            
            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            // This prevents semi-transparent overlays from accumulating on repaint
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            
            // Base macOS background only; effects handled under clipping in PaintWithEffects
            using (var baseBrush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(baseBrush, owner.ClientRectangle);
            }

            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;

            // Gentle macOS-like inner highlights/shading (using helper)
            var r = owner.ClientRectangle;
            // Top highlight
            FormPainterRenderHelper.PaintTopHighlight(g, r, Math.Max(1, r.Height / 4), 40);
            // Bottom shade
            var bottomBand = new Rectangle(r.Left, r.Bottom - Math.Max(1, r.Height / 6), r.Width, Math.Max(1, r.Height / 6));
            FormPainterRenderHelper.PaintGradientBackground(g, bottomBand,
                Color.FromArgb(0, 0, 0, 0),
                Color.FromArgb(16, 0, 0, 0),
                LinearGradientMode.Vertical);
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);

            // macOS title bar with subtle translucency (using helper)
            FormPainterRenderHelper.PaintGradientBackground(g, captionRect,
                Color.FromArgb(240, 240, 240), // Light gray
                Color.FromArgb(220, 220, 220), // Slightly darker
                LinearGradientMode.Vertical);

            // Add subtle inner shadow for depth (using helper)
            FormPainterRenderHelper.PaintGradientBackground(g, captionRect,
                Color.FromArgb(60, 0, 0, 0),   // Top shadow
                Color.FromArgb(0, 0, 0, 0),     // Fade to transparent
                LinearGradientMode.Vertical);

            // Draw refined traffic light buttons with 3D effects
            // Reads positions directly from CurrentLayout to stay in sync with hit areas
            DrawTrafficLights(g, owner);

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            // Built-in caption elements (icon/theme/Style/custom buttons) - drawn before title text
            // so icon is painted first and title text sits next to it, not under it
            owner.PaintBuiltInCaptionElements(g);

            // Draw title text with macOS typography - after icon so rendering order is correct
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect,
                metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            var path = owner.BorderShape; // Do NOT dispose - path is cached and owned by BeepiFormPro
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, metrics.BorderWidth));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            return new ShadowEffect
            {
                Color = Color.FromArgb(20, 0, 0, 0),
                Blur = 18,
                OffsetY = 10,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(10);
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.High;
        }

        public bool SupportsAnimations => false;

        public void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            // Orchestrated: shadow, base background, clipped background effects, borders, caption
            var originalClip = g.Clip;
            var shadow = GetShadowEffect(owner);
            var radius = GetCornerRadius(owner);

            // 1) Shadow
            if (!shadow.Inner)
            {
                DrawShadow(g, rect, shadow, radius);
            }

            // 2) Base background
            PaintBackground(g, owner);

            // 3) Background effects with clipping (subtle overlay)
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            // Always paint over entire form background since this runs in OnPaintBackground
            g.Clip = new Region(path);

            PaintBackgroundEffects(g, owner, rect);

            // 4) Reset clip
            g.Clip = originalClip;

            // 5) Borders and caption
            PaintBorders(g, owner);
            if (owner.ShowCaptionBar)
            {
                PaintCaption(g, owner, owner.CurrentLayout.CaptionRect);
            }

            g.Clip = originalClip;
        }

        private void PaintBackgroundEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            // Subtle translucent overlay (using helper)
            FormPainterRenderHelper.PaintSolidBackground(g, owner.ClientRectangle, Color.FromArgb(15, 0, 0, 0));

            // Mild gradient for depth (using helper)
            FormPainterRenderHelper.PaintGradientBackground(g, owner.ClientRectangle,
                Color.FromArgb(10, 255, 255, 255),
                Color.FromArgb(0, 0, 0, 0),
                LinearGradientMode.Vertical);
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

        // Painter-owned non-client border rendering for macOS Style
        public void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            var rect = new Rectangle(0, 0, owner.Width, owner.Height);
            using var path = CreateRoundedRectanglePath(rect, radius);
            using var pen = new Pen(Color.FromArgb(100, metrics.BorderColor), Math.Max(1, borderThickness))
            {
                Alignment = PenAlignment.Inset
            };
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);

            // Thin bright top edge typical of macOS
            using var hi = new Pen(Color.FromArgb(110, 255, 255, 255), 1f) { Alignment = PenAlignment.Inset };
            int y = Math.Max(0, borderThickness - 1);
            g.DrawLine(hi, 0, y, owner.Width - 1, y);
        }

        /// <summary>
        /// Draws macOS-style traffic light buttons with 3D effects.
        /// Uses positions from CurrentLayout to stay consistent with registered hit areas.
        /// </summary>
        private void DrawTrafficLights(Graphics g, BeepiFormPro owner)
        {
            var layout = owner.CurrentLayout;

            // Traffic light colors
            var red    = Color.FromArgb(255, 95, 87);
            var yellow = Color.FromArgb(255, 189, 46);
            var green  = Color.FromArgb(39, 201, 63);

            bool closeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("close"))    ?? false;
            bool minHovered   = owner._interact?.IsHovered(owner._hits?.GetHitArea("minimize")) ?? false;
            bool maxHovered   = owner._interact?.IsHovered(owner._hits?.GetHitArea("maximize")) ?? false;

            // Draw each traffic light centered within its registered hit-area rect
            DrawTrafficLightInRect(g, layout.CloseButtonRect,    red,    closeHovered, "close");
            DrawTrafficLightInRect(g, layout.MinimizeButtonRect, yellow, minHovered,   "minimize");
            DrawTrafficLightInRect(g, layout.MaximizeButtonRect, green,  maxHovered,   "maximize");
        }

        /// <summary>
        /// Draws a single traffic light circle centered within the given hit-area rectangle.
        /// </summary>
        private void DrawTrafficLightInRect(Graphics g, Rectangle hitRect, Color color, bool isHovered, string buttonType)
        {
            if (hitRect.IsEmpty) return;

            // Use the registered button size; center the circle inside the hit rect
            int size = Math.Min(hitRect.Width, hitRect.Height);
            int x = hitRect.X + (hitRect.Width  - size) / 2;
            int y = hitRect.Y + (hitRect.Height - size) / 2;

            DrawTrafficLightButton(g, x, y, size, color, isHovered, buttonType);
        }

        /// <summary>
        /// Draws a single traffic light button with highlight and shadow
        /// </summary>
        private void DrawTrafficLightButton(Graphics g, int x, int y, int size, Color color, bool isHovered, string buttonType)
        {
            // Main button - brighter on hover
            Color fillColor = isHovered ? ControlPaint.Light(color, 0.15f) : color;
            using var buttonBrush = new SolidBrush(fillColor);
            g.FillEllipse(buttonBrush, x, y, size, size);

            // Highlight (top-left) - larger on hover
            int highlightAlpha = isHovered ? 140 : 100;
            float highlightSize = isHovered ? 0.7f : 0.6f;
            using var highlightBrush = new SolidBrush(Color.FromArgb(highlightAlpha, 255, 255, 255));
            g.FillEllipse(highlightBrush, x, y, size * highlightSize, size * highlightSize);

            // Shadow (bottom-right)
            using var shadowBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0));
            g.FillEllipse(shadowBrush, x + size * 0.4f, y + size * 0.4f, size * 0.6f, size * 0.6f);

            // Border - thicker on hover
            float borderWidth = isHovered ? 1f : 0.5f;
            using var borderPen = new Pen(Color.FromArgb(60, 0, 0, 0), borderWidth);
            g.DrawEllipse(borderPen, x, y, size, size);

            // Symbol on hover
            if (isHovered)
            {
                using (var symbolPen = new Pen(Color.FromArgb(180, 0, 0, 0), 1.5f))
                {
                    int cx = x + size / 2;
                    int cy = y + size / 2;
                    int symSize = 6;
                    
                    switch (buttonType)
                    {
                        case "close":
                            // X icon
                            g.DrawLine(symbolPen, cx - 2, cy - 2, cx + 2, cy + 2);
                            g.DrawLine(symbolPen, cx + 2, cy - 2, cx - 2, cy + 2);
                            break;
                            
                        case "minimize":
                            // Minus icon
                            g.DrawLine(symbolPen, cx - 3, cy, cx + 3, cy);
                            break;
                            
                        case "maximize":
                            // Plus/Zoom icon - usually two arrows or a plus in old macOS, simply + or arrows in new
                            // Modern macOS shows a + for fullscreen/maximize behavior or arrows
                            // Let's use arrows (diagonal) style or simple +
                            // Simple + is easier to read at small size
                            g.DrawLine(symbolPen, cx - 3, cy, cx + 3, cy); // Horizontal
                            g.DrawLine(symbolPen, cx, cy - 3, cx, cy + 3); // Vertical
                            break;
                    }
                }
            }
        }

        public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
        {
            var layout = new PainterLayoutInfo();
            
            // NOTE: _hits.Clear() is handled by EnsureLayoutCalculated - do not call here
            
            // If caption bar is hidden, skip button layout
            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                owner.CurrentLayout = layout;
                return;
            }
            
            // Calculate caption height based on font and padding (macOS uses standard padding)
            var captionHeight = owner.Font.Height + 16; // 8px padding top and bottom
            
            // Set caption rectangle
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            owner._hits.Register("caption", layout.CaptionRect, HitAreaType.Drag);
            
            // macOS: Traffic light buttons positioned on the LEFT
            var buttonSize   = 12; // macOS traffic lights are small circles
            var buttonHalf   = buttonSize / 2;
            var buttonY      = (captionHeight - buttonSize) / 2;
            var buttonSpacing = 8;

            // Compute safe left start to avoid the rounded corner arc (radius=10 for macOS)
            var cornerRadius = GetCornerRadius(owner);
            int safeLeftX = FormPainterMetrics.GetCaptionLeftSafeX(
                cornerRadius.TopLeft, buttonY + buttonHalf, buttonHalf) + 2; // +2 visual gap
            safeLeftX = Math.Max(safeLeftX, 12); // Minimum 12px from edge

            var leftX = safeLeftX;
            
            // Close button (red, leftmost)
            if (owner.ShowCloseButton)
            {
                layout.CloseButtonRect = new Rectangle(leftX, buttonY, buttonSize, buttonSize);
                owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
                leftX += buttonSize + buttonSpacing;
            }
            
            // Minimize/Maximize buttons (yellow/green, middle/right of traffic lights)
            if (owner.ShowMinMaxButtons)
            {
                layout.MinimizeButtonRect = new Rectangle(leftX, buttonY, buttonSize, buttonSize);
                owner._hits.RegisterHitArea("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
                leftX += buttonSize + buttonSpacing;
                
                layout.MaximizeButtonRect = new Rectangle(leftX, buttonY, buttonSize, buttonSize);
                owner._hits.RegisterHitArea("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
                leftX += buttonSize + buttonSpacing;
            }
            
            // RIGHT side: Theme/Style buttons (standard Windows-style placement)
            // Also keep right-side elements away from the top-right corner arc
            int safeRightX = FormPainterMetrics.GetCaptionLeftSafeX(
                cornerRadius.TopRight, captionHeight / 2, captionHeight / 2) + 2;
            var rightButtonWidth = 32;
            var rightX = owner.ClientSize.Width - rightButtonWidth - safeRightX;
            
            // Style button (if shown)
            if (owner.ShowStyleButton)
            {
                layout.StyleButtonRect = new Rectangle(rightX, 0, rightButtonWidth, captionHeight);
                owner._hits.RegisterHitArea("Style", layout.StyleButtonRect, HitAreaType.Button);
                rightX -= rightButtonWidth;
            }
            
            // Theme button (if shown)
            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(rightX, 0, rightButtonWidth, captionHeight);
                owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
                rightX -= rightButtonWidth;
            }
            
            // Custom action button (only if ShowCustomActionButton is true)
            if (owner.ShowCustomActionButton)
            {
                layout.CustomActionButtonRect = new Rectangle(rightX, 0, rightButtonWidth, captionHeight);
                owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
                rightX -= rightButtonWidth;
            }
            
            // Search box (between title and right-side buttons)
            int searchBoxWidth   = 200;
            int searchBoxPadding = 8;
            if (owner.ShowSearchBox)
            {
                layout.SearchBoxRect = new Rectangle(rightX - searchBoxWidth - searchBoxPadding, searchBoxPadding / 2, 
                    searchBoxWidth, captionHeight - searchBoxPadding);
                owner._hits.RegisterHitArea("search", layout.SearchBoxRect, HitAreaType.TextBox);
                rightX -= searchBoxWidth + searchBoxPadding;
            }
            else
            {
                layout.SearchBoxRect = Rectangle.Empty;
            }
            
            // Icon and title areas: start after traffic lights, end before right-side buttons
            var iconSize    = 16;
            var iconPadding = 8;
            var iconStartX  = leftX + 4; // Small gap after last traffic light

            layout.IconRect = new Rectangle(iconStartX, (captionHeight - iconSize) / 2, iconSize, iconSize);
            if (owner.ShowIcon && owner.Icon != null)
            {
                owner._hits.RegisterHitArea("icon", layout.IconRect, HitAreaType.Icon);
            }

            var titleX     = layout.IconRect.Right + iconPadding;
            var titleWidth = Math.Max(0, rightX - titleX - 8);
            layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
            owner._hits.RegisterHitArea("title", layout.TitleRect, HitAreaType.Caption);
            
            // Content rectangle (below caption), with corner-safe insets for the bottom corners
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);

            // Expose safe insets so child controls can avoid being clipped by rounded corners
            int bottomCornerRadius = Math.Max(cornerRadius.BottomLeft, cornerRadius.BottomRight);
            layout.SafeContentInsets = new System.Windows.Forms.Padding(
                left:   bottomCornerRadius > 0 ? FormPainterMetrics.GetCaptionLeftSafeX(cornerRadius.BottomLeft, layout.ContentRect.Height, layout.ContentRect.Height / 2) : 0,
                top:    0,
                right:  bottomCornerRadius > 0 ? FormPainterMetrics.GetCaptionLeftSafeX(cornerRadius.BottomRight, layout.ContentRect.Height, layout.ContentRect.Height / 2) : 0,
                bottom: bottomCornerRadius
            );
            
            owner.CurrentLayout = layout;
        }
    }
}
