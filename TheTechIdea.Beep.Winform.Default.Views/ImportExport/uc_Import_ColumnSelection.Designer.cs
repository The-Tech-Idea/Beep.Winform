using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Default.Views.ImportExport
{
    partial class uc_Import_ColumnSelection
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            splitMain        = new SplitContainer();
            previewGrid      = new DataGridView();
            lblPreviewStatus = new BeepLabel();
            colSelectionGrid = new DataGridView();
            colCheck         = new DataGridViewCheckBoxColumn();
            colName          = new DataGridViewTextBoxColumn();
            colType          = new DataGridViewTextBoxColumn();
            colSample        = new DataGridViewTextBoxColumn();
            toolbarPanel     = new Panel();
            btnSelectAll     = new BeepButton();
            btnSelectNone    = new BeepButton();
            btnRefreshPreview= new BeepButton();
            lblPreviewStatus2= new BeepLabel();

            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)previewGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)colSelectionGrid).BeginInit();
            toolbarPanel.SuspendLayout();
            SuspendLayout();

            //
            // toolbarPanel
            //
            toolbarPanel.Dock     = DockStyle.Top;
            toolbarPanel.Height   = 38;
            toolbarPanel.Name     = "toolbarPanel";
            toolbarPanel.Controls.Add(lblPreviewStatus2);
            toolbarPanel.Controls.Add(btnRefreshPreview);
            toolbarPanel.Controls.Add(btnSelectNone);
            toolbarPanel.Controls.Add(btnSelectAll);

            btnSelectAll.Location  = new System.Drawing.Point(6, 5);
            btnSelectAll.Size      = new System.Drawing.Size(90, 28);
            btnSelectAll.Name      = "btnSelectAll";
            btnSelectAll.Text      = "Select All";
            btnSelectAll.Theme     = "DefaultType";
            btnSelectAll.Click    += (_, _) => ToggleAll(true);

            btnSelectNone.Location = new System.Drawing.Point(102, 5);
            btnSelectNone.Size     = new System.Drawing.Size(90, 28);
            btnSelectNone.Name     = "btnSelectNone";
            btnSelectNone.Text     = "Select None";
            btnSelectNone.Theme    = "DefaultType";
            btnSelectNone.Click   += (_, _) => ToggleAll(false);

            btnRefreshPreview.Location = new System.Drawing.Point(198, 5);
            btnRefreshPreview.Size     = new System.Drawing.Size(100, 28);
            btnRefreshPreview.Name     = "btnRefreshPreview";
            btnRefreshPreview.Text     = "Refresh Preview";
            btnRefreshPreview.Theme    = "DefaultType";
            btnRefreshPreview.Click   += (_, _) => _ = LoadPreviewAsync();

            lblPreviewStatus2.AutoSize  = false;
            lblPreviewStatus2.Location  = new System.Drawing.Point(308, 8);
            lblPreviewStatus2.Size      = new System.Drawing.Size(320, 22);
            lblPreviewStatus2.Name      = "lblPreviewStatus2";
            lblPreviewStatus2.Text      = "";
            lblPreviewStatus2.ShowAllBorders = false;
            lblPreviewStatus2.Theme     = "DefaultType";

            //
            // splitMain
            //
            splitMain.Dock = DockStyle.Fill;
            splitMain.Name = "splitMain";
            splitMain.SplitterDistance = 340;  // preview on left (wider)

            // Panel1 = preview grid + status label at top
            splitMain.Panel1.Controls.Add(previewGrid);
            splitMain.Panel1.Controls.Add(lblPreviewStatus);

            // Panel2 = column selection grid
            splitMain.Panel2.Controls.Add(colSelectionGrid);

            //
            // lblPreviewStatus
            //
            lblPreviewStatus.Dock       = DockStyle.Top;
            lblPreviewStatus.Height     = 24;
            lblPreviewStatus.Name       = "lblPreviewStatus";
            lblPreviewStatus.Text       = "Preview (first 5 rows)";
            lblPreviewStatus.TextAlign  = System.Drawing.ContentAlignment.MiddleLeft;
            lblPreviewStatus.ShowAllBorders = false;
            lblPreviewStatus.Theme      = "DefaultType";

            //
            // previewGrid
            //
            previewGrid.AllowUserToAddRows    = false;
            previewGrid.AllowUserToDeleteRows = false;
            previewGrid.ReadOnly              = true;
            previewGrid.Dock                  = DockStyle.Fill;
            previewGrid.AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.AllCells;
            previewGrid.Name                  = "previewGrid";

            //
            // colSelectionGrid
            //
            colSelectionGrid.AllowUserToAddRows    = false;
            colSelectionGrid.AllowUserToDeleteRows = false;
            colSelectionGrid.AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill;
            colSelectionGrid.Dock                  = DockStyle.Fill;
            colSelectionGrid.Name                  = "colSelectionGrid";
            colSelectionGrid.Columns.AddRange(
                new DataGridViewColumn[] { colCheck, colName, colType, colSample });
            colSelectionGrid.CellValueChanged += ColGrid_CellValueChanged;
            colSelectionGrid.CurrentCellDirtyStateChanged += ColGrid_DirtyChanged;

            colCheck.HeaderText  = "";        colCheck.Name  = "colCheck";  colCheck.Width  = 30;
            colName.HeaderText   = "Column";  colName.Name   = "colName";   colName.ReadOnly= true;
            colType.HeaderText   = "Type";    colType.Name   = "colType";   colType.ReadOnly= true; colType.Width = 80;
            colSample.HeaderText = "Sample";  colSample.Name = "colSample"; colSample.ReadOnly= true;

            //
            // uc_Import_ColumnSelection
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode       = AutoScaleMode.Font;
            Controls.Add(splitMain);
            Controls.Add(toolbarPanel);
            Name = "uc_Import_ColumnSelection";
            Size = new System.Drawing.Size(680, 480);

            ((System.ComponentModel.ISupportInitialize)previewGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)colSelectionGrid).EndInit();
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            toolbarPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitMain;
        private DataGridView previewGrid;
        private DataGridView colSelectionGrid;
        private DataGridViewCheckBoxColumn colCheck;
        private DataGridViewTextBoxColumn  colName;
        private DataGridViewTextBoxColumn  colType;
        private DataGridViewTextBoxColumn  colSample;
        private BeepLabel   lblPreviewStatus;
        private Panel       toolbarPanel;
        private BeepButton  btnSelectAll;
        private BeepButton  btnSelectNone;
        private BeepButton  btnRefreshPreview;
        private BeepLabel   lblPreviewStatus2;
    }
}
