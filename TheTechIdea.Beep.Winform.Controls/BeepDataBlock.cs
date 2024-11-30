using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DataNavigator;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDataBlock : BeepControl
    {
        protected IUnitofWork _data;

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
                        // Unsubscribe from previous events
                        _data.Units.CurrentChanged -= Units_CurrentChanged;
                    }

                    _data = value;
                    if (_data != null)
                    {
                        // Subscribe to new events
                        _data.Units.CurrentChanged += Units_CurrentChanged;
                    }
                    Refresh();
                }
            }
        }

        private BeepDataNavigator dataNavigator;

        // Master record for filtering detail data
        private dynamic _masterRecord;
        private string _foreignKeyPropertyName;
        private string _masterKeyPropertyName;

        // Support for multiple detail blocks
        private List<ChildBlockInfo> _childBlocks = new List<ChildBlockInfo>();

        public BeepDataBlock()
        {
            // Initialize component
            InitializeComponent();

            // Initialize the data navigator
            InitializeDataNavigator();
        }

        private void InitializeComponent()
        {
            this.AutoScroll = true;
            this.BackColor = Color.White;
        }

        /// <summary>
        /// Creates controls based on the entity structure.
        /// </summary>
        private void CreateControlsBasedOnStructure()
        {
            if (Structure == null)
                return;

            int yPosition = 10;
            int xPosition = 10;
            int labelWidth = 100;
            int controlWidth = 200;
            int controlHeight = 25;
            int verticalSpacing = 5;

            foreach (var field in Structure.Fields)
            {
                // Create label
                Label label = new Label
                {
                    Text = field.fieldname,
                    Location = new Point(xPosition, yPosition),
                    Width = labelWidth,
                    Height = controlHeight
                };
                this.Controls.Add(label);

                // Create control
                var control = CreateAndBindBeepControl(field);
                if (control != null)
                {
                    Control winControl = control as Control;
                    winControl.Location = new Point(xPosition + labelWidth + 5, yPosition);
                    winControl.Width = controlWidth;
                    winControl.Height = controlHeight;

                    this.Controls.Add(winControl);
                    UIComponents.Add(control);

                    yPosition += controlHeight + verticalSpacing;
                }
            }
        }

        /// <summary>
        /// Creates and binds a BeepControl based on the field type.
        /// </summary>
        private IBeepUIComponent CreateAndBindBeepControl(EntityField field)
        {
            BeepControl beepControl = null;

            // Select BeepControl type based on field category
            switch (field.fieldCategory)
            {
                case DbFieldCategory.String:
                    beepControl = new BeepTextBox();
                    break;

                case DbFieldCategory.Numeric:
                case DbFieldCategory.Currency:
                    beepControl = new BeepNumericUpDown();
                    break;

                case DbFieldCategory.Date:
                case DbFieldCategory.Timestamp:
                    beepControl = new BeepDatePicker();
                    break;

                case DbFieldCategory.Boolean:
                    beepControl = new BeepCheckBox();
                    break;

                // Add cases for other field categories as needed
                default:
                    beepControl = new BeepTextBox();
                    break;
            }

            // Bind BeepControl to data
            if (beepControl != null && Data != null)
            {
                // Set the DataContext to the CurrentItem of UnitOfWork
                beepControl.DataContext = Data.Units.Current;

                // Set the property name to bind to
                beepControl.SetBinding("Value", field.fieldname);
            }

            return beepControl as IBeepUIComponent;
        }

        /// <summary>
        /// Initializes the data navigator and hooks up events.
        /// </summary>
        private void InitializeDataNavigator()
        {
            dataNavigator = new BeepDataNavigator
            {
                Dock = DockStyle.Bottom,
                Height = 40
            };

            dataNavigator.UnitOfWork = Data;

            // Hook up navigation events
            // No need to subscribe to navigation events since BeepDataNavigator handles them internally

            // Subscribe to CRUD events to relay them to the parent or handle as needed
            dataNavigator.NewRecordCreated += DataNavigator_NewRecordCreated;
            dataNavigator.SaveCalled += DataNavigator_SaveCalled;
            dataNavigator.DeleteCalled += DataNavigator_DeleteCalled;

            this.Controls.Add(dataNavigator);
        }

        // Event handlers for CRUD events raised by the data navigator
        private void DataNavigator_NewRecordCreated(object sender, BeepEventDataArgs e)
        {
            // Raise the event to notify listeners or handle it here
            // You can create a new record in your data source here if needed
        }

        private void DataNavigator_SaveCalled(object sender, BeepEventDataArgs e)
        {
            // Handle save logic here or notify the data source
        }

        private void DataNavigator_DeleteCalled(object sender, BeepEventDataArgs e)
        {
            // Handle delete logic here or notify the data source
        }

        /// <summary>
        /// Handles CurrentChanged event from the Units collection.
        /// </summary>
        private void Units_CurrentChanged(object sender, EventArgs e)
        {
            UpdateDataContext();
            NotifyChildBlocksOfMasterChange();
        }

        /// <summary>
        /// Updates the DataContext of each UI component.
        /// </summary>
        private void UpdateDataContext()
        {
            if (Data != null)
            {
                foreach (var component in UIComponents)
                {
                    component.DataContext = Data.Units.Current;
                    component.RefreshBinding(); // Ensure each component refreshes its bindings
                }
            }
        }

        /// <summary>
        /// Overrides the Refresh method to rebuild the UI components.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();

            // Clear existing controls except dataNavigator
            Control[] controlsToRemove = new Control[this.Controls.Count];
            this.Controls.CopyTo(controlsToRemove, 0);
            foreach (var control in controlsToRemove)
            {
                if (control != dataNavigator)
                {
                    this.Controls.Remove(control);
                }
            }
            UIComponents.Clear();

            // Re-initialize controls
            CreateControlsBasedOnStructure();

            // Update data navigator's UnitOfWork
            if (dataNavigator != null && Data != null)
            {
                dataNavigator.UnitOfWork = Data;
                dataNavigator.UpdateRecordCountDisplay();
            }
        }

        // Support for multiple detail blocks
        /// <summary>
        /// Adds a child data block to the current block.
        /// </summary>
        /// <param name="childBlock">The child data block instance.</param>
        /// <param name="masterKeyPropertyName">The property name of the master key in the master entity.</param>
        /// <param name="foreignKeyPropertyName">The property name of the foreign key in the child entity.</param>
        public void AddChildBlock(BeepDataBlock childBlock, string masterKeyPropertyName, string foreignKeyPropertyName)
        {
            var childInfo = new ChildBlockInfo
            {
                ChildBlock = childBlock,
                MasterKeyPropertyName = masterKeyPropertyName,
                ForeignKeyPropertyName = foreignKeyPropertyName
            };

            _childBlocks.Add(childInfo);

            // Set initial master record for the child block
            var masterRecord = Data?.Units.Current;
            childBlock.SetMasterRecord(masterRecord, masterKeyPropertyName, foreignKeyPropertyName);
        }

        /// <summary>
        /// Notifies all child blocks that the master record has changed.
        /// </summary>
        private void NotifyChildBlocksOfMasterChange()
        {
            var masterRecord = Data?.Units.Current;
            foreach (var childInfo in _childBlocks)
            {
                childInfo.ChildBlock.SetMasterRecord(masterRecord, childInfo.MasterKeyPropertyName, childInfo.ForeignKeyPropertyName);
            }
        }

        /// <summary>
        /// Sets the master record for this data block (used if this block is a detail block).
        /// </summary>
        /// <param name="masterRecord">The master record object.</param>
        /// <param name="masterKeyPropertyName">The property name of the master record's key.</param>
        /// <param name="foreignKeyPropertyName">The property name of the detail record's foreign key.</param>
        public void SetMasterRecord(dynamic masterRecord, string masterKeyPropertyName, string foreignKeyPropertyName)
        {
            _masterRecord = masterRecord;
            _masterKeyPropertyName = masterKeyPropertyName;
            _foreignKeyPropertyName = foreignKeyPropertyName;

            ApplyMasterDetailFilter();
            Refresh();
        }

        /// <summary>
        /// Applies filtering based on the master record to show related detail records.
        /// </summary>
        private void ApplyMasterDetailFilter()
        {
            if (_masterRecord == null || string.IsNullOrEmpty(_foreignKeyPropertyName) || string.IsNullOrEmpty(_masterKeyPropertyName))
            {
                // Remove filter
                Data.Units.RemoveFilter();
            }
            else
            {
                var masterKeyValue = GetPropertyValue(_masterRecord, _masterKeyPropertyName);

                // Build filter predicate
                Func<dynamic, bool> filterPredicate = entity =>
                {
                    var value = GetPropertyValue(entity, _foreignKeyPropertyName);
                    return value != null && value.Equals(masterKeyValue);
                };

                // Apply filter
                Data.Units.ApplyFilter(filterPredicate);
            }

            Data.Units.MoveFirst();
        }

        /// <summary>
        /// Helper method to get property value using reflection.
        /// </summary>
        private object GetPropertyValue(dynamic entity, string propertyName)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var property = entity.GetType().GetProperty(propertyName);
            if (property == null)
                throw new InvalidOperationException($"Property '{propertyName}' not found on type '{entity.GetType().Name}'.");

            return property.GetValue(entity);
        }

        /// <summary>
        /// Holds information about a child data block.
        /// </summary>
        private class ChildBlockInfo
        {
            public BeepDataBlock ChildBlock { get; set; }
            public string MasterKeyPropertyName { get; set; }
            public string ForeignKeyPropertyName { get; set; }
        }
    }
}
