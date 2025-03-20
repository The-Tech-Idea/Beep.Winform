using System.ComponentModel;

using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepPopupListForm : BeepPopupForm
    {
        #region "Popup List Properties"
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
        [Description("Config Title of the control.")]
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

        public BeepButton CurrenItemButton { get { return _beepListBox.CurrenItemButton; } private set { } }

        public int Menuitemheight { get; private set; } = 15;
        #endregion "Popup List Properties"
        public BeepPopupListForm()
        {
            InitializeComponent();

        }
        public BeepPopupListForm(List<SimpleItem> items)
        {
            InitializeComponent();

            _beepListBox.SelectedItemChanged += BeepListBox_SelectedItemChanged;
            _beepListBox.ItemClicked += BeepListBox_ItemClicked;
       //     OnLeave += BeepPopupListForm_OnLeave;
            if (items.Count > 0)
            {
                InitializeMenu(items);
            }
        }
        public void InitializeMenu(List<SimpleItem> items)
        {
            _beepListBox.ListItems = new BindingList<SimpleItem>(items);
            _beepListBox.Theme = Theme;
            _beepListBox.InitializeMenu();
            int _maxListHeight = Width;
            int _maxListWidth = 50;

            //    InitListbox();
            // 2) Create a borderless popup form
            //  _popupForm = new BeepPopupForm1();

            _beepListBox.ShowHilightBox = false;
            _beepListBox.Dock = DockStyle.None;
            //using (BeepButton btn = new BeepButton())
            //{
            //    if (!_beepListBox.UseThemeFont)
            //    {
            //        btn.TextFont = _listbuttontextFont;
            //    }else
            //        btn.TextFont = _beepListBox.Font;

            //    Menuitemheight = _beepListBox.GetPreferredSize(Value.Empty).Height;
            //}
            _beepListBox.MenuItemHeight = Menuitemheight;


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

           
            _beepListBox.ShowAllBorders = false;

             Size = new Size(finalWidth, neededHeight);
            _beepListBox.Dock = DockStyle.Fill; // Manually size and position
        }
     

        private void BeepListBox_ItemClicked(object? sender, SimpleItem e)
        {
            SelectedItem = e;
            if (SelectedItem != null)
            {
                CurrenItemButton = _beepListBox.CurrenItemButton;
            }
         //   DialogResult = DialogResult.OK; // Mark the dialog as "OK"
           // Close();
        }

        private void BeepListBox_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            SelectedItem = e.SelectedItem;
            DialogResult = DialogResult.OK;
            if(SelectedItem != null)
            {
                CurrenItemButton =_beepListBox.CurrenItemButton;
            }
        }

        public SimpleItem ShowPopup(string Title, Control triggerControl, BeepPopupFormPosition position, bool showtitle = false)
        {
            if (_beepListBox.ListItems == null) return null;
            if (_beepListBox.ListItems.Count == 0) return null;
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
        public SimpleItem ShowPopup(string Title, Control triggerControl, Point pointAdjusment, BeepPopupFormPosition position, bool showtitle = false)
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
            base.ShowPopup(triggerControl, position, pointAdjusment);

            return SelectedItem;

        }
        public SimpleItem ShowPopup(string Title, Point anchorPoint,  BeepPopupFormPosition position, bool showtitle = false)
        {
            ShowTitle = showtitle;
            _beepListBox.TitleText = Title;

            // Calculate the size of the popup
            int neededHeight = _beepListBox.GetMaxHeight();
            int finalHeight = Math.Min(neededHeight, _maxListHeight);

            // Calculate max width of items
            foreach (var item in _beepListBox.ListItems)
            {
                _maxListWidth = Math.Max(_maxListWidth, TextRenderer.MeasureText(item.Text, _beepListBox.TextFont).Width);
            }

            int finalWidth = Math.Min(Width, _maxListWidth + 10);

            // Ensure the popup width is at least as wide as the trigger area
            if (finalWidth < Width)
            {
                finalWidth = Width;
            }

            Size = new Size(finalWidth, neededHeight);
            _beepListBox.Dock = DockStyle.Fill; // Fill the popup

            // Adjust position based on alignment and adjustment point
            Point popupLocation =anchorPoint;

            // Set the location and show the popup
            Location = popupLocation;
            Show(); // Config the popup (or ShowDialog if modal behavior is required)

            return SelectedItem;
        }

        public override void ApplyTheme()
        {
            if(Theme == null)
            {
                return;
            }
            BackColor =_currentTheme.ButtonBackColor;
            //beepPanel1.Theme = beepuiManager1.Theme;
            BorderColor = _currentTheme.BorderColor;
            ForeColor = _currentTheme.ButtonForeColor;
            if(_beepListBox!=null)      _beepListBox.Theme = Theme;

            Invalidate();
        }

    }
}
