using System.ComponentModel;
using System.Drawing.Design;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Editors;
using TheTechIdea.Beep.Vis.Modules;


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
        private BeepButton button;
        private BeepLabel label;
        private BeepImage image;
        //private FlowLayoutPanel _stripPanel;
        //private List<BeepButton> _buttons = new List<BeepButton>();
        private ToolStripOrientation _orientation = ToolStripOrientation.Horizontal;
        private SimpleMenuList items;

        public event EventHandler<BeepEventDataArgs> ButtonClicked;

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleMenuList Buttons
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
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (items == null || items.Count == 0)
                return;

            int buttonWidth = 48;
            int buttonHeight = 48;
            int spacing = 8;
            int x = DrawingRect.Left + spacing;
            int y = DrawingRect.Top + spacing;

            // Remove old hit areas before adding new ones
            ClearHitList();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                // Reuse the single button field for drawing
                if (button == null)
                    button = new BeepButton();

                button.Text = string.IsNullOrEmpty(item.Text) ? item.Name : item.Text;
                button.ImagePath = item.ImagePath;
                button.ToolTipText = item.DisplayField;
                button.Tag = item;
                button.Theme = this.Theme;
                button.ApplyThemeOnImage = true;
                button.IsChild = true;
                button.ShowAllBorders = false;
                button.ShowShadow = false;
                button.IsBorderAffectedByTheme = false;
                button.IsShadowAffectedByTheme = false;
                button.ImageAlign = ContentAlignment.MiddleCenter;
                button.TextImageRelation = string.IsNullOrEmpty(item.ImagePath) ? TextImageRelation.Overlay : TextImageRelation.ImageAboveText;

                var rect = _orientation == ToolStripOrientation.Horizontal
                    ? new Rectangle(x, y, buttonWidth, buttonHeight)
                    : new Rectangle(x, y, buttonWidth, buttonHeight);

                button.Draw(g, rect);

                // Add hit area for this button
                int idx = i; // capture for lambda
                AddHitArea(
                    $"Button_{idx}",
                    rect,
                    button,
                    () => OnButtonClick(item)
                );

                // Advance position
                if (_orientation == ToolStripOrientation.Horizontal)
                    x += buttonWidth + spacing;
                else
                    y += buttonHeight + spacing;
            }
        }
        private void OnButtonClick(SimpleItem item)
        {
            if (item == null) return;
            var args = new BeepEventDataArgs(item.Text ?? item.Name, item);
            ButtonClicked?.Invoke(this, args);
        }
        public BeepToolStrip()
        {
          

            // Initialize an empty rootnodeitems collection by default
            items = new SimpleMenuList();
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

            Invalidate();
            PerformLayout();
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
            Invalidate();
            PerformLayout();
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
            Invalidate();
        }
    }
}
