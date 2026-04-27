using System.ComponentModel;
using System.Drawing.Design;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;

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
    [Description("A flyout menu control that displays a list of items.")]
    public class BeepFlyoutMenu : BaseControl
    {
        private BeepButton _dropDownButton;
        private BeepLabel _dropDownLabel;
        private BeepListBox _menu;
        private bool _isExpanded = false;
        private SlideDirection _flyoutDirection = SlideDirection.Bottom;
        private LabelPosition _labelPosition = LabelPosition.Left;
        private FlyoutMenuLabelVisibility _labelVisibility = FlyoutMenuLabelVisibility.Always;
        private readonly int _flyoutMenuWidth = 200;
        private readonly int _collapsedHeight = 25;
        private readonly int _maxMenuHeight = 200;
        private int _minTouchTargetWidth = 44;
        private bool _popupOpen = false;

        private List<BeepFlyoutMenu> beepFlyoutMenus = new List<BeepFlyoutMenu>();

        public EventHandler<BeepEventDataArgs> MenuClicked;

        public event EventHandler PopupOpened;
        public event EventHandler PopupClosed;

        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        private int _selectedIndex;

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => items;
            set
            {
                items = value;
                if (_menu != null)
                {
                    _menu.ListItems = value;
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

        public BeepFlyoutMenu()
        {
            Height = _collapsedHeight;
            ApplyThemeToChilds = false;
            Text = "Flyout Menu";
            InitDropDownMenu();
            InitMenu();
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            SlideFrom = SlideDirection.Bottom;
            AnimationType = DisplayAnimationType.SlideAndFade;
            UpdateControlLayout();
            UpdateMenuPosition();
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
                Visible = false,
                Height = 0,
                ListItems = items,
                Theme = Theme,
                Width = _flyoutMenuWidth,
                Location = new Point(0, Height),
                ShowAllBorders = false,
                ShowShadow = false,
                IsFrameless = true,
            };
            _menu.Theme = Theme;
            _menu.SelectedItemChanged += (s, e) =>
            {
                SelectedIndex = _menu.SelectedIndex;
                if (SelectedIndex >= 0)
                {
                    _dropDownLabel.Text = _menu.ListItems[SelectedIndex].Text;
                }
                ToggleMenu(this, EventArgs.Empty);
            };
            _menu.Click += (s, e) =>
            {
                if (SelectedIndex >= 0)
                {
                    MenuClicked?.Invoke(this, new BeepEventDataArgs("MenuClick", _menu.ListItems[SelectedIndex]));
                }
            };
            Controls.Add(_menu);
        }

        private void UpdateButtonIcon()
        {
            string iconPath = _flyoutDirection switch
            {
                SlideDirection.Left => _isExpanded
                    ? "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-left.svg"
                    : "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-right.svg",
                SlideDirection.Right => _isExpanded
                    ? "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-right.svg"
                    : "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-left.svg",
                SlideDirection.Bottom => _isExpanded
                    ? "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-up.svg"
                    : "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-down.svg",
                _ => string.Empty,
            };

            _dropDownButton.ImagePath = iconPath;
        }

        private void UpdateMenuPosition()
        {
            if (Parent == null) return;

            var globalPosition = this.PointToScreen(Point.Empty);

            switch (_flyoutDirection)
            {
                case SlideDirection.Bottom:
                    _menu.Location = new Point(globalPosition.X, globalPosition.Y + Height);
                    break;
                case SlideDirection.Right:
                    _menu.Location = new Point(globalPosition.X + Width, globalPosition.Y);
                    break;
                case SlideDirection.Left:
                    _menu.Location = new Point(globalPosition.X - _flyoutMenuWidth, globalPosition.Y);
                    break;
                default:
                    break;
            }
        }

        private void ToggleMenu(object sender, EventArgs e)
        {
            if (!_isExpanded)
            {
                if (items.Count == 0)
                {
                    MessageBox.Show("No rootnodeitems available.");
                    return;
                }
                ShowMenu();
            }
            else
            {
                HideMenu();
            }
        }

        private void ShowMenu()
        {
            _isExpanded = true;
            _popupOpen = true;
            UpdateButtonIcon();
            UpdateLabelVisibility();
            UpdateMenuPosition();
            _menu.ListItems = items;
            _menu.Width = _flyoutMenuWidth;
            _menu.Height = Math.Min(items.Count * _menu.MenuItemHeight + 5, _maxMenuHeight);

            var parentForm = FindForm();
            if (parentForm != null)
            {
                parentForm.Controls.Add(_menu);
            }
            _menu.Visible = true;
            _menu.BringToFront();

            OnPopupOpened(EventArgs.Empty);
        }

        private void HideMenu()
        {
            _isExpanded = false;
            _popupOpen = false;
            UpdateButtonIcon();
            UpdateLabelVisibility();

            _menu.Visible = false;

            var parentForm = FindForm();
            if (parentForm != null && _menu.Parent?.Equals(parentForm) == true)
            {
                parentForm.Controls.Remove(_menu);
            }

            OnPopupClosed(EventArgs.Empty);
        }

        public void CloseChildPopup()
        {
            if (_popupOpen)
            {
                HideMenu();
            }
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

            UpdateMenuPosition();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_dropDownButton != null)
            {
                _dropDownButton.Location = new Point(DrawingRect.Width - _dropDownButton.Width - 5, (DrawingRect.Height - _dropDownButton.Height) / 2);
            }
            if (_menu != null)
            {
                UpdateControlLayout();
            }
        }
    }
}
