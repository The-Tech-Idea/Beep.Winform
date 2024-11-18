using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ModernSideMenu;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepAccordion : BeepControl
    {
        private bool isCollapsed = false;
        private const int expandedWidth = 200;
        private const int collapsedWidth = 64;
        private const int animationStep = 20;
        private BeepLabel logo;
        private BeepButton toggleButton;
        private FlowLayoutPanel itemsPanel;
        private List<BeepButton> buttons = new List<BeepButton>();
        private int itemHeight = 40;

        public event EventHandler<BeepEventDataArgs> ItemClick;
        public event EventHandler<BeepEventDataArgs> ToggleClicked;
        // Define the items collection property with designer support
        private SimpleMenuItemCollection items = new SimpleMenuItemCollection();

        private string _title = "Accordion";
        [Category("Appearance")]
        [Description("The title of the accordion")]
        [DefaultValue("Accordion")]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                if (logo != null)
                    logo.Text = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleMenuItemCollection ListItems
        {
            get => items;
            set
            {
                items = value;
                InitializeMenu();
            }
        }

        public BeepAccordion()
        {
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            _isControlinvalidated = true;
            InitializeAccordion();
            items.ListChanged += Items_ListChanged; // Handle changes in the item collection
        }

        private void Items_ListChanged(object sender, ListChangedEventArgs e)
        {
            InitializeMenu(); // Re-initialize menu on collection change
        }

        private void InitializeAccordion()
        {
            // Set up logo
            logo = new BeepLabel
            {
                Size = new Size(expandedWidth, 32),
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.home.svg", // Default logo path
                Location = new Point(0, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                MaxImageSize = new Size(30, 30),
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = Title
            };
            Controls.Add(logo);

            // Set up toggle button
            toggleButton = new BeepButton
            {
                Size = new Size(expandedWidth, 32),
                Text = "",
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.hamburger.svg", // Default hamburger icon path
                MaxImageSize = new Size(24, 24),
                ImageAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, logo.Height),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ShowAllBorders = false,
                ShowShadow = false,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false
            };
            toggleButton.Click += ToggleButton_Click;
            Controls.Add(toggleButton);

            // Set up items panel
            itemsPanel = new FlowLayoutPanel
            {
                Location = new Point(0, toggleButton.Bottom),
                Size = new Size(expandedWidth, Height - toggleButton.Bottom),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true
            };
            Controls.Add(itemsPanel);

            // Initialize menu items
            InitializeMenu();
        }

        private void ToggleButton_Click(object sender, EventArgs e)
        {
            isCollapsed = !isCollapsed;
            var x = new BeepEventDataArgs("ToggleClicked",isCollapsed);
       
            ToggleClicked?.Invoke(this, x);
            StartAccordionAnimation();
        }

        private void InitializeMenu()
        {
            // Clear existing controls
            itemsPanel.Controls.Clear();
            buttons.Clear();

            if (items == null || items.Count == 0)
                return;

            // Create a BeepButton for each item in ListItems
            foreach (var item in items)
            {
                var itemButton = new BeepButton
                {
                    Text = item.Text,
                    ImagePath = item.Image,
                    MaxImageSize = new Size(30, 30),
                    TextImageRelation = TextImageRelation.ImageBeforeText,
                    ImageAlign = ContentAlignment.MiddleLeft,
                    TextAlign = ContentAlignment.MiddleLeft,
                    HideText = isCollapsed, // Hide text if collapsed
                    Height = itemHeight,
                    Width = expandedWidth // Initial width to expanded
                };

                // Apply theme and add hover effects
                itemButton.MouseEnter += (s, e) => itemButton.BackColor = _currentTheme.SelectedRowBackColor;
                itemButton.MouseLeave += (s, e) => itemButton.BackColor = _currentTheme.SideMenuBackColor;
                itemButton.Click += ItemButton_Click;
                // Add item to the list and panel
                buttons.Add(itemButton);
                itemsPanel.Controls.Add(itemButton);
            }
        }

        private void ItemButton_Click(object? sender, EventArgs e)
        {
            var x = new BeepEventDataArgs("ItemClicked", sender);
            x.Cancel = false;
            ItemClick?.Invoke(this, x);
        }

        private async void StartAccordionAnimation()
        {
            int targetWidth = isCollapsed ? collapsedWidth : expandedWidth;
            int currentWidth = Width;

            while (currentWidth != targetWidth)
            {
                currentWidth += isCollapsed ? -animationStep : animationStep;

                if (isCollapsed && currentWidth < targetWidth) currentWidth = targetWidth;
                if (!isCollapsed && currentWidth > targetWidth) currentWidth = targetWidth;

                Width = currentWidth;
                logo.Width = currentWidth;
                toggleButton.Width = currentWidth;
                itemsPanel.Width = currentWidth;

                foreach (var button in buttons)
                {
                    button.Width = currentWidth;
                    button.HideText = isCollapsed;
                }
                logo.Text = isCollapsed ? "" : Title;
                await Task.Delay(15); // Adjust for smoother animation
            }
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (!_isControlinvalidated) return;

            BackColor = _currentTheme.SideMenuBackColor;
            logo.Theme = Theme;
            toggleButton.Theme = Theme;

            foreach (var button in buttons)
            {
                button.Theme = Theme;
                button.BackColor = _currentTheme.SideMenuBackColor;
                button.ForeColor = _currentTheme.SideMenuForeColor;
            }
        }
    }
}
