using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.TextFields.Helpers;
 

namespace TheTechIdea.Beep.Winform.Controls.TextFields
{
    [ToolboxItem(true)]
    [Description("A modern Material Design text box control with advanced capabilities powered by helper classes.")]
    [DisplayName("Beep Material TextBox")]
    [Category("Beep Controls")]
    public partial class BeepMaterialTextField : BeepControl, IBeepTextBox
    {
        #region Private Fields
        private BeepMaterialTextFieldHelper _materialHelper;
        private TextBoxValidationHelper _validationHelper;
        private MaterialTextFieldDrawingHelper _drawingHelper;
        private SmartAutoCompleteHelper _autoCompleteHelper;
        private TextBoxAdvancedEditingHelper _advancedEditingHelper;
        private MaterialTextFieldInputHelper _inputHelper;
        private MaterialTextFieldCaretHelper _caretHelper;
        
        private BeepImage _beepImage;
        private string _placeholderText = "Enter text";
        
        // Material Design specific fields
        private MaterialTextFieldVariant _variant = MaterialTextFieldVariant.Outlined;
        private MaterialTextFieldDensity _density = MaterialTextFieldDensity.Standard;
        private string _labelText = string.Empty;
        private string _helperText = string.Empty;
        private string _errorText = string.Empty;
        private string _prefixText = string.Empty;
        private string _suffixText = string.Empty;
        private bool _isRequired = false;
        private bool _isFocused = false;
        private bool _hasError = false;
        private bool _showCharacterCounter = false;
        
        // Animation fields
        private Timer _animationTimer;
        private float _labelAnimationProgress = 0f;
        private float _focusAnimationProgress = 0f;
        
        // Text handling fields (following BeepSimpleTextBox pattern)
        private Rectangle _textRect;
        private List<string> _lines = new List<string>();
        private bool _isInitializing = true;
        
        // Text behavior fields
        private bool _multiline = false;
        private bool _readOnly = false;
        private int _maxLength = 0;
        private char _passwordChar = '\0';
        private bool _useSystemPasswordChar = false;
        
        // Icon fields
        private string _leadingIconPath = string.Empty;
        private string _trailingIconPath = string.Empty;
        private bool _showClearButton = false;

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Applies a predefined style preset that configures variant, density, radius, fill, and helper/label behavior.")]
        [DefaultValue(MaterialTextFieldStylePreset.Default)]
        public MaterialTextFieldStylePreset StylePreset
        {
            get => _stylePreset;
            set
            {
                if (_stylePreset == value) return;
                _stylePreset = value;
                ApplyStylePreset(_stylePreset);
            }
        }
        private MaterialTextFieldStylePreset _stylePreset = MaterialTextFieldStylePreset.Default;

        #endregion

        #region Constructor
        public BeepMaterialTextField() : base()
        {
            _isInitializing = true;
            
            InitializeComponent();
            InitializeHelpers();
            SetupDefaults();
            
            UpdateDrawingRect();
            UpdateLines();
            
            _isInitializing = false;
        }

