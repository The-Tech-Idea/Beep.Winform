using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Globalization;
using TheTechIdea.Beep.Shared;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Description("A text box control with Beep styling.")]
    [DisplayName("Beep TextBox")]
    [Category("Beep Controls")]
    public class BeepTextBox : BeepControl
    {
        #region "Properties"
      
        public new event EventHandler TextChanged;
        private TextBox _innerTextBox;
        private BeepButton beepImage;
        private string _maskFormat = "";
        private bool _onlyDigits = false;
        private bool _onlyCharacters = false;
        private TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;
        private ContentAlignment _imageAlign = ContentAlignment.MiddleLeft;
        private Size _maxImageSize = new Size(16, 16); // Default image size
        private string? _imagepath;
        private bool _multiline = false;
        int padding = 2;
        int xpadding = 1;
        int offset = 0;
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
                UseThemeFont = false;
                Font = _textFont;
                _innerTextBox.Font = _textFont;
                Invalidate();


            }
        }
        private bool _showverticalscrollbar = false;
        [Browsable(true)]
        [Category("Appearance")]
        public bool ShowVerticalScrollBar
        {
            get => _showverticalscrollbar;
            set
            {
                _showverticalscrollbar = value;
                if (value)
                {
                    _innerTextBox.ScrollBars = ScrollBars.Vertical;
                }
                else
                {
                    _innerTextBox.ScrollBars = ScrollBars.None;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public int PreferredHeight
        {
            get => _innerTextBox.PreferredHeight;

        }
        // Provide a public property that returns single-line height based on the current font
        [Browsable(false)]
        public int SingleLineHeight
        {
            get
            {
               return GetSingleLineHeight();
            }
        }
        // show the inner textbox properties like multiline
        [Browsable(true)]
        [Category("Appearance")]
        public bool Multiline
        {
            get => _multiline;
            set
            {
                _multiline = value;
                _innerTextBox.Multiline = value;
                AdjustTextBoxHeight();
                PositionInnerTextBoxAndImage();
                Invalidate();
            }
        }
        //[Browsable(true)]
        //[Category("Appearance")]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        //public TextBox InnerTextBox
        //{
        //    get => _innerTextBox;
        //}
        [Browsable(true)]
        [Category("Appearance")]
        public bool ReadOnly
        {
            get => _innerTextBox.ReadOnly;
            set
            {
                _innerTextBox.ReadOnly = value;
                Invalidate();
            }
        }
      
        [Browsable(true)]
        [Category("Appearance")]
        public bool UseSystemPasswordChar
        {
            get => _innerTextBox.UseSystemPasswordChar;
            set
            {
                _innerTextBox.UseSystemPasswordChar = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public char PasswordChar
        {
            get => _innerTextBox.PasswordChar;
            set
            {
                _innerTextBox.PasswordChar = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool WordWrap
        {
            get => _innerTextBox.WordWrap;
            set
            {
                _innerTextBox.WordWrap = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool AcceptsReturn
        {
            get => _innerTextBox.AcceptsReturn;
            set
            {
                _innerTextBox.AcceptsReturn = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool AcceptsTab
        {
            get => _innerTextBox.AcceptsTab;
            set
            {
                _innerTextBox.AcceptsTab = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool ShowScrollbars
        {
            get => _innerTextBox.ScrollBars != ScrollBars.None;
            set
            {
                _innerTextBox.ScrollBars = value ? ScrollBars.Both : ScrollBars.None;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public ScrollBars ScrollBars
        {
            get => _innerTextBox.ScrollBars;
            set
            {
                _innerTextBox.ScrollBars = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool AutoSize
        {
            get => _innerTextBox.AutoSize;
            set
            {
                _innerTextBox.AutoSize = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool CausesValidation
        {
            get => _innerTextBox.CausesValidation;
            set
            {
                _innerTextBox.CausesValidation = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool HideSelection
        {
            get => _innerTextBox.HideSelection;
            set
            {
                _innerTextBox.HideSelection = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool Modified
        {
            get => _innerTextBox.Modified;
            set
            {
                _innerTextBox.Modified = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool Enabled
        {
            get => _innerTextBox.Enabled;
            set
            {
                _innerTextBox.Enabled = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public HorizontalAlignment TextAlignment
        {
            get => _innerTextBox.TextAlign;
            set
            {
                _innerTextBox.TextAlign = value;
                Invalidate();
            }
        }
        private Color _backcolor;
        [Browsable(true)]
        [Category("Appearance")]
        public override Color BackColor
        {
            get => _backcolor;
            set
            {
                _backcolor = value;
                _innerTextBox.BackColor = value;
                // Invalidate();  // Trigger repaint
            }
        }
        private Color _forcolor;
        [Browsable(true)]
        [Category("Appearance")]
        public override Color ForeColor 
        {
            get => _forcolor;
            set
            {
                _forcolor = value;
                _innerTextBox.ForeColor = value;
                // Invalidate();  // Trigger repaint
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("PlaceholderText  in the control.")]
        public string PlaceholderText
        {
            get => _innerTextBox.PlaceholderText;
            set
            {
                _innerTextBox.PlaceholderText = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text displayed in the control.")]
        public new string Text
        {
            get => _innerTextBox.Text;
            set
            {
                if (!_isApplyingMask)
                {
                    _innerTextBox.Text = value;
                    ApplyMaskFormat();
                    Invalidate();
                }
            }
        }
        #region "Format and Masking"
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Restrict input to digits only.")]
        public bool OnlyDigits
        {
            get => _onlyDigits;
            set => _onlyDigits = value;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Restrict input to alphabetic characters only.")]
        public bool OnlyCharacters
        {
            get => _onlyCharacters;
            set => _onlyCharacters = value;
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Specify the mask or format for text display.")]
        public TextBoxMaskFormat MaskFormat
        {
            get => _maskFormatEnum;
            set
            {
                _maskFormatEnum = value;
                ApplyMaskFormat();
                Invalidate();
            }
        }

        private TextBoxMaskFormat _maskFormatEnum = TextBoxMaskFormat.None;
        // Optional: For custom mask formats
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Specify a custom mask format.")]
        public string CustomMask
        {
            get => _customMask;
            set
            {
                _customMask = value;
                if (MaskFormat == TextBoxMaskFormat.Custom)
                {
                    ApplyMaskFormat();
                }
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Specify the custom date and time format for display.")]
        public string DateFormat
        {
            get => _dateFormat;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _dateFormat = "MM/dd/yyyy"; // Default format
                }
                else
                {
                    // Validate the format string
                    try
                    {
                        DateTime.Now.ToString(value, CultureInfo.CurrentCulture);
                        _dateFormat = value;
                    }
                    catch (FormatException)
                    {
                        throw new FormatException($"The date format '{value}' is invalid.");
                    }
                }
                ApplyMaskFormat();
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Specify the custom time format for display.")]
        public string TimeFormat
        {
            get => _timeFormat;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _timeFormat = "HH:mm:ss"; // Default time format (24-hour)
                }
                else
                {
                    // Validate the format string
                    try
                    {
                        DateTime.Now.ToString(value, CultureInfo.CurrentCulture);
                        _timeFormat = value;
                    }
                    catch (FormatException)
                    {
                        throw new FormatException($"The time format '{value}' is invalid.");
                    }
                }
                ApplyTimeFormat();
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Specify the custom date and time format for display.")]
        public string DateTimeFormat
        {
            get => _dateTimeFormat;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _dateTimeFormat = $"{_dateFormat} {_timeFormat}"; // Default combined format
                }
                else
                {
                    // Validate the format string
                    try
                    {
                        // Attempt to format the current date and time with the provided format
                        DateTime.Now.ToString(value, CultureInfo.CurrentCulture);
                        _dateTimeFormat = value;
                    }
                    catch (FormatException)
                    {
                        throw new FormatException($"The date and time format '{value}' is invalid.");
                    }
                }
                ApplyMaskFormat(); // Reapply formatting with the new DateTimeFormat
                Invalidate(); // Refresh the control to reflect changes
            }
        }
        private string _dateTimeFormat = "MM/dd/yyyy HH:mm:ss"; // Default combined format

        private string _timeFormat = "HH:mm:ss"; // Default format
        private string _dateFormat = "MM/dd/yyyy HH:mm:ss"; // Default format

        private string _customMask = string.Empty;
        #endregion "Format and Masking"
        #region "Image Properties"
        [Browsable(true)]
        [Category("Appearance")]
        public Size MaxImageSize
        {
            get => _maxImageSize;
            set
            {
                _maxImageSize = value;
                beepImage.Size = value;
                beepImage.MaxImageSize = new Size(value.Width-1,value.Height-1);
              //  PositionInnerTextBoxAndImage();
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public TextImageRelation TextImageRelation
        {
            get => _textImageRelation;
            set
            {
                _textImageRelation = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public ContentAlignment ImageAlign
        {
            get => _imageAlign;
            set
            {
                _imageAlign = value;
                Invalidate();
            }
        }
        bool _applyThemeOnImage = false;

        private int spacing;

        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
                if (value)
                {

                    if (ApplyThemeOnImage)
                    {
                        beepImage.ApplyThemeOnImage = true;
                        //  beepImage.Theme = Theme;
                        ApplyTheme();
                    }
                }
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Specify the image file to load (SVG, PNG, JPG, etc.).")]
        public string ImagePath
        {
            get => beepImage?.ImagePath;
            set
            {
                if (beepImage == null)
                {
                    beepImage = new BeepButton();

                }
                if (beepImage != null)
                {
                    beepImage.ImagePath = value;
                    if (ApplyThemeOnImage)
                    {
                       // beepImage.Theme = Theme;
                        beepImage.ApplyThemeOnImage = true;
                     //   beepImage.ApplyThemeToSvg();
                        ApplyTheme();
                    }
                    Invalidate(); // Repaint when the image changes
                                  // UpdateSize();
                   // PositionInnerTextBoxAndImage();
                }
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new bool IsChild
        {
            get => _isChild;
            set
            {
                _isChild = value;
                base.IsChild = value;
                Control parent = null;
                if (this.Parent != null)
                {
                    parent = this.Parent;
                }
                if (parent != null)
                {
                    if (value)
                    {
                        parentbackcolor = parent.BackColor;
                        TempBackColor = _innerTextBox.BackColor;
                        BackColor = parentbackcolor;
                        _innerTextBox.BackColor = parentbackcolor;
                        beepImage.BackColor = parentbackcolor;
                    }
                    else
                    {
                        beepImage.BackColor = _tempbackcolor;
                        BackColor = _tempbackcolor;
                        _innerTextBox.BackColor = _tempbackcolor;
                    }
                }
                else
                {
                 //   Debug.WriteLine($"No Parent ");
                    return;
                }
                Invalidate();
               _innerTextBox.Invalidate()  ;  // Trigger repaint
            }
        }
        #endregion "Image Properties"
        #region "AutoCompelete Properties"

        [Browsable(true)]
        [Category("Appearance")]
        [Description("AutoComplete displayed in the control.")]
        public AutoCompleteMode AutoCompleteMode
        {
            get => _innerTextBox.AutoCompleteMode;
            set
            {
                _innerTextBox.AutoCompleteMode = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("AutoComplete displayed in the control.")]
        public AutoCompleteSource AutoCompleteSource
        {
            get => _innerTextBox.AutoCompleteSource;
            set
            {
                _innerTextBox.AutoCompleteSource = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("AutoComplete Custom Source .")]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get => _innerTextBox.AutoCompleteCustomSource;
            set
            {
                _innerTextBox.AutoCompleteCustomSource = value;
                Invalidate();
            }
        }
     
        #endregion "AutoCompelete Properties"
        #endregion "Properties"
        #region "Constructors"
        public BeepTextBox()
        {
        //    SetStyle(ControlStyles.SupportsTransparentBackColor, false);
            //      BackColor = Color.Black;
            Padding = new Padding(3);
            InitializeComponents();

           // AutoSize = true;
            BoundProperty = "Text";
            Padding = new Padding(0);
           _innerTextBox.BorderStyle = BorderStyle.None;
            //  BorderStyle = BorderStyle.FixedSingle;
            _innerTextBox.Invalidated += BeepTextBox_Invalidated;
            beepImage.IsChild = true;
         //   ApplyTheme(); // Ensure _currentTheme is initialized
            // Ensure size adjustments occur after initialization
            UpdateDrawingRect();
            AdjustTextBoxHeight();
            PositionInnerTextBoxAndImage();
         //   _innerTextBox.BringToFront();

            //AdjustTextBoxHeight();
            //PositionInnerTextBoxAndImage();
        }
        #endregion "Constructors"
        #region "Initialization"
        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
        }
        protected override Size DefaultSize => new Size(200, GetSingleLineHeight());

        public int SelectionStart { get => _innerTextBox.SelectionStart; set { _innerTextBox.SelectionStart = value; } }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
        }
        protected override void InitLayout()
        {
            base.InitLayout();

        }
        private void InitializeComponents()
        {
            _innerTextBox = new TextBox
            {
                BorderStyle = System.Windows.Forms.BorderStyle.None,
                // Multiline = true,
                //ScrollBars= ScrollBars.Both
            };
            
            // IsCustomeBorder=true;   
            _innerTextBox.TextChanged += InnerTextBox_TextChanged;
            _innerTextBox.KeyPress += InnerTextBox_KeyPress;
            _innerTextBox.KeyDown += OnSearchKeyDown;
            _innerTextBox.MouseEnter += OnMouseEnter;
            _innerTextBox.MouseLeave += OnMouseLeave;
 //           _innerTextBox.TextChanged += (s, e) => Invalidate(); // Repaint to apply formatting
            Controls.Add(_innerTextBox);
            _innerTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            beepImage = new BeepButton() 
            {
                Size = _maxImageSize,
                ShowAllBorders = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                IsChild = true,
                
                MaxImageSize = new Size(MaxImageSize.Width-1, MaxImageSize.Height-1), Dock = DockStyle.None, Margin = new Padding(0) 
            
            };
            Controls.Add(beepImage);
            //       Console.WriteLine("InitializeComponents");
            //AdjustTextBoxHeight();
           // PositionInnerTextBoxAndImage();

        }
        #endregion "Initialization"
        #region "Paint and Invalidate"
        private void BeepTextBox_Invalidated(object? sender, InvalidateEventArgs e)
        {
            _isControlinvalidated = true;
            _innerTextBox.Update();
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_isControlinvalidated)
            {
              //  if (ShowAllBorders) { _innerTextBox.BorderStyle = BorderStyle.None; } else { _innerTextBox.BorderStyle = BorderStyle.FixedSingle; }
                _isControlinvalidated = false;
             //   _innerTextBox.Invalidate();
                

            }
        }
        #endregion "Paint and Invalidate"
        #region "Size and Position"
        // protected override Size DefaultSize => GetDefaultSize();
        private int GetSingleLineHeight()
        {
            // Ensure DrawingRect is updated
            UpdateDrawingRect();
            int textBoxHeight;
          //  padding = BorderThickness + offset;
            //spacing = 1;
            using (TextBox tempTextBox = new TextBox())
            {
                tempTextBox.Multiline = false;
                tempTextBox.BorderStyle = BorderStyle.None;
                tempTextBox.Font = Font;
                tempTextBox.Refresh();
                textBoxHeight = tempTextBox.PreferredHeight + (padding * 2);

                // Calculate the total height, including borders and padding
            }
       //    Console.WriteLine($" GetSingleLineHeight : {textBoxHeight}");

            return textBoxHeight;
        }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // Ensure DrawingRect is updated
            UpdateDrawingRect();
            // check if it is multiline
            if (_multiline)
            {
                // Multiline: just set the control's bounds as requested
                base.SetBoundsCore(x, y, width, height, specified);
                return;
            }
            // Ensure the control's height is always a multiple of the single-line height
            int singleLineHeight = GetSingleLineHeight();
            int adjustedHeight = height;
            if (height % singleLineHeight != 0)
            {
                adjustedHeight = (height / singleLineHeight) * singleLineHeight;
            }
            // Adjust the control's height to fit the single-line height
            base.SetBoundsCore(x, y, width, adjustedHeight, specified);
        
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // If single line, don't allow a bigger or smaller height than necessary
            if (!_multiline)
            {
                int singleLineHeight = GetSingleLineHeight();
                // Force this control’s height to the single-line height
                
                    // single line
                    _innerTextBox.Multiline = false;
                    _innerTextBox.ScrollBars = ScrollBars.None;
                    // Just keep the text box's native preferred height

                    // This is the critical part:
                   
                    _innerTextBox.Height = singleLineHeight;
                    // This is the critical part:


                    // Force the user control to match that height 
                    this.Height = singleLineHeight + (BorderThickness * 2);
                    // Or add some extra margin if your border is drawn outside the DrawingRect

                    // Then set the actual textbox height
                    _innerTextBox.Height = singleLineHeight;

                // Or you could do:
                // this.MinimumSize = new Size(0, singleLineHeight);
                // this.MaximumSize = new Size(0, singleLineHeight);
            }
            UpdateDrawingRect();
            AdjustTextBoxHeight();
            PositionInnerTextBoxAndImage();
            
        }
        private void AdjustTextBoxHeight()
        {
            if (_multiline)
            {
                _innerTextBox.Multiline = true;
              //  _innerTextBox.ScrollBars = ScrollBars.Vertical;
                // fill the entire DrawingRect minus some padding
                // (this ensures the text box is inside the beepcontrol border)
                int fillWidth = DrawingRect.Width - (xpadding * 2);
                int fillHeight = DrawingRect.Height - (padding * 2);
                _innerTextBox.Multiline = true;
                if (fillWidth < 1) fillWidth = 1;
                if (fillHeight < 1) fillHeight = 1;

                _innerTextBox.Location = new Point(DrawingRect.X + xpadding, DrawingRect.Y + padding);
                _innerTextBox.Size = new Size(fillWidth, fillHeight);
            }
           
        }
        private void PositionInnerTextBoxAndImage()
        {
            bool hasImage = !string.IsNullOrEmpty(ImagePath);
            beepImage.Visible = hasImage;

            // 1) Figure out the final drawing rectangle *inside* any border or theming
            //    Typically, "DrawingRect" is something like:
            //       new Rectangle(BorderThickness, BorderThickness,
            //                     Width - 2*BorderThickness,
            //                     Height - 2*BorderThickness);
            //    We'll trust it’s already sized properly.
            Rectangle rect = DrawingRect;

            // 2) Subtract any internal padding from the usable width
            int availableWidth = rect.Width - (xpadding * 2);

            // 3) The text box height is presumably set already (e.g., single-line vs multiline).
            //    If you’re using a single-line approach, ensure _innerTextBox.Height 
            //    is large enough (PreferredHeight + a margin, etc.).
            int textBoxHeight = _innerTextBox.Height;
            int totalHeight = rect.Height;

            // 4) If you want to center the text box vertically in single-line mode:
            //    For multi-line, you might want top-left alignment, but let's assume vertical center for now.
            int textBoxY = rect.Y + (totalHeight - textBoxHeight) / 2;

            // 5) Positioning the image if it exists
            if (hasImage)
            {
                // Adjust beepImage size if bigger than the max
                Size finalImageSize = beepImage.Size;
                if (finalImageSize.Width > _maxImageSize.Width || finalImageSize.Height > _maxImageSize.Height)
                {
                    float scaleFactor = Math.Min(
                        (float)_maxImageSize.Width / finalImageSize.Width,
                        (float)_maxImageSize.Height / finalImageSize.Height
                    );
                    finalImageSize = new Size(
                        (int)(finalImageSize.Width * scaleFactor),
                        (int)(finalImageSize.Height * scaleFactor)
                    );
                }
                beepImage.Size = finalImageSize;

                // Align image with the text box’s vertical center
                int imageY = textBoxY + (textBoxHeight - beepImage.Height) / 2;

                if (_textImageRelation == TextImageRelation.ImageBeforeText)
                {
                    // 6) Position the image at left + xpadding
                    int imageX = rect.X + xpadding;
                    beepImage.Location = new Point(imageX, imageY);

                    // 7) Now the text box starts after the image plus some spacing
                    int textBoxX = beepImage.Right + spacing;

                    // 8) The available text width is the remainder of rect minus image width & spacing
                    int textWidth = availableWidth - beepImage.Width - spacing;
                    if (textWidth < 0) textWidth = 0;  // clamp to 0 if not enough room

                    _innerTextBox.Location = new Point(textBoxX, textBoxY);
                    _innerTextBox.Width = textWidth;
                }
                else
                {
                    // 9) Text is on the left, image on the right
                    int textBoxX = rect.X + xpadding;
                    // subtract space for image + spacing
                    int textWidth = availableWidth - beepImage.Width - spacing;
                    if (textWidth < 0) textWidth = 0;

                    _innerTextBox.Location = new Point(textBoxX, textBoxY);
                    _innerTextBox.Width = textWidth;

                    // place the image after the text
                    int imageX = _innerTextBox.Right + spacing;
                    beepImage.Location = new Point(imageX, imageY);
                }
            }
            else
            {
                // 10) No image, so the text box spans the entire width
                int textBoxX = rect.X + xpadding;
                _innerTextBox.Location = new Point(textBoxX, textBoxY);

                // Use the full available width
                _innerTextBox.Width = availableWidth;
            }
        }


        #endregion "Size and Position"
        #region "Mouse Events"
        private void OnMouseEnter(object? sender, EventArgs e)
        {
            base.OnMouseEnter(e);
            BorderColor = HoverBackColor;
            Invalidate();
        }
        private void OnMouseLeave(object? sender, EventArgs e)
        {
            base.OnMouseLeave(e);
            BorderColor = _currentTheme.BorderColor;
            Invalidate();
        }

        #endregion "Mouse Events"
        #region "Key Events"
        private void InnerTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!_isApplyingMask)
            {
                _isApplyingMask = true;
                ApplyMaskFormat();
                _isApplyingMask = false;

              //  Text = _innerTextBox.Text;
                TextChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }
        private void InnerTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Retrieve the current culture's decimal and group separators
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string groupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;

            // Handle input restrictions based on OnlyDigits and OnlyCharacters
            if (OnlyDigits)
            {
                // Allow digits, control characters, decimal separator, and group separator
                if (!char.IsDigit(e.KeyChar) &&
                    !char.IsControl(e.KeyChar) &&
                    e.KeyChar.ToString() != decimalSeparator &&
                    e.KeyChar.ToString() != groupSeparator)
                {
                    e.Handled = true;
                    return;
                }

                // If decimal separator is pressed, allow only one occurrence
                if (e.KeyChar.ToString() == decimalSeparator && _innerTextBox.Text.Contains(decimalSeparator))
                {
                    e.Handled = true;
                    return;
                }
            }
            else if (OnlyCharacters)
            {
                // Allow only letters and control characters
                if (!char.IsLetter(e.KeyChar) &&
                    !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                    return;
                }
            }

            // Additional restrictions based on MaskFormat
            switch (MaskFormat)
            {
                case TextBoxMaskFormat.Currency:
                case TextBoxMaskFormat.Percentage:
                case TextBoxMaskFormat.Decimal:
                    // Allow only one decimal separator based on current culture
                    if (e.KeyChar.ToString() == decimalSeparator && _innerTextBox.Text.Contains(decimalSeparator))
                    {
                        e.Handled = true;
                    }
                    break;

                case TextBoxMaskFormat.PhoneNumber:
                    // Allow digits, control characters, and specific phone number symbols
                    if (!char.IsDigit(e.KeyChar) &&
                        !char.IsControl(e.KeyChar) &&
                        e.KeyChar != '(' &&
                        e.KeyChar != ')' &&
                        e.KeyChar != '-' &&
                        e.KeyChar != ' ' &&
                        e.KeyChar != '+')
                    {
                        e.Handled = true;
                    }
                    break;

                case TextBoxMaskFormat.Email:
                    // Allow standard email characters: letters, digits, and specific symbols
                    if (!char.IsLetterOrDigit(e.KeyChar) &&
                        !char.IsControl(e.KeyChar) &&
                        e.KeyChar != '@' &&
                        e.KeyChar != '.' &&
                        e.KeyChar != '_' &&
                        e.KeyChar != '-' &&
                        e.KeyChar != '+')
                    {
                        e.Handled = true;
                    }
                    break;

                case TextBoxMaskFormat.URL:
                    // Allow standard URL characters: letters, digits, and specific symbols
                    if (!char.IsLetterOrDigit(e.KeyChar) &&
                        !char.IsControl(e.KeyChar) &&
                        e.KeyChar != '/' &&
                        e.KeyChar != ':' &&
                        e.KeyChar != '.' &&
                        e.KeyChar != '?' &&
                        e.KeyChar != '#' &&
                        e.KeyChar != '&' &&
                        e.KeyChar != '=' &&
                        e.KeyChar != '%' &&
                        e.KeyChar != '-' &&
                        e.KeyChar != '_' &&
                        e.KeyChar != '~' &&
                        e.KeyChar != ':')
                    {
                        e.Handled = true;
                    }
                    break;

                case TextBoxMaskFormat.IPAddress:
                    // Allow digits, control characters, and dots
                    if (!char.IsDigit(e.KeyChar) &&
                        !char.IsControl(e.KeyChar) &&
                        e.KeyChar != '.')
                    {
                        e.Handled = true;
                    }
                    else if (e.KeyChar == '.' && _innerTextBox.Text.EndsWith("."))
                    {
                        // Prevent consecutive dots
                        e.Handled = true;
                    }
                    break;

                case TextBoxMaskFormat.CreditCard:
                    // Allow only digits, control characters, and spaces
                    if (!char.IsDigit(e.KeyChar) &&
                        !char.IsControl(e.KeyChar) &&
                        e.KeyChar != ' ')
                    {
                        e.Handled = true;
                    }
                    break;

                case TextBoxMaskFormat.Hexadecimal:
                    // Allow only hexadecimal characters and control characters
                    if (!Uri.IsHexDigit(e.KeyChar) &&
                        !char.IsControl(e.KeyChar))
                    {
                        e.Handled = true;
                    }
                    break;

                case TextBoxMaskFormat.Custom:
                    // Implement custom mask restrictions if necessary
                    // Example: Allow only specific characters or enforce a pattern
                    // This depends on how CustomMask is defined and intended to be used
                    break;

                case TextBoxMaskFormat.None:
                default:
                    // No additional restrictions
                    break;
            }
        }
        internal void ScrollToCaret()
        {
            _innerTextBox.ScrollToCaret();
            Invalidate();
        }
        internal void AppendText(string v)
        {
            _innerTextBox.AppendText(v);
            _innerTextBox.Invalidate();
            Invalidate();
        }
        #endregion "Key Events"
        #region "Search Events"
        private void OnSearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) OnSearchTriggered();
        }
        public event EventHandler SearchTriggered;
        protected virtual void OnSearchTriggered() => SearchTriggered?.Invoke(this, EventArgs.Empty);

        #endregion "Search Events"
        #region "Masking Logic"
        private void ApplyMaskFormat()
        {
            switch (MaskFormat)
            {
                case TextBoxMaskFormat.Currency:
                    ApplyCurrencyFormat();
                    break;
                case TextBoxMaskFormat.Percentage:
                    ApplyPercentageFormat();
                    break;
                case TextBoxMaskFormat.Date:
                    ApplyDateFormat();
                    break;
                case TextBoxMaskFormat.Time:
                    ApplyTimeFormat();
                    break;
                case TextBoxMaskFormat.PhoneNumber:
                    ApplyPhoneNumberFormat();
                    break;
                case TextBoxMaskFormat.Email:
                    // Email format doesn't alter display but can trigger validation
                    break;
                case TextBoxMaskFormat.SocialSecurityNumber:
                    ApplySSNFormat();
                    break;
                case TextBoxMaskFormat.ZipCode:
                    ApplyZipCodeFormat();
                    break;
                case TextBoxMaskFormat.IPAddress:
                    ApplyIPAddressFormat();
                    break;
                case TextBoxMaskFormat.CreditCard:
                    ApplyCreditCardFormat();
                    break;
                case TextBoxMaskFormat.Hexadecimal:
                    // Hexadecimal input is handled via KeyPress restrictions
                    break;
                case TextBoxMaskFormat.URL:
                    // URL format doesn't alter display but can trigger validation
                    break;
                case TextBoxMaskFormat.TimeSpan:
                    ApplyTimeSpanFormat();
                    break;
                case TextBoxMaskFormat.Decimal:
                    ApplyDecimalFormat();
                    break;
                case TextBoxMaskFormat.CurrencyWithoutSymbol:
                    ApplyCurrencyWithoutSymbolFormat();
                    break;
                case TextBoxMaskFormat.DateTime:
                    ApplyDateTimeFormat();
                    break;
                case TextBoxMaskFormat.Year:
                    ApplyYearFormat();
                    break;
                case TextBoxMaskFormat.MonthYear:
                    ApplyMonthYearFormat();
                    break;
                case TextBoxMaskFormat.Custom:
                    ApplyCustomFormat();
                    break;
                case TextBoxMaskFormat.Alphanumeric:
                case TextBoxMaskFormat.Numeric:
                case TextBoxMaskFormat.Password:
                case TextBoxMaskFormat.None:
                default:
                    // No formatting applied
                    break;
            }
        }
        private void ApplyCurrencyFormat()
        {
            if (decimal.TryParse(_innerTextBox.Text, out decimal value))
            {
                _innerTextBox.Text = value.ToString("C2"); // Currency format with 2 decimal places
                _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
            }
        }
        private void ApplyPercentageFormat()
        {
            if (decimal.TryParse(_innerTextBox.Text, out decimal value))
            {
                _innerTextBox.Text = value.ToString("P2"); // Percentage format with 2 decimal places
                _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
            }
        }
        private void ApplyDateFormat()
        {
            if (DateTime.TryParse(_innerTextBox.Text, out DateTime date))
            {
                _innerTextBox.Text = date.ToString(_dateFormat, CultureInfo.CurrentCulture); // Use custom date format
                _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
            }
        }
        private void ApplyTimeFormat()
        {
            if (DateTime.TryParse(_innerTextBox.Text, out DateTime time))
            {
                _innerTextBox.Text = time.ToString(_timeFormat, CultureInfo.CurrentCulture); // Use custom time format
                _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
            }
        }
        private void ApplyDateTimeFormat()
        {
            if (DateTime.TryParse(_innerTextBox.Text, out DateTime dateTime))
            {
                _innerTextBox.Text = dateTime.ToString(_dateTimeFormat);
                _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
            }
        }
        private void ApplyPhoneNumberFormat()
        {
            // Simple formatting for U.S. phone numbers
            string digits = Regex.Replace(_innerTextBox.Text, @"\D", "");
            if (digits.Length >= 10)
            {
                _innerTextBox.Text = Regex.Replace(digits, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3");
                _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
            }
        }
        private void ApplySSNFormat()
        {
            // Formats SSN as XXX-XX-XXXX
            string digits = Regex.Replace(_innerTextBox.Text, @"\D", "");
            if (digits.Length >= 9)
            {
                _innerTextBox.Text = Regex.Replace(digits, @"(\d{3})(\d{2})(\d{4})", "$1-$2-$3");
                _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
            }
        }
        private void ApplyZipCodeFormat()
        {
            // Formats ZIP code as XXXXX or XXXXX-XXXX
            string digits = Regex.Replace(_innerTextBox.Text, @"\D", "");
            if (digits.Length == 5)
            {
                _innerTextBox.Text = digits;
            }
            else if (digits.Length == 9)
            {
                _innerTextBox.Text = Regex.Replace(digits, @"(\d{5})(\d{4})", "$1-$2");
            }
            _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
        }
        private void ApplyIPAddressFormat()
        {
            // Simple IPv4 formatting
            string digits = Regex.Replace(_innerTextBox.Text, @"\D", "");
            if (digits.Length > 12) digits = digits.Substring(0, 12); // Max 12 digits for IPv4

            string ip = "";
            for (int i = 0; i < digits.Length; i++)
            {
                ip += digits[i];
                if ((i + 1) % 3 == 0 && i < digits.Length - 1)
                {
                    ip += ".";
                }
            }

            _innerTextBox.Text = ip;
            _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
        }
        private void ApplyCreditCardFormat()
        {
            // Formats credit card numbers as XXXX XXXX XXXX XXXX
            string digits = Regex.Replace(_innerTextBox.Text, @"\D", "");
            digits = digits.Length > 16 ? digits.Substring(0, 16) : digits;

            string formatted = Regex.Replace(digits, @"(\d{4})(?=\d)", "$1 ");
            _innerTextBox.Text = formatted.Trim();
            _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
        }
        private void ApplyTimeSpanFormat()
        {
            if (TimeSpan.TryParse(_innerTextBox.Text, out TimeSpan ts))
            {
                _innerTextBox.Text = ts.ToString(@"hh\:mm\:ss");
                _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
            }
        }
        private void ApplyDecimalFormat()
        {
            if (decimal.TryParse(_innerTextBox.Text, out decimal value))
            {
                _innerTextBox.Text = value.ToString("N2"); // Numeric format with 2 decimal places
                _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
            }
        }
        private void ApplyCurrencyWithoutSymbolFormat()
        {
            if (decimal.TryParse(_innerTextBox.Text, out decimal value))
            {
                _innerTextBox.Text = value.ToString("N2"); // Numeric format without currency symbol
                _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
            }
        }
        private void ApplyYearFormat()
        {
            if (int.TryParse(_innerTextBox.Text, out int year))
            {
                _innerTextBox.Text = year.ToString("D4"); // Ensure 4-digit year
                _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
            }
        }
        private void ApplyMonthYearFormat()
        {
            // Formats as MM/yyyy
            string digits = Regex.Replace(_innerTextBox.Text, @"\D", "");
            if (digits.Length >= 6)
            {
                _innerTextBox.Text = Regex.Replace(digits, @"(\d{2})(\d{4})", "$1/$2");
            }
            _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
        }
        private void ApplyCustomFormat()
        {
            if (!string.IsNullOrEmpty(CustomMask))
            {
                // Implement custom formatting based on CustomMask
                // This could involve regex-based formatting or other rules
                // For demonstration, let's assume CustomMask is a format string with placeholders
                try
                {
                    // Example: "AAA-9999" where A=Letter and 9=Digit
                    string formatted = "";
                    int digitIndex = 0, letterIndex = 0;
                    foreach (char maskChar in CustomMask)
                    {
                        if (maskChar == 'A' && letterIndex < _innerTextBox.Text.Length && char.IsLetter(_innerTextBox.Text[letterIndex]))
                        {
                            formatted += char.ToUpper(_innerTextBox.Text[letterIndex++]);
                        }
                        else if (maskChar == '9' && digitIndex < _innerTextBox.Text.Length && char.IsDigit(_innerTextBox.Text[digitIndex]))
                        {
                            formatted += _innerTextBox.Text[digitIndex++];
                        }
                        else
                        {
                            formatted += maskChar;
                        }
                    }

                    _innerTextBox.Text = formatted;
                    _innerTextBox.SelectionStart = _innerTextBox.Text.Length;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error applying custom mask: {ex.Message}");
                }
            }
        }
        private bool _isApplyingMask = false;
        #endregion "Masking Logic"
        #region "IBeepComponent Implementation"
      
        public bool ValidateData(out string message)
        {
            return EntityHelper.ValidateData(this.MaskFormat, this.Text, this.CustomMask, out message);
        }
        public void Clear()
        {
            _innerTextBox.Clear();
        }
        public void Focus()
        {
            _innerTextBox.Focus();
        }
        public void SelectAll()
        {
            _innerTextBox.SelectAll();
        }
        public bool SetFocus()
        {
            return _innerTextBox.Focus();
        }
        public override void SetValue(object value)
        {
            Text = value?.ToString();

        }
        public override object GetValue()
        {
            return Text;

        }
        public override void ClearValue()
        {
            Text = "";

        }
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Draw Image and Text
            if (beepImage.Visible)
            {
                beepImage.Draw(graphics, rectangle);
            }
            // Draw Text
            if (!string.IsNullOrEmpty(Text))
            {
                TextRenderer.DrawText(graphics, Text, Font, rectangle, ForeColor, TextFormatFlags.Left);
            }
        }


        #endregion "IBeepComponent Implementation"
        #region "Format Strings"

        // Define format strings for various MaskFormats
        private string _currencyFormat = "C2";       // Currency with 2 decimal places
        private string _percentageFormat = "P2";     // Percentage with 2 decimal places
        private string _decimalFormat = "N2";        // Number with 2 decimal places
        private string _monthYearFormat = "MM/yyyy"; // Month and Year format

        #endregion "Format Strings"
        #region "Theme and Style"
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _textFont = Font;
        //    Console.WriteLine("Font Changed");
            if (AutoSize)
            {
                Size textSize = TextRenderer.MeasureText(Text, _textFont);
                this.Size = new Size(textSize.Width + Padding.Horizontal, textSize.Height + Padding.Vertical);
            }
        }
        public override void ApplyTheme()
        {
            //base.ApplyTheme();
            // var themeBackColor = Color.FromArgb(255, _currentTheme.TextBoxBackColor.R, _currentTheme.TextBoxBackColor.G, _currentTheme.TextBoxBackColor.B);
             this.BackColor = _currentTheme.TextBoxBackColor;
            _innerTextBox.BackColor = _currentTheme.TextBoxBackColor;
            _innerTextBox.ForeColor = _currentTheme.TextBoxForeColor;
            // Remove borders to prevent interference
            _innerTextBox.BorderStyle = BorderStyle.None;

            // Force the text box to refresh
            _innerTextBox.Refresh();
            if (UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                if (_innerTextBox != null)
                {
                    //_innerTextBox.BeginInvoke(new Action(() => _innerTextBox.Font = _textFont));
                    _innerTextBox.Font = _textFont;
                }
             
                //InnerTextBox.Font=_listbuttontextFont;
                Font=_textFont;
            }
         

            beepImage.IsChild = true;
            beepImage.BackColor = _currentTheme.TextBoxBackColor;
            beepImage.ForeColor = _currentTheme.TextBoxForeColor;
            beepImage.BorderColor = _currentTheme.BorderColor;
            beepImage.HoverBackColor = _currentTheme.ButtonHoverBackColor;
            beepImage.HoverForeColor = _currentTheme.ButtonHoverForeColor;
            beepImage.PressedBackColor = _currentTheme.SelectedRowBackColor;
            beepImage.PressedForeColor = _currentTheme.SelectedRowForeColor;
            beepImage.ShowAllBorders = false;
            beepImage.IsFramless = true;
            if (IsChild)
            {
                BackColor = _currentTheme.TextBoxBackColor;
            }
            if (ApplyThemeOnImage)
            {
               // beepImage.ImageEmbededin = ImageEmbededin.TextBox;
                beepImage.ApplyThemeToSvg();
            }
            //  Refresh();           // Forcing the current control to refresh
            //   Parent?.Refresh();   // Ensuring the parent is also updated
            _innerTextBox.Invalidate();
            beepImage.Invalidate();
            Invalidate();
            Refresh();
        }
        #endregion "Theme and Style"

    }
}
