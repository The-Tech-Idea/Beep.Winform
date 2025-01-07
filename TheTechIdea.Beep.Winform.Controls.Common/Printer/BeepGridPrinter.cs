using System.Drawing.Printing;
using TheTechIdea.Beep.Logger;

namespace TheTechIdea.Beep.Desktop.Common.Printer
{
    /// <summary>
    /// Provides functionalities to print a DataGridView with custom formatting and pagination.
    /// </summary>
    public class BeepGridPrinter : IDisposable
    {
        #region Fields and Properties

        private readonly PrintDocument printDoc;
        private PrintPreviewDialog previewDialog;
        private DataGridView dgv;
        private readonly IDMLogger logger;

        // Logging
        private bool disposedValue = false;

        // Embedded Images
        public List<EmbeddedImage> EmbeddedImages { get; } = new List<EmbeddedImage>();

        // Page Settings
        public Margins PrintMargins
        {
            get => printDoc.DefaultPageSettings.Margins;
            set => printDoc.DefaultPageSettings.Margins = value;
        }

        public PageSettings PageSettings => printDoc.DefaultPageSettings;

        // Header and Footer Settings
        public bool PrintHeader { get; set; } = true;
        public bool PrintFooter { get; set; } = true;
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Footer { get; set; } = string.Empty;
        public bool ShowPageNumbers { get; set; } = true;
        public bool PageNumberInHeader { get; set; } = false;
        public string PageText { get; set; } = "Page ";
        public string PageSeparator { get; set; } = " of ";
        public string PartText { get; set; } = " Part ";
        public bool PageNumberOnSeparateLine { get; set; } = true;
        public bool ShowTotalPageNumbers { get; set; } = false;

        // Column and Row Settings
        public bool PrintColumnHeaders { get; set; } = true;
        public bool PrintRowHeaders { get; set; } = true;
        public bool KeepRowsTogether { get; set; } = true;
        public float KeepRowsTogetherTolerance { get; set; } = 2.0f; // Pixels tolerance

        // Column Width Overrides and Styles
        public Dictionary<string, float> ColumnWidths { get; } = new Dictionary<string, float>();
        public Dictionary<string, DataGridViewCellStyle> ColumnStyles { get; } = new Dictionary<string, DataGridViewCellStyle>();
        public Dictionary<string, DataGridViewCellStyle> AlternatingRowColumnStyles { get; } = new Dictionary<string, DataGridViewCellStyle>();
        public List<string> FixedColumns { get; } = new List<string>();
        public List<string> HideColumns { get; } = new List<string>();
        public string BreakOnValueChange { get; set; } = string.Empty;

        // Pagination
        private List<RowData> rowsToPrint = new List<RowData>();
        private List<DataGridViewColumn> columnsToPrint = new List<DataGridViewColumn>();
        private List<PageDef> pageSets = new List<PageDef>();
        private int currentPageSet = 0;
        private int lastRowPrinted = -1;
        private float rowStartLocation = 0;

        // Page Metrics
        private float pageHeight;
        private float pageWidth;
        private float printWidth;
        private float headerHeight;
        private float footerHeight;
        private float pageNumberHeight;
        private float titleHeight;
        private float subtitleHeight;
        private float columnHeaderHeight;
        private float rowHeaderWidth = 0;

        // Formatting
        private StringFormat columnHeaderFormat;
        private StringFormat rowHeaderFormat;
        private StringFormat cellFormat;

