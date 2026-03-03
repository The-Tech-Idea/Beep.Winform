using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Wizard
{
    public sealed class WizardPage
    {
        public string Title { get; set; } = string.Empty;
        public Control? Content { get; set; }
        public Func<bool>? Validate { get; set; }
    }
}
