using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Hosts
{
    public partial class BeepTabHeaderHost
    {
        public bool TryResolveContextMenuTab(Point location, out int tabIndex)
        {
            if (TryHitCloseButton(location, out tabIndex))
            {
                return true;
            }

            return TryHitTab(location, out tabIndex);
        }
    }
}