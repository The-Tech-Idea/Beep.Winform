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
        private BaseControlMaterialHelper _materialHelper;

        #region Material fields
        private bool _bcEnableMaterialStyle = false;
        private MaterialTextFieldVariant _bcMaterialVariant = MaterialTextFieldVariant.Outlined;
        private int _bcMaterialRadius = 8;
        private bool _bcShowFill = false;
        private Color _bcFillColor = Color.FromArgb(245, 245, 245);
        private Color _bcOutlineColor = Color.FromArgb(140, 140, 140);
        private Color _bcPrimaryColor = Color.FromArgb(25, 118, 210);

        private string _bcLeadingIconPath = string.Empty;
        private string _bcTrailingIconPath = string.Empty;
        private string _bcLeadingImagePath = string.Empty;
        private string _bcTrailingImagePath = string.Empty;
        private int _bcIconSize = 20;
        private int _bcIconPadding = 8;

        private MaterialTextFieldStylePreset _stylePreset = MaterialTextFieldStylePreset.Default;
        #endregion

        #region Material properties
        [Browsable(true), Category("Material Style"), DefaultValue(false)]
        public bool EnableMaterialStyle
        {
            get => _bcEnableMaterialStyle;
            set
            {
                _bcEnableMaterialStyle = value;
                IsCustomeBorder = false; // keep ControlPaintHelper drawing borders
                Invalidate();
            }
        }

        [Browsable(true), Category("Material Style"), DefaultValue(MaterialTextFieldVariant.Outlined)]
        public MaterialTextFieldVariant MaterialVariant
        {
            get => _bcMaterialVariant;
            set
            {
                _bcMaterialVariant = value;
                MaterialBorderVariant = value; // ControlPaintHelper paints according to enum
                Invalidate();
            }
        }

        [Browsable(true), Category("Material Style"), DefaultValue(8)]
        public int MaterialBorderRadius
        {
            get => _bcMaterialRadius;
            set
            {
                _bcMaterialRadius = Math.Max(0, value);
                BorderRadius = _bcMaterialRadius; // keep background rounding consistent
                Invalidate();
            }
        }

        [Browsable(true), Category("Material Style"), DefaultValue(false)]
        public bool MaterialShowFill { get => _bcShowFill; set { _bcShowFill = value; Invalidate(); } }

        [Browsable(true), Category("Material Style")] 
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

        [Browsable(true), Category("Material Style")] public Color MaterialOutlineColor { get => _bcOutlineColor; set { _bcOutlineColor = value; Invalidate(); } }
        [Browsable(true), Category("Material Style")] public Color MaterialPrimaryColor { get => _bcPrimaryColor; set { _bcPrimaryColor = value; Invalidate(); } }

        [Browsable(true), Category("Material Style - Icons")]
        public string LeadingIconPath { get => _bcLeadingIconPath; set { _bcLeadingIconPath = value ?? string.Empty; Invalidate(); } }

        [Browsable(true), Category("Material Style - Icons")] 
        public string TrailingIconPath { get => _bcTrailingIconPath; set { _bcTrailingIconPath = value ?? string.Empty; Invalidate(); } }
        [Browsable(true), Category("Material Style - Icons")]
        [TypeConverter(typeof(BeepImagesPathConverter))]
        public string LeadingImagePath 
        { get => _bcLeadingImagePath; set { _bcLeadingImagePath = value ?? string.Empty; Invalidate(); } }
        [Browsable(true), Category("Material Style - Icons")]
        [TypeConverter(typeof(BeepImagesPathConverter))]
        public string TrailingImagePath 
        { get => _bcTrailingImagePath; set { _bcTrailingImagePath = value ?? string.Empty; Invalidate(); } }
        [Browsable(true), Category("Material Style - Icons"), DefaultValue(20)] public int MaterialIconSize { get => _bcIconSize; set { _bcIconSize = Math.Max(12, value); Invalidate(); } }
        [Browsable(true), Category("Material Style - Icons"), DefaultValue(8)] public int MaterialIconPadding { get => _bcIconPadding; set { _bcIconPadding = Math.Max(0, value); Invalidate(); } }

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
                    break;
            }

            // Sync helper painter
            MaterialBorderVariant = MaterialVariant;
            FilledBackgroundColor = MaterialFillColor;
            BorderRadius = MaterialBorderRadius;
            _materialHelper?.UpdateLayout();
            Invalidate();
        }
        #endregion

        #region Partial hook implementation
        partial void DrawCustomBorder_Ext(Graphics g)
        {
            if (!_bcEnableMaterialStyle) return;
            _materialHelper ??= new BaseControlMaterialHelper(this);
            _materialHelper.UpdateLayout();

            var leadRect = _materialHelper.GetLeadingIconRect();
            var trailRect = _materialHelper.GetTrailingIconRect();
            UpdateMaterialIconHitAreas(leadRect, trailRect);

            _materialHelper.DrawIconsOnly(g);
        }

        private void UpdateMaterialIconHitAreas(Rectangle leadRect, Rectangle trailRect)
        {
            if (HitList != null)
            {
                var stale = HitList.Where(h => h.Name == "MaterialLeadingIcon" || h.Name == "MaterialTrailingIcon").ToList();
                foreach (var s in stale)
                {
                    if ((s.Name == "MaterialLeadingIcon" && leadRect.IsEmpty) || (s.Name == "MaterialTrailingIcon" && trailRect.IsEmpty))
                    {
                        HitList.Remove(s);
                    }
                }
            }

            if (!leadRect.IsEmpty) _hitTest?.AddHitArea("MaterialLeadingIcon", leadRect, null, () => OnLeadingIconClick());
            if (!trailRect.IsEmpty) _hitTest?.AddHitArea("MaterialTrailingIcon", trailRect, null, () => OnTrailingIconClick());
        }
        #endregion

        #region Icon click hooks + events
        public event EventHandler LeadingIconClicked;
        public event EventHandler TrailingIconClicked;
        protected virtual void OnLeadingIconClick() => LeadingIconClicked?.Invoke(this, EventArgs.Empty);
        protected virtual void OnTrailingIconClick() => TrailingIconClicked?.Invoke(this, EventArgs.Empty);
        #endregion
    }
}
