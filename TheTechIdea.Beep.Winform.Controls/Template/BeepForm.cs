using TheTechIdea.Beep.Vis.Modules;
using Svg;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Winform.Controls.Converters;

namespace TheTechIdea.Beep.Winform.Controls.Template
{
    public partial class BeepForm : frm_Addin
    {
        bool IsDialog=false;
        public event EventHandler<FormClosingEventArgs> PreClose;
        private int _borderRadius = 20;  // The radius for rounded corners, you can adjust this
                                         // Enable drag of form when using custom title bar (optional)
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public BeepForm()
        {
            InitializeComponent();
            this.FormClosing += BeepForm_FormClosing;
            // Set the form's properties for customization
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);  // Redraw on resizing

            init();
            ApplyTheme(); // Apply theme when form loads
            SetRoundedCorners(_borderRadius);
        }
        private void BeepForm_Load(object sender, EventArgs e)
        {
            ApplyTheme(); // Apply theme when form loads
            SetRoundedCorners(_borderRadius);
        }

        // Method to set rounded corners
        private void SetRoundedCorners(int borderRadius)
        {
            var path = new GraphicsPath();
            path.AddArc(new Rectangle(0, 0, borderRadius, borderRadius), 180, 90);  // Top-left corner
            path.AddArc(new Rectangle(this.Width - borderRadius, 0, borderRadius, borderRadius), 270, 90);  // Top-right corner
            path.AddArc(new Rectangle(this.Width - borderRadius, this.Height - borderRadius, borderRadius, borderRadius), 0, 90);  // Bottom-right corner
            path.AddArc(new Rectangle(0, this.Height - borderRadius, borderRadius, borderRadius), 90, 90);  // Bottom-left corner
            path.CloseAllFigures();
            this.Region = new Region(path);
        }
        // Override OnResize to handle form resizing with rounded corners
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetRoundedCorners(_borderRadius);  // Reapply rounded corners on resize
        }

        // Optional: Override OnPaint to draw custom border if needed
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Graphics g = e.Graphics)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(_theme.BorderColor, 2))
                {
                    g.DrawPath(pen, GetRoundedRectPath(this.ClientRectangle, _borderRadius));
                }
            }
        }
        // Create a GraphicsPath for rounded rectangle
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90); // Top-left corner
            path.AddArc(rect.X + rect.Width - diameter, rect.Y, diameter, diameter, 270, 90); // Top-right corner
            path.AddArc(rect.X + rect.Width - diameter, rect.Y + rect.Height - diameter, diameter, diameter, 0, 90); // Bottom-right corner
            path.AddArc(rect.X, rect.Y + rect.Height - diameter, diameter, diameter, 90, 90); // Bottom-left corner
            path.CloseFigure();
            return path;
        }

        #region "Public Properties and Methods"
        private BeepTheme _theme = BeepThemesManager.LightTheme;
        private string _themeName = "LightTheme";
        private EnumBeepThemes _themeEnum = EnumBeepThemes.LightTheme;
        [Browsable(true)]
        [TypeConverter(typeof(ThemeConverter))]
        public EnumBeepThemes Theme
        {
            get => _themeEnum;
            set
            {
                _themeEnum = value;
                // set theme and _themename
                _theme = BeepThemesManager.GetTheme(value);
                _themeName = BeepThemesManager.GetThemeName(value);
                ApplyTheme(); // Apply the selected theme when changed
            }
        }
        public void SetTitle(string title)
        {
            this.Titlelabel.Text = title;
        }
        public void SetTitle(string title, string icon)
        {
            this.Titlelabel.Text = title;
            this.Icon = new Icon(icon);
        }
        public void SetDialog(bool isDialog)
        {
            if (isDialog)
            {
                this.Closebutton.Visible = false;
                this.Maxbutton.Visible = false;
                this.Minbutton.Visible = false;
                IsDialog = true;
            }
            else
            {
              
                this.Closebutton.Visible = true;
                this.Maxbutton.Visible = true;
                this.Minbutton.Visible = true;
                IsDialog = false;
            }
        }
        public void SetLogo(Image image)
        {
            IconpictureBox.Image = image;
            IconpictureBox.Visible = true;
            IconpictureBox.SizeMode = PictureBoxSizeMode.Zoom;

        }
        public bool AddControl(UserControl control, string title,int extraheight=0,int extrawidth=0)
        {
            if (control != null)
            {
                Insidepanel.Controls.Clear();
                Insidepanel.Controls.Add(control);
                //Insidepanel.AutoSize = false; // Ensure AutoSize is off
                //Insidepanel.Dock = DockStyle.None; // Ensure Dock is not overriding size
                //Insidepanel.Anchor = AnchorStyles.Top | AnchorStyles.Left; // Ensure Anchor is not overriding size
                //Insidepanel.Top = Toppanel.Height+1;
                //Insidepanel.Left = LeftLine.Width+1;
                Title = title;
                 this.Titlelabel.Text = title;
                control.Left = 0;
                control.Top = 0;
                this.Height = control.Height+Toppanel.Height+BottomLine.Height+ extraheight+5;// + TopPanel.Height ;
                this.Width = control.Width +LeftLine.Width+RightLine.Width+extrawidth +5;
       //         Insidepanel.Width = control.Width - RightLine.Width- LeftLine.Width;
       //         Insidepanel.Height = control.Height - BottomLine.Height + extraheight-2;
               
               
                
                return true;
            }
            return false;
        }
        public string Title { get { return this.Text; } set { this.Text = value; } }
        #endregion "Public Properties and Methods"
        #region "Resource Loaders"


        #endregion
        #region "Form Variables"
        private bool _resizing;
        private bool _moving;
        private Point _lastMousePos;
        private Point _formStartPos;
        private int bordersize = 5;

    
        public IBeepService Beepservice { get; }


        private IServiceProvider serviceprovider;
     
        private bool isHomeCreated;
        private bool IsCompetitionListCreated = false;
        private bool IsMyCompetitionsCreated = false;
        private bool IsTrainingCreated = false;
        private bool IsTutorialCreated = false;
        #endregion "Form Variables"
        #region "Init Form"
        private void init()
        {
            this.Closebutton.Click += Closebutton_Click;
            this.Maxbutton.Click += Maxbutton_Click;
            this.Minbutton.Click += Minbutton_Click;
            // use topline and bottom line and leftline and rightline to do resize for form using  mouse
            // Resizing lines
            this.TopLine.MouseDown += Resize_MouseDown;
            this.BottomLine.MouseDown += Resize_MouseDown;
            this.LeftLine.MouseDown += Resize_MouseDown;
            this.RightLine.MouseDown += Resize_MouseDown;
            this.TopLine.MouseMove += Resize_MouseMove;
            this.BottomLine.MouseMove += Resize_MouseMove;
            this.LeftLine.MouseMove += Resize_MouseMove;
            this.RightLine.MouseMove += Resize_MouseMove;
            this.TopLine.MouseUp += Resize_MouseUp;
            this.BottomLine.MouseUp += Resize_MouseUp;
            this.LeftLine.MouseUp += Resize_MouseUp;
            this.RightLine.MouseUp += Resize_MouseUp;
            // Moving form using mouse on Panel1
            this.Toppanel.MouseDown += Content_MouseDown;
            this.Toppanel.MouseMove += Content_MouseMove;
            this.Toppanel.MouseUp += Content_MouseUp;

            // set the border size of the form
            this.RightLine.Width = bordersize;
            this.LeftLine.Width = bordersize;
            this.TopLine.Height = bordersize;
            this.BottomLine.Height = bordersize;
        }
        #endregion "Init Form"
        #region "Form Move"
        private void Content_MouseDown(object sender, MouseEventArgs e)
        {
            //if (IsDialog)
            //{
            //    return;
            //}
            if (e.Button == MouseButtons.Left)
            {
                _moving = true;
                _lastMousePos = e.Location;
            }
        }

        private void Content_MouseMove(object sender, MouseEventArgs e)
        {
            if (_moving)
            {
                Point currentScreenPos = PointToScreen(e.Location);
                this.Location = new Point(currentScreenPos.X - _lastMousePos.X, currentScreenPos.Y - _lastMousePos.Y);
            }
        }

        private void Content_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _moving = false;
            }
        }
        #endregion "Form Move"
        #region "Form Resize"
        private void Resize_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsDialog )
            {
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                // check if mouse is on the border of the form
                // change mouse cursor to resize cursor

                Control line = sender as Control;
                if (line == TopLine || line == BottomLine)
                {
                    this.Cursor = Cursors.SizeNS;
                }
                else if (line == LeftLine || line == RightLine)
                {
                    this.Cursor = Cursors.SizeWE;
                }

                _resizing = true;
                _lastMousePos = e.Location;

            }
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);  // Allow dragging the form
        }

        private void Resize_MouseMove(object sender, MouseEventArgs e)
        {
            if (_resizing)
            {
                Control line = sender as Control;

                int dx = e.X - _lastMousePos.X;
                int dy = e.Y - _lastMousePos.Y;

                if (line == TopLine)
                {
                    this.Height -= dy;
                    this.Top += dy;
                }
                else if (line == BottomLine)
                {
                    this.Height += dy;
                }
                else if (line == LeftLine)
                {
                    this.Width -= dx;
                    this.Left += dx;
                }
                else if (line == RightLine)
                {
                    this.Width += dx;
                }
            }
        }

        private void Resize_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _resizing = false;
                this.Cursor = Cursors.Default;
            }
        }
        #endregion "Form Resize"
        #region "Form Actions and Events"
        private void BeepForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PreClose?.Invoke(this, e);
        }

        private void Minbutton_Click(object? sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Minimized;
            }
            else if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;

            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;

            }
            this.Maxbutton.BackgroundImage = Properties.Resources.minimize;
            this.Maxbutton.BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void Maxbutton_Click(object? sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                this.Maxbutton.BackgroundImage = Properties.Resources.shrink;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                this.Maxbutton.BackgroundImage = Properties.Resources.maximize;
            }
            this.Maxbutton.BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void Closebutton_Click(object? sender, EventArgs e)
        {
            this.Close();
        }


        #endregion "Form Actions and Events"
        #region "Theme"
        private void ApplyTheme()
        {
            // Apply theme to form background
            this.BackColor = _theme.BackgroundColor;

            // Apply theme to border panels
            TopLine.BackColor = _theme.BorderColor;
            BottomLine.BackColor = _theme.BorderColor;
            LeftLine.BackColor = _theme.BorderColor;
            RightLine.BackColor = _theme.BorderColor;

            // Apply theme to title bar
            Toppanel.BackColor = _theme.TitleBarColor;
            Titlelabel.ForeColor = _theme.TitleForColor;
            Titlelabel.Font = BeepThemesManager.ToFont(_theme.TitleStyle);  // Assuming you have a method to convert TypographyStyle to TextFont

            // Apply theme to buttons (Close, Max, Min)
            ApplyButtonTheme(Closebutton, _theme.CloseButtonColor, _theme.TitleBarCloseHoverColor, _theme.TitleBarCloseHoverTextColor, _theme.TitleBarCloseActiveColor);
            ApplyButtonTheme(Maxbutton, _theme.MaxButtonColor, _theme.TitleBarMaxHoverColor, _theme.TitleBarMaxHoverTextColor, _theme.TitleBarMaxActiveColor);
            ApplyButtonTheme(Minbutton, _theme.MinButtonColor, _theme.TitleBarMinHoverColor, _theme.TitleBarMinHoverTextColor, _theme.TitleBarMinActiveColor);

            // Apply hover effects for the title bar border
            this.TopLine.MouseEnter += (s, e) => { this.TopLine.BackColor = _theme.TitleBarBorderHoverColor; };
            this.TopLine.MouseLeave += (s, e) => { this.TopLine.BackColor = _theme.TitleBarBorderColor; };

            // Set theme for icon (if applicable)
           // SetLogo(theme.IconPath);  // Assuming theme.IconPath holds the path to the theme-specific logo
        }

        // Helper method to apply theme to buttons with hover and active states
        private void ApplyButtonTheme(Button button, Color backColor, Color hoverColor, Color hoverTextColor, Color activeColor)
        {
            button.BackColor = backColor;
            button.FlatAppearance.BorderSize = 0; // Remove the default border
            button.FlatStyle = FlatStyle.Flat;

            button.MouseEnter += (s, e) =>
            {
                button.BackColor = hoverColor;
                button.ForeColor = hoverTextColor;
            };
            button.MouseLeave += (s, e) =>
            {
                button.BackColor = backColor;
                button.ForeColor = Color.White;  // You can adjust the default text color as per the theme
            };
            button.MouseDown += (s, e) =>
            {
                button.BackColor = activeColor;
            };
            button.MouseUp += (s, e) =>
            {
                button.BackColor = hoverColor;
            };
        }

        // Assuming you have a method to convert TypographyStyle to TextFont
        private Font ToFont(TypographyStyle style)
        {
            FontStyle fontStyle = FontStyle.Regular;
            if (style.FontWeight == FontWeight.Bold)
                fontStyle = FontStyle.Bold;
            else if (style.FontStyle == FontStyle.Italic)
                fontStyle = FontStyle.Italic;

            return new Font(style.FontFamily, style.FontSize, fontStyle);
        }

        public void SetLogo(string svgPath)
        {
            SvgDocument svgDoc = SvgDocument.Open(svgPath);
            IconpictureBox.Image = svgDoc.Draw(32, 32);  // Render SVG at the desired size
        }

        public void ApplyTheme(Color backgroundColor, Color borderColor)
        {
            this.BackColor = backgroundColor;
            this.BottomLine.BackColor = borderColor;
            this.RightLine.BackColor = borderColor;
            this.LeftLine.BackColor = borderColor;
            this.TopLine.BackColor = borderColor;
        }
        #endregion "Theme"

    }
}
