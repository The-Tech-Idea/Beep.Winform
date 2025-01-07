using System.ComponentModel;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Functions Panel")]
    [Category("Beep Controls")]
    [Description("A panel control that displays common functions as buttons.")]
    public class BeepFunctionsPanel : BeepControl
    {
        public BeepButton btnRefresh, btnSearch, btnFilter, btnSort, btnPrint, btnExport, btnClose, btnHelp, btnSettings, btnAbout, btnExit;
        public int ButtonWidth { get; set; } = 20;
        public int ButtonHeight { get; set; } = 20;
        public int XOffset { get; set; } = 5;
        public int YOffset { get; set; } = 5;
        public Dictionary<string, BeepButton> FunctionButtons { get; set; } = new Dictionary<string, BeepButton>();
        private Orientation _orientation = Orientation.Horizontal;

        public event EventHandler<BeepEventDataArgs> FunctionCalled;
        bool _applyThemeOnImage = false;
        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
                ApplyTheme();
                Invalidate();
            }
        }
        int buttonSpacing = 5;
        public int ButtonSpacing
        {
            get => buttonSpacing;
            set
            {
                buttonSpacing = value;
                ArrangeFunctionButtons();
                Invalidate();
            }
        }
        // Override OnHandleCreated to ensure layout runs correctly once control is created
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ArrangeFunctionButtons();
            PerformLayout();
            Invalidate();
        }
        public BeepFunctionsPanel()
        {
            this.ShowShadow = false;                                                                                              
            UpdateDrawingRect();
            CreateFunctionButtons();
            ArrangeFunctionButtons();
            PerformLayout();  // Ensure immediate layout update
            Invalidate();
        }
       
        // Property to set the panel's orientation
        public Orientation PanelOrientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                ArrangeFunctionButtons(); // Re-arrange buttons when orientation changes
                Invalidate();
            }
        }
        
        private void CreateFunctionButtons()
        {
            Controls.Clear();
            FunctionButtons.Clear();
            btnRefresh = CreateButton("Refresh", Button_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.refresh.svg");
            btnSearch = CreateButton("Search", Button_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.search.svg");
            btnFilter = CreateButton("Filter", Button_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.filter.svg");
            btnSort = CreateButton("Sort", Button_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.sort.svg");
            btnPrint = CreateButton("Print", Button_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.print.svg");
            btnExport = CreateButton("Export", Button_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.export.svg");
            btnClose = CreateButton("Close", Button_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.close.svg");
            btnHelp = CreateButton("Help", Button_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.info.svg");
            btnSettings = CreateButton("Settings", Button_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.settings.svg");
            btnAbout = CreateButton("About", Button_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.about.svg");
            btnExit = CreateButton("Exit", Button_Click, "TheTechIdea.Beep.Winform.Controls.GFX.SVG.power.svg");
            FunctionButtons.Add("Refresh", btnRefresh);
            FunctionButtons.Add("Search", btnSearch);
            FunctionButtons.Add("Filter", btnFilter);
            FunctionButtons.Add("Sort", btnSort);
            FunctionButtons.Add("Print", btnPrint);
            FunctionButtons.Add("Export", btnExport);
            FunctionButtons.Add("Close", btnClose);
            FunctionButtons.Add("Help", btnHelp);
            FunctionButtons.Add("Settings", btnSettings);
            FunctionButtons.Add("About", btnAbout);
            FunctionButtons.Add("Exit", btnExit);

            foreach (var button in FunctionButtons)
            {
                button.Value.Theme = Theme;
                button.Value.ApplyThemeToSvg();
                Controls.Add(button.Value);
            }
           
        }

        private BeepButton CreateButton(string text, EventHandler onClick, string imagePath = null)
        {
            var btn = new BeepButton
            {
                Text = text,
                Size = new Size(ButtonWidth, ButtonHeight),
                MaxImageSize = new Size(ButtonWidth-2 , ButtonHeight-2),
                Tag = text,
                IsChild = true,
                Theme = Theme,
                ShowAllBorders = false,
                ShowShadow = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                Anchor = AnchorStyles.None,
                ApplyThemeOnImage=true
                
               
            };
            if (!string.IsNullOrEmpty(imagePath))
            {
                btn.ImagePath = imagePath;
                btn.ImageAlign = ContentAlignment.MiddleCenter;
                btn.TextImageRelation = TextImageRelation.ImageAboveText;
                btn.Text = "";
                btn.ToolTipText = text;
            }
            btn.Theme = Theme;
            btn.Click += onClick;
            return btn;
        }

        private void ArrangeFunctionButtons()
        {

            // Calculate starting X and Y positions to center align within DrawingRect
            SuspendLayout();  // Suspend layout changes for smooth resizing
            //  int y = drawRectY + YOffset;
            if (_orientation == Orientation.Horizontal)
            {
                this.MinimumSize = new Size((ButtonWidth * FunctionButtons.Count) + (buttonSpacing * FunctionButtons.Count + 2), ButtonHeight + 9); // Set based on layout needs
                this.Size = new Size((ButtonWidth * FunctionButtons.Count) + (buttonSpacing * FunctionButtons.Count + 2), ButtonHeight + 9); // Set based on layout needs
               
            }
            else
            {
                this.MinimumSize = new Size(ButtonWidth + 9, (ButtonHeight * FunctionButtons.Count) + (buttonSpacing * FunctionButtons.Count + 2)); // Set based on layout needs
                this.Size = new Size(ButtonWidth + 9, (ButtonHeight * FunctionButtons.Count) + (buttonSpacing * FunctionButtons.Count + 2)); // Set based on layout needs
            }
            UpdateDrawingRect();
            // Get the dimensions of DrawingRect
            int drawRectX = DrawingRect.X;
            int drawRectY = DrawingRect.Y;
            int drawRectWidth = DrawingRect.Width;
            int drawRectHeight = DrawingRect.Height;
            int x = drawRectX + XOffset;
          
            if (_orientation == Orientation.Horizontal)
            {
                int y = drawRectY + (drawRectHeight - ButtonHeight) / 2; // Center vertically within DrawingRect
                foreach (var dx in FunctionButtons)
                {
                    BeepButton button = dx.Value;
                    button.Anchor = AnchorStyles.None;
                    button.Location = new Point(x, y);
                    x += ButtonWidth + buttonSpacing; // Move X position for the next button
                }
            }
            else
            {
                int y = drawRectY +YOffset; // Center vertically within DrawingRect
                // Arrange buttons vertically
                foreach (var dx in FunctionButtons)
                {
                    BeepButton button = dx.Value;
                    button.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    button.Location = new Point(x, y);
                    y += ButtonHeight + buttonSpacing; // Move Y position for the next button
                }
            }
            ResumeLayout(false);  // Resume layout and force layout update
        }
      protected override void OnResize(EventArgs e)
        {
           // base.OnResize(e);
            ArrangeFunctionButtons();
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Draw base elements (border, shadow, etc.)

            // Draw title text if enabled
            
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
           
        }
        protected override void OnMouseLeave(EventArgs e)
        {

        }
        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    // Do not call base.OnPaintBackground(e);
        //    // Fill the background with the control's background color

        //    if (IsChild)
        //    {
        //        using (SolidBrush brush = new SolidBrush(parentbackcolor))
        //        {
        //            e.Graphics.FillRectangle(brush, DrawingRect);
        //        }
        //    }
        //    else
        //    {
        //        using (SolidBrush brush = new SolidBrush(BackColor))
        //        {
        //            e.Graphics.FillRectangle(brush, DrawingRect);
        //        }
        //    }
        //}
        public override void ApplyTheme()
        {
            //base.ApplyTheme();
            BackColor = _currentTheme.BackgroundColor;;
            foreach (Control ctrl in Controls)
            { 
                ApplyThemeToControl(ctrl);
                //if (ctrl is BeepButton)
                //{
                //    ((BeepButton)ctrl).Theme = Theme;
                //    if (ApplyThemeOnImage)
                //    {
                //        ((BeepButton)ctrl).ApplyThemeOnImage=true;
                //    }

                //}
                //else if (ctrl is BeepLabel)
                //{
                //    ((BeepLabel)ctrl).Theme = Theme;
                //}
            }
            Invalidate();
        }
        private void Button_Click(object sender, EventArgs e)
        {
            if (sender is BeepButton button && button.Tag is string tag)
            {
                FunctionCalled?.Invoke(this, new BeepEventDataArgs(tag, null));
            }
        }
    }
}
