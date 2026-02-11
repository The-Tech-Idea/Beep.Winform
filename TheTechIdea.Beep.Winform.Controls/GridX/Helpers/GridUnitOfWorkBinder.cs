using System;
using System.ComponentModel;
using System.Collections.Specialized;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Editor.UOW;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Bridges UnitOfWork instances to BeepGridPro without runtime reflection.
    /// - Reads Units from known UOW contracts
    /// - Binds it to the grid data and navigator
    /// - Subscribes to list change notifications to keep the grid refreshed
    /// </summary>
    internal class GridUnitOfWorkBinder
    {
        private readonly BeepGridPro _grid;
        private IUnitofWork? _uow;
        private IUnitOfWorkWrapper? _uowWrapper;
        private object? _units;

        private IBindingList? _asBindingList;
        private INotifyCollectionChanged? _asNotifyCollectionChanged;
        private bool _isRefreshingBinding;

        public GridUnitOfWorkBinder(BeepGridPro grid) { _grid = grid; }

        public void Attach(IUnitofWork? unitOfWork, IUnitOfWorkWrapper? unitOfWorkWrapper = null)
        {
            Detach();
            _uow = unitOfWork;
            _uowWrapper = unitOfWorkWrapper;
            if (_uow == null && _uowWrapper == null) return;
            RefreshBinding();
            SubscribeUowEvents();
        }

        public void Detach()
        {
            DetachUnitChangeListeners();

            UnsubscribeUowEvents();

            _units = null;
            _uow = null;
            _uowWrapper = null;
        }

        private void RefreshBinding()
        {
            if (_isRefreshingBinding) return;
            _isRefreshingBinding = true;

            try
            {
                DetachUnitChangeListeners();

                int selectedRow = _grid.Selection.RowIndex;
                int selectedCol = _grid.Selection.ColumnIndex;

                _units = GetUnits();
                if (_units == null) return;

                // Feed grid data and navigator
                _grid.Data.Bind(_units);
                _grid.Navigator.BindTo(_units);
                _grid.Data.InitializeData();

                // Hook change notifications to keep the view live
                _asBindingList = _units as IBindingList;
                if (_asBindingList != null)
                {
                    _asBindingList.ListChanged += OnListChanged;
                }
                _asNotifyCollectionChanged = _units as INotifyCollectionChanged;
                if (_asNotifyCollectionChanged != null)
                {
                    _asNotifyCollectionChanged.CollectionChanged += OnCollectionChanged;
                }

                _grid.Layout.Recalculate();

                if (_grid.Rows.Count > 0 && selectedRow >= 0)
                {
                    int rowIndex = Math.Max(0, Math.Min(selectedRow, _grid.Rows.Count - 1));
                    int colIndex = Math.Max(0, Math.Min(selectedCol, Math.Max(0, _grid.Columns.Count - 1)));
                    _grid.SelectCell(rowIndex, colIndex);
                }

                _grid.SafeInvalidate();
            }
            finally
            {
                _isRefreshingBinding = false;
            }
        }

        private void DetachUnitChangeListeners()
        {
            if (_asBindingList != null)
            {
                _asBindingList.ListChanged -= OnListChanged;
                _asBindingList = null;
            }
            if (_asNotifyCollectionChanged != null)
            {
                _asNotifyCollectionChanged.CollectionChanged -= OnCollectionChanged;
                _asNotifyCollectionChanged = null;
            }
        }

        private void OnListChanged(object sender, ListChangedEventArgs e)
        {
            if (_isRefreshingBinding) return;

            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                bool canFastRefresh =
                    e.NewIndex >= 0 &&
                    e.NewIndex < _grid.Rows.Count &&
                    _grid.Rows[e.NewIndex].RowData is INotifyPropertyChanged;

                if (canFastRefresh)
                {
                    _grid.InvalidateRow(e.NewIndex);
                }
                else
                {
                    RefreshBinding();
                }
                return;
            }

            RefreshBinding();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isRefreshingBinding) return;
            RefreshBinding();
        }

        private object? GetUnits()
        {
            if (_uow != null) return _uow.Units;
            if (_uowWrapper != null) return _uowWrapper.Units;
            return null;
        }

        private void SubscribeUowEvents()
        {
            if (_uow != null)
            {
                _uow.PreDelete += HandleUowPreChange;
                _uow.PreInsert += HandleUowPreChange;
                _uow.PreCreate += HandleUowPreChange;
                _uow.PreUpdate += HandleUowPreChange;
                _uow.PreQuery += HandleUowPreChange;
                _uow.PreCommit += HandleUowPreChange;

                _uow.PostQuery += HandleUowPostQuery;
                _uow.PostInsert += HandleUowPostChange;
                _uow.PostCreate += HandleUowPostChange;
                _uow.PostUpdate += HandleUowPostChange;
                _uow.PostEdit += HandleUowPostChange;
                _uow.PostDelete += HandleUowPostChange;
                _uow.PostCommit += HandleUowPostCommit;
            }

            if (_uowWrapper != null && _uow == null)
            {
                _grid.Navigator.WrapperEventForwarded += HandleWrapperForwardedEvent;
            }
        }

        private void UnsubscribeUowEvents()
        {
            if (_uow != null)
            {
                _uow.PreDelete -= HandleUowPreChange;
                _uow.PreInsert -= HandleUowPreChange;
                _uow.PreCreate -= HandleUowPreChange;
                _uow.PreUpdate -= HandleUowPreChange;
                _uow.PreQuery -= HandleUowPreChange;
                _uow.PreCommit -= HandleUowPreChange;

                _uow.PostQuery -= HandleUowPostQuery;
                _uow.PostInsert -= HandleUowPostChange;
                _uow.PostCreate -= HandleUowPostChange;
                _uow.PostUpdate -= HandleUowPostChange;
                _uow.PostEdit -= HandleUowPostChange;
                _uow.PostDelete -= HandleUowPostChange;
                _uow.PostCommit -= HandleUowPostCommit;
            }

            _grid.Navigator.WrapperEventForwarded -= HandleWrapperForwardedEvent;
        }

        private static void HandleUowPreChange(object sender, UnitofWorkParams e)
        {
            // Reserved for future busy/validation UI hints.
        }

        private void HandleUowPostQuery(object sender, UnitofWorkParams e)
        {
            // Units might be replaced by query operations.
            RefreshBinding();
        }

        private void HandleUowPostChange(object sender, UnitofWorkParams e)
        {
            if (e.EventAction == EventAction.PostUpdate || e.EventAction == EventAction.PostEdit)
            {
                _grid.SafeInvalidate();
                return;
            }

            RefreshBinding();
        }

        private void HandleUowPostCommit(object sender, UnitofWorkParams e)
        {
            _grid.SafeInvalidate();
        }

        private void HandleWrapperForwardedEvent(object? sender, UnitofWorkParams e)
        {
            switch (e.EventAction)
            {
                case EventAction.PostQuery:
                    RefreshBinding();
                    break;
                case EventAction.PostInsert:
                case EventAction.PostCreate:
                case EventAction.PostDelete:
                    RefreshBinding();
                    break;
                case EventAction.PostUpdate:
                case EventAction.PostEdit:
                    _grid.SafeInvalidate();
                    break;
                case EventAction.PostCommit:
                case EventAction.PostRollback:
                    _grid.SafeInvalidate();
                    break;
            }
        }
    }
}
