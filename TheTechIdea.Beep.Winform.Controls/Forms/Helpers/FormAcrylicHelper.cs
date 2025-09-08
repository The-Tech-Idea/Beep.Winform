using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Helper for managing Windows acrylic and blur effects.
    /// Provides OS-dependent blur/mica capabilities with graceful fallback.
    /// </summary>
    internal class FormAcrylicHelper : IDisposable
    {
        private readonly IBeepModernFormHost _host;
        private bool _disposed = false;
        private bool _acrylicEnabled = false;
        private bool _micaEnabled = false;
        private BackdropType _currentBackdrop = BackdropType.None;

        /// <summary>Backdrop types supported by Windows</summary>
        public enum BackdropType
        {
            None = 0,
            Blur = 1,
            Acrylic = 2,
            Mica = 3,
            Tabbed = 4,
            Transient = 5
        }

        /// <summary>Gets whether acrylic effect is currently enabled</summary>
        public bool IsAcrylicEnabled => _acrylicEnabled;

        /// <summary>Gets whether mica effect is currently enabled</summary>
        public bool IsMicaEnabled => _micaEnabled;

        /// <summary>Gets the current backdrop type</summary>
        public BackdropType CurrentBackdrop => _currentBackdrop;

        /// <summary>Gets whether the current OS supports acrylic effects</summary>
        public bool IsSupported => Environment.OSVersion.Version.Major >= 10;

        public FormAcrylicHelper(IBeepModernFormHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }

        /// <summary>
        /// Enables acrylic blur effect on the form.
        /// </summary>
        /// <param name="tintColor">Optional tint color for the acrylic effect</param>
        /// <returns>True if successful, false if not supported or failed</returns>
        public bool EnableAcrylic(Color? tintColor = null)
        {
            if (_disposed || !IsSupported || !_host.AsForm.IsHandleCreated)
                return false;

            try
            {
                DisableAll(); // Clear any existing effects

                var accent = new ACCENT_POLICY
                {
                    AccentState = ACCENT_STATE.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                    AccentFlags = 2,
                    GradientColor = (uint)((0xCC << 24) | 
                        ((tintColor?.B ?? _host.AsForm.BackColor.B) << 16) |
                        ((tintColor?.G ?? _host.AsForm.BackColor.G) << 8) |
                        (tintColor?.R ?? _host.AsForm.BackColor.R))
                };

                if (SetWindowCompositionAttribute(_host.AsForm.Handle, accent))
                {
                    _acrylicEnabled = true;
                    _currentBackdrop = BackdropType.Acrylic;
                    return true;
                }
            }
            catch (Exception)
            {
                // Ignore errors and fall back gracefully
            }

            return false;
        }

        /// <summary>
        /// Enables mica effect on the form (Windows 11+).
        /// </summary>
        /// <returns>True if successful, false if not supported or failed</returns>
        public bool EnableMica()
        {
            if (_disposed || !IsSupported || !_host.AsForm.IsHandleCreated)
                return false;

            try
            {
                DisableAll(); // Clear any existing effects

                int trueVal = 1;
                if (DwmSetWindowAttribute(_host.AsForm.Handle, DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT, ref trueVal, Marshal.SizeOf<int>()) == 0)
                {
                    _micaEnabled = true;
                    _currentBackdrop = BackdropType.Mica;
                    return true;
                }
            }
            catch (Exception)
            {
                // Ignore errors and fall back gracefully
            }

            return false;
        }

        /// <summary>
        /// Enables blur behind effect.
        /// </summary>
        /// <returns>True if successful, false if not supported or failed</returns>
        public bool EnableBlur()
        {
            if (_disposed || !IsSupported || !_host.AsForm.IsHandleCreated)
                return false;

            try
            {
                DisableAll(); // Clear any existing effects

                var accent = new ACCENT_POLICY
                {
                    AccentState = ACCENT_STATE.ACCENT_ENABLE_BLURBEHIND
                };

                if (SetWindowCompositionAttribute(_host.AsForm.Handle, accent))
                {
                    _currentBackdrop = BackdropType.Blur;
                    return true;
                }
            }
            catch (Exception)
            {
                // Ignore errors and fall back gracefully
            }

            return false;
        }

        /// <summary>
        /// Enables system backdrop effect (Windows 11+).
        /// </summary>
        /// <param name="backdropType">Type of backdrop to enable</param>
        /// <returns>True if successful, false if not supported or failed</returns>
        public bool EnableSystemBackdrop(BackdropType backdropType)
        {
            if (_disposed || !IsSupported || !_host.AsForm.IsHandleCreated)
                return false;

            try
            {
                DisableAll(); // Clear any existing effects

                int backdropValue = (int)backdropType;
                var attr = (DWMWINDOWATTRIBUTE)38; // DWMWA_SYSTEMBACKDROP_TYPE

                if (DwmSetWindowAttribute(_host.AsForm.Handle, attr, ref backdropValue, Marshal.SizeOf<int>()) == 0)
                {
                    _currentBackdrop = backdropType;
                    return true;
                }
            }
            catch (Exception)
            {
                // Ignore errors and fall back gracefully
            }

            return false;
        }

        /// <summary>
        /// Disables all backdrop effects.
        /// </summary>
        public void DisableAll()
        {
            if (_disposed || !_host.AsForm.IsHandleCreated)
                return;

            try
            {
                // Disable acrylic
                var accent = new ACCENT_POLICY
                {
                    AccentState = ACCENT_STATE.ACCENT_DISABLED
                };
                SetWindowCompositionAttribute(_host.AsForm.Handle, accent);

                // Disable mica
                int falseVal = 0;
                DwmSetWindowAttribute(_host.AsForm.Handle, DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT, ref falseVal, Marshal.SizeOf<int>());

                // Disable system backdrop
                int noneVal = 0;
                var attr = (DWMWINDOWATTRIBUTE)38; // DWMWA_SYSTEMBACKDROP_TYPE
                DwmSetWindowAttribute(_host.AsForm.Handle, attr, ref noneVal, Marshal.SizeOf<int>());
            }
            catch (Exception)
            {
                // Ignore errors
            }

            _acrylicEnabled = false;
            _micaEnabled = false;
            _currentBackdrop = BackdropType.None;
        }

        /// <summary>
        /// Reapplies the current backdrop effect (useful after theme changes).
        /// </summary>
        public void Reapply()
        {
            if (_disposed)
                return;

            var currentBackdrop = _currentBackdrop;
            DisableAll();

            // Reapply the effect
            switch (currentBackdrop)
            {
                case BackdropType.Acrylic:
                    EnableAcrylic();
                    break;
                case BackdropType.Mica:
                    EnableMica();
                    break;
                case BackdropType.Blur:
                    EnableBlur();
                    break;
                case BackdropType.Tabbed:
                case BackdropType.Transient:
                    EnableSystemBackdrop(currentBackdrop);
                    break;
            }
        }

        #region Win32 Interop

        private bool SetWindowCompositionAttribute(IntPtr hwnd, ACCENT_POLICY accent)
        {
            try
            {
                int size = Marshal.SizeOf(accent);
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(accent, ptr, false);

                var data = new WINDOWCOMPOSITIONATTRIBDATA
                {
                    Attribute = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY,
                    Data = ptr,
                    SizeOfData = size
                };

                int result = SetWindowCompositionAttribute(hwnd, ref data);
                Marshal.FreeHGlobal(ptr);
                
                return result != 0;
            }
            catch
            {
                return false;
            }
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WINDOWCOMPOSITIONATTRIBDATA data);

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);

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

        private enum DWMWINDOWATTRIBUTE
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_MICA_EFFECT = 1029
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

        #endregion

        public void Dispose()
        {
            if (!_disposed)
            {
                DisableAll();
                _disposed = true;
            }
        }
    }
}