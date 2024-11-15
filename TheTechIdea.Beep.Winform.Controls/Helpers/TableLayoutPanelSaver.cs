using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class TableLayoutPanelSaver
    {
        public static void LoadTableLayoutPanelLayout(string filePath, TableLayoutPanel panel)
        {
            string json = File.ReadAllText(filePath);
            var layout = JsonConvert.DeserializeObject<dynamic>(json);

            panel.ColumnStyles.Clear();
            foreach (var col in layout.Columns)
            {
                panel.ColumnStyles.Add(new ColumnStyle((SizeType)Enum.Parse(typeof(SizeType), col.SizeType.ToString()), (float)col.Width));
            }

            panel.RowStyles.Clear();
            foreach (var row in layout.Rows)
            {
                panel.RowStyles.Add(new RowStyle((SizeType)Enum.Parse(typeof(SizeType), row.SizeType.ToString()), (float)row.Height));
            }

            panel.Controls.Clear();
            foreach (var ctrlInfo in layout.Controls)
            {
                Control control = (Control)Activator.CreateInstance(Type.GetType((string)ctrlInfo.Type));
                control.Text = ctrlInfo.Properties.Text;
                // Set other properties as needed
                panel.Controls.Add(control, (int)ctrlInfo.Column, (int)ctrlInfo.Row);
            }
        }
        public static void SaveTableLayoutPanelLayout(TableLayoutPanel panel, string filePath)
        {
            var layout = new
            {
                Rows = GetRowsOrColumns(panel.RowStyles),
                Columns = GetRowsOrColumns(panel.ColumnStyles),
                Controls = GetControlsInfo(panel)
            };

            string json = JsonConvert.SerializeObject(layout, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        private static List<dynamic> GetRowsOrColumns(TableLayoutStyleCollection styles)
        {
            List<dynamic> styleList = new List<dynamic>();
            foreach (dynamic style in styles)
            {
                styleList.Add(new
                {
                    SizeType = style.SizeType.ToString(),
                    WidthOrHeight = style.SizeType == SizeType.Absolute ? style.Width : style.SizeType == SizeType.Percent ? style.Height : (float?)null
                });
            }
            return styleList;
        }
        private static List<dynamic> GetControlsInfo(TableLayoutPanel panel)
        {
            List<dynamic> controlsInfo = new List<dynamic>();
            foreach (Control control in panel.Controls)
            {
                int row = panel.GetRow(control);
                int column = panel.GetColumn(control);
                controlsInfo.Add(new
                {
                    Type = control.GetType().AssemblyQualifiedName,
                    Properties = new
                    {
                        control.Text
                        // Add other properties as needed
                    },
                    Row = row,
                    Column = column
                });
            }
            return controlsInfo;
        }
    }
}
