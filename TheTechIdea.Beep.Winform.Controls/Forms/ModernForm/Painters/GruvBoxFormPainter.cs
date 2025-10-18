using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// GruvBox retro style with 3D beveled buttons and warm textured aesthetic
    /// </summary>
    internal sealed class GruvBoxFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.GruvBox, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);

            // GruvBox: warm textured background with subtle grain/noise
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
            
            // Add warm retro grain texture (horizontal scan-like pattern)
            using (var grainPen = new Pen(Color.FromArgb(5, 255, 200, 100), 1))
            {
                for (int y = 0; y < owner.ClientRectangle.Height; y += 3)
                {
                    g.DrawLine(grainPen, 0, y, owner.ClientRectangle.Width, y);
                }
            }
            
            // Add subtle warm glow at top
            using (var glowBrush = new LinearGradientBrush(
                new Rectangle(0, 0, owner.ClientRectangle.Width, 40),
                Color.FromArgb(15, 251, 184, 108), // Warm orange glow
                Color.FromArgb(0, 251, 184, 108),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(glowBrush, 0, 0, owner.ClientRectangle.Width, 40);
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // GruvBox: warm gradient caption with retro feel
            using (var capBrush = new LinearGradientBrush(captionRect,
                Color.FromArgb(20, 251, 184, 108), // Warm top
                Color.FromArgb(10, 211, 134, 56),  // Darker warm bottom
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(capBrush, captionRect);
            }
            
            // Base caption color
            using (var baseBrush = new SolidBrush(metrics.CaptionColor))
            {
                g.FillRectangle(baseBrush, captionRect);
            }

            // Paint GruvBox 3D beveled rectangle buttons (UNIQUE RETRO SKIN)
            PaintGruvBeveledButtons(g, owner, captionRect, metrics);

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom GruvBox beveled buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }
        
        /// <summary>
        /// Paint GruvBox 3D beveled rectangle buttons (UNIQUE RETRO SKIN)
        /// Classic Win95-style raised buttons with ControlPaint bevels
        /// </summary>
        private void PaintGruvBeveledButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            var themeRect = owner.CurrentLayout.ThemeButtonRect;
            var styleRect = owner.CurrentLayout.StyleButtonRect;
            
            int buttonSize = 20;
            int buttonY = (captionRect.Height - buttonSize) / 2;
            
            // Close button: Red 3D beveled rectangle
            int cx = closeRect.X + (closeRect.Width - buttonSize) / 2;
            var closeButtonRect = new Rectangle(cx, buttonY, buttonSize, buttonSize);
            
            // Fill with warm red
            using (var fillBrush = new SolidBrush(Color.FromArgb(200, 80, 60)))
            {
                g.FillRectangle(fillBrush, closeButtonRect);
            }
            
            // Draw 3D raised bevel (light top-left, dark bottom-right)
            ControlPaint.DrawBorder3D(g, closeButtonRect, Border3DStyle.Raised, Border3DSide.All);
            
            // X icon
            using (var iconPen = new Pen(Color.White, 2f))
            {
                int iconSize = 8;
                int icx = closeButtonRect.X + closeButtonRect.Width / 2;
                int icy = closeButtonRect.Y + closeButtonRect.Height / 2;
                g.DrawLine(iconPen, icx - iconSize/2, icy - iconSize/2, icx + iconSize/2, icy + iconSize/2);
                g.DrawLine(iconPen, icx + iconSize/2, icy - iconSize/2, icx - iconSize/2, icy + iconSize/2);
            }
            
            // Maximize button: Gray 3D beveled rectangle
            int mx = maxRect.X + (maxRect.Width - buttonSize) / 2;
            var maxButtonRect = new Rectangle(mx, buttonY, buttonSize, buttonSize);
            
            // Fill with warm gray
            using (var fillBrush = new SolidBrush(Color.FromArgb(100, 90, 80)))
            {
                g.FillRectangle(fillBrush, maxButtonRect);
            }
            
            // Draw 3D raised bevel
            ControlPaint.DrawBorder3D(g, maxButtonRect, Border3DStyle.Raised, Border3DSide.All);
            
            // Square icon
            using (var iconPen = new Pen(Color.White, 2f))
            {
                int sqSize = 7;
                int imx = maxButtonRect.X + maxButtonRect.Width / 2;
                int imy = maxButtonRect.Y + maxButtonRect.Height / 2;
                g.DrawRectangle(iconPen, imx - sqSize/2, imy - sqSize/2, sqSize, sqSize);
            }
            
            // Minimize button: Gray 3D beveled rectangle
            int mnx = minRect.X + (minRect.Width - buttonSize) / 2;
            var minButtonRect = new Rectangle(mnx, buttonY, buttonSize, buttonSize);
            
            // Fill with warm gray
            using (var fillBrush = new SolidBrush(Color.FromArgb(100, 90, 80)))
            {
                g.FillRectangle(fillBrush, minButtonRect);
            }
            
            // Draw 3D raised bevel
            ControlPaint.DrawBorder3D(g, minButtonRect, Border3DStyle.Raised, Border3DSide.All);
            
            // Line icon
            using (var iconPen = new Pen(Color.White, 2f))
            {
                int lineSize = 8;
                int imnx = minButtonRect.X + minButtonRect.Width / 2;
                int imny = minButtonRect.Y + minButtonRect.Height / 2;
                g.DrawLine(iconPen, imnx - lineSize/2, imny, imnx + lineSize/2, imny);
            }
            
            // Theme button: Warm orange 3D beveled rectangle with palette icon (GruvBox orange #fe8019)
            if (!themeRect.IsEmpty)
            {
                int tx = themeRect.X + (themeRect.Width - buttonSize) / 2;
                var themeButtonRect = new Rectangle(tx, buttonY, buttonSize, buttonSize);
                
                using (var fillBrush = new SolidBrush(Color.FromArgb(254, 128, 25)))
                {
                    g.FillRectangle(fillBrush, themeButtonRect);
                }
                
                ControlPaint.DrawBorder3D(g, themeButtonRect, Border3DStyle.Raised, Border3DSide.All);
                
                // Palette icon
                using (var iconBrush = new SolidBrush(Color.White))
                {
                    int itx = themeButtonRect.X + themeButtonRect.Width / 2;
                    int ity = themeButtonRect.Y + themeButtonRect.Height / 2;
                    g.FillEllipse(iconBrush, itx - 4, ity - 3, 3, 3);
                    g.FillEllipse(iconBrush, itx + 1, ity - 3, 3, 3);
                    g.FillEllipse(iconBrush, itx - 1, ity + 1, 3, 3);
                }
            }
            
            // Style button: Warm yellow 3D beveled rectangle with brush icon (GruvBox yellow #fabd2f)
            if (!styleRect.IsEmpty)
            {
                int sx = styleRect.X + (styleRect.Width - buttonSize) / 2;
                var styleButtonRect = new Rectangle(sx, buttonY, buttonSize, buttonSize);
                
                using (var fillBrush = new SolidBrush(Color.FromArgb(250, 189, 47)))
                {
                    g.FillRectangle(fillBrush, styleButtonRect);
                }
                
                ControlPaint.DrawBorder3D(g, styleButtonRect, Border3DStyle.Raised, Border3DSide.All);
                
                // Brush icon
                using (var iconPen = new Pen(Color.White, 1.5f))
                {
                    int isx = styleButtonRect.X + styleButtonRect.Width / 2;
                    int isy = styleButtonRect.Y + styleButtonRect.Height / 2;
                    g.DrawLine(iconPen, isx - 3, isy - 3, isx - 3, isy + 3);
                    g.DrawLine(iconPen, isx, isy - 3, isx, isy + 3);
                    g.DrawLine(iconPen, isx + 3, isy - 3, isx + 3, isy + 3);
                }
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = owner.BorderShape; 
            // GruvBox: warm-tinted border (thicker for retro feel)
            using var pen = new Pen(Color.FromArgb(80, 251, 184, 108), 2);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // GruvBox: warm shadow with earth tone tint (retro aesthetic)
            return new ShadowEffect
            {
                Color = Color.FromArgb(40, 40, 30, 20), // Warm brownish shadow
                Blur = 8, // Medium blur for retro depth
                OffsetY = 4, // Moderate offset
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(6); // GruvBox moderate rounded corners
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
            
            owner._hits.Clear();
            
            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                owner.CurrentLayout = layout;
                return;
            }
            
            int captionHeight = Math.Max(metrics.CaptionHeight, (int)(owner.Font.Height * metrics.FontHeightMultiplier));
            
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            owner._hits.Register("caption", layout.CaptionRect, HitAreaType.Drag);
            
            int buttonWidth = metrics.ButtonWidth;
            int buttonX = owner.ClientSize.Width - buttonWidth;
            
            if (owner.ShowCloseButton)
            {
                layout.CloseButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            if (owner.ShowMinMaxButtons)
            {
                layout.MaximizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
                
                layout.MinimizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Style button (if shown)
            if (owner.ShowStyleButton)
            {
                layout.StyleButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("style", layout.StyleButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Theme button (if shown)
            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Custom action button (if theme/style not shown)
            if (!owner.ShowThemeButton && !owner.ShowStyleButton)
            {
                layout.CustomActionButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
            }
            
            int iconX = metrics.IconLeftPadding;
            int iconY = (captionHeight - metrics.IconSize) / 2;
            layout.IconRect = new Rectangle(iconX, iconY, metrics.IconSize, metrics.IconSize);
            if (owner.ShowIcon && owner.Icon != null)
            {
                owner._hits.Register("icon", layout.IconRect, HitAreaType.Icon);
            }
            
            int titleX = layout.IconRect.Right + metrics.TitleLeftPadding;
            int titleWidth = buttonX - titleX - metrics.ButtonSpacing;
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
