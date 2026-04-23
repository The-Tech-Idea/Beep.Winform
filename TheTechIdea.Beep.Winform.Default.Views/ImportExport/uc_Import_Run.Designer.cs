using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;

namespace TheTechIdea.Beep.Winform.Default.Views.ImportExport
{
    partial class uc_Import_Run
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
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
            components          = new System.ComponentModel.Container();
            statusTimer         = new System.Windows.Forms.Timer(components);
            beepButton_Run      = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            beepButton_Pause    = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            beepButton_Resume   = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            beepButton_Cancel   = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            cbPreflight         = new System.Windows.Forms.CheckBox();
            cbAddMissing        = new System.Windows.Forms.CheckBox();
            cbSyncDraft         = new System.Windows.Forms.CheckBox();
            beepCheckBoxLastRun = new TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool();
            beepLogBox          = new System.Windows.Forms.RichTextBox();
            statusProgressBar   = new System.Windows.Forms.ProgressBar();
            statusLabelRows     = new System.Windows.Forms.Label();
            statusLabelElapsed  = new System.Windows.Forms.Label();
            lblThroughput       = new System.Windows.Forms.Label();
            lblBatchInfo        = new System.Windows.Forms.Label();
            buttonPanel         = new System.Windows.Forms.Panel();
            optionsPanel        = new System.Windows.Forms.Panel();
            statusPanel         = new System.Windows.Forms.Panel();
            // Summary card
            summaryCardPanel    = new System.Windows.Forms.Panel();
            lblSummaryCard      = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            btnToggleSummary    = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            // Log/Error split
            splitLogError       = new System.Windows.Forms.SplitContainer();
            errorGrid           = new System.Windows.Forms.DataGridView();
            colErrRow           = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colErrField         = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colErrValue         = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colErrMsg           = new System.Windows.Forms.DataGridViewTextBoxColumn();
            // Bottom action bar
            actionBar           = new System.Windows.Forms.Panel();
            btnExportErrors     = new TheTechIdea.Beep.Winform.Controls.BeepButton();

