using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Fluent Design form painter with acrylic background and shadow effects.
    /// </summary>
    internal sealed class FluentFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        // Cache the noise bitmap and texture brush to avoid recreating on every paint
        private static Bitmap _cachedNoiseBitmap;
        private static TextureBrush _cachedNoiseBrush;
        private static readonly object _noiseLock = new object();

        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Fluent, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);

            // Paint only the base background across the entire form.
            // Effects (noise/gradients) are applied separately under controlled clipping in PaintWithEffects.
            using var baseBrush = new SolidBrush(Color.FromArgb(240, 243, 249, 253)); // Fluent light blue-gray
            g.FillRectangle(baseBrush, owner.ClientRectangle);
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);

            // Fluent reveal highlight effect
            using var highlightBrush = new LinearGradientBrush(
                captionRect,
                Color.FromArgb(40, 255, 255, 255), // Top highlight
                Color.FromArgb(0, 255, 255, 255),   // Fade to transparent
                LinearGradientMode.Vertical);
            g.FillRectangle(highlightBrush, captionRect);

            // Draw accent line with Fluent Design reveal effect
            using var accentPen = new Pen(Color.FromArgb(255, 0, 120, 215), 3f); // Fluent blue
            accentPen.StartCap = LineCap.Round;
            accentPen.EndCap = LineCap.Round;
            g.DrawLine(accentPen, captionRect.Left + 8, captionRect.Bottom - 2, captionRect.Right - 8, captionRect.Bottom - 2);

            // Add subtle depth line above accent
            using var depthPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1f);
            g.DrawLine(depthPen, captionRect.Left, captionRect.Bottom - 6, captionRect.Right, captionRect.Bottom - 6);

            // Draw title text with Fluent typography
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, 
                metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // Built-in caption elements
            owner.PaintBuiltInCaptionElements(g);
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, metrics.BorderWidth));
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Simplified shadow for better performance
            return new ShadowEffect
            {
                Color = Color.FromArgb(15, 0, 0, 0), // Reduced opacity
                Blur = 8,  // Reduced blur
                OffsetY = 4, // Reduced offset
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(8);
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.High;
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
            // Orchestrated painting: use painter functions, apply effects under clipping only.
            var shadow = GetShadowEffect(owner);
            var radius = GetCornerRadius(owner);

            // Save original clip
            var originalClip = g.Clip;

            // 1) Optional shadow
            if (!shadow.Inner)
            {
                DrawShadow(g, rect, shadow, radius);
            }

            // 2) Paint base background across the entire form
            PaintBackground(g, owner);

            // 3) Apply background effects with clipping (exclude content unless PaintOverContentArea)
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

        // Draw fluent background effects (acrylic noise and gradient) under controlled clipping
        private void PaintBackgroundEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            // Add acrylic noise texture
            DrawAcrylicNoise(g, owner.ClientRectangle);

            // Add subtle gradient overlay for depth
            using var gradientBrush = new LinearGradientBrush(
                owner.ClientRectangle,
                Color.FromArgb(60, 255, 255, 255), // Light acrylic overlay
                Color.FromArgb(20, 0, 120, 215),   // Subtle blue tint
                LinearGradientMode.Vertical);
            g.FillRectangle(gradientBrush, owner.ClientRectangle);
        }

        private void DrawShadow(Graphics g, Rectangle rect, ShadowEffect shadow, CornerRadius radius)
        {
            // Validate input rectangle
            if (rect.Width <= 0 || rect.Height <= 0) return;

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

        // Painter-owned non-client border rendering for Fluent style
        public void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            var outerRect = new Rectangle(0, 0, owner.Width, owner.Height);
            using var path = CreateRoundedRectanglePath(outerRect, radius);
            using var pen = new Pen(Color.FromArgb(120, metrics.BorderColor), Math.Max(1, borderThickness))
            {
                Alignment = PenAlignment.Inset
            };
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);

            // Faint top-edge light to match Fluent highlight
            using var hi = new Pen(Color.FromArgb(30, 255, 255, 255), 1f) { Alignment = PenAlignment.Inset };
            int y = Math.Max(0, borderThickness - 1);
            g.DrawLine(hi, 0, y, owner.Width - 1, y);
        }

        /// <summary>
        /// Draws subtle noise texture for acrylic effect using cached bitmap and brush
        /// </summary>
        private void DrawAcrylicNoise(Graphics g, Rectangle rect)
        {
            // Use cached noise bitmap and brush, create once
            if (_cachedNoiseBrush == null)
            {
                lock (_noiseLock)
                {
                    if (_cachedNoiseBrush == null)
                    {
                        // Create noise pattern only once
                        var noiseBitmap = new Bitmap(64, 64);
                        var random = new Random(42); // Fixed seed for consistent pattern
                        
                        // Draw noise using graphics for better performance than SetPixel
                        using (var gfx = Graphics.FromImage(noiseBitmap))
                        {
                            // Fill with transparent white
                            gfx.Clear(Color.Transparent);
                            
                            // Draw random noise pixels (reduced density for performance)
                            for (int i = 0; i < 200; i++)
                            {
                                int x = random.Next(64);
                                int y = random.Next(64);
                                int alpha = random.Next(5, 15);
                                using (var brush = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255)))
                                {
                                    gfx.FillRectangle(brush, x, y, 1, 1);
                                }
                            }
                        }
                        
                        _cachedNoiseBitmap = noiseBitmap;
                        _cachedNoiseBrush = new TextureBrush(noiseBitmap) { WrapMode = WrapMode.Tile };
                    }
                }
            }

            // Use cached texture brush - no new allocations
            g.FillRectangle(_cachedNoiseBrush, rect);
        }
    }
}