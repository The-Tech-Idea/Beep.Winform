using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    public sealed class BeepToastOptions
    {
        public string? Title { get; set; }
        public string Message { get; set; } = "";
        public BeepToastKind Kind { get; set; } = BeepToastKind.Info;
        public int DurationMs { get; set; } = 3500;
        public string? ActionText { get; set; }
        public Action? Action { get; set; }
        public int MaxWidth { get; set; } = 360;
        public Padding Margin { get; set; } = new Padding(16, 8, 16, 8);
    }
}
