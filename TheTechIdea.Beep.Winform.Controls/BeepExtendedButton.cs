using System.ComponentModel;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using System.Drawing.Drawing2D;
using System.Drawing.Text;



namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Extended Button")]
    [Category("Beep Controls")]
    [Description("A control that displays a button with an extended button.")]
    public class BeepExtendedButton:BeepControl
    {
        private BeepButton extendButton;
        private BeepButton button;
        Panel highlightPanel;
        private int buttonWidth = 200;
        private int extendbuttonWidth = 22;
        private int buttonHeight = 30;
        private int starty = 1;
        private int startx = 1;
        
        private Size _imagesize = new Size(20, 20);
        bool _applyThemeOnImage = false;

        public TextImageRelation TextImageRelation
        {
            get => button.TextImageRelation;
            set
            {
                if (button != null) button.TextImageRelation = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public ContentAlignment ImageAlign
        {
            get => button.ImageAlign;
            set
            {
                if (button != null) button.ImageAlign = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public ContentAlignment TextAlign
        {
            get => button.TextAlign;
            set
            {
                if (button != null)  button.TextAlign = value;
                Invalidate();
            }
        }


        [Browsable(true)]
        [Category("Appearance")]
        [Description("Change Value of main Button inside control")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ButtonWidth
        {
            get => button.Width;
            set
            {
                if (button != null) button.Width = value;
                UpdateDrawingRect();
                _isControlinvalidated = true;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public Size MaxImageSize
        {
            get => button.MaxImageSize;
            set
            {
                _imagesize = value;
                if(button != null)
                {
                    button.MaxImageSize = value;
                    button.MaxImageSize = _imagesize;
                    extendButton.MaxImageSize = _imagesize;
                }
              
                Invalidate(); // Repaint when the max image size changes
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Change Value of  images inside control")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int RightButtonSize
        {
            get => extendbuttonWidth;
            set
            {
                if(extendButton != null)
                {
                    extendbuttonWidth = value;
                    extendButton.Width = value;
                    //  extendButton.Height = value;
                    _isControlinvalidated = true;
                    Invalidate();
                }
        
            }
        }
       
        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
                if (button != null)
                {
                    button.ApplyThemeOnImage = value;
                    extendButton.ApplyThemeOnImage = value;
                    if (ApplyThemeOnImage)
                    {
                        extendButton.Theme = Theme;
                        button.Theme = Theme;
                    }
                    _isControlinvalidated = true;
                    Invalidate();
                }
              
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool UseScaledFont
        {
            get => button.UseScaledFont;
            set
            {
                if(button != null)     button.UseScaledFont = value;
                Invalidate();  // Trigger repaint
            }
        }
        private string _buttonImagePath;
        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load.")]
        public string ImagePath
        {
            get => _buttonImagePath;
            set
            {
                _buttonImagePath = value;
                if (button != null)
                {
                    button.ImagePath = value;
                    if (ApplyThemeOnImage)
                    {
                        button.Theme = Theme;
                    }
                    _isControlinvalidated = true;
                    Invalidate(); // Repaint when the image changes
                                  // UpdateSize();
                  //  _isControlinvalidated = true;
                }
            }
        }
        private string _extendedbuttonImagePath;
        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load.")]
        public string ExtendButtonImagePath
        {
            get => _extendedbuttonImagePath;
            set
            {
                _extendedbuttonImagePath=value;
                if (extendButton != null)
                {
                 
                    extendButton.ImagePath = value;
                    if (ApplyThemeOnImage)
                    {
                        extendButton.Theme = Theme;
                        extendButton.ApplyThemeToSvg();
                        extendButton.ApplyTheme();
                    }
                    _isControlinvalidated = true;
                    Invalidate(); // Repaint when the image changes
                                  // UpdateSize();
                  //  _isControlinvalidated = true;
                }
            }
        }
        [Browsable(true)]
        [Category("Behavior")]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                if (_isSelected)
                {

                    BackColor = _currentTheme.ButtonActiveBackColor;
                    ForeColor = _currentTheme.ButtonActiveForeColor;
                }
                else
                {
                    BackColor = _currentTheme.ButtonBackColor;
                    ForeColor = _currentTheme.ButtonForeColor;
                }
                Invalidate(); // Repaint to reflect selection state
            }
        }
        private Font _textFont = new Font("Arial", 10);
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {

                _textFont = value;
                UseThemeFont = false;
                if (button != null)
                {


                  //  _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
                    button.TextFont = _textFont;

                }

                Invalidate();


            }
        }
        #region Events
        public event EventHandler<BeepEventDataArgs> ButtonClick;
        public event EventHandler<BeepEventDataArgs> ExtendButtonClick;
        #endregion
        private SimpleItem _menuItem;
        private bool _isSelected;

        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public SimpleItem MenuItem
        {
            get => _menuItem;
            set
            {
                _menuItem = value;
                if (button != null && value!=null)
                {
                    if (value.Text != null) { button.Text = value.Text; }
                    
                    if (!string.IsNullOrEmpty(value.ImagePath))
                    {
                        button.ImagePath = value.ImagePath;
                    }
                    
                }
                _isControlinvalidated= true;
                Invalidate();
            }
        }
        public BeepExtendedButton()
        {
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 100;
                Height = buttonHeight;
            }
          //  CreateMenuItemPanel();
            BoundProperty= "Text";
        }
        protected override void InitLayout()
        {
        //   // Console.WriteLine("Control InitLayout");
            base.InitLayout();
            UpdateDrawingRect();
            CreateMenuItemPanel();
            _isControlinvalidated = true;
        //   // Console.WriteLine("Control InitLayout 1");
            Invalidate();
        //   // Console.WriteLine("Control InitLayout 2");
        }
        private void CreateMenuItemPanel()
        {
            

            // Clear existing controls before re-creating the layout
            Controls.Clear();
            UpdateDrawingRect();
             DrawingRect.Inflate(-1, -1);
            buttonHeight = DrawingRect.Height ;
            // Main button setup
           //// Console.WriteLine("Control CreateMenuItemPanel");
            button = new BeepButton
            {
                Size = new Size(DrawingRect.Width - RightButtonSize -  4, buttonHeight),
                Location = new Point(startx, starty),
                
                MaxImageSize = new Size(RightButtonSize-2, RightButtonSize-2),
                Text = this.Text,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleLeft,
                IsBorderAffectedByTheme = false,
                Theme = Theme,
                ShowAllBorders = false,
                ShowShadow = false,
                IsFrameless = true,
                BorderSize = 0,
                IsRounded = false,
                UseScaledFont = true,
                ApplyThemeOnImage = ApplyThemeOnImage
            };
         //  // Console.WriteLine("Control CreateMenuItemPanel 1");
            if (MenuItem != null)
            {
                button.Text = MenuItem.Text;
                if (!string.IsNullOrEmpty(MenuItem.ImagePath))
                {
                    button.ImagePath = MenuItem.ImagePath;
                }
            }
          // // Console.WriteLine("Control CreateMenuItemPanel 2");
            // Extend button setup
            extendButton = new BeepButton
            {
                HideText = true,
                Size = new Size(RightButtonSize , buttonHeight),
                Location = new Point(DrawingRect.Width-RightButtonSize , starty),
               
                MaxImageSize = new Size(RightButtonSize - 2, RightButtonSize-2),
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                IsFrameless = true,
                Theme = Theme,
                ShowAllBorders = false,
                ShowShadow = false,
                IsRounded = false,
                IsBorderAffectedByTheme = false,
                BorderSize = 0,
                  
                ApplyThemeOnImage = ApplyThemeOnImage
            };
          // // Console.WriteLine("Control CreateMenuItemPanel 3");
            if (!string.IsNullOrEmpty(_buttonImagePath))
            {
                button.ImagePath = _buttonImagePath;
            }
          // // Console.WriteLine("Control CreateMenuItemPanel 4");
            if (!string.IsNullOrEmpty(_extendedbuttonImagePath))
            {
                extendButton.ImagePath = _extendedbuttonImagePath;
            }
          // // Console.WriteLine("Control CreateMenuItemPanel 5");
            // Add highlight panel, main button, and extend button to the panel
            Controls.Add(highlightPanel);
            Controls.Add(button);
            Controls.Add(extendButton);

            // Event handlers
            //button.MouseEnter += (s, e) => { highlightPanel.BackColor = _currentTheme.ButtonHoverBackColor; };
            //button.MouseLeave += (s, e) => { highlightPanel.BackColor = _currentTheme.BackColor; };
            //button.MouseHover += (s, e) => { highlightPanel.BackColor = _currentTheme.ButtonHoverBackColor; };
            button.Click += MenuItemButton_Click;
            extendButton.Click += ExtendButton_Click;
          // // Console.WriteLine("Control CreateMenuItemPanel 6");
            // Add the panel to the control

            rearrangeControls();
           //// Console.WriteLine("Control CreateMenuItemPanel 7");
            _isControlinvalidated = false;
        }


        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    base.OnPaint(e);
        //   //// Console.WriteLine("Control Invalidated 1 ");
        //    if (_isControlinvalidated)
        //    {
        //       //// Console.WriteLine("Control Invalidated 2");
        //        Controls.Clear();
        //        CreateMenuItemPanel();
        //        _isControlinvalidated = false;
        //    }

        //}
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
         
            _isControlinvalidated = true;
            rearrangeControls();
            Invalidate();
        }
        private void rearrangeControls()
        {
            UpdateDrawingRect();
           // DrawingRect.Inflate(-1, -1);
            if (button != null && extendButton != null)
            {
                buttonHeight = DrawingRect.Height ;
                // We want a small gap (like 2px) between the two buttons:
                int gapBetweenButtons = 1;

                // Extended button has a known width:
                int extButtonWidth = RightButtonSize;   // your chosen formula
                                                  // The main button gets whatever is left in the DrawingRect minus the gap.
                int mainButtonWidth = DrawingRect.Width - extButtonWidth
                                      - (startx * 2)  // left & right padding if needed
                                      - gapBetweenButtons;

                // Make sure mainButtonWidth doesn’t become negative in small resizes
                if (mainButtonWidth < 0) mainButtonWidth = 0;

                // Now set sizes:
                button.Size = new Size(mainButtonWidth, buttonHeight);
                button.Location = new Point(startx, starty);

                extendButton.Size = new Size(extButtonWidth, buttonHeight);

                // Place the extend button to the right of the main button with the gap:
                extendButton.Location = new Point(DrawingRect.Width - RightButtonSize, starty);
            }

        }

        private void ExtendButton_Click(object? sender, EventArgs e)
        {
            ExtendButtonClick?.Invoke(this, new BeepEventDataArgs("ExentededButtonClick", MenuItem));
        }

        private void MenuItemButton_Click(object? sender, EventArgs e)
        {
            ButtonClick?.Invoke(this, new BeepEventDataArgs("ButtonClick", MenuItem));

        }
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Enable anti-aliasing for smooth rendering
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Fill the background with the theme's button background color
            using (SolidBrush backgroundBrush = new SolidBrush(_currentTheme.ButtonBackColor))
            {
                graphics.FillRectangle(backgroundBrush, rectangle);
            }

            // Draw the highlight panel **before** drawing the buttons (only if applicable)
            if (highlightPanel != null && highlightPanel.Visible)
            {
                Rectangle highlightRect = new Rectangle(button.Left, button.Top, button.Width, button.Height);
                using (SolidBrush highlightBrush = new SolidBrush(_currentTheme.ButtonHoverBackColor))
                {
                    graphics.FillRectangle(highlightBrush, highlightRect);
                }
            }

            // Draw the border if needed
            if (BorderThickness > 0)
            {
                using (Pen borderPen = new Pen(_currentTheme.BorderColor, BorderThickness))
                {
                    graphics.DrawRectangle(borderPen, rectangle);
                }
            }

            // Define rectangles for child components
            Rectangle buttonRect = new Rectangle(button.Left, button.Top, button.Width, button.Height);
            Rectangle extendButtonRect = new Rectangle(extendButton.Left, extendButton.Top, extendButton.Width, extendButton.Height);

            // Draw the main button
            if (button != null)
            {
                button.Draw(graphics, buttonRect);
            }

            // Draw the extend button
            if (extendButton != null)
            {
                extendButton.Draw(graphics, extendButtonRect);
            }
        }

        #region "Theme"
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _textFont = Font;
            // // Console.WriteLine("Font Changed");
            if (AutoSize)
            {
                Size textSize = TextRenderer.MeasureText(Text, _textFont);
                this.Size = new Size(textSize.Width + Padding.Horizontal, textSize.Height + Padding.Vertical);
            }
        }
        public override void ApplyTheme()
        {
            if (button == null) return;

            BackColor = _currentTheme.ButtonBackColor;
            ForeColor = _currentTheme.ButtonForeColor;
            HoverBackColor = _currentTheme.ButtonHoverBackColor;
            HoverForeColor = _currentTheme.ButtonHoverForeColor;
            ActiveBackColor = _currentTheme.ButtonActiveBackColor;
            button.Theme = Theme;
            if (UseThemeFont)
            {
                button.UseThemeFont = true;
                _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
                button.Font = _textFont;
            }else
            {
                button.Font = _textFont;
            }
            if (IsChild)
            {
                BackColor = _currentTheme.ButtonBackColor;
                extendButton.BackColor = _currentTheme.ButtonBackColor; 
                button.BackColor = _currentTheme.ButtonBackColor;

            }
            button.IsChild = IsChild;
            extendButton.Theme = Theme;
            extendButton.IsChild = IsChild;
            Font = _textFont;

            ApplyThemeToSvg();
            Invalidate();  // Trigger repaint
        }
        public void ApplyThemeToSvg()
        {

            if (extendButton != null) // Safely apply theme to beepImage
            {
                extendButton.ApplyThemeOnImage = ApplyThemeOnImage;
                if (ApplyThemeOnImage)
                {

                    extendButton.ImageEmbededin = ImageEmbededin.Button;
                    extendButton.Theme = Theme;
                    extendButton.ApplyThemeToSvg();
                }

            }


        }
        #endregion "Theme"
    }
}
