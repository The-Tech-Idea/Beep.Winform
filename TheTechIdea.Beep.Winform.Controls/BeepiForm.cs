using System.Runtime.InteropServices;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm : Form
    {
        private const int BorderRadius = 10;
        private const int ButtonSize = 30;
        private Point lastMousePosition;
        private bool isResizing = false;
        private bool ishandled = false;
        public BeepiForm()
        {
            InitializeComponent();
            ishandled = false   ;
            beepPanel1.IsFramless = true;
            TitleLabel.IsFramless = true;
           // this.Resize += (s, e) => doresize();
        }
        public void ShowTitle(bool show)
        {
         
            TitleLabel.Visible = show;
        }

        //private void doresize()
        //{
          
        //        CloseButton.Left = beepPanel1.Left + beepPanel1.Width - BorderRadius - CloseButton.Width - 20;
        //        MaximizeButton.Left = CloseButton.Left - BorderRadius - MaximizeButton.Width;
        //        MinimizeButton.Left = MaximizeButton.Left - BorderRadius - MinimizeButton.Width;
        //        TitleLabel.Width = MinimizeButton.Left- TitleLabel.Left -  20;
        //}

        protected override void OnHandleCreated(EventArgs e)
        {
           
            if (!ishandled)
            {
                InitializeForm();
                ishandled = true;
            }
            base.OnHandleCreated(e);
        }
        private void InitializeForm()
        {
            // Enable double buffering on the form
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

            // Apply border and custom form styles
            FormBorderStyle = FormBorderStyle.None;
            Padding = new Padding(BorderRadius);

            // Button events
            CloseButton.Click += (s, e) => Close();
          //  ClosebeepButton.LogoImage = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.close.svg";

            MaximizeButton.Click += (s, e) => ToggleMaximize();
          //  MaximizebeepButton.LogoImage = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.maximize.svg";
            MinimizeButton.Click += (s, e) => WindowState = FormWindowState.Minimized;
            //MinimizebeepButton.LogoImage = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.minimize.svg";

            // Enable dragging on TitlebeepLabel
            beepPanel1.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) BeginDrag(e.Location); };
            beepPanel1.MouseMove += (s, e) => { if (e.Button == MouseButtons.Left) PerformDrag(e.Location); };
            beepPanel1.MouseUp += (s, e) => EndDrag();
            // align close button ,maximize and minimize button to the end of form
          

            //CloseButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.close.svg";
            //MaximizeButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.maximize.svg";
            //MinimizeButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.minimize.svg";
            CloseButton.ToolTipText = "Close";
            MaximizeButton.ToolTipText = "Maximize";
            MinimizeButton.ToolTipText = "Minimize";


            CloseButton.ApplyThemeOnImage = true;
            MaximizeButton.ApplyThemeOnImage = true;
            MinimizeButton.ApplyThemeOnImage = true;
            TitleLabel.ApplyThemeOnImage = true;

            CloseButton.ImageAlign = ContentAlignment.MiddleCenter;
            CloseButton.TextAlign = ContentAlignment.MiddleCenter;
            CloseButton.TextImageRelation = TextImageRelation.ImageAboveText;
          //  CloseButton.Text = "";
            CloseButton.MaxImageSize = CloseButton.Size;
            MaximizeButton.ImageAlign = ContentAlignment.MiddleCenter;
            MaximizeButton.TextAlign = ContentAlignment.MiddleCenter;
            MaximizeButton.TextImageRelation = TextImageRelation.ImageAboveText;
           // MaximizeButton.Text = "";
            MaximizeButton.MaxImageSize = CloseButton.Size;
            MinimizeButton.ImageAlign = ContentAlignment.MiddleCenter;
            MinimizeButton.TextAlign = ContentAlignment.MiddleCenter;
            MinimizeButton.TextImageRelation = TextImageRelation.ImageAboveText;
          //  MinimizeButton.Text = "";
            MinimizeButton.MaxImageSize = CloseButton.Size;
            // Resize settings
            MouseDown += BeepiForm_MouseDown;
            MouseMove += BeepiForm_MouseMove;
            MouseUp += BeepiForm_MouseUp;

            // Apply initial theme
            ApplyTheme();
        }
        public void InitializeForm(Form form)
        {
           form=this;
            InitializeForm();
            // Enable double buffering on the form
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

            // Apply border and custom form styles
            FormBorderStyle = FormBorderStyle.None;
            Padding = new Padding(BorderRadius);

            // Button events
            CloseButton.Click += (s, e) => Close();
            MaximizeButton.Click += (s, e) => ToggleMaximize();
            MinimizeButton.Click += (s, e) => WindowState = FormWindowState.Minimized;
            //CloseButton.Left = beepPanel1.Left +beepPanel1.Width- BorderRadius - CloseButton.Width-20;
            //MaximizeButton.Left = CloseButton.Left - BorderRadius - MaximizeButton.Width;
            //MinimizeButton.Left = MaximizeButton.Left - BorderRadius - MinimizeButton.Width;

            CloseButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.close.svg";
            MaximizeButton.ImagePath= "TheTechIdea.Beep.Winform.Controls.GFX.SVG.maximize.svg";
            MaximizeButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.minimize.svg";
            CloseButton.ToolTipText = "Close";
            MaximizeButton.ToolTipText = "Maximize";
            MinimizeButton.ToolTipText = "Minimize";
            CloseButton.ImageAlign= ContentAlignment.MiddleCenter;
            CloseButton.TextAlign= ContentAlignment.MiddleCenter;
            CloseButton.TextImageRelation = TextImageRelation.ImageAboveText;
//CloseButton.Text = "";
            CloseButton.MaxImageSize = CloseButton.Size;
            MaximizeButton.ImageAlign = ContentAlignment.MiddleCenter;
            MaximizeButton.TextAlign = ContentAlignment.MiddleCenter;
            MaximizeButton.TextImageRelation = TextImageRelation.ImageAboveText;
          //  MaximizeButton.Text = "";
            MaximizeButton.MaxImageSize = CloseButton.Size;
            MinimizeButton.ImageAlign = ContentAlignment.MiddleCenter;
            MinimizeButton.TextAlign = ContentAlignment.MiddleCenter;
            MinimizeButton.TextImageRelation = TextImageRelation.ImageAboveText;
          //  MinimizeButton.Text = "";
            MinimizeButton.MaxImageSize = CloseButton.Size;
            // Enable dragging on TitlebeepLabel
            beepPanel1.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) BeginDrag(e.Location); };
            beepPanel1.MouseMove += (s, e) => { if (e.Button == MouseButtons.Left) PerformDrag(e.Location); };
            beepPanel1.MouseUp += (s, e) => EndDrag();

            // Resize settings
            MouseDown += BeepiForm_MouseDown;
            MouseMove += BeepiForm_MouseMove;
            MouseUp += BeepiForm_MouseUp;

            // Apply initial theme
            ApplyTheme();
        }
        #region Window Resizing

        private void BeepiForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (e.Location.X >= Width - BorderRadius || e.Location.Y >= Height - BorderRadius))
            {
                isResizing = true;
                lastMousePosition = e.Location;
            }
        }

        private void BeepiForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing)
            {
                SuspendLayout(); // Suspend layout to minimize flicker
                int dx = e.X - lastMousePosition.X;
                int dy = e.Y - lastMousePosition.Y;

                if (Width + dx >= MinimumSize.Width) Width += dx;
                if (Height + dy >= MinimumSize.Height) Height += dy;

                lastMousePosition = e.Location;
                //CloseButton.Left = beepPanel1.Left + beepPanel1.Width - BorderRadius - CloseButton.Width - 20;
                //MaximizeButton.Left = CloseButton.Left - BorderRadius - MaximizeButton.Width;
                //MinimizeButton.Left = MaximizeButton.Left - BorderRadius - MinimizeButton.Width;
                //TitleLabel.Width = beepPanel1.Width - beepPanel1.Left - 20;
                ResumeLayout(true); // Resume layout
            }
            else
            {
                // Change cursor near the edges
                if (e.X >= Width - BorderRadius || e.Y >= Height - BorderRadius)
                    Cursor = Cursors.SizeNWSE;
                else
                    Cursor = Cursors.Default;
            }
        }

        private void BeepiForm_MouseUp(object sender, MouseEventArgs e)
        {
            isResizing = false;
        }

        #endregion
        #region Drag Support for TitlebeepLabel

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        private void BeginDrag(Point cursorPosition)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = Location;
        }

        private void PerformDrag(Point cursorPosition)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                Location = Point.Add(dragFormPoint, new Size(diff));
            }
        }

        private void EndDrag()
        {
            dragging = false;
        }

        #endregion
        #region Theme Application
        public void ApplyTheme()
        {    
            // Customize theme based on BeepUIManager or global settings
            BeepTheme theme = BeepThemesManager.GetTheme(beepuiManager1.Theme);
            BackColor = theme.BackColor;
            //beepPanel1.IsChild = true;
            beepPanel1.Theme= beepuiManager1.Theme;
        //    CloseButton.IsChild = true;
            CloseButton.Theme = beepuiManager1.Theme;
        //    MaximizeButton.IsChild = true;
            MaximizeButton.Theme = beepuiManager1.Theme;
           // MinimizeButton.IsChild = true;
            MinimizeButton.Theme = beepuiManager1.Theme;
          //  MaximizeButton.IsChild = true;
            TitleLabel.Theme = beepuiManager1.Theme;
            Invalidate();

        }
        #endregion
        #region Maximize Toggle

        private void ToggleMaximize()
        {
            WindowState = WindowState == FormWindowState.Maximized
                ? FormWindowState.Normal
                : FormWindowState.Maximized;
        }

        #endregion
        #region Rounded Corners

        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (WindowState != FormWindowState.Maximized)
            {
                Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, BorderRadius, BorderRadius));
            }
            else
            {
                Region = null;
            }
        }

        #endregion
    }
}
