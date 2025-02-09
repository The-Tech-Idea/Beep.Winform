using TheTechIdea.Beep.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheTechIdea.Beep.DataBase;

namespace TheTechIdea.Beep.Winform.Controls.DataNavigator
{
    public enum BeepBindingSourceMode
    {
        ObservableBinidingList,
        UnitofWok
    }
    public class BeepBindingSource:BindingSource
    {
        #region Properties
        private BeepBindingSourceMode BindingSourceMode { get; set; } = BeepBindingSourceMode.ObservableBinidingList;
        public List<IUnitofWork> ChildUnitofWorks { get; set; } = new List<IUnitofWork>();
        public List<ChildRelation> Childs { get; set; } = new List<ChildRelation>();
        public IUnitofWork UnitofWork
        {
            get => _unitofWork;
            set => _unitofWork = value;
        }
        
        
        public int Count => _observableList?.Count ?? 0;
        public bool IsBindingSuspended { get; private set; }
        private ObservableBindingList<Entity> _observableList;
        private IUnitofWork _unitofWork;
        #endregion Properties
        #region Events
        public event EventHandler CurrentChanged;
        public event EventHandler<ListChangedEventArgs> ListChanged;
        //public event AddingNewEventHandler AddingNew;
        //public event EventHandler<EventArgs> DataSourceChanged;

        #endregion Events
        #region Constructors
        public BeepBindingSource()
        {
            _observableList = new ObservableBindingList<Entity>();
            ChildUnitofWorks = new List<IUnitofWork>();
            Childs = new List<ChildRelation>();
            AttachBaseBindingSourceEvents();
        }
        public BeepBindingSource(IUnitofWork work) : this()
        {
            _unitofWork = work;
            if(_unitofWork != null)
            {
                DataSource = _unitofWork.Units;
                BindingSourceMode = BeepBindingSourceMode.UnitofWok;
            }
        }

        public BeepBindingSource(ObservableBindingList<Entity> list) : this()
        {
          
            DataSource = list;
            BindingSourceMode = BeepBindingSourceMode.ObservableBinidingList;
        }
        #endregion Constructors
        #region "Base Binding Source Events"
        private void BeepBindingSource_CurrentItemChanged(object? sender, EventArgs e)
        {

        }

        private void BeepBindingSource_DataError(object? sender, BindingManagerDataErrorEventArgs e)
        {
     
        }

        private void BeepBindingSource_BindingComplete(object? sender, BindingCompleteEventArgs e)
        {

        }

        private void BeepBindingSource_PositionChanged(object? sender, EventArgs e)
        {
           
        }

        private void BeepBindingSource_AddingNew(object? sender, AddingNewEventArgs e)
        {
       
        }

        private void BeepBindingSource_ListChanged(object? sender, ListChangedEventArgs e)
        {
            
        }

        private void BeepBindingSource_CurrentChanged(object? sender, EventArgs e)
        {
             

        }

        private void BeepBindingSource_DataSourceChanged(object? sender, EventArgs e)
        {
            if (BindingSourceMode == BeepBindingSourceMode.ObservableBinidingList)
            {
            
            }
        }
        #endregion "Base Binding Source Events"
        #region "CRUD Methods"
      
        #endregion "CRUD Methods"
        #region "Filtering and Sorting"
  

        #endregion "Filtering and Sorting"
        #region "Binding Source Events"
     
        #endregion "Binding Source Events"
        #region "Attachment and Detachment"
        private void AttachBaseBindingSourceEvents()
        {
            this.DataSourceChanged += BeepBindingSource_DataSourceChanged;
            this.CurrentChanged += BeepBindingSource_CurrentChanged; ;
            this.ListChanged += BeepBindingSource_ListChanged;
            this.AddingNew += BeepBindingSource_AddingNew;
            this.PositionChanged += BeepBindingSource_PositionChanged;
            this.BindingComplete += BeepBindingSource_BindingComplete;
            this.DataError += BeepBindingSource_DataError;
            this.CurrentItemChanged += BeepBindingSource_CurrentItemChanged;
        }
        private void DetachBaseBindingSourceEvents()
        {
            this.DataSourceChanged -= BeepBindingSource_DataSourceChanged;
            this.CurrentChanged -= BeepBindingSource_CurrentChanged; ;
            this.ListChanged -= BeepBindingSource_ListChanged;
            this.AddingNew -= BeepBindingSource_AddingNew;
            this.PositionChanged -= BeepBindingSource_PositionChanged;
            this.BindingComplete -= BeepBindingSource_BindingComplete;
            this.DataError -= BeepBindingSource_DataError;
            this.CurrentItemChanged -= BeepBindingSource_CurrentItemChanged;
        }

     
        #endregion "Attachment and Detachment"
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
       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DetachBaseBindingSourceEvents();
                _observableList = null;
                _unitofWork = null;
            }
            base.Dispose(disposing);
        }
    }
}
