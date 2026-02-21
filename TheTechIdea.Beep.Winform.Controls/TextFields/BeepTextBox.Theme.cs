using System;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// MenuStyle and Style methods for BeepTextBox
    /// </summary>
    public partial class BeepTextBox
    {
        #region "Theme and Style"
        
        public override void ApplyTheme()
        {
            if (_currentTheme == null) return;
            base.ApplyTheme();
            
            if (IsChild && Parent != null)
            {
                ParentBackColor = Parent.BackColor;
                BackColor = ParentBackColor;
            }
            else
            {
                BackColor = _currentTheme.TextBoxBackColor;
            }

            ForeColor = _currentTheme.TextBoxForeColor;
            SelectedBackColor = _currentTheme.TextBoxBackColor;
            SelectedForeColor = _currentTheme.TextBoxForeColor;
            HoverBackColor = _currentTheme.TextBoxHoverBackColor;
            HoverForeColor = _currentTheme.TextBoxHoverForeColor;
            DisabledBackColor = _currentTheme.DisabledBackColor;
            DisabledForeColor = _currentTheme.DisabledForeColor;
            BorderColor = _currentTheme.TextBoxBorderColor;
            _focusBorderColor = _currentTheme.FocusIndicatorColor;
            _placeholderTextColor = _currentTheme.TextBoxPlaceholderColor;
            
            if (UseThemeFont)
            {
                // Use DPI-aware font from BeepFontManager
                // Use TextBoxFont (usually BodyMedium size) instead of LabelSmall which is too small
                var themeFont = BeepFontManager.ToFont(_currentTheme.TextBoxFont);
                
                // Fallback if TextBoxFont is not defined in theme
                if (themeFont == null)
                {
                    themeFont = BeepFontManager.ToFont(_currentTheme.BodyMedium);
                }
                
                // Final fallback
                if (themeFont == null)
                {
                     themeFont = BeepFontManager.ToFont(_currentTheme.LabelMedium);
                }

                if (themeFont != null)
                {
                    // Use Point-based font so TextRenderer renders at the same visual size
                    // as other controls (e.g. BeepButton). Point fonts scale with DPI automatically.
                    // GetFontForPainter produces a Pixel-unit font that renders ~25% smaller
                    // because 8px != 8pt (8pt = ~10.67px at 96 DPI).
                    _textFont = BeepFontManager.GetFont(
                        themeFont.Name,
                        themeFont.SizeInPoints,
                        themeFont.Style);
                }
                
                // Invalidate cached derived fonts
                _characterCountFont?.Dispose();
                _characterCountFont = null;
                _lineNumberFont?.Dispose();
                _lineNumberFont = null;
                
                // Recompute min-height for the new font size
                RecomputeMinHeight();
            }
            
            if (_beepImage != null)
            {
                _beepImage.IsChild = true;
                _beepImage.ImageEmbededin = ImageEmbededin.TextBox;
                _beepImage.ParentBackColor = BackColor; 
                _beepImage.BackColor = BackColor;
                _beepImage.ForeColor = _currentTheme.TextBoxForeColor;
                _beepImage.BorderColor = _currentTheme.BorderColor;
                _beepImage.HoverBackColor = _currentTheme.TextBoxHoverBackColor;
                _beepImage.HoverForeColor = _currentTheme.TextBoxHoverForeColor;
                _beepImage.ShowAllBorders = false;
                _beepImage.IsFrameless = true;
                _beepImage.IsBorderAffectedByTheme = false;
                _beepImage.IsShadowAffectedByTheme = false;
                _beepImage.BorderColor = _currentTheme.TextBoxBorderColor;
                 
                if (ApplyThemeOnImage)
                {
                    _beepImage.ApplyThemeOnImage = ApplyThemeOnImage;
                    _beepImage.ApplyThemeToSvg();
                }
            }
            
            UpdateDrawingRect();
            Invalidate();
        }
        
        #endregion
    }
}

