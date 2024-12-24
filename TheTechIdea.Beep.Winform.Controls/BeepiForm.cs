using System.ComponentModel;
using System.Runtime.InteropServices;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Converters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm : Form
    {
        private const int ResizeMargin = 5; // Margin for resizing
        private const int BorderRadius = 10;
        private const int BorderThickness = 5; // Thickness of the custom border
        private Color _borderColor = Color.Blue; // Default border color
        private const int ButtonSize = 30;
        private Point lastMousePosition;
        private bool isResizing = false;
        private bool isDragging = false;
        private Point dragStartCursorPoint;
        private Point dragStartFormPoint;
        private Point resizeStartCursorPoint;
        private Size resizeStartFormSize;
        private bool ishandled = false;


        protected EnumBeepThemes _themeEnum = EnumBeepThemes.DefaultTheme;
        protected BeepTheme _currentTheme = BeepThemesManager.DefaultTheme;

        [Browsable(true)]
        [TypeConverter(typeof(ThemeConverter))]
        public EnumBeepThemes Theme
        {
            get => _themeEnum;
            set
            {
                _themeEnum = value;
                _currentTheme = BeepThemesManager.GetTheme(value);
               
                ApplyTheme();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Sets the color of the form's border.")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate(); // Redraw the form when the color changes
            }
        }
        public BeepiForm()
        {
            InitializeComponent();
            ishandled = false;

            // Enable double buffering on the form
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            this.Padding = new Padding(0);

            InitializeForm();
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
            if (ishandled) return;
            ishandled = true;

            // Apply border and custom form styles
            FormBorderStyle = FormBorderStyle.None;
            Padding = new Padding(5);
            Margin = new Padding(5);

            //beepPanel1.IsFramless = true;
            //beepPanel1.Dock = DockStyle.Top;
            //beepPanel1.Height = 10;
            //beepPanel1.BringToFront();

            //// Enable dragging on TitlebeepLabel
            //beepPanel1.MouseDown += BeepPanel1_MouseDown;
            //beepPanel1.MouseMove += BeepPanel1_MouseMove;
            //beepPanel1.MouseUp += BeepPanel1_MouseUp;
            //beepPanel1.MouseEnter += BeepPanel1_MouseEnter;

            //// Resize settings
            //MouseDown += BeepiForm_MouseDown;
            //MouseMove += BeepiForm_MouseMove;
            //MouseUp += BeepiForm_MouseUp;

            // Apply initial theme
            ApplyTheme();
        }

        #region Drag Support for beepPanel1

       

        //private void BeepPanel1_MouseEnter(object? sender, EventArgs e)
        //{
        //    Cursor = Cursors.Hand;
        //}

        //private void BeepPanel1_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        isDragging = true;
        //        dragStartCursorPoint = Cursor.Position;
        //        dragStartFormPoint = Location;
        //    }
        //}

        //private void BeepPanel1_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (isDragging)
        //    {
        //        Point diff = Point.Subtract(Cursor.Position, new Size(dragStartCursorPoint));
        //        Location = Point.Add(dragStartFormPoint, new Size(diff));
        //    }
        //}

        //private void BeepPanel1_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        isDragging = false;
        //        Cursor = Cursors.Default;
        //    }
        //}

        #endregion

        #region Window Resizing
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                // Determine if we're resizing or dragging
                if (IsNearEdge(e.Location))
                {
                    isResizing = true;
                    resizeStartCursorPoint = Cursor.Position;
                    resizeStartFormSize = Size;
                }
                else
                {
                    isDragging = true;
                    dragStartCursorPoint = Cursor.Position;
                    dragStartFormPoint = Location;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (isResizing)
            {
                HandleResizing();
            }
            else if (isDragging)
            {
                HandleDragging();
            }
            else
            {
                // Change cursor appearance based on mouse position
                if (IsNearEdge(e.Location))
                {
                    Cursor = Cursors.SizeNWSE; // Resize cursor
                }
                else
                {
                    Cursor = Cursors.Default; // Default cursor
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                isResizing = false;
                isDragging = false;
            }
        }

        private bool IsNearEdge(Point location)
        {
            return location.X >= Width - ResizeMargin || location.Y >= Height - ResizeMargin;
        }

        private void HandleResizing()
        {
            Point diff = Point.Subtract(Cursor.Position, new Size(resizeStartCursorPoint));
            Size newSize = new Size(
                Math.Max(MinimumSize.Width, resizeStartFormSize.Width + diff.X),
                Math.Max(MinimumSize.Height, resizeStartFormSize.Height + diff.Y)
            );

            Size = newSize;
        }
        private void HandleDragging()
        {
            Point diff = Point.Subtract(Cursor.Position, new Size(dragStartCursorPoint));
            Location = Point.Add(dragStartFormPoint, new Size(diff));
        }
        #endregion
        #region Theme Application
        public virtual void ApplyTheme()
        {
            BeepTheme theme = BeepThemesManager.GetTheme(beepuiManager1.Theme);
            BackColor = theme.BackColor;
            //beepPanel1.Theme = beepuiManager1.Theme;
            BorderColor = theme.BorderColor;
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
        #region Rounded Corners and DPI Awareness

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw the custom border
            // Draw the custom border
            using (Pen borderPen = new Pen(_borderColor, BorderThickness))
            {
                // Account for the border thickness to prevent overlap
                Rectangle borderRectangle = new Rectangle(
                    BorderThickness / 2,
                    BorderThickness / 2,
                    Width - BorderThickness,
                    Height - BorderThickness
                );
                e.Graphics.DrawRectangle(borderPen, borderRectangle);
            }
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~0xC00000; // Remove WS_CAPTION and WS_BORDER
                return cp;
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (WindowState != FormWindowState.Maximized)
            {
                Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, BorderRadius, BorderRadius));
            }
        }
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

       

        #endregion
    }
}
