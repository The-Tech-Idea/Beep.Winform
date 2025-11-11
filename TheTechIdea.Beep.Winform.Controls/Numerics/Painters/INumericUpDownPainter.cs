using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Interface for BeepNumericUpDown layout painters.
    /// NOTE: Visual styling (colors, shadows, borders, backgrounds) is handled by BeepStyling.
    /// Painters only handle layout calculations, button positioning, and text formatting based on NumericStyle.
    /// </summary>
    public interface INumericUpDownPainter
    {
        /// <summary>
        /// Calculates the layout rectangles for different NumericStyle types.
        /// Returns the text area, up button area, and down button area.
        /// Does NOT paint visual elements - only computes layout.
        /// </summary>
        NumericLayoutInfo CalculateLayout(INumericUpDownPainterContext context, Rectangle bounds);

        /// <summary>
        /// Formats the numeric value according to the NumericStyle and display mode.
        /// </summary>
        string FormatValue(INumericUpDownPainterContext context);

        /// <summary>
        /// Paints button icons/symbols (arrows, +/- symbols, etc.) only.
        /// Background, border, shadow are handled by BeepStyling.
        /// </summary>
        void PaintButtonIcons(Graphics g, INumericUpDownPainterContext context, Rectangle upButtonRect, Rectangle downButtonRect);

        /// <summary>
        /// Paints the value text with proper formatting.
        /// Background is already painted by BeepStyling.
        /// </summary>
        void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText);

        /// <summary>
        /// Updates hit areas for mouse interaction based on the calculated layout.
        /// </summary>
        void UpdateHitAreas(INumericUpDownPainterContext context, Rectangle bounds, 
            Action<string, Rectangle, Action> registerHitArea);
    }

    /// <summary>
    /// Contains layout information calculated by the painter
    /// </summary>
    public class NumericLayoutInfo
    {
        /// <summary>Text display area</summary>
        public Rectangle TextRect { get; set; }

        /// <summary>Up/Increment button area</summary>
        public Rectangle UpButtonRect { get; set; }

        /// <summary>Down/Decrement button area</summary>
        public Rectangle DownButtonRect { get; set; }

        /// <summary>Additional custom areas for specific NumericStyle types (e.g., slider thumb)</summary>
        public Rectangle CustomArea1 { get; set; }

        /// <summary>Second custom area if needed</summary>
        public Rectangle CustomArea2 { get; set; }

        /// <summary>Indicates if buttons should be displayed</summary>
        public bool ShowButtons { get; set; } = true;
    }

    /// <summary>
    /// Context interface providing BeepNumericUpDown state to painters.
    /// Painters use this to calculate layout and format values.
    /// Visual styling properties (colors, shadows) are no longer used by painters.
    /// </summary>
    public interface INumericUpDownPainterContext
    {
        // Value properties
        /// <summary>Current numeric value</summary>
        decimal Value { get; }
        /// <summary>Minimum allowed value</summary>
        decimal MinimumValue { get; }
        /// <summary>Maximum allowed value</summary>
        decimal MaximumValue { get; }
        /// <summary>Increment/decrement step value</summary>
        decimal IncrementValue { get; }
        /// <summary>Number of decimal places to display</summary>
        int DecimalPlaces { get; }

        // State properties
        /// <summary>Whether the control is in edit mode</summary>
        bool IsEditing { get; }
        /// <summary>Whether mouse is hovering over the control</summary>
        bool IsHovered { get; }
        /// <summary>Whether the control has focus</summary>
        bool IsFocused { get; }
        /// <summary>Whether the control is enabled</summary>
        bool IsEnabled { get; }
        /// <summary>Whether the control is read-only</summary>
        bool IsReadOnly { get; }

        // Button states
        /// <summary>Whether up button is pressed</summary>
        bool UpButtonPressed { get; }
        /// <summary>Whether down button is pressed</summary>
        bool DownButtonPressed { get; }
        /// <summary>Whether mouse is over up button</summary>
        bool UpButtonHovered { get; }
        /// <summary>Whether mouse is over down button</summary>
        bool DownButtonHovered { get; }
        /// <summary>Whether to show spin buttons</summary>
        bool ShowSpinButtons { get; }

        // Layout properties (used by painters for calculations)
        /// <summary>Whether control has rounded corners</summary>
        bool IsRounded { get; }
        /// <summary>Border radius value (for layout calculations)</summary>
        int BorderRadius { get; }

        // Display properties
        /// <summary>Display mode (percentage, currency, etc.)</summary>
        NumericUpDownDisplayMode DisplayMode { get; }
        /// <summary>The NumericStyle determining layout and interaction</summary>
        NumericStyle NumericStyle { get; }
        /// <summary>Size of spin buttons</summary>
        NumericSpinButtonSize ButtonSize { get; }
        /// <summary>Text prefix</summary>
        string Prefix { get; }
        /// <summary>Text suffix</summary>
        string Suffix { get; }
        /// <summary>Unit of measurement</summary>
        string Unit { get; }
        /// <summary>Whether to use thousands separator</summary>
        bool ThousandsSeparator { get; }
        /// <summary>Whether negative values are allowed</summary>
        bool AllowNegative { get; }

        // Theme (for text colors only, not backgrounds/borders/shadows)
        /// <summary>Current theme for text color references</summary>
        IBeepTheme Theme { get; }

        // Actions
        /// <summary>Increment the value</summary>
        void IncreaseValue();
        /// <summary>Decrement the value</summary>
        void DecrementValue();
    }
}
