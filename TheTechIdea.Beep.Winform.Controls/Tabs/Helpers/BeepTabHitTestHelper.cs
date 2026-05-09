using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    /// <summary>
    /// Shared hit testing for the premium header host and the current BeepTabs shell.
    /// </summary>
    public static class BeepTabHitTestHelper
    {
        public static bool TryHitTab(BeepTabHeaderLayoutSnapshot snapshot, Point point, out int tabIndex)
        {
            tabIndex = -1;
            if (snapshot == null)
            {
                return false;
            }

            foreach (BeepTabHeaderItemLayout item in snapshot.Items)
            {
                if (!item.HitTest(point))
                {
                    continue;
                }

                tabIndex = item.Item.Index;
                return true;
            }

            return false;
        }

        public static bool TryHitCloseButton(BeepTabHeaderLayoutSnapshot snapshot, Point point, out int tabIndex)
        {
            tabIndex = -1;
            if (snapshot == null)
            {
                return false;
            }

            foreach (BeepTabHeaderItemLayout item in snapshot.Items)
            {
                if (!item.HitTestCloseButton(point))
                {
                    continue;
                }

                tabIndex = item.Item.Index;
                return true;
            }

            return false;
        }
    }
}