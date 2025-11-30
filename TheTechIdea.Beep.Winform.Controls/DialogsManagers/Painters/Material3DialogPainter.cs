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
    /// Material Design 3 dialog painter
    /// Features: Large rounded corners (28px), tonal colors, elevated surfaces, prominent icons
    /// </summary>
    public class Material3DialogPainter : DialogPainterBase
    {
        // Material 3 color tokens
        private static readonly Color SurfaceContainer = Color.FromArgb(255, 251, 254);
        private static readonly Color SurfaceContainerHigh = Color.FromArgb(236, 230, 240);
        private static readonly Color OnSurface = Color.FromArgb(28, 27, 31);
        private static readonly Color OnSurfaceVariant = Color.FromArgb(73, 69, 79);
        private static readonly Color Outline = Color.FromArgb(121, 116, 126);
        private static readonly Color OutlineVariant = Color.FromArgb(202, 196, 208);
        private static readonly Color Primary = Color.FromArgb(103, 80, 164);
        private static readonly Color OnPrimary = Color.White;
        private static readonly Color PrimaryContainer = Color.FromArgb(234, 221, 255);
        private static readonly Color Error = Color.FromArgb(179, 38, 30);
        private static readonly Color ErrorContainer = Color.FromArgb(249, 222, 220);
        private static readonly Color Success = Color.FromArgb(56, 142, 60);
        private static readonly Color Warning = Color.FromArgb(237, 108, 2);

        private const int CORNER_RADIUS = 28;
        private const int PADDING = 24;
        private const int ICON_SIZE = 48;

        public override void Paint(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Paint shadow first
            if (config.ShowShadow)
                PaintShadow(g, bounds, config);

            // Adjust bounds for shadow
            var contentBounds = new Rectangle(bounds.X + 4, bounds.Y + 4, bounds.Width - 8, bounds.Height - 8);

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

            // Surface container background
            Color bgColor = config.BackColor ?? SurfaceContainer;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillPath(brush, path);
            }
        }

        public override void PaintBorder(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme)
        {
            // Material 3 dialogs typically don't have visible borders, just elevation
            // But we can add a subtle outline variant
            using var path = GraphicsExtensions.GetRoundedRectPath(bounds, CORNER_RADIUS);
            using var pen = new Pen(OutlineVariant, 1);
            g.DrawPath(pen, path);
        }

        public override void PaintShadow(Graphics g, Rectangle bounds, DialogConfig config)
        {
            // Material 3 elevation level 3 shadow (modal dialogs)
            int shadowLayers = 12;
            int offsetY = 6;

            for (int i = shadowLayers; i > 0; i--)
            {
                int alpha = (int)(40.0f * (1.0f - (float)i / shadowLayers));
                var shadowRect = new Rectangle(
                    bounds.X + 2 - i / 2,
                    bounds.Y + offsetY - i / 2,
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
            // Material 3 uses tonal icon containers
            Color iconContainerColor = GetIconContainerColor(config.IconType, config.Preset);
            Color iconColor = GetIconColor(config.IconType, config.Preset);

            // Draw circular container
            using (var brush = new SolidBrush(iconContainerColor))
            {
                g.FillEllipse(brush, iconRect);
            }

            // Draw icon
            string iconPath = GetIconPath(config);
            if (!string.IsNullOrEmpty(iconPath))
            {
                try
                {
                    var innerRect = new Rectangle(
                        iconRect.X + 8, iconRect.Y + 8,
                        iconRect.Width - 16, iconRect.Height - 16
                    );
                    StyledImagePainter.PaintWithTint(g, innerRect, iconPath, iconColor, 1f, 4);
                }
                catch
                {
                    // Fallback: draw symbol
                    DrawIconSymbol(g, iconRect, config.IconType, iconColor);
                }
            }
        }

        public override void PaintTitle(Graphics g, Rectangle titleRect, DialogConfig config, IBeepTheme theme)
        {
            var titleFont = config.TitleFont ?? new Font("Segoe UI", 16, FontStyle.Regular);
            using var brush = new SolidBrush(OnSurface);
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
            using var brush = new SolidBrush(OnSurfaceVariant);
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
            var buttonFont = config.ButtonFont ?? new Font("Segoe UI", 11, FontStyle.Regular);

            for (int i = 0; i < Math.Min(buttons.Length, layout.ButtonRects.Length); i++)
            {
                bool isPrimary = i == buttons.Length - 1;
                PaintMaterial3Button(g, layout.ButtonRects[i], buttons[i], isPrimary, config, buttonFont);
            }
        }

        private void PaintMaterial3Button(Graphics g, Rectangle rect, BeepDialogButtons button, bool isPrimary, DialogConfig config, Font font)
        {
            int radius = 20; // Material 3 full-rounded buttons

            Color bgColor, fgColor;

            if (isPrimary)
            {
                // Filled button
                bgColor = GetPrimaryColor(config.Preset);
                fgColor = OnPrimary;
            }
            else
            {
                // Text button (no background)
                bgColor = Color.Transparent;
                fgColor = Primary;
            }

            using (var path = GraphicsExtensions.GetRoundedRectPath(rect, radius))
            {
                if (bgColor != Color.Transparent)
                {
                    using var brush = new SolidBrush(bgColor);
                    g.FillPath(brush, path);
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

        private Color GetIconContainerColor(BeepDialogIcon icon, DialogPreset preset)
        {
            if (preset != DialogPreset.None)
            {
                return preset switch
                {
                    DialogPreset.Success => Color.FromArgb(200, 230, 201),
                    DialogPreset.Danger => ErrorContainer,
                    DialogPreset.Warning => Color.FromArgb(255, 243, 224),
                    _ => PrimaryContainer
                };
            }

            return icon switch
            {
                BeepDialogIcon.Success => Color.FromArgb(200, 230, 201),
                BeepDialogIcon.Error => ErrorContainer,
                BeepDialogIcon.Warning => Color.FromArgb(255, 243, 224),
                _ => PrimaryContainer
            };
        }

        private Color GetIconColor(BeepDialogIcon icon, DialogPreset preset)
        {
            if (preset != DialogPreset.None)
            {
                return preset switch
                {
                    DialogPreset.Success => Success,
                    DialogPreset.Danger => Error,
                    DialogPreset.Warning => Warning,
                    _ => Primary
                };
            }

            return icon switch
            {
                BeepDialogIcon.Success => Success,
                BeepDialogIcon.Error => Error,
                BeepDialogIcon.Warning => Warning,
                _ => Primary
            };
        }

        private Color GetPrimaryColor(DialogPreset preset)
        {
            return preset switch
            {
                DialogPreset.Success => Success,
                DialogPreset.Danger => Error,
                DialogPreset.Warning => Warning,
                _ => Primary
            };
        }

        private void DrawIconSymbol(Graphics g, Rectangle rect, BeepDialogIcon icon, Color color)
        {
            using var pen = new Pen(color, 3);
            var center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            int size = rect.Width / 3;

            switch (icon)
            {
                case BeepDialogIcon.Success:
                    // Checkmark
                    g.DrawLine(pen, center.X - size, center.Y, center.X - size / 3, center.Y + size / 2);
                    g.DrawLine(pen, center.X - size / 3, center.Y + size / 2, center.X + size, center.Y - size / 2);
                    break;
                case BeepDialogIcon.Error:
                    // X
                    g.DrawLine(pen, center.X - size / 2, center.Y - size / 2, center.X + size / 2, center.Y + size / 2);
                    g.DrawLine(pen, center.X + size / 2, center.Y - size / 2, center.X - size / 2, center.Y + size / 2);
                    break;
                case BeepDialogIcon.Warning:
                    // Exclamation
                    g.DrawLine(pen, center.X, center.Y - size / 2, center.X, center.Y + size / 4);
                    g.FillEllipse(new SolidBrush(color), center.X - 3, center.Y + size / 2, 6, 6);
                    break;
                case BeepDialogIcon.Question:
                    // Question mark
                    using (var font = new Font("Segoe UI", rect.Width / 3, FontStyle.Bold))
                    using (var brush = new SolidBrush(color))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString("?", font, brush, rect, sf);
                    }
                    break;
                default:
                    // Info - i
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

