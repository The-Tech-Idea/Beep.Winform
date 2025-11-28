using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Form;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls.Widgets
{
    public enum FormWidgetStyle
    {
        FieldGroup,       // Related inputs grouped together
        ValidationPanel,  // Form section with validation display
        FormSection,      // Organized form section with title
        InputCard,        // Styled input container
        FormStep,         // Multi-step form progression
        FieldSet,         // Traditional fieldset styling
        InlineForm,       // Horizontal form layout
        CompactForm,      // Space-efficient form design
        ValidatedInput,   // Single input with validation
        FormSummary       // Form data summary display
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Form Widget")]
    [Category("Beep Widgets")]
    [Description("Form widget for data entry, validation, and form management with multiple layout styles.")]
    public class BeepFormWidget : BaseControl
    {
        #region Fields
        private FormWidgetStyle _style = FormWidgetStyle.FieldGroup;
        private IWidgetPainter _painter;
        private string _title = "Form Widget";
        private string _subtitle = "Data Entry";
        private string _description = "";
        private Color _accentColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor ?? Color.Empty; // Blue
        private Color _validColor = BeepStyling.CurrentTheme?.SuccessColor ?? Color.Empty;
        private Color _invalidColor = BeepStyling.CurrentTheme?.ErrorColor ?? Color.Empty;
        private Color _errorColor = BeepStyling.CurrentTheme?.ErrorColor ?? Color.Empty;
        private Color _warningColor = BeepStyling.CurrentTheme?.WarningColor ?? Color.Empty;
        private Color _textBoxBackColor = BeepStyling.CurrentTheme?.TextBoxBackColor ?? Color.Empty;
        private Color _textBoxForeColor = BeepStyling.CurrentTheme?.TextBoxForeColor ?? Color.Empty;
        private Color _textBoxBorderColor = BeepStyling.CurrentTheme?.TextBoxBorderColor ?? Color.Empty;
        private Color _placeholderColor = BeepStyling.CurrentTheme?.TextBoxPlaceholderColor ?? Color.Empty;
        private Color _errorBackColor = Color.FromArgb(255, 235, 235);
        private Color _errorPlaceholderColor = Color.FromArgb(244, 67, 54);
        private Color _hoverBackColor = Color.FromArgb(248, 248, 248);
        private Color _hoverForeColor = Color.Black;
        private Color _focusedBackColor = Color.FromArgb(240, 248, 255);
        private Color _focusedForeColor = Color.Black;
        private Color _buttonBackColor = BeepStyling.CurrentTheme?.ButtonBackColor ?? Color.Empty;
        private Color _buttonForeColor = BeepStyling.CurrentTheme?.ButtonForeColor ?? Color.Empty;
        private Color _buttonBorderColor = BeepStyling.CurrentTheme?.ButtonBorderColor ?? Color.Empty;
        private Color _labelForeColor = BeepStyling.CurrentTheme?.CardTitleForeColor ?? Color.Empty;
        private Color _helpTextForeColor = BeepStyling.CurrentTheme?.CardSubTitleForeColor ?? Color.Empty;
        private List<FormField> _fields = new List<FormField>();
        private List<ValidationResult> _validationResults = new List<ValidationResult>();
        private bool _showValidation = true;
        private bool _showRequired = true;
        private bool _isReadOnly = false;
        private bool _showProgress = false;
        private int _currentStep = 1;
        private int _totalSteps = 3;
        private FormLayout _layout = FormLayout.Vertical;
        private FormValidationMode _validationMode = FormValidationMode.OnSubmit;

        // Events
        public event EventHandler<BeepEventDataArgs> FieldChanged;
        public event EventHandler<BeepEventDataArgs> ValidationChanged;
        public event EventHandler<BeepEventDataArgs> StepChanged;
        public event EventHandler<BeepEventDataArgs> FormSubmitted;
        public event EventHandler<BeepEventDataArgs> FormReset;
        #endregion

        #region Constructor
        public BeepFormWidget() : base()
        {
            IsChild = false;
            Padding = new Padding(5);
            this.Size = new Size(350, 250);
            ApplyThemeToChilds = false;
            InitializeSampleData();
            ApplyTheme();
            CanBeHovered = true;
            InitializePainter();
        }

        private void InitializeSampleData()
        {
            _fields.AddRange(new[]
            {
                new FormField 
                { 
                    Name = "firstName", 
                    Label = "First Name", 
                    Type = FormFieldType.Text, 
                    Value = "John", 
                    IsRequired = true,
                    Placeholder = "Enter first name"
                },
                new FormField 
                { 
                    Name = "lastName", 
                    Label = "Last Name", 
                    Type = FormFieldType.Text, 
                    Value = "Doe", 
                    IsRequired = true,
                    Placeholder = "Enter last name"
                },
                new FormField 
                { 
                    Name = "email", 
                    Label = "Email Address", 
                    Type = FormFieldType.Email, 
                    Value = "john.doe@example.com", 
                    IsRequired = true,
                    Placeholder = "Enter email address"
                },
                new FormField 
                { 
                    Name = "phone", 
                    Label = "Phone Number", 
                    Type = FormFieldType.Phone, 
                    Value = "+1 (555) 123-4567", 
                    IsRequired = false,
                    Placeholder = "Enter phone number"
                },
                new FormField 
                { 
                    Name = "department", 
                    Label = "Department", 
                    Type = FormFieldType.Dropdown, 
                    Value = "Engineering", 
                    IsRequired = true,
                    Options = new List<string> { "Engineering", "Design", "Marketing", "Sales", "HR" }
                }
            });

            _validationResults.AddRange(new[]
            {
                new ValidationResult { FieldName = "firstName", IsValid = true, Message = "" },
                new ValidationResult { FieldName = "lastName", IsValid = true, Message = "" },
                new ValidationResult { FieldName = "email", IsValid = true, Message = "" },
                new ValidationResult { FieldName = "phone", IsValid = true, Message = "" },
                new ValidationResult { FieldName = "department", IsValid = true, Message = "" }
            });
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                case FormWidgetStyle.FieldGroup:
                    _painter = new FieldGroupPainter();
                    break;
                case FormWidgetStyle.ValidationPanel:
                    _painter = new ValidationPanelPainter();
                    break;
                case FormWidgetStyle.FormSection:
                    _painter = new FormSectionPainter();
                    break;
                case FormWidgetStyle.InputCard:
                    _painter = new InputCardPainter();
                    break;
                case FormWidgetStyle.FormStep:
                    _painter = new FormStepPainter();
                    break;
                case FormWidgetStyle.FieldSet:
                    _painter = new FieldSetPainter();
                    break;
                case FormWidgetStyle.InlineForm:
                    _painter = new InlineFormPainter();
                    break;
                case FormWidgetStyle.CompactForm:
                    _painter = new CompactFormPainter();
                    break;
                case FormWidgetStyle.ValidatedInput:
                    _painter = new ValidatedInputPainter();
                    break;
                case FormWidgetStyle.FormSummary:
                    _painter = new FormSummaryPainter();
                    break;
                default:
                    _painter = new FieldGroupPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
        }
        #endregion

        #region Properties
        [Category("Form")]
        [Description("Visual Style of the form widget.")]
        public FormWidgetStyle Style
        {
            get => _style;
            set { _style = value; InitializePainter(); Invalidate(); }
        }

        [Category("Form")]
        [Description("Title text for the form widget.")]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        [Category("Form")]
        [Description("Subtitle text for the form widget.")]
        public string Subtitle
        {
            get => _subtitle;
            set { _subtitle = value; Invalidate(); }
        }

        [Category("Form")]
        [Description("Description text for the form widget.")]
        public string Description
        {
            get => _description;
            set { _description = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Primary accent color for the form widget.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color for valid field indicators.")]
        public Color ValidColor
        {
            get => _validColor;
            set { _validColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color for error indicators.")]
        public Color ErrorColor
        {
            get => _errorColor;
            set { _errorColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color for warning indicators.")]
        public Color WarningColor
        {
            get => _warningColor;
            set { _warningColor = value; Invalidate(); }
        }

        [Category("Form")]
        [Description("Whether to show validation indicators.")]
        public bool ShowValidation
        {
            get => _showValidation;
            set { _showValidation = value; Invalidate(); }
        }

        [Category("Form")]
        [Description("Whether to show required field indicators.")]
        public bool ShowRequired
        {
            get => _showRequired;
            set { _showRequired = value; Invalidate(); }
        }

        [Category("Form")]
        [Description("Whether the form is in read-only mode.")]
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set { _isReadOnly = value; Invalidate(); }
        }

        [Category("Form")]
        [Description("Whether to show progress indicator for multi-step forms.")]
        public bool ShowProgress
        {
            get => _showProgress;
            set { _showProgress = value; Invalidate(); }
        }

        [Category("Form")]
        [Description("Current step in multi-step form (1-based).")]
        public int CurrentStep
        {
            get => _currentStep;
            set { _currentStep = Math.Max(1, value); Invalidate(); }
        }

        [Category("Form")]
        [Description("Total number of steps in multi-step form.")]
        public int TotalSteps
        {
            get => _totalSteps;
            set { _totalSteps = Math.Max(1, value); Invalidate(); }
        }

        [Category("Form")]
        [Description("Layout Style for form fields.")]
        public FormLayout Layout
        {
            get => _layout;
            set { _layout = value; Invalidate(); }
        }

        [Category("Form")]
        [Description("When to perform field validation.")]
        public FormValidationMode ValidationMode
        {
            get => _validationMode;
            set { _validationMode = value; Invalidate(); }
        }

        [Category("Form")]
        [Description("Collection of form fields.")]
        public List<FormField> Fields
        {
            get => _fields;
            set { _fields = value ?? new List<FormField>(); Invalidate(); }
        }

        [Category("Form")]
        [Description("Collection of validation results.")]
        public List<ValidationResult> ValidationResults
        {
            get => _validationResults;
            set { _validationResults = value ?? new List<ValidationResult>(); Invalidate(); }
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            var ctx = new WidgetContext
            {
                DrawingRect = DrawingRect,
                Title = _title,
                Value = _subtitle,
                AccentColor = _accentColor,
                ShowIcon = true,
                IsInteractive = !_isReadOnly,
                CornerRadius = BorderRadius,
                
                // Form-specific typed properties
                Fields = _fields,
                ValidationResults = _validationResults,
                Description = _description,
                ShowValidation = _showValidation,
                ShowRequired = _showRequired,
                IsReadOnly = _isReadOnly,
                CurrentStep = _currentStep,
                TotalSteps = _totalSteps
            };

            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;

            _painter?.DrawBackground(g, ctx);
            _painter?.DrawContent(g, ctx);
            _painter?.DrawForegroundAccents(g, ctx);

            RefreshHitAreas(ctx);
            _painter?.UpdateHitAreas(this, ctx, (name, rect) => { });
        }

        private void RefreshHitAreas(WidgetContext ctx)
        {
            ClearHitList();

            if (!ctx.ContentRect.IsEmpty)
            {
                AddHitArea("Form", ctx.ContentRect, null, () =>
                {
                    FormSubmitted?.Invoke(this, new BeepEventDataArgs("FormSubmitted", this));
                });
            }

            if (!ctx.HeaderRect.IsEmpty)
            {
                AddHitArea("Header", ctx.HeaderRect, null, () =>
                {
                    StepChanged?.Invoke(this, new BeepEventDataArgs("StepChanged", this));
                });
            }

            // Add hit areas for individual form fields
            for (int i = 0; i < _fields.Count && i < 6; i++) // Limit to 6 visible fields
            {
                int fieldIndex = i; // Capture for closure
                AddHitArea($"Field{i}", new Rectangle(), null, () =>
                {
                    FieldChanged?.Invoke(this, new BeepEventDataArgs("FieldChanged", this) { EventData = _fields[fieldIndex] });
                });
            }

            // Add hit areas for validation indicators
            if (_showValidation)
            {
                for (int i = 0; i < _validationResults.Count && i < 6; i++)
                {
                    int validationIndex = i; // Capture for closure
                    AddHitArea($"Validation{i}", new Rectangle(), null, () =>
                    {
                        ValidationChanged?.Invoke(this, new BeepEventDataArgs("ValidationChanged", this) { EventData = _validationResults[validationIndex] });
                    });
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Validates all form fields
        /// </summary>
        public bool ValidateForm()
        {
            _validationResults.Clear();
            bool isFormValid = true;

            foreach (var field in _fields)
            {
                var validation = ValidateField(field);
                _validationResults.Add(validation);
                if (!validation.IsValid)
                    isFormValid = false;
            }

            Invalidate();
            ValidationChanged?.Invoke(this, new BeepEventDataArgs("ValidationChanged", this) { EventData = isFormValid });
            return isFormValid;
        }

        /// <summary>
        /// Validates a single form field
        /// </summary>
        private ValidationResult ValidateField(FormField field)
        {
            var result = new ValidationResult { FieldName = field.Name, IsValid = true, Message = "" };

            // Required field validation
            if (field.IsRequired && string.IsNullOrWhiteSpace(field.Value?.ToString()))
            {
                result.IsValid = false;
                result.Message = $"{field.Label} is required";
                return result;
            }

            // Type-specific validation
            switch (field.Type)
            {
                case FormFieldType.Email:
                    if (!string.IsNullOrEmpty(field.Value?.ToString()) && !IsValidEmail(field.Value.ToString()))
                    {
                        result.IsValid = false;
                        result.Message = "Please enter a valid email address";
                    }
                    break;
                case FormFieldType.Phone:
                    if (!string.IsNullOrEmpty(field.Value?.ToString()) && !IsValidPhone(field.Value.ToString()))
                    {
                        result.IsValid = false;
                        result.Message = "Please enter a valid phone number";
                    }
                    break;
                case FormFieldType.Number:
                    if (!string.IsNullOrEmpty(field.Value?.ToString()) && !double.TryParse(field.Value.ToString(), out _))
                    {
                        result.IsValid = false;
                        result.Message = "Please enter a valid number";
                    }
                    break;
            }

            return result;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            // Simple phone validation - contains digits and basic formatting
            var cleaned = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");
            return cleaned.Length >= 10 && cleaned.Length <= 15;
        }

        /// <summary>
        /// Resets all form fields to default values
        /// </summary>
        public void ResetForm()
        {
            foreach (var field in _fields)
            {
                field.Value = field.DefaultValue;
            }
            _validationResults.Clear();
            Invalidate();
            FormReset?.Invoke(this, new BeepEventDataArgs("FormReset", this));
        }

        /// <summary>
        /// Gets form data as dictionary
        /// </summary>
        public Dictionary<string, object> GetFormData()
        {
            var data = new Dictionary<string, object>();
            foreach (var field in _fields)
            {
                data[field.Name] = field.Value;
            }
            return data;
        }

        /// <summary>
        /// Sets form data from dictionary
        /// </summary>
        public void SetFormData(Dictionary<string, object> data)
        {
            foreach (var field in _fields)
            {
                if (data.ContainsKey(field.Name))
                {
                    field.Value = data[field.Name];
                }
            }
            Invalidate();
        }
        #endregion

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            // Apply form-specific theme colors
            BackColor = _currentTheme.BackColor;
            ForeColor = _currentTheme.ForeColor;
            
            // Update input field colors
            _textBoxBackColor = _currentTheme.TextBoxBackColor;
            _textBoxForeColor = _currentTheme.TextBoxForeColor;
            _textBoxBorderColor = _currentTheme.TextBoxBorderColor;
            _placeholderColor = _currentTheme.TextBoxPlaceholderColor;
            
            // Update validation state colors
            _errorBackColor = _currentTheme.TextBoxErrorBackColor;
            _errorPlaceholderColor = _currentTheme.TextBoxErrorPlaceholderColor;
            _validColor = _currentTheme.SuccessColor;
            _invalidColor = _currentTheme.ErrorColor;
            _warningColor = _currentTheme.WarningColor;
            
            // Update interactive state colors
            _hoverBackColor = _currentTheme.TextBoxHoverBackColor;
            _hoverForeColor = _currentTheme.TextBoxHoverForeColor;
            _focusedBackColor = _currentTheme.TextBoxSelectedBackColor;
            _focusedForeColor = _currentTheme.TextBoxSelectedForeColor;
            
            // Update button colors
            _buttonBackColor = _currentTheme.ButtonBackColor;
            _buttonForeColor = _currentTheme.ButtonForeColor;
            _buttonBorderColor = _currentTheme.ButtonBorderColor;
            
            // Update label colors
            _labelForeColor = _currentTheme.CardTitleForeColor;
            _helpTextForeColor = _currentTheme.CardSubTitleForeColor;
            
            InitializePainter();
            Invalidate();
        }
    }

    /// <summary>
    /// Form layout enumeration
    /// </summary>
    public enum FormLayout
    {
        Vertical,     // Vertical field layout
        Horizontal,   // Horizontal field layout
        Grid,         // Grid-based layout
        Inline,       // Inline layout
        Stacked       // Stacked card layout
    }

    /// <summary>
    /// Form validation mode enumeration
    /// </summary>
    public enum FormValidationMode
    {
        OnSubmit,     // Validate when form is submitted
        OnBlur,       // Validate when field loses focus
        OnChange,     // Validate on every field change
        Realtime     // Real-time validation as user types
    }

    /// <summary>
    /// Form field type enumeration
    /// </summary>
    public enum FormFieldType
    {
        Text,         // Text input
        Email,        // Email input
        Password,     // Password input
        Number,       // Number input
        Phone,        // Phone number input
        Date,         // Date picker
        Dropdown,     // Dropdown selection
        Checkbox,     // Checkbox
        Radio,        // Radio button
        TextArea,     // Multi-line text area
        File,         // File upload
        Color,        // Color picker
        Range,        // Range slider
        Hidden        // Hidden field
    }

    /// <summary>
    /// Form field data structure
    /// </summary>
    public class FormField
    {
        public string Name { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public FormFieldType Type { get; set; } = FormFieldType.Text;
        public object Value { get; set; }
        public object DefaultValue { get; set; }
        public string Placeholder { get; set; } = string.Empty;
        public string HelpText { get; set; } = string.Empty;
        public bool IsRequired { get; set; } = false;
        public bool IsReadOnly { get; set; } = false;
        public bool IsVisible { get; set; } = true;
        public List<string> Options { get; set; } = new List<string>(); // For dropdown/radio
        public int MaxLength { get; set; } = 0; // 0 = no limit
        public string ValidationPattern { get; set; } = string.Empty; // Regex pattern
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
        public object Tag { get; set; }
    }

    /// <summary>
    /// Validation result data structure
    /// </summary>
    public class ValidationResult
    {
        public string FieldName { get; set; } = string.Empty;
        public bool IsValid { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
        public string Code { get; set; } = string.Empty;
    }

    /// <summary>
    /// Validation severity enumeration
    /// </summary>
    public enum ValidationSeverity
    {
        Info,         // Information message
        Warning,      // Warning message
        Error         // Error message
    }
}