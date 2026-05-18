using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        public new void UpdateLayout()
        {
            base.UpdateLayout();

            if (!_controlsInitialized || _layout == null)
            {
                return;
            }

            Rectangle contentRect = GetCalendarLayoutRect();
            if (contentRect.Width <= 0 || contentRect.Height <= 0)
            {
                return;
            }

            ApplyResponsiveButtonLabels(contentRect.Width);
            PrepareToolbarButtons();

            int selectorButtonHeight = GetToolbarButtonHeight(
                _monthViewButton,
                _weekViewButton,
                _workWeekViewButton,
                _dayViewButton,
                _agendaViewButton,
                _timelineViewButton,
                _listViewButton,
                _createEventButton,
                _duplicateEventButton,
                _editEventButton,
                _deleteEventButton);

            int sidebarWidth = GetResponsiveSidebarWidth(contentRect.Width);
            _layout.UpdateLayout(contentRect, selectorButtonHeight, sidebarWidth, ScaleMetric(Math.Max(0, GridLeftGutter)), GetMetricScale());

            SuspendLayout();
            try
            {
                LayoutHeaderButtons();
                LayoutViewSelectorButtons(selectorButtonHeight);
            }
            finally
            {
                ResumeLayout(false);
            }

            UpdateViewButtonStates();
        }

        private Rectangle GetCalendarLayoutRect()
        {
            Rectangle rect = GetContentRectForDrawing();
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                rect = ClientRectangle;
            }

            int margin = ScaleMetric(CalendarLayoutMetrics.OuterMargin);
            if (rect.Width > margin * 2 && rect.Height > margin * 2)
            {
                rect.Inflate(-margin, -margin);
            }

            return rect;
        }

        private void PrepareToolbarButtons()
        {
            foreach (var button in GetToolbarButtons().Where(button => button != null))
            {
                button.Theme = Theme;
                button.TextAlign = ContentAlignment.MiddleCenter;
                button.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            }
        }

        private int GetResponsiveSidebarWidth(int availableWidth)
        {
            if (!_state.ShowSidebar)
            {
                return 0;
            }

            int preferred = ScaleMetric(CalendarLayoutMetrics.SidebarWidth);
            int minimum = ScaleMetric(220);
            int width = Math.Min(preferred, Math.Max(0, availableWidth / 3));
            return width >= minimum ? width : 0;
        }

        private int GetToolbarButtonHeight(params BeepButton[] buttons)
        {
            int minHeight = ScaleMetric(32);
            int maxHeight = ScaleMetric(40);
            int preferred = buttons
                .Where(button => button != null)
                .Select(button => button.GetPreferredSize(Size.Empty).Height)
                .DefaultIfEmpty(minHeight)
                .Max();

            return Math.Max(minHeight, Math.Min(maxHeight, preferred));
        }

        private void LayoutHeaderButtons()
        {
            Rectangle header = _rects.HeaderRect;
            if (header.Width <= 0 || header.Height <= 0)
            {
                return;
            }

            int padding = ScaleMetric(8);
            int spacing = ScaleMetric(6);
            int buttonHeight = Math.Min(ScaleMetric(36), Math.Max(1, header.Height - (padding * 2)));
            int y = header.Y + Math.Max(0, (header.Height - buttonHeight) / 2);
            int x = header.X + padding;
            int rightLimit = header.Right - padding;

            x = PlaceButtonLeft(_prevButton, x, y, ScaleMetric(36), buttonHeight, rightLimit, spacing);
            x = PlaceButtonLeft(_nextButton, x, y, ScaleMetric(36), buttonHeight, rightLimit, spacing);
            x = PlaceButtonLeft(_todayButton, x, y, GetButtonWidth(_todayButton, 64, 96), buttonHeight, rightLimit, spacing);
            x = PlaceButtonLeft(_undoButton, x, y, GetButtonWidth(_undoButton, 44, 82), buttonHeight, rightLimit, spacing);
            PlaceButtonLeft(_redoButton, x, y, GetButtonWidth(_redoButton, 44, 82), buttonHeight, rightLimit, spacing);
        }

        private void LayoutViewSelectorButtons(int preferredButtonHeight)
        {
            Rectangle selector = _rects.ViewSelectorRect;
            if (selector.Width <= 0 || selector.Height <= 0)
            {
                return;
            }

            int paddingX = ScaleMetric(8);
            int paddingY = ScaleMetric(6);
            int spacing = ScaleMetric(6);
            int buttonHeight = Math.Min(preferredButtonHeight, Math.Max(1, selector.Height - (paddingY * 2)));
            int y = selector.Y + Math.Max(0, (selector.Height - buttonHeight) / 2);

            int right = selector.Right - paddingX;
            right = PlaceButtonRight(_createEventButton, right, y, GetButtonWidth(_createEventButton, 44, 150), buttonHeight, selector.X + paddingX, spacing);
            right = PlaceButtonRight(_deleteEventButton, right, y, GetButtonWidth(_deleteEventButton, 36, 86), buttonHeight, selector.X + paddingX, spacing);
            right = PlaceButtonRight(_editEventButton, right, y, GetButtonWidth(_editEventButton, 36, 76), buttonHeight, selector.X + paddingX, spacing);
            right = PlaceButtonRight(_duplicateEventButton, right, y, GetButtonWidth(_duplicateEventButton, 44, 102), buttonHeight, selector.X + paddingX, spacing);

            int left = selector.X + paddingX;
            int viewRightLimit = Math.Max(left, right - spacing);
            foreach (var button in new[]
            {
                _monthViewButton,
                _weekViewButton,
                _workWeekViewButton,
                _dayViewButton,
                _agendaViewButton,
                _timelineViewButton,
                _listViewButton
            })
            {
                left = PlaceButtonLeft(button, left, y, GetButtonWidth(button, 38, 110), buttonHeight, viewRightLimit, spacing);
            }
        }

        private int PlaceButtonLeft(BeepButton button, int x, int y, int width, int height, int rightLimit, int spacing)
        {
            if (button == null)
            {
                return x;
            }

            if (x + width > rightLimit)
            {
                button.Visible = false;
                return x;
            }

            button.Visible = true;
            button.Bounds = new Rectangle(x, y, Math.Max(1, width), Math.Max(1, height));
            return x + width + spacing;
        }

        private int PlaceButtonRight(BeepButton button, int right, int y, int width, int height, int leftLimit, int spacing)
        {
            if (button == null)
            {
                return right;
            }

            int x = right - width;
            if (x < leftLimit)
            {
                button.Visible = false;
                return right;
            }

            button.Visible = true;
            button.Bounds = new Rectangle(x, y, Math.Max(1, width), Math.Max(1, height));
            return x - spacing;
        }

        private int GetButtonWidth(BeepButton button, int minWidth, int maxWidth)
        {
            if (button == null)
            {
                return ScaleMetric(minWidth);
            }

            Size preferred = button.GetPreferredSize(Size.Empty);
            int measured = Math.Max(preferred.Width, TextRenderer.MeasureText(button.Text ?? string.Empty, button.Font ?? Font).Width + ScaleMetric(22));
            return Math.Max(ScaleMetric(minWidth), Math.Min(ScaleMetric(maxWidth), measured));
        }

        private BeepButton[] GetToolbarButtons()
        {
            return new[]
            {
                _prevButton,
                _nextButton,
                _todayButton,
                _undoButton,
                _redoButton,
                _monthViewButton,
                _weekViewButton,
                _workWeekViewButton,
                _dayViewButton,
                _agendaViewButton,
                _timelineViewButton,
                _listViewButton,
                _createEventButton,
                _duplicateEventButton,
                _editEventButton,
                _deleteEventButton
            };
        }

        private void UpdateViewButtonStates()
        {
            if (_monthViewButton == null) return;

            Color selectedColor = _currentTheme?.CalendarSelectedDateBackColor ?? _currentTheme?.PrimaryColor ?? _currentTheme?.AccentColor ?? Color.FromArgb(66, 133, 244);
            Color normalColor = _currentTheme?.CalendarBackColor ?? _currentTheme?.SurfaceColor ?? _currentTheme?.BackColor ?? BackColor;
            Color selectedForeColor = _currentTheme?.CalendarSelectedDateForColor ?? _currentTheme?.OnPrimaryColor ?? Color.White;
            Color normalForeColor = _currentTheme?.CalendarForeColor ?? _currentTheme?.ForeColor ?? ForeColor;

            _monthViewButton.BackColor = _state.ViewMode == CalendarViewMode.Month ? selectedColor : normalColor;
            _monthViewButton.ForeColor = _state.ViewMode == CalendarViewMode.Month ? selectedForeColor : normalForeColor;
            _weekViewButton.BackColor  = _state.ViewMode == CalendarViewMode.Week  ? selectedColor : normalColor;
            _weekViewButton.ForeColor  = _state.ViewMode == CalendarViewMode.Week  ? selectedForeColor : normalForeColor;
            _workWeekViewButton.BackColor = _state.ViewMode == CalendarViewMode.WorkWeek ? selectedColor : normalColor;
            _workWeekViewButton.ForeColor = _state.ViewMode == CalendarViewMode.WorkWeek ? selectedForeColor : normalForeColor;
            _dayViewButton.BackColor   = _state.ViewMode == CalendarViewMode.Day   ? selectedColor : normalColor;
            _dayViewButton.ForeColor   = _state.ViewMode == CalendarViewMode.Day   ? selectedForeColor : normalForeColor;
            _agendaViewButton.BackColor = _state.ViewMode == CalendarViewMode.Agenda ? selectedColor : normalColor;
            _agendaViewButton.ForeColor = _state.ViewMode == CalendarViewMode.Agenda ? selectedForeColor : normalForeColor;
            _timelineViewButton.BackColor = _state.ViewMode == CalendarViewMode.Timeline ? selectedColor : normalColor;
            _timelineViewButton.ForeColor = _state.ViewMode == CalendarViewMode.Timeline ? selectedForeColor : normalForeColor;
            _listViewButton.BackColor  = _state.ViewMode == CalendarViewMode.List  ? selectedColor : normalColor;
            _listViewButton.ForeColor  = _state.ViewMode == CalendarViewMode.List  ? selectedForeColor : normalForeColor;

            if (_undoButton != null)
            {
                _undoButton.Enabled = CanUndo;
            }

            if (_redoButton != null)
            {
                _redoButton.Enabled = CanRedo;
            }

            if (_duplicateEventButton != null)
            {
                _duplicateEventButton.Enabled = _state.SelectedEvent != null;
            }

            if (_editEventButton != null)
            {
                _editEventButton.Enabled = _state.SelectedEvent != null;
            }

            if (_deleteEventButton != null)
            {
                _deleteEventButton.Enabled = _state.SelectedEvent != null;
            }
        }

    }
}
