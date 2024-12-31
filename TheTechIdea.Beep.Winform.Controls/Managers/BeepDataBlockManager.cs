using System.ComponentModel;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Converters;

namespace TheTechIdea.Beep.Winform.Controls.Managers
{
   
    public class DataBlockManager :Component,IDisposable
    {
        protected IUnitofWork _data;
        public bool IsInQueryMode { get; private set; } = false;

        public IEntityStructure Structure => Data != null ? Data.EntityStructure : null;

        public List<IBeepUIComponent> UIComponents { get; set; } = new List<IBeepUIComponent>();

        [Browsable(true)]
        [Category("Data")]
        [TypeConverter(typeof(UnitOfWorkConverter))]
        public IUnitofWork Data
        {
            get { return _data; }
            set
            {
                if (_data != value)
                {
                    if (_data != null)
                    {
                        _data.Units.CurrentChanged -= Units_CurrentChanged;
                    }

                    _data = value;
                    if (_data != null)
                    {
                        _data.Units.CurrentChanged += Units_CurrentChanged;
                    }
                    //Refresh();
                }
            }
        }
        private dynamic _masterRecord;
        private string _foreignKeyPropertyName;
        private string _masterKeyPropertyName;
        private List<DataBlockManager> _childBlocks = new List<DataBlockManager>();
        [Browsable(true)]
        [Category("Data Blocks")]
        public List<DataBlockManager> ChildBlocks => _childBlocks;
        private DataBlockManager _parentBlock;
        private bool disposedValue;

        [TypeConverter(typeof(DataBlockConverter))]
        public DataBlockManager ParentBlock
        {
            get => _parentBlock;
            set
            {
                if (_parentBlock == value) return;

                if (_parentBlock != null)
                {
                    // Remove this block from the parent's children
                    _parentBlock.ChildBlocks.Remove(this);

                    // Unsubscribe from the parent's events
                    if (_parentBlock.Data?.Units != null)
                    {
                        _parentBlock.Data.Units.CurrentChanged -= Parent_CurrentChanged;
                    }
                }

                _parentBlock = value;

                if (_parentBlock != null)
                {
                    // Add this block to the new parent's children
                    _parentBlock.ChildBlocks.Add(this);

                    // Subscribe to the parent's events
                    if (_parentBlock.Data?.Units != null)
                    {
                        _parentBlock.Data.Units.CurrentChanged += Parent_CurrentChanged;
                    }
                }

                // Refresh the current block to reflect the change
                //Refresh();
            }
        }

        public string ParentKeyPropertyName
        {
            get => _masterKeyPropertyName;
            set => _masterKeyPropertyName = value;
        }
       
