using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.TextFields;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Multi-column LOV selection popup backed by <see cref="BeepGridPro"/>.
    /// Replaces the single-column <c>BeepContextMenu</c> used by <see cref="BeepListofValuesBox"/>.
    /// The grid automatically inherits all control styles and theming from the host application.
    /// </summary>
    public class BeepLovPopup : Form
    {
        #region Child Controls
        private Panel       _headerPanel  = null!;
        private BeepTextBox _searchBox    = null!;
        private BeepButton  _closeButton  = null!;
        private BeepGridPro _grid         = null!;
        private Panel       _footerPanel  = null!;
        private Label       _countLabel   = null!;
        private BeepButton  _cancelButton = null!;
        private BeepButton  _okButton     = null!;
        #endregion

        #region Data
        private List<SimpleItem> _allItems      = new();
        private List<SimpleItem> _filteredItems = new();
        private SimpleItem?      _pendingSelection;
        // Cancellation for in-flight async loads
        private CancellationTokenSource? _loadCts;
        #endregion

        #region Recent Items
        private FlowLayoutPanel            _recentPanel  = null!;
        private readonly Queue<SimpleItem> _recentItems  = new Queue<SimpleItem>();
        private const int MaxRecentItems = 5;
        private const int RecentPanelH   = 34;
        #endregion

        #region Loading State
        private Panel                          _loadingOverlay = null!;
        private Label                          _loadingLabel   = null!;
        private System.Windows.Forms.Timer     _spinnerTimer   = null!;
        private int                            _spinnerFrame   = 0;
        private static readonly string[]       SpinnerFrames   =
            { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
        #endregion

        #region Theming Properties
        /// <summary>Theme name string forwarded to all BeepControl children.</summary>
        public string  LovTheme        { get; set; } = string.Empty;
        /// <summary>Whether to let theme colors override appearance.</summary>
        public bool    UseThemeColors  { get; set; }
        /// <summary>The resolved theme instance for direct color access.</summary>
        public IBeepTheme? CurrentTheme { get; set; }
        #endregion

        #region Configuration Properties
        /// <summary>Dialog title shown in the header.</summary>
        public string LovTitle { get; set; } = "Select Value";

        /// <summary>
        /// Optional explicit column definitions forwarded to <see cref="BeepGridPro.Columns"/>.
        /// When empty the popup auto-generates Key + Display-Value columns.
        /// </summary>
        public List<BeepColumnConfig> LovColumns { get; set; } = new();

        /// <summary>Maximum height, in pixels, of the popup form.</summary>
        public int MaxPopupHeight { get; set; } = 360;
        #endregion

        #region Result
        /// <summary>The item the user confirmed (set after <see cref="ItemAccepted"/> fires).</summary>
        public SimpleItem? SelectedItem { get; private set; }
        /// <summary>Gets or sets the recent-selection history so the owner control
        /// can persist and restore it across popup open/close cycles.
        /// The list is ordered oldest-first (same as the internal queue).</summary>
        public List<SimpleItem> RecentItems
        {
            get => _recentItems.ToList();
            set
            {
                _recentItems.Clear();
                if (value == null) return;
                // Honour the Max cap while restoring
                foreach (var i in value.TakeLast(MaxRecentItems))
                    _recentItems.Enqueue(i);
                UpdateRecentPanel();
            }
        }

        #endregion

        #region Events
        /// <summary>Raised when the user confirms a selection (OK / double-click / Enter).</summary>
        public event EventHandler<SimpleItem>? ItemAccepted;

        /// <summary>Raised when the user dismisses without selecting (Cancel / Escape / close).</summary>
        public event EventHandler? Cancelled;

        /// <summary>Raised whenever the search text changes (useful for server-side filtering).</summary>
        public event EventHandler<string>? SearchChanged;
        #endregion

        #region Layout Constants
        private const int HeaderH      = 44;
        private const int FooterH      = 44;
        private const int MinPopupW    = 280;
        private const int MaxVisRows   = 12;
        private const int BorderPx     = 1;
        // RecentPanelH defined in #region Recent Items
        #endregion

        #region Constructor
        public BeepLovPopup()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar   = false;
            StartPosition   = FormStartPosition.Manual;
            KeyPreview      = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponents();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            // ── Header ─────────────────────────────────────────────────
            _headerPanel = new Panel
            {
                Height = HeaderH,
                Dock   = DockStyle.Top
            };
            _headerPanel.Resize += (_, __) => PositionHeaderControls();

            _searchBox = new BeepTextBox
            {
                IsChild         = true,
                IsFrameless     = false,
                PlaceholderText = "Search…",
                Height          = 26
            };
            _searchBox.TextChanged += SearchBox_TextChanged;

            _closeButton = new BeepButton
            {
                Text           = "✕",
                HideText       = false,
                Width          = 30,
                Height         = 30,
                ShowAllBorders = false,
                IsChild        = true
            };
            _closeButton.Click += (_, __) => Cancel();

            _headerPanel.Controls.Add(_searchBox);
            _headerPanel.Controls.Add(_closeButton);

            // ── Grid ───────────────────────────────────────────────────
            _grid = new BeepGridPro
            {
                Dock                    = DockStyle.Fill,
                ReadOnly                = true,
                MultiSelect             = false,
                ShowNavigator           = false,
                AllowColumnReorder      = false,
                ShowColumnHeaders       = true,
                ColumnHeaderHeight      = 26,
                RowHeight               = 24,
                AllowUserToResizeRows   = false,
                AllowUserToResizeColumns = true
            };
            _grid.RowSelectionChanged += Grid_RowSelectionChanged;
            _grid.MouseDoubleClick    += (_, __) => Accept();

            // ── Footer ─────────────────────────────────────────────────
            _footerPanel = new Panel
            {
                Height = FooterH,
                Dock   = DockStyle.Bottom
            };
            _footerPanel.Resize += (_, __) => PositionFooterControls();

            _countLabel = new Label
            {
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            _cancelButton = new BeepButton
            {
                Text    = "Cancel",
                Width   = 76,
                Height  = 28,
                IsChild = true
            };
            _cancelButton.Click += (_, __) => Cancel();

            _okButton = new BeepButton
            {
                Text    = "OK",
                Width   = 76,
                Height  = 28,
                IsChild = true
            };
            _okButton.Click += (_, __) => Accept();

            _footerPanel.Controls.Add(_countLabel);
            _footerPanel.Controls.Add(_cancelButton);
            _footerPanel.Controls.Add(_okButton);

            // ── Recent items chip strip ──────────────────────────────
            _recentPanel = new FlowLayoutPanel
            {
                Height        = RecentPanelH,
                Dock          = DockStyle.Top,
                Visible       = false,
                Padding       = new Padding(6, 3, 6, 3),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents  = false,
                AutoScroll    = false
            };

            // ── Loading overlay (non-docked, shown on demand) ─────────
            _loadingLabel = new Label
            {
                AutoSize  = false,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font      = new Font("Segoe UI", 11f, FontStyle.Regular),
                Text      = SpinnerFrames[0] + "  Loading…"
            };
            _loadingOverlay = new Panel { Visible = false };
            _loadingOverlay.Controls.Add(_loadingLabel);

            _spinnerTimer = new System.Windows.Forms.Timer { Interval = 80 };
            _spinnerTimer.Tick += (_, __) =>
            {
                _spinnerFrame = (_spinnerFrame + 1) % SpinnerFrames.Length;
                _loadingLabel.Text = SpinnerFrames[_spinnerFrame] + "  Loading…";
            };

            // Dock order (highest z → processed first for docking):
            //   _footerPanel(Bottom) → _headerPanel(Top) → _recentPanel(Top) → _grid(Fill)
            // _loadingOverlay is added last (highest z-order) but non-docked; see PositionLoadingOverlay().
            Controls.Add(_grid);
            Controls.Add(_recentPanel);
            Controls.Add(_headerPanel);
            Controls.Add(_footerPanel);
            Controls.Add(_loadingOverlay);
        }
        #endregion

        #region Public API

        /// <summary>
        /// Loads <paramref name="items"/>, configures the grid, then positions and shows the popup
        /// below <paramref name="screenOrigin"/>. <paramref name="ownerWidth"/> is used as the
        /// minimum popup width so the popup aligns with the parent field.
        /// <paramref name="preloadSearch"/> is written into the search box before filtering so
        /// the popup can open with items already narrowed (e.g. after the user typed text in
        /// the key field and pressed F9).
        /// </summary>
        public void ShowAt(List<SimpleItem> items, Point screenOrigin, int ownerWidth,
                           string preloadSearch = "")
        {
            _allItems        = items ?? new List<SimpleItem>();
            _pendingSelection = null;

            // Pre-seed search if the caller supplied a filter (e.g. user typed text + pressed F9)
            _searchBox.TextChanged -= SearchBox_TextChanged;   // suppress event during setup
            _searchBox.Text  = preloadSearch ?? string.Empty;
            _searchBox.TextChanged += SearchBox_TextChanged;

            FilterItems(_searchBox.Text);

            ConfigureGridColumns();
            RebindGrid();

            // ── Recent chips (rebuilt before sizing so height is known) ───
            UpdateRecentPanel();

            // ── Size the form ──────────────────────────────────────────
            int popupW    = Math.Max(MinPopupW, ownerWidth);
            int visRows   = Math.Min(_filteredItems.Count, MaxVisRows);
            int gridH     = visRows * _grid.RowHeight + _grid.ColumnHeaderHeight + 4;
            int recentH   = _recentPanel.Visible ? RecentPanelH : 0;
            int formH     = HeaderH + recentH + gridH + FooterH;
            formH         = Math.Clamp(formH, HeaderH + recentH + FooterH + 60, MaxPopupHeight);

            Width  = popupW;
            Height = formH;

            ApplyLovTheme();
            PositionLoadingOverlay();

            // ── Screen position ────────────────────────────────────────
            Rectangle screen = Screen.FromPoint(screenOrigin).WorkingArea;
            int x = Math.Clamp(screenOrigin.X, screen.Left, screen.Right  - Width);
            int y = screenOrigin.Y;
            if (y + Height > screen.Bottom)
                y = screenOrigin.Y - Height;

            Location = new Point(x, y);

            UpdateCount();
            PositionHeaderControls();
            PositionFooterControls();

            if (!Visible)
                Show();

            _searchBox.Focus();
        }

        /// <summary>
        /// Applies the current theme to every child control.
        /// Safe to call multiple times (e.g. after the parent control's theme changes).
        /// </summary>
        public void ApplyLovTheme()
        {
            if (CurrentTheme == null) return;

            BackColor = CurrentTheme.PanelBackColor;
            ForeColor = CurrentTheme.ForeColor;

            _headerPanel.BackColor = CurrentTheme.GridHeaderBackColor != Color.Empty
                ? CurrentTheme.GridHeaderBackColor
                : CurrentTheme.PanelBackColor;

            _footerPanel.BackColor    = CurrentTheme.PanelBackColor;
            _countLabel.ForeColor     = CurrentTheme.SecondaryTextColor;
            _countLabel.BackColor     = Color.Transparent;

            // Theme the recent panel chips
            _recentPanel.BackColor = CurrentTheme.PanelBackColor;
            foreach (Control c in _recentPanel.Controls)
            {
                if (c is BeepButton chip)
                {
                    chip.Theme          = LovTheme;
                    chip.UseThemeColors = UseThemeColors;
                    chip.ApplyTheme();
                }
            }

            // Theme loading overlay
            Color accentFg = CurrentTheme.ForeColor;
            _loadingOverlay.BackColor = Color.FromArgb(220, CurrentTheme.PanelBackColor);
            _loadingLabel  .ForeColor = accentFg;
            _loadingLabel  .BackColor = Color.Transparent;

            void ApplyChild(BaseControl c)
            {
                c.Theme         = LovTheme;
                c.UseThemeColors = UseThemeColors;
                c.ApplyTheme();
            }

            ApplyChild(_searchBox);
            ApplyChild(_closeButton);
            ApplyChild(_okButton);
            ApplyChild(_cancelButton);

            _grid.Theme         = LovTheme;
            _grid.UseThemeColors = UseThemeColors;
            _grid.ApplyTheme();

            Invalidate();
        }

        /// <summary>Shows a spinning loading overlay over the grid area.
        /// Call <see cref="EndLoading"/> when items are ready.</summary>
        public void BeginLoading()
        {
            _spinnerFrame = 0;
            _loadingLabel.Text = SpinnerFrames[0] + "  Loading…";
            PositionLoadingOverlay();
            _loadingOverlay.Visible = true;
            _loadingOverlay.BringToFront();
            _spinnerTimer.Start();
        }

        /// <summary>Stops the spinner, loads <paramref name="items"/> into the grid
        /// and hides the overlay.</summary>
        public void EndLoading(List<SimpleItem> items)
        {
            _spinnerTimer.Stop();
            _allItems = items ?? new List<SimpleItem>();
            FilterItems(_searchBox.Text);
            UpdateRecentPanel();
            _loadingOverlay.Visible = false;
        }

        /// <summary>
        /// Shows the popup (with an immediate loading overlay) then fires
        /// <paramref name="loader"/> on a background thread.  When the results
        /// arrive the overlay disappears and the grid is populated.
        /// Any previously in-flight load is cancelled before starting.
        /// </summary>
        /// <param name="loader">Async factory; receives a <see cref="CancellationToken"/>
        /// and must return the full item list.</param>
        /// <param name="preloadSearch">Optional search text to pre-seed.</param>
        public async Task LoadItemsAsync(
            Func<CancellationToken, Task<List<SimpleItem>>> loader,
            string preloadSearch = "")
        {
            if (loader == null) throw new ArgumentNullException(nameof(loader));

            // Cancel any previous in-flight load
            _loadCts?.Cancel();
            _loadCts?.Dispose();
            _loadCts = new CancellationTokenSource();
            var token = _loadCts.Token;

            BeginLoading();

            List<SimpleItem> results;
            try
            {
                results = await Task.Run(() => loader(token), token).ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
                return;   // popup was closed / new load started — silently discard
            }
            catch (Exception ex)
            {
                if (!IsDisposed && IsHandleCreated)
                {
                    _spinnerTimer.Stop();
                    _loadingLabel.Text = "⚠️  Load failed: " + ex.Message;
                }
                return;
            }

            // Check the popup wasn't closed while we were loading
            if (token.IsCancellationRequested || IsDisposed || !IsHandleCreated) return;

            // Pre-seed search before EndLoading so filtering happens in one pass
            if (!string.IsNullOrEmpty(preloadSearch))
            {
                _searchBox.TextChanged -= SearchBox_TextChanged;
                _searchBox.Text  = preloadSearch;
                _searchBox.TextChanged += SearchBox_TextChanged;
            }

            EndLoading(results);
        }

        #endregion

        #region Recent Items Helpers

        /// <summary>Rebuilds the chips strip from <see cref="_recentItems"/>.
        /// Chips are ordered most-recent first and accept their item on click.</summary>
        private void UpdateRecentPanel()
        {
            _recentPanel.SuspendLayout();
            _recentPanel.Controls.Clear();

            // Build chips most-recent first
            var ordered = _recentItems.Reverse().ToList();
            foreach (var item in ordered)
            {
                string  label = item.Text ?? item.Value?.ToString() ?? "?";
                if (label.Length > 18) label = label[..17] + "…";

                var chip = new BeepButton
                {
                    Text             = label,
                    Tag              = item,
                    Height           = 22,
                    AutoSize         = false,
                    Width            = TextRenderer.MeasureText(label,
                                           SystemFonts.DefaultFont).Width + 20,
                    IsChild          = true,
                    ShowAllBorders   = true,
                    HideText         = false,
                    ToolTipText      = item.Description ?? item.Text ?? string.Empty
                };
                chip.Click += (s, _) =>
                {
                    if (s is BeepButton b && b.Tag is SimpleItem si)
                    {
                        SelectedItem = si;
                        ItemAccepted?.Invoke(this, si);
                        // Push again so it stays at the front of recents
                        PushRecent(si);
                        Hide();
                    }
                };

                if (CurrentTheme != null)
                {
                    chip.Theme          = LovTheme;
                    chip.UseThemeColors = UseThemeColors;
                    chip.ApplyTheme();
                }

                _recentPanel.Controls.Add(chip);
            }

            _recentPanel.ResumeLayout(true);
            _recentPanel.Visible = ordered.Count > 0;
        }

        /// <summary>Adds <paramref name="item"/> to the front of the recent-items queue,
        /// evicting the oldest if the queue is full.</summary>
        private void PushRecent(SimpleItem item)
        {
            if (item == null) return;

            // Remove duplicate if already in queue
            var list = _recentItems.ToList();
            list.RemoveAll(i => i.Value?.ToString() == item.Value?.ToString());
            list.Add(item);

            // Keep the queue capped
            while (list.Count > MaxRecentItems)
                list.RemoveAt(0);

            _recentItems.Clear();
            foreach (var i in list)
                _recentItems.Enqueue(i);
        }

        #endregion

        #region Grid Column Configuration

        private void ConfigureGridColumns()
        {
            _grid.Columns.Clear();

            if (LovColumns != null && LovColumns.Count > 0)
            {
                // Caller-supplied column definitions
                foreach (var col in LovColumns)
                    _grid.Columns.Add(col);
            }
            else
            {
                // Default: two-column layout matching SimpleItem.Value and SimpleItem.Text
                _grid.Columns.Add(new BeepColumnConfig
                {
                    ColumnName    = "Value",
                    ColumnCaption = "Key",
                    Width         = 90,
                    MinWidth      = 40,
                    ReadOnly      = true
                });
                _grid.Columns.Add(new BeepColumnConfig
                {
                    ColumnName    = "Text",
                    ColumnCaption = "Display Value",
                    Width         = Math.Max(160, Width - 110),
                    MinWidth      = 80,
                    ReadOnly      = true
                });
            }
        }

        private void RebindGrid()
        {
            _grid.DataSource = new BindingList<SimpleItem>(_filteredItems);
        }

        #endregion

        #region Layout Helpers

        private void PositionHeaderControls()
        {
            const int pad  = 6;
            int btnW  = _closeButton.Width;
            int closeX    = _headerPanel.Width - btnW - pad;
            int searchW   = Math.Max(50, closeX - pad * 2);
            int cy        = (_headerPanel.Height - _searchBox.Height) / 2;

            _searchBox.SetBounds(pad, cy, searchW, _searchBox.Height);
            _closeButton.SetBounds(
                closeX,
                (_headerPanel.Height - _closeButton.Height) / 2,
                btnW,
                _closeButton.Height);
        }

        private void PositionFooterControls()
        {
            const int pad = 8;
            int btnY   = (_footerPanel.Height - _okButton.Height) / 2;
            int okX    = _footerPanel.Width - _okButton.Width - pad;
            int cancelX = okX - _cancelButton.Width - pad;

            _okButton    .SetBounds(okX,     btnY, _okButton    .Width, _okButton    .Height);
            _cancelButton.SetBounds(cancelX, btnY, _cancelButton.Width, _cancelButton.Height);
            _countLabel  .SetBounds(pad, 0,  Math.Max(0, cancelX - pad * 2), _footerPanel.Height);
        }

        /// <summary>Keeps the loading overlay covering the grid (non-docked area).</summary>
        private void PositionLoadingOverlay()
        {
            int yOffset = HeaderH + (_recentPanel?.Visible == true ? RecentPanelH : 0);
            int h       = Math.Max(0, ClientSize.Height - yOffset - FooterH);
            _loadingOverlay?.SetBounds(0, yOffset, ClientSize.Width, h);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PositionLoadingOverlay();
        }

        /// <summary>Paints a 1-px themed border around the popup form.</summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Color borderColor = CurrentTheme?.BorderColor ?? TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.ControlDark);
            using var pen = new Pen(borderColor, BorderPx);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }

        #endregion

        #region Filtering

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            FilterItems(_searchBox.Text);
            SearchChanged?.Invoke(this, _searchBox.Text);
        }

        private void FilterItems(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                _filteredItems = _allItems.ToList();
            }
            else
            {
                string q = search.Trim().ToLowerInvariant();
                _filteredItems = _allItems.Where(i =>
                    (i.Text?       .ToLowerInvariant().Contains(q) ?? false) ||
                    (i.Value?      .ToString().ToLowerInvariant().Contains(q) ?? false) ||
                    (i.Description?.ToLowerInvariant().Contains(q) ?? false)
                ).ToList();
            }

            _pendingSelection = null;
            RebindGrid();
            UpdateCount();
        }

        private void UpdateCount()
        {
            int shown = _filteredItems.Count;
            int total = _allItems.Count;
            _countLabel.Text = shown == total
                ? $"{total} record{(total == 1 ? "" : "s")}"
                : $"{shown} of {total}";
        }

        #endregion

        #region Selection

        private void Grid_RowSelectionChanged(object sender, BeepRowSelectedEventArgs e)
        {
            int idx = e.RowIndex;
            if (idx >= 0 && idx < _filteredItems.Count)
                _pendingSelection = _filteredItems[idx];
        }

        private void Accept()
        {
            // Fall back to first item if nothing was explicitly clicked
            var item = _pendingSelection
                    ?? (_filteredItems.Count > 0 ? _filteredItems[0] : null);
            if (item == null) return;

            PushRecent(item);
            SelectedItem = item;
            ItemAccepted?.Invoke(this, item);
            Hide();
        }

        private void Cancel()
        {
            // Cancel any in-flight async load
            _loadCts?.Cancel();
            Cancelled?.Invoke(this, EventArgs.Empty);
            Hide();
        }

        #endregion

        #region Keyboard & Focus

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Cancel();
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    Accept();
                    e.Handled = true;
                    break;
                case Keys.F9:
                    Accept();
                    e.Handled = true;
                    break;
            }
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            _loadCts?.Cancel();   // stop any pending async load
            if (Visible) Hide();
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _loadCts?       .Cancel();
                _loadCts?       .Dispose();
                _spinnerTimer?  .Stop();
                _spinnerTimer?  .Dispose();
                _searchBox?     .Dispose();
                _closeButton?   .Dispose();
                _grid?          .Dispose();
                _cancelButton?  .Dispose();
                _okButton?      .Dispose();
                _headerPanel?   .Dispose();
                _footerPanel?   .Dispose();
                _recentPanel?   .Dispose();
                _loadingOverlay?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
