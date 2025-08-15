using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Winform.Controls.Models;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Stepper Bar (Drawn)")]
    [Description("A drawn circular stepper bar with click interaction and optional images.")]
    public class BeepStepperBar : BeepControl
    {
        private Orientation orientation = Orientation.Horizontal;
        private int selectedIndex = -1;
        private Size buttonSize = new Size(30, 30);
        private List<Rectangle> buttonBounds = new();
        private readonly Dictionary<int, Image> stepImages = new();

        private Timer animationTimer;
        private float animationProgress = 1f;
        private const int animationDuration = 200; // milliseconds
        private DateTime animationStartTime;
        private int animatingToIndex = -1;

      

        [Browsable(true)]
        [Category("Appearance")]
        public Orientation Orientation
        {
            get => orientation;
            set { orientation = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Layout")]
        public Size ButtonSize
        {
            get => buttonSize;
            set { buttonSize = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Data")]
        public BindingList<SimpleItem> ListItems { get; set; } = new();

        [Browsable(true)]
        [Category("Appearance")]
        public string CheckImage { get; set; } = "check.svg";

        [Browsable(true)]
        [Category("Appearance")]
        public StepDisplayMode DisplayMode { get; set; } = StepDisplayMode.StepNumber;

        [Browsable(false)]
        public int SelectedIndex => selectedIndex;

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        public BeepStepperBar()
        {
            DoubleBuffered = true;
            ListItems.ListChanged += (s, e) => Invalidate();

            animationTimer = new Timer { Interval = 16 };
            animationTimer.Tick += (s, e) =>
            {
                var elapsed = (DateTime.Now - animationStartTime).TotalMilliseconds;
                animationProgress = (float)Math.Min(1, elapsed / animationDuration);
                if (animationProgress >= 1f)
                {
                    animationTimer.Stop();
                    selectedIndex = animatingToIndex;
                }
                Invalidate();
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            buttonBounds.Clear();

            if (ListItems.Count == 0) return;

            int count = ListItems.Count;
            int spacing = 20;
            int stepTotalSize = orientation == Orientation.Horizontal ? buttonSize.Width : buttonSize.Height;
            int totalLength = (stepTotalSize + spacing) * count - spacing;

            Point startPoint = orientation == Orientation.Horizontal
                ? new Point((Width - totalLength) / 2, (Height - buttonSize.Height) / 2)
                : new Point((Width - buttonSize.Width) / 2, (Height - totalLength) / 2);

            for (int i = 0; i < count; i++)
            {
                int x = orientation == Orientation.Horizontal
                    ? startPoint.X + i * (buttonSize.Width + spacing)
                    : startPoint.X;

                int y = orientation == Orientation.Horizontal
                    ? startPoint.Y
                    : startPoint.Y + i * (buttonSize.Height + spacing);

                Rectangle rect = new Rectangle(x, y, buttonSize.Width, buttonSize.Height);
                buttonBounds.Add(rect);

                // Draw connector line
                if (i > 0)
                {
                    Point p1 = orientation == Orientation.Horizontal
                        ? new Point(buttonBounds[i - 1].Right, buttonBounds[i - 1].Top + buttonSize.Height / 2)
                        : new Point(buttonBounds[i - 1].Left + buttonSize.Width / 2, buttonBounds[i - 1].Bottom);

                    Point p2 = orientation == Orientation.Horizontal
                        ? new Point(rect.Left, rect.Top + buttonSize.Height / 2)
                        : new Point(rect.Left + buttonSize.Width / 2, rect.Top);

                    e.Graphics.DrawLine(new Pen(_currentTheme.BorderColor, 2), p1, p2);
                }

                // Animate highlight
                float scale = 1f;
                if (i == animatingToIndex && animationProgress < 1f)
                {
                    scale = 1f + 0.2f * (1 - animationProgress);
                }

                Rectangle inflated = Rectangle.Inflate(rect, (int)(rect.Width * (scale - 1) / 2), (int)(rect.Height * (scale - 1) / 2));
                Color fillColor = ListItems[i].IsChecked ? _currentTheme.ButtonPressedBackColor
               : i == selectedIndex ? _currentTheme.ButtonBackColor
               : _currentTheme.DisabledBackColor;

                using (Brush fillBrush = new SolidBrush(fillColor))
                {
                    e.Graphics.FillEllipse(fillBrush, inflated);
                }

                // Draw inside content
                switch (DisplayMode)
                {
                    case StepDisplayMode.CheckImage:
                        if (ListItems[i].IsChecked && !string.IsNullOrWhiteSpace(CheckImage))
                        {
                            if (!stepImages.ContainsKey(i))
                            {
                                try { stepImages[i] = ImageLoader.LoadImageFromResource(CheckImage); } catch { }
                            }
                            if (stepImages[i] != null)
                            {
                                e.Graphics.DrawImage(stepImages[i], rect);
                            }
                        }
                        break;
                    case StepDisplayMode.StepNumber:
                        string numText = (i + 1).ToString();
                        var numSize = e.Graphics.MeasureString(numText, Font);
                        var numX = rect.Left + (rect.Width - numSize.Width) / 2;
                        var numY = rect.Top + (rect.Height - numSize.Height) / 2;
                        using (var numBrush = new SolidBrush(_currentTheme.ButtonForeColor))
                        {
                            e.Graphics.DrawString(numText, Font, numBrush, numX, numY);
                        }
                        break;
                    case StepDisplayMode.SvgIcon:
                        // Extend here if needed
                        break;
                }

                // Draw optional text (header + subtext)
                string header = ListItems[i].Name ?? "";
                string subtext = ListItems[i].Text ?? "";
                var headerFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
                var headerSize = e.Graphics.MeasureString(header, headerFont);
                var subSize = e.Graphics.MeasureString(subtext, Font);
                float totalTextHeight = headerSize.Height + subSize.Height;
                float textX = rect.Left + (rect.Width - Math.Max(headerSize.Width, subSize.Width)) / 2;
                float baseY = rect.Bottom + 4;

                using (var headerBrush = new SolidBrush(_currentTheme.CardTitleForeColor))
                using (var subBrush = new SolidBrush(_currentTheme.CardSubTitleForeColor))
                {
                    if (orientation == Orientation.Horizontal)
                    {
                        e.Graphics.DrawString(header, headerFont, headerBrush, textX, baseY);
                        e.Graphics.DrawString(subtext, Font, subBrush, rect.Left + (rect.Width - subSize.Width) / 2, baseY + headerSize.Height);
                    }
                    else
                    {
                        e.Graphics.DrawString(header, headerFont, headerBrush, rect.Right + 6, rect.Top);
                        e.Graphics.DrawString(subtext, Font, subBrush, rect.Right + 6, rect.Top + headerSize.Height);
                    }
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            IsPressed = false;
            IsSelected=false;
            for (int i = 0; i < buttonBounds.Count; i++)
            {
                if (buttonBounds[i].Contains(e.Location))
                {
                    UpdateCurrentStep(i);
                    break;
                }
            }
        }

        public void UpdateCurrentStep(int index)
        {
            if (index < 0 || index >= ListItems.Count || index == selectedIndex)
                return;

            animatingToIndex = index;
            animationStartTime = DateTime.Now;
            animationProgress = 0f;
            animationTimer.Start();

            // Update checked state visually
            for (int i = 0; i < ListItems.Count; i++)
            {
                ListItems[i].IsChecked = i <= index;
            }

            Invalidate();
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(ListItems[index]));
        }

        public void UpdateCheckedState(SimpleItem item)
        {
            int index = ListItems.IndexOf(item);
            if (index >= 0 && index < ListItems.Count)
            {
                UpdateCurrentStep(index);
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
            else
            {
                BackColor = _currentTheme.CardBackColor;
            }
        
            ForeColor = _currentTheme.ButtonForeColor;
            HoverBackColor = _currentTheme.ButtonHoverBackColor;
            HoverForeColor = _currentTheme.ButtonHoverForeColor;
            DisabledBackColor = _currentTheme.DisabledBackColor;
            DisabledForeColor = _currentTheme.DisabledForeColor;
            FocusBackColor = _currentTheme.ButtonSelectedBackColor;
            FocusForeColor = _currentTheme.ButtonSelectedForeColor;


            PressedBackColor = _currentTheme.ButtonPressedBackColor;
            PressedForeColor = _currentTheme.ButtonPressedForeColor;

            //  if (_beepListBox != null)   _beepListBox.Theme = Theme;
            //if (UseThemeFont)
            //{
            //    _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            //    Font = _textFont;
            //}

            //beepImage.Theme = Theme;
            //ApplyThemeToSvg();
            Invalidate();  // Trigger repaint
        }

    }
    public enum StepDisplayMode
    {
        StepNumber,
        CheckImage,
        SvgIcon
    }
}
