using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Vis.Modules
{

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
        Question
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
        Diagonal45
    }

    public enum ChartType
    {
        Line,
        Bar,
        Pie,
        Bubble
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
        KeepAspectRatioByHeight
    }


}
