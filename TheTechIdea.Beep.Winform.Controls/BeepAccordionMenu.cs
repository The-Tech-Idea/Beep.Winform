using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Editors;

using Timer = System.Windows.Forms.Timer;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepAccordionMenu), "BeepAccordion.bmp")]
    [Description("A collapsible accordion control with expandable menu items.")]
    [DisplayName("Beep Accordion Menu")]
    public class BeepAccordionMenu : BeepControl
    {
        private bool isCollapsed = false;
        private const int DefaultExpandedWidth = 200;
        private const int DefaultCollapsedWidth = 64;
        private int animationStep = 20;
        private int animationDelay = 15;

        private BeepLabel logo;
        private BeepButton toggleButton;
        private Panel itemsPanel;
        private List<BeepAccordionMenuItem> menuItems = new List<BeepAccordionMenuItem>();
        private int itemHeight = 40;
        private bool isInitialized = false;
        private Timer animationTimer;
        private bool isAnimating = false;

        [Category("Behavior")]
        public int ExpandedWidth { get; set; } = DefaultExpandedWidth;

        [Category("Behavior")]
        public int CollapsedWidth { get; set; } = DefaultCollapsedWidth;

        [Category("Animation")]
        public int AnimationStep
        {
            get => animationStep;
            set => animationStep = Math.Max(1, value);
        }

        [Category("Animation")]
        public int AnimationDelay
        {
            get => animationDelay;
            set => animationDelay = Math.Max(1, value);
        }

        [Category("Appearance")]
        public int ItemHeight
        {
            get => itemHeight;
            set
            {
                itemHeight = Math.Max(20, value);
                if (isInitialized)
                {
                    InitializeMenu();
                    UpdateItemsPanelSize();
                }
            }
        }

        public event EventHandler<BeepMouseEventArgs> ItemClick;
        public event EventHandler<BeepMouseEventArgs> ToggleClicked;

        private string _title = "Accordion";
        [Category("Appearance")]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                if (logo != null)
                    logo.Text = value;
                if (toggleButton != null)
                    toggleButton.Text = value;
                Invalidate();
            }
        }

        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
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
                        InitializeMenu();
                        UpdateItemsPanelSize();
                        Invalidate();
                    }
                }
            }
        }
        protected override Size DefaultSize => new Size(DefaultExpandedWidth, 200);
        public BeepAccordionMenu()
        {
            items = new BindingList<SimpleItem>();
            DoubleBuffered = true;
            ApplyThemeToChilds = false;
            TabStop = true;
          
            Padding = new Padding(5);

            animationTimer = new Timer { Interval = 10 };
            animationTimer.Tick += AnimationTimer_Tick;

            logo = new BeepLabel
            {
                Size = new Size(ExpandedWidth - 10, 32),
                Location = new Point(5, 5),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                MaxImageSize = new Size(30, 30),
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = Title,
                IsFrameless = true,
                IsBorderAffectedByTheme = false,
                IsChild = true
            };
            Controls.Add(logo);

            toggleButton = new BeepButton
            {
                Size = new Size(ExpandedWidth - 10, 32),
                Text = "",
                HideText = true,
                IsFrameless = true,
                IsBorderAffectedByTheme = false,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.hamburger.svg",
                MaxImageSize = new Size(24, 24),
                ImageAlign = ContentAlignment.MiddleCenter,
                Location = new Point(5, logo.Bottom + 5),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                IsChild = true
            };
            toggleButton.Click += ToggleButton_Click;
            Controls.Add(toggleButton);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try
            {
                if (!isInitialized)
                {
                    InitializeAccordion();
                    InitializeMenu();
                    Invalidate();
                    isInitialized = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Crash in OnHandleCreated: {ex.Message}\n{ex.StackTrace}");
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (isInitialized)
            {
                UpdateItemsPanelSize();
                AdjustControlWidths(Width);
            }
        }

        private void InitializeAccordion()
        {
            if (itemsPanel == null)
            {
                itemsPanel = new Panel
                {
                    Location = new Point(5, toggleButton?.Bottom + 5 ?? 42),
                    Size = new Size(ExpandedWidth - 10, Height - (toggleButton?.Bottom ?? 37) - logo.Height - 15),
                    AutoScroll = true,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
                };
                Controls.Add(itemsPanel);
            }
        }

        private void ToggleButton_Click(object sender, EventArgs e)
        {
            isCollapsed = !isCollapsed;
            StartAccordionAnimation();
            ToggleClicked?.Invoke(this, new BeepMouseEventArgs("ToggleClicked", isCollapsed));
            foreach (var item in menuItems)
            {
                item.SetCollapsedState(isCollapsed);
            }
        }

        private void StartAccordionAnimation()
        {
            if (!isInitialized) return;

            isAnimating = true;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (!isAnimating) return;

            int targetWidth = isCollapsed ? CollapsedWidth : ExpandedWidth;
            int currentWidth = Width;

            if (isCollapsed)
            {
                currentWidth -= AnimationStep;
                if (currentWidth <= targetWidth)
                {
                    currentWidth = targetWidth;
                    animationTimer.Stop();
                    isAnimating = false;
                }
            }
            else
            {
                currentWidth += AnimationStep;
                if (currentWidth >= targetWidth)
                {
                    currentWidth = targetWidth;
                    animationTimer.Stop();
                    isAnimating = false;
                }
            }

            Width = currentWidth;
            AdjustControlWidths(currentWidth);
            Invalidate();
        }

        private void AdjustControlWidths(int width)
        {
            int padding = 5;
            int buttonWidth = width - (2 * padding);

            if (logo != null)
            {
                logo.Width = buttonWidth;
                logo.Location = new Point(padding, padding);
            }

            if (toggleButton != null)
            {
                toggleButton.Width = buttonWidth;
                toggleButton.Location = new Point(padding, logo?.Bottom + padding ?? padding);
            }

            if (itemsPanel != null)
            {
                itemsPanel.Width = buttonWidth;
                itemsPanel.Location = new Point(padding, toggleButton?.Bottom + padding ?? (logo?.Bottom + 2 * padding ?? 2 * padding));

                foreach (Control control in itemsPanel.Controls)
                {
                    if (control is BeepAccordionMenuItem menuItem)
                    {
                        menuItem.AdjustWidth(buttonWidth);
                    }
                }
            }
        }

        private void InitializeMenu()
        {
            if (itemsPanel == null || items == null) return;

            itemsPanel.SuspendLayout();
            itemsPanel.Controls.Clear();
            menuItems.Clear();

            int yOffset = 0;

            foreach (var item in items)
            {
                var menuItem = new BeepAccordionMenuItem(item, itemHeight)
                {
                    Location = new Point(0, yOffset),
                    Width = itemsPanel.Width
                };
                menuItem.ItemClick += (s, e) => ItemClick?.Invoke(s, e);
                menuItem.HeightChanged += MenuItem_HeightChanged; // Subscribe to height changes
                itemsPanel.Controls.Add(menuItem);
                menuItems.Add(menuItem);
                yOffset += menuItem.Height + 5;
            }

            itemsPanel.ResumeLayout();
            AdjustControlWidths(Width);
            UpdateItemsPanelSize();
            itemsPanel.Invalidate();
        }
        private void MenuItem_HeightChanged(object sender, EventArgs e)
        {
            if (sender is BeepAccordionMenuItem changedItem)
            {
                int index = menuItems.IndexOf(changedItem);
                if (index >= 0)
                {
                    int yOffset = changedItem.Top + changedItem.Height + 5;
                    for (int i = index + 1; i < menuItems.Count; i++)
                    {
                        menuItems[i].Location = new Point(0, yOffset);
                        yOffset += menuItems[i].Height + 5;
                    }
                }
            }
            UpdateItemsPanelSize();
        }
        private void UpdateItemsPanelSize()
        {
            if (itemsPanel == null || toggleButton == null || logo == null) return;

            itemsPanel.Location = new Point(5, toggleButton.Bottom + 5);
            itemsPanel.Size = new Size(Width - 10, Height - toggleButton.Bottom - logo.Height - 15);
            itemsPanel.PerformLayout();
        }

        public override void ApplyTheme()
        {
         //   base.ApplyTheme();
            BackColor = _currentTheme?.SideMenuBackColor ?? Color.FromArgb(51, 51, 51);

            if (logo != null)
            {
                logo.Theme = Theme;
                logo.ForeColor = _currentTheme?.SideMenuForeColor ?? Color.White;
            }

            if (toggleButton != null)
            {
                toggleButton.Theme = Theme;
                toggleButton.ApplyThemeOnImage = true;
                toggleButton.ForeColor = _currentTheme?.SideMenuForeColor ?? Color.White;
                toggleButton.ApplyThemeToSvg();
            }

            if (itemsPanel != null)
            {
                foreach (Control control in itemsPanel.Controls)
                {
                    if (control is BeepAccordionMenuItem menuItem)
                    {
                        menuItem.Theme = Theme;
                       // menuItem.ApplyTheme();
                    }
                }
            }
        }
    }
}