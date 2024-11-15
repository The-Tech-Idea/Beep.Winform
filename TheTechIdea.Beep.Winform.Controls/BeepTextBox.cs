using TheTechIdea.Beep.Vis.Modules;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
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
                if (beepImage != null)
                {
                    beepImage.ImagePath = value;
                    beepImage.ApplyThemeToSvg();
                    beepImage.ApplyTheme();
                    Invalidate();
                }
            }
        }
        public BeepTextBox()
        {
            InitializeComponents();
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
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
            int padding = BorderThickness + 10;
            Size imageSize = beepImage.HasImage ? beepImage.GetImageSize() : Size.Empty;
            _innerTextBox.Multiline =true   ;
            if(string.IsNullOrEmpty(ImagePath))
            {
                _innerTextBox.Location = new Point(padding, padding);
                _innerTextBox.Width = Width - padding * 2;
            }
            else
            if (TextImageRelation == TextImageRelation.ImageBeforeText)
            {
                // Place the image on the left, with text to the right
                _innerTextBox.Location = new Point(imageSize.Width + padding * 2, padding);
                _innerTextBox.Width = Width - imageSize.Width - padding *3;
            }
            else if (TextImageRelation == TextImageRelation.TextBeforeImage)
            {
                // Place the text on the left, with image on the right
                _innerTextBox.Location = new Point(padding-10, padding);
                _innerTextBox.Width = Width - imageSize.Width - padding * 3;
            }
            else
            {
                // No specific relation; occupy full width
                _innerTextBox.Location = new Point(padding-15, padding);
                _innerTextBox.Width = Width - padding * 2;
            }

            // Set the height of the inner TextBox
            _innerTextBox.Height = Height - padding * 2;

            // Adjust beepImage position according to ImageAlign setting
            if (beepImage.HasImage)
            {
                beepImage.Size = imageSize;
                switch (ImageAlign)
                {
                    case ContentAlignment.TopLeft:
                        beepImage.Location = new Point(padding, padding);
                        break;
                    case ContentAlignment.TopRight:
                        beepImage.Location = new Point(Width - imageSize.Width - padding, padding);
                        break;
                    case ContentAlignment.BottomLeft:
                        beepImage.Location = new Point(padding, Height - imageSize.Height - padding);
                        break;
                    case ContentAlignment.BottomRight:
                        beepImage.Location = new Point(Width - imageSize.Width - padding, Height - imageSize.Height - padding);
                        break;
                    case ContentAlignment.MiddleLeft:
                        beepImage.Location = new Point(padding, (Height - imageSize.Height) / 2);
                        break;
                    case ContentAlignment.MiddleRight:
                        beepImage.Location = new Point(Width - imageSize.Width - padding, (Height - imageSize.Height) / 2);
                        break;
                    case ContentAlignment.TopCenter:
                        beepImage.Location = new Point((Width - imageSize.Width) / 2, padding);
                        break;
                    case ContentAlignment.BottomCenter:
                        beepImage.Location = new Point((Width - imageSize.Width) / 2, Height - imageSize.Height - padding);
                        break;
                    case ContentAlignment.MiddleCenter:
                        beepImage.Location = new Point((Width - imageSize.Width) / 2, (Height - imageSize.Height) / 2);
                        break;
                }
            }
        }


        public override void ApplyTheme()
        {
            //base.ApplyTheme();
            _innerTextBox.BackColor = _currentTheme.TextBoxBackColor;
            _innerTextBox.ForeColor = _currentTheme.TextBoxForeColor;
            BackColor=_currentTheme.TextBoxBackColor;
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
