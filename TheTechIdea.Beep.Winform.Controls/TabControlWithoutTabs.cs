using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls
{
    // A TabControl that hides the default tab headers
    [DesignerCategory("Code")]
    [ToolboxItem(false)]
    public class TabControlWithoutTabs : TabControl
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
        // Custom event that notifies when TabPages change
        public event EventHandler TabPagesChanged;
        protected override void WndProc(ref Message m)
        {
            // TCM_ADJUSTRECT message = 0x1328
            if (m.Msg == 0x1328 && !DesignMode)
            {
                // Return zero rect to hide tabs
                m.Result = IntPtr.Zero;
                return;
            }
            base.WndProc(ref m);
        }
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            // A tab (TabPage) was added
            TabPagesChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            // A tab (TabPage) was removed
            TabPagesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
