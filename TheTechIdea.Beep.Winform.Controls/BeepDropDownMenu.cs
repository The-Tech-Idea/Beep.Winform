using TheTechIdea.Beep.Vis.Modules;
using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Template;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDropDownMenu : BeepControl  // Inherit from BeepControl
    {
        public List<SimpleMenuItem> Items { get; set; } // Public property to access menu items
        private HighlightSidePanel _highlightSidePanel;
        private BeepPanel _menuPanel;
        private BeepButton _dropDownButton;
        private bool _isExpanded = false;

        public BeepDropDownMenu(BeepTheme theme)
        {
            // Call the base constructor from BeepControl
            Theme = BeepThemesManager.GetThemeToEnum(theme);
            Items = new List<SimpleMenuItem>();

            // Create the dropdown button that triggers the menu
            _dropDownButton = new BeepButton
            {
                Text = "Dropdown",
                BackColor = _currentTheme.ButtonBackColor,
                ForeColor = _currentTheme.ButtonForeColor,
                Height = 40,
                Dock = DockStyle.Top
            };

            _dropDownButton.Click += ToggleMenu;

            // Create the panel for menu items
            _menuPanel = new BeepPanel
            {
                BackColor = _currentTheme.BackgroundColor,
                Visible = false,
              
                Dock = DockStyle.Top,
                Height = 0,
                Theme = BeepThemesManager.GetThemeToEnum(theme)
            };

            // Highlight panel to show active item
            _highlightSidePanel = new HighlightSidePanel
            {
                Width = 5,
                BackColor = theme.AccentColor,
                Visible = false
            };

            // Add controls to DropDownMenu
            Controls.Add(_menuPanel);
            Controls.Add(_highlightSidePanel);
            Controls.Add(_dropDownButton);

            ApplyTheme(); // Apply initial theme
        }

        // Method to show the dropdown at a specified location
        public void Show(Control parentControl, int x, int y)
        {
            // Set the location relative to the parent control
            Location = new Point(x, y);

            // Add the dropdown to the parent control
            if (!parentControl.Controls.Contains(this))
            {
                parentControl.Controls.Add(this);
            }

            // Ensure it's visible
            BringToFront();
            Visible = true;

            // Expand the menu if it isn't already expanded
            if (!_isExpanded)
            {
                ToggleMenu(this, EventArgs.Empty); // Toggle menu to open
            }
        }

        // Populates the dropdown with menu items and applies icons if available
        public void PopulateMenu()
        {
            _menuPanel.Controls.Clear();

            foreach (var item in Items)
            {
                var button = CreateMenuButton(item);
                _menuPanel.Controls.Add(button);
            }
        }

        // Create BeepButton for each menu item
        private BeepButton CreateMenuButton(SimpleMenuItem item)
        {
            var button = new BeepButton
            {
                Text = item.Text,
                BackColor = _currentTheme.ButtonBackColor,
                ForeColor = _currentTheme.ButtonForeColor,
                Dock = DockStyle.Top,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                Tag = item
            };

            // Load the SVG icon if it's specified in the SimpleMenuItem
            if (!string.IsNullOrEmpty(item.Image))
            {
                var icon = LoadSvgIcon(item.Image);
                if (icon != null)
                {
                    button.Image = icon; // Assign the SVG image to the button
                    button.ImageAlign = ContentAlignment.MiddleLeft; // Align icon to the left
                }
            }

            button.MouseEnter += (s, e) => HighlightItem(button);
            button.MouseLeave += (s, e) => _highlightSidePanel.RemoveHighlight();
            button.Click += (s, e) => OnMenuItemClick(item);

            return button;
        }

        // Load SVG icon from the embedded resource
        private Image LoadSvgIcon(string svgResourcePath)
        {
            try
            {
                var assembly = GetType().Assembly;
                using (var stream = assembly.GetManifestResourceStream(svgResourcePath))
                {
                    if (stream != null)
                    {
                        SvgDocument svgDoc = SvgDocument.Open<SvgDocument>(stream);
                        return svgDoc.Draw(24, 24);  // Render at 24x24 pixels
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load SVG icon: {ex.Message}");
            }
            return null;
        }

        // Toggle dropdown menu visibility
        private void ToggleMenu(object sender, EventArgs e)
        {
            _isExpanded = !_isExpanded;
            AnimateMenu(_isExpanded);
        }

        // Animate menu expansion and collapse
        private void AnimateMenu(bool isExpanding)
        {
            var targetHeight = isExpanding ? Math.Min(200, Items.Count * 40) : 0;
            var timer = new Timer { Interval = 15 };
            timer.Tick += (s, e) =>
            {
                if (isExpanding && _menuPanel.Height < targetHeight)
                {
                    _menuPanel.Height += 10;
                }
                else if (!isExpanding && _menuPanel.Height > 0)
                {
                    _menuPanel.Height -= 10;
                }
                else
                {
                    timer.Stop();
                    _menuPanel.Visible = isExpanding;
                }
            };
            _menuPanel.Visible = true;
            timer.Start();
        }

        // Handle hover to move the highlight panel
        private void HighlightItem(Control button)
        {
            _highlightSidePanel.Height = button.Height;
            _highlightSidePanel.Top = button.Top;
            _highlightSidePanel.Visible = true;
        }

        // Handle menu item click event
        private void OnMenuItemClick(SimpleMenuItem item)
        {
            MessageBox.Show($"Selected item: {item.Text}");
            CollapseMenu();
        }

        // Collapse menu after selection
        private void CollapseMenu()
        {
            _isExpanded = false;
            AnimateMenu(false);
        }

        // Apply theme to the dropdown and its components
        public override void ApplyTheme()
        {
            //base.ApplyTheme();
            _menuPanel.BackColor = _currentTheme.BackgroundColor;
            _dropDownButton.BackColor = _currentTheme.ButtonBackColor;
            _dropDownButton.ForeColor = _currentTheme.ButtonForeColor;
            _highlightSidePanel.BackColor = _currentTheme.AccentColor;

            // Apply theme to each menu item button
            foreach (Control control in _menuPanel.Controls)
            {
                if (control is BeepButton button)
                {
                    button.BackColor = _currentTheme.ButtonBackColor;
                    button.ForeColor = _currentTheme.ButtonForeColor;
                }
            }
        }
    }
}
