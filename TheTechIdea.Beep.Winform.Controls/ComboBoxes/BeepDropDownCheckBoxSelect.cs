using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Forms;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Icons;

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
        private IComboBoxPopupHost _popupHost;
        private string _popupSearchText = string.Empty;

        private ComboBoxType _comboBoxType = ComboBoxType.MultiChipSearch;

        // Features
        [Browsable(true), Category("Data"), Description("Available items to choose from")]        
        public List<SimpleItem> Items => _items;

        [Browsable(true), Category("Data"), Description("Currently selected items")]
        public List<SimpleItem> SelectedItems => _selected;

        [Browsable(true), Category("Behavior"), Description("Maximum number of items that can be selected (0 = unlimited)")]
        public int MaxSelection { get; set; } = 0;

        [Browsable(true), Category("Appearance"), Description("Placeholder text when no selection")]
        public string Placeholder { get; set; } = "Select...";

        [Browsable(true), Category("Appearance"), Description("Visual variant shared with BeepComboBox token model.")]
        [DefaultValue(ComboBoxType.MultiChipSearch)]
        public ComboBoxType ComboBoxType
        {
            get => _comboBoxType;
            set
            {
                if (_comboBoxType == value) return;
                _comboBoxType = value;
                Invalidate();
            }
        }

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

        private ComboBoxVisualTokens GetVisualTokens() => ComboBoxVisualTokenCatalog.Resolve(_comboBoxType);

        private static int ScaleLogicalX(int px) =>
            (int)Math.Round(px * (BeepThemesManager.DpiScaleX > 0f ? BeepThemesManager.DpiScaleX : 1f));

        private static int ScaleLogicalY(int px) =>
            (int)Math.Round(px * (BeepThemesManager.DpiScaleY > 0f ? BeepThemesManager.DpiScaleY : 1f));
        private static bool IsSameSimpleItem(SimpleItem left, SimpleItem right)
            => string.Equals(BeepComboBox.GetSimpleItemIdentity(left), BeepComboBox.GetSimpleItemIdentity(right), StringComparison.OrdinalIgnoreCase);

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
            var tokens = GetVisualTokens();
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
                    int selectedIndex = _selected.FindIndex(item => IsSameSimpleItem(item, si));
                    if (selectedIndex >= 0)
                    {
                        _selected.RemoveAt(selectedIndex);
                        SelectionChanged?.Invoke(this, EventArgs.Empty);
                        SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(si));
                        Invalidate();
                        if (_popupHost != null && _popupHost.IsVisible)
                        {
                            var model = ComboBoxPopupModelBuilder.Build(_items, _selected, null, string.Empty, ComboBoxType, true, true, false);
                            _popupHost.UpdateModel(model);
                        }
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

            var tokens = GetVisualTokens();
            // Draw placeholder / chips
            var chips = GetChipsLayout(contentRect);

            int padding = ScaleLogicalX(Math.Max(4, tokens.InnerPadding.Left));
            int y = contentRect.Y + padding / 2;
            using (var pen = new Pen(BorderColor))
            using (var brush = new SolidBrush(ForeColor))
            {
                if (_selected.Count == 0)
                {
                    // Placeholder
                    var placeholderRect = new Rectangle(contentRect.X + padding, y, Math.Max(10, contentRect.Width - ScaleLogicalX(tokens.ButtonWidth) - (padding * 3)), contentRect.Height - padding);
                    Color phColor = _currentTheme?.TextBoxPlaceholderColor ?? Color.FromArgb(150, ForeColor);
                    if (ThemeContrastHelper.ContrastRatio(phColor, BackColor) < 2.8)
                    {
                        phColor = ThemeContrastHelper.AdjustForegroundToContrast(phColor, BackColor, 2.8);
                    }
                    TextRenderer.DrawText(g, Placeholder, TextFont, placeholderRect, phColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }
                else
                {
                    // Draw chips
                    foreach (var c in chips)
                    {
                        Color chipBack = _currentTheme?.ComboBoxSelectedBackColor != Color.Empty
                            ? _currentTheme.ComboBoxSelectedBackColor
                            : (_currentTheme?.PrimaryColor ?? Color.FromArgb(0, 180, 230));
                        Color chipFore = _currentTheme?.ComboBoxSelectedForeColor != Color.Empty
                            ? _currentTheme.ComboBoxSelectedForeColor
                            : (_currentTheme?.OnPrimaryColor ?? Color.White);
                        Color chipBorder = _currentTheme?.ComboBoxSelectedBorderColor != Color.Empty
                            ? _currentTheme.ComboBoxSelectedBorderColor
                            : (_currentTheme?.PrimaryColor ?? Color.FromArgb(0, 140, 190));
                        using (var chipBrush = new SolidBrush(chipBack))
                        using (var chipPen = new Pen(chipBorder))
                        {
                            int chipRadius = Math.Max(ScaleLogicalX(tokens.CornerRadius), c.Rect.Height / 2);
                            g.FillPath(chipBrush, GraphicsExtensions.GetRoundedRectPath(c.Rect, chipRadius));
                            g.DrawPath(chipPen, GraphicsExtensions.GetRoundedRectPath(c.Rect, chipRadius));

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
                                TextRenderer.DrawText(g, c.Text, TextFont, textRect, chipFore, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                            }
                            else
                            {
                                TextRenderer.DrawText(g, c.Text, TextFont, c.TextRect, chipFore, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                            }

                            using (var path = new GraphicsPath())
                            {
                                path.AddRectangle(c.CloseRect);
                                StyledImagePainter.PaintWithTint(g, path, SvgsUI.X, chipFore, 0.85f);
                            }
                        }
                    }
                }

                // Draw drop-down button divider and arrow
                var btnRect = GetButtonRectFromContent(contentRect);
                int dividerX = btnRect.Left - padding / 2;
                using (var dividerPen = new Pen(Color.FromArgb(100, BorderColor), 1))
                {
                    g.DrawLine(dividerPen, new Point(dividerX, contentRect.Y + 4), new Point(dividerX, contentRect.Bottom - 4));
                }
                DrawDropdownArrow(g, btnRect);

                // Draw validation error if any
                if (RequireAtLeastOne && _selected.Count == 0)
                {
                    var err = string.IsNullOrEmpty(ValidationError) ? "(at least one item is required)" : ValidationError;
                    int errHeight = 16;
                    var lblRect = new Rectangle(contentRect.Left, contentRect.Bottom - errHeight, contentRect.Width, errHeight);
                    using var errBrush = new SolidBrush(Color.FromArgb(200, Color.Red));
                    using var errBgBrush = new SolidBrush(Color.FromArgb(20, Color.Red));
                    g.FillRectangle(errBgBrush, lblRect);
                    TextRenderer.DrawText(g, err, new Font(TextFont.FontFamily, TextFont.Size * 0.85f), lblRect, Color.FromArgb(200, Color.Red), TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
            }
        }

        private void DrawDropdownArrow(Graphics g, Rectangle r)
        {
            var iconRect = Rectangle.Inflate(r, -ScaleLogicalX(7), -ScaleLogicalY(7));
            if (iconRect.Width > 0 && iconRect.Height > 0)
            {
                Color iconColor = _currentTheme?.ComboBoxForeColor ?? ForeColor;
                using (var path = new GraphicsPath())
                {
                    path.AddRectangle(iconRect);
                    StyledImagePainter.PaintWithTint(g, path, SvgsUI.ChevronDown, iconColor, 0.85f);
                }
            }
        }

        private Rectangle GetButtonRectFromContent(Rectangle contentRect)
        {
            var tokens = GetVisualTokens();
            int padding = ScaleLogicalX(Math.Max(4, tokens.InnerPadding.Left));
            int buttonWidth = ScaleLogicalX(Math.Max(20, tokens.ButtonWidth));
            int x = contentRect.Right - buttonWidth - padding / 2;
            int y = contentRect.Y + (padding / 2);
            int h = Math.Max(0, contentRect.Height - padding);
            return new Rectangle(x, y, buttonWidth, h);
        }

        private struct ChipLayout { public SimpleItem Item; public string Text; public Rectangle Rect; public Rectangle TextRect; public Rectangle CloseRect; }

        private List<ChipLayout> GetChipsLayout(Rectangle contentRect)
        {
            var tokens = GetVisualTokens();
            int padding = ScaleLogicalX(Math.Max(4, tokens.InnerPadding.Left));
            var list = new List<ChipLayout>();
            int x = contentRect.X + padding;
            int y = contentRect.Y + padding / 2;
            int maxW = contentRect.Width - ScaleLogicalX(tokens.ButtonWidth) - (padding * 3);
            int lineHeight = Math.Max(ScaleLogicalY(tokens.ChipHeight), TextFont.Height + ScaleLogicalY(8));
            int cx = x;
            int cy = y;
            int usedW = 0;
            foreach (var s in _selected)
            {
                string text = string.IsNullOrEmpty(s.DisplayField) ? s.Text ?? s.Name : s.DisplayField;
                SizeF szF = TextUtils.MeasureText(text, TextFont, int.MaxValue);
                Size sz = new Size((int)szF.Width, (int)szF.Height);
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
            if (_popupHost != null && _popupHost.IsVisible)
            {
                ClosePopup();
                return;
            }
            ShowPopup();
        }

        private void ShowPopup()
        {
            if (_popupHost != null) ClosePopup();

            _popupSearchText = string.Empty;
            _popupHost = CreatePopupHostForType(ComboBoxType);
            _popupHost.RowCommitted += PopupHost_RowCommitted;
            _popupHost.PopupClosed += PopupHost_PopupClosed;
            _popupHost.SearchTextChanged += PopupHost_SearchTextChanged;

            var model = ComboBoxPopupModelBuilder.Build(
                _items,
                _selected,
                null,
                _popupSearchText,
                ComboBoxType,
                true, // isMultiSelect
                true, // showSelectAll
                false // showFooter
            );

            _popupHost.ShowPopup(this, model, new Rectangle(0, 0, Width, Height));
        }

        private void PopupHost_RowCommitted(object sender, ComboBoxRowCommittedEventArgs e)
        {
            if (e.Row == null || e.Row.SourceItem == null) return;

            var item = e.Row.SourceItem;
            bool newlyChecked = e.Row.IsChecked;

            if (MaxSelection > 0 && _selected.Count >= MaxSelection && newlyChecked)
            {
                // Enforce max selection limit
                return;
            }

            int idx = _selected.FindIndex(existing => IsSameSimpleItem(existing, item));
            if (idx >= 0)
            {
                if (RequireAtLeastOne && _selected.Count <= 1)
                {
                    return; // Enforce at least one constraint
                }
                _selected.RemoveAt(idx);
            }
            else
            {
                _selected.Add(item);
            }

            SelectionChanged?.Invoke(this, EventArgs.Empty);
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(item));
            Invalidate();

            if (CloseOnSelection)
            {
                ClosePopup();
            }
            else if (_popupHost != null)
            {
                var model = ComboBoxPopupModelBuilder.Build(
                    _items,
                    _selected,
                    null,
                    _popupSearchText,
                    ComboBoxType,
                    true,
                    true,
                    false);
                _popupHost.UpdateModel(model);
            }
        }

        private void PopupHost_SearchTextChanged(object sender, ComboBoxSearchChangedEventArgs e)
        {
            if (_popupHost == null)
            {
                return;
            }

            _popupSearchText = e?.SearchText ?? string.Empty;
            var model = ComboBoxPopupModelBuilder.Build(
                _items,
                _selected,
                null,
                _popupSearchText,
                ComboBoxType,
                true,
                true,
                false);
            _popupHost.UpdateModel(model);
        }

        private void PopupHost_PopupClosed(object sender, ComboBoxPopupClosedEventArgs e)
        {
            if (_popupHost != null)
            {
                _popupHost.RowCommitted -= PopupHost_RowCommitted;
                _popupHost.PopupClosed -= PopupHost_PopupClosed;
                _popupHost.SearchTextChanged -= PopupHost_SearchTextChanged;
                _popupHost = null;
            }
            _popupSearchText = string.Empty;
            Invalidate();
        }

        private void ClosePopup()
        {
            if (_popupHost != null)
            {
                _popupHost.ClosePopup(false);
                _popupHost = null;
            }
            _popupSearchText = string.Empty;
        }

        private static IComboBoxPopupHost CreatePopupHostForType(ComboBoxType type)
        {
            return type switch
            {
                ComboBoxType.OutlineDefault => new OutlineDefaultPopupHostForm(),
                ComboBoxType.OutlineSearchable => new OutlineSearchablePopupHostForm(),
                ComboBoxType.FilledSoft => new FilledSoftPopupHostForm(),
                ComboBoxType.RoundedPill => new RoundedPillPopupHostForm(),
                ComboBoxType.SegmentedTrigger => new SegmentedTriggerPopupHostForm(),
                ComboBoxType.MultiChipCompact => new MultiChipCompactPopupHostForm(),
                ComboBoxType.MultiChipSearch => new MultiChipSearchPopupHostForm(),
                ComboBoxType.DenseList => new DenseListPopupHostForm(),
                ComboBoxType.MinimalBorderless => new MinimalBorderlessPopupHostForm(),
                ComboBoxType.CommandMenu => new CommandMenuPopupHostForm(),
                ComboBoxType.VisualDisplay => new VisualDisplayPopupHostForm(),
                _ => new MultiChipSearchPopupHostForm(),
            };
        }
    }
}
