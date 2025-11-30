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
    /// Fluent Design dialog painter (Windows 11 style)
    /// Features: Mica/Acrylic effect simulation, subtle shadows, 8px corners, reveal effects
    /// </summary>
    public class FluentDialogPainter : DialogPainterBase
    {
        // Fluent Design color tokens
        private static readonly Color SolidBackgroundFillColorBase = Color.FromArgb(243, 243, 243);
        private static readonly Color SolidBackgroundFillColorSecondary = Color.FromArgb(238, 238, 238);
        private static readonly Color TextFillColorPrimary = Color.FromArgb(0, 0, 0);
        private static readonly Color TextFillColorSecondary = Color.FromArgb(96, 96, 96);
        private static readonly Color AccentDefault = Color.FromArgb(0, 120, 212);
        private static readonly Color AccentLight = Color.FromArgb(96, 160, 224);
        private static readonly Color StrokeColorControlDefault = Color.FromArgb(0, 0, 0, 15);
        private static readonly Color StrokeColorControlOnAccent = Color.FromArgb(255, 255, 255, 20);
        private static readonly Color SubtleFillColorSecondary = Color.FromArgb(0, 0, 0, 10);
        private static readonly Color SystemFillColorCritical = Color.FromArgb(196, 43, 28);
        private static readonly Color SystemFillColorSuccess = Color.FromArgb(15, 123, 15);
        private static readonly Color SystemFillColorCaution = Color.FromArgb(157, 93, 0);
        private static readonly Color SystemFillColorAttention = Color.FromArgb(0, 99, 177);

        private const int CORNER_RADIUS = 8;
        private const int PADDING = 24;
        private const int ICON_SIZE = 40;

        public override void Paint(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Paint shadow
            if (config.ShowShadow)
                PaintShadow(g, bounds, config);

            var contentBounds = new Rectangle(bounds.X + 2, bounds.Y + 2, bounds.Width - 4, bounds.Height - 4);

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

            // Simulate Mica effect with layered backgrounds
            Color bgColor = config.BackColor ?? SolidBackgroundFillColorBase;

            // Base layer
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillPath(brush, path);
            }

            // Subtle noise texture simulation (horizontal gradient lines)
            using (var pen = new Pen(Color.FromArgb(3, 255, 255, 255), 1))
            {
                for (int y = bounds.Top + 2; y < bounds.Bottom - 2; y += 3)
                {
                    g.DrawLine(pen, bounds.Left + CORNER_RADIUS, y, bounds.Right - CORNER_RADIUS, y);
                }
            }

            // Top highlight (Mica characteristic)
            var highlightRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, 2);
            using (var highlightBrush = new LinearGradientBrush(
                highlightRect,
                Color.FromArgb(40, 255, 255, 255),
                Color.FromArgb(0, 255, 255, 255),
                90f))
            {
                g.FillRectangle(highlightBrush, highlightRect);
            }
        }

        public override void PaintBorder(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme)
        {
            using var path = GraphicsExtensions.GetRoundedRectPath(bounds, CORNER_RADIUS);

            // Fluent uses a very subtle border
            using var pen = new Pen(Color.FromArgb(20, 0, 0, 0), 1);
            g.DrawPath(pen, path);

            // Inner highlight
            var innerBounds = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2);
            using var innerPath = GraphicsExtensions.GetRoundedRectPath(innerBounds, CORNER_RADIUS - 1);
            using var innerPen = new Pen(Color.FromArgb(10, 255, 255, 255), 1);
            g.DrawPath(innerPen, innerPath);
        }

        public override void PaintShadow(Graphics g, Rectangle bounds, DialogConfig config)
        {
            // Fluent uses soft, diffuse shadows
            int shadowLayers = 8;

            for (int i = shadowLayers; i > 0; i--)
            {
                int alpha = (int)(25.0f * (1.0f - (float)i / shadowLayers));
                var shadowRect = new Rectangle(
                    bounds.X - i / 2,
                    bounds.Y + i / 2,
                    bounds.Width + i,
                    bounds.Height + i
                );

                using var path = GraphicsExtensions.GetRoundedRectPath(shadowRect, CORNER_RADIUS + i / 2);
                using var brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));
                g.FillPath(brush, path);
            }
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, DialogConfig config, IBeepTheme theme)
        {
            Color iconColor = GetSystemColor(config.IconType, config.Preset);

            // Fluent icons are typically flat, no container
            string iconPath = GetIconPath(config);
            if (!string.IsNullOrEmpty(iconPath))
            {
                try
                {
                    StyledImagePainter.PaintWithTint(g, iconRect, iconPath, iconColor, 1f, 4);
                }
                catch
                {
                    DrawFluentIcon(g, iconRect, config.IconType, iconColor);
                }
            }
            else
            {
                DrawFluentIcon(g, iconRect, config.IconType, iconColor);
            }
        }

        public override void PaintTitle(Graphics g, Rectangle titleRect, DialogConfig config, IBeepTheme theme)
        {
            // Fluent uses Segoe UI Variable (we'll use Segoe UI)
            var titleFont = config.TitleFont ?? new Font("Segoe UI Semibold", 14, FontStyle.Regular);
            using var brush = new SolidBrush(TextFillColorPrimary);
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
            var messageFont = config.MessageFont ?? new Font("Segoe UI", 10, FontStyle.Regular);
            using var brush = new SolidBrush(TextFillColorSecondary);
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
            var buttonFont = config.ButtonFont ?? new Font("Segoe UI", 10, FontStyle.Regular);

            for (int i = 0; i < Math.Min(buttons.Length, layout.ButtonRects.Length); i++)
            {
                bool isPrimary = i == buttons.Length - 1;
                PaintFluentButton(g, layout.ButtonRects[i], buttons[i], isPrimary, config, buttonFont);
            }
        }

        private void PaintFluentButton(Graphics g, Rectangle rect, BeepDialogButtons button, bool isPrimary, DialogConfig config, Font font)
        {
            int radius = 4; // Fluent uses smaller corner radius for buttons

            Color bgColor, fgColor, borderColor;

            if (isPrimary)
            {
                // Accent button
                bgColor = GetAccentColor(config.Preset);
                fgColor = Color.White;
                borderColor = Color.FromArgb(40, 255, 255, 255);
            }
            else
            {
                // Standard button
                bgColor = SolidBackgroundFillColorSecondary;
                fgColor = TextFillColorPrimary;
                borderColor = Color.FromArgb(15, 0, 0, 0);
            }

            using (var path = GraphicsExtensions.GetRoundedRectPath(rect, radius))
            {
                // Background
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Border
                using (var pen = new Pen(borderColor, 1))
                {
                    g.DrawPath(pen, path);
                }

                // Bottom border accent (Fluent characteristic)
                if (isPrimary)
                {
                    using var bottomPen = new Pen(Color.FromArgb(60, 0, 0, 0), 1);
                    g.DrawLine(bottomPen, rect.Left + radius, rect.Bottom - 1, rect.Right - radius, rect.Bottom - 1);
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

        private Color GetSystemColor(BeepDialogIcon icon, DialogPreset preset)
        {
            if (preset != DialogPreset.None)
            {
                return preset switch
                {
                    DialogPreset.Success => SystemFillColorSuccess,
                    DialogPreset.Danger => SystemFillColorCritical,
                    DialogPreset.Warning => SystemFillColorCaution,
                    _ => SystemFillColorAttention
                };
            }

            return icon switch
            {
                BeepDialogIcon.Success => SystemFillColorSuccess,
                BeepDialogIcon.Error => SystemFillColorCritical,
                BeepDialogIcon.Warning => SystemFillColorCaution,
                _ => SystemFillColorAttention
            };
        }

        private Color GetAccentColor(DialogPreset preset)
        {
            return preset switch
            {
                DialogPreset.Success => SystemFillColorSuccess,
                DialogPreset.Danger => SystemFillColorCritical,
                DialogPreset.Warning => SystemFillColorCaution,
                _ => AccentDefault
            };
        }

        private void DrawFluentIcon(Graphics g, Rectangle rect, BeepDialogIcon icon, Color color)
        {
            // Fluent Segoe MDL2 Assets style icons
            using var pen = new Pen(color, 2);
            var center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            int size = rect.Width / 3;

            switch (icon)
            {
                case BeepDialogIcon.Success:
                    // Checkmark
                    var checkPoints = new Point[]
                    {
                        new Point(center.X - size, center.Y),
                        new Point(center.X - size / 3, center.Y + size * 2 / 3),
                        new Point(center.X + size, center.Y - size / 2)
                    };
                    g.DrawLines(pen, checkPoints);
                    break;

                case BeepDialogIcon.Error:
                    // Circle with X
                    g.DrawEllipse(pen, rect.X + 4, rect.Y + 4, rect.Width - 8, rect.Height - 8);
                    g.DrawLine(pen, center.X - size / 2, center.Y - size / 2, center.X + size / 2, center.Y + size / 2);
                    g.DrawLine(pen, center.X + size / 2, center.Y - size / 2, center.X - size / 2, center.Y + size / 2);
                    break;

                case BeepDialogIcon.Warning:
                    // Triangle with exclamation
                    var trianglePoints = new Point[]
                    {
                        new Point(center.X, rect.Y + 4),
                        new Point(rect.X + 4, rect.Bottom - 4),
                        new Point(rect.Right - 4, rect.Bottom - 4)
                    };
                    g.DrawPolygon(pen, trianglePoints);
                    g.DrawLine(pen, center.X, center.Y - 4, center.X, center.Y + 4);
                    g.FillEllipse(new SolidBrush(color), center.X - 2, center.Y + 8, 4, 4);
                    break;

                case BeepDialogIcon.Question:
                    // Circle with question mark
                    g.DrawEllipse(pen, rect.X + 4, rect.Y + 4, rect.Width - 8, rect.Height - 8);
                    using (var font = new Font("Segoe UI", rect.Width / 3, FontStyle.Bold))
                    using (var brush = new SolidBrush(color))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString("?", font, brush, rect, sf);
                    }
                    break;

                default:
                    // Info circle
                    g.DrawEllipse(pen, rect.X + 4, rect.Y + 4, rect.Width - 8, rect.Height - 8);
                    using (var font = new Font("Segoe UI", rect.Width / 3, FontStyle.Bold))
                    using (var brush = new SolidBrush(color))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString("i", font, brush, rect, sf);
                    }
                    break;
            }
        }
    }
}

