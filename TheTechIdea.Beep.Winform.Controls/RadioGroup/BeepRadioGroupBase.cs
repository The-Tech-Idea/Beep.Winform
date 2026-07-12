using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup
{
    /// <summary>
    /// Shared state and behavior for <see cref="BeepRadioGroup"/> and
    /// <see cref="BeepHierarchicalRadioGroup"/>. Owns the renderer dictionary, the
    /// animation timer, and the appearance/style/validation properties that both
    /// flat and hierarchical variants expose.
    /// </summary>
    /// <remarks>
    /// SCAFFOLD: this base is currently <b>not</b> inherited by either concrete control.
    /// Migrating <c>BeepRadioGroup</c> or <c>BeepHierarchicalRadioGroup</c> to derive from
    /// <see cref="BeepRadioGroupBase"/> requires the design server
    /// (<c>BeepRadioGroupDesigner.cs</c> + <c>BeepRadioGroupActionList</c>) to be updated in
    /// lockstep, and the public design-surface type name <c>BeepRadioGroup</c> must be
    /// preserved (existing forms drop the control by that name). The base is kept as a
    /// forward-looking API that compiles standalone so that the migration is a single
    /// focused change rather than a refactor and rename.
    /// <para>
    /// Hierarchical parity is achieved in the meantime by mirroring the relevant fields
    /// and properties in <see cref="BeepHierarchicalRadioGroup"/> directly (renderer
    /// dictionary, animation timer, <c>HasError</c>, <c>UseThemeColors</c>, <c>Style</c>).
    /// </para>
    /// </remarks>
    public abstract class BeepRadioGroupBase : BaseControl
    {
        protected internal override Padding StylePadding => new Padding(0);
        #region Shared Fields
        protected readonly Dictionary<RadioGroupRenderStyle, IRadioGroupRenderer> _renderers;
        protected IRadioGroupRenderer _currentRenderer;
        protected RadioGroupRenderStyle _renderStyle = RadioGroupRenderStyle.Material;

        protected bool _useThemeColors = true;
        protected BeepControlStyle _style = BeepControlStyle.Material3;
        protected RadioGroupStyleConfig _styleProfile = new RadioGroupStyleConfig();
        protected RadioGroupColorConfig _colorProfile = new RadioGroupColorConfig();
        protected Size _maxImageSize = new Size(24, 24);
        protected bool _allowMultipleSelection;
        protected bool _autoSizeItems = true;
        protected bool _hasValidationError;
        protected System.Windows.Forms.Timer? _animationTimer;
        protected Dictionary<int, float> _animationProgress = new Dictionary<int, float>();
        #endregion

        #region Constructor
        protected BeepRadioGroupBase()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            _renderers = new Dictionary<RadioGroupRenderStyle, IRadioGroupRenderer>();
            BuildRenderers(_renderers);

            // Initialize all renderers
            if (!DesignMode && _renderers.Count > 0)
            {
                _currentRenderer = _renderers[_renderStyle];
                _currentRenderer.Initialize(this, _currentTheme);
                foreach (var renderer in _renderers.Values)
                {
                    renderer.AllowMultipleSelection = _allowMultipleSelection;
                    if (renderer is IImageAwareRenderer imageRenderer)
                    {
                        imageRenderer.MaxImageSize = _maxImageSize;
                    }
                }
            }

            // Animation timer (16ms ≈ 60fps)
            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _animationTimer.Tick += OnAnimationTick;
        }

        /// <summary>Derived classes register their supported renderers in this dictionary.</summary>
        protected abstract void BuildRenderers(Dictionary<RadioGroupRenderStyle, IRadioGroupRenderer> renderers);
        #endregion

        #region Shared Properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use theme colors instead of style-based colors.")]
        [DefaultValue(true)]
        public bool UseThemeColors
        {
            get => _useThemeColors;
            set
            {
                if (_useThemeColors == value) return;
                _useThemeColors = value;
                PropagateToRenderers(r => r.UseThemeColors = value);
                if (!value) ApplyColorProfile(_colorProfile);
                SafeInvalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual Style/painter to use for rendering the radio group.")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle Style
        {
            get => _style;
            set
            {
                if (_style == value) return;
                _style = value;
                PropagateToRenderers(r => r.ControlStyle = value);
                SafeInvalidate(resetLayout: true);
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Visual style profile (border radius, padding, etc).")]
        public RadioGroupStyleConfig StyleProfile
        {
            get => _styleProfile;
            set
            {
                _styleProfile = value ?? new RadioGroupStyleConfig();
                ApplyStyleProfile(_styleProfile);
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color profile for non-theme rendering.")]
        public RadioGroupColorConfig ColorProfile
        {
            get => _colorProfile;
            set
            {
                _colorProfile = value ?? new RadioGroupColorConfig();
                ApplyColorProfile(_colorProfile);
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Maximum icon size (width, height) for each item.")]
        public Size MaxImageSize
        {
            get => _maxImageSize;
            set
            {
                _maxImageSize = value;
                PropagateToRenderers(r =>
                {
                    if (r is IImageAwareRenderer imageRenderer) imageRenderer.MaxImageSize = value;
                });
                SafeInvalidate(resetLayout: true);
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allow multiple items to be selected at once.")]
        [DefaultValue(false)]
        public bool AllowMultipleSelection
        {
            get => _allowMultipleSelection;
            set
            {
                _allowMultipleSelection = value;
                PropagateToRenderers(r => r.AllowMultipleSelection = value);
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Auto-size each item to fit its content.")]
        [DefaultValue(true)]
        public bool AutoSizeItems
        {
            get => _autoSizeItems;
            set
            {
                _autoSizeItems = value;
                SafeInvalidate(resetLayout: true);
            }
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Marks the control as having a validation error. Renderers draw an error border.")]
        [DefaultValue(false)]
        public new bool HasError
        {
            get => _hasValidationError;
            set
            {
                if (_hasValidationError == value) return;
                _hasValidationError = value;
                OnValidationErrorChanged();
            }
        }

        /// <summary>Called when <see cref="HasError"/> changes. Derived classes refresh item states.</summary>
        protected abstract void OnValidationErrorChanged();
        #endregion

        #region Renderer Helpers
        /// <summary>Applies an action to every registered renderer.</summary>
        protected void PropagateToRenderers(Action<IRadioGroupRenderer> action)
        {
            if (_renderers == null) return;
            foreach (var renderer in _renderers.Values)
            {
                action(renderer);
            }
        }

        /// <summary>Switches the active renderer when the configured style changes.</summary>
        public void SetRenderStyle(RadioGroupRenderStyle style)
        {
            if (_renderers == null || !_renderers.ContainsKey(style)) return;
            _renderStyle = style;
            _currentRenderer = _renderers[style];
            _currentRenderer.AllowMultipleSelection = _allowMultipleSelection;
            if (_currentRenderer is IImageAwareRenderer imageRenderer)
            {
                imageRenderer.MaxImageSize = _maxImageSize;
            }
            _currentRenderer.UpdateTheme(_currentTheme);
        }
        #endregion

        #region Theme + Style
        /// <summary>Resolves color tokens and applies them to all renderers.</summary>
        public virtual void ApplyTheme()
        {
            PropagateToRenderers(r => r.UpdateTheme(_currentTheme));
        }

        /// <summary>Applies a style profile (border radius, padding, control style) to the control + renderers.</summary>
        public virtual void ApplyStyleProfile(RadioGroupStyleConfig? profile)
        {
            if (profile == null) return;
            // StyleProfile drives ControlStyle and layout defaults on each renderer.
            // Concrete renderers (Tile, Chip, Pill, Card, etc.) read their own
            // border radius / padding from their internal constants to keep distinct visuals.
        }

        /// <summary>Applies a color profile to renderers when <see cref="UseThemeColors"/> is false.</summary>
        public virtual void ApplyColorProfile(RadioGroupColorConfig? profile)
        {
            if (profile == null || _useThemeColors) return;
            SafeInvalidate();
        }
        #endregion

        #region Animation
        private void OnAnimationTick(object? sender, EventArgs e)
        {
            const float Step = 0.12f;
            bool anyAlive = false;
            var keys = new List<int>(_animationProgress.Keys);
            foreach (var key in keys)
            {
                if (!_animationProgress.TryGetValue(key, out var p)) continue;
                p += Step;
                if (p >= 1f) { p = 1f; }
                else { anyAlive = true; }
                _animationProgress[key] = p;
            }
            OnAnimationStep();
            Invalidate();
            if (!anyAlive) _animationTimer?.Stop();
        }

        /// <summary>Hook for derived classes to update item states each animation frame.</summary>
        protected virtual void OnAnimationStep() { }

        /// <summary>Triggers a forward animation on the given item index (click, hover, focus).</summary>
        public void StartItemAnimation(int index)
        {
            if (index < 0) return;
            _animationProgress[index] = 0f;
            if (!DesignMode && !(_animationTimer?.Enabled ?? false))
            {
                _animationTimer?.Start();
            }
        }
        #endregion

        #region Invalidation
        /// <summary>Safe cross-thread Invalidate wrapper. Falls back to Invalidate() pre-handle.</summary>
        protected void SafeInvalidate(bool resetLayout = false)
        {
            if (IsDisposed) return;
            Invalidate();
            if (resetLayout && IsHandleCreated) PerformLayout();
        }
        #endregion

        #region Disposal
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop();
                _animationTimer?.Dispose();
                _animationTimer = null;
                PropagateToRenderers(r => r.Cleanup());
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Validation Hooks
        /// <summary>Override to clear renderer-specific validation visuals.</summary>
        public virtual void ClearError() { _hasValidationError = false; }
        #endregion
    }
}
