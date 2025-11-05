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
            return FormPainterMetrics.DefaultFor(FormStyle.Material, owner);
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
        /// Paints the caption bar with Material Design 3 Style vertical accent bar and state layer.
        /// Features a 6px primary color bar on the left edge with hover state indication.
        /// </summary>
        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            var primary = metrics.BorderColor;

            // Draw vertical accent bar on the left (Material 3 Style)
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

            // Paint Material 3 buttons (custom implementation)
            PaintMaterial3Buttons(g, owner, captionRect, metrics);
            
            // Paint icon only (not system buttons - we handle those custom)
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }
        
        /// <summary>
        /// Paint Material Design 3 buttons with proper state layers and icons
        /// Following Material 3 guidelines: 40dp touch targets, 24dp icons, rounded ends
        /// </summary>
        private void PaintMaterial3Buttons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            // Material 3: 40dp (40px) touch targets with 24dp (24px) icon areas
            int touchTarget = 40;
            int iconArea = 24;
            int padding = (captionRect.Height - touchTarget) / 2;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Close button: Error color with state layer
            PaintMaterial3Button(g, closeRect, Color.FromArgb(186, 26, 26), padding, touchTarget, iconArea, "close", metrics);
            
            // Maximize button: On-surface variant with state layer
            PaintMaterial3Button(g, maxRect, Color.FromArgb(73, 69, 79), padding, touchTarget, iconArea, "maximize", metrics);
            
            // Minimize button: On-surface variant with state layer
            PaintMaterial3Button(g, minRect, Color.FromArgb(73, 69, 79), padding, touchTarget, iconArea, "minimize", metrics);
            
            // Theme/Style buttons if shown
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintMaterial3Button(g, styleRect, Color.FromArgb(103, 80, 164), padding, touchTarget, iconArea, "Style", metrics);
            }

            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintMaterial3Button(g, themeRect, Color.FromArgb(230, 74, 25), padding, touchTarget, iconArea, "theme", metrics);
            }
        }
        
        private void PaintMaterial3Button(Graphics g, Rectangle buttonRect, Color baseColor, int padding, int touchTarget, int iconArea, string buttonType, FormPainterMetrics metrics)
        {
            int centerX = buttonRect.X + buttonRect.Width / 2;
            int centerY = buttonRect.Y + buttonRect.Height / 2;
            var touchRect = new Rectangle(centerX - touchTarget / 2, centerY - touchTarget / 2, touchTarget, touchTarget);
            var iconRect = new Rectangle(centerX - iconArea / 2, centerY - iconArea / 2, iconArea, iconArea);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Material 3 state layer (8% opacity for hover state)
            using (var stateBrush = new SolidBrush(Color.FromArgb(20, baseColor)))
            {
                g.FillEllipse(stateBrush, touchRect);
            }
            
            // Draw icon with Material 3 Style (2dp stroke, round caps)
            int iconSize = 12; // 12px actual icon within 24px icon area
            int iconCenterX = iconRect.X + iconRect.Width / 2;
            int iconCenterY = iconRect.Y + iconRect.Height / 2;
            
            using (var iconPen = new Pen(baseColor, 2f))
            {
                iconPen.StartCap = LineCap.Round;
                iconPen.EndCap = LineCap.Round;
                iconPen.LineJoin = LineJoin.Round;
                
                switch (buttonType)
                {
                    case "close":
                        // Material X with rounded ends
                        g.DrawLine(iconPen, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2,
                            iconCenterX + iconSize / 2, iconCenterY + iconSize / 2);
                        g.DrawLine(iconPen, iconCenterX + iconSize / 2, iconCenterY - iconSize / 2,
                            iconCenterX - iconSize / 2, iconCenterY + iconSize / 2);
                        break;
                        
                    case "maximize":
                        // Outlined square with 2px rounded corners
                        using (var path = new GraphicsPath())
                        {
                            int halfSize = iconSize / 2;
                            var sqRect = new Rectangle(iconCenterX - halfSize, iconCenterY - halfSize, iconSize, iconSize);
                            int radius = 2;
                            
                            path.AddArc(sqRect.X, sqRect.Y, radius * 2, radius * 2, 180, 90);
                            path.AddArc(sqRect.Right - radius * 2, sqRect.Y, radius * 2, radius * 2, 270, 90);
                            path.AddArc(sqRect.Right - radius * 2, sqRect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                            path.AddArc(sqRect.X, sqRect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                            path.CloseFigure();
                            
                            g.DrawPath(iconPen, path);
                        }
                        break;
                        
                    case "minimize":
                        // Horizontal line with round caps
                        g.DrawLine(iconPen, iconCenterX - iconSize / 2, iconCenterY, iconCenterX + iconSize / 2, iconCenterY);
                        break;
                        
                    case "Style":
                        // Grid icon (Material Style)
                        int gridSize = 3;
                        int spacing = 5;
                        for (int i = 0; i < 2; i++)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                int x = iconCenterX - spacing / 2 + i * spacing;
                                int y = iconCenterY - spacing / 2 + j * spacing;
                                g.FillEllipse(new SolidBrush(baseColor), x - gridSize / 2, y - gridSize / 2, gridSize, gridSize);
                            }
                        }
                        break;
                        
                    case "theme":
                        // Contrast icon (Material accessibility)
                        g.DrawEllipse(iconPen, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2, iconSize, iconSize);
                        using (var fillBrush = new SolidBrush(baseColor))
                        {
                            g.FillPie(fillBrush, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2, 
                                iconSize, iconSize, 270, 180);
                        }
                        break;
                }
            }
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
                owner._hits.RegisterHitArea("Style", layout.StyleButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Theme button (if shown)
            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Custom action button (if theme/Style not shown)
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

        // Painter-owned non-client border rendering for Material Style
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
