using System.ComponentModel;

using TheTechIdea.Beep.Desktop.Common;

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
     
        #endregion "Popup List Properties"
        public BeepPopupListForm(List<SimpleItem> items)
        {
            InitializeComponent();
            InitializeMenu(items);
            _beepListBox.SelectedItemChanged += BeepListBox_SelectedItemChanged;
            _beepListBox.ItemClicked += BeepListBox_ItemClicked;
            OnLeave += BeepPopupListForm_OnLeave;
        }
        public void InitializeMenu(List<SimpleItem> items)
        {
            _beepListBox.ListItems = new BindingList<SimpleItem>(items);
            _beepListBox.InitializeMenu();
        }
        private void BeepPopupListForm_OnLeave(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void BeepListBox_ItemClicked(object? sender, SimpleItem e)
        {
            
        }

        private void BeepListBox_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            SelectedItem = e.SelectedItem;
        }

       
        public  virtual void ShowPopup(Control triggerControl, Point location)
        {
            if (_isPopupOpen) return;
            // Always create a new instance from scratch
        
            _isPopupOpen = true;
           
            int _maxListHeight = Width;
            int _maxListWidth = 100;

            //    InitListbox();
            // 2) Create a borderless popup form
            //  _popupForm = new BeepPopupForm();
            
            _beepListBox.ShowHilightBox = false;
            _beepListBox.Dock = DockStyle.None;
            _beepListBox.MenuItemHeight = 15;
            _beepListBox.InitializeMenu();

            int neededHeight = _beepListBox.GetMaxHeight();
            int finalHeight = Math.Min(neededHeight, _maxListHeight);
            // possibly also compute width
            int finalWidth = Math.Max(Width, _maxListWidth);

            // The popup form is sized to fit beepListBox
            Size = new Size(finalWidth, neededHeight);
            // Position popup just below the main control
            var screenPoint = new Point(-(finalWidth / 2), Height + 5);
            _beepListBox.Theme = Theme;
            _beepListBox.ShowAllBorders = false;
            //_popupForm.BackColor = _currentTheme.BackColor;
   
            _beepListBox.Dock = DockStyle.Fill; // Manually size and position
            base.ShowPopup(this, location);
           
        }
      
    }
}
