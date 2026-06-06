using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private List<CalendarToolbarButton> _toolbarButtons;
        private int _toolbarHoveredIndex = -1;

        private struct CalendarToolbarButton
        {
            public string Key;
            public string IconPath;
            public string Label;
            public Rectangle Bounds;
            public Action Action;
            public bool IsViewSelector;
            // GAP F - the CalendarViewMode this button activates. The
            // toolbar's active-state check (IsViewActive) compares this
            // against _state.ViewMode, so setting ViewMode = Month in code
            // correctly highlights the toolbar button (not just Week1..7
            // keys via painter.Key).
            public CalendarViewMode ViewMode;
        }

        private void InitializeToolbar()
        {
            _toolbarButtons = new List<CalendarToolbarButton>
            {
                new() { Key = "prev",  IconPath = SvgsUIcons.Common.ChevronLeft,  Action = () => NavigatePreviousPeriod() },
                new() { Key = "today", IconPath = SvgsUIcons.Common.Calendar,    Label = "Today", Action = () => GoToToday() },
                new() { Key = "next",  IconPath = SvgsUIcons.Common.ChevronRight, Action = () => NavigateNextPeriod() },
                new() { Key = "spacer1" },
                new() { Key = "undo",  IconPath = SvgsUIcons.Common.Undo, Action = () => UndoMutation() },
                new() { Key = "redo",  IconPath = SvgsUIcons.Common.Redo, Action = () => RedoMutation() },
                new() { Key = "spacer2" },
                new() { Key = "create", IconPath = SvgsUIcons.Common.Add,  Label = "New", Action = () => OnCreateEventRequested(_state.SelectedDate) },
                new() { Key = "edit",   IconPath = SvgsUIcons.Common.Edit, Action = () => EditSelectedEvent() },
                new() { Key = "delete", IconPath = SvgsUIcons.Common.Delete, Action = () => DeleteSelectedEvent() },
            };

            // View-selector buttons are built dynamically from the registered
            // view painters (Week1..Week7). Adding a new view is a factory
            // change only — the toolbar picks it up automatically and routes
            // through the painter's Key/DisplayLabel metadata.
            foreach (var kvp in ViewPainterFactory.GetRegisteredViews())
            {
                CalendarViewMode mode = kvp.Key;
                ICalendarViewPainter painter = kvp.Value;
                _toolbarButtons.Add(new CalendarToolbarButton
                {
                    Key = painter.Key,
                    Label = painter.DisplayLabel,
                    Action = () => SwitchView(mode),
                    IsViewSelector = true,
                    ViewMode = mode
                });
            }
        }

        private void LayoutToolbar(Rectangle headerRect, Rectangle viewSelectorRect)
        {
            if (_toolbarButtons == null) InitializeToolbar();

            int iconSize = ScaleMetric(18);
            int btnHeight = ScaleMetric(28);
            int btnPad = ScaleMetric(6);
            int btnGap = ScaleMetric(4);
            int spacerW = ScaleMetric(8);

            int x = headerRect.X + btnPad;
            int y = headerRect.Y + (headerRect.Height - btnHeight) / 2;

            for (int i = 0; i < _toolbarButtons.Count; i++)
            {
                var btn = _toolbarButtons[i];
                if (btn.IsViewSelector) continue;

                if (btn.Key.StartsWith("spacer"))
                {
                    x += spacerW;
                    _toolbarButtons[i] = btn;
                    continue;
                }

                int w = iconSize + btnPad * 2;
                if (!string.IsNullOrEmpty(btn.Label))
                    w += ScaleMetric(4) + EstimateTextWidth(btn.Label);

                if (!string.IsNullOrEmpty(btn.IconPath))
                    w = Math.Max(w, btnHeight);

                btn.Bounds = new Rectangle(x, y, w, btnHeight);
                _toolbarButtons[i] = btn;
                x += w + btnGap;
            }

            x = viewSelectorRect.X + btnPad;
            y = viewSelectorRect.Y + (viewSelectorRect.Height - btnHeight) / 2;
            int selHeight = Math.Max(btnHeight, ScaleMetric(24));

            for (int i = 0; i < _toolbarButtons.Count; i++)
            {
                var btn = _toolbarButtons[i];
                if (!btn.IsViewSelector) continue;

                int w = btnPad * 2 + EstimateTextWidth(btn.Label);
                w = Math.Max(w, ScaleMetric(60));

                btn.Bounds = new Rectangle(x, y, w, selHeight);
                _toolbarButtons[i] = btn;
                x += w + btnGap;
            }
        }

        private void PaintToolbar(Graphics g)
        {
            if (_toolbarButtons == null || _currentTheme == null) return;

            Color textColor = _currentTheme.PrimaryTextColor;
            Color accentColor = _currentTheme.PrimaryColor;
            Color hoverBg = Color.FromArgb(25, accentColor);

            int iconSize = ScaleMetric(16);

            foreach (var btn in _toolbarButtons)
            {
                if (btn.Key.StartsWith("spacer")) continue;
                if (btn.Bounds.IsEmpty) continue;

                bool isHovered = _toolbarHoveredIndex >= 0 &&
                    _toolbarButtons[_toolbarHoveredIndex].Key == btn.Key;
                bool isActive = btn.IsViewSelector && IsViewActive(btn.Key);

                if (isActive)
                {
                    using (var brush = new SolidBrush(accentColor))
                        g.FillRectangle(brush, btn.Bounds);
                }
                else if (isHovered)
                {
                    using (var brush = new SolidBrush(hoverBg))
                        g.FillRectangle(brush, btn.Bounds);
                }

                int innerX = btn.Bounds.X + ScaleMetric(4);
                int innerY = btn.Bounds.Y + (btn.Bounds.Height - iconSize) / 2;

                if (!string.IsNullOrEmpty(btn.IconPath))
                {
                    Rectangle iconRect;
                    if (!string.IsNullOrEmpty(btn.Label))
                    {
                        iconRect = new Rectangle(innerX, innerY, iconSize, iconSize);
                        innerX += iconSize + ScaleMetric(2);
                    }
                    else
                    {
                        iconRect = new Rectangle(
                            btn.Bounds.X + (btn.Bounds.Width - iconSize) / 2,
                            innerY, iconSize, iconSize);
                    }

                    try { StyledImagePainter.Paint(g, iconRect, btn.IconPath); }
                    catch { }
                }

                if (!string.IsNullOrEmpty(btn.Label))
                {
                    Color labelColor = isActive ? Color.White : textColor;
                    Rectangle textRect = new Rectangle(innerX, btn.Bounds.Y,
                        btn.Bounds.Right - innerX - ScaleMetric(4), btn.Bounds.Height);
                    using (var brush = new SolidBrush(labelColor))
                    using (var fmt = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    })
                    {
                        g.DrawString(btn.Label, Font, brush, textRect, fmt);
                    }
                }
            }
        }

        private void HitTestToolbar(Point location)
        {
            if (_toolbarButtons == null) return;
            for (int i = 0; i < _toolbarButtons.Count; i++)
            {
                if (_toolbarButtons[i].Bounds.Contains(location))
                {
                    _toolbarHoveredIndex = i;
                    return;
                }
            }
            _toolbarHoveredIndex = -1;
        }

        private void ExecuteToolbarClick(Point location)
        {
            if (_toolbarButtons == null) return;
            for (int i = 0; i < _toolbarButtons.Count; i++)
            {
                if (_toolbarButtons[i].Bounds.Contains(location))
                {
                    _toolbarButtons[i].Action?.Invoke();
                    DeactivateAllCellComponents();
                    Invalidate();
                    return;
                }
            }
        }

        private bool IsViewActive(string key)
        {
            if (_viewPainter == null) return false;
            // GAP F - match on the active CalendarViewMode first (handles
            // legacy enum values like Month/Week/Day/Agenda/List/Timeline
            // that don't appear in the toolbar's button list but are still
            // valid ViewMode values). Fall back to the painter.Key match
            // for safety.
            foreach (var btn in _toolbarButtons)
            {
                if (btn.IsViewSelector && btn.Key == key)
                {
                    return btn.ViewMode == _state.ViewMode || _viewPainter.Key == key;
                }
            }
            return _viewPainter.Key == key;
        }

        private static int EstimateTextWidth(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            return text.Length * 8;
        }
    }
}
