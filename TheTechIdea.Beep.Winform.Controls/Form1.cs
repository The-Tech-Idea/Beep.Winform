

using System.ComponentModel;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class Form1 : BeepiForm
    {

        public UnitofWork<EntityStructure> structures { get; set; } = new UnitofWork<EntityStructure>();

        public IUnitofWork<EntityStructure> structures2 { get; set; } = new UnitofWork<EntityStructure>();

        public string testparam { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void beepButton1_Click(object sender, EventArgs e)
        {

        }
    }
}
