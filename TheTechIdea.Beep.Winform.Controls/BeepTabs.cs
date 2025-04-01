using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Converters;

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
        // Add to the BeepTabs class
        public event EventHandler<TabRemovedEventArgs> TabRemoved;
        protected EnumBeepThemes _themeEnum = EnumBeepThemes.DefaultTheme;
        protected BeepTheme _currentTheme = BeepThemesManager.DefaultTheme;
        [Browsable(true)]
        [TypeConverter(typeof(ThemeEnumConverter))]
        public EnumBeepThemes Theme
        {
            get => _themeEnum;
            set
            {
                _themeEnum = value;
                _currentTheme = BeepThemesManager.GetTheme(value);
                //      this.ApplyTheme();
                ApplyTheme();
            }
        }

     

        private int _headerHeight = 60;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The size of the custom header area. For horizontal headers, this is the height; for vertical, the width.")]
        public int HeaderHeight
        {
            get => _headerHeight;
            set { _headerHeight = value; UpdateLayout(); Invalidate(); }
        }

        private TabHeaderPosition _headerPosition = TabHeaderPosition.Top;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The position of the tab header (Top, Bottom, Left, or Right).")]
        public TabHeaderPosition HeaderPosition
        {
            get => _headerPosition;
            set { _headerPosition = value; UpdateLayout(); Invalidate(); }
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
                        UpdateLayout();
                        Invalidate(); // Redraw to reflect the selection
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
                    UpdateLayout();
                    Invalidate(); // Redraw to reflect the selection
                }
            }
        }

        private const int CloseButtonSize = 16;
        private const int CloseButtonPadding = 6;
        private BeepImage closeIcon;

        public BeepTabs()
        {
            Alignment = TabAlignment.Top; // Force built-in header to Top (hidden)
            Appearance = TabAppearance.FlatButtons;
            ItemSize = new Size(0, 1); // Hide default headers
            SizeMode = TabSizeMode.Fixed;
            DrawMode = TabDrawMode.OwnerDrawFixed;
            Padding = new Point(0, 0); // Remove default padding to avoid gaps
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            closeIcon = new BeepImage
            {
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.close.svg",
                ScaleMode = ImageScaleMode.KeepAspectRatio,
                ApplyThemeOnImage = false,
                Size = new Size(CloseButtonSize, CloseButtonSize)
            };

            this.DrawItem += BeepTabs_DrawItem;
            this.MouseClick += BeepTabs_MouseClick;
            this.SelectedIndexChanged += BeepTabs_SelectedIndexChanged;
        }

     

        public override Rectangle DisplayRectangle
        {
            get
            {
                switch (_headerPosition)
                {
                    case TabHeaderPosition.Top:
                        return new Rectangle(0, HeaderHeight, ClientSize.Width, ClientSize.Height - HeaderHeight);
                    case TabHeaderPosition.Bottom:
                        return new Rectangle(0, 0, ClientSize.Width, ClientSize.Height - HeaderHeight);
                    case TabHeaderPosition.Left:
                        return new Rectangle(HeaderHeight, 0, ClientSize.Width - HeaderHeight, ClientSize.Height);
                    case TabHeaderPosition.Right:
                        return new Rectangle(0, 0, ClientSize.Width - HeaderHeight, ClientSize.Height);
                    default:
                        return base.DisplayRectangle;
                }
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            UpdateLayout();
        }

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

        private void UpdateLayout()
        {
            Rectangle rect = DisplayRectangle;
            foreach (TabPage page in TabPages)
            {
                page.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
            }
            Invalidate();
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size baseSize = base.GetPreferredSize(proposedSize);
            switch (HeaderPosition)
            {
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
            // Use the parent’s BackColor (panel) if available, otherwise fall back to control’s BackColor
            Color backgroundColor = Parent?.BackColor ?? BackColor;
            e.Graphics.Clear(backgroundColor);
            // Extend the clip region to include the header to avoid gaps
            Rectangle fullRegion = ClientRectangle;
            switch (HeaderPosition)
            {
                case TabHeaderPosition.Top:
                    fullRegion = new Rectangle(0, 0, ClientSize.Width, HeaderHeight);
                    break;
                case TabHeaderPosition.Bottom:
                    fullRegion = new Rectangle(0, ClientSize.Height - HeaderHeight, ClientSize.Width, HeaderHeight);
                    break;
                case TabHeaderPosition.Left:
                    fullRegion = new Rectangle(0, 0, HeaderHeight, ClientSize.Height);
                    break;
                case TabHeaderPosition.Right:
                    fullRegion = new Rectangle(ClientSize.Width - HeaderHeight, 0, HeaderHeight, ClientSize.Height);
                    break;
            }
            e.Graphics.SetClip(fullRegion, CombineMode.Intersect);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            DrawTabHeaders(e.Graphics);
            base.OnPaint(e);
        }

        private void DrawTabHeaders(Graphics g)
        {
            if (TabCount == 0)
                return;

            // Get the panel's background color
            Color panelColor = Parent?.BackColor ?? BackColor;

            // Fill the entire header region with the panel's background color and draw tabs
            switch (_headerPosition)
            {
                case TabHeaderPosition.Top:
                    {
                        Rectangle headerRegion = new Rectangle(0, 0, Width, HeaderHeight);
                        using (SolidBrush brush = new SolidBrush(panelColor))
                        {
                            g.FillRectangle(brush, headerRegion); // Fill the entire top region
                        }

                        int tabWidth = Width / TabCount;
                        int remainingPixels = Width % TabCount; // Calculate remaining pixels to distribute
                        int currentX = 0;
                        for (int i = 0; i < TabCount; i++)
                        {
                            // Adjust the width of the last tab to fill any remaining space
                            int adjustedWidth = tabWidth + (i == TabCount - 1 ? remainingPixels : 0);
                            Rectangle tabRect = new Rectangle(currentX, 0, adjustedWidth, HeaderHeight);
                            DrawHeaderForTab(g, tabRect, i, false);
                            currentX += adjustedWidth;
                        }
                        break;
                    }
                case TabHeaderPosition.Bottom:
                    {
                        Rectangle headerRegion = new Rectangle(0, ClientSize.Height - HeaderHeight, Width, HeaderHeight);
                        using (SolidBrush brush = new SolidBrush(panelColor))
                        {
                            g.FillRectangle(brush, headerRegion);
                        }

                        int tabWidth = Width / TabCount;
                        int remainingPixels = Width % TabCount;
                        int currentX = 0;
                        for (int i = 0; i < TabCount; i++)
                        {
                            int adjustedWidth = tabWidth + (i == TabCount - 1 ? remainingPixels : 0);
                            Rectangle tabRect = new Rectangle(currentX, ClientSize.Height - HeaderHeight, adjustedWidth, HeaderHeight);
                            DrawHeaderForTab(g, tabRect, i, false);
                            currentX += adjustedWidth;
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

                        int tabHeight = Height / TabCount;
                        int remainingPixels = Height % TabCount;
                        int currentY = 0;
                        for (int i = 0; i < TabCount; i++)
                        {
                            int adjustedHeight = tabHeight + (i == TabCount - 1 ? remainingPixels : 0);
                            Rectangle tabRect = new Rectangle(0, currentY, HeaderHeight, adjustedHeight);
                            DrawHeaderForTab(g, tabRect, i, true);
                            currentY += adjustedHeight;
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

                        int tabHeight = Height / TabCount;
                        int remainingPixels = Height % TabCount;
                        int currentY = 0;
                        for (int i = 0; i < TabCount; i++)
                        {
                            int adjustedHeight = tabHeight + (i == TabCount - 1 ? remainingPixels : 0);
                            Rectangle tabRect = new Rectangle(ClientSize.Width - HeaderHeight, currentY, HeaderHeight, adjustedHeight);
                            DrawHeaderForTab(g, tabRect, i, true);
                            currentY += adjustedHeight;
                        }
                        break;
                    }
            }
        }

        private void DrawHeaderForTab(Graphics g, Rectangle tabRect, int index, bool vertical)
        {
            bool isSelected = (SelectedIndex == index);
            // Use a slightly darker shade for the selected tab, otherwise transparent to show the panel color
            Color backgroundColor = isSelected
                ? ControlPaint.Dark(Parent?.BackColor ?? BackColor, 0.2f) // Slightly darker for selected
                : Color.Transparent; // Let the panel color show through for unselected tabs
            Color textColor = isSelected
                ? (_currentTheme?.ActiveTabForeColor ?? Color.White) // White text for selected
                : (_currentTheme?.TabForeColor ?? Color.LightGray); // Light gray for unselected

            using (GraphicsPath path = GetRoundedRect(tabRect, 4)) // Reduced radius to minimize gaps
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
                    PointF textPoint = new PointF(tabRect.X + 10, tabRect.Y + (tabRect.Height - textSize.Height) / 2);
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
                DrawCloseButton(g, tabRect);
            else
                DrawCloseButtonVertical(g, tabRect);
        }

        private void DrawCloseButton(Graphics g, Rectangle tabRect)
        {
            Rectangle closeRect = new Rectangle(
                tabRect.Right - CloseButtonSize - CloseButtonPadding,
                tabRect.Top + (tabRect.Height - CloseButtonSize) / 2,
                CloseButtonSize,
                CloseButtonSize
            );
            closeIcon.DrawingRect = closeRect;
            closeIcon.Draw(g, closeRect);
        }

        private void DrawCloseButtonVertical(Graphics g, Rectangle tabRect)
        {
            Rectangle closeRect = new Rectangle(
                tabRect.X + (tabRect.Width - CloseButtonSize) / 2,
                tabRect.Bottom - CloseButtonSize - CloseButtonPadding,
                CloseButtonSize,
                CloseButtonSize
            );
            closeIcon.DrawingRect = closeRect;
            closeIcon.Draw(g, closeRect);
        }

        private void BeepTabs_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Custom drawing handled in OnPaint
        }

        private void BeepTabs_MouseClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine($"MouseClick in BeepTabs at screen: {e.Location}, client: {PointToClient(Cursor.Position)}");
            Console.WriteLine($"MouseClick in BeepTabs at screen: {e.Location}, client: {PointToClient(Cursor.Position)}");

            int tabCount = TabCount;
            Point clientPoint = e.Location; // Use event coordinates directly
                                            // Guard against zero tabs to prevent division by zero
            if (tabCount == 0)
            {
                Debug.WriteLine("No tabs present, ignoring click.");
                return;
            }
            switch (_headerPosition)
            {
                case TabHeaderPosition.Top:
                case TabHeaderPosition.Bottom:
                    {
                        int tabWidth = Width / tabCount;
                        int remainingPixels = Width % tabCount;
                        int yPos = _headerPosition == TabHeaderPosition.Top ? 0 : ClientSize.Height - HeaderHeight;
                        int currentX = 0;
                        for (int i = 0; i < tabCount; i++)
                        {
                            int adjustedWidth = tabWidth + (i == tabCount - 1 ? remainingPixels : 0);
                            Rectangle tabRect = new Rectangle(currentX, yPos, adjustedWidth, HeaderHeight);
                            Rectangle closeRect = GetCloseButtonRect(tabRect, false);
                            string tabText = TabPages[i].Text;
                            Debug.WriteLine($"Tab {i} rect: {tabRect}, Close rect: {closeRect}");
                            if (closeRect.Contains(clientPoint))
                            {
                                Debug.WriteLine($"Close button clicked for tab {i}");
                                try { TabPages.RemoveAt(i); } catch (Exception ex) { Console.WriteLine("Error removing tab: " + ex.Message); }
                                TabRemoved?.Invoke(this, new TabRemovedEventArgs { TabText = tabText });
                                return;
                            }
                            if (tabRect.Contains(clientPoint))
                            {
                                SelectedIndex = i;
                                Debug.WriteLine($"Tab {i} selected");
                                return;
                            }
                            currentX += adjustedWidth;
                        }
                        break;
                    }
                case TabHeaderPosition.Left:
                case TabHeaderPosition.Right:
                    {
                        int tabHeight = Height / tabCount;
                        int remainingPixels = Height % tabCount;
                        int xPos = _headerPosition == TabHeaderPosition.Left ? 0 : ClientSize.Width - HeaderHeight;
                        int currentY = 0;
                        for (int i = 0; i < tabCount; i++)
                        {
                            int adjustedHeight = tabHeight + (i == tabCount - 1 ? remainingPixels : 0);
                            Rectangle tabRect = new Rectangle(xPos, currentY, HeaderHeight, adjustedHeight);
                            Rectangle closeRect = GetCloseButtonRect(tabRect, true);
                            string tabText = TabPages[i].Text;
                            Debug.WriteLine($"Tab {i} rect: {tabRect}, Close rect: {closeRect}");
                            if (closeRect.Contains(clientPoint))
                            {
                                Debug.WriteLine($"Close button clicked for tab {i}");
                                try { TabPages.RemoveAt(i); } catch (Exception ex) { Console.WriteLine("Error removing tab: " + ex.Message); }
                                TabRemoved?.Invoke(this, new TabRemovedEventArgs { TabText = tabText });
                                return;
                            }
                            if (tabRect.Contains(clientPoint))
                            {
                                SelectedIndex = i;
                                Debug.WriteLine($"Tab {i} selected");
                                return;
                            }
                            currentY += adjustedHeight;
                        }
                        break;
                    }
            }
        }

        private void BeepTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            Invalidate(); // Redraw headers to reflect the new selection
        }

        private Rectangle GetCloseButtonRect(Rectangle tabRect, bool vertical)
        {
            if (!vertical)
            {
                return new Rectangle(
                    tabRect.Right - CloseButtonSize - CloseButtonPadding,
                    tabRect.Top + (tabRect.Height - CloseButtonSize) / 2,
                    CloseButtonSize,
                    CloseButtonSize
                );
            }
            else
            {
                return new Rectangle(
                    tabRect.X + (tabRect.Width - CloseButtonSize) / 2,
                    tabRect.Bottom - CloseButtonSize - CloseButtonPadding,
                    CloseButtonSize,
                    CloseButtonSize
                );
            }
        }

        private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.Location, new Size(diameter, diameter));
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
        public virtual void ApplyTheme()
        {
            if (_currentTheme == null)
                return;
            BackColor = _currentTheme.PanelBackColor;
            ForeColor = _currentTheme.ButtonForeColor;
            // change tab pages color
            foreach (TabPage page in TabPages)
            {
                page.BackColor = _currentTheme.PanelBackColor;
                page.ForeColor = _currentTheme.ButtonForeColor;
                // check if the page has a control and pass theme to it
                if (page.Controls.Count > 0)
                {
                    foreach (Control ctrl in page.Controls)
                    {
                        if (ctrl is IBeepUIComponent )
                        {
                            IBeepUIComponent bp=(IBeepUIComponent)ctrl;
                            bp.Theme = Theme;
                           // bp.ApplyTheme();
                        }
                        if(ctrl is IDM_Addin)
                        {
                           
                            foreach (var item in ctrl.Controls)
                            {
                                if (item is IBeepUIComponent)
                                {
                                    IBeepUIComponent bp = (IBeepUIComponent)item;
                                    bp.Theme = Theme;
                                    // bp.ApplyTheme();
                                }
                            }
                            
                        }
                    }
                }
            }

            Invalidate();
        }
        // Add to the BeepTabs class
        public  void ReceiveMouseClick(Point clientLocation)
        {
            Debug.WriteLine($"ReceiveMouseClick in BeepTabs at {clientLocation}");
            OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, clientLocation.X, clientLocation.Y, 0));
        }
    }
    public class TabRemovedEventArgs : EventArgs
    {
        public string TabText { get; set; }
    }
}