using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    public partial class BeepSideBar
    {
        #region Event Fields
        private SimpleItem _hoveredItem;
        private Point _lastMousePosition;
        private bool _showToggleButton = true;
        #endregion

        #region Event Properties
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Category("Behavior")]
        [System.ComponentModel.Description("Show the collapse/expand toggle button.")]
        [System.ComponentModel.DefaultValue(true)]
        public bool ShowToggleButton
        {
            get => _showToggleButton;
            set
            {
                _showToggleButton = value;
                Invalidate();
            }
        }
        #endregion

        #region Mouse Events
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            _lastMousePosition = e.Location;
            SimpleItem previousHovered = _hoveredItem;
            _hoveredItem = GetItemAtPoint(e.Location);

            if (_hoveredItem != previousHovered)
            {
                Invalidate();
            }

            // Update cursor
            Cursor = _hoveredItem != null ? Cursors.Hand : Cursors.Default;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveredItem != null)
            {
                _hoveredItem = null;
                Invalidate();
            }

            Cursor = Cursors.Default;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button != MouseButtons.Left)
                return;

            // Check if toggle button was clicked
            if (IsPointInToggleButton(e.Location))
            {
                Toggle();
                return;
            }

            // Check if an expand/collapse icon was clicked
            var itemWithIcon = GetItemWithExpandIconAtPoint(e.Location);
            if (itemWithIcon != null)
            {
                ToggleItemExpansion(itemWithIcon);
                return;
            }

            // Check if a menu item was clicked
            var clickedItem = GetItemAtPoint(e.Location);
            if (clickedItem != null)
            {
                if (ClickTogglesExpansion && clickedItem.Children != null && clickedItem.Children.Count > 0)
                {
                    switch (ClickTogglesExpansionMode)
                    {
                        case ClickTogglesExpansionMode.ToggleThenSelect:
                            ToggleItemExpansion(clickedItem);
                            OnItemClicked(clickedItem);
                            break;
                        case ClickTogglesExpansionMode.SelectThenToggle:
                            OnItemClicked(clickedItem);
                            ToggleItemExpansion(clickedItem);
                            break;
                        case ClickTogglesExpansionMode.ToggleOnly:
                        default:
                            ToggleItemExpansion(clickedItem);
                            break;
                    }
                }
                else
                {
                    OnItemClicked(clickedItem);
                }
            }
        }
        #endregion

        #region Keyboard Events
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (_items == null || _items.Count == 0)
                return;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    SelectPreviousItem();
                    e.Handled = true;
                    break;

                case Keys.Down:
                    SelectNextItem();
                    e.Handled = true;
                    break;

                case Keys.Enter:
                case Keys.Space:
                    if (_selectedItem != null)
                    {
                        if (_selectedItem.Children != null && _selectedItem.Children.Count > 0)
                        {
                            if (ClickTogglesExpansion)
                            {
                                switch (ClickTogglesExpansionMode)
                                {
                                    case ClickTogglesExpansionMode.ToggleThenSelect:
                                        ToggleItemExpansion(_selectedItem);
                                        OnItemClicked(_selectedItem);
                                        break;
                                    case ClickTogglesExpansionMode.SelectThenToggle:
                                        OnItemClicked(_selectedItem);
                                        ToggleItemExpansion(_selectedItem);
                                        break;
                                    case ClickTogglesExpansionMode.ToggleOnly:
                                    default:
                                        ToggleItemExpansion(_selectedItem);
                                        break;
                                }
                            }
                            else
                            {
                                ToggleItemExpansion(_selectedItem);
                            }
                        }
                        else
                        {
                            OnItemClicked(_selectedItem);
                        }
                    }
                    e.Handled = true;
                    break;

                case Keys.Left:
                    if (_selectedItem != null && IsItemExpanded(_selectedItem))
                    {
                        CollapseItem(_selectedItem);
                    }
                    else if (!IsCollapsed)
                    {
                        IsCollapsed = true;
                    }
                    e.Handled = true;
                    break;

                case Keys.Right:
                    if (IsCollapsed)
                    {
                        IsCollapsed = false;
                    }
                    else if (_selectedItem != null && _selectedItem.Children != null && _selectedItem.Children.Count > 0 && !IsItemExpanded(_selectedItem))
                    {
                        ExpandItem(_selectedItem);
                    }
                    e.Handled = true;
                    break;

                case Keys.Home:
                    if (_items.Count > 0)
                    {
                        SelectedItem = _items[0];
                    }
                    e.Handled = true;
                    break;

                case Keys.End:
                    if (_items.Count > 0)
                    {
                        SelectedItem = _items[_items.Count - 1];
                    }
                    e.Handled = true;
                    break;
            }
        }
        #endregion

        #region Navigation Methods
        private void SelectPreviousItem()
        {
            var flatList = GetFlatItemList();
            if (flatList.Count == 0) return;

            int currentIndex = _selectedItem != null ? flatList.IndexOf(_selectedItem) : -1;
            int newIndex = currentIndex > 0 ? currentIndex - 1 : flatList.Count - 1;
            SelectedItem = flatList[newIndex];
        }

        private void SelectNextItem()
        {
            var flatList = GetFlatItemList();
            if (flatList.Count == 0) return;

            int currentIndex = _selectedItem != null ? flatList.IndexOf(_selectedItem) : -1;
            int newIndex = (currentIndex + 1) % flatList.Count;
            SelectedItem = flatList[newIndex];
        }
        #endregion
    }
}
