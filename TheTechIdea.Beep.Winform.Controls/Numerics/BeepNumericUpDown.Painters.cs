using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Numerics.Painters;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Numerics
{
    /// <summary>
    /// Partial class for BeepNumericUpDown painter integration
    /// </summary>
    public partial class BeepNumericUpDown
    {
        #region Painter Fields
        private BeepControlStyle _style = BeepControlStyle.Material3;
        private INumericUpDownPainter _currentPainter;
        private Dictionary<string, (Rectangle rect, Action action)> _hitAreas = 
            new Dictionary<string, (Rectangle, Action)>();
        #endregion

        #region Painter Properties
        [Category("Appearance")]
        [Description("The visual Style painter for the numeric up-down control")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle Style
        {
            get => _style;
            set
            {
                if (_style != value)
                {
                    _style = value;
                    InitializePainter();
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public INumericUpDownPainter CurrentPainter => _currentPainter;
        #endregion

        #region Painter Initialization
        private void InitializePainter()
        {
            _currentPainter = _style switch
            {
                BeepControlStyle.Material3 => new Material3NumericUpDownPainter(),
                BeepControlStyle.iOS15 => new iOS15NumericUpDownPainter(),
                BeepControlStyle.AntDesign => new AntDesignNumericUpDownPainter(),
                BeepControlStyle.Fluent2 => new Fluent2NumericUpDownPainter(),
                BeepControlStyle.MaterialYou => new MaterialYouNumericUpDownPainter(),
                BeepControlStyle.Windows11Mica => new Windows11MicaNumericUpDownPainter(),
                BeepControlStyle.MacOSBigSur => new MacOSBigSurNumericUpDownPainter(),
                BeepControlStyle.ChakraUI => new ChakraUINumericUpDownPainter(),
                BeepControlStyle.TailwindCard => new TailwindCardNumericUpDownPainter(),
                BeepControlStyle.NotionMinimal => new NotionMinimalNumericUpDownPainter(),
                BeepControlStyle.Minimal => new MinimalNumericUpDownPainter(),
                BeepControlStyle.VercelClean => new VercelCleanNumericUpDownPainter(),
                BeepControlStyle.StripeDashboard => new StripeDashboardNumericUpDownPainter(),
                BeepControlStyle.DarkGlow => new DarkGlowNumericUpDownPainter(),
                BeepControlStyle.DiscordStyle => new DiscordStyleNumericUpDownPainter(),
                BeepControlStyle.GradientModern => new GradientModernNumericUpDownPainter(),
                _ => new Material3NumericUpDownPainter()
            };

            RefreshHitAreas();
        }
        #endregion

        #region Hit Area Management
        private void RefreshHitAreas()
        {
            _hitAreas.Clear();

            if (_currentPainter != null && ClientRectangle.Width > 0 && ClientRectangle.Height > 0)
            {
                var context = new NumericUpDownPainterContext(this);
                _currentPainter.UpdateHitAreas(context, ClientRectangle, (name, rect, action) =>
                {
                    _hitAreas[name] = (rect, action);
                });
            }
        }

        private void UpdateButtonHoverState(Point mouseLocation)
        {
            bool prevUpHover = _upButtonHovered;
            bool prevDownHover = _downButtonHovered;

            _upButtonHovered = false;
            _downButtonHovered = false;

            foreach (var kvp in _hitAreas)
            {
                if (kvp.Value.rect.Contains(mouseLocation))
                {
                    if (kvp.Key == "UpButton")
                        _upButtonHovered = true;
                    else if (kvp.Key == "DownButton")
                        _downButtonHovered = true;
                }
            }

            if (prevUpHover != _upButtonHovered || prevDownHover != _downButtonHovered)
            {
                Invalidate();
            }
        }

        private bool HandleHitAreaClick(Point mouseLocation)
        {
            foreach (var kvp in _hitAreas)
            {
                if (kvp.Value.rect.Contains(mouseLocation))
                {
                    kvp.Value.action?.Invoke();
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Context Adapter Class
        /// <summary>
        /// Adapter class to expose BeepNumericUpDown properties to painters
        /// </summary>
        private class NumericUpDownPainterContext : INumericUpDownPainterContext
        {
            private readonly BeepNumericUpDown _control;

            public NumericUpDownPainterContext(BeepNumericUpDown control)
            {
                _control = control;
            }

            // Value properties
            public decimal Value => _control._value;
            public decimal MinimumValue => _control._minimumValue;
            public decimal MaximumValue => _control._maximumValue;
            public decimal IncrementValue => _control._incrementValue;
            public int DecimalPlaces => _control._decimalPlaces;

            // State properties
            public bool IsEditing => _control._isEditing;
            public bool IsHovered => _control.IsHovered;
            public bool IsFocused => _control.IsFocused;
            public bool IsEnabled => _control.Enabled;
            public bool IsReadOnly => _control._readOnly;

            // Button states
            public bool UpButtonPressed => _control._upButtonPressed;
            public bool DownButtonPressed => _control._downButtonPressed;
            public bool UpButtonHovered => _control._upButtonHovered;
            public bool DownButtonHovered => _control._downButtonHovered;
            public bool ShowSpinButtons => _control._showSpinButtons;

            // Visual properties
            public bool IsRounded => _control.IsRounded;
            public int BorderRadius => _control.BorderRadius;
            public Color AccentColor => _control.AccentColor;
            public bool UseThemeColors => _control.UseThemeColors;
            public IBeepTheme Theme => _control._currentTheme;

            // Display properties
            public NumericUpDownDisplayMode DisplayMode => _control._displayMode;
            public NumericSpinButtonSize ButtonSize => _control._buttonSize;
            public string Prefix => _control._prefix;
            public string Suffix => _control._suffix;
            public string Unit => _control._unit;
            public bool ThousandsSeparator => _control._thousandsSeparator;
            public bool AllowNegative => _control._allowNegative;
            public bool EnableShadow
            {
                get => _control.EnableShadow;
                set => _control.EnableShadow = value;
            }

            // Actions
            public void IncreaseValue() => _control.IncrementValueInternal();
            public void DecrementValue() => _control.DecrementValueInternal();
        }
        #endregion
    }
}
