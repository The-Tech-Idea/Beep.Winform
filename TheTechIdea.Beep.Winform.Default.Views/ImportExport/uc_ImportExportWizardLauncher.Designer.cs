using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.GridX;

namespace TheTechIdea.Beep.Winform.Default.Views.ImportExport
{
    partial class uc_ImportExportWizardLauncher
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // ── Control instantiation ──────────────────────────────────────────
            directionPanel       = new System.Windows.Forms.Panel();
            lblMode              = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            cmbDirection         = new TheTechIdea.Beep.Winform.Controls.BeepComboBox();

            sourcePanel          = new System.Windows.Forms.Panel();
            lblSourceDS          = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            cmbSourceDS          = new TheTechIdea.Beep.Winform.Controls.BeepComboBox();
            lblSourceEntity      = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            cmbSourceEntity      = new TheTechIdea.Beep.Winform.Controls.BeepComboBox();

            destPanel            = new System.Windows.Forms.Panel();
            lblDestDS            = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            cmbDestDS            = new TheTechIdea.Beep.Winform.Controls.BeepComboBox();
            lblDestEntity        = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            cmbDestEntity        = new TheTechIdea.Beep.Winform.Controls.BeepComboBox();

            optionsPanel         = new System.Windows.Forms.Panel();
            chkCreateIfNotExists = new TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool();
            chkAddMissing        = new TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool();

            templatePanel        = new System.Windows.Forms.Panel();
            lblRecentTemplates   = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            cmbRecentTemplates   = new TheTechIdea.Beep.Winform.Controls.BeepComboBox();
            btnQuickImport       = new TheTechIdea.Beep.Winform.Controls.BeepButton();

            buttonPanel          = new System.Windows.Forms.Panel();
            btnLaunch            = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            btnSwap              = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            btnClearLog          = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            btnViewLastSummary   = new TheTechIdea.Beep.Winform.Controls.BeepButton();

            historyPanel         = new System.Windows.Forms.Panel();
            historyGrid          = new BeepGridPro();

            txtLog               = new System.Windows.Forms.RichTextBox();

