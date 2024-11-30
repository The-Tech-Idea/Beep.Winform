using TheTechIdea.Beep.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheTechIdea.Beep.DataBase;

namespace TheTechIdea.Beep.Winform.Controls.DataNavigator
{
    public class BeepBindingSource
    {
        private ObservableBindingList<Entity> _observableList;
        private IUnitofWork _unitofWork;

        public event EventHandler<BeepEventDataArgs> CurrentChanged;
        public event EventHandler<ListChangedEventArgs> ListChanged;
        public event AddingNewEventHandler AddingNew;
        public event EventHandler<EventArgs> DataSourceChanged;

        public BeepBindingSource()
        {
            _observableList = new ObservableBindingList<Entity>();
            ChildUnitofWorks = new List<IUnitofWork>();
            Childs = new List<ChildRelation>();
            AttachHandlers(_observableList);
        }

        public BeepBindingSource(IUnitofWork work) : this()
        {
            _unitofWork = work;
        }

        public BeepBindingSource(ObservableBindingList<Entity> list) : this()
        {
            DataSource = list;
        }

        public IUnitofWork UnitofWork
        {
            get => _unitofWork;
            set => _unitofWork = value;
        }

        public List<IUnitofWork> ChildUnitofWorks { get; set; } = new List<IUnitofWork>();
        public List<ChildRelation> Childs { get; set; } = new List<ChildRelation>();

        public object DataSource
        {
            get => _observableList;
            set
            {
                if (_observableList != value)
                {
                    DetachHandlers(_observableList);
                    _observableList = value as ObservableBindingList<Entity> ?? new ObservableBindingList<Entity>();
                    AttachHandlers(_observableList);
                    OnCurrentChanged();
                    DataSourceChanged?.Invoke(this, EventArgs.Empty);

                    if (!IsBindingSuspended)
                        OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
                }
            }
        }

        public object Current => _observableList?.Current;
        public int Position
        {
            get => _observableList?.CurrentIndex ?? -1;
            set => _observableList?.MoveTo(value);
        }
        public int Count => _observableList?.Count ?? 0;
        public bool IsBindingSuspended { get; private set; }

        public void MoveNext() => _observableList?.MoveNext();
        public void MovePrevious() => _observableList?.MovePrevious();
        public void MoveFirst() => _observableList?.MoveFirst();
        public void MoveLast() => _observableList?.MoveLast();

        public void AddNew()
        {
            var e = new AddingNewEventArgs(null);
            OnAddingNew(e);
            if (e.NewObject is Entity newEntity)
                _observableList?.Add(newEntity);
            else
                _observableList?.AddNew();
        }

        public void RemoveCurrent()
        {
            if (Position >= 0 && Position < (_observableList?.Count ?? 0))
                _observableList?.RemoveAt(Position);
        }

        public void ResetBindings(bool metadataChanged)
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        public void SuspendBinding() => IsBindingSuspended = true;
        public void ResumeBinding()
        {
            IsBindingSuspended = false;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        public void EndEdit() => _unitofWork?.Commit();
        public void CancelEdit() => _unitofWork?.Rollback();

        public void RemoveFilter() => _observableList?.RemoveFilter();
        public string Filter
        {
            get => _observableList?.Filter;
            set
            {
                if (_observableList != null) _observableList.Filter = value;
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        public void ApplySort(string propertyName, ListSortDirection direction)
        {
            _observableList?.Sort(propertyName);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemMoved, -1));
        }

        public void RemoveSort()
        {
            _observableList?.RemoveSort();
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private void AttachHandlers(ObservableBindingList<Entity> list)
        {
            if (list != null)
            {
                list.CurrentChanged += ObservableList_CurrentChanged;
                list.ListChanged += ObservableList_ListChanged;
            }
        }

        private void DetachHandlers(ObservableBindingList<Entity> list)
        {
            if (list != null)
            {
                list.CurrentChanged -= ObservableList_CurrentChanged;
                list.ListChanged -= ObservableList_ListChanged;
            }
        }

        private void ObservableList_CurrentChanged(object sender, EventArgs e) => OnCurrentChanged();

        private void ObservableList_ListChanged(object sender, ListChangedEventArgs e) => OnListChanged(e);

        protected virtual void OnCurrentChanged()
        {
            var beepEventDataArgs = new BeepEventDataArgs("CurrentChanged", null);
            CurrentChanged?.Invoke(this, beepEventDataArgs);
        }

        protected virtual void OnListChanged(ListChangedEventArgs e) => ListChanged?.Invoke(this, e);

        protected virtual void OnAddingNew(AddingNewEventArgs e) => AddingNew?.Invoke(this, e);

        #region "Child Relations"

        //public void LoadChildrenForCurrentParent()
        //{
        //    if (Current is not Entity currentEntity) return;

        //    foreach (var relation in Childs)
        //    {
        //        var childEntities = LoadChildEntities(relation, currentEntity);
        //        relation.ChildBindingSource = new ObservableBindingList<Entity>(childEntities);
        //    }
        //}

        //private List<Entity> LoadChildEntities(ChildRelation relation, Entity parentEntity)
        //{
        //    var filter = $"{relation.child_column} = '{parentEntity.GetValue(relation.parent_column)}'";
        //    return _unitofWork?.Get(relation.child_table, filter) ?? new List<Entity>();
        //}

        #endregion
    }
}
