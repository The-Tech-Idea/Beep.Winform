using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Winform.Controls.Editors;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepAccordion), "BeepAccordion.bmp")]
    [Description("A collapsible accordion control with a logo and menu items.")]
    [DisplayName("Beep Accordion")]
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
        private string logoImagePath;
        public event EventHandler<BeepMouseEventArgs> ItemClick;
        public event EventHandler<BeepMouseEventArgs> ToggleClicked;
        // Define the rootnodeitems collection property with designer support
        private SimpleItemCollection items = new SimpleItemCollection();
        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public string LogoImage
        {
            get => logo?.ImagePath;
            set
            {
                if (logo == null)
                {
                    logo = new BeepLabel();

                }
                else
                if (logo != null)
                {
                    logo.ImagePath = value;
                    logo.Theme = Theme;
                    Properties.Settings.Default.LogoImagePath = logo?.ImagePath;
                    Properties.Settings.Default.Save();  // Save immediately or when needed
                    Invalidate(); // Repaint when the image changes
                                  // UpdateSize();
                }
            }
        }

        private string _title = "Accordion";
        [Category("Appearance")]
        [Description("The title of the accordion")]
        [DefaultValue("Accordion")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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
        public SimpleItemCollection ListItems
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
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = expandedWidth;
                Height = 200;
            }
          
            BackColor = Color.FromArgb(51, 51, 51);
            ForeColor = Color.White;
        
            DoubleBuffered = true;
            ApplyThemeToChilds = false;
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            _isControlinvalidated = true;
            InitializeAccordion();
            logoImagePath = string.Empty ; // GetLogImagePathStringValueFromSettings(); 
            if (!string.IsNullOrEmpty(logoImagePath))
            {
               logo.ImagePath = logoImagePath;
               
            }
           
            LogoImage = Properties.Settings.Default.LogoImagePath;
            items.ListChanged += Items_ListChanged; // Handle changes in the item collection
        }
        private string GetLogImagePathStringValueFromSettings()
        {

            // get the LogoImagePath from settings even if its relative and using in other places

          //  string path = "TheTechIdea.Beep.Winform.Controls.LibrarySettings.LogoImagePath";
          //  string path = Properties.Settings.Default.LogoImagePath;
            return "TheTechIdea.Beep.Winform.Controls.LibrarySettings.LogoImagePath"; ;
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

            // Set up rootnodeitems panel
            itemsPanel = new FlowLayoutPanel
            {
                Location = new Point(0, toggleButton.Bottom),
                Size = new Size(expandedWidth, Height - toggleButton.Bottom),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true
            };
            Controls.Add(itemsPanel);

            // Initialize menu rootnodeitems
            InitializeMenu();
        }

        private void ToggleButton_Click(object sender, EventArgs e)
        {
            isCollapsed = !isCollapsed;
            var x = new BeepMouseEventArgs("ToggleClicked",isCollapsed);
       
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

            // Create a BeepButton for each item in CurrentMenutems
            foreach (var item in items)
            {
                var itemButton = new BeepButton
                {
                    Text = item.Text,
                    ImagePath = item.ImagePath,
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
            var x = new BeepMouseEventArgs("ItemClicked", sender);
            x.Handled = false;
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