        private void InitializeComponent()
        {
            // Initialize image component
            _beepImage = new BeepImage()
            {
                Size = new Size(24, 24),
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

            // Initialize animation timer
            _animationTimer = new Timer { Interval = 16 }; // 60 FPS
            _animationTimer.Tick += AnimationTimer_Tick;

            // Set control styles for optimal rendering
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.Selectable |
                     ControlStyles.StandardClick |
                     ControlStyles.StandardDoubleClick, true);
            SetStyle(ControlStyles.ContainerControl, false);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        private void InitializeHelpers()
        {
            _materialHelper = new BeepMaterialTextFieldHelper(this);
            _validationHelper = new TextBoxValidationHelper(this);
            _drawingHelper = new MaterialTextFieldDrawingHelper(this, _materialHelper);
            _autoCompleteHelper = new SmartAutoCompleteHelper(this);
            _advancedEditingHelper = new TextBoxAdvancedEditingHelper(this);
            _caretHelper = new MaterialTextFieldCaretHelper(this, _materialHelper);
            _inputHelper = new MaterialTextFieldInputHelper(this, _materialHelper);
        }

        private void SetupDefaults()
        {
            Size = GetDefaultSizeForDensity(_density); // Material Design standard with density support
            TabStop = true;
            BoundProperty = "Text";
            BorderRadius = 4;
            ShowAllBorders = true;
            IsShadowAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            
            // Initialize properties
            _text = string.Empty;
            
            // Material Design specific defaults
            MaterialBorderVariant = MaterialTextFieldVariant.Outlined;
            EnableRippleEffect = true;

            // Ensure initial height is sized to current font if single-line
            AutoAdjustHeight();
        }

        /// <summary>
        /// Get default size based on Material Design density
        /// </summary>
        private Size GetDefaultSizeForDensity(MaterialTextFieldDensity density)
        {
            return density switch
            {
                MaterialTextFieldDensity.Dense => new Size(280, 56),
                MaterialTextFieldDensity.Comfortable => new Size(280, 88),
                _ => new Size(280, 72) // Standard
            };
        }

        /// <summary>
        /// Update drawing rectangle for text content (following BeepSimpleTextBox pattern)
        /// </summary>
        private void UpdateDrawingRect()
        {
            _textRect = ClientRectangle;
            _textRect.Inflate(-Padding.Horizontal / 2, -Padding.Vertical / 2);
            
            // Reserve space for Material Design elements
            _materialHelper?.UpdateLayout();
        }

        /// <summary>
        /// Update lines collection (following BeepSimpleTextBox pattern)
        /// </summary>
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
                string[] splitLines = _text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                _lines.AddRange(splitLines);
            }
            else
            {
                string singleLineText = _text.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                _lines.Add(singleLineText);
            }
            
            if (_lines.Count == 0)
                _lines.Add("");
        }

        protected override Size DefaultSize => GetDefaultSizeForDensity(_density);
        #endregion

        #region Public Properties - Text Content
        
