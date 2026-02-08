using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views
{
    [AddinAttribute(Caption = "Home", Name = "Form1", misc = "Form1", menu = "Form1", addinType = AddinType.Page, displayType = DisplayType.Popup, ObjectType = "Beep")]
    public partial class Form1 : TemplateForm
    {
        public Form1()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void beepPanel1_Click(object sender, EventArgs e)
        {

        }
    }
}