            buttonPanel.SuspendLayout();
            optionsPanel.SuspendLayout();
            statusPanel.SuspendLayout();
            summaryCardPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitLogError).BeginInit();
            splitLogError.Panel1.SuspendLayout();
            splitLogError.Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorGrid).BeginInit();
            actionBar.SuspendLayout();
            SuspendLayout();

            //
            // statusTimer
            //
            statusTimer.Interval = 500;

            //
            // buttonPanel
            //
            buttonPanel.Dock   = System.Windows.Forms.DockStyle.Top;
            buttonPanel.Height = 40;
            buttonPanel.Name   = "buttonPanel";
            buttonPanel.TabIndex = 0;
            buttonPanel.Controls.Add(beepButton_Cancel);
            buttonPanel.Controls.Add(beepButton_Resume);
            buttonPanel.Controls.Add(beepButton_Pause);
            buttonPanel.Controls.Add(beepButton_Run);

            //
            // beepButton_Run
            //
            beepButton_Run.Location  = new System.Drawing.Point(6, 6);
            beepButton_Run.Name      = "beepButton_Run";
            beepButton_Run.Size      = new System.Drawing.Size(110, 28);
            beepButton_Run.TabIndex  = 0;
            beepButton_Run.Text      = "Run Import";
            beepButton_Run.Theme     = "DefaultType";

            //
            // beepButton_Pause
            //
            beepButton_Pause.Location  = new System.Drawing.Point(122, 6);
            beepButton_Pause.Name      = "beepButton_Pause";
            beepButton_Pause.Size      = new System.Drawing.Size(90, 28);
            beepButton_Pause.TabIndex  = 1;
            beepButton_Pause.Text      = "Pause";
            beepButton_Pause.Theme     = "DefaultType";

            //
            // beepButton_Resume
            //
            beepButton_Resume.Location  = new System.Drawing.Point(218, 6);
            beepButton_Resume.Name      = "beepButton_Resume";
            beepButton_Resume.Size      = new System.Drawing.Size(90, 28);
            beepButton_Resume.TabIndex  = 2;
            beepButton_Resume.Text      = "Resume";
            beepButton_Resume.Theme     = "DefaultType";

            //
            // beepButton_Cancel
            //
            beepButton_Cancel.Location  = new System.Drawing.Point(314, 6);
            beepButton_Cancel.Name      = "beepButton_Cancel";
            beepButton_Cancel.Size      = new System.Drawing.Size(90, 28);
            beepButton_Cancel.TabIndex  = 3;
            beepButton_Cancel.Text      = "Cancel";
            beepButton_Cancel.Theme     = "DefaultType";

            //
            // optionsPanel
            //
            optionsPanel.Dock   = System.Windows.Forms.DockStyle.Top;
            optionsPanel.Height = 90;
            optionsPanel.Name   = "optionsPanel";
            optionsPanel.TabIndex = 1;
            optionsPanel.Controls.Add(beepCheckBoxLastRun);
            optionsPanel.Controls.Add(cbSyncDraft);
            optionsPanel.Controls.Add(cbAddMissing);
            optionsPanel.Controls.Add(cbPreflight);

            //
            // cbPreflight
            //
            cbPreflight.AutoSize  = true;
            cbPreflight.Location  = new System.Drawing.Point(12, 8);
            cbPreflight.Name      = "cbPreflight";
            cbPreflight.Size      = new System.Drawing.Size(180, 19);
            cbPreflight.TabIndex  = 0;
            cbPreflight.Text      = "Run Migration Preflight";

            //
            // cbAddMissing
            //
            cbAddMissing.AutoSize  = true;
            cbAddMissing.Checked   = true;
            cbAddMissing.CheckState = System.Windows.Forms.CheckState.Checked;
            cbAddMissing.Location  = new System.Drawing.Point(12, 33);
            cbAddMissing.Name      = "cbAddMissing";
            cbAddMissing.Size      = new System.Drawing.Size(160, 19);
            cbAddMissing.TabIndex  = 1;
            cbAddMissing.Text      = "Add Missing Columns";

            //
            // cbSyncDraft
            //
            cbSyncDraft.AutoSize  = true;
            cbSyncDraft.Location  = new System.Drawing.Point(12, 58);
            cbSyncDraft.Name      = "cbSyncDraft";
            cbSyncDraft.Size      = new System.Drawing.Size(190, 19);
            cbSyncDraft.TabIndex  = 2;
            cbSyncDraft.Text      = "Create Sync Profile Draft";

            //
            // beepCheckBoxLastRun
            //
            beepCheckBoxLastRun.AutoSize  = false;
            beepCheckBoxLastRun.Location  = new System.Drawing.Point(300, 30);
            beepCheckBoxLastRun.Name      = "beepCheckBoxLastRun";
            beepCheckBoxLastRun.Size      = new System.Drawing.Size(220, 23);
            beepCheckBoxLastRun.TabIndex  = 3;
            beepCheckBoxLastRun.Text      = "Import Ran Successfully";
            beepCheckBoxLastRun.IsReadOnly = true;
            beepCheckBoxLastRun.Theme     = "DefaultType";

            //
            // statusPanel
            //
            statusPanel.Dock   = System.Windows.Forms.DockStyle.Top;
            statusPanel.Height = 30;
            statusPanel.Name   = "statusPanel";
            statusPanel.TabIndex = 2;
            statusPanel.Controls.Add(statusLabelElapsed);
            statusPanel.Controls.Add(statusLabelRows);
            statusPanel.Controls.Add(statusProgressBar);

            //
            // statusProgressBar
            //
            statusProgressBar.Location  = new System.Drawing.Point(6, 6);
            statusProgressBar.Name      = "statusProgressBar";
            statusProgressBar.Size      = new System.Drawing.Size(380, 18);
            statusProgressBar.TabIndex  = 0;

            //
            // statusLabelRows
            //
            statusLabelRows.AutoSize = true;
            statusLabelRows.Location = new System.Drawing.Point(394, 7);
            statusLabelRows.Name     = "statusLabelRows";
            statusLabelRows.Size     = new System.Drawing.Size(100, 15);
            statusLabelRows.TabIndex = 1;
            statusLabelRows.Text     = "Rows: 0 / 0";

            //
            // statusLabelElapsed
            //
            statusLabelElapsed.AutoSize = true;
            statusLabelElapsed.Location = new System.Drawing.Point(500, 7);
            statusLabelElapsed.Name     = "statusLabelElapsed";
            statusLabelElapsed.Size     = new System.Drawing.Size(100, 15);
            statusLabelElapsed.TabIndex = 2;
            statusLabelElapsed.Text     = "Elapsed: 00:00:00";
            //
            // lblThroughput
            //
            lblThroughput.AutoSize = true;
            lblThroughput.Location = new System.Drawing.Point(610, 7);
            lblThroughput.Name     = "lblThroughput";
            lblThroughput.Size     = new System.Drawing.Size(80, 15);
            lblThroughput.TabIndex = 3;
            lblThroughput.Text     = "";
            statusPanel.Controls.Add(lblThroughput);
            //
            // lblBatchInfo
            //
            lblBatchInfo.AutoSize = true;
            lblBatchInfo.Location = new System.Drawing.Point(700, 7);
            lblBatchInfo.Name     = "lblBatchInfo";
            lblBatchInfo.Size     = new System.Drawing.Size(80, 15);
            lblBatchInfo.TabIndex = 4;
            lblBatchInfo.Text     = "";
            statusPanel.Controls.Add(lblBatchInfo);

            //
            // beepLogBox
            //
            beepLogBox.Dock      = System.Windows.Forms.DockStyle.Fill;
            beepLogBox.Name      = "beepLogBox";
            beepLogBox.ReadOnly  = true;
            beepLogBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            beepLogBox.TabIndex  = 3;
            //
            // splitLogError  (replaces the plain beepLogBox at the fill position)
            //
            splitLogError.Dock         = System.Windows.Forms.DockStyle.Fill;
            splitLogError.Name         = "splitLogError";
            splitLogError.Orientation  = System.Windows.Forms.Orientation.Vertical;
            splitLogError.SplitterDistance = 280;
            splitLogError.Panel1.Controls.Add(beepLogBox);
            splitLogError.Panel2.Controls.Add(errorGrid);
            //
            // errorGrid
            //
            errorGrid.AllowUserToAddRows         = false;
            errorGrid.AllowUserToDeleteRows      = false;
            errorGrid.AutoSizeColumnsMode        = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            errorGrid.ColumnHeadersHeightSizeMode= System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            errorGrid.Dock      = System.Windows.Forms.DockStyle.Fill;
            errorGrid.Name      = "errorGrid";
            errorGrid.ReadOnly  = true;
            errorGrid.TabIndex  = 0;
            errorGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { colErrRow, colErrField, colErrValue, colErrMsg });
            colErrRow.HeaderText   = "Row";     colErrRow.Name   = "colErrRow";   colErrRow.Width   = 50;
            colErrField.HeaderText = "Field";   colErrField.Name = "colErrField"; colErrField.Width = 100;
            colErrValue.HeaderText = "Value";   colErrValue.Name = "colErrValue"; colErrValue.Width = 100;
            colErrMsg.HeaderText   = "Message"; colErrMsg.Name   = "colErrMsg";
            //
            // summaryCardPanel
            //
            summaryCardPanel.Dock      = System.Windows.Forms.DockStyle.Top;
            summaryCardPanel.Height    = 70;
            summaryCardPanel.Name      = "summaryCardPanel";
            summaryCardPanel.TabIndex  = 10;
            summaryCardPanel.Controls.Add(btnToggleSummary);
            summaryCardPanel.Controls.Add(lblSummaryCard);
            //
            // lblSummaryCard
            //
            lblSummaryCard.AutoSize  = false;
            lblSummaryCard.Location  = new System.Drawing.Point(38, 6);
            lblSummaryCard.Size      = new System.Drawing.Size(590, 58);
            lblSummaryCard.Name      = "lblSummaryCard";
            lblSummaryCard.Text      = "(no run yet)";
            lblSummaryCard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblSummaryCard.ShowAllBorders = false;
            lblSummaryCard.Theme     = "DefaultType";
            //
            // btnToggleSummary
            //
            btnToggleSummary.Location = new System.Drawing.Point(6, 6);
            btnToggleSummary.Size     = new System.Drawing.Size(28, 28);
            btnToggleSummary.Name     = "btnToggleSummary";
            btnToggleSummary.Text     = "▼";
            btnToggleSummary.Theme    = "DefaultType";
            btnToggleSummary.TabIndex = 0;
            //
            // actionBar
            //
            actionBar.Dock    = System.Windows.Forms.DockStyle.Bottom;
            actionBar.Height  = 36;
            actionBar.Name    = "actionBar";
            actionBar.TabIndex= 11;
            actionBar.Controls.Add(btnExportErrors);
            //
            // btnExportErrors
            //
            btnExportErrors.Location = new System.Drawing.Point(6, 4);
            btnExportErrors.Size     = new System.Drawing.Size(150, 27);
            btnExportErrors.Name     = "btnExportErrors";
            btnExportErrors.Text     = "Export Errors to CSV";
            btnExportErrors.Theme    = "DefaultType";
            btnExportErrors.TabIndex = 0;
            btnExportErrors.Enabled  = false;

            //
            // uc_Import_Run
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(splitLogError);
            Controls.Add(actionBar);
            Controls.Add(statusPanel);
            Controls.Add(optionsPanel);
            Controls.Add(buttonPanel);
            Controls.Add(summaryCardPanel);
            Name = "uc_Import_Run";
            Size = new System.Drawing.Size(646, 530);

            buttonPanel.ResumeLayout(false);
            optionsPanel.ResumeLayout(false);
            optionsPanel.PerformLayout();
            statusPanel.ResumeLayout(false);
            statusPanel.PerformLayout();
            summaryCardPanel.ResumeLayout(false);
            splitLogError.Panel1.ResumeLayout(false);
            splitLogError.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitLogError).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorGrid).EndInit();
            actionBar.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer statusTimer;
        private BeepButton beepButton_Run;
        private BeepButton beepButton_Pause;
        private BeepButton beepButton_Resume;
        private BeepButton beepButton_Cancel;
        private System.Windows.Forms.CheckBox cbPreflight;
        private System.Windows.Forms.CheckBox cbAddMissing;
        private System.Windows.Forms.CheckBox cbSyncDraft;
        private BeepCheckBoxBool beepCheckBoxLastRun;
        private System.Windows.Forms.RichTextBox beepLogBox;
        private System.Windows.Forms.ProgressBar statusProgressBar;
        private System.Windows.Forms.Label statusLabelRows;
        private System.Windows.Forms.Label statusLabelElapsed;
        private System.Windows.Forms.Label lblThroughput;
        private System.Windows.Forms.Label lblBatchInfo;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Panel optionsPanel;
        private System.Windows.Forms.Panel statusPanel;
        // Summary card
        private System.Windows.Forms.Panel summaryCardPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel lblSummaryCard;
        private TheTechIdea.Beep.Winform.Controls.BeepButton btnToggleSummary;
        // Log/Error split
        private System.Windows.Forms.SplitContainer splitLogError;
        private System.Windows.Forms.DataGridView errorGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn colErrRow;
        private System.Windows.Forms.DataGridViewTextBoxColumn colErrField;
        private System.Windows.Forms.DataGridViewTextBoxColumn colErrValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colErrMsg;
        // Action bar
        private System.Windows.Forms.Panel actionBar;
        private TheTechIdea.Beep.Winform.Controls.BeepButton btnExportErrors;
    }
}
