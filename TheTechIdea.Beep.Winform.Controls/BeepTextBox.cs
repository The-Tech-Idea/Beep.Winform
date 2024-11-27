using TheTechIdea.Beep.Vis.Modules;
using System;
using System.ComponentModel;

using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Controls")]
    public class BeepTextBox : BeepControl
    {
        private TextBox _innerTextBox;
        private BeepImage beepImage;
        private string _maskFormat = "";
        private bool _onlyDigits = false;
        private bool _onlyCharacters = false;
        private TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;
        private ContentAlignment _imageAlign = ContentAlignment.MiddleLeft;
        private Size _maxImageSize = new Size(16, 16); // Default image size
        private string? _imagepath;
        private bool _multiline=false;


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
        public  AutoCompleteMode AutoCompleteMode
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
        public BeepTextBox()
        {
            InitializeComponents();
            Size = new Size(150, 23);
            _innerTextBox.TextChanged += (s, e) => Invalidate(); // Repaint to apply formatting
            this.Invalidated += BeepTextBox_Invalidated;

        }
        
        private void BeepTextBox_Invalidated(object? sender, InvalidateEventArgs e)
        {
            _isControlinvalidated=true;
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
            Controls.Add(_innerTextBox);
        
            beepImage = new BeepImage { Size = _maxImageSize, Dock = DockStyle.None, Margin = new Padding(0) };
            AdjustTextBoxHeight();
            PositionInnerTextBoxAndImage();
        }
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
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustTextBoxHeight();
            PositionInnerTextBoxAndImage();
           
        }
        private void AdjustTextBoxHeight()
        {
            // Override the TextBox's height
            int innerHeight = Height - 2; // Adjust for padding if necessary
            if (_innerTextBox.Multiline)
            {
                _innerTextBox.Height = innerHeight; // Allow full height for multiline mode
            }
            else
            {
                // For single-line TextBox, ensure it's not restricted by font size
                int minimumHeight = _innerTextBox.Font.Height + 2; // Minimum height based on font size
                _innerTextBox.Height = Math.Max(innerHeight, minimumHeight);
            }
        }
        private void PositionInnerTextBoxAndImage()
        {
            int padding = 2; // Adjust padding as needed
            Size imageSize = beepImage.HasImage ? beepImage.GetImageSize() : Size.Empty;

            // Ensure the image size respects the maximum size
            if (imageSize.Width > _maxImageSize.Width || imageSize.Height > _maxImageSize.Height)
            {
                float scaleFactor = Math.Min(
                    (float)_maxImageSize.Width / imageSize.Width,
                    (float)_maxImageSize.Height / imageSize.Height);
                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }

            if (string.IsNullOrEmpty(ImagePath))
            {
                _innerTextBox.Location = new Point(padding, padding);
                _innerTextBox.Width = Width - 2 * padding;
            }
            else if (_textImageRelation == TextImageRelation.ImageBeforeText)
            {
                beepImage.Location = new Point(padding, (Height - imageSize.Height) / 2);
                beepImage.Size = imageSize;

                _innerTextBox.Location = new Point(beepImage.Right + padding, padding);
                _innerTextBox.Width = Width - beepImage.Width - 3 * padding;
            }
            else if (_textImageRelation == TextImageRelation.TextBeforeImage)
            {
                _innerTextBox.Location = new Point(padding, padding);
                _innerTextBox.Width = Width - imageSize.Width - 3 * padding;

                beepImage.Location = new Point(_innerTextBox.Right + padding, (Height - imageSize.Height) / 2);
                beepImage.Size = imageSize;
            }

            _innerTextBox.Height = _innerTextBox.Multiline ? Height - 2 * padding : _innerTextBox.Font.Height + 2;
        }



        public override void ApplyTheme()
        {
            //base.ApplyTheme();
            _innerTextBox.BackColor = _currentTheme.TextBoxBackColor;
            _innerTextBox.ForeColor = _currentTheme.TextBoxForeColor;
            Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
            BackColor =_currentTheme.TextBoxBackColor;
            beepImage.ApplyTheme();
            Invalidate();
        }

   
        private void OnSearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) OnSearchTriggered();
        }
        public event EventHandler SearchTriggered;
        protected virtual void OnSearchTriggered() => SearchTriggered?.Invoke(this, EventArgs.Empty);

    }
}
