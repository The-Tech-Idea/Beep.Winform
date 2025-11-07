using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Painters
{
    /// <summary>
    /// Modern dialog painter with full BeepStyling and StyledImagePainter integration
    /// Supports all 20+ BeepControlStyle designs
    /// </summary>
    public class BeepStyledDialogPainter : DialogPainterBase
    {
        #region Main Paint Method

        public override void Paint(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme)
        {
            if (config == null || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            // Set high quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Paint in order: Shadow -> Background -> Border -> Content -> Buttons
            if (config.ShowShadow || config.EnableShadow)
            {
                PaintShadow(g, bounds, config);
            }

            PaintBackground(g, bounds, config, theme);
            PaintBorder(g, bounds, config, theme);

            var layout = CalculateLayout(bounds, config);

            if (config.ShowIcon && !layout.IconRect.IsEmpty)
            {
                PaintIcon(g, layout.IconRect, config, theme);
            }

            if (!string.IsNullOrEmpty(config.Title) && !layout.TitleRect.IsEmpty)
            {
                PaintTitle(g, layout.TitleRect, config, theme);
            }

            if (!string.IsNullOrEmpty(config.Message) && !layout.MessageRect.IsEmpty)
            {
                PaintMessage(g, layout.MessageRect, config, theme);
            }

            if (!string.IsNullOrEmpty(config.Details) && !layout.DetailsRect.IsEmpty)
            {
                PaintDetails(g, layout.DetailsRect, config, theme);
            }

            if (layout.ButtonRects != null && layout.ButtonRects.Length > 0)
            {
                PaintButtons(g, layout.ButtonAreaRect, config, theme);
            }

            if (config.ShowCloseButton && !layout.CloseButtonRect.IsEmpty)
            {
                PaintCloseButton(g, layout.CloseButtonRect, config, theme);
            }
        }

        #endregion

        #region Background Painting

        /// <summary>
        /// Paint background using BeepStyling system
        /// </summary>
        public override void PaintBackground(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme)
        {
            var beepStyle = DialogStyleAdapter.GetBeepControlStyle(config);
            var colors = DialogStyleAdapter.GetColors(config, theme);

            int radius = StyleBorders.GetRadius(beepStyle);

            using (var path = CreateRoundedRectangle(bounds, radius))
            {
                // Priority 1: Custom background color
                if (config.BackColor.HasValue)
                {
                    using (var brush = new SolidBrush(config.BackColor.Value))
                    {
                        g.FillPath(brush, path);
                    }
                    return;
                }

                // Priority 2: Use BeepStyling with theme colors
                if (config.UseBeepThemeColors && theme != null)
                {
                    var savedTheme = BeepStyling.CurrentTheme;
                    var savedUseTheme = BeepStyling.UseThemeColors;
                    var savedStyle = BeepStyling.CurrentControlStyle;

                    try
                    {
                        BeepStyling.CurrentTheme = theme;
                        BeepStyling.UseThemeColors = true;
                        BeepStyling.SetControlStyle(beepStyle);

                        // Use BeepStyling.PaintStyleBackground for GraphicsPath-based rendering
                        BeepStyling.PaintStyleBackground(g, path, beepStyle, true);
                    }
                    finally
                    {
                        BeepStyling.CurrentTheme = savedTheme;
                        BeepStyling.UseThemeColors = savedUseTheme;
                        BeepStyling.SetControlStyle(savedStyle);
                    }
                }
                else
                {
                    // Priority 3: Standard BeepStyling without theme override
                    var savedStyle = BeepStyling.CurrentControlStyle;

                    try
                    {
                        BeepStyling.SetControlStyle(beepStyle);
                        BeepStyling.PaintStyleBackground(g, path, beepStyle);
                    }
                    finally
                    {
                        BeepStyling.SetControlStyle(savedStyle);
                    }
                }
            }
        }

        #endregion

        #region Border Painting

        /// <summary>
        /// Paint border using BeepStyling BorderPainters
        /// </summary>
        public override void PaintBorder(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme)
        {
            var beepStyle = DialogStyleAdapter.GetBeepControlStyle(config);
            var colors = DialogStyleAdapter.GetColors(config, theme);

            int radius = StyleBorders.GetRadius(beepStyle);

            using (var path = CreateRoundedRectangle(bounds, radius))
            {
                if (config.BorderColor.HasValue)
                {
                    // Custom border color specified
                    int borderWidth = (int)StyleBorders.GetBorderWidth(beepStyle);
                    using (var pen = new Pen(config.BorderColor.Value, borderWidth))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Use BeepStyling.PaintStyleBorder for consistent border rendering
                    var savedStyle = BeepStyling.CurrentControlStyle;
                    var savedTheme = BeepStyling.CurrentTheme;
                    var savedUseTheme = BeepStyling.UseThemeColors;

                    try
                    {
                        BeepStyling.SetControlStyle(beepStyle);

                        if (config.UseBeepThemeColors && theme != null)
                        {
                            BeepStyling.CurrentTheme = theme;
                            BeepStyling.UseThemeColors = true;
                        }

                        // Paint border using BeepStyling
                        BeepStyling.PaintStyleBorder(g, path, false, beepStyle);
                    }
                    finally
                    {
                        BeepStyling.SetControlStyle(savedStyle);
                        BeepStyling.CurrentTheme = savedTheme;
                        BeepStyling.UseThemeColors = savedUseTheme;
                    }
                }
            }
        }

        #endregion

        #region Shadow Painting

        /// <summary>
        /// Paint shadow using BeepStyling ShadowPainters
        /// </summary>
        public override void PaintShadow(Graphics g, Rectangle bounds, DialogConfig config)
        {
            if (!config.ShowShadow && !config.EnableShadow)
                return;

            var beepStyle = DialogStyleAdapter.GetBeepControlStyle(config);

            if (!StyleShadows.HasShadow(beepStyle))
                return;

            var savedStyle = BeepStyling.CurrentControlStyle;

            try
            {
                BeepStyling.SetControlStyle(beepStyle);

                int blur = StyleShadows.GetShadowBlur(beepStyle);
                int offsetY = StyleShadows.GetShadowOffsetY(beepStyle);
                int offsetX = StyleShadows.GetShadowOffsetX(beepStyle);
                Color shadowColor = config.ShadowColor ?? StyleShadows.GetShadowColor(beepStyle);

                // Create shadow bounds
                Rectangle shadowBounds = new Rectangle(
                    bounds.X + offsetX,
                    bounds.Y + offsetY,
                    bounds.Width,
                    bounds.Height
                );

                int radius = StyleBorders.GetRadius(beepStyle);

                // Simple shadow with blur simulation
                for (int i = blur; i > 0; i--)
                {
                    int alpha = (int)(shadowColor.A * (i / (float)blur) * 0.3f);
                    Color blurColor = Color.FromArgb(alpha, shadowColor);

                    Rectangle blurRect = new Rectangle(
                        shadowBounds.X - (blur - i),
                        shadowBounds.Y - (blur - i),
                        shadowBounds.Width + (blur - i) * 2,
                        shadowBounds.Height + (blur - i) * 2
                    );

                    using (var path = CreateRoundedRectangle(blurRect, radius))
                    using (var brush = new SolidBrush(blurColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
            finally
            {
                BeepStyling.SetControlStyle(savedStyle);
            }
        }

        #endregion

        #region Icon Painting

        /// <summary>
        /// Paint icon using StyledImagePainter with theme tinting
        /// </summary>
        public override void PaintIcon(Graphics g, Rectangle iconRect, DialogConfig config, IBeepTheme theme)
        {
            if (!config.ShowIcon || iconRect.IsEmpty)
                return;

            var iconPath = DialogStyleAdapter.GetIconPath(config);
            if (string.IsNullOrEmpty(iconPath))
                return;

            var beepStyle = DialogStyleAdapter.GetBeepControlStyle(config);
            int cornerRadius = Math.Min(8, StyleBorders.GetRadius(beepStyle));

            using (var path = CreateRoundedRectangle(iconRect, cornerRadius))
            {
                try
                {
                    if (config.ApplyThemeOnIcon && theme != null)
                    {
                        // Apply theme tinting to icon
                        var iconColor = DialogStyleAdapter.GetIconColor(config, theme);
                        StyledImagePainter.PaintWithTint(g, path, iconPath, iconColor, 0.8f, cornerRadius);
                    }
                    else
                    {
                        // Standard icon rendering
                        StyledImagePainter.Paint(g, path, iconPath, beepStyle);
                    }
                }
                catch
                {
                    // Fallback: draw placeholder rectangle
                    using (var brush = new SolidBrush(Color.FromArgb(200, 200, 200)))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
        }

        #endregion

        #region Text Painting

        /// <summary>
        /// Paint dialog title
        /// </summary>
        public override void PaintTitle(Graphics g, Rectangle titleRect, DialogConfig config, IBeepTheme theme)
        {
            if (string.IsNullOrEmpty(config.Title) || titleRect.IsEmpty)
                return;

            var colors = DialogStyleAdapter.GetColors(config, theme);
            var titleFont = GetTitleFont(config);

            using (var brush = new SolidBrush(colors.Foreground))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                g.DrawString(config.Title, titleFont, brush, titleRect, format);
            }
        }

        /// <summary>
        /// Paint dialog message
        /// </summary>
        public override void PaintMessage(Graphics g, Rectangle messageRect, DialogConfig config, IBeepTheme theme)
        {
            if (string.IsNullOrEmpty(config.Message) || messageRect.IsEmpty)
                return;

            var colors = DialogStyleAdapter.GetColors(config, theme);
            var messageFont = GetMessageFont(config);

            using (var brush = new SolidBrush(colors.Foreground))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near,
                    Trimming = StringTrimming.Word
                };

                g.DrawString(config.Message, messageFont, brush, messageRect, format);
            }
        }

        /// <summary>
        /// Paint dialog details
        /// </summary>
        private void PaintDetails(Graphics g, Rectangle detailsRect, DialogConfig config, IBeepTheme theme)
        {
            if (string.IsNullOrEmpty(config.Details) || detailsRect.IsEmpty)
                return;

            var colors = DialogStyleAdapter.GetColors(config, theme);
            var detailsFont = GetDetailsFont(config);

            using (var brush = new SolidBrush(Color.FromArgb(160, colors.Foreground)))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near,
                    Trimming = StringTrimming.Word
                };

                g.DrawString(config.Details, detailsFont, brush, detailsRect, format);
            }
        }

        #endregion

        #region Button Painting

        /// <summary>
        /// Paint dialog buttons using BeepStyling
        /// </summary>
        public override void PaintButtons(Graphics g, Rectangle buttonBounds, DialogConfig config, IBeepTheme theme)
        {
            var layout = CalculateLayout(buttonBounds, config);
            if (layout.ButtonRects == null || layout.ButtonRects.Length == 0)
                return;

            var buttons = config.Buttons;
            var colors = DialogStyleAdapter.GetColors(config, theme);
            var beepStyle = DialogStyleAdapter.GetBeepControlStyle(config);
            var buttonFont = GetButtonFont(config);

            for (int i = 0; i < Math.Min(buttons.Length, layout.ButtonRects.Length); i++)
            {
                var buttonRect = layout.ButtonRects[i];
                var button = buttons[i];
                var buttonText = DialogStyleAdapter.GetButtonText(button);

                PaintSingleButton(g, buttonRect, buttonText, beepStyle, colors, buttonFont, theme);
            }
        }

        private void PaintSingleButton(Graphics g, Rectangle buttonRect, string text, 
            BeepControlStyle style, DialogColors colors, Font font, IBeepTheme theme)
        {
            int radius = Math.Min(6, StyleBorders.GetRadius(style));

            using (var path = CreateRoundedRectangle(buttonRect, radius))
            {
                // Button background
                using (var brush = new SolidBrush(colors.ButtonBackground))
                {
                    g.FillPath(brush, path);
                }

                // Button border
                using (var pen = new Pen(colors.ButtonBorder, 1))
                {
                    g.DrawPath(pen, path);
                }

                // Button text
                using (var brush = new SolidBrush(colors.ButtonForeground))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    g.DrawString(text, font, brush, buttonRect, format);
                }
            }
        }

        #endregion

        #region Close Button

        private void PaintCloseButton(Graphics g, Rectangle closeRect, DialogConfig config, IBeepTheme theme)
        {
            var colors = DialogStyleAdapter.GetColors(config, theme);

            // Draw X symbol
            using (var pen = new Pen(colors.Foreground, 2))
            {
                int padding = 6;
                g.DrawLine(pen,
                    closeRect.Left + padding, closeRect.Top + padding,
                    closeRect.Right - padding, closeRect.Bottom - padding);
                g.DrawLine(pen,
                    closeRect.Right - padding, closeRect.Top + padding,
                    closeRect.Left + padding, closeRect.Bottom - padding);
            }
        }

        #endregion
    }
}
