using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyDialog()
        {
            this.DialogBackColor = SurfaceColor;
            this.DialogForeColor = ForeColor;
            this.DialogYesButtonBackColor = SuccessColor;
            this.DialogYesButtonForeColor = OnPrimaryColor;
            this.DialogYesButtonHoverBackColor = PanelGradiantStartColor;
            this.DialogYesButtonHoverForeColor = ForeColor;
            this.DialogYesButtonHoverBorderColor = ActiveBorderColor;
            this.DialogCancelButtonBackColor = SecondaryColor;
            this.DialogCancelButtonForeColor = ForeColor;
            this.DialogCancelButtonHoverBackColor = PanelGradiantStartColor;
            this.DialogCancelButtonHoverForeColor = ForeColor;
            this.DialogCancelButtonHoverBorderColor = ActiveBorderColor;
            this.DialogCloseButtonBackColor = ErrorColor;
            this.DialogCloseButtonForeColor = OnPrimaryColor;
            this.DialogCloseButtonHoverBackColor = PanelGradiantStartColor;
            this.DialogCloseButtonHoverForeColor = ForeColor;
            this.DialogCloseButtonHoverBorderColor = ActiveBorderColor;
            this.DialogHelpButtonBackColor = SecondaryColor;
            this.DialogNoButtonBackColor = ErrorColor;
            this.DialogNoButtonForeColor = OnPrimaryColor;
            this.DialogNoButtonHoverBackColor = PanelGradiantStartColor;
            this.DialogNoButtonHoverForeColor = ForeColor;
            this.DialogNoButtonHoverBorderColor = ActiveBorderColor;
            this.DialogOkButtonBackColor = PrimaryColor;
            this.DialogOkButtonForeColor = OnPrimaryColor;
            this.DialogOkButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogOkButtonHoverForeColor = ForeColor;
            this.DialogOkButtonHoverBorderColor = ActiveBorderColor;
            this.DialogWarningButtonBackColor = WarningColor;
            this.DialogWarningButtonForeColor = OnPrimaryColor;
            this.DialogWarningButtonHoverBackColor = PanelGradiantStartColor;
            this.DialogWarningButtonHoverForeColor = ForeColor;
            this.DialogWarningButtonHoverBorderColor = ActiveBorderColor;
            this.DialogErrorButtonBackColor = ErrorColor;
            this.DialogErrorButtonForeColor = OnPrimaryColor;
            this.DialogErrorButtonHoverBackColor = PanelGradiantStartColor;
            this.DialogErrorButtonHoverForeColor = ForeColor;
            this.DialogErrorButtonHoverBorderColor = ActiveBorderColor;
            this.DialogInformationButtonBackColor = SecondaryColor;
            this.DialogInformationButtonForeColor = OnPrimaryColor;
            this.DialogInformationButtonHoverBackColor = PanelGradiantStartColor;
            this.DialogInformationButtonHoverForeColor = ForeColor;
            this.DialogInformationButtonHoverBorderColor = ActiveBorderColor;
            this.DialogQuestionButtonBackColor = AccentColor;
            this.DialogQuestionButtonForeColor = OnPrimaryColor;
            this.DialogQuestionButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogQuestionButtonHoverForeColor = ForeColor;
            this.DialogQuestionButtonHoverBorderColor = ActiveBorderColor;
        }
    }
}
