using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tree;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class Form1 : BeepiForm
    {
        
        public Form1()
        {
            InitializeComponent();
            // this.tabControl1.TabPanels[0].Controls.Add(beepAppBar1);
            this.beepButton2.Click += BeepButton2_Click;
            beepButton3.Click += BeepButton3_Click;
        }

        private void BeepButton3_Click(object? sender, EventArgs e)
        {
            BeepPopupForm x = new BeepPopupForm();
            x.StartPosition = FormStartPosition.CenterParent;
            var y = new BeepButton() { };
            y.Text = "Hello";
            y.Left = 10;
            y.Top = 10; 
            x.Controls.Add(y);
            x.BorderThickness = 5;
            x.Show();
        }

        private void BeepButton2_Click(object? sender, EventArgs e)
        {
           beepTree1.PopulateTree(BeepTreeDataGenerator.GenerateMockData(100,4, 4));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void beepListofValuesBox1_Click(object sender, EventArgs e)
        {

        }

        private void beepAppBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
