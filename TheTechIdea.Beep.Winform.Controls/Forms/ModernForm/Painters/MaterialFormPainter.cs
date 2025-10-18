using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Material Design 3 form painter with vertical accent bar and elevation effects.
    /// Features a prominent vertical accent bar, elevation shadows, and Material 3 color tokens.
    /// </summary>
    internal sealed class MaterialFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Material, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        /// <summary>
        /// Paints a Material Design 3 surface with elevation tint.
        /// </summary>
        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            // Base surface color
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }

            // Subtle elevation tint from top (Material surface depth)
            using (var elevation = new System.Drawing.Drawing2D.LinearGradientBrush(
                owner.ClientRectangle,
                Color.FromArgb(14, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0),
                System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                g.FillRectangle(elevation, owner.ClientRectangle);
            }
        }

        /// <summary>
        /// Paints the caption bar with Material Design 3 style vertical accent bar and state layer.
        /// Features a 6px primary color bar on the left edge with hover state indication.
        /// </summary>
        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            var primary = metrics.BorderColor;

            // Draw vertical accent bar on the left (Material 3 style)
            using var brush = new SolidBrush(primary);
            int accentBarWidth = Math.Max(0, metrics.AccentBarWidth);
            var bar = new Rectangle(captionRect.Left, captionRect.Top, accentBarWidth, captionRect.Height);
            g.FillRectangle(brush, bar);

            // Add subtle state layer effect
            using var stateBrush = new SolidBrush(Color.FromArgb(8, primary));
            var stateRect = new Rectangle(captionRect.Left + accentBarWidth, captionRect.Top, captionRect.Width - accentBarWidth, captionRect.Height);
            g.FillRectangle(stateBrush, stateRect);

            // Draw title text with Material 3 typography spacing (16px padding)
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                // Built-in caption elements
                owner.PaintBuiltInCaptionElements(g);
        }

        /// <summary>
        /// Paints a 1px border around the form using Material3 border color.
        /// </summary>
        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = owner.BorderShape;
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, metrics.BorderWidth));
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            return new ShadowEffect
            {
                Color = Color.FromArgb(25, 0, 0, 0),
                Blur = 12,
                OffsetY = 4,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(12);
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.Ultra;
        }

        public bool SupportsAnimations => true;

        public void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            // Orchestrated: background, clipped background effects, then borders and caption.
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
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            // Always paint over entire form background since this runs in OnPaintBackground
            g.Clip = new Region(path);

            PaintBackgroundEffects(g, owner, rect, path);

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

            // Simple shadow implementation - expand rectangle and draw with alpha
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

        private System.Drawing.Drawing2D.GraphicsPath CreateRoundedRectanglePath(Rectangle rect, CornerRadius radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();

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

        // Material background effects (subtle elevation gradient) under controlled clipping
        private void PaintBackgroundEffects(Graphics g, BeepiFormPro owner, Rectangle rect, System.Drawing.Drawing2D.GraphicsPath path)
        {
            using var elevationBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                rect,
                Color.FromArgb(10, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0),
                System.Drawing.Drawing2D.LinearGradientMode.Vertical);
            g.FillPath(elevationBrush, path);
        }

        public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
        {
            var layout = new PainterLayoutInfo();
            FormPainterMetrics metrics = FormPainterMetrics.DefaultFor(FormStyle.Material, owner.UseThemeColors ? owner.CurrentTheme : null);
            
            owner._hits.Clear();
            
            // If caption bar is hidden, skip button layout
            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                owner.CurrentLayout = layout;
                return;
            }
            
            // Calculate caption height based on font and padding (Material Design 3 uses more padding)
            var captionHeight = owner.Font.Height + 32; // 16px padding top and bottom for Material 3
            
            // Set caption rectangle
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            
            // Set content rectangle (below caption)
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
            
            // Calculate button positions (right-aligned in caption, accounting for accent bar)
            var buttonSize = new Size(32, captionHeight);
            var buttonY = 0;
            var buttonX = owner.ClientSize.Width - buttonSize.Width;
            
            // Close button
            if (owner.ShowCloseButton)
            {
                layout.CloseButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Maximize/Minimize buttons
            if (owner.ShowMinMaxButtons)
            {
                layout.MaximizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
                
                layout.MinimizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
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
            
            // Icon and title areas (left side of caption, after accent bar)
            var iconSize = metrics.IconSize;
            var iconPadding = metrics.IconLeftPadding; // More padding for Material 3
            int accentBarWidth = Math.Max(0, metrics.AccentBarWidth);

            layout.IconRect = new Rectangle(accentBarWidth + iconPadding, (captionHeight - iconSize) / 2, iconSize, iconSize);
            owner._hits.RegisterHitArea("icon", layout.IconRect, HitAreaType.Icon);
            
            var titleX = accentBarWidth + iconPadding + iconSize + iconPadding;
            var titleWidth = buttonX - titleX - iconPadding;
            layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
            owner._hits.RegisterHitArea("title", layout.TitleRect, HitAreaType.Caption);
            
            owner.CurrentLayout = layout;
        }

        // Painter-owned non-client border rendering for Material style
        public void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);

            // Build outer and inner shapes (rounded) and fill the ring region
            var outerRect = new Rectangle(0, 0, owner.Width, owner.Height);
            using var outerPath = CreateRoundedRectanglePath(outerRect, radius);

            // Compute inner rect by insetting by borderThickness on all sides
            var innerRect = Rectangle.Inflate(outerRect, -borderThickness, -borderThickness);
            // Derive inner radius by subtracting thickness (clamped to >= 0)
            var innerRadius = new CornerRadius(
                Math.Max(0, radius.TopLeft - borderThickness),
                Math.Max(0, radius.TopRight - borderThickness),
                Math.Max(0, radius.BottomLeft - borderThickness),
                Math.Max(0, radius.BottomRight - borderThickness)
            );

            using var innerPath = (innerRect.Width > 0 && innerRect.Height > 0)
                ? CreateRoundedRectanglePath(innerRect, innerRadius)
                : null;

            using var ringRegion = new Region(outerPath);
            if (innerPath != null)
            {
                ringRegion.Exclude(innerPath);
            }
            else
            {
                ringRegion.Exclude(innerRect);
            }

            using var br = new SolidBrush(metrics.BorderColor);
            g.FillRegion(br, ringRegion);

            // Special top-edge treatment: integrate the Material accent bar to the very edge
            // Ensure the top border continues the accent bar color and add a subtle inner highlight across the rest of top edge
            int accentBarWidth = Math.Max(0, metrics.AccentBarWidth); // must match PaintCaption
            if (owner.ShowCaptionBar && owner.FormStyle == FormStyle.Material)
            {
                // Overpaint the top-left band with primary (accent continuation)
                using var primary = new SolidBrush(metrics.BorderColor);
                g.FillRectangle(primary, new Rectangle(0, 0, Math.Min(accentBarWidth, owner.Width), borderThickness));

                // Add a subtle inner highlight along the top edge excluding the accent segment
                using var hiPen = new Pen(Color.FromArgb(40, 255, 255, 255), 1f)
                {
                    Alignment = System.Drawing.Drawing2D.PenAlignment.Inset
                };
                int highlightY = Math.Max(0, borderThickness - 1);
                int startX = Math.Min(accentBarWidth, owner.Width);
                if (owner.Width - startX > 1)
                {
                    g.DrawLine(hiPen, startX, highlightY, owner.Width - 1, highlightY);
                }
            }
        }
    }
}
