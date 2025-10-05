using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Painters
{
    /// <summary>
    /// Step-by-step tooltip painter with progress indicators
    /// Perfect for tutorials, walkthroughs, and multi-step processes
    /// </summary>
    public class StepToolTipPainter : IToolTipPainter
    {
        public int CurrentStep { get; set; } = 1;
        public int TotalSteps { get; set; } = 1;
        public string StepTitle { get; set; } = "";
        public bool ShowNavigationButtons { get; set; } = true;

        public void Paint(Graphics g, Rectangle bounds, ToolTipConfig config, ToolTipPlacement actualPlacement, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (config.ShowShadow)
                PaintShadow(g, bounds, config, theme);

            PaintBackground(g, bounds, config, theme);
            PaintBorder(g, bounds, config, theme);
            PaintContent(g, bounds, config, theme);

            if (config.ShowArrow)
                PaintArrow(g, bounds, config, actualPlacement, theme);
        }

        public void PaintBackground(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            var (backColor, _, _) = ToolTipHelpers.GetThemeColors(theme, config.Theme);

            using (var path = CreateRoundedRectangle(bounds, 10))
            {
                // Clean solid background
                using (var brush = new SolidBrush(backColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        public void PaintBorder(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            var (_, _, borderColor) = ToolTipHelpers.GetThemeColors(theme, config.Theme);

            using (var path = CreateRoundedRectangle(bounds, 10))
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawPath(pen, path);
            }

            // Top accent bar (theme color)
            var accentColor = GetAccentColorForTheme(theme, config.Theme);
            using (var accentPath = CreateTopAccentPath(bounds, 10, 3))
            using (var accentBrush = new SolidBrush(accentColor))
            {
                g.FillPath(accentBrush, accentPath);
            }
        }

        public void PaintArrow(Graphics g, Rectangle bounds, ToolTipConfig config, ToolTipPlacement actualPlacement, IBeepTheme theme)
        {
            var (backColor, _, borderColor) = ToolTipHelpers.GetThemeColors(theme, config.Theme);
            var arrowPoints = ToolTipHelpers.CalculateArrowPosition(bounds, actualPlacement);

            if (arrowPoints.Length > 0)
            {
                using (var arrowPath = ToolTipHelpers.CreateArrowPath(arrowPoints))
                using (var brush = new SolidBrush(backColor))
                using (var pen = new Pen(borderColor, 1))
                {
                    g.FillPath(brush, arrowPath);
                    g.DrawPath(pen, arrowPath);
                }
            }
        }

        public void PaintContent(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            var (backColor, foreColor, _) = ToolTipHelpers.GetThemeColors(theme, config.Theme);
            var contentRect = new Rectangle(bounds.X + 16, bounds.Y + 16, bounds.Width - 32, bounds.Height - 32);

            int currentY = contentRect.Y;

            // Step indicator at top ("Step X of Y")
            currentY = DrawStepIndicator(g, contentRect, currentY, foreColor, theme);
            currentY += 12; // Gap

            // Progress dots
            currentY = DrawProgressDots(g, contentRect, currentY, theme, config);
            currentY += 16; // Gap

            // Icon (optional, left aligned)
            Rectangle? iconRect = null;
            int textX = contentRect.X;
            int textWidth = contentRect.Width;

            if (!string.IsNullOrEmpty(config.ImagePath) || !string.IsNullOrEmpty(config.IconPath) || config.Icon != null)
            {
                int iconSize = Math.Min(config.MaxImageSize.Width, 32);
                iconRect = new Rectangle(contentRect.X, currentY, iconSize, iconSize);
                DrawImageFromPath(g, iconRect.Value, config, theme);

                textX = iconRect.Value.Right + 12;
                textWidth = contentRect.Right - textX;
            }

            // Step title (bold, if provided)
            if (!string.IsNullOrEmpty(StepTitle))
            {
                using (var titleFont = new Font(config.Font?.FontFamily ?? FontFamily.GenericSansSerif, 10f, FontStyle.Bold))
                using (var brush = new SolidBrush(foreColor))
                {
                    var titleSize = g.MeasureString(StepTitle, titleFont, textWidth);
                    var titleRect = new Rectangle(textX, currentY, textWidth, (int)Math.Ceiling(titleSize.Height));
                    g.DrawString(StepTitle, titleFont, brush, titleRect, new StringFormat
                    {
                        LineAlignment = StringAlignment.Near,
                        Alignment = StringAlignment.Near,
                        Trimming = StringTrimming.EllipsisCharacter
                    });
                    currentY += (int)Math.Ceiling(titleSize.Height) + 6;
                }
            }

            // Main title (from config)
            if (!string.IsNullOrEmpty(config.Title) && config.Title != StepTitle)
            {
                using (var titleFont = new Font(config.Font?.FontFamily ?? FontFamily.GenericSansSerif, 9.5f, FontStyle.Bold))
                using (var brush = new SolidBrush(foreColor))
                {
                    var titleSize = g.MeasureString(config.Title, titleFont, textWidth);
                    var titleRect = new Rectangle(textX, currentY, textWidth, (int)Math.Ceiling(titleSize.Height));
                    g.DrawString(config.Title, titleFont, brush, titleRect, new StringFormat
                    {
                        LineAlignment = StringAlignment.Near,
                        Alignment = StringAlignment.Near,
                        Trimming = StringTrimming.EllipsisCharacter
                    });
                    currentY += (int)Math.Ceiling(titleSize.Height) + 6;
                }
            }

            // Description text
            if (!string.IsNullOrEmpty(config.Text))
            {
                using (var textFont = config.Font ?? new Font(FontFamily.GenericSansSerif, 9f))
                using (var brush = new SolidBrush(ColorUtils.ChangeColorBrightness(foreColor, -0.15f)))
                {
                    int remainingHeight = contentRect.Bottom - currentY - (ShowNavigationButtons ? 40 : 0);
                    var textRect = new Rectangle(textX, currentY, textWidth, remainingHeight);

                    g.DrawString(config.Text, textFont, brush, textRect, new StringFormat
                    {
                        LineAlignment = StringAlignment.Near,
                        Alignment = StringAlignment.Near,
                        Trimming = StringTrimming.EllipsisCharacter,
                        FormatFlags = StringFormatFlags.LineLimit
                    });
                }
            }

            // Navigation buttons at bottom
            if (ShowNavigationButtons)
            {
                DrawNavigationButtons(g, contentRect, foreColor, theme, config);
            }
        }

        public void PaintShadow(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            var shadowBounds = new Rectangle(bounds.X + 3, bounds.Y + 3, bounds.Width, bounds.Height);

            using (var path = CreateRoundedRectangle(shadowBounds, 10))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
            {
                g.FillPath(shadowBrush, path);
            }
        }

        public Size CalculateSize(ToolTipConfig config, IBeepTheme theme)
        {
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                int padding = 16;
                int width = 320;
                int height = padding * 2;

                // Step indicator
                height += 18;
                height += 12; // Gap

                // Progress dots
                height += 12;
                height += 16; // Gap

                // Icon space
                int iconSpace = 0;
                if (!string.IsNullOrEmpty(config.ImagePath) || !string.IsNullOrEmpty(config.IconPath) || config.Icon != null)
                {
                    iconSpace = Math.Min(config.MaxImageSize.Width, 32);
                }

                // Step title
                if (!string.IsNullOrEmpty(StepTitle))
                {
                    using (var font = new Font(config.Font?.FontFamily ?? FontFamily.GenericSansSerif, 10f, FontStyle.Bold))
                    {
                        var size = g.MeasureString(StepTitle, font, width - padding * 2 - iconSpace);
                        height += (int)Math.Ceiling(size.Height) + 6;
                    }
                }

                // Main title
                if (!string.IsNullOrEmpty(config.Title))
                {
                    using (var font = new Font(config.Font?.FontFamily ?? FontFamily.GenericSansSerif, 9.5f, FontStyle.Bold))
                    {
                        var size = g.MeasureString(config.Title, font, width - padding * 2 - iconSpace);
                        height += (int)Math.Ceiling(size.Height) + 6;
                    }
                }

                // Text
                if (!string.IsNullOrEmpty(config.Text))
                {
                    using (var font = config.Font ?? new Font(FontFamily.GenericSansSerif, 9f))
                    {
                        var size = g.MeasureString(config.Text, font, width - padding * 2 - iconSpace);
                        height += (int)Math.Ceiling(size.Height);
                    }
                }

                // Navigation buttons
                if (ShowNavigationButtons)
                {
                    height += 40;
                }

                // Clamp to max size
                if (config.MaxSize.Width > 0)
                    width = Math.Min(width, config.MaxSize.Width);
                if (config.MaxSize.Height > 0)
                    height = Math.Min(height, config.MaxSize.Height);

                return new Size(width, height);
            }
        }

        #region Helper Methods

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private GraphicsPath CreateTopAccentPath(Rectangle rect, int cornerRadius, int accentHeight)
        {
            var path = new GraphicsPath();
            int diameter = cornerRadius * 2;

            // Top-left arc
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            // Top line
            path.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius, rect.Y);
            // Top-right arc
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            // Right side of accent
            path.AddLine(rect.Right, rect.Y + cornerRadius, rect.Right, rect.Y + accentHeight);
            // Bottom line
            path.AddLine(rect.Right, rect.Y + accentHeight, rect.X, rect.Y + accentHeight);
            // Left side of accent
            path.AddLine(rect.X, rect.Y + accentHeight, rect.X, rect.Y + cornerRadius);
            path.CloseFigure();

            return path;
        }

        private Color GetAccentColorForTheme(IBeepTheme theme, ToolTipTheme tooltipTheme)
        {
            return tooltipTheme switch
            {
                ToolTipTheme.Primary => theme.AccentColor,
                ToolTipTheme.Success => theme.SuccessColor,
                ToolTipTheme.Warning => theme.WarningColor,
                ToolTipTheme.Error => theme.ErrorColor,
                ToolTipTheme.Info => theme.InfoColor,
                _ => theme.AccentColor
            };
        }

        private int DrawStepIndicator(Graphics g, Rectangle contentRect, int y, Color foreColor, IBeepTheme theme)
        {
            string stepText = $"Step {CurrentStep} of {TotalSteps}";
            var accentColor = GetAccentColorForTheme(theme, ToolTipTheme.Primary);

            using (var font = new Font(FontFamily.GenericSansSerif, 8.5f, FontStyle.Bold))
            using (var brush = new SolidBrush(accentColor))
            {
                var size = g.MeasureString(stepText, font);
                var rect = new Rectangle(contentRect.X, y, (int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
                g.DrawString(stepText, font, brush, rect);

                return y + (int)Math.Ceiling(size.Height);
            }
        }

        private int DrawProgressDots(Graphics g, Rectangle contentRect, int y, IBeepTheme theme, ToolTipConfig config)
        {
            var accentColor = GetAccentColorForTheme(theme, config.Theme);
            var inactiveColor = Color.FromArgb(80, accentColor);

            int dotSize = 8;
            int dotSpacing = 6;
            int totalWidth = (TotalSteps * dotSize) + ((TotalSteps - 1) * dotSpacing);
            int startX = contentRect.X + (contentRect.Width - totalWidth) / 2;

            for (int i = 1; i <= TotalSteps; i++)
            {
                int x = startX + ((i - 1) * (dotSize + dotSpacing));
                var dotRect = new Rectangle(x, y, dotSize, dotSize);

                using (var brush = new SolidBrush(i == CurrentStep ? accentColor : inactiveColor))
                {
                    g.FillEllipse(brush, dotRect);
                }

                // Active step ring
                if (i == CurrentStep)
                {
                    using (var pen = new Pen(accentColor, 2))
                    {
                        var ringRect = new Rectangle(x - 2, y - 2, dotSize + 4, dotSize + 4);
                        g.DrawEllipse(pen, ringRect);
                    }
                }
            }

            return y + dotSize;
        }

        private void DrawImageFromPath(Graphics g, Rectangle iconRect, ToolTipConfig config, IBeepTheme theme)
        {
            string imagePath = config.ImagePath ?? config.IconPath;

            if (!string.IsNullOrEmpty(imagePath))
            {
                using (var painter = new ImagePainter(imagePath, theme))
                {
                    if (config.ApplyThemeOnImage)
                        painter.ApplyThemeToSvg();

                    painter.DrawImage(g, iconRect);
                }
            }
            else if (config.Icon != null)
            {
                g.DrawImage(config.Icon, iconRect);
            }
        }

        private void DrawNavigationButtons(Graphics g, Rectangle contentRect, Color foreColor, IBeepTheme theme, ToolTipConfig config)
        {
            int buttonHeight = 32;
            int buttonY = contentRect.Bottom - buttonHeight;
            int buttonWidth = 70;
            int buttonSpacing = 8;

            var accentColor = GetAccentColorForTheme(theme, config.Theme);

            // "Next" button (or "Finish" on last step)
            string nextText = CurrentStep >= TotalSteps ? "Finish" : "Next";
            var nextRect = new Rectangle(
                contentRect.Right - buttonWidth,
                buttonY,
                buttonWidth,
                buttonHeight);

            DrawButton(g, nextRect, nextText, accentColor, true);

            // "Previous" button (if not first step)
            if (CurrentStep > 1)
            {
                var prevRect = new Rectangle(
                    nextRect.X - buttonWidth - buttonSpacing,
                    buttonY,
                    buttonWidth,
                    buttonHeight);

                DrawButton(g, prevRect, "Previous", foreColor, false);
            }

            // "Skip" button (if not last step)
            if (CurrentStep < TotalSteps)
            {
                using (var font = new Font(FontFamily.GenericSansSerif, 8f, FontStyle.Underline))
                using (var brush = new SolidBrush(ColorUtils.ChangeColorBrightness(foreColor, -0.3f)))
                {
                    var skipText = "Skip";
                    var skipSize = g.MeasureString(skipText, font);
                    var skipRect = new Rectangle(
                        contentRect.X,
                        buttonY + (buttonHeight - (int)skipSize.Height) / 2,
                        (int)skipSize.Width,
                        (int)skipSize.Height);

                    g.DrawString(skipText, font, brush, skipRect);
                }
            }
        }

        private void DrawButton(Graphics g, Rectangle rect, string text, Color color, bool isPrimary)
        {
            using (var path = CreateRoundedRectangle(rect, 6))
            {
                if (isPrimary)
                {
                    using (var brush = new SolidBrush(color))
                    {
                        g.FillPath(brush, path);
                    }

                    using (var font = new Font(FontFamily.GenericSansSerif, 8.5f, FontStyle.Bold))
                    using (var brush = new SolidBrush(Color.White))
                    {
                        var sf = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
                        g.DrawString(text, font, brush, rect, sf);
                    }
                }
                else
                {
                    using (var pen = new Pen(ColorUtils.ChangeColorBrightness(color, -0.3f), 1))
                    {
                        g.DrawPath(pen, path);
                    }

                    using (var font = new Font(FontFamily.GenericSansSerif, 8.5f, FontStyle.Regular))
                    using (var brush = new SolidBrush(ColorUtils.ChangeColorBrightness(color, -0.2f)))
                    {
                        var sf = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
                        g.DrawString(text, font, brush, rect, sf);
                    }
                }
            }
        }

        #endregion
    }
}
