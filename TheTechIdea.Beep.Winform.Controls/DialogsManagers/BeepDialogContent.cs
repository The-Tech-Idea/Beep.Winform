using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    public sealed class BeepDialogContent
    {
        public string Title { get; set; } = "Dialog";
        public string Message { get; set; } = "";
        public Control? CustomBody { get; set; } // optional: use your own layout/content
        public IEnumerable<BeepDialogButton> Buttons { get; set; } = Enumerable.Empty<BeepDialogButton>();
        public Icon? Icon { get; set; }
        // If provided, dialog shows a validation summary in footer and disables default button while invalid.
        public Func<ValidationState>? Validator { get; set; }
        // Which subtree to watch for changes (TextChanged, CheckedChanged, etc.) to auto re-validate.
        // If null, CustomBody is used; if both null, only manual validation occurs right before OK.
        public Control? ValidationRoot { get; set; }
    }
}
