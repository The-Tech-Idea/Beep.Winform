using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns.CustomDataGridViewColumns;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    public class BeepGridPersistence
    {
        // Method to save the current grid layout to a file
        public static void SaveGridLayout(BeepGrid grid)
        {
          grid.SaveGridLayout();
        }

        // Method to load the grid layout from a file
        public static void LoadGridLayout(BeepGrid grid)
        {
            grid.LoadGridLayout();
        }

        // Helper method to extract custom properties from columns
        private static Dictionary<string, object> GetCustomColumnProperties(DataGridViewColumn column)
        {
            var properties = new Dictionary<string, object>();

            if (column is BeepDataGridViewComboBoxColumn comboBoxColumn)
            {
                properties["ParentColumn"] = comboBoxColumn.ParentColumn;
                properties["CascadingMap"] = comboBoxColumn.CascadingMap;
                properties["Query"] = comboBoxColumn.Query;
            }

            // Add similar cases for other custom column types

            return properties;
        }

        // Helper method to set custom properties for columns during deserialization
        private static void SetCustomColumnProperties(DataGridViewColumn column, Dictionary<string, object> properties)
        {
            if (column is BeepDataGridViewComboBoxColumn comboBoxColumn)
            {
                comboBoxColumn.ParentColumn = properties.ContainsKey("ParentColumn") ? (string)properties["ParentColumn"] : null;
                comboBoxColumn.CascadingMap = properties.ContainsKey("CascadingMap") ? (Dictionary<string, List<ColumnLookupList>>)properties["CascadingMap"] : null;
                comboBoxColumn.Query = properties.ContainsKey("Query") ? (string)properties["Query"] : null;
            }

            // Add similar cases for other custom column types
        }
    }

    // Class to hold the grid column configuration
    public class GridColumnConfig
    {
        public string ColumnType { get; set; }
        public string HeaderText { get; set; }
        public string DataPropertyName { get; set; }
        public bool Visible { get; set; }
        public int Width { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
}
