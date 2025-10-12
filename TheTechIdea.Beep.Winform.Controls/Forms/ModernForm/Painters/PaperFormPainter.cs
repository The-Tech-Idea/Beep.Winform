using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Material Design Paper style with elevation shadows and layered depth
    /// </summary>
    internal sealed class PaperFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Paper, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);

            // Material Paper: subtle texture with elevation effect
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
            
            // Add subtle material texture (noise pattern)
            Random rand = new Random(owner.ClientRectangle.GetHashCode());
            using (var textureBrush = new SolidBrush(Color.FromArgb(3, 255, 255, 255)))
            {
                for (int i = 0; i < 50; i++)
                {
                    int x = rand.Next(owner.ClientRectangle.Width);
                    int y = rand.Next(owner.ClientRectangle.Height);
                    g.FillRectangle(textureBrush, x, y, 1, 1);
                }
            }
            
            // Material elevation line at top (subtle white, 1px)
            using (var linePen = new Pen(Color.FromArgb(40, 255, 255, 255), 1))
            {
                g.DrawLine(linePen, 0, 0, owner.ClientRectangle.Width, 0);
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Material Paper caption: elevated surface with subtle gradient
            using (var capBrush = new LinearGradientBrush(captionRect, 
                Color.FromArgb(10, 255, 255, 255), 
                Color.FromArgb(5, 0, 0, 0), 
                LinearGradientMode.Vertical))
            {
                var blend = new ColorBlend(3);
                blend.Colors = new[] { 
                    Color.FromArgb(8, 255, 255, 255), 
                    metrics.CaptionColor, 
                    Color.FromArgb(3, 0, 0, 0) 
                };
                blend.Positions = new[] { 0f, 0.5f, 1f };
                capBrush.InterpolationColors = blend;
                
                g.FillRectangle(capBrush, captionRect);
            }
            
            // Material elevation separator line (bottom of caption)
            using (var linePen = new Pen(Color.FromArgb(20, 0, 0, 0), 1))
            {
                g.DrawLine(linePen, 0, captionRect.Bottom - 1, captionRect.Width, captionRect.Bottom - 1);
            }

            // Paint Material Paper double-border CIRCLE buttons (UNIQUE)
            PaintPaperCircleButtons(g, owner, captionRect, metrics);

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            owner.PaintBuiltInCaptionElements(g);
        }
        
        /// <summary>
        /// Paint Material Design Paper double-border circle buttons (UNIQUE SKIN)
        /// Concentric ring buttons with material elevation shadows
        /// </summary>
        private void PaintPaperCircleButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            int circleDiameter = 22; // Circle button size
            int circleY = (captionRect.Height - circleDiameter) / 2;
            
            // Close button: Red double-border circle with X
            int cx = closeRect.X + closeRect.Width / 2;
            int cy = circleY + circleDiameter / 2;
            
            // Outer ring (2px, darker red)
            using (var outerPen = new Pen(Color.FromArgb(180, 50, 50), 2))
            {
                g.DrawEllipse(outerPen, cx - circleDiameter/2, cy - circleDiameter/2, circleDiameter, circleDiameter);
            }
            
            // Inner ring (1px, lighter red, inset by 3px)
            using (var innerPen = new Pen(Color.FromArgb(220, 80, 80), 1))
            {
                int innerSize = circleDiameter - 6;
                g.DrawEllipse(innerPen, cx - innerSize/2, cy - innerSize/2, innerSize, innerSize);
            }
            
            // X icon (8px, centered)
            using (var iconPen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.5f))
            {
                int iconSize = 8;
                g.DrawLine(iconPen, cx - iconSize/2, cy - iconSize/2, cx + iconSize/2, cy + iconSize/2);
                g.DrawLine(iconPen, cx + iconSize/2, cy - iconSize/2, cx - iconSize/2, cy + iconSize/2);
            }
            
            // Maximize button: Gray double-border circle with square
            int mx = maxRect.X + maxRect.Width / 2;
            int my = circleY + circleDiameter / 2;
            
            // Outer ring (2px, dark gray)
            using (var outerPen = new Pen(Color.FromArgb(100, 100, 100), 2))
            {
                g.DrawEllipse(outerPen, mx - circleDiameter/2, my - circleDiameter/2, circleDiameter, circleDiameter);
            }
            
            // Inner ring (1px, lighter gray)
            using (var innerPen = new Pen(Color.FromArgb(140, 140, 140), 1))
            {
                int innerSize = circleDiameter - 6;
                g.DrawEllipse(innerPen, mx - innerSize/2, my - innerSize/2, innerSize, innerSize);
            }
            
            // Square icon (7px)
            using (var iconPen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.5f))
            {
                int sqSize = 7;
                g.DrawRectangle(iconPen, mx - sqSize/2, my - sqSize/2, sqSize, sqSize);
            }
            
            // Minimize button: Gray double-border circle with line
            int mnx = minRect.X + minRect.Width / 2;
            int mny = circleY + circleDiameter / 2;
            
            // Outer ring (2px, dark gray)
            using (var outerPen = new Pen(Color.FromArgb(100, 100, 100), 2))
            {
                g.DrawEllipse(outerPen, mnx - circleDiameter/2, mny - circleDiameter/2, circleDiameter, circleDiameter);
            }
            
            // Inner ring (1px, lighter gray)
            using (var innerPen = new Pen(Color.FromArgb(140, 140, 140), 1))
            {
                int innerSize = circleDiameter - 6;
                g.DrawEllipse(innerPen, mnx - innerSize/2, mny - innerSize/2, innerSize, innerSize);
            }
            
            // Line icon (8px horizontal)
            using (var iconPen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.5f))
            {
                int lineSize = 8;
                g.DrawLine(iconPen, mnx - lineSize/2, mny, mnx + lineSize/2, mny);
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            
            // Material Paper: subtle elevated border (1px with alpha)
            using var pen = new Pen(Color.FromArgb(40, metrics.BorderColor), 1);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Material Design Paper: LAYERED elevation shadow (multiple blur layers for depth)
            // This creates the "floating paper" effect characteristic of Material Design
            return new ShadowEffect
            {
                Color = Color.FromArgb(50, 0, 0, 0), // Deeper shadow for elevation
                Blur = 12, // Wider blur for Material elevation (not flat like Arc)
                OffsetY = 6, // Larger offset for elevated paper effect
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(7); // Material Paper smooth corners (not 4px like Arc)
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
