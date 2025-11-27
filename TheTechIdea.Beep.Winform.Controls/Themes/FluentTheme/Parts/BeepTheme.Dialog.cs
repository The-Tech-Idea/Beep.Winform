using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyDialog()
        {
            this.DialogBackColor = SurfaceColor;
            this.DialogForeColor = ForeColor;
            this.DialogYesButtonBackColor = PrimaryColor;
            this.DialogYesButtonForeColor = OnPrimaryColor;
            this.DialogYesButtonHoverBackColor = AccentColor;
            this.DialogYesButtonHoverForeColor = OnPrimaryColor;
            this.DialogYesButtonHoverBorderColor = AccentColor;
            this.DialogCancelButtonBackColor = SurfaceColor;
            this.DialogCancelButtonForeColor = ForeColor;
            this.DialogCancelButtonHoverBackColor = SecondaryColor;
            this.DialogCancelButtonHoverForeColor = ForeColor;
            this.DialogCancelButtonHoverBorderColor = ActiveBorderColor;
            this.DialogCloseButtonBackColor = SurfaceColor;
            this.DialogCloseButtonForeColor = ErrorColor;
            this.DialogCloseButtonHoverBackColor = ErrorColor;
            this.DialogCloseButtonHoverForeColor = OnPrimaryColor;
            this.DialogCloseButtonHoverBorderColor = ErrorColor;
            this.DialogHelpButtonBackColor = AccentColor;
            this.DialogNoButtonBackColor = SurfaceColor;
            this.DialogNoButtonForeColor = ForeColor;
            this.DialogNoButtonHoverBackColor = SecondaryColor;
            this.DialogNoButtonHoverForeColor = ForeColor;
            this.DialogNoButtonHoverBorderColor = ActiveBorderColor;
            this.DialogOkButtonBackColor = PrimaryColor;
            this.DialogOkButtonForeColor = OnPrimaryColor;
            this.DialogOkButtonHoverBackColor = AccentColor;
            this.DialogOkButtonHoverForeColor = OnPrimaryColor;
            this.DialogOkButtonHoverBorderColor = AccentColor;
            this.DialogWarningButtonBackColor = WarningColor;
            this.DialogWarningButtonForeColor = OnWarningColor;
            this.DialogWarningButtonHoverBackColor = WarningColor;
            this.DialogWarningButtonHoverForeColor = OnWarningColor;
            this.DialogWarningButtonHoverBorderColor = WarningColor;
            this.DialogErrorButtonBackColor = ErrorColor;
            this.DialogErrorButtonForeColor = OnPrimaryColor;
            this.DialogErrorButtonHoverBackColor = ErrorColor;
            this.DialogErrorButtonHoverForeColor = OnPrimaryColor;
            this.DialogErrorButtonHoverBorderColor = ErrorColor;
            this.DialogInformationButtonBackColor = InfoColor;
            this.DialogInformationButtonForeColor = OnInfoColor;
            this.DialogInformationButtonHoverBackColor = InfoColor;
            this.DialogInformationButtonHoverForeColor = OnInfoColor;
            this.DialogInformationButtonHoverBorderColor = InfoColor;
            this.DialogQuestionButtonBackColor = SurfaceColor;
            this.DialogQuestionButtonForeColor = ForeColor;
            this.DialogQuestionButtonHoverBackColor = SecondaryColor;
            this.DialogQuestionButtonHoverForeColor = ForeColor;
            this.DialogQuestionButtonHoverBorderColor = ActiveBorderColor;
        }
    }
}