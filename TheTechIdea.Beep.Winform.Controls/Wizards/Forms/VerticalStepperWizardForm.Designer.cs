using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Forms
{
    partial class VerticalStepperWizardForm
    {
        private System.ComponentModel.IContainer components = null;

        private TableLayoutPanel _root;
        private TableLayoutPanel _rightColumn;
        private BeepStepperBar _stepper;
        private Panel _errorPanel;
        private Label _lblError;
        private BufferedPanel _contentPanel;
        private TableLayoutPanel _buttonPanel;
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
        /// Builds the form's layout, on the design surface.
        /// </summary>
        /// <remarks>
        /// Everything here is plain construction plus <c>Controls.Add</c> - no loops, no
        /// conditionals, nothing the designer cannot serialise - so the form is visible and
        /// editable at design time. It previously built every control in the constructor, which
        /// left the design surface blank.
        /// <para>
        /// The optional Skip and Help buttons are created unconditionally and their
        /// <c>Visible</c> comes from the config at runtime; a conditional in this method would
        /// break the designer.
        /// </para>
        /// </remarks>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _root = new TableLayoutPanel();
            _rightColumn = new TableLayoutPanel();
            _stepper = new BeepStepperBar();
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
            _rightColumn.SuspendLayout();
            _errorPanel.SuspendLayout();
            _buttonPanel.SuspendLayout();
            SuspendLayout();
            //
            // _root - the vertical step rail on the left, everything else on the right
            //
            _root.ColumnCount = 2;
            _root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280F));
            _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _root.RowCount = 1;
            _root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _root.Dock = DockStyle.Fill;
            _root.Name = "_root";
            _root.Controls.Add(_stepper, 0, 0);
            _root.Controls.Add(_rightColumn, 1, 0);
            //
            // _rightColumn
            //
            _rightColumn.ColumnCount = 1;
            _rightColumn.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _rightColumn.RowCount = 3;
            _rightColumn.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            _rightColumn.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _rightColumn.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            _rightColumn.Dock = DockStyle.Fill;
            _rightColumn.Margin = new Padding(0);
            _rightColumn.Name = "_rightColumn";
            _rightColumn.Controls.Add(_errorPanel, 0, 0);
            _rightColumn.Controls.Add(_contentPanel, 0, 1);
            _rightColumn.Controls.Add(_buttonPanel, 0, 2);
            //
            // _sidePanel
            //
            // The framework's stepper, vertical - not a Panel plus SidePanel_Paint.
            _stepper.Dock = DockStyle.Fill;
            _stepper.Margin = new Padding(0);
            _stepper.Name = "_stepper";
            _stepper.Orientation = Orientation.Vertical;
            //
            // _contentPanel
            //
            _contentPanel.Dock = DockStyle.Fill;
            _contentPanel.Margin = new Padding(0);
            _contentPanel.Name = "_contentPanel";
            _contentPanel.Padding = new Padding(30, 30, 30, 20);
            //
            // _buttonPanel
            //
            // Cancel | Skip | Help | (spacer) | Back | Next. The spacer column absorbs the slack,
            // so the left group stays left and the right group stays right at every width.
            //
            // This replaced six hand-computed Left coordinates that were derived from
            // _buttonPanel.ClientSize.Width BEFORE the panel had been added to the form - the
            // panel still had its default width at that moment, so every button was positioned
            // against the wrong number until the first resize moved them.
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
            _buttonPanel.Padding = new Padding(20, 15, 20, 15);
            _buttonPanel.Controls.Add(_btnCancel, 0, 0);
            _buttonPanel.Controls.Add(_btnSkip, 1, 0);
            _buttonPanel.Controls.Add(_btnHelp, 2, 0);
            _buttonPanel.Controls.Add(_btnBack, 4, 0);
            _buttonPanel.Controls.Add(_btnNext, 5, 0);
            //
            // navigation buttons
            //
            _btnCancel.Anchor = AnchorStyles.Left;
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new System.Drawing.Size(100, 36);
            _btnCancel.Text = "Cancel";
            _btnCancel.Click += BtnCancel_Click;

            _btnSkip.Anchor = AnchorStyles.Left;
            _btnSkip.Margin = new Padding(10, 3, 3, 3);
            _btnSkip.Name = "_btnSkip";
            _btnSkip.Size = new System.Drawing.Size(100, 36);
            _btnSkip.Text = "Skip";
            _btnSkip.Visible = false;
            _btnSkip.Click += BtnSkip_Click;

            _btnHelp.Anchor = AnchorStyles.Left;
            _btnHelp.Margin = new Padding(10, 3, 3, 3);
            _btnHelp.Name = "_btnHelp";
            _btnHelp.Size = new System.Drawing.Size(80, 36);
            _btnHelp.Text = "Help";
            _btnHelp.Visible = false;
            _btnHelp.Click += BtnHelp_Click;

            _btnBack.Anchor = AnchorStyles.Right;
            _btnBack.Enabled = false;
            _btnBack.Name = "_btnBack";
            _btnBack.Size = new System.Drawing.Size(100, 36);
            _btnBack.Text = "Back";
            _btnBack.Click += BtnBack_Click;

            _btnNext.Anchor = AnchorStyles.Right;
            _btnNext.Margin = new Padding(10, 3, 3, 3);
            _btnNext.Name = "_btnNext";
            _btnNext.Size = new System.Drawing.Size(120, 36);
            _btnNext.Text = "Next";
            _btnNext.Click += BtnNext_Click;
            //
            // _errorPanel
            //
            _errorPanel.Dock = DockStyle.Fill;
            _errorPanel.Margin = new Padding(0);
            _errorPanel.Name = "_errorPanel";
            _errorPanel.Padding = new Padding(16, 0, 16, 0);
            _errorPanel.Visible = false;
            _errorPanel.Controls.Add(_lblError);

            _lblError.Dock = DockStyle.Fill;
            _lblError.Name = "_lblError";
            _lblError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // VerticalStepperWizardForm
            //
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(900, 600);
            Controls.Add(_root);
            KeyPreview = true;
            Name = "VerticalStepperWizardForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Wizard";
            _buttonPanel.ResumeLayout(false);
            _errorPanel.ResumeLayout(false);
            _rightColumn.ResumeLayout(false);
            _root.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
