using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Vis.Modules
{
    public class DockItemState
    {
        public SimpleItem Item { get; set; }
        public float CurrentScale { get; set; } = 1.0f;
        public float TargetScale { get; set; } = 1.0f;
        public bool IsHovered { get; set; }
        public bool IsSelected { get; set; }
    }

   

    public enum ButtonType
    {
        Normal,
        AnimatedArrow,
        ExpandingIcon,
        SlidingArrow,
        SlidingBackground
    }

    public enum ImageClipShape
    {
        None,       // No clipping (default rectangle)
        Circle,     // Perfect circle
        RoundedRect, // Rounded rectangle (uses BorderRadius)
        Ellipse,    // Oval/ellipse
        Diamond,    // Diamond shape
        Triangle,   // Triangle shape
        Hexagon,    // Hexagon shape
        Custom,
        RoundedRectangle
    }
    public enum ImageEmbededin
    {
        Button,
        Form,
        Label,
        TextBox,
        ComboBox,
        ListBox,
        DataGridView,
        TreeView,
        ListView,
        Panel,
        GroupBox,
        TabControl,
        TabPage,
        AppBar,
        SideBar,
        Menu,
        MenuBar,
        None,
        CheckBox
    }
    public enum SortDirection
    {
        None,
        Ascending,
        Descending
    }

    // Add enum for grid styles
    public enum BeepGridStyle
    {
        Classic,
        Modern,
        Flat,
        Material,
        Dark
    }
    /// <summary>
    /// Enum for modern gradient types
    /// </summary>
    public enum ModernGradientType
    {
        None,
        Linear,
        Radial,
        Conic,
        Mesh,
        Subtle
    }
    public enum BeepShapeType
    {
        Line,
        Rectangle,
        Ellipse,
        Triangle,
        Star,
        Diamond,
        Pentagon,
        Hexagon,
        Octagon
    }
    #region Supporting Enums and Classes
    public enum ShapeFillStyle
    {
        None,
        Solid,
        Gradient,
        Hatch
    }

    public enum ResizeHandle
    {
        None = -1,
        TopLeft = 0,
        TopRight = 1,
        BottomLeft = 2,
        BottomRight = 3
    }
    #endregion
    #region Supporting Classes and Enums
    

    public enum DockPosition
    {
        Top,
        Bottom,
        Left,
        Right,
        Center
    }

    public enum DockOrientation
    {
        Horizontal,
        Vertical
    }
    #endregion
    /// <summary>
    /// Defines the color schemes for Material components
    /// </summary>
    public enum MaterialColorScheme
    {
        Primary,    // Primary theme color
        Secondary,  // Secondary theme color
        Error,      // For error states
        Warning,    // For warning states
        Info,       // For informational states
        Success     // For success states
    }

    public enum ValidationTypeBasedonIcon
    {
        None,
        Error,
        Warning,
        Info,
        Success,
        Alert,
        Likely,
        Important,
        Heart,
        Help,
        Question,
        Ignore,
        Cool
    }
    // First, add the new enums for Material-UI TextField variants
    public enum TextFieldVariant
    {
        Standard,  // Underline only
        Filled,    // With background fill and underline
        Outlined   // With outline border
    }

    public enum TextFieldSize
    {
        Small,
        Medium
    }

    public enum TextFieldColor
    {
        Primary,
        Secondary,
        Error,
        Warning,
        Info,
        Success
    }
    #region "React-Style UI Enums"
    // Add this enum to define Material UI TextField variants
    public enum MaterialTextFieldVariant
    {
        Standard,   // Bottom border only, label animates upward when focused or filled
        Outlined,   // Full border outline, with floating label that moves to the border when focused
        Filled      // Filled background with bottom border, label animates upward when focused
    }

    /// <summary>
    /// Material Design density options for text fields
    /// </summary>
    public enum MaterialTextFieldDensity
    {
        Standard,    // Default spacing (72px height)
        Dense,       // Reduced spacing (56px height)
        Comfortable  // Increased spacing (88px height)
    }
    public enum ReactUIVariant
    {
        Default,
        Outlined,
        Filled,
        Text,
        Contained,
        Ghost
    }

    public enum ReactUISize
    {
        ExtraSmall,
        Small,
        Medium,
        Large,
        ExtraLarge
    }

    public enum ReactUIColor
    {
        Primary,
        Secondary,
        Success,
        Error,
        Warning,
        Info,
        Default
    }

    public enum ReactUIDensity
    {
        Compact,
        Standard,
        Comfortable
    }

    public enum ReactUIElevation
    {
        None,
        Low,
        Medium,
        High,
        Custom
    }

    public enum ReactUIShape
    {
        Square,
        Rounded,
        Circular,
        Pill
    }

    public enum ReactUIAnimation
    {
        None,
        Ripple,
        Fade,
        Scale,
        Slide
    }
    #endregion
    public enum ChevronDirection
    {
        Forward,   // arrows point to the right (→)
        Backward,   // arrows point to the left (←)
            Up,Down
    }
   
    public enum WizardState
    {
        None,
        Start,
        Finish,
        Cancel,
        Next,
        Previous,
        Edit,
        Save,
        Delete,
        Load,
        Close,
        Open,
        Show,
    }
    public enum LabelPosition
    {
        Left,
        Right
    }
    public enum TypeStyleFontSize { None, Small, Medium, Big, Banner, Large, ExtraLarge, ExtraExtraLarge, ExtraExtraExtraLarge }
    public enum CustomBorderStyle { None, Solid, Dashed, Dotted }
    public enum DisplayAnimationType { None, Popup, Slide, Fade, SlideAndFade }
    public enum EasingType { Linear, EaseIn, EaseOut, EaseInOut }
    public enum SlideDirection { Left, Right, Top, Bottom }
    public enum BadgeShape { Circle, Rectangle, RoundedRectangle }


    public enum AggregationType
    {
        None,
        Sum,
        Average,
        Count,
        Min,
        Max,
        First,
        Last,
        DistinctCount,
        Custom
    }
    public enum TestimonialViewType
    {
        Classic,    // Default testimonial with image on left
        Minimal,    // Centered layout with company logo
        Compact,    // Smallest version, reduced padding
        SocialCard  // New modern social media-style testimonial
    }


    public enum DateFormatStyle
    {
        ShortDate,        // "MM/dd/yyyy"
        LongDate,         // "dddd, MMMM dd, yyyy"
        YearMonth,        // "MMMM yyyy"
        Custom,           // Uses the DateFormat string
        FullDateTime,     // "dddd, MMMM dd, yyyy HH:mm:ss"
        ShortDateTime,    // "MM/dd/yyyy HH:mm"
        DayMonthYear,     // "dd MMMM yyyy"
        ISODate,          // "yyyy-MM-dd"
        ISODateTime,      // "yyyy-MM-dd HH:mm:ss"
        TimeOnly,         // "HH:mm:ss"
        ShortTime,        // "HH:mm"
        MonthDay,         // "MMMM dd"
        DayOfWeek,        // "dddd"
        RFC1123,          // "ddd, dd MMM yyyy HH:mm:ss GMT"
        UniversalSortable // "yyyy-MM-dd HH:mm:ssZ"
    }
    public enum DataSourceMode
    {
        None,
        Table,
        Query,
        CascadingMap,
        View,
        StoredProc,
        File,
        WebService,
        RestAPI,
        OData,
        GraphQL,
        Custom
    }
    public enum GridDataSourceType
    {
        Fixed,
        BindingSource,
        IDataSource
    }
    public enum CheckBoxState
    {
        Unchecked,
        Checked,
        Indeterminate
    }

    public enum CheckMarkShape
    {
        Square,
        Circle,
        CustomSvg
    }
    public enum BeepDialogButtonSchema
    {
        None,
        OkCancel,
        YesNo,
        YesNoCancel,
        Ok,
        Cancel,
        Yes,
        No,
        AbortRetryIgnore,
        RetryCancel,
        ContinueStop,
        Custom
    }
    public enum BeepDialogButtons
    {
        None,
        Ok,
        Cancel,
        Yes,
        No,
        Custom,
        Abort,
        Retry,
        Ignore,
        Continue,
        Stop,
        Suspend,
        Resume

    }
    public enum BeepDialogIcon
    {
        None,
        Information,
        Warning,
        Error,
        Question,
        Success
    }

    //
    // Summary:
    //     Specifies identifiers to indicate the return value of a dialog box.
    public enum BeepPopupFormPosition
    {
        Top,
        Bottom,
        Left,
        Right
    }
    public enum BeepDialogResult
    {
        //
        // Summary:
        //     Nothing is returned from the dialog box. This means that the modal dialog continues
        //     running.
        None = 0,
        //
        // Summary:
        //     The dialog box return value is OK (usually sent from a button labeled OK).
        OK = 1,
        //
        // Summary:
        //     The dialog box return value is Cancel (usually sent from a button labeled Cancel).
        Cancel = 2,
        //
        // Summary:
        //     The dialog box return value is Abort (usually sent from a button labeled Abort).
        Abort = 3,
        //
        // Summary:
        //     The dialog box return value is Retry (usually sent from a button labeled Retry).
        Retry = 4,
        //
        // Summary:
        //     The dialog box return value is Ignore (usually sent from a button labeled Ignore).
        Ignore = 5,
        //
        // Summary:
        //     The dialog box return value is Yes (usually sent from a button labeled Yes).
        Yes = 6,
        //
        // Summary:
        //     The dialog box return value is No (usually sent from a button labeled No).
        No = 7,
        Continue=8,
        Stop = 9,

    }
    public enum ContainerTypeEnum
    {
        SinglePanel,
        TabbedPanel
    }
    public enum BeepMouseButtons
    {
        //
        // Summary:
        //     The left mouse button was pressed.
        Left = 0x100000,
        //
        // Summary:
        //     No mouse button was pressed.
        None = 0x0,
        //
        // Summary:
        //     The right mouse button was pressed.
        Right = 0x200000,
        //
        // Summary:
        //     The middle mouse button was pressed.
        Middle = 0x400000,
        //
        // Summary:
        //     The first XButton was pressed.
        XButton1 = 0x800000,
        //
        // Summary:
        //     The second XButton was pressed.
        XButton2 = 0x1000000
    }
    public enum  DialogShowAnimation
    {
        None,
        FadeIn,
        SlideInFromTop,
        SlideInFromBottom,
        SlideInFromLeft,
        SlideInFromRight,
        ZoomIn

    }
    public enum TextBoxMaskFormat
    {
        None,                // No masking or formatting applied
        Currency,            // Formats input as currency (e.g., $1,234.56)
        Percentage,          // Formats input as a percentage (e.g., 12.34%)
        Date,                // Formats input as a date (e.g., MM/dd/yyyy)
        Time,                // Formats input as time (e.g., HH:mm:ss)
        PhoneNumber,         // Formats input as a phone number (e.g., (123) 456-7890)
        Email,               // Validates input as an email address
        SocialSecurityNumber,// Formats input as SSN (e.g., 123-45-6789)
        ZipCode,             // Formats input as a ZIP code (e.g., 12345 or 12345-6789)
        Numeric,             // Allows only numeric input
        Alphanumeric,        // Allows both letters and numbers
        Password,            // Masks input as password characters
        IPAddress,           // Formats input as an IP address (e.g., 192.168.0.1)
        CreditCard,          // Formats input as a credit card number (e.g., 1234 5678 9012 3456)
        Hexadecimal,         // Allows only hexadecimal characters (0-9, A-F)
        Custom,              // Allows for a custom mask format defined by the developer
        URL,                 // Validates input as a URL (e.g., https://www.example.com)
        TimeSpan,            // Formats input as a TimeSpan (e.g., 12:34:56)
        Decimal,             // Formats input as a decimal number with specified precision
        CurrencyWithoutSymbol,// Formats input as currency without the currency symbol (e.g., 1,234.56)
        DateTime,            // Formats input as both date and time (e.g., MM/dd/yyyy HH:mm:ss)
        Year,                // Formats input to accept only the year (e.g., 2025)
        MonthYear            // Formats input to accept month and year (e.g., MM/yyyy)
    }

    public enum AxisType
    {
        Numeric,
        Text,
        Date
    }

    public enum TextAlignment
    {
        Horizontal,
        Vertical,
        Diagonal45,
        Right,
        Left,
        Above,
        Below
    }

    public enum ChartType
    {
        Line,
        Bar,
        Pie,
        Bubble,Area
    }
    public enum ChartStyle
    {
        Light,
        Dark
    }
    public enum ChartLegendPosition
    {
        Top,
        Bottom,
        Left,
        Right
    }
    public enum ChartLegendAlignment
    {
        Center,
        Start,
        End
    }
    public enum ChartLegendOrientation
    {
        Horizontal,
        Vertical
    }
    public enum ChartDataPointStyle
    {
        Circle,
        Square,
        Diamond,
        Triangle
    }

    public enum ImageScaleMode
    {
        None,
        Stretch,
        KeepAspectRatio,
        KeepAspectRatioByWidth,
        KeepAspectRatioByHeight,
        Fill
    }
   

}
