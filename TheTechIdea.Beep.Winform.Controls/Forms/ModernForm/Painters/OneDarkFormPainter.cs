using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Atom One Dark editor theme
    /// </summary>
    internal sealed class OneDarkFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.OneDark, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            
            // OneDark flat code editor style
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
            
            // Subtle grid pattern (like code editor gutter) - 1px dots every 40px
            using (var gridPen = new Pen(Color.FromArgb(8, 255, 255, 255)))
            {
                for (int x = 40; x < owner.ClientRectangle.Width; x += 40)
                {
                    for (int y = 40; y < owner.ClientRectangle.Height; y += 40)
                    {
                        g.DrawRectangle(gridPen, x, y, 1, 1);
                    }
                }
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            using (var capBrush = new SolidBrush(metrics.CaptionColor))
            {
                g.FillRectangle(capBrush, captionRect);
            }

            // Paint OneDark octagonal buttons (UNIQUE)
            PaintOneDarkOctagonButtons(g, owner, captionRect, metrics);

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            owner.PaintBuiltInCaptionElements(g);
        }
        
        /// <summary>
        /// Paint OneDark octagonal buttons (UNIQUE - 8-sided polygons)
        /// </summary>
        private void PaintOneDarkOctagonButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            int octSize = 18;
            int octY = (captionRect.Height - octSize) / 2;
            
            // Close button: Red octagon with X
            int cx = closeRect.X + closeRect.Width / 2;
            using (var octPath = CreateOctagonPath(cx, octY + octSize/2, octSize/2))
            {
                using (var octBrush = new SolidBrush(Color.FromArgb(224, 108, 117)))
                {
                    g.FillPath(octBrush, octPath);
                }
                
                using (var iconPen = new Pen(Color.White, 1.5f))
                {
                    int iconSize = 7;
                    int cy = octY + octSize/2;
                    g.DrawLine(iconPen, cx - iconSize/2, cy - iconSize/2, cx + iconSize/2, cy + iconSize/2);
                    g.DrawLine(iconPen, cx + iconSize/2, cy - iconSize/2, cx - iconSize/2, cy + iconSize/2);
                }
            }
            
            // Maximize button: Dark octagon with square
            int mx = maxRect.X + maxRect.Width / 2;
            using (var octPath = CreateOctagonPath(mx, octY + octSize/2, octSize/2))
            {
                using (var octBrush = new SolidBrush(Color.FromArgb(75, 75, 75)))
                {
                    g.FillPath(octBrush, octPath);
                }
                
                using (var iconPen = new Pen(Color.White, 1.3f))
                {
                    int sqSize = 6;
                    int my = octY + octSize/2;
                    g.DrawRectangle(iconPen, mx - sqSize/2, my - sqSize/2, sqSize, sqSize);
                }
            }
            
            // Minimize button: Dark octagon with line
            int mnx = minRect.X + minRect.Width / 2;
            using (var octPath = CreateOctagonPath(mnx, octY + octSize/2, octSize/2))
            {
                using (var octBrush = new SolidBrush(Color.FromArgb(75, 75, 75)))
                {
                    g.FillPath(octBrush, octPath);
                }
                
                using (var iconPen = new Pen(Color.White, 1.3f))
                {
                    int lineSize = 7;
                    int mny = octY + octSize/2;
                    g.DrawLine(iconPen, mnx - lineSize/2, mny, mnx + lineSize/2, mny);
                }
            }
        }
        
        /// <summary>
        /// Create octagon path (8-sided regular polygon)
        /// </summary>
        private GraphicsPath CreateOctagonPath(int centerX, int centerY, int radius)
        {
            var path = new GraphicsPath();
            var points = new PointF[8];
            
            for (int i = 0; i < 8; i++)
            {
                double angle = Math.PI / 4 * i;
                points[i] = new PointF(
                    centerX + (float)(radius * Math.Cos(angle)),
                    centerY + (float)(radius * Math.Sin(angle))
                );
            }
            
            path.AddPolygon(points);
            path.CloseFigure();
            return path;
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            using var pen = new Pen(Color.FromArgb(Math.Max(0, metrics.BorderColor.A - 50), metrics.BorderColor), 1);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            return new ShadowEffect
            {
                Color = Color.FromArgb(35, 0, 0, 0),
                Blur = 10,
                OffsetY = 4,
                Inner = false
            };
        }

      

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.High;
        }


        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(4);
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
