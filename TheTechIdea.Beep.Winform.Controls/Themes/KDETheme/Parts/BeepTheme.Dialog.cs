using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyDialog()
        {
            this.DialogBackColor = PanelBackColor;
            this.DialogForeColor = ForeColor;
            this.DialogYesButtonBackColor = PrimaryColor;
            this.DialogYesButtonForeColor = ForeColor;
            this.DialogYesButtonHoverBackColor = PanelBackColor;
            this.DialogYesButtonHoverForeColor = ForeColor;
            this.DialogYesButtonHoverBorderColor = BorderColor;
            this.DialogCancelButtonBackColor = SecondaryColor;
            this.DialogCancelButtonForeColor = ForeColor;
            this.DialogCancelButtonHoverBackColor = PanelBackColor;
            this.DialogCancelButtonHoverForeColor = ForeColor;
            this.DialogCancelButtonHoverBorderColor = BorderColor;
            this.DialogCloseButtonBackColor = SecondaryColor;
            this.DialogCloseButtonForeColor = ForeColor;
            this.DialogCloseButtonHoverBackColor = PanelBackColor;
            this.DialogCloseButtonHoverForeColor = ForeColor;
            this.DialogCloseButtonHoverBorderColor = BorderColor;
            this.DialogHelpButtonBackColor = SecondaryColor;
            this.DialogNoButtonBackColor = SecondaryColor;
            this.DialogNoButtonForeColor = ForeColor;
            this.DialogNoButtonHoverBackColor = PanelBackColor;
            this.DialogNoButtonHoverForeColor = ForeColor;
            this.DialogNoButtonHoverBorderColor = BorderColor;
            this.DialogOkButtonBackColor = PrimaryColor;
            this.DialogOkButtonForeColor = ForeColor;
            this.DialogOkButtonHoverBackColor = PanelBackColor;
            this.DialogOkButtonHoverForeColor = ForeColor;
            this.DialogOkButtonHoverBorderColor = BorderColor;
            this.DialogWarningButtonBackColor = WarningColor;
            this.DialogWarningButtonForeColor = ForeColor;
            this.DialogWarningButtonHoverBackColor = WarningColor;
            this.DialogWarningButtonHoverForeColor = ForeColor;
            this.DialogWarningButtonHoverBorderColor = BorderColor;
            this.DialogErrorButtonBackColor = ErrorColor;
            this.DialogErrorButtonForeColor = ForeColor;
            this.DialogErrorButtonHoverBackColor = ErrorColor;
            this.DialogErrorButtonHoverForeColor = ForeColor;
            this.DialogErrorButtonHoverBorderColor = BorderColor;
            this.DialogInformationButtonBackColor = AccentColor;
            this.DialogInformationButtonForeColor = ForeColor;
            this.DialogInformationButtonHoverBackColor = AccentColor;
            this.DialogInformationButtonHoverForeColor = ForeColor;
            this.DialogInformationButtonHoverBorderColor = BorderColor;
            this.DialogQuestionButtonBackColor = PrimaryColor;
            this.DialogQuestionButtonForeColor = ForeColor;
            this.DialogQuestionButtonHoverBackColor = PanelBackColor;
            this.DialogQuestionButtonHoverForeColor = ForeColor;
            this.DialogQuestionButtonHoverBorderColor = BorderColor;
        }
    }
}
