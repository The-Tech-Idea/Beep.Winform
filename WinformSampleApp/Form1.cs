using TheTechIdea.Beep.Winform.Controls.GridX;

namespace WinformSampleApp
{
    public partial class Form1 : Form
    {
        private BeepGridPro grid;

        public Form1()
        {
            InitializeComponent();

            // Create and configure BeepGridPro
            grid = new BeepGridPro();
            grid.Dock = DockStyle.Fill;
            grid.Location = new System.Drawing.Point(0, 0);
            grid.Size = new System.Drawing.Size(800, 600);

            // Add to form
            this.Controls.Add(grid);

            // Set form properties
            this.Text = "BeepGridPro Test - Check Debug Output";
            this.Size = new System.Drawing.Size(820, 640);
        }
    }
}
