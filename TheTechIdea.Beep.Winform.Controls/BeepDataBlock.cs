using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Desktop.Controls.Common;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepDataBlock))]
    [Category("Beep Controls")]
    [DisplayName("Beep Data Block")]
    public class BeepDataBlock : BeepControl, IDisposable, IBeepDataBlock
    {
        public event EventHandler<UnitofWorkParams> EventDataChanged;
        public DataBlockMode BlockMode { get;  set; } = DataBlockMode.CRUD;
        public Dictionary<string, IBeepUIComponent> UIComponents { get; set; } = new();
        public List<IBeepDataBlock> ChildBlocks { get; set; } = new();
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Master-Detail")]
        [Description("Gets or sets the parent block for this block.")]
        [TypeConverter(typeof(DataBlockConverter))]
        [DisplayName("Parent Block")]
        public IBeepDataBlock ParentBlock { get; set; }
        public bool IsInQueryMode { get; private set; } = false;

        private IUnitofWork _data;
        private Dictionary<IBeepUIComponent, Binding[]> _preservedBindings = new();
        private bool _disposed;

        public IEntityStructure EntityStructure { get; private set; }
        public List<EntityField> Fields => EntityStructure?.Fields;


        public dynamic MasterRecord { get; private set; }

        [Browsable(true)]
        [Category("Master-Detail")]
        public string MasterKeyPropertyName { get; set; }

        [Browsable(true)]
        [Category("Master-Detail")]
        public string ForeignKeyPropertyName { get; set; }

        [Browsable(true)]
        [Category("Data")]
        public List<RelationShipKeys> Relationships { get; set; } = new();
        //[TypeConverter(typeof(UnitOfWorkClassesConverter))]
        //[Browsable(true)]
        //[Category("Data")]
        //[Description("Gets or sets the data context for data binding.")]
        //[DisplayName("Data Context / ViewModel")]
        //public new  object DataContext
        //{
        //    get => _dataContext;
        //    set
        //    {
        //        _dataContext = value;
        //        OnDataContextChanged();
        //    }
        //}
        private object _dataContext;

        [TypeConverter(typeof(UnitOfWorksConverter))]
        [Browsable(true)]
        [Category("Data")]
        public IUnitofWork Data
        {
            get => _data;
            set
            {
                if (_data != value)
                {
                    if (_data != null)
                    {
                        _data.Units.CurrentChanged -= Units_CurrentChanged;
                        _data.PreUpdate -= HandleDataChanges;
                        _data.PreInsert -= HandleDataChanges;
                        _data.PreDelete -= HandleDataChanges;
                        _data.PreQuery -= HandleDataChanges;
                        _data.PreCreate -= HandleDataChanges;
                        _data.PreCommit -= HandleDataChanges;
                        _data.PostUpdate -= HandleDataChanges;
                        _data.PostInsert -= HandleDataChanges;
                        _data.PostDelete -= HandleDataChanges;
                        _data.PostQuery -= HandleDataChanges;
                        _data.PostCreate -= HandleDataChanges;
                        _data.PostCommit -= HandleDataChanges;

                    }
                        
                        

                    _data = value;

                    if (_data != null)
                    {
                        _data.Units.CurrentChanged += Units_CurrentChanged;
                        _data.PreUpdate += HandleDataChanges;
                        _data.PreInsert += HandleDataChanges;
                        _data.PreDelete += HandleDataChanges;
                        _data.PreQuery += HandleDataChanges;
                        _data.PreCreate += HandleDataChanges;
                        _data.PreCommit += HandleDataChanges;
                        _data.PostUpdate += HandleDataChanges;
                        _data.PostInsert   += HandleDataChanges;
                        _data.PostDelete += HandleDataChanges;
                        _data.PostQuery += HandleDataChanges;
                        _data.PostCreate += HandleDataChanges;
                        _data.PostCommit += HandleDataChanges;
                    }
                      

                    EntityStructure = _data?.EntityStructure;
                    InitializeEntityRelationships();
                }
            }
        }

        private void HandleDataChanges(object? sender, UnitofWorkParams e)
        {
            if(BlockMode== DataBlockMode.CRUD)
            {
                EventDataChanged?.Invoke(sender, e);
            }
        }
        public BeepDataBlock()
        {
            IsShadowAffectedByTheme = false;
            IsRoundedAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
        }

        private void InitializeEntityRelationships()
        {
            if (EntityStructure != null)
            {
                Relationships.Clear();

                foreach (var relationship in EntityStructure.Relations)
                {
                    var keys = new RelationShipKeys
                    {
                        RelatedEntityID = relationship.RelatedEntityID,
                        RelatedEntityColumnID = relationship.RelatedEntityColumnID,
                        EntityColumnID = relationship.EntityColumnID
                    };
                    Relationships.Add(keys);
                }

                MasterKeyPropertyName = string.Join(", ", Relationships.Select(r => r.RelatedEntityColumnID));
                ForeignKeyPropertyName = string.Join(", ", Relationships.Select(r => r.EntityColumnID));
            }
        }
        protected virtual void OnDataContextChanged()
        {
            Data = null;

            // If DataContext is set, let UnitOfWorksConverter populate the dropdown for the Data property
            var unitOfWorkProperties = DataContext?.GetType()
                .GetProperties()
                .Where(p => ProjectHelper.IsUnitOfWorkType(p.PropertyType))
                .ToList();

            if (unitOfWorkProperties?.Count > 0)
            {
                // Automatically select the first IUnitofWork, or leave it null for user selection
                Data = unitOfWorkProperties.First().GetValue(DataContext) as IUnitofWork;
            }
        }
        public void SwitchBlockMode(DataBlockMode newMode)
        {
            if (newMode == BlockMode)
                return;

            if (BlockMode == DataBlockMode.CRUD)
                HandleDataChanges();

            BlockMode = newMode;

            foreach (var component in UIComponents.Values)
            {
                if (component is Control winFormsControl)
                {
                    if (newMode == DataBlockMode.Query)
                    {
                        _preservedBindings[component] = winFormsControl.DataBindings.Cast<Binding>().ToArray();
                        winFormsControl.DataBindings.Clear();
                        component.ClearValue();
                        component.ShowToolTip("Enter query criteria");
                    }
                    else if (newMode == DataBlockMode.CRUD)
                    {
                        if (_preservedBindings.ContainsKey(component))
                        {
                            foreach (var binding in _preservedBindings[component])
                                winFormsControl.DataBindings.Add(binding);

                            _preservedBindings.Remove(component);
                        }

                        component.RefreshBinding();
                        component.HideToolTip();
                    }
                }
            }

            foreach (var childBlock in ChildBlocks)
            {
                childBlock.SwitchBlockMode(newMode);
            }
        }
        public void HandleDataChanges()
        {
            foreach (var component in UIComponents.Values)
            {
                if (component.ValidateData(out string message))
                {
                    Console.WriteLine($"Validated {component.GuidID}: {message}");
                }
                else
                {
                    Console.WriteLine($"Validation failed for {component.GuidID}: {message}");
                }
            }

            foreach (var childBlock in ChildBlocks)
            {
                childBlock.HandleDataChanges();
            }

            if (_data?.IsDirty == true)
            {
                _data.Commit().Wait();
            }
        }
        public void SetMasterRecord(dynamic masterRecord)
        {
            MasterRecord = masterRecord;
            ApplyMasterDetailFilter();
        }
        private void ApplyMasterDetailFilter()
        {
            if (MasterRecord == null || Relationships == null || !Relationships.Any())
            {
                Data?.Units.RemoveFilter();
            }
            else
            {
                var masterKeyValues = Relationships.Select(r => GetPropertyValue(MasterRecord, r.RelatedEntityColumnID)).ToArray();

                Func<dynamic, bool> filterPredicate = entity =>
                {
                    var foreignKeyValues = Relationships.Select(r => GetPropertyValue(entity, r.EntityColumnID)).ToArray();
                    return masterKeyValues.SequenceEqual(foreignKeyValues);
                };

                Data?.Units.ApplyFilter(filterPredicate);
            }

            Data?.Units.MoveFirst();
        }
        private object GetPropertyValue(dynamic entity, string propertyName)
        {
            try
            {
                var property = entity?.GetType().GetProperty(propertyName);
                return property?.GetValue(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing property '{propertyName}': {ex.Message}");
                return null;
            }
        }
        private void Units_CurrentChanged(object sender, EventArgs e)
        {
            foreach (var childBlock in ChildBlocks)
            {
                childBlock.SetMasterRecord(Data?.Units.Current);
            }
        }
        public void RemoveChildBlock(IBeepDataBlock childBlock)
        {
            if (ChildBlocks.Contains(childBlock))
            {
                // Remove the child block
                ChildBlocks.Remove(childBlock);

                // Clear the child's parent reference
                childBlock.ParentBlock = null;
            }
        }
        public void RemoveParentBlock()
        {
            if (ParentBlock != null)
            {
                // Remove this block from the parent’s child list
                ParentBlock.ChildBlocks.Remove(this);

                // Clear the parent reference
                ParentBlock = null;

                // Clear master record and relationship properties
                MasterRecord = null;
                MasterKeyPropertyName = null;
                ForeignKeyPropertyName = null;
            }
        }
        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            ParentBlock?.RemoveChildBlock(this);

            foreach (var child in ChildBlocks.ToList())
            {
                child.RemoveParentBlock();
            }

            if (_data != null)
            {
                _data.Units.CurrentChanged -= Units_CurrentChanged;
            }

            GC.SuppressFinalize(this);
        }
    }

}
