using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Desktop.Common.KeyManagement
{
    public class GlobalKeyHandler : IMessageFilter
    {
        private const int WM_KEYDOWN = 0x0100;
        public event EventHandler<KeyEventArgs> KeyDown;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_KEYDOWN)
            {
                var keyData = (Keys)m.WParam.ToInt32() | Control.ModifierKeys;
                var args = new KeyEventArgs(keyData);
                KeyDown?.Invoke(null, args);
                return args.Handled;
            }
            return false;
        }

    }
}
