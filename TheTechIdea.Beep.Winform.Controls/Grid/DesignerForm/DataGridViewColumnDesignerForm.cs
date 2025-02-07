
using System.Data;
using System.Reflection;
using TheTechIdea.Beep.Winform.Controls.Grid.DataColumns;


namespace TheTechIdea.Beep.Winform.Controls.Grid.DesignerForm
{
    public partial class DataGridViewColumnDesignerForm : Form
    {
        private BeepGrid targetDataGridView;
        private int selectedcolumnindex;
        private DataGridViewColumn selectedcolumn;
        private bool IsChanged = false;
        public DataGridViewColumnDesignerForm(BeepGrid targetGrid)
        {
            InitializeComponent();
            targetDataGridView = targetGrid;
            listBox1.DisplayMember = "Name";
            ColumnTypecomboBox.DisplayMember = "Name";
            ToTypecomboBox.DisplayMember = "Name";
            //List<Type> types = new List<Type>();
            //types.AddRange(GetCustomeColumnTypes().ToList());
            //types.AddRange(GetAvailableColumnTypes());
            ToTypecomboBox.DataSource = GetAllColumnTypes();
            ColumnTypecomboBox.DataSource = GetAllColumnTypes();
           // ColumnTypecomboBox.SelectedIndexChanged += ColumnTypecomboBox_SelectedIndexChanged;
            PopulateColumnEditorGrid();
            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
            Changebutton.Click += Changebutton_Click;
            InitializeSaveLoadButtons();
        }

        private void Changebutton_Click(object? sender, EventArgs e)
        {
            selectedcolumnindex = listBox1.SelectedIndex;
            selectedcolumn = targetDataGridView.Columns[selectedcolumnindex];
            Type selectedColumnType = (Type)ToTypecomboBox.SelectedItem;

            if (selectedcolumn == null || selectedColumnType == null)
            {
                MessageBox.Show("Invalid column selection or type.");
                return;
            }

            // Create new column of the selected type
            DataGridViewColumn newColumn = (DataGridViewColumn)Activator.CreateInstance(selectedColumnType);

            // Copy essential properties from the existing column
            newColumn.HeaderText = selectedcolumn.HeaderText;
            newColumn.Width = selectedcolumn.Width;
            newColumn.Visible = selectedcolumn.Visible;
            newColumn.DataPropertyName = selectedcolumn.DataPropertyName;
            newColumn.Name = selectedcolumn.Name;

            // You can use reflection here to copy custom properties dynamically, as needed
            CopyCustomProperties(selectedcolumn, newColumn);

            // Replace the column in the DataGridView
            targetDataGridView.Columns.RemoveAt(selectedcolumnindex);
            targetDataGridView.Columns.Insert(selectedcolumnindex, newColumn);
            targetDataGridView.Refresh();
        }

        // Helper method to copy custom properties
        private void CopyCustomProperties(DataGridViewColumn sourceColumn, DataGridViewColumn targetColumn)
        {
            var properties = sourceColumn.GetType().GetProperties();
            foreach (var prop in properties)
            {
                // Copy properties that exist in both source and target columns
                if (targetColumn.GetType().GetProperty(prop.Name) != null)
                {
                    var value = prop.GetValue(sourceColumn);
                    prop.SetValue(targetColumn, value);
                }
            }
        }
        public DataGridViewColumnDesignerForm()
        {
            InitializeComponent();
          

        }
    



        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
         //   MessageBox.Config($"Before: Selected Index: {selectedcolumnindex}");
            // The rootnodeitems in listBox1 are DataGridViewColumn objects.
            DataGridViewColumn selectedColumn = listBox1.SelectedItem as DataGridViewColumn;

            if (selectedColumn == null)
            {
                MessageBox.Show("Column not found");
                return;
            }

            int columnIndex = targetDataGridView.Columns.IndexOf(selectedColumn);
            if (columnIndex == -1)
            {
                MessageBox.Show("Column not found");
                return;
            }
          
            // Rest of your code...
            selectedcolumn = targetDataGridView.Columns[columnIndex];
            // ...

            if (selectedcolumnindex < 0 || selectedcolumnindex >= targetDataGridView.Columns.Count)
            {
                MessageBox.Show("Handle the out-of-range index here");
                // Handle the out-of-range index here
                // e.g., display an error message or disable elements that require a valid selection
                return;
            }

           // selectedcolumn = targetDataGridView.GridViewColumns[selectedcolumnindex];
            propertyGrid1.SelectedObject = selectedcolumn;

            IsChanged = false; // Set the flag before changing the index

            // Make sure to handle if selectedcolumn.GetType() is null or not found in GetAllColumnTypes()
            int indexToSet = GetAllColumnTypes().IndexOf(selectedcolumn.GetType());
            if (indexToSet >= 0 && indexToSet < ColumnTypecomboBox.Items.Count)
            {
                ColumnTypecomboBox.SelectedIndex = indexToSet;
            }
            else
            {
                MessageBox.Show("Handle error, e.g., set ColumnTypecomboBox.SelectedIndex to -1 or display an error");
                // Handle error, e.g., set ColumnTypecomboBox.SelectedIndex to -1 or display an error
            }
          //  MessageBox.Config($"After: Selected Index: {selectedcolumnindex}");
            IsChanged = true; // Reset the flag after changing the index
        }


        private List<Type> GetAvailableColumnTypes()
        {
            Type baseType = typeof(DataGridViewColumn);

            return Assembly.GetAssembly(baseType)
                           .GetTypes()
                           .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t))
                           .ToList();

        }
        private List<Type> GetAllColumnTypes()
        {
            Type baseType = typeof(DataGridViewColumn);

            // Start with the dynamic list
            List<Type> types = Assembly.GetAssembly(baseType)
                                        .GetTypes()
                                        .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t) && t != baseType)
                                        .ToList();

            // Add custom types (this will only add the custom type if it's not already in the list)
            types.AddRange(GetCustomeColumnTypes().Except(types));

            return types;
        }

        private List<Type> GetCustomeColumnTypes()
        {

            return new List<Type>
            {
                //typeof(BeepDataGridViewSvgColumn),
               // typeof(BeepDataGridViewProgressBarColumn),
               // typeof(BeepListViewEditingControl),
                typeof(BeepDataGridViewNumericColumn)
                // Add more as needed
            };

        }
        private void PopulateColumnEditorGrid()
        {
            foreach (DataGridViewColumn column in targetDataGridView.Columns)
            {
                listBox1.Items.Add(column);
            }

        }
        private void InitializeSaveLoadButtons()
        {
            Button saveButton = new Button
            {
                Text = "Save Layout",
                Location = new System.Drawing.Point(10, 630)
            };
            saveButton.Click += (s, e) =>
            {
               
                    BeepGridPersistence.SaveGridLayout(targetDataGridView);

            };

            Button loadButton = new Button
            {
                Text = "Load Layout",
                Location = new System.Drawing.Point(110, 630)
            };
            loadButton.Click += (s, e) =>
            {
              
                    BeepGridPersistence.LoadGridLayout(targetDataGridView);
                    PopulateColumnEditorGrid(); // Refresh the UI after loading

            };

            this.Controls.Add(saveButton);
            this.Controls.Add(loadButton);
        }

    }
}
