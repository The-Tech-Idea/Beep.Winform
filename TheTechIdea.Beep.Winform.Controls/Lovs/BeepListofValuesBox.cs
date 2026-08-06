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
using TheTechIdea.Beep.Winform.Controls.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.ToolTips;
using TheTechIdea.Beep.Winform.Controls.Lovs;
using TheTechIdea.Beep.Winform.Controls.Lovs.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;
using TheTechIdea.Beep.Icons;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep List of Values Box")]
    [Description("A control that displays a list of values with a popup context menu selection, similar to Oracle Forms LOV.")]
    public partial class BeepListofValuesBox : BaseControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.ListOfValues;
        protected internal override Padding StylePadding => new Padding(0);
        #region Fields
        private BeepTextBox  _keyTextBox;
        // _valueTextBox removed (Phase 6) â€” display value is painted directly in DrawContent
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

        // â”€â”€ Phase 6 options â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        // Off by default. The badge was built to show the key inside a painted value area; with a real
        // key text box beside it the same key appeared twice, once in the box and once in the pill.
        private bool _showKeyBadge = false;

        // â”€â”€ Phase 13: recent-selection history (persisted across popup opens) â”€â”€
        private List<SimpleItem> _recentHistory = new List<SimpleItem>();

        /// <summary>
        /// Every item this control has seen, from <see cref="ListItems"/> or from a loader.
        /// </summary>
        /// <remarks>
        /// Validation and display lookup used to consult <c>ListItems</c> alone. <c>ItemsLoader</c>
        /// filled the popup and never wrote back, so with a loader set — the query-backed case an
        /// Oracle LOV exists for — a key the loader had just returned failed validation and was
        /// reverted, including the one the user had picked from the popup a moment earlier.
        /// </remarks>
        private readonly Dictionary<string, SimpleItem> _known = new(StringComparer.Ordinal);

        private bool _restrictToList = true;

        /// <summary>
        /// Set while reverting the key box, so the resulting TextChanged does not undo the rejection.
        /// </summary>
        /// <remarks>
        /// Reverting assigns the text box, which re-enters KeyTextBox_TextChanged with the reverted
        /// key. That key is usually empty, empty is always valid, and the valid branch cleared the very
        /// error the revert was raised for - so a refused key reverted silently after all.
        /// </remarks>
        private bool _reverting;
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
                Remember(_items);
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
                string key = value ?? string.Empty;

                // Assigning Text runs the whole validation cycle synchronously through
                // KeyTextBox_TextChanged. This setter used to repeat that work afterwards, and its
                // ClearKeyError wiped the rejection the assignment had just raised - so a refused key
                // ended up looking accepted. One path only: assign, and let the handler decide.
                if (!string.Equals(_keyTextBox.Text, key, StringComparison.Ordinal))
                {
                    _keyTextBox.Text = key;
                    return;
                }

                // The text is already what was asked for, so no TextChanged will fire.
                ApplyKey(key);
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
        [DefaultValue(false)]
        [Description("Show a coloured key-badge pill next to the display value. Off by default: the key box already shows the key.")]
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
                PlaceholderText = "Key"   // the key column is a short code; "Enter key..." truncated to "Enter k..."
            };
            _keyTextBox.TextChanged += KeyTextBox_TextChanged;

            Controls.Add(_keyTextBox);
            _keyTextBox.Dock = DockStyle.None;

            // Use BaseControlâ€™s built-in trailing icon as the dropdown toggle â€” no separate BeepButton child needed
            TrailingIconPath      = SvgsUI.ChevronDown;
            TrailingIconClickable = true;
            TrailingIconClicked  += (_, __) => OpenPopup();

            // Forward mouse events for proper hover/focus behaviour
            _keyTextBox.MouseEnter += (s, e) => OnMouseEnter(e);
            _keyTextBox.MouseHover += (s, e) => OnMouseHover(e);
            _keyTextBox.MouseLeave += (s, e) => OnMouseLeave(e);

            // A LOV is a data-entry field and has to look like one. ShowAllBorders defaults to false
            // on BaseControl, so the control rendered as floating text with a chevron beside it - no
            // frame, nothing to say where the field began or ended.
            ShowAllBorders = true;
            BorderThickness = 1;

            _lastValidKey = null;

            // Without this BaseControl has no idea which property carries the value, so the control
            // could not participate in data binding - on a control whose entire job is to supply a
            // foreign key to a bound field.
            BoundProperty = nameof(SelectedKey);

            AdjustLayout();
        }

        protected override void InitLayout()
        {
            base.InitLayout();

            // Size is NOT forced here. This assigned Width = 300 and Height = 30 unconditionally, so
            // every instance snapped back to 300x30 no matter what the designer or caller set - the
            // control could not be made narrow for a code field or wide for a description.
            // DefaultSize already supplies the starting size.
            AdjustLayout();
        }
        #endregion

        #region Layout and Drawing
        private void GetHeight()
        {
            padding      = BorderThickness;
            spacing      = 5;
            buttonHeight = _keyTextBox != null ? _keyTextBox.PreferredHeight : 24;
            Height       = Math.Max(Height, buttonHeight + (padding * 2));
        }

        private void AdjustLayout()
        {
            UpdateDrawingRect();
            GetHeight();
            PositionKeyBox();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustLayout();
            Invalidate();
        }

        /// <summary>Paints the read-only value area after BaseControl has drawn the field.</summary>
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            PaintValueArea(g);
        }

        // PaintValueArea, BuildRoundedPath, ContrastForeColor's caller, ScaleLogicalX/Y and the
        // DrawContent and Draw overrides are gone: the value area is composed from a BeepLabel and a
        // badge label in BeepListofValuesBox.Composition.cs. They measured text, built a rounded path
        // and picked a contrasting foreground by luminance - all of which a label does by existing -
        // and the displayed value was pixels rather than something the accessibility tree could read.
        //
        // The Draw(Graphics, Rectangle) override went with them. It is BaseControl's extension point
        // for rendering an UNPARENTED control into a rectangle, which a composed control cannot do:
        // its children are what render. Nothing in the solution rendered a LOV that way.

        #endregion

        #region Event Handlers
        // (TrailingIconClicked fires OpenPopup â€” wired in InitializeComponents)

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
            // No action required â€” popup already hidden
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
            _lovPopup.SearchLoader     = SearchLoader;
            _lovPopup.PageLoader       = PageLoader;
            _lovPopup.PageSize         = PageSize;
            _lovPopup.SearchDebounceMs = SearchDebounceMs;
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
                // Phase 12: Async path â€” show popup immediately with empty list + spinner,
                // then fill the grid once the loader completes.
                _lovPopup.ShowAt(new List<SimpleItem>(), origin, Width, preloadSearch: "", ownerHeight: Height);
                var loaded = await _lovPopup.LoadItemsAsync(ItemsLoader, preloadSearch);

                // Write the loader's results back. This was a comment describing what should happen
                // above code that did not do it, which is the whole of the headline defect: everything
                // the loader returned was invisible to validation and display lookup.
                Remember(loaded);
                _itemsLoadedOnce = true;
                UpdateDisplayValue();

                // A key accepted provisionally before the list existed is now decidable.
                if (_restrictToList && !string.IsNullOrEmpty(SelectedKey) && Resolve(SelectedKey) == null)
                    RejectKey(SelectedKey);
            }
            else
            {
                // Synchronous path: items already in _items list
                _lovPopup.ShowAt(_items, origin, Width, preloadSearch, ownerHeight: Height);
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

        // â”€â”€ Keyboard Navigation (Phase 8) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // F9 â€” Oracle Forms standard to open LOV
            if (keyData == Keys.F9)
            {
                string preload = _keyTextBox?.Focused == true ? _keyTextBox.Text : string.Empty;
                OpenPopup(preload);
                return true;
            }

            // Alt+Down â€” Windows combobox standard
            if (keyData == (Keys.Alt | Keys.Down))
            {
                OpenPopup();
                return true;
            }

            // Delete / Backspace â€” clear the current selection
            if ((keyData == Keys.Delete || keyData == Keys.Back)
                && !string.IsNullOrEmpty(SelectedKey)
                && !(_keyTextBox?.Focused == true))
            {
                ClearSelection();
                return true;
            }

            // Escape when popup is open â€” close it
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

            // A revert is not a user edit: it must not re-run validation and clear its own error.
            if (_reverting) return;

            ApplyKey(_keyTextBox.Text);
        }

        /// <summary>
        /// Accepts or refuses a key. The single place a key is decided on.
        /// </summary>
        /// <remarks>
        /// Typing and assigning <see cref="SelectedKey"/> both land here, so both behave identically —
        /// they used to differ, and the quieter programmatic path was the one a data-binding caller
        /// hit, making a refused bound value disappear with nothing to explain it.
        /// </remarks>
        private void ApplyKey(string key)
        {
            if (ValidateKey(key))
            {
                UpdateLastValidKey(key);
                UpdateDisplayValue();
                ClearKeyError();
                ResolveIfUnknown(key);
            }
            else if (!string.IsNullOrEmpty(key))
            {
                ShowNotification($"'{key}' is not in the list.", ToolTipType.Warning, 2000);
                RejectKey(key);
            }

            Invalidate();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Resolves a single key without fetching the whole list — Oracle Forms' validate-from-list.
        /// </summary>
        /// <remarks>
        /// A LOV over ten thousand rows must not load them all to check one foreign key. When this is
        /// set, a key the control has not seen is looked up through it; when it is not set, the control
        /// falls back to whatever <see cref="ItemsLoader"/> eventually returns.
        /// </remarks>
        [Browsable(false)]
        public Func<string, CancellationToken, Task<SimpleItem?>>? KeyResolver { get; set; }

        /// <summary>
        /// When set, the popup's search box queries the source instead of filtering loaded rows.
        /// </summary>
        /// <remarks>
        /// The three loaders answer three different questions, and a large LOV wants all of them:
        /// <see cref="ItemsLoader"/> for "what is in the list", this for "what matches what I typed",
        /// and <see cref="KeyResolver"/> for "what is this one key". Only the first existed, so every
        /// search filtered a list that had to be loaded in full first.
        /// </remarks>
        [Browsable(false)]
        public Func<string, CancellationToken, Task<List<SimpleItem>>>? SearchLoader { get; set; }

        /// <summary>
        /// When set, the popup fetches one window of rows at a time instead of a whole result set.
        /// </summary>
        /// <remarks>
        /// The fourth and last loader, and the only one that bounds the <i>query</i>: <c>MaxRows</c>
        /// bounds what the client binds, but a source is still free to fetch fifty thousand rows so the
        /// popup can discard 49,500. Takes precedence over <see cref="SearchLoader"/>.
        /// </remarks>
        [Browsable(false)]
        public Func<string, int, int, CancellationToken, Task<LovPage>>? PageLoader { get; set; }

        /// <summary>Rows per window when <see cref="PageLoader"/> is used.</summary>
        [Browsable(true)]
        [Category("LOV")]
        [DefaultValue(100)]
        [Description("Rows fetched per window when PageLoader is set.")]
        public int PageSize { get; set; } = 100;

        /// <summary>How long typing must pause before a server-side search is issued.</summary>
        [Browsable(true)]
        [Category("LOV")]
        [DefaultValue(250)]
        [Description("Milliseconds of typing pause before SearchLoader is queried.")]
        public int SearchDebounceMs { get; set; } = 250;

        private bool _itemsLoadedOnce;
        private string? _resolvingKey;

        /// <summary>Starts a background lookup for a key the control has not seen.</summary>
        private void ResolveIfUnknown(string key)
        {
            if (string.IsNullOrEmpty(key) || _known.ContainsKey(key)) return;
            if (!CanResolveLater) return;

            BeginResolve(key);
        }

        /// <summary>Looks a provisionally-accepted key up in the background.</summary>
        /// <remarks>
        /// Prefers <see cref="KeyResolver"/> - one row, one query. Falls back to running
        /// <see cref="ItemsLoader"/> once, so a caller who configured only the bulk loader still gets a
        /// display value instead of a bare key.
        /// </remarks>
        private async void BeginResolve(string key)
        {
            if (_resolvingKey == key) return;
            if (KeyResolver == null && ItemsLoader == null) return;
            _resolvingKey = key;

            try
            {
                SimpleItem? item;
                if (KeyResolver != null)
                {
                    item = await KeyResolver(key, CancellationToken.None).ConfigureAwait(true);
                }
                else
                {
                    var all = await ItemsLoader!(CancellationToken.None).ConfigureAwait(true);
                    Remember(all);
                    _itemsLoadedOnce = true;
                    item = Resolve(key);
                }

                if (IsDisposed) return;

                if (item != null)
                {
                    Remember(new[] { item });
                    UpdateDisplayValue();
                    ClearKeyError();
                }
                else if (_restrictToList && SelectedKey == key)
                {
                    // The resolver is authoritative: it looked and the key is not there.
                    RejectKey(key);
                }
            }
            catch (Exception ex)
            {
                // Reported, not swallowed - but the key stays as the user left it. Failing to reach the
                // lookup is not evidence the key is wrong, and clearing their input on a network blip
                // would be worse than showing an unresolved key.
                BeepLog.Failure(this, $"resolve LOV key '{key}'", ex);
            }
            finally
            {
                if (_resolvingKey == key) _resolvingKey = null;
            }
        }

        /// <summary>Rejects a key: reverts to the last valid one and says why, on every path.</summary>
        private void RejectKey(string key)
        {
            ErrorText = $"'{key}' is not in the list.";
            HasError  = true;

            if (_keyTextBox != null)
            {
                _reverting = true;
                try { _keyTextBox.Text = _lastValidKey?.ToString() ?? string.Empty; }
                finally { _reverting = false; }
            }

            UpdateDisplayValue();
            KeyRejected?.Invoke(this, key);
            Invalidate();
        }

        /// <summary>Clears a previous validation error.</summary>
        private void ClearKeyError()
        {
            if (!HasError) return;

            ErrorText = string.Empty;
            HasError  = false;
        }

        /// <summary>
        /// Raised when a key was refused because it is not in the list.
        /// </summary>
        /// <remarks>
        /// A caller binding to a data source needs to know its value was refused. Without this the
        /// rejection is only visible as an inline error on screen.
        /// </remarks>
        [Category("LOV")]
        [Description("Raised when a key is refused because it is not in the list.")]
        public event EventHandler<string>? KeyRejected;

        /// <summary>
        /// Whether the field only accepts keys present in the list.
        /// </summary>
        /// <remarks>
        /// Oracle Forms distinguishes a validated LOV from a non-validated one. Validation was
        /// unconditional here, so a LOV used as a suggestion list - where free text is the point - was
        /// impossible to build.
        /// </remarks>
        [Browsable(true)]
        [Category("LOV")]
        [DefaultValue(true)]
        [Description("When true, only keys present in the list are accepted. When false, free text is allowed.")]
        public bool RestrictToList
        {
            get => _restrictToList;
            set
            {
                if (_restrictToList == value) return;
                _restrictToList = value;

                // Relaxing the rule must clear an error the old rule raised.
                if (!_restrictToList) ClearKeyError();
                Invalidate();
            }
        }

        /// <summary>
        /// Fields this LOV returns into other controls when a row is chosen.
        /// </summary>
        /// <remarks>
        /// Oracle Forms' return items: picking a department fills its number here and its location in
        /// another field. The control could only ever fill itself.
        /// </remarks>
        [Browsable(true)]
        [Category("LOV")]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Fields returned into other controls when a row is chosen.")]
        public List<LovReturnMapping> ReturnMappings { get; } = new List<LovReturnMapping>();

        /// <summary>Raised with the chosen item, so a caller gets the whole row rather than the key.</summary>
        /// <remarks>
        /// <c>SelectionChanged</c> carries <c>EventArgs.Empty</c>, so a caller had to reach back for
        /// <c>SelectedItem</c> to learn what happened.
        /// </remarks>
        [Category("LOV")]
        [Description("Raised with the chosen item when a selection is made.")]
        public event EventHandler<SimpleItem>? ItemSelected;

        /// <summary>Pushes the chosen row's fields into whatever <see cref="ReturnMappings"/> names.</summary>
        private void ApplyReturnMappings(SimpleItem item)
        {
            if (item == null || ReturnMappings.Count == 0) return;

            foreach (var map in ReturnMappings)
            {
                if (map == null || !map.IsUsable) continue;

                try
                {
                    object? value = map.Read(item);

                    if (map.Assign != null) { map.Assign(value); continue; }
                    if (map.Target == null) continue;

                    if (map.Target is BaseControl beep) beep.SetValue(value!);
                    else map.Target.Text = value?.ToString() ?? string.Empty;
                }
                catch (Exception ex)
                {
                    // One bad mapping must not stop the others, and must not stop the selection the
                    // user just made from being applied.
                    BeepLog.Failure(this, $"return LOV field '{map.Field}'", ex);
                }
            }
        }

        /// <summary>Records items so a key from any source can later be resolved and validated.</summary>
        private void Remember(IEnumerable<SimpleItem>? items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                string? key = item?.Value?.ToString();
                if (!string.IsNullOrEmpty(key)) _known[key!] = item!;
            }
        }

        /// <summary>The item behind a key, from any source this control has seen.</summary>
        private SimpleItem? Resolve(string? key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            return _known.TryGetValue(key!, out var item) ? item : null;
        }

        /// <summary>
        /// Whether a key may be held.
        /// </summary>
        /// <remarks>
        /// An empty key is always allowed - that is how a selection is cleared. When
        /// <see cref="RestrictToList"/> is false the control accepts anything, which is the free-entry
        /// LOV Oracle Forms calls a non-validated list.
        /// </remarks>
        private bool ValidateKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return true;
            if (!_restrictToList) return true;
            if (_known.ContainsKey(key)) return true;

            // The list is fetched by a query that has not run yet - the ordinary case when a bound form
            // loads a record before the user has ever opened the LOV. Rejecting here would throw away
            // valid data because the control had not looked it up, so the key is accepted provisionally
            // and resolved afterwards by the caller.
            //
            // This method stays free of side effects on purpose. Kicking the lookup off from here meant
            // that a resolver returning an already-completed task ran its continuation inline, BEFORE
            // the caller had assigned the text box - so the "is this still the current key" guard
            // compared against the previous value and never rejected.
            return CanResolveLater;
        }

        /// <summary>Whether an unknown key might still turn out to be valid.</summary>
        private bool CanResolveLater =>
            KeyResolver != null || (ItemsLoader != null && !_itemsLoadedOnce);

        private void UpdateLastValidKey(string key)
        {
            _lastValidKey = Resolve(key)?.Value ?? (object?)(_restrictToList ? null : key);
        }

        private void UpdateDisplayValue()
        {
            _selectedDisplayValue = Resolve(SelectedKey)?.Text ?? string.Empty;
            Invalidate();
        }

        private void SetSelectedItem(SimpleItem item)
        {
            if (item == null) return;

            // Remember it BEFORE assigning the text box. Assigning Text re-enters
            // KeyTextBox_TextChanged synchronously, which validates - so an item that arrived from a
            // loader rather than from ListItems used to fail validation and revert the selection the
            // user had just made in the popup.
            Remember(new[] { item });

            if (_keyTextBox != null)
            {
                _keyTextBox.Text = item.Value?.ToString() ?? string.Empty;
            }
            _lastValidKey = item.Value;
            UpdateDisplayValue();

            ApplyReturnMappings(item);
            ItemSelected?.Invoke(this, item);
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

        // â”€â”€ LOV Popup Configuration â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

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
        // â”€â”€ Phase 12: Async item loader â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
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

        // â”€â”€ Phase 13: recent selections â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        /// <summary>
        /// The most-recent selections made through this controlâ€™s LOV popup.
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
        }        // â”€â”€ Label / helper text convenience overrides â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
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

