using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes
{
    public partial class BeepCheckBox<T>
    {
        private void ClearGraphicsCaches()
        {
            foreach (var brush in _brushCache.Values)
            {
                brush?.Dispose();
            }
            _brushCache.Clear();

            foreach (var pen in _penCache.Values)
            {
                pen?.Dispose();
            }
            _penCache.Clear();

            foreach (var path in _pathCache.Values)
            {
                path?.Dispose();
            }
            _pathCache.Clear();
        }

        private Rectangle GetDirtyRegion(bool includeText)
        {
            Rectangle dirty = _lastCheckBoxRect;
            if (includeText)
            {
                dirty = dirty.IsEmpty ? _lastTextRect : Rectangle.Union(dirty, _lastTextRect);
            }

            if (dirty.IsEmpty)
            {
                dirty = DrawingRect.IsEmpty ? ClientRectangle : DrawingRect;
            }

            dirty.Inflate(4, 4);
            return dirty;
        }

        private void RequestVisualRefresh(bool includeText)
        {
            _stateChanged = true;
            Invalidate(GetDirtyRegion(includeText));
        }

        private T ResolveCheckedStateValue()
        {
            return _hasCheckedStateValue ? _checkedStateValue : _checkedValue;
        }

        private T ResolveUncheckedStateValue()
        {
            return _hasUncheckedStateValue ? _uncheckedStateValue : _uncheckedValue;
        }

        private T ResolveIndeterminateStateValue()
        {
            if (_hasIndeterminateStateValue)
            {
                return _indeterminateStateValue;
            }

            return _useUncheckedValueForIndeterminate
                ? ResolveUncheckedStateValue()
                : ResolveCheckedStateValue();
        }

        private T ResolveValueForState(CheckBoxState state)
        {
            return state switch
            {
                CheckBoxState.Checked => ResolveCheckedStateValue(),
                CheckBoxState.Indeterminate => ResolveIndeterminateStateValue(),
                _ => ResolveUncheckedStateValue()
            };
        }

        private void UpdateCurrentValue()
        {
            _currentValue = ResolveValueForState(_state);
            OnStateChanged();
        }

        private void UpdateStateFromValue()
        {
            if (EqualityComparer<T>.Default.Equals(_currentValue, ResolveCheckedStateValue()))
            {
                _state = CheckBoxState.Checked;
            }
            else if (EqualityComparer<T>.Default.Equals(_currentValue, ResolveUncheckedStateValue()))
            {
                _state = CheckBoxState.Unchecked;
            }
            else if (EqualityComparer<T>.Default.Equals(_currentValue, ResolveIndeterminateStateValue()))
            {
                _state = CheckBoxState.Indeterminate;
            }
            else
            {
                if (typeof(T) == typeof(bool))
                {
                    bool current = Convert.ToBoolean(_currentValue);
                    _state = current ? CheckBoxState.Checked : CheckBoxState.Unchecked;
                }
                else if (_currentValue == null)
                {
                    _state = CheckBoxState.Indeterminate;
                }
                else
                {
                    _state = CheckBoxState.Indeterminate;
                }
            }
            RequestVisualRefresh(includeText: true);
        }

        private void OnStateChanged()
        {
            // Only update CurrentValue if necessary, avoid recursion
            T newValue = ResolveValueForState(State);
            _currentValue = newValue; // Set directly, bypass setter to avoid recursion
            StateChanged?.Invoke(this, EventArgs.Empty);
            RequestVisualRefresh(includeText: true);
        }

        private void ToggleState()
        {
            switch (State)
            {
                case CheckBoxState.Unchecked:
                    State = CheckBoxState.Checked;
                    break;
                case CheckBoxState.Checked:
                    State = CheckBoxState.Indeterminate;
                    break;
                case CheckBoxState.Indeterminate:
                    State = CheckBoxState.Unchecked;
                    break;
            }
        }



        #region Helper Methods
        // Optimized color selection
        private Color GetCheckBoxBackColor()
        {
            return _state == CheckBoxState.Checked
                ? CheckBoxThemeHelpers.GetCheckedBackgroundColor(_currentTheme, UseThemeColors)
                : CheckBoxThemeHelpers.GetUncheckedBackgroundColor(_currentTheme, UseThemeColors);
        }
        #endregion
    }
}
