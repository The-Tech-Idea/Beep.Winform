using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Glass-effect form painter with advanced transparency, blur effects, and mica-style appearance.
    /// Features multi-layer transparency, subtle noise texture, and frosted glass caption bar.
    /// </summary>
    internal sealed class GlassFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Glass, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            // Paint only the base mica/glass color across the entire form.
            var baseLayer = Color.FromArgb(220, metrics.CaptionColor.R, metrics.CaptionColor.G, metrics.CaptionColor.B);
            using var baseBrush = new SolidBrush(baseLayer);
            g.FillRectangle(baseBrush, owner.ClientRectangle);
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);

            // Frosted glass effect for caption
            using var frostBrush = new SolidBrush(Color.FromArgb(180, 255, 255, 255));
            g.FillRectangle(frostBrush, captionRect);

            // Mica highlight at top
            using var highlightPen = new Pen(Color.FromArgb(120, 255, 255, 255), 1f);
            g.DrawLine(highlightPen, captionRect.Left, captionRect.Top, captionRect.Right, captionRect.Top);

            // Subtle inner shadow at bottom
            using var shadowPen = new Pen(Color.FromArgb(60, 0, 0, 0), 1f);
            g.DrawLine(shadowPen, captionRect.Left, captionRect.Bottom - 1, captionRect.Right, captionRect.Bottom - 1);

            // Title with glass-appropriate contrast
            var textRect = owner.CurrentLayout.TitleRect;
            var glassTextColor = metrics.CaptionTextColor; // High contrast for glass
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, glassTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // Built-in caption elements
            owner.PaintBuiltInCaptionElements(g);
        }

        /// <summary>
        /// Paints a subtle glass border around the form.
        /// </summary>
        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);

            // Create rounded rectangle path for border
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);

            // Draw subtle glass border
            using var borderPen = new Pen(Color.FromArgb(40, 255, 255, 255), 1);
            g.DrawPath(borderPen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            return new ShadowEffect
            {
                Color = Color.FromArgb(15, 0, 0, 0),
                Blur = 20,
                OffsetY = 6,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(16);
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.Ultra;
        }

        public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
        {
            var layout = new PainterLayoutInfo();
            
            // Calculate caption height based on font and padding
            var captionHeight = owner.Font.Height + 16; // 8px padding top and bottom
            
            // Set caption rectangle
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            
            // Set content rectangle (below caption)
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
            
            // Calculate button positions (right-aligned in caption)
            var buttonSize = new Size(32, captionHeight);
            var buttonY = 0;
            var buttonX = owner.ClientSize.Width - buttonSize.Width;
            
            // Close button
            layout.CloseButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
            owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
            buttonX -= buttonSize.Width;
            
            // Maximize button
            layout.MaximizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
            owner._hits.RegisterHitArea("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
            buttonX -= buttonSize.Width;
            
            // Minimize button
            layout.MinimizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
            owner._hits.RegisterHitArea("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
            buttonX -= buttonSize.Width;
            
            // Style button (if shown)
            if (owner.ShowStyleButton)
            {
                layout.StyleButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("style", layout.StyleButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Theme button (if shown)
            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Custom action button (if theme/style not shown)
            if (!owner.ShowThemeButton && !owner.ShowStyleButton)
            {
                layout.CustomActionButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
            }
            
            // Icon and title areas (left side of caption)
            var iconSize = 16;
            var iconPadding = 8;
            layout.IconRect = new Rectangle(iconPadding, (captionHeight - iconSize) / 2, iconSize, iconSize);
            owner._hits.RegisterHitArea("icon", layout.IconRect, HitAreaType.Icon);
            
            var titleX = iconPadding + iconSize + iconPadding;
            var titleWidth = buttonX - titleX - iconPadding;
            layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
            owner._hits.RegisterHitArea("title", layout.TitleRect, HitAreaType.Caption);
            
            owner.CurrentLayout = layout;
        }

        public bool SupportsAnimations => true;

        public void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            // Orchestrated painting: base background, clipped background effects, then borders and caption.
            var shadow = GetShadowEffect(owner);
            var radius = GetCornerRadius(owner);

            // Save original clip
            var originalClip = g.Clip;

            // 1) Shadow
            if (!shadow.Inner)
            {
                DrawShadow(g, rect, shadow, radius);
            }

            // 2) Base background across entire form
            PaintBackground(g, owner);

            // 3) Background effects with clipping (exclude content unless overridden)
            using var path = CreateRoundedRectanglePath(rect, radius);
            // Always paint over entire form background since this runs in OnPaintBackground
            g.Clip = new Region(path);

            PaintBackgroundEffects(g, owner, rect);

            // 4) Reset clip
            g.Clip = originalClip;

            // 5) Borders and caption without clipping
            PaintBorders(g, owner);
            if (owner.ShowCaptionBar)
            {
                PaintCaption(g, owner, owner.CurrentLayout.CaptionRect);
            }

            // Ensure clip is reset
            g.Clip = originalClip;
        }

        private void DrawShadow(Graphics g, Rectangle rect, ShadowEffect shadow, CornerRadius radius)
        {
            // Validate input rectangle
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Enhanced shadow for glass effect
            var shadowRect = new Rectangle(
                rect.X + shadow.OffsetX - shadow.Blur,
                rect.Y + shadow.OffsetY - shadow.Blur,
                rect.Width + shadow.Blur * 2,
                rect.Height + shadow.Blur * 2);

            // Ensure shadow rect has valid dimensions
            if (shadowRect.Width <= 0 || shadowRect.Height <= 0) return;

            using var shadowBrush = new SolidBrush(shadow.Color);
            using var shadowPath = CreateRoundedRectanglePath(shadowRect, radius);
            g.FillPath(shadowBrush, shadowPath);
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, CornerRadius radius)
        {
            var path = new GraphicsPath();

            // Validate rectangle dimensions
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(new Rectangle(rect.X, rect.Y, Math.Max(1, rect.Width), Math.Max(1, rect.Height)));
                return path;
            }

            // Clamp radius values to prevent oversized corners
            int maxRadius = Math.Min(rect.Width, rect.Height) / 2;
            int topLeft = Math.Max(0, Math.Min(radius.TopLeft, maxRadius));
            int topRight = Math.Max(0, Math.Min(radius.TopRight, maxRadius));
            int bottomRight = Math.Max(0, Math.Min(radius.BottomRight, maxRadius));
            int bottomLeft = Math.Max(0, Math.Min(radius.BottomLeft, maxRadius));

            // If all radii are zero, just add a rectangle
            if (topLeft == 0 && topRight == 0 && bottomRight == 0 && bottomLeft == 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            // Top-left corner
            if (topLeft > 0)
                path.AddArc(rect.X, rect.Y, topLeft * 2, topLeft * 2, 180, 90);
            else
                path.AddLine(rect.X, rect.Y, rect.X, rect.Y);

            // Top-right corner
            if (topRight > 0)
                path.AddArc(rect.Right - topRight * 2, rect.Y, topRight * 2, topRight * 2, 270, 90);
            else
                path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);

            // Bottom-right corner
            if (bottomRight > 0)
                path.AddArc(rect.Right - bottomRight * 2, rect.Bottom - bottomRight * 2, bottomRight * 2, bottomRight * 2, 0, 90);
            else
                path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);

            // Bottom-left corner
            if (bottomLeft > 0)
                path.AddArc(rect.X, rect.Bottom - bottomLeft * 2, bottomLeft * 2, bottomLeft * 2, 90, 90);
            else
                path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom);

            path.CloseFigure();
            return path;
        }

        // Painter-owned non-client border rendering for Glass style
        public void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            var outer = new Rectangle(0, 0, owner.Width, owner.Height);
            using var outerPath = CreateRoundedRectanglePath(outer, radius);
            var inner = Rectangle.Inflate(outer, -borderThickness, -borderThickness);
            var innerRadius = new CornerRadius(
                Math.Max(0, radius.TopLeft - borderThickness),
                Math.Max(0, radius.TopRight - borderThickness),
                Math.Max(0, radius.BottomLeft - borderThickness),
                Math.Max(0, radius.BottomRight - borderThickness)
            );
            using var innerPath = (inner.Width > 0 && inner.Height > 0) ? CreateRoundedRectanglePath(inner, innerRadius) : null;
            using var ring = new Region(outerPath);
            if (innerPath != null) ring.Exclude(innerPath); else ring.Exclude(inner);
            using var br = new SolidBrush(Color.FromArgb(60, 255, 255, 255));
            g.FillRegion(br, ring);

            // Bright top edge line for glass sheen
            using var hi = new Pen(Color.FromArgb(140, 255, 255, 255), 1f) { Alignment = PenAlignment.Inset };
            int y = Math.Max(0, borderThickness - 1);
            g.DrawLine(hi, 0, y, owner.Width - 1, y);
        }

        /// <summary>
        /// Adds subtle noise texture for frosted glass effect
        /// </summary>
        private void AddNoiseTexture(Graphics g, Rectangle rect, float intensity)
        {
            var random = new Random(42); // Fixed seed for consistent noise
            using var noiseBrush = new SolidBrush(Color.FromArgb((int)(intensity * 255), 255, 255, 255));
            
            // Draw random noise dots for frosted effect
            for (int i = 0; i < rect.Width * rect.Height * intensity * 0.1f; i++)
            {
                int x = rect.Left + random.Next(rect.Width);
                int y = rect.Top + random.Next(rect.Height);
                g.FillRectangle(noiseBrush, x, y, 1, 1);
            }
        }

        // Glass background effects (mica gradient + noise) to be applied under controlled clipping
        private void PaintBackgroundEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            using var micaBrush = new LinearGradientBrush(
                owner.ClientRectangle,
                Color.FromArgb(25, 255, 255, 255),  // Bright top
                Color.FromArgb(8, 0, 0, 0),          // Subtle dark bottom
                LinearGradientMode.Vertical);
            g.FillRectangle(micaBrush, owner.ClientRectangle);

            AddNoiseTexture(g, owner.ClientRectangle, 0.02f);
        }
    }
}
