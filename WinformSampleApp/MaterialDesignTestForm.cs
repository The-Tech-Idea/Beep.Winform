using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace WinformSampleApp
{
    public partial class MaterialDesignTestForm : Form
    {
        private BeepMaterialTextField textField1;
        private BeepMaterialTextField textField2;
        private BeepMaterialTextField textField3;
        private Button btnToggleElevation;
        private Button btnChangeElevation;
        private Label lblElevation;

        public MaterialDesignTestForm()
        {
            InitializeComponent();
            SetupMaterialDesignTest();
        }

        private void SetupMaterialDesignTest()
        {
            this.Text = "Material Design 3.0 Test";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create test text fields with different configurations
            textField1 = new BeepMaterialTextField();
            textField1.Location = new Point(50, 50);
            textField1.Size = new Size(250, 60);
            textField1.LabelText = "Standard Field";
            textField1.LeadingIconPath = "search"; // Using icon name
            textField1.MaterialElevationLevel = 1;
            textField1.Text = "Search something...";

            textField2 = new BeepMaterialTextField();
            textField2.Location = new Point(50, 120);
            textField2.Size = new Size(250, 60);
            textField2.LabelText = "With Trailing Icon";
            textField2.TrailingIconPath = "eye"; // Using icon name
            textField2.MaterialElevationLevel = 2;
            textField2.Text = "Password field";

            textField3 = new BeepMaterialTextField();
            textField3.Location = new Point(50, 190);
            textField3.Size = new Size(250, 60);
            textField3.LabelText = "No Elevation";
            textField3.LeadingIconPath = "user";
            textField3.MaterialElevationLevel = 0;
            textField3.MaterialUseElevation = false;
            textField3.Text = "No shadow";

            // Control buttons
            btnToggleElevation = new Button();
            btnToggleElevation.Text = "Toggle Elevation";
            btnToggleElevation.Location = new Point(350, 50);
            btnToggleElevation.Size = new Size(120, 35);
            btnToggleElevation.Click += BtnToggleElevation_Click;

            btnChangeElevation = new Button();
            btnChangeElevation.Text = "Change Elevation";
            btnChangeElevation.Location = new Point(350, 100);
            btnChangeElevation.Size = new Size(120, 35);
            btnChangeElevation.Click += BtnChangeElevation_Click;

            lblElevation = new Label();
            lblElevation.Text = "Current Elevation: 1";
            lblElevation.Location = new Point(350, 150);
            lblElevation.AutoSize = true;

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                textField1, textField2, textField3,
                btnToggleElevation, btnChangeElevation, lblElevation
            });
        }

        private void BtnToggleElevation_Click(object sender, EventArgs e)
        {
            textField1.MaterialUseElevation = !textField1.MaterialUseElevation;
            textField2.MaterialUseElevation = !textField2.MaterialUseElevation;
            textField3.MaterialUseElevation = !textField3.MaterialUseElevation;

            btnToggleElevation.Text = textField1.MaterialUseElevation ? "Disable Elevation" : "Enable Elevation";
        }

        private void BtnChangeElevation_Click(object sender, EventArgs e)
        {
            int currentLevel = textField1.MaterialElevationLevel;
            int newLevel = (currentLevel + 1) % 6; // Cycle through 0-5

            textField1.MaterialElevationLevel = newLevel;
            textField2.MaterialElevationLevel = newLevel;
            textField3.MaterialElevationLevel = newLevel;

            lblElevation.Text = $"Current Elevation: {newLevel}";
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Name = "MaterialDesignTestForm";
            this.Text = "Material Design 3.0 Test";
            this.ResumeLayout(false);
        }
    }
}
