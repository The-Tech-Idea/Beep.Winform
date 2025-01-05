using System.ComponentModel;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class Form1 : BeepiForm
    {
        [Browsable(true)]
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public UnitofWork<ConnectionProperties> Connections { get; set; }
        public Form1()
        {
            InitializeComponent();
            // Initialize Connections
            Connections = new UnitofWork<ConnectionProperties>();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
