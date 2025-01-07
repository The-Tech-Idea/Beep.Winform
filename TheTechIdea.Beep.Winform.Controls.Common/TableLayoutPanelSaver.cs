using Newtonsoft.Json;


namespace TheTechIdea.Beep.Desktop.Common
{
    public static class TableLayoutPanelSaver
    {
        /// <summary>
        /// Loads row/column definitions and controls from a JSON file
        /// and populates the specified TableLayoutPanel accordingly.
        /// </summary>
        /// <param name="filePath">Path to the JSON layout file.</param>
        /// <param name="panel">Target TableLayoutPanel to apply the layout to.</param>
        public static void LoadTableLayoutPanelLayout(string filePath, TableLayoutPanel panel)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Layout file not found: {filePath}");
            }

            string json;
            try
            {
                json = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                // Optionally handle or log the exception, then rethrow
                throw new IOException($"Error reading layout file '{filePath}': {ex.Message}", ex);
            }

            // We'll expect the JSON structure like:
            // {
            //   "Version": 1,
            //   "Columns": [ { "SizeType": "Percent", "SizeValue": 50 }, ... ],
            //   "Rows":    [ { "SizeType": "Absolute", "SizeValue": 100 }, ... ],
            //   "Controls": [
            //     {
            //       "Type": "...AssemblyQualifiedName...",
            //       "Properties": { "Text": "some text", ... },
            //       "Row": 0,
            //       "Column": 1
            //     }
            //   ]
            // }

            dynamic layout = JsonConvert.DeserializeObject(json);

            // If we track a version, we can do:
            // int version = layout.Version != null ? (int)layout.Version : 1;

            // Clear existing row/column definitions
            panel.ColumnStyles.Clear();
            panel.RowStyles.Clear();

            // Convert dynamic arrays into enumerations
            var columns = layout.Columns;
            var rows = layout.Rows;

            // Set the panel's column/row counts to match the JSON arrays
            panel.ColumnCount = columns.Count;
            panel.RowCount = rows.Count;

            // Rebuild column styles
            foreach (var col in columns)
            {
                string sizeTypeString = (string)col.SizeType;
                SizeType sizeType = (SizeType)Enum.Parse(typeof(SizeType), sizeTypeString);
                float sizeValue = (float)col.SizeValue;  // e.g. 100.0, 50.0, etc.

                panel.ColumnStyles.Add(new ColumnStyle(sizeType, sizeValue));
            }

            // Rebuild row styles
            foreach (var row in rows)
            {
                string sizeTypeString = (string)row.SizeType;
                SizeType sizeType = (SizeType)Enum.Parse(typeof(SizeType), sizeTypeString);
                float sizeValue = (float)row.SizeValue;

                panel.RowStyles.Add(new RowStyle(sizeType, sizeValue));
            }

            // Clear existing controls
            panel.Controls.Clear();

            // Recreate controls from the JSON
            var controls = layout.Controls;
            foreach (var ctrlInfo in controls)
            {
                string assemblyQualifiedTypeName = (string)ctrlInfo.Type;
                int rowIndex = (int)ctrlInfo.Row;
                int colIndex = (int)ctrlInfo.Column;

                // Attempt to create the control
                Control control = CreateControlInstance(assemblyQualifiedTypeName);

                // If the JSON includes some property dictionary, apply them
                var props = ctrlInfo.Properties;
                if (props != null)
                {
                    // For example, if "Text" is present
                    // you can read it: control.Text = props.Text
                    if (props.Text != null)
                    {
                        control.Text = (string)props.Text;
                    }
                    // Extend for more properties as needed
                }

                // Add to the table
                // NOTE: consider bounds checking if rowIndex/colIndex might be out of range
                if (rowIndex >= 0 && rowIndex < panel.RowCount &&
                    colIndex >= 0 && colIndex < panel.ColumnCount)
                {
                    panel.Controls.Add(control, colIndex, rowIndex);
                }
                else
                {
                    // Optionally handle out-of-bounds issues, or skip
                }
            }
        }

        /// <summary>
        /// Saves the layout (row/column definitions and control information)
        /// of a TableLayoutPanel to a JSON file.
        /// </summary>
        /// <param name="panel">The TableLayoutPanel to read from.</param>
        /// <param name="filePath">Destination file path.</param>
        public static void SaveTableLayoutPanelLayout(TableLayoutPanel panel, string filePath)
        {
            // Build a layout object with versioning, row/col definitions, controls
            var layout = new
            {
                Version = 1, // Optional version property
                Rows = GetRowOrColumnStyles(panel.RowStyles),
                Columns = GetRowOrColumnStyles(panel.ColumnStyles),
                Controls = GetControlsInfo(panel)
            };

            string json = JsonConvert.SerializeObject(layout, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Helper method to convert a TableLayoutStyleCollection (RowStyles or ColumnStyles)
        /// into a list of objects capturing SizeType and dimension.
        /// </summary>
        private static List<dynamic> GetRowOrColumnStyles(TableLayoutStyleCollection styles)
        {
            var styleList = new List<dynamic>();

            // Each style is either a RowStyle or ColumnStyle
            foreach (dynamic style in styles)
            {
                // We unify naming: "SizeValue"
                // For columns, it's style.Width
                // For rows, it's style.Height
                // but either type can be read from the style's .Width or .Height property
                float sizeValue = 0f;

                if (style is ColumnStyle colStyle)
                {
                    sizeValue = colStyle.Width;
                }
                else if (style is RowStyle rowStyle)
                {
                    sizeValue = rowStyle.Height;
                }

                styleList.Add(new
                {
                    SizeType = style.SizeType.ToString(),
                    SizeValue = sizeValue
                });
            }
            return styleList;
        }

        /// <summary>
        /// Extracts relevant info from all controls in the panel for serialization.
        /// </summary>
        private static List<dynamic> GetControlsInfo(TableLayoutPanel panel)
        {
            var controlsInfo = new List<dynamic>();

            foreach (Control control in panel.Controls)
            {
                int row = panel.GetRow(control);
                int column = panel.GetColumn(control);

                controlsInfo.Add(new
                {
                    // Store the type info so we can re-instantiate at load time
                    Type = control.GetType().AssemblyQualifiedName,

                    // Example property serialization
                    Properties = new
                    {
                        control.Text
                        // Add more if you want, e.g. control.Name, 
                        // or color properties, etc.
                    },
                    Row = row,
                    Column = column
                });
            }
            return controlsInfo;
        }

        /// <summary>
        /// Attempts to create a Control instance from an assembly-qualified type name.
        /// If it fails, returns a fallback Label or you could throw an exception.
        /// </summary>
        private static Control CreateControlInstance(string assemblyQualifiedTypeName)
        {
            try
            {
                Type type = Type.GetType(assemblyQualifiedTypeName);
                if (type == null)
                {
                    // Could throw an exception or fallback
                    return new Label() { Text = $"Unknown type: {assemblyQualifiedTypeName}" };
                }
                return (Control)Activator.CreateInstance(type);
            }
            catch
            {
                // Fallback approach
                return new Label() { Text = $"Failed to create {assemblyQualifiedTypeName}" };
            }
        }
    }
}
