using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Desktop.Common.Util;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum TabHeaderPosition { Top, Bottom, Left, Right }

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(TabControl))]
    [Category("Beep Controls")]
    [DisplayName("Beep Tabs")]
    [Description("A fully custom tab control with themed headers and SVG close buttons.")]
    public class BeepTabs : TabControl
    {

        #region "DPI Scaling Support"
        protected float DpiScaleFactor { get; private set; } = 1.0f;

        protected virtual void UpdateDpiScaling(Graphics g)
        {
            DpiScaleFactor = DpiScalingHelper.GetDpiScaleFactor(g);
        }

        protected int ScaleValue(int value)
        {
            return DpiScalingHelper.ScaleValue(value, DpiScaleFactor);
        }

        protected Size ScaleSize(Size size)
        {
            return DpiScalingHelper.ScaleSize(size, DpiScaleFactor);
        }

        protected Font ScaleFont(Font font)
        {
            return DpiScalingHelper.ScaleFont(font, DpiScaleFactor);
        }

        // Add DPI change handling
        // ✅ Update the close icon size when DPI changes
        // ✅ Update OnDpiChangedAfterParent to refresh layout
        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            base.OnDpiChangedAfterParent(e);

            using (Graphics g = CreateGraphics())
            {
                UpdateDpiScaling(g);
            }

            if (closeIcon != null)
            {
                closeIcon.Size = new Size(GetScaledCloseButtonSize(), GetScaledCloseButtonSize());
            }

            // Force layout update with new DPI scaling
            //UpdateLayoutWithDpi();
            Invalidate();
        }
        #endregion
        public event EventHandler<TabRemovedEventArgs> TabRemoved;
       
        protected IBeepTheme _currentTheme = BeepThemesManager.GetDefaultTheme();
        private string _theme;
        [Browsable(true)]
        [TypeConverter(typeof(ThemeEnumConverter))]
        public string Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                _currentTheme = BeepThemesManager.GetTheme(value);
                ApplyTheme();
            }
        }

        private int _headerHeight = 30;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The size of the custom header area. For horizontal headers, this is the height; for vertical, the width.")]
        public int HeaderHeight
        {
            get => _headerHeight;
            set
            {
                _headerHeight = Math.Max(10, value);
              //  UpdateLayoutWithDpi();
                Invalidate();
            }
        }

        private TabHeaderPosition _headerPosition = TabHeaderPosition.Top;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The position of the tab header (Top, Bottom, Left, or Right).")]
        public TabHeaderPosition HeaderPosition
        {
            get => _headerPosition;
            set
            {
                _headerPosition = value;
                UpdateLayout();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Selects a tab by its TabPage reference.")]
        public TabPage SelectTab
        {
            get => SelectedTab;
            set
            {
                if (value != null)
                {
                    int index = TabPages.IndexOf(value);
                    if (index >= 0)
                    {
                        SelectedIndex = index;
                        Invalidate();
                    }
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Selects a tab by its index.")]
        public int SelectTabByIndex
        {
            set
            {
                if (value >= 0 && value < TabCount)
                {
                    SelectedIndex = value;
                    Invalidate();
                }
            }
        }

        // Replace hardcoded constants with DPI-aware properties
        private int GetScaledCloseButtonSize() => ScaleValue(24);
        private int GetScaledCloseButtonPadding() => ScaleValue(8);
        private int GetScaledTextPadding() => ScaleValue(12);
        private int GetScaledMinTabWidth() => ScaleValue(60);
        private int GetScaledMaxTabWidth() => ScaleValue(250);
        private int GetScaledMinTabHeight() => ScaleValue(60);
        private int GetScaledMaxTabHeight() => ScaleValue(250);

        // Keep original constants for reference
        private const int CloseButtonSize = 16;
        private const int CloseButtonPadding = 8;
        private const int TextPadding = 12;
        private const int MinTabWidth = 60;
        private const int MaxTabWidth = 250;
        private const int MinTabHeight = 60;
        private const int MaxTabHeight = 250;
        private BeepImage closeIcon;
        private bool _dontresize;

        // Drag-and-drop state
        private int _draggedTabIndex = -1;
        private int _dropTargetIndex = -1;
        private float _dropMarkerX = -1;
        private float _dropMarkerY = -1;
        private bool _isMouseDown = false;
        private Point _dragStartPosition = Point.Empty;
        private bool _isDragging = false;
        private DateTime _mouseDownTime;

        public BeepTabs()
        {
            Alignment = TabAlignment.Top;
            Appearance = TabAppearance.FlatButtons;
           
            SizeMode = TabSizeMode.Fixed;
            DrawMode = TabDrawMode.OwnerDrawFixed;
            
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            // Enable drag-and-drop
            AllowDrop = true;
            // Initialize with default values - DPI scaling will be applied later
            ItemSize = new Size(0, 1);
            Padding = new Point(5, 5);

            closeIcon = new BeepImage
            {
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.close.svg",
                ScaleMode = ImageScaleMode.KeepAspectRatio,
                ApplyThemeOnImage = false,
                Size = new Size(GetScaledCloseButtonSize(), GetScaledCloseButtonSize())
            };
            // Defer DPI scaling until handle is created
            this.HandleCreated += BeepTabs_HandleCreated;

            this.DrawItem += BeepTabs_DrawItem;
            this.MouseClick += BeepTabs_MouseClick;
            this.SelectedIndexChanged += BeepTabs_SelectedIndexChanged;
            this.MouseDown += BeepTabs_MouseDown;
            this.MouseMove += BeepTabs_MouseMove;
            this.MouseUp += BeepTabs_MouseUp;
            this.DragEnter += BeepTabs_DragEnter;
            this.DragOver += BeepTabs_DragOver;
            this.DragDrop += BeepTabs_DragDrop;
            this.DragLeave += BeepTabs_DragLeave;

            this.AccessibleRole = AccessibleRole.PageTabList;
            this.AccessibleName = "Beep Tabs";
        }
        // ✅ Initialize DPI scaling when handle is created
        private void BeepTabs_HandleCreated(object sender, EventArgs e)
        {
            using (Graphics g = CreateGraphics())
            {
                UpdateDpiScaling(g);
            }

            // Now set DPI-scaled values
            closeIcon.Size = new Size(GetScaledCloseButtonSize(), GetScaledCloseButtonSize());
            UpdateLayout();
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                int scaledHeaderHeight = ScaleValue(_headerHeight);
                switch (_headerPosition)
                {
                    case TabHeaderPosition.Top:
                        return new Rectangle(0, scaledHeaderHeight, Width, ClientSize.Height - scaledHeaderHeight);
                    case TabHeaderPosition.Bottom:
                        return new Rectangle(0, 0, ClientSize.Width, ClientSize.Height - scaledHeaderHeight);
                    case TabHeaderPosition.Left:
                        return new Rectangle(scaledHeaderHeight, 0, ClientSize.Width - scaledHeaderHeight, ClientSize.Height);
                    case TabHeaderPosition.Right:
                        return new Rectangle(0, 0, ClientSize.Width - scaledHeaderHeight, ClientSize.Height);
                    default:
                        return base.DisplayRectangle;
                }
            }
        }

        public int LastTabSelected { get; private set; }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (e.Control is TabPage)
                UpdateLayout();
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            if (e.Control is TabPage)
                UpdateLayout();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            Rectangle rect = DisplayRectangle;
            foreach (TabPage page in TabPages)
            {
                page.Bounds = rect;
            }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size baseSize = base.GetPreferredSize(proposedSize);
            switch (HeaderPosition)
            {
                case TabHeaderPosition.Top:
                case TabHeaderPosition.Bottom:
                    baseSize.Height += HeaderHeight;
                    break;
                case TabHeaderPosition.Left:
                case TabHeaderPosition.Right:
                    baseSize.Width += HeaderHeight;
                    break;
            }
            return baseSize;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Let Windows/child controls paint backgrounds; only fill our header to prevent overdraw
            int scaledHeaderHeight = ScaleValue(_headerHeight);
            Rectangle headerRegion;
            switch (_headerPosition)
            {
                case TabHeaderPosition.Top:
                    headerRegion = new Rectangle(0, 0, ClientSize.Width, scaledHeaderHeight);
                    break;
                case TabHeaderPosition.Bottom:
                    headerRegion = new Rectangle(0, ClientSize.Height - scaledHeaderHeight, ClientSize.Width, scaledHeaderHeight);
                    break;
                case TabHeaderPosition.Left:
                    headerRegion = new Rectangle(0, 0, scaledHeaderHeight, ClientSize.Height);
                    break;
                case TabHeaderPosition.Right:
                    headerRegion = new Rectangle(ClientSize.Width - scaledHeaderHeight, 0, scaledHeaderHeight, ClientSize.Height);
                    break;
                default:
                    headerRegion = Rectangle.Empty;
                    break;
            }

            if (!headerRegion.IsEmpty)
            {
                using var brush = new SolidBrush(Parent?.BackColor ?? BackColor);
                e.Graphics.FillRectangle(brush, headerRegion);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Update DPI scaling first
            UpdateDpiScaling(e.Graphics);
            
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Color backgroundColor = Parent?.BackColor ?? BackColor;
            e.Graphics.Clear(backgroundColor);

            if (e.ClipRectangle.IntersectsWith(ClientRectangle))
            {
                DrawTabHeaders(e.Graphics);
            }

            // Draw drop marker if dragging
            if (_headerPosition == TabHeaderPosition.Top || _headerPosition == TabHeaderPosition.Bottom)
            {
                if (_dropMarkerX >= 0)
                {
                    using (Pen pen = new Pen(Color.Black, 2) { DashStyle = DashStyle.Solid })
                    {
                        e.Graphics.DrawLine(pen, _dropMarkerX, 0, _dropMarkerX, HeaderHeight);
                    }
                }
            }
            else if (_headerPosition == TabHeaderPosition.Left || _headerPosition == TabHeaderPosition.Right)
            {
                if (_dropMarkerX >= 0)
                {
                    using (Pen pen = new Pen(Color.Black, 2) { DashStyle = DashStyle.Solid })
                    {
                        float xPos = _headerPosition == TabHeaderPosition.Left ? 0 : ClientSize.Width - HeaderHeight;
                        e.Graphics.DrawLine(pen, xPos, _dropMarkerX, xPos + HeaderHeight, _dropMarkerX);
                    }
                }
            }
        }

        private float[] CalculateTabSizes(Graphics g, bool vertical)
        {
            // ✅ Don't call UpdateDpiScaling here since it's already called in OnPaint
            // UpdateDpiScaling(g); // Remove this line

            float[] sizes = new float[TabCount];
            using (Font font = new Font(this.Font, FontStyle.Regular))
            {
                for (int i = 0; i < TabCount; i++)
                {
                    string text = TabPages[i].Text;
                    SizeF textSize = g.MeasureString(text, font);
                    if (vertical)
                    {
                        float height = textSize.Width + (GetScaledTextPadding() * 2);
                        sizes[i] = Math.Max(GetScaledMinTabHeight(), Math.Min(GetScaledMaxTabHeight(), height));
                    }
                    else
                    {
                        float width = textSize.Width + (GetScaledTextPadding() * 2) + GetScaledCloseButtonSize() + (GetScaledCloseButtonPadding() * 2);
                        sizes[i] = Math.Max(GetScaledMinTabWidth(), Math.Min(GetScaledMaxTabWidth(), width));
                    }
                    ////MiscFunctions.SendLog($"Tab {i} size: {sizes[i]} (Text: {text})");
                }
            }
            return sizes;
        }
        private void DrawTabHeaders(Graphics g)
        {
            if (TabCount == 0)
                return;

            Color panelColor = Parent?.BackColor ?? BackColor;
            float[] tabSizes = CalculateTabSizes(g, _headerPosition == TabHeaderPosition.Left || _headerPosition == TabHeaderPosition.Right);

            // Use scaled header height consistently
            int scaledHeaderHeight = ScaleValue(_headerHeight);

            switch (_headerPosition)
            {
                case TabHeaderPosition.Top:
                    {
                        Rectangle headerRegion = new Rectangle(0, 0, Width, scaledHeaderHeight);
                        using (SolidBrush brush = new SolidBrush(panelColor))
                        {
                            g.FillRectangle(brush, headerRegion);
                        }

                        float currentX = 0;
                        for (int i = 0; i < TabCount; i++)
                        {
                            RectangleF tabRect = new RectangleF(currentX, 0, tabSizes[i], scaledHeaderHeight);
                            DrawHeaderForTab(g, tabRect, i, false);
                            currentX += tabSizes[i];
                        }
                        break;
                    }
                case TabHeaderPosition.Bottom:
                    {
                        Rectangle headerRegion = new Rectangle(0, ClientSize.Height - scaledHeaderHeight, Width, scaledHeaderHeight);
                        using (SolidBrush brush = new SolidBrush(panelColor))
                        {
                            g.FillRectangle(brush, headerRegion);
                        }

                        float currentX = 0;
                        for (int i = 0; i < TabCount; i++)
                        {
                            RectangleF tabRect = new RectangleF(currentX, ClientSize.Height - scaledHeaderHeight, tabSizes[i], scaledHeaderHeight);
                            DrawHeaderForTab(g, tabRect, i, false);
                            currentX += tabSizes[i];
                        }
                        break;
                    }
                case TabHeaderPosition.Left:
                    {
                        Rectangle headerRegion = new Rectangle(0, 0, HeaderHeight, Height);
                        using (SolidBrush brush = new SolidBrush(panelColor))
                        {
                            g.FillRectangle(brush, headerRegion);
                        }

                        float currentY = 0;
                        for (int i = 0; i < TabCount; i++)
                        {
                            RectangleF tabRect = new RectangleF(0, currentY, HeaderHeight, tabSizes[i]);
                            DrawHeaderForTab(g, tabRect, i, true);
                           ////MiscFunctions.SendLog($"Drawing Tab {i} at {tabRect}");
                            g.DrawRectangle(Pens.Cyan, Rectangle.Truncate(tabRect));
                            currentY += tabSizes[i];
                        }
                        break;
                    }
                case TabHeaderPosition.Right:
                    {
                        Rectangle headerRegion = new Rectangle(ClientSize.Width - HeaderHeight, 0, HeaderHeight, Height);
                        using (SolidBrush brush = new SolidBrush(panelColor))
                        {
                            g.FillRectangle(brush, headerRegion);
                        }

                        float currentY = 0;
                        for (int i = 0; i < TabCount; i++)
                        {
                            RectangleF tabRect = new RectangleF(ClientSize.Width - HeaderHeight, currentY, HeaderHeight, tabSizes[i]);
                            DrawHeaderForTab(g, tabRect, i, true);
                           ////MiscFunctions.SendLog($"Drawing Tab {i} at {tabRect}");
                            g.DrawRectangle(Pens.Cyan, Rectangle.Truncate(tabRect));
                            currentY += tabSizes[i];
                        }
                        break;
                    }
            }
        }

        private void DrawHeaderForTab(Graphics g, RectangleF tabRect, int index, bool vertical)
        {
            bool isSelected = (SelectedIndex == index);
            Color backgroundColor = isSelected ? _currentTheme.TabSelectedBackColor : _currentTheme.TabBackColor;
            Color textColor = isSelected ? _currentTheme.TabSelectedForeColor : _currentTheme.TabForeColor;

            g.SetClip(tabRect, CombineMode.Replace);

            using (GraphicsPath path = GetRoundedRect(tabRect, ScaleValue(4)))
            using (SolidBrush brush = new SolidBrush(backgroundColor))
            {
                g.FillPath(brush, path);
            }

            string text = TabPages[index].Text;
            using (SolidBrush textBrush = new SolidBrush(textColor))
            using (Font font = new Font(this.Font, isSelected ? FontStyle.Bold : FontStyle.Regular))
            {
                if (!vertical)
                {
                    SizeF textSize = g.MeasureString(text, font);
                    PointF textPoint = new PointF(tabRect.X + GetScaledTextPadding(), tabRect.Y + (tabRect.Height - textSize.Height) / 2);
                    g.DrawString(text, font, textBrush, textPoint);
                }
                else
                {
                    GraphicsState state = g.Save();
                    g.TranslateTransform(tabRect.X + tabRect.Width / 2, tabRect.Y + tabRect.Height / 2);
                    g.RotateTransform(90);
                    SizeF textSize = g.MeasureString(text, font);
                    PointF textPoint = new PointF(-textSize.Width / 2, -textSize.Height / 2);
                    g.DrawString(text, font, textBrush, textPoint);
                    g.Restore(state);
                }
            }

            if (!vertical)
                DrawCloseButton(g, tabRect, false);
            else
                DrawCloseButton(g, tabRect, true);

            g.ResetClip();
        }

        private void DrawCloseButton(Graphics g, RectangleF tabRect, bool vertical)
        {
            RectangleF closeRect = GetCloseButtonRect(tabRect, vertical);
            closeIcon.DrawingRect = Rectangle.Truncate(closeRect);
            closeIcon.Draw(g, Rectangle.Truncate(closeRect));
        }

        private RectangleF GetCloseButtonRect(RectangleF tabRect, bool vertical)
        {
            // ✅ Use scaled values consistently
            int scaledCloseButtonSize = GetScaledCloseButtonSize();
            int scaledCloseButtonPadding = GetScaledCloseButtonPadding();

            if (vertical)
            {
                return new RectangleF(
                    tabRect.X + (tabRect.Width - scaledCloseButtonSize) / 2,
                    tabRect.Bottom - scaledCloseButtonSize - scaledCloseButtonPadding,
                    scaledCloseButtonSize,
                    scaledCloseButtonSize
                );
            }
            return new RectangleF(
                tabRect.Right - scaledCloseButtonSize - scaledCloseButtonPadding,
                tabRect.Top + (tabRect.Height - scaledCloseButtonSize) / 2,
                scaledCloseButtonSize,
                scaledCloseButtonSize
            );
        }

        private GraphicsPath GetRoundedRect(RectangleF rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            RectangleF arc = new RectangleF(rect.Location, new SizeF(diameter, diameter));
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void BeepTabs_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Custom drawing handled in OnPaint
        }

        private void BeepTabs_DragEnter(object sender, DragEventArgs e)
        {
           ////MiscFunctions.SendLog($"DragEnter: DataPresent={e.Data.GetDataPresent(typeof(int))}");
            if (e.Data.GetDataPresent(typeof(int)))
            {
               ////MiscFunctions.SendLog("DragEnter: Drag data is valid (int)");
                e.Effect = DragDropEffects.Move;
            }
            else
            {
               ////MiscFunctions.SendLog("DragEnter: Drag data is invalid");
                e.Effect = DragDropEffects.None;
            }
        }

        private void BeepTabs_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(int)))
            {
               ////MiscFunctions.SendLog("DragOver: Invalid drag data");
                e.Effect = DragDropEffects.None;
                ResetDragState();
                return;
            }

            int draggedIndex = (int)e.Data.GetData(typeof(int));
            if (draggedIndex < 0 || draggedIndex >= TabCount)
            {
               ////MiscFunctions.SendLog($"DragOver: Invalid dragged index {draggedIndex}");
                e.Effect = DragDropEffects.None;
                ResetDragState();
                return;
            }

            e.Effect = DragDropEffects.Move;

            // Calculate drop target
            Point clientPoint = PointToClient(new Point(e.X, e.Y));
            float[] tabSizes = CalculateTabSizes(CreateGraphics(), _headerPosition == TabHeaderPosition.Left || _headerPosition == TabHeaderPosition.Right);
            _dropTargetIndex = -1;
            _dropMarkerX = -1;
            _dropMarkerY = -1;

            switch (_headerPosition)
            {
                case TabHeaderPosition.Top:
                case TabHeaderPosition.Bottom:
                    {
                        float currentX = 0;
                        int yPos = _headerPosition == TabHeaderPosition.Top ? 0 : ClientSize.Height - HeaderHeight;
                        for (int i = 0; i < TabCount; i++)
                        {
                            RectangleF tabRect = new RectangleF(currentX, yPos, tabSizes[i], HeaderHeight);
                            if (clientPoint.X >= currentX && clientPoint.X < currentX + tabSizes[i])
                            {
                                _dropTargetIndex = i;
                                if (i != draggedIndex)
                                {
                                    _dropMarkerX = clientPoint.X < currentX + tabSizes[i] / 2 ? currentX : currentX + tabSizes[i];
                                }
                                break;
                            }
                            currentX += tabSizes[i];
                        }

                        if (_dropTargetIndex == -1 && clientPoint.X >= currentX)
                        {
                            _dropTargetIndex = TabCount;
                            _dropMarkerX = currentX;
                        }
                        break;
                    }
                case TabHeaderPosition.Left:
                case TabHeaderPosition.Right:
                    {
                        float currentY = 0;
                        int xPos = _headerPosition == TabHeaderPosition.Left ? 0 : ClientSize.Width - HeaderHeight;
                        for (int i = 0; i < TabCount; i++)
                        {
                            RectangleF tabRect = new RectangleF(xPos, currentY, HeaderHeight, tabSizes[i]);
                            if (clientPoint.Y >= currentY && clientPoint.Y < currentY + tabSizes[i])
                            {
                                _dropTargetIndex = i;
                                if (i != draggedIndex)
                                {
                                    _dropMarkerX = clientPoint.Y < currentY + tabSizes[i] / 2 ? currentY : currentY + tabSizes[i];
                                }
                                break;
                            }
                            currentY += tabSizes[i];
                        }

                        if (_dropTargetIndex == -1 && clientPoint.Y >= currentY)
                        {
                            _dropTargetIndex = TabCount;
                            _dropMarkerX = currentY;
                        }
                        break;
                    }
            }

           ////MiscFunctions.SendLog($"DragOver: DropTargetIndex={_dropTargetIndex}, DropMarkerX={_dropMarkerX}, DropMarkerY={_dropMarkerY}, ClientPoint={clientPoint}");
            Invalidate();
        }

        private void BeepTabs_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(int)))
            {
               ////MiscFunctions.SendLog("DragDrop: Invalid drag data");
                ResetDragState();
                return;
            }

            int draggedIndex = (int)e.Data.GetData(typeof(int));
            if (_dropTargetIndex < 0 || draggedIndex == _dropTargetIndex || draggedIndex < 0 || draggedIndex >= TabCount)
            {
               ////MiscFunctions.SendLog($"DragDrop: Invalid drop - DropTargetIndex={_dropTargetIndex}, DraggedIndex={draggedIndex}");
                ResetDragState();
                return;
            }

            int newIndex = _dropTargetIndex;
            if (_dropTargetIndex > draggedIndex)
            {
                newIndex--;
            }

           ////MiscFunctions.SendLog($"DragDrop: Reordering - Moving tab {draggedIndex} to position {newIndex}");

            TabPage tab = TabPages[draggedIndex];
            TabPages.RemoveAt(draggedIndex);
            TabPages.Insert(newIndex, tab);

            SelectedIndex = newIndex;
            LastTabSelected = newIndex;

            ResetDragState();
        }

        private void BeepTabs_DragLeave(object sender, EventArgs e)
        {
           ////MiscFunctions.SendLog("DragLeave: Drag operation canceled or left control");
            ResetDragState();
        }

        private void ResetDragState()
        {
            _draggedTabIndex = -1;
            _dropTargetIndex = -1;
            _dropMarkerX = -1;
            _dropMarkerY = -1;
            _isMouseDown = false;
            _isDragging = false;
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
            Invalidate();
        }

        private void BeepTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        public virtual void ApplyTheme()
        {
          
            if (_currentTheme == null)
            {
               ////MiscFunctions.SendLog("Warning: _currentTheme is null, falling back to default colors.");
                BackColor = Color.LightGray;
                ForeColor = Color.Black;
                return;
            }
            BackColor = _currentTheme.TabBackColor;
            ForeColor = _currentTheme.TabForeColor;
            foreach (TabPage page in TabPages)
            {
                page.BackColor = _currentTheme.TabBackColor;
                page.ForeColor = _currentTheme.TabForeColor;
                if (page.Controls.Count > 0)
                {
                    foreach (Control ctrl in page.Controls)
                    {
                        if (ctrl is IBeepUIComponent bp)
                        {
                            bp.Theme = Theme;
                        }
                        if (ctrl is IDM_Addin)
                        {
                            foreach (var item in ctrl.Controls)
                            {
                                if (item is IBeepUIComponent bpItem)
                                {
                                    bpItem.Theme = Theme;
                                    
                                }
                            }
                        }
                    }
                }
            }
            Invalidate();
        }
        #region Mouse Events
        private void BeepTabs_MouseClick(object sender, MouseEventArgs e)
        {
           ////MiscFunctions.SendLog($"MouseClick: Click at {e.Location}, IsDragging={_isDragging}, TimeSinceMouseDown={(DateTime.Now - _mouseDownTime).TotalMilliseconds}ms");

            // Skip click processing if a drag operation is in progress or if the click was too quick (likely part of a drag attempt)
            if (_isDragging || (DateTime.Now - _mouseDownTime).TotalMilliseconds < 200)
            {
               ////MiscFunctions.SendLog("MouseClick: Skipped due to drag or quick release");
                return;
            }

            int tabCount = TabCount;
   
            if (tabCount == 0)
            {
               ////MiscFunctions.SendLog("MouseClick: No tabs present, ignoring click.");
                return;
            }


            Point clientPoint = e.Location;
            float[] tabSizes = CalculateTabSizes(CreateGraphics(), _headerPosition == TabHeaderPosition.Left || _headerPosition == TabHeaderPosition.Right);
            int scaledHeaderHeight = ScaleValue(_headerHeight);

            switch (_headerPosition)
            {
                case TabHeaderPosition.Top:
                case TabHeaderPosition.Bottom:
                    {
                        float currentX = 0;
                        int yPos = _headerPosition == TabHeaderPosition.Top ? 0 : ClientSize.Height - scaledHeaderHeight;

                        for (int i = 0; i < TabCount; i++)
                        {
                            RectangleF tabRect = new RectangleF(currentX, yPos, tabSizes[i], scaledHeaderHeight);
                            RectangleF closeRect = GetCloseButtonRect(tabRect, false);

                            if (closeRect.Contains(clientPoint))
                            {
                                string tabText = TabPages[i].Text;
                                TabPages.RemoveAt(i);
                                TabRemoved?.Invoke(this, new TabRemovedEventArgs { TabText = tabText });
                                return;
                            }
                            if (tabRect.Contains(clientPoint))
                            {
                                SelectedIndex = i;
                                LastTabSelected = i;
                                return;
                            }
                            currentX += tabSizes[i];
                        }
                        break;
                    }
                case TabHeaderPosition.Left:
                case TabHeaderPosition.Right:
                    {
                        float currentY = 0;
                        int xPos = _headerPosition == TabHeaderPosition.Left ? 0 : ClientSize.Width - HeaderHeight;
                        for (int i = 0; i < tabCount; i++)
                        {
                            RectangleF tabRect = new RectangleF(xPos, currentY, HeaderHeight, tabSizes[i]);
                            RectangleF closeRect = GetCloseButtonRect(tabRect, true);
                            string tabText = TabPages[i].Text;
                           ////MiscFunctions.SendLog($"MouseClick: Tab {i} rect: {tabRect}, Close rect: {closeRect}");
                            if (closeRect.Contains(clientPoint))
                            {
                               ////MiscFunctions.SendLog($"MouseClick: Close button clicked for tab {i}");
                                try
                                {
                                    TabPages.RemoveAt(i);
                                    TabRemoved?.Invoke(this, new TabRemovedEventArgs { TabText = tabText });
                                }
                                catch (Exception ex)
                                {
                                   ////MiscFunctions.SendLog($"MouseClick: Error removing tab: {ex.Message}");
                                }
                                return;
                            }
                            if (tabRect.Contains(clientPoint))
                            {
                                SelectedIndex = i;
                                LastTabSelected = i;
                               ////MiscFunctions.SendLog($"MouseClick: Tab {i} selected");
                                return;
                            }
                            currentY += tabSizes[i];
                        }
                        break;
                    }
            }
        }

        private void BeepTabs_MouseDown(object sender, MouseEventArgs e)
        {
           ////MiscFunctions.SendLog($"MouseDown: Button={e.Button}, Location={e.Location}, Clicks={e.Clicks}, Delta={e.Delta}");

            if (e.Button != MouseButtons.Left)
            {
               ////MiscFunctions.SendLog($"MouseDown: Ignored, not left button (Button={e.Button}) at {e.Location}");
                return;
            }

            if (TabCount == 0)
            {
               ////MiscFunctions.SendLog("MouseDown: No tabs present, ignoring.");
                return;
            }

            // Reset drag state
            _draggedTabIndex = -1;
            _isMouseDown = true;
            _isDragging = false;
            _dragStartPosition = e.Location;
            _mouseDownTime = DateTime.Now;

           ////MiscFunctions.SendLog($"MouseDown: Button down at {e.Location}, IsMouseDown={_isMouseDown}, DragStartPosition={_dragStartPosition}");

            // Capture the mouse to ensure MouseMove and MouseUp events are received
            Capture = true;

            // Detect if clicking on a tab (but not the close button)
            float[] tabSizes = CalculateTabSizes(CreateGraphics(), _headerPosition == TabHeaderPosition.Left || _headerPosition == TabHeaderPosition.Right);
            switch (_headerPosition)
            {
                case TabHeaderPosition.Top:
                case TabHeaderPosition.Bottom:
                    {
                        float currentX = 0;
                        int yPos = _headerPosition == TabHeaderPosition.Top ? 0 : ClientSize.Height - HeaderHeight;
                        for (int i = 0; i < TabCount; i++)
                        {
                            RectangleF tabRect = new RectangleF(currentX, yPos, tabSizes[i], HeaderHeight);
                            RectangleF closeRect = GetCloseButtonRect(tabRect, false);
                            if (tabRect.Contains(e.Location) && !closeRect.Contains(e.Location))
                            {
                                _draggedTabIndex = i;
                               ////MiscFunctions.SendLog($"MouseDown: Potential drag on tab {_draggedTabIndex}, TabRect={tabRect}, CloseRect={closeRect}");
                                break;
                            }
                            currentX += tabSizes[i];
                        }
                        break;
                    }
                case TabHeaderPosition.Left:
                case TabHeaderPosition.Right:
                    {
                        float currentY = 0;
                        int xPos = _headerPosition == TabHeaderPosition.Left ? 0 : ClientSize.Width - HeaderHeight;
                        for (int i = 0; i < TabCount; i++)
                        {
                            RectangleF tabRect = new RectangleF(xPos, currentY, HeaderHeight, tabSizes[i]);
                            RectangleF closeRect = GetCloseButtonRect(tabRect, true);
                            if (tabRect.Contains(e.Location) && !closeRect.Contains(e.Location))
                            {
                                _draggedTabIndex = i;
                               ////MiscFunctions.SendLog($"MouseDown: Potential drag on tab {_draggedTabIndex}, TabRect={tabRect}, CloseRect={closeRect}");
                                break;
                            }
                            currentY += tabSizes[i];
                        }
                        break;
                    }
            }
        }

        private void BeepTabs_MouseMove(object sender, MouseEventArgs e)
        {
           ////MiscFunctions.SendLog($"MouseMove: At {e.Location}, IsMouseDown={_isMouseDown}, DraggedTabIndex={_draggedTabIndex}, IsDragging={_isDragging}, Capture={Capture}");

            if (!_isMouseDown || _draggedTabIndex < 0 || _isDragging)
                return;

            // Check if the mouse has moved at least 1 pixel in any direction
            int deltaX = Math.Abs(e.Location.X - _dragStartPosition.X);
            int deltaY = Math.Abs(e.Location.Y - _dragStartPosition.Y);
            if (deltaX > 1 || deltaY > 1)
            {
               ////MiscFunctions.SendLog($"MouseMove: Starting drag for tab {_draggedTabIndex} at {e.Location}, DeltaX={deltaX}, DeltaY={deltaY}");
                _isDragging = true;

                // Initiate drag operation
                DragDropEffects result = DoDragDrop(_draggedTabIndex, DragDropEffects.Move);

                // Reset state after drag operation completes
               ////MiscFunctions.SendLog($"MouseMove: Drag operation completed with result={result}");
                _isDragging = false;
                _isMouseDown = false;
                _draggedTabIndex = -1;
                Capture = false;
                Invalidate();
            }
        }

        private void BeepTabs_MouseUp(object sender, MouseEventArgs e)
        {
           ////MiscFunctions.SendLog($"MouseUp: At {e.Location}, IsMouseDown={_isMouseDown}, IsDragging={_isDragging}");

            _isMouseDown = false;
            _isDragging = false;
            _draggedTabIndex = -1;
            Capture = false; // Release mouse capture
        }
        public void ReceiveMouseClick(Point clientLocation)
        {
           ////MiscFunctions.SendLog($"ReceiveMouseClick in BeepTabs at {clientLocation}");
            OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, clientLocation.X, clientLocation.Y, 0));
        }
        public void ReceiveMouseMove(Point clientLocation)
        {
           ////MiscFunctions.SendLog($"ReceiveMouseMove in BeepTabs at {clientLocation}");
            OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, clientLocation.X, clientLocation.Y, 0));
        }
        public void ReceiveMouseUp(Point clientLocation)
        {
           ////MiscFunctions.SendLog($"ReceiveMouseUp in BeepTabs at {clientLocation}");
            OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, clientLocation.X, clientLocation.Y, 0));
        }
        public void ReceiveMouseDown(Point clientLocation)
        {
           ////MiscFunctions.SendLog($"ReceiveMouseDown in BeepTabs at {clientLocation}");
            OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, clientLocation.X, clientLocation.Y, 0));
        }
        #endregion

        public void SuspendFormLayout()
        {
            this.SuspendLayout();
            _dontresize = true;
            if (SelectedTab == null) return;
            foreach (Control ctrl in SelectedTab.Controls)
            {
                ctrl.SuspendLayout();
                if (ctrl is IBeepUIComponent bp)
                {
                    bp.SuspendFormLayout();
                }
            }
        }

        public void ResumeFormLayout()
        {
            if (SelectedTab == null) return;
            this.ResumeLayout(true);
            _dontresize = false;
            UpdateLayout();
            foreach (Control ctrl in SelectedTab.Controls)
            {
                if (ctrl is IBeepUIComponent bp)
                {
                    bp.ResumeFormLayout();
                }
                ctrl.ResumeLayout(true);
                if (ctrl.Dock == DockStyle.Fill)
                {
                    ctrl.Size = SelectedTab.ClientSize;
                }
                else if (ctrl.Anchor != AnchorStyles.None)
                {
                    ctrl.Refresh();
                }
            }
            SelectedTab.Invalidate();
            Invalidate();
        }
        //// ✅ Add DPI-aware layout update method
        //private void UpdateLayoutWithDpi()
        //{
        //    Rectangle rect = DisplayRectangle; // Now DPI-aware
        //    foreach (TabPage page in TabPages)
        //    {
        //        page.Bounds = rect;

        //        // Ensure child controls respect the new bounds
        //        foreach (Control control in page.Controls)
        //        {
        //            if (control.Dock == DockStyle.Fill)
        //            {
        //                control.Size = rect.Size;
        //            }
        //        }
        //    }
        //}

      

    }

    public class TabRemovedEventArgs : EventArgs
    {
        public string TabText { get; set; }
    }
}