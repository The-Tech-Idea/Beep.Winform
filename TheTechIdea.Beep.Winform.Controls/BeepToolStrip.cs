using System.ComponentModel;
using System.Drawing.Design;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Winform.Controls.Editors;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum ToolStripOrientation
    {
        Horizontal,
        Vertical
    }
    [ToolboxItem(true)]
    [DisplayName("Beep ToolStrip")]
    [Category("Beep Controls")]
    [Description("A control that displays a collection of buttons in a toolbar format.")]
    public class BeepToolStrip : BeepControl
    {
        private FlowLayoutPanel _stripPanel;
        private List<BeepButton> _buttons = new List<BeepButton>();
        private ToolStripOrientation _orientation = ToolStripOrientation.Horizontal;
        private SimpleItemCollection items;

        public event EventHandler<BeepEventDataArgs> ButtonClicked;

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleItemCollection Buttons
        {
            get => items;
            set
            {
                if (items != null)
                {
                    items.ListChanged -= Items_ListChanged;
                }

                items = value;

                if (items != null)
                {
                    items.ListChanged += Items_ListChanged;
                }

                // Refresh toolbar from the updated rootnodeitems collection
                RefreshToolbarFromItems();
            }
        }

        public BeepToolStrip()
        {
            _stripPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = SystemColors.Control
            };

            Controls.Add(_stripPanel);

            // Initialize an empty rootnodeitems collection by default
            items = new SimpleItemCollection();
            items.ListChanged += Items_ListChanged;

            ApplyTheme();
        }

        private void Items_ListChanged(object? sender, ListChangedEventArgs e)
        {
            // Whenever the rootnodeitems collection changes, rebuild the toolbar
            RefreshToolbarFromItems();
        }

        private void RefreshToolbarFromItems()
        {
            // Clear existing buttons
            foreach (var btn in _buttons)
            {
                btn.Click -= OnButtonClick;
            }
            _buttons.Clear();
            _stripPanel.Controls.Clear();

            // Create new buttons for each SimpleItem in the collection
            if (items != null)
            {
                foreach (var item in items)
                {
                    var button = CreateButtonFromItem(item);
                    AddButton(button);
                }
            }

            Invalidate();
            PerformLayout();
        }

        private BeepButton CreateButtonFromItem(SimpleItem item)
        {
            var btn = new BeepButton
            {
                Text = string.IsNullOrEmpty(item.Text) ? item.Name : item.Text,
                // Apply additional item properties if desired
                IsChild = true,
                ShowAllBorders = false,
                ShowShadow = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageAboveText,
                ToolTipText = item.DisplayField,
                Theme = this.Theme,
                ApplyThemeOnImage = true,
                Tag = item

            };

            // If there is an image specified, set the ImagePath
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                btn.ImagePath = item.ImagePath;
            }
            else
            {
                // If no image is provided, consider removing text-image relation or adjusting style
                btn.TextImageRelation = TextImageRelation.Overlay;
            }

            return btn;
        }

        [Browsable(true)]
        [Category("Layout")]
        public ToolStripOrientation Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                UpdateOrientation();
            }
        }

        private void UpdateOrientation()
        {
            _stripPanel.FlowDirection = (_orientation == ToolStripOrientation.Horizontal)
                ? FlowDirection.LeftToRight
                : FlowDirection.TopDown;

            Invalidate();
            PerformLayout();
        }

        public void AddButton(BeepButton button)
        {
            button.Theme = this.Theme;
            button.ApplyTheme();
            button.Click += OnButtonClick;
            _buttons.Add(button);
            _stripPanel.Controls.Add(button);
        }

        public void RemoveButton(BeepButton button)
        {
            if (_buttons.Contains(button))
            {
                button.Click -= OnButtonClick;
                _buttons.Remove(button);
                _stripPanel.Controls.Remove(button);
            }
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            if (sender is BeepButton btn)
            {
                var dataArgs = new BeepEventDataArgs(btn.Text, btn);
                
                ButtonClicked?.Invoke(this, dataArgs);
            }
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            BackColor = _currentTheme.PanelBackColor;

            foreach (var btn in _buttons)
            {
                btn.Theme = this.Theme;
                btn.ApplyTheme();
            }

            Invalidate();
        }
    }
}
