using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes
{
    public partial class BeepCheckBox<T>
    {
        // Cache lifecycle policy (BCHK-P5-001):
        //   _brushCache / _penCache / _pathCache are GDI wrappers created on demand in Draw()
        //   and cleared whenever the theme, DPI, or font changes (ClearGraphicsCaches).
        //   They are also flushed in Dispose(true) below so no GDI handles leak if the
        //   control is removed from a form without a theme-change event preceding it.
        //   _textFont ownership is tracked by _ownsTextFont:
        //     - true when created by theme helpers inside ApplyTheme()
        //     - false when assigned from outside via TextFont property
        //   Dispose(true) only disposes _textFont when _ownsTextFont is true.
        //   _painter is stateless and allocated per CheckBoxStyle change; it holds no GDI resources
        //   and does not need explicit disposal.
        private void ClearGraphicsCaches()
        {
            BeepCheckBoxDiagnostics.RecordCacheRebuild();
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearGraphicsCaches();

                // Dispose only theme-owned fonts created internally by ApplyTheme().
                if (_ownsTextFont && _textFont != null)
                {
                    _textFont.Dispose();
                    _textFont = null;
                    _ownsTextFont = false;
                }
            }

            base.Dispose(disposing);
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
            BeepCheckBoxDiagnostics.RecordInvalidation();
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
            StateChanged?.Invoke(this, EventArgs.Empty);
            RequestVisualRefresh(includeText: true);
        }

        private void UpdateStateFromValue()
        {
            SetStateCore(ResolveStateFromCurrentValue(_currentValue), syncCurrentValue: false, raiseEvents: false, raiseSubmitChanges: false);
        }

        private void SetCurrentValueCore(T value, bool raiseEvents, bool raiseSubmitChanges)
        {
            if (EqualityComparer<T>.Default.Equals(_currentValue, value))
            {
                return;
            }

            _currentValue = value;
            SetStateCore(ResolveStateFromCurrentValue(_currentValue), syncCurrentValue: false, raiseEvents: raiseEvents, raiseSubmitChanges: raiseSubmitChanges);
        }

        private CheckBoxState ResolveStateFromCurrentValue(T currentValue)
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, ResolveCheckedStateValue()))
            {
                return CheckBoxState.Checked;
            }

            if (EqualityComparer<T>.Default.Equals(currentValue, ResolveUncheckedStateValue()))
            {
                return CheckBoxState.Unchecked;
            }

            if (EqualityComparer<T>.Default.Equals(currentValue, ResolveIndeterminateStateValue()))
            {
                return CheckBoxState.Indeterminate;
            }

            if (typeof(T) == typeof(bool))
            {
                bool isChecked = Convert.ToBoolean(currentValue);
                return isChecked ? CheckBoxState.Checked : CheckBoxState.Unchecked;
            }

            return currentValue == null ? CheckBoxState.Indeterminate : CheckBoxState.Indeterminate;
        }

        private void SetStateCore(CheckBoxState newState, bool syncCurrentValue, bool raiseEvents, bool raiseSubmitChanges)
        {
            if (_state == newState)
            {
                if (syncCurrentValue)
                {
                    T resolvedValue = ResolveValueForState(newState);
                    if (!EqualityComparer<T>.Default.Equals(_currentValue, resolvedValue))
                    {
                        _currentValue = resolvedValue;
                        RequestVisualRefresh(includeText: true);
                    }
                }
                // P5-002: No invalidation when state is unchanged and no value sync needed.
                // WinForms will still repaint the control on parent-forced repaints via DrawContent.

                if (raiseSubmitChanges)
                {
                    RaiseSubmitChanges();
                }

                return;
            }

            CheckBoxState previousState = _state;
            bool previousChecked = previousState == CheckBoxState.Checked;

            _state = newState;
            _stateChanged = true;

            if (syncCurrentValue)
            {
                _currentValue = ResolveValueForState(_state);
            }

            if (raiseEvents)
            {
                OnStateChanged(previousState, previousChecked != (_state == CheckBoxState.Checked), raiseSubmitChanges);
                return;
            }

            RequestVisualRefresh(includeText: true);
            if (raiseSubmitChanges)
            {
                RaiseSubmitChanges();
            }
        }

        private void OnStateChanged(CheckBoxState previousState, bool raiseCheckedChanged, bool raiseSubmitChanges)
        {
            CheckStateChanged?.Invoke(this, EventArgs.Empty);
            if (raiseCheckedChanged)
            {
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }

            StateChanged?.Invoke(this, EventArgs.Empty);
            RequestVisualRefresh(includeText: true);

            // Notify assistive technologies of the state change
            if (IsHandleCreated)
            {
                AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
                AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
            }

            if (raiseSubmitChanges)
            {
                RaiseSubmitChanges();
            }
        }

        private void ToggleState()
        {
            BeepCheckBoxDiagnostics.RecordToggle();
            if (!ThreeState)
            {
                State = Checked ? CheckBoxState.Unchecked : CheckBoxState.Checked;
                return;
            }

            switch (State)
            {
                case CheckBoxState.Unchecked:
                    State = CheckBoxState.Checked;
                    break;
                case CheckBoxState.Checked:
                    State = CheckBoxState.Indeterminate;
                    break;
                default:
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
