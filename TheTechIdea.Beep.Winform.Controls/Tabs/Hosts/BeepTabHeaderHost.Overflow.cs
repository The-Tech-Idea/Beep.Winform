using System.ComponentModel;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Hosts
{
    public partial class BeepTabHeaderHost
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal BeepTabOverflowState OverflowState { get; private set; } = BeepTabOverflowState.Empty;

        internal void SetOverflowState(BeepTabOverflowState overflowState)
        {
            OverflowState = overflowState ?? BeepTabOverflowState.Empty;
        }
    }
}