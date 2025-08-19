using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Models
{
    /// <summary>
    /// Text editing operation types for undo/redo system
    /// </summary>
    public enum TextEditOperationType
    {
        Insert,
        Delete,
        Replace,
        Cut,
        Paste,
        Character,
        Backspace,
        Word,
        Line,
        Format,
        AutoComplete,
        Checkpoint
    }

    /// <summary>
    /// Text alignment options
    /// </summary>
    public enum TextBoxAlignment
    {
        Left,
        Center,
        Right,
        Justify
    }

    /// <summary>
    /// Input validation result
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string WarningMessage { get; set; } = string.Empty;
        public object SuggestedValue { get; set; }

        public ValidationResult(bool isValid = true)
        {
            IsValid = isValid;
        }

        public ValidationResult(string errorMessage)
        {
            IsValid = false;
            ErrorMessage = errorMessage;
        }
    }

    /// <summary>
    /// Text formatting options
    /// </summary>
    public class TextFormattingOptions
    {
        public bool UpperCase { get; set; }
        public bool LowerCase { get; set; }
        public bool TitleCase { get; set; }
        public bool TrimWhitespace { get; set; }
        public bool RemoveExtraSpaces { get; set; }
        public string CustomFormat { get; set; }
    }

    /// <summary>
    /// Caret behavior settings
    /// </summary>
    public class CaretSettings
    {
        public int BlinkInterval { get; set; } = 500;
        public int Width { get; set; } = 1;
        public bool Visible { get; set; } = true;
        public bool ShowInReadOnly { get; set; } = false;
    }

    /// <summary>
    /// Selection appearance settings
    /// </summary>
    public class SelectionSettings
    {
        public System.Drawing.Color BackColor { get; set; } = System.Drawing.SystemColors.Highlight;
        public System.Drawing.Color ForeColor { get; set; } = System.Drawing.SystemColors.HighlightText;
        public bool AllowMultipleSelections { get; set; } = false;
        public bool HideWhenNotFocused { get; set; } = true;
    }

    /// <summary>
    /// Line number display settings
    /// </summary>
    public class LineNumberSettings
    {
        public bool Visible { get; set; } = false;
        public int MarginWidth { get; set; } = 40;
        public System.Drawing.Color ForeColor { get; set; } = System.Drawing.Color.Gray;
        public System.Drawing.Color BackColor { get; set; } = System.Drawing.Color.FromArgb(240, 240, 240);
        public System.Drawing.Font Font { get; set; }
        public bool ShowLeadingZeros { get; set; } = false;
        public string Format { get; set; } = "{0}";
    }

    /// <summary>
    /// Scrolling behavior settings
    /// </summary>
    public class ScrollingSettings
    {
        public bool ShowVerticalScrollBar { get; set; } = true;
        public bool ShowHorizontalScrollBar { get; set; } = true;
        public bool AutoScroll { get; set; } = true;
        public int ScrollSensitivity { get; set; } = 3;
        public bool SmoothScrolling { get; set; } = false;
    }

    /// <summary>
    /// Performance optimization settings
    /// </summary>
    public class PerformanceSettings
    {
        public bool EnableVirtualScrolling { get; set; } = true;
        public bool CacheLineMetrics { get; set; } = true;
        public bool CacheFontMetrics { get; set; } = true;
        public int MaxCacheSize { get; set; } = 1000;
        public bool EnableSmartRendering { get; set; } = true;
    }

    /// <summary>
    /// Comprehensive text box configuration
    /// </summary>
    public class TextBoxConfiguration
    {
        public TheTechIdea.Beep.Vis.Modules.TextBoxMaskFormat MaskFormat { get; set; } = TheTechIdea.Beep.Vis.Modules.TextBoxMaskFormat.None;
        public string CustomMask { get; set; } = string.Empty;
        public TextFormattingOptions Formatting { get; set; } = new TextFormattingOptions();
        public CaretSettings Caret { get; set; } = new CaretSettings();
        public SelectionSettings Selection { get; set; } = new SelectionSettings();
        public LineNumberSettings LineNumbers { get; set; } = new LineNumberSettings();
        public ScrollingSettings Scrolling { get; set; } = new ScrollingSettings();
        public PerformanceSettings Performance { get; set; } = new PerformanceSettings();
        public bool ReadOnly { get; set; } = false;
        public bool Multiline { get; set; } = false;
        public bool WordWrap { get; set; } = false;
        public bool AcceptsReturn { get; set; } = false;
        public bool AcceptsTab { get; set; } = false;
        public int MaxLength { get; set; } = 0;
        public string PlaceholderText { get; set; } = string.Empty;
    }
}