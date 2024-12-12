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
        }

        private void BeepButton2_Click(object? sender, EventArgs e)
        {
           beepTree1.PopulateTree(BeepTreeDataGenerator.GenerateMockData(30, 3, 3));
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
