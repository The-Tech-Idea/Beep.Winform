using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Forms.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepiForm.Style.cs - Theme and Style Application Methods
    /// </summary>
    public partial class BeepiForm
    {
        #region Apply Theme
        public virtual void ApplyTheme()
        {
            if (InDesignHost)
            {
                Invalidate();
                return;
            }

            SuspendLayout();
            try
            {
                Color newBackColor = _currentTheme?.BackColor ?? SystemColors.Control;
                if (newBackColor == Color.Transparent || newBackColor == Color.Empty)
                    newBackColor = SystemColors.Control;

                BackColor = newBackColor;
                BorderColor = _currentTheme?.BorderColor ?? SystemColors.ControlDark;

                _captionHelper?.UpdateTheme();
                ApplyFormStyle();
            }
            finally
            {
                ResumeLayout(true);
                Invalidate();

                // Repaint non-client area (border) when theme changes
                if (IsHandleCreated && WindowState != FormWindowState.Maximized)
                {
                    User32.RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero,
                        User32.RDW_FRAME | User32.RDW_INVALIDATE | User32.RDW_UPDATENOW);
                }
                Update();
            }
        }
        #endregion

        #region Apply Form Style
        protected void ApplyFormStyle()
        {
            if (InDesignHost)
            {
                Invalidate();
                return;
            }

            // 1) Pull all structural metrics (including BorderRadius/BorderThickness) from defaults
            ApplyMetrics(_formStyle);

            // 2) Apply style-specific visual tweaks (no radius/thickness overrides here)
            switch (_formStyle)
            {
                case BeepFormStyle.Modern:
                    ApplyThemeMapping();
                    // Disable glow for Modern to avoid any colored halo; keep clean border only
                    _enableGlow = false;
                    _glowColor = Color.Transparent;
                    break;

                case BeepFormStyle.Metro:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(80, 0, 100, 200);
                    break;

                case BeepFormStyle.Glass:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(120, 255, 255, 255);
                    break;

                case BeepFormStyle.Office:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(90, 50, 100, 200);
                    break;

                case BeepFormStyle.ModernDark:
                    _glowColor = Color.FromArgb(80, 0, 0, 0);
                    BackColor = Color.FromArgb(32, 32, 32);
                    BorderColor = Color.FromArgb(64, 64, 64);
                    break;

                case BeepFormStyle.Material:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(60, 0, 0, 0);
                    BorderColor = Color.Transparent;
                    break;

                case BeepFormStyle.Minimal:
                    ApplyThemeMapping();
                    break;

                case BeepFormStyle.Classic:
                    BackColor = SystemColors.Control;
                    BorderColor = SystemColors.ActiveBorder;
                    break;

                case BeepFormStyle.Gnome:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(60, 100, 100, 100);
                    break;

                case BeepFormStyle.Kde:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(80, 0, 120, 200);
                    break;

                case BeepFormStyle.Cinnamon:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(90, 200, 100, 50);
                    break;

                case BeepFormStyle.Elementary:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(70, 150, 150, 150);
                    break;

                case BeepFormStyle.Fluent:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(120, 0, 120, 255);
                    break;

                case BeepFormStyle.NeoBrutalist:
                    BackColor = Color.White;
                    BorderColor = Color.Black;
                    _glowColor = Color.Transparent;
                    break;

                case BeepFormStyle.Neon:
                    BackColor = Color.FromArgb(20, 20, 20);
                    BorderColor = Color.FromArgb(0, 255, 255);
                    _glowColor = Color.FromArgb(150, 0, 255, 255);
                    break;

                case BeepFormStyle.Retro:
                    BackColor = Color.FromArgb(45, 25, 45);
                    BorderColor = Color.FromArgb(255, 100, 200);
                    _glowColor = Color.FromArgb(120, 255, 50, 150);
                    break;

                case BeepFormStyle.Gaming:
                    BackColor = Color.FromArgb(15, 15, 25);
                    BorderColor = Color.FromArgb(0, 255, 0);
                    _glowColor = Color.FromArgb(100, 0, 255, 0);
                    break;

                case BeepFormStyle.Corporate:
                    BackColor = Color.FromArgb(248, 248, 248);
                    BorderColor = Color.FromArgb(180, 180, 180);
                    _glowColor = Color.FromArgb(40, 100, 100, 100);
                    break;

                case BeepFormStyle.Artistic:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(140, 200, 50, 150);
                    break;

                case BeepFormStyle.HighContrast:
                    BackColor = Color.Black;
                    BorderColor = Color.White;
                    _glowColor = Color.Transparent;
                    break;

                case BeepFormStyle.Soft:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(80, 200, 200, 255);
                    break;

                case BeepFormStyle.Industrial:
                    BackColor = Color.FromArgb(60, 60, 70);
                    BorderColor = Color.FromArgb(120, 120, 130);
                    _glowColor = Color.FromArgb(100, 200, 200, 200);
                    break;

                case BeepFormStyle.Custom:
                    ApplyThemeMapping();
                    break;
            }

            // 3) Push to helpers and region
            SyncStyleToHelpers();
            UpdateLogoPainterTheme();
            ApplyAcrylicEffectIfNeeded();

            // Ensure padding reflects border thickness when not maximized
            if (WindowState != FormWindowState.Maximized)
                Padding = new Padding(Math.Max(0, _borderThickness));

            if (UseHelperInfrastructure && _regionHelper != null)
                _regionHelper.EnsureRegion(true);

            if (IsHandleCreated && _animateStyleChange)
                _ = AnimateOpacityAsync(0.8, 1.0, 200);

            MarkPathsDirty();
            Invalidate();
        }
        #endregion

        #region Style Helper Methods
        private void SyncStyleToHelpers()
        {
            if (InDesignHost || !UseHelperInfrastructure || _shadowGlow == null)
                return;

            _shadowGlow.ShadowColor = _shadowColor;
            _shadowGlow.ShadowDepth = _shadowDepth;
            _shadowGlow.EnableGlow = _enableGlow;
            _shadowGlow.GlowColor = _glowColor;
            _shadowGlow.GlowSpread = _glowSpread;
        }

        private void ApplyMetrics(BeepFormStyle style)
        {
            if (InDesignHost) return;

            if (!BeepFormStyleMetricsDefaults.Map.TryGetValue(style, out var m))
                return;

            _borderRadius = m.BorderRadius;
            // Don't override user's BorderThickness: _borderThickness = m.BorderThickness;
            _shadowDepth = m.ShadowDepth;
            _enableGlow = m.EnableGlow;
            _glowSpread = m.GlowSpread;

            SyncStyleToHelpers();
        }

        private void ApplyThemeMapping()
        {
            if (InDesignHost || _currentTheme == null) return;

            BackColor = _currentTheme.ButtonBackColor;
            BorderColor = _currentTheme.BorderColor;
        }

        private void UpdateLogoPainterTheme()
        {
            if (_captionHelper != null && !string.IsNullOrEmpty(_captionHelper.LogoImagePath))
            {
                try
                {
                    _captionHelper.LogoImagePath = _captionHelper.LogoImagePath;
                }
                catch { }
            }
        }

        public void ApplyPreset(string key)
        {
            if (StylePresets.TryGet(key, out var m))
            {
                _borderRadius = m.BorderRadius;
                _borderThickness = m.BorderThickness;
                _shadowDepth = m.ShadowDepth;
                _enableGlow = m.EnableGlow;
                _glowSpread = m.GlowSpread;

                if (!InDesignHost) SyncStyleToHelpers();
                Invalidate();
            }
        }
        #endregion

        #region Maximize Helpers
        public void ToggleMaximize()
        {
            if (InDesignHost) return;
            WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        }

        private void ApplyMaximizedWindowFix()
        {
            if (InDesignHost) return;

            if (WindowState == FormWindowState.Maximized)
            {
                _savedBorderRadius = _borderRadius;
                _savedBorderThickness = _borderThickness;
                _borderRadius = 0;
                _borderThickness = 0;
                Padding = new Padding(0);
                Region = null;
            }
            else
            {
                _borderRadius = _savedBorderRadius;
                _borderThickness = _savedBorderThickness;
                Padding = new Padding(Math.Max(0, _borderThickness));
            }
        }
        #endregion

        #region Animations
        private async Task AnimateOpacityAsync(double from, double to, int durationMs)
        {
            if (InDesignHost) return;

            try
            {
                double start = from, end = to;
                int steps = 10;
                double delta = (end - start) / steps;
                int delay = Math.Max(8, durationMs / steps);

                Opacity = Math.Clamp(start, 0, 1);

                for (int i = 0; i < steps; i++)
                {
                    await Task.Delay(delay);
                    Opacity = Math.Clamp(Opacity + delta, 0, 1);
                }

                Opacity = Math.Clamp(end, 0, 1);
            }
            catch { }
        }
        #endregion

        #region Backdrop Methods
        private void ApplyAcrylicEffectIfNeeded()
        {
            if (InDesignHost || !IsHandleCreated) return;

            if (_formStyle == BeepFormStyle.Glass && _enableAcrylicForGlass)
                TryEnableAcrylic();
            else
                TryDisableAcrylic();
        }

        private void ApplyMicaBackdropIfNeeded()
        {
            if (InDesignHost || !IsHandleCreated) return;

            if (_enableMicaBackdrop)
                TryEnableMica();
            else
                TryDisableMica();
        }

        private void ApplyBackdrop()
        {
            if (InDesignHost) return;

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
                    TryEnableSystemBackdrop(3);
                    break;
                case BackdropType.Transient:
                    TryEnableSystemBackdrop(2);
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
            if (InDesignHost) return;
            try
            {
                var attr = (DwmApi.DWMWINDOWATTRIBUTE)38;
                DwmApi.SetWindowAttribute(this.Handle, attr, ref type, System.Runtime.InteropServices.Marshal.SizeOf<int>());
            }
            catch { }
        }

        private void TryEnableBlurBehind()
        {
            if (InDesignHost) return;
            try
            {
                DwmApi.EnableBlurBehind(Handle);
            }
            catch { }
        }

        private void TryEnableAcrylic()
        {
            if (InDesignHost) return;
            try
            {
                DwmApi.EnableAcrylic(Handle, BackColor);
            }
            catch { }
        }

        private void TryDisableAcrylic()
        {
            if (InDesignHost) return;
            try
            {
                DwmApi.DisableAcrylic(Handle);
            }
            catch { }
        }

        private void TryEnableMica()
        {
            if (InDesignHost) return;
            try
            {
                DwmApi.SetMicaBackdrop(Handle, true);
            }
            catch { }
        }

        private void TryDisableMica()
        {
            if (InDesignHost) return;
            try
            {
                DwmApi.SetMicaBackdrop(Handle, false);
            }
            catch { }
        }
        #endregion

        #region Region Methods
        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                if (rect.Width > 0 && rect.Height > 0)
                    path.AddRectangle(rect);
                return path;
            }

            int diameter = Math.Min(rect.Width, rect.Height);
            diameter = Math.Min(diameter, radius * 2);

            if (diameter <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            try
            {
                Rectangle arcRect = new Rectangle(rect.X, rect.Y, diameter, diameter);
                path.AddArc(arcRect, 180, 90);

                arcRect.X = rect.Right - diameter;
                path.AddArc(arcRect, 270, 90);

                arcRect.Y = rect.Bottom - diameter;
                path.AddArc(arcRect, 0, 90);

                arcRect.X = rect.Left;
                path.AddArc(arcRect, 90, 90);

                path.CloseFigure();
            }
            catch (ArgumentException)
            {
                path.Reset();
                if (rect.Width > 0 && rect.Height > 0)
                    path.AddRectangle(rect);
            }

            return path;
        }
        #endregion
    }
}
