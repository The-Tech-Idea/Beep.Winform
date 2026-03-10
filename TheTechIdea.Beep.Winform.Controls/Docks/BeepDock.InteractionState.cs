using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepDock
    {
        private void SetHoveredIndex(int index, bool fromKeyboard = false)
        {
            if (index < -1 || index >= _itemStates.Count)
            {
                index = -1;
            }

            if (_hoveredIndex == index)
            {
                return;
            }

            for (int i = 0; i < _itemStates.Count; i++)
            {
                _itemStates[i].IsHovered = i == index;
                if (!fromKeyboard && i != index)
                {
                    _itemStates[i].IsFocused = false;
                }
            }

            _hoveredIndex = index;
            if (index >= 0 && index < _itemStates.Count)
            {
                ItemHovered?.Invoke(this, new DockItemEventArgs(_itemStates[index].Item, index));
                QueueTooltipForIndex(index);
            }
            else
            {
                HideDockTooltip();
            }
        }

        private void SetPressedIndex(int index)
        {
            _pressedIndex = index;
            for (int i = 0; i < _itemStates.Count; i++)
            {
                _itemStates[i].IsPressed = i == index;
            }
        }

        private void SetFocusedIndex(int index)
        {
            if (index < -1 || index >= _itemStates.Count)
            {
                index = -1;
            }

            _focusedIndex = index;
            for (int i = 0; i < _itemStates.Count; i++)
            {
                _itemStates[i].IsFocused = i == index;
            }

            if (index >= 0)
            {
                SetHoveredIndex(index, fromKeyboard: true);
            }
        }

        private static bool IsItemDisabled(SimpleItem? item)
        {
            return item != null && (!item.IsVisible || !item.IsEnabled);
        }

        private void UpdateDisabledStates()
        {
            foreach (var state in _itemStates)
            {
                state.IsDisabled = IsItemDisabled(state.Item);
            }
        }

        private void HoverIntentTimer_Tick(object? sender, EventArgs e)
        {
            _hoverIntentTimer.Stop();
            if (_config.ShowTooltips && _hoveredIndex >= 0 && _hoveredIndex < _itemStates.Count)
            {
                ShowDockTooltip(_hoveredIndex);
            }
        }

        private void QueueTooltipForIndex(int index)
        {
            _hoverIntentTimer.Stop();
            HideDockTooltip();

            if (!_config.ShowTooltips || index < 0 || index >= _itemStates.Count)
            {
                return;
            }

            _hoverIntentTimer.Interval = Math.Max(0, _config.HoverEnterDelay);
            _hoverIntentTimer.Start();
        }

        private void ShowDockTooltip(int index)
        {
            if (index < 0 || index >= _itemStates.Count)
            {
                return;
            }

            var state = _itemStates[index];
            if (state.IsDisabled)
            {
                return;
            }

            HideDockTooltip();
            var center = PointToScreen(new Point(state.Bounds.Left + state.Bounds.Width / 2, state.Bounds.Top));
            _activeTooltip = new Docks.BeepDockTooltip(state.Item, _currentTheme);
            _activeTooltip.ShowTooltip(center, delay: 0);
        }

        private void HideDockTooltip()
        {
            if (_activeTooltip == null)
            {
                return;
            }

            try
            {
                _activeTooltip.HideTooltip();
            }
            catch
            {
                _activeTooltip.Dispose();
            }

            _activeTooltip = null;
        }
    }
}
