using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton
{
    public partial class BeepAdvancedButton
    {
        /// <summary>
        /// Register or clear hit areas for split buttons.
        /// </summary>
        private void RegisterSplitButtonAreas()
        {
            _hitTest?.HitList.RemoveAll(h => h.Name == "LeftButtonArea" || h.Name == "RightButtonArea");

            if (_buttonShape == ButtonShape.Split && Width > 0 && Height > 0)
            {
                int halfWidth = Width / 2;
                Rectangle leftArea = new Rectangle(0, 0, halfWidth, Height);
                _hitTest?.AddHitArea("LeftButtonArea", leftArea, component: null!, () =>
                {
                    HandleLeftAreaClick();
                });

                Rectangle rightArea = new Rectangle(halfWidth, 0, halfWidth, Height);
                _hitTest?.AddHitArea("RightButtonArea", rightArea, component: null!, () =>
                {
                    HandleRightAreaClick();
                });
            }
        }

        private void HandleLeftAreaClick()
        {
            if (!Enabled) return;

            if (_buttonStyle == AdvancedButtonStyle.Toggle)
            {
                bool oldValue = IsToggled;
                IsToggled = true;
                if (oldValue != IsToggled)
                {
                    ToggleChanged?.Invoke(this, new ToggleChangedEventArgs(IsToggled, "Left"));
                }
            }

            LeftAreaClicked?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        private void HandleRightAreaClick()
        {
            if (!Enabled) return;

            if (_buttonStyle == AdvancedButtonStyle.Toggle)
            {
                bool oldValue = IsToggled;
                IsToggled = false;
                if (oldValue != IsToggled)
                {
                    ToggleChanged?.Invoke(this, new ToggleChangedEventArgs(IsToggled, "Right"));
                }
            }

            RightAreaClicked?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        private void UpdateAreaHoverStates(Point mouseLocation)
        {
            if (_buttonShape == ButtonShape.Split && Width > 0)
            {
                int halfWidth = Width / 2;
                Rectangle clientRect = ClientRectangle;

                bool wasLeftHovered = _leftAreaHovered;
                bool wasRightHovered = _rightAreaHovered;

                _leftAreaHovered = mouseLocation.X < halfWidth && clientRect.Contains(mouseLocation);
                _rightAreaHovered = mouseLocation.X >= halfWidth && clientRect.Contains(mouseLocation);

                if (wasLeftHovered != _leftAreaHovered || wasRightHovered != _rightAreaHovered)
                {
                    Invalidate();
                }
            }
        }

        private ButtonShape GetDefaultShapeForStyle(AdvancedButtonStyle style)
        {
            return style switch
            {
                AdvancedButtonStyle.FAB => ButtonShape.Circle,
                AdvancedButtonStyle.Ghost => ButtonShape.Pill,
                AdvancedButtonStyle.Toggle => ButtonShape.Split,
                AdvancedButtonStyle.Solid => ButtonShape.RoundedRectangle,
                AdvancedButtonStyle.Icon => ButtonShape.RoundedRectangle,
                AdvancedButtonStyle.Text => ButtonShape.RoundedRectangle,
                AdvancedButtonStyle.Outlined => ButtonShape.RoundedRectangle,
                AdvancedButtonStyle.Link => ButtonShape.Rectangle,
                AdvancedButtonStyle.Gradient => ButtonShape.RoundedRectangle,
                AdvancedButtonStyle.IconText => ButtonShape.RoundedRectangle,
                AdvancedButtonStyle.FlatWeb => ButtonShape.Rectangle,
                AdvancedButtonStyle.LowerThird => ButtonShape.Rectangle,
                AdvancedButtonStyle.StickerLabel => ButtonShape.RoundedRectangle,
                _ => ButtonShape.RoundedRectangle
            };
        }

        private void UpdateButtonSize()
        {
            Size newSize = _buttonSize switch
            {
                AdvancedButtonSize.Small => _smallSize,
                AdvancedButtonSize.Medium => _mediumSize,
                AdvancedButtonSize.Large => _largeSize,
                _ => _mediumSize
            };

            if (_buttonStyle == AdvancedButtonStyle.FAB)
            {
                int size = _buttonSize switch
                {
                    AdvancedButtonSize.Small => 40,
                    AdvancedButtonSize.Medium => 56,
                    AdvancedButtonSize.Large => 72,
                    _ => 56
                };
                int scaledFab = DpiScalingHelper.ScaleValue(size, this);
                newSize = new Size(scaledFab, scaledFab);
            }
            else
            {
                newSize = DpiScalingHelper.ScaleSize(newSize, this);
            }

            Size = newSize;
        }
    }
}
