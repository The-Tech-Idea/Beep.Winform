using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Description("A Material Design text box control with React-like features")]
    [DisplayName("Beep Material TextBox")]
    [Category("Beep Controls")]
    public class BeepMaterialTextBox :BeepControl
    {
        public BeepMaterialTextBox()
        {
            this.Text = "Beep Material TextBox";
            this.BackColor = System.Drawing.Color.White;
            this.ForeColor = System.Drawing.Color.Black;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }
    }
   
}
