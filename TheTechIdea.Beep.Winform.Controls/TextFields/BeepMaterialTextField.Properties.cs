using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.TextFields.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.TextFields
{
    public partial class BeepMaterialTextField
    {
        #region Material Elevation (MD3)
        private int _materialElevationLevel = 0;
        private bool _materialUseElevation = true;

        [Browsable(true)]
        [Category("Material Design 3.0")]
        [Description("Elevation level for shadow effects (0-5). Higher values create more pronounced shadows.")]
        [DefaultValue(0)]
        public int MaterialElevationLevel
        {
            get => _materialElevationLevel;
            set
            {
                int clamped = Math.Max(0, Math.Min(value, 5));
                if (_materialElevationLevel == clamped) return;
                _materialElevationLevel = clamped;
                // Hook for future shadow rendering; for now just redraw
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Material Design 3.0")]
        [Description("Enable or disable elevation shadow effects.")]
        [DefaultValue(true)]
        public bool MaterialUseElevation
        {
            get => _materialUseElevation;
            set
            {
                if (_materialUseElevation == value) return;
                _materialUseElevation = value;
                // Hook for future shadow rendering; for now just redraw
                Invalidate();
            }
        }
        #endregion

        #region Advanced Text Properties from BeepSimpleTextBox
        bool _applyThemeOnImage = false;


        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
              
                if (value!= _applyThemeOnImage)
                {
                    _applyThemeOnImage = value;
                    if (_beepImage!=null && _currentTheme != null)
                    {
                        _beepImage.Theme = Theme;
                        _beepImage.ApplyThemeOnImage= value;
                        _beepImage.ApplyTheme();
                    }
                    Invalidate();
                }
              
            }
        }
        [Browsable(true)]
        [Category("Text")]
        [Description("Text alignment within the control.")]
        [DefaultValue(HorizontalAlignment.Left)]
        public HorizontalAlignment TextAlignment { get; set; } = HorizontalAlignment.Left;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Accept Enter key for new lines.")]
        [DefaultValue(false)]
        public bool AcceptsReturn { get; set; } = false;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Accept Tab key for indentation.")]
        [DefaultValue(false)]
        public bool AcceptsTab { get; set; } = false;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Enable word wrapping in multiline mode.")]
        [DefaultValue(false)]
        public bool WordWrap { get; set; } = false;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color for placeholder text.")]
        public Color PlaceholderTextColor { get; set; } = Color.Gray;

        #endregion

        #region Selection Properties

        [Browsable(false)]
        public int SelectionStart
        {
            get => _materialHelper?.GetSelectionStart() ?? 0;
            set => _materialHelper?.SetSelectionStart(value);
        }

        [Browsable(false)]
        public int SelectionLength
        {
            get => _materialHelper?.GetSelectionLength() ?? 0;
            set => _materialHelper?.SetSelectionLength(value);
        }

        [Browsable(false)]
        public string SelectedText => _materialHelper?.GetSelectedText() ?? string.Empty;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Background color for selected text.")]
        public Color SelectionBackColor { get; set; } = SystemColors.Highlight;

        #endregion

        #region Validation Properties

        [Browsable(true)]
        [Category("Validation")]
        [Description("Specifies the mask format for the text box.")]
        [DefaultValue(TextBoxMaskFormat.None)]
        public TextBoxMaskFormat MaskFormat
        {
            get => _validationHelper?.MaskFormat ?? TextBoxMaskFormat.None;
            set
            {
                if (_validationHelper != null)
                {
                    _validationHelper.MaskFormat = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Custom mask pattern for input validation.")]
        public string CustomMask
        {
            get => _validationHelper?.CustomMask ?? string.Empty;
            set
            {
                if (_validationHelper != null)
                {
                    _validationHelper.CustomMask = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Date format pattern for date masking.")]
        public string DateFormat
        {
            get => _validationHelper?.DateFormat ?? "MM/dd/yyyy";
            set
            {
                if (_validationHelper != null)
                {
                    _validationHelper.DateFormat = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Time format pattern for time masking.")]
        public string TimeFormat
        {
            get => _validationHelper?.TimeFormat ?? "HH:mm:ss";
            set
            {
                if (_validationHelper != null)
                {
                    _validationHelper.TimeFormat = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Allow only numeric input.")]
        [DefaultValue(false)]
        public bool OnlyDigits
        {
            get => _validationHelper?.OnlyDigits ?? false;
            set
            {
                if (_validationHelper != null)
                {
                    _validationHelper.OnlyDigits = value;
                }
            }
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Allow only alphabetic characters.")]
        [DefaultValue(false)]
        public bool OnlyCharacters
        {
            get => _validationHelper?.OnlyCharacters ?? false;
            set
            {
                if (_validationHelper != null)
                {
                    _validationHelper.OnlyCharacters = value;
                }
            }
        }

        #endregion

        #region AutoComplete Properties

        [Browsable(true)]
        [Category("AutoComplete")]
        [Description("AutoComplete mode for the textbox.")]
        [DefaultValue(AutoCompleteMode.None)]
        public AutoCompleteMode AutoCompleteMode
        {
            get => _autoCompleteHelper?.AutoCompleteMode ?? AutoCompleteMode.None;
            set
            {
                if (_autoCompleteHelper != null)
                {
                    _autoCompleteHelper.AutoCompleteMode = value;
                }
            }
        }

        [Browsable(true)]
        [Category("AutoComplete")]
        [Description("AutoComplete source for the textbox.")]
        [DefaultValue(AutoCompleteSource.None)]
        public AutoCompleteSource AutoCompleteSource
        {
            get => _autoCompleteHelper?.AutoCompleteSource ?? AutoCompleteSource.None;
            set
            {
                if (_autoCompleteHelper != null)
                {
                    _autoCompleteHelper.AutoCompleteSource = value;
                }
            }
        }

        [Browsable(true)]
        [Category("AutoComplete")]
        [Description("Custom AutoComplete source collection.")]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get => _autoCompleteHelper?.AutoCompleteCustomSource;
            set
            {
                if (_autoCompleteHelper != null)
                {
                    _autoCompleteHelper.AutoCompleteCustomSource = value;
                }
            }
        }

        [Browsable(true)]
        [Category("AutoComplete")]
        [Description("Minimum characters required to trigger autocomplete.")]
        [DefaultValue(1)]
        public int AutoCompleteMinimumLength
        {
            get => _autoCompleteHelper?.AutoCompleteMinimumLength ?? 1;
            set
            {
                if (_autoCompleteHelper != null)
                {
                    _autoCompleteHelper.AutoCompleteMinimumLength = value;
                }
            }
        }

        [Browsable(true)]
        [Category("AutoComplete")]
        [Description("Maximum number of autocomplete suggestions.")]
        [DefaultValue(10)]
        public int AutoCompleteMaxSuggestions
        {
            get => _autoCompleteHelper?.AutoCompleteMaxSuggestions ?? 10;
            set
            {
                if (_autoCompleteHelper != null)
                {
                    _autoCompleteHelper.AutoCompleteMaxSuggestions = value;
                }
            }
        }

        [Browsable(true)]
        [Category("AutoComplete")]
        [Description("AutoComplete case sensitivity.")]
        [DefaultValue(false)]
        public bool AutoCompleteCaseSensitive
        {
            get => _autoCompleteHelper?.AutoCompleteCaseSensitive ?? false;
            set
            {
                if (_autoCompleteHelper != null)
                {
                    _autoCompleteHelper.AutoCompleteCaseSensitive = value;
                }
            }
        }

        #endregion

        #region Line Numbers Properties

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Shows or hides line numbers on the left side of the textbox.")]
        public bool ShowLineNumbers { get; set; } = false;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(40)]
        [Description("Width of the line number margin.")]
        public int LineNumberMarginWidth { get; set; } = 40;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Foreground color for line numbers.")]
        public Color LineNumberForeColor { get; set; } = Color.Gray;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Background color for line number margin.")]
        public Color LineNumberBackColor { get; set; } = Color.FromArgb(240, 240, 240);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for line numbers.")]
        public Font LineNumberFont { get; set; }

        #endregion

        #region Advanced Editing Properties

        [Browsable(true)]
        [Category("Advanced Editing")]
        [Description("Enable syntax highlighting for code editing.")]
        [DefaultValue(false)]
        public bool SyntaxHighlightingEnabled
        {
            get => _advancedEditingHelper?.SyntaxHighlightingEnabled ?? false;
            set
            {
                if (_advancedEditingHelper != null)
                {
                    _advancedEditingHelper.SyntaxHighlightingEnabled = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Advanced Editing")]
        [Description("Syntax language for highlighting.")]
        [DefaultValue(SyntaxLanguage.PlainText)]
        public SyntaxLanguage SyntaxLanguage { get; set; } = SyntaxLanguage.PlainText;

        [Browsable(true)]
        [Category("Advanced Editing")]
        [Description("Enable code folding for structured text.")]
        [DefaultValue(false)]
        public bool CodeFoldingEnabled
        {
            get => _advancedEditingHelper?.CodeFoldingEnabled ?? false;
            set
            {
                if (_advancedEditingHelper != null)
                {
                    _advancedEditingHelper.CodeFoldingEnabled = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Advanced Editing")]
        [Description("Enable bracket matching highlighting.")]
        [DefaultValue(false)]
        public bool BracketMatchingEnabled
        {
            get => _advancedEditingHelper?.BracketMatchingEnabled ?? false;
            set
            {
                if (_advancedEditingHelper != null)
                {
                    _advancedEditingHelper.BracketMatchingEnabled = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Advanced Editing")]
        [Description("Show whitespace characters.")]
        [DefaultValue(false)]
        public bool ShowWhitespace
        {
            get => _advancedEditingHelper?.ShowWhitespace ?? false;
            set
            {
                if (_advancedEditingHelper != null)
                {
                    _advancedEditingHelper.ShowWhitespace = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Advanced Editing")]
        [Description("Enable auto-indentation for new lines.")]
        [DefaultValue(true)]
        public bool AutoIndentEnabled
        {
            get => _advancedEditingHelper?.AutoIndentEnabled ?? true;
            set
            {
                if (_advancedEditingHelper != null)
                {
                    _advancedEditingHelper.AutoIndentEnabled = value;
                }
            }
        }

        [Browsable(false)]
        [Description("List of bookmarks in the text.")]
        public System.Collections.Generic.List<object> Bookmarks
        {
            get => _advancedEditingHelper?.Bookmarks?.Cast<object>().ToList() ?? new System.Collections.Generic.List<object>();
        }

        #endregion

        #region Image Properties from BeepSimpleTextBox

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
                    _materialHelper?.UpdateLayout();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Size of the image displayed in the textbox.")]
        public Size MaxImageSize
        {
            get => _beepImage?.Size ?? new Size(24, 24);
            set
            {
                if (_beepImage != null)
                {
                    _beepImage.Size = value;
                    _materialHelper?.UpdateLayout();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Relationship between text and image.")]
        [DefaultValue(TextImageRelation.ImageBeforeText)]
        public TextImageRelation TextImageRelation { get; set; } = TextImageRelation.ImageBeforeText;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Align image within the text field.")]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment ImageAlign { get; set; } = ContentAlignment.MiddleLeft;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show or hide the image.")]
        [DefaultValue(false)]
        public bool ImageVisible
        {
            get => _beepImage?.Visible ?? false;
            set
            {
                if (_beepImage != null)
                {
                    _beepImage.Visible = value && !string.IsNullOrEmpty(_beepImage.ImagePath);
                    _materialHelper?.UpdateLayout();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Apply theme styling to the image for consistent appearance.")]
        [DefaultValue(false)]
        public bool ApplyThemeToImage
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

        #endregion

        #region Material Design Animation Properties

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Duration of label animation in milliseconds.")]
        [DefaultValue(200)]
        public int LabelAnimationDuration { get; set; } = 200;

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Duration of focus animation in milliseconds.")]
        [DefaultValue(150)]
        public int FocusAnimationDuration { get; set; } = 150;

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Scale factor for floating label.")]
        [DefaultValue(0.75f)]
        public float FloatingLabelScale { get; set; } = 0.75f;

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Enable smooth animations.")]
        [DefaultValue(true)]
        public bool EnableAnimations { get; set; } = true;

        #endregion

        #region Material Colors Properties

        [Browsable(true)]
        [Category("Material Colors")]
        [Description("Primary color for focus states.")]
        public Color PrimaryColor { get; set; } = Color.FromArgb(98, 0, 238);

        [Browsable(true)]
        [Category("Material Colors")]
        [Description("Error color for validation states.")]
        public Color ErrorColor { get; set; } = Color.FromArgb(179, 38, 30);

        [Browsable(true)]
        [Category("Material Colors")]
        [Description("Surface color for backgrounds.")]
        public Color SurfaceColor { get; set; } = Color.White;

        [Browsable(true)]
        [Category("Material Colors")]
        [Description("Outline color for borders.")]
        public Color OutlineColor { get; set; } = Color.FromArgb(121, 116, 126);

        [Browsable(true)]
        [Category("Material Colors")]
        [Description("Color for label text.")]
        public Color LabelColor { get; set; } = Color.FromArgb(99, 99, 99);

        [Browsable(true)]
        [Category("Material Colors")]
        [Description("Color for helper text.")]
        public Color HelperTextColor { get; set; } = Color.FromArgb(117, 117, 117);

        #endregion

        #region Computed Properties

      

        [Browsable(false)]
        public bool ShouldFloatLabel => _isFocused || HasContent || !string.IsNullOrEmpty(_placeholderText);

        [Browsable(false)]
        public string DisplayText
        {
            get
            {
                if (_useSystemPasswordChar && !string.IsNullOrEmpty(_text))
                    return new string('•', _text.Length);
                else if (_passwordChar != '\0' && !string.IsNullOrEmpty(_text))
                    return new string(_passwordChar, _text.Length);
                return _text;
            }
        }

        [Browsable(false)]
        public string EffectivePlaceholderText
        {
            get
            {
                if (string.IsNullOrEmpty(_text) && !_isFocused && !string.IsNullOrEmpty(_placeholderText))
                    return _placeholderText;
                return string.Empty;
            }
        }

        [Browsable(false)]
        public string EffectiveHelperText => _hasError ? _errorText : _helperText;

        [Browsable(false)]
        public Color EffectiveHelperTextColor => _hasError ? ErrorColor : HelperTextColor;

        [Browsable(false)]
        public string CharacterCountText => GetCharacterCountDisplay();

        /// <summary>
        /// Get current character count for counter display - internal version
        /// </summary>
        private string GetCharacterCountDisplay()
        {
            if (_maxLength > 0 && _showCharacterCounter)
            {
                return $"{_text.Length}/{_maxLength}";
            }
            return _showCharacterCounter ? _text.Length.ToString() : string.Empty;
        }

        [Browsable(false)]
        public bool IsCharacterCountVisible => _showCharacterCounter && (_maxLength > 0 || !string.IsNullOrEmpty(_text));

        [Browsable(false)]
        public bool IsCharacterLimitExceeded => _maxLength > 0 && _text.Length > _maxLength;

        [Browsable(false)]
        public string EffectivePrefixText => !string.IsNullOrEmpty(_prefixText) ? _prefixText : string.Empty;

        [Browsable(false)]
        public string EffectiveSuffixText => !string.IsNullOrEmpty(_suffixText) ? _suffixText : string.Empty;

        [Browsable(false)]
        public bool HasPrefixText => !string.IsNullOrEmpty(_prefixText);

        [Browsable(false)]
        public bool HasSuffixText => !string.IsNullOrEmpty(_suffixText);

        [Browsable(false)]
        public int DensityHeight => _density switch
        {
            MaterialTextFieldDensity.Dense => 56,
            MaterialTextFieldDensity.Comfortable => 88,
            _ => 72 // Standard
        };

        #endregion
    }
}
