using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Terminal/Console Style form painter with monospace aesthetic and minimal UI.
    /// Features sharp edges, monochrome colors, and text-focused design.
    /// </summary>
    internal sealed class TerminalFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Terminal, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            
            // Terminal: Solid dark background (black or dark gray)
            g.SmoothingMode = SmoothingMode.None; // Pixel-perfect, no anti-aliasing
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
            
            // Optional: Subtle scanline effect for CRT monitor aesthetic
            using (var scanLinePen = new Pen(Color.FromArgb(8, 255, 255, 255), 1))
            {
                for (int y = 0; y < owner.ClientRectangle.Height; y += 2)
                {
                    g.DrawLine(scanLinePen, 0, y, owner.ClientRectangle.Width, y);
                }
            }
            
            // Optional: Terminal grid pattern (very subtle)
            using (var gridPen = new Pen(Color.FromArgb(5, 0, 255, 0), 1))
            {
                int gridSize = 20;
                for (int x = 0; x < owner.ClientRectangle.Width; x += gridSize)
                {
                    g.DrawLine(gridPen, x, 0, x, owner.ClientRectangle.Height);
                }
                for (int y = 0; y < owner.ClientRectangle.Height; y += gridSize)
                {
                    g.DrawLine(gridPen, 0, y, owner.ClientRectangle.Width, y);
                }
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            
            // Terminal: Minimal flat caption bar
            g.SmoothingMode = SmoothingMode.None;
            using var capBrush = new SolidBrush(metrics.CaptionColor);
            g.FillRectangle(capBrush, captionRect);
            
            // Terminal prompt-Style bottom border with ASCII characters
            using (var borderPen = new Pen(Color.FromArgb(0, 255, 0), 2)) // Classic green terminal color
            {
                g.DrawLine(borderPen, captionRect.Left, captionRect.Bottom - 1, 
                    captionRect.Right, captionRect.Bottom - 1);
            }
            
            // Paint Terminal ASCII-Style buttons (UNIQUE SKIN)
            PaintTerminalButtons(g, owner, captionRect, metrics);

            // Draw title text with terminal font Style
            var textRect = owner.CurrentLayout.TitleRect;
            
            // Use monospace font if possible
            Font terminalFont = owner.Font;
            try
            {
                terminalFont = new Font("Consolas", owner.Font.Size, FontStyle.Bold);
            }
            catch
            {
                try
                {
                    terminalFont = new Font("Courier New", owner.Font.Size, FontStyle.Bold);
                }
                catch
                {
                    terminalFont = owner.Font;
                }
            }
            
            // Add terminal prompt prefix to title
            string terminalTitle = $"> {owner.Text ?? string.Empty}";
            TextRenderer.DrawText(g, terminalTitle, terminalFont, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom Terminal ASCII buttons
            // Only paint the icon if it exists
            if (owner.ShowIcon && owner.Icon != null)
            {
                owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
            }
            
            if (terminalFont != owner.Font)
            {
                terminalFont.Dispose();
            }
        }

        /// <summary>
        /// Paint Terminal ASCII-Style buttons (UNIQUE SKIN)
        /// Features: ASCII characters, monochrome, pixelated rectangles
        /// </summary>
        private void PaintTerminalButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;

            int buttonSize = 18;
            int padding = (captionRect.Height - buttonSize) / 2;

            // Close button: Red ASCII [X]
            PaintTerminalButton(g, closeRect, Color.FromArgb(255, 80, 80), padding, buttonSize, "close");

            // Maximize button: Green ASCII [□]
            PaintTerminalButton(g, maxRect, Color.FromArgb(80, 255, 80), padding, buttonSize, "maximize");

            // Minimize button: Yellow ASCII [-]
            PaintTerminalButton(g, minRect, Color.FromArgb(255, 255, 80), padding, buttonSize, "minimize");

            // Theme/Style buttons if shown
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintTerminalButton(g, styleRect, Color.FromArgb(128, 128, 255), padding, buttonSize, "Style");
            }

            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintTerminalButton(g, themeRect, Color.FromArgb(255, 128, 255), padding, buttonSize, "theme");
            }
        }

        private void PaintTerminalButton(Graphics g, Rectangle buttonRect, Color baseColor, int padding, int size, string buttonType)
        {
            int centerX = buttonRect.X + buttonRect.Width / 2;
            int centerY = buttonRect.Y + buttonRect.Height / 2;
            var rect = new Rectangle(centerX - size / 2, centerY - size / 2, size, size);

            g.SmoothingMode = SmoothingMode.None; // Pixel-perfect

            // Terminal: Simple solid rectangle background
            using (var bgBrush = new SolidBrush(Color.FromArgb(40, baseColor)))
            {
                g.FillRectangle(bgBrush, rect);
            }

            // ASCII-Style border (double-line effect)
            using (var borderPen = new Pen(baseColor, 2))
            {
                g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width, rect.Height);
            }

            // Inner border for double-line effect
            using (var innerPen = new Pen(Color.FromArgb(180, baseColor), 1))
            {
                g.DrawRectangle(innerPen, rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4);
            }

            // Draw terminal-Style icons using GDI+ primitives instead of Unicode
            // FIXED: Was using Unicode characters that might not render properly
            using (var iconPen = new Pen(baseColor, 2f))
            {
                int iconSize = 8;
                 centerX = rect.X + rect.Width / 2;
                 centerY = rect.Y + rect.Height / 2;

                switch (buttonType)
                {
                    case "close":
                        // X icon
                        g.DrawLine(iconPen, centerX - iconSize / 2, centerY - iconSize / 2,
                            centerX + iconSize / 2, centerY + iconSize / 2);
                        g.DrawLine(iconPen, centerX + iconSize / 2, centerY - iconSize / 2,
                            centerX - iconSize / 2, centerY + iconSize / 2);
                        break;
                        
                    case "maximize":
                        // Square icon (was Unicode □)
                        g.DrawRectangle(iconPen, centerX - iconSize / 2, centerY - iconSize / 2, iconSize, iconSize);
                        break;
                        
                    case "minimize":
                        // Underscore/line icon
                        g.DrawLine(iconPen, centerX - iconSize / 2, centerY, centerX + iconSize / 2, centerY);
                        break;
                        
                    case "Style":
                        // Palette icon (was Unicode ◘) - circle with dots
                        g.DrawEllipse(iconPen, centerX - iconSize / 2, centerY - iconSize / 2, iconSize, iconSize);
                        using (var dotBrush = new SolidBrush(baseColor))
                        {
                            g.FillEllipse(dotBrush, centerX - 2, centerY - 2, 2, 2);
                            g.FillEllipse(dotBrush, centerX + 1, centerY - 1, 2, 2);
                        }
                        break;
                        
                    case "theme":
                        // Sun/brightness icon (was Unicode ☼) - circle with rays
                        g.DrawEllipse(iconPen, centerX - iconSize / 3, centerY - iconSize / 3, 
                            iconSize * 2 / 3, iconSize * 2 / 3);
                        // Draw 8 rays around the circle
                        for (int i = 0; i < 8; i++)
                        {
                            double angle = Math.PI * 2 * i / 8;
                            int innerX = centerX + (int)(Math.Cos(angle) * iconSize / 3);
                            int innerY = centerY + (int)(Math.Sin(angle) * iconSize / 3);
                            int outerX = centerX + (int)(Math.Cos(angle) * iconSize / 2);
                            int outerY = centerY + (int)(Math.Sin(angle) * iconSize / 2);
                            g.DrawLine(iconPen, innerX, innerY, outerX, outerY);
                        }
                        break;
                }
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);

            // Terminal: Single-line sharp border (no anti-aliasing)
            g.SmoothingMode = SmoothingMode.None;

            var rect = owner.ClientRectangle;

            // Outer border - terminal green or specified border color
            using (var borderPen = new Pen(metrics.BorderColor, 2))
            {
                g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }

            // Draw terminal ASCII corner characters using GDI+ primitives
            // FIXED: Was using Unicode box-drawing characters that might not render properly
            using (var cornerPen = new Pen(metrics.BorderColor, 2))
            {
                int cornerSize = 8;
                
                // Top-left corner (was ┌)
                g.DrawLine(cornerPen, rect.Left, rect.Top + cornerSize, rect.Left, rect.Top);
                g.DrawLine(cornerPen, rect.Left, rect.Top, rect.Left + cornerSize, rect.Top);
                
                // Top-right corner (was ┐)
                g.DrawLine(cornerPen, rect.Right - cornerSize, rect.Top, rect.Right, rect.Top);
                g.DrawLine(cornerPen, rect.Right, rect.Top, rect.Right, rect.Top + cornerSize);
                
                // Bottom-left corner (was └)
                g.DrawLine(cornerPen, rect.Left, rect.Bottom - cornerSize, rect.Left, rect.Bottom);
                g.DrawLine(cornerPen, rect.Left, rect.Bottom, rect.Left + cornerSize, rect.Bottom);
                
                // Bottom-right corner (was ┘)
                g.DrawLine(cornerPen, rect.Right - cornerSize, rect.Bottom, rect.Right, rect.Bottom);
                g.DrawLine(cornerPen, rect.Right, rect.Bottom - cornerSize, rect.Right, rect.Bottom);
            }
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            // Terminal: No shadow (flat design)
            return new ShadowEffect
            {
                Color = Color.FromArgb(0, 0, 0, 0),
                Blur = 0,
                OffsetX = 0,
                OffsetY = 0,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(0); // Sharp corners (no rounding)
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.None; // Pixel-perfect rendering
        }

        public bool SupportsAnimations => false;

        public void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            var originalClip = g.Clip;
            
            // Terminal: No shadow, just direct painting
            PaintBackground(g, owner);

            // No clipping needed for terminal Style (sharp edges)
            
            PaintBorders(g, owner);
            if (owner.ShowCaptionBar)
            {
                PaintCaption(g, owner, owner.CurrentLayout.CaptionRect);
            }

            g.Clip = originalClip;
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, CornerRadius radius)
        {
            var path = new GraphicsPath();
            // Terminal uses sharp corners, so just add rectangle
            path.AddRectangle(rect);
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
                owner._hits.RegisterHitArea("Style", layout.StyleButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Theme button (if shown)
            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
                owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
                buttonX -= buttonWidth;
            }
            
            // Custom action button (fallback)
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
            var outer = new Rectangle(0, 0, owner.Width, owner.Height);
            
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, borderThickness));
            g.SmoothingMode = SmoothingMode.None;
            g.DrawRectangle(pen, outer.X, outer.Y, outer.Width - 1, outer.Height - 1);
        }
    }
}
