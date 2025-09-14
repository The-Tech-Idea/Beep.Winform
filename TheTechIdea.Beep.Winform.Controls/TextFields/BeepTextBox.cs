using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.TextFields.Helpers;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Description("A modern, high-performance text box control with advanced capabilities.")]
    [DisplayName("Beep TextBox")]
    [Category("Beep Controls")]
    public class BeepTextBox : BaseControl, IBeepTextBox
    {
        #region "Helper Instance"
        
        /// <summary>
        /// The main helper that coordinates all functionality
        /// </summary>
        private BeepSimpleTextBoxHelper _helper;
        
        /// <summary>
        /// Gets the helper instance for internal use
        /// </summary>
        internal BeepSimpleTextBoxHelper Helper => _helper;
        
        #endregion
        
        #region "Core Fields"
        
        private BeepImage _beepImage;
        private string _placeholderText = "";
        private Rectangle _textRect;
        private List<string> _lines = new List<string>();
        private bool _isInitializing = true;
        
        // Performance optimizations
        private Timer _delayedUpdateTimer;
        private bool _needsTextUpdate = false;
        private bool _needsLayoutUpdate = false;
        
        // Animation support
        private Timer _animationTimer;
        private float _focusAnimationProgress = 0f;
        private bool _isFocusAnimating = false;
        
        // Smart features
        private DateTime _lastKeyPressTime = DateTime.MinValue;
        private bool _isTyping = false;
        private Timer _typingTimer;
        
        #endregion
        
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
                // Layout is affected when character count visibility changes
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
        
        // Legacy properties for compatibility
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
                    _beepImage.Visible = !string.IsNullOrEmpty(value);
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
                    using (Graphics g = CreateGraphics())
                    {
                        return (int)Math.Ceiling(g.MeasureString("Aj", TextFont).Height) + Padding.Vertical + _borderWidth * 2;
                    }
                }
            }
        }
        
        [Browsable(false)]
        public int SingleLineHeight
        {
            get
            {
                using (Graphics g = CreateGraphics())
                {
                    return (int)Math.Ceiling(g.MeasureString("Aj", TextFont).Height);
                }
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

        // Allow callers to bypass Material size constraints when measuring preferred size
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DisableMaterialConstraints { get; set; } = false;
        
        #endregion
        
        #region "Events"
        
        public new event EventHandler TextChanged;
        public event EventHandler SearchTriggered;
        public event EventHandler<EventArgs> TypingStarted;
        public event EventHandler<EventArgs> TypingStopped;
        
        #endregion
        
        #region "Constructor"
        
        public BeepTextBox() : base()
        {
            _isInitializing = true;
            
            InitializeComponents();
            SetControlStyles();
            InitializeProperties();
            InitializeTimers();
            
            _helper = new BeepSimpleTextBoxHelper(this);
            
            UpdateDrawingRect();
            UpdateLines();
            
            _isInitializing = false;
        }
        
        #endregion
        
        #region "Initialization"
        
        private void InitializeComponents()
        {
            _beepImage = new BeepImage()
            {
                Size = _maxImageSize,
                ShowAllBorders = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                IsChild = true,
                ClipShape = ImageClipShape.None,
                Visible = false,
                IsFrameless = true,
                ImageEmbededin = ImageEmbededin.TextBox,
                ScaleMode = ImageScaleMode.KeepAspectRatio,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            
            if (_currentTheme != null)
            {
                _beepImage.Theme = _currentTheme.ToString();
                _beepImage.BackColor = _currentTheme.TextBoxBackColor;
            }
        }
        
        private void SetControlStyles()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.Selectable |
                     ControlStyles.StandardClick |
                     ControlStyles.StandardDoubleClick |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.ContainerControl, false);
         
        }
        
        private void InitializeProperties()
        {
            _text = string.Empty;
            BoundProperty = "Text";
         
            ShowAllBorders = true;
        
            TabStop = true;
          
         //   StylePreset = Models.MaterialTextFieldStylePreset.MaterialStandard;
            // Modern defaults
          //  Font = new Font("Segoe UI", 10);
          //  _textFont = Font;
        }
        
        private void InitializeTimers()
        {
            // Delayed update timer for performance
            _delayedUpdateTimer = new Timer()
            {
                Interval = 50 // 50ms delay for batching updates
            };
            _delayedUpdateTimer.Tick += DelayedUpdateTimer_Tick;
            
            // Animation timer
            _animationTimer = new Timer()
            {
                Interval = 16 // ~60 FPS
            };
            _animationTimer.Tick += AnimationTimer_Tick;
            
            // Typing timer
            _typingTimer = new Timer()
            {
                Interval = 1000 // 1 second
            };
            _typingTimer.Tick += TypingTimer_Tick;
        }
        
        protected override Size DefaultSize => new Size(200, 34);
        
        #endregion
        
        #region "Timer Event Handlers"
        
        private void DelayedUpdateTimer_Tick(object sender, EventArgs e)
        {
            _delayedUpdateTimer.Stop();
            
            if (_needsTextUpdate)
            {
                UpdateLines();
                _helper?.InvalidateAllCaches();
                _needsTextUpdate = false;
            }
            
            if (_needsLayoutUpdate)
            {
                UpdateDrawingRect();
                _helper?.HandleResize();
                _needsLayoutUpdate = false;
            }
            
            Invalidate();
        }
        
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (_isFocusAnimating && _enableFocusAnimation)
            {
                if (Focused)
                {
                    _focusAnimationProgress = Math.Min(1.0f, _focusAnimationProgress + 0.1f);
                }
                else
                {
                    _focusAnimationProgress = Math.Max(0.0f, _focusAnimationProgress - 0.1f);
                }
                
                if (_focusAnimationProgress <= 0.0f || _focusAnimationProgress >= 1.0f)
                {
                    _isFocusAnimating = false;
                    _animationTimer.Stop();
                }
                
                Invalidate();
            }
        }
        
        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            _typingTimer.Stop();
            _isTyping = false;
            TypingStopped?.Invoke(this, EventArgs.Empty);
            
            if (_enableTypingIndicator)
            {
                Invalidate();
            }
        }
        
        #endregion
        
        #region "Event Overrides"
        
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _helper?.HandleFocusGained();
            
            if (_enableFocusAnimation && _enableSmartFeatures)
            {
                _isFocusAnimating = true;
                _animationTimer.Start();
            }
            else
            {
                Invalidate();
            }
        }
        
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _helper?.HandleFocusLost();
            
            if (_enableFocusAnimation && _enableSmartFeatures)
            {
                _isFocusAnimating = true;
                _animationTimer.Start();
            }
            else
            {
                Invalidate();
            }
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!_readOnly)
            {
                Focus();
                _helper?.HandleMouseClick(e, _textRect);
            }
        }
        
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (_multiline)
            {
                _helper?.Scrolling?.HandleMouseWheel(e);
            }
        }
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _needsLayoutUpdate = true;
            _delayedUpdateTimer?.Stop();
            _delayedUpdateTimer?.Start();

            // Ensure single-line controls are not shorter than the font height + padding/border.
            try
            {
                if (!_multiline)
                {
                    using (Graphics g = CreateGraphics())
                    {
                        int textHeight = (int)Math.Ceiling(g.MeasureString("Aj", _textFont).Height);
                        int minHeight = textHeight + Padding.Vertical + (_borderWidth * 2);
                        // Don't force the Height; enforce minimum via MinimumSize so user-defined size is respected
                        if (MinimumSize.Height < minHeight)
                        {
                            MinimumSize = new Size(MinimumSize.Width, minHeight);
                        }
                    }
                }
            }
            catch
            {
                // Ignore measurement errors in design-time or without a device context
            }
        }
        
        #endregion
        
        #region "Key Handling"
        
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.Back:
                case Keys.Delete:
                    return true;
                case Keys.Enter:
                    RaiseSubmitChanges();
                    return _multiline && _acceptsReturn;
                case Keys.Tab:
                    RaiseSubmitChanges();
                    return _acceptsTab;
                case Keys.Up:
                case Keys.Down:
                    return _multiline;
                default:
                    return base.IsInputKey(keyData);
            }
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            if (_readOnly) return;
            
            // Start typing indicator
            if (!_isTyping && _enableTypingIndicator && _enableSmartFeatures)
            {
                _isTyping = true;
                _lastKeyPressTime = DateTime.Now;
                TypingStarted?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
            
            // Reset typing timer
            _typingTimer.Stop();
            _typingTimer.Start();
            
            // Handle keyboard shortcuts
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Z:
                        if (e.Shift)
                            _helper?.UndoRedo?.Redo();
                        else
                            _helper?.UndoRedo?.Undo();
                        e.Handled = true;
                        return;
                    case Keys.Y:
                        _helper?.UndoRedo?.Redo();
                        e.Handled = true;
                        return;
                    case Keys.A:
                        SelectAll();
                        e.Handled = true;
                        return;
                    case Keys.C:
                        Copy();
                        e.Handled = true;
                        return;
                    case Keys.V:
                        Paste();
                        e.Handled = true;
                        return;
                    case Keys.X:
                        Cut();
                        e.Handled = true;
                        return;
                }
            }
            
            // Handle navigation
            switch (e.KeyCode)
            {
                case Keys.Left:
                    _helper?.Caret?.MoveCaret(-1, e.Shift);
                    e.Handled = true;
                    break;
                case Keys.Right:
                    _helper?.Caret?.MoveCaret(1, e.Shift);
                    e.Handled = true;
                    break;
                case Keys.Up:
                    if (_multiline)
                    {
                        _helper?.Caret?.MoveCaretVertical(-1, e.Shift);
                        e.Handled = true;
                    }
                    break;
                case Keys.Down:
                    if (_multiline)
                    {
                        _helper?.Caret?.MoveCaretVertical(1, e.Shift);
                        e.Handled = true;
                    }
                    break;
                case Keys.Home:
                    HandleHomeKey(e.Control, e.Shift);
                    e.Handled = true;
                    break;
                case Keys.End:
                    HandleEndKey(e.Control, e.Shift);
                    e.Handled = true;
                    break;
                case Keys.Back:
                    HandleBackspace();
                    e.Handled = true;
                    break;
                case Keys.Delete:
                    HandleDelete();
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    if (_multiline && _acceptsReturn)
                    {
                        _helper?.InsertText(Environment.NewLine);
                        UpdateLines();
                        ValidateCaretPosition();
                        ScrollToCaret();
                        Invalidate();
                    }
                    else
                    {
                        SearchTriggered?.Invoke(this, EventArgs.Empty);
                    }
                    e.Handled = true;
                    break;
                case Keys.Tab:
                    if (_acceptsTab)
                    {
                        _helper?.InsertText("\t");
                        UpdateLines();
                        ValidateCaretPosition();
                        ScrollToCaret();
                        Invalidate();
                        e.Handled = true;
                    }
                    break;
            }
        }
        
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            
            if (_readOnly)
            {
                e.Handled = true;
                return;
            }
            
            if (char.IsControl(e.KeyChar))
            {
                return; // Already handled in OnKeyDown
            }
            
            // Check max length
            if (_maxLength > 0 && _text.Length >= _maxLength && _helper?.Caret?.SelectionLength == 0)
            {
                e.Handled = true;
                return;
            }
            
            // Insert the character
            _helper?.InsertText(e.KeyChar.ToString());
            
            // Ensure proper updates after character insertion
            UpdateLines();
            ValidateCaretPosition();
            
            // Auto-scroll for multiline
            if (_multiline)
            {
                ScrollToCaret();
            }
            
            Invalidate();
            e.Handled = true;
        }
        
        #endregion
        
        #region "Key Handling Helpers"
        
        private void HandleHomeKey(bool control, bool shift)
        {
            if (_helper?.Caret == null) return;
            
            int newPosition;
            if (control)
            {
                newPosition = 0;
            }
            else if (_multiline)
            {
                var lines = GetLines();
                var currentLineInfo = GetLineInfoFromPosition(_helper.Caret.CaretPosition, lines);
                newPosition = GetPositionFromLineInfo(currentLineInfo.LineIndex, 0, lines);
            }
            else
            {
                newPosition = 0;
            }
            
            _helper.Caret.CaretPosition = newPosition;
            if (!shift)
            {
                _helper.Caret.ClearSelection();
            }
            else
            {
                UpdateSelection(newPosition);
            }
        }
        // Override GetPreferredSize to bypass Material constraints when needed
        public override Size GetPreferredSize(Size proposedSize)
        {
            if (DisableMaterialConstraints)
            {
                // Use a simple text measurement for preferred size
                using (Graphics g = CreateGraphics())
                {
                    var textSize = g.MeasureString(string.IsNullOrEmpty(Text) ? PlaceholderText : Text, TextFont);
                    int width = (int)Math.Ceiling(textSize.Width) + Padding.Horizontal + (BorderWidth * 2);
                    int height = (int)Math.Ceiling(textSize.Height) + Padding.Vertical + (BorderWidth * 2);
                    return new Size(width, height);
                }
            }

            return base.GetPreferredSize(proposedSize);
        }

        

      
        private void HandleEndKey(bool control, bool shift)
        {
            if (_helper?.Caret == null) return;
            
            int newPosition;
            if (control)
            {
                newPosition = _text.Length;
            }
            else if (_multiline)
            {
                var lines = GetLines();
                var currentLineInfo = GetLineInfoFromPosition(_helper.Caret.CaretPosition, lines);
                newPosition = GetPositionFromLineInfo(currentLineInfo.LineIndex, lines[currentLineInfo.LineIndex].Length, lines);
            }
            else
            {
                newPosition = _text.Length;
            }
            
            _helper.Caret.CaretPosition = newPosition;
            if (!shift)
            {
                _helper.Caret.ClearSelection();
            }
            else
            {
                UpdateSelection(newPosition);
            }
        }
        
        private void UpdateSelection(int newCaretPosition)
        {
            if (_helper?.Caret == null) return;
            
            if (_helper.Caret.SelectionLength == 0)
            {
                _helper.Caret.SelectionStart = _helper.Caret.CaretPosition;
            }
            
            int selStart = Math.Min(_helper.Caret.SelectionStart, newCaretPosition);
            int selEnd = Math.Max(_helper.Caret.SelectionStart, newCaretPosition);
            
            _helper.Caret.SelectionStart = selStart;
            _helper.Caret.SelectionLength = selEnd - selStart;
            _helper.Caret.CaretPosition = newCaretPosition;
        }
        
        private void HandleBackspace()
        {
            if (_helper?.Caret?.HasSelection == true)
            {
                _helper.Caret.RemoveSelectedText();
            }
            else if (_helper?.Caret?.CaretPosition > 0)
            {
                _helper.DeleteText(_helper.Caret.CaretPosition - 1, 1);
            }
            
            // Ensure proper updates after deletion
            UpdateLines();
            ValidateCaretPosition();
            Invalidate();
        }
        
        private void HandleDelete()
        {
            if (_helper?.Caret?.HasSelection == true)
            {
                _helper.Caret.RemoveSelectedText();
            }
            else if (_helper?.Caret?.CaretPosition < _text.Length)
            {
                _helper.DeleteText(_helper.Caret.CaretPosition, 1);
            }
            
            // Ensure proper updates after deletion
            UpdateLines();
            ValidateCaretPosition();
            Invalidate();
        }
        
        #endregion
        
        #region "Text Operations"
        
        public void Copy()
        {
            if (!string.IsNullOrEmpty(SelectedText))
            {
                try
                {
                    Clipboard.SetText(SelectedText);
                }
                catch (ExternalException)
                {
                    // Clipboard access failed, ignore
                }
            }
        }
        
        public void Cut()
        {
            if (!string.IsNullOrEmpty(SelectedText) && !_readOnly)
            {
                Copy();
                _helper?.Caret?.RemoveSelectedText();
                // Ensure text is updated and lines are recalculated
                UpdateLines();
                ValidateCaretPosition();
                Invalidate();
            }
        }
        
        public void Paste()
        {
            if (!_readOnly && Clipboard.ContainsText())
            {
                try
                {
                    string clipboardText = Clipboard.GetText();
                    
                    // Handle multiline paste in single-line mode
                    if (!_multiline && clipboardText.Contains('\n'))
                    {
                        clipboardText = clipboardText.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                    }
                    
                    // Respect max length
                    if (_maxLength > 0)
                    {
                        int availableLength = _maxLength - _text.Length + (_helper?.Caret?.SelectionLength ?? 0);
                        if (clipboardText.Length > availableLength)
                        {
                            clipboardText = clipboardText.Substring(0, Math.Max(0, availableLength));
                        }
                    }
                    
                    _helper?.InsertText(clipboardText);
                    // Ensure proper updates after paste
                    UpdateLines();
                    ValidateCaretPosition();
                    ScrollToCaret();
                    Invalidate();
                }
                catch (ExternalException)
                {
                    // Clipboard access failed, ignore
                }
            }
        }
        
        public void SelectAll()
        {
            _helper?.Caret?.SelectAll();
            Invalidate();
        }
        
        public void Clear()
        {
            if (!_readOnly)
            {
                Text = "";
                if (_helper?.Caret != null)
                {
                    _helper.Caret.CaretPosition = 0;
                    _helper.Caret.SelectionStart = 0;
                    _helper.Caret.SelectionLength = 0;
                }
                UpdateLines();
                Invalidate();
            }
        }
        
        public void ScrollToCaret()
        {
            _helper?.Scrolling?.ScrollToCaret(_helper.Caret?.CaretPosition ?? 0);
        }
        
        /// <summary>
        /// Insert text at current caret position (public method for external use)
        /// </summary>
        public void InsertText(string text)
        {
            if (!_readOnly && !string.IsNullOrEmpty(text))
            {
                // Handle multiline insertion in single-line mode
                if (!_multiline && text.Contains('\n'))
                {
                    text = text.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                }
                
                _helper?.InsertText(text);
                UpdateLines();
                ValidateCaretPosition();
                ScrollToCaret();
                Invalidate();
            }
        }
        
        #endregion
        
        #region "Drawing"
        
        protected override void OnPaint(PaintEventArgs e)
        {
            // Enable high-quality rendering
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            base.OnPaint(e);
          
        }
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            // Draw focus animation border
            if (Focused && _enableFocusAnimation && _focusAnimationProgress > 0)
            {
                DrawFocusAnimation(g);
            }

            // Draw character count if enabled
            if (_showCharacterCount && _maxLength > 0)
            {
                DrawCharacterCount(g);
            }

            // Draw typing indicator
            if (_isTyping && _enableTypingIndicator && _enableSmartFeatures)
            {
                DrawTypingIndicator(g);
            }

            // Let helper draw the main content
            _helper?.DrawAll(g, ClientRectangle, _textRect);
        }
        private void DrawFocusAnimation(Graphics g)
        {
            if (_focusAnimationProgress <= 0) return;
            
            var focusRect = ClientRectangle;
            focusRect.Inflate(-1, -1);
            
            int alpha = (int)(255 * _focusAnimationProgress);
            using (var pen = new Pen(Color.FromArgb(alpha, _focusBorderColor), _borderWidth))
            {
                if (BorderRadius > 0)
                {
                    using (var path = GetRoundedRectPath(focusRect, BorderRadius))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    g.DrawRectangle(pen, focusRect);
                }
            }
        }
        
        private void DrawCharacterCount(Graphics g)
        {
            string countText = $"{_text.Length}/{_maxLength}";
            using (var font = new Font(_textFont.FontFamily, _textFont.Size * 0.8f))
            {
                var textSize = g.MeasureString(countText, font);
                var location = new PointF(Width - textSize.Width - 5, Height - textSize.Height - 2);
                
                Color textColor = _text.Length > _maxLength * 0.9 ? Color.Red : Color.Gray;
                using (var brush = new SolidBrush(textColor))
                {
                    g.DrawString(countText, font, brush, location);
                }
            }
        }
        
        private void DrawTypingIndicator(Graphics g)
        {
            // Simple typing indicator - small dot in corner
            var indicatorRect = new Rectangle(Width - 12, Height - 12, 8, 8);
            using (var brush = new SolidBrush(Color.Green))
            {
                g.FillEllipse(brush, indicatorRect);
            }
        }
        
        #endregion
        
        #region "Helper Methods"
        
        private void UpdateDrawingRect()
        {
            // Always keep BaseControl's drawing rectangles up to date
            base.UpdateDrawingRect();

            // Prefer Material content rectangle when Material style is enabled
            Rectangle contentArea;
            if (EnableMaterialStyle)
            {
                // Ensure material helper layout is updated before fetching rects
                UpdateMaterialLayout();
                contentArea = GetMaterialContentRectangle();

                // Fallback if material content rect is empty
                if (contentArea.Width <= 0 || contentArea.Height <= 0)
                {
                    contentArea = DrawingRect;
                }
            }
            else
            {
                contentArea = DrawingRect;
            }

            _textRect = contentArea;

            // Apply padding and border offsets inside the content area
            int borderOffset = Math.Max(0, _borderWidth);
            _textRect.Inflate(-(Padding.Horizontal / 2 + borderOffset), -(Padding.Vertical / 2 + borderOffset));
            
            // Reserve space for line numbers when enabled and multiline
            if (_showLineNumbers && _multiline)
            {
                _textRect.X += _lineNumberMarginWidth;
                _textRect.Width = Math.Max(1, _textRect.Width - _lineNumberMarginWidth);
            }
            
            // Reserve space for character count
            if (_showCharacterCount && _maxLength > 0)
            {
                // Compute character count height dynamically so layout respects DPI and font size
                try
                {
                    using (var g = CreateGraphics())
                    using (var font = new Font(_textFont.FontFamily, _textFont.Size * 0.8f))
                    {
                        var countText = $"{_text.Length}/{_maxLength}";
                        var size = g.MeasureString(countText, font);
                        int reserve = Math.Max(12, (int)Math.Ceiling(size.Height) + 4);
                        _textRect.Height = Math.Max(1, _textRect.Height - reserve);
                    }
                }
                catch
                {
                    _textRect.Height = Math.Max(1, _textRect.Height - 15);
                }
            }
        }
        
        private void InvalidateLayout()
        {
            _needsLayoutUpdate = true;
            _delayedUpdateTimer?.Stop();
            _delayedUpdateTimer?.Start();
        }
        
        private void OnTextChangedInternal(string oldText, string newText)
        {
            Modified = oldText != newText;
            
            TextChanged?.Invoke(this, EventArgs.Empty);
        }
        
        public string GetDisplayText()
        {
            string actualText = GetActualText();
            
            if (string.IsNullOrEmpty(actualText))
            {
                return string.IsNullOrEmpty(PlaceholderText) ? "" : PlaceholderText;
            }
            
            return actualText;
        }
        
        private string GetActualText()
        {
            if (string.IsNullOrEmpty(_text) ||
                _text.StartsWith("BeepTextBox") ||
                _text.Equals(Name) ||
                (_isInitializing && string.IsNullOrWhiteSpace(_text)))
            {
                return string.Empty;
            }
            
            // Handle password characters
            if (_useSystemPasswordChar && !string.IsNullOrEmpty(_text))
            {
                return new string('•', _text.Length);
            }
            else if (_passwordChar != '\0' && !string.IsNullOrEmpty(_text))
            {
                return new string(_passwordChar, _text.Length);
            }
            
            return _text;
        }
        
        public List<string> GetLines() => new List<string>(_lines);
        
        private (int LineIndex, int ColumnIndex) GetLineInfoFromPosition(int position, List<string> lines)
        {
            if (lines.Count == 0) return (0, 0);
            if (position <= 0) return (0, 0);

            int currentPos = 0;

            for (int i = 0; i < lines.Count; i++)
            {
                int lineLength = lines[i].Length;
                int lineEndPos = currentPos + lineLength;

                if (position <= lineEndPos)
                {
                    return (i, position - currentPos);
                }

                currentPos = lineEndPos;

                if (i < lines.Count - 1)
                {
                    if (currentPos < _text.Length)
                    {
                        if (currentPos + 1 < _text.Length &&
                            _text[currentPos] == '\r' && _text[currentPos + 1] == '\n')
                        {
                            currentPos += 2;
                        }
                        else if (_text[currentPos] == '\n' || _text[currentPos] == '\r')
                        {
                            currentPos += 1;
                        }
                    }
                }
            }

            int lastLineIndex = lines.Count - 1;
            return (lastLineIndex, lines[lastLineIndex].Length);
        }
        
        private int GetPositionFromLineInfo(int lineIndex, int columnIndex, List<string> lines)
        {
            if (lineIndex >= lines.Count) return _text.Length;
            if (lineIndex < 0) return 0;

            int position = 0;
            int currentLine = 0;
            int i = 0;

            while (i < _text.Length && currentLine < lineIndex)
            {
                if (_text[i] == '\r')
                {
                    if (i + 1 < _text.Length && _text[i + 1] == '\n')
                    {
                        i += 2;
                    }
                    else
                    {
                        i += 1;
                    }
                    currentLine++;
                }
                else if (_text[i] == '\n')
                {
                    i += 1;
                    currentLine++;
                }
                else
                {
                    i += 1;
                }
            }

            position = i;
            columnIndex = Math.Max(0, Math.Min(columnIndex, lines[lineIndex].Length));
            position += columnIndex;

            return Math.Min(position, _text.Length);
        }
        
        /// <summary>
        /// Validates and corrects caret position after text changes
        /// </summary>
        private void ValidateCaretPosition()
        {
            if (_helper?.Caret != null)
            {
                int textLength = _text?.Length ?? 0;
                
                // Ensure caret position is within valid range
                if (_helper.Caret.CaretPosition > textLength)
                {
                    _helper.Caret.CaretPosition = textLength;
                }
                
                // Ensure selection is within valid range
                if (_helper.Caret.SelectionStart > textLength)
                {
                    _helper.Caret.SelectionStart = textLength;
                    _helper.Caret.SelectionLength = 0;
                }
                else if (_helper.Caret.SelectionStart + _helper.Caret.SelectionLength > textLength)
                {
                    _helper.Caret.SelectionLength = textLength - _helper.Caret.SelectionStart;
                }
            }
        }
        
        private void UpdateLines()
        {
            _lines.Clear();
            
            if (string.IsNullOrEmpty(_text))
            {
                _lines.Add("");
                return;
            }
            
            if (_multiline)
            {
                // Handle all types of line endings properly
                string[] splitLines = _text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                _lines.AddRange(splitLines);
                
                // Handle word wrapping if enabled and we have a valid text rectangle
                if (_wordWrap && _textRect.Width > 10) // Minimum width check
                {
                    var wrappedLines = new List<string>();
                    using (var g = CreateGraphics())
                    {
                        foreach (var line in _lines)
                        {
                            if (string.IsNullOrEmpty(line))
                            {
                                wrappedLines.Add("");
                            }
                            else
                            {
                                wrappedLines.AddRange(WrapLine(line, g));
                            }
                        }
                    }
                    _lines = wrappedLines;
                }
            }
            else
            {
                // For single line, replace all line breaks with spaces
                string singleLineText = _text.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                _lines.Add(singleLineText);
            }
            
            // Ensure we always have at least one line
            if (_lines.Count == 0)
                _lines.Add("");
        }
        
        private List<string> WrapLine(string line, Graphics g)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(line))
            {
                result.Add("");
                return result;
            }
            
            // Check if the line fits without wrapping
            var lineSize = g.MeasureString(line, _textFont);
            if (lineSize.Width <= _textRect.Width)
            {
                result.Add(line);
                return result;
            }
            
            // Word-based wrapping
            var words = line.Split(' ');
            var currentLine = "";
            
            foreach (var word in words)
            {
                var testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
                var testSize = g.MeasureString(testLine, _textFont);
                
                if (testSize.Width <= _textRect.Width)
                {
                    currentLine = testLine;
                }
                else
                {
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        result.Add(currentLine);
                        currentLine = word;
                    }
                    else
                    {
                        // Word is too long for the line, check if we need character-level wrapping
                        if (g.MeasureString(word, _textFont).Width > _textRect.Width)
                        {
                            result.AddRange(WrapLongWord(word, g));
                            currentLine = "";
                        }
                        else
                        {
                            result.Add(word);
                            currentLine = "";
                        }
                    }
                }
            }
            
            if (!string.IsNullOrEmpty(currentLine))
            {
                result.Add(currentLine);
            }
            
            return result;
        }
        
        private List<string> WrapLongWord(string word, Graphics g)
        {
            var result = new List<string>();
            var currentPart = "";
            
            foreach (char c in word)
            {
                var testPart = currentPart + c;
                var testSize = g.MeasureString(testPart, _textFont);
                
                if (testSize.Width <= _textRect.Width)
                {
                    currentPart = testPart;
                }
                else
                {
                    if (!string.IsNullOrEmpty(currentPart))
                    {
                        result.Add(currentPart);
                        currentPart = c.ToString();
                    }
                    else
                    {
                        // Even a single character doesn't fit, add it anyway
                        result.Add(c.ToString());
                        currentPart = "";
                    }
                }
            }
            
            if (!string.IsNullOrEmpty(currentPart))
            {
                result.Add(currentPart);
            }
            
            return result;
        }
        
        public void AppendText(string text)
        {
            if (!_readOnly && !string.IsNullOrEmpty(text))
            {
                string newText = _text + text;
                
                // Respect max length
                if (_maxLength > 0 && newText.Length > _maxLength)
                {
                    newText = newText.Substring(0, _maxLength);
                }
                
                // Update text property (this will trigger UpdateLines)
                Text = newText;
                
                // Update caret position to end of text
                if (_helper?.Caret != null)
                {
                    _helper.Caret.CaretPosition = _text.Length;
                    _helper.Caret.SelectionStart = _text.Length;
                    _helper.Caret.SelectionLength = 0;
                }
                
                // For multiline controls, ensure the new content is visible
                if (_multiline)
                {
                    ScrollToCaret();
                }
                
                // Ensure visual update
                Invalidate();
            }
        }
        
        #endregion
        
        #region "IBeepComponent Implementation"
        
        public bool ValidateData(out string message)
        {
            return _helper?.ValidateAllData(out message) ?? EntityHelper.ValidateData(MaskFormat, Text, CustomMask, out message);
        }
        
        public override void SetValue(object value)
        {
            Text = value?.ToString() ?? string.Empty;
        }
        
        public override object GetValue()
        {
            return Text;
        }
        
        public override void ClearValue()
        {
            Text = string.Empty;
        }
        
        #endregion
        
        #region "IBeepTextBox Implementation"
        
        void IBeepTextBox.Focus()
        {
            Focus();
        }
        
        #endregion
        
        #region "Theme and Style"
        
        public override void ApplyTheme()
        {
            if (_currentTheme == null) return;
            base.ApplyTheme();
            
            if (IsChild && Parent != null)
            {
                ParentBackColor = Parent.BackColor;
                BackColor = ParentBackColor;
            }
            else
            {
                BackColor = _currentTheme.TextBoxBackColor;
            }

            ForeColor = _currentTheme.TextBoxForeColor;
            SelectedBackColor = _currentTheme.TextBoxBackColor;
            SelectedForeColor = _currentTheme.TextBoxForeColor;
            HoverBackColor = _currentTheme.TextBoxHoverBackColor;
            HoverForeColor = _currentTheme.TextBoxHoverForeColor;
            DisabledBackColor = _currentTheme.DisabledBackColor;
            DisabledForeColor = _currentTheme.DisabledForeColor;
            BorderColor = _currentTheme.BorderColor;
            _focusBorderColor = _currentTheme.FocusIndicatorColor;
            _placeholderTextColor = _currentTheme.TextBoxPlaceholderColor;
            
            if (UseThemeFont)
            {
                _textFont?.Dispose();
                _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
            }
            
            if (_beepImage != null)
            {
                _beepImage.IsChild = true;
                _beepImage.ImageEmbededin = ImageEmbededin.TextBox;
                _beepImage.ParentBackColor = BackColor; 
                _beepImage.BackColor = BackColor;
                _beepImage.ForeColor = _currentTheme.TextBoxForeColor;
                _beepImage.BorderColor = _currentTheme.BorderColor;
                _beepImage.HoverBackColor = _currentTheme.TextBoxHoverBackColor;
                _beepImage.HoverForeColor = _currentTheme.TextBoxHoverForeColor;
                _beepImage.ShowAllBorders = false;
                _beepImage.IsFrameless = true;
                _beepImage.IsBorderAffectedByTheme = false;
                _beepImage.IsShadowAffectedByTheme = false;
                _beepImage.BorderColor = _currentTheme.TextBoxBorderColor;
                 
                if (ApplyThemeOnImage)
                {
                    _beepImage.ApplyThemeOnImage = ApplyThemeOnImage;
                    _beepImage.ApplyThemeToSvg();
                }
            }
            
            // Recompute text rectangle after theme changes (padding, material, etc.)
            UpdateDrawingRect();
            Invalidate();
        }
        
        #endregion
        
        #region "Cleanup"
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _delayedUpdateTimer?.Stop();
                _delayedUpdateTimer?.Dispose();
                _animationTimer?.Stop();
                _animationTimer?.Dispose();
                _typingTimer?.Stop();
                _typingTimer?.Dispose();
                
                _helper?.Dispose();
                _textFont?.Dispose();
                _lineNumberFont?.Dispose();
            }
            base.Dispose(disposing);
        }
        
        #endregion
        
        #region "Draw Override"
        
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            if (graphics == null || rectangle.Width <= 0 || rectangle.Height <= 0)
                return;

            // High-quality rendering for drawing mode
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Use material content rect if available; otherwise base on provided rectangle
            Rectangle textRect = rectangle;
            if (EnableMaterialStyle)
            {
                // Approximate content area within the provided rectangle
                var padding = GetMaterialStylePadding();
                var effects = GetMaterialEffectsSpace();
                textRect = new Rectangle(
                    rectangle.X + padding.Left + effects.Width / 2,
                    rectangle.Y + padding.Top + effects.Height / 2,
                    Math.Max(0, rectangle.Width - padding.Horizontal - effects.Width),
                    Math.Max(0, rectangle.Height - padding.Vertical - effects.Height)
                );
            }

            // Apply internal padding and border
            int borderOffset = Math.Max(0, _borderWidth);
            textRect.Inflate(-(Padding.Horizontal / 2 + borderOffset), -(Padding.Vertical / 2 + borderOffset));
            
            if (_showLineNumbers && _multiline)
            {
                textRect.X += _lineNumberMarginWidth;
                textRect.Width = Math.Max(1, textRect.Width - _lineNumberMarginWidth);
            }

            if (_showCharacterCount && _maxLength > 0)
            {
                // Measure using the provided graphics so drawing-mode layout matches runtime layout
                using (var font = new Font(_textFont.FontFamily, _textFont.Size * 0.8f))
                {
                    try
                    {
                        var countText = $"{_text.Length}/{_maxLength}";
                        var size = graphics.MeasureString(countText, font);
                        int reserve = Math.Max(12, (int)Math.Ceiling(size.Height) + 4);
                        textRect.Height = Math.Max(1, textRect.Height - reserve);
                    }
                    catch
                    {
                        textRect.Height = Math.Max(1, textRect.Height - 15);
                    }
                }
            }

            _helper?.DrawAll(graphics, rectangle, textRect);
        }
        
        #endregion
    }
}
