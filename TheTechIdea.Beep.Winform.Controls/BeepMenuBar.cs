using System;
using System.Collections.Generic;
using System.ComponentModel;

using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.TableLayout;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Card")]
    [Category("Beep Controls")]
    public class BeepMenuBar : BeepControl
    {
        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        private int _selectedIndex = -1;
        private int buttonheight = 20;
        private int buttonwidth = 60;
        private BeepPopupForm _popupForm;
        #region "Properties"

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
                // InitializeMenu();
            }
        }

        public event EventHandler SelectedIndexChanged;

        protected virtual void OnSelectedIndexChanged(EventArgs e) => SelectedIndexChanged?.Invoke(this, e);

        private BeepListBox maindropdownmenu = new BeepListBox();
        private Dictionary<string, BeepButton> menumainbar = new Dictionary<string, BeepButton>();
        private bool _isPopupOpen;

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0)
                {
                    _selectedIndex = value;
                    //  HighlightSelectedButton();
                    OnSelectedIndexChanged(EventArgs.Empty);
                }
            }
        }
        #endregion "Properties"
        public BeepMenuBar()
        {
            if (items == null)
            {
                items = new SimpleItemCollection();
            }
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 200;
                Height = 20;
            }

            items.ListChanged += Items_ListChanged;
            this.Invalidated += BeepListBox_Invalidated;
            _popupForm = new BeepPopupForm();
            BoundProperty = "SelectedItem";
            IsFramless = true;

        }

        protected override void InitLayout()
        {
            base.InitLayout();
            Dock = DockStyle.Top;
            InitMenu();
             ApplyTheme();
        }
        private void BeepListBox_Invalidated(object? sender, InvalidateEventArgs e)
        {

        }

        private void Items_ListChanged(object? sender, ListChangedEventArgs e)
        {
            InitMenu();
        }

        private void InitMenu()
        {
            // Clear contols
            // 
            this.Controls.Clear();
            menumainbar.Clear();
            // Create main menu buttons
            int padding = 1;
            maindropdownmenu.Visible = false;
            maindropdownmenu.Width = Width;
            maindropdownmenu.Height = Height;
            maindropdownmenu.Top = this.Bottom;
            maindropdownmenu.Left = this.Left;
            maindropdownmenu.Visible = false;
            maindropdownmenu.BoundProperty = "SelectedItem";
            maindropdownmenu.SelectedIndexChanged += Maindropdownmenu_SelectedIndexChanged;
            this.Controls.Add(maindropdownmenu);
            //Get Max number of buttons that can fit in the width
            int maxbuttons = (DrawingRect.Width / buttonwidth);
            if (maxbuttons == 0)
            {
                maxbuttons = 1;
            }
            // Get Required width of buttons
            int requiredwidth = items.Count * (buttonwidth * padding);
            // Get Starting point of drawing so that all button are centered
            int startpoint = (DrawingRect.Width - requiredwidth) / 2;
            int i = 0;
            foreach (SimpleItem item in items)
            {
                BeepButton btn = new BeepButton();
                btn.Text = item.Text;
                btn.Tag = item;
                btn.ImagePath = item.ImagePath;
                btn.Width = buttonwidth;
                btn.Height = buttonheight;
                btn.OverrideFontSize = TypeStyleFontSize.Small;
                btn.MaxImageSize = new System.Drawing.Size(16, 16);
                btn.ImageAlign = ContentAlignment.MiddleLeft;
                btn.TextAlign = ContentAlignment.MiddleCenter;
                btn.ApplyThemeOnImage = false;
                btn.IsChild = true;
                btn.ShowAllBorders = false;
                btn.Click -= Btn_Click;
                btn.Click += Btn_Click;
                btn.Anchor = AnchorStyles.Top;
                btn.Left = startpoint + (i * (buttonwidth + padding));
                btn.Top = DrawingRect.Top + padding;
                this.Controls.Add(btn);
                menumainbar.Add(item.Text, btn);
                i++;
            }
        }
        private void ShowPopup(SimpleItem item, Point point)
        {
            BeepListBox _beepListBox = new BeepListBox();
            _beepListBox.ShowTitle = false;
            _beepListBox.ShowAllBorders = false;
            _beepListBox.ShowAllBorders = false;
            _beepListBox.ShowHilightBox  = false;
            _beepListBox.ShowShadow = false;
            _beepListBox.Theme = Theme;
            _beepListBox.ListItems = item.Children;

            if (_isPopupOpen ) {
                _popupForm = new BeepPopupForm();

            };
            _isPopupOpen = true;
          
            // Rebuild beepListBox's layout
            _beepListBox.InitializeMenu();

            int neededHeight = _beepListBox.GetMaxHeight() + 5;
            int finalHeight = neededHeight;
            // possibly also compute width
            int finalWidth = _beepListBox.GetMaxWidth()+5;

            // The popup form is sized to fit beepListBox
            _popupForm.Size = new Size(finalWidth, neededHeight);
            // Position popup just below the main control
            var screenPoint = this.PointToScreen(point);
            _popupForm.Location = screenPoint;
            _beepListBox.Theme = Theme;
            _beepListBox.ShowAllBorders = false;
            //_popupForm.BackColor = _currentTheme.BackColor;
            _popupForm.Theme = Theme;
            _popupForm.Controls.Add(_beepListBox);
            _beepListBox.Dock = DockStyle.Fill; // Manually size and position
            _popupForm.BorderThickness = 2;

            _popupForm.Show();
            _popupForm.BringToFront();
            _popupForm.Invalidate();
        }

        private void ClosePopup()
        {
            if (_popupForm != null)
            {
                _popupForm.Close();
            }
            _isPopupOpen = false;
        }
        private void UnpressAllButtons()
        {
            foreach (var button in menumainbar.Values)
            {
                button.IsSelected = false;
            }
        }
        private void UnpressAllButtonsExcept(BeepButton btn)
        {
            foreach (var button in menumainbar.Values)
            {
                if (button != btn)
                    button.IsSelected = false;
            }
        }

        private void Btn_Click(object? sender, EventArgs e)
        {
         
            BeepButton btn = (BeepButton)sender;
            UnpressAllButtonsExcept(btn);
            SimpleItem item = (SimpleItem)btn.Tag;
            if (_isPopupOpen)
            {
                ClosePopup();
                 _popupForm = new BeepPopupForm();

            }
            if (item.Children.Count>0)
            {
              
                ShowPopup(item,new Point(btn.Left,Height+5));
            }
            else
            {
                SelectedIndex = items.IndexOf(item);
            }
        }

        private void Maindropdownmenu_SelectedIndexChanged(object? sender, EventArgs e)
        {
            
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            BackColor = _currentTheme.SideMenuBackColor;
            foreach (var item in Controls)
            {
                if (item is BeepButton)
                {
                    BeepButton btn = (BeepButton)item;
                    btn.Theme = Theme;
                    btn.ApplyThemeOnImage=false;
                    btn.ForeColor = ColorUtils.GetForColor(BackColor, _currentTheme.ButtonForeColor);
                    //btn.ForeColor = ColorUtils.GetForColor(parentbackcolor, btn.ForeColor);
                }

            }
        }
    }
}