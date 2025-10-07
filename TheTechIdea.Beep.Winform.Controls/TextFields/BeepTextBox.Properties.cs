using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Properties for BeepTextBox
    /// </summary>
    public partial class BeepTextBox
    {
        #region "Properties - Core Text"
        
        public override string Text
        {
            get => _text;
            set
            {
                string newValue = value ?? "";
                
                // Only filter out during initialization or if it's obviously a designer default
                if (_isInitializing && (string.IsNullOrEmpty(newValue) || newValue.StartsWith("BeepTextBox")))
                {
                    _text = "";
                    return;
                }

                if (_text != newValue)
                {
                    string oldText = _text;
                    _text = newValue;
                    
                    // Update lines and helper immediately for better responsiveness
                    UpdateLines();
                    _helper?.InvalidateAllCaches();
                    
                    // Trigger events and updates
                    OnTextChangedInternal(oldText, _text);
                    
                    // Use immediate invalidation instead of delayed timer during normal operation
                    if (!_isInitializing)
                    {
                        Invalidate();
                    }
                    else
                    {
                        // Use delayed update only during initialization
                        _needsTextUpdate = true;
                        _delayedUpdateTimer?.Stop();
                        _delayedUpdateTimer?.Start();
                    }
                }
            }
        }
        
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Placeholder text shown when textbox is empty.")]
        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                if (_placeholderText != value)
                {
                    _placeholderText = value ?? "";
                    if (string.IsNullOrEmpty(_text))
                    {
                        Invalidate();
                    }
                }
            }
        }
        
        #endregion
        
        #region "Properties - Modern Features"
        
        private bool _enableSmartFeatures = true;
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Enable modern smart features like typing indicators and smooth animations.")]
        public bool EnableSmartFeatures
        {
            get => _enableSmartFeatures;
            set
            {
                _enableSmartFeatures = value;
                if (!value)
                {
                    _typingTimer?.Stop();
                    _animationTimer?.Stop();
                }
            }
        }
        
        private bool _enableFocusAnimation = true;
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Enable smooth focus border animation.")]
        public bool EnableFocusAnimation
        {
            get => _enableFocusAnimation;
            set => _enableFocusAnimation = value;
        }
        
        private bool _enableTypingIndicator = true;
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Enable visual typing indicator effects.")]
        public bool EnableTypingIndicator
        {
            get => _enableTypingIndicator;
            set => _enableTypingIndicator = value;
        }
        
        private int _maxLength = 0;
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(0)]
        [Description("Maximum number of characters allowed. 0 means no limit.")]
        public int MaxLength
        {
            get => _maxLength;
            set => _maxLength = Math.Max(0, value);
        }
        
        private bool _showCharacterCount = false;
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Show character count when MaxLength is set.")]
        public bool ShowCharacterCount
        {
            get => _showCharacterCount;
            set
            {
                _showCharacterCount = value;
                InvalidateLayout();
            }
        }
        
        private bool _enableSpellCheck = false;
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Enable basic spell checking (requires dictionary).")]
        public bool EnableSpellCheck
        {
            get => _enableSpellCheck;
            set => _enableSpellCheck = value;
        }
        
        #endregion
        
        #region "Properties - Appearance"
        
        private Font _textFont = new Font("Segoe UI", 10);
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                if (_textFont != value)
                {
                    _textFont?.Dispose();
                    _textFont = value ?? new Font("Segoe UI", 10);
                     UseThemeFont = false;
                    _helper?.InvalidateAllCaches();
                    // recompute cached metrics (no Graphics)
                    RecomputeMinHeight();
                    InvalidateLayout();
                }
            }
        }
        
        private Color _placeholderTextColor = Color.Gray;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color for placeholder text.")]
        public Color PlaceholderTextColor
        {
            get => _placeholderTextColor;
            set
            {
                _placeholderTextColor = value;
                if (_currentTheme != null)
                    _currentTheme.TextBoxPlaceholderColor = value;
                if (string.IsNullOrEmpty(_text))
                {
                    Invalidate();
                }
            }
        }
        
        private Color _focusBorderColor = Color.DodgerBlue;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Border color when the control has focus.")]
        public Color FocusBorderColor
        {
            get => _focusBorderColor;
            set
            {
                _focusBorderColor = value;
                if (Focused)
                {
                    Invalidate();
                }
            }
        }
        
        private int _borderWidth = 1;
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(1)]
        [Description("Width of the border in pixels.")]
        public int BorderWidth
        {
            get => _borderWidth;
            set
            {
                _borderWidth = Math.Max(0, Math.Min(value, 5));
                Invalidate();
            }
        }
        
        #endregion
        
        #region "Properties - Behavior"
        
        private bool _multiline = false;
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allow multiple lines of text.")]
        public bool Multiline
        {
            get => _multiline;
            set
            {
                _multiline = value;
                if (value)
                {
                    AcceptsReturn = true;
                }
                UpdateLines();
                InvalidateLayout();
            }
        }
        
        private bool _readOnly = false;
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Prevent text editing.")]
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                _readOnly = value;
                TabStop = !value;
                Invalidate();
            }
        }
        
        private bool _acceptsReturn = false;
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Accept Enter key for new lines.")]
        public bool AcceptsReturn
        {
            get => _acceptsReturn;
            set
            {
                _acceptsReturn = value;
                if (_multiline && !value)
                {
                    _acceptsReturn = true;
                }
            }
        }
        
        private bool _acceptsTab = false;
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Accept Tab key for indentation.")]
        public bool AcceptsTab
        {
            get => _acceptsTab;
            set => _acceptsTab = value;
        }
        
        private HorizontalAlignment _textAlignment = HorizontalAlignment.Left;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text alignment within the control.")]
        public HorizontalAlignment TextAlignment
        {
            get => _textAlignment;
            set
            {
                _textAlignment = value;
                Invalidate();
            }
        }
        
        private bool _wordWrap = false;
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Enable word wrapping in multiline mode.")]
        public bool WordWrap
        {
            get => _wordWrap;
            set
            {
                _wordWrap = value;
                UpdateLines();
                Invalidate();
            }
        }
        
        private bool _useSystemPasswordChar = false;
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Use system password character.")]
        public bool UseSystemPasswordChar
        {
            get => _useSystemPasswordChar;
            set
            {
                _useSystemPasswordChar = value;
                Invalidate();
            }
        }
        
        private char _passwordChar = '\0';
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Custom password character.")]
        public char PasswordChar
        {
            get => _passwordChar;
            set
            {
                _passwordChar = value;
                Invalidate();
            }
        }
        
        #endregion
        
        #region "Properties - Validation and Masking"
        
        [Browsable(true)]
        [Category("Validation")]
        [Description("Specifies the mask format for the text box.")]
        [DefaultValue(TextBoxMaskFormat.None)]
        public TextBoxMaskFormat MaskFormat
        {
            get => _helper?.Validation?.MaskFormat ?? TextBoxMaskFormat.None;
            set
            {
                if (_helper?.Validation != null)
                {
                    _helper.Validation.MaskFormat = value;
                    Invalidate();
                }
            }
        }
        
        [Browsable(true)]
        [Category("Validation")]
        [Description("Custom mask pattern for input validation.")]
        public string CustomMask
        {
            get => _helper?.Validation?.CustomMask ?? string.Empty;
            set
            {
                if (_helper?.Validation != null)
                {
                    _helper.Validation.CustomMask = value;
                    Invalidate();
                }
            }
        }
        
        [Browsable(true)]
        [Category("Validation")]
        [Description("Date format pattern for date masking.")]
        public string DateFormat
        {
            get => _helper?.Validation?.DateFormat ?? "MM/dd/yyyy";
            set
            {
                if (_helper?.Validation != null)
                {
                    _helper.Validation.DateFormat = value;
                    Invalidate();
                }
            }
        }
        
        [Browsable(true)]
        [Category("Validation")]
        [Description("Time format pattern for time masking.")]
        public string TimeFormat
        {
            get => _helper?.Validation?.TimeFormat ?? "HH:mm:ss";
            set
            {
                if (_helper?.Validation != null)
                {
                    _helper.Validation.TimeFormat = value;
                    Invalidate();
                }
            }
        }
        
        [Browsable(true)]
        [Category("Validation")]
        [Description("Date time format pattern for datetime masking.")]
        public string DateTimeFormat
        {
            get => _helper?.Validation?.DateTimeFormat ?? "MM/dd/yyyy HH:mm:ss";
            set
            {
                if (_helper?.Validation != null)
                {
                    _helper.Validation.DateTimeFormat = value;
                    Invalidate();
                }
            }
        }
        
        [Browsable(true)]
        [Category("Validation")]
        [Description("Allow only numeric input.")]
        public bool OnlyDigits
        {
            get => _helper?.Validation?.OnlyDigits ?? false;
            set
            {
                if (_helper?.Validation != null)
                {
                    _helper.Validation.OnlyDigits = value;
                }
            }
        }
        
        [Browsable(true)]
        [Category("Validation")]
        [Description("Allow only alphabetic characters.")]
        public bool OnlyCharacters
        {
            get => _helper?.Validation?.OnlyCharacters ?? false;
            set
            {
                if (_helper?.Validation != null)
                {
                    _helper.Validation.OnlyCharacters = value;
                }
            }
        }
        
        #endregion
        
        #region "Properties - AutoComplete"
        
        [Browsable(true)]
        [Category("AutoComplete")]
        [Description("AutoComplete mode for the textbox.")]
        public AutoCompleteMode AutoCompleteMode
        {
            get => _helper?.AutoComplete?.AutoCompleteMode ?? AutoCompleteMode.None;
            set
            {
                if (_helper?.AutoComplete != null)
                {
                    _helper.AutoComplete.AutoCompleteMode = value;
                }
            }
        }
        
        [Browsable(true)]
        [Category("AutoComplete")]
        [Description("AutoComplete source for the textbox.")]
        public AutoCompleteSource AutoCompleteSource
        {
            get => _helper?.AutoComplete?.AutoCompleteSource ?? AutoCompleteSource.None;
            set
            {
                if (_helper?.AutoComplete != null)
                {
                    _helper.AutoComplete.AutoCompleteSource = value;
                }
            }
        }
        
        [Browsable(true)]
        [Category("AutoComplete")]
        [Description("Custom AutoComplete source collection.")]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get => _helper?.AutoComplete?.AutoCompleteCustomSource;
            set
            {
                if (_helper?.AutoComplete != null)
                {
                    _helper.AutoComplete.AutoCompleteCustomSource = value;
                }
            }
        }
        
        #endregion
        
        #region "Properties - Scrolling"
        
        [Browsable(true)]
        [Category("Scrolling")]
        [DefaultValue(true)]
        [Description("Shows or hides the vertical scrollbar.")]
        public bool ShowVerticalScrollBar
        {
            get => _helper?.Scrolling?.ShowVerticalScrollBar ?? true;
            set
            {
                if (_helper?.Scrolling != null)
                {
                    _helper.Scrolling.ShowVerticalScrollBar = value;
                    Invalidate();
                }
            }
        }
        
        [Browsable(true)]
        [Category("Scrolling")]
        [DefaultValue(true)]
        [Description("Shows or hides the horizontal scrollbar.")]
        public bool ShowHorizontalScrollBar
        {
            get => _helper?.Scrolling?.ShowHorizontalScrollBar ?? true;
            set
            {
                if (_helper?.Scrolling != null)
                {
                    _helper.Scrolling.ShowHorizontalScrollBar = value;
                    Invalidate();
                }
            }
        }
        
        [Browsable(true)]
        [Category("Scrolling")]
        [Description("Shows scrollbars when content overflows.")]
        public bool ShowScrollbars
        {
            get => ShowVerticalScrollBar && ShowHorizontalScrollBar;
            set
            {
                ShowVerticalScrollBar = value;
                ShowHorizontalScrollBar = value;
            }
        }
        
        [Browsable(true)]
        [Category("Scrolling")]
        [Description("Type of scrollbars to show.")]
        public ScrollBars ScrollBars
        {
            get
            {
                if (ShowVerticalScrollBar && ShowHorizontalScrollBar) return ScrollBars.Both;
                if (ShowVerticalScrollBar) return ScrollBars.Vertical;
                if (ShowHorizontalScrollBar) return ScrollBars.Horizontal;
                return ScrollBars.None;
            }
            set
            {
                switch (value)
                {
                    case ScrollBars.None:
                        ShowVerticalScrollBar = false;
                        ShowHorizontalScrollBar = false;
                        break;
                    case ScrollBars.Horizontal:
                        ShowVerticalScrollBar = false;
                        ShowHorizontalScrollBar = true;
                        break;
                    case ScrollBars.Vertical:
                        ShowVerticalScrollBar = true;
                        ShowHorizontalScrollBar = false;
                        break;
                    case ScrollBars.Both:
                        ShowVerticalScrollBar = true;
                        ShowHorizontalScrollBar = true;
                        break;
                }
                Invalidate();
            }
        }
        
        #endregion
        
        #region "Properties - Selection and Caret"
        
        [Browsable(false)]
        public int SelectionStart
        {
            get => _helper?.Caret?.SelectionStart ?? 0;
            set
            {
                if (_helper?.Caret != null)
                {
                    _helper.Caret.SelectionStart = value;
                    _helper.Caret.CaretPosition = value;
                    Invalidate();
                }
            }
        }
        
        [Browsable(false)]
        public int SelectionLength
        {
            get => _helper?.Caret?.SelectionLength ?? 0;
            set
            {
                if (_helper?.Caret != null)
                {
                    _helper.Caret.SelectionLength = value;
                    Invalidate();
                }
            }
        }
        
        [Browsable(false)]
        public string SelectedText => _helper?.Caret?.SelectedText ?? string.Empty;
        
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Background color for selected text.")]
        public Color SelectionBackColor { get; set; } = SystemColors.Highlight;
        
        #endregion
        
        #region "Properties - Image"
        
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Path to the image file to display in the textbox.")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ImagePath
        {
            get => _beepImage?.ImagePath ?? "";
            set
            {
                if (_beepImage != null)
                {
                    _beepImage.ImagePath = value;
                    _beepImage.Visible = _imageVisible && !string.IsNullOrEmpty(value);
                    InvalidateLayout();
                }
            }
        }
        
        private Size _maxImageSize = new Size(20, 20);
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Size of the image displayed in the textbox.")]
        public Size MaxImageSize
        {
            get => _maxImageSize;
            set
            {
                _maxImageSize = value;
                if (_beepImage != null)
                {
                    _beepImage.Size = _maxImageSize;
                }
                InvalidateLayout();
            }
        }
        
        private TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Relationship between text and image.")]
        public TextImageRelation TextImageRelation
        {
            get => _textImageRelation;
            set
            {
                _textImageRelation = value;
                InvalidateLayout();
            }
        }
        
        private ContentAlignment _imageAlign = ContentAlignment.MiddleLeft;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Align image within the textbox.")]
        public ContentAlignment ImageAlign
        {
            get => _imageAlign;
            set
            {
                _imageAlign = value;
                InvalidateLayout();
            }
        }
        
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Apply theme styling to the image.")]
        public bool ApplyThemeOnImage
        {
            get => _beepImage?.ApplyThemeOnImage ?? false;
            set
            {
                if (_beepImage != null)
                {
                    _beepImage.ApplyThemeOnImage = value;
                    if (value && _currentTheme != null)
                    {
                        _beepImage.Theme = Theme;
                        _beepImage.ApplyTheme();
                    }
                }
                Invalidate();
            }
        }
        
        public BeepImage BeepImage => _beepImage;
        
        private bool _imageVisible = false;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Controls the visibility of the image in the textbox.")]
        public bool ImageVisible
        {
            get => _imageVisible;
            set
            {
                _imageVisible = value;
                if (_beepImage != null)
                {
                    _beepImage.Visible = _imageVisible && !string.IsNullOrEmpty(ImagePath);
                    InvalidateLayout();
                }
            }
        }
        
        private System.Windows.Forms.Padding _imageMargin = new System.Windows.Forms.Padding(2);
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Margin around the image.")]
        public System.Windows.Forms.Padding ImageMargin
        {
            get => _imageMargin;
            set
            {
                _imageMargin = value;
                InvalidateLayout();
            }
        }
        
        #endregion
        
        #region "Properties - Line Numbers"
        
        private bool _showLineNumbers = false;
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Shows or hides line numbers on the left side of the textbox.")]
        public bool ShowLineNumbers
        {
            get => _showLineNumbers;
            set
            {
                _showLineNumbers = value;
                InvalidateLayout();
            }
        }
        
        private int _lineNumberMarginWidth = 40;
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(40)]
        [Description("Width of the line number margin.")]
        public int LineNumberMarginWidth
        {
            get => _lineNumberMarginWidth;
            set
            {
                _lineNumberMarginWidth = Math.Max(20, value);
                if (_showLineNumbers)
                {
                    InvalidateLayout();
                }
            }
        }
        
        private Color _lineNumberForeColor = Color.Gray;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Foreground color for line numbers.")]
        public Color LineNumberForeColor
        {
            get => _lineNumberForeColor;
            set
            {
                _lineNumberForeColor = value;
                if (_showLineNumbers)
                    Invalidate();
            }
        }
        
        private Color _lineNumberBackColor = Color.FromArgb(248, 248, 248);
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Background color for line number margin.")]
        public Color LineNumberBackColor
        {
            get => _lineNumberBackColor;
            set
            {
                _lineNumberBackColor = value;
                if (_showLineNumbers)
                    Invalidate();
            }
        }
        
        private Font _lineNumberFont;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for line numbers.")]
        public Font LineNumberFont
        {
            get => _lineNumberFont ?? TextFont;
            set
            {
                _lineNumberFont?.Dispose();
                _lineNumberFont = value;
                if (_showLineNumbers)
                    Invalidate();
            }
        }
        
        #endregion
        
        #region "Properties - Legacy Compatibility"
        
        [Browsable(true)]
        [Category("Appearance")]
        public int PreferredHeight
        {
            get
            {
                if (_multiline)
                {
                    return Height;
                }
                else
                {
                    // use cached minimum height (updated on font change)
                    return Math.Max(1, _cachedMinHeightPx);
                }
            }
        }
        
        [Browsable(false)]
        public int SingleLineHeight
        {
            get
            {
                // use cached text height
                return Math.Max(1, _cachedTextHeightPx);
            }
        }
        
        [Browsable(true)]
        [Category("Behavior")]
        public bool CausesValidation { get; set; } = true;
        
        [Browsable(true)]
        [Category("Behavior")]
        public bool HideSelection { get; set; } = true;
        
        [Browsable(true)]
        [Category("Behavior")]
        public bool Modified { get; set; } = false;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DisableMaterialConstraints { get; set; } = false;
        
        #endregion
    }
}
