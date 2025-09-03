using System.Runtime.InteropServices;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm
    {
        private BackdropType _backdrop = BackdropType.None;

        public BackdropType Backdrop
        {
            get => _backdrop;
            set { _backdrop = value; if (IsHandleCreated) ApplyBackdrop(); }
        }

        private void ApplyBackdrop()
        {
            // Reset to default window before applying
            try { TryDisableMica(); } catch { }
            try { TryDisableAcrylic(); } catch { }

            switch (_backdrop)
            {
                case BackdropType.Mica:
                    TryEnableMica();
                    break;
                case BackdropType.Acrylic:
                    TryEnableAcrylic();
                    break;
                case BackdropType.Tabbed:
                    TryEnableSystemBackdrop(3); // Tabbed
                    break;
                case BackdropType.Transient:
                    TryEnableSystemBackdrop(2); // Transient
                    break;
                case BackdropType.Blur:
                    TryEnableBlurBehind();
                    break;
                case BackdropType.None:
                default:
                    break;
            }
        }

        private void TryEnableSystemBackdrop(int type)
        {
            try
            {
                // Use existing DwmSetWindowAttribute from Mica partial
                var attr = (DWMWINDOWATTRIBUTE)38; // DWMWA_SYSTEMBACKDROP_TYPE
                DwmSetWindowAttribute(this.Handle, attr, ref type, Marshal.SizeOf<int>());
            }
            catch { }
        }

        private void TryEnableBlurBehind()
        {
            try
            {
                // Reuse Acrylic interop types
                var accent = new ACCENT_POLICY { AccentState = ACCENT_STATE.ACCENT_ENABLE_BLURBEHIND };
                int size = Marshal.SizeOf(accent);
                IntPtr pAccent = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(accent, pAccent, false);
                var data = new WINDOWCOMPOSITIONATTRIBDATA { Attribute = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY, Data = pAccent, SizeOfData = size };
                SetWindowCompositionAttribute(this.Handle, ref data);
                Marshal.FreeHGlobal(pAccent);
            }
            catch { }
        }
    }
}
