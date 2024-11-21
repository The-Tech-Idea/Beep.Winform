using TheTechIdea.Beep.Winform.Controls.Template;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDropDownMenu : BeepListBox
    {
        private BeepButton _dropDownButton;
        private bool _isExpanded = false;

        public BeepDropDownMenu()
        {
            InitDropDownMenu();
        }

        /// <summary>
        /// Initialize the dropdown menu components.
        /// </summary>
        private void InitDropDownMenu()
        {
            // Create the dropdown button
            _dropDownButton = new BeepButton
            {
                Text = "Dropdown",
                BackColor = _currentTheme.ButtonBackColor,
                ForeColor = _currentTheme.ButtonForeColor,
                Height = 40,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                ImageAlign = ContentAlignment.MiddleRight,
                FlatStyle = FlatStyle.Flat,
            };

            // Add click event to toggle menu visibility
            _dropDownButton.Click += ToggleMenu;

            // Adjust DrawingRect to leave space for the dropdown button
            DrawingRect = new Rectangle(DrawingRect.X, DrawingRect.Y + _dropDownButton.Height, DrawingRect.Width, DrawingRect.Height - _dropDownButton.Height);

            // Add the dropdown button to the controls
            Controls.Add(_dropDownButton);
        }

        /// <summary>
        /// Toggle the visibility of the dropdown menu.
        /// </summary>
        private void ToggleMenu(object sender, EventArgs e)
        {
            _isExpanded = !_isExpanded;
            AnimateMenu(_isExpanded);
        }

        /// <summary>
        /// Animate the expansion or collapse of the dropdown menu.
        /// </summary>
        private void AnimateMenu(bool isExpanding)
        {
            var targetHeight = isExpanding ? Math.Min(200, ListItems.Count * 40) : 0;
            var timer = new System.Windows.Forms.Timer { Interval = 15 };

            timer.Tick += (s, e) =>
            {
                if (isExpanding && this.Height < targetHeight + _dropDownButton.Height)
                {
                    this.Height += 10;
                }
                else if (!isExpanding && this.Height > _dropDownButton.Height)
                {
                    this.Height -= 10;
                }
                else
                {
                    timer.Stop();
                    Height = isExpanding ? targetHeight + _dropDownButton.Height : _dropDownButton.Height;
                }
            };

            timer.Start();
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            // Apply theme to the dropdown button
            _dropDownButton.BackColor = _currentTheme.ButtonBackColor;
            _dropDownButton.ForeColor = _currentTheme.ButtonForeColor;

            // Optionally apply additional theming to the dropdown menu
            BackColor = _currentTheme.PanelBackColor;
        }

        /// <summary>
        /// Override InitializeMenu to ensure items are adjusted correctly for dropdown.
        /// </summary>
        public override void InitializeMenu()
        {
            base.InitializeMenu();

            // Adjust position of menu items to start below the dropdown button
            foreach (Control control in Controls.OfType<Panel>().Where(c => c.Tag is SimpleMenuItem))
            {
                control.Top += _dropDownButton.Height;
            }
        }

        /// <summary>
        /// Handle selection and notify the parent or caller of the selected item.
        /// </summary>
        private void MenuItemButton_Click(object sender, EventArgs e)
        {
            if (sender is BeepButton clickedButton && clickedButton.Tag is SimpleMenuItem selectedItem)
            {
                _dropDownButton.Text = selectedItem.Text;
                _dropDownButton.Image = clickedButton.Image;
                ToggleMenu(this, EventArgs.Empty); // Collapse the menu
                SelectedIndex = ListItems.IndexOf(selectedItem); // Update the selected index
                OnSelectedIndexChanged(EventArgs.Empty);
            }
        }
    }
}
