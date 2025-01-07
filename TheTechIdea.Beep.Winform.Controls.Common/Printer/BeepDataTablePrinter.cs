using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using TheTechIdea.Beep.Logger;

namespace TheTechIdea.Beep.Desktop.Common.Printer
{
    public class BeepDataTablePrinter : IDisposable
    {
        private readonly PrintDocument printDoc;
        private DataTable dataTable;
        private readonly IDMLogger logger;

        // Header and Footer Settings
        public string Title { get; set; }
        public Font TitleFont { get; set; } = new Font("Arial", 16, FontStyle.Bold);
        public Color TitleColor { get; set; } = Color.Black;

        public string Footer { get; set; }
        public Font FooterFont { get; set; } = new Font("Arial", 10, FontStyle.Italic);
        public Color FooterColor { get; set; } = Color.Gray;

        public bool ShowPageNumbers { get; set; } = true;
        public Font PageNumberFont { get; set; } = new Font("Arial", 8);
        public Color PageNumberColor { get; set; } = Color.Gray;

        private int currentPage;

        public BeepDataTablePrinter(IDMLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            printDoc = new PrintDocument();
            printDoc.PrintPage += PrintPage;
            printDoc.BeginPrint += BeginPrint;

            logger.LogInfo("BeepDataTablePrinter initialized.");
        }

        public void Print(DataTable table)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));

            dataTable = table;

            using (PrintDialog dialog = new PrintDialog { Document = printDoc })
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    printDoc.Print();
                }
            }
        }

        private void BeginPrint(object sender, PrintEventArgs e)
        {
            logger.LogInfo("Printing started.");
            currentPage = 1;
        }
        public void ShowPrintPreview(DataTable table)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));

            dataTable = table;

            using (PrintPreviewDialog previewDialog = new PrintPreviewDialog { Document = printDoc })
            {
                previewDialog.WindowState = FormWindowState.Maximized;
                previewDialog.ShowDialog();
            }
        }
        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Rectangle bounds = e.MarginBounds;
            float y = bounds.Top;

            // Draw Header (Title)
            if (!string.IsNullOrEmpty(Title))
            {
                using var titleBrush = new SolidBrush(TitleColor);
                graphics.DrawString(Title, TitleFont, titleBrush, bounds.Left, y);
                y += TitleFont.GetHeight(graphics) + 10;
            }

            // Draw DataTable Content
            if (dataTable != null)
            {
                DrawDataTable(graphics, bounds, ref y);
            }

            // Draw Footer
            if (!string.IsNullOrEmpty(Footer))
            {
                using var footerBrush = new SolidBrush(FooterColor);
                graphics.DrawString(Footer, FooterFont, footerBrush, bounds.Left, e.MarginBounds.Bottom - FooterFont.GetHeight(graphics));
            }

            // Draw Page Numbers
            if (ShowPageNumbers)
            {
                string pageNumberText = $"Page {currentPage}";
                using var pageBrush = new SolidBrush(PageNumberColor);
                graphics.DrawString(pageNumberText, PageNumberFont, pageBrush, bounds.Right - 50, e.MarginBounds.Bottom - PageNumberFont.GetHeight(graphics));
            }

            currentPage++;

            // Indicate whether there are more pages
            e.HasMorePages = false;
        }
        private void DrawDataTable(Graphics graphics, Rectangle bounds, ref float y)
        {
            float startX = bounds.Left;
            float columnWidth = bounds.Width / dataTable.Columns.Count;
            float rowHeight = new Font("Arial", 10).GetHeight(graphics);

            // Draw Column Headers
            foreach (DataColumn column in dataTable.Columns)
            {
                graphics.DrawString(column.ColumnName, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, startX, y);
                startX += columnWidth;
            }

            // Draw Horizontal Line Below Column Headers
            y += rowHeight; // Move to the next row position
            using (Pen pen = new Pen(Color.Black, 1))
            {
                graphics.DrawLine(pen, bounds.Left, y, bounds.Right, y);
            }

            y += 5; // Add spacing below the line

            // Draw Rows
            foreach (DataRow row in dataTable.Rows)
            {
                startX = bounds.Left;
                foreach (var item in row.ItemArray)
                {
                    graphics.DrawString(item.ToString(), new Font("Arial", 10), Brushes.Black, startX, y);
                    startX += columnWidth;
                }
                y += rowHeight;

                // Check for page overflow
                if (y + rowHeight > bounds.Bottom)
                {
                    y = bounds.Top; // Reset Y position
                    break;
                }
            }
        }

        public void Dispose()
        {
            printDoc?.Dispose();
        }
    }
}
