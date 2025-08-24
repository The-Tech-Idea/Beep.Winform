using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Models
{
    /// <summary>
    /// Event arguments for UIStyle change events
    /// </summary>
    public class FormUIStyleChangedEventArgs : EventArgs
    {
        public FormUIStyle OldStyle { get; }
        public FormUIStyle NewStyle { get; }

        public FormUIStyleChangedEventArgs(FormUIStyle oldStyle, FormUIStyle newStyle)
        {
            OldStyle = oldStyle;
            NewStyle = newStyle;
        }
    }

}
