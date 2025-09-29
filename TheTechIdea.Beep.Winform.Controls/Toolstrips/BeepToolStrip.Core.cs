using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Toolstrips
{
    public partial class BeepToolStrip
    {
        private readonly Dictionary<ToolStripPainterKind, Painters.IToolStripPainter> _painters = new();
        private ToolStripPainterKind _painterKind = ToolStripPainterKind.TabsUnderline;
        private Dictionary<string, object> _parameters = new();

        [Category("Appearance")]
        public ToolStripPainterKind PainterKind { get => _painterKind; set { _painterKind = value; Invalidate(); } }

        [Browsable(false)]
        public Dictionary<string, object> Parameters { get => _parameters; set { _parameters = value ?? new(); Invalidate(); } }

        private Painters.IToolStripPainter GetActivePainter() => _painters.TryGetValue(_painterKind, out var p) ? p : null;

        public void RegisterPainter(ToolStripPainterKind kind, Painters.IToolStripPainter painter)
        {
            if (painter == null) return; _painters[kind] = painter;
        }

        private void EnsureDefaultPainters()
        {
            if (_painters.Count > 0) return;
            RegisterPainter(ToolStripPainterKind.TabsUnderline, new Painters.UnderlineTabsPainter());
            RegisterPainter(ToolStripPainterKind.TabsPill, new Painters.PillTabsPainter());
            RegisterPainter(ToolStripPainterKind.TabsOutline, new Painters.OutlineTabsPainter());
            RegisterPainter(ToolStripPainterKind.TabsSegmented, new Painters.SegmentedTabsPainter());
            RegisterPainter(ToolStripPainterKind.TabsFilled, new Painters.FilledTabsPainter());
            RegisterPainter(ToolStripPainterKind.MinimalUnderline, new Painters.MinimalUnderlineTabsPainter());
            // extras
            RegisterPainter(ToolStripPainterKind.TabsThickUnderline, new Painters.ThickUnderlineTabsPainter());
            RegisterPainter(ToolStripPainterKind.TabsDotIndicator, new Painters.DotIndicatorTabsPainter());
            RegisterPainter(ToolStripPainterKind.TabsIconOnly, new Painters.IconOnlyTabsPainter());
            RegisterPainter(ToolStripPainterKind.TabsGhost, new Painters.GhostTabsPainter());
            RegisterPainter(ToolStripPainterKind.TabsConnectedSegmented, new Painters.ConnectedSegmentedTabsPainter());
            RegisterPainter(ToolStripPainterKind.TabsElevatedPill, new Painters.ElevatedPillTabsPainter());
            // new batch
            RegisterPainter(ToolStripPainterKind.TabsSegmentedEqual, new Painters.SegmentedEqualTabsPainter());
            RegisterPainter(ToolStripPainterKind.TabsSelectedShadow, new Painters.SelectedShadowTabsPainter());
            RegisterPainter(ToolStripPainterKind.TabsRoundedUnderline, new Painters.RoundedUnderlineTabsPainter());
        }

        protected override void DrawContent(Graphics g)
        {
            if (_painters != null && _painters.Count == 0) EnsureDefaultPainters();
            var painter = GetActivePainter();
            var rect = DrawingRect;
            if (painter != null)
            {
                // let painters draw everything, also hit areas
                _areaRects.Clear(); ClearHitList();
                painter.Paint(g, rect, _currentTheme, this, Parameters);
                painter.UpdateHitAreas(this, rect, _currentTheme, Parameters, (name, r) =>
                {
                    _areaRects[name] = r;
                    int index = -1;
                    if (name.StartsWith("Item:")) int.TryParse(name.Substring(5), out index);
                    AddHitArea(name, r, this, () => { if (index >= 0) RaiseItemClick(index); });
                });
                return;
            }
        }

        // Helper: compute rectangles for connected segmented drawing
        internal List<Rectangle> GetItemRects(Rectangle bounds, System.Drawing.Font font, int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            var rects = new List<Rectangle>();
            if (Buttons == null || Buttons.Count == 0) return rects;

            if (Orientation == ToolStripOrientation.Vertical)
            {
                int y = bounds.Top + spacing;
                int x = bounds.Left + spacing;
                int maxWidth = bounds.Width - spacing * 2;
                foreach (var it in Buttons)
                {
                    string text = string.IsNullOrEmpty(it.Text) ? it.Name : it.Text;
                    var textSize = TextRenderer.MeasureText(text, font);
                    int w = maxWidth;
                    int h = (int)textSize.Height + padV * 2;
                    if (!string.IsNullOrEmpty(it.ImagePath))
                    {
                        if (iconPlacement.Equals("Left", System.StringComparison.OrdinalIgnoreCase))
                            h = System.Math.Max(h, iconSize + padV * 2);
                        else
                            h = iconSize + iconGap + (int)textSize.Height + padV * 2;
                    }
                    rects.Add(new Rectangle(x, y, w, h));
                    y += h + spacing;
                }
            }
            else
            {
                int x = bounds.Left + spacing;
                int y = bounds.Top + spacing;
                foreach (var it in Buttons)
                {
                    string text = string.IsNullOrEmpty(it.Text) ? it.Name : it.Text;
                    var textSize = TextRenderer.MeasureText(text, font);
                    int w = (int)textSize.Width + padH * 2;
                    int h = (int)textSize.Height + padV * 2;
                    if (!string.IsNullOrEmpty(it.ImagePath))
                    {
                        if (iconPlacement.Equals("Left", System.StringComparison.OrdinalIgnoreCase))
                        { w += iconSize + iconGap; h = System.Math.Max(h, iconSize + padV * 2); }
                        else
                        { h = iconSize + iconGap + (int)textSize.Height + padV * 2; w = System.Math.Max(w, iconSize + padH * 2); }
                    }
                    rects.Add(new Rectangle(x, bounds.Top + (bounds.Height - h) / 2, w, h));
                    x += w + spacing;
                }
            }
            return rects;
        }

        public void UseUnderlineSlideAnimation()
        {
            // Wrap current painter with underline slide decorator
            if (_painters.TryGetValue(_painterKind, out var inner) && inner is Painters.IToolStripPainter)
            {
                _painters[_painterKind] = new Painters.UnderlineSlideDecoratorPainter(inner);
                Invalidate();
            }
        }
    }
}
