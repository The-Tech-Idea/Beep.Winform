using System.Runtime.InteropServices;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm
    {
        private bool _enableMicaBackdrop = false;

        public bool EnableMicaBackdrop
        {
            get => _enableMicaBackdrop;
            set { _enableMicaBackdrop = value; if (IsHandleCreated) ApplyMicaBackdropIfNeeded(); }
        }

        private void ApplyMicaBackdropIfNeeded()
        {
            if (!IsHandleCreated) return;
            if (_enableMicaBackdrop)
            {
                TryEnableMica();
            }
            else
            {
                TryDisableMica();
            }
        }

        private void TryEnableMica()
        {
            try
            {
                int trueValue = 1;
                DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT, ref trueValue, Marshal.SizeOf<int>());
            }
            catch { }
        }

        private void TryDisableMica()
        {
            try
            {
                int falseValue = 0;
                DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT, ref falseValue, Marshal.SizeOf<int>());
            }
            catch { }
        }

        private enum DWMWINDOWATTRIBUTE
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_MICA_EFFECT = 1029 // unofficial/SDK dependent
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);
    }
}
