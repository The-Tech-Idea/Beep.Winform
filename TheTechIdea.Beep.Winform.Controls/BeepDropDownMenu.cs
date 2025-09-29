using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Dropdown Menu")]
    [Category("Beep Controls")]
    [Description("A dropdown menu control that displays a list of items.")]
    public class BeepDropDownMenu : BeepListBox
    {
        // Remove the physical button control
        // private BeepButton _dropDownButton;

        // Keep a button instance just for drawing
        private BeepButton _dropDownButtonGDI;

        private bool _isExpanded = false;
        private int _maxMenuHeight = 200; // Increased maximum height for better visibility
        private int _collapsedHeight = 30; // Fixed height when not expanded
        private System.Windows.Forms.Timer _animationTimer;
        private int _targetHeight;
        private const int AnimationInterval = 15; // Animation speed
        private const int HeightIncrement = 10; // Height change per tick
        bool _isAnimating = false;
        private int _buttonWidth = 25;
        private int _padding = 2;

        public override string ToString()
        {
            return "Beep DropDown";
        }

        public BeepDropDownMenu()
        {
            InitializeControl();
            Text = "DropDown/Combo";
        }

        /// <summary>
        /// Initializes the dropdown menu control.
        /// </summary>
        private void InitializeControl()
        {
            
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 150;
                Height = _collapsedHeight;
            }
            ShowTitleLine = false;
            UpdateDrawingRect();

            // Initialize the GDI button for drawing
            _dropDownButtonGDI = new BeepButton
            {
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageAboveText,
                HideText = true,
                ShowAllBorders = false,
                ShowShadow = false,
                IsChild = true,
                IsBorderAffectedByTheme = false,
                IsRoundedAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                Theme = Theme,
                MaxImageSize = new Size(_buttonWidth - 2, _buttonWidth - 2)
            };

            // Set initial image based on expansion state
            SetDropDownButtonImage();

            // Add a hit area for the dropdown button
            AddDropDownButtonHitArea();
        }

        /// <summary>
        /// Sets the dropdown button's image based on the expansion state.
        /// </summary>
        private void SetDropDownButtonImage()
        {
            if (_isExpanded)
            {
                _dropDownButtonGDI.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-up.svg";
            }
            else
            {
                _dropDownButtonGDI.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-down.svg";
            }
        }

        /// <summary>
        /// Add a hit area for the dropdown button
        /// </summary>
        private void AddDropDownButtonHitArea()
        {
            // Calculate the button's rectangle
            Rectangle buttonRect = new Rectangle(
                DrawingRect.Right - _buttonWidth - _padding,
                2,
                _buttonWidth,
                _buttonWidth
            );

            // Add hit area for the button
            AddHitArea(
                "DropDownButton",
                buttonRect,
                null,
                () => ToggleMenu()
            );
        }

        /// <summary>
        /// Toggles the dropdown menu's visibility.
        /// </summary>
        private void ToggleMenu()
        {
            if (!_isExpanded)
            {
                if (ListItems == null || !ListItems.Any())
                {
                    MessageBox.Show("No items available to display.", "Information");
                    return;
                }

                ShowMenu();
            }
            else
            {
                HideMenu();
            }
        }

        /// <summary>
        /// Expands the dropdown menu with animation.
        /// </summary>
        private void ShowMenu()
        {
            _isExpanded = true;
            UpdateDrawingRect();
            _targetHeight = Math.Min(_maxMenuHeight, TitleBottomY + GetMaxHeight() + 10);
            SetDropDownButtonImage();
            StartAnimation();
        }

        /// <summary>
        /// Collapses the dropdown menu with animation.
        /// </summary>
        private void HideMenu()
        {
            _isExpanded = false;
            SetDropDownButtonImage();
            _targetHeight = _collapsedHeight;
            StartAnimation();
        }

        /// <summary>
        /// Starts the animation for expanding or collapsing the menu.
        /// </summary>
        private void StartAnimation()
        {
            _animationTimer?.Stop();
            _isAnimating = true;
            _animationTimer = new System.Windows.Forms.Timer { Interval = AnimationInterval };
            _animationTimer.Tick += AnimateMenu;
            _animationTimer.Start();
        }

        /// <summary>
        /// Animates the dropdown menu's height.
        /// </summary>
        private void AnimateMenu(object sender, EventArgs e)
        {
            if (_isExpanded)
            {
                if (Height < _targetHeight)
                {
                    Height = Math.Min(Height + HeightIncrement, _targetHeight);
                }
                else
                {
                    Height = _targetHeight;
                    _animationTimer.Stop();
                    _isAnimating = false;
                }
            }
            else
            {
                if (Height > _targetHeight)
                {
                    Height = Math.Max(Height - HeightIncrement, _targetHeight);
                }
                else
                {
                    Height = _targetHeight;
                    _animationTimer.Stop();
                    _isAnimating = false;
                }
            }

            Invalidate();
        }

        /// <summary>
        /// Updates the drawing rectangles and hit areas when resized
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Update DrawingRect if necessary
            UpdateDrawingRect();

            // Clear hit areas and recreate the dropdown button hit area
            ClearHitList();
            AddDropDownButtonHitArea();

            // Adjust the height of the control if it's not expanded
            if (!_isExpanded && !_isAnimating)
            {
                this.Height = _collapsedHeight;
            }
        }

        /// <summary>
        /// Draw the content including the dropdown button
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            // Draw the base control content first
            base.DrawContent(g);

            // Calculate the dropdown button rectangle
            Rectangle buttonRect = new Rectangle(
                DrawingRect.Right - _buttonWidth - _padding,
                2,
                _buttonWidth,
                _buttonWidth
            );

            // Draw the dropdown button using the BeepButton instance
            _dropDownButtonGDI.Draw(g, buttonRect);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            IsHovered = false;
            base.OnMouseHover(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            IsHovered = false;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            IsHovered = false;
            base.OnMouseMove(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            IsHovered = false;
            base.OnMouseEnter(e);
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            // Apply theme to the GDI button
            if (_dropDownButtonGDI != null)
            {
                _dropDownButtonGDI.Theme = Theme;
            }

            Invalidate();
        }
    }
}
