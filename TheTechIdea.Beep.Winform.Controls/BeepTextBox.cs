using TheTechIdea.Beep.Vis.Modules;
using System;
using System.ComponentModel;

using TheTechIdea.Beep.Winform.Controls.Helpers;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Controls")]
    public class BeepTextBox : BeepControl
    {
        #region "Properties"
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

        int offset = 1;
        [Browsable(true)]
        [Category("Appearance")]
        public int PreferredHeight
        {
            get => _innerTextBox.PreferredHeight;

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

                //if (!_multiline)
                //{
                //    // Fix the height when not multiline
                //    int singleLineHeight = GetSingleLineHeight();
                //    this.MinimumSize = new Size(0, singleLineHeight);
                //    this.MaximumSize = new Size(0, singleLineHeight);
                //    Height = singleLineHeight;
                //}
                //else
                //{
                //    // Allow resizing when multiline
                //    this.MinimumSize = new Size(0, 0);
                //    this.MaximumSize = new Size(0, 0);
                //}

                AdjustTextBoxHeight();
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TextBox InnerTextBox
        {
            get => _innerTextBox;
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
        public bool scrollbars
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
        public override string Text
        {
            get => _innerTextBox.Text;
            set
            {
                _innerTextBox.Text = ControlExtensions.GetFormattedText(value, MaskFormat); //ApplyMaskFormat(value);
                Invalidate();
            }
        }

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
        [Description("The font applied to the text displayed by the control.")]
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                if (_innerTextBox != null)
                {
                    _innerTextBox.Font = value;
                    AdjustTextBoxHeight(); // Adjust height based on new font size
                    Invalidate();
                }
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Specify the mask or format for text display (e.g., Currency, Percentage).")]
        public string MaskFormat
        {
            get => _maskFormat;
            set
            {
                _maskFormat = value;
                Text = _innerTextBox.Text; // Reapply formatting
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
        private int padding;
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
                        beepImage.Theme = Theme;

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
                    beepImage = new BeepImage();

                }
                if (beepImage != null)
                {
                    beepImage.ImagePath = value;
                    if (ApplyThemeOnImage)
                    {
                        beepImage.Theme = Theme;
                        beepImage.ApplyThemeOnImage = true;
                        beepImage.ApplyThemeToSvg();
                        beepImage.ApplyTheme();
                    }
                    Invalidate(); // Repaint when the image changes
                                  // UpdateSize();
                }
            }
        }
        #endregion "Properties"

        public BeepTextBox()
        {
            InitializeComponents();
          
            AutoSize = false;
            IsChild = true;
            BoundProperty = "Text";
            ApplyTheme(); // Ensure _currentTheme is initialized
            // Ensure size adjustments occur after initialization
            UpdateDrawingRect();
            AdjustTextBoxHeight();
            PositionInnerTextBoxAndImage();
          
            //AdjustTextBoxHeight();
            //PositionInnerTextBoxAndImage();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
        }
        protected override Size DefaultSize => new Size (200, GetSingleLineHeight());

        public int SelectionStart { get => _innerTextBox.SelectionStart; set { _innerTextBox.SelectionStart = value; } }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
        }
        protected override void InitLayout()
        {
            base.InitLayout();
           
        }

        private void BeepTextBox_Invalidated(object? sender, InvalidateEventArgs e)
        {
            _isControlinvalidated=true;
           // Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //if (_isControlinvalidated) {
            //    if (ShowAllBorders) { _innerTextBox.BorderStyle = BorderStyle.None; } else { _innerTextBox.BorderStyle = BorderStyle.FixedSingle; }
            //    _isControlinvalidated = false;
            //}
        }
        private void InitializeComponents()
        {
            _innerTextBox = new TextBox
            {
                BorderStyle = System.Windows.Forms.BorderStyle.None,
               // Multiline = true,
                //ScrollBars= ScrollBars.Both
            };
             IsCustomeBorder=true;   
            _innerTextBox.TextChanged += InnerTextBox_TextChanged;
            _innerTextBox.KeyPress += InnerTextBox_KeyPress;
            _innerTextBox.KeyDown += OnSearchKeyDown;
            _innerTextBox.MouseEnter += OnMouseEnter;
            _innerTextBox.MouseLeave += OnMouseLeave;
            _innerTextBox.TextChanged += (s, e) => Invalidate(); // Repaint to apply formatting
            Controls.Add(_innerTextBox);
        
            beepImage = new BeepImage { Size = _maxImageSize, Dock = DockStyle.None, Margin = new Padding(0) };
     //       Console.WriteLine("InitializeComponents");
            //AdjustTextBoxHeight();
            //PositionInnerTextBoxAndImage();

        }
        #region "Size and Position"
       // protected override Size DefaultSize => GetDefaultSize();
        private int GetSingleLineHeight()
        {
            // Ensure DrawingRect is updated
            UpdateDrawingRect();
            int textBoxHeight;
            padding = BorderThickness + offset;
            spacing = 5;
            using (TextBox tempTextBox = new TextBox())
            {
                tempTextBox.Multiline = false;
                tempTextBox.BorderStyle = BorderStyle.None;
                tempTextBox.Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall);

                 textBoxHeight = tempTextBox.PreferredHeight + (padding * 2);

                // Calculate the total height, including borders and padding
            }
       //    Console.WriteLine($" GetSingleLineHeight : {textBoxHeight}");

            return textBoxHeight;
        }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (!_multiline)
            {
                // Update DrawingRect to get accurate measurements
                UpdateDrawingRect();

                int singleLineHeight = GetSingleLineHeight();

                // Set Minimum and Maximum height to enforce fixed height
                this.MinimumSize = new Size(0, singleLineHeight);
                this.MaximumSize = new Size(0, singleLineHeight);

                height = singleLineHeight;
                specified &= ~BoundsSpecified.Height; // Remove the Height flag to prevent external changes
            }
            else
            {
                // Remove any height constraints when multiline
                this.MinimumSize = new Size(0, 0);
                this.MaximumSize = new Size(0, 0);
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
           // Console.WriteLine("OnResize");  
            UpdateDrawingRect();
            AdjustTextBoxHeight();
            PositionInnerTextBoxAndImage();
        }
        private void AdjustTextBoxHeight()
        {
            if (_multiline)
            {
                // Set the inner TextBox's height to fill the DrawingRect
                _innerTextBox.Height = DrawingRect.Height;
                _innerTextBox.Location = new Point(DrawingRect.X, DrawingRect.Y);
            }
            else
            {
                // Ensure DrawingRect is updated
                UpdateDrawingRect();

                // Set the inner TextBox's height to its preferred height
                _innerTextBox.Height = _innerTextBox.PreferredHeight;
             //   Console.WriteLine($" _innerTextBox.Height  :{_innerTextBox.Height}");
                // Center the inner TextBox vertically within the DrawingRect
                int textBoxY = DrawingRect.Y + (DrawingRect.Height - _innerTextBox.Height) / 2;
                _innerTextBox.Location = new Point(DrawingRect.X, textBoxY);
            }
        //    Console.WriteLine("AdjustTextBoxHeight");
            // Set the width of the inner TextBox to match the DrawingRect
            _innerTextBox.Width = DrawingRect.Width;
        }
        private void PositionInnerTextBoxAndImage()
        {
        //   Console.WriteLine("PositionInnerTextBoxAndImage");
            // Ensure that DrawingRect is updated
            UpdateDrawingRect();

            // Get the image size
            Size imageSize = beepImage.HasImage ? beepImage.GetImageSize() : Size.Empty;

            // Scale image size to respect the max image size
            if (imageSize.Width > _maxImageSize.Width || imageSize.Height > _maxImageSize.Height)
            {
                float scaleFactor = Math.Min(
                    (float)_maxImageSize.Width / imageSize.Width,
                    (float)_maxImageSize.Height / imageSize.Height);
                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }

            // Calculate the height of the inner TextBox
            int textBoxHeight = _innerTextBox.Multiline ? DrawingRect.Height : GetSingleLineHeight();

            // Center the TextBox vertically if not multiline
            int textBoxY = DrawingRect.Y + (DrawingRect.Height - textBoxHeight) / 2;

            if (string.IsNullOrEmpty(ImagePath))
            {
            //    Console.WriteLine($" Hight :{textBoxHeight}");
                // Position the TextBox
                _innerTextBox.Location = new Point(DrawingRect.X, textBoxY);
                _innerTextBox.Size = new Size(DrawingRect.Width, textBoxHeight);
            }
            else if (_textImageRelation == TextImageRelation.ImageBeforeText)
            {
                // Position the image inside DrawingRect
                int imageY = DrawingRect.Y + (DrawingRect.Height - imageSize.Height) / 2;
                beepImage.Location = new Point(DrawingRect.X, imageY);
                beepImage.Size = imageSize;

                // Position the TextBox next to the image
                int textBoxX = beepImage.Right;
                int textBoxWidth = DrawingRect.Right - beepImage.Right;

                _innerTextBox.Location = new Point(textBoxX, textBoxY);
                _innerTextBox.Size = new Size(textBoxWidth, textBoxHeight);
            }
            else if (_textImageRelation == TextImageRelation.TextBeforeImage)
            {
                // Position the TextBox
                int textBoxWidth = DrawingRect.Width - imageSize.Width;
                _innerTextBox.Location = new Point(DrawingRect.X, textBoxY);
                _innerTextBox.Size = new Size(textBoxWidth, textBoxHeight);

                // Position the image after the TextBox
                int imageY = DrawingRect.Y + (DrawingRect.Height - imageSize.Height) / 2;
                beepImage.Location = new Point(_innerTextBox.Right, imageY);
                beepImage.Size = imageSize;
            }
        }
        #endregion "Size and Position"
        public override void DrawCustomBorder(PaintEventArgs e)
        {
            if(!ShowAllBorders) 
            { _innerTextBox.BorderStyle = BorderStyle.None; } 
            else { _innerTextBox.BorderStyle = BorderStyle.FixedSingle; }
        }
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

       
        private void InnerTextBox_TextChanged(object sender, EventArgs e)
        {
            Text = _innerTextBox.Text;
            Invalidate();
        }

        private void InnerTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((OnlyDigits && !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) ||
                (OnlyCharacters && !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }
     


        public override void ApplyTheme()
        {
            //base.ApplyTheme();
            _innerTextBox.BackColor = _currentTheme.TextBoxBackColor;
            _innerTextBox.ForeColor = _currentTheme.TextBoxForeColor;
            Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
            BackColor =_currentTheme.BackColor  ;
            beepImage.ApplyTheme();
            Invalidate();
        }

   
        private void OnSearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) OnSearchTriggered();
        }
        public event EventHandler SearchTriggered;
        protected virtual void OnSearchTriggered() => SearchTriggered?.Invoke(this, EventArgs.Empty);

        internal void ScrollToCaret()
        {
            _innerTextBox.ScrollToCaret();
        }

        internal void AppendText(string v)
        {
           _innerTextBox.AppendText(v);
        }
    }
}