        public string ForeignKeyPropertyName
        {
            get => _foreignKeyPropertyName;
            set => _foreignKeyPropertyName = value;
        }
        public DataBlockManager()
        {
         
        }
        public void RemoveParentBlock()
        {
            if (_parentBlock != null)
            {
                // Unsubscribe from ParentBlock's CurrentChanged event
                if (_parentBlock.Data?.Units != null)
                {
                    _parentBlock.Data.Units.CurrentChanged -= Parent_CurrentChanged;
                }

                // Remove this block from the ParentBlock's child list
                _parentBlock.ChildBlocks.Remove(this);

                // Clear the parent reference
                _parentBlock = null;

                // Reset the master record and refresh
                _masterRecord = null;
                _masterKeyPropertyName = null;
                _foreignKeyPropertyName = null;
                //Refresh();
            }
        }
        public void AddChildBlock(DataBlockManager childBlock, string masterKeyPropertyName, string foreignKeyPropertyName)
        {
            if (childBlock.ParentBlock != null)
            {
                // Remove the child from its current parent
                childBlock.RemoveParentBlock();
            }

            // Add this block as the parent
            _childBlocks.Add(childBlock);
            childBlock.ParentBlock = this;
            childBlock._masterKeyPropertyName = masterKeyPropertyName;
            childBlock._foreignKeyPropertyName = foreignKeyPropertyName;

            // Set the master record for the child block
            childBlock.SetMasterRecord(Data?.Units.Current, masterKeyPropertyName, foreignKeyPropertyName);
        }
        public void SetMasterRecord(dynamic masterRecord, string masterKeyPropertyName, string foreignKeyPropertyName)
        {
            _masterRecord = masterRecord;
            _masterKeyPropertyName = masterKeyPropertyName;
            _foreignKeyPropertyName = foreignKeyPropertyName;

            ApplyMasterDetailFilter();
            //Refresh();
        }
        private void Parent_CurrentChanged(object sender, EventArgs e)
        {
            if (_parentBlock?.Data?.Units?.Current != null)
            {
                UpdateDataFromParent();
                NotifyChildBlocksOfMasterChange();
            }
            else
            {
                // If ParentBlock or its data is removed, reset this block
                _masterRecord = null;
                //Refresh();
            }
        }
        private void addthisblockaschild(DataBlockManager parentBlock)
        {
            if (parentBlock == null)
            {
                throw new ArgumentNullException(nameof(parentBlock), "ParentBlock cannot be null.");
            }

            parentBlock.AddChildBlock(this, parentBlock.ParentKeyPropertyName, this._foreignKeyPropertyName);
        }
        private void UpdateDataFromParent()
        {
            if (ParentBlock != null && !string.IsNullOrEmpty(ParentKeyPropertyName) && !string.IsNullOrEmpty(ForeignKeyPropertyName))
            {
                var masterRecord = ParentBlock.Data?.Units.Current;
                SetMasterRecord(masterRecord, ParentKeyPropertyName, ForeignKeyPropertyName);
            }
        }
        private void ValidateParentBlock(DataBlockManager parentBlock)
        {
            if (parentBlock != null && parentBlock == this)
            {
                throw new InvalidOperationException("A block cannot be its own parent.");
            }
        }
        private void OnParentBlockChanged()
        {
            //Refresh();
        }
        private void NotifyChildBlocksOfMasterChange()
        {
            var masterRecord = Data?.Units.Current;
            foreach (var childBlock in _childBlocks)
            {
                childBlock.SetMasterRecord(masterRecord, _masterKeyPropertyName, _foreignKeyPropertyName);
            }
        }
        private void ApplyMasterDetailFilter()
        {
            if (_masterRecord == null || string.IsNullOrEmpty(_foreignKeyPropertyName) || string.IsNullOrEmpty(_masterKeyPropertyName))
            {
                Data.Units.RemoveFilter();
            }
            else
            {
                var masterKeyValue = GetPropertyValue(_masterRecord, _masterKeyPropertyName);

                Func<dynamic, bool> filterPredicate = entity =>
                {
                    var value = GetPropertyValue(entity, _foreignKeyPropertyName);
                    return value != null && value.Equals(masterKeyValue);
                };

                Data.Units.ApplyFilter(filterPredicate);
            }

            Data.Units.MoveFirst();
        }
        private object GetPropertyValue(dynamic entity, string propertyName)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var property = entity.GetType().GetProperty(propertyName);
            if (property == null)
                throw new InvalidOperationException($"Property '{propertyName}' not found on type '{entity.GetType().Name}'.");

            return property.GetValue(entity);
        }
          public void RemoveChildBlock(DataBlockManager childBlock)
        {
            if (_childBlocks.Contains(childBlock))
            {
                // Remove the child block
                _childBlocks.Remove(childBlock);

                // Clear the child's parent reference
                childBlock.RemoveParentBlock();
                // Clear the child's parent reference
                childBlock.ParentBlock = null;
            }
        }
        private void Units_CurrentChanged(object sender, EventArgs e)
        {
            NotifyChildBlocksOfMasterChange();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Notify the parent to remove this block
                if (_parentBlock != null)
                {
                    _parentBlock.ChildBlocks.Remove(this);
                    _parentBlock = null;
                }

                // Notify all child blocks to remove their reference to this block
                foreach (var childBlock in ChildBlocks.ToList())
                {
                    childBlock.ParentBlock = null; // This will also remove the child from this block's list
                }

                // Clear child blocks
                ChildBlocks.Clear();

                // Unsubscribe from events
                if (Data?.Units != null)
                {
                    Data.Units.CurrentChanged -= Units_CurrentChanged;
                }

            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DataBlockManager()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
