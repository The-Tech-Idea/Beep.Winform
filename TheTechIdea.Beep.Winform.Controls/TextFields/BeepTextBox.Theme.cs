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
            BorderColor = _currentTheme.BorderColor;
            _focusBorderColor = _currentTheme.FocusIndicatorColor;
            _placeholderTextColor = _currentTheme.TextBoxPlaceholderColor;
            
            if (UseThemeFont)
            {
                _textFont = BeepFontManager.ToFont(_currentTheme.LabelSmall);
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
