using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDropDownMenu : BeepListBox
    {
        private BeepButton _dropDownButton;

        private bool _isExpanded = false;
        private  int _maxMenuHeight = 200; // Increased maximum height for better visibility
        private  int _collapsedHeight = 30; // Fixed height when not expanded
        private System.Windows.Forms.Timer _animationTimer;
        private int _targetHeight;
        private const int AnimationInterval = 15; // Animation speed
        private const int HeightIncrement = 10; // Height change per tick
        bool _isAnimating = false;
        private int _buttonWidth=25;
        private int _padding = 2;
        public override string ToString()
        {
            return "Beep DropDown";
        }

        public BeepDropDownMenu()
        {
            InitializeControl();
            InitializeDropDownButton();
            Text= "DropDown/Combo";

        }

        /// <summary>
        /// Initializes the dropdown menu control.
        /// </summary>
        private void InitializeControl()
        {
            _isControlinvalidated = true;
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 150;
                Height = _collapsedHeight;
            }
            ShowTitleLine = false;
            UpdateDrawingRect();
            
            InitDropDownMenu();
        }

        /// <summary>
        /// Initializes the dropdown button.
        /// </summary>
        private void InitializeDropDownButton()
        {
            if (_dropDownButton != null)
            {
                Controls.Remove(_dropDownButton);
            }

            // Create the dropdown button
            _dropDownButton = new BeepButton
            {
                Size = new Size(_buttonWidth, TitleBottomY),
                Location = new Point(DrawingRect.Width - _buttonWidth,  2), // Align to the right
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageAboveText,
                HideText=true,
                ShowAllBorders = false,
                ShowShadow = false,
                IsChild = true,
                IsBorderAffectedByTheme = false,
                IsRoundedAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                Theme = Theme
            };

            // Set initial image based on expansion state
            SetDropDownButtonImage();

            // Add click event to toggle menu visibility
            _dropDownButton.Click += ToggleMenu;

            // Add the dropdown button to the controls
            Controls.Add(_dropDownButton);
            _dropDownButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            _isControlinvalidated = false;
        }

        /// <summary>
        /// Sets the dropdown button's image based on the expansion state.
        /// </summary>
        private void SetDropDownButtonImage()
        {
            if (_isExpanded)
            {
                _dropDownButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-up.svg";
            }
            else
            {
                _dropDownButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-down.svg";
            }
        }

        /// <summary>
        /// Toggles the dropdown menu's visibility.
        /// </summary>
        private void ToggleMenu(object sender, EventArgs e)
        {
            SetDropDownButtonImage();
            if (!_isExpanded)
            {
                if (ListItems == null || !ListItems.Any())
                {
                    MessageBox.Show("Information", "No items available to display.");
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
            _targetHeight = Math.Min(_maxMenuHeight,TitleBottomY+ GetMaxHeight()+10) ; // Adjusted item height
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
            _isAnimating=true;
            _animationTimer = new System.Windows.Forms.Timer { Interval = AnimationInterval };
            _animationTimer.Tick += AnimateMenu;
            _animationTimer.Start();
            _isAnimating= false;
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
                }
            }
        }

        /// <summary>
        /// Ensures the dropdown button is positioned correctly and applies fixed height.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_dropDownButton == null) return;

            // Update DrawingRect if necessary
            UpdateDrawingRect();
            //// Define right padding (adjust as needed)
            //int rightPadding = 2;

            //// Calculate the Y position to center the button vertically
            //int dropdownButtonHeight = TriangleButton.Height - 4;
            //int centerY = (this.Height - dropdownButtonHeight) / 2;

            //// Calculate the X position based on the control's client width
            //// Ensuring the button stays within the client area
            //int xPos = this.ClientSize.Width - _buttonsize - rightPadding;

            //// Set the button's location and size
            //TriangleButton.Location = new Point(xPos, centerY);
            //TriangleButton.Size = new Size(_buttonsize, dropdownButtonHeight);
            if (_dropDownButton != null)
            {
                int dropdownButtonHeight = TitleBottomY - 4;
                int centerY =  2;
                _dropDownButton.Left = DrawingRect.Right - _buttonWidth - _padding;
                _dropDownButton.Top = centerY;
                _dropDownButton.Width = _buttonWidth;
                _dropDownButton.Height = _buttonWidth;
                _dropDownButton.MaxImageSize = new Size(_dropDownButton.Width - 2, _dropDownButton.Height - 2);
                Console.WriteLine($"TitlelineY: {TitleBottomY}");
            }
            // Adjust the height of the control if it's not expanded
            if (!_isExpanded)
            {
                this.Height = _collapsedHeight;
                UpdateDrawingRect();
            }
        }

        /// <summary>
        /// Ensures the dropdown button is initialized and the menu is updated.
        /// </summary>
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    base.OnPaint(e);
        //    if (_isControlinvalidated)
        //    {
        //        InitDropDownMenu();
        //        _isControlinvalidated = false;
        //    }
        //}

        /// <summary>
        /// Initializes the dropdown menu.
        /// </summary>
        private void InitDropDownMenu()
        {
           // ShowTitleLine = true;
            if (_currentTheme == null)
            {
                // Initialize a default theme to avoid null reference issues
                _currentTheme = BeepThemesManager.DefaultTheme;
            }
            InitializeDropDownButton();
        }

        /// <summary>
        /// Handles item selection and updates the dropdown button's text.
        /// </summary>
        protected override void MenuItemButton_Click(object sender, EventArgs e)
        {
            base.MenuItemButton_Click(sender, e);

            if (sender is BeepButton clickedButton && clickedButton.Tag is SimpleItem selectedItem)
            {
                _dropDownButton.Text = selectedItem.Text; // Update dropdown button text
                HideMenu(); // Collapse the menu
            }
        }

        /// <summary>
        /// Ensures the dropdown menu is initialized when the menu is created.
        /// </summary>
        public override void InitializeMenu()
        {
            if (ListItems == null) return;

            base.InitializeMenu();

            // Adjust position of menu items if necessary
            // For example, offsetting items based on the dropdown button's height
        }
        protected override void OnMouseHover(EventArgs e)
        {
            IsHovered = false;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            IsHovered = false;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            IsHovered = false;
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            IsHovered = false;
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_dropDownButton != null) _dropDownButton.Theme = Theme;
        }
    }
}
