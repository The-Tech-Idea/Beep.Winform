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

        // RB-C02: Fonts from theme — never disposed, managed by BeepThemesManager
        private Font GetTabFont() =>
            BeepThemesManager.ToFont(_theme?.TabTypography) ?? SystemFonts.DefaultFont;

        private Font GetSelFont() => GetTabFont(); // same font, selection shown via background

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

        // RB-C02: Uses BeepRibbonPainter for real Office 365 tab appearance
        private Rendering.BeepRibbonPainter? _rbPainter;
        private Rendering.BeepRibbonPainter RBPainter =>
            _rbPainter ??= new Rendering.BeepRibbonPainter(_theme ?? new RibbonTheme(), this);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            RBPainter.PaintTabStripBackground(g, ClientRectangle);
            if (_tabs.Count == 0) return;
            for (int i = 0; i < _tabs.Count; i++)
            {
                var rect = GetTabRect(i);
                RBPainter.PaintTab(g, rect, _tabs[i].Text,
                    isActive: i == _selectedIndex,
                    isHovered: _tabs[i].IsHovered);
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
