using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IBeepTheme
    {
        // Theme Identity
        string ThemeName { get; set; }
        string ThemeGuid { get; set; }

        // Core Properties
        Color BackColor { get; set; }
        Color PanelBackColor { get; set; }
        Color PanelGradiantStartColor { get; set; }
        Color PanelGradiantEndColor { get; set; }
        Color PanelGradiantMiddleColor { get; set; }
        LinearGradientMode PanelGradiantDirection { get; set; }
        Color DisabledBackColor { get; set; }
        Color DisabledForeColor { get; set; }
        Color DisabledBorderColor { get; set; }
        Color BorderColor { get; set; }
        Color ActiveBorderColor { get; set; }
        Color InactiveBorderColor { get; set; }

        // Color Palette
        Color PrimaryColor { get; set; }
        Color SecondaryColor { get; set; }
        Color AccentColor { get; set; }
        Color BackgroundColor { get; set; }
        Color SurfaceColor { get; set; }
        Color ErrorColor { get; set; }
        Color WarningColor { get; set; }
        Color SuccessColor { get; set; }
        Color OnPrimaryColor { get; set; }
        Color OnBackgroundColor { get; set; }

        // Blockquote and Code
        Color BlockquoteBorderColor { get; set; }
        Color InlineCodeBackgroundColor { get; set; }
        Color CodeBlockBackgroundColor { get; set; }
        Color CodeBlockBorderColor { get; set; }

        // Button Properties
        TypographyStyle ButtonFont { get; set; }
        TypographyStyle ButtonHoverFont { get; set; }
        TypographyStyle ButtonSelectedFont { get; set; }
        Color ButtonHoverBackColor { get; set; }
        Color ButtonHoverForeColor { get; set; }
        Color ButtonHoverBorderColor { get; set; }
        Color ButtonSelectedBorderColor { get; set; }
        Color ButtonSelectedBackColor { get; set; }
        Color ButtonSelectedForeColor { get; set; }
        Color ButtonSelectedHoverBackColor { get; set; }
        Color ButtonSelectedHoverForeColor { get; set; }
        Color ButtonSelectedHoverBorderColor { get; set; }
        Color ButtonBackColor { get; set; }
        Color ButtonForeColor { get; set; }
        Color ButtonBorderColor { get; set; }
        Color ButtonErrorBackColor { get; set; }
        Color ButtonErrorForeColor { get; set; }
        Color ButtonErrorBorderColor { get; set; }
        Color ButtonPressedBackColor { get; set; }
        Color ButtonPressedForeColor { get; set; }
        Color ButtonPressedBorderColor { get; set; }

        // Card Properties
        TypographyStyle CardTitleFont { get; set; }
        Color CardTextForeColor { get; set; }
        Color CardBackColor { get; set; }
        Color CardTitleForeColor { get; set; }
        TypographyStyle CardSubTitleFont { get; set; }
        Color CardSubTitleForeColor { get; set; }
        TypographyStyle CardHeaderStyle { get; set; }
        TypographyStyle CardparagraphStyle { get; set; }
        TypographyStyle CardSubTitleStyle { get; set; }
        Color CardrGradiantStartColor { get; set; }
        Color CardGradiantEndColor { get; set; }
        Color CardGradiantMiddleColor { get; set; }
        LinearGradientMode CardGradiantDirection { get; set; }

        // Chart Properties
        TypographyStyle ChartTitleFont { get; set; }
        TypographyStyle ChartSubTitleFont { get; set; }
        Color ChartBackColor { get; set; }
        Color ChartLineColor { get; set; }
        Color ChartFillColor { get; set; }
        Color ChartAxisColor { get; set; }
        Color ChartTitleColor { get; set; }
        Color ChartTextColor { get; set; }
        Color ChartLegendBackColor { get; set; }
        Color ChartLegendTextColor { get; set; }
        Color ChartLegendShapeColor { get; set; }
        Color ChartGridLineColor { get; set; }
        List<Color> ChartDefaultSeriesColors { get; set; }

        // CheckBox Properties
        Color CheckBoxBackColor { get; set; }
        Color CheckBoxForeColor { get; set; }
        Color CheckBoxBorderColor { get; set; }
        Color CheckBoxCheckedBackColor { get; set; }
        Color CheckBoxCheckedForeColor { get; set; }
        Color CheckBoxCheckedBorderColor { get; set; }
        Color CheckBoxHoverBackColor { get; set; }
        Color CheckBoxHoverForeColor { get; set; }
        Color CheckBoxHoverBorderColor { get; set; }
        TypographyStyle CheckBoxFont { get; set; }
        TypographyStyle CheckBoxCheckedFont { get; set; }

        // ComboBox Properties
        Color ComboBoxBackColor { get; set; }
        Color ComboBoxForeColor { get; set; }
        Color ComboBoxBorderColor { get; set; }
        Color ComboBoxHoverBackColor { get; set; }
        Color ComboBoxHoverForeColor { get; set; }
        Color ComboBoxHoverBorderColor { get; set; }
        Color ComboBoxSelectedBackColor { get; set; }
        Color ComboBoxSelectedForeColor { get; set; }
        Color ComboBoxSelectedBorderColor { get; set; }
        Color ComboBoxErrorBackColor { get; set; }
        Color ComboBoxErrorForeColor { get; set; }
        TypographyStyle ComboBoxItemFont { get; set; }
        TypographyStyle ComboBoxListFont { get; set; }

        // Company Properties
        Color CompanyPopoverBackgroundColor { get; set; }
        Color CompanyTitleColor { get; set; }
        TypographyStyle CompanyTitleFont { get; set; }
        Color CompanySubtitleColor { get; set; }
        TypographyStyle CompanySubTitleFont { get; set; }
        Color CompanyDescriptionColor { get; set; }
        TypographyStyle CompanyDescriptionFont { get; set; }
        Color CompanyLinkColor { get; set; }
        TypographyStyle CompanyLinkFont { get; set; }
        Color CompanyButtonBackgroundColor { get; set; }
        Color CompanyButtonTextColor { get; set; }
        TypographyStyle CompanyButtonFont { get; set; }
        Color CompanyDropdownBackgroundColor { get; set; }
        Color CompanyDropdownTextColor { get; set; }
        Color CompanyLogoBackgroundColor { get; set; }

        // Dashboard Properties
        TypographyStyle DashboardTitleFont { get; set; }
        TypographyStyle DashboardSubTitleFont { get; set; }
        Color DashboardBackColor { get; set; }
        Color DashboardCardBackColor { get; set; }
        Color DashboardCardHoverBackColor { get; set; }
        Color DashboardTitleForeColor { get; set; }
        Color DashboardTitleBackColor { get; set; }
        TypographyStyle DashboardTitleStyle { get; set; }
        Color DashboardSubTitleForeColor { get; set; }
        Color DashboardSubTitleBackColor { get; set; }
        TypographyStyle DashboardSubTitleStyle { get; set; }
        Color DashboardGradiantStartColor { get; set; }
        Color DashboardGradiantEndColor { get; set; }
        Color DashboardGradiantMiddleColor { get; set; }
        LinearGradientMode DashboardGradiantDirection { get; set; }

        // Dialog Properties
        Color DialogBackColor { get; set; }
        Color DialogForeColor { get; set; }
        TypographyStyle DialogYesButtonFont { get; set; }
        TypographyStyle DialogNoButtonFont { get; set; }
        TypographyStyle DialogOkButtonFont { get; set; }
        TypographyStyle DialogCancelButtonFont { get; set; }
        TypographyStyle DialogWarningButtonFont { get; set; }
        TypographyStyle DialogErrorButtonFont { get; set; }
        TypographyStyle DialogInformationButtonFont { get; set; }
        TypographyStyle DialogQuestionButtonFont { get; set; }
        TypographyStyle DialogHelpButtonFont { get; set; }
        TypographyStyle DialogCloseButtonFont { get; set; }
        TypographyStyle DialogYesButtonHoverFont { get; set; }
        TypographyStyle DialogNoButtonHoverFont { get; set; }
        TypographyStyle DialogOkButtonHoverFont { get; set; }
        Color DialogYesButtonBackColor { get; set; }
        Color DialogYesButtonForeColor { get; set; }
        Color DialogYesButtonHoverBackColor { get; set; }
        Color DialogYesButtonHoverForeColor { get; set; }
        Color DialogYesButtonHoverBorderColor { get; set; }
        Color DialogCancelButtonBackColor { get; set; }
        Color DialogCancelButtonForeColor { get; set; }
        Color DialogCancelButtonHoverBackColor { get; set; }
        Color DialogCancelButtonHoverForeColor { get; set; }
        Color DialogCancelButtonHoverBorderColor { get; set; }
        Color DialogCloseButtonBackColor { get; set; }
        Color DialogCloseButtonForeColor { get; set; }
        Color DialogCloseButtonHoverBackColor { get; set; }
        Color DialogCloseButtonHoverForeColor { get; set; }
        Color DialogCloseButtonHoverBorderColor { get; set; }
        Color DialogHelpButtonBackColor { get; set; }
        Color DialogNoButtonBackColor { get; set; }
        Color DialogNoButtonForeColor { get; set; }
        Color DialogNoButtonHoverBackColor { get; set; }
        Color DialogNoButtonHoverForeColor { get; set; }
        Color DialogNoButtonHoverBorderColor { get; set; }
        Color DialogOkButtonBackColor { get; set; }
        Color DialogOkButtonForeColor { get; set; }
        Color DialogOkButtonHoverBackColor { get; set; }
        Color DialogOkButtonHoverForeColor { get; set; }
        Color DialogOkButtonHoverBorderColor { get; set; }
        Color DialogWarningButtonBackColor { get; set; }
        Color DialogWarningButtonForeColor { get; set; }
        Color DialogWarningButtonHoverBackColor { get; set; }
        Color DialogWarningButtonHoverForeColor { get; set; }
        Color DialogWarningButtonHoverBorderColor { get; set; }
        Color DialogErrorButtonBackColor { get; set; }
        Color DialogErrorButtonForeColor { get; set; }
        Color DialogErrorButtonHoverBackColor { get; set; }
        Color DialogErrorButtonHoverForeColor { get; set; }
        Color DialogErrorButtonHoverBorderColor { get; set; }
        Color DialogInformationButtonBackColor { get; set; }
        Color DialogInformationButtonForeColor { get; set; }
        Color DialogInformationButtonHoverBackColor { get; set; }
        Color DialogInformationButtonHoverForeColor { get; set; }
        Color DialogInformationButtonHoverBorderColor { get; set; }
        Color DialogQuestionButtonBackColor { get; set; }
        Color DialogQuestionButtonForeColor { get; set; }
        Color DialogQuestionButtonHoverBackColor { get; set; }
        Color DialogQuestionButtonHoverForeColor { get; set; }
        Color DialogQuestionButtonHoverBorderColor { get; set; }
    }
}
