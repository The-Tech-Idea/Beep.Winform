using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyDialog()
        {
            this.DialogBackColor = PanelBackColor;
            this.DialogForeColor = ForeColor;
            this.DialogYesButtonBackColor = PrimaryColor;
            this.DialogYesButtonForeColor = ForeColor;
            this.DialogYesButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogYesButtonHoverForeColor = ForeColor;
            this.DialogYesButtonHoverBorderColor = ActiveBorderColor;
            this.DialogCancelButtonBackColor = SecondaryColor;
            this.DialogCancelButtonForeColor = ForeColor;
            this.DialogCancelButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogCancelButtonHoverForeColor = ForeColor;
            this.DialogCancelButtonHoverBorderColor = ActiveBorderColor;
            this.DialogCloseButtonBackColor = SecondaryColor;
            this.DialogCloseButtonForeColor = ForeColor;
            this.DialogCloseButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogCloseButtonHoverForeColor = ForeColor;
            this.DialogCloseButtonHoverBorderColor = ActiveBorderColor;
            this.DialogHelpButtonBackColor = SecondaryColor;
            this.DialogNoButtonBackColor = SecondaryColor;
            this.DialogNoButtonForeColor = ForeColor;
            this.DialogNoButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogNoButtonHoverForeColor = ForeColor;
            this.DialogNoButtonHoverBorderColor = ActiveBorderColor;
            this.DialogOkButtonBackColor = PrimaryColor;
            this.DialogOkButtonForeColor = ForeColor;
            this.DialogOkButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogOkButtonHoverForeColor = ForeColor;
            this.DialogOkButtonHoverBorderColor = ActiveBorderColor;
            this.DialogWarningButtonBackColor = WarningColor;
            this.DialogWarningButtonForeColor = ForeColor;
            this.DialogWarningButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogWarningButtonHoverForeColor = ForeColor;
            this.DialogWarningButtonHoverBorderColor = ActiveBorderColor;
            this.DialogErrorButtonBackColor = ErrorColor;
            this.DialogErrorButtonForeColor = ForeColor;
            this.DialogErrorButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogErrorButtonHoverForeColor = ForeColor;
            this.DialogErrorButtonHoverBorderColor = ActiveBorderColor;
            this.DialogInformationButtonBackColor = PrimaryColor;
            this.DialogInformationButtonForeColor = ForeColor;
            this.DialogInformationButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogInformationButtonHoverForeColor = ForeColor;
            this.DialogInformationButtonHoverBorderColor = ActiveBorderColor;
            this.DialogQuestionButtonBackColor = PrimaryColor;
            this.DialogQuestionButtonForeColor = ForeColor;
            this.DialogQuestionButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogQuestionButtonHoverForeColor = ForeColor;
            this.DialogQuestionButtonHoverBorderColor = ActiveBorderColor;
        }
    }
}
