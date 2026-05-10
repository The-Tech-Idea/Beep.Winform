using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro
    {
        protected IBeepTheme? _currentTheme; // nullable; resolved on demand
        private bool _globalThemeEventsRegistered;
        private bool _isApplyingGlobalThemeStyle;

        public string ThemeName { get; private set; } = "DefaultTheme";

        private void ApplyResolvedTheme(IBeepTheme? resolvedTheme)
        {
            var appliedTheme = resolvedTheme ?? BeepThemesManager.GetDefaultTheme();
            if (appliedTheme == null)
                return;

            _currentTheme = appliedTheme;
            _theme = appliedTheme.ThemeName;
            ThemeName = appliedTheme.ThemeName;

            try { PaintersFactory.ClearCache(); } catch { }
            try { FormPainterMetrics.InvalidateCache(); } catch { }

            ApplyTheme();
        }

        public IBeepTheme CurrentTheme
        {
            get
            {
                if (_currentTheme == null)
                {
                    // Lazy load the theme based on the ThemeName property
                    _currentTheme = BeepThemesManager.GetTheme(ThemeName);
                    if (_currentTheme == null)
                    {
                        // Fallback to a default theme if not found
                        _currentTheme = BeepThemesManager.GetDefaultTheme();
                    }
                }
                return _currentTheme;
            }
            set
            {
                ApplyResolvedTheme(value);
            }
        }

       
        public virtual void ApplyTheme()
        {
            var appliedTheme = CurrentTheme;

            _formpaintermaterics = null;
            InvalidateLayout();
            BackColor = FormPainterMetrics.DefaultForCached(FormStyle, UseThemeColors ? appliedTheme : null).BackgroundColor;

          // pass theme to controls or apply theme-specific settings here
            foreach (var control in this.Controls)
            {
                if (control is IBeepUIComponent themableControl)
                {
                    themableControl.Theme = appliedTheme.ThemeName;
                }
            }
            Invalidate(true); // Redraw with new theme
        }
        public virtual void ApplyTheme(string themeName)
        {
            var theme = BeepThemesManager.GetTheme(themeName);
            if (theme != null)
            {
                ApplyResolvedTheme(theme);
            }
            else
            {
                throw new ArgumentException($"Theme '{themeName}' not found.");
            }
        }

        private void InitializeGlobalThemeSynchronization()
        {
            if (InDesignModeSafe)
            {
                return;
            }

            RegisterGlobalThemeEvents();
            ApplyGlobalThemeAndStyleSafe(BeepThemesManager.CurrentThemeName, BeepThemesManager.CurrentStyle);
        }

        private void RegisterGlobalThemeEvents()
        {
            if (_globalThemeEventsRegistered || InDesignModeSafe)
            {
                return;
            }

            BeepThemesManager.ThemeChanged += OnGlobalThemeChanged;
            BeepThemesManager.FormStyleChanged += OnGlobalFormStyleChanged;
            _globalThemeEventsRegistered = true;
        }

        private void UnregisterGlobalThemeEvents()
        {
            if (!_globalThemeEventsRegistered)
            {
                return;
            }

            BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
            BeepThemesManager.FormStyleChanged -= OnGlobalFormStyleChanged;
            _globalThemeEventsRegistered = false;
        }

        private void OnGlobalThemeChanged(object? sender, ThemeChangeEventArgs e)
        {
            ApplyGlobalThemeAndStyleSafe(e.NewThemeName, BeepThemesManager.CurrentStyle);
        }

        private void OnGlobalFormStyleChanged(object? sender, StyleChangeEventArgs e)
        {
            ApplyGlobalThemeAndStyleSafe(BeepThemesManager.CurrentThemeName, e.NewStyle);
        }

        private void ApplyGlobalThemeAndStyleSafe(string? themeName = null, FormStyle? formStyle = null)
        {
            if (IsDisposed || Disposing)
            {
                return;
            }

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke((System.Windows.Forms.MethodInvoker)(() =>
                        ApplyGlobalThemeAndStyle(themeName, formStyle)));
                }
                catch
                {
                    // Best effort only while closing/disposing.
                }
                return;
            }

            ApplyGlobalThemeAndStyle(themeName, formStyle);
        }

        private void ApplyGlobalThemeAndStyle(string? themeName, FormStyle? formStyle)
        {
            if (_isApplyingGlobalThemeStyle || IsDisposed || Disposing)
            {
                return;
            }

            _isApplyingGlobalThemeStyle = true;
            try
            {
                var resolvedTheme = string.IsNullOrWhiteSpace(themeName)
                    ? BeepThemesManager.CurrentThemeName
                    : themeName;
                var resolvedStyle = formStyle ?? BeepThemesManager.CurrentStyle;

                if (!string.IsNullOrWhiteSpace(resolvedTheme))
                {
                    Theme = resolvedTheme;
                }

                FormStyle = resolvedStyle;
                InvalidateLayout();
                PerformLayout();
                Invalidate(true);
            }
            finally
            {
                _isApplyingGlobalThemeStyle = false;
            }
        }
    }
}
