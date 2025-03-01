using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Grid;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design
{
    public class BeepGridColumnConfigCollectionEditor : CollectionEditor
    {
        public BeepGridColumnConfigCollectionEditor(Type type) : base(type) { }

        // Define the type of items in the collection
        protected override Type CreateCollectionItemType()
        {
            return typeof(BeepGridColumnConfig);
        }

        // Customize the creation of new instances
        protected override object CreateInstance(Type itemType)
        {
            var instance = (BeepGridColumnConfig)base.CreateInstance(itemType);
            // Use Context to get the current collection size
            if (Context?.Instance != null && Context.Instance is BeepSimpleGrid grid)
            {
                int count = grid.Columns.Count;
                instance.ColumnCaption = $"Column {count + 1}";
                instance.ColumnName = $"Column{count + 1}";
                instance.Index = count; // Auto-increment ColumnIndex
            }
            else
            {
                instance.ColumnCaption = "Column 1";
                instance.ColumnName = "Column1";
                instance.Index = 0;
            }
            instance.Width = 100;
            instance.Visible = true;
            instance.ColumnType = DbFieldCategory.String;
            instance.CellEditor = BeepGridColumnType.Text;
            return instance;
        }

        // Customize how items are displayed in the collection editor
        protected override string GetDisplayText(object value)
        {
            if (value is BeepGridColumnConfig config)
            {
                string display = $"{config.Index}: {config.ColumnCaption}";
                if (!string.IsNullOrEmpty(config.ColumnName) && config.ColumnName != config.ColumnCaption)
                {
                    display += $" ({config.ColumnName})";
                }
                return display;
            }
            return base.GetDisplayText(value);
        }

        // Customize the editor form title and layout
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm form = base.CreateCollectionForm();
            form.Text = "Beep Grid Column Editor";
            form.Size = new Size(600, 400); // Make the editor larger for better usability
            return form;
        }

        // Ensure changes are reflected in the grid
        protected override object SetItems(object editValue, object[] value)
        {
            var result = base.SetItems(editValue, value);
            if (editValue is List<BeepGridColumnConfig> columns)
            {
                // Reassign ColumnIndex based on order in the collection
                for (int i = 0; i < columns.Count; i++)
                {
                    columns[i].Index = i;
                }
            }
            return result;
        }
    }
}
