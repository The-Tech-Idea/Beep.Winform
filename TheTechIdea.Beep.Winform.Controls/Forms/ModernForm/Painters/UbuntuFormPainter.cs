using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Ubuntu Unity desktop style
    /// </summary>
    internal sealed class UbuntuFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Ubuntu, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Ubuntu warm gradient (subtle 6% lighter at top)
            using (var gradBrush = new LinearGradientBrush(
                owner.ClientRectangle,
                ControlPaint.Light(metrics.BackgroundColor, 0.06f),
                metrics.BackgroundColor,
                90f))
            {
                g.FillRectangle(gradBrush, owner.ClientRectangle);
            }
            
            // Ubuntu Unity launcher-inspired vertical accent line (4px wide, left edge)
            var accentColor = Color.FromArgb(180, 233, 84, 32); // Ubuntu orange with transparency
            using (var accentBrush = new SolidBrush(accentColor))
            {
                g.FillRectangle(accentBrush, new Rectangle(0, 0, 4, owner.ClientRectangle.Height));
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Caption with subtle gradient
            using (var capBrush = new LinearGradientBrush(
                captionRect,
                ControlPaint.Light(metrics.CaptionColor, 0.04f),
                metrics.CaptionColor,
                90f))
            {
                g.FillRectangle(capBrush, captionRect);
            }
            
            // Ubuntu orange accent line at top of caption
            using (var accentPen = new Pen(Color.FromArgb(120, 233, 84, 32), 2))
            {
                g.DrawLine(accentPen, captionRect.Left, captionRect.Top, captionRect.Right, captionRect.Top);
            }

            // Paint Ubuntu pill-shaped buttons (UNIQUE)
            PaintUbuntuButtons(g, owner, captionRect, metrics);

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            owner.PaintBuiltInCaptionElements(g);
        }
        
        /// <summary>
        /// Paint Ubuntu Unity-style pill-shaped buttons (UNIQUE)
        /// </summary>
        private void PaintUbuntuButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            // Ubuntu pill shape: rounded rectangle (height-2 for padding, 14px corner radius)
            int pillHeight = captionRect.Height - 8;
            int pillY = (captionRect.Height - pillHeight) / 2;
            
            // Close button: Ubuntu orange pill with X symbol
            var closePill = new Rectangle(closeRect.X + 6, pillY, closeRect.Width - 12, pillHeight);
            using (var closePath = CreatePillPath(closePill, pillHeight / 2))
            {
                // Ubuntu orange fill
                using (var orangeBrush = new SolidBrush(Color.FromArgb(233, 84, 32)))
                {
                    g.FillPath(orangeBrush, closePath);
                }
                
                // White X symbol (8px)
                using (var xPen = new Pen(Color.White, 1.5f))
                {
                    int xSize = 8;
                    int cx = closePill.X + closePill.Width / 2;
                    int cy = closePill.Y + closePill.Height / 2;
                    g.DrawLine(xPen, cx - xSize/2, cy - xSize/2, cx + xSize/2, cy + xSize/2);
                    g.DrawLine(xPen, cx + xSize/2, cy - xSize/2, cx - xSize/2, cy + xSize/2);
                }
            }
            
            // Maximize button: dark pill with square symbol
            var maxPill = new Rectangle(maxRect.X + 6, pillY, maxRect.Width - 12, pillHeight);
            using (var maxPath = CreatePillPath(maxPill, pillHeight / 2))
            {
                using (var darkBrush = new SolidBrush(Color.FromArgb(60, 60, 60)))
                {
                    g.FillPath(darkBrush, maxPath);
                }
                
                // White square symbol (7px)
                using (var squarePen = new Pen(Color.White, 1.5f))
                {
                    int sSize = 7;
                    int mx = maxPill.X + maxPill.Width / 2;
                    int my = maxPill.Y + maxPill.Height / 2;
                    g.DrawRectangle(squarePen, mx - sSize/2, my - sSize/2, sSize, sSize);
                }
            }
            
            // Minimize button: dark pill with horizontal line symbol
            var minPill = new Rectangle(minRect.X + 6, pillY, minRect.Width - 12, pillHeight);
            using (var minPath = CreatePillPath(minPill, pillHeight / 2))
            {
                using (var darkBrush = new SolidBrush(Color.FromArgb(60, 60, 60)))
                {
                    g.FillPath(darkBrush, minPath);
                }
                
                // White line symbol (8px)
                using (var linePen = new Pen(Color.White, 1.5f))
                {
                    int lSize = 8;
                    int mnx = minPill.X + minPill.Width / 2;
                    int mny = minPill.Y + minPill.Height / 2;
                    g.DrawLine(linePen, mnx - lSize/2, mny, mnx + lSize/2, mny);
                }
            }
        }
        
        /// <summary>
        /// Create pill-shaped path (rounded rectangle)
        /// </summary>
        private GraphicsPath CreatePillPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            
            // Thin border with subtle Ubuntu orange tint
            var borderColor = Color.FromArgb(Math.Max(0, metrics.BorderColor.A - 50), 
                Math.Min(255, metrics.BorderColor.R + 20),
                metrics.BorderColor.G,
                Math.Min(255, metrics.BorderColor.B - 10));
            using var pen = new Pen(borderColor, 1);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Warm shadow with subtle orange tint (Ubuntu characteristic)
            return new ShadowEffect
            {
                Color = Color.FromArgb(22, 30, 15, 10), // Warm shadow (less blue, more red)
                Blur = 12,
                OffsetY = 5,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(7); // Ubuntu's characteristic corner radius
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.Ultra; // Smooth Ubuntu aesthetic
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
            owner._hits.Clear();
            
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            owner._hits.Register("caption", layout.CaptionRect, HitAreaType.Drag);
            
            int buttonWidth = metrics.ButtonWidth;
            int buttonX = owner.ClientSize.Width - buttonWidth;
            
            layout.CloseButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
            owner._hits.Register("close", layout.CloseButtonRect, HitAreaType.Button);
            buttonX -= buttonWidth;
            
            layout.MaximizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
            owner._hits.Register("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
            buttonX -= buttonWidth;
            
            layout.MinimizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
            owner._hits.Register("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
            
            int iconX = metrics.IconLeftPadding;
            int iconY = (captionHeight - metrics.IconSize) / 2;
            layout.IconRect = new Rectangle(iconX, iconY, metrics.IconSize, metrics.IconSize);
            if (owner.ShowIcon && owner.Icon != null)
            {
                owner._hits.Register("icon", layout.IconRect, HitAreaType.Icon);
            }
            
            int titleX = layout.IconRect.Right + metrics.TitleLeftPadding;
            int titleWidth = layout.MinimizeButtonRect.Left - metrics.ButtonSpacing - titleX;
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
