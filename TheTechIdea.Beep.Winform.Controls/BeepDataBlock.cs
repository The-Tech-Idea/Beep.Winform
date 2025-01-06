using System.ComponentModel;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.DataBase;

using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Logic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepDataBlock))]
    [Category("Beep Controls")]
    [DisplayName("Beep Data Block")]
    public partial class BeepDataBlock : BeepControl, IDisposable, IBeepDataBlock
    {
        #region "Fields"
        private IUnitofWork _data;
        private bool _disposed;
        private IBeepService beepService;
        public Dictionary<string, IBeepUIComponent> UIComponents { get; set; } = new();
        private Dictionary<IBeepUIComponent, Binding[]> _preservedBindings = new();
        private BindingList<BeepComponents> _components; // these class will be used to create the UI Components  and presist the fields data and link between EntityFields from entitystructure and IBeepUIComponenet created for that field
        #endregion "Fields"
        #region "Properties"
        public event EventHandler<UnitofWorkParams> EventDataChanged;
        public DataBlockMode BlockMode { get; set; } = DataBlockMode.CRUD;
        public List<IBeepDataBlock> ChildBlocks { get; set; } = new();
        public bool IsInQueryMode { get; private set; } = false;

        [Browsable(true)]
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(BeepUIComponentTypeConverter))]
        [Description("Gets or sets the Field and Controls for this block.")]
        public BindingList<BeepComponents> Components
        {
            get => _components;
            set
            { 
                _components = value;
               
            }
        }
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Master-Detail")]
        [Description("Gets or sets the parent block for this block.")]
        [TypeConverter(typeof(DataBlockConverter))]
        [DisplayName("Parent Block")]
        public IBeepDataBlock ParentBlock { get; set; }
        [Browsable(true)]
        [Category("Data")]
        public string EntityName { get; set; }
        [Browsable(false)]
        public IEntityStructure EntityStructure { get;private  set; }
        [Browsable(true)]
        [Category("Data")]
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

        private Type _selectedEntity;
        [TypeConverter(typeof(EntityTypeConverter))]
        [Browsable(true)]
        [Category("Data")]
        public Type SelectedEntityType
        {
            get => _selectedEntity;
            set
            {
                if (value != null)
                {
                    _selectedEntity = value;


                        EntityStructure =EntityHelper.GetEntityStructureFromType(value);
                        if (EntityStructure != null)
                        {
                            EntityName = EntityStructure.EntityName;
                            InitializeEntityRelationships();
                        }
                        else
                        {
                            // Handle the case where EntityStructure is null
                            Console.WriteLine("EntityStructure is null after GetEntityStructureFromType.");
                        }
                    }
                    else
                    {
                        // Handle the case where beepService or beepService.util is null
                        Console.WriteLine("beepService or beepService.util is null.");
                    }
                
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IUnitofWork Data
        {
            get => _data;
            set
            {
                if (_data != value)
                {
                    if (_data != null)
                    {

                        UnsubscribeEvents(_data);
                    }
                        
                        

                    _data = value;

                    if (_data != null)
                    {
                        SubscribeEvents(_data);
                    }
                      

                    EntityStructure = _data?.EntityStructure;
                    InitializeEntityRelationships();
                }
            }
        }
        #endregion "Properties"
        #region "Constructors"
        public BeepDataBlock()
        {
            UIComponents = EntityHelper.GetAllAvailableUIComponents();
            IsShadowAffectedByTheme = false;
            IsRoundedAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            Components = new BindingList<BeepComponents>();
            Components.ListChanged += Components_ListChanged;

            // Load existing components or initialize new ones
         //   InitializeControls();
        }
        #endregion "Constructors"
        #region "Event Handlers"
        private void HandleDataChanges(object? sender, UnitofWorkParams e)
        {
            if (BlockMode == DataBlockMode.CRUD)
            {
                EventDataChanged?.Invoke(sender, e);
            }
        }
        private void SubscribeEvents(IUnitofWork data)
        {
            _data.Units.CurrentChanged += Units_CurrentChanged;
            _data.PreUpdate += HandleDataChanges;
            _data.PreInsert += HandleDataChanges;
            _data.PreDelete += HandleDataChanges;
            _data.PreQuery += HandleDataChanges;
            _data.PreCreate += HandleDataChanges;
            _data.PreCommit += HandleDataChanges;
            _data.PostUpdate += HandleDataChanges;
            _data.PostInsert += HandleDataChanges;
            _data.PostDelete += HandleDataChanges;
            _data.PostQuery += HandleDataChanges;
            _data.PostCreate += HandleDataChanges;
            _data.PostCommit += HandleDataChanges;
        }
        private void UnsubscribeEvents(IUnitofWork data)
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
        #endregion "Event Handlers"
        #region "Handle Relations"
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
        public void AddChildBlock(IBeepDataBlock childBlock)
        {
            if (childBlock == null)
                throw new ArgumentNullException(nameof(childBlock));
            if (!ChildBlocks.Contains(childBlock))
            {
                // Add the child block
                ChildBlocks.Add(childBlock);
                // Set the child's parent reference
                childBlock.ParentBlock = this;
            }
        }
        public void AddRelationship(RelationShipKeys relationship)
        {
            if (relationship == null)
                throw new ArgumentNullException(nameof(relationship));
            Relationships.Add(relationship);
            MasterKeyPropertyName = string.Join(", ", Relationships.Select(r => r.RelatedEntityColumnID));
            ForeignKeyPropertyName = string.Join(", ", Relationships.Select(r => r.EntityColumnID));
        }
        public void RemoveRelationship(RelationShipKeys relationship)
        {
            if (relationship == null)
                throw new ArgumentNullException(nameof(relationship));
            Relationships.Remove(relationship);
            MasterKeyPropertyName = string.Join(", ", Relationships.Select(r => r.RelatedEntityColumnID));
            ForeignKeyPropertyName = string.Join(", ", Relationships.Select(r => r.EntityColumnID));
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
                var masterKeyValues = Relationships.Select(r => EntityHelper.GetPropertyValue(MasterRecord, r.RelatedEntityColumnID)).ToArray();

                Func<dynamic, bool> filterPredicate = entity =>
                {
                    var foreignKeyValues = Relationships.Select(r => EntityHelper.GetPropertyValue(entity, r.EntityColumnID)).ToArray();
                    return masterKeyValues.SequenceEqual(foreignKeyValues);
                };

                Data?.Units.ApplyFilter(filterPredicate);
            }

            Data?.Units.MoveFirst();
        }
        #endregion "Handle Relations"
        #region "Block Management"
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
        private void Units_CurrentChanged(object sender, EventArgs e)
        {
            foreach (var childBlock in ChildBlocks)
            {
                childBlock.SetMasterRecord(Data?.Units.Current);
            }
        }
        #endregion "Block Management"
        #region "Beep UI Components Methods"
        private void InitializeControls()
        {
            if (EntityStructure == null || EntityStructure.Fields == null)
            {
                Console.WriteLine("EntityStructure or Fields is null. Cannot initialize controls.");
                return;
            }

            lock (_uiComponentsLock)
            {
                foreach (var field in EntityStructure.Fields)
                {
                    // Check if the field already exists in the Components list
                    var existingComponent = Components.FirstOrDefault(c => c.Name == field.fieldname);

                    if (existingComponent == null)
                    {
                        // Create a new component entry for this field if it doesn't exist
                        var newComponent = new BeepComponents
                        {
                            Name = field.fieldname,
                            TypeFullName = ControlExtensions.GetDefaultControlType(field.fieldCategory).FullName,
                            GUID = Guid.NewGuid().ToString(),
                            Left = 10,  // Default position
                            Top = 10 + Components.Count * 30,  // Stack vertically
                            Width = 200,  // Default width
                            Height = 25,  // Default height
                            BoundProperty = field.fieldname,
                            DataSourceProperty = field.fieldname,
                            LinkedProperty = string.Empty
                        };

                        // Add the new component to the Components list
                        Components.Add(newComponent);

                        existingComponent = newComponent;
                    }

                    // Create or retrieve the UI component
                    Type componentType = Type.GetType(existingComponent.TypeFullName);
                    if (componentType == null)
                    {
                        Console.WriteLine($"Cannot find type {existingComponent.TypeFullName} for field {field.fieldname}.");
                        continue;
                    }

                    var component = GetUIComponent(componentType);

                    // Configure the component's properties based on the existing component
                    component.ComponentName = existingComponent.Name;
                    component.Left = existingComponent.Left;
                    component.Top = existingComponent.Top;
                    component.Width = existingComponent.Width;
                    component.Height = existingComponent.Height;
                    component.GuidID = existingComponent.GUID;
                    component.BoundProperty = existingComponent.BoundProperty;

                    // Add the component to the UIComponents dictionary
                    UIComponents[component.GuidID] = component;

                    // Add the component to the Controls collection
                    if (component is Control winFormsControl)
                    {
                        Controls.Add(winFormsControl);
                    }
                }
            }
        }

        private readonly object _uiComponentsLock = new object();
        private IBeepUIComponent GetUIComponent(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            lock (_uiComponentsLock)
            {
                // Check if the component already exists in the dictionary
                var existingComponent = UIComponents.Values.FirstOrDefault(c => c.GetType() == type);
                if (existingComponent != null)
                    return existingComponent;

                // Ensure the type implements IBeepUIComponent and derives from Control
                if (!typeof(IBeepUIComponent).IsAssignableFrom(type))
                {
                    Console.WriteLine("Type {TypeName} does not implement IBeepUIComponent.", type.FullName);
                    return null;
                }

                if (!typeof(Control).IsAssignableFrom(type))
                {
                    Console.WriteLine("Type {TypeName} does not derive from Control.", type.FullName);
                    return null;
                }

                // Ensure the type has a parameterless constructor
                if (type.GetConstructor(Type.EmptyTypes) == null)
                {
                    Console.WriteLine("Type {TypeName} does not have a parameterless constructor.", type.FullName);
                    return null;
                }

                IBeepUIComponent componentInstance = null;
                try
                {
                    // Instantiate the component
                    componentInstance = (IBeepUIComponent)Activator.CreateInstance(type);
                }
                catch (Exception ex)
                {
                   beepService.DMEEditor.Logger.LogError("Error creating instance of {TypeName}.");
                    return null;
                }

                if (componentInstance != null)
                {
                    // Initialize component properties if necessary
                    componentInstance.DataContext = this.DataContext;
                    componentInstance.BlockID = this.BlockID;

                    // Cast to Control to add to Controls collection
                    if (componentInstance is Control winFormsControl)
                    {
                        // Optionally set default properties
                        winFormsControl.Visible = false; // Initially hidden or set as needed

                        // Add to Controls collection
                        this.Controls.Add(winFormsControl);
                    }

                    // Add to UIComponents dictionary
                    UIComponents[componentInstance.GuidID] = componentInstance;

                    Console.WriteLine("Component {ComponentName} of type {TypeName} instantiated and added to UIComponents.", componentInstance.ComponentName, type.FullName);
                }

                return componentInstance;
            }
        }
        private void InitializeComponentControl(BeepComponents component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            lock (_uiComponentsLock)
            {
                Type componentType = Type.GetType(component.TypeFullName);
                if (componentType == null)
                {
                   beepService.lg.LogError("Cannot find type {TypeFullName} for component GUID: {GUID}.");
                    return;
                }

                IBeepUIComponent control = GetUIComponent(componentType);
                if (control == null)
                {
                    beepService.lg.LogError("Failed to retrieve or instantiate component for type {TypeName}.");
                    return;
                }

                // Set component properties based on BeepComponents data
                control.ComponentName = component.Name;
                control.Left = component.Left;
                control.Top = component.Top;
                control.Width = component.Width;
                control.Height = component.Height;
                control.Id = component.Id;
                control.GuidID = component.GUID;
                control.LinkedProperty = component.LinkedProperty;
                control.BoundProperty = component.BoundProperty;
                control.DataSourceProperty = component.DataSourceProperty;
                control.BlockID = component.FieldID;

                // Apply category-specific settings
                switch (component.Category)
                {
                    case DbFieldCategory.String:
                        control.ShowToolTip("Enter text.");
                        break;
                    case DbFieldCategory.Numeric:
                        control.ShowToolTip("Enter a numeric value.");
                        break;
                    case DbFieldCategory.Date:
                        control.ShowToolTip("Select a date.");
                        break;
                    case DbFieldCategory.Boolean:
                        control.ShowToolTip("Toggle boolean value.");
                        break;
                    case DbFieldCategory.Binary:
                        control.ShowToolTip("Enter binary data.");
                        break;
                    case DbFieldCategory.Guid:
                        control.ShowToolTip("Enter a GUID.");
                        break;
                    case DbFieldCategory.Json:
                        control.ShowToolTip("Enter JSON data.");
                        break;
                    case DbFieldCategory.Xml:
                        control.ShowToolTip("Enter XML data.");
                        break;
                    case DbFieldCategory.Geography:
                        control.ShowToolTip("Enter geographical data.");
                        break;
                    case DbFieldCategory.Currency:
                        control.ShowToolTip("Enter a currency amount.");
                        break;
                    case DbFieldCategory.Enum:
                        control.ShowToolTip("Select an option from the list.");
                        break;
                    case DbFieldCategory.Timestamp:
                        control.HideToolTip(); // Timestamps may not require user interaction
                        break;
                    case DbFieldCategory.Complex:
                        control.HideToolTip(); // Complex types may be handled differently
                        break;
                    default:
                        control.HideToolTip();
                        break;
                }

                // Ensure the control is visible and properly positioned
                if (control is Control winFormsControl)
                {
                    winFormsControl.Visible = true;
                    winFormsControl.BringToFront();
                }

                beepService.lg.LogInfo("Initialized component {ComponentName} (GUID: {GUID}) at position ({Left}, {Top}).");

                // Position components after initialization
                PositionUIComponents();
              
            }
        }
        private void UpdateComponentControl(BeepComponents component)
        {
            lock (_uiComponentsLock)
            {
                if(component == null)
                {
                    return;
                }
                // check if the control Type has changed and update it
                if (UIComponents.ContainsKey(component.GUID))
                {
                    var control = UIComponents[component.GUID];
                    if (control.GetType().FullName != component.TypeFullName)
                    {
                        control = GetUIComponent(Type.GetType(component.TypeFullName));
                        if (control != null)
                        {
                            control.Left = component.Left;
                            control.Top = component.Top;
                            control.Width = component.Width;
                            control.Height = component.Height;
                            control.Id = component.Id;
                            control.GuidID = component.GUID;
                            control.Id = component.Id;
                            control.LinkedProperty = component.LinkedProperty;
                            control.BoundProperty = component.BoundProperty;
                            control.DataSourceProperty = component.DataSourceProperty;
                            control.BlockID = component.FieldID;
                            UIComponents[component.GUID] = control;
                        }
                    }
                }else
                {
                    // update control properties
                    if (UIComponents.ContainsKey(component.GUID))
                    {
                        var control = UIComponents[component.GUID];
                        control.Left = component.Left;
                        control.Top = component.Top;
                        control.Width = component.Width;
                        control.Height = component.Height;
                        control.Id = component.Id;
                        control.GuidID = component.GUID;
                        control.Id = component.Id;
                        control.LinkedProperty = component.LinkedProperty;
                        control.BoundProperty = component.BoundProperty;
                        control.DataSourceProperty = component.DataSourceProperty;
                        control.BlockID = component.FieldID;
                        UIComponents[component.GUID] = control;
                    }
                }


            }
        }
        private void RemoveComponentControl(int index)
        {
            if (index < 0 || index >= Components.Count) return;

            var component = Components[index];

            if (UIComponents.ContainsKey(component.GUID))
            {
                var control = UIComponents[component.GUID];

                if (control is Control winFormsControl)
                {
                    Controls.Remove(winFormsControl);
                    winFormsControl.Dispose();
                }

                UIComponents.Remove(component.GUID);
            }
        }
        private void ClearAllComponentControls()
        {
            lock (_uiComponentsLock)
            {
                foreach (var control in UIComponents.Values.ToList())
                {
                    if (control is Control winFormsControl)
                    {
                        this.Controls.Remove(winFormsControl);
                        winFormsControl.Dispose();
                    }

                    UIComponents.Remove(control.GuidID);
                }
            }
        }
        private void Components_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    InitializeComponentControl(Components[e.NewIndex]);
                    break;
                case ListChangedType.ItemChanged:
                    UpdateComponentControl(Components[e.NewIndex]);
                    break;
                case ListChangedType.ItemDeleted:
                    RemoveComponentControl(e.NewIndex);
                    break;
                case ListChangedType.Reset:
                    ClearAllComponentControls();
                    break;
            }
        }
        private void PositionUIComponents()
        {
            lock (_uiComponentsLock)
            {
                // get data from the presisted Components and create the UI Components
                foreach (var component in Components)
                {
                    IBeepUIComponent control = GetUIComponent(component.Type);
                    if (control != null)
                    {
                        control.Left = component.Left;
                        control.Top = component.Top;
                        control.Width = component.Width;
                        control.Height = component.Height;
                        control.Id = component.Id;
                        control.GuidID = component.GUID;
                        control.Id = component.Id;
                        control.LinkedProperty = component.LinkedProperty;
                        control.BoundProperty = component.BoundProperty;
                        control.DataSourceProperty = component.DataSourceProperty;
                        control.BlockID = component.FieldID;
                        UIComponents[component.GUID] = control;
                    }
                }
            }
        }

        #endregion "Beep UI Co  mponents Methods"
        #region "Util"

        #endregion "Util"
        #region "Dispose"
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
        #endregion "Dispose"

    }

}
