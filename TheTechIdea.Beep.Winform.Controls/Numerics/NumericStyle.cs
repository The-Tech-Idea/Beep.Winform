using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Numerics
{
    /// <summary>
    /// Defines the visual and functional style for numeric input controls.
    /// Each style determines the layout, button placement, and interaction pattern.
    /// Visual styling (colors, shadows, borders) is handled by BeepStyling and BeepControlStyle.
    /// </summary>
    public enum NumericStyle
    {
        /// <summary>
        /// Standard numeric input with spin buttons on left and right sides
        /// </summary>
        [Description("Standard numeric input with side buttons")]
        Standard,

        /// <summary>
        /// Compact stepper with minimal buttons on right side only
        /// </summary>
        [Description("Compact stepper with right-side buttons")]
        CompactStepper,

        /// <summary>
        /// Horizontal slider with draggable thumb
        /// </summary>
        [Description("Horizontal slider control")]
        Slider,

        /// <summary>
        /// Currency input with formatting and symbol
        /// </summary>
        [Description("Currency input with symbol")]
        Currency,

        /// <summary>
        /// Percentage input (0-100)
        /// </summary>
        [Description("Percentage input control")]
        Percentage,

        /// <summary>
        /// Phone number input with formatted display
        /// </summary>
        [Description("Phone number input")]
        Phone,

        /// <summary>
        /// Decimal number input with high precision
        /// </summary>
        [Description("Decimal number input")]
        Decimal,

        /// <summary>
        /// Integer-only input, no decimal places
        /// </summary>
        [Description("Integer-only input")]
        Integer,

        /// <summary>
        /// Rating control (stars, hearts, etc.)
        /// </summary>
        [Description("Rating control")]
        Rating,

        /// <summary>
        /// Progress indicator with numeric value
        /// </summary>
        [Description("Progress with numeric value")]
        Progress,

        /// <summary>
        /// Time input (hours, minutes, seconds)
        /// </summary>
        [Description("Time input control")]
        Time,

        /// <summary>
        /// Scientific notation input
        /// </summary>
        [Description("Scientific notation input")]
        Scientific,

        /// <summary>
        /// Circular dial/knob control
        /// </summary>
        [Description("Circular dial control")]
        Dial,

        /// <summary>
        /// Vertical stepper control
        /// </summary>
        [Description("Vertical stepper")]
        VerticalStepper,

        /// <summary>
        /// Calculator-style numeric pad
        /// </summary>
        [Description("Calculator-style input")]
        Calculator,

        /// <summary>
        /// Temperature input with unit conversion
        /// </summary>
        [Description("Temperature input")]
        Temperature,

        /// <summary>
        /// Minimal inline editing style
        /// </summary>
        [Description("Minimal inline editing")]
        Inline,

        /// <summary>
        /// Touch-optimized large buttons
        /// </summary>
        [Description("Touch-optimized input")]
        TouchOptimized,

        /// <summary>
        /// Read-only display with optional quick edit
        /// </summary>
        [Description("Read-only display")]
        Display,

        /// <summary>
        /// Chip/tag style numeric input
        /// </summary>
        [Description("Chip-style numeric input")]
        Chip
    }
}
