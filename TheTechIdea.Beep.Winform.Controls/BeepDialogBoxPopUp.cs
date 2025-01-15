using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepDialogBoxPopUp : BeepPopupForm
    {
        public enum DialogButtons
        {
            None,
            Ok,
            OkCancel,
            YesNo,
            YesNoCancel
        }
        public enum DialogIcon
        {
            None,
            Information,
            Warning,
            Error,
            Question
        }
        public enum DialogResults
        {
            None,
            Ok,
            Cancel,
            Yes,
            No
        }
        public Dictionary<DialogButtons,BeepButton>  Buttons { get; set; } = new Dictionary<DialogButtons, BeepButton>();
        public Dictionary<DialogIcon, BeepImage> Icons { get; set; } = new Dictionary<DialogIcon, BeepImage>();
        public DialogResults Result { get; set; } = DialogResults.None;

        public BeepDialogBoxPopUp()
        {
            InitializeComponent();
           
        }
    



    }
}
