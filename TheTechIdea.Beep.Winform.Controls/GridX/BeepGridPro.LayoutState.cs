using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.GridX.LayoutState;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Per-user layout persistence for <see cref="BeepGridPro"/>. The host
    /// supplies the key; the grid never guesses a user identity.
    /// </summary>
    public partial class BeepGridPro
    {
        private IGridLayoutStateStore? _layoutStateStore;
        private GridLayoutState? _layoutState;
        private string _layoutStateKey = string.Empty;
        private bool _layoutStateDirty;
        private bool _isApplyingLayoutState;

        /// <summary>
        /// Store used by <see cref="SaveLayoutState"/> and
        /// <see cref="LoadLayoutState"/>. Defaults to a JSON file store under
        /// LocalApplicationData.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IGridLayoutStateStore LayoutStateStore
        {
            get => _layoutStateStore ??= new FileGridLayoutStateStore();
            set => _layoutStateStore = value
                ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Key used when no explicit key is passed to save/load. Convention:
        /// <c>{application}/{user}/{gridName}</c>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string LayoutStateKey
        {
            get => _layoutStateKey;
            set => _layoutStateKey = value ?? string.Empty;
        }

        /// <summary>True when a persisted column layout has changed since the last save.</summary>
        [Browsable(false)]
        public bool IsLayoutStateDirty => _layoutStateDirty;

        /// <summary>
        /// Captures the current persisted column properties (order, width,
        /// sticky, visibility) into a state object.
        /// </summary>
        public GridLayoutState CaptureLayoutState()
        {
            var state = new GridLayoutState
            {
                GridKey = _layoutStateKey,
                CapturedAtUtc = DateTimeOffset.UtcNow
            };

            foreach (var column in Data.Columns)
            {
                if (IsSystemColumn(column)) continue;

                state.Columns.Add(new GridColumnLayout
                {
                    ColumnName = column.ColumnName,
                    DisplayOrder = column.DisplayOrder,
                    Width = column.Width,
                    Sticked = column.Sticked,
                    Visible = column.Visible
                });
            }

            return state;
        }

        /// <summary>
        /// Saves the current column layout under <paramref name="key"/> (or
        /// <see cref="LayoutStateKey"/> when omitted). Returns false and
        /// reports the failure instead of throwing.
        /// </summary>
        public bool SaveLayoutState(string? key = null)
        {
            string effectiveKey = ResolveLayoutStateKey(key);
            if (string.IsNullOrWhiteSpace(effectiveKey))
            {
                ReportOperationError("LayoutState.Save",
                    new InvalidOperationException("LayoutStateKey is not set."));
                return false;
            }

            try
            {
                var state = CaptureLayoutState();
                state.GridKey = effectiveKey;

                if (!LayoutStateStore.Save(effectiveKey, state))
                    return false;

                _layoutState = state;
                _layoutStateKey = effectiveKey;
                _layoutStateDirty = false;
                return true;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                ReportOperationError("LayoutState.Save", ex);
                return false;
            }
        }

        /// <summary>
        /// Loads and applies a saved column layout. Unknown columns are
        /// ignored; new columns keep their generated values.
        /// </summary>
        public bool LoadLayoutState(string? key = null)
        {
            string effectiveKey = ResolveLayoutStateKey(key);
            if (string.IsNullOrWhiteSpace(effectiveKey))
            {
                ReportOperationError("LayoutState.Load",
                    new InvalidOperationException("LayoutStateKey is not set."));
                return false;
            }

            try
            {
                var state = LayoutStateStore.Load(effectiveKey);
                if (state == null) return false;

                _layoutState = state;
                _layoutStateKey = effectiveKey;
                _layoutStateDirty = false;

                ApplyLayoutState(state);
                return true;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                ReportOperationError("LayoutState.Load", ex);
                return false;
            }
        }

        /// <summary>
        /// Deletes the persisted layout and forgets the in-memory copy. The
        /// current columns are left as they are; call
        /// <see cref="AutoGenerateColumns"/> to rebuild defaults.
        /// </summary>
        public void ResetLayoutState(string? key = null)
        {
            string effectiveKey = ResolveLayoutStateKey(key);

            try
            {
                if (!string.IsNullOrWhiteSpace(effectiveKey))
                    LayoutStateStore.Delete(effectiveKey);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                ReportOperationError("LayoutState.Reset", ex);
            }

            _layoutState = null;
            _layoutStateDirty = false;
        }

        /// <summary>
        /// Applies a captured/saved layout by column name. Widths are clamped
        /// by <see cref="Models.BeepColumnConfig.MinWidth"/>/<c>MaxWidth</c>,
        /// and system columns are never touched.
        /// </summary>
        public void ApplyLayoutState(GridLayoutState state)
        {
            if (state == null || state.Columns.Count == 0) return;

            var byName = new Dictionary<string, GridColumnLayout>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in state.Columns)
            {
                if (string.IsNullOrWhiteSpace(entry.ColumnName)) continue;
                byName[entry.ColumnName] = entry;
            }

            if (byName.Count == 0) return;

            _isApplyingLayoutState = true;
            try
            {
                foreach (var column in Data.Columns)
                {
                    if (IsSystemColumn(column)) continue;
                    if (!byName.TryGetValue(column.ColumnName, out var layout)) continue;

                    column.DisplayOrder = layout.DisplayOrder;
                    column.Width = layout.Width;
                    column.Sticked = layout.Sticked;
                    column.Visible = layout.Visible;
                }
            }
            finally
            {
                _isApplyingLayoutState = false;
            }

            Layout.Recalculate();
            ScrollBars?.UpdateBars();
            SafeInvalidate();
        }

        internal bool IsApplyingLayoutState => _isApplyingLayoutState;

        internal void MarkLayoutStateDirty()
        {
            if (!_isApplyingLayoutState)
                _layoutStateDirty = true;
        }

        internal void ApplyPersistedLayoutState()
        {
            if (_layoutState != null)
                ApplyLayoutState(_layoutState);
        }

        private string ResolveLayoutStateKey(string? key)
            => string.IsNullOrWhiteSpace(key) ? _layoutStateKey : key!.Trim();

        private static bool IsSystemColumn(Models.BeepColumnConfig column)
            => column.IsSelectionCheckBox || column.IsRowNumColumn || column.IsRowID;
    }
}
