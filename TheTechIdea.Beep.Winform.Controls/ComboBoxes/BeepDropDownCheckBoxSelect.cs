using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Forms;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes
{
    /// <summary>
    /// Drop-down multi-select control with chips and popup checklist.
    /// Uses BeepPopupForm for the popup content. Draws UI in DrawContent.
    /// Items are SimpleItem and images (if any) are painted using StyledImagePainter.
    /// </summary>
    public class BeepDropDownCheckBoxSelect : BaseControl
    {
        private readonly List<SimpleItem> _items = new List<SimpleItem>();
        private readonly List<SimpleItem> _selected = new List<SimpleItem>();
        private BeepPopupForm _popup;
        private PopupContent _popupContent;

        private int _buttonWidth = 24;
        private int _padding = 6;

        // Features
        [Browsable(true), Category("Data"), Description("Available items to choose from")]        
        public List<SimpleItem> Items => _items;

        [Browsable(true), Category("Data"), Description("Currently selected items")]
        public List<SimpleItem> SelectedItems => _selected;

        [Browsable(true), Category("Behavior"), Description("Maximum number of items that can be selected (0 = unlimited)")]
        public int MaxSelection { get; set; } = 0;

        [Browsable(true), Category("Appearance"), Description("Placeholder text when no selection")]
        public string Placeholder { get; set; } = "Select...";

        [Browsable(true), Category("Behavior"), Description("Close popup when an item is selected")]
        public bool CloseOnSelection { get; set; } = false;

        [Browsable(true), Category("Validation"), Description("Require at least one item")]
        public bool RequireAtLeastOne { get; set; } = false;

        [Browsable(true), Category("Validation"), Description("Current validation error message")]
        public string ValidationError { get; private set; } = string.Empty;

        // Events similar to other controls
        public event EventHandler SelectionChanged;
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        public BeepDropDownCheckBoxSelect()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            Height = 30;
        }

        // Helper to add items easily
        public void AddItem(SimpleItem item)
        {
            if (item == null) return;
            _items.Add(item);
        }

        public void AddItem(string text, string imagePath = null)
        {
            var it = new SimpleItem { Text = text, Name = text, ImagePath = imagePath };
            _items.Add(it);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            UpdateDrawingRect();
            var contentRect = DrawingRect;
            var btnRect = GetButtonRectFromContent(contentRect);
            if (btnRect.Contains(e.Location))
            {
                TogglePopup();
                return;
            }

            // Check if clicked on a chip close area
            var chips = GetChipsLayout(contentRect);
            foreach (var c in chips)
            {
                if (c.CloseRect.Contains(e.Location))
                {
                    // remove
                    var si = c.Item;
                    if (_selected.Contains(si))
                    {
                        _selected.Remove(si);
                        SelectionChanged?.Invoke(this, EventArgs.Empty);
                        SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(si));
                        Invalidate();
                        _popupContent?.SyncSelected(_selected);
                    }
                    return;
                }
            }

            this.Focus();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            Invalidate();
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            UpdateDrawingRect();
            Rectangle contentRect = DrawingRect;

            // Draw placeholder / chips
            var chips = GetChipsLayout(contentRect);

            int y = contentRect.Y + _padding/2;
            using (var pen = new Pen(BorderColor))
            using (var brush = new SolidBrush(ForeColor))
            {
                if (_selected.Count == 0)
                {
                    // Placeholder
                    var placeholderRect = new Rectangle(contentRect.X + _padding, y, Math.Max(10, contentRect.Width - _buttonWidth - (_padding*3)), contentRect.Height - _padding);
                    Color phColor = Color.FromArgb(150, ForeColor);
                    TextRenderer.DrawText(g, Placeholder, this.Font, placeholderRect, phColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }
                else
                {
                    // Draw chips
                    foreach (var c in chips)
                    {
                        using (var chipBrush = new SolidBrush(Color.FromArgb(0, 180, 230)))
                        using (var chipTextBrush = new SolidBrush(Color.White))
                        using (var chipPen = new Pen(Color.FromArgb(0,140,190)))
                        {
                            g.FillPath(chipBrush, GraphicsExtensions.GetRoundedRectPath(c.Rect, 6));
                            g.DrawPath(chipPen, GraphicsExtensions.GetRoundedRectPath(c.Rect, 6));

                            // Draw image if present (left side)
                            if (!string.IsNullOrEmpty(c.Item?.ImagePath))
                            {
                                var imgRect = new Rectangle(c.Rect.X + 4, c.Rect.Y + 4, c.Rect.Height - 8, c.Rect.Height - 8);
                                try
                                {
                                    StyledImagePainter.Paint(g, imgRect, c.Item.ImagePath,BeepControlStyle.Minimal);
                                }
                                catch { }

                                var textRect = new Rectangle(c.TextRect.X + imgRect.Width + 4, c.TextRect.Y, c.TextRect.Width - (imgRect.Width + 4), c.TextRect.Height);
                                TextRenderer.DrawText(g, c.Text, this.Font, textRect, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                            }
                            else
                            {
                                TextRenderer.DrawText(g, c.Text, this.Font, c.TextRect, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                            }

                            // draw small X box
                            g.FillEllipse(Brushes.White, c.CloseRect);
                            using (var xPen = new Pen(Color.FromArgb(0,140,190), 1.5f))
                            {
                                g.DrawLine(xPen, c.CloseRect.Left+3, c.CloseRect.Top+3, c.CloseRect.Right-3, c.CloseRect.Bottom-3);
                                g.DrawLine(xPen, c.CloseRect.Left+3, c.CloseRect.Bottom-3, c.CloseRect.Right-3, c.CloseRect.Top+3);
                            }
                        }
                    }
                }

                // Draw drop-down button divider and arrow
                var btnRect = GetButtonRectFromContent(contentRect);
                int dividerX = btnRect.Left - _padding/2;
                using (var dividerPen = new Pen(Color.FromArgb(100, BorderColor), 1))
                {
                    g.DrawLine(dividerPen, new Point(dividerX, contentRect.Y + 4), new Point(dividerX, contentRect.Bottom - 4));
                }
                DrawDropdownArrow(g, btnRect);

                // Draw validation error if any
                if (RequireAtLeastOne && _selected.Count == 0)
                {
                    var err = string.IsNullOrEmpty(ValidationError) ? "(at least one item is required)" : ValidationError;
                    var lblRect = new Rectangle(contentRect.Left, contentRect.Bottom + 2, contentRect.Width, 18);
                    TextRenderer.DrawText(g, err, this.Font, lblRect, Color.FromArgb(200, Color.Red), TextFormatFlags.Left | TextFormatFlags.Top);
                }
            }
        }

        private void DrawDropdownArrow(Graphics g, Rectangle r)
        {
            int arrowSize = Math.Min(10, Math.Min(r.Width - 8, r.Height - 8));
            int cx = r.Left + r.Width/2;
            int cy = r.Top + r.Height/2;
            Point[] pts = new Point[] {
                new Point(cx - arrowSize/2, cy - 1),
                new Point(cx + arrowSize/2, cy - 1),
                new Point(cx, cy + arrowSize/2)
            };
            using (SolidBrush b = new SolidBrush(ForeColor)) g.FillPolygon(b, pts);
        }

        private Rectangle GetButtonRectFromContent(Rectangle contentRect)
        {
            int x = contentRect.Right - _buttonWidth - _padding/2;
            int y = contentRect.Y + (_padding/2);
            int h = Math.Max(0, contentRect.Height - _padding);
            return new Rectangle(x, y, _buttonWidth, h);
        }

        private struct ChipLayout { public SimpleItem Item; public string Text; public Rectangle Rect; public Rectangle TextRect; public Rectangle CloseRect; }

        private List<ChipLayout> GetChipsLayout(Rectangle contentRect)
        {
            var list = new List<ChipLayout>();
            int x = contentRect.X + _padding;
            int y = contentRect.Y + _padding/2;
            int maxW = contentRect.Width - _buttonWidth - (_padding*3);
            int lineHeight = this.Font.Height + 8;
            int cx = x;
            int cy = y;
            int usedW = 0;
            foreach (var s in _selected)
            {
                string text = string.IsNullOrEmpty(s.DisplayField) ? s.Text ?? s.Name : s.DisplayField;
                Size sz = TextRenderer.MeasureText(text, this.Font);
                int chipW = Math.Max(40, sz.Width + 28);
                if (usedW + chipW > maxW && usedW > 0)
                {
                    // wrap
                    cy += lineHeight + 4;
                    usedW = 0;
                }
                var rect = new Rectangle(cx + usedW, cy, chipW, lineHeight);
                var textRect = new Rectangle(rect.X + 8, rect.Y, rect.Width - 20, rect.Height);
                var closeRect = new Rectangle(rect.Right - 16, rect.Y + (rect.Height - 12)/2, 12, 12);
                list.Add(new ChipLayout { Item = s, Text = text, Rect = rect, TextRect = textRect, CloseRect = closeRect });
                usedW += chipW + 6;
            }
            return list;
        }

        private void TogglePopup()
        {
            if (_popup != null && !_popup.IsDisposed && _popup.Visible)
            {
                ClosePopup();
                return;
            }
            ShowPopup();
        }

        private void ShowPopup()
        {
            if (_popup != null && !_popup.IsDisposed) ClosePopup();

            _popup = new BeepPopupForm();
            _popup.Theme = this.Theme;
            _popup.AutoClose = true;
            _popup.FormStyle = FormStyle.Modern;

            _popupContent = new PopupContent(_items, _selected, MaxSelection, RequireAtLeastOne, this.Theme, this.TextFont);
            _popupContent.Dock = DockStyle.Fill;
            _popupContent.SelectionChanged += PopupContent_SelectionChanged;

            _popup.Controls.Add(_popupContent);

            // size
            int w = Math.Max(200, this.Width);
            int h = 220;
            _popup.Size = new Size(w, h);

            Point loc = this.PointToScreen(new Point(0, this.Height));
            _popup.ShowPopup(this, BeepPopupFormPosition.Bottom, w, h);
        }

        private void PopupContent_SelectionChanged(object sender, EventArgs e)
        {
            if (_popupContent == null) return;
            // sync
            _selected.Clear();
            _selected.AddRange(_popupContent.GetSelected());
            SelectionChanged?.Invoke(this, EventArgs.Empty);
            // raise individual selected events for each newly selected item? raise generic
            foreach(var it in _selected) SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(it));
            Invalidate();
            if (CloseOnSelection)
            {
                ClosePopup();
            }
        }

        private void ClosePopup()
        {
            try
            {
                if (_popup != null && !_popup.IsDisposed)
                {
                    _popup.CloseCascade();
                    _popup = null;
                    _popupContent = null;
                }
            }
            catch { }
        }

        #region Inner PopupContent
        private class PopupContent : UserControl
        {
            private BeepTextBox _searchBox;
            private CheckedListBox _chkList;
            private FlowLayoutPanel _chipPanel;
            private List<SimpleItem> _items;
            private List<SimpleItem> _selected;
            private int _maxSel;
            private bool _requireOne;
            private string _theme;
            private Font _textFont;

            public event EventHandler SelectionChanged;

            public PopupContent(List<SimpleItem> items, List<SimpleItem> selected, int maxSelection, bool requireOne, string theme, Font textFont)
            {
                _items = new List<SimpleItem>(items);
                _selected = new List<SimpleItem>(selected);
                _maxSel = maxSelection;
                _requireOne = requireOne;
                _theme = theme;
                _textFont = textFont;
                Initialize();
            }

            private void Initialize()
            {
                this.Padding = new Padding(6);
                _searchBox = new BeepTextBox { Dock = DockStyle.Top, PlaceholderText = "Search...", Height = 28, Theme = _theme, TextFont = _textFont };
                _searchBox.TextChanged += (s, e) => ApplyFilter();

                _chipPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, AutoScroll = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = true };
                _chkList = new CheckedListBox { Dock = DockStyle.Fill, CheckOnClick = true, Font = _textFont };
                _chkList.ItemCheck += _chkList_ItemCheck;

                this.Controls.Add(_chkList);
                this.Controls.Add(new Label { Height = 4, Dock = DockStyle.Top });
                this.Controls.Add(_chipPanel);
                this.Controls.Add(_searchBox);
                RefreshItems();
            }

            private void _chkList_ItemCheck(object sender, ItemCheckEventArgs e)
            {
                this.BeginInvoke((Action)(() => {
                    var item = _chkList.Items[e.Index] as SimpleItem;
                    if(item == null) return;
                    bool willBeChecked = e.NewValue == CheckState.Checked;
                    if (willBeChecked)
                    {
                        if (_maxSel > 0 && _selected.Count >= _maxSel)
                        {
                            // reject
                            _chkList.ItemCheck -= _chkList_ItemCheck;
                            _chkList.SetItemCheckState(e.Index, CheckState.Unchecked);
                            _chkList.ItemCheck += _chkList_ItemCheck;
                            return;
                        }
                        if (!_selected.Contains(item)) _selected.Add(item);
                    }
                    else
                    {
                        if (_selected.Contains(item)) _selected.Remove(item);
                    }

                    UpdateChips();
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }));
            }

            public void SyncSelected(List<SimpleItem> selected)
            {
                _selected.Clear();
                _selected.AddRange(selected);
                RefreshCheckedState();
                UpdateChips();
            }

            private void RefreshItems()
            {
                ApplyFilter();
                UpdateChips();
            }

            private void RefreshCheckedState()
            {
                for (int i = 0; i < _chkList.Items.Count; i++)
                {
                    var it = _chkList.Items[i] as SimpleItem;
                    _chkList.SetItemChecked(i, _selected.Contains(it));
                }
            }

            private void UpdateChips()
            {
                _chipPanel.Controls.Clear();
                foreach (var s in _selected)
                {
                    var chipBtn = new BeepButton
                    {
                        Text = string.IsNullOrEmpty(s.DisplayField) ? s.Text ?? s.Name : s.DisplayField,
                        Font = _textFont,
                        Height = 26,
                        Margin = new Padding(4),
                        Padding = new Padding(6, 0, 6, 0),
                        BorderRadius = 12,
                        IsColorFromTheme = true,
                        BackColor = Color.FromArgb(0, 180, 230),
                        ForeColor = Color.White,
                        LeadingIconPath = !string.IsNullOrEmpty(s.ImagePath) ? s.ImagePath : null,
                        TrailingIconPath = TheTechIdea.Beep.Icons.Svgs.Close,
                        TextImageRelation = TextImageRelation.ImageBeforeText,
                        ImageAlign = ContentAlignment.MiddleLeft,
                        TextAlign = ContentAlignment.MiddleCenter,
                        ButtonType = ButtonType.Normal,
                        AutoSizeContent = true,
                        MinimumSize = new Size(40, 26),
                        MaximumSize = new Size(200, 26),
                    };
                    chipBtn.Theme= _theme;
                    chipBtn.Click += (se, ev) => {
                        if (_selected.Contains(s))
                        {
                            _selected.Remove(s);
                            RefreshCheckedState();
                            UpdateChips();
                            SelectionChanged?.Invoke(this, EventArgs.Empty);
                        }
                    };
                    _chipPanel.Controls.Add(chipBtn);
                }
            }

            private void ApplyFilter()
            {
                var q = _searchBox.Text?.Trim().ToLowerInvariant();
                _chkList.Items.Clear();
                foreach (var it in _items)
                {
                    if (string.IsNullOrEmpty(q) || (it.DisplayField ?? it.Text ?? it.Name).ToLowerInvariant().Contains(q))
                    {
                        _chkList.Items.Add(it, _selected.Contains(it));
                    }
                }
            }

            public IEnumerable<SimpleItem> GetSelected() => _selected.AsReadOnly();
        }
        #endregion
    }
}
