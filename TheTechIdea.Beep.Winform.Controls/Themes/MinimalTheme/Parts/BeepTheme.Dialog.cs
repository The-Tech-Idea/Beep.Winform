using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyDialog()
        {
            this.DialogBackColor = BackgroundColor;
            this.DialogForeColor = ForeColor;
            this.DialogYesButtonBackColor = PrimaryColor;
            this.DialogYesButtonForeColor = OnPrimaryColor;
            this.DialogYesButtonHoverBackColor = PrimaryColor;
            this.DialogYesButtonHoverForeColor = OnPrimaryColor;
            this.DialogYesButtonHoverBorderColor = ActiveBorderColor;
            this.DialogCancelButtonBackColor = SurfaceColor;
            this.DialogCancelButtonForeColor = ForeColor;
            this.DialogCancelButtonHoverBackColor = SurfaceColor;
            this.DialogCancelButtonHoverForeColor = ForeColor;
            this.DialogCancelButtonHoverBorderColor = ActiveBorderColor;
            this.DialogCloseButtonBackColor = SurfaceColor;
            this.DialogCloseButtonForeColor = ForeColor;
            this.DialogCloseButtonHoverBackColor = SurfaceColor;
            this.DialogCloseButtonHoverForeColor = ForeColor;
            this.DialogCloseButtonHoverBorderColor = ActiveBorderColor;
            this.DialogHelpButtonBackColor = SurfaceColor;
            this.DialogNoButtonBackColor = SurfaceColor;
            this.DialogNoButtonForeColor = ForeColor;
            this.DialogNoButtonHoverBackColor = SurfaceColor;
            this.DialogNoButtonHoverForeColor = ForeColor;
            this.DialogNoButtonHoverBorderColor = ActiveBorderColor;
            this.DialogOkButtonBackColor = PrimaryColor;
            this.DialogOkButtonForeColor = OnPrimaryColor;
            this.DialogOkButtonHoverBackColor = PrimaryColor;
            this.DialogOkButtonHoverForeColor = OnPrimaryColor;
            this.DialogOkButtonHoverBorderColor = ActiveBorderColor;
            this.DialogWarningButtonBackColor = WarningColor;
            this.DialogWarningButtonForeColor = OnPrimaryColor;
            this.DialogWarningButtonHoverBackColor = WarningColor;
            this.DialogWarningButtonHoverForeColor = OnPrimaryColor;
            this.DialogWarningButtonHoverBorderColor = ActiveBorderColor;
            this.DialogErrorButtonBackColor = ErrorColor;
            this.DialogErrorButtonForeColor = OnPrimaryColor;
            this.DialogErrorButtonHoverBackColor = ErrorColor;
            this.DialogErrorButtonHoverForeColor = OnPrimaryColor;
            this.DialogErrorButtonHoverBorderColor = ActiveBorderColor;
            this.DialogInformationButtonBackColor = PrimaryColor;
            this.DialogInformationButtonForeColor = OnPrimaryColor;
            this.DialogInformationButtonHoverBackColor = PrimaryColor;
            this.DialogInformationButtonHoverForeColor = OnPrimaryColor;
            this.DialogInformationButtonHoverBorderColor = ActiveBorderColor;
            this.DialogQuestionButtonBackColor = SurfaceColor;
            this.DialogQuestionButtonForeColor = ForeColor;
            this.DialogQuestionButtonHoverBackColor = SurfaceColor;
            this.DialogQuestionButtonHoverForeColor = ForeColor;
            this.DialogQuestionButtonHoverBorderColor = ActiveBorderColor;
        }
    }
}
