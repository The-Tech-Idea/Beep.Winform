using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.IDE.Extensions.Controls
{
    /// <summary>
    /// Enhanced Properties control for Beep IDE Extensions
    /// Provides detailed property editing for various Beep components
    /// </summary>
    public partial class BeepPropertiesControl : UserControl
    {
        #region Fields
        private PropertyGrid propertyGrid;
        private ComboBox objectTypeComboBox;
        private Label objectTypeLabel;
        private Panel headerPanel;
        private TextBox searchTextBox;
        private Button refreshButton;
        private Label selectedObjectLabel;
        
        private object currentSelectedObject;
        private Type currentObjectType;
        private Dictionary<string, object> propertyHistory;
        #endregion

        #region Constructor
        public BeepPropertiesControl()
        {
            InitializeComponent();
            InitializeControls();
            SetupEventHandlers();
            
            propertyHistory = new Dictionary<string, object>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the currently selected object for property editing
        /// </summary>
        public object SelectedObject
        {
            get => currentSelectedObject;
            set
            {
                currentSelectedObject = value;
                UpdatePropertyGrid();
                UpdateObjectTypeDisplay();
                OnSelectedObjectChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Gets the type of the currently selected object
        /// </summary>
        public Type SelectedObjectType => currentObjectType;
        #endregion

        #region Initialization
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Dock = DockStyle.Fill;
            this.BackColor = SystemColors.Window;
            this.Size = new Size(300, 500);
            this.Name = "BeepPropertiesControl";
            
            this.ResumeLayout(false);
        }

        private void InitializeControls()
        {
            // Header panel
            headerPanel = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = SystemColors.Control,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Object type label
            objectTypeLabel = new Label()
            {
                Text = "Object Type:",
                Location = new Point(5, 5),
                Size = new Size(80, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Object type combo box
            objectTypeComboBox = new ComboBox()
            {
                Location = new Point(90, 5),
                Size = new Size(200, 21),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };

            // Selected object label
            selectedObjectLabel = new Label()
            {
                Location = new Point(5, 30),
                Size = new Size(285, 20),
                Text = "No object selected",
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = SystemColors.GrayText
            };

            // Search textbox
            searchTextBox = new TextBox()
            {
                Location = new Point(5, 55),
                Size = new Size(220, 20),
                PlaceholderText = "Search properties..."
            };

            // Refresh button
            refreshButton = new Button()
            {
                Location = new Point(230, 55),
                Size = new Size(60, 20),
                Text = "Refresh",
                UseVisualStyleBackColor = true
            };

            // Property grid
            propertyGrid = new PropertyGrid()
            {
                Dock = DockStyle.Fill,
                PropertySort = PropertySort.CategorizedAlphabetical,
                HelpVisible = true,
                ToolbarVisible = true,
                CommandsVisibleIfAvailable = true
            };

            // Add controls to header panel
            headerPanel.Controls.AddRange(new Control[] 
            {
                objectTypeLabel, objectTypeComboBox, selectedObjectLabel,
                searchTextBox, refreshButton
            });

            // Add controls to main control
            this.Controls.Add(propertyGrid);
            this.Controls.Add(headerPanel);

            // Initialize object type combo box
            PopulateObjectTypeComboBox();
        }

        private void SetupEventHandlers()
        {
            propertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
            propertyGrid.SelectedObjectsChanged += PropertyGrid_SelectedObjectsChanged;
            objectTypeComboBox.SelectedIndexChanged += ObjectTypeComboBox_SelectedIndexChanged;
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            refreshButton.Click += RefreshButton_Click;
        }

        private void PopulateObjectTypeComboBox()
        {
            var objectTypes = new List<string>
            {
                "None",
                "EntityStructure",
                "EntityField", 
                "ConnectionProperties",
                "BeepComponents",
                "BeepDataBlock",
                "Custom Object"
            };

            objectTypeComboBox.Items.AddRange(objectTypes.ToArray());
            objectTypeComboBox.SelectedIndex = 0;
        }
        #endregion

        #region Property Grid Management
        private void UpdatePropertyGrid()
        {
            try
            {
                if (currentSelectedObject != null)
                {
                    propertyGrid.SelectedObject = currentSelectedObject;
                    currentObjectType = currentSelectedObject.GetType();
                    
                    // Store in history
                    var objectKey = $"{currentObjectType.Name}_{DateTime.Now:HHmmss}";
                    if (!propertyHistory.ContainsKey(objectKey))
                    {
                        propertyHistory[objectKey] = currentSelectedObject;
                    }

                    // Limit history size
                    if (propertyHistory.Count > 10)
                    {
                        var oldestKey = propertyHistory.Keys.First();
                        propertyHistory.Remove(oldestKey);
                    }
                }
                else
                {
                    propertyGrid.SelectedObject = null;
                    currentObjectType = null;
                }

                UpdatePropertyGridDisplay();
            }
            catch (Exception ex)
            {
                ShowError($"Error updating property grid: {ex.Message}");
            }
        }

        private void UpdatePropertyGridDisplay()
        {
            // Customize property grid based on object type
            if (currentSelectedObject != null)
            {
                switch (currentSelectedObject)
                {
                    case EntityStructure entity:
                        ConfigureForEntityStructure(entity);
                        break;
                    case EntityField field:
                        ConfigureForEntityField(field);
                        break;
                    case ConnectionProperties connection:
                        ConfigureForConnectionProperties(connection);
                        break;
                    default:
                        ConfigureForGenericObject();
                        break;
                }
            }
        }

        private void ConfigureForEntityStructure(EntityStructure entity)
        {
            selectedObjectLabel.Text = $"Entity: {entity.EntityName}";
            selectedObjectLabel.ForeColor = Color.Blue;
            
            // Could add custom property descriptors here for better editing experience
        }

        private void ConfigureForEntityField(EntityField field)
        {
            selectedObjectLabel.Text = $"Field: {field.fieldname} ({field.fieldtype})";
            selectedObjectLabel.ForeColor = Color.Green;
        }

        private void ConfigureForConnectionProperties(ConnectionProperties connection)
        {
            selectedObjectLabel.Text = $"Connection: {connection.ConnectionName}";
            selectedObjectLabel.ForeColor = Color.Orange;
        }

        private void ConfigureForGenericObject()
        {
            selectedObjectLabel.Text = $"Object: {currentObjectType?.Name ?? "Unknown"}";
            selectedObjectLabel.ForeColor = SystemColors.ControlText;
        }

        private void UpdateObjectTypeDisplay()
        {
            if (currentObjectType != null)
            {
                var typeName = currentObjectType.Name;
                
                // Try to find matching item in combo box
                for (int i = 0; i < objectTypeComboBox.Items.Count; i++)
                {
                    if (objectTypeComboBox.Items[i].ToString().Contains(typeName))
                    {
                        objectTypeComboBox.SelectedIndex = i;
                        return;
                    }
                }
                
                // If not found, select "Custom Object"
                objectTypeComboBox.SelectedIndex = objectTypeComboBox.Items.Count - 1;
            }
            else
            {
                objectTypeComboBox.SelectedIndex = 0; // "None"
            }
        }
        #endregion

        #region Search and Filtering
        private void FilterProperties(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Show all properties
                propertyGrid.BrowsableAttributes = new AttributeCollection();
                return;
            }

            try
            {
                // This is a simplified search - in a full implementation,
                // you would create custom property descriptors to filter properties
                propertyGrid.Refresh();
            }
            catch (Exception ex)
            {
                ShowError($"Error filtering properties: {ex.Message}");
            }
        }
        #endregion

        #region Event Handlers
        private void PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            try
            {
                // Notify that a property has changed
                OnPropertyValueChanged?.Invoke(this, new PropertyChangedEventArgs(e.ChangedItem.PropertyDescriptor.Name));
                
                // Update display to reflect changes
                UpdatePropertyGridDisplay();
            }
            catch (Exception ex)
            {
                ShowError($"Error handling property value change: {ex.Message}");
            }
        }

        private void PropertyGrid_SelectedObjectsChanged(object sender, EventArgs e)
        {
            // Handle when the selected object in the property grid changes
            OnSelectedObjectChanged?.Invoke(this, propertyGrid.SelectedObject);
        }

        private void ObjectTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // This could be used to filter available objects by type
            // For now, it's just informational
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            // Implement property search/filtering
            FilterProperties(searchTextBox.Text);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            try
            {
                propertyGrid.Refresh();
                UpdatePropertyGridDisplay();
                OnRefreshRequested?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ShowError($"Error refreshing properties: {ex.Message}");
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the selected object and focuses on a specific property
        /// </summary>
        /// <param name="obj">The object to select</param>
        /// <param name="propertyName">The property to focus on</param>
        public void SetSelectedObjectAndProperty(object obj, string propertyName)
        {
            SelectedObject = obj;
            
            if (!string.IsNullOrEmpty(propertyName))
            {
                try
                {
                    // Find and select the specific property
                    var properties = TypeDescriptor.GetProperties(obj);
                    var property = properties.Find(propertyName, false);
                    
                    if (property != null)
                    {
                        // This would require custom implementation to select specific property
                        propertyGrid.Refresh();
                    }
                }
                catch (Exception ex)
                {
                    ShowError($"Error selecting property '{propertyName}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Gets the current property values as a dictionary
        /// </summary>
        /// <returns>Dictionary of property names and values</returns>
        public Dictionary<string, object> GetCurrentPropertyValues()
        {
            var values = new Dictionary<string, object>();
            
            if (currentSelectedObject != null)
            {
                try
                {
                    var properties = TypeDescriptor.GetProperties(currentSelectedObject);
                    foreach (PropertyDescriptor property in properties)
                    {
                        if (property.CanResetValue(currentSelectedObject))
                        {
                            values[property.Name] = property.GetValue(currentSelectedObject);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError($"Error getting property values: {ex.Message}");
                }
            }
            
            return values;
        }

        /// <summary>
        /// Applies property values from a dictionary
        /// </summary>
        /// <param name="values">Dictionary of property names and values</param>
        public void ApplyPropertyValues(Dictionary<string, object> values)
        {
            if (currentSelectedObject == null || values == null) return;

            try
            {
                var properties = TypeDescriptor.GetProperties(currentSelectedObject);
                
                foreach (var kvp in values)
                {
                    var property = properties.Find(kvp.Key, false);
                    if (property != null && !property.IsReadOnly)
                    {
                        property.SetValue(currentSelectedObject, kvp.Value);
                    }
                }
                
                propertyGrid.Refresh();
                UpdatePropertyGridDisplay();
            }
            catch (Exception ex)
            {
                ShowError($"Error applying property values: {ex.Message}");
            }
        }
        #endregion

        #region Helper Methods
        private void ShowError(string message)
        {
            MessageBox.Show(message, "Properties Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                propertyHistory?.Clear();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised when the selected object changes
        /// </summary>
        public event EventHandler<object> OnSelectedObjectChanged;

        /// <summary>
        /// Raised when a property value is changed
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> OnPropertyValueChanged;

        /// <summary>
        /// Raised when refresh is requested
        /// </summary>
        public event EventHandler OnRefreshRequested;
        #endregion
    }
