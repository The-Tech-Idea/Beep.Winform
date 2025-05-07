using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Models
{

    public class ValidationEventArgs : EventArgs
    {
        public string Value { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; } = true;
        public bool Cancel { get; set; } = false;
    }
}
