using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    /// <summary>
    /// Backdrop effects support for BeepiFormPro.
    /// Provides Acrylic, Mica, Blur, Tabbed, and Transient backdrop effects using Windows APIs.
    /// </summary>
    public partial class BeepiFormPro
    {
        #region Backdrop Fields
        private BackdropType _backdrop = BackdropType.None;
        private bool _enableAcrylicForGlass = true;
        private bool _enableMicaBackdrop = false;
        #endregion

        #region Backdrop Properties

        /// <summary>
        /// Gets or sets the backdrop type for the form.
        /// Supports None, Blur, Acrylic, Mica, Tabbed, and Transient effects.
        /// </summary>
        [System.ComponentModel.Category("Beep Backdrop")]
        [System.ComponentModel.DefaultValue(BackdropType.None)]
        [System.ComponentModel.Description("Type of backdrop effect to apply to the form")]
        public BackdropType Backdrop
        {
            get => _backdrop;
            set
            {
                if (_backdrop != value)
                {
                    _backdrop = value;
                    if (IsHandleCreated)
                    {
                        ApplyBackdrop();
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to enable Acrylic effect when FormStyle is Glass.
        /// </summary>
        [System.ComponentModel.Category("Beep Backdrop")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("Enable Acrylic effect when FormStyle is set to Glass")]
        public bool EnableAcrylicForGlass
        {
            get => _enableAcrylicForGlass;
            set
            {
                if (_enableAcrylicForGlass != value)
                {
                    _enableAcrylicForGlass = value;
                    if (IsHandleCreated)
                    {
                        ApplyAcrylicEffectIfNeeded();
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to enable Windows 11 Mica backdrop.
        /// </summary>
        [System.ComponentModel.Category("Beep Backdrop")]
        [System.ComponentModel.DefaultValue(false)]
        [System.ComponentModel.Description("Enable Windows 11 Mica backdrop effect")]
        public bool EnableMicaBackdrop
        {
            get => _enableMicaBackdrop;
            set
            {
                if (_enableMicaBackdrop != value)
                {
                    _enableMicaBackdrop = value;
                    if (IsHandleCreated)
                    {
                        ApplyMicaBackdropIfNeeded();
                        Invalidate();
                    }
                }
            }
        }

        #endregion

        #region Backdrop Application Methods

        /// <summary>
        /// Applies Acrylic effect if FormStyle is Glass and EnableAcrylicForGlass is true.
        /// </summary>
        private void ApplyAcrylicEffectIfNeeded()
        {
            if (!IsHandleCreated) return;

            try
            {
                if ( _enableAcrylicForGlass)
                {
                    TryEnableAcrylic();
                }
                else
                {
                    TryDisableAcrylic();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to apply Acrylic effect: {ex.Message}");
            }
        }

        /// <summary>
        /// Applies Mica backdrop if EnableMicaBackdrop is true.
        /// </summary>
        private void ApplyMicaBackdropIfNeeded()
        {
            if (!IsHandleCreated) return;

            try
            {
                if (_enableMicaBackdrop)
                {
                    TryEnableMica();
                }
                else
                {
                    TryDisableMica();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to apply Mica backdrop: {ex.Message}");
            }
        }

        /// <summary>
        /// Applies the currently selected backdrop type.
        /// </summary>
        private void ApplyBackdrop()
        {
            if (!IsHandleCreated) return;

            try
            {
                switch (_backdrop)
                {
                    case BackdropType.Mica:
                        TryEnableMica();
                        break;

                    case BackdropType.Acrylic:
                        TryEnableAcrylic();
                        break;

                    case BackdropType.Tabbed:
                        TryEnableSystemBackdrop(2); // DWMSBT_TABBEDWINDOW
                        break;

                    case BackdropType.Transient:
                        TryEnableSystemBackdrop(3); // DWMSBT_TRANSIENTWINDOW
                        break;

                    case BackdropType.Blur:
                        TryEnableBlurBehind();
                        break;

                    case BackdropType.None:
                    default:
                        TryDisableAcrylic();
                        TryDisableMica();
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to apply backdrop: {ex.Message}");
            }
        }

        /// <summary>
        /// Override OnHandleCreated to apply backdrop effects when window handle is created.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // Update DPI scale AFTER handle is created and form is initialized
            // This ensures we don't interfere with AutoScale initialization
            UpdateDpiScale();
            // Apply backdrop effects
            //ApplyBackdrop();
            //ApplyAcrylicEffectIfNeeded();
            //ApplyMicaBackdropIfNeeded();
        }

        #endregion

        #region Windows API Implementation

        /// <summary>
        /// Tries to enable a system backdrop type (Windows 11 22H2+).
        /// </summary>
        /// <param name="type">Backdrop type: 1=Auto, 2=None, 3=MainWindow, 4=TransientWindow, 5=TabbedWindow</param>
        private void TryEnableSystemBackdrop(int type)
        {
            if (!IsHandleCreated) return;

            try
            {
                int value = type;
                DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref value, sizeof(int));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to enable system backdrop: {ex.Message}");
            }
        }

        /// <summary>
        /// Enables blur-behind effect using DWM composition.
        /// </summary>
        private void TryEnableBlurBehind()
        {
            if (!IsHandleCreated) return;

            try
            {
                var accent = new ACCENT_POLICY
                {
                    AccentState = ACCENT_STATE.ACCENT_ENABLE_BLURBEHIND,
                    AccentFlags = 0,
                    GradientColor = 0,
                    AnimationId = 0
                };

                var accentStructSize = Marshal.SizeOf(accent);
                var accentPtr = Marshal.AllocHGlobal(accentStructSize);
                Marshal.StructureToPtr(accent, accentPtr, false);

                var data = new WINDOWCOMPOSITIONATTRIBDATA
                {
                    Attribute = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY,
                    Data = accentPtr,
                    SizeOfData = accentStructSize
                };

                SetWindowCompositionAttribute(Handle, ref data);
                Marshal.FreeHGlobal(accentPtr);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to enable blur-behind: {ex.Message}");
            }
        }

        /// <summary>
        /// Enables Acrylic effect (Windows 10 Fall Creators Update+).
        /// </summary>
        private void TryEnableAcrylic()
        {
            if (!IsHandleCreated) return;

            try
            {
                // Acrylic with slight tint
                var accent = new ACCENT_POLICY
                {
                    AccentState = ACCENT_STATE.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                    AccentFlags = 2, // Enable gradient
                    GradientColor = 0x01000000, // Very subtle dark tint (ABGR format)
                    AnimationId = 0
                };

                var accentStructSize = Marshal.SizeOf(accent);
                var accentPtr = Marshal.AllocHGlobal(accentStructSize);
                Marshal.StructureToPtr(accent, accentPtr, false);

                var data = new WINDOWCOMPOSITIONATTRIBDATA
                {
                    Attribute = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY,
                    Data = accentPtr,
                    SizeOfData = accentStructSize
                };

                SetWindowCompositionAttribute(Handle, ref data);
                Marshal.FreeHGlobal(accentPtr);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to enable Acrylic: {ex.Message}");
            }
        }

        /// <summary>
        /// Disables Acrylic effect.
        /// </summary>
        private void TryDisableAcrylic()
        {
            if (!IsHandleCreated) return;

            try
            {
                var accent = new ACCENT_POLICY
                {
                    AccentState = ACCENT_STATE.ACCENT_DISABLED,
                    AccentFlags = 0,
                    GradientColor = 0,
                    AnimationId = 0
                };

                var accentStructSize = Marshal.SizeOf(accent);
                var accentPtr = Marshal.AllocHGlobal(accentStructSize);
                Marshal.StructureToPtr(accent, accentPtr, false);

                var data = new WINDOWCOMPOSITIONATTRIBDATA
                {
                    Attribute = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY,
                    Data = accentPtr,
                    SizeOfData = accentStructSize
                };

                SetWindowCompositionAttribute(Handle, ref data);
                Marshal.FreeHGlobal(accentPtr);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to disable Acrylic: {ex.Message}");
            }
        }

        /// <summary>
        /// Enables Windows 11 Mica backdrop effect.
        /// </summary>
        private void TryEnableMica()
        {
            if (!IsHandleCreated) return;

            try
            {
                int value = 1;
                DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT, ref value, sizeof(int));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to enable Mica: {ex.Message}");
            }
        }

        /// <summary>
        /// Disables Windows 11 Mica backdrop effect.
        /// </summary>
        private void TryDisableMica()
        {
            if (!IsHandleCreated) return;

            try
            {
                int value = 0;
                DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT, ref value, sizeof(int));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to disable Mica: {ex.Message}");
            }
        }

        #endregion

        #region Windows API Declarations

        /// <summary>
        /// Sets window composition attributes for backdrop effects.
        /// </summary>
        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WINDOWCOMPOSITIONATTRIBDATA data);

        /// <summary>
        /// Sets DWM (Desktop Window Manager) window attributes.
        /// </summary>
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);

        /// <summary>
        /// Window composition attribute types.
        /// </summary>
        private enum WINDOWCOMPOSITIONATTRIB
        {
            WCA_ACCENT_POLICY = 19,
            WCA_USEDARKMODECOLORS = 26
        }

        /// <summary>
        /// Accent state for window composition.
        /// </summary>
        private enum ACCENT_STATE
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
            ACCENT_ENABLE_HOSTBACKDROP = 5
        }

        /// <summary>
        /// Accent policy structure for window composition.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct ACCENT_POLICY
        {
            public ACCENT_STATE AccentState;
            public int AccentFlags;
            public uint GradientColor;
            public int AnimationId;
        }

        /// <summary>
        /// Window composition attribute data structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWCOMPOSITIONATTRIBDATA
        {
            public WINDOWCOMPOSITIONATTRIB Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        /// <summary>
        /// DWM window attributes for modern Windows effects.
        /// </summary>
        private enum DWMWINDOWATTRIBUTE
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_MICA_EFFECT = 1029,
            DWMWA_SYSTEMBACKDROP_TYPE = 38
        }

        #endregion
    }
}
