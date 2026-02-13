using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyDialog()
        {
            this.DialogBackColor = BackgroundColor;
            this.DialogForeColor = ForeColor;
            this.DialogYesButtonBackColor = PrimaryColor;
            this.DialogYesButtonForeColor = ForeColor;
            this.DialogYesButtonHoverBackColor = BackgroundColor;
            this.DialogYesButtonHoverForeColor = ForeColor;
            this.DialogYesButtonHoverBorderColor = BorderColor;
            this.DialogCancelButtonBackColor = SecondaryColor;
            this.DialogCancelButtonForeColor = ForeColor;
            this.DialogCancelButtonHoverBackColor = BackgroundColor;
            this.DialogCancelButtonHoverForeColor = ForeColor;
            this.DialogCancelButtonHoverBorderColor = BorderColor;
            this.DialogCloseButtonBackColor = SecondaryColor;
            this.DialogCloseButtonForeColor = ForeColor;
            this.DialogCloseButtonHoverBackColor = BackgroundColor;
            this.DialogCloseButtonHoverForeColor = ForeColor;
            this.DialogCloseButtonHoverBorderColor = BorderColor;
            this.DialogHelpButtonBackColor = SecondaryColor;
            this.DialogNoButtonBackColor = SecondaryColor;
            this.DialogNoButtonForeColor = ForeColor;
            this.DialogNoButtonHoverBackColor = BackgroundColor;
            this.DialogNoButtonHoverForeColor = ForeColor;
            this.DialogNoButtonHoverBorderColor = BorderColor;
            this.DialogOkButtonBackColor = PrimaryColor;
            this.DialogOkButtonForeColor = ForeColor;
            this.DialogOkButtonHoverBackColor = BackgroundColor;
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
            this.DialogInformationButtonBackColor = SecondaryColor;
            this.DialogInformationButtonForeColor = OnPrimaryColor;
            this.DialogInformationButtonHoverBackColor = SecondaryColor;
            this.DialogInformationButtonHoverForeColor = OnPrimaryColor;
            this.DialogInformationButtonHoverBorderColor = BorderColor;
            this.DialogQuestionButtonBackColor = AccentColor;
            this.DialogQuestionButtonForeColor = OnPrimaryColor;
            this.DialogQuestionButtonHoverBackColor = AccentColor;
            this.DialogQuestionButtonHoverForeColor = OnPrimaryColor;
            this.DialogQuestionButtonHoverBorderColor = BorderColor;
        }
    }
}
