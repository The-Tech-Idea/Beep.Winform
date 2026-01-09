using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes
{
    public partial class BeepCheckBox<T>
    {
        private void UpdateCurrentValue()
        {
            _currentValue = _state == CheckBoxState.Checked ? _checkedValue : _uncheckedValue;
            OnStateChanged();
        }

        private void UpdateStateFromValue()
        {
            if (typeof(T) == typeof(bool))
            {
                bool current = Convert.ToBoolean(_currentValue);
                bool checkedVal = Convert.ToBoolean(_checkedValue);
                bool uncheckedVal = Convert.ToBoolean(_uncheckedValue);

                if (current)
                {
                    _state = CheckBoxState.Checked;
                }
                else if (current == uncheckedVal)
                {
                    _state = CheckBoxState.Unchecked;
                }
                else
                {
                    _state = CheckBoxState.Indeterminate;
                }
            }
            else
            {
                if (_currentValue == null)
                {
                    _state = CheckBoxState.Indeterminate;
                }
                else if (EqualityComparer<T>.Default.Equals(_currentValue, _checkedValue))
                {
                    _state = CheckBoxState.Checked;
                }
                else if (EqualityComparer<T>.Default.Equals(_currentValue, _uncheckedValue))
                {
                    _state = CheckBoxState.Unchecked;
                }
                else
                {
                    _state = CheckBoxState.Indeterminate;
                }
            }
            Invalidate(); // Redraw without calling OnStateChanged
        }

        private void OnStateChanged()
        {
            // Only update CurrentValue if necessary, avoid recursion
            T newValue = State == CheckBoxState.Checked ? CheckedValue : UncheckedValue;
            _currentValue = newValue; // Set directly, bypass setter to avoid recursion
            StateChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
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
            OnStateChanged();
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            // Apply font theme based on ControlStyle
            CheckBoxFontHelpers.ApplyFontTheme(ControlStyle);

            if (Theme != null && _currentTheme != null)
            {
                if (_beepImage != null)
                    _beepImage.Theme = Theme;

                // Use theme helpers for consistent color retrieval
                ForeColor = CheckBoxThemeHelpers.GetForegroundColor(
                    _currentTheme,
                    UseThemeColors);
                BackColor = _currentTheme.BackColor;

                // Update checkbox size and spacing based on style
                checkboxsize = CheckBoxStyleHelpers.GetRecommendedCheckBoxSize(_checkBoxStyle);
                Spacing = CheckBoxStyleHelpers.GetRecommendedSpacing(_checkBoxStyle);
                Padding = new Padding(CheckBoxStyleHelpers.GetRecommendedPadding(_checkBoxStyle));

                if (UseThemeFont)
                {
                    _textFont = _currentTheme.CheckBoxFont != null
                        ? BeepThemesManager.ToFont(_currentTheme.CheckBoxFont)
                        : CheckBoxFontHelpers.GetCheckBoxFont(ControlStyle);
                }
            }
            Invalidate();
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
