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


        // show the inner textbox properties like multiline
        [Browsable(true)]
        [Category("Appearance")]
        public bool Multiline
        {
            get => _innerTextBox.Multiline;
            set
            {
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

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Specify the image file to load (SVG, PNG, JPG, etc.).")]
        public string ImagePath
        {
            get => beepImage?.ImagePath;
            set
            {
                //if (beepImage != null)
                //{
                //    beepImage.ImagePath = value;
                //    beepImage.ApplyThemeToSvg();
                //    beepImage.ApplyTheme();
                //    Invalidate();
                //}
            }
        }
        public BeepTextBox()
        {
            InitializeComponents();
            Size = new Size(150, 30);
            _innerTextBox.TextChanged += (s, e) => Invalidate(); // Repaint to apply formatting
        }

        private void InitializeComponents()
        {
            _innerTextBox = new TextBox
            {
                BorderStyle = System.Windows.Forms.BorderStyle.None,
                Multiline = true,
            };
            _innerTextBox.TextChanged += InnerTextBox_TextChanged;
            _innerTextBox.KeyPress += InnerTextBox_KeyPress;
            _innerTextBox.KeyDown += OnSearchKeyDown;
            _innerTextBox.MouseEnter += OnMouseEnter;
            _innerTextBox.MouseLeave += OnMouseLeave;
            Controls.Add(_innerTextBox);

            beepImage = new BeepImage { Size = _maxImageSize, Dock = DockStyle.None, Margin = new Padding(0) };
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
            PositionInnerTextBoxAndImage();
        }
        private void PositionInnerTextBoxAndImage()
        {
            int padding = BorderThickness ;
            Size imageSize = beepImage.HasImage ? beepImage.GetImageSize() : Size.Empty;
            _innerTextBox.Multiline = true;

            // Limit image size to MaxImageSize
            if (imageSize.Width > _maxImageSize.Width || imageSize.Height > _maxImageSize.Height)
            {
                float scaleFactor = Math.Min(
                    (float)_maxImageSize.Width / imageSize.Width,
                    (float)_maxImageSize.Height / imageSize.Height);
                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }
            // Determine the width and height of the inner text box based on DrawingRect
            int innerTextBoxWidth = DrawingRect.Width - padding * 2;
            int innerTextBoxHeight = DrawingRect.Height - padding * 2;

            if (string.IsNullOrEmpty(_imagepath))
            {
                _innerTextBox.Location = new Point(DrawingRect.Left + padding, DrawingRect.Top + padding);
                _innerTextBox.Width = innerTextBoxWidth;
            }
            else if (TextImageRelation == TextImageRelation.ImageBeforeText)
            {
                // Place the image on the left, with text to the right
                _innerTextBox.Location = new Point(DrawingRect.Left + imageSize.Width + padding * 2, DrawingRect.Top + padding);
                _innerTextBox.Width = DrawingRect.Width - imageSize.Width - padding * 3;
            }
            else if (TextImageRelation == TextImageRelation.TextBeforeImage)
            {
                // Place the text on the left, with image on the right
                _innerTextBox.Location = new Point(DrawingRect.Left + padding, DrawingRect.Top + padding);
                _innerTextBox.Width = DrawingRect.Width - imageSize.Width - padding * 3;
            }
            else
            {
                // No specific relation; occupy full width within DrawingRect
                _innerTextBox.Location = new Point(DrawingRect.Left + padding, DrawingRect.Top + padding);
                _innerTextBox.Width = innerTextBoxWidth;
            }

            // Set the height of the inner TextBox to fit within DrawingRect
            _innerTextBox.Height = innerTextBoxHeight;

            // Adjust beepImage position according to ImageAlign setting and DrawingRect
            if (beepImage.HasImage)
            {
                beepImage.Size = imageSize;
                switch (ImageAlign)
                {
                    case ContentAlignment.TopLeft:
                        beepImage.Location = new Point(DrawingRect.Left + padding, DrawingRect.Top + padding);
                        break;
                    case ContentAlignment.TopRight:
                        beepImage.Location = new Point(DrawingRect.Right - imageSize.Width - padding, DrawingRect.Top + padding);
                        break;
                    case ContentAlignment.BottomLeft:
                        beepImage.Location = new Point(DrawingRect.Left + padding, DrawingRect.Bottom - imageSize.Height - padding);
                        break;
                    case ContentAlignment.BottomRight:
                        beepImage.Location = new Point(DrawingRect.Right - imageSize.Width - padding, DrawingRect.Bottom - imageSize.Height - padding);
                        break;
                    case ContentAlignment.MiddleLeft:
                        beepImage.Location = new Point(DrawingRect.Left + padding, DrawingRect.Top + (DrawingRect.Height - imageSize.Height) / 2);
                        break;
                    case ContentAlignment.MiddleRight:
                        beepImage.Location = new Point(DrawingRect.Right - imageSize.Width - padding, DrawingRect.Top + (DrawingRect.Height - imageSize.Height) / 2);
                        break;
                    case ContentAlignment.TopCenter:
                        beepImage.Location = new Point(DrawingRect.Left + (DrawingRect.Width - imageSize.Width) / 2, DrawingRect.Top + padding);
                        break;
                    case ContentAlignment.BottomCenter:
                        beepImage.Location = new Point(DrawingRect.Left + (DrawingRect.Width - imageSize.Width) / 2, DrawingRect.Bottom - imageSize.Height - padding);
                        break;
                    case ContentAlignment.MiddleCenter:
                        beepImage.Location = new Point(DrawingRect.Left + (DrawingRect.Width - imageSize.Width) / 2, DrawingRect.Top + (DrawingRect.Height - imageSize.Height) / 2);
                        break;
                }
            }
        }


        public override void ApplyTheme()
        {
            //base.ApplyTheme();
            _innerTextBox.BackColor = _currentTheme.TextBoxBackColor;
            _innerTextBox.ForeColor = _currentTheme.TextBoxForeColor;
            BackColor=_currentTheme.BackgroundColor;
            beepImage.ApplyTheme();
            Invalidate();
        }

        public override void ApplyTheme(EnumBeepThemes theme)
        {
            Theme = theme;
            ApplyTheme();
        }
        private void OnSearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) OnSearchTriggered();
        }
        public event EventHandler SearchTriggered;
        protected virtual void OnSearchTriggered() => SearchTriggered?.Invoke(this, EventArgs.Empty);

    }
}
