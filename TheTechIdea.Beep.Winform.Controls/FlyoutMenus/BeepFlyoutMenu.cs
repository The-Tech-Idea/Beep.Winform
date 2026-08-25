using System.ComponentModel;
using System.Drawing.Design;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.FlyoutMenus
{
    public enum FlyoutMenuLabelVisibility
    {
        Always,
        IconOnly,
        ExpandedOnly
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Flyout Menu")]
    [Category("Beep Controls")]
    [Description("A flyout menu control that displays a list of items using the full BeepListBox feature set.")]
    public class BeepFlyoutMenu : BaseControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.FlyoutPanel;
        private BeepButton _dropDownButton;
        private BeepLabel _dropDownLabel;
        private BeepListBox _menu;
        private bool _isExpanded = false;
        private SlideDirection _flyoutDirection = SlideDirection.Bottom;
        private LabelPosition _labelPosition = LabelPosition.Left;
        private FlyoutMenuLabelVisibility _labelVisibility = FlyoutMenuLabelVisibility.Always;
        private int _menuWidth = 220;
        private int _maxMenuHeight = 280;
        private int _minTouchTargetWidth = 44;
        private bool _popupOpen = false;
        private ListBoxType _listBoxType = ListBoxType.Standard;
        private bool _showSearch = false;
        private bool _showCheckBox = false;
        private bool _showItemIcons = true;
        private bool _multiSelect = false;

        private readonly FlyoutManager _flyoutManager = new FlyoutManager();

        public event EventHandler<BeepEventDataArgs> MenuClicked;

        public event EventHandler PopupOpened;
        public event EventHandler PopupClosed;

        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        private int _selectedIndex;

        #region Proxy Properties (forwarded to BeepListBox)

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual style of the popup list. Choose from 30+ variants including Standard, Searchable, Checkbox, Card, Avatar, etc.")]
        [DefaultValue(ListBoxType.Standard)]
        public ListBoxType ListBoxStyle
        {
            get => _listBoxType;
            set
            {
                if (_listBoxType != value)
                {
                    _listBoxType = value;
                    if (_menu != null)
                    {
                        _menu.ListBoxType = value;
                    }
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Width of the popup menu in pixels")]
        [DefaultValue(220)]
        public int MenuWidth
        {
            get => _menuWidth;
            set
            {
                _menuWidth = Math.Max(100, value);
                if (_menu != null)
                    _menu.Width = _menuWidth;
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Maximum height of the popup menu before scrollbars appear")]
        [DefaultValue(280)]
        public int MaxMenuHeight
        {
            get => _maxMenuHeight;
            set
            {
                _maxMenuHeight = Math.Max(80, value);
                if (_menu != null && _popupOpen)
                    RecalculateMenuHeight();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show a search box at the top of the popup list")]
        [DefaultValue(false)]
        public bool ShowSearch
        {
            get => _showSearch;
            set
            {
                _showSearch = value;
                if (_menu != null)
                    _menu.ShowSearch = value;
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show checkboxes for multi-selection")]
        [DefaultValue(false)]
        public bool ShowCheckBox
        {
            get => _showCheckBox;
            set
            {
                _showCheckBox = value;
                if (_menu != null)
                    _menu.ShowCheckBox = value;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show item icons in the popup list")]
        [DefaultValue(true)]
        public bool ShowItemIcons
        {
            get => _showItemIcons;
            set
            {
                _showItemIcons = value;
                if (_menu != null)
                    _menu.ShowImage = value;
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allow selecting multiple items")]
        [DefaultValue(false)]
        public bool MultiSelect
        {
            get => _multiSelect;
            set
            {
                _multiSelect = value;
                if (_menu != null)
                    _menu.MultiSelect = value;
            }
        }

        #endregion

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => items;
            set
            {
                items = value ?? new BindingList<SimpleItem>();
                if (_menu != null)
                {
                    _menu.ListItems = items;
                }
            }
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0 && value < (_menu?.ListItems.Count ?? 0))
                {
                    _selectedIndex = value;
                    if (_menu != null)
                    {
                        _menu.SelectedIndex = value;
                    }
                    OnSelectedIndexChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler SelectedIndexChanged;

        protected virtual void OnSelectedIndexChanged(EventArgs e) => SelectedIndexChanged?.Invoke(this, e);

        [Category("Flyout")]
        [Description("Direction the flyout menu expands")]
        public SlideDirection FlyoutDirection
        {
            get => _flyoutDirection;
            set
            {
                _flyoutDirection = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Flyout")]
        [Description("Controls when the label is displayed")]
        [DefaultValue(FlyoutMenuLabelVisibility.Always)]
        public FlyoutMenuLabelVisibility LabelVisibility
        {
            get => _labelVisibility;
            set
            {
                _labelVisibility = value;
                UpdateLabelVisibility();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Sets the position of the label relative to the dropdown button.")]
        public LabelPosition LabelPosition
        {
            get => _labelPosition;
            set
            {
                _labelPosition = value;
                UpdateControlLayout();
            }
        }

        [Category("Flyout")]
        [Description("Minimum touch target width in pixels")]
        [DefaultValue(44)]
        public int MinTouchTargetWidth
        {
            get => _minTouchTargetWidth;
            set
            {
                _minTouchTargetWidth = Math.Max(32, value);
                if (_dropDownButton != null && _dropDownButton.Width < _minTouchTargetWidth)
                {
                    _dropDownButton.Width = _minTouchTargetWidth;
                    UpdateControlLayout();
                }
            }
        }

        [Browsable(false)]
        public bool IsPopupOpen => _popupOpen;

        [Browsable(true)]
        [Category("Flyout")]
        [Description("Whether the flyout menu is currently expanded (design-time only)")]
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    UpdateButtonIcon();
                    UpdateLabelVisibility();
                    Invalidate();
                }
            }
        }

        public BeepFlyoutMenu()
        {
            Height = 28;
            ApplyThemeToChilds = false;
            Text = "Flyout Menu";
            InitDropDownMenu();
            InitMenu();
            _flyoutManager.Closed += OnFlyoutClosed;
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            SlideFrom = SlideDirection.Bottom;
            AnimationType = DisplayAnimationType.SlideAndFade;
            UpdateControlLayout();
        }

        private void InitDropDownMenu()
        {
            if (_dropDownButton != null) Controls.Remove(_dropDownButton);
            if (_dropDownLabel != null) Controls.Remove(_dropDownLabel);

            _dropDownLabel = new BeepLabel
            {
                Text = Text,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.None,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Padding = new Padding(5, 0, 0, 0),
                ShowShadow = false,
                ShowAllBorders = false,
                IsFrameless = true,
                IsChild = true
            };

            _dropDownButton = new BeepButton
            {
                Height = DrawingRect.Height,
                Width = Math.Max(20, _minTouchTargetWidth),
                MaxImageSize = new Size(20, DrawingRect.Height - 4),
                Dock = DockStyle.None,
                ApplyThemeOnImage = true,
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ShowAllBorders = false,
                ShowShadow = false,
                IsFrameless = true,
                HideText = true,
                IsChild = true
            };

            UpdateButtonIcon();
            _dropDownButton.Click += ToggleMenu;

            Controls.Add(_dropDownLabel);
            Controls.Add(_dropDownButton);
            UpdateLabelVisibility();
        }

        private void UpdateLabelVisibility()
        {
            if (_dropDownLabel == null) return;

            switch (_labelVisibility)
            {
                case FlyoutMenuLabelVisibility.Always:
                    _dropDownLabel.Visible = true;
                    break;
                case FlyoutMenuLabelVisibility.IconOnly:
                    _dropDownLabel.Visible = false;
                    break;
                case FlyoutMenuLabelVisibility.ExpandedOnly:
                    _dropDownLabel.Visible = _isExpanded;
                    break;
            }
        }

        private void InitMenu()
        {
            _menu = new BeepListBox
            {
                ListItems = items,
                Theme = Theme,
                Width = _menuWidth,
                ShowAllBorders = true,
                ShowShadow = true,
                IsFrameless = false,
                ListBoxType = _listBoxType,
                BorderRadius = 8,
                ShowImage = _showItemIcons,
                MultiSelect = _multiSelect,
                ShowSearch = _showSearch,
                ShowCheckBox = _showCheckBox,
                TabStop = false
            };

            _menu.Theme = Theme;

            // Forward selection events
            _menu.SelectedItemChanged += (s, e) =>
            {
                _selectedIndex = _menu.SelectedIndex;
                if (_selectedIndex >= 0 && _selectedIndex < _menu.ListItems.Count)
                {
                    _dropDownLabel.Text = _menu.ListItems[_selectedIndex].Text;
                }
                OnSelectedIndexChanged(EventArgs.Empty);
                // Close popup on selection (unless multi-select)
                if (!_menu.MultiSelect)
                {
                    HideMenu();
                }
            };

            // Forward click events
            _menu.ItemClicked += (s, item) =>
            {
                MenuClicked?.Invoke(this, new BeepEventDataArgs("MenuClick", item));
            };
        }

        private void UpdateButtonIcon()
        {
            string iconPath = _flyoutDirection switch
            {
                SlideDirection.Left => _isExpanded
                    ? SvgsUI.ArrowBadgeLeft
                    : SvgsUI.ArrowBadgeRight,
                SlideDirection.Right => _isExpanded
                    ? SvgsUI.ArrowBadgeRight
                    : SvgsUI.ArrowBadgeLeft,
                SlideDirection.Bottom => _isExpanded
                    ? SvgsUI.ArrowBadgeUp
                    : SvgsUI.ArrowBadgeDown,
                _ => string.Empty,
            };

            _dropDownButton.ImagePath = iconPath;
        }

        private void ToggleMenu(object sender, EventArgs e)
        {
            if (!_isExpanded)
            {
                if (items.Count == 0)
                    return; // Silently ignore empty menus
                ShowMenu();
            }
            else
            {
                HideMenu();
            }
        }

        private void RecalculateMenuHeight()
        {
            if (_menu == null) return;
            int contentHeight = _menu.MenuItemHeight * items.Count;
            if (_menu.ShowSearch)
                contentHeight += _menu.SearchAreaHeight;
            _menu.Height = Math.Min(contentHeight, _maxMenuHeight);
        }

        private void ShowMenu()
        {
            if (_menu == null) return;

            _menu.ListItems = items;
            _menu.Width = _menuWidth;
            RecalculateMenuHeight();

            _flyoutManager.Show(
                _menu,
                this,
                MapFlyoutPosition(_flyoutDirection),
                new Size(_menuWidth, _menu.Height));

            _isExpanded = true;
            _popupOpen = true;
            UpdateButtonIcon();
            UpdateLabelVisibility();

            OnPopupOpened(EventArgs.Empty);
        }

        private void HideMenu()
        {
            _flyoutManager.Close();
        }

        private void OnFlyoutClosed(object? sender, EventArgs e)
        {
            if (!_isExpanded && !_popupOpen) return;

            _isExpanded = false;
            _popupOpen = false;
            UpdateButtonIcon();
            UpdateLabelVisibility();
            OnPopupClosed(EventArgs.Empty);
        }

        private static BeepPopupFormPosition MapFlyoutPosition(SlideDirection direction) => direction switch
        {
            SlideDirection.Left => BeepPopupFormPosition.Left,
            SlideDirection.Right => BeepPopupFormPosition.Right,
            SlideDirection.Top => BeepPopupFormPosition.Top,
            _ => BeepPopupFormPosition.Bottom,
        };

        public void CloseChildPopup()
        {
            HideMenu();
        }

        protected virtual void OnPopupOpened(EventArgs e) => PopupOpened?.Invoke(this, e);
        protected virtual void OnPopupClosed(EventArgs e) => PopupClosed?.Invoke(this, e);

        private void UpdateControlLayout()
        {
            if (_dropDownLabel == null || _dropDownButton == null) return;

            _dropDownLabel.Top = DrawingRect.Top;
            _dropDownLabel.Height = DrawingRect.Height;
            _dropDownButton.Top = DrawingRect.Top;
            _dropDownButton.Height = DrawingRect.Height;

            if (_labelPosition == LabelPosition.Left)
            {
                _dropDownLabel.Left = DrawingRect.Left;
                _dropDownLabel.Width = DrawingRect.Width - _dropDownButton.Width;
                _dropDownButton.Left = _dropDownLabel.Right;
            }
            else if (_labelPosition == LabelPosition.Right)
            {
                _dropDownButton.Left = DrawingRect.Left;
                _dropDownLabel.Left = _dropDownButton.Right;
                _dropDownLabel.Width = DrawingRect.Width - _dropDownButton.Width;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateControlLayout();
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_menu != null)
            {
                _menu.Theme = Theme;
                _menu.ApplyTheme();
            }
            if (_dropDownLabel != null)
            {
                _dropDownLabel.Theme = Theme;
                _dropDownLabel.ApplyTheme();
            }
            if (_dropDownButton != null)
            {
                _dropDownButton.Theme = Theme;
                _dropDownButton.ApplyTheme();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _flyoutManager.Closed -= OnFlyoutClosed;
                _flyoutManager.Dispose(); // closes any open flyout before releasing the menu
                _menu?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
