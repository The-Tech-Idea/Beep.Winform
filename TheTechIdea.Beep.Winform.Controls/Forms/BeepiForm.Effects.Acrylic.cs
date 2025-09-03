using System.Runtime.InteropServices;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm
    {
        private bool _enableAcrylicForGlass = true;

        public bool EnableAcrylicForGlass
        {
            get => _enableAcrylicForGlass;
            set { _enableAcrylicForGlass = value; if (IsHandleCreated) ApplyAcrylicEffectIfNeeded(); }
        }

        // Called from Style partial after style is applied
        partial void ApplyAcrylicEffectIfNeeded()
        {
            if (!IsHandleCreated) return;
            if (_formStyle == BeepFormStyle.Glass && _enableAcrylicForGlass)
            {
                TryEnableAcrylic();
            }
            else
            {
                TryDisableAcrylic();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyAcrylicEffectIfNeeded();
        }

        private void TryEnableAcrylic()
        {
            try
            {
                // Try modern acrylic via SetWindowCompositionAttribute
                var accent = new ACCENT_POLICY
                {
                    AccentState = ACCENT_STATE.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                    AccentFlags = 2, // Default flags
                    GradientColor = (uint)((0xCC << 24) | (BackColor.B << 16) | (BackColor.G << 8) | BackColor.R),
                    AnimationId = 0
                };
                int size = Marshal.SizeOf(accent);
                IntPtr pAccent = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(accent, pAccent, false);

                var data = new WINDOWCOMPOSITIONATTRIBDATA
                {
                    Attribute = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY,
                    Data = pAccent,
                    SizeOfData = size
                };
                SetWindowCompositionAttribute(this.Handle, ref data);
                Marshal.FreeHGlobal(pAccent);
            }
            catch { /* ignore if not supported */ }
        }

        private void TryDisableAcrylic()
        {
            try
            {
                var accent = new ACCENT_POLICY { AccentState = ACCENT_STATE.ACCENT_DISABLED };
                int size = Marshal.SizeOf(accent);
                IntPtr pAccent = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(accent, pAccent, false);

                var data = new WINDOWCOMPOSITIONATTRIBDATA
                {
                    Attribute = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY,
                    Data = pAccent,
                    SizeOfData = size
                };
                SetWindowCompositionAttribute(this.Handle, ref data);
                Marshal.FreeHGlobal(pAccent);
            }
            catch { }
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WINDOWCOMPOSITIONATTRIBDATA data);

        private enum WINDOWCOMPOSITIONATTRIB
        {
            WCA_ACCENT_POLICY = 19
        }

        private enum ACCENT_STATE
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
            ACCENT_ENABLE_HOSTBACKDROP = 5
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ACCENT_POLICY
        {
            public ACCENT_STATE AccentState;
            public int AccentFlags;
            public uint GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWCOMPOSITIONATTRIBDATA
        {
            public WINDOWCOMPOSITIONATTRIB Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }
    }
}
