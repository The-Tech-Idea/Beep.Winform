using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Template;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepFlyMenu:BeepListBox
    {
        private BeepButton _dropDownButton;
        private bool _isExpanded = false;
        private readonly int _maxMenuHeight = 200; // Maximum dropdown height
        private readonly int _collapsedHeight = 30; // Fixed height when not expanded
        public BeepFlyMenu()
        {
            InitDropDownMenu();
            UpdateDrawingRect();
            Height = _collapsedHeight; // Initially collapsed to the size of the button
        }

        /// <summary>
        /// Initialize the dropdown menu components.
        /// </summary>
        private void InitDropDownMenu()
        {
            if (_currentTheme == null)
            {
                // Initialize a default theme to avoid null reference issues
                _currentTheme = BeepThemesManager.DefaultTheme;
            }
            if(_dropDownButton!=null)
            {
                Controls.Remove(_dropDownButton);
            }
            // Create the dropdown button
            _dropDownButton = new BeepButton
            {

                // put dropdown symbole 
                Text = "▼",
                BackColor = _currentTheme.ButtonBackColor,
                ForeColor = _currentTheme.ButtonForeColor,
                Height = _collapsedHeight-2,
                Size = new System.Drawing.Size(30, _collapsedHeight-2),
                Location = new System.Drawing.Point(DrawingRect.Width-30, DrawingRect.Top),
                TextAlign = ContentAlignment.MiddleLeft,
                ImageAlign = ContentAlignment.MiddleRight,
                ShowAllBorders = false,
                ShowShadow = false,
                IsChild = true

            };
            _dropDownButton.Theme = Theme;
            // Add click event to toggle menu visibility
            _dropDownButton.Click += ToggleMenu;

            // Set initial size and adjust DrawingRect
          
            Height = _dropDownButton.Height; // Initially collapsed

            // Add the dropdown button to the controls
            Controls.Add(_dropDownButton);
        }

        /// <summary>
        /// Toggle the visibility of the dropdown menu.
        /// </summary>
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
        /// Show the dropdown menu.
        /// </summary>
        private void ShowMenu()
        {
            _isExpanded = true;

            var targetHeight = Math.Min(200, ListItems.Count * 40);

            var timer = new Timer { Interval = 15 };
            timer.Tick += (s, e) =>
            {
                if (this.Height < targetHeight + _dropDownButton.Height)
                {
                    this.Height += 10;
                }
                else
                {
                    this.Height = targetHeight + _dropDownButton.Height;
                    timer.Stop();
                }
            };

            timer.Start();
        }
        /// <summary>
        /// Hide the dropdown menu.
        /// </summary>
        private void HideMenu()
        {
            _isExpanded = false;

            var timer = new Timer { Interval = 15 };
            timer.Tick += (s, e) =>
            {
                if (this.Height > _dropDownButton.Height)
                {
                    this.Height -= 10;
                }
                else
                {
                    this.Height = _dropDownButton.Height;
                    timer.Stop();
                }
            };

            timer.Start();
        }

        /// <summary>
        /// Animate the expansion or collapse of the dropdown menu.
        /// </summary>
        //private void AnimateMenu(bool isExpanding)
        //{
        //    if (ListItems == null || ListItems.Count == 0) return;

        //    var targetHeight = isExpanding ? Math.Min(200, ListItems.Count * 40) : 0;
        //    var timer = new Timer { Interval = 15 };

        //    timer.Tick += (s, e) =>
        //    {
        //        if (isExpanding && this.Height < targetHeight + _dropDownButton.Height)
        //        {
        //            this.Height += 10;
        //        }
        //        else if (!isExpanding && this.Height > _dropDownButton.Height)
        //        {
        //            this.Height -= 10;
        //        }
        //        else
        //        {
        //            timer.Stop();
        //            Height = isExpanding ? targetHeight + _dropDownButton.Height : _dropDownButton.Height;
        //        }
        //    };

        //    timer.Start();
        //}
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!DesignMode)
            {
                if (!_isExpanded)
                {
                    // Ensure the height is fixed at runtime when not expanded
                    Height = _collapsedHeight;
                }
            }
            else
            {
                // Enforce a fixed height during design time
                Height = _collapsedHeight;
            }
            if (_dropDownButton != null)
            {
                _dropDownButton.Location = new System.Drawing.Point(DrawingRect.Width - 30, DrawingRect.Top);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
           
            base.OnPaint(e);
            if(_dropDownButton == null)                InitDropDownMenu();

        }
        //public override void ApplyTheme()
        //{
        //    if (_currentTheme == null) return;

        //    base.ApplyTheme();

        //    // Apply theme to the dropdown button
        //    _dropDownButton.BackColor = _currentTheme.ButtonBackColor;
        //    _dropDownButton.ForeColor = _currentTheme.ButtonForeColor;

        //    // Optionally apply additional theming to the dropdown menu
        //    BackColor = _currentTheme.PanelBackColor;
        //}

        /// <summary>
        /// Override InitializeMenu to ensure items are adjusted correctly for dropdown.
        /// </summary>
        public override void InitializeMenu()
        {
            if (ListItems == null) return;

            base.InitializeMenu();

            // Adjust position of menu items to start below the dropdown button
            foreach (Control control in Controls.OfType<Panel>().Where(c => c.Tag is SimpleMenuItem))
            {
                control.Top += _dropDownButton.Height;
            }
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