        // Preview Settings
        public double PrintPreviewZoom { get; set; } = 1.0;
        public Form Owner { get; set; }
        public Icon PreviewIcon { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the BeepPrinter class with the specified logger.
        /// </summary>
        /// <param name="logger">An instance of IDMLogger for logging.</param>
        public BeepGridPrinter(IDMLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            printDoc = new PrintDocument();
            printDoc.PrintPage += PrintPageEventHandler;
            printDoc.BeginPrint += BeginPrintEventHandler;

            // Set default margins
            PrintMargins = new Margins(60, 60, 40, 40);

            // Initialize default string formats
            InitializeStringFormats();

            logger.LogInfo("BeepPrinter initialized.");
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes default string formats for headers and cells.
        /// </summary>
        private void InitializeStringFormats()
        {
            columnHeaderFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.Word,
                FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip
            };

            rowHeaderFormat = new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.Word,
                FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip
            };

            cellFormat = new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.Word,
                FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the printing process and sends the DataGridView to the printer.
        /// </summary>
        /// <param name="dgv">The DataGridView to print.</param>
        public void PrintDataGridView(DataGridView dgv)
        {
            if (dgv == null) throw new ArgumentNullException(nameof(dgv));

            this.dgv = dgv;
            PreparePrint();

            using (PrintDialog printDialog = new PrintDialog())
            {
                printDialog.Document = printDoc;
                printDialog.AllowSomePages = true;
                printDialog.AllowCurrentPage = true;
                printDialog.AllowSelection = true;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    logger.LogInfo("User initiated print.");
                    printDoc.Print();
                }
                else
                {
                    logger.LogInfo("Print canceled by user.");
                }
            }
        }

        /// <summary>
        /// Starts the printing process and shows a print preview dialog.
        /// </summary>
        /// <param name="dgv">The DataGridView to preview and print.</param>
        public void PrintPreviewDataGridView(DataGridView dgv)
        {
            if (dgv == null) throw new ArgumentNullException(nameof(dgv));

            this.dgv = dgv;
            PreparePrint();

            if (previewDialog == null)
            {
                previewDialog = new PrintPreviewDialog
                {
                    Document = printDoc,
                    UseAntiAlias = true,
                    Owner = Owner,
                    PrintPreviewControl = { Zoom = PrintPreviewZoom },
                    Width = PreviewDisplayWidth(),
                    Height = PreviewDisplayHeight()
                };

                if (PreviewIcon != null)
                {
                    previewDialog.Icon = PreviewIcon;
                }

                logger.LogInfo("PrintPreviewDialog initialized.");
            }

            logger.LogInfo("Displaying print preview.");
            previewDialog.ShowDialog();
        }

        /// <summary>
        /// Configures the print setup without displaying any dialog.
        /// </summary>
        /// <param name="dgv">The DataGridView to print.</param>
        public void PrintNoDisplay(DataGridView dgv)
        {
            if (dgv == null) throw new ArgumentNullException(nameof(dgv));

            this.dgv = dgv;
            PreparePrint();
            printDoc.Print();
            logger.LogInfo("Print started without display.");
        }

