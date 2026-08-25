using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Advanced-painter interaction surface for <see cref="BeepButton"/>: the
    /// dedicated toggle state and split-button left/right hit areas that back
    /// <see cref="AdvancedButtonStyle.Toggle"/> and the <see cref="ButtonShape.Split"/>
    /// shape. Kept in a partial so the main class stays focused on rendering and
    /// classic-button behaviour.
    /// </summary>
    public partial class BeepButton
    {
        // Advanced toggle/split state.
        private bool _isToggled = false;
        private bool _leftAreaHovered = false;
        private bool _rightAreaHovered = false;
        private bool _leftAreaPressed = false;
        private bool _rightAreaPressed = false;

        /// <summary>Raised when the advanced <see cref="AdvancedButtonStyle.Toggle"/> style flips state.</summary>
        public event EventHandler<ToggleChangedEventArgs>? ToggleChanged;

        /// <summary>Raised when the left half of a Split-shape toggle button is clicked.</summary>
        public event EventHandler? LeftAreaClicked;

        /// <summary>Raised when the right half of a Split-shape toggle button is clicked.</summary>
        public event EventHandler? RightAreaClicked;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("For the advanced Toggle style, indicates whether the button is toggled on.")]
        [DefaultValue(false)]
        public bool IsToggled
        {
            get => _isToggled;
            set
            {
                if (_isToggled != value)
                {
                    _isToggled = value;
                    Invalidate();
                }
            }
        }

        /// <summary>Flips the advanced toggle state.</summary>
        public void Toggle()
        {
            IsToggled = !IsToggled;
        }

        /// <summary>
        /// True when the active advanced style resolves to a split shape
        /// (the default shape for <see cref="AdvancedButtonStyle.Toggle"/>).
        /// </summary>
        private bool IsSplitShape()
        {
            return _buttonVisualStyle.HasValue
                && (_shape ?? GetDefaultShapeForStyle(_buttonVisualStyle.Value)) == ButtonShape.Split;
        }

        /// <summary>
        /// Registers, or clears, the left/right hit areas for the split shape.
        /// Clicks are dispatched through BaseControl's hit-action pipeline, which
        /// routes them to <see cref="HandleLeftAreaClick"/> / <see cref="HandleRightAreaClick"/>.
        /// </summary>
        private void RegisterSplitButtonAreas()
        {
            _hitTest?.HitList.RemoveAll(h => h.Name == "LeftButtonArea" || h.Name == "RightButtonArea");

            if (!IsSplitShape() || Width <= 0 || Height <= 0)
            {
                return;
            }

            int halfWidth = Width / 2;
            _hitTest?.AddHitArea("LeftButtonArea", new Rectangle(0, 0, halfWidth, Height), null, HandleLeftAreaClick);
            _hitTest?.AddHitArea("RightButtonArea", new Rectangle(halfWidth, 0, halfWidth, Height), null, HandleRightAreaClick);
        }

        private void HandleLeftAreaClick()
        {
            if (!Enabled) return;

            if (_buttonVisualStyle == AdvancedButtonStyle.Toggle)
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

            if (_buttonVisualStyle == AdvancedButtonStyle.Toggle)
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
            if (!IsSplitShape() || Width <= 0)
            {
                return;
            }

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

        private void UpdateSplitAreaPressedStates(Point mouseLocation)
        {
            if (!IsSplitShape() || Width <= 0)
            {
                _leftAreaPressed = false;
                _rightAreaPressed = false;
                return;
            }

            int halfWidth = Width / 2;
            _leftAreaPressed = mouseLocation.X < halfWidth;
            _rightAreaPressed = mouseLocation.X >= halfWidth;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            UpdateAreaHoverStates(e.Location);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RegisterSplitButtonAreas();
            UpdateChevronRegion();
            UpdateCircleRegion();
        }
    }
}
