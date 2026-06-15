using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public sealed class BeepRibbonTabStrip : Control
    {
        private readonly List<RibbonTab> _tabs = new();
        private int _selectedIndex = -1;
        private RibbonTheme? _theme;
        private Font? _cachedTabFont;
        private Font? _cachedSelFont;

        public event EventHandler? SelectedIndexChanged;

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex != value && value >= 0 && value < _tabs.Count)
                {
                    _selectedIndex = value;
                    SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        public IReadOnlyList<RibbonTab> Tabs => _tabs;
        public RibbonTab? SelectedTab => _selectedIndex >= 0 && _selectedIndex < _tabs.Count ? _tabs[_selectedIndex] : null;

        public BeepRibbonTabStrip()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint
                | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            Height = 28;
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            MouseClick += OnTabMouseClick;
            MouseMove += OnTabMouseMove;
            MouseLeave += (_, _) => Invalidate();
        }

        public RibbonTab AddTab(string text)
        {
            var tab = new RibbonTab(text);
            _tabs.Add(tab);
            if (_selectedIndex < 0) _selectedIndex = 0;
            Invalidate();
            return tab;
        }

        public void RemoveTab(RibbonTab tab)
        {
            _tabs.Remove(tab);
            if (_selectedIndex >= _tabs.Count) _selectedIndex = _tabs.Count - 1;
            Invalidate();
        }

        public void Clear()
        {
            _tabs.Clear();
            _selectedIndex = -1;
            Invalidate();
        }

        public bool ContainsTab(RibbonTab tab) => _tabs.Contains(tab);

        public void ApplyTheme(RibbonTheme theme)
        {
            _theme = theme;
            _cachedTabFont?.Dispose();
            _cachedSelFont?.Dispose();
            _cachedTabFont = null;
            _cachedSelFont = null;
            Invalidate();
        }

        private Font GetTabFont()
        {
            if (_cachedTabFont == null || _cachedTabFont.FontFamily != Font.FontFamily)
            {
                _cachedTabFont?.Dispose();
                _cachedTabFont = new Font(Font.FontFamily, 9.5f, FontStyle.Regular);
            }
            return _cachedTabFont;
        }

        private Font GetSelFont()
        {
            if (_cachedSelFont == null || _cachedSelFont.FontFamily != Font.FontFamily)
            {
                _cachedSelFont?.Dispose();
                _cachedSelFont = new Font(Font.FontFamily, 9.5f, FontStyle.Bold);
            }
            return _cachedSelFont;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                MouseClick -= OnTabMouseClick;
                MouseMove -= OnTabMouseMove;
                _cachedTabFont?.Dispose();
                _cachedSelFont?.Dispose();
                _cachedTabFont = null;
                _cachedSelFont = null;
            }
            base.Dispose(disposing);
        }

        private void OnTabMouseClick(object? sender, MouseEventArgs e)
        {
            for (int i = 0; i < _tabs.Count; i++)
            {
                var rect = GetTabRect(i);
                if (rect.Contains(e.Location))
                {
                    SelectedIndex = i;
                    return;
                }
            }
        }

        private void OnTabMouseMove(object? sender, MouseEventArgs e)
        {
            bool anyHovered = false;
            for (int i = 0; i < _tabs.Count; i++)
            {
                var hovered = GetTabRect(i).Contains(e.Location);
                if (_tabs[i].IsHovered != hovered)
                {
                    _tabs[i].IsHovered = hovered;
                    anyHovered = true;
                }
            }
            if (anyHovered) Invalidate();
        }

        internal Rectangle GetTabRect(int index)
        {
            int x = 4;
            for (int i = 0; i < index; i++)
            {
                x += MeasureTabWidth(_tabs[i].Text) + 4;
            }
            return new Rectangle(x, 0, MeasureTabWidth(_tabs[index].Text) + 8, Height - 4);
        }

        private int MeasureTabWidth(string text)
        {
            return TextRenderer.MeasureText(text, GetTabFont()).Width + 16;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bgColor = _theme?.QuickAccessBack ?? Color.FromArgb(230, 230, 230);
            using var bgBrush = new SolidBrush(bgColor);
            g.FillRectangle(bgBrush, ClientRectangle);

            if (_tabs.Count == 0) return;

            int bottomLine = Height - 3;
            var selColor = _theme?.TabActiveBack ?? Color.White;
            var selTextColor = _theme?.Text ?? SystemColors.ControlText;
            var inactiveTextColor = _theme?.DisabledText ?? Color.FromArgb(120, 120, 120);
            var hoverColor = _theme?.HoverBack ?? Color.FromArgb(210, 220, 235);
            var focusColor = _theme?.FocusBorder ?? Color.FromArgb(0, 120, 215);

            var tabFont = GetTabFont();
            var selFont = GetSelFont();

            for (int i = 0; i < _tabs.Count; i++)
            {
                var rect = GetTabRect(i);
                var tab = _tabs[i];

                if (i == _selectedIndex)
                {
                    using var selBrush = new SolidBrush(selColor);
                    g.FillRectangle(selBrush, rect);
                    using var topPen = new Pen(focusColor, 2);
                    g.DrawLine(topPen, rect.Left + 2, rect.Top, rect.Right - 2, rect.Top);

                    using var textBrush = new SolidBrush(selTextColor);
                    g.DrawString(tab.Text, selFont, textBrush,
                        new RectangleF(rect.Left + 4, rect.Top + 4, rect.Width - 8, rect.Height - 6));
                }
                else if (tab.IsHovered)
                {
                    using var hovBrush = new SolidBrush(hoverColor);
                    g.FillRectangle(hovBrush, rect);

                    int radius = _theme?.CornerRadius ?? 4;
                    radius = Math.Min(radius, Math.Min(rect.Width / 2, rect.Height / 2));

                    using var textBrush = new SolidBrush(selTextColor);
                    g.DrawString(tab.Text, tabFont, textBrush,
                        new RectangleF(rect.Left + 4, rect.Top + 4, rect.Width - 8, rect.Height - 6));
                }
                else
                {
                    using var textBrush = new SolidBrush(inactiveTextColor);
                    g.DrawString(tab.Text, tabFont, textBrush,
                        new RectangleF(rect.Left + 4, rect.Top + 4, rect.Width - 8, rect.Height - 6));
                }
            }

            using var bottomPen = new Pen(_theme?.GroupBorder ?? Color.FromArgb(200, 200, 200));
            g.DrawLine(bottomPen, 0, bottomLine, Width, bottomLine);

            if (_selectedIndex >= 0)
            {
                var selRect = GetTabRect(_selectedIndex);
                using var selPen = new Pen(selColor, 3);
                g.DrawLine(selPen, selRect.Left + 1, bottomLine, selRect.Right - 1, bottomLine);
            }
        }
    }

    public sealed class RibbonTab
    {
        public string Text { get; }
        public bool IsHovered { get; set; }
        public Panel? ContentPanel { get; set; }
        public object? Tag { get; set; }

        public RibbonTab(string text)
        {
            Text = text ?? string.Empty;
        }
    }
}
