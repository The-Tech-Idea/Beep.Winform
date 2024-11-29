using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.DataNavigator;
using TheTechIdea.Beep.Vis.Modules;
using DialogResult = System.Windows.Forms.DialogResult;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDataBlock:BeepDataBlock<Entity> 
    {
        public BeepDataBlock()
        {
        }
    }

    public class BeepDataBlock<T> : BeepPanel where T : Entity, INotifyPropertyChanged, new()
    {
        protected UnitofWork<T> _data;

        public IEntityStructure Structure => Data != null ? Data.EntityStructure : null;

        public List<IBeepUIComponent> UIComponents { get; set; } = new List<IBeepUIComponent>();

        [Browsable(true)]
        [Category("Data")]
        [TypeConverter(typeof(UnitOfWorkConverter))]
        public UnitofWork<T> Data
        {
            get { return _data; }
            set
            {
                if (_data != value)
                {
                    if (_data != null)
                    {
                        _data.PropertyChanged -= Data_PropertyChanged;
                    }
                    _data = value;
                    if (_data != null)
                    {
                        _data.PropertyChanged += Data_PropertyChanged;
                        _data.Units.CurrentChanged += Units_CurrentChanged;
                    }
                    Refresh();
                }
            }
        }

        private BeepDataNavigator dataNavigator;

        // Master record for filtering detail data
        private object _masterRecord;
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
        private void CreateControlsBasedonStructure()
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
                    beepControl = new BeepNumericUpDown();
                    break;

                case DbFieldCategory.Date:
                    beepControl = new BeepDatePicker();
                    break;

                default:
                    beepControl = new BeepTextBox();
                    break;
            }

            // Bind BeepControl to data
            if (beepControl != null && Data != null)
            {
                // Set the DataContext to the CurrentItem of UnitOfWork
                beepControl.DataContext = Data.CurrentItem;

                // Set the property name to bind to
                beepControl.SetBinding("Text", field.fieldname);
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

            dataNavigator.BindingSource = new BeepBindingSource();
            dataNavigator.BindingSource.DataSource = Data?.Units;

            // Hook up events
            dataNavigator.NewRecordCreated += DataNavigator_NewRecordCreated;
            dataNavigator.SaveCalled += DataNavigator_SaveCalled;
            dataNavigator.DeleteCalled += DataNavigator_DeleteCalled;

            this.Controls.Add(dataNavigator);
        }

        /// <summary>
        /// Handles the NewRecordCreated event from the data navigator.
        /// </summary>
        private void DataNavigator_NewRecordCreated(object sender, BeepEventDataArgs e)
        {
            var newEntity = new T();

            // If this is a detail block, set the foreign key value to match the master record
            if (_masterRecord != null && !string.IsNullOrEmpty(_foreignKeyPropertyName))
            {
                var masterKeyValue = GetPropertyValue(_masterRecord, _masterKeyPropertyName);
                SetPropertyValue(newEntity, _foreignKeyPropertyName, masterKeyValue);
            }

            Data.Create(newEntity);
            Data.MoveLast(); // Move to the new record
            UpdateDataContext();
        }

        /// <summary>
        /// Handles the SaveCalled event from the data navigator.
        /// </summary>
        private async void DataNavigator_SaveCalled(object sender, BeepEventDataArgs e)
        {
            if (ValidateData())
            {
                await Data.Commit();
                MessageBox.Show("Record saved successfully.");
            }
        }

        /// <summary>
        /// Handles the DeleteCalled event from the data navigator.
        /// </summary>
        private async void DataNavigator_DeleteCalled(object sender, BeepEventDataArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to delete this item?",
                                                 "Confirm Delete!",
                                                 MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                await Data.DeleteAsync(Data.CurrentItem);
                UpdateDataContext();
            }
        }

        /// <summary>
        /// Validates data before saving.
        /// </summary>
        private bool ValidateData()
        {
            foreach (var component in UIComponents)
            {
                if (!component.ValidateData(out string errorMessage))
                {
                    MessageBox.Show(errorMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Handles property changes in the UnitOfWork.
        /// </summary>
        private void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentItem")
            {
                UpdateDataContext();
                NotifyChildBlocksOfMasterChange();
            }
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
            foreach (var component in UIComponents)
            {
                component.DataContext = Data.CurrentItem;
                component.RefreshBinding(); // Ensure each component refreshes its bindings
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
            CreateControlsBasedonStructure();

            // Update data navigator's BindingSource
            if (dataNavigator != null && Data != null)
            {
                dataNavigator.BindingSource.DataSource = Data.Units;
                dataNavigator.BindingSource.Position = Data.Units.CurrentIndex;
                dataNavigator.UpdateRecordCountDisplay();
            }
        }

        // Support for multiple detail blocks
        /// <summary>
        /// Adds a child data block to the current block.
        /// </summary>
        /// <typeparam name="TChild">The type of the child entity.</typeparam>
        /// <param name="childBlock">The child data block instance.</param>
        /// <param name="masterKeyPropertyName">The property name of the master key in the master entity.</param>
        /// <param name="foreignKeyPropertyName">The property name of the foreign key in the child entity.</param>
        public void AddChildBlock<TChild>(BeepDataBlock<Entity> childBlock, string masterKeyPropertyName, string foreignKeyPropertyName)
            where TChild : Entity, INotifyPropertyChanged, new()
        {
            var childInfo = new ChildBlockInfo
            {
                ChildBlock = childBlock,
                MasterKeyPropertyName = masterKeyPropertyName,
                ForeignKeyPropertyName = foreignKeyPropertyName
            };

            _childBlocks.Add(childInfo);

            // Set initial master record for the child block
            var masterRecord = Data?.CurrentItem;
            childBlock.SetMasterRecord(masterRecord, masterKeyPropertyName, foreignKeyPropertyName);
        }

        /// <summary>
        /// Notifies all child blocks that the master record has changed.
        /// </summary>
        private void NotifyChildBlocksOfMasterChange()
        {
            var masterRecord = Data?.CurrentItem;
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
        public void SetMasterRecord(object masterRecord, string masterKeyPropertyName, string foreignKeyPropertyName)
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

                // Build filter string
                string filterString = $"{_foreignKeyPropertyName} = '{masterKeyValue}'";
                Data.Units.Filter = filterString;
            }

          //  Data.Units.RefreshBinding();
            Data.Units.MoveFirst();
        }

        /// <summary>
        /// Helper method to get property value using reflection.
        /// </summary>
        private object GetPropertyValue(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property == null)
                throw new InvalidOperationException($"Property '{propertyName}' not found on type '{obj.GetType().Name}'.");

            return property.GetValue(obj);
        }

        /// <summary>
        /// Helper method to set property value using reflection.
        /// </summary>
        private void SetPropertyValue(object obj, string propertyName, object value)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property == null)
                throw new InvalidOperationException($"Property '{propertyName}' not found on type '{obj.GetType().Name}'.");

            property.SetValue(obj, value);
        }

        /// <summary>
        /// Holds information about a child data block.
        /// </summary>
        private class ChildBlockInfo
        {
            public BeepDataBlock<Entity> ChildBlock { get; set; }
            public string MasterKeyPropertyName { get; set; }
            public string ForeignKeyPropertyName { get; set; }
        }
    }
}
