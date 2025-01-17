using System.ComponentModel;

using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepPopupListForm : BeepPopupForm
    {
        #region "Popup List Properties"


        private bool _isPopupOpen;
        private bool _popupmode = false;
        private int _maxListHeight = 100;
        private int _maxListWidth = 100;
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool PopupMode
        {
            get => _popupmode;
            set
            {
                _popupmode = value;
            }
        }
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => _beepListBox.ListItems;
            set => _beepListBox.ListItems = value;
        }
        // The item currently chosen by the user
        private SimpleItem _selectedItem;
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            private set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnSelectedItemChanged(_selectedItem); //
                }
            }
        }
        [Browsable(false)]
        public int SelectedIndex
        {
            get => _beepListBox.SelectedIndex;
            set
            {
                if (value >= 0 && value < _beepListBox.ListItems.Count)
                {
                    _beepListBox.SelectedIndex = value;
                    SelectedItem = _beepListBox.ListItems[value];
                }
            }
        }
        private Font _textFont = new Font("Arial", 10);
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {

                _textFont = value;
                if (_beepListBox != null)
                {
                    _beepListBox.UseThemeFont = false;
                    _beepListBox.TextFont = value;
                }

                Invalidate();


            }
        }
        private bool _showtitle = true;
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Show Title of the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitle
        {
            get => _showtitle;
            set
            {
                _showtitle = value;
                if (_beepListBox != null)
                {
                    _beepListBox.ShowTitle = value;
                }
                Invalidate();
            }
        }
        #endregion "Popup List Properties"
        public BeepPopupListForm(List<SimpleItem> items)
        {
            InitializeComponent();

            _beepListBox.SelectedItemChanged += BeepListBox_SelectedItemChanged;
            _beepListBox.ItemClicked += BeepListBox_ItemClicked;
            OnLeave += BeepPopupListForm_OnLeave;
            if (items.Count > 0)
            {
                InitializeMenu(items);
            }
        }
        public void InitializeMenu(List<SimpleItem> items)
        {
            _beepListBox.ListItems = new BindingList<SimpleItem>(items);
            _beepListBox.InitializeMenu();
            int _maxListHeight = Width;
            int _maxListWidth = 50;

            //    InitListbox();
            // 2) Create a borderless popup form
            //  _popupForm = new BeepPopupForm1();

            _beepListBox.ShowHilightBox = false;
            _beepListBox.Dock = DockStyle.None;
            _beepListBox.MenuItemHeight = 15;


            int neededHeight = _beepListBox.GetMaxHeight();
            int finalHeight = Math.Min(neededHeight, _maxListHeight);
            // possibly also compute width
            // get max width of all items
            foreach (var item in items)
            {
                _maxListWidth = Math.Max(_maxListWidth, TextRenderer.MeasureText(item.Text, _beepListBox.TextFont).Width);

            }
            int finalWidth = Math.Min(Width, _maxListWidth + 10);

            // Position popup just below the main control

            _beepListBox.Theme = Theme;
            _beepListBox.ShowAllBorders = false;

            Size = new Size(finalWidth, neededHeight);
            _beepListBox.Dock = DockStyle.Fill; // Manually size and position
        }
        private void BeepPopupListForm_OnLeave(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel; // Mark the dialog as "Cancelled"
            this.Close();
        }

        private void BeepListBox_ItemClicked(object? sender, SimpleItem e)
        {
            SelectedItem = e;
            DialogResult = DialogResult.OK; // Mark the dialog as "OK"
            Close();
        }

        private void BeepListBox_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            SelectedItem = e.SelectedItem;
        }

        public SimpleItem ShowPopup(string Title, Control triggerControl, BeepPopupFormPosition position, bool showtitle = false)
        {
            ShowTitle = showtitle;
            _beepListBox.TitleText = Title;
            int neededHeight = _beepListBox.GetMaxHeight();
            int finalHeight = Math.Min(neededHeight, _maxListHeight);
            // possibly also compute width
            // get max width of all items
            foreach (var item in _beepListBox.ListItems)
            {
                _maxListWidth = Math.Max(_maxListWidth, TextRenderer.MeasureText(item.Text, _beepListBox.TextFont).Width);

            }
            int finalWidth = Math.Min(Width, _maxListWidth + 10);

            if (finalWidth < triggerControl.Width)
            {
                finalWidth = triggerControl.Width;
            }
            Size = new Size(finalWidth, neededHeight);
            _beepListBox.Dock = DockStyle.Fill; // Manually size and position
            base.ShowPopup(triggerControl, position);
            return SelectedItem;

        }
    }
}
