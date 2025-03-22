using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class NoBorderMonthCalendar : MonthCalendar
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Remove native border styles.
                cp.ExStyle &= ~0x00000200; // WS_EX_CLIENTEDGE
                cp.Style &= ~0x00800000; // WS_BORDER
                return cp;
            }
        }

        //protected override void OnHandleCreated(EventArgs e)
        //{
        //    base.OnHandleCreated(e);
        //    // Remove the themed border by disabling visual styles on this control.
        //    SetWindowTheme(this.Handle, "", "");
        //}
    }

}
