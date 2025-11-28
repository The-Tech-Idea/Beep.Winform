using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Models;


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
        private Color _bcFillColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.TextBoxBackColor ?? Color.Empty;
        private Color _bcOutlineColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.TextBoxBorderColor ?? Color.Empty;
        private Color _bcPrimaryColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor ?? Color.Empty;
        private bool _bcUseVariantPadding = true;
        private Padding _bcCustomMaterialPadding = Padding.Empty;
        private int _bcElevationLevel = 0;
        private bool _bcUseElevation = true;
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
        private Color _bcErrorColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ErrorColor ?? Color.Empty; // Material Design error color
        #endregion

        #region Material properties


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
        public string LeadingImagePath
        {
            get => _bcLeadingImagePath;
            set { _bcLeadingImagePath = value ?? string.Empty; UpdateMaterialLayout(); Invalidate(); }
        }

        [Browsable(true), Category("Icons")]
        [Description("Image path for the trailing (right) icon - alternative to SVG path.")]
        public string TrailingImagePath
        {
            get => _bcTrailingImagePath;
            set { _bcTrailingImagePath = value ?? string.Empty; UpdateMaterialLayout(); Invalidate(); }
        }

        [Browsable(true), Category("Icons")]
        [Description("Show clear button when field has content.")]
        [DefaultValue(false)]
        public bool ShowClearButton
        {
            get => _bcShowClearButton;
            set { _bcShowClearButton = value; UpdateMaterialLayout(); Invalidate(); }
        }

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
        public int IconSize
        {
            get => _bcIconSize;
            set { _bcIconSize = Math.Max(12, value); UpdateMaterialLayout(); Invalidate(); }
        }

        [Browsable(true), Category("Icons")]
        [Description("Padding between icons and text.")]
        [DefaultValue(8)]
        public int IconPadding
        {
            get => _bcIconPadding;
            set { _bcIconPadding = Math.Max(0, value); UpdateMaterialLayout(); Invalidate(); }
        }

     
        [Browsable(true), Category("Material ProgressBarStyle - Validation")]
        public string ErrorText
        {
            get => _bcErrorText;
            set { _bcErrorText = value ?? string.Empty; _bcHasError = !string.IsNullOrEmpty(value); Invalidate(); }
        }

        [Browsable(true), Category("Material ProgressBarStyle - Validation")]
        public bool HasError { get => _bcHasError; set { _bcHasError = value; Invalidate(); } }

        [Browsable(true), Category("Material ProgressBarStyle - Validation")]
        public Color ErrorColor { get => _bcErrorColor; set { _bcErrorColor = value; Invalidate(); } }

        #endregion

        #region Preset application
    
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

    }
}
