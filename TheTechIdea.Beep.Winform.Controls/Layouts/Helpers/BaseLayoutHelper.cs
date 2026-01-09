using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Cards;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.TextFields;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    /// <summary>
    /// Base helper class for layout helpers providing common functionality
    /// for creating Beep controls with theme-aware styling.
    /// </summary>
    public static class BaseLayoutHelper
    {
        /// <summary>
        /// Creates a BeepButton with theme-aware styling.
        /// </summary>
        public static BeepButton CreateBeepButton(string text, LayoutOptions options)
        {
            var button = new BeepButton
            {
                Text = text,
                ControlStyle = options.ControlStyle,
                UseThemeColors = options.UseThemeColors
            };

            if (options.Theme != null)
            {
                button.Theme = options.Theme.GetType().Name;
            }

            if (options.DefaultFont != null)
            {
                button.Font = options.DefaultFont;
            }

            return button;
        }

        /// <summary>
        /// Creates a BeepCard with theme-aware styling.
        /// </summary>
        public static BeepCard CreateBeepCard(LayoutOptions options)
        {
            var card = new BeepCard
            {
                ControlStyle = options.ControlStyle,
                UseThemeColors = options.UseThemeColors
            };

            if (options.Theme != null)
            {
                card.Theme = options.Theme.GetType().Name;
            }

            if (!options.BackgroundColor.IsEmpty)
            {
                card.BackColor = options.BackgroundColor;
            }

            return card;
        }

        /// <summary>
        /// Creates a BeepTextBox with theme-aware styling.
        /// </summary>
        public static BeepTextBox CreateBeepTextBox(string text, LayoutOptions options)
        {
            var textBox = new BeepTextBox
            {
                Text = text,
                ControlStyle = options.ControlStyle,
                UseThemeColors = options.UseThemeColors
            };

            if (options.Theme != null)
            {
                textBox.Theme = options.Theme.GetType().Name;
            }

            if (options.DefaultFont != null)
            {
                textBox.Font = options.DefaultFont;
            }

            return textBox;
        }

        /// <summary>
        /// Creates a styled Panel with theme-aware colors.
        /// </summary>
        public static Panel CreateStyledPanel(LayoutOptions options)
        {
            var panel = new Panel
            {
                BorderStyle = options.ShowBorders ? BorderStyle.FixedSingle : BorderStyle.None
            };

            // Apply theme colors if available
            if (options.UseThemeColors && options.Theme != null)
            {
                panel.BackColor = options.BackgroundColor.IsEmpty 
                    ? options.Theme.BackColor 
                    : options.BackgroundColor;
                
                if (options.ShowBorders && !options.BorderColor.IsEmpty)
                {
                    // Note: Panel doesn't support custom border colors directly
                    // This would need to be handled via custom painting
                }
            }
            else if (!options.BackgroundColor.IsEmpty)
            {
                panel.BackColor = options.BackgroundColor;
            }

            return panel;
        }

        /// <summary>
        /// Creates a styled Label with theme-aware colors and font.
        /// </summary>
        public static Label CreateStyledLabel(string text, LayoutOptions options, ContentAlignment textAlign = ContentAlignment.MiddleLeft)
        {
            var label = new Label
            {
                Text = text,
                TextAlign = textAlign,
                AutoSize = false
            };

            // Apply theme colors if available
            if (options.UseThemeColors && options.Theme != null)
            {
                label.ForeColor = options.ForegroundColor.IsEmpty 
                    ? options.Theme.ForeColor 
                    : options.ForegroundColor;
                
                label.BackColor = options.BackgroundColor.IsEmpty 
                    ? options.Theme.BackColor 
                    : options.BackgroundColor;
            }
            else
            {
                if (!options.ForegroundColor.IsEmpty)
                    label.ForeColor = options.ForegroundColor;
                if (!options.BackgroundColor.IsEmpty)
                    label.BackColor = options.BackgroundColor;
            }

            // Apply font
            if (options.DefaultFont != null)
            {
                label.Font = options.DefaultFont;
            }
            else if (options.Theme != null)
            {
                label.Font = new Font(options.Theme.FontFamily ?? "Segoe UI", options.DefaultFontSize);
            }

            return label;
        }

        /// <summary>
        /// Gets a theme-aware color, falling back to a default if theme is not available.
        /// </summary>
        public static Color GetThemeColor(LayoutOptions options, string colorKey, Color defaultColor)
        {
            if (options.UseThemeColors && options.Theme != null)
            {
                var color = BeepStyling.GetThemeColor(colorKey);
                if (!color.IsEmpty)
                    return color;
            }

            return defaultColor;
        }

        /// <summary>
        /// Gets the background color from options or theme.
        /// </summary>
        public static Color GetBackgroundColor(LayoutOptions options)
        {
            if (!options.BackgroundColor.IsEmpty)
                return options.BackgroundColor;

            if (options.UseThemeColors && options.Theme != null)
                return options.Theme.BackColor;

            return SystemColors.Control;
        }

        /// <summary>
        /// Gets the foreground color from options or theme.
        /// </summary>
        public static Color GetForegroundColor(LayoutOptions options)
        {
            if (!options.ForegroundColor.IsEmpty)
                return options.ForegroundColor;

            if (options.UseThemeColors && options.Theme != null)
                return options.Theme.ForeColor;

            return SystemColors.ControlText;
        }

        /// <summary>
        /// Gets the border color from options or theme.
        /// </summary>
        public static Color GetBorderColor(LayoutOptions options)
        {
            if (!options.BorderColor.IsEmpty)
                return options.BorderColor;

            if (options.UseThemeColors && options.Theme != null)
                return GetThemeColor(options, "Border", options.Theme.BorderColor);

            return SystemColors.ControlDark;
        }

        /// <summary>
        /// Gets a font from options or theme, or creates a default font.
        /// </summary>
        public static Font GetFont(LayoutOptions options, float? sizeOverride = null)
        {
            if (options.DefaultFont != null)
            {
                if (sizeOverride.HasValue)
                    return new Font(options.DefaultFont.FontFamily, sizeOverride.Value, options.DefaultFont.Style);
                return options.DefaultFont;
            }

            if (options.Theme != null)
            {
                float fontSize = sizeOverride ?? options.DefaultFontSize;
                return new Font(options.Theme.FontFamily ?? "Segoe UI", fontSize);
            }

            return new Font("Segoe UI", sizeOverride ?? options.DefaultFontSize);
        }

        /// <summary>
        /// Applies spacing and padding to a control.
        /// </summary>
        public static void ApplySpacing(Control control, LayoutOptions options, Padding? customPadding = null)
        {
            control.Margin = new Padding(options.Spacing);
            control.Padding = customPadding ?? options.Padding;
        }
    }
}
