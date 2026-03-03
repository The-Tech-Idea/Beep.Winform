using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.ToolTips;
using TheTechIdea.Beep.Winform.Controls.Lovs.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Icons;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep List of Values Box")]
    [Description("A control that displays a list of values with a popup context menu selection, similar to Oracle Forms LOV.")]
    public class BeepListofValuesBox : BaseControl
    {
        #region Fields
        private BeepTextBox  _keyTextBox;
        // _valueTextBox removed (Phase 6) — display value is painted directly in DrawContent
        private string _selectedDisplayValue = string.Empty;
        private BeepLovPopup _lovPopup;
        private List<SimpleItem> _items = new List<SimpleItem>();
        private int padding = 1;
        private int spacing = 1;
        private int buttonHeight;
        private object _lastValidKey;
        // Cached fonts (rebuilt in RebuildFonts via ApplyTheme)
        private Font? _fieldFont;
        private Font? _badgeFont;

        // ── Phase 6 options ─────────────────────────────────────────────
        private bool _showKeyBadge = true;

        // ── Phase 13: recent-selection history (persisted across popup opens) ──
        private List<SimpleItem> _recentHistory = new List<SimpleItem>();
        #endregion

        #region Properties
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<SimpleItem> ListItems
        {
            get => _items;
            set
            {
                _items = value ?? new List<SimpleItem>();
                UpdateDisplayValue();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The selected key (Value property of SimpleItem).")]
        public string SelectedKey
        {
            get => _keyTextBox?.Text ?? string.Empty;
            set
            {
                if (_keyTextBox == null) return;
                
                if (ValidateKey(value))
                {
                    _keyTextBox.Text = value;
                    UpdateLastValidKey(value);
                    UpdateDisplayValue();
                    Invalidate();
                }
                else if (!string.IsNullOrEmpty(value))
                {
                    _keyTextBox.Text = _lastValidKey?.ToString() ?? string.Empty;
                    UpdateDisplayValue();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The selected display value.")]
        public string SelectedDisplayValue
        {
            get => _selectedDisplayValue;
            private set
            {
                _selectedDisplayValue = value ?? string.Empty;
                Invalidate();
            }
        }

        /// <summary>When true a coloured pill badge showing the raw key is drawn inside
        /// the value display area to the left of the display text.</summary>
        [Browsable(true)]
        [Category("LOV")]
        [DefaultValue(true)]
        [Description("Show a coloured key-badge pill next to the selected display value.")]
        public bool ShowKeyBadge
        {
            get => _showKeyBadge;
            set { _showKeyBadge = value; Invalidate(); }
        }
        #endregion

        #region Constructor
        public BeepListofValuesBox()
        {
            InitializeComponents();
            ApplyTheme();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            // Initialize key textbox (editable) using BeepTextBox
            _keyTextBox = new BeepTextBox
            {
                IsChild         = true,
                IsFrameless     = true,
                Visible         = true,
                PlaceholderText = "Enter key..."
            };
            _keyTextBox.TextChanged += KeyTextBox_TextChanged;

            Controls.Add(_keyTextBox);

            // Use BaseControl’s built-in trailing icon as the dropdown toggle — no separate BeepButton child needed
            TrailingIconPath      = SvgsUI.ChevronDown;
            TrailingIconClickable = true;
            TrailingIconClicked  += (_, __) => OpenPopup();

            // Forward mouse events for proper hover/focus behaviour
            _keyTextBox.MouseEnter += (s, e) => OnMouseEnter(e);
            _keyTextBox.MouseHover += (s, e) => OnMouseHover(e);
            _keyTextBox.MouseLeave += (s, e) => OnMouseLeave(e);

            _lastValidKey = null;
            AdjustLayout();
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            Width = 300;
            Height = 30;
            AdjustLayout();
        }
        #endregion

        #region Layout and Drawing
        private void GetHeight()
        {
            padding     = BorderThickness;
            spacing     = 5;
            buttonHeight = _keyTextBox != null ? _keyTextBox.PreferredHeight : 24;
            Height       = Math.Max(Height, buttonHeight + (padding * 2));
        }

        private void AdjustLayout()
        {
            UpdateDrawingRect();
            GetHeight();

            // Prefer the painter’s content rect (excludes trailing icon area).  
            // Fall back to DrawingRect when the painter hasn’t measured yet.
            Rectangle contentRect = GetAdjustedContentRect();
            if (contentRect.IsEmpty || contentRect.Width <= 0)
                contentRect = DrawingRect;

            int totalWidth = contentRect.Width;
            int centerY    = contentRect.Top + (contentRect.Height - buttonHeight) / 2;

            // Key field: 22 % of the available content width
            int keyWidth   = Math.Max(40, (int)(totalWidth * 0.22));
            // Value field: remainder
            int valueWidth = Math.Max(40, totalWidth - keyWidth - spacing);

            if (_keyTextBox != null)
            {
                _keyTextBox.Location = new Point(contentRect.Left + padding, centerY);
                _keyTextBox.Width    = keyWidth - 1;
                _keyTextBox.Height   = buttonHeight;
            }
            // Value display area is painted in DrawContent — no child control needed
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustLayout();
            Invalidate();
        }

        protected override void DrawContent(Graphics g)
        {
            // Let BaseControl paint border, background, shadow, leading/trailing icons
            base.DrawContent(g);
            PaintValueArea(g, GetAdjustedContentRect());
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            base.Draw(graphics, rectangle);
            // When rendered inside a grid/container also paint the value area
            Rectangle contentRect = GetAdjustedContentRect();
            if (contentRect.IsEmpty) contentRect = rectangle;
            PaintValueArea(graphics, contentRect);
        }

        /// <summary>Paints the read-only value-display area (right 78 %) directly onto
        /// the control's graphics. Called from both <see cref="DrawContent"/> and
        /// <see cref="Draw"/>.</summary>
        private void PaintValueArea(Graphics g, Rectangle contentRect)
        {
            if (contentRect.IsEmpty || contentRect.Width <= 0) return;

            int keyW     = Math.Max(40, (int)(contentRect.Width * 0.22));
            int valueX   = contentRect.Left + padding + keyW + spacing;
            int valueW   = Math.Max(0, contentRect.Right - valueX - padding);
            if (valueW <= 0) return;

            var valueArea = new Rectangle(valueX, contentRect.Top + padding,
                                          valueW, contentRect.Height - padding * 2);
            if (valueArea.Height <= 0) return;

            // Fill value area background (seamlessly matches the field background)
            Color bgColor = _currentTheme?.PanelBackColor ?? BackColor;
            using (var bgBrush = new SolidBrush(bgColor))
                g.FillRectangle(bgBrush, valueArea);

            bool hasKey   = !string.IsNullOrEmpty(SelectedKey);
            bool hasValue = !string.IsNullOrEmpty(_selectedDisplayValue);

            if (!hasKey && !hasValue)
            {
                // Placeholder text
                Color phColor = _currentTheme?.SecondaryTextColor ?? Color.Gray;
                TextRenderer.DrawText(g, "Select a value\u2026", _fieldFont ?? Font,
                    valueArea, phColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                return;
            }

            int x = valueArea.X;

            // ── Key badge pill (optional) ──────────────────────────────
            if (_showKeyBadge && hasKey && _badgeFont != null)
            {
                string  badgeText = SelectedKey;
                Size    textSz    = TextRenderer.MeasureText(g, badgeText, _badgeFont,
                                        new Size(valueArea.Width / 2, valueArea.Height),
                                        TextFormatFlags.NoPrefix);
                int     badgeW   = textSz.Width  + ScaleLogicalX(10);
                int     badgeH   = Math.Min(textSz.Height + ScaleLogicalY(4),
                                           valueArea.Height - ScaleLogicalY(2));
                int     badgeY   = valueArea.Top + (valueArea.Height - badgeH) / 2;
                var     badgeRect = new Rectangle(x, badgeY, badgeW, badgeH);

                Color badgeBg = _currentTheme?.AccentColor ?? Color.SteelBlue;
                Color badgeFg = ContrastForeColor(badgeBg);

                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = BuildRoundedPath(badgeRect, ScaleLogicalX(4)))
                using (var fill = new SolidBrush(badgeBg))
                    g.FillPath(fill, path);
                g.SmoothingMode = SmoothingMode.Default;

                TextRenderer.DrawText(g, badgeText, _badgeFont, badgeRect, badgeFg,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.NoPrefix);

                x += badgeW + ScaleLogicalX(5);
            }

            // ── Display value text ────────────────────────────────────
            if (hasValue && x < valueArea.Right)
            {
                Color fgColor = _currentTheme?.ForeColor ?? ForeColor;
                var textBounds = new Rectangle(x, valueArea.Top,
                                               valueArea.Right - x, valueArea.Height);
                TextRenderer.DrawText(g, _selectedDisplayValue, _fieldFont ?? Font,
                    textBounds, fgColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }
        }

        // ── Painting helpers ──────────────────────────────────────────────

        /// <summary>DPI-aware horizontal pixel scaling via <see cref="BeepThemesManager.DpiScaleX"/>.</summary>
        private static int ScaleLogicalX(int px) =>
            (int)Math.Round(px * (BeepThemesManager.DpiScaleX > 0f ? BeepThemesManager.DpiScaleX : 1f));

        /// <summary>DPI-aware vertical pixel scaling via <see cref="BeepThemesManager.DpiScaleY"/>.</summary>
        private static int ScaleLogicalY(int px) =>
            (int)Math.Round(px * (BeepThemesManager.DpiScaleY > 0f ? BeepThemesManager.DpiScaleY : 1f));

        private static GraphicsPath BuildRoundedPath(Rectangle rect, int radius)
        {
            int r  = Math.Max(1, radius);
            int d  = r * 2;
            var gp = new GraphicsPath();
            gp.AddArc(rect.Left,          rect.Top,           d, d, 180, 90);
            gp.AddArc(rect.Right - d,     rect.Top,           d, d, 270, 90);
            gp.AddArc(rect.Right - d,     rect.Bottom - d,    d, d,   0, 90);
            gp.AddArc(rect.Left,          rect.Bottom - d,    d, d,  90, 90);
            gp.CloseFigure();
            return gp;
        }

        /// <summary>Returns black or white depending on which provides higher
        /// contrast against <paramref name="bg"/>.</summary>
        private static Color ContrastForeColor(Color bg)
        {
            // Perceived luminance (ITU-R BT.601)
            double lum = (0.299 * bg.R + 0.587 * bg.G + 0.114 * bg.B) / 255.0;
            return lum > 0.55 ? Color.FromArgb(30, 30, 30) : Color.White;
        }
        #endregion

        #region Event Handlers
        // (TrailingIconClicked fires OpenPopup — wired in InitializeComponents)

        private void LovPopup_ItemAccepted(object sender, SimpleItem item)
        {
            if (item == null) return;

            SetSelectedItem(item);

            // Phase 13: Keep _recentHistory in sync with what the popup tracks
            if (_lovPopup != null && !_lovPopup.IsDisposed)
                _recentHistory = _lovPopup.RecentItems;
        }

        private void LovPopup_Cancelled(object sender, EventArgs e)
        {
            // No action required — popup already hidden
        }

        /// <summary>Opens the LOV popup, optionally pre-seeding the search box.</summary>
        private async void OpenPopup(string preloadSearch = "")
        {
            if (_lovPopup == null || _lovPopup.IsDisposed)
            {
                _lovPopup = new BeepLovPopup();
                _lovPopup.ItemAccepted += LovPopup_ItemAccepted;
                _lovPopup.Cancelled    += LovPopup_Cancelled;
            }

            _lovPopup.LovTitle       = LovTitle;
            _lovPopup.LovColumns     = LovColumns;
            _lovPopup.MaxPopupHeight = MaxPopupHeight;
            _lovPopup.LovTheme       = _currentTheme?.ThemeName ?? Theme;
            _lovPopup.UseThemeColors = UseThemeColors;
            _lovPopup.CurrentTheme   = _currentTheme;

            // Phase 13: restore recent-selection history into the popup
            _lovPopup.RecentItems = _recentHistory;

            Form? parentForm = FindForm();
            if (parentForm != null)
                _lovPopup.Owner = parentForm;

            Point origin = PointToScreen(new Point(0, Height));

            if (ItemsLoader != null)
            {
                // Phase 12: Async path — show popup immediately with empty list + spinner,
                // then fill the grid once the loader completes.
                _lovPopup.ShowAt(new List<SimpleItem>(), origin, Width, preloadSearch: "");
                await _lovPopup.LoadItemsAsync(ItemsLoader, preloadSearch);

                // After a successful async load, keep _items in sync for subsequent sync opens
                // (e.g. if the control is used offline without a loader)
            }
            else
            {
                // Synchronous path: items already in _items list
                _lovPopup.ShowAt(_items, origin, Width, preloadSearch);
            }
        }

        /// <summary>Clears the current selection and raises <see cref="SelectionChanged"/>.</summary>
        private void ClearSelection()
        {
            if (_keyTextBox != null) _keyTextBox.Text = string.Empty;
            _selectedDisplayValue = string.Empty;
            _lastValidKey = null;
            OnSelectionChanged();
            Invalidate();
        }

        // ── Keyboard Navigation (Phase 8) ─────────────────────────────────
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // F9 — Oracle Forms standard to open LOV
            if (keyData == Keys.F9)
            {
                string preload = _keyTextBox?.Focused == true ? _keyTextBox.Text : string.Empty;
                OpenPopup(preload);
                return true;
            }

            // Alt+Down — Windows combobox standard
            if (keyData == (Keys.Alt | Keys.Down))
            {
                OpenPopup();
                return true;
            }

            // Delete / Backspace — clear the current selection
            if ((keyData == Keys.Delete || keyData == Keys.Back)
                && !string.IsNullOrEmpty(SelectedKey)
                && !(_keyTextBox?.Focused == true))
            {
                ClearSelection();
                return true;
            }

            // Escape when popup is open — close it
            if (keyData == Keys.Escape && _lovPopup?.Visible == true)
            {
                _lovPopup.Hide();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void KeyTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_keyTextBox == null) return;

            string newKey = _keyTextBox.Text;
            if (ValidateKey(newKey))
            {
                UpdateLastValidKey(newKey);
                UpdateDisplayValue();
                // Clear any previous validation error
                if (HasError)
                {
                    ErrorText = string.Empty;
                    HasError  = false;
                }
            }
            else if (!string.IsNullOrEmpty(newKey))
            {
                // Show error inline (BaseControl ErrorText) + notification tooltip
                ErrorText = "Invalid key — not in the list.";
                HasError  = true;
                ShowNotification("Invalid key. Please enter a valid value from the list.",
                                 ToolTipType.Warning, 2000);
                _keyTextBox.Text = _lastValidKey?.ToString() ?? string.Empty;
                UpdateDisplayValue();
            }
        }
        #endregion

        #region Helper Methods
        private bool ValidateKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return true;
            return _items.Any(i => i.Value?.ToString() == key);
        }

        private void UpdateLastValidKey(string key)
        {
            var selectedItem = _items.FirstOrDefault(i => i.Value?.ToString() == key);
            _lastValidKey = selectedItem?.Value;
        }

        private void UpdateDisplayValue()
        {
            var selectedItem = _items.FirstOrDefault(i => i.Value?.ToString() == SelectedKey);
            _selectedDisplayValue = selectedItem?.Text ?? string.Empty;
            Invalidate();
        }

        private void SetSelectedItem(SimpleItem item)
        {
            if (item == null) return;
            
            if (_keyTextBox != null)
            {
                _keyTextBox.Text = item.Value?.ToString() ?? string.Empty;
            }
            _lastValidKey = item.Value;
            UpdateDisplayValue();
            
            // Raise selection changed event
            OnSelectionChanged();
            
            Invalidate();
        }

        public void Reset()
        {
            _items.Clear();
            if (_keyTextBox != null)
                _keyTextBox.Text = string.Empty;
            _selectedDisplayValue = string.Empty;
            _lastValidKey = null;
            Invalidate();
        }
        
        /// <summary>
        /// Raises the SelectionChanged event
        /// </summary>
        protected virtual void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Event raised when the selected item changes
        /// </summary>
        public event EventHandler SelectionChanged;
        #endregion

        #region Theme and Value Management
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            // Rebuild cached fonts from the current theme
            RebuildFonts();

            if (_keyTextBox == null)
                return;

            // Apply theme to key textbox
            _keyTextBox.Theme         = _currentTheme?.ThemeName ?? Theme;
            _keyTextBox.UseThemeColors = UseThemeColors;
            if (_fieldFont != null) _keyTextBox.Font = _fieldFont;
            _keyTextBox.ApplyTheme();

            // Forward theme to popup if it already exists
            if (_lovPopup != null && !_lovPopup.IsDisposed)
            {
                _lovPopup.LovTheme       = _currentTheme?.ThemeName ?? Theme;
                _lovPopup.UseThemeColors = UseThemeColors;
                _lovPopup.CurrentTheme   = _currentTheme;
                _lovPopup.ApplyLovTheme();
            }

            Invalidate();
        }

        public override void SetValue(object value)
        {
            if (value is SimpleItem item)
            {
                SetSelectedItem(item);
            }
            else if (value != null)
            {
                SelectedKey = value.ToString();
            }
            else
            {
                SelectedKey = string.Empty;
            }
        }

        public override object GetValue()
        {
            return _items.FirstOrDefault(i => i.Value?.ToString() == SelectedKey);
        }

        /// <summary>Rebuilds <see cref="_fieldFont"/> and <see cref="_badgeFont"/> from the
        /// current theme using <see cref="LovFontHelpers"/>. Safe to call repeatedly.</summary>
        private void RebuildFonts()
        {
            Font newField = LovFontHelpers.GetLovFontFromTheme(_currentTheme);
            Font newBadge = LovFontHelpers.GetBadgeFontFromTheme(_currentTheme);

            // Dispose old instances only when they differ
            if (_fieldFont != null && !ReferenceEquals(_fieldFont, newField))
            {
                _fieldFont.Dispose();
            }
            _fieldFont = newField;

            if (_badgeFont != null && !ReferenceEquals(_badgeFont, newBadge))
            {
                _badgeFont.Dispose();
            }
            _badgeFont = newBadge;
        }
        
        /// <summary>
        /// Gets the selected SimpleItem
        /// </summary>
        [Browsable(false)]
        public SimpleItem SelectedItem
        {
            get => _items.FirstOrDefault(i => i.Value?.ToString() == SelectedKey);
        }

        // ── LOV Popup Configuration ─────────────────────────────────────

        [Browsable(true)]
        [Category("LOV")]
        [Description("Title shown in the selection popup header.")]
        public string LovTitle { get; set; } = "Select Value";

        [Browsable(true)]
        [Category("LOV")]
        [Description("Maximum height of the selection popup.")]
        public int MaxPopupHeight { get; set; } = 360;

        [Browsable(true)]
        [Category("LOV")]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Optional explicit column definitions for the popup grid. Leave empty for auto Key+Value columns.")]
        public List<BeepColumnConfig> LovColumns { get; set; } = new List<BeepColumnConfig>();
        // ── Phase 12: Async item loader ──────────────────────────
        /// <summary>
        /// Optional async factory used to populate the LOV popup.
        /// When set to a non-null delegate, opening the popup will show a
        /// loading spinner immediately, then call this delegate on a background
        /// thread.  The results replace <see cref="ListItems"/> once loaded.
        /// When null (default) the popup is populated synchronously from
        /// <see cref="ListItems"/>.
        /// </summary>
        [Browsable(false)]
        public Func<CancellationToken, Task<List<SimpleItem>>>? ItemsLoader { get; set; }

        // ── Phase 13: recent selections ──────────────────────────
        /// <summary>
        /// The most-recent selections made through this control’s LOV popup.
        /// Ordered oldest-first; capped at 5 items.
        /// You can persist this list (e.g. to user settings) and re-assign it
        /// to restore the history on the next session.
        /// </summary>
        [Browsable(false)]
        public List<SimpleItem> RecentSelections
        {
            get => new List<SimpleItem>(_recentHistory);
            set
            {
                _recentHistory = value ?? new List<SimpleItem>();
                // Sync into the popup if it is already open
                if (_lovPopup != null && !_lovPopup.IsDisposed)
                    _lovPopup.RecentItems = _recentHistory;
            }
        }        // ── Label / helper text convenience overrides ──────────────────────
        // Shadow base properties so that setting a non-empty value auto-enables the On flag.

        [Browsable(true)]
        [Category("LOV")]
        [Description("Label text shown above the field. Setting this also enables LabelTextOn.")]
        public new string LabelText
        {
            get => base.LabelText;
            set
            {
                base.LabelText = value;
                if (!string.IsNullOrEmpty(value) && !LabelTextOn)
                    LabelTextOn = true;
            }
        }

        [Browsable(true)]
        [Category("LOV")]
        [Description("Helper / hint text shown below the field. Setting this also enables HelperTextOn.")]
        public new string HelperText
        {
            get => base.HelperText;
            set
            {
                base.HelperText = value;
                if (!string.IsNullOrEmpty(value) && !HelperTextOn)
                    HelperTextOn = true;
            }
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_lovPopup != null)
                {
                    _lovPopup.ItemAccepted -= LovPopup_ItemAccepted;
                    _lovPopup.Cancelled    -= LovPopup_Cancelled;
                    if (!_lovPopup.IsDisposed)
                        _lovPopup.Close();
                    _lovPopup.Dispose();
                    _lovPopup = null;
                }
                
                if (_keyTextBox != null)
                {
                    _keyTextBox.TextChanged -= KeyTextBox_TextChanged;
                    _keyTextBox.Dispose();
                    _keyTextBox = null;
                }
                
                _fieldFont?.Dispose();
                _fieldFont = null;
                _badgeFont?.Dispose();
                _badgeFont = null;
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}

