using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyDialog()
        {
            this.DialogBackColor = BackgroundColor;
            this.DialogForeColor = ForeColor;
            this.DialogYesButtonBackColor = PrimaryColor;
            this.DialogYesButtonForeColor = ForeColor;
            this.DialogYesButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogYesButtonHoverForeColor = ForeColor;
            this.DialogYesButtonHoverBorderColor = SecondaryColor;
            this.DialogCancelButtonBackColor = SecondaryColor;
            this.DialogCancelButtonForeColor = ForeColor;
            this.DialogCancelButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogCancelButtonHoverForeColor = ForeColor;
            this.DialogCancelButtonHoverBorderColor = SecondaryColor;
            this.DialogCloseButtonBackColor = SecondaryColor;
            this.DialogCloseButtonForeColor = ForeColor;
            this.DialogCloseButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogCloseButtonHoverForeColor = ForeColor;
            this.DialogCloseButtonHoverBorderColor = SecondaryColor;
            this.DialogHelpButtonBackColor = SecondaryColor;
            this.DialogNoButtonBackColor = SecondaryColor;
            this.DialogNoButtonForeColor = ForeColor;
            this.DialogNoButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogNoButtonHoverForeColor = ForeColor;
            this.DialogNoButtonHoverBorderColor = SecondaryColor;
            this.DialogOkButtonBackColor = PrimaryColor;
            this.DialogOkButtonForeColor = ForeColor;
            this.DialogOkButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogOkButtonHoverForeColor = ForeColor;
            this.DialogOkButtonHoverBorderColor = SecondaryColor;
            this.DialogWarningButtonBackColor = WarningColor;
            this.DialogWarningButtonForeColor = ForeColor;
            this.DialogWarningButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogWarningButtonHoverForeColor = ForeColor;
            this.DialogWarningButtonHoverBorderColor = SecondaryColor;
            this.DialogErrorButtonBackColor = ErrorColor;
            this.DialogErrorButtonForeColor = OnPrimaryColor;
            this.DialogErrorButtonHoverBackColor = ErrorColor;
            this.DialogErrorButtonHoverForeColor = OnPrimaryColor;
            this.DialogErrorButtonHoverBorderColor = SecondaryColor;
            this.DialogInformationButtonBackColor = PrimaryColor;
            this.DialogInformationButtonForeColor = ForeColor;
            this.DialogInformationButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogInformationButtonHoverForeColor = ForeColor;
            this.DialogInformationButtonHoverBorderColor = SecondaryColor;
            this.DialogQuestionButtonBackColor = PrimaryColor;
            this.DialogQuestionButtonForeColor = ForeColor;
            this.DialogQuestionButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogQuestionButtonHoverForeColor = ForeColor;
            this.DialogQuestionButtonHoverBorderColor = SecondaryColor;
        }
    }
}
