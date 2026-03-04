using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Default.Views.ImportExport
{
    partial class uc_Import_MapFields
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
            components         = new System.ComponentModel.Container();
            beepDataGrid1      = new System.Windows.Forms.DataGridView();
            btnSelectAll       = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            btnSelectNone      = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            btnAutoMatch       = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            toolbarPanel       = new System.Windows.Forms.Panel();
            // template controls
            cmbTemplateLoad    = new TheTechIdea.Beep.Winform.Controls.BeepComboBox();
            btnTemplateSave    = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            btnTemplateDelete  = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            // status bar
            statusPanel        = new System.Windows.Forms.Panel();
            lblMappingStatus   = new TheTechIdea.Beep.Winform.Controls.BeepLabel();

            ((System.ComponentModel.ISupportInitialize)beepDataGrid1).BeginInit();
            toolbarPanel.SuspendLayout();
            statusPanel.SuspendLayout();
            SuspendLayout();

            //
            // toolbarPanel
            //
            toolbarPanel.Dock     = System.Windows.Forms.DockStyle.Top;
            toolbarPanel.Height   = 72;
            toolbarPanel.Name     = "toolbarPanel";
            toolbarPanel.TabIndex = 0;
            toolbarPanel.Controls.Add(btnAutoMatch);
            toolbarPanel.Controls.Add(btnSelectNone);
            toolbarPanel.Controls.Add(btnSelectAll);
            toolbarPanel.Controls.Add(btnTemplateDelete);
            toolbarPanel.Controls.Add(btnTemplateSave);
            toolbarPanel.Controls.Add(cmbTemplateLoad);
            //
            // cmbTemplateLoad
            //
            cmbTemplateLoad.Location  = new System.Drawing.Point(6, 39);
            cmbTemplateLoad.Name      = "cmbTemplateLoad";
            cmbTemplateLoad.Size      = new System.Drawing.Size(180, 27);
            cmbTemplateLoad.TabIndex  = 3;
            cmbTemplateLoad.Theme     = "DefaultType";
            //
            // btnTemplateSave
            //
            btnTemplateSave.Location  = new System.Drawing.Point(192, 39);
            btnTemplateSave.Name      = "btnTemplateSave";
            btnTemplateSave.Size      = new System.Drawing.Size(130, 27);
            btnTemplateSave.TabIndex  = 4;
            btnTemplateSave.Text      = "Save Template…";
            btnTemplateSave.Theme     = "DefaultType";
            //
            // btnTemplateDelete
            //
            btnTemplateDelete.Location  = new System.Drawing.Point(328, 39);
            btnTemplateDelete.Name      = "btnTemplateDelete";
            btnTemplateDelete.Size      = new System.Drawing.Size(28, 27);
            btnTemplateDelete.TabIndex  = 5;
            btnTemplateDelete.Text      = "✕";
            btnTemplateDelete.Theme     = "DefaultType";
            //
            // statusPanel
            //
            statusPanel.Dock    = System.Windows.Forms.DockStyle.Bottom;
            statusPanel.Height  = 28;
            statusPanel.Name    = "statusPanel";
            statusPanel.TabIndex= 2;
            statusPanel.Controls.Add(lblMappingStatus);
            //
            // lblMappingStatus
            //
            lblMappingStatus.AutoSize  = false;
            lblMappingStatus.Dock      = System.Windows.Forms.DockStyle.Fill;
            lblMappingStatus.Name      = "lblMappingStatus";
            lblMappingStatus.Text      = "0 fields mapped";
            lblMappingStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblMappingStatus.ShowAllBorders = false;
            lblMappingStatus.Theme     = "DefaultType";

            //
            // btnSelectAll
            //
            btnSelectAll.Location  = new System.Drawing.Point(6, 5);
            btnSelectAll.Name      = "btnSelectAll";
            btnSelectAll.Size      = new System.Drawing.Size(110, 28);
            btnSelectAll.TabIndex  = 0;
            btnSelectAll.Text      = "Select All";
            btnSelectAll.Theme     = "DefaultType";
            btnSelectAll.Click    += new System.EventHandler(this.SelectAll_Click);

            //
            // btnSelectNone
            //
            btnSelectNone.Location  = new System.Drawing.Point(122, 5);
            btnSelectNone.Name      = "btnSelectNone";
            btnSelectNone.Size      = new System.Drawing.Size(110, 28);
            btnSelectNone.TabIndex  = 1;
            btnSelectNone.Text      = "Select None";
            btnSelectNone.Theme     = "DefaultType";
            btnSelectNone.Click    += new System.EventHandler(this.SelectNone_Click);

            //
            // btnAutoMatch
            //
            btnAutoMatch.Location  = new System.Drawing.Point(238, 5);
            btnAutoMatch.Name      = "btnAutoMatch";
            btnAutoMatch.Size      = new System.Drawing.Size(110, 28);
            btnAutoMatch.TabIndex  = 2;
            btnAutoMatch.Text      = "Auto Match";
            btnAutoMatch.Theme     = "DefaultType";
            btnAutoMatch.Click    += new System.EventHandler(this.AutoMatch_Click);

            //
            // beepDataGrid1
            //
            beepDataGrid1.AllowUserToAddRows        = false;
            beepDataGrid1.AllowUserToDeleteRows     = false;
            beepDataGrid1.AutoSizeColumnsMode       = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            beepDataGrid1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            beepDataGrid1.Dock      = System.Windows.Forms.DockStyle.Fill;
            beepDataGrid1.Name      = "beepDataGrid1";
            beepDataGrid1.TabIndex  = 1;

            //
            // uc_Import_MapFields
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(beepDataGrid1);
            Controls.Add(statusPanel);
            Controls.Add(toolbarPanel);
            Name = "uc_Import_MapFields";
            Size = new System.Drawing.Size(646, 463);

            ((System.ComponentModel.ISupportInitialize)beepDataGrid1).EndInit();
            toolbarPanel.ResumeLayout(false);
            statusPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView beepDataGrid1;
        private BeepButton btnSelectAll;
        private BeepButton btnSelectNone;
        private BeepButton btnAutoMatch;
        private System.Windows.Forms.Panel toolbarPanel;
        // template
        private TheTechIdea.Beep.Winform.Controls.BeepComboBox cmbTemplateLoad;
        private BeepButton btnTemplateSave;
        private BeepButton btnTemplateDelete;
        // status
        private System.Windows.Forms.Panel statusPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel lblMappingStatus;
    }
}