        [Browsable(true)]
        [Category("Text")]
        [Description("The text content of the text field.")]
        public override string Text
        {
            get => _text;
            set
            {
                if (_isInitializing || value != null && value.StartsWith("beepMaterialTextField"))
                {
                    _text = "";
                    return;
                }

                if (_text != value)
                {
                    _text = value ?? "";
                    UpdateLines();
                    UpdateLabelAnimation();
                    OnTextChanged(EventArgs.Empty);
                    Invalidate();
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
                _placeholderText = value ?? string.Empty;
                Invalidate();
            }
        }

        #endregion

        #region Public Properties - Material Design

        [Browsable(true)]
        [Category("Material Design")]
        [Description("The Material Design variant of the text field.")]
        [DefaultValue(MaterialTextFieldVariant.Outlined)]
        public MaterialTextFieldVariant Variant
        {
            get => _variant;
            set
            {
                if (_variant != value)
                {
                    _variant = value;
                    MaterialBorderVariant = value;
                    _materialHelper?.ApplyVariant(value);
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("The density of the text field, affecting its vertical padding and placeholder animation.")]
        [DefaultValue(MaterialTextFieldDensity.Standard)]
        public MaterialTextFieldDensity Density
        {
            get => _density;
            set
            {
                if (_density != value)
                {
                    _density = value;
                    Size = GetDefaultSizeForDensity(_density);
                    _materialHelper?.UpdateLayout();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("The floating label text.")]
        public string LabelText
        {
            get => _labelText;
            set
            {
                _labelText = value ?? string.Empty;
                UpdateLabelAnimation();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Helper text shown below the text field.")]
        public string HelperText
        {
            get => _helperText;
            set
            {
                _helperText = value ?? string.Empty;
                _materialHelper?.UpdateLayout();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Error text shown when validation fails.")]
        public string ErrorText
        {
            get => _errorText;
            set
            {
                _errorText = value ?? string.Empty;
                _hasError = !string.IsNullOrEmpty(_errorText);
                _materialHelper?.UpdateLayout();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Text that appears before the input text.")]
        public string PrefixText
        {
            get => _prefixText;
            set
            {
                _prefixText = value ?? string.Empty;
                _materialHelper?.UpdateLayout();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Text that appears after the input text.")]
        public string SuffixText
        {
            get => _suffixText;
            set
            {
                _suffixText = value ?? string.Empty;
                _materialHelper?.UpdateLayout();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Whether the field is required.")]
        [DefaultValue(false)]
        public bool IsRequired
        {
            get => _isRequired;
            set
            {
                _isRequired = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Show character counter based on MaxLength property.")]
        [DefaultValue(false)]
        public bool ShowCharacterCounter
        {
            get => _showCharacterCounter;
            set
            {
                _showCharacterCounter = value;
                _materialHelper?.UpdateLayout();
                Invalidate();
            }
        }

        #endregion

        #region Public Properties - Enhanced Icon Support

        [Browsable(true)]
        [Category("Icons")]
        [Description("SVG path for the leading (left) icon.")]
        [TypeConverter(typeof(BeepImagesPathConverter))]
        public string LeadingIconPath
        {
            get => _leadingIconPath;
            set
            {
                _leadingIconPath = value ?? string.Empty;
                _materialHelper?.UpdateIcons();
                UpdateDrawingRect(); // Recalculate text area when icon changes
                AutoAdjustHeight();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Icons")]
        [Description("SVG path for the trailing (right) icon.")]
        [TypeConverter(typeof(BeepImagesPathConverter))]
        public string TrailingIconPath
        {
            get => _trailingIconPath;
            set
            {
                _trailingIconPath = value ?? string.Empty;
                _materialHelper?.UpdateIcons();
                UpdateDrawingRect(); // Recalculate text area when icon changes
                AutoAdjustHeight();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Icons")]
        [Description("Image path for the leading (left) icon - alternative to SVG path.")]
        public string LeadingImagePath { get; set; } = string.Empty;

        [Browsable(true)]
        [Category("Icons")]
        [Description("Image path for the trailing (right) icon - alternative to SVG path.")]
        public string TrailingImagePath { get; set; } = string.Empty;

        [Browsable(true)]
        [Category("Icons")]
        [Description("Show clear button when field has content.")]
        [DefaultValue(false)]
        public bool ShowClearButton
        {
            get => _showClearButton;
            set
            {
                _showClearButton = value;
                _materialHelper?.UpdateIcons();
                UpdateDrawingRect(); // Recalculate text area when clear button changes
                AutoAdjustHeight();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Icons")]
        [Description("Enable click events for the leading icon.")]
        [DefaultValue(true)]
        public bool LeadingIconClickable { get; set; } = true;

        [Browsable(true)]
        [Category("Icons")]
        [Description("Enable click events for the trailing icon.")]
        [DefaultValue(true)]
        public bool TrailingIconClickable { get; set; } = true;

        [Browsable(true)]
        [Category("Icons")]
        [Description("Size of the icons in pixels.")]
        [DefaultValue(24)]
        public int IconSize { get; set; } = 24;

        [Browsable(true)]
        [Category("Icons")]
        [Description("Padding between icons and text.")]
        [DefaultValue(8)]
        public int IconPadding { get; set; } = 8;

        [Browsable(false)]
        public BeepImage BeepImage => _beepImage;

        /// <summary>
        /// Check if the field has any content for clear button logic
        /// </summary>
        [Browsable(false)]
        public bool HasContent => !string.IsNullOrEmpty(_text);

        #endregion

        #region Public Properties - Curved Styling

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Border radius for curved appearance. Set to 0 for square corners.")]
        [DefaultValue(4)]
        public int CurvedBorderRadius { get; set; } = 4;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Enable search box style with high border radius.")]
        [DefaultValue(false)]
        public bool SearchBoxStyle
        {
            get => CurvedBorderRadius >= 20;
            set
            {
                if (value)
                {
                    CurvedBorderRadius = Math.Max(Height / 2, 20);
                }
                else
                {
                    CurvedBorderRadius = 4;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Background fill color for filled appearance.")]
        public Color FillColor { get; set; } = Color.FromArgb(245, 245, 245);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Enable background fill.")]
        [DefaultValue(true)]
        public bool ShowFill { get; set; } = true;

        #endregion
        
        #region Public Properties - Behavior

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allow multiple lines of text.")]
        [DefaultValue(false)]
        public bool Multiline
        {
            get => _multiline;
            set
            {
                _multiline = value;
                _materialHelper?.UpdateLayout();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Prevent text editing.")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                _readOnly = value;
                Cursor = value ? Cursors.Default : Cursors.IBeam;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Maximum number of characters allowed.")]
        [DefaultValue(0)]
        public int MaxLength
        {
            get => _maxLength;
            set => _maxLength = Math.Max(0, value);
        }

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

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Use system password character.")]
        [DefaultValue(false)]
        public bool UseSystemPasswordChar
        {
            get => _useSystemPasswordChar;
            set
            {
                _useSystemPasswordChar = value;
                Invalidate();
            }
        }

        #endregion

        #region Public Properties - Layout
        [Browsable(true)]
        [Category("Layout")]
        [Description("Automatically adjusts control Height for single-line mode based on current font and icon size.")]
        [DefaultValue(true)]
        public bool AutoSizeHeight { get; set; } = true;
        #endregion

        #region Internal Properties for Helpers

        internal bool IsFocused => _isFocused;
        internal bool HasError => _hasError;
        internal float LabelAnimationProgress => _labelAnimationProgress;
        internal float FocusAnimationProgress => _focusAnimationProgress;
        internal Timer AnimationTimer => _animationTimer;
     

        #endregion

        #region Events
        public new event EventHandler TextChanged;
        public event EventHandler LeadingIconClicked;
        public event EventHandler TrailingIconClicked;
        public event EventHandler ClearButtonClicked;
        #endregion

        #region Helper Methods

        private void UpdateLabelAnimation()
        {
            bool shouldFloat = _isFocused || !string.IsNullOrEmpty(_text) || !string.IsNullOrEmpty(_placeholderText);
            _materialHelper?.StartLabelAnimation(shouldFloat);
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            _materialHelper?.UpdateAnimations();
        }

        /// <summary>
        /// Computes ideal minimum height from current TextFont and updates MinimumSize.
        /// Also adjusts Height in single-line mode when AutoSizeHeight is true.
        /// </summary>
        private void AutoAdjustHeight()
        {
            // Always compute minimum height from font to avoid clipping
            int baseHeight;
            int labelHeight;
            int helperTextHeight;
            int spacing;

            using (var g = CreateGraphics())
            {
                baseHeight = TextRenderer.MeasureText(g, "Ag", this.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                float labelScale = 0.75f; // default floating label scale
                labelHeight = (int)Math.Ceiling(baseHeight * labelScale);
                using var helperFont = new Font(this.Font.FontFamily, Math.Max(8f, this.Font.Size - 1f));
                helperTextHeight = TextRenderer.MeasureText(g, "Ag", helperFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                spacing = Math.Max(4, baseHeight / 4);
            }

            int iconHeight = (!string.IsNullOrEmpty(_leadingIconPath) || !string.IsNullOrEmpty(_trailingIconPath) || _showClearButton) ? IconSize : 0;
            int contentHeight = Math.Max(baseHeight, iconHeight);

            int topPad, bottomPad;
            if (SearchBoxStyle || CurvedBorderRadius > 10)
            {
                topPad = Math.Max(2, spacing / 2);
                bottomPad = Math.Max(2, spacing / 2);
            }
            else
            {
                topPad = Math.Max(8, labelHeight);
                bottomPad = Math.Max(8, spacing);
            }

            int inputHeight = contentHeight + topPad + bottomPad;
            int extra = (!string.IsNullOrEmpty(_helperText) || !string.IsNullOrEmpty(_errorText)) ? (helperTextHeight + spacing) : 0;
            int minHeight = Math.Max(inputHeight + extra, baseHeight + 8);

            // Update MinimumSize to enforce non-clipping baseline
            if (MinimumSize.Height != minHeight)
            {
                MinimumSize = new Size(MinimumSize.Width, minHeight);
            }

            // Only auto-adjust actual Height when single-line and enabled
            if (!_multiline && AutoSizeHeight && Height < minHeight)
            {
                Height = minHeight;
                _materialHelper?.UpdateLayout();
                Invalidate();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // Ensure minimum height is set as soon as handle exists
            AutoAdjustHeight();
        }
        /// <summary>
        /// Gets the display text (handles placeholder and password characters)
        /// Enhanced version following BeepSimpleTextBox pattern
        /// </summary>
        public string GetDisplayText()
        {
            // Get actual text (excluding default/placeholder-like text)
            string actualText = GetActualText();
            
            // Return placeholder if no actual text
            if (string.IsNullOrEmpty(actualText))
            {
                return string.IsNullOrEmpty(PlaceholderText) ? "" : PlaceholderText;
            }
            
            return actualText;
        }

        /// <summary>
        /// Gets the actual text without placeholder logic
        /// </summary>
        private string GetActualText()
        {
            // Check for empty or default text conditions
            if (string.IsNullOrEmpty(_text) ||
                _text.StartsWith("beepMaterialTextField") ||
                _text.Equals(Name) ||
                _isInitializing && string.IsNullOrWhiteSpace(_text))
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

        /// <summary>
        /// Gets the current lines collection
        /// </summary>
        public new List<string> GetLines()
        {
            return _materialHelper?.GetLines()?.ToList() ?? _lines;
        }

        internal void RaiseClearButtonClicked()
        {
            ClearButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        internal void RaiseLeadingIconClicked()
        {
            LeadingIconClicked?.Invoke(this, EventArgs.Empty);
        }

        internal void RaiseTrailingIconClicked()
        {
            TrailingIconClicked?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Event Overrides

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _isFocused = true;
            _materialHelper?.StartFocusAnimation(true);
            _caretHelper?.StartCaretBlink();
            UpdateLabelAnimation();
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _isFocused = false;
            _materialHelper?.StartFocusAnimation(false);
            _caretHelper?.StopCaretBlink();
            UpdateLabelAnimation();
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _inputHelper?.HandleMouseClick(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateDrawingRect();
            _materialHelper?.UpdateLayout();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Call base.OnPaint to get proper BeepControl drawing pipeline
            base.OnPaint(e);
        }

        protected override void DrawContent(Graphics g)
        {
            // Use the Material Design drawing helper for content rendering
            // This follows the BeepControl pattern where DrawContent handles the main content
            _drawingHelper?.DrawAll(g, DrawingRect);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            AutoAdjustHeight();
        }

        #endregion

        #region Key Handling - Using Helper Classes

        protected override bool IsInputKey(Keys keyData)
        {
            return _inputHelper?.IsInputKey(keyData) ?? base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            _inputHelper?.HandleKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            _inputHelper?.HandleKeyPress(e);
        }

        #endregion

        #region Text Operations - Making it a True TextBox

        // Text operations are implemented in BeepMaterialTextField.Methods.cs
        // to avoid duplication

        #endregion

        #region Cleanup

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Dispose();
                _materialHelper?.Dispose();
                _drawingHelper?.Dispose();
                _autoCompleteHelper?.Dispose();
                _advancedEditingHelper?.Dispose();
                _caretHelper?.Dispose();
                _beepImage?.Dispose();
                
                // Note: _inputHelper and _validationHelper don't implement IDisposable
            }
            base.Dispose(disposing);
        }

        #endregion

        #region IBeepTextBox Implementation

        private Font _textFont = new Font("Arial", 10);
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

                _textFont = value;

                SafeApplyFont(_textFont);
                UseThemeFont = false;
                AutoAdjustHeight();
                Invalidate();


            }
        }
        public Graphics CreateGraphics()
        {
            return base.CreateGraphics();
        }

        void IBeepTextBox.Focus()
        {
            Focus();
        }

        #endregion

        #region Public Properties - Style Preset

        /// <summary>
        /// Apply a style preset by setting multiple properties consistently.
        /// </summary>
        public void ApplyStylePreset(MaterialTextFieldStylePreset preset)
        {
            switch (preset)
            {
                case MaterialTextFieldStylePreset.MaterialOutlined:
                    Variant = MaterialTextFieldVariant.Outlined;
                    Density = MaterialTextFieldDensity.Standard;
                    CurvedBorderRadius = 8;
                    ShowFill = false;
                    break;
                case MaterialTextFieldStylePreset.MaterialFilled:
                    Variant = MaterialTextFieldVariant.Filled;
                    Density = MaterialTextFieldDensity.Standard;
                    CurvedBorderRadius = 8;
                    ShowFill = true;
                    FillColor = Color.FromArgb(0xEE, 0xEA, 0xF0); // subtle surface
                    break;
                case MaterialTextFieldStylePreset.MaterialStandard:
                    Variant = MaterialTextFieldVariant.Standard;
                    Density = MaterialTextFieldDensity.Standard;
                    CurvedBorderRadius = 4;
                    ShowFill = false;
                    break;
                case MaterialTextFieldStylePreset.PillOutlined:
                    Variant = MaterialTextFieldVariant.Outlined;
                    CurvedBorderRadius = Math.Max(Height / 2, 20);
                    ShowFill = false;
                    break;
                case MaterialTextFieldStylePreset.PillFilled:
                    Variant = MaterialTextFieldVariant.Filled;
                    CurvedBorderRadius = Math.Max(Height / 2, 20);
                    ShowFill = true;
                    FillColor = Color.FromArgb(245, 245, 245);
                    break;
                case MaterialTextFieldStylePreset.DenseOutlined:
                    Variant = MaterialTextFieldVariant.Outlined;
                    Density = MaterialTextFieldDensity.Dense;
                    CurvedBorderRadius = 6;
                    ShowFill = false;
                    break;
                case MaterialTextFieldStylePreset.DenseFilled:
                    Variant = MaterialTextFieldVariant.Filled;
                    Density = MaterialTextFieldDensity.Dense;
                    CurvedBorderRadius = 6;
                    ShowFill = true;
                    FillColor = Color.FromArgb(245, 245, 245);
                    break;
                case MaterialTextFieldStylePreset.ComfortableOutlined:
                    Variant = MaterialTextFieldVariant.Outlined;
                    Density = MaterialTextFieldDensity.Comfortable;
                    CurvedBorderRadius = 10;
                    ShowFill = false;
                    break;
                case MaterialTextFieldStylePreset.ComfortableFilled:
                    Variant = MaterialTextFieldVariant.Filled;
                    Density = MaterialTextFieldDensity.Comfortable;
                    CurvedBorderRadius = 10;
                    ShowFill = true;
                    FillColor = Color.FromArgb(245, 245, 245);
                    break;
                case MaterialTextFieldStylePreset.Default:
                default:
                    // keep current, but ensure layout refresh
                    break;
            }

            // Ensure layout updates for any preset change
            _materialHelper?.UpdateLayout();
            AutoAdjustHeight();
            Invalidate();
        }

        #endregion

        #region Public Properties - Supporting Space

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Controls how much vertical space is reserved under the input for helper text and/or counter.")]
        [DefaultValue(SupportingSpaceMode.WhenHasTextOrCounter)]
        public SupportingSpaceMode ReserveSupportingSpace { get; set; } = SupportingSpaceMode.WhenHasTextOrCounter;

        #endregion
    }
}