            directionPanel.SuspendLayout();
            sourcePanel.SuspendLayout();
            destPanel.SuspendLayout();
            optionsPanel.SuspendLayout();
            templatePanel.SuspendLayout();
            buttonPanel.SuspendLayout();
            historyPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)historyGrid).BeginInit();
            SuspendLayout();

            // ── directionPanel ─────────────────────────────────────────────────
            directionPanel.Dock     = System.Windows.Forms.DockStyle.Top;
            directionPanel.Height   = 36;
            directionPanel.Name     = "directionPanel";
            directionPanel.Padding  = new System.Windows.Forms.Padding(4, 4, 4, 0);
            directionPanel.TabIndex = 0;
            directionPanel.Controls.Add(cmbDirection);
            directionPanel.Controls.Add(lblMode);

            lblMode.AutoSize  = false;
            lblMode.Location  = new System.Drawing.Point(6, 8);
            lblMode.Name      = "lblMode";
            lblMode.Size      = new System.Drawing.Size(70, 23);
            lblMode.TabIndex  = 0;
            lblMode.Text      = "Mode:";
            lblMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblMode.ShowAllBorders = false;
            lblMode.Theme     = "DefaultType";

            cmbDirection.Location  = new System.Drawing.Point(82, 5);
            cmbDirection.Name      = "cmbDirection";
            cmbDirection.Size      = new System.Drawing.Size(130, 27);
            cmbDirection.TabIndex  = 1;
            cmbDirection.Theme     = "DefaultType";

            // ── sourcePanel ────────────────────────────────────────────────────
            sourcePanel.Dock     = System.Windows.Forms.DockStyle.Top;
            sourcePanel.Height   = 36;
            sourcePanel.Name     = "sourcePanel";
            sourcePanel.Padding  = new System.Windows.Forms.Padding(4, 4, 4, 0);
            sourcePanel.TabIndex = 1;
            sourcePanel.Controls.Add(cmbSourceEntity);
            sourcePanel.Controls.Add(lblSourceEntity);
            sourcePanel.Controls.Add(cmbSourceDS);
            sourcePanel.Controls.Add(lblSourceDS);

            lblSourceDS.AutoSize  = false;
            lblSourceDS.Location  = new System.Drawing.Point(6, 8);
            lblSourceDS.Name      = "lblSourceDS";
            lblSourceDS.Size      = new System.Drawing.Size(80, 23);
            lblSourceDS.TabIndex  = 0;
            lblSourceDS.Text      = "Source:";
            lblSourceDS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblSourceDS.ShowAllBorders = false;
            lblSourceDS.Theme     = "DefaultType";

            cmbSourceDS.Anchor    = System.Windows.Forms.AnchorStyles.None;
            cmbSourceDS.Location  = new System.Drawing.Point(90, 5);
            cmbSourceDS.Name      = "cmbSourceDS";
            cmbSourceDS.Size      = new System.Drawing.Size(190, 27);
            cmbSourceDS.TabIndex  = 1;
            cmbSourceDS.Theme     = "DefaultType";

            lblSourceEntity.AutoSize  = false;
            lblSourceEntity.Location  = new System.Drawing.Point(288, 8);
            lblSourceEntity.Name      = "lblSourceEntity";
            lblSourceEntity.Size      = new System.Drawing.Size(60, 23);
            lblSourceEntity.TabIndex  = 2;
            lblSourceEntity.Text      = "Entity:";
            lblSourceEntity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblSourceEntity.ShowAllBorders = false;
            lblSourceEntity.Theme     = "DefaultType";

            cmbSourceEntity.Anchor    = System.Windows.Forms.AnchorStyles.None;
            cmbSourceEntity.Location  = new System.Drawing.Point(352, 5);
            cmbSourceEntity.Name      = "cmbSourceEntity";
            cmbSourceEntity.Size      = new System.Drawing.Size(190, 27);
            cmbSourceEntity.TabIndex  = 3;
            cmbSourceEntity.Theme     = "DefaultType";

            // ── destPanel ──────────────────────────────────────────────────────
            destPanel.Dock     = System.Windows.Forms.DockStyle.Top;
            destPanel.Height   = 36;
            destPanel.Name     = "destPanel";
            destPanel.Padding  = new System.Windows.Forms.Padding(4, 4, 4, 0);
            destPanel.TabIndex = 2;
            destPanel.Controls.Add(cmbDestEntity);
            destPanel.Controls.Add(lblDestEntity);
            destPanel.Controls.Add(cmbDestDS);
            destPanel.Controls.Add(lblDestDS);

            lblDestDS.AutoSize  = false;
            lblDestDS.Location  = new System.Drawing.Point(6, 8);
            lblDestDS.Name      = "lblDestDS";
            lblDestDS.Size      = new System.Drawing.Size(80, 23);
            lblDestDS.TabIndex  = 0;
            lblDestDS.Text      = "Destination:";
            lblDestDS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblDestDS.ShowAllBorders = false;
            lblDestDS.Theme     = "DefaultType";

            cmbDestDS.Anchor    = System.Windows.Forms.AnchorStyles.None;
            cmbDestDS.Location  = new System.Drawing.Point(90, 5);
            cmbDestDS.Name      = "cmbDestDS";
            cmbDestDS.Size      = new System.Drawing.Size(190, 27);
            cmbDestDS.TabIndex  = 1;
            cmbDestDS.Theme     = "DefaultType";

            lblDestEntity.AutoSize  = false;
            lblDestEntity.Location  = new System.Drawing.Point(288, 8);
            lblDestEntity.Name      = "lblDestEntity";
            lblDestEntity.Size      = new System.Drawing.Size(60, 23);
            lblDestEntity.TabIndex  = 2;
            lblDestEntity.Text      = "Entity:";
            lblDestEntity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblDestEntity.ShowAllBorders = false;
            lblDestEntity.Theme     = "DefaultType";

            cmbDestEntity.Anchor    = System.Windows.Forms.AnchorStyles.None;
            cmbDestEntity.Location  = new System.Drawing.Point(352, 5);
            cmbDestEntity.Name      = "cmbDestEntity";
            cmbDestEntity.Size      = new System.Drawing.Size(190, 27);
            cmbDestEntity.TabIndex  = 3;
            cmbDestEntity.Theme     = "DefaultType";

            // ── optionsPanel ───────────────────────────────────────────────────
            optionsPanel.Dock     = System.Windows.Forms.DockStyle.Top;
            optionsPanel.Height   = 32;
            optionsPanel.Name     = "optionsPanel";
            optionsPanel.Padding  = new System.Windows.Forms.Padding(4, 4, 4, 0);
            optionsPanel.TabIndex = 3;
            optionsPanel.Controls.Add(chkAddMissing);
            optionsPanel.Controls.Add(chkCreateIfNotExists);

            chkCreateIfNotExists.AutoSize  = false;
            chkCreateIfNotExists.Location  = new System.Drawing.Point(6, 5);
            chkCreateIfNotExists.Name      = "chkCreateIfNotExists";
            chkCreateIfNotExists.Size      = new System.Drawing.Size(210, 23);
            chkCreateIfNotExists.TabIndex  = 0;
            chkCreateIfNotExists.Text      = "Create destination if not exists";
            chkCreateIfNotExists.CurrentValue = true;
            chkCreateIfNotExists.Theme     = "DefaultType";

            chkAddMissing.AutoSize  = false;
            chkAddMissing.Location  = new System.Drawing.Point(224, 5);
            chkAddMissing.Name      = "chkAddMissing";
            chkAddMissing.Size      = new System.Drawing.Size(175, 23);
            chkAddMissing.TabIndex  = 1;
            chkAddMissing.Text      = "Add missing columns";
            chkAddMissing.CurrentValue = true;
            chkAddMissing.Theme     = "DefaultType";

            // ── templatePanel ─────────────────────────────────────────────────
            templatePanel.Dock     = System.Windows.Forms.DockStyle.Top;
            templatePanel.Height   = 36;
            templatePanel.Name     = "templatePanel";
            templatePanel.Padding  = new System.Windows.Forms.Padding(4, 4, 4, 0);
            templatePanel.TabIndex = 4;
            templatePanel.Controls.Add(btnQuickImport);
            templatePanel.Controls.Add(cmbRecentTemplates);
            templatePanel.Controls.Add(lblRecentTemplates);

            lblRecentTemplates.AutoSize  = false;
            lblRecentTemplates.Location  = new System.Drawing.Point(6, 8);
            lblRecentTemplates.Name      = "lblRecentTemplates";
            lblRecentTemplates.Size      = new System.Drawing.Size(88, 23);
            lblRecentTemplates.Text      = "Template:";
            lblRecentTemplates.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblRecentTemplates.ShowAllBorders = false;
            lblRecentTemplates.Theme     = "DefaultType";

            cmbRecentTemplates.Location  = new System.Drawing.Point(98, 5);
            cmbRecentTemplates.Name      = "cmbRecentTemplates";
            cmbRecentTemplates.Size      = new System.Drawing.Size(220, 27);
            cmbRecentTemplates.TabIndex  = 0;
            cmbRecentTemplates.Theme     = "DefaultType";

            btnQuickImport.Location  = new System.Drawing.Point(326, 5);
            btnQuickImport.Name      = "btnQuickImport";
            btnQuickImport.Size      = new System.Drawing.Size(120, 27);
            btnQuickImport.TabIndex  = 1;
            btnQuickImport.Text      = "⚡ Quick Import";
            btnQuickImport.Theme     = "DefaultType";

            // ── buttonPanel ────────────────────────────────────────────────────
            buttonPanel.Dock     = System.Windows.Forms.DockStyle.Top;
            buttonPanel.Height   = 42;
            buttonPanel.Name     = "buttonPanel";
            buttonPanel.Padding  = new System.Windows.Forms.Padding(4, 6, 4, 0);
            buttonPanel.TabIndex = 4;
            buttonPanel.Controls.Add(btnClearLog);
            buttonPanel.Controls.Add(btnSwap);
            buttonPanel.Controls.Add(btnLaunch);
            buttonPanel.Controls.Add(btnViewLastSummary);

            btnLaunch.Location  = new System.Drawing.Point(6, 6);
            btnLaunch.Name      = "btnLaunch";
            btnLaunch.Size      = new System.Drawing.Size(190, 30);
            btnLaunch.TabIndex  = 0;
            btnLaunch.Text      = "Launch Import Wizard";
            btnLaunch.Theme     = "DefaultType";

            btnSwap.Location  = new System.Drawing.Point(202, 6);
            btnSwap.Name      = "btnSwap";
            btnSwap.Size      = new System.Drawing.Size(120, 30);
            btnSwap.TabIndex  = 1;
            btnSwap.Text      = "⇄ Swap";
            btnSwap.Theme     = "DefaultType";

            btnClearLog.Location  = new System.Drawing.Point(328, 6);
            btnClearLog.Name      = "btnClearLog";
            btnClearLog.Size      = new System.Drawing.Size(100, 30);
            btnClearLog.TabIndex  = 2;
            btnClearLog.Text      = "Clear Log";
            btnClearLog.Theme     = "DefaultType";

            btnViewLastSummary.Location  = new System.Drawing.Point(434, 6);
            btnViewLastSummary.Name      = "btnViewLastSummary";
            btnViewLastSummary.Size      = new System.Drawing.Size(130, 30);
            btnViewLastSummary.TabIndex  = 3;
            btnViewLastSummary.Text      = "📄 Last Summary";
            btnViewLastSummary.Theme     = "DefaultType";
            btnViewLastSummary.Enabled   = false;

            // ── historyPanel ───────────────────────────────────────────────────
            historyPanel.Dock     = System.Windows.Forms.DockStyle.Top;
            historyPanel.Height   = 110;
            historyPanel.Name     = "historyPanel";
            historyPanel.TabIndex = 6;
            historyPanel.Controls.Add(historyGrid);
         historyGrid.Dock      = System.Windows.Forms.DockStyle.Fill;
            historyGrid.Name      = "historyGrid";
            historyGrid.ReadOnly  = true;
        
            historyGrid.TabIndex  = 0;
            historyGrid.Theme     = "DefaultType";
            // Columns defined in code-behind via InitHistoryGrid()

            // ── txtLog ─────────────────────────────────────────────────────────
            txtLog.Dock       = System.Windows.Forms.DockStyle.Fill;
            txtLog.Font       = new System.Drawing.Font("Consolas", 9F);
            txtLog.Name       = "txtLog";
            txtLog.ReadOnly   = true;
            txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            txtLog.TabIndex   = 5;

            // ── UserControl ────────────────────────────────────────────────────
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(txtLog);
            Controls.Add(historyPanel);
            Controls.Add(buttonPanel);
            Controls.Add(templatePanel);
            Controls.Add(optionsPanel);
            Controls.Add(destPanel);
            Controls.Add(sourcePanel);
            Controls.Add(directionPanel);
            Name = "uc_ImportExportWizardLauncher";
            Size = new System.Drawing.Size(660, 520);

            directionPanel.ResumeLayout(false);
            sourcePanel.ResumeLayout(false);
            destPanel.ResumeLayout(false);
            optionsPanel.ResumeLayout(false);
            templatePanel.ResumeLayout(false);
            buttonPanel.ResumeLayout(false);
            historyPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)historyGrid).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel        directionPanel;
        private BeepLabel                         lblMode;
        private BeepComboBox                      cmbDirection;

        private System.Windows.Forms.Panel        sourcePanel;
        private BeepLabel                         lblSourceDS;
        private BeepComboBox                      cmbSourceDS;
        private BeepLabel                         lblSourceEntity;
        private BeepComboBox                      cmbSourceEntity;

        private System.Windows.Forms.Panel        destPanel;
        private BeepLabel                         lblDestDS;
        private BeepComboBox                      cmbDestDS;
        private BeepLabel                         lblDestEntity;
        private BeepComboBox                      cmbDestEntity;

        private System.Windows.Forms.Panel        optionsPanel;
        private BeepCheckBoxBool                  chkCreateIfNotExists;
        private BeepCheckBoxBool                  chkAddMissing;

        private System.Windows.Forms.Panel        templatePanel;
        private BeepLabel                         lblRecentTemplates;
        private BeepComboBox                      cmbRecentTemplates;
        private BeepButton                        btnQuickImport;

        private System.Windows.Forms.Panel        buttonPanel;
        private BeepButton                        btnLaunch;
        private BeepButton                        btnSwap;
        private BeepButton                        btnClearLog;
        private BeepButton                        btnViewLastSummary;

        private System.Windows.Forms.Panel        historyPanel;
        private BeepGridPro historyGrid;

        private System.Windows.Forms.RichTextBox  txtLog;
    }
}
