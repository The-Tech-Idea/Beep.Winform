using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Nord theme with frost gradients and rounded triangle buttons
    /// </summary>
    internal sealed class NordFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Nord, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);

            // Nord: frost gradient background
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
            
            // Frost gradient overlay (subtle blue-white tint from top)
            using (var frostBrush = new LinearGradientBrush(
                new Rectangle(0, 0, owner.ClientRectangle.Width, owner.ClientRectangle.Height / 3),
                Color.FromArgb(8, 200, 220, 240),  // Icy blue-white
                Color.FromArgb(0, 200, 220, 240),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(frostBrush, 0, 0, owner.ClientRectangle.Width, owner.ClientRectangle.Height / 3);
            }
            
            // Subtle frost line at top (icy blue)
            using (var linePen = new Pen(Color.FromArgb(40, 180, 200, 230), 1))
            {
                g.DrawLine(linePen, 0, 0, owner.ClientRectangle.Width, 0);
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Nord: frost gradient caption
            using (var capBrush = new LinearGradientBrush(captionRect,
                Color.FromArgb(10, 200, 220, 240),
                Color.FromArgb(5, 200, 220, 240),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(capBrush, captionRect);
            }
            
            // Base caption
            using (var baseBrush = new SolidBrush(metrics.CaptionColor))
            {
                g.FillRectangle(baseBrush, captionRect);
            }

            // Paint Nord rounded triangle buttons (UNIQUE SKIN)
            PaintNordTriangleButtons(g, owner, captionRect, metrics);

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            owner.PaintBuiltInCaptionElements(g);
        }
        
        /// <summary>
        /// Paint Nord rounded triangle buttons (UNIQUE SKIN)
        /// Smooth rounded triangles with frost gradient fills
        /// </summary>
        private void PaintNordTriangleButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            int triangleSize = 18;
            int triangleY = (captionRect.Height - triangleSize) / 2;
            
            // Close button: Red-frost triangle
            int cx = closeRect.X + closeRect.Width / 2;
            int cy = triangleY + triangleSize / 2;
            
            using (var trianglePath = CreateRoundedTrianglePath(cx, cy, triangleSize))
            {
                // Frost gradient fill (red with icy tint)
                using (var gradBrush = new LinearGradientBrush(
                    new Rectangle(cx - triangleSize/2, cy - triangleSize/2, triangleSize, triangleSize),
                    Color.FromArgb(200, 191, 97, 106),  // Nord red
                    Color.FromArgb(200, 180, 90, 100),  // Darker
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(gradBrush, trianglePath);
                }
                
                // Frost outline
                using (var outlinePen = new Pen(Color.FromArgb(150, 220, 230, 240), 1.5f))
                {
                    g.DrawPath(outlinePen, trianglePath);
                }
            }
            
            // X icon
            using (var iconPen = new Pen(Color.FromArgb(255, 236, 239, 244), 1.5f)) // Nord snow white
            {
                int iconSize = 6;
                g.DrawLine(iconPen, cx - iconSize/2, cy - iconSize/2, cx + iconSize/2, cy + iconSize/2);
                g.DrawLine(iconPen, cx + iconSize/2, cy - iconSize/2, cx - iconSize/2, cy + iconSize/2);
            }
            
            // Maximize button: Blue-frost triangle
            int mx = maxRect.X + maxRect.Width / 2;
            int my = triangleY + triangleSize / 2;
            
            using (var trianglePath = CreateRoundedTrianglePath(mx, my, triangleSize))
            {
                // Frost gradient (blue)
                using (var gradBrush = new LinearGradientBrush(
                    new Rectangle(mx - triangleSize/2, my - triangleSize/2, triangleSize, triangleSize),
                    Color.FromArgb(200, 129, 161, 193),  // Nord frost blue
                    Color.FromArgb(200, 120, 150, 180),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(gradBrush, trianglePath);
                }
                
                using (var outlinePen = new Pen(Color.FromArgb(150, 220, 230, 240), 1.5f))
                {
                    g.DrawPath(outlinePen, trianglePath);
                }
            }
            
            // Square icon
            using (var iconPen = new Pen(Color.FromArgb(255, 236, 239, 244), 1.5f))
            {
                int sqSize = 6;
                g.DrawRectangle(iconPen, mx - sqSize/2, my - sqSize/2, sqSize, sqSize);
            }
            
            // Minimize button: Teal-frost triangle
            int mnx = minRect.X + minRect.Width / 2;
            int mny = triangleY + triangleSize / 2;
            
            using (var trianglePath = CreateRoundedTrianglePath(mnx, mny, triangleSize))
            {
                // Frost gradient (teal)
                using (var gradBrush = new LinearGradientBrush(
                    new Rectangle(mnx - triangleSize/2, mny - triangleSize/2, triangleSize, triangleSize),
                    Color.FromArgb(200, 136, 192, 208),  // Nord aurora teal
                    Color.FromArgb(200, 125, 180, 195),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(gradBrush, trianglePath);
                }
                
                using (var outlinePen = new Pen(Color.FromArgb(150, 220, 230, 240), 1.5f))
                {
                    g.DrawPath(outlinePen, trianglePath);
                }
            }
            
            // Line icon
            using (var iconPen = new Pen(Color.FromArgb(255, 236, 239, 244), 1.5f))
            {
                int lineSize = 7;
                g.DrawLine(iconPen, mnx - lineSize/2, mny, mnx + lineSize/2, mny);
            }
        }
        
        /// <summary>
        /// Create rounded triangle path (3-sided with curved corners)
        /// </summary>
        private GraphicsPath CreateRoundedTrianglePath(int centerX, int centerY, int size)
        {
            var path = new GraphicsPath();
            int halfSize = size / 2;
            
            // Triangle points (pointing up)
            var p1 = new PointF(centerX, centerY - halfSize);         // Top
            var p2 = new PointF(centerX + halfSize, centerY + halfSize/2);   // Bottom right
            var p3 = new PointF(centerX - halfSize, centerY + halfSize/2);   // Bottom left
            
            // Create rounded triangle by adding curves between points
            path.AddCurve(new[] { p1, p2, p3, p1 }, 0.5f);
            path.CloseFigure();
            
            return path;
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            
            // Arc thin border (1px)
            using var pen = new Pen(Color.FromArgb(Math.Max(0, metrics.BorderColor.A - 50), metrics.BorderColor), 1);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Arc Linux material flat shadow (minimal blur)
            return new ShadowEffect
            {
                Color = Color.FromArgb(30, 0, 0, 0), // Slightly darker for flat material
                Blur = 6, // Minimal blur for flat aesthetic
                OffsetY = 3, // Small offset
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(4); // Arc small corner radius
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
