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

        public string ThemeName { get; private set; }

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
                _currentTheme = value;
                ThemeName = value.ThemeName; // Keep ThemeName in sync

                // Clear painter caches when theme changes so cached brushes/rasters are regenerated
                try { PaintersFactory.ClearCache(); } catch { }

                Invalidate(); // Redraw with new theme
            }
        }

       
        public virtual void ApplyTheme()
        {
            if (_currentTheme == null)
            {
                return;
            }

          // pass theme to controls or apply theme-specific settings here
            foreach (var control in this.Controls)
            {
                if (control is IBeepUIComponent themableControl)
                {
                    themableControl.Theme=Theme;
                }
            }
            Invalidate(); // Redraw with new theme
        }
        public virtual void ApplyTheme(string themeName)
        {
            var theme = BeepThemesManager.GetTheme(themeName);
            if (theme != null)
            {
                CurrentTheme = theme;
               // Invalidate();
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
