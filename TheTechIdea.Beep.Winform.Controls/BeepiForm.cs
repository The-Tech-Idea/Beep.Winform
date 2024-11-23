using System.ComponentModel;
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
         
            // Enable double buffering on the form
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

            InitializeForm();

            //this.Resize += (s, e) => doresize();
        }
        protected override void InitLayout()
        {
            base.InitLayout();

        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!ishandled)
            {
                InitializeForm();
                ishandled = true;
            }
          
        }
        private void InitializeForm()
        {
            if(ishandled) return;
            ishandled = true;
            beepPanel1.IsFramless = true;

            // Apply border and custom form styles
            FormBorderStyle = FormBorderStyle.None;
          //  Padding = new Padding(BorderRadius);
            Padding = new Padding(0);
            Margin = new Padding(0);

            beepPanel1.Dock = DockStyle.Top;
            beepPanel1.Height = 10;
            beepPanel1.BringToFront();
            // Enable dragging on TitlebeepLabel
            // Enable dragging on beepPanel1
            beepPanel1.MouseDown += BeepPanel1_MouseDown;
            beepPanel1.MouseMove += BeepPanel1_MouseMove;
            beepPanel1.MouseUp += BeepPanel1_MouseUp;
            beepPanel1.MouseEnter += BeepPanel1_MouseEnter;
            // align close button ,maximize and minimize button to the end of form

            // Resize settings
            MouseDown += BeepiForm_MouseDown;
            MouseMove += BeepiForm_MouseMove;
            MouseUp += BeepiForm_MouseUp;

            // Apply initial theme
            ApplyTheme();
        }

        #region Drag Support for beepPanel1

        private bool isDragging = false;
        private Point dragStartCursorPoint;
        private Point dragStartFormPoint;

        private void BeepPanel1_MouseEnter(object? sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
        }
        private void BeepPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStartCursorPoint = Cursor.Position; // Capture the cursor position
                dragStartFormPoint = Location; // Capture the form's position
            }
        }

        private void BeepPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragStartCursorPoint));
                Location = Point.Add(dragStartFormPoint, new Size(diff));
            }
        }

        private void BeepPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false; // Stop dragging
                Cursor = Cursors.Default;
            }
        }

        #endregion
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
       
            Invalidate();

        }
        #endregion
        #region Maximize Toggle

        public void ToggleMaximize()
        {
            WindowState = WindowState == FormWindowState.Maximized
                ? FormWindowState.Normal
                : FormWindowState.Maximized;
        }

        #endregion
        #region Rounded Corners
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Environment.OSVersion.Version.Major >= 6) // Windows Vista or higher
            {
                SetProcessDPIAware();
            }
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                //cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED

                //// WS_EX_LAYERED for no borders, WS_EX_COMPOSITED for smooth resize
                //cp.ExStyle |= 0x00080000; // WS_EX_LAYERED

                // WS_POPUP to avoid any system border enforcement
                cp.Style &= ~0xC00000; // Remove WS_CAPTION and WS_BORDER
                return cp;
            }
        }
    

        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if(WindowState != FormWindowState.Maximized)
            {
                Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, BorderRadius, BorderRadius));
            }
                
          
        }

        #endregion
    }
}
