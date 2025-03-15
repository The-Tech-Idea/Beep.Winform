using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Editors;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(false)]
    public class BeepAccordionMenuItem : BeepControl
    {
        private Panel buttonRowPanel;
        private BeepButton mainButton;
        private BeepButton toggleButton;
        private Panel submenuPanel;
        private SimpleItem item;
        private int itemHeight;
        private bool isChild;
        Panel highlightPanel = new Panel();
        Panel spacingPanel = new Panel();
        private bool isInitialized = false;
        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        public event EventHandler HeightChanged; // New event to notify height changes
        public event EventHandler<BeepMouseEventArgs> ItemClick;

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => items;
            set
            {
                if (value != null && value != items)
                {
                    items = value;
                    if (isInitialized)
                    {
                        InitializeSubmenu();
                        AdjustLayout();
                        Invalidate();
                    }
                }
            }
        }

        public BeepAccordionMenuItem(SimpleItem item, int itemHeight, bool isChild = false)
        {
            this.item = item ?? throw new ArgumentNullException(nameof(item));
            this.itemHeight = itemHeight;
            this.isChild = isChild;

            // Initialize ListItems with the item's children
            if (item.Children != null)
            {
                ListItems = new BindingList<SimpleItem>(item.Children);
            }

            InitializeComponent();
        }
   
        private void InitializeComponent()
        {
            DoubleBuffered = true;
            Width = 200 - 10; // Default width, will be adjusted by parent
            Height = itemHeight; // Initial height for main button row

            // Create a fixed-height panel for the main button row
            buttonRowPanel = new Panel
            {
                Height = itemHeight,
                Width = Width,
                Location = new Point(0, 0),
                BackColor = _currentTheme?.SideMenuBackColor ?? Color.FromArgb(51, 51, 51)
            };

             highlightPanel = new Panel
            {
                Width = 5,
                Height = itemHeight,
                Dock = DockStyle.Left,
                BackColor = _currentTheme?.SideMenuBackColor ?? Color.FromArgb(51, 51, 51),
                Visible = true,
                Tag = "HiLight"
            };

             spacingPanel = new Panel
            {
                Width = 2,
                Height = itemHeight,
                Dock = DockStyle.Left,
                BackColor = _currentTheme?.SideMenuBackColor ?? Color.FromArgb(51, 51, 51),
                Visible = true,
                Tag = "Spacing"
            };

            int toggleWidth = (items != null && items.Count > 0) ? 30 : 0;
            int buttonWidth = buttonRowPanel.Width - (highlightPanel.Width + spacingPanel.Width + toggleWidth);
            mainButton = new BeepButton
            {
                Dock = DockStyle.Left,
                Text = item.Text,
                ImagePath = item.ImagePath,
                MaxImageSize = new Size(30, 30),
                TextImageRelation = TextImageRelation.ImageBeforeText,
                TextAlign = ContentAlignment.MiddleLeft,
                ImageAlign = ContentAlignment.MiddleLeft,
                Height = itemHeight,
                Width = Math.Max(10, buttonWidth),
                Theme = Theme,
                IsChild = true,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                Tag = item,
                HideText = false
            };

            mainButton.MouseEnter += (s, e) =>
            {
               buttonRowPanel.BackColor = _currentTheme?.ButtonHoverBackColor ?? Color.Gray;
                highlightPanel.BackColor = _currentTheme?.ButtonHoverBackColor ?? Color.Gray;
            };
            mainButton.MouseLeave += (s, e) =>
            {
                buttonRowPanel.BackColor = _currentTheme?.ButtonBackColor ?? Color.FromArgb(51, 51, 51);
                highlightPanel.BackColor = _currentTheme?.ButtonBackColor ?? Color.FromArgb(51, 51, 51);
            };
            mainButton.Click += (s, e) => ItemClick?.Invoke(this, new BeepMouseEventArgs("ItemClicked", item));

            buttonRowPanel.Controls.Add(highlightPanel);
            buttonRowPanel.Controls.Add(spacingPanel);
            buttonRowPanel.Controls.Add(mainButton);

            // Create submenuPanel by default
            submenuPanel = new Panel
            {
                Width = Width - 20,
                Height = 0,
                Visible = false,
                Tag = item,
                Location = new Point(20, itemHeight),
                AutoSize = false
            };

            if (items != null && items.Count > 0)
            {
                toggleButton = new BeepButton
                {
                    Text = "▶",
                    Width = 30,
                    Height = itemHeight,
                    Dock = DockStyle.Right,
                    Tag = item,
                    TextAlign = ContentAlignment.MiddleCenter,
                    IsChild = true,
                    IsBorderAffectedByTheme = false,
                    IsShadowAffectedByTheme = false,
                    ShowAllBorders = false,
                    ShowShadow = false
                };
                toggleButton.Click += ToggleSubMenu;
                buttonRowPanel.Controls.Add(toggleButton);
                toggleButton.BringToFront();
            }

            Controls.Add(buttonRowPanel);
            Controls.Add(submenuPanel);

            isInitialized = true;
            InitializeSubmenu();
        }

        private void InitializeSubmenu()
        {
            submenuPanel.Controls.Clear();

            int childTop = 0;
            foreach (var child in items)
            {
                var childItem = new BeepAccordionMenuItem(child, itemHeight, true)
                {
                    Width = submenuPanel.Width - 7,
                    Height = itemHeight,
                    Location = new Point(0, childTop)
                };
                childItem.ItemClick += (s, e) => ItemClick?.Invoke(s, e);
                submenuPanel.Controls.Add(childItem);
                childTop += childItem.Height + 5;
            }

            // Update submenu height if expanded
            if (submenuPanel.Visible)
            {
                submenuPanel.Height = submenuPanel.Controls.Count * (itemHeight + 5);
                Height = itemHeight + submenuPanel.Height;
            }
            else
            {
                submenuPanel.Height = 0;
                Height = itemHeight;
            }
        }

        private void ToggleSubMenu(object sender, EventArgs e)
        {
            bool isExpanded = submenuPanel.Visible;
            submenuPanel.Visible = !isExpanded;
            int submenuHeight = isExpanded ? 0 : submenuPanel.Controls.Count * (itemHeight + 5);
            submenuPanel.Height = submenuHeight;
            if (toggleButton != null)
                toggleButton.Text = isExpanded ? "▶" : "▼";

            // Adjust the overall control height
            Height = itemHeight + submenuHeight;

            // Notify parent of height change
            HeightChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustLayout();
        }

        public void AdjustWidth(int width)
        {
            Width = width;
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            if (buttonRowPanel == null || mainButton == null || submenuPanel == null) return;

            buttonRowPanel.Width = Width;
            int toggleWidth = toggleButton != null ? 30 : 0;
            mainButton.Width = Math.Max(10, Width - (7 + toggleWidth));
            if (toggleButton != null)
            {
                toggleButton.Width = 30;
                toggleButton.Height = itemHeight;
            }

            submenuPanel.Width = Width - 20;
            submenuPanel.Location = new Point(20, itemHeight);

            int childTop = 0;
            foreach (Control child in submenuPanel.Controls)
            {
                if (child is BeepAccordionMenuItem childItem)
                {
                    childItem.Width = submenuPanel.Width - 7;
                    childItem.Location = new Point(0, childTop);
                    childItem.AdjustWidth(submenuPanel.Width - 7);
                    childTop += childItem.Height + 5;
                }
            }

            if (submenuPanel.Visible)
            {
                submenuPanel.Height = submenuPanel.Controls.Count * (itemHeight + 5);
            }
            else
            {
                submenuPanel.Height = 0;
            }

            Height = itemHeight + submenuPanel.Height;
        }

        public void SetCollapsedState(bool collapsed)
        {
            mainButton.HideText = collapsed;
            mainButton.ImageAlign = collapsed ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
            mainButton.TextImageRelation = collapsed ? TextImageRelation.Overlay : TextImageRelation.ImageBeforeText;
            foreach (Control child in submenuPanel.Controls)
            {
                if (child is BeepAccordionMenuItem childItem)
                {
                    childItem.SetCollapsedState(collapsed);
                }
            }
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            BackColor = _currentTheme?.ButtonBackColor ?? Color.FromArgb(51, 51, 51);
            buttonRowPanel.BackColor = _currentTheme?.ButtonBackColor ?? Color.FromArgb(51, 51, 51);
          
            mainButton.Theme = Theme;
            // mainButton.ApplyTheme();
            highlightPanel.BackColor = _currentTheme.ButtonBackColor;
            spacingPanel.BackColor = _currentTheme.ButtonBackColor;
            foreach (Control ctrl in buttonRowPanel.Controls)
            {
                if (ctrl is Panel panel && panel.Tag?.ToString() == "HiLight")
                {
                    panel.BackColor = _currentTheme?.ButtonBackColor ?? Color.FromArgb(51, 51, 51);
                }
            }

            if (toggleButton != null)
            {
                toggleButton.BackColor = _currentTheme?.ButtonBackColor ?? Color.FromArgb(51, 51, 51);
                toggleButton.ForeColor = _currentTheme?.SideMenuForeColor ?? Color.White;
                toggleButton.Theme = Theme;
                toggleButton.ApplyTheme();
            }

            foreach (Control child in submenuPanel.Controls)
            {
                if (child is BeepAccordionMenuItem childItem)
                {
                    childItem.Theme = Theme;
                   // childItem.ApplyTheme();
                }
            }
        }
    }
}