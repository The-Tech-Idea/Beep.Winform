using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Template;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDropDownMenu : BeepListBox
    {
        private BeepButton _dropDownButton;
        private bool _isExpanded = false;
        private readonly int _maxMenuHeight = 200; // Maximum dropdown height
        private readonly int _collapsedHeight = 25; // Fixed height when not expanded
        private System.Windows.Forms.Timer _animationTimer;
        private int _targetHeight;

        public BeepDropDownMenu()
        {
            _isControlinvalidated = true;
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 200;
                Height = _collapsedHeight;
            }
            ShowTitleLine = true;
            UpdateDrawingRect();
         
            Height = _collapsedHeight; // Initially collapsed to the size of the button
            InitDropDownMenu();
        }

        /// <summary>
        /// Initialize the dropdown menu components.
        /// </summary>
        private void InitDropDownMenu()
        {
            ShowTitleLine = true;
            if (_currentTheme == null)
            {
                // Initialize a default theme to avoid null reference issues
                _currentTheme = BeepThemesManager.DefaultTheme;
            }
            if (_dropDownButton != null)
            {
                Controls.Remove(_dropDownButton);
            }

            // Create the dropdown button
            _dropDownButton = new BeepButton
            {
                BackColor = _currentTheme.ButtonBackColor,
                ForeColor = _currentTheme.ButtonForeColor,
                Size = new Size(30, DrawingRect.Height),
                MaxImageSize = new Size(30, DrawingRect.Height),
                // Center the button vertically and align to the right side
                Location = new Point(DrawingRect.Width - 30, DrawingRect.Top+2),
               // ApplyThemeOnImage = true,
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.Overlay,
                ShowAllBorders = false,
                ShowShadow = false,
                IsFramless = true,
                IsChild = true
            };
            
            if (_isExpanded)
            {
                _dropDownButton.ImagePath= "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-right.svg";
            }else
            {
                _dropDownButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-down.svg";
            }
            _dropDownButton.Theme = Theme;

            // Add click event to toggle menu visibility
            _dropDownButton.Click += ToggleMenu;

            // Add the dropdown button to the controls
            Controls.Add(_dropDownButton);
            _isControlinvalidated = false;
        }

        /// <summary>
        /// Toggle the dropdown menu visibility.
        /// </summary>
        private void ToggleMenu(object sender, EventArgs e)
        {
            if (!_isExpanded)
            {
                if (ListItems == null || !ListItems.Any())
                {
                    MessageBox.Show("No items available.");
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
        /// Show the dropdown menu with animation.
        /// </summary>
        private void ShowMenu()
        {
            _isExpanded = true;

            // Calculate target height based on items and maximum menu height
            _targetHeight = Math.Min(_maxMenuHeight, ListItems.Count * 40) + _dropDownButton.Height;

            StartAnimation();
        }

        /// <summary>
        /// Hide the dropdown menu with animation.
        /// </summary>
        private void HideMenu()
        {
            _isExpanded = false;

            // Target height when collapsed
            _targetHeight = _dropDownButton.Height;

            StartAnimation();
        }

        /// <summary>
        /// Start the animation for expanding or collapsing the menu.
        /// </summary>
        private void StartAnimation()
        {
            _animationTimer?.Stop();
            _animationTimer = new System.Windows.Forms.Timer { Interval = 15 };
            _animationTimer.Tick += AnimateMenu;
            _animationTimer.Start();
        }

        /// <summary>
        /// Animate the dropdown menu height.
        /// </summary>
        private void AnimateMenu(object sender, EventArgs e)
        {
            int increment = 10; // Height increment per tick

            if (_isExpanded)
            {
                if (Height < _targetHeight)
                {
                    Height = Math.Min(Height + increment, _targetHeight);
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
                    Height = Math.Max(Height - increment, _targetHeight);
                }
                else
                {
                    Height = _targetHeight;
                    _animationTimer.Stop();
                }
            }
        }

        /// <summary>
        /// Ensure the dropdown button is positioned correctly and apply fixed height at runtime or design time.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!DesignMode)
            {
                if (!_isExpanded)
                {
                    Height = _collapsedHeight;
                }
            }
            else
            {
                Height = _collapsedHeight;
            }
            if (_dropDownButton != null)
            {
                _dropDownButton.Location = new Point(DrawingRect.Width - 30, DrawingRect.Top+2);
            }
        }

        /// <summary>
        /// Ensure the dropdown button is initialized and the menu is updated.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //  
            if (_isControlinvalidated)
            {
                InitDropDownMenu();
                _isControlinvalidated = false;
            }
                
        }

        /// <summary>
        /// Override InitializeMenu to adjust menu items' positions relative to the dropdown button.
        /// </summary>
        public override void InitializeMenu()
        {
            if (ListItems == null) return;

            base.InitializeMenu();

            // Adjust position of menu items to start below the dropdown button
            //foreach (Control control in Controls.OfType<Panel>().Where(c => c.Tag is SimpleMenuItem))
            //{
            //    control.Top += _dropDownButton.Height;
            //}
        }

        /// <summary>
        /// Handle item selection and update dropdown button text.
        /// </summary>
        protected override void MenuItemButton_Click(object sender, EventArgs e)
        {
            base.MenuItemButton_Click(sender, e);

            if (sender is BeepButton clickedButton && clickedButton.Tag is SimpleMenuItem selectedItem)
            {
                _dropDownButton.Text = selectedItem.Text; // Update dropdown button text
                HideMenu(); // Collapse the menu
            }
        }
    }
}
