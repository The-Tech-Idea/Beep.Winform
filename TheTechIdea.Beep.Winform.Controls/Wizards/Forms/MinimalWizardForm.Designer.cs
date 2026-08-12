using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Forms
{
    partial class MinimalWizardForm
    {
        private System.ComponentModel.IContainer components = null;

        // ── Layout ────────────────────────────────────────────────────────────────
        private TableLayoutPanel _root;
        private Panel _headerPanel;
        private Panel _errorPanel;
        private Label _lblError;
        private BufferedPanel _contentPanel;
        private TableLayoutPanel _buttonPanel;

        // ── Navigation ────────────────────────────────────────────────────────────
        private BeepButton _btnCancel;
        private BeepButton _btnSkip;
        private BeepButton _btnHelp;
        private BeepButton _btnBack;
        private BeepButton _btnNext;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Builds the form's layout. Visible on the design surface, because everything here is
        /// plain construction and <c>Controls.Add</c> - no loops, no conditionals, no method calls
        /// the designer cannot serialise.
        /// </summary>
        /// <remarks>
        /// This replaced a runtime <c>InitializeControls</c> + <c>LayoutControls</c> pair. Two
        /// things came with the move:
        /// <list type="bullet">
        /// <item>The form showed nothing at design time - every control was built in the
        /// constructor, so the designer surface was blank.</item>
        /// <item><c>LayoutControls</c> positioned the buttons from
        /// <c>_buttonPanel.ClientSize.Width</c> **before the panel had been added to the form**, so
        /// the panel still had its default width and every button landed in the wrong place until
        /// the first resize. A <see cref="TableLayoutPanel"/> has no such ordering hazard.</item>
        /// </list>
        /// The optional Skip and Help buttons are created unconditionally here and their
        /// <c>Visible</c> is set from the config at runtime - a conditional in this method would
        /// break the designer.
        /// </remarks>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _root = new TableLayoutPanel();
            _headerPanel = new Panel();
            _errorPanel = new Panel();
            _lblError = new Label();
            _contentPanel = new BufferedPanel();
            _buttonPanel = new TableLayoutPanel();
            _btnCancel = new BeepButton();
            _btnSkip = new BeepButton();
            _btnHelp = new BeepButton();
            _btnBack = new BeepButton();
            _btnNext = new BeepButton();
            _root.SuspendLayout();
            _errorPanel.SuspendLayout();
            _buttonPanel.SuspendLayout();
            SuspendLayout();
            //
            // _root
            //
            _root.ColumnCount = 1;
            _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _root.RowCount = 4;
            _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));
            _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            _root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            _root.Dock = DockStyle.Fill;
            _root.Name = "_root";
            _root.Controls.Add(_headerPanel, 0, 0);
            _root.Controls.Add(_errorPanel, 0, 1);
            _root.Controls.Add(_contentPanel, 0, 2);
            _root.Controls.Add(_buttonPanel, 0, 3);
            //
            // _headerPanel
            //
            // 72, not 60: the band holds the dot row (top 25 + ring) AND the step title
            // (25px + padding). 60 clipped the title's bottom half at 96 DPI and worse scaled.
            _headerPanel.BackColor = System.Drawing.Color.Transparent;
            _headerPanel.Dock = DockStyle.Fill;
            _headerPanel.Margin = new Padding(0);
            _headerPanel.Name = "_headerPanel";
            _headerPanel.Paint += HeaderPanel_Paint;
            //
            // _errorPanel
            //
            _errorPanel.Dock = DockStyle.Fill;
            _errorPanel.Margin = new Padding(0);
            _errorPanel.Name = "_errorPanel";
            _errorPanel.Padding = new Padding(16, 0, 16, 0);
            _errorPanel.Visible = false;
            _errorPanel.Controls.Add(_lblError);
            //
            // _lblError
            //
            _lblError.AutoEllipsis = true;
            _lblError.Dock = DockStyle.Fill;
            _lblError.Name = "_lblError";
            _lblError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // _contentPanel
            //
            _contentPanel.Dock = DockStyle.Fill;
            _contentPanel.Margin = new Padding(0);
            _contentPanel.Name = "_contentPanel";
            _contentPanel.Padding = new Padding(40, 20, 40, 20);
            //
            // _buttonPanel
            //
            // Cancel | Skip | Help | (spacer) | Back | Next - the spacer column takes the slack, so
            // the left group stays left and the right group stays right at any width. The previous
            // version computed six Left coordinates by hand.
            _buttonPanel.ColumnCount = 6;
            _buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _buttonPanel.RowCount = 1;
            _buttonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _buttonPanel.Dock = DockStyle.Fill;
            _buttonPanel.Margin = new Padding(0);
            _buttonPanel.Name = "_buttonPanel";
            _buttonPanel.Padding = new Padding(20, 10, 20, 10);
            _buttonPanel.Controls.Add(_btnCancel, 0, 0);
            _buttonPanel.Controls.Add(_btnSkip, 1, 0);
            _buttonPanel.Controls.Add(_btnHelp, 2, 0);
            _buttonPanel.Controls.Add(_btnBack, 4, 0);
            _buttonPanel.Controls.Add(_btnNext, 5, 0);
            //
            // _btnCancel
            //
            _btnCancel.Anchor = AnchorStyles.Left;
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new System.Drawing.Size(100, 36);
            _btnCancel.Text = "Cancel";
            _btnCancel.Click += BtnCancel_Click;
            //
            // _btnSkip
            //
            _btnSkip.Anchor = AnchorStyles.Left;
            _btnSkip.Margin = new Padding(10, 3, 3, 3);
            _btnSkip.Name = "_btnSkip";
            _btnSkip.Size = new System.Drawing.Size(100, 36);
            _btnSkip.Text = "Skip";
            _btnSkip.Visible = false;
            _btnSkip.Click += BtnSkip_Click;
            //
            // _btnHelp
            //
            _btnHelp.Anchor = AnchorStyles.Left;
            _btnHelp.Margin = new Padding(10, 3, 3, 3);
            _btnHelp.Name = "_btnHelp";
            _btnHelp.Size = new System.Drawing.Size(80, 36);
            _btnHelp.Text = "Help";
            _btnHelp.Visible = false;
            _btnHelp.Click += BtnHelp_Click;
            //
            // _btnBack
            //
            _btnBack.Anchor = AnchorStyles.Right;
            _btnBack.Enabled = false;
            _btnBack.Name = "_btnBack";
            _btnBack.Size = new System.Drawing.Size(100, 36);
            _btnBack.Text = "Back";
            _btnBack.Click += BtnBack_Click;
            //
            // _btnNext
            //
            _btnNext.Anchor = AnchorStyles.Right;
            _btnNext.Margin = new Padding(10, 3, 3, 3);
            _btnNext.Name = "_btnNext";
            _btnNext.Size = new System.Drawing.Size(120, 36);
            _btnNext.Text = "Next";
            _btnNext.Click += BtnNext_Click;
            //
            // MinimalWizardForm
            //
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(700, 500);
            Controls.Add(_root);
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MinimalWizardForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Wizard";
            _buttonPanel.ResumeLayout(false);
            _errorPanel.ResumeLayout(false);
            _root.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
