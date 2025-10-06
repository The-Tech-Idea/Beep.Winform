using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.Base;
 
using TheTechIdea.Beep.Winform.Controls.Models;
using ContentAlignment = System.Drawing.ContentAlignment;
using TextImageRelation = System.Windows.Forms.TextImageRelation;




namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Extended Button")]
    [Category("Beep Controls")]
    [Description("A control that displays a button with an extended button.")]
    public class BeepExtendedButton: BaseControl
    {
        // Drawing components (not child controls)
        private BeepButton _mainButton;
        private BeepButton _extendButton;
        
        // Layout rectangles
        private Rectangle _mainButtonRect;
        private Rectangle _extendButtonRect;
        
        // Layout parameters
        private int buttonWidth = 200;
        private int extendbuttonWidth = 22;
        private int buttonHeight = 30;
        private int starty = 1;
        private int startx = 1;
        
        private Size _imagesize = new Size(20, 20);
        bool _applyThemeOnImage = false;

        public TextImageRelation TextImageRelation
        {
            get => _mainButton?.TextImageRelation ?? TextImageRelation.ImageBeforeText;
            set
            {
                if (_mainButton != null) _mainButton.TextImageRelation = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public ContentAlignment ImageAlign
        {
            get => _mainButton?.ImageAlign ?? ContentAlignment.MiddleLeft;
            set
            {
                if (_mainButton != null) _mainButton.ImageAlign = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public ContentAlignment TextAlign
        {
            get => _mainButton?.TextAlign ?? ContentAlignment.MiddleCenter;
            set
            {
                if (_mainButton != null)  _mainButton.TextAlign = value;
                Invalidate();
            }
        }


        [Browsable(true)]
        [Category("Appearance")]
        [Description("Change Value of main Button inside control")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ButtonWidth
        {
            get => buttonWidth;
            set
            {
                buttonWidth = value;
                UpdateDrawingRect();
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public Size MaxImageSize
        {
            get => _imagesize;
            set
            {
                _imagesize = value;
                if(_mainButton != null)
                {
                    _mainButton.MaxImageSize = value;
                }
                if(_extendButton != null)
                {
                    _extendButton.MaxImageSize = value;
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
                if(value > 0)
                {
                    extendbuttonWidth = value;
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
                if (_mainButton != null)
                {
                    _mainButton.ApplyThemeOnImage = value;
                }
                if (_extendButton != null)
                {
                    _extendButton.ApplyThemeOnImage = value;
                }
                if (ApplyThemeOnImage)
                {
                    if (_extendButton != null) _extendButton.Theme = Theme;
                    if (_mainButton != null) _mainButton.Theme = Theme;
                }
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool UseScaledFont
        {
            get => _mainButton?.UseScaledFont ?? false;
            set
            {
                if(_mainButton != null) _mainButton.UseScaledFont = value;
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
                if (_mainButton != null)
                {
                    _mainButton.ImagePath = value;
                    if (ApplyThemeOnImage)
                    {
                        _mainButton.Theme = Theme;
                    }
                    Invalidate(); // Repaint when the image changes
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
                if (_extendButton != null)
                {
                    _extendButton.ImagePath = value;
                    if (ApplyThemeOnImage)
                    {
                        _extendButton.Theme = Theme;
                        _extendButton.ApplyThemeToSvg();
                        _extendButton.ApplyTheme();
                    }
                    Invalidate(); // Repaint when the image changes
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

                    BackColor = _currentTheme.ButtonSelectedBackColor;
                    ForeColor = _currentTheme.ButtonSelectedForeColor;
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
                if (_mainButton != null)
                {
                    _mainButton.TextFont = _textFont;
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
                if (_mainButton != null && value != null)
                {
                    if (value.Text != null) { _mainButton.Text = value.Text; }
                    
                    if (!string.IsNullOrEmpty(value.ImagePath))
                    {
                        _mainButton.ImagePath = value.ImagePath;
                    }
                }
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
            BorderRadius = 3;
            BoundProperty = "Text";
            
            // Configure the main control to be borderless like BeepAppBar
            IsBorderAffectedByTheme = false;
            IsShadowAffectedByTheme = false;
            IsRoundedAffectedByTheme = false;
            ShowAllBorders = false;
            ShowShadow = false;
            IsFrameless = false; // Keep false so we can have our own border if needed
            IsRounded = false;
            ApplyThemeToChilds = false;
            
            // Initialize drawing components (not child controls)
            InitializeDrawingComponents();
        }
        
        protected override Size DefaultSize => new Size(150, 36);
        
        protected override void InitLayout()
        {
            base.InitLayout();
            InitializeDrawingComponents(); // Ensure components are initialized
            SetupHitAreas(); // Setup hit areas for interaction
            Invalidate();
        }
        private void InitializeDrawingComponents()
        {
            // Initialize main button (drawing component only, not added to Controls)
            _mainButton = new BeepButton
            {
                MaxImageSize = new Size(RightButtonSize - 2, RightButtonSize - 2),
                Text = this.Text,
                TextImageRelation =  System.Windows.Forms.TextImageRelation.ImageAboveText,
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
                ApplyThemeOnImage = ApplyThemeOnImage,
                IsChild = true,
                // Clear all borders explicitly
                ShowTopBorder = false,
                ShowBottomBorder = false,
                ShowLeftBorder = false,
                ShowRightBorder = false,
                BorderThickness = 0
            };

            // Initialize extend button (drawing component only, not added to Controls)
            _extendButton = new BeepButton
            {
                HideText = true,
                MaxImageSize = new Size(RightButtonSize - 2, RightButtonSize - 2),
                TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                IsFrameless = true,
                Theme = Theme,
                ShowAllBorders = false,
                ShowShadow = false,
                IsRounded = false,
                IsBorderAffectedByTheme = false,
                BorderSize = 0,
                ApplyThemeOnImage = ApplyThemeOnImage,
                IsChild = true,
                // Clear all borders explicitly
                ShowTopBorder = false,
                ShowBottomBorder = false,
                ShowLeftBorder = false,
                ShowRightBorder = false,
                BorderThickness = 0
            };

            // Apply stored image paths if they exist
            if (!string.IsNullOrEmpty(_buttonImagePath))
            {
                _mainButton.ImagePath = _buttonImagePath;
            }

            if (!string.IsNullOrEmpty(_extendedbuttonImagePath))
            {
                _extendButton.ImagePath = _extendedbuttonImagePath;
            }

            // Apply MenuItem if it exists
            if (_menuItem != null)
            {
                _mainButton.Text = _menuItem.Text;
                if (!string.IsNullOrEmpty(_menuItem.ImagePath))
                {
                    _mainButton.ImagePath = _menuItem.ImagePath;
                }
            }
        }

        private void CalculateLayout(out Rectangle mainButtonRect, out Rectangle extendButtonRect)
        {
            UpdateDrawingRect();
            Rectangle drawingRect = this.DrawingRect;
            
            // Use DrawingRect dimensions instead of control dimensions
            int availableHeight = drawingRect.Height;
            int availableWidth = drawingRect.Width;
            
            int gapBetweenButtons = 1;
            int extButtonWidth = RightButtonSize;
            int mainButtonWidth = availableWidth - extButtonWidth - gapBetweenButtons;
            
            // Make sure mainButtonWidth doesn't become negative
            if (mainButtonWidth < 0) mainButtonWidth = 0;

            // Position rectangles relative to DrawingRect, not control bounds
            mainButtonRect = new Rectangle(
                drawingRect.X, 
                drawingRect.Y, 
                mainButtonWidth, 
                availableHeight);
                
            extendButtonRect = new Rectangle(
                drawingRect.X + mainButtonWidth + gapBetweenButtons, 
                drawingRect.Y, 
                extButtonWidth, 
                availableHeight);
            
            // Store rectangles for use in other methods
            _mainButtonRect = mainButtonRect;
            _extendButtonRect = extendButtonRect;
        }

        private void SetupHitAreas()
        {
            // Clear existing hit areas
            ClearHitList();

            // Calculate current layout
            CalculateLayout(out Rectangle mainButtonRect, out Rectangle extendButtonRect);

            // Add hit area for main button
            AddHitArea("MainButton", mainButtonRect, _mainButton, HandleMainButtonClick);

            // Add hit area for extend button  
            AddHitArea("ExtendButton", extendButtonRect, _extendButton, HandleExtendButtonClick);
        }

        private void HandleMainButtonClick()
        {
            ButtonClick?.Invoke(this, new BeepEventDataArgs("ButtonClick", MenuItem));
        }

        private void HandleExtendButtonClick()
        {
            ExtendButtonClick?.Invoke(this, new BeepEventDataArgs("ExentededButtonClick", MenuItem));
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetupHitAreas(); // Recalculate hit areas on resize
            Invalidate();
        }
        private void ExtendButton_Click(object? sender, EventArgs e)
        {
            ExtendButtonClick?.Invoke(this, new BeepEventDataArgs("ExentededButtonClick", MenuItem));
        }

        private void MenuItemButton_Click(object? sender, EventArgs e)
        {
            ButtonClick?.Invoke(this, new BeepEventDataArgs("ButtonClick", MenuItem));

        }
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            
            // Calculate layout for current drawing
            CalculateLayout(out Rectangle mainButtonRect, out Rectangle extendButtonRect);
            
            // Draw components using Graphics
            DrawComponents(g, mainButtonRect, extendButtonRect);
        }

        private void DrawComponents(Graphics g, Rectangle mainButtonRect, Rectangle extendButtonRect)
        {
            // Enable high-quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Draw the main button if it exists
            if (_mainButton != null && mainButtonRect.Width > 0 && mainButtonRect.Height > 0)
            {
                _mainButton.Draw(g, mainButtonRect);
            }

            // Draw the extend button if it exists
            if (_extendButton != null && extendButtonRect.Width > 0 && extendButtonRect.Height > 0)
            {
                _extendButton.Draw(g, extendButtonRect);
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
            if (_mainButton == null) return;

            BackColor = _currentTheme.ButtonBackColor;
            ForeColor = _currentTheme.ButtonForeColor;
            HoverBackColor = _currentTheme.ButtonHoverBackColor;
            HoverForeColor = _currentTheme.ButtonHoverForeColor;
            SelectedForeColor = _currentTheme.ButtonSelectedForeColor;
            
            // Apply theme to drawing components
            _mainButton.Theme = Theme;
            _extendButton.Theme = Theme;
            
            if (UseThemeFont)
            {
                _mainButton.UseThemeFont = true;
                _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
                _mainButton.Font = _textFont;
            }
            else
            {
                _mainButton.Font = _textFont;
            }
            
            if (IsChild)
            {
                BackColor = _currentTheme.ButtonBackColor;
                _extendButton.BackColor = _currentTheme.ButtonBackColor; 
                _mainButton.BackColor = _currentTheme.ButtonBackColor;
            }
            
            _mainButton.IsChild = IsChild;
            _extendButton.IsChild = IsChild;
            Font = _textFont;

            ApplyThemeToSvg();
            Invalidate();  // Trigger repaint
        }
        
        public void ApplyThemeToSvg()
        {
            if (_extendButton != null) // Safely apply theme to extend button
            {
                _extendButton.ApplyThemeOnImage = ApplyThemeOnImage;
                if (ApplyThemeOnImage)
                {
                    _extendButton.ImageEmbededin = ImageEmbededin.Button;
                    _extendButton.Theme = Theme;
                    _extendButton.ApplyThemeToSvg();
                }
            }
            
            if (_mainButton != null) // Also apply to main button
            {
                _mainButton.ApplyThemeOnImage = ApplyThemeOnImage;
                if (ApplyThemeOnImage)
                {
                    _mainButton.ImageEmbededin = ImageEmbededin.Button;
                    _mainButton.Theme = Theme;
                    _mainButton.ApplyThemeToSvg();
                }
            }
        }
        #endregion "Theme"
    }
}
