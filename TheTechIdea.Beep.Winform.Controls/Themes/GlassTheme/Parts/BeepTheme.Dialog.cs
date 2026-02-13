using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyDialog()
        {
            this.DialogBackColor = SurfaceColor;
            this.DialogForeColor = ForeColor;
            this.DialogYesButtonBackColor = PrimaryColor;
            this.DialogYesButtonForeColor = ForeColor;
            this.DialogYesButtonHoverBackColor = PanelGradiantStartColor;
            this.DialogYesButtonHoverForeColor = ForeColor;
            this.DialogYesButtonHoverBorderColor = ActiveBorderColor;
            this.DialogCancelButtonBackColor = SecondaryColor;
            this.DialogCancelButtonForeColor = ForeColor;
            this.DialogCancelButtonHoverBackColor = PanelGradiantStartColor;
            this.DialogCancelButtonHoverForeColor = ForeColor;
            this.DialogCancelButtonHoverBorderColor = ActiveBorderColor;
            this.DialogCloseButtonBackColor = SecondaryColor;
            this.DialogCloseButtonForeColor = ForeColor;
            this.DialogCloseButtonHoverBackColor = PanelGradiantStartColor;
            this.DialogCloseButtonHoverForeColor = ForeColor;
            this.DialogCloseButtonHoverBorderColor = ActiveBorderColor;
            this.DialogHelpButtonBackColor = SecondaryColor;
            this.DialogNoButtonBackColor = SecondaryColor;
            this.DialogNoButtonForeColor = ForeColor;
            this.DialogNoButtonHoverBackColor = PanelGradiantStartColor;
            this.DialogNoButtonHoverForeColor = ForeColor;
            this.DialogNoButtonHoverBorderColor = ActiveBorderColor;
            this.DialogOkButtonBackColor = PrimaryColor;
            this.DialogOkButtonForeColor = ForeColor;
            this.DialogOkButtonHoverBackColor = PanelGradiantStartColor;
            this.DialogOkButtonHoverForeColor = ForeColor;
            this.DialogOkButtonHoverBorderColor = ActiveBorderColor;
            this.DialogWarningButtonBackColor = WarningColor;
            this.DialogWarningButtonForeColor = OnBackgroundColor;
            this.DialogWarningButtonHoverBackColor = WarningColor;
            this.DialogWarningButtonHoverForeColor = OnBackgroundColor;
            this.DialogWarningButtonHoverBorderColor = ActiveBorderColor;
            this.DialogErrorButtonBackColor = ErrorColor;
            this.DialogErrorButtonForeColor = OnPrimaryColor;
            this.DialogErrorButtonHoverBackColor = ErrorColor;
            this.DialogErrorButtonHoverForeColor = OnPrimaryColor;
            this.DialogErrorButtonHoverBorderColor = ActiveBorderColor;
            this.DialogInformationButtonBackColor = SecondaryColor;
            this.DialogInformationButtonForeColor = OnPrimaryColor;
            this.DialogInformationButtonHoverBackColor = SecondaryColor;
            this.DialogInformationButtonHoverForeColor = OnPrimaryColor;
            this.DialogInformationButtonHoverBorderColor = ActiveBorderColor;
            this.DialogQuestionButtonBackColor = PrimaryColor;
            this.DialogQuestionButtonForeColor = ForeColor;
            this.DialogQuestionButtonHoverBackColor = PanelGradiantStartColor;
            this.DialogQuestionButtonHoverForeColor = ForeColor;
            this.DialogQuestionButtonHoverBorderColor = ActiveBorderColor;
        }
    }
}
