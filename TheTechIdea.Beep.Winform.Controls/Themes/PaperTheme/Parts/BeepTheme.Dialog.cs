using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyDialog()
        {
            this.DialogBackColor = BackgroundColor;
            this.DialogForeColor = ForeColor;
            this.DialogYesButtonBackColor = PrimaryColor;
            this.DialogYesButtonForeColor = ForeColor;
            this.DialogYesButtonHoverBackColor = BackgroundColor;
            this.DialogYesButtonHoverForeColor = ForeColor;
            this.DialogYesButtonHoverBorderColor = InactiveBorderColor;
            this.DialogCancelButtonBackColor = SecondaryColor;
            this.DialogCancelButtonForeColor = ForeColor;
            this.DialogCancelButtonHoverBackColor = BackgroundColor;
            this.DialogCancelButtonHoverForeColor = ForeColor;
            this.DialogCancelButtonHoverBorderColor = InactiveBorderColor;
            this.DialogCloseButtonBackColor = SecondaryColor;
            this.DialogCloseButtonForeColor = ForeColor;
            this.DialogCloseButtonHoverBackColor = BackgroundColor;
            this.DialogCloseButtonHoverForeColor = ForeColor;
            this.DialogCloseButtonHoverBorderColor = InactiveBorderColor;
            this.DialogHelpButtonBackColor = SecondaryColor;
            this.DialogNoButtonBackColor = SecondaryColor;
            this.DialogNoButtonForeColor = ForeColor;
            this.DialogNoButtonHoverBackColor = BackgroundColor;
            this.DialogNoButtonHoverForeColor = ForeColor;
            this.DialogNoButtonHoverBorderColor = InactiveBorderColor;
            this.DialogOkButtonBackColor = PrimaryColor;
            this.DialogOkButtonForeColor = ForeColor;
            this.DialogOkButtonHoverBackColor = BackgroundColor;
            this.DialogOkButtonHoverForeColor = ForeColor;
            this.DialogOkButtonHoverBorderColor = InactiveBorderColor;
            this.DialogWarningButtonBackColor = WarningColor;
            this.DialogWarningButtonForeColor = ForeColor;
            this.DialogWarningButtonHoverBackColor = BackgroundColor;
            this.DialogWarningButtonHoverForeColor = ForeColor;
            this.DialogWarningButtonHoverBorderColor = InactiveBorderColor;
            this.DialogErrorButtonBackColor = ErrorColor;
            this.DialogErrorButtonForeColor = ForeColor;
            this.DialogErrorButtonHoverBackColor = BackgroundColor;
            this.DialogErrorButtonHoverForeColor = ForeColor;
            this.DialogErrorButtonHoverBorderColor = InactiveBorderColor;
            this.DialogInformationButtonBackColor = PrimaryColor;
            this.DialogInformationButtonForeColor = ForeColor;
            this.DialogInformationButtonHoverBackColor = BackgroundColor;
            this.DialogInformationButtonHoverForeColor = ForeColor;
            this.DialogInformationButtonHoverBorderColor = InactiveBorderColor;
            this.DialogQuestionButtonBackColor = PrimaryColor;
            this.DialogQuestionButtonForeColor = ForeColor;
            this.DialogQuestionButtonHoverBackColor = BackgroundColor;
            this.DialogQuestionButtonHoverForeColor = ForeColor;
            this.DialogQuestionButtonHoverBorderColor = InactiveBorderColor;
        }
    }
}
