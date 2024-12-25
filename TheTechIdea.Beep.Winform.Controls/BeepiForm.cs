using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Converters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm : Form
    {
        protected int _resizeMargin = 5; // Margin for resizing
        protected int _borderRadius =5;
        protected int _borderThickness = 3; // Thickness of the custom border
        private Color _borderColor = Color.Red; // Default border color
        private const int ButtonSize = 30;
        private Point lastMousePosition;
        private bool isResizing = false;
        private bool isDragging = false;
        private Point dragStartCursorPoint;
        private Point dragStartFormPoint;
        private Point resizeStartCursorPoint;
        private Size resizeStartFormSize;
        private bool ishandled = false;
        private bool _inpopupmode = false;


        protected EnumBeepThemes _themeEnum = EnumBeepThemes.DefaultTheme;
        protected BeepTheme _currentTheme = BeepThemesManager.DefaultTheme;

        [Browsable(true)]
        [Category("Appearance")]
        public bool InPopMode
        {
            get => _inpopupmode;
            set
            {
                _inpopupmode = value;
                Invalidate(); // Redraw the form when the color changes
            }
        }
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
            // Apply border and custom form styles
            FormBorderStyle = FormBorderStyle.None;
            Padding = new Padding(_borderThickness); // Adjust padding based on _borderThickness
            Margin = new Padding(_resizeMargin);
            InitializeForm();
        }
        protected override void OnControlAdded(ControlEventArgs e)
        {
           base.OnControlAdded(e);
            Console.WriteLine($"1 Control Added {e.Control.Text}");
            AdjustControls();
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
            this.Padding = new Padding(_borderThickness); // Adjust padding based on _borderThickness

          //  Margin = new Padding(5);
            // Apply initial theme
            ApplyTheme();
        }

      
        #region Window Resizing
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if(_inpopupmode) return;
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
            if (_inpopupmode) return;
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
            if (_inpopupmode) return;
            if (e.Button == MouseButtons.Left)
            {
                isResizing = false;
                isDragging = false;
            }
        }

        private bool IsNearEdge(Point location)
        {
            return location.X >= Width - _resizeMargin || location.Y >= Height - _resizeMargin;
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
            // Enable anti-aliasing for smoother borders
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            // Draw the custom border
            // Skip drawing if border thickness is zero
            if (_borderThickness == 0)
            {
                return;
            }
            using (Pen borderPen = new Pen(_borderColor, _borderThickness))
            {
                // Account for the border thickness to prevent overlap
                Rectangle borderRectangle = new Rectangle(
                      _borderThickness / 2,
                      _borderThickness / 2,
                    Width - (_borderThickness ),
                    Height - (_borderThickness )
                );

            
                borderPen.Alignment = PenAlignment.Center;
                e.Graphics.DrawRectangle(borderPen, borderRectangle);
            }

          
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Update the rounded corners region
            if (Region != null)
            {
                Region.Dispose();
            }

            // Define the rounded region for the form
            Region = Region.FromHrgn(CreateRoundRectRgn(
                0, // No adjustment for border thickness
                0, // No adjustment for border thickness
                Width,
                Height,
                _borderRadius * 2,
                _borderRadius * 2
            ));

        }
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
       //     AdjustControls();
        }
        public void AdjustControls()
        {
            Rectangle adjustedClientArea = GetAdjustedClientRectangle();

            foreach (Control control in Controls)
            {
                if (control.Dock == DockStyle.Fill)
                {
                    control.Bounds = adjustedClientArea;
                }
                else if (control.Dock == DockStyle.Top)
                {
                    control.Bounds = new Rectangle(
                        adjustedClientArea.Left,
                        adjustedClientArea.Top,
                        adjustedClientArea.Width,
                        control.Height
                    );
                    adjustedClientArea.Y += control.Height;
                    adjustedClientArea.Height -= control.Height;
                }
                else if (control.Dock == DockStyle.Bottom)
                {
                    control.Bounds = new Rectangle(
                        adjustedClientArea.Left,
                        adjustedClientArea.Bottom - control.Height,
                        adjustedClientArea.Width,
                        control.Height
                    );
                    adjustedClientArea.Height -= control.Height;
                }
                else if (control.Dock == DockStyle.Left)
                {
                    control.Bounds = new Rectangle(
                        adjustedClientArea.Left,
                        adjustedClientArea.Top,
                        control.Width,
                        adjustedClientArea.Height
                    );
                    adjustedClientArea.X += control.Width;
                    adjustedClientArea.Width -= control.Width;
                }
                else if (control.Dock == DockStyle.Right)
                {
                    control.Bounds = new Rectangle(
                        adjustedClientArea.Right - control.Width,
                        adjustedClientArea.Top,
                        control.Width,
                        adjustedClientArea.Height
                    );
                    adjustedClientArea.Width -= control.Width;
                }
                else if (control.Dock == DockStyle.None)
                {
                    Console.WriteLine($"Control is not docked {control.Left}-{adjustedClientArea.Left}");
                    // Non-docked controls are clamped within the adjusted client area
                    control.Left = Math.Max(control.Left, adjustedClientArea.Left+1);
                    control.Top = Math.Max(control.Top, adjustedClientArea.Top+1);
                    control.Width = Math.Min(control.Width, adjustedClientArea.Width - control.Left + adjustedClientArea.Left);
                    control.Height = Math.Min(control.Height, adjustedClientArea.Height - control.Top + adjustedClientArea.Top);
                }
            }
        }
        public Rectangle GetAdjustedClientRectangle()
        {
            return new Rectangle(
                _borderThickness ,
                _borderThickness ,
                ClientSize.Width - (2 * _borderThickness),
                ClientSize.Height - (2 * _borderThickness )
            );
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
        protected new  Rectangle DisplayRectangle
        {
            get
            {
                // Adjust for the custom borders
                int adjustedWidth = ClientSize.Width - (_borderThickness * 2);
                int adjustedHeight = ClientSize.Height - (_borderThickness * 2);

                // Ensure the dockable area respects the borders
                return new Rectangle(
                    _borderThickness,
                    _borderThickness,
                    Math.Max(0, adjustedWidth),
                    Math.Max(0, adjustedHeight)
                );
            }
        }
        public override Size GetPreferredSize(Size proposedSize)
        {
            // Adjust size based on border thickness
            int adjustedWidth = proposedSize.Width - (_borderThickness * 2);
            int adjustedHeight = proposedSize.Height - (_borderThickness * 2);

            return new Size(adjustedWidth, adjustedHeight);
        }
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

       

        #endregion
    }
}
