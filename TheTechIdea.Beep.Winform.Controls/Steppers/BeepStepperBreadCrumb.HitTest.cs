namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepStepperBreadCrumb
    {
        private const string ChevronHitAreaPrefix = "Stepper.Chevron.";

        private static string GetChevronHitAreaName(int index) => $"{ChevronHitAreaPrefix}{index}";

        private static bool TryGetStepIndexFromHitArea(ControlHitTest hitTest, out int stepIndex)
        {
            stepIndex = -1;
            if (hitTest == null || string.IsNullOrWhiteSpace(hitTest.Name) || !hitTest.Name.StartsWith(ChevronHitAreaPrefix))
            {
                return false;
            }

            return int.TryParse(hitTest.Name.Substring(ChevronHitAreaPrefix.Length), out stepIndex);
        }

        private void RegisterChevronHitAreas()
        {
            _hitTest?.ClearHitList();
            if (_hitTest == null || chevronBounds.Count == 0)
            {
                return;
            }

            for (int i = 0; i < chevronBounds.Count; i++)
            {
                _hitTest.AddHitArea(GetChevronHitAreaName(i), chevronBounds[i], null, null);
            }
        }

        private int GetPreciseHoveredIndex(System.Drawing.Point location)
        {
            for (int i = 0; i < chevronPaths.Count; i++)
            {
                using var region = new System.Drawing.Region(chevronPaths[i]);
                if (region.IsVisible(location))
                {
                    return i;
                }
            }

            if (_hitTest != null && _hitTest.HitTest(location, out var hit) && TryGetStepIndexFromHitArea(hit, out int fallbackIndex))
            {
                return fallbackIndex;
            }

            return -1;
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            _hitTest?.HandleMouseMove(e.Location);
            int hoveredIndex = GetPreciseHoveredIndex(e.Location);
            if (hoveredIndex != _hoveredStepIndex)
            {
                _hoveredStepIndex = hoveredIndex;
                UpdateTooltipForHoveredStep();
            }
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);
            _hitTest?.HandleMouseLeave();

            _hoveredStepIndex = -1;
            if (!string.IsNullOrEmpty(_currentTooltipKey))
            {
                _ = ToolTips.ToolTipManager.Instance.HideTooltipAsync(_currentTooltipKey);
                _currentTooltipKey = null;
            }
        }

        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseClick(e);
            IsPressed = false;
            IsSelected = false;

            if (_hitTest == null || !_hitTest.HitTest(e.Location, out _))
            {
                return;
            }

            int stepIndex = GetPreciseHoveredIndex(e.Location);
            if (stepIndex >= 0)
            {
                OnStepClicked(stepIndex);
                ShowStepNotification(stepIndex);
            }
        }
    }
}
