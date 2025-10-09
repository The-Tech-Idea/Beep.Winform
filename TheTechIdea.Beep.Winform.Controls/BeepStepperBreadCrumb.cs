using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
   
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Stepper Breadcrumb")]
    [Description("A breadcrumb-style stepper control that draws chevron-shaped steps directly, with clickable steps and optional orientation.")]
    public class BeepStepperBreadCrumb : BeepControl
    {
        private Orientation orientation = Orientation.Horizontal;
        private int selectedIndex = -1;
        private readonly List<GraphicsPath> chevronPaths = new List<GraphicsPath>(); // For precise click detection

        // Animation
        private System.Windows.Forms.Timer animationTimer;
        private int animFrame;
        private const int animFramesTotal = 10;
        private int animIndex = -1;
        private Color animStart, animEnd;
       

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Which way middle chevrons should point.")]
        public ChevronDirection Direction { get; set; } = ChevronDirection.Forward;

        private string _defaultImagePathForStepButtons = "check.svg";
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Default image path for checked steps. If not set, no image is displayed.")]
        public string CheckImage
        {
            get => _defaultImagePathForStepButtons;
            set
            {
                _defaultImagePathForStepButtons = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Controls whether the stepper is laid out horizontally or vertically.")]
        public Orientation Orientation
        {
            get => orientation;
            set
            {
                orientation = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The steps to display.")]
        public BindingList<SimpleItem> ListItems { get; set; } = new BindingList<SimpleItem>();

        [Browsable(false)]
        public int SelectedIndex => selectedIndex;

        [Browsable(false)]
        public SimpleItem SelectedItem => selectedIndex >= 0 && selectedIndex < ListItems.Count ? ListItems[selectedIndex] : null;

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        public BeepStepperBreadCrumb()
        {
            ListItems.ListChanged += (s, e) => Invalidate();
            SizeChanged += (s, e) => Invalidate();
            DoubleBuffered = true;
            Padding = new Padding(5);
            animationTimer = new System.Windows.Forms.Timer { Interval = 25 };
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (animIndex < 0 || animIndex >= ListItems.Count)
            {
                animationTimer.Stop();
                return;
            }

            animFrame++;
            float t = animFrame / (float)animFramesTotal;
            if (t > 1f) t = 1f;
            int r = (int)(animStart.R + (animEnd.R - animStart.R) * t);
            int g = (int)(animStart.G + (animEnd.G - animStart.G) * t);
            int b = (int)(animStart.B + (animEnd.B - animStart.B) * t);
            animatedColors[animIndex] = Color.FromArgb(r, g, b);
            Invalidate();

            if (animFrame >= animFramesTotal)
            {
                animationTimer.Stop();
                animIndex = -1;
                animatedColors.Remove(animIndex);
            }
        }

        private Dictionary<int, Color> animatedColors = new Dictionary<int, Color>();

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);
        }
    protected override void DrawContent(Graphics g)
{
    base.DrawContent(g);
    UpdateDrawingRect();
    chevronPaths.Clear();
    animatedColors.Clear();

    if (ListItems == null || ListItems.Count == 0)
        return;

    int count       = ListItems.Count;
    int totalLen    = orientation == Orientation.Horizontal 
                         ? DrawingRect.Width  
                         : DrawingRect.Height;
    int crossLen    = orientation == Orientation.Horizontal 
                         ? DrawingRect.Height 
                         : DrawingRect.Width;
    int stepLen     = totalLen / Math.Max(1, count);

    // Depth of each arrowhead. Tune (/3 or /4) if you want shallower points.
    int arrowSize   = crossLen / 4; 

    for (int i = 0; i < count; i++)
    {
        int x = orientation == Orientation.Horizontal
                    ? DrawingRect.Left + (i * stepLen)
                    : DrawingRect.Left;
        int y = orientation == Orientation.Horizontal
                    ? DrawingRect.Top
                    : DrawingRect.Top + (i * stepLen) ;

            Point[] pts;

        if (orientation == Orientation.Horizontal)
        {
            if (i == 0)
            {
                // ───► first: straight left, arrow on right
                pts = new[]
                {
                    new Point(x,                  y),
                    new Point(x + stepLen - arrowSize, y),
                    new Point(x + stepLen,        y + crossLen/2),
                    new Point(x + stepLen - arrowSize, y + crossLen),
                    new Point(x,                  y + crossLen),
                    new Point(x,                  y)

                };
            }
            else if (i == count - 1)
            {
                // ◄─── last: arrow on left, straight right
                pts = new[]
                {
                    new Point(x,                  y),
                    new Point(x + stepLen,        y),
                    new Point(x + stepLen,        y + crossLen),
                    new Point(x,                  y + crossLen),
                    new Point(x + arrowSize,      (y+ crossLen)- crossLen/2),
                    new Point(x ,      y ),
                };
            }
            else
            {
                // ◄───► middle: arrow on both sides
                pts = new[]
                {
                    new Point(x,                  y),
                    new Point(x + stepLen - arrowSize, y),
                     new Point(x + stepLen,        y + crossLen/2),
                    new Point(x + stepLen - arrowSize, y + crossLen),
                    new Point(x,                  y + crossLen),
                     new Point(x + arrowSize,      (y+ crossLen)- crossLen/2),
                    new Point(x ,      y ),

                };
            }
        }
        else
        {
            // Vertical: mirror the same pattern top/bottom
            if (i == 0)
            {
                // straight top, arrow at bottom
                pts = new[]
                {
                    new Point(x,                  y),
                    new Point(x + crossLen,       y),
                    new Point(x + crossLen/2,     y + stepLen),
                    new Point(x,                  y + stepLen)
                };
            }
            else if (i == count - 1)
            {
                // arrow at top, straight bottom
                pts = new[]
                {
                    new Point(x + crossLen/2,     y),
                    new Point(x + crossLen,       y + stepLen - arrowSize),
                    new Point(x + crossLen,       y + stepLen),
                    new Point(x,                  y + stepLen)
                };
            }
            else
            {
                // middle: arrow on both top & bottom
                pts = new[]
                {
                    new Point(x + crossLen/2,     y),               // top tip
                    new Point(x + crossLen,       y + arrowSize),   // right‑top slant
                    new Point(x + crossLen,       y + stepLen - arrowSize), // right‑bottom
                    new Point(x + crossLen/2,     y + stepLen),     // bottom tip
                    new Point(x,                  y + stepLen - arrowSize), // left‑bottom
                    new Point(x,                  y + arrowSize)     // left‑top
                };
            }
        }

        // hit‑test
        var path = new GraphicsPath();
        path.AddPolygon(pts);
        chevronPaths.Add(path);

        // fill
        Color fill = animatedColors.ContainsKey(i)
            ? animatedColors[i]
            : i < selectedIndex
                ? _currentTheme.ButtonSelectedBackColor
                : i == selectedIndex
                    ? _currentTheme.ButtonBackColor
                    : _currentTheme.DisabledBackColor;

        using (var br = new SolidBrush(fill))
            g.FillPolygon(br, pts);

        using (var pen = new Pen(_currentTheme.ShadowColor, _borderThickness))
            g.DrawPolygon(pen, pts);

               if(orientation == Orientation.Horizontal)
                {
                    // Get item info
                    var headerText = ListItems[i].Name ?? "";
                    var subText = ListItems[i].Text ?? "";

                    // Fonts
                    var headerFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
                    var subFont = Font;

                    // Measure both
                    var headerSize = g.MeasureString(headerText, headerFont);
                    var subSize = g.MeasureString(subText, subFont);

                    // Total vertical space
                    float totalTextHeight = headerSize.Height + subSize.Height;

                    // Starting Y so both lines are vertically centered as a block
                    float startY = y + (crossLen - totalTextHeight) / 2;

                    // Horizontal X center
                    float headerX = x + (stepLen - headerSize.Width) / 2;
                    float subX = x + (stepLen - subSize.Width) / 2;

                    // Colors
                    Color foreColor = i == selectedIndex ? _currentTheme.AccentTextColor : _currentTheme.ButtonForeColor;

                    // Draw header
                    using (var brush = new SolidBrush(foreColor))
                    {
                        g.DrawString(headerText, headerFont, brush, headerX, startY);
                        g.DrawString(subText, subFont, brush, subX, startY + headerSize.Height);
                    }

                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            IsPressed = false;
            IsSelected = false;
            for (int i = 0; i < chevronPaths.Count; i++)
            {
                using (Region region = new Region(chevronPaths[i]))
                {
                    if (region.IsVisible(e.Location))
                    {
                        OnStepClicked(i);
                        break;
                    }
                }
            }
        }

        private void OnStepClicked(int index)
        {
            if (index >= 0 && index < ListItems.Count)
            {
                animIndex = index;
                animFrame = 0;
                animStart = selectedIndex == index ? _currentTheme.ButtonBackColor : _currentTheme.DisabledBackColor;
                animEnd = _currentTheme.ButtonBackColor;
                animationTimer.Start();
                selectedIndex = index;
                SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(SelectedItem));
                Invalidate();
            }
        }

   

        public void UpdateCheckedState(SimpleItem item)
        {
            int index = ListItems.IndexOf(item);
            if (index >= 0 && index < ListItems.Count)
            {
                UpdateCurrentStep(index);
            }
        }

        public void UpdateCurrentStep(int index)
        {
            if (index >= 0 && index < ListItems.Count)
            {
                ListItems[index].IsChecked = true;
                SetAllStepsBefore(index);
                Invalidate();
            }
        }

        private void SetAllStepsBefore(int index)
        {
            for (int i = 0; i < ListItems.Count; i++)
            {
                ListItems[i].IsChecked = i <= index;
            }
        }
        public override void ApplyTheme()
        {
            //   base.ApplyTheme();
            if (IsChild && Parent != null)
            {
                BackColor = Parent.BackColor;
                ParentBackColor = Parent.BackColor;
            }
            BackColor = _currentTheme.CardBackColor;
            ForeColor = _currentTheme.CardTextForeColor;
            HoverBackColor = _currentTheme.ButtonHoverBackColor;
            HoverForeColor = _currentTheme.ButtonHoverForeColor;
            DisabledBackColor = _currentTheme.DisabledBackColor;
            DisabledForeColor = _currentTheme.DisabledForeColor;
            FocusBackColor = _currentTheme.ButtonSelectedBackColor;
            FocusForeColor = _currentTheme.ButtonSelectedForeColor;


            PressedBackColor = _currentTheme.ButtonPressedBackColor;
            PressedForeColor = _currentTheme.ButtonPressedForeColor;

            //  if (_beepListBox != null)   _beepListBox.MenuStyle = MenuStyle;
            //if (UseThemeFont)
            //{
            //    _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            //    Font = _textFont;
            //}

            //beepImage.MenuStyle = MenuStyle;
            //ApplyThemeToSvg();
            Invalidate();  // Trigger repaint
        }
    }
}