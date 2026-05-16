using System.ComponentModel;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    partial class uc_NuggetsInstall_Step_VersionSource
    {
        private IContainer? components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            _tlpRoot        = new TableLayoutPanel();
            _lblPackageId   = new BeepLabel();
            _lblVersionCap  = new BeepLabel();
            _cmbVersion     = new BeepComboBox();
            _lblSourceCap   = new BeepLabel();
            _cmbSource      = new BeepComboBox();
            _chkPrerelease  = new BeepCheckBoxBool();

            _tlpRoot.SuspendLayout();
            SuspendLayout();

            // _tlpRoot – 2 cols: label | input; 4 rows
            _tlpRoot.Dock = DockStyle.Fill;
            _tlpRoot.ColumnCount = 2;
            _tlpRoot.RowCount = 4;
            _tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            _tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _tlpRoot.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _tlpRoot.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _tlpRoot.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _tlpRoot.Padding = new Padding(12);
            _tlpRoot.Name = "_tlpRoot";

            _lblPackageId.AutoSize = true;
            _lblPackageId.Anchor = AnchorStyles.Left;
            _lblPackageId.Margin = new Padding(0, 0, 0, 10);
            _lblPackageId.Name = "_lblPackageId";
            _tlpRoot.SetColumnSpan(_lblPackageId, 2);

            _lblVersionCap.Text = "Version:";
            _lblVersionCap.AutoSize = true;
            _lblVersionCap.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _lblVersionCap.Margin = new Padding(0, 0, 6, 8);
            _lblVersionCap.Name = "_lblVersionCap";

            _cmbVersion.Dock = DockStyle.Fill;
            _cmbVersion.Margin = new Padding(0, 0, 0, 8);
            _cmbVersion.Name = "_cmbVersion";

            _lblSourceCap.Text = "Source:";
            _lblSourceCap.AutoSize = true;
            _lblSourceCap.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _lblSourceCap.Margin = new Padding(0, 0, 6, 8);
            _lblSourceCap.Name = "_lblSourceCap";

            _cmbSource.Dock = DockStyle.Fill;
            _cmbSource.Margin = new Padding(0, 0, 0, 8);
            _cmbSource.Name = "_cmbSource";

            _chkPrerelease.Text = "Include prerelease versions";
            _chkPrerelease.AutoSize = true;
            _chkPrerelease.Anchor = AnchorStyles.Left;
            _chkPrerelease.Name = "_chkPrerelease";
            _chkPrerelease.StateChanged += ChkPrerelease_StateChanged;
            _tlpRoot.SetColumnSpan(_chkPrerelease, 2);

            _tlpRoot.Controls.Add(_lblPackageId,  0, 0);
            _tlpRoot.Controls.Add(_lblVersionCap, 0, 1);
            _tlpRoot.Controls.Add(_cmbVersion,    1, 1);
            _tlpRoot.Controls.Add(_lblSourceCap,  0, 2);
            _tlpRoot.Controls.Add(_cmbSource,     1, 2);
            _tlpRoot.Controls.Add(_chkPrerelease, 0, 3);

            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_tlpRoot);
            Name = "uc_NuggetsInstall_Step_VersionSource";

            _tlpRoot.ResumeLayout(false);
            ResumeLayout(false);
        }

        private TableLayoutPanel _tlpRoot;
        private BeepLabel _lblPackageId;
        private BeepLabel _lblVersionCap;
        private BeepComboBox _cmbVersion;
        private BeepLabel _lblSourceCap;
        private BeepComboBox _cmbSource;
        private BeepCheckBoxBool _chkPrerelease;
    }
}
