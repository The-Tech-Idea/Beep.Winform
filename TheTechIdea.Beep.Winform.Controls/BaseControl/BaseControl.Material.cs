using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Base
{
    // Material rendering extension for BaseControl (partial)
    public partial class BaseControl
    {
        private void ClearPainterHitAreas()
        {
            try
            {
                var names = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "MaterialLeadingIcon","MaterialTrailingIcon",
                    "ClassicLeadingIcon","ClassicTrailingIcon",
                    "Card_Main",
                    "NeoLeadingIcon","NeoTrailingIcon",
                    "ReadingCardLeadingIcon","ReadingCardTrailingIcon",
                    "ReadingCard_Settings","ReadingCard_Main",
                    "Button_Main",
                    "ShortcutCard_Main"
                };
                var list = _hitTest?.HitList;
                if (list != null && list.Count > 0)
                {
                    list.RemoveAll(h => names.Contains(h.Name));
                }
            }
            catch { }
        }
        
        #region Material fields
        private MaterialTextFieldVariant _bcMaterialVariant = MaterialTextFieldVariant.Outlined;
        private int _bcMaterialRadius = 8;
        private bool _bcShowFill = false;
        private Color _bcFillColor = Color.FromArgb(245, 245, 245);
        private Color _bcOutlineColor = Color.FromArgb(140, 140, 140);
        private Color _bcPrimaryColor = Color.FromArgb(25, 118, 210);
        private bool _bcUseVariantPadding = true;
        private Padding _bcCustomMaterialPadding = Padding.Empty;

        private string _bcLeadingIconPath = string.Empty;
        private string _bcTrailingIconPath = string.Empty;
        private string _bcLeadingImagePath = string.Empty;
        private string _bcTrailingImagePath = string.Empty;
        private int _bcIconSize = 20;
        private int _bcIconPadding = 8;
        private bool _bcShowClearButton = false;
        private bool _bcLeadingIconClickable = true;
        private bool _bcTrailingIconClickable = true;

        private string _bcErrorText = string.Empty;
        private bool _bcHasError = false;
        private Color _bcErrorColor = Color.FromArgb(176, 0, 32); // Material Design error color
        private MaterialTextFieldStylePreset _stylePreset = MaterialTextFieldStylePreset.Default;

        // Material Design 3.0 elevation properties
        private int _bcElevationLevel = 0;
        private bool _bcUseElevation = true;
        #endregion

        #region Material properties

        [Browsable(true), Category("Material ProgressBarStyle"), DefaultValue(true)]
        public bool MaterialUseVariantPadding
        {
            get => _bcUseVariantPadding;
            set
            {
                if (_bcUseVariantPadding != value)
                {
                    _bcUseVariantPadding = value;
                    IsCustomeBorder = value; // keep ControlPaintHelper drawing borders
                    OnMaterialPropertyChanged();
                    Invalidate();
                }
            }
        }

        [Browsable(true), Category("Material ProgressBarStyle"), DefaultValue(MaterialTextFieldVariant.Outlined)]
        public MaterialTextFieldVariant MaterialVariant
        {
            get => _bcMaterialVariant;
            set
            {
                if (_bcMaterialVariant != value)
                {
                    _bcMaterialVariant = value;
                    MaterialBorderVariant = value; // ControlPaintHelper paints according to enum
                    OnMaterialPropertyChanged();
                    Invalidate();
                }
            }
        }

        [Browsable(true), Category("Material ProgressBarStyle"), DefaultValue(8)]
        public int MaterialBorderRadius
        {
            get => _bcMaterialRadius;
            set
            {
                if (_bcMaterialRadius != Math.Max(0, value))
                {
                    _bcMaterialRadius = Math.Max(0, value);
                    BorderRadius = _bcMaterialRadius; // keep background rounding consistent
                    OnMaterialPropertyChanged();
                    Invalidate();
                }
            }
        }

        [Browsable(true), Category("Material ProgressBarStyle"), DefaultValue(false)]
        public bool MaterialShowFill { get => _bcShowFill; set { _bcShowFill = value; Invalidate(); } }

        [Browsable(true), Category("Material ProgressBarStyle")]
        public Color MaterialFillColor
        {
            get => _bcFillColor;
            set
            {
                _bcFillColor = value;
                FilledBackgroundColor = value; // mirror to ControlPaintHelper background
                Invalidate();
            }
        }

        [Browsable(true), Category("Material ProgressBarStyle")] public Color MaterialOutlineColor { get => _bcOutlineColor; set { _bcOutlineColor = value; Invalidate(); } }
        [Browsable(true), Category("Material ProgressBarStyle")] public Color MaterialPrimaryColor { get => _bcPrimaryColor; set { _bcPrimaryColor = value; Invalidate(); } }

        [Browsable(true), Category("Icons")]
        [Description("SVG path for the leading (left) icon.")]
        [TypeConverter(typeof(BeepImagesPathConverter))]
        public string LeadingIconPath
        {
            get => _bcLeadingIconPath;
            set
            {
                _bcLeadingIconPath = value ?? string.Empty;
                UpdateMaterialLayout();
                Invalidate();
            }
        }

        [Browsable(true), Category("Icons")]
        [Description("SVG path for the trailing (right) icon.")]
        [TypeConverter(typeof(BeepImagesPathConverter))]
        public string TrailingIconPath
        {
            get => _bcTrailingIconPath;
            set
            {
                _bcTrailingIconPath = value ?? string.Empty;
                UpdateMaterialLayout();
                Invalidate();
            }
        }
        [Browsable(true), Category("Icons")]
        [Description("Image path for the leading (left) icon - alternative to SVG path.")]
        public string LeadingImagePath { get => _bcLeadingImagePath; set { _bcLeadingImagePath = value ?? string.Empty; UpdateMaterialLayout(); Invalidate(); } }

        [Browsable(true), Category("Icons")]
        [Description("Image path for the trailing (right) icon - alternative to SVG path.")]
        public string TrailingImagePath { get => _bcTrailingImagePath; set { _bcTrailingImagePath = value ?? string.Empty; UpdateMaterialLayout(); Invalidate(); } }

        [Browsable(true), Category("Icons")]
        [Description("Show clear button when field has content.")]
        [DefaultValue(false)]
        public bool ShowClearButton { get => _bcShowClearButton; set { _bcShowClearButton = value; UpdateMaterialLayout(); Invalidate(); } }

        [Browsable(true), Category("Icons")]
        [Description("Enable click events for the leading icon.")]
        [DefaultValue(true)]
        public bool LeadingIconClickable { get => _bcLeadingIconClickable; set { _bcLeadingIconClickable = value; } }

        [Browsable(true), Category("Icons")]
        [Description("Enable click events for the trailing icon.")]
        [DefaultValue(true)]
        public bool TrailingIconClickable { get => _bcTrailingIconClickable; set { _bcTrailingIconClickable = value; } }

        [Browsable(true), Category("Icons")]
        [Description("Size of the icons in pixels.")]
        [DefaultValue(24)]
        public int IconSize { get => _bcIconSize; set { _bcIconSize = Math.Max(12, value); UpdateMaterialLayout(); Invalidate(); } }

        [Browsable(true), Category("Icons")]
        [Description("Padding between icons and text.")]
        [DefaultValue(8)]
        public int IconPadding { get => _bcIconPadding; set { _bcIconPadding = Math.Max(0, value); UpdateMaterialLayout(); Invalidate(); } }

        // Material Design specific icon properties (aliases for compatibility)
        [Browsable(false)]
        public int MaterialIconSize { get => IconSize; set => IconSize = value; }

        [Browsable(false)]
        public int MaterialIconPadding { get => IconPadding; set => IconPadding = value; }

        #endregion

        #region Material properties

        [Browsable(true), Category("Material ProgressBarStyle - Validation")]
        public string ErrorText { get => _bcErrorText; set { _bcErrorText = value ?? string.Empty; _bcHasError = !string.IsNullOrEmpty(value); Invalidate(); } }

        [Browsable(true), Category("Material ProgressBarStyle - Validation")]
        public bool HasError { get => _bcHasError; set { _bcHasError = value; Invalidate(); } }

        [Browsable(true), Category("Material ProgressBarStyle - Validation")]
        public Color ErrorColor { get => _bcErrorColor; set { _bcErrorColor = value; Invalidate(); } }

        // Important: expose the preset exactly like BeepMaterialTextField
        [Browsable(true)]
        [Category("Material Design")]
        [Description("Applies a predefined style preset that configures variant, density, radius, fill, and helper/label behavior.")]
        [DefaultValue(MaterialTextFieldStylePreset.Default)]
        public MaterialTextFieldStylePreset StylePreset
        {
            get => _stylePreset;
            set
            {
                if (_stylePreset == value) return;
                _stylePreset = value;
                ApplyStylePreset(_stylePreset);
            }
        }

        [Browsable(true), Category("Material Design 3.0")]
        [Description("Elevation level for shadow effects (0-5). Higher values create more pronounced shadows.")]
        [DefaultValue(0)]
        public int MaterialElevationLevel
        {
            get => _bcElevationLevel;
            set
            {
                _bcElevationLevel = Math.Max(0, Math.Min(value, 5));
                Invalidate();
            }
        }

        [Browsable(true), Category("Material Design 3.0")]
         [Description("Enable or disable elevation shadow effects.")]
         [DefaultValue(true)]
         public bool MaterialUseElevation
         {
             get => _bcUseElevation;
             set
             {
                 _bcUseElevation = value;
                 Invalidate();
             }
         }
        #endregion

        #region Preset application
        public void ApplyStylePreset(MaterialTextFieldStylePreset preset)
        {
            switch (preset)
            {
                case MaterialTextFieldStylePreset.MaterialOutlined:
                    MaterialVariant = MaterialTextFieldVariant.Outlined;
                    MaterialBorderRadius = 8;
                    MaterialShowFill = false;
                    break;
                case MaterialTextFieldStylePreset.MaterialFilled:
                    MaterialVariant = MaterialTextFieldVariant.Filled;
                    MaterialBorderRadius = 8;
                    MaterialShowFill = true;
                    MaterialFillColor = Color.FromArgb(0xEE, 0xEA, 0xF0);
                    break;
                case MaterialTextFieldStylePreset.MaterialStandard:
                    MaterialVariant = MaterialTextFieldVariant.Standard;
                    MaterialBorderRadius = 4;
                    MaterialShowFill = false;
                    break;
                case MaterialTextFieldStylePreset.PillOutlined:
                    MaterialVariant = MaterialTextFieldVariant.Outlined;
                    MaterialBorderRadius = Math.Max(Height / 2, 20);
                    MaterialShowFill = false;
                    break;
                case MaterialTextFieldStylePreset.PillFilled:
                    MaterialVariant = MaterialTextFieldVariant.Filled;
                    MaterialBorderRadius = Math.Max(Height / 2, 20);
                    MaterialShowFill = true;
                    MaterialFillColor = Color.FromArgb(245, 245, 245);
                    break;
                case MaterialTextFieldStylePreset.DenseOutlined:
                    MaterialVariant = MaterialTextFieldVariant.Outlined;
                    MaterialBorderRadius = 6;
                    MaterialShowFill = false;
                    break;
                case MaterialTextFieldStylePreset.DenseFilled:
                    MaterialVariant = MaterialTextFieldVariant.Filled;
                    MaterialBorderRadius = 6;
                    MaterialShowFill = true;
                    MaterialFillColor = Color.FromArgb(245, 245, 245);
                    break;
                case MaterialTextFieldStylePreset.ComfortableOutlined:
                    MaterialVariant = MaterialTextFieldVariant.Outlined;
                    MaterialBorderRadius = 10;
                    MaterialShowFill = false;
                    break;
                case MaterialTextFieldStylePreset.ComfortableFilled:
                    MaterialVariant = MaterialTextFieldVariant.Filled;
                    MaterialBorderRadius = 10;
                    MaterialShowFill = true;
                    MaterialFillColor = Color.FromArgb(245, 245, 245);
                    break;
                case MaterialTextFieldStylePreset.Default:
                default:
                    // Apply a sane baseline instead of doing nothing
                    MaterialVariant = MaterialTextFieldVariant.Outlined;
                    MaterialBorderRadius = 8;
                    MaterialShowFill = false;
                    break;
            }

            // Sync helper painter
            MaterialBorderVariant = MaterialVariant;
            FilledBackgroundColor = MaterialFillColor;
            BorderRadius = MaterialBorderRadius;
            UpdateMaterialLayout();
             Invalidate();
        }

        /// <summary>
        /// Gets the adjusted content rectangle that excludes icon areas for proper content drawing
        /// </summary>
        public Rectangle GetAdjustedContentRect()
        {
            // Use painter-provided content rectangle
            EnsurePainter();
            _painter?.UpdateLayout(this);
            return _painter?.ContentRect ?? Rectangle.Empty;
        }

        /// <summary>
        /// Gets the main content rectangle for text and child controls
        /// </summary>
        public Rectangle GetContentRect()
        {
            // Use painter-provided content rectangle
            EnsurePainter();
            _painter?.UpdateLayout(this);
            return _painter?.ContentRect ?? Rectangle.Empty;
        }
        #endregion

        #region Partial hook implementation
        partial void DrawCustomBorder_Ext(Graphics g)
        {
            // No-op: painters now own all drawing and hit area registration
            // This partial method is kept for binary compatibility but does nothing
        }
        #endregion

        #region Icon click hooks + events
        public event EventHandler LeadingIconClicked;
        public event EventHandler TrailingIconClicked;
        protected virtual void OnLeadingIconClick() => LeadingIconClicked?.Invoke(this, EventArgs.Empty);
        protected virtual void OnTrailingIconClick() => TrailingIconClicked?.Invoke(this, EventArgs.Empty);
        
        // Internal wrappers for painters to trigger clicks safely
        internal void TriggerLeadingIconClick() => OnLeadingIconClick();
        internal void TriggerTrailingIconClick() => OnTrailingIconClick();
        #endregion

        #region Size change handling
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            // Update painter layout when control size changes
            UpdateMaterialLayout();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            // Update painter layout when font changes
            UpdateMaterialLayout();
        }
        #endregion

        [Browsable(false), Category("Material ProgressBarStyle")]
        [Description("Override Material internal content padding. When not empty, this padding is used instead of variant defaults.")]
        public Padding MaterialCustomPadding
        {
            get => _bcCustomMaterialPadding;
            set
            {
                _bcCustomMaterialPadding = value;
                UpdateMaterialLayout();
                Invalidate();
            }
        }
    }
}
