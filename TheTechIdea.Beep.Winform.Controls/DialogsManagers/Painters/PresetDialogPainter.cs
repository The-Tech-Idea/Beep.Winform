using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Painters
{
    /// <summary>
    /// Dialog painter for preset-based dialogs using BeepStyling
    /// Handles all DialogPreset styles with semantic button emphasis
    /// Colors come from BeepStyling, not custom palettes
    /// </summary>
    public class PresetDialogPainter : DialogPainterBase
    {
        private readonly DialogPreset _preset;
        private readonly BeepControlStyle _style;

        /// <summary>
        /// Create preset dialog painter
        /// </summary>
        /// <param name="preset">Dialog preset to use</param>
        /// <param name="style">BeepControlStyle for visual appearance</param>
        public PresetDialogPainter(DialogPreset preset, BeepControlStyle style = BeepControlStyle.Material3)
        {
            _preset = preset;
            _style = style;
        }

        #region Main Paint

        /// <summary>
        /// Paint complete dialog with preset styling
        /// </summary>
        public override void Paint(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme)
        {
            if (config == null || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Paint shadow first
            if (config.ShowShadow || config.EnableShadow)
                PaintShadow(g, bounds, config);

            // Calculate layout
            var layout = CalculateLayout(bounds, config);

            // Paint background
            PaintBackground(g, layout.ContentArea, config, theme);

            // Paint border
            PaintBorder(g, layout.ContentArea, config, theme);

            // Paint icon
            if (config.ShowIcon && !layout.IconRect.IsEmpty)
                PaintIcon(g, layout.IconRect, config, theme);

            // Paint title
            if (!string.IsNullOrEmpty(config.Title) && !layout.TitleRect.IsEmpty)
                PaintTitle(g, layout.TitleRect, config, theme);

            // Paint message
            if (!string.IsNullOrEmpty(config.Message) && !layout.MessageRect.IsEmpty)
                PaintMessage(g, layout.MessageRect, config, theme);

            // Paint buttons
            if (layout.ButtonRects != null && layout.ButtonRects.Length > 0)
                PaintButtons(g, layout.ButtonAreaRect, config, theme);
        }

        #endregion

        #region Background

        /// <summary>
        /// Paint dialog background using BeepStyling colors
        /// </summary>
        public override void PaintBackground(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme)
        {
            int radius = DialogPresetStyling.GetCornerRadius(_style);
            Color bgColor = config.BackColor ?? DialogPresetStyling.GetBackgroundColor(_preset, _style);

            using (var path = CreateRoundedRectangle(bounds, radius))
            {
                // Fill background
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        #endregion

        #region Border

        /// <summary>
        /// Paint dialog border using BeepStyling colors
        /// </summary>
        public override void PaintBorder(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme)
        {
            int radius = DialogPresetStyling.GetCornerRadius(_style);
            Color borderColor = config.BorderColor ?? DialogPresetStyling.GetBorderColor(_style);

            if (borderColor == Color.Transparent)
                return; // No border for smooth presets

            using (var path = CreateRoundedRectangle(bounds, radius))
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawPath(pen, path);
            }
        }

        #endregion

        #region Shadow

        /// <summary>
        /// Paint shadow effect based on preset style
        /// </summary>
        public override void PaintShadow(Graphics g, Rectangle bounds, DialogConfig config)
        {
            if (_preset.ToString().Contains("Raised"))
            {
                // Strong shadow for raised presets
                PaintRaisedShadow(g, bounds);
            }
            else if (_preset.ToString().StartsWith("Smooth"))
            {
                // Subtle shadow for smooth presets
                PaintSmoothShadow(g, bounds);
            }
            else
            {
                // Default shadow
                PaintDefaultShadow(g, bounds);
            }
        }

        private void PaintRaisedShadow(Graphics g, Rectangle bounds)
        {
            int radius = DialogPresetStyling.GetCornerRadius(_style);
            int shadowSize = 12;
            Rectangle shadowRect = new Rectangle(
                bounds.X + 2,
                bounds.Y + 4,
                bounds.Width,
                bounds.Height
            );

            using (var path = CreateRoundedRectangle(shadowRect, radius))
            {
                // Multi-layer shadow for depth
                for (int i = shadowSize; i > 0; i--)
                {
                    int alpha = (int)(60.0f * (1.0f - (float)i / shadowSize));
                    Color shadowColor = DialogPresetStyling.GetShadowColor(_style);
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(alpha, shadowColor)))
                    using (var shadowPath = CreateRoundedRectangle(
                        new Rectangle(shadowRect.X - i, shadowRect.Y - i, shadowRect.Width + i * 2, shadowRect.Height + i * 2),
                        radius + i))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }
            }
        }

        private void PaintSmoothShadow(Graphics g, Rectangle bounds)
        {
            int radius = DialogPresetStyling.GetCornerRadius(_style);
            int shadowSize = 6;
            Rectangle shadowRect = new Rectangle(
                bounds.X + 1,
                bounds.Y + 2,
                bounds.Width,
                bounds.Height
            );

            using (var path = CreateRoundedRectangle(shadowRect, radius))
            {
                for (int i = shadowSize; i > 0; i--)
                {
                    int alpha = (int)(30.0f * (1.0f - (float)i / shadowSize));
                    Color shadowColor = DialogPresetStyling.GetShadowColor(_style);
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(alpha, shadowColor)))
                    using (var shadowPath = CreateRoundedRectangle(
                        new Rectangle(shadowRect.X - i, shadowRect.Y - i, shadowRect.Width + i * 2, shadowRect.Height + i * 2),
                        radius + i))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }
            }
        }

        private void PaintDefaultShadow(Graphics g, Rectangle bounds)
        {
            int radius = DialogPresetStyling.GetCornerRadius(_style);
            int shadowSize = 8;

            using (var path = CreateRoundedRectangle(bounds, radius))
            {
                for (int i = shadowSize; i > 0; i--)
                {
                    int alpha = (int)(40.0f * (1.0f - (float)i / shadowSize));
                    Color shadowColor = DialogPresetStyling.GetShadowColor(_style);
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(alpha, shadowColor)))
                    using (var shadowPath = CreateRoundedRectangle(
                        new Rectangle(bounds.X - i, bounds.Y - i, bounds.Width + i * 2, bounds.Height + i * 2),
                        radius + i))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }
            }
        }

        #endregion

        #region Icon

        /// <summary>
        /// Paint dialog icon with theme tinting
        /// </summary>
        public override void PaintIcon(Graphics g, Rectangle iconRect, DialogConfig config, IBeepTheme theme)
        {
            string iconPath = DialogStyleAdapter.GetIconPath(config);
            if (string.IsNullOrEmpty(iconPath))
                return;

            Color iconColor = config.ApplyThemeOnIcon ? DialogPresetStyling.GetIconTint(_preset, _style) : Color.Transparent;
            int cornerRadius = DialogPresetStyling.GetCornerRadius(_style);

            try
            {
                using (var path = CreateRoundedRectangle(iconRect, cornerRadius))
                {
                    // Use PaintWithTint method from StyledImagePainter
                    StyledImagePainter.PaintWithTint(
                        g,
                        path,
                        iconPath,
                        iconColor,
                        config.ApplyThemeOnIcon ? 1f : 0f,
                        cornerRadius
                    );
                }
            }
            catch
            {
                // Fallback: draw simple icon placeholder
                Color fallbackColor = DialogPresetStyling.GetIconTint(_preset, _style);
                using (var pen = new Pen(fallbackColor, 2))
                {
                    g.DrawEllipse(pen, iconRect);
                    g.DrawLine(pen, iconRect.X + iconRect.Width / 2, iconRect.Y + iconRect.Height / 4,
                                   iconRect.X + iconRect.Width / 2, iconRect.Y + iconRect.Height / 2);
                    g.DrawLine(pen, iconRect.X + iconRect.Width / 2, iconRect.Y + iconRect.Height * 3 / 4,
                                   iconRect.X + iconRect.Width / 2, iconRect.Y + iconRect.Height * 3 / 4 + 2);
                }
            }
        }

        #endregion

        #region Text

        /// <summary>
        /// Paint dialog title
        /// </summary>
        public override void PaintTitle(Graphics g, Rectangle titleRect, DialogConfig config, IBeepTheme theme)
        {
            if (string.IsNullOrEmpty(config.Title))
                return;

            Color textColor = config.ForeColor ?? DialogPresetStyling.GetForegroundColor(_style);
            Font titleFont = config.TitleFont ?? GetTitleFont(config);

            using (var brush = new SolidBrush(textColor))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                g.DrawString(config.Title, titleFont, brush, titleRect, sf);
            }
        }

        /// <summary>
        /// Paint dialog message
        /// </summary>
        public override void PaintMessage(Graphics g, Rectangle messageRect, DialogConfig config, IBeepTheme theme)
        {
            if (string.IsNullOrEmpty(config.Message))
                return;

            Color textColor = config.ForeColor ?? DialogPresetStyling.GetForegroundColor(_style);
            Font messageFont = config.MessageFont ?? GetMessageFont(config);

            using (var brush = new SolidBrush(textColor))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.Word,
                FormatFlags = StringFormatFlags.LineLimit
            })
            {
                g.DrawString(config.Message, messageFont, brush, messageRect, sf);
            }
        }

        #endregion

        #region Buttons

        /// <summary>
        /// Paint dialog buttons
        /// </summary>
        public override void PaintButtons(Graphics g, Rectangle buttonBounds, DialogConfig config, IBeepTheme theme)
        {
            var layout = CalculateLayout(buttonBounds, config);
            if (layout.ButtonRects == null || layout.ButtonRects.Length == 0)
                return;

            var buttons = config.Buttons ?? Array.Empty<Vis.Modules.BeepDialogButtons>();
            bool isRaised = DialogPresetStyling.UsesRaisedStyle(_preset);

            for (int i = 0; i < Math.Min(buttons.Length, layout.ButtonRects.Length); i++)
            {
                bool isPrimary = i == buttons.Length - 1; // Last button is primary
                PaintButton(g, layout.ButtonRects[i], buttons[i], isPrimary, isRaised);
            }
        }

        private void PaintButton(Graphics g, Rectangle buttonRect, Vis.Modules.BeepDialogButtons button, bool isPrimary, bool isRaised)
        {
            int radius = 8;
            var emphasis = DialogPresetStyling.GetButtonEmphasis(_preset);
            Color buttonColor = isPrimary 
                ? DialogPresetStyling.GetPrimaryButtonColor(emphasis, _style)
                : DialogPresetStyling.GetSecondaryButtonColor(_style);
            Color textColor = GetButtonTextColor(isPrimary);
            string buttonText = DialogStyleAdapter.GetButtonText(button);

            using (var path = CreateRoundedRectangle(buttonRect, radius))
            {
                // Fill button
                using (var brush = new SolidBrush(buttonColor))
                {
                    g.FillPath(brush, path);
                }

                // Add button shadow if raised
                if (isRaised)
                {
                    PaintButtonShadow(g, buttonRect, radius);
                }

                // Draw button text
                using (var textBrush = new SolidBrush(textColor))
                using (var font = new Font("Segoe UI", 9.5f, FontStyle.Regular))
                using (var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                {
                    g.DrawString(buttonText, font, textBrush, buttonRect, sf);
                }
            }
        }

        private void PaintButtonShadow(Graphics g, Rectangle buttonRect, int radius)
        {
            Rectangle shadowRect = new Rectangle(buttonRect.X, buttonRect.Y + 2, buttonRect.Width, buttonRect.Height);
            using (var path = CreateRoundedRectangle(shadowRect, radius))
            {
                for (int i = 3; i > 0; i--)
                {
                    int alpha = (int)(40.0f * (1.0f - (float)i / 3));
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                    using (var shadowPath = CreateRoundedRectangle(
                        new Rectangle(shadowRect.X, shadowRect.Y + i, shadowRect.Width, shadowRect.Height),
                        radius))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }
            }
        }

        private Color GetButtonTextColor(bool isPrimary)
        {
            var theme = BeepThemesManager.CurrentTheme;
            var emphasis = DialogPresetStyling.GetButtonEmphasis(_preset);
            
            // For filled buttons or danger/warning emphasis, use contrasting text
            if (emphasis.UseFilledButtons || emphasis.PrimaryIsDanger || emphasis.PrimaryIsWarning)
            {
                return Color.White;
            }

            // Use dialog foreground color
            Color foreground = DialogPresetStyling.GetForegroundColor(_style);
            return isPrimary ? foreground : Color.FromArgb(200, foreground);
        }

        #endregion
    }
}
