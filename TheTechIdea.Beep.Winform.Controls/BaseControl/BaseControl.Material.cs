using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Converters;

namespace TheTechIdea.Beep.Winform.Controls.Base
{
    public partial class BaseControl
    {
        #region Icon & Validation Fields
        private string _bcLeadingIconPath = string.Empty;
        private string _bcTrailingIconPath = string.Empty;
        private string _bcLeadingImagePath = string.Empty;
        private string _bcTrailingImagePath = string.Empty;
        private string _iconKey = string.Empty;
        private int _bcIconSize = 20;
        private int _bcIconPadding = 8;
        private bool _bcShowClearButton = false;
        private bool _bcLeadingIconClickable = true;
        private bool _bcTrailingIconClickable = true;

        private string _bcErrorText = string.Empty;
        private bool _bcHasError = false;
        private Color _bcErrorColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ErrorColor ?? Color.Empty;
        #endregion

        #region Icon Properties

        [Browsable(true), Category("Icons")]
        [Description("SVG path for the leading (left) icon.")]
        [TypeConverter(typeof(BeepImagesPathConverter))]
        public string LeadingIconPath
        {
            get => _bcLeadingIconPath;
            set
            {
                _bcLeadingIconPath = value ?? string.Empty;
                UpdatePainterLayout();
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
                Invalidate();
            }
        }

        [Browsable(true), Category("Icons")]
        [Description("Image path for the leading (left) icon - alternative to SVG path.")]
        public string LeadingImagePath
        {
            get => _bcLeadingImagePath;
            set
            {
                _bcLeadingImagePath = value ?? string.Empty;
                Invalidate();
            }
        }

        [Browsable(true), Category("Icons")]
        [Description("Image path for the trailing (right) icon - alternative to SVG path.")]
        public string TrailingImagePath
        {
            get => _bcTrailingImagePath;
            set { _bcTrailingImagePath = value ?? string.Empty; Invalidate(); }
        }

        [Browsable(true), Category("Icons")]
        [Description("Show clear button when field has content.")]
        [DefaultValue(false)]
        public bool ShowClearButton
        {
            get => _bcShowClearButton;
            set { _bcShowClearButton = value; Invalidate(); }
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
            set { _bcIconSize = Math.Max(12, value); Invalidate(); }
        }

        [Browsable(true), Category("Icons")]
        [Description("Padding between icons and text.")]
        [DefaultValue(8)]
        public int IconPadding
        {
            get => _bcIconPadding;
            set { _bcIconPadding = Math.Max(0, value); Invalidate(); }
        }

        [Browsable(true), Category("Icons")]
        [Description("Optional icon key from the shared icon catalog.")]
        [TypeConverter(typeof(IconCatalogKeyConverter))]
        public string IconKey
        {
            get => _iconKey;
            set
            {
                var normalized = value ?? string.Empty;
                if (string.Equals(_iconKey, normalized, StringComparison.Ordinal))
                    return;
                _iconKey = normalized;
                Invalidate();
            }
        }

        #endregion

        #region Validation Properties

        [Browsable(true), Category("Validation")]
        public string ErrorText
        {
            get => _bcErrorText;
            set { _bcErrorText = value ?? string.Empty; _bcHasError = !string.IsNullOrEmpty(value); Invalidate(); }
        }

        [Browsable(true), Category("Validation")]
        public bool HasError { get => _bcHasError; set { _bcHasError = value; Invalidate(); } }

        [Browsable(true), Category("Validation")]
        public Color ErrorColor { get => _bcErrorColor; set { _bcErrorColor = value; Invalidate(); } }

        #endregion

        #region Content Rect Helpers

        public Rectangle GetAdjustedContentRect()
        {
            EnsurePainter();
            _painter?.UpdateLayout(this);
            return _painter?.ContentRect ?? Rectangle.Empty;
        }

        public Rectangle GetContentRect()
        {
            EnsurePainter();
            _painter?.UpdateLayout(this);
            return _painter?.ContentRect ?? Rectangle.Empty;
        }

        #endregion

        #region Partial Hook

        partial void DrawCustomBorder_Ext(Graphics g)
        {
        }

        #endregion

        #region Icon Click Events

        public event EventHandler LeadingIconClicked;
        public event EventHandler TrailingIconClicked;
        protected virtual void OnLeadingIconClick() => LeadingIconClicked?.Invoke(this, EventArgs.Empty);
        protected virtual void OnTrailingIconClick() => TrailingIconClicked?.Invoke(this, EventArgs.Empty);

        internal void TriggerLeadingIconClick() => OnLeadingIconClick();
        internal void TriggerTrailingIconClick() => OnTrailingIconClick();

        #endregion
    }
}
