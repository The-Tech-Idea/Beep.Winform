using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;


namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// A playful, cartoon-inspired painter with bold outlines, comic book effects, and vibrant styling.
    /// Features thick borders, drop shadows, and comic book-style caption with speech bubble elements.
    /// </summary>
    internal sealed class CartoonFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Cartoon, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            // Vibrant base
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }

            // Subtle halftone dots for comic page texture
            using var dotBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0));
            int spacing = 8;
            var r = owner.ClientRectangle;
            for (int y = r.Top; y < r.Bottom; y += spacing)
            {
                for (int x = r.Left; x < r.Right; x += spacing)
                {
                    g.FillEllipse(dotBrush, x, y, 1, 1);
                }
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            
            // Bold cartoon border around caption
            using var borderPen = new Pen(metrics.BorderColor, 3f);
            g.DrawRectangle(borderPen, captionRect);

            // Comic book style gradient fill
            using var gradBrush = new LinearGradientBrush(
                captionRect,
                Color.FromArgb(255, 255, 253, 208), // Light yellow
                Color.FromArgb(255, 255, 218, 121), // Golden yellow
                LinearGradientMode.Vertical);
            var innerRect = Rectangle.Inflate(captionRect, -3, -3);
            g.FillRectangle(gradBrush, innerRect);

            // Comic book speed lines effect
            DrawComicSpeedLines(g, captionRect);

            // Title with bold, cartoon-style font effect
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect,
                metrics.CaptionTextColor, // High contrast black
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // Built-in caption elements
            owner.PaintBuiltInCaptionElements(g);
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
           using var path = owner.BorderShape;
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, metrics.BorderWidth));
            pen.Alignment = PenAlignment.Center;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            return new ShadowEffect
            {
                Color = Color.FromArgb(30, 0, 0, 0),
                Blur = 15,
                OffsetY = 8,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(20);
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.Ultra;
        }

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

            // 3) Background effects with clipping (none special here, keep structure)
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
            // No additional background effects for cartoon; caption has its own effects
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

        /// <summary>
        /// Draws comic book style speed lines for cartoon effect
        /// </summary>
        private void DrawComicSpeedLines(Graphics g, Rectangle rect)
        {
            using var speedPen = new Pen(Color.FromArgb(100, 0, 0, 0), 1f);
            var random = new Random(123); // Fixed seed for consistent pattern
            
            // Draw diagonal speed lines
            for (int i = 0; i < 8; i++)
            {
                int startX = rect.Left + random.Next(rect.Width);
                int startY = rect.Top + random.Next(rect.Height / 3);
                int endX = startX + random.Next(20, 40);
                int endY = startY + random.Next(10, 20);
                
                if (endX < rect.Right && endY < rect.Bottom)
                {
                    g.DrawLine(speedPen, startX, startY, endX, endY);
                }
            }
        }

        public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
        {
            var layout = new PainterLayoutInfo();
            
            // If caption bar is hidden, skip button layout
            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                owner.CurrentLayout = layout;
                return;
            }
            
            // Calculate caption height based on font and padding (Cartoon uses fun padding)
            var captionHeight = owner.Font.Height + 20; // 10px padding top and bottom for cartoon style
            
            // Set caption rectangle
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            
            // Set content rectangle (below caption)
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
            
            // Calculate button positions (right-aligned in caption, cartoon style)
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
            
            // Icon and title areas (left side of caption, cartoon style)
            var iconSize = 16;
            var iconPadding = 10; // Fun padding for cartoon
            layout.IconRect = new Rectangle(iconPadding, (captionHeight - iconSize) / 2, iconSize, iconSize);
            owner._hits.RegisterHitArea("icon", layout.IconRect, HitAreaType.Icon);
            
            var titleX = iconPadding + iconSize + iconPadding;
            var titleWidth = buttonX - titleX - iconPadding;
            layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
            owner._hits.RegisterHitArea("title", layout.TitleRect, HitAreaType.Caption);
            
            owner.CurrentLayout = layout;
        }

        // Painter-owned non-client border rendering for Cartoon style
        public void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            var outer = new Rectangle(0, 0, owner.Width, owner.Height);
            using var path = CreateRoundedRectanglePath(outer, radius);
            using var pen = new Pen(metrics.BorderColor, Math.Max(2, borderThickness + 1))
            {
                Alignment = PenAlignment.Inset,
                LineJoin = LineJoin.Round
            };
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);

            // Playful dotted top edge accent
            using var dotBrush = new SolidBrush(Color.FromArgb(80, metrics.BorderColor));
            int y = Math.Max(0, borderThickness / 2);
            for (int x = 8; x < owner.Width - 8; x += 12)
            {
                g.FillEllipse(dotBrush, x, y, 2, 2);
            }
        }

        public bool SupportsAnimations => false;
    }
}
