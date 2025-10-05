using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Interface for BeepNumericUpDown painters
    /// Defines the contract for rendering numeric up-down controls with different visual styles
    /// </summary>
    public interface INumericUpDownPainter
    {
        /// <summary>
        /// Paints the entire numeric up-down control including background, text, and buttons
        /// </summary>
        void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds);

        /// <summary>
        /// Paints just the spin buttons (up and down)
        /// </summary>
        void PaintButtons(Graphics g, INumericUpDownPainterContext context, Rectangle upButtonRect, Rectangle downButtonRect);

        /// <summary>
        /// Paints the value text area
        /// </summary>
        void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText);

        /// <summary>
        /// Updates hit areas for mouse interaction
        /// </summary>
        void UpdateHitAreas(INumericUpDownPainterContext context, Rectangle bounds, 
            Action<string, Rectangle, Action> registerHitArea);
    }

    /// <summary>
    /// Context interface providing BeepNumericUpDown state to painters
    /// </summary>
    public interface INumericUpDownPainterContext
    {
        // Value properties
        decimal Value { get; }
        decimal MinimumValue { get; }
        decimal MaximumValue { get; }
        decimal IncrementValue { get; }
        int DecimalPlaces { get; }

        // State properties
        bool IsEditing { get; }
        bool IsHovered { get; }
        bool IsFocused { get; }
        bool IsEnabled { get; }
        bool IsReadOnly { get; }

        // Button states
        bool UpButtonPressed { get; }
        bool DownButtonPressed { get; }
        bool UpButtonHovered { get; }
        bool DownButtonHovered { get; }
        bool ShowSpinButtons { get; }

        // Visual properties
        bool IsRounded { get; }
        int BorderRadius { get; }
        Color AccentColor { get; }
        bool UseThemeColors { get; }
        IBeepTheme Theme { get; }

        // Display properties
        NumericUpDownDisplayMode DisplayMode { get; }
        NumericSpinButtonSize ButtonSize { get; }
        string Prefix { get; }
        string Suffix { get; }
        string Unit { get; }
        bool ThousandsSeparator { get; }
        bool AllowNegative { get; }
        bool EnableShadow { get; set; }
        // Actions
        void IncreaseValue();
        void DecrementValue();
    }
}
