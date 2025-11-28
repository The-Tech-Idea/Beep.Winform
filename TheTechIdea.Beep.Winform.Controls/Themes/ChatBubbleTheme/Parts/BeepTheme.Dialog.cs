using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyDialog()
        {
            this.DialogBackColor = PanelGradiantStartColor;
            this.DialogForeColor = ForeColor;
            this.DialogYesButtonBackColor = PanelGradiantStartColor;
            this.DialogYesButtonForeColor = ForeColor;
            this.DialogYesButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogYesButtonHoverForeColor = ForeColor;
            this.DialogYesButtonHoverBorderColor = BorderColor;
            this.DialogCancelButtonBackColor = PanelGradiantStartColor;
            this.DialogCancelButtonForeColor = ForeColor;
            this.DialogCancelButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogCancelButtonHoverForeColor = ForeColor;
            this.DialogCancelButtonHoverBorderColor = BorderColor;
            this.DialogCloseButtonBackColor = PanelGradiantStartColor;
            this.DialogCloseButtonForeColor = ForeColor;
            this.DialogCloseButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogCloseButtonHoverForeColor = ForeColor;
            this.DialogCloseButtonHoverBorderColor = BorderColor;
            this.DialogHelpButtonBackColor = PanelGradiantStartColor;
            this.DialogNoButtonBackColor = PanelGradiantStartColor;
            this.DialogNoButtonForeColor = ForeColor;
            this.DialogNoButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogNoButtonHoverForeColor = ForeColor;
            this.DialogNoButtonHoverBorderColor = BorderColor;
            this.DialogOkButtonBackColor = PanelGradiantStartColor;
            this.DialogOkButtonForeColor = ForeColor;
            this.DialogOkButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogOkButtonHoverForeColor = ForeColor;
            this.DialogOkButtonHoverBorderColor = BorderColor;
            this.DialogWarningButtonBackColor = WarningColor;
            this.DialogWarningButtonForeColor = OnPrimaryColor;
            this.DialogWarningButtonHoverBackColor = WarningColor;
            this.DialogWarningButtonHoverForeColor = OnPrimaryColor;
            this.DialogWarningButtonHoverBorderColor = BorderColor;
            this.DialogErrorButtonBackColor = ErrorColor;
            this.DialogErrorButtonForeColor = OnPrimaryColor;
            this.DialogErrorButtonHoverBackColor = ErrorColor;
            this.DialogErrorButtonHoverForeColor = OnPrimaryColor;
            this.DialogErrorButtonHoverBorderColor = BorderColor;
            this.DialogInformationButtonBackColor = PrimaryColor;
            this.DialogInformationButtonForeColor = OnPrimaryColor;
            this.DialogInformationButtonHoverBackColor = PrimaryColor;
            this.DialogInformationButtonHoverForeColor = OnPrimaryColor;
            this.DialogInformationButtonHoverBorderColor = BorderColor;
            this.DialogQuestionButtonBackColor = PanelGradiantStartColor;
            this.DialogQuestionButtonForeColor = ForeColor;
            this.DialogQuestionButtonHoverBackColor = PanelGradiantMiddleColor;
            this.DialogQuestionButtonHoverForeColor = ForeColor;
            this.DialogQuestionButtonHoverBorderColor = BorderColor;
        }
    }
}