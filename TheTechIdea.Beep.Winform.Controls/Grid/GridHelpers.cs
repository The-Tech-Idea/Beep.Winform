
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    public static class GridHelpers
    {

        public static DataGridView dataGridViewToPrint;
        public static PrinterSettings myprintsettings { get; set; } = new PrinterSettings();
        public static PageSettings mypagesettings { get; set; } = new PageSettings();
        public static void PrintGrid(IDMEEditor dMEEditor, DataGridView dataGridView,string title,string subtitle,string footer,bool landscape=true)
        {
            dataGridViewToPrint = dataGridView;
      
            BeepPrinter printer =new BeepPrinter(dMEEditor);
            printer.Title = title;

            printer.SubTitle = subtitle;

            printer.SubTitleFormatFlags = StringFormatFlags.LineLimit |

                                          StringFormatFlags.NoClip;

            printer.PageNumbers = true;

            printer.PageNumberInHeader = false;

            printer.PorportionalColumns = true;

            printer.HeaderCellAlignment = StringAlignment.Near;

            printer.Footer = footer;

            printer.FooterSpacing = 15;

          
            // use saved settings

            if (null != myprintsettings)

                printer.printDocument.PrinterSettings = myprintsettings;

            if (null != mypagesettings)

                printer.printDocument.DefaultPageSettings = mypagesettings;



          //  if (DialogResult.OK == printer.DisplayPrintDialog())  // replace DisplayPrintDialog() 

            // with your own print dialog

            //{

                // save users' settings 

                myprintsettings = printer.PrintSettings;

                mypagesettings = printer.PageSettings;


            printer.printDocument.DefaultPageSettings.Landscape = landscape;
            // print without displaying the printdialog

            printer.PrintPreviewDataGridView(dataGridViewToPrint);

            //}
        }
        public static void ExportGridToCSV(DataGridView dataGridView1)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = "Output.csv";
                bool fileError = false;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(sfd.FileName))
                    {
                        try
                        {
                            File.Delete(sfd.FileName);
                        }
                        catch (IOException ex)
                        {
                            fileError = true;
                            MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                        }
                    }
                    if (!fileError)
                    {
                        try
                        {
                            int columnCount = dataGridView1.Columns.Count;
                            string columnNames = "";
                            string[] outputCsv = new string[dataGridView1.Rows.Count + 1];
                            for (int i = 0; i < columnCount; i++)
                            {
                                columnNames += dataGridView1.Columns[i].HeaderText.ToString() + ",";
                            }
                            outputCsv[0] += columnNames;

                            for (int i = 1; (i - 1) < dataGridView1.Rows.Count; i++)
                            {
                                for (int j = 0; j < columnCount; j++)
                                {
                                    string vl = dataGridView1.Rows[i - 1].Cells[j].Value == null ? " " : dataGridView1.Rows[i - 1].Cells[j].Value.ToString();
                                    outputCsv[i] +=vl + ",";
                                }
                            }

                            File.WriteAllLines(sfd.FileName, outputCsv, Encoding.UTF8);
                            MessageBox.Show("Data Exported Successfully !!!", "Info");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error :" + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No Record To Export !!!", "Info");
            }
        }
        public static void ApplyPropertiesToColumn(DataGridViewColumn column, DataGridViewColumnConfiguration config)
        {
            column.Name = config.Name;
            column.DisplayIndex = config.DisplayIndex;
            column.DataPropertyName = config.DataPropertyName;
            column.AutoSizeMode = config.AutoSizeMode;

            // Set DefaultCellStyle if available
            if (config.DefaultCellStyle != null)
            {
                column.DefaultCellStyle = new DataGridViewCellStyle(config.DefaultCellStyle);
            }

            column.HeaderText = config.HeaderText;

            // Set HeaderCell if available
            if (config.HeaderCell != null)
            {
                column.HeaderCell = (DataGridViewColumnHeaderCell)config.HeaderCell.Clone();
            }

            column.Visible = config.Visible;
            column.Width = config.Width;
            column.ReadOnly = config.ReadOnly;
            column.Resizable = config.Resizable;
            column.SortMode = config.SortMode; // Convert SortGlyphDirection

            // Handle other properties as needed
            column.CellTemplate = config.CellTemplate;
            column.ContextMenuStrip = config.ContextMenuStrip;
            column.DefaultCellStyle = config.DefaultCellStyle;
            column.DefaultHeaderCellType = config.DefaultHeaderCellType;
          
            column.DividerWidth = config.DividerWidth;
            column.FillWeight = config.FillWeight;
            column.Frozen = config.Frozen;
          
       
            column.MinimumWidth = config.MinimumWidth;
            column.ToolTipText = config.ToolTipText;
            column.ValueType = config.ValueType;

            // Add handling for any other properties you've included in DataGridViewColumnConfiguration
        }

    }
}