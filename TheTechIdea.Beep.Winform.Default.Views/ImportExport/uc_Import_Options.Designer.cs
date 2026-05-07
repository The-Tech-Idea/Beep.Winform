using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;

namespace TheTechIdea.Beep.Winform.Default.Views.ImportExport
{
    partial class uc_Import_Options
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
            outerPanel       = new Panel();
            grpBatch         = new Panel();
            lblBatchSize     = new BeepLabel();
            numBatchSize     = new NumericUpDown();
            grpDryRun        = new Panel();
            chkDryRun        = new TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool();
            lblDryRunRows    = new BeepLabel();
            numDryRunRows    = new NumericUpDown();
            grpValidation    = new Panel();
            chkRunValidation = new TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool();
            chkSkipBlanks    = new TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool();
            chkPreflight     = new TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool();
            chkSyncDraft     = new TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool();
            lblPreflightStatus = new BeepLabel();
            grpPreflight     = new Panel();
            lblPreflightHdr  = new BeepLabel();
            txtPreflight     = new RichTextBox();
            grpQualityRules  = new Panel();
            qualityRulesGrid = new TheTechIdea.Beep.Winform.Controls.GridX.BeepGridPro();
            btnAddQualityRule    = new BeepButton();
            btnRemoveQualityRule = new BeepButton();
            btnClearQualityRules = new BeepButton();
            lblQualityRules      = new BeepLabel();

            outerPanel.SuspendLayout();
            grpBatch.SuspendLayout();
            grpDryRun.SuspendLayout();
            grpValidation.SuspendLayout();
            grpPreflight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numBatchSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDryRunRows).BeginInit();
            SuspendLayout();

            // ── outerPanel ───────────────────────────────────────────────────
            outerPanel.Dock    = DockStyle.Fill;
            outerPanel.Name    = "outerPanel";
            outerPanel.Padding = new Padding(12, 8, 12, 8);
            outerPanel.Controls.Add(grpPreflight);
            outerPanel.Controls.Add(grpQualityRules);
            outerPanel.Controls.Add(grpValidation);
            outerPanel.Controls.Add(grpDryRun);
            outerPanel.Controls.Add(grpBatch);

            // ── grpBatch — Batch Size ─────────────────────────────────────────
            grpBatch.Dock      = DockStyle.Top;
            grpBatch.Name      = "grpBatch";
            grpBatch.Height    = 60;
            grpBatch.Padding   = new Padding(8, 4, 8, 4);
            grpBatch.Controls.Add(numBatchSize);
            grpBatch.Controls.Add(lblBatchSize);

            lblBatchSize.AutoSize  = false;
            lblBatchSize.Location  = new System.Drawing.Point(8, 18);
            lblBatchSize.Size      = new System.Drawing.Size(160, 24);
            lblBatchSize.Name      = "lblBatchSize";
            lblBatchSize.Text      = "Batch size (rows per commit):";
            lblBatchSize.ShowAllBorders = false;
            lblBatchSize.Theme     = "DefaultType";
            lblBatchSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            numBatchSize.Location  = new System.Drawing.Point(174, 16);
            numBatchSize.Size      = new System.Drawing.Size(100, 28);
            numBatchSize.Name      = "numBatchSize";
            numBatchSize.Minimum   = 1;
            numBatchSize.Maximum   = 10000;
            numBatchSize.Value     = 50;
            numBatchSize.Increment = 50;

            // ── grpDryRun — Dry Run ───────────────────────────────────────────
            grpDryRun.Dock     = DockStyle.Top;
            grpDryRun.Name     = "grpDryRun";
            grpDryRun.Height   = 60;
            grpDryRun.Padding  = new Padding(8, 4, 8, 4);
            grpDryRun.Controls.Add(numDryRunRows);
            grpDryRun.Controls.Add(lblDryRunRows);
            grpDryRun.Controls.Add(chkDryRun);

            chkDryRun.AutoSize  = false;
            chkDryRun.Location  = new System.Drawing.Point(8, 18);
            chkDryRun.Size      = new System.Drawing.Size(140, 24);
            chkDryRun.Name      = "chkDryRun";
            chkDryRun.Text      = "Dry run (preview only)";
            chkDryRun.Theme     = "DefaultType";
            chkDryRun.Click    += ChkDryRun_Changed;

            lblDryRunRows.AutoSize  = false;
            lblDryRunRows.Location  = new System.Drawing.Point(158, 18);
            lblDryRunRows.Size      = new System.Drawing.Size(90, 24);
            lblDryRunRows.Name      = "lblDryRunRows";
            lblDryRunRows.Text      = "Max rows:";
            lblDryRunRows.ShowAllBorders = false;
            lblDryRunRows.Theme     = "DefaultType";
            lblDryRunRows.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblDryRunRows.Visible   = false;

            numDryRunRows.Location  = new System.Drawing.Point(252, 16);
            numDryRunRows.Size      = new System.Drawing.Size(80, 28);
            numDryRunRows.Name      = "numDryRunRows";
            numDryRunRows.Minimum   = 1;
            numDryRunRows.Maximum   = 10000;
            numDryRunRows.Value     = 100;
            numDryRunRows.Visible   = false;

            // ── grpValidation ────────────────────────────────────────────────
            grpValidation.Dock     = DockStyle.Top;
            grpValidation.Name     = "grpValidation";
            grpValidation.Height   = 140;
            grpValidation.Padding  = new Padding(8, 4, 8, 4);
            grpValidation.Controls.Add(lblPreflightStatus);
            grpValidation.Controls.Add(chkSyncDraft);
            grpValidation.Controls.Add(chkPreflight);
            grpValidation.Controls.Add(chkSkipBlanks);
            grpValidation.Controls.Add(chkRunValidation);

            chkRunValidation.AutoSize  = false;
            chkRunValidation.Location  = new System.Drawing.Point(8, 8);
            chkRunValidation.Size      = new System.Drawing.Size(280, 24);
            chkRunValidation.Name      = "chkRunValidation";
            chkRunValidation.Text      = "Validate data before importing";
            chkRunValidation.CurrentValue = true;
            chkRunValidation.Theme     = "DefaultType";

            chkSkipBlanks.AutoSize  = false;
            chkSkipBlanks.Location  = new System.Drawing.Point(8, 36);
            chkSkipBlanks.Size      = new System.Drawing.Size(280, 24);
            chkSkipBlanks.Name      = "chkSkipBlanks";
            chkSkipBlanks.Text      = "Skip empty values on update";
            chkSkipBlanks.Theme     = "DefaultType";

            chkPreflight.AutoSize  = false;
            chkPreflight.Location  = new System.Drawing.Point(8, 64);
            chkPreflight.Size      = new System.Drawing.Size(280, 24);
            chkPreflight.Name      = "chkPreflight";
            chkPreflight.Text      = "Run schema compatibility check";
            chkPreflight.Theme     = "DefaultType";
            chkPreflight.Click    += ChkPreflight_Changed;

            chkSyncDraft.AutoSize  = false;
            chkSyncDraft.Location  = new System.Drawing.Point(8, 92);
            chkSyncDraft.Size      = new System.Drawing.Size(280, 24);
            chkSyncDraft.Name      = "chkSyncDraft";
            chkSyncDraft.Text      = "Save sync profile draft";
            chkSyncDraft.Theme     = "DefaultType";

            lblPreflightStatus.AutoSize  = false;
            lblPreflightStatus.Location  = new System.Drawing.Point(296, 64);
            lblPreflightStatus.Size      = new System.Drawing.Size(280, 24);
            lblPreflightStatus.Name      = "lblPreflightStatus";
            lblPreflightStatus.Text      = string.Empty;
            lblPreflightStatus.ShowAllBorders = false;
            lblPreflightStatus.Theme     = "DefaultType";
            lblPreflightStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ── grpQualityRules ───────────────────────────────────────────────
            grpQualityRules.Dock     = DockStyle.Top;
            grpQualityRules.Name     = "grpQualityRules";
            grpQualityRules.Height   = 160;
            grpQualityRules.Padding  = new Padding(8, 4, 8, 4);
            grpQualityRules.Controls.Add(btnClearQualityRules);
            grpQualityRules.Controls.Add(btnRemoveQualityRule);
            grpQualityRules.Controls.Add(btnAddQualityRule);
            grpQualityRules.Controls.Add(qualityRulesGrid);
            grpQualityRules.Controls.Add(lblQualityRules);

            lblQualityRules.Dock        = DockStyle.Top;
            lblQualityRules.Height      = 22;
            lblQualityRules.Name        = "lblQualityRules";
            lblQualityRules.Text        = "Data Quality Rules";
            lblQualityRules.ShowAllBorders = false;
            lblQualityRules.Theme       = "DefaultType";
            lblQualityRules.TextAlign   = System.Drawing.ContentAlignment.MiddleLeft;

            qualityRulesGrid.Dock      = DockStyle.Top;
            qualityRulesGrid.Height    = 100;
            qualityRulesGrid.Name      = "qualityRulesGrid";
            qualityRulesGrid.Columns.Add(new TheTechIdea.Beep.Winform.Controls.Models.BeepColumnConfig { ColumnName = "Selected", ColumnCaption = "", Width = 30, IsSelectionCheckBox = true });
            qualityRulesGrid.Columns.Add(new TheTechIdea.Beep.Winform.Controls.Models.BeepColumnConfig { ColumnName = "RuleType", ColumnCaption = "Rule Type", Width = 100 });
            qualityRulesGrid.Columns.Add(new TheTechIdea.Beep.Winform.Controls.Models.BeepColumnConfig { ColumnName = "FieldName", ColumnCaption = "Field", Width = 100 });
            qualityRulesGrid.Columns.Add(new TheTechIdea.Beep.Winform.Controls.Models.BeepColumnConfig { ColumnName = "Parameters", ColumnCaption = "Parameters", Width = 120 });
            qualityRulesGrid.Columns.Add(new TheTechIdea.Beep.Winform.Controls.Models.BeepColumnConfig { ColumnName = "OnFailure", ColumnCaption = "On Failure", Width = 80 });

            btnAddQualityRule.Text = "Add";
            btnAddQualityRule.Location = new System.Drawing.Point(8, 130);
            btnAddQualityRule.Size = new System.Drawing.Size(60, 24);
            btnAddQualityRule.Name = "btnAddQualityRule";

            btnRemoveQualityRule.Text = "Remove";
            btnRemoveQualityRule.Location = new System.Drawing.Point(76, 130);
            btnRemoveQualityRule.Size = new System.Drawing.Size(60, 24);
            btnRemoveQualityRule.Name = "btnRemoveQualityRule";

            btnClearQualityRules.Text = "Clear";
            btnClearQualityRules.Location = new System.Drawing.Point(144, 130);
            btnClearQualityRules.Size = new System.Drawing.Size(60, 24);
            btnClearQualityRules.Name = "btnClearQualityRules";

            // ── grpPreflight — Summary ────────────────────────────────────────
            grpPreflight.Dock     = DockStyle.Fill;
            grpPreflight.Name     = "grpPreflight";
            grpPreflight.Padding  = new Padding(8, 4, 8, 4);
            grpPreflight.Controls.Add(txtPreflight);
            grpPreflight.Controls.Add(lblPreflightHdr);

            lblPreflightHdr.Dock        = DockStyle.Top;
            lblPreflightHdr.Height      = 26;
            lblPreflightHdr.Name        = "lblPreflightHdr";
            lblPreflightHdr.Text        = "Pre-flight summary";
            lblPreflightHdr.ShowAllBorders = false;
            lblPreflightHdr.Theme       = "DefaultType";
            lblPreflightHdr.TextAlign   = System.Drawing.ContentAlignment.MiddleLeft;

            txtPreflight.Dock      = DockStyle.Fill;
            txtPreflight.Name      = "txtPreflight";
            txtPreflight.ReadOnly  = true;
            txtPreflight.BorderStyle = BorderStyle.None;
            txtPreflight.BackColor = System.Drawing.SystemColors.Control;
            txtPreflight.Font      = new System.Drawing.Font("Segoe UI", 9F);

            // ── uc_Import_Options ─────────────────────────────────────────────
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode       = AutoScaleMode.Font;
            Controls.Add(outerPanel);
            Name = "uc_Import_Options";
            Size = new System.Drawing.Size(640, 420);

            ((System.ComponentModel.ISupportInitialize)numBatchSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDryRunRows).EndInit();
            grpBatch.ResumeLayout(false);
            grpDryRun.ResumeLayout(false);
            grpValidation.ResumeLayout(false);
            grpPreflight.ResumeLayout(false);
            outerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel            outerPanel;
        private Panel            grpBatch;
        private BeepLabel        lblBatchSize;
        private NumericUpDown    numBatchSize;
        private Panel            grpDryRun;
        private BeepCheckBoxBool     chkDryRun;
        private BeepLabel        lblDryRunRows;
        private NumericUpDown    numDryRunRows;
        private Panel            grpValidation;
        private BeepCheckBoxBool     chkRunValidation;
        private BeepCheckBoxBool     chkSkipBlanks;
        private BeepCheckBoxBool     chkPreflight;
        private BeepCheckBoxBool     chkSyncDraft;
        private BeepLabel        lblPreflightStatus;
        private Panel            grpPreflight;
        private BeepLabel        lblPreflightHdr;
        private RichTextBox      txtPreflight;
        private Panel            grpQualityRules;
        private TheTechIdea.Beep.Winform.Controls.GridX.BeepGridPro qualityRulesGrid;
        private BeepButton       btnAddQualityRule;
        private BeepButton       btnRemoveQualityRule;
        private BeepButton       btnClearQualityRules;
        private BeepLabel        lblQualityRules;
    }
}
