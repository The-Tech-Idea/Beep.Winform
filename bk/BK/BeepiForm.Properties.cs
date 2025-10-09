using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Forms.Caption;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepiForm.Properties.cs - All Property Declarations
    /// </summary>
    public partial class BeepiForm
    {
        #region Core Properties
        [Browsable(true)]
        [Category("Appearance")]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                if (beepuiManager1 != null)
                    beepuiManager1.Title = value;
                Text = value;
                if (!InDesignHost) Text = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool ApplyThemeToChilds
        {
            get => _applythemetochilds;
            set => _applythemetochilds = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The Thickness of the form's border.")]
        [DefaultValue(3)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = value;
                MarkPathsDirty();
                if (!InDesignHost && UseHelperInfrastructure)
                    _state.RegionDirty = true;
                if (!InDesignHost && WindowState != FormWindowState.Maximized)
                    Padding = new Padding(Math.Max(0, _borderThickness));

                if (IsHandleCreated && WindowState != FormWindowState.Maximized)
                {
                    User32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0,
                        User32.SWP_NOMOVE | User32.SWP_NOSIZE | User32.SWP_NOZORDER | User32.SWP_FRAMECHANGED);
                    User32.RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, User32.RDW_FRAME | User32.RDW_INVALIDATE | User32.RDW_UPDATENOW);
                    if (UseHelperInfrastructure && _regionHelper != null)
                        _regionHelper.EnsureRegion(true);
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The radius of the form's border.")]
        [DefaultValue(5)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = Math.Max(0, value);
                MarkPathsDirty();
                if (!InDesignHost && UseHelperInfrastructure)
                {
                    _state.RegionDirty = true;
                    _regionHelper?.InvalidateRegion();
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool InPopMode
        {
            get => _inpopupmode;
            set
            {
                _inpopupmode = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [TypeConverter(typeof(ThemeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Theme
        {
            get => _theme;
            set
            {
                if (value != _theme)
                {
                    _theme = value;
                    if (!InDesignHost)
                    {
                        _currentTheme = BeepThemesManager.GetTheme(value);
                        ApplyTheme();
                    }
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Sets the color of the form's border.")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
                if (IsHandleCreated && WindowState != FormWindowState.Maximized)
                {
                    User32.RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, User32.RDW_FRAME | User32.RDW_INVALIDATE | User32.RDW_UPDATENOW);
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use theme colors instead of custom accent color.")]
        [DefaultValue(true)]
        public bool UseThemeColors
        {
            get => _useThemeColors;
            set
            {
                _useThemeColors = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual style/painter to use for rendering the sidebar.")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle Style
        {
            get => _controlstyle;
            set
            {
                if (_controlstyle != value)
                {
                    _controlstyle = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Style Properties
        [Category("Beep Style")]
        [DefaultValue(BeepFormStyle.Modern)]
        public BeepFormStyle FormStyle
        {
            get => _formStyle;
            set
            {
                if (_formStyle == value) return;
                _formStyle = value;
                try
                {
                    ApplyFormStyle();
                    _captionHelper?.SetStyle(value);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"FormStyle apply error: {ex.Message}");
                }
                Invalidate();
            }
        }

        [Category("Beep Style")]
        public Color ShadowColor
        {
            get => _shadowColor;
            set
            {
                if (_shadowColor != value)
                {
                    _shadowColor = value;
                    if (!InDesignHost) SyncStyleToHelpers();
                    Invalidate();
                }
            }
        }

        [Category("Beep Style")]
        [DefaultValue(6)]
        public int ShadowDepth
        {
            get => _shadowDepth;
            set
            {
                if (_shadowDepth != value)
                {
                    _shadowDepth = value;
                    if (!InDesignHost) SyncStyleToHelpers();
                    Invalidate();
                }
            }
        }

        [Category("Beep Style")]
        [DefaultValue(true)]
        public bool EnableGlow
        {
            get => _enableGlow;
            set
            {
                if (_enableGlow != value)
                {
                    _enableGlow = value;
                    if (!InDesignHost) SyncStyleToHelpers();
                    Invalidate();
                }
            }
        }

        [Category("Beep Style")]
        public Color GlowColor
        {
            get => _glowColor;
            set
            {
                if (_glowColor != value)
                {
                    _glowColor = value;
                    if (!InDesignHost) SyncStyleToHelpers();
                    Invalidate();
                }
            }
        }

        [Category("Beep Style")]
        [DefaultValue(8f)]
        public float GlowSpread
        {
            get => _glowSpread;
            set
            {
                if (Math.Abs(_glowSpread - value) > float.Epsilon)
                {
                    _glowSpread = Math.Max(0f, value);
                    if (!InDesignHost) SyncStyleToHelpers();
                    Invalidate();
                }
            }
        }

        [Category("Beep Style")]
        [DefaultValue(true)]
        [Description("If true, when the current theme is dark, renderer preset keys with .dark will be applied (if available).")]
        public bool AutoPickDarkPresets { get; set; } = true;

        [Category("Beep Style")]
        [DefaultValue(true)]
        [Description("If true, switching CaptionRenderer maps FormStyle and tries to apply a matching preset.")]
        public bool AutoApplyRendererPreset { get; set; } = true;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BeepFormStylePresets StylePresets { get; } = new();
        #endregion

        #region Logo Properties
        [Category("Beep Logo")]
        [Description("Path to the logo image file (SVG, PNG, JPG, etc.)")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string LogoImagePath
        {
            get => _captionHelper?.LogoImagePath ?? string.Empty;
            set
            {
                if (_captionHelper != null)
                {
                    _captionHelper.LogoImagePath = value;
                    Invalidate();
                }
            }
        }

        [Category("Beep Logo")]
        [DefaultValue(false)]
        [Description("Whether to show the logo on the form caption")]
        public bool ShowLogo
        {
            get => _captionHelper?.ShowLogo ?? false;
            set
            {
                if (_captionHelper != null) _captionHelper.ShowLogo = value;
            }
        }

        [Browsable(false)]
        [Description("Alias for ShowLogo - used by BeepFormUIManager")]
        public bool ShowIconInCaption
        {
            get => ShowLogo;
            set => ShowLogo = value;
        }

        [Category("Beep Logo")]
        [Description("Size of the logo icon")]
        public Size LogoSize
        {
            get => _captionHelper?.LogoSize ?? new Size(20, 20);
            set
            {
                if (_captionHelper != null) _captionHelper.LogoSize = value;
            }
        }

        [Category("Beep Logo")]
        [Description("Margin around the logo icon")]
        public Padding LogoMargin
        {
            get => _captionHelper?.LogoMargin ?? new Padding(8, 8, 8, 8);
            set
            {
                if (_captionHelper != null) _captionHelper.LogoMargin = value;
            }
        }
        #endregion

        #region Caption Properties
        [Category("Beep Caption")]
        [DefaultValue(true)]
        public bool ShowCaptionBar
        {
            get => _captionHelper?.ShowCaptionBar ?? _legacyShowCaptionBar;
            set
            {
                if (_captionHelper != null) _captionHelper.ShowCaptionBar = value;
                _legacyShowCaptionBar = value;
                Invalidate();
                if (IsHandleCreated) PerformLayout();
            }
        }

        [Category("Beep Caption")]
        [DefaultValue(36)]
        public int CaptionHeight
        {
            get => _captionHelper?.CaptionHeight ?? _legacyCaptionHeight;
            set
            {
                if (value < 24) value = 24;
                if (_captionHelper != null) _captionHelper.CaptionHeight = value;
                _legacyCaptionHeight = value;
                Invalidate();
                if (IsHandleCreated) PerformLayout();
            }
        }

        [Category("Beep Caption")]
        [DefaultValue(true)]
        public bool ShowSystemButtons
        {
            get => _captionHelper?.ShowSystemButtons ?? _legacyShowSystemButtons;
            set
            {
                if (_captionHelper != null) _captionHelper.ShowSystemButtons = value;
                _legacyShowSystemButtons = value;
                Invalidate();
            }
        }

        [Category("Beep Caption")]
        [DefaultValue(true)]
        public bool EnableCaptionGradient
        {
            get => _captionHelper?.EnableCaptionGradient ?? _legacyEnableCaptionGradient;
            set
            {
                if (_captionHelper != null) _captionHelper.EnableCaptionGradient = value;
                _legacyEnableCaptionGradient = value;
                Invalidate();
            }
        }

        [Category("Beep Caption")]
        [Description("Padding around the caption content")]
        public Padding CaptionPadding
        {
            get => _captionHelper?.LogoMargin ?? new Padding(8, 8, 8, 8);
            set
            {
                if (_captionHelper != null) _captionHelper.LogoMargin = value;
                Invalidate();
            }
        }

        [Category("Beep Caption")]
        [DefaultValue(false)]
        [Description("Enable Windows 11 immersive dark mode for the window")]
        public bool UseImmersiveDarkMode
        {
            get => _useImmersiveDarkMode;
            set
            {
                if (_useImmersiveDarkMode != value)
                {
                    _useImmersiveDarkMode = value;
                    if (!InDesignHost && IsHandleCreated) EnableModernWindowFeatures();
                }
            }
        }

        [Category("Beep Caption")]
        [DefaultValue(CaptionRendererKind.Windows)]
        [TypeConverter(typeof(TheTechIdea.Beep.Winform.Controls.Forms.Caption.Design.CaptionRendererKindConverter))]
        [Editor(typeof(TheTechIdea.Beep.Winform.Controls.Forms.Caption.Design.CaptionRendererKindEditor), typeof(UITypeEditor))]
        [Obsolete("Use FormStyle property instead. CaptionRenderer is deprecated and will be removed in a future version.")]
        public CaptionRendererKind CaptionRenderer
        {
            get => _captionHelper?.RendererKind ?? CaptionRendererKind.Windows;
            set
            {
                _captionHelper?.SwitchRenderer(value);
                Invalidate();
            }
        }

        [Category("Beep Caption")]
        [DefaultValue(false)]
        public bool ShowThemeButton
        {
            get => _captionHelper?.ShowThemeButton ?? false;
            set
            {
                if (_captionHelper != null)
                {
                    _captionHelper.ShowThemeButton = value;
                    Invalidate();
                }
            }
        }

        [Category("Beep Caption")]
        [DefaultValue(false)]
        public bool ShowStyleButton
        {
            get => _captionHelper?.ShowStyleButton ?? false;
            set
            {
                if (_captionHelper != null)
                {
                    _captionHelper.ShowStyleButton = value;
                    Invalidate();
                }
            }
        }

        [Category("Beep Caption")]
        [Description("Icon path for the Theme button (optional)")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string ThemeButtonIconPath
        {
            get => _captionHelper?.ThemeButtonIconPath ?? string.Empty;
            set
            {
                if (_captionHelper != null)
                {
                    _captionHelper.ThemeButtonIconPath = value;
                    Invalidate();
                }
            }
        }

        [Category("Beep Caption")]
        [Description("Icon path for the Style button (optional)")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string StyleButtonIconPath
        {
            get => _captionHelper?.StyleButtonIconPath ?? string.Empty;
            set
            {
                if (_captionHelper != null)
                {
                    _captionHelper.StyleButtonIconPath = value;
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(CaptionBarExtraButtonKind.None)]
        public CaptionBarExtraButtonKind CaptionExtraButton
        {
            get
            {
                if ((_captionHelper?.ShowThemeButton ?? false) && !(_captionHelper?.ShowStyleButton ?? false))
                    return CaptionBarExtraButtonKind.Themes;
                if (!(_captionHelper?.ShowThemeButton ?? false) && (_captionHelper?.ShowStyleButton ?? false))
                    return CaptionBarExtraButtonKind.StyleToggle;
                return CaptionBarExtraButtonKind.None;
            }
            set
            {
                if (_captionHelper == null) return;
                _captionHelper.ShowThemeButton = value == CaptionBarExtraButtonKind.Themes;
                _captionHelper.ShowStyleButton = value == CaptionBarExtraButtonKind.StyleToggle;
                Invalidate();
            }
        }
        #endregion

        #region Animation Properties
        [Category("Beep Animations")]
        [DefaultValue(true)]
        public bool AnimateMaximizeRestore
        {
            get => _animateMaximizeRestore;
            set => _animateMaximizeRestore = value;
        }

        [Category("Beep Animations")]
        [DefaultValue(true)]
        public bool AnimateStyleChange
        {
            get => _animateStyleChange;
            set => _animateStyleChange = value;
        }
        #endregion

        #region Backdrop Properties
        [Category("Beep Backdrop")]
        [DefaultValue(true)]
        public bool EnableAcrylicForGlass
        {
            get => _enableAcrylicForGlass;
            set
            {
                _enableAcrylicForGlass = value;
                if (!InDesignHost && IsHandleCreated) ApplyAcrylicEffectIfNeeded();
            }
        }

        [Category("Beep Backdrop")]
        [DefaultValue(false)]
        public bool EnableMicaBackdrop
        {
            get => _enableMicaBackdrop;
            set
            {
                _enableMicaBackdrop = value;
                if (!InDesignHost && IsHandleCreated) ApplyMicaBackdropIfNeeded();
            }
        }

        [Category("Beep Backdrop")]
        [DefaultValue(BackdropType.None)]
        public BackdropType Backdrop
        {
            get => _backdrop;
            set
            {
                _backdrop = value;
                if (!InDesignHost && IsHandleCreated) ApplyBackdrop();
            }
        }
        #endregion

        #region Snap Hints Properties
        [Category("Beep Snap Hints")]
        [DefaultValue(true)]
        public bool ShowSnapHints
        {
            get => _showSnapHints;
            set
            {
                _showSnapHints = value;
                Invalidate();
            }
        }
        #endregion

        #region DPI Properties
        [Browsable(true)]
        [Category("DPI/Scaling")]
        [Description("How DPI awareness and scaling are handled. Framework = let WinForms manage DPI. Manual = use BeepiForm's custom handling.")]
        public DpiHandlingMode DpiMode { get; set; } = DpiHandlingMode.Framework;

        public enum DpiHandlingMode
        {
            Framework,
            Manual
        }
        #endregion

        #region Border Drawing Property
        [Category("Beep Border")]
        [DefaultValue(true)]
        [Description("Enable custom non-client border drawing")]
        public bool DrawCustomWindowBorder
        {
            get => _drawCustomWindowBorder;
            set
            {
                _drawCustomWindowBorder = value;
                if (IsHandleCreated && !InDesignHost)
                {
                    User32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0,
                        User32.SWP_NOMOVE | User32.SWP_NOSIZE | User32.SWP_NOZORDER | User32.SWP_FRAMECHANGED);
                    User32.RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero,
                        User32.RDW_FRAME | User32.RDW_INVALIDATE | User32.RDW_UPDATENOW);
                }
            }
        }
        #endregion
    }
}
