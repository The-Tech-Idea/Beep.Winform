using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Runtime.InteropServices;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using System.Drawing.Drawing2D;



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
        private BeepImage beepImage;
        private string _maskFormat = "";
        private bool _onlyDigits = false;
        private bool _onlyCharacters = false;
        private TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;
        private ContentAlignment _imageAlign = ContentAlignment.MiddleLeft;
        private Size _maxImageSize = new Size(16, 16); // Default image size
        private string? _imagepath;
        private bool _multiline = false;
        int padding = 1;
      
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
            get
            {
                if (_multiline)
                {
                    return _innerTextBox.PreferredHeight;
                }
                else
                {
                    // Calculate single-line height without extra padding
                    using (TextBox tempTextBox = new TextBox())
                    {
                        tempTextBox.Multiline = false;
                        tempTextBox.BorderStyle = BorderStyle.None;
                        tempTextBox.Font = TextFont;
                        return tempTextBox.PreferredHeight;
                    }
                }
            }
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
                _innerTextBox.ScrollBars = ScrollBars.Vertical;
                _innerTextBox.WordWrap = true;
                // _innerTextBox.Multiline = value;
                //AdjustTextBoxHeight();
                //PositionInnerTextBoxAndImage();
                Invalidate();
            }
        }
        
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
        public HorizontalAlignment TextAlignment
        {
            get => _innerTextBox.TextAlign;
            set
            {
                _innerTextBox.TextAlign = value;
                Invalidate();
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
        [Category("Data")]
        [Description("The Text  represent for the control.")]
        public override string Text
        {
            get => _text;
            set
            {
                _text = value;
                _isControlinvalidated = true;
                _innerTextBox.Text = value;
                Invalidate();  // Trigger repaint when the text changes
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
                if (beepImage != null)
                {
                    beepImage.Size = value;
                  
                }
                PositionInnerTextBoxAndImage();
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
                        beepImage.ApplyThemeOnImage = ApplyThemeOnImage;
                         beepImage.Theme = Theme;
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
                    beepImage = new BeepImage()
                    {
                        Size = _maxImageSize,
                        ShowAllBorders = false,
                        IsBorderAffectedByTheme = false,
                        IsShadowAffectedByTheme = false,
                        IsChild = true,
                      
                        Dock = DockStyle.None,
                        Margin = new Padding(0)
                    };

                }
                if (beepImage != null)
                {
                    beepImage.ImagePath = value;
                    if (ApplyThemeOnImage)
                    {
                       // beepImage.Theme = Theme;
                        beepImage.ApplyThemeOnImage = ApplyThemeOnImage;
                        beepImage.ApplyThemeToSvg();
                        ApplyTheme();
                    }
                    Invalidate(); // Repaint when the image changes
                                  // UpdateSize();
                   // PositionInnerTextBoxAndImage();
                }
            }
        }
        //[Browsable(true)]
        //[Category("Appearance")]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        //public new bool IsChild
        //{
        //    get => _isChild;
        //    set
        //    {
        //        _isChild = value;
        //        base.IsChild = value;
        //        Control parent = null;
        //        if (this.Parent != null)
        //        {
        //            parent = this.Parent;
        //        }
        //        if (parent != null)
        //        {
        //            if (value)
        //            {
        //                parentbackcolor = parent.BackColor;
        //                TempBackColor = _innerTextBox.BackColor;
        //                BackColor = parentbackcolor;
        //                _innerTextBox.BackColor = parentbackcolor;
        //                beepImage.BackColor = parentbackcolor;
        //                beepImage.ParentBackColor = parentbackcolor;
        //            }
        //            else
        //            {
        //                beepImage.BackColor = _tempbackcolor;
        //                BackColor = _tempbackcolor;
        //                _innerTextBox.BackColor = _tempbackcolor;
        //            }
        //        }
        //        else
        //        {
        //         //   //Debug.WriteLine($"No Parent ");
        //            return;
        //        }
        //        Invalidate();
        //       _innerTextBox.Invalidate()  ;  // Trigger repaint
        //    }
        //}
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
        public BeepTextBox():base()
        {
           
            //      BackColor = Color.Black;
           // Padding = new Padding(3);
            InitializeComponents();
            base.TextChanged += BeepTextBox_TextChanged;
            base.ForeColorChanged += BeepTextBox_ForeColorChanged;
            base.BackColorChanged += BeepTextBox_BackColorChanged;
            base.EnabledChanged += BeepTextBox_EnabledChanged;
           
            // AutoSize = true;
            BoundProperty = "Text";
            BorderRadius=3;
            Padding = new Padding(3);
            beepImage.IsChild = true;
            ShowAllBorders=true;
            IsShadowAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            AutoSize = false; // Prevents automatic shrinking
           // Value = DefaultSize; // Ensures a default size
            //   ApplyTheme(); // Ensure _currentTheme is initialized
            // Ensure size adjustments occur after initialization
            UpdateDrawingRect();
            AdjustTextBoxHeight();
            PositionInnerTextBoxAndImage();
            _innerTextBox.TextChanged += (s, e) =>
            {
                _text = _innerTextBox.Text;
                TextChanged?.Invoke(this, EventArgs.Empty);
                ////MiscFunctions.SendLog($"📝 BeepTextBox.TextChanged: {_innerTextBox.Text}");
            };
            // Inside BeepTextBox constructor or initialization
            _innerTextBox.GotFocus += (s, e) => {
                // Set a flag if needed beyond standard Focused property
                // Invalidate to trigger redraw with focus styles
                this.Invalidate();
                // Potentially update parent AppBar if applicable? (Check BeepAppBar logic)
            };
            _innerTextBox.LostFocus += (s, e) => {
                this.Invalidate();
            };
            _innerTextBox.Font = TextFont; // Ensure font is set early
            _innerTextBox.BorderStyle = BorderStyle.None;
            _innerTextBox.Multiline = false; // Default to single-line
            _innerTextBox.AutoSize = false; // Prevent auto-resize
            //_innerTextBox.GotFocus += (s, e) =>
            //{
            //   ////MiscFunctions.SendLog($"🎯 BeepTextBox Got Focus: {_innerTextBox.Text}");
            //};
        }


        #endregion "Constructors"
        #region "Initialization"
        private void InitializeScaling()
        {
            float scaleFactor = DeviceDpi / 96f;
            padding = (int)(2 * scaleFactor);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            InitializeScaling();
            AdjustTextBoxHeight();
            PositionInnerTextBoxAndImage();
        }
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
            _innerTextBox.BorderStyle = BorderStyle.None;
       //     _innerTextBox.AcceptsTab = true;
            _innerTextBox.Invalidated += BeepTextBox_Invalidated;
            //  _innerTextBox.MouseEnter += OnMouseEnter;
            //  _innerTextBox.MouseLeave += OnMouseLeave;
            //           _innerTextBox.TextChanged += (s, e) => Invalidate(); // Repaint to apply formatting
            Controls.Add(_innerTextBox);
            _innerTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            beepImage = new BeepImage() 
            {
                Size = _maxImageSize,
                ShowAllBorders = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                IsChild = true,
                ClipShape = ImageClipShape.None

            };
            Controls.Add(beepImage);
            //      // Console.WriteLine("InitializeComponents");
            //AdjustTextBoxHeight();
           // PositionInnerTextBoxAndImage();

        }
        #endregion "Initialization"
        #region "Paint and Invalidate"
        private void BeepTextBox_EnabledChanged(object? sender, EventArgs e)
        {
            _innerTextBox.Enabled = Enabled;
        }

        private void BeepTextBox_BackColorChanged(object? sender, EventArgs e)
        {
            _innerTextBox.BackColor = BackColor;
        }

        private void BeepTextBox_ForeColorChanged(object? sender, EventArgs e)
        {
            _innerTextBox.ForeColor = ForeColor;
        }
        private void BeepTextBox_TextChanged(object sender, EventArgs e)
        {
         //  ////MiscFunctions.SendLog($"✅ BeepLabel TextChanged: {base.Text}");
          
            _innerTextBox.Text = Text;
            if (_isApplyingMask)
            {
                ApplyMaskFormat();

            }
            // Invalidate();
        }
        private void BeepTextBox_Invalidated(object? sender, InvalidateEventArgs e)
        {
    //        _isControlinvalidated = true;
      //      if(_innerTextBox!=null)   _innerTextBox.Invalidate();
            Invalidate();
        }
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    base.OnPaint(e);

        //}
        //protected override void DrawContent(Graphics g)
        //{
        //    base.DrawContent(g);
        //    UpdateDrawingRect();
        //    _innerTextBox.Invalidate();
        //}

        #endregion "Paint and Invalidate"
        #region "Size and Position"
        // protected override Value DefaultSize => GetDefaultSize();
        private int GetSingleLineHeight()
        {
            // Don't add additional padding here as it's already accounted for elsewhere
            using (TextBox tempTextBox = new TextBox())
            {
                tempTextBox.Text = "A";
                tempTextBox.Multiline = false;
                tempTextBox.BorderStyle = BorderStyle.None;
                tempTextBox.Font = TextFont;
                tempTextBox.Refresh();
                // Return raw height without adding extra padding
                return tempTextBox.PreferredHeight;
            }
        }

        //protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        //{
        //    // Ensure DrawingRect is updated from BeepControl
        //    UpdateDrawingRect();

        //    // Prevent zero-size issues
        //    int minWidth = Math.Max(50, DrawingRect.Width);
        //    int minHeight = Math.Max(GetSingleLineHeight(), DrawingRect.Height);

        //    // Ensure the width and height do not go below the minimum
        //    width = Math.Max(width, minWidth);
        //    height = Math.Max(height, minHeight);

        //    // Prevent auto-resizing conflict when multiline but AutoSize is false
        //    if (_multiline && !AutoSize)
        //    {
        //        base.SetBoundsCore(DrawingRect.X, DrawingRect.Y, DrawingRect.Width, DrawingRect.Height, specified);
        //        return;
        //    }

        //    // Ensure height is always a multiple of the single-line height
        //    int singleLineHeight = GetSingleLineHeight();
        //    if (height % singleLineHeight != 0)
        //    {
        //        height = ((height / singleLineHeight) * singleLineHeight);
        //    }

        //    // Apply the final constrained size within DrawingRect
        //    base.SetBoundsCore(x, y, width, height, specified);
        //}

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Width == 0 || Height == 0) return;

            UpdateDrawingRect();
            Rectangle rect = DrawingRect;
            int singleLineHeight = GetSingleLineHeight();
            int minWidth = Math.Max(10, rect.Width);
            int minHeight = _multiline ? rect.Height : singleLineHeight;

            if (Width < minWidth) Width = minWidth;
            if (Height < minHeight) Height = minHeight;

            AdjustTextBoxHeight();
            PositionInnerTextBoxAndImage();
            // Update inner TextBox text alignment to match drawn text
            switch (TextAlignment)
            {
                case HorizontalAlignment.Center:
                    _innerTextBox.TextAlign = HorizontalAlignment.Center;
                    break;
                case HorizontalAlignment.Right:
                    _innerTextBox.TextAlign = HorizontalAlignment.Right;
                    break;
                case HorizontalAlignment.Left:
                default:
                    _innerTextBox.TextAlign = HorizontalAlignment.Left;
                    break;
            }
        }
        private void AdjustTextBoxHeight()
        {
            UpdateDrawingRect(); // Ensure DrawingRect is current

            // Calculate available space accounting for padding only once
            int availableWidth = DrawingRect.Width - (padding * 2);
            int availableHeight = DrawingRect.Height - (padding * 2);

            if (_multiline)
            {
                _innerTextBox.Multiline = true;
                _innerTextBox.Size = new Size(Math.Max(1, availableWidth), Math.Max(1, availableHeight));
            }
            else
            {
                _innerTextBox.Multiline = false;
                // For single line, use the full width but only the needed height
                _innerTextBox.Size = new Size(Math.Max(1, availableWidth), GetSingleLineHeight());
            }

            // Position the textbox with proper padding
            _innerTextBox.Location = new Point(DrawingRect.X + padding, DrawingRect.Y + padding);
        }
        private void PositionInnerTextBoxAndImage()
        {
            beepImage.IsChild = true;
            beepImage.ParentBackColor = BackColor;
            bool hasImage = !string.IsNullOrEmpty(ImagePath) && beepImage != null;
            if (beepImage != null)
            {
                beepImage.Visible = hasImage;
            }

            // Early exit if control is too small or not initialized
            if (Width == 0 || Height == 0 || DrawingRect.Width <= 0 || DrawingRect.Height <= 0)
            {
                return;
            }

            Rectangle rect = DrawingRect;
            int availableWidth = rect.Width - (padding * 2);
            int availableHeight = rect.Height - (padding * 2);

            // Get single line height if not multiline
            int textBoxHeight = _multiline ? availableHeight : GetSingleLineHeight();

            // Calculate vertical center position for both text box and image
            int textBoxY = rect.Y + ((rect.Height - textBoxHeight) / 2);

            if (hasImage && beepImage != null)
            {
                // Ensure image size is correctly set using the MaxImageSize property
                beepImage.Size = new Size(_maxImageSize.Width, _maxImageSize.Height);
           //     beepImage.MaxImageSize = new Size(_maxImageSize.Width, _maxImageSize.Height);

                // Calculate vertical center for image
                int imageY = rect.Y + ((rect.Height - _maxImageSize.Height) / 2);

                // Position based on TextImageRelation
                if (_textImageRelation == TextImageRelation.ImageBeforeText)
                {
                    // Image on the left
                    int imageX = rect.X + padding;
                    beepImage.Location = new Point(imageX, imageY);

                    // Text box starts after image with spacing
                    int textBoxX = imageX + _maxImageSize.Width + spacing;
                    int textBoxWidth = Math.Max(1, availableWidth - _maxImageSize.Width - spacing);

                    _innerTextBox.Location = new Point(textBoxX, textBoxY);
                    _innerTextBox.Size = new Size(textBoxWidth, textBoxHeight);
                }
                else // TextImageRelation.TextBeforeImage
                {
                    // Text box on the left
                    int textBoxX = rect.X + padding;
                    int textBoxWidth = Math.Max(1, availableWidth - _maxImageSize.Width - spacing);

                    _innerTextBox.Location = new Point(textBoxX, textBoxY);
                    _innerTextBox.Size = new Size(textBoxWidth, textBoxHeight);

                    // Image after text
                    int imageX = textBoxX + textBoxWidth + spacing;
                    beepImage.Location = new Point(imageX, imageY);
                }
            }
            else
            {
                // No image, position text box to take full width
                _innerTextBox.Location = new Point(rect.X + padding, textBoxY);
                _innerTextBox.Size = new Size(Math.Max(1, availableWidth), textBoxHeight);
            }
        }


        #endregion "Size and Position"
        #region "Mouse Events"
        //protected override void OnMouseEnter(EventArgs e)
        //{

        //}
        //protected override void OnMouseLeave(EventArgs e)
        //{

        //}

        //protected override void OnGotFocus(EventArgs e)
        //{

        //}

        #endregion "Mouse Events"
        #region "Key Events"
        private void InnerTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_isApplyingMask)
            {
                _isApplyingMask = true;
                ApplyMaskFormat();
                _isApplyingMask = false;

              //  Text = _innerTextBox.Text;
                TextChanged?.Invoke(this, EventArgs.Empty);
              //  Invalidate();
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
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private const int WM_VSCROLL = 0x115;
        private const int SB_BOTTOM = 7;

        private void ForceScrollToBottom()
        {
            if (_innerTextBox != null)
                SendMessage(_innerTextBox.Handle, WM_VSCROLL, SB_BOTTOM, 0);
        }

        public void ScrollToCaret()
        {
           
            
            _innerTextBox.ScrollToCaret();
            
          
            Invalidate();
        }
        public void AppendText(string v)
        {
            if (_innerTextBox == null) return;

            _innerTextBox.AppendText(v);

            // Move the caret to the end
            _innerTextBox.SelectionStart = _innerTextBox.Text.Length;

            // Call ScrollToCaret() asynchronously to allow UI updates
            _innerTextBox.BeginInvoke(new Action(() =>
            {
                _innerTextBox.ScrollToCaret();
                ForceScrollToBottom();
            }));

            _innerTextBox.Invalidate();
            Invalidate();
        }



        #endregion "Key Events"
        #region "Search Events"
        private void OnSearchKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
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
            this.Text = value?.ToString();
            _innerTextBox.Text = this.Text;
        }
        public override object GetValue()
        {
            return _innerTextBox.Text;

        }
        public override void ClearValue()
        {
            Text = "";

        }
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Enable high-quality rendering
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.ResetTransform();

            // Clip all drawing to the rectangle bounds
            using (Region clipRegion = new Region(rectangle))
            {
                graphics.Clip = clipRegion;

                // First, draw the background
                Color bgColor = Enabled ? BackColor : DisabledBackColor;
                using (SolidBrush bgBrush = new SolidBrush(bgColor))
                {
                    if (IsRounded)
                    {
                        using (GraphicsPath path = GetRoundedRectPath(rectangle, BorderRadius))
                        {
                            graphics.FillPath(bgBrush, path);
                        }
                    }
                    else
                    {
                        graphics.FillRectangle(bgBrush, rectangle);
                    }
                }

                // Draw the border
                Color borderColor = BorderColor;
                using (Pen borderPen = new Pen(borderColor, 1))
                {
                    if (IsRounded)
                    {
                        using (GraphicsPath path = GetRoundedRectPath(rectangle, BorderRadius))
                        {
                            graphics.DrawPath(borderPen, path);
                        }
                    }
                    else
                    {
                        graphics.DrawRectangle(borderPen, rectangle);
                    }
                }

                // Calculate inner rectangle for content (text and image)
                Rectangle innerRect = new Rectangle(
                    rectangle.X + padding,
                    rectangle.Y + padding,
                    rectangle.Width - (padding * 2),
                    rectangle.Height - (padding * 2)
                );

                // Draw Image if visible
                if (beepImage != null && !string.IsNullOrEmpty(beepImage.ImagePath))
                {
                    // Calculate image position based on TextImageRelation
                    Rectangle imageRect;

                    if (_textImageRelation == TextImageRelation.ImageBeforeText)
                    {
                        // Image on left
                        imageRect = new Rectangle(
                            innerRect.X,
                            innerRect.Y + ((innerRect.Height - _maxImageSize.Height) / 2),
                            _maxImageSize.Width,
                            _maxImageSize.Height
                        );

                        // Adjust inner rectangle for text
                        innerRect.X += _maxImageSize.Width + spacing;
                        innerRect.Width -= _maxImageSize.Width + spacing;
                    }
                    else
                    {
                        // Image on right
                        imageRect = new Rectangle(
                            innerRect.Right - _maxImageSize.Width,
                            innerRect.Y + ((innerRect.Height - _maxImageSize.Height) / 2),
                            _maxImageSize.Width,
                            _maxImageSize.Height
                        );

                        // Adjust inner rectangle for text
                        innerRect.Width -= _maxImageSize.Width + spacing;
                    }

                    // Draw the image
                    beepImage.Draw(graphics, imageRect);
                }

                // Determine text to draw - either actual text or placeholder
                string textToDraw = Text;
                Color textColor = ForeColor;

                // Use placeholder text if actual text is empty
                if (string.IsNullOrEmpty(textToDraw) && !string.IsNullOrEmpty(PlaceholderText))
                {
                    textToDraw = PlaceholderText;
                    textColor = _currentTheme?.TextBoxPlaceholderColor ?? Color.Gray;
                }

                // Draw the text if there's something to draw
                if (!string.IsNullOrEmpty(textToDraw))
                {
                    TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;

                    if (Multiline)
                    {
                        flags |= TextFormatFlags.WordBreak; // Allow word wrapping for multiline
                    }
                    else
                    {
                        flags |= TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter; // Force single line for non-multiline
                    }

                    // Set text alignment
                    switch (TextAlignment)
                    {
                        case HorizontalAlignment.Center:
                            flags |= TextFormatFlags.HorizontalCenter;
                            break;
                        case HorizontalAlignment.Right:
                            flags |= TextFormatFlags.Right;
                            break;
                        case HorizontalAlignment.Left:
                        default:
                            flags |= TextFormatFlags.Left;
                            break;
                    }

                    // Handle password masking if needed
                    if (UseSystemPasswordChar && !string.IsNullOrEmpty(Text))
                    {
                        // Use a solid bullet for password masking
                        textToDraw = new string('•', Text.Length);
                    }
                    else if (PasswordChar != '\0' && !string.IsNullOrEmpty(Text))
                    {
                        // Use the custom password char
                        textToDraw = new string(PasswordChar, Text.Length);
                    }

                    // Draw the text
                    TextRenderer.DrawText(graphics, textToDraw, TextFont, innerRect, textColor, flags);
                }

                graphics.ResetClip();
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
        //   // Console.WriteLine("Font Changed");
            if (AutoSize)
            {
                Size textSize = TextRenderer.MeasureText(Text, _textFont);
                this.Size = new Size(textSize.Width + Padding.Horizontal, textSize.Height + Padding.Vertical);
            }
        }
        public void AfterThemeApplied()
        {
            beepImage.BackColor = BackColor;
            beepImage.ParentBackColor = BackColor;
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

            //  ////MiscFunctions.SendLog($"AfterThemeApplied: {this.Text}");
        }
        public override void ApplyTheme()
        {
            if (_currentTheme == null) return;
            base.ApplyTheme();
            if (IsChild && Parent != null)
            {
                parentbackcolor = Parent.BackColor;
                BackColor = parentbackcolor;
                _innerTextBox.BackColor = parentbackcolor;
            }
            else
            {
                this.BackColor = _currentTheme.TextBoxBackColor;
                _innerTextBox.BackColor = _currentTheme.TextBoxBackColor;
            }

                // var themeBackColor = Color.FromArgb(255, _currentTheme.TextBoxBackColor.R, _currentTheme.TextBoxBackColor.G, _currentTheme.TextBoxBackColor.B);
              
            
            _innerTextBox.ForeColor = _currentTheme.TextBoxForeColor;
            ForeColor = _currentTheme.TextBoxForeColor;
          
            SelectedBackColor = _currentTheme.TextBoxBackColor;
            SelectedForeColor = _currentTheme.TextBoxForeColor;
            HoverBackColor = _currentTheme.TextBoxHoverBackColor;
            HoverForeColor = _currentTheme.TextBoxHoverForeColor;
            DisabledBackColor = _currentTheme.DisabledBackColor;
            DisabledForeColor = _currentTheme.DisabledForeColor;
            // Remove borders to prevent interference
            _innerTextBox.BorderStyle = BorderStyle.None;
            BorderColor=_currentTheme.BorderColor;
            // Force the text box to refresh
          
            if (UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
               
            }
            if (_innerTextBox != null)
            {
                //_innerTextBox.BeginInvoke(new Action(() => _innerTextBox.Font = _textFont));
                _innerTextBox.Font = _textFont;
            }

            //InnerTextBox.Font=_listbuttontextFont;
            SafeApplyFont(TextFont ?? _textFont);

            beepImage.IsChild = true;
            beepImage.ImageEmbededin = ImageEmbededin.None;
            beepImage.ParentBackColor = BackColor; 
            beepImage.BackColor = BackColor;
            beepImage.ForeColor = _currentTheme.TextBoxForeColor;
            beepImage.BorderColor = _currentTheme.BorderColor;
            beepImage.HoverBackColor = _currentTheme.TextBoxHoverBackColor;
            beepImage.HoverForeColor = _currentTheme.TextBoxHoverForeColor;
           
            beepImage.ShowAllBorders = false;
            beepImage.IsFrameless = true;
            beepImage.IsBorderAffectedByTheme = false;
            beepImage.IsShadowAffectedByTheme = false;
            beepImage.BorderColor = _currentTheme.TextBoxBorderColor;
            if (beepImage != null)
            {
                
                if (ApplyThemeOnImage)
                {
                    // beepImage.Theme = Theme;
                    beepImage.ApplyThemeOnImage = ApplyThemeOnImage;
                    beepImage.ApplyThemeToSvg();
                    
                }
                Invalidate(); // Repaint when the image changes
                              // UpdateSize();
                              // PositionInnerTextBoxAndImage();
            }
            AfterThemeApplied();
        }
        #endregion "Theme and Style"
    }
}