        /// <summary>
        /// Configures the print preview without displaying the standard print dialog.
        /// </summary>
        /// <param name="dgv">The DataGridView to preview and print.</param>
        public void PrintPreviewNoDisplay(DataGridView dgv)
        {
            if (dgv == null) throw new ArgumentNullException(nameof(dgv));

            this.dgv = dgv;
            PreparePrint();

            if (previewDialog == null)
            {
                previewDialog = new PrintPreviewDialog
                {
                    Document = printDoc,
                    UseAntiAlias = true,
                    Owner = Owner,
                    PrintPreviewControl = { Zoom = PrintPreviewZoom },
                    Width = PreviewDisplayWidth(),
                    Height = PreviewDisplayHeight()
                };

                if (PreviewIcon != null)
                {
                    previewDialog.Icon = PreviewIcon;
                }

                logger.LogInfo("PrintPreviewDialog initialized.");
            }

            logger.LogInfo("Displaying print preview without print dialog.");
            previewDialog.ShowDialog();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Prepares the DataGridView for printing by determining rows and columns to print and setting up pagination.
        /// </summary>
        private void PreparePrint()
        {
            logger.LogInfo("Preparing print setup.");
            SetupPrintRange();
            MeasurePrintArea(Graphics.FromHwnd(nint.Zero));
            Paginate();
            logger.LogInfo("Print setup completed.");
        }

        /// <summary>
        /// Measures the print area and calculates necessary metrics.
        /// </summary>
        /// <param name="g">Graphics context for measurement.</param>
        private void MeasurePrintArea(Graphics g)
        {
            logger.LogInfo("Measuring print area.");
            // Implement measurement logic here...
            // This includes calculating column widths, row heights, and overall layout.
            // For brevity, the implementation details are omitted.
        }

        /// <summary>
        /// Sets up the range of rows and columns to print based on user selections and configurations.
        /// </summary>
        private void SetupPrintRange()
        {
            logger.LogInfo("Setting up print range.");

            // Example: Select all rows and columns if no specific selection is made.
            rowsToPrint = dgv.Rows.Cast<DataGridViewRow>()
                .Where(row => row.Visible && !row.IsNewRow)
                .Select(row => new RowData { Row = row, Height = row.Height })
                .ToList();

            columnsToPrint = dgv.Columns.Cast<DataGridViewColumn>()
                .Where(col => col.Visible && !HideColumns.Contains(col.Name))
                .OrderBy(col => col.DisplayIndex)
                .ToList();

            logger.LogInfo($"Total Rows to Print: {rowsToPrint.Count}");
            logger.LogInfo($"Total Columns to Print: {columnsToPrint.Count}");
        }

        /// <summary>
        /// Calculates the number of pages required based on the measured print area.
        /// </summary>
        private void Paginate()
        {
            logger.LogInfo("Calculating pagination.");
            // Implement pagination logic here...
            // This includes determining how many pages are needed based on the content and layout.
            // For brevity, the implementation details are omitted.
        }

        /// <summary>
        /// Calculates the display width for the print preview dialog.
        /// </summary>
        /// <returns>The width in pixels.</returns>
        private int PreviewDisplayWidth()
        {
            double displayWidth = printDoc.DefaultPageSettings.Bounds.Width + 3 * printDoc.DefaultPageSettings.HardMarginY;
            return (int)(displayWidth * PrintPreviewZoom);
        }

        /// <summary>
        /// Calculates the display height for the print preview dialog.
        /// </summary>
        /// <returns>The height in pixels.</returns>
        private int PreviewDisplayHeight()
        {
            double displayHeight = printDoc.DefaultPageSettings.Bounds.Height + 3 * printDoc.DefaultPageSettings.HardMarginX;
            return (int)(displayHeight * PrintPreviewZoom);
        }

        /// <summary>
        /// Handles the BeginPrint event to reset counters and prepare for a new print job.
        /// </summary>
        private void BeginPrintEventHandler(object sender, PrintEventArgs e)
        {
            logger.LogInfo("BeginPrint event triggered.");
            currentPageSet = 0;
            lastRowPrinted = -1;
            rowStartLocation = 0;
        }

        /// <summary>
        /// Handles the PrintPage event to render each page.
        /// </summary>
        private void PrintPageEventHandler(object sender, PrintPageEventArgs e)
        {
            logger.LogInfo("PrintPage event triggered.");
            e.HasMorePages = PrintPage(e.Graphics);
        }

        /// <summary>
        /// Renders a single page.
        /// </summary>
        /// <param name="g">Graphics context for rendering.</param>
        /// <returns>True if there are more pages to print; otherwise, false.</returns>
        private bool PrintPage(Graphics g)
        {
            // Implement the logic to render a single page.
            // This includes drawing headers, footers, column headers, and data rows.
            // For brevity, the implementation details are omitted.

            logger.LogInfo("Rendering a page.");
            // Example: Determine if more pages are needed
            bool hasMorePages = false;

            // TODO: Implement actual rendering logic.

            return hasMorePages;
        }

        #endregion

        #region IDisposable Support

        /// <summary>
        /// Releases unmanaged and optionally managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects)
                    printDoc.Dispose();
                    previewDialog?.Dispose();
                }

                // Free unmanaged resources (unmanaged objects) and override finalizer
                // Set large fields to null

                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by the BeepPrinter.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
