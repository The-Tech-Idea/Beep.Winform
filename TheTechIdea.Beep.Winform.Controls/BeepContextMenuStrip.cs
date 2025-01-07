using System.ComponentModel;
using TheTechIdea.Beep.Desktop.Common;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Context Menu")]
    [Category("Beep Controls")]
    [Description("A context menu control that displays a list of items.")]
    public class BeepContextMenuStrip : BeepListBox
    {
        private BeepPopupForm popupForm;

        public event EventHandler<SimpleItem> ItemClicked;

        public BeepContextMenuStrip()
        {
            // Adjust default properties for a context menu look
            ShowCheckBox = false;
            BorderStyle = BorderStyle.None; // In case you have a border property
            BackColor = Color.WhiteSmoke;  // Default background, will be overridden by theme
            ApplyTheme(); // apply current theme

            SelectedIndexChanged += BeepContextMenuStrip_SelectedIndexChanged;
        }

        private void BeepContextMenuStrip_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If an item is selected and no checkboxes are involved, we can treat that as an item click.
            // If you rely on CheckBoxes, you could handle that logic differently.
            if (SelectedIndex >= 0 && SelectedIndex < ListItems.Count)
            {
                var selectedItem = ListItems[SelectedIndex];
                OnItemClicked(selectedItem);
            }
        }

        protected virtual void OnItemClicked(SimpleItem item)
        {
            ItemClicked?.Invoke(this, item);
            Hide();
        }

        /// <summary>
        /// Shows the context menu at a specified screen location.
        /// </summary>
        /// <param name="location">Screen coordinates where the menu should appear.</param>
        public void Show(Point location)
        {
            if (popupForm != null && !popupForm.IsDisposed)
            {
                popupForm.Close();
                popupForm.Dispose();
            }

            popupForm = new BeepPopupForm
            {
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                TopMost = true
            };

            // Determine size of the popup form
            // After calling InitializeMenu, the items are placed as controls
            // You can measure the required size
            int width = this.Width;
            int height = this.Controls.OfType<Panel>().Sum(p => p.Height) + TitleBottomY + 4;

            // Adjust popupForm size
            popupForm.Size = new Size(width, height);

            // Position the popup form at desired location
            popupForm.Location = location;

            // Add current control to popupForm
            popupForm.Controls.Add(this);
            this.Dock = DockStyle.Fill;

            // Handle click outside and lose focus to close menu
            popupForm.Deactivate += (s, e) => { Hide(); };

            // Show popup
            popupForm.Show();
            popupForm.BringToFront();
        }

        /// <summary>
        /// Hides the context menu.
        /// </summary>
        public void Hide()
        {
            if (popupForm != null && !popupForm.IsDisposed)
            {
                popupForm.Close();
                popupForm.Dispose();
                popupForm = null;
            }
        }

        // Override ItemClicked or other event from BeepListBox if needed
        public override void ListItemClicked(object sender)
        {

            base.ListItemClicked(sender);
            if (sender is BeepButton btn && btn.Tag is SimpleItem item)
            {
                ItemClicked?.Invoke(this, item);
                Hide();
            }
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (popupForm != null && _currentTheme != null)
            {
                popupForm.BackColor = _currentTheme.PanelBackColor;
            }
        }
    }
}
