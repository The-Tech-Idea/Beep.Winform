using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Stepper Bar")]
    [Description("A circular stepper control with clickable BeepCircularButtons, optional orientation, animated transitions, and minimum connector length.")]
    public class BeepSteppperBar : BeepControl
    {
        private Orientation orientation = Orientation.Horizontal;
        private readonly List<BeepLabel> stepLabels = new List<BeepLabel>();
        private readonly List<BeepCircularButton> stepButtons = new List<BeepCircularButton>();
        private readonly List<Panel> connectors = new List<Panel>();
        private int selectedIndex = -1;

        // Animation
        private System.Windows.Forms.Timer animationTimer;
        private int animFrame;
        private const int animFramesTotal = 10;
        private BeepCircularButton animButton;
        private Color animStart, animEnd;

        private Size buttonsize = Size.Empty;

        private int currentstep = 0;

        private string _defaultimagepathforstepbuttons = "check.svg"; // Default image path
        [Browsable(true)]
        [Category("Layout")]
        [Description("Default image path for step buttons. If not set, no image will be displayed.")]
        public string CheckImage
        {
            get => _defaultimagepathforstepbuttons;
            set
            {
                _defaultimagepathforstepbuttons = value;

            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Size of the step buttons. Default is 20X20 pixels.")]
        public Size ButtonSize
        {
            get => buttonsize;
            set
            {
                buttonsize = value;
                foreach (var btn in stepButtons)
                {
                    btn.Size = buttonsize;
                }
                InitLayoutSteps();
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
                InitLayoutSteps();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Minimum length in pixels for connectors between steps.")]
        public int MinConnectorLength { get; set; } = 20;

        [Browsable(true)]
        [Category("Data")]
        [Description("The steps to display.")]
        public BindingList<SimpleItem> ListItems { get; set; } = new BindingList<SimpleItem>();

        [Browsable(false)]
        public int SelectedIndex => selectedIndex;

        [Browsable(false)]
        public SimpleItem SelectedItem => selectedIndex >= 0 && selectedIndex < ListItems.Count ? ListItems[selectedIndex] : null;

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        public BeepSteppperBar()
        {
            ListItems.ListChanged += (s, e) => InitLayoutSteps();
            this.SizeChanged += (s, e) => InitLayoutSteps();

            animationTimer = new System.Windows.Forms.Timer { Interval = 25 };
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (animButton == null)
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
            animButton.BackColor = Color.FromArgb(r, g, b);

            if (animFrame >= animFramesTotal)
            {
                animationTimer.Stop();
                animButton = null;
            }
        }
        private void InitLayoutSteps()
        {
            this.SuspendLayout();
            Controls.Clear();
            stepButtons.Clear();
            connectors.Clear();

            if (ListItems == null || ListItems.Count == 0)
            {
                ResumeLayout();
                return;
            }

            int count = ListItems.Count;
            UpdateDrawingRect();

            int totalLength = orientation == Orientation.Horizontal ? DrawingRect.Width : DrawingRect.Height;
            int slots = Math.Max(0, count - 1);

            Size actualButtonSize;
            if (buttonsize.IsEmpty)
            {
                int crossLength = orientation == Orientation.Horizontal ? DrawingRect.Height : DrawingRect.Width;
                int availableForButtons = totalLength - slots * MinConnectorLength;
                if (availableForButtons < 0) availableForButtons = totalLength;
                int dynSize = Math.Min(crossLength, availableForButtons / Math.Max(1, count));
                actualButtonSize = new Size(dynSize, dynSize);
            }
            else
            {
                actualButtonSize = buttonsize;
            }

            int lineLength;
            if (slots > 0)
            {
                int lengthAvailable = totalLength - (orientation == Orientation.Horizontal ? actualButtonSize.Width : actualButtonSize.Height) * count;
                lineLength = lengthAvailable / slots;
                if (lineLength < MinConnectorLength) lineLength = MinConnectorLength;
            }
            else
            {
                lineLength = 0;
            }

            for (int i = 0; i < count; i++)
            {
                var btn = new BeepCircularButton
                {
                    Text = ListItems[i].Text,
                    Theme = this.Theme,
                    IsChild = true,
                    ImagePath = ListItems[i].IsChecked ? CheckImage : string.Empty,
                    Size = actualButtonSize
                };
                int idx = i;
                btn.Click += (s, e) => OnStepClicked(idx);
                stepButtons.Add(btn);
                Controls.Add(btn);

                if (orientation == Orientation.Horizontal)
                {
                    int yBtn = DrawingRect.Top + (DrawingRect.Height - btn.Height) / 2;
                    int xBtn = DrawingRect.Left + i * (actualButtonSize.Width + lineLength);
                    btn.Location = new Point(xBtn, yBtn);
                    btn.TextLocation = TextLocation.Below;
                }
                else
                {
                    int xBtn = DrawingRect.Left + (DrawingRect.Width - actualButtonSize.Width) / 2;
                    int yBtn = DrawingRect.Top + i * (actualButtonSize.Height + lineLength);
                    btn.Location = new Point(xBtn, yBtn);
                    btn.TextLocation = TextLocation.Right;
                }

                if (i < count - 1)
                {
                    var connector = new Panel { BackColor = _currentTheme.BorderColor };
                    connectors.Add(connector);
                    Controls.Add(connector);
                }
            }

            UpdateConnectors(actualButtonSize,lineLength);
            ResumeLayout();
        }

        private void OnStepClicked(int index)
        {
            if (index >= 0 && index < stepButtons.Count)
            {
                var btn = stepButtons[index];
                animButton = btn;
                animFrame = 0;
                animStart = btn.BackColor;
                animEnd = _currentTheme.ButtonBackColor;
                animationTimer.Start();
            }
            selectedIndex = index;
            //   UpdateSelectionVisuals();
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(SelectedItem));
        }

        private void UpdateSelectionVisuals()
        {
            for (int i = 0; i < stepButtons.Count; i++)
            {
                var btn = stepButtons[i];
                if (i < selectedIndex)
                    btn.BackColor = _currentTheme.ButtonPressedBackColor;
                else if (i == selectedIndex)
                    btn.BackColor = _currentTheme.ButtonBackColor;
                else
                    btn.BackColor = _currentTheme.DisabledBackColor;
            }
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            foreach (var btn in stepButtons)
            {
                btn.Theme = this.Theme;
                // btn.ApplyTheme();
            }
            foreach (var lbl in stepLabels)
            {
                lbl.Theme = this.Theme;

                //lbl.ApplyTheme();
            }
        }
      
        public void UpdateCheckedState(SimpleItem item)
        {
            int index = ListItems.IndexOf(item);
            if (index >= 0 && index < stepButtons.Count)
            {
               
                UpdateCurrentStep(index);
            }
        }
        public void UpdateCurrentStep(int index)
        {
            if (index >= 0 && index < stepButtons.Count)
            {
              //  ListItems[index].IsChecked = true;
                currentstep = index;
                var btn = stepButtons[index];
                
                //animButton = btn;
                //animFrame = 0;
                //animStart = btn.BackColor;
                //animEnd = _currentTheme.ButtonBackColor;
               // animationTimer.Start();
                SetAllStepsBefore(index);
                updatestatus();  
            }
        }
        private void SetAllStepsBefore(int index)
        {
            for (int i = 0; i < stepButtons.Count; i++)
            {
                if (i <= index)
                {
                    ListItems[i].IsChecked = true;
                    stepButtons[i].IsSelected = true;
                   
                }
                else
                {
                    ListItems[i].IsChecked = false;
                    stepButtons[i].IsSelected = false;
                }
            }
        }

        private void updatestatus()
        {
            this.SuspendLayout();
            //foreach (var item in stepButtons)
            //{
            //    item.SuspendLayout();
            //}

            if (ListItems == null || ListItems.Count == 0)
            {
                ResumeLayout();
                return;
            }

            int count = ListItems.Count;
            UpdateDrawingRect();

            int totalLength = orientation == Orientation.Horizontal ? DrawingRect.Width : DrawingRect.Height;
            int slots = Math.Max(0, count - 1);

            Size actualButtonSize;
            if (buttonsize.IsEmpty)
            {
                int crossLength = orientation == Orientation.Horizontal ? DrawingRect.Height : DrawingRect.Width;
                int availableForButtons = totalLength - slots * MinConnectorLength;
                if (availableForButtons < 0) availableForButtons = totalLength;
                int dynSize = Math.Min(crossLength, availableForButtons / Math.Max(1, count));
                actualButtonSize = new Size(dynSize, dynSize);
            }
            else
            {
                actualButtonSize = buttonsize;
            }
            Rectangle circlebounds = new Rectangle(0, 0, actualButtonSize.Width, actualButtonSize.Height);
            if (stepButtons.Count>0)
            {
                circlebounds= stepButtons[0].GetCircleBounds() ;
            }
            else
            {
                return;
            }
            int lineLength;
            if (slots > 0)
            {
                int lengthAvailable = totalLength - (orientation == Orientation.Horizontal ? actualButtonSize.Width : actualButtonSize.Height) * count;
                lineLength = (lengthAvailable / slots)+(circlebounds.Width/2);
                if (lineLength < MinConnectorLength) lineLength = MinConnectorLength;
            }
            else
            {
                lineLength = 0;
            }

            for (int i = 0; i < count; i++)
            {
                var btn = stepButtons[i];
                btn.Text = ListItems[i].Text;
                btn.ImagePath = ListItems[i].IsChecked ? CheckImage : string.Empty;

                if (orientation == Orientation.Horizontal)
                {
                    int yBtn = DrawingRect.Top + (DrawingRect.Height - btn.Height) / 2;
                    int xBtn = DrawingRect.Left + i * (actualButtonSize.Width + lineLength);
                    btn.Location = new Point(xBtn, yBtn);
                }
                else
                {
                    int xBtn = DrawingRect.Left + (DrawingRect.Width - actualButtonSize.Width) / 2;
                    int yBtn = DrawingRect.Top + i * (actualButtonSize.Height + lineLength);
                    btn.Location = new Point(xBtn, yBtn);
                }
            }

            UpdateConnectors(actualButtonSize, lineLength);

            //foreach (var item in stepButtons)
            //{
            //    item.ResumeLayout();
            //}
            ResumeLayout();
        }
        private void UpdateConnectors(Size actualButtonSize,int linelength)
        {
            int thickness = 2;
            for (int i = 0; i < connectors.Count; i++)
            {
                if(i >= stepButtons.Count - 1) break; // No connector after the last button
                var btn = stepButtons[i];
                var nextBtn = stepButtons[i + 1];
                var connector = connectors[i];
                connector.BringToFront();
                connector.BackColor = _currentTheme.BorderColor;
               int y= btn.CircleMidYOffset;
                int x = btn.CircleMidXOffset;
                Point point = btn.CircleCenterOffset;
                Size sz = btn.Size;
                if (orientation == Orientation.Horizontal)
                {
                    int startX = btn.Right-(x/2)+4; // Start at the right edge of the current button
                    int endX = nextBtn.Left; // End at the left edge of the next button
                    int centerY = btn.Top+btn.CircleMidYOffset + btn.Height / 2 - thickness / 2; // Center vertically
                    connector.SetBounds(startX, centerY, linelength+x-8, thickness);
                }
                else
                {
                    int centerX = btn.Left + btn.Width / 2 - thickness / 2; // Center horizontally
                    int startY = btn.Bottom - (y / 2) + 4; // Start at the bottom edge of the current button
                    int endY = nextBtn.Top; // End at the top edge of the next button
                    connector.SetBounds(centerX, startY, thickness, linelength + y - 8);
                }
            }
        }
    }
    }
