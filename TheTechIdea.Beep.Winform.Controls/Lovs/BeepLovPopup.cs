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
using TheTechIdea.Beep.Winform.Controls.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Lovs;
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
        private TableLayoutPanel _headerPanel = null!;
        private BeepTextBox _searchBox    = null!;
        private BeepButton  _closeButton  = null!;
        private BeepGridPro _grid         = null!;
        private TableLayoutPanel _footerPanel = null!;
        private BeepLabel   _countLabel   = null!;
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

        /// <summary>Raised whenever the search text changes.</summary>
        public event EventHandler<string>? SearchChanged;

        /// <summary>
        /// When set, searching asks the source instead of filtering the loaded list.
        /// </summary>
        /// <remarks>
        /// A LOV over a large table cannot load every row to filter it in memory. With this set the
        /// grid shows what the source returns for the current search text, debounced so a typist does
        /// not launch a query per keystroke.
        /// </remarks>
        public Func<string, CancellationToken, Task<List<SimpleItem>>>? SearchLoader { get; set; }

        private System.Windows.Forms.Timer? _searchDebounce;
        private CancellationTokenSource? _searchCts;

        /// <summary>How long typing must pause before a server search is issued.</summary>
        public int SearchDebounceMs { get; set; } = 250;

        /// <summary>
        /// Most rows the popup will bind at once. 0 means no cap.
        /// </summary>
        /// <remarks>
        /// A loader is free to return everything, and a LOV over a large table often will. Binding all
        /// of it froze the popup with no indication why. The cap bounds that, and the count line says
        /// plainly that it is showing a subset — silently truncating would be worse than slow.
        /// </remarks>
        public int MaxRows { get; set; } = 500;

        /// <summary>Rows dropped by <see cref="MaxRows"/> in the current view, or 0.</summary>
        private int _cappedAway;

        /// <summary>
        /// When set, the popup asks the source for one window of rows at a time.
        /// </summary>
        /// <remarks>
        /// <see cref="MaxRows"/> bounds what the client binds; it does not stop a source fetching fifty
        /// thousand rows so the popup can throw 49,500 away. This is the loader that fixes the query
        /// cost rather than the render cost, and it takes precedence over
        /// <see cref="SearchLoader"/> when both are set.
        /// </remarks>
        public Func<string, int, int, CancellationToken, Task<LovPage>>? PageLoader { get; set; }

        /// <summary>Rows per window when <see cref="PageLoader"/> is used.</summary>
        public int PageSize { get; set; } = 100;

        private int  _pageOffset;
        private int  _pageTotal = -1;
        private bool _lastPageWasFull;

        private BeepButton _prevPageButton = null!;
        private BeepButton _nextPageButton = null!;
        #endregion

        #region Layout Constants
        private const int HeaderH      = 44;
        private const int FooterH      = 44;
        private const int MinPopupW    = 280;
        /// <summary>Rows the popup always shows room for. The only input to its height.</summary>
        public int VisibleRows { get; set; } = 10;
        private const int BorderPx     = 1;
        /// <summary>Slack for the grid's own scrollbar and edges.</summary>
        private const int GridChromeH  = 18;
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
            // Search fills, the close button hugs. Both used to be given bounds by arithmetic on every
            // resize; the column styles say the same thing once, declaratively.
            _headerPanel = new TableLayoutPanel
            {
                Height      = HeaderH,
                Dock        = DockStyle.Top,
                ColumnCount = 2,
                RowCount    = 1,
                Padding     = new Padding(6, 0, 6, 0),
            };
            _headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            _headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _headerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

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

            _searchBox.Dock    = DockStyle.Fill;
            _searchBox.Margin  = new Padding(0, 9, 6, 9);
            _closeButton.Margin = new Padding(0, 7, 0, 7);
            _closeButton.Anchor = AnchorStyles.None;

            _headerPanel.Controls.Add(_searchBox,   0, 0);
            _headerPanel.Controls.Add(_closeButton, 1, 0);

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
                AllowUserToResizeColumns = true,

                // A LOV result set is exactly what virtualization is for: only the visible rows are
                // materialised, so binding a few hundred candidates costs a screenful of work.
                EnableVirtualization    = true,

                // BeepGridPro brings its own title, toolbar, search box and import/export/print
                // buttons. Inside a value picker that is a second search box next to the popup's own
                // and a print button on a list you are choosing from - all of it off.
                ShowGridTitle           = false,
                ShowToolbar             = false,
                ShowFilterButton        = false,
                ShowTopFilterPanel      = false,
                ShowAddButton           = false,
                ShowEditButton          = false,
                ShowDeleteButton        = false,
                ShowCheckBox            = false,
            };


            _grid.RowSelectionChanged += Grid_RowSelectionChanged;
            _grid.MouseDoubleClick    += (_, __) => Accept();

            // ── Footer ─────────────────────────────────────────────────
            // Count fills, the two buttons hug the right edge.
            _footerPanel = new TableLayoutPanel
            {
                Height      = FooterH,
                Dock        = DockStyle.Bottom,
                ColumnCount = 3,
                RowCount    = 1,
                Padding     = new Padding(8, 0, 8, 0),
            };
            _footerPanel.ColumnCount = 5;
            _footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            _footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));   // prev
            _footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));   // next
            _footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _footerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            _countLabel = new BeepLabel
            {
                AutoSize    = false,
                Dock        = DockStyle.Fill,
                IsChild     = true,
                IsFrameless = true,
                TextAlign   = ContentAlignment.MiddleLeft,
                Margin      = new Padding(0),
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

            _prevPageButton = new BeepButton
            {
                Text = "‹", Width = 30, Height = 26, IsChild = true, Visible = false,
                AccessibleName = "Previous page", Margin = new Padding(0, 9, 4, 9), Anchor = AnchorStyles.None,
            };
            _prevPageButton.Click += async (_, __) => await LoadPage(_pageOffset - PageSize).ConfigureAwait(true);

            _nextPageButton = new BeepButton
            {
                Text = "›", Width = 30, Height = 26, IsChild = true, Visible = false,
                AccessibleName = "Next page", Margin = new Padding(0, 9, 8, 9), Anchor = AnchorStyles.None,
            };
            _nextPageButton.Click += async (_, __) => await LoadPage(_pageOffset + PageSize).ConfigureAwait(true);

            _cancelButton.Margin = new Padding(0, 8, 8, 8);
            _okButton.Margin     = new Padding(0, 8, 0, 8);
            _cancelButton.Anchor = AnchorStyles.None;
            _okButton.Anchor     = AnchorStyles.None;

            _footerPanel.Controls.Add(_countLabel,     0, 0);
            _footerPanel.Controls.Add(_prevPageButton, 1, 0);
            _footerPanel.Controls.Add(_nextPageButton, 2, 0);
            _footerPanel.Controls.Add(_cancelButton,   3, 0);
            _footerPanel.Controls.Add(_okButton,       4, 0);

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
                           string preloadSearch = "", int ownerHeight = 0)
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

            _ownerWidth   = ownerWidth;
            _anchor       = screenOrigin;
            _anchorOwnerH = ownerHeight;
            ApplyFixedSize();

            ApplyLovTheme();
            PositionLoadingOverlay();

            PlaceOnScreen();

            UpdateCount();

            if (!Visible)
                Show();

            _searchBox.Focus();

            if (PageLoader != null)
            {
                _pageOffset = 0;
                _pageTotal  = -1;
                _ = LoadPage(0);
            }
        }

        private int   _ownerWidth = MinPopupW;
        private Point _anchor;          // screen point the popup hangs from (the field's bottom-left)
        private int   _anchorOwnerH;    // the field's height, so the popup can flip above it

        /// <summary>
        /// Sizes the popup to the rows it is actually showing.
        /// </summary>
        /// <remarks>
        /// Called again whenever rows arrive. Sizing happened once in <c>ShowAt</c>, before any async
        /// load had returned — so a popup opened with a page loader was measured against an empty list
        /// and rendered with a grid of zero height: a search box, a footer, and no rows at all.
        /// </remarks>
        /// <summary>
        /// Gives the popup its fixed geometry.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>The size does not depend on what the popup contains.</b> Height is always
        /// <see cref="VisibleRows"/> rows plus the header, footer and column header; width is always
        /// the field's width, floored at <see cref="MinPopupW"/>. Three rows or three hundred, two
        /// columns or five, the dropdown is the same size.
        /// </para>
        /// <para>
        /// It used to be derived from the row count, so the same control produced a 305px popup for
        /// four departments and a 349px one for a page of twenty-five — and every attempt to correct
        /// the column arithmetic on top of that left a gap on one side or a scrollbar on the other,
        /// because the geometry itself was the moving part.
        /// </para>
        /// </remarks>
        private void ApplyFixedSize()
        {
            int recentH = _recentPanel.Visible ? RecentPanelH : 0;
            int gridH   = (VisibleRows * _grid.RowHeight) + _grid.ColumnHeaderHeight + GridChromeH;

            int formH  = HeaderH + recentH + gridH + FooterH;

            // The dropdown is exactly as wide as the control it drops from. There was a MinPopupW
            // floor of 280px here, so a 150px field opened a 280px popup that overhung it by 130px -
            // and the one anchored test used a 300px field, which is above the floor, so the mismatch
            // could not show. A dropdown wider than its control is not a dropdown.
            int popupW = _ownerWidth > 0 ? _ownerWidth : MinPopupW;

            if (Width == popupW && Height == formH) return;

            Width  = popupW;
            Height = formH;

            PositionLoadingOverlay();

            // Re-place after re-sizing: the recent-chips strip can appear between opens, and the popup
            // must stay against the field rather than keeping an old top edge.
            PlaceOnScreen();
        }

        /// <summary>
        /// Puts the popup against the field it was opened from, flipping above it when there is no
        /// room below.
        /// </summary>
        private void PlaceOnScreen()
        {
            if (_anchor.IsEmpty) return;

            Rectangle screen = Screen.FromPoint(_anchor).WorkingArea;

            int x = Math.Clamp(_anchor.X, screen.Left, Math.Max(screen.Left, screen.Right - Width));

            // Below the field by default; above it if the popup would fall off the bottom and there is
            // more room above. _anchor is the field's BOTTOM edge, so flipping subtracts the field's
            // own height as well as the popup's.
            int below = _anchor.Y;
            int above = _anchor.Y - _anchorOwnerH - Height;

            int y = below;
            if (below + Height > screen.Bottom && above >= screen.Top)
                y = above;

            y = Math.Clamp(y, screen.Top, Math.Max(screen.Top, screen.Bottom - Height));

            Location = new Point(x, y);
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
            ApplyChild(_countLabel);

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
            ApplyFixedSize();
            FitColumns();
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
        public async Task<List<SimpleItem>> LoadItemsAsync(
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
                // Expected flow rather than a failure - the popup was closed or a newer load started -
                // so this reports at Info, not Error. It still reports: a LOV that opens empty is a
                // support question, and "the load was cancelled" is the answer.
                BeepLog.Info(this, "LOV load cancelled", "popup closed or superseded by a newer load");

                // Stop the spinner. This used to return without touching it, leaving the timer ticking
                // on a hidden form for the life of the process.
                StopSpinner();
                return new List<SimpleItem>();
            }
            catch (Exception ex)
            {
                BeepLog.Failure(this, "load LOV items", ex);

                if (!IsDisposed && IsHandleCreated)
                {
                    _spinnerTimer.Stop();
                    _loadingLabel.Text = "Load failed - " + ex.Message;
                }
                return new List<SimpleItem>();
            }

            // Check the popup wasn't closed while we were loading
            if (token.IsCancellationRequested || IsDisposed || !IsHandleCreated)
            {
                StopSpinner();
                return new List<SimpleItem>();
            }

            // Pre-seed search before EndLoading so filtering happens in one pass
            if (!string.IsNullOrEmpty(preloadSearch))
            {
                _searchBox.TextChanged -= SearchBox_TextChanged;
                _searchBox.Text  = preloadSearch;
                _searchBox.TextChanged += SearchBox_TextChanged;
            }

            EndLoading(results);
            return results ?? new List<SimpleItem>();
        }

        /// <summary>Stops the spinner and hides the overlay, safe on a disposed or unshown form.</summary>
        private void StopSpinner()
        {
            if (IsDisposed) return;

            _spinnerTimer?.Stop();
            if (_loadingOverlay != null) _loadingOverlay.Visible = false;
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
                    Width         = 90,     // seed ratio only; FitColumns scales it to the popup
                    MinWidth      = 0,
                    ReadOnly      = true
                });
                _grid.Columns.Add(new BeepColumnConfig
                {
                    ColumnName    = "Text",
                    ColumnCaption = "Display Value",
                    Width         = 210,    // seed ratio only; FitColumns scales it to the popup
                    MinWidth      = 0,
                    ReadOnly      = true
                });
            }
        }

        /// <summary>
        /// Makes the columns span the grid exactly, with the display column taking the slack.
        /// </summary>
        /// <remarks>
        /// The display column's width was computed in <c>ConfigureGridColumns</c> as
        /// <c>Width - 110</c> — the <b>popup form's</b> width, read before the popup had been sized
        /// the final width, and counting neither the grid's own borders nor its vertical scrollbar. The
        /// columns therefore overflowed the grid and it grew a horizontal scrollbar, which is the
        /// misalignment inside the dropdown: the header and the rows ran wider than the popup.
        ///
        /// Fitting has to happen after the popup is sized and again whenever it resizes, which is why
        /// it is not a one-time calculation.
        /// </remarks>
        private void FitColumns()
        {
            if (_grid?.Columns == null || _grid.Columns.Count == 0) return;

            var visible = _grid.Columns.Where(c => c.Visible).ToList();
            if (visible.Count == 0) return;

            // No reserve of any kind: the columns fill the grid's full width. Holding space back for a
            // scrollbar left a blank strip down the right whenever the list was short, and making the
            // reserve conditional just moved the inconsistency around - the columns changed width
            // depending on how many rows happened to be loaded.
            int available = _grid.ClientSize.Width;
            if (available <= 0) return;

            int current = visible.Sum(c => c.Width);
            if (current <= 0) return;

            // Scale every visible column by the same factor so they fill the width exactly. Giving one
            // column a fixed width and the last one the remainder only works while the popup is wide:
            // a 150px dropdown left 128px for a 90px key column plus an 80px minimum display column,
            // so the columns overflowed and the text was clipped inside a popup that was itself the
            // right size. Scaling has no such threshold, and it does not care how many columns there
            // are or which ones the grid decided to create.
            double factor = (double)available / current;

            int used = 0;
            for (int i = 0; i < visible.Count; i++)
            {
                var col = visible[i];

                // The last column absorbs the rounding, so the total lands on `available` exactly.
                int width = i == visible.Count - 1
                    ? available - used
                    : Math.Max(1, (int)Math.Round(col.Width * factor));

                col.MinWidth = 0;   // a minimum wider than its share is what forced the overflow
                col.Width = Math.Max(1, width);
                used += col.Width;
            }
        }

        /// <summary>
        /// Takes the gutter columns out of a value picker, and takes their width with them.
        /// </summary>
        /// <remarks>
        /// Hiding a column does not make it zero-width, and the grid's scroll extent counts the width
        /// either way. The selection, row-number and row-ID gutters carried 29 + 30 + 30 = 89px of
        /// invisible width, so on a 340px popup the columns summed to 407 and the grid grew a
        /// horizontal scrollbar under rows that looked like they fitted.
        /// </remarks>
        private void HideGutterColumns()
        {
            try
            {
                foreach (var col in _grid.Columns.Where(c => c.IsRowNumColumn || c.IsRowID || c.IsSelectionCheckBox))
                {
                    col.Visible = false;
                    col.MinWidth = 0;
                    col.Width = 0;
                }
            }
            catch (Exception ex)
            {
                // Cosmetic: a visible gutter is not worth failing to open the popup over.
                BeepLog.Fallback(this, "hide the LOV gutter columns", ex);
            }
        }

        private void RebindGrid()
        {
            var bound = _filteredItems;
            _cappedAway = 0;

            if (MaxRows > 0 && bound.Count > MaxRows)
            {
                _cappedAway = bound.Count - MaxRows;
                bound = bound.Take(MaxRows).ToList();
            }

            _grid.DataSource = new BindingList<SimpleItem>(bound);

            // AFTER binding, not after Columns.Clear(): the grid re-creates its own selection,
            // row-number and row-ID columns as part of binding, so hiding them any earlier hid columns
            // that did not exist yet and were then recreated visible.
            HideGutterColumns();
            FitColumns();
        }

        #endregion

        #region Layout Helpers

        // PositionHeaderControls and PositionFooterControls are gone. They set bounds by arithmetic on
        // every resize - close button width subtracted from panel width, search width derived from
        // that, buttons walked leftwards from the right edge. The column styles state the same
        // intent once and the layout engine keeps it true.

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
            FitColumns();
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
            SearchChanged?.Invoke(this, _searchBox.Text);

            if (PageLoader == null && SearchLoader == null)
            {
                FilterItems(_searchBox.Text);
                return;
            }

            // Server-side: restart the debounce rather than querying on every keystroke.
            _searchDebounce ??= CreateDebounceTimer();
            _searchDebounce.Stop();
            _searchDebounce.Interval = Math.Max(1, SearchDebounceMs);
            _searchDebounce.Start();
        }

        private System.Windows.Forms.Timer CreateDebounceTimer()
        {
            var timer = new System.Windows.Forms.Timer();
            timer.Tick += async (_, __) =>
            {
                timer.Stop();

                // A new search starts at the first window again.
                if (PageLoader != null) await LoadPage(0).ConfigureAwait(true);
                else await RunServerSearch(_searchBox.Text).ConfigureAwait(true);
            };
            return timer;
        }

        /// <summary>
        /// Fetches one window of rows starting at <paramref name="offset"/>.
        /// </summary>
        /// <remarks>
        /// The offset is clamped rather than trusted: the buttons are the usual caller, but a search
        /// landing on a smaller result set can leave the current offset past the end.
        /// </remarks>
        private async Task LoadPage(int offset)
        {
            if (PageLoader == null || IsDisposed) return;

            string search = _searchBox.Text;
            offset = Math.Max(0, offset);
            if (_pageTotal >= 0 && offset >= _pageTotal) offset = Math.Max(0, _pageTotal - PageSize);

            _searchCts?.Cancel();
            _searchCts?.Dispose();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            BeginLoading();

            try
            {
                var page = await Task.Run(() => PageLoader(search, offset, PageSize, token), token)
                                     .ConfigureAwait(true);
                if (token.IsCancellationRequested || IsDisposed) { StopSpinner(); return; }

                page ??= LovPage.Empty;

                _pageOffset       = offset;
                _pageTotal        = page.Total;
                _lastPageWasFull  = page.Rows.Count >= PageSize;
                _allItems         = page.Rows;
                _filteredItems    = page.Rows.ToList();
                _pendingSelection = null;

                RebindGrid();
                UpdateCount();
                UpdatePageButtons();
                ApplyFixedSize();
                FitColumns();
                StopSpinner();
            }
            catch (OperationCanceledException)
            {
                BeepLog.Info(this, "LOV page load cancelled", $"superseded while fetching offset {offset}");
                StopSpinner();
            }
            catch (Exception ex)
            {
                BeepLog.Failure(this, $"load LOV page at offset {offset}", ex);
                if (!IsDisposed && IsHandleCreated)
                {
                    _spinnerTimer.Stop();
                    _loadingLabel.Text = "Load failed - " + ex.Message;
                }
            }
        }

        /// <summary>Shows the paging buttons only when there is somewhere to page to.</summary>
        private void UpdatePageButtons()
        {
            bool paging = PageLoader != null;

            _prevPageButton.Visible = paging && _pageOffset > 0;
            _nextPageButton.Visible = paging && (_pageTotal >= 0
                ? _pageOffset + PageSize < _pageTotal
                : _lastPageWasFull);   // no total reported: a full window implies there may be more
        }

        /// <summary>Asks <see cref="SearchLoader"/> for the rows matching <paramref name="search"/>.</summary>
        private async Task RunServerSearch(string search)
        {
            if (SearchLoader == null || IsDisposed) return;

            _searchCts?.Cancel();
            _searchCts?.Dispose();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            BeginLoading();

            try
            {
                var results = await Task.Run(() => SearchLoader(search, token), token).ConfigureAwait(true);
                if (token.IsCancellationRequested || IsDisposed) { StopSpinner(); return; }

                // The source has already filtered, so these become the list AND the filtered view.
                _allItems      = results ?? new List<SimpleItem>();
                _filteredItems = _allItems.ToList();
                _pendingSelection = null;

                RebindGrid();
                UpdateCount();
                ApplyFixedSize();
                StopSpinner();
            }
            catch (OperationCanceledException)
            {
                BeepLog.Info(this, "LOV search cancelled", $"superseded while searching for '{search}'");
                StopSpinner();
            }
            catch (Exception ex)
            {
                BeepLog.Failure(this, $"search LOV for '{search}'", ex);
                if (!IsDisposed && IsHandleCreated)
                {
                    _spinnerTimer.Stop();
                    _loadingLabel.Text = "Search failed - " + ex.Message;
                }
            }
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
            int matched = _filteredItems.Count;
            int total   = _allItems.Count;
            int shown   = matched - _cappedAway;

            if (PageLoader != null)
            {
                int first = _pageOffset + 1;
                int last  = _pageOffset + matched;

                _countLabel.Text = _pageTotal >= 0
                    ? $"{first}-{last} of {_pageTotal}"
                    : $"{first}-{last}";
                return;
            }

            if (_cappedAway > 0)
            {
                // Say it rather than quietly showing fewer rows than were found.
                _countLabel.Text = $"showing first {shown} of {matched} - narrow your search";
                return;
            }

            _countLabel.Text = matched == total
                ? $"{total} record{(total == 1 ? "" : "s")}"
                : $"{matched} of {total}";
        }

        #endregion

        #region Selection

        private void Grid_RowSelectionChanged(object sender, BeepRowSelectedEventArgs e)
        {
            int idx = e.RowIndex;
            if (idx >= 0 && idx < _filteredItems.Count)
                _pendingSelection = _filteredItems[idx];
        }

        /// <summary>
        /// Confirms the highlighted row, or the only row left after filtering.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This used to fall back to <c>_filteredItems[0]</c> whenever nothing was highlighted, so
        /// pressing Enter in the search box committed whatever happened to be at the top of the list.
        /// On a control whose job is to put a foreign key into a record, silently choosing a row the
        /// user never looked at is the worst thing it can do.
        /// </para>
        /// <para>
        /// A single remaining row is different, and is kept: narrowing a search until one candidate is
        /// left and pressing Enter is how a LOV is meant to be driven, and there is nothing ambiguous
        /// about the choice.
        /// </para>
        /// </remarks>
        private void Accept()
        {
            var item = _pendingSelection
                    ?? (_filteredItems.Count == 1 ? _filteredItems[0] : null);

            if (item == null)
            {
                // Nothing chosen and more than one candidate: say so rather than guessing.
                if (_filteredItems.Count > 1) FocusGrid();
                return;
            }

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
                case Keys.F9:
                    Accept();
                    e.Handled = true;
                    break;

                // Down from the search box moves into the list and highlights the first row. Without
                // it the only way to choose a row was the mouse, since Enter deliberately no longer
                // guesses one.
                case Keys.Down when _searchBox.Focused:
                    FocusGrid();
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>Moves focus to the grid and highlights the first row if none is highlighted.</summary>
        private void FocusGrid()
        {
            if (_filteredItems.Count == 0) return;

            _grid.Focus();

            if (_pendingSelection == null)
            {
                _pendingSelection = _filteredItems[0];
                try { _grid.Selection.SelectCell(0, 0); }
                catch (Exception ex)
                {
                    // Highlighting is a convenience; failing to do it must not stop the popup working.
                    BeepLog.Fallback(this, "highlight the first LOV row", ex);
                }
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
                _searchCts?     .Cancel();
                _searchCts?     .Dispose();
                _searchDebounce?.Stop();
                _searchDebounce?.Dispose();
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
