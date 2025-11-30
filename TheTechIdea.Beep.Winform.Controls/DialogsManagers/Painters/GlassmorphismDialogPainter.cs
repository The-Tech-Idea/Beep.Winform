using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Painters
{
    /// <summary>
    /// Glassmorphism dialog painter
    /// Features: Frosted glass effect, blur simulation, gradient borders, transparency
    /// </summary>
    public class GlassmorphismDialogPainter : DialogPainterBase
    {
        // Glassmorphism colors
        private static readonly Color GlassBackground = Color.FromArgb(180, 255, 255, 255);
        private static readonly Color GlassBorder = Color.FromArgb(100, 255, 255, 255);
        private static readonly Color TextPrimary = Color.FromArgb(30, 30, 60);
        private static readonly Color TextSecondary = Color.FromArgb(80, 80, 120);
        private static readonly Color AccentBlue = Color.FromArgb(99, 102, 241);
        private static readonly Color AccentGreen = Color.FromArgb(34, 197, 94);
        private static readonly Color AccentRed = Color.FromArgb(239, 68, 68);
        private static readonly Color AccentYellow = Color.FromArgb(234, 179, 8);

        private const int CORNER_RADIUS = 20;
        private const int PADDING = 28;
        private const int ICON_SIZE = 52;
        private const int BORDER_WIDTH = 2;

        public override void Paint(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Paint shadow/glow
            if (config.ShowShadow)
                PaintShadow(g, bounds, config);

            var contentBounds = new Rectangle(bounds.X + 6, bounds.Y + 6, bounds.Width - 12, bounds.Height - 12);

            PaintBackground(g, contentBounds, config, theme);
            PaintBorder(g, contentBounds, config, theme);

            var layout = CalculateLayout(contentBounds, config);

            if (config.ShowIcon && !layout.IconRect.IsEmpty)
                PaintIcon(g, layout.IconRect, config, theme);

            if (!string.IsNullOrEmpty(config.Title) && !layout.TitleRect.IsEmpty)
                PaintTitle(g, layout.TitleRect, config, theme);

            if (!string.IsNullOrEmpty(config.Message) && !layout.MessageRect.IsEmpty)
                PaintMessage(g, layout.MessageRect, config, theme);

            if (layout.ButtonRects != null && layout.ButtonRects.Length > 0)
                PaintButtons(g, layout.ButtonAreaRect, config, theme);
        }

        public override void PaintBackground(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme)
        {
            using var path = GraphicsExtensions.GetRoundedRectPath(bounds, CORNER_RADIUS);

            // Simulate frosted glass with semi-transparent white
            Color bgColor = config.BackColor ?? GlassBackground;

            // Base frosted layer
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillPath(brush, path);
            }

            // Noise texture simulation for blur effect
            var random = new Random(42); // Fixed seed for consistent pattern
            using (var noiseBrush = new SolidBrush(Color.FromArgb(8, 255, 255, 255)))
            {
                for (int i = 0; i < 100; i++)
                {
                    int x = bounds.X + random.Next(bounds.Width);
                    int y = bounds.Y + random.Next(bounds.Height);
                    g.FillEllipse(noiseBrush, x, y, 2, 2);
                }
            }

            // Top gradient highlight (glass reflection)
            var highlightRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 3);
            using (var highlightBrush = new LinearGradientBrush(
                highlightRect,
                Color.FromArgb(60, 255, 255, 255),
                Color.FromArgb(0, 255, 255, 255),
                90f))
            {
                using var highlightPath = GraphicsExtensions.GetRoundedRectPath(highlightRect, CORNER_RADIUS);
                g.FillPath(highlightBrush, highlightPath);
            }

            // Inner glow
            var innerBounds = new Rectangle(bounds.X + 4, bounds.Y + 4, bounds.Width - 8, bounds.Height - 8);
            using var innerPath = GraphicsExtensions.GetRoundedRectPath(innerBounds, CORNER_RADIUS - 2);
            using var innerPen = new Pen(Color.FromArgb(30, 255, 255, 255), 1);
            g.DrawPath(innerPen, innerPath);
        }

        public override void PaintBorder(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme)
        {
            using var path = GraphicsExtensions.GetRoundedRectPath(bounds, CORNER_RADIUS);

            // Gradient border (glassmorphism characteristic)
            using var borderBrush = new LinearGradientBrush(
                bounds,
                Color.FromArgb(120, 255, 255, 255),
                Color.FromArgb(40, 255, 255, 255),
                45f);

            using var borderPen = new Pen(borderBrush, BORDER_WIDTH);
            g.DrawPath(borderPen, path);

            // Outer glow based on preset
            Color glowColor = GetGlowColor(config.Preset, config.IconType);
            using var glowPen = new Pen(Color.FromArgb(30, glowColor), 3);
            var glowBounds = new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width + 2, bounds.Height + 2);
            using var glowPath = GraphicsExtensions.GetRoundedRectPath(glowBounds, CORNER_RADIUS + 1);
            g.DrawPath(glowPen, glowPath);
        }

        public override void PaintShadow(Graphics g, Rectangle bounds, DialogConfig config)
        {
            // Soft diffuse shadow with color tint
            Color shadowColor = GetGlowColor(config.Preset, config.IconType);
            int shadowLayers = 15;

            for (int i = shadowLayers; i > 0; i--)
            {
                int alpha = (int)(20.0f * (1.0f - (float)i / shadowLayers));
                var shadowRect = new Rectangle(
                    bounds.X - i,
                    bounds.Y + i / 2,
                    bounds.Width + i * 2,
                    bounds.Height + i * 2
                );

                using var path = GraphicsExtensions.GetRoundedRectPath(shadowRect, CORNER_RADIUS + i);
                using var brush = new SolidBrush(Color.FromArgb(alpha, shadowColor));
                g.FillPath(brush, path);
            }
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, DialogConfig config, IBeepTheme theme)
        {
            Color iconColor = GetAccentColor(config.IconType, config.Preset);

            // Glass icon container
            using (var containerPath = GraphicsExtensions.GetRoundedRectPath(iconRect, 16))
            {
                // Frosted background
                using (var brush = new SolidBrush(Color.FromArgb(100, iconColor)))
                {
                    g.FillPath(brush, containerPath);
                }

                // Glass border
                using (var borderBrush = new LinearGradientBrush(
                    iconRect,
                    Color.FromArgb(80, 255, 255, 255),
                    Color.FromArgb(20, 255, 255, 255),
                    45f))
                using (var pen = new Pen(borderBrush, 1))
                {
                    g.DrawPath(pen, containerPath);
                }
            }

            // Icon
            string iconPath = GetIconPath(config);
            var innerRect = new Rectangle(iconRect.X + 10, iconRect.Y + 10, iconRect.Width - 20, iconRect.Height - 20);

            if (!string.IsNullOrEmpty(iconPath))
            {
                try
                {
                    StyledImagePainter.PaintWithTint(g, innerRect, iconPath, Color.White, 1f, 4);
                }
                catch
                {
                    DrawGlassIcon(g, innerRect, config.IconType, Color.White);
                }
            }
            else
            {
                DrawGlassIcon(g, innerRect, config.IconType, Color.White);
            }
        }

        public override void PaintTitle(Graphics g, Rectangle titleRect, DialogConfig config, IBeepTheme theme)
        {
            var titleFont = config.TitleFont ?? new Font("Segoe UI Semibold", 16, FontStyle.Regular);
            using var brush = new SolidBrush(TextPrimary);
            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.EllipsisCharacter
            };
            g.DrawString(config.Title, titleFont, brush, titleRect, sf);
        }

        public override void PaintMessage(Graphics g, Rectangle messageRect, DialogConfig config, IBeepTheme theme)
        {
            var messageFont = config.MessageFont ?? new Font("Segoe UI", 11, FontStyle.Regular);
            using var brush = new SolidBrush(TextSecondary);
            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.Word
            };
            g.DrawString(config.Message, messageFont, brush, messageRect, sf);
        }

        public override void PaintButtons(Graphics g, Rectangle buttonBounds, DialogConfig config, IBeepTheme theme)
        {
            var layout = CalculateLayout(buttonBounds, config);
            if (layout.ButtonRects == null || layout.ButtonRects.Length == 0)
                return;

            var buttons = config.Buttons ?? new[] { BeepDialogButtons.Ok };
            var buttonFont = config.ButtonFont ?? new Font("Segoe UI Semibold", 10, FontStyle.Regular);

            for (int i = 0; i < Math.Min(buttons.Length, layout.ButtonRects.Length); i++)
            {
                bool isPrimary = i == buttons.Length - 1;
                PaintGlassButton(g, layout.ButtonRects[i], buttons[i], isPrimary, config, buttonFont);
            }
        }

        private void PaintGlassButton(Graphics g, Rectangle rect, BeepDialogButtons button, bool isPrimary, DialogConfig config, Font font)
        {
            int radius = 12;

            Color bgColor, fgColor;

            if (isPrimary)
            {
                bgColor = GetAccentColor(config.IconType, config.Preset);
                fgColor = Color.White;
            }
            else
            {
                bgColor = Color.FromArgb(100, 255, 255, 255);
                fgColor = TextPrimary;
            }

            using (var path = GraphicsExtensions.GetRoundedRectPath(rect, radius))
            {
                // Glass background
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Top highlight
                var highlightRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height / 2);
                using (var highlightBrush = new LinearGradientBrush(
                    highlightRect,
                    Color.FromArgb(isPrimary ? 60 : 80, 255, 255, 255),
                    Color.FromArgb(0, 255, 255, 255),
                    90f))
                {
                    using var highlightPath = GraphicsExtensions.GetRoundedRectPath(highlightRect, radius);
                    g.FillPath(highlightBrush, highlightPath);
                }

                // Border
                using (var borderBrush = new LinearGradientBrush(
                    rect,
                    Color.FromArgb(100, 255, 255, 255),
                    Color.FromArgb(30, 255, 255, 255),
                    45f))
                using (var pen = new Pen(borderBrush, 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Text
            string text = GetButtonText(button, config);
            using (var brush = new SolidBrush(fgColor))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(text, font, brush, rect, sf);
            }
        }

        private Color GetAccentColor(BeepDialogIcon icon, DialogPreset preset)
        {
            if (preset != DialogPreset.None)
            {
                return preset switch
                {
                    DialogPreset.Success => AccentGreen,
                    DialogPreset.Danger => AccentRed,
                    DialogPreset.Warning => AccentYellow,
                    _ => AccentBlue
                };
            }

            return icon switch
            {
                BeepDialogIcon.Success => AccentGreen,
                BeepDialogIcon.Error => AccentRed,
                BeepDialogIcon.Warning => AccentYellow,
                _ => AccentBlue
            };
        }

        private Color GetGlowColor(DialogPreset preset, BeepDialogIcon icon)
        {
            return GetAccentColor(icon, preset);
        }

        private void DrawGlassIcon(Graphics g, Rectangle rect, BeepDialogIcon icon, Color color)
        {
            using var pen = new Pen(color, 2.5f);
            var center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            int size = rect.Width / 3;

            switch (icon)
            {
                case BeepDialogIcon.Success:
                    var checkPoints = new Point[]
                    {
                        new Point(center.X - size, center.Y),
                        new Point(center.X - size / 4, center.Y + size * 2 / 3),
                        new Point(center.X + size, center.Y - size / 2)
                    };
                    g.DrawLines(pen, checkPoints);
                    break;

                case BeepDialogIcon.Error:
                    g.DrawLine(pen, center.X - size / 2, center.Y - size / 2, center.X + size / 2, center.Y + size / 2);
                    g.DrawLine(pen, center.X + size / 2, center.Y - size / 2, center.X - size / 2, center.Y + size / 2);
                    break;

                case BeepDialogIcon.Warning:
                    g.DrawLine(pen, center.X, center.Y - size / 2, center.X, center.Y + size / 4);
                    g.FillEllipse(new SolidBrush(color), center.X - 3, center.Y + size / 2, 6, 6);
                    break;

                default:
                    using (var font = new Font("Segoe UI", rect.Width / 2.5f, FontStyle.Bold))
                    using (var brush = new SolidBrush(color))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        string symbol = icon == BeepDialogIcon.Question ? "?" : "i";
                        g.DrawString(symbol, font, brush, rect, sf);
                    }
                    break;
            }
        }
    }
}

