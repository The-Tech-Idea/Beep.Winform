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
    /// Neumorphism (Soft UI) dialog painter
    /// Features: Soft shadows, embossed elements, monochromatic palette, subtle depth
    /// </summary>
    public class NeumorphismDialogPainter : DialogPainterBase
    {
        // Neumorphism base colors
        private static readonly Color BaseColor = Color.FromArgb(224, 229, 236);
        private static readonly Color LightShadow = Color.FromArgb(255, 255, 255);
        private static readonly Color DarkShadow = Color.FromArgb(163, 177, 198);
        private static readonly Color TextPrimary = Color.FromArgb(55, 65, 81);
        private static readonly Color TextSecondary = Color.FromArgb(107, 114, 128);
        private static readonly Color AccentBlue = Color.FromArgb(79, 70, 229);
        private static readonly Color AccentGreen = Color.FromArgb(16, 185, 129);
        private static readonly Color AccentRed = Color.FromArgb(239, 68, 68);
        private static readonly Color AccentYellow = Color.FromArgb(245, 158, 11);

        private const int CORNER_RADIUS = 24;
        private const int PADDING = 28;
        private const int ICON_SIZE = 56;
        private const int SHADOW_OFFSET = 8;
        private const int SHADOW_BLUR = 16;

        public override void Paint(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Neumorphism uses shadows as the main visual element
            var contentBounds = new Rectangle(
                bounds.X + SHADOW_BLUR,
                bounds.Y + SHADOW_BLUR,
                bounds.Width - SHADOW_BLUR * 2,
                bounds.Height - SHADOW_BLUR * 2
            );

            // Paint shadows (convex/raised style)
            if (config.ShowShadow)
                PaintShadow(g, contentBounds, config);

            PaintBackground(g, contentBounds, config, theme);

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

            // Solid base color (neumorphism uses flat backgrounds)
            Color bgColor = config.BackColor ?? BaseColor;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillPath(brush, path);
            }
        }

        public override void PaintBorder(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme)
        {
            // Neumorphism typically doesn't use visible borders
            // The shadow creates the edge definition
        }

        public override void PaintShadow(Graphics g, Rectangle bounds, DialogConfig config)
        {
            // Convex (raised) neumorphic shadow
            // Light shadow (top-left)
            for (int i = SHADOW_BLUR; i > 0; i--)
            {
                int alpha = (int)(80.0f * (1.0f - (float)i / SHADOW_BLUR));
                var lightRect = new Rectangle(
                    bounds.X - i,
                    bounds.Y - i,
                    bounds.Width,
                    bounds.Height
                );

                using var path = GraphicsExtensions.GetRoundedRectPath(lightRect, CORNER_RADIUS);
                using var brush = new SolidBrush(Color.FromArgb(alpha, LightShadow));
                g.FillPath(brush, path);
            }

            // Dark shadow (bottom-right)
            for (int i = SHADOW_BLUR; i > 0; i--)
            {
                int alpha = (int)(60.0f * (1.0f - (float)i / SHADOW_BLUR));
                var darkRect = new Rectangle(
                    bounds.X + i,
                    bounds.Y + i,
                    bounds.Width,
                    bounds.Height
                );

                using var path = GraphicsExtensions.GetRoundedRectPath(darkRect, CORNER_RADIUS);
                using var brush = new SolidBrush(Color.FromArgb(alpha, DarkShadow));
                g.FillPath(brush, path);
            }
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, DialogConfig config, IBeepTheme theme)
        {
            Color accentColor = GetAccentColor(config.IconType, config.Preset);

            // Neumorphic inset icon container
            PaintNeumorphicInset(g, iconRect, 20);

            // Accent circle inside
            var innerRect = new Rectangle(iconRect.X + 6, iconRect.Y + 6, iconRect.Width - 12, iconRect.Height - 12);
            using (var brush = new SolidBrush(accentColor))
            {
                g.FillEllipse(brush, innerRect);
            }

            // Icon
            string iconPath = GetIconPath(config);
            var iconInnerRect = new Rectangle(innerRect.X + 8, innerRect.Y + 8, innerRect.Width - 16, innerRect.Height - 16);

            if (!string.IsNullOrEmpty(iconPath))
            {
                try
                {
                    StyledImagePainter.PaintWithTint(g, iconInnerRect, iconPath, Color.White, 1f, 4);
                }
                catch
                {
                    DrawNeumorphicIcon(g, iconInnerRect, config.IconType, Color.White);
                }
            }
            else
            {
                DrawNeumorphicIcon(g, iconInnerRect, config.IconType, Color.White);
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
                PaintNeumorphicButton(g, layout.ButtonRects[i], buttons[i], isPrimary, config, buttonFont);
            }
        }

        private void PaintNeumorphicButton(Graphics g, Rectangle rect, BeepDialogButtons button, bool isPrimary, DialogConfig config, Font font)
        {
            int radius = 16;
            Color accentColor = GetAccentColor(config.IconType, config.Preset);

            if (isPrimary)
            {
                // Raised primary button with accent color
                PaintNeumorphicRaised(g, rect, radius, accentColor);

                // Text
                string text = GetButtonText(button, config);
                using (var brush = new SolidBrush(Color.White))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(text, font, brush, rect, sf);
                }
            }
            else
            {
                // Raised secondary button with base color
                PaintNeumorphicRaised(g, rect, radius, BaseColor);

                // Text
                string text = GetButtonText(button, config);
                using (var brush = new SolidBrush(TextPrimary))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(text, font, brush, rect, sf);
                }
            }
        }

        private void PaintNeumorphicRaised(Graphics g, Rectangle rect, int radius, Color baseColor)
        {
            int shadowSize = 4;

            // Light shadow (top-left)
            for (int i = shadowSize; i > 0; i--)
            {
                int alpha = (int)(60.0f * (1.0f - (float)i / shadowSize));
                var lightRect = new Rectangle(rect.X - i, rect.Y - i, rect.Width, rect.Height);
                using var path = GraphicsExtensions.GetRoundedRectPath(lightRect, radius);
                using var brush = new SolidBrush(Color.FromArgb(alpha, LightShadow));
                g.FillPath(brush, path);
            }

            // Dark shadow (bottom-right)
            for (int i = shadowSize; i > 0; i--)
            {
                int alpha = (int)(40.0f * (1.0f - (float)i / shadowSize));
                var darkRect = new Rectangle(rect.X + i, rect.Y + i, rect.Width, rect.Height);
                using var path = GraphicsExtensions.GetRoundedRectPath(darkRect, radius);
                using var brush = new SolidBrush(Color.FromArgb(alpha, DarkShadow));
                g.FillPath(brush, path);
            }

            // Base
            using (var path = GraphicsExtensions.GetRoundedRectPath(rect, radius))
            using (var brush = new SolidBrush(baseColor))
            {
                g.FillPath(brush, path);
            }

            // Inner highlight gradient
            var highlightRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height / 2);
            using (var highlightBrush = new LinearGradientBrush(
                highlightRect,
                Color.FromArgb(30, 255, 255, 255),
                Color.FromArgb(0, 255, 255, 255),
                90f))
            {
                using var highlightPath = GraphicsExtensions.GetRoundedRectPath(highlightRect, radius);
                g.FillPath(highlightBrush, highlightPath);
            }
        }

        private void PaintNeumorphicInset(Graphics g, Rectangle rect, int radius)
        {
            int shadowSize = 4;

            // Inset shadows are reversed - dark on top-left, light on bottom-right

            // Dark shadow (top-left, inside)
            for (int i = shadowSize; i > 0; i--)
            {
                int alpha = (int)(40.0f * (1.0f - (float)i / shadowSize));
                var darkRect = new Rectangle(rect.X + (shadowSize - i), rect.Y + (shadowSize - i), rect.Width - (shadowSize - i) * 2, rect.Height - (shadowSize - i) * 2);
                using var path = GraphicsExtensions.GetRoundedRectPath(darkRect, radius - (shadowSize - i));
                using var pen = new Pen(Color.FromArgb(alpha, DarkShadow), 1);
                g.DrawPath(pen, path);
            }

            // Base inset
            using (var path = GraphicsExtensions.GetRoundedRectPath(rect, radius))
            using (var brush = new SolidBrush(Color.FromArgb(210, 215, 225)))
            {
                g.FillPath(brush, path);
            }

            // Light shadow (bottom-right, inside)
            for (int i = shadowSize; i > 0; i--)
            {
                int alpha = (int)(50.0f * (1.0f - (float)i / shadowSize));
                var lightRect = new Rectangle(rect.X + i, rect.Y + i, rect.Width - i * 2, rect.Height - i * 2);
                using var path = GraphicsExtensions.GetRoundedRectPath(lightRect, radius - i);
                using var pen = new Pen(Color.FromArgb(alpha, LightShadow), 1);
                g.DrawPath(pen, path);
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

        private void DrawNeumorphicIcon(Graphics g, Rectangle rect, BeepDialogIcon icon, Color color)
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

