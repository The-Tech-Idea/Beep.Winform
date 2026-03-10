using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Docks.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDock - Item Management
    /// </summary>
    public partial class BeepDock
    {
        #region Item Management
        private void Items_ListChanged(object? sender, ListChangedEventArgs e)
        {
            InitializeItems();
        }

        private void InitializeItems()
        {
            _itemStates.Clear();

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var state = new Docks.DockItemState
                {
                    Item = item,
                    Index = i,
                    CurrentScale = 1.0f,
                    TargetScale = 1.0f,
                    CurrentOpacity = 1.0f,
                    IsHovered = false,
                    IsPressed = false,
                    IsFocused = false,
                    IsDisabled = !item.IsVisible || !item.IsEnabled,
                    IsSelected = item == _selectedItem,
                    Bounds = Rectangle.Empty,
                    HitBounds = Rectangle.Empty
                };
                _itemStates.Add(state);
            }

            UpdateDisabledStates();
            UpdateItemBounds();
            Invalidate();
        }

        private void UpdateSelectionStates()
        {
            foreach (var state in _itemStates)
            {
                state.IsSelected = state.Item == _selectedItem;
            }

            UpdateDisabledStates();
        }

        private void UpdateItemBounds()
        {
            if (_items.Count == 0)
            {
                UpdateDockSize();
                return;
            }

            // Calculate item bounds using layout helper
            var bounds = DockLayoutHelper.CalculateItemBounds(
                ClientRectangle,
                _items.ToList(),
                _config,
                _hoveredIndex,
                1.0f
            );

            // Update item states with calculated bounds
            for (int i = 0; i < bounds.Length && i < _itemStates.Count; i++)
            {
                _itemStates[i].Bounds = bounds[i];
                _itemStates[i].HitBounds = DockHitTestHelper.CalculateHitBounds(bounds[i], 4);
            }

            // Basic overflow affordance for narrow layouts.
            _overflowStartIndex = -1;
            _overflowBounds = Rectangle.Empty;
            if (_config.EnableOverflow && _itemStates.Count > 0)
            {
                var maxExtent = _config.Orientation == Docks.DockOrientation.Horizontal
                    ? ClientRectangle.Width
                    : ClientRectangle.Height;
                var reserved = _config.ItemSize + _config.Spacing;
                var consumed = 0;
                for (int i = 0; i < _itemStates.Count; i++)
                {
                    var s = _itemStates[i];
                    var extent = _config.Orientation == Docks.DockOrientation.Horizontal ? s.Bounds.Width : s.Bounds.Height;
                    consumed += extent + _config.Spacing;
                    if (consumed + reserved > maxExtent)
                    {
                        _overflowStartIndex = i;
                        break;
                    }
                }

                if (_overflowStartIndex >= 0)
                {
                    var lastVisible = _overflowStartIndex - 1;
                    if (lastVisible >= 0 && lastVisible < _itemStates.Count)
                    {
                        var anchor = _itemStates[lastVisible].Bounds;
                        _overflowBounds = _config.Orientation == Docks.DockOrientation.Horizontal
                            ? new Rectangle(anchor.Right + _config.Spacing, anchor.Y, _config.ItemSize, anchor.Height)
                            : new Rectangle(anchor.X, anchor.Bottom + _config.Spacing, anchor.Width, _config.ItemSize);
                    }
                }
            }

            UpdateDockSize();
        }

        private void UpdateDockSize()
        {
            var size = DockLayoutHelper.CalculateDockSize(_items.Count, _config);
            
            if (_config.Orientation == Docks.DockOrientation.Horizontal)
            {
                MinimumSize = new Size(100, size.Height);
                Height = size.Height;
            }
            else
            {
                MinimumSize = new Size(size.Width, 100);
                Width = size.Width;
            }
        }
        #endregion
    }
}
