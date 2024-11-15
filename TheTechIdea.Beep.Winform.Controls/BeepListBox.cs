using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ModernSideMenu;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepListBox : BeepPanel
    {
        private List<BeepButton> _buttons = new List<BeepButton>();
        private int _selectedIndex = -1;
        private SimpleMenuItemCollection _menuitems = new SimpleMenuItemCollection();

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleMenuItemCollection ListItems
        {
            get => _menuitems;
            set
            {
                _menuitems = value;
                UpdateButtons();
            }
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0 && value < _buttons.Count)
                {
                    _selectedIndex = value;
                    HighlightSelectedButton();
                    OnSelectedIndexChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler SelectedIndexChanged;

        protected virtual void OnSelectedIndexChanged(EventArgs e) => SelectedIndexChanged?.Invoke(this, e);

        public BeepListBox()
        {
            //AutoScroll = true;
         
        }
        protected override void InitLayout()
        {
            base.InitLayout();
            BorderThickness = 1;
            _menuitems.ListChanged += Items_ListChanged;
            UpdateButtons();
            TitleText = "List Box";

        }
        private void Items_ListChanged(object sender, ListChangedEventArgs e) => UpdateButtons();

        public override void ApplyTheme()
        {
            //base.ApplyTheme();
           
            UpdateButtons(); // Ensure buttons are updated with the new theme
        }

        private void UpdateButtons()
        {
            // Clear previous buttons
            foreach (var button in _buttons)
            {
                button.Click -= Button_Click;
                Controls.Remove(button);
            }

            _buttons.Clear();
            // Calculate TitleBottomY
            CalculateTitleBottomY();
            // Calculate starting position below title line
            int y = TitleBottomY + 10; // 10px offset to avoid overlap with title line

            // Create new buttons based on ListItems collection
            foreach (var item in ListItems)
            {
                var button = new BeepButton
                {
                    Text = item.Text,
                    Width = DrawingRect.Width - 20,
                    Height = 30,
                    IsChild = true,
                    BorderSize = 0,
                    ParentBackColor = _currentTheme.BackgroundColor,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Location = new Point(DrawingRect.Left + 10, y),
                  
                };
                if(_currentTheme != null)
                {
                    //button.BackColor = BackColor;
                    //button.ForeColor = _currentTheme.TitleForColor;
                    //button.Font = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
                    button.Theme = Theme;
                }
                if(item.Image != null)
                {
                    button.MaxImageSize = new Size(24, 24);
                    button.ImagePath = item.Image;
                }
                button.Click += Button_Click;

                _buttons.Add(button);
                Controls.Add(button);
                y += button.Height + 5; // Add spacing for the next button
            }

            //AutoScrollMinSize = new Size(Width, y+10);
            Invalidate();
        }


        private void Button_Click(object sender, EventArgs e)
        {
            if (sender is BeepButton clickedButton)
                SelectedIndex = _buttons.IndexOf(clickedButton);
        }

        private void HighlightSelectedButton()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].BackColor = (i == _selectedIndex)
                    ? _currentTheme?.ButtonActiveBackColor ?? Color.LightBlue
                    : _currentTheme?.ButtonBackColor ?? Color.LightGray;
            }
        }

        //private void ApplyThemeToButton(BeepButton button)
        //{
        //    if (_currentTheme != null)
        //    {
        //        button.BackColor = _currentTheme.ButtonBackColor;
        //        button.ForeColor = _currentTheme.ButtonForeColor;
        //        button.BorderColor = _currentTheme.BorderColor;
        //        button.Font = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
        //    }
        //}
    }
}
