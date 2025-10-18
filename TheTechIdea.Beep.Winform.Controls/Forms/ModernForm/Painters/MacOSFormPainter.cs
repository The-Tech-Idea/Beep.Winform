using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// macOS-style form painter with traffic light buttons and subtle borders.
    /// </summary>
    internal sealed class MacOSFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.MacOS, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            // Base macOS background only; effects handled under clipping in PaintWithEffects
            using (var baseBrush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(baseBrush, owner.ClientRectangle);
            }

            // Gentle macOS-like inner highlights/shading
            var r = owner.ClientRectangle;
            // Top highlight
            var topBand = new Rectangle(r.Left, r.Top, r.Width, Math.Max(1, r.Height / 4));
            using (var topGrad = new LinearGradientBrush(
                topBand,
                Color.FromArgb(40, 255, 255, 255),
                Color.FromArgb(0, 255, 255, 255),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(topGrad, topBand);
            }
            // Bottom shade
            var bottomBand = new Rectangle(r.Left, r.Bottom - Math.Max(1, r.Height / 6), r.Width, Math.Max(1, r.Height / 6));
            using (var botGrad = new LinearGradientBrush(
                bottomBand,
                Color.FromArgb(0, 0, 0, 0),
                Color.FromArgb(16, 0, 0, 0),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(botGrad, bottomBand);
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);

            // macOS title bar with subtle translucency
            using var titleBrush = new LinearGradientBrush(
                captionRect,
                Color.FromArgb(240, 240, 240), // Light gray
                Color.FromArgb(220, 220, 220), // Slightly darker
                LinearGradientMode.Vertical);
            g.FillRectangle(titleBrush, captionRect);

            // Add subtle inner shadow for depth
            using var shadowBrush = new LinearGradientBrush(
                captionRect,
                Color.FromArgb(60, 0, 0, 0),   // Top shadow
                Color.FromArgb(0, 0, 0, 0),     // Fade to transparent
                LinearGradientMode.Vertical);
            g.FillRectangle(shadowBrush, captionRect);

            // Draw refined traffic light buttons with 3D effects
            DrawTrafficLights(g, captionRect);

            // Draw title text with macOS typography
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect,
         metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // Built-in caption elements (icon/title/theme/style/custom/system buttons)
            owner.PaintBuiltInCaptionElements(g);
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = owner.BorderShape;
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
            // Subtle translucent overlay and mild gradient for depth
            using var translucentBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0));
            g.FillRectangle(translucentBrush, owner.ClientRectangle);

            using var depth = new LinearGradientBrush(
                owner.ClientRectangle,
                Color.FromArgb(10, 255, 255, 255),
                Color.FromArgb(0, 0, 0, 0),
                LinearGradientMode.Vertical);
            g.FillRectangle(depth, owner.ClientRectangle);
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

        // Painter-owned non-client border rendering for macOS style
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
        /// Draws macOS-style traffic light buttons with 3D effects
        /// </summary>
        private void DrawTrafficLights(Graphics g, Rectangle captionRect)
        {
            var buttonSize = 12;
            var spacing = 8;
            var topOffset = (captionRect.Height - buttonSize) / 2;
            var leftOffset = 12;

            // Traffic light colors
            var red = Color.FromArgb(255, 95, 87);
            var yellow = Color.FromArgb(255, 189, 46);
            var green = Color.FromArgb(39, 201, 63);

            // Draw each button with 3D effect
            DrawTrafficLightButton(g, captionRect.Left + leftOffset, captionRect.Top + topOffset, buttonSize, red);
            DrawTrafficLightButton(g, captionRect.Left + leftOffset + spacing + buttonSize, captionRect.Top + topOffset, buttonSize, yellow);
            DrawTrafficLightButton(g, captionRect.Left + leftOffset + (spacing + buttonSize) * 2, captionRect.Top + topOffset, buttonSize, green);
        }

        /// <summary>
        /// Draws a single traffic light button with highlight and shadow
        /// </summary>
        private void DrawTrafficLightButton(Graphics g, int x, int y, int size, Color color)
        {
            // Main button
            using var buttonBrush = new SolidBrush(color);
            g.FillEllipse(buttonBrush, x, y, size, size);

            // Highlight (top-left)
            using var highlightBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
            g.FillEllipse(highlightBrush, x, y, size * 0.6f, size * 0.6f);

            // Shadow (bottom-right)
            using var shadowBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0));
            g.FillEllipse(shadowBrush, x + size * 0.4f, y + size * 0.4f, size * 0.6f, size * 0.6f);

            // Border
            using var borderPen = new Pen(Color.FromArgb(60, 0, 0, 0), 0.5f);
            g.DrawEllipse(borderPen, x, y, size, size);
        }

        public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
        {
            var layout = new PainterLayoutInfo();
            
            owner._hits.Clear();
            
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
            var buttonSize = 12; // macOS traffic lights are small circles
            var buttonY = (captionHeight - buttonSize) / 2;
            var buttonSpacing = 8;
            var leftX = 12; // Start from left edge with padding
            
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
            var rightButtonWidth = 32;
            var rightX = owner.ClientSize.Width - rightButtonWidth;
            
            // Style button (if shown)
            if (owner.ShowStyleButton)
            {
                layout.StyleButtonRect = new Rectangle(rightX, 0, rightButtonWidth, captionHeight);
                owner._hits.RegisterHitArea("style", layout.StyleButtonRect, HitAreaType.Button);
                rightX -= rightButtonWidth;
            }
            
            // Theme button (if shown)
            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(rightX, 0, rightButtonWidth, captionHeight);
                owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
                rightX -= rightButtonWidth;
            }
            
            // Custom action button (if theme/style not shown)
            if (!owner.ShowThemeButton && !owner.ShowStyleButton)
            {
                layout.CustomActionButtonRect = new Rectangle(rightX, 0, rightButtonWidth, captionHeight);
                owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
                rightX -= rightButtonWidth;
            }
            
            // Icon and title areas: start after traffic lights, end before right-side buttons
            var iconSize = 16;
            var iconPadding = 8;
            var titleStartX = leftX + 16; // After last traffic light with padding

            var iconX = titleStartX;
            layout.IconRect = new Rectangle(iconX, (captionHeight - iconSize) / 2, iconSize, iconSize);
            if (owner.ShowIcon && owner.Icon != null)
            {
                owner._hits.RegisterHitArea("icon", layout.IconRect, HitAreaType.Icon);
            }

            var titleX = iconX + iconSize + iconPadding;
            var titleWidth = Math.Max(0, rightX - titleX - 8);
            layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
            owner._hits.RegisterHitArea("title", layout.TitleRect, HitAreaType.Caption);
            
            // Set content rectangle (below caption)
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
            
            owner.CurrentLayout = layout;
        }
    }
}