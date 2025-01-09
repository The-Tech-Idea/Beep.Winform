
using System.Drawing.Printing;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Desktop.Common.Printer
{
    public class BeepContainerPrinter
    {
        private Control container;
        private PrintDocument printDocument;
        private string title;
        private float overallScale;

        public BeepContainerPrinter(Control container, string title = "Print Preview")
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));
            this.title = title;

            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;
            printDocument.BeginPrint += PrintDocument_BeginPrint;
        }

        private void PrintDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            // Calculate overall scale to fit the entire container within the printable area
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                float dpiX = g.DpiX;
                float dpiY = g.DpiY;

                float pageWidth = printDocument.DefaultPageSettings.PrintableArea.Width * dpiX / 100;
                float pageHeight = printDocument.DefaultPageSettings.PrintableArea.Height * dpiY / 100;

                overallScale = Math.Min(pageWidth / container.Width, pageHeight / container.Height);
            }
        }

        public void ShowPrintPreview()
        {
            using (PrintPreviewDialog previewDialog = new PrintPreviewDialog())
            {
                previewDialog.Document = printDocument;
                previewDialog.WindowState = FormWindowState.Maximized;
                previewDialog.ShowDialog();
            }
        }

        public void Print()
        {
            using (PrintDialog printDialog = new PrintDialog())
            {
                printDialog.Document = printDocument;
                if (printDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;

            // Calculate offsets for centering the content
            float offsetX = e.MarginBounds.Left;
            float offsetY = e.MarginBounds.Top;

            // Apply overall scaling
            g.ScaleTransform(overallScale, overallScale);

            // Translate graphics context to offset position
            g.TranslateTransform(offsetX / overallScale, offsetY / overallScale);

            // Draw each control with dynamic scaling
            foreach (Control child in container.Controls)
            {
                DrawControl(child, g, overallScale);
            }

            // Reset graphics transform
            g.ResetTransform();
        }

        private void DrawControl(Control control, Graphics g, float scale)
        {
            // Calculate bounds relative to container as Rectangle
            //Rectangle scaledrect = new Rectangle(
            //    (int)(control.Left * scale),
            //    (int)(control.Top * scale),
            //    (int)(control.Width * scale),
            //    (int)(control.Height * scale)
            //);

            Rectangle scaledBounds = new Rectangle(
                (int)(control.Left * scale),
                (int)(control.Top * scale),
                (int)(control.Width * scale),
                (int)(control.Height * scale)
            );


            // Draw the control background
            using (Brush backgroundBrush = new SolidBrush(control.BackColor))
            {
                g.FillRectangle(backgroundBrush, scaledBounds);
            }

            // Draw the control text (if applicable)
            if (control is Label || control is Button || control is CheckBox || control is RadioButton)
            {
                TextRenderer.DrawText(
                    g,
                    control.Text,
                    control.Font,
                    Rectangle.Round(scaledBounds),
                    control.ForeColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                );
            }

            // Draw the control image (if applicable)
            if (control is PictureBox pictureBox && pictureBox.Image != null)
            {
                g.DrawImage(pictureBox.Image, scaledBounds);
            }
            if(control is IBeepUIComponent)
            {
                IBeepUIComponent comp = (IBeepUIComponent)control;
                comp.Draw(g, scaledBounds);
            }
            // Handle custom rendering for grids
            if (control is DataGridView grid)
            {
                DrawDataGridView(grid, g, scale);
            }

            // Recursively draw child controls
            foreach (Control child in control.Controls)
            {
                DrawControl(child, g, scale);
            }
        }

        private void DrawDataGridView(DataGridView grid, Graphics g, float scale)
        {
            float yOffset = grid.ColumnHeadersHeight * scale;

            // Draw column headers
            foreach (DataGridViewColumn column in grid.Columns)
            {
                RectangleF headerBounds = new RectangleF(
                    grid.Left + column.DisplayIndex * column.Width * scale,
                    grid.Top * scale,
                    column.Width * scale,
                    grid.ColumnHeadersHeight * scale
                );

                TextRenderer.DrawText(
                    g,
                    column.HeaderText,
                    grid.Font,
                    Rectangle.Round(headerBounds),
                    grid.ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );
            }

            // Draw rows
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                DataGridViewRow row = grid.Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    RectangleF cellBounds = new RectangleF(
                        grid.Left + grid.Columns[j].DisplayIndex * grid.Columns[j].Width * scale,
                        (grid.Top + yOffset + i * grid.RowTemplate.Height) * scale,
                        grid.Columns[j].Width * scale,
                        grid.RowTemplate.Height * scale
                    );

                    TextRenderer.DrawText(
                        g,
                        row.Cells[j].Value?.ToString() ?? string.Empty,
                        grid.Font,
                        Rectangle.Round(cellBounds),
                        grid.ForeColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                    );
                }
            }
        }
    }

}
