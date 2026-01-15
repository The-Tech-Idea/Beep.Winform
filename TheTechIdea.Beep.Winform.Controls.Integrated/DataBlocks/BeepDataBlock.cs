using System.ComponentModel;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Integrated.Modules;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Editor.UOWManager.Models;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls.Integrated.DataBlocks.Helpers;
using TheTechIdea.Beep.Services;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepDataBlock))]
    [Category("Beep Controls")]
    [DisplayName("Beep Data Block")]
    [Description("A data block control that displays and manages data.")]
   
    public partial class BeepDataBlock : BaseControl, IDisposable, IBeepDataBlock
    {
        #region "Fields"
        private IUnitofWork _data;
        private bool _disposed;
        private IBeepService beepService;
        private IDMEEditor _dmeEditor;
        public Dictionary<string, IBeepUIComponent> UIComponents { get; set; } = new();
        private Dictionary<IBeepUIComponent, Binding[]> _preservedBindings = new();
        private BindingList<BeepComponents> _components; // these class will be used to create the UI Components  and presist the fields data and link between EntityFields from entitystructure and IBeepUIComponenet created for that field
        #endregion "Fields"
        #region "Properties"
        public event EventHandler<UnitofWorkParams> EventDataChanged;
        public DataBlockMode BlockMode { get; set; } = DataBlockMode.CRUD;
        public List<IBeepDataBlock> ChildBlocks { get; set; } = new();
        public bool IsInQueryMode { get; private set; } = false;
        public string Status { get; private set; }

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

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDMEEditor DMEEditor
        {
            get => _dmeEditor;
            set
            {
                _dmeEditor = value;
                if (_dmeEditor != null && beepService == null)
                {
                    // Try to get BeepService from DMEEditor
                    InitializeServices();
                }
            }
        }

        [Browsable(true)]
        [Category("Form Coordination")]
        [Description("Name of the form this block belongs to (for FormsManager coordination)")]
        public string FormName { get; set; }

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
                        ClearAllComponentControls(); // Clear previous controls
                        InitializeControls(); // Auto-generate controls
                    }
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
                    ClearAllComponentControls(); // Clear previous controls
                    InitializeControls(); // Auto-generate controls
                }
            }
        }

        // NOTE: IBeepDataBlock.BlockMode implementation removed (was throwing NotImplementedException)
        // The working BlockMode property above (line 33) satisfies the interface requirement
        #endregion "Properties"
        #region "Constructors"
        public BeepDataBlock()
        {
            // Visual configuration
            IsShadowAffectedByTheme = false;
            IsRoundedAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            
            // Component collection setup
            Components = new BindingList<BeepComponents>();
            Components.ListChanged += Components_ListChanged;
            
            // Initialize services (critical fix for beepService null reference)
            InitializeServices();
        }
        #endregion "Constructors"
        #region "Event Handlers"
        private async void HandleDataChanges(object? sender, UnitofWorkParams e)
        {
            // e.Entity or e.Data should reference the current record.
            // User can modify e.Entity in their event handler to set default values or update properties.

            var args = new BeepDataBlockEventArgs("Handling data changes (transaction event).");
            OnAction?.Invoke(this, args);
            if (args.Cancel)
                return;

            // Fire appropriate trigger based on event type
            if (sender == _data)
            {
                // Determine which trigger to fire based on the event
                // This integrates the new trigger system with existing UnitofWork events
                if (e != null)
                {
                    // Fire corresponding triggers
                    // Note: Trigger execution is already integrated in the operation methods
                }
            }

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
            ApplyMasterDetailFilter().Wait();
        }
        public async Task<bool> ApplyMasterDetailFilter()
        {
            try
            {
                if (Data?.IsDirty == true)
                {
                    var args = new BeepDataBlockEventArgs("Data has unsaved changes. Please save or cancel before changing master record.");
                    OnAction?.Invoke(this, args);
                    if (args.Cancel)
                        return false;
                }

                if (MasterRecord == null || Relationships == null || !Relationships.Any())
                {
                    // No master: load all records
                    if (Data is IUnitofWork unitOfWork)
                    {
                        var getMethod = unitOfWork.GetType().GetMethod("Get", Type.EmptyTypes);
                        if (getMethod != null)
                        {
                            var task = (Task)getMethod.Invoke(unitOfWork, null);
                            await task;
                        }
                    }
                }
                else
                {
                    // Build filters based on master keys
                    var filters = new List<AppFilter>();
                    foreach (var rel in Relationships)
                    {
                        var masterValue = EntityHelper.GetPropertyValue(MasterRecord, rel.RelatedEntityColumnID)?.ToString();
                        if (!string.IsNullOrEmpty(masterValue))
                        {
                            filters.Add(new AppFilter
                            {
                               FieldName = rel.EntityColumnID,
                                Operator = "=",
                                FilterValue = masterValue
                            });
                        }
                    }
                    // Fetch only related records
                    if (Data is IUnitofWork unitOfWork)
                    {
                        var getWithFilters = unitOfWork.GetType().GetMethod("Get", new[] { typeof(List<AppFilter>) });
                        if (getWithFilters != null)
                        {
                            var task = (Task)getWithFilters.Invoke(unitOfWork, new object[] { filters });
                            await task;
                        }
                    }
                }
                Data?.Units.MoveFirst();
                return true;
            }
            catch (Exception ex)
            {
                var args = new BeepDataBlockEventArgs($"Error in ApplyMasterDetailFilter: {ex.Message}", ex);
                OnAction?.Invoke(this, args);
                // If Cancel is set by handler, respect it (optional for error case)
                return false;
            }
        }
        #endregion "Handle Relations"
        #region "Block Management"
        // Synchronous interface implementation for IBeepDataBlock
        public void SwitchBlockMode(DataBlockMode newMode)
        {
            // For compatibility, call the async version and wait (not recommended for UI, but required for interface)
            SwitchBlockModeAsync(newMode).GetAwaiter().GetResult();
        }

        public async Task SwitchBlockModeAsync(DataBlockMode newMode)
        {
            // Check for unsaved changes when switching modes
            if (!await CheckAndHandleUnsavedChangesRecursiveAsync())
                return;
            var args = new BeepDataBlockEventArgs($"Switching block mode to {newMode}.");
            OnAction?.Invoke(this, args);
            if (args.Cancel)
                return;

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
                        component.HideToolTip();
                    }
                }
            }

            foreach (var childBlock in ChildBlocks)
            {
                if (childBlock is BeepDataBlock childBeepBlock)
                    await childBeepBlock.SwitchBlockModeAsync(newMode);
               
            }
        }

        public void HandleDataChanges()
        {
            foreach (var component in UIComponents.Values)
            {
                if (component.ValidateData(out string message))
                {
                   // Console.WriteLine($"Validated {component.GuidID}: {message}");
                }
                else
                {
                   // Console.WriteLine($"Validation failed for {component.GuidID}: {message}");
                }
            }

            foreach (var childBlock in ChildBlocks)
            {
                childBlock.HandleDataChanges();
            }

            if (_data?.IsDirty == true)
            {
                // Use enhanced Unit of Work helper for commit
                var commitResult = DataBlockUnitOfWorkHelper.CommitAsync(_data).Result;
                if (commitResult.Flag != Errors.Ok)
                {
                    Status = $"Commit failed: {commitResult.Message}";
                }
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
               // Console.WriteLine("EntityStructure or Fields is null. Cannot initialize controls.");
                return;
            }

            lock (_uiComponentsLock)
            {
                foreach (var field in EntityStructure.Fields)
                {
                    // Check if the field already exists in the Components list
                    var existingComponent = Components.FirstOrDefault(c => c.Name == field.FieldName);

                    if (existingComponent == null)
                    {
                        // Create a new component entry for this field if it doesn't exist
                        var newComponent = new BeepComponents
                        {
                            Name = field.FieldName,
                            TypeFullName = ControlExtensions.GetDefaultControlType(field.FieldCategory).FullName,
                            GUID = Guid.NewGuid().ToString(),
                            Left = 10,  // Default position
                            Top = 10 + Components.Count * 30,  // Stack vertically
                            Width = 200,  // Default width
                            Height = 25,  // Default height
                            BoundProperty = field.FieldName,
                            DataSourceProperty = field.FieldName,
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
                       // Console.WriteLine($"Cannot find type {existingComponent.TypeFullName} for field {field.FieldName}.");
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
                   // Console.WriteLine("Type {TypeName} does not implement IBeepUIComponent.", type.FullName);
                    return null;
                }

                if (!typeof(Control).IsAssignableFrom(type))
                {
                   // Console.WriteLine("Type {TypeName} does not derive from Control.", type.FullName);
                    return null;
                }

                // Ensure the type has a parameterless constructor
                if (type.GetConstructor(Type.EmptyTypes) == null)
                {
                   // Console.WriteLine("Type {TypeName} does not have a parameterless constructor.", type.FullName);
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

                   // Console.WriteLine("Component {ComponentName} of type {TypeName} instantiated and added to UIComponents.", componentInstance.ComponentName, type.FullName);
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

        #endregion "Beep UI Components Methods"
        #region "Enter-Query / Execute-Query Mode"
        /// <summary>
        /// Collects query parameters from UI controls and builds a List<AppFilter> for querying.
        /// </summary>
        private List<AppFilter> GetQueryFiltersFromControls()
        {
            var filters = new List<AppFilter>();
            foreach (var component in UIComponents.Values)
            {
                if (!string.IsNullOrEmpty(component.BoundProperty))
                {
                    var value = component.GetValue();
                    if (value != null && !string.IsNullOrEmpty(value.ToString()))
                    {
                        filters.Add(new AppFilter
                        {
                           FieldName = component.BoundProperty,
                            Operator = "=", // You can enhance this to support other operators
                            FilterValue = value.ToString()
                        });
                    }
                }
            }
            return filters;
        }

        public async Task ExecuteQueryAsync()
        {
            if (!IsInQueryMode)
                return;

            // Use enhanced Unit of Work helper
            var filters = GetQueryFiltersFromControls();
            await ExecuteQueryWithUnitOfWorkAsync(filters, null);
            IsInQueryMode = false;
        }
        #endregion
        #region "Record Navigation Methods"
        public async Task MoveNextAsync()
        {
            // Use enhanced Unit of Work helper
            await MoveNextWithUnitOfWorkAsync();
        }

        public async Task MovePreviousAsync()
        {
            // Use enhanced Unit of Work helper
            await MovePreviousWithUnitOfWorkAsync();
        }

        public async Task InsertRecordAsync(Entity newRecord)
        {
            // Use enhanced Unit of Work helper
            await InsertRecordWithUnitOfWorkAsync(newRecord);
        }

        public async Task DeleteCurrentRecordAsync()
        {
            // Use enhanced Unit of Work helper
            await DeleteCurrentRecordWithUnitOfWorkAsync();
        }

        public async Task RollbackAsync()
        {
            // Use enhanced Unit of Work helper
            await RollbackWithUnitOfWorkAsync();
        }

        public void UndoLastChange()
        {
            if (Data != null)
            {
                try
                {
                    Data.UndoLastChange();
                    Status = "Undo last change successful.";
                }
                catch (Exception ex)
                {
                    Status = $"Undo last change failed: {ex.Message}";
                }
            }
        }
        #endregion
        #region "Util"
        /// <summary>
        /// Event raised when unsaved changes are detected before a critical operation.
        /// </summary>
        public event EventHandler<BlockDirtyEventArgs> OnUnsavedChanges;

        /// <summary>
        /// Checks if this block or any child block is dirty, and raises OnUnsavedChanges event for user handling.
        /// Returns true if operation should continue, false if cancelled.
        /// </summary>
        public bool CheckAndHandleUnsavedChanges()
        {
            bool cancel = false;
            // Check this block
            if (Data?.IsDirty == true)
            {
                var args = new BlockDirtyEventArgs(this);
                OnUnsavedChanges?.Invoke(this, args);
                if (args.Cancel)
                    cancel = true;
            }
            // Check child blocks recursively
            foreach (var child in ChildBlocks)
            {
                if (!cancel && child is BeepDataBlock childBlock)
                {
                    if (!childBlock.CheckAndHandleUnsavedChanges())
                        cancel = true;
                }
            }
            return !cancel;
        }

        /// <summary>
        /// Checks if this block or any child block is dirty, raises OnUnsavedChanges ONCE (from master),
        /// and saves or rolls back changes for all dirty blocks based on user choice.
        /// Returns true if operation should continue, false if cancelled.
        /// </summary>
        public async Task<bool> CheckAndHandleUnsavedChangesRecursiveAsync()
        {
            // Gather all dirty blocks (self and children)
            var dirtyBlocks = new List<BeepDataBlock>();
            CollectDirtyBlocks(this, dirtyBlocks);
            if (dirtyBlocks.Count == 0)
                return true;

            var args = new BlockDirtyEventArgs(this);
            OnUnsavedChanges?.Invoke(this, args);
            if (args.Cancel)
            {
                // User chose to cancel
                foreach (var block in dirtyBlocks)
                {
                    if (block.Data != null)
                        await block.Data.Rollback();
                }
                return false;
            }
            else
            {
                // User chose to save
                foreach (var block in dirtyBlocks)
                {
                    if (block.Data != null)
                        await block.Data.Commit();
                }
                return true;
            }
        }

        /// <summary>
        /// Helper to collect all dirty blocks recursively.
        /// </summary>
        private void CollectDirtyBlocks(BeepDataBlock block, List<BeepDataBlock> dirtyBlocks)
        {
            if (block.Data?.IsDirty == true)
                dirtyBlocks.Add(block);
            foreach (var child in block.ChildBlocks)
            {
                if (child is BeepDataBlock childBlock)
                    CollectDirtyBlocks(childBlock, dirtyBlocks);
            }
        }

        public class BlockDirtyEventArgs : EventArgs
        {
            public BeepDataBlock Block { get; }
            public bool Cancel { get; set; }
            public BlockDirtyEventArgs(BeepDataBlock block) { Block = block; }
        }
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

        #region "Service Initialization"
        
        /// <summary>
        /// Initialize beepService and DMEEditor
        /// Walks up parent chain to find Form with IBeepService or DMEEditor property
        /// </summary>
        private void InitializeServices()
        {
            try
            {
                // Try to find IBeepService or DMEEditor from parent form
                FindServicesInParentChain();
                
                // Design-time: services may be null, operations will check before using
                // This is expected and safe
            }
            catch
            {
                // Silently fail - services are optional for design-time
                // Runtime operations will check for null before using
            }
        }
        
        /// <summary>
        /// Walk up parent chain to find Form with IBeepService or DMEEditor
        /// </summary>
        private void FindServicesInParentChain()
        {
            if (!IsHandleCreated)
                return;
                
            var parent = this.Parent;
            while (parent != null)
            {
                if (parent is Form form)
                {
                    // Check if form implements IBeepService
                    if (form is IBeepService service)
                    {
                        beepService = service;
                        _dmeEditor = service.DMEEditor;
                        return;
                    }
                    
                    // Check if form has a BeepService property
                    var serviceProperty = form.GetType().GetProperty("BeepService");
                    if (serviceProperty != null)
                    {
                        var serviceValue = serviceProperty.GetValue(form);
                        if (serviceValue is IBeepService svc)
                        {
                            beepService = svc;
                            _dmeEditor = svc.DMEEditor;
                            return;
                        }
                    }
                    
                    // Check if form has a DMEEditor property
                    var dmeProperty = form.GetType().GetProperty("DMEEditor");
                    if (dmeProperty != null)
                    {
                        var dmeValue = dmeProperty.GetValue(form);
                        if (dmeValue is IDMEEditor dme)
                        {
                            _dmeEditor = dme;
                            // beepService remains null, but we have DMEEditor
                            return;
                        }
                    }
                }
                parent = parent.Parent;
            }
        }
        
        #endregion "Service Initialization"

        #region "Events"
        // Event args for BeepDataBlock actions/errors
        public class BeepDataBlockEventArgs : EventArgs
        {
            public string Message { get; set; }
            public Exception Exception { get; set; }
            public bool Cancel { get; set; }
          
            public BeepDataBlockEventArgs(string message, Exception ex = null)
            {
                Message = message;
                Exception = ex;
            }
        }

        // Event for actions/errors
        public event EventHandler<BeepDataBlockEventArgs> OnAction;

        // Events for Oracle Forms-like triggers
        public event EventHandler<UnitofWorkParams> OnPreQuery;
        public event EventHandler<UnitofWorkParams> OnPostQuery;
        public event EventHandler<UnitofWorkParams> OnPreInsert;
        public event EventHandler<UnitofWorkParams> OnPostInsert;
        public event EventHandler<UnitofWorkParams> OnPreUpdate;
        public event EventHandler<UnitofWorkParams> OnPostUpdate;
        public event EventHandler<UnitofWorkParams> OnPreDelete;
        public event EventHandler<UnitofWorkParams> OnPostDelete;
        public event EventHandler<UnitofWorkParams> OnValidateRecord;
        public event EventHandler<UnitofWorkParams> OnValidateItem;
        public event EventHandler<UnitofWorkParams> OnNewRecord;
        public event EventHandler<UnitofWorkParams> OnNewBlock;
        #endregion "Events"
    }

}
