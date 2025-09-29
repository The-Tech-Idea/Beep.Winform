using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Editors;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls.Toolstrips
{
    public enum ToolStripOrientation
    {
        Horizontal,
        Vertical
    }

    [ToolboxItem(true)]
    [DisplayName("Beep ToolStrip")]
    [Category("Beep Controls")]
    [Description("Modern, painter-driven toolstrip/tabs control.")]
    public partial class BeepToolStrip : BaseControl
    {
        private SimpleMenuList _items;
        private ToolStripOrientation _orientation = ToolStripOrientation.Horizontal;
        private int _selectedIndex = -1;

        // hover/press overlay similar to BeepProgressBar
        private readonly System.Collections.Generic.Dictionary<string, Rectangle> _areaRects = new();
        private string _hoverArea;
        private string _pressedArea;

        public event EventHandler<BeepEventDataArgs> ButtonClicked;
        public event EventHandler<int> SelectedIndexChanged;

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleMenuList Buttons
        {
            get => _items;
            set
            {
                if (_items != null) _items.ListChanged -= Items_ListChanged;
                _items = value ?? new SimpleMenuList();
                _items.ListChanged += Items_ListChanged;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        public ToolStripOrientation Orientation
        {
            get => _orientation;
            set { _orientation = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(-1)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex == value) return;
                _selectedIndex = (value < -1 || _items == null || _items.Count == 0) ? -1 : Math.Min(value, _items.Count - 1);
                SelectedIndexChanged?.Invoke(this, _selectedIndex);
                Invalidate();
            }
        }

        public BeepToolStrip()
        {
            DoubleBuffered = true;
            _items = new SimpleMenuList();
            _items.ListChanged += Items_ListChanged;
            Height = 42;
            ApplyTheme();
            AddChildExternalDrawing(this, DrawHoverPressedOverlay, DrawingLayer.AfterAll);
        }

        private void Items_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            if (_selectedIndex >= _items.Count) _selectedIndex = _items.Count - 1;
            Invalidate();
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;
            BackColor = _currentTheme.PanelBackColor;
            Invalidate();
        }

        private void DrawHoverPressedOverlay(Graphics g, Rectangle childBounds)
        {
            if (string.IsNullOrEmpty(_hoverArea) && string.IsNullOrEmpty(_pressedArea)) return;
            var name = _pressedArea ?? _hoverArea; if (string.IsNullOrEmpty(name)) return; if (!_areaRects.TryGetValue(name, out var rect)) return;
            var fill = _pressedArea != null ? (_currentTheme.TabHoverForeColor.IsEmpty ? Color.FromArgb(60, Color.Black) : Color.FromArgb(80, _currentTheme.TabHoverForeColor)) : (_currentTheme.TabHoverBackColor.IsEmpty ? Color.FromArgb(40, Color.Black) : Color.FromArgb(60, _currentTheme.TabHoverBackColor));
            var border = _currentTheme.TabHoverBorderColor.IsEmpty ? Color.FromArgb(120, Color.Black) : _currentTheme.TabHoverBorderColor;
            using var b = new SolidBrush(fill); using var p = new Pen(border, 1); g.FillRectangle(b, rect); g.DrawRectangle(p, rect);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var nh = _areaRects.FirstOrDefault(kv => kv.Value.Contains(e.Location)).Key;
            if (nh != _hoverArea)
            {
                _hoverArea = nh; SetChildExternalDrawingRedraw(this, true); Invalidate();
            }
        }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hoverArea = null; _pressedArea = null; SetChildExternalDrawingRedraw(this, true); Invalidate(); }
        protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); if (e.Button == MouseButtons.Left) { _pressedArea = _areaRects.FirstOrDefault(kv => kv.Value.Contains(e.Location)).Key; if (_pressedArea != null) { SetChildExternalDrawingRedraw(this, true); Invalidate(); } } }
        protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); if (_pressedArea != null) { _pressedArea = null; SetChildExternalDrawingRedraw(this, true); Invalidate(); } }

        internal void RaiseItemClick(int index)
        {
            if (_items == null || index < 0 || index >= _items.Count) return;
            SelectedIndex = index;
            var it = _items[index];
            ButtonClicked?.Invoke(this, new BeepEventDataArgs(it.Text ?? it.Name, it));
        }
    }
}
