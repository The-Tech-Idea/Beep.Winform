using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    // Minimal wrapper for DWM-related features used by BeepiForm.Style
    internal static class DwmApi
    {
        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_SYSTEMBACKDROP_TYPE = 38
        }

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);

        // Expose a safe wrapper with the same signature style used in BeepiForm.Style
        public static void SetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref int value, int size)
        {
            try { _ = DwmSetWindowAttribute(hwnd, attr, ref value, size); } catch { }
        }

        // The below helpers are stubs; they attempt to enable effects if available, otherwise no-op
        public static void EnableBlurBehind(IntPtr hwnd)
        {
            // Could P/Invoke DwmEnableBlurBehindWindow; no-op if unavailable
            try { } catch { }
        }

        public static void EnableAcrylic(IntPtr hwnd, Color baseColor)
        {
            // Acrylic requires Windows 10+ with specific APIs; keep as no-op stub
            try { } catch { }
        }

        public static void DisableAcrylic(IntPtr hwnd)
        {
            try { } catch { }
        }

        public static void SetMicaBackdrop(IntPtr hwnd, bool enable)
        {
            try
            {
                int val = enable ? 1 : 0;
                SetWindowAttribute(hwnd, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref val, Marshal.SizeOf<int>());
            }
            catch { }
        }
    }
}
