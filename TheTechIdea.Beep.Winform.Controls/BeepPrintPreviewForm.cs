using System.Drawing.Printing;


namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepPrintPreviewForm : BeepiForm
    {
        private ToolStrip toolStrip;
        private ToolStripButton btnPrev, btnNext, btnPrint, btnClose;
        private ToolStripComboBox zoomCombo;
        private ToolStripLabel pageLabel;
        private PrintPreviewControl previewControl;
        private PrintDocument printDocument;
        private int totalPages;

        public BeepPrintPreviewForm(PrintDocument document)
        {
            Text = "Print Preview";
            Width = 1000;
            Height = 800;

            printDocument = document;
            // 1) Pre-render to get total pages
            var oldController = printDocument.PrintController;
            var previewController = new PreviewPrintController();
            printDocument.PrintController = previewController;
            printDocument.Print(); // this runs PrintPage but only into memory
            var infos = previewController.GetPreviewPageInfo();
            totalPages = infos?.Length ?? 1;
            printDocument.PrintController = oldController; // restore

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            toolStrip = new ToolStrip();

            // Prev
            btnPrev = new ToolStripButton("←") { ToolTipText = "Previous Page" };
            btnPrev.Click += (s, e) =>
            {
                if (previewControl.StartPage > 0)
                {
                    previewControl.StartPage--;
                    UpdatePageLabel();
                }
            };
            toolStrip.Items.Add(btnPrev);

            // Page X of Y
            pageLabel = new ToolStripLabel();
            toolStrip.Items.Add(pageLabel);

            // Next
            btnNext = new ToolStripButton("→") { ToolTipText = "Next Page" };
            btnNext.Click += (s, e) =>
            {
                if (previewControl.StartPage < totalPages - 1)
                {
                    previewControl.StartPage++;
                    UpdatePageLabel();
                }
            };
            toolStrip.Items.Add(btnNext);

            toolStrip.Items.Add(new ToolStripSeparator());

            // Print
            btnPrint = new ToolStripButton("Print");
            btnPrint.Click += BtnPrint_Click;
            toolStrip.Items.Add(btnPrint);

            // Close
            btnClose = new ToolStripButton("Close");
            btnClose.Click += BtnClose_Click;
            toolStrip.Items.Add(btnClose);

            toolStrip.Items.Add(new ToolStripSeparator());

            // Zoom
            toolStrip.Items.Add(new ToolStripLabel("Zoom:"));
            zoomCombo = new ToolStripComboBox { Width = 80 };
            zoomCombo.Items.AddRange(new object[] { "50%", "100%", "150%", "200%", "Fit Width", "Fit Page" });
            zoomCombo.SelectedIndex = 1;
            zoomCombo.SelectedIndexChanged += ZoomCombo_SelectedIndexChanged;
            toolStrip.Items.Add(zoomCombo);

            toolStrip.Dock = DockStyle.Top;
            Controls.Add(toolStrip);

            // Preview control
            previewControl = new PrintPreviewControl
            {
                Document = printDocument,
                Dock = DockStyle.Fill,
                Zoom = 1.0
            };
            previewControl.StartPageChanged += (s, e) => UpdatePageLabel();
            Controls.Add(previewControl);

            UpdatePageLabel();
        }

        private void UpdatePageLabel()
        {
            int current = previewControl.StartPage + 1;
            pageLabel.Text = $"Page {current} of {totalPages}";
            btnPrev.Enabled = current > 1;
            btnNext.Enabled = current < totalPages;
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            using var dlg = new PrintDialog { Document = printDocument };
            if (dlg.ShowDialog(this) == DialogResult.OK)
                printDocument.Print();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ZoomCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            previewControl.AutoZoom = false;
            switch (zoomCombo.SelectedItem.ToString())
            {
                case "50%": previewControl.Zoom = 0.5; break;
                case "100%": previewControl.Zoom = 1.0; break;
                case "150%": previewControl.Zoom = 1.5; break;
                case "200%": previewControl.Zoom = 2.0; break;
                case "Fit Width":
                    previewControl.AutoZoom = true;
                    previewControl.AutoZoom = false;
                    previewControl.Zoom =
                        previewControl.ClientSize.Width /
                        previewControl.Document.DefaultPageSettings.PrintableArea.Width;
                    break;
                case "Fit Page":
                    previewControl.AutoZoom = true;
                    break;
            }
        }

        // Call this from your grid when you want to preview:
        public void ShowPreview()
        {
            // No 'using'—the form must stay alive until closed
            ShowDialog();
        }
    }
}
