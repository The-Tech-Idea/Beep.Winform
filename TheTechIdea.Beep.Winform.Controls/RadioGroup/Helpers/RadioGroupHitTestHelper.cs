using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers
{
    /// <summary>
    /// Helper class for managing mouse interactions and hit testing for BeepRadioGroupAdvanced
    /// </summary>
    internal class RadioGroupHitTestHelper : IDisposable
    {
        private readonly BaseControl _owner;
        private readonly ControlHitTestHelper _hitTestHelper;
        
        private List<SimpleItem> _items = new List<SimpleItem>();
        private List<Rectangle> _itemRectangles = new List<Rectangle>();
        private int _hoveredIndex = -1;
        private int _focusedIndex = -1;
        public Func<int, bool> IsItemInteractive { get; set; }

        public RadioGroupHitTestHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _hitTestHelper = new ControlHitTestHelper(_owner);
            
            // Subscribe to hit test events
            _hitTestHelper.HitDetected += OnHitDetected;
        }

        #region Properties
        public int HoveredIndex 
        { 
            get => _hoveredIndex;
            private set
            {
                if (_hoveredIndex != value)
                {
                    _hoveredIndex = value;
                    HoveredIndexChanged?.Invoke(this, new IndexChangedEventArgs(_hoveredIndex));
                }
            }
        }

        public int FocusedIndex
        {
            get => _focusedIndex;
            set
            {
                if (_focusedIndex != value)
                {
                    _focusedIndex = value;
                    FocusedIndexChanged?.Invoke(this, new IndexChangedEventArgs(_focusedIndex));
                }
            }
        }

        public SimpleItem HoveredItem => _hoveredIndex >= 0 && _hoveredIndex < _items.Count ? _items[_hoveredIndex] : null;
        public SimpleItem FocusedItem => _focusedIndex >= 0 && _focusedIndex < _items.Count ? _items[_focusedIndex] : null;
        #endregion

        #region Events
        public event EventHandler<IndexChangedEventArgs> HoveredIndexChanged;
        public event EventHandler<IndexChangedEventArgs> FocusedIndexChanged;
        public event EventHandler<ItemClickEventArgs> ItemClicked;
        public event EventHandler<ItemClickEventArgs> ItemDoubleClicked;
        public event EventHandler<ItemHoverEventArgs> ItemHoverEnter;
        public event EventHandler<ItemHoverEventArgs> ItemHoverLeave;
        #endregion

        #region Setup Methods
        /// <summary>
        /// Updates the items and rectangles for hit testing
        /// </summary>
        public void UpdateItems(List<SimpleItem> items, List<Rectangle> rectangles)
        {
            _items = items ?? new List<SimpleItem>();
            _itemRectangles = rectangles ?? new List<Rectangle>();

            if (_focusedIndex >= _items.Count)
            {
                FocusedIndex = -1;
            }
            if (_hoveredIndex >= _items.Count)
            {
                HoveredIndex = -1;
            }
            
            // Clear existing hit areas
            _hitTestHelper.ClearHitList();
            
            // Add hit areas for each item
            for (int i = 0; i < Math.Min(_items.Count, _itemRectangles.Count); i++)
            {
                var item = _items[i];
                var rect = _itemRectangles[i];
                
                _hitTestHelper.AddHitArea(
                    $"RadioItem_{i}",
                    rect,
                    null,
                    () => HandleItemClick(i, MouseButtons.Left)
                );
            }
        }

        /// <summary>
        /// Clears all hit test areas
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            _itemRectangles.Clear();
            _hitTestHelper.ClearHitList();
            HoveredIndex = -1;
            FocusedIndex = -1;
        }

        public void ResetInteractionState(bool notifyHoverLeave = true)
        {
            if (notifyHoverLeave && _hoveredIndex >= 0 && _hoveredIndex < _items.Count)
            {
                var leaveItem = _items[_hoveredIndex];
                if (leaveItem != null)
                {
                    ItemHoverLeave?.Invoke(this, new ItemHoverEventArgs(_hoveredIndex, leaveItem));
                }
            }

            HoveredIndex = -1;
            FocusedIndex = -1;
        }
        #endregion

        #region Mouse Handling
        /// <summary>
        /// Handles mouse move events to update hover state
        /// </summary>
        public void HandleMouseMove(Point location)
        {
            int newHoverIndex = FindItemAt(location);
            if (newHoverIndex >= 0 && IsItemInteractive != null && !IsItemInteractive(newHoverIndex))
            {
                newHoverIndex = -1;
            }
            
            if (newHoverIndex != _hoveredIndex)
            {
                // Handle hover leave
                if (_hoveredIndex >= 0)
                {
                    var leaveItem = _hoveredIndex < _items.Count ? _items[_hoveredIndex] : null;
                    if (leaveItem != null)
                    {
                        ItemHoverLeave?.Invoke(this, new ItemHoverEventArgs(_hoveredIndex, leaveItem));
                    }
                }
                
                // Update hover index
                HoveredIndex = newHoverIndex;
                
                // Handle hover enter
                if (_hoveredIndex >= 0)
                {
                    var enterItem = _hoveredIndex < _items.Count ? _items[_hoveredIndex] : null;
                    if (enterItem != null)
                    {
                        ItemHoverEnter?.Invoke(this, new ItemHoverEventArgs(_hoveredIndex, enterItem));
                    }
                }
                
                RedrawOwner(); // Request redraw for hover effects
            }
        }

        /// <summary>
        /// Handles mouse leave events
        /// </summary>
        public void HandleMouseLeave()
        {
            if (_hoveredIndex >= 0)
            {
                var leaveItem = _hoveredIndex < _items.Count ? _items[_hoveredIndex] : null;
                if (leaveItem != null)
                {
                    ItemHoverLeave?.Invoke(this, new ItemHoverEventArgs(_hoveredIndex, leaveItem));
                }
            }
            
            HoveredIndex = -1;
            RedrawOwner();
        }

        /// <summary>
        /// Handles mouse click events
        /// </summary>
        public void HandleMouseClick(Point location, MouseButtons button)
        {
            int clickedIndex = FindItemAt(location);
            if (clickedIndex >= 0 && (IsItemInteractive == null || IsItemInteractive(clickedIndex)))
            {
                HandleItemClick(clickedIndex, button);
            }
        }

        /// <summary>
        /// Handles mouse double-click events
        /// </summary>
        public void HandleMouseDoubleClick(Point location, MouseButtons button)
        {
            int clickedIndex = FindItemAt(location);
            if (clickedIndex >= 0 && clickedIndex < _items.Count && (IsItemInteractive == null || IsItemInteractive(clickedIndex)))
            {
                var item = _items[clickedIndex];
                ItemDoubleClicked?.Invoke(this, new ItemClickEventArgs(clickedIndex, item, button));
            }
        }

        private void HandleItemClick(int index, MouseButtons button)
        {
            if (index >= 0 && index < _items.Count)
            {
                if (IsItemInteractive != null && !IsItemInteractive(index))
                {
                    return;
                }

                var item = _items[index];
                
                // Update focus
                FocusedIndex = index;
                
                // Raise click event
                ItemClicked?.Invoke(this, new ItemClickEventArgs(index, item, button));
                
                RedrawOwner();
            }
        }
        #endregion

        #region Keyboard Handling
        /// <summary>
        /// Handles keyboard navigation
        /// </summary>
        public bool HandleKeyDown(Keys keyCode, RadioGroupOrientation orientation)
        {
            if (_items.Count == 0) return false;

            int currentFocus = _focusedIndex >= 0 ? _focusedIndex : FindFirstInteractiveIndex();
            int newFocusIndex = currentFocus;

            switch (keyCode)
            {
                case Keys.Up:
                    if (orientation == RadioGroupOrientation.Vertical || orientation == RadioGroupOrientation.Grid)
                    {
                        newFocusIndex = FindNextInteractiveIndex(currentFocus, -1);
                    }
                    break;

                case Keys.Down:
                    if (orientation == RadioGroupOrientation.Vertical || orientation == RadioGroupOrientation.Grid)
                    {
                        newFocusIndex = FindNextInteractiveIndex(currentFocus, 1);
                    }
                    break;

                case Keys.Left:
                    if (orientation == RadioGroupOrientation.Horizontal || orientation == RadioGroupOrientation.Flow)
                    {
                        newFocusIndex = FindNextInteractiveIndex(currentFocus, -1);
                    }
                    break;

                case Keys.Right:
                    if (orientation == RadioGroupOrientation.Horizontal || orientation == RadioGroupOrientation.Flow)
                    {
                        newFocusIndex = FindNextInteractiveIndex(currentFocus, 1);
                    }
                    break;

                case Keys.Home:
                    newFocusIndex = FindFirstInteractiveIndex();
                    break;

                case Keys.End:
                    newFocusIndex = FindLastInteractiveIndex();
                    break;

                case Keys.Space:
                case Keys.Enter:
                    if (currentFocus >= 0 && (IsItemInteractive == null || IsItemInteractive(currentFocus)))
                    {
                        HandleItemClick(currentFocus, MouseButtons.Left);
                        return true;
                    }
                    break;

                default:
                    return false;
            }

            if (newFocusIndex >= 0 && newFocusIndex != _focusedIndex)
            {
                FocusedIndex = newFocusIndex;
                RedrawOwner();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Moves focus to the next item in tab order
        /// </summary>
        public bool MoveFocusNext()
        {
            if (_items.Count == 0) return false;

            int start = _focusedIndex >= 0 ? _focusedIndex : -1;
            int newIndex = FindNextInteractiveIndex(start, 1, wrap: true);
            if (newIndex != _focusedIndex)
            {
                FocusedIndex = newIndex;
                RedrawOwner();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Moves focus to the previous item in tab order
        /// </summary>
        public bool MoveFocusPrevious()
        {
            if (_items.Count == 0) return false;

            int start = _focusedIndex >= 0 ? _focusedIndex : _items.Count;
            int newIndex = FindNextInteractiveIndex(start, -1, wrap: true);
            if (newIndex != _focusedIndex)
            {
                FocusedIndex = newIndex;
                RedrawOwner();
                return true;
            }
            return false;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Finds the item index at the specified location
        /// </summary>
        private int FindItemAt(Point location)
        {
            for (int i = 0; i < _itemRectangles.Count; i++)
            {
                if (_itemRectangles[i].Contains(location))
                {
                    return i;
                }
            }
            return -1;
        }

        private int FindFirstInteractiveIndex()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (IsItemInteractive == null || IsItemInteractive(i))
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindLastInteractiveIndex()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                if (IsItemInteractive == null || IsItemInteractive(i))
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindNextInteractiveIndex(int startIndex, int step, bool wrap = false)
        {
            if (_items.Count == 0 || step == 0) return -1;

            int attempts = 0;
            int index = startIndex;
            while (attempts < _items.Count)
            {
                index += step;
                if (index < 0 || index >= _items.Count)
                {
                    if (!wrap)
                    {
                        return startIndex;
                    }

                    index = index < 0 ? _items.Count - 1 : 0;
                }

                if (IsItemInteractive == null || IsItemInteractive(index))
                {
                    return index;
                }

                attempts++;
            }

            return startIndex;
        }

        /// <summary>
        /// Gets the rectangle for a specific item index
        /// </summary>
        public Rectangle GetItemRectangle(int index)
        {
            if (index >= 0 && index < _itemRectangles.Count)
                return _itemRectangles[index];
            return Rectangle.Empty;
        }

        /// <summary>
        /// Gets the item at the specified index
        /// </summary>
        public SimpleItem GetItem(int index)
        {
            if (index >= 0 && index < _items.Count)
                return _items[index];
            return null;
        }

        /// <summary>
        /// Scrolls to make the specified item visible
        /// </summary>
        public void EnsureItemVisible(int index)
        {
            if (index < 0 || index >= _itemRectangles.Count)
                return;

            Rectangle itemRect = _itemRectangles[index];
            Rectangle ownerRect = _owner.ClientRectangle;

            // Simple visibility check - more sophisticated scrolling could be added
            if (!ownerRect.IntersectsWith(itemRect))
            {
                // Item is not visible, request scroll
                // This would need to be handled by the parent control
                RedrawOwner();
            }
        }

        private void RedrawOwner()
        {
            if (_owner is BeepRadioGroup radioGroup)
            {
                radioGroup.RequestVisualRefresh();
            }
            else
            {
                _owner.Invalidate();
            }
        }

        private void OnHitDetected(object sender, ControlHitTestArgs e)
        {
            // Handle hit test detection if needed
            // The action is already executed by the hit test helper
        }

        public void Dispose()
        {
            _hitTestHelper.HitDetected -= OnHitDetected;
            Clear();
            IsItemInteractive = null;
            HoveredIndexChanged = null;
            FocusedIndexChanged = null;
            ItemClicked = null;
            ItemDoubleClicked = null;
            ItemHoverEnter = null;
            ItemHoverLeave = null;
        }
        #endregion
    }

    #region Event Args Classes
    public class IndexChangedEventArgs : EventArgs
    {
        public int Index { get; }

        public IndexChangedEventArgs(int index)
        {
            Index = index;
        }
    }

    public class ItemClickEventArgs : EventArgs
    {
        public int Index { get; }
        public SimpleItem Item { get; }
        public MouseButtons Button { get; }

        public ItemClickEventArgs(int index, SimpleItem item, MouseButtons button)
        {
            Index = index;
            Item = item;
            Button = button;
        }
    }

    public class ItemHoverEventArgs : EventArgs
    {
        public int Index { get; }
        public SimpleItem Item { get; }

        public ItemHoverEventArgs(int index, SimpleItem item)
        {
            Index = index;
            Item = item;
        }
    }
    #endregion
}