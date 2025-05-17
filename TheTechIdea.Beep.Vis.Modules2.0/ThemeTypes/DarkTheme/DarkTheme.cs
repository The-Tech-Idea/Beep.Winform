using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules.ThemeTypes
{
    public partial class DarkTheme : BeepTheme
    {
        public DarkTheme()
        {
            // Core theme properties
            ThemeGuid = Guid.NewGuid().ToString();
            FontFamily = "Roboto"; // Clean, modern font for a sleek dark look
            FontSize = 12f;
            IsDarkTheme = true; // Dark theme for a classic dark mode

            // Base Colors (Dark Palette)
            PrimaryColor = Color.FromArgb(0, 150, 136);     // Teal (subtle highlight)
            SecondaryColor = Color.FromArgb(66, 66, 66);    // Dark Gray (muted depth)
            AccentColor = Color.FromArgb(189, 189, 189);    // Silver (soft shine)
            BackgroundColor = Color.FromArgb(33, 33, 33);   // Near-Black (deep base)
            SurfaceColor = Color.FromArgb(55, 55, 55);      // Slightly Lighter Gray (surface depth)
            ErrorColor = Color.FromArgb(244, 67, 54);       // Red (clear warning)
            WarningColor = Color.FromArgb(255, 152, 0);     // Orange (visible caution)
            SuccessColor = Color.FromArgb(76, 175, 80);     // Green (calm success)
            OnPrimaryColor = Color.FromArgb(255, 255, 255); // White for text on primary
            OnBackgroundColor = Color.FromArgb(224, 224, 224); // Light Gray for text on background

            // UI Elements
            BackColor = Color.FromArgb(33, 33, 33);
            PanelBackColor = Color.FromArgb(55, 55, 55);
            DisabledBackColor = Color.FromArgb(90, 90, 90);
            DisabledForeColor = Color.FromArgb(120, 120, 120);
            DisabledBorderColor = Color.FromArgb(100, 100, 100);
            BorderColor = Color.FromArgb(97, 97, 97);       // Medium Gray (subtle outline)
            ActiveBorderColor = Color.FromArgb(0, 150, 136);
            InactiveBorderColor = Color.FromArgb(97, 97, 97);

            // Gradient Properties (Dark gradient)
            GradientStartColor = Color.FromArgb(33, 33, 33);
            GradientEndColor = Color.FromArgb(50, 50, 50);  // Slightly Lighter Gray (smooth fade)
            GradientDirection = LinearGradientMode.Vertical;

            // AppBar
            AppBarBackColor = Color.FromArgb(0, 150, 136);
            AppBarForeColor = Color.FromArgb(255, 255, 255);
            AppBarButtonForeColor = Color.FromArgb(255, 255, 255);
            AppBarButtonBackColor = Color.FromArgb(66, 66, 66);
            AppBarTextBoxBackColor = Color.FromArgb(55, 55, 55);
            AppBarTextBoxForeColor = Color.FromArgb(224, 224, 224);
            AppBarLabelForeColor = Color.FromArgb(255, 255, 255);
            AppBarLabelBackColor = Color.FromArgb(0, 150, 136);
            AppBarTitleForeColor = Color.FromArgb(255, 255, 255);
            AppBarTitleBackColor = Color.FromArgb(0, 150, 136);
            AppBarSubTitleForeColor = Color.FromArgb(189, 189, 189);
            AppBarSubTitleBackColor = Color.FromArgb(0, 150, 136);
            AppBarCloseButtonColor = Color.FromArgb(244, 67, 54);
            AppBarMaxButtonColor = Color.FromArgb(76, 175, 80);
            AppBarMinButtonColor = Color.FromArgb(255, 152, 0);
            AppBarTitleStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 18f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 255),
                LineHeight = 1.2f
            };
            AppBarSubTitleStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(189, 189, 189),
                LineHeight = 1.2f
            };
            AppBarTextStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(255, 255, 255),
                LineHeight = 1.2f
            };

            // Styles
            TitleStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 24f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            SubtitleStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 16f,
                FontWeight = FontWeight.Medium,
                TextColor = Color.FromArgb(66, 66, 66),
                LineHeight = 1.2f
            };
            BodyStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(224, 224, 224),
                LineHeight = 1.5f
            };
            CaptionStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 10f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(189, 189, 189),
                LineHeight = 1.2f
            };
            ButtonStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Medium,
                TextColor = Color.FromArgb(255, 255, 255),
                LineHeight = 1.2f
            };
            LinkStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(0, 150, 136),
                IsUnderlined = true,
                LineHeight = 1.2f
            };
            OverlineStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 10f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(189, 189, 189),
                LineHeight = 1.2f
            };

            // Button Colors
            ButtonHoverBackColor = Color.FromArgb(20, 170, 156);
            ButtonHoverForeColor = Color.FromArgb(255, 255, 255);
            ButtonHoverBorderColor = Color.FromArgb(0, 150, 136);
            ButtonSelectedBorderColor = Color.FromArgb(66, 66, 66);
            ButtonSelectedBackColor = Color.FromArgb(0, 150, 136);
            ButtonSelectedForeColor = Color.FromArgb(255, 255, 255);
            ButtonSelectedHoverBackColor = Color.FromArgb(20, 170, 156);
            ButtonSelectedHoverForeColor = Color.FromArgb(255, 255, 255);
            ButtonSelectedHoverBorderColor = Color.FromArgb(66, 66, 66);
            ButtonBackColor = Color.FromArgb(66, 66, 66);
            ButtonForeColor = Color.FromArgb(255, 255, 255);
            ButtonBorderColor = Color.FromArgb(0, 150, 136);
            ButtonErrorBackColor = Color.FromArgb(244, 67, 54);
            ButtonErrorForeColor = Color.FromArgb(255, 255, 255);
            ButtonErrorBorderColor = Color.FromArgb(200, 50, 40);
            ButtonPressedBackColor = Color.FromArgb(0, 130, 116);
            ButtonPressedForeColor = Color.FromArgb(255, 255, 255);
            ButtonPressedBorderColor = Color.FromArgb(0, 150, 136);

            // Textbox Colors
            TextBoxBackColor = Color.FromArgb(55, 55, 55);
            TextBoxForeColor = Color.FromArgb(224, 224, 224);
            TextBoxBorderColor = Color.FromArgb(97, 97, 97);
            TextBoxHoverBorderColor = Color.FromArgb(0, 150, 136);
            TextBoxHoverBackColor = Color.FromArgb(75, 75, 75);
            TextBoxHoverForeColor = Color.FromArgb(224, 224, 224);
            TextBoxSelectedBorderColor = Color.FromArgb(0, 150, 136);
            TextBoxSelectedBackColor = Color.FromArgb(55, 55, 55);
            TextBoxSelectedForeColor = Color.FromArgb(224, 224, 224);
            TextBoxPlaceholderColor = Color.FromArgb(189, 189, 189);
            TextBoxErrorBorderColor = Color.FromArgb(244, 67, 54);
            TextBoxErrorBackColor = Color.FromArgb(55, 55, 55);
            TextBoxErrorForeColor = Color.FromArgb(224, 224, 224);
            TextBoxErrorTextColor = Color.FromArgb(244, 67, 54);

            // Label Colors
            LabelBackColor = Color.FromArgb(55, 55, 55);
            LabelForeColor = Color.FromArgb(224, 224, 224);
            LabelBorderColor = Color.FromArgb(97, 97, 97);
            LabelHoverBorderColor = Color.FromArgb(0, 150, 136);
            LabelHoverBackColor = Color.FromArgb(75, 75, 75);
            LabelHoverForeColor = Color.FromArgb(224, 224, 224);
            LabelSelectedBorderColor = Color.FromArgb(0, 150, 136);
            LabelSelectedBackColor = Color.FromArgb(55, 55, 55);
            LabelSelectedForeColor = Color.FromArgb(224, 224, 224);
            LabelDisabledBackColor = Color.FromArgb(90, 90, 90);
            LabelDisabledForeColor = Color.FromArgb(120, 120, 120);
            LabelDisabledBorderColor = Color.FromArgb(100, 100, 100);

            // ComboBox Colors
            ComboBoxBackColor = Color.FromArgb(55, 55, 55);
            ComboBoxForeColor = Color.FromArgb(224, 224, 224);
            ComboBoxBorderColor = Color.FromArgb(97, 97, 97);

            // CheckBox Colors
            CheckBoxBackColor = Color.FromArgb(55, 55, 55);
            CheckBoxForeColor = Color.FromArgb(224, 224, 224);
            CheckBoxBorderColor = Color.FromArgb(97, 97, 97);
            CheckBoxSelectedForeColor = Color.FromArgb(255, 255, 255);
            CheckBoxSelectedBackColor = Color.FromArgb(0, 150, 136);
            CheckBoxHoverBackColor = Color.FromArgb(75, 75, 75);
            CheckBoxHoverForeColor = Color.FromArgb(224, 224, 224);
            CheckBoxHoverBorderColor = Color.FromArgb(0, 150, 136);

            // Radio Button Colors
            RadioButtonBackColor = Color.FromArgb(55, 55, 55);
            RadioButtonForeColor = Color.FromArgb(224, 224, 224);
            RadioButtonBorderColor = Color.FromArgb(97, 97, 97);
            RadioButtonSelectedForeColor = Color.FromArgb(255, 255, 255);
            RadioButtonSelectedBackColor = Color.FromArgb(0, 150, 136);
            RadioButtonHoverBackColor = Color.FromArgb(75, 75, 75);
            RadioButtonHoverForeColor = Color.FromArgb(224, 224, 224);
            RadioButtonHoverBorderColor = Color.FromArgb(0, 150, 136);

            // Progress Bar Colors
            ProgressBarBackColor = Color.FromArgb(97, 97, 97);
            ProgressBarForeColor = Color.FromArgb(0, 150, 136);
            ProgressBarBorderColor = Color.FromArgb(66, 66, 66);
            ProgressBarInsideTextColor = Color.FromArgb(255, 255, 255);
            ProgressBarHoverBackColor = Color.FromArgb(117, 117, 117);
            ProgressBarHoverForeColor = Color.FromArgb(0, 150, 136);
            ProgressBarHoverBorderColor = Color.FromArgb(20, 170, 156);
            ProgressBarHoverInsideTextColor = Color.FromArgb(255, 255, 255);

            // ScrollBar Colors
            ScrollBarBackColor = Color.FromArgb(55, 55, 55);
            ScrollBarThumbColor = Color.FromArgb(0, 150, 136);
            ScrollBarTrackColor = Color.FromArgb(97, 97, 97);
            ScrollBarHoverThumbColor = Color.FromArgb(20, 170, 156);
            ScrollBarHoverTrackColor = Color.FromArgb(117, 117, 117);
            ScrollBarActiveThumbColor = Color.FromArgb(189, 189, 189);

            // Status Bar Colors
            StatusBarBackColor = Color.FromArgb(0, 150, 136);
            StatusBarForeColor = Color.FromArgb(255, 255, 255);
            StatusBarBorderColor = Color.FromArgb(0, 130, 116);
            StatusBarHoverBackColor = Color.FromArgb(20, 170, 156);
            StatusBarHoverForeColor = Color.FromArgb(255, 255, 255);
            StatusBarHoverBorderColor = Color.FromArgb(0, 150, 136);

            // Textbox Link Colors
            LinkColor = Color.FromArgb(0, 150, 136);
            VisitedLinkColor = Color.FromArgb(0, 110, 96);
            HoverLinkColor = Color.FromArgb(20, 170, 156);

            // ToolTip Colors
            ToolTipBackColor = Color.FromArgb(55, 55, 55);
            ToolTipForeColor = Color.FromArgb(224, 224, 224);
            ToolTipBorderColor = Color.FromArgb(97, 97, 97);
            ToolTipShadowColor = Color.FromArgb(0, 0, 0);
            ToolTipShadowOpacity = Color.FromArgb(255, 20, 20, 20);
            ToolTipTextColor = Color.FromArgb(224, 224, 224);
            ToolTipLinkColor = Color.FromArgb(0, 150, 136);
            ToolTipLinkHoverColor = Color.FromArgb(20, 170, 156);
            ToolTipLinkVisitedColor = Color.FromArgb(0, 110, 96);

            // Tab Colors
            TabBackColor = Color.FromArgb(55, 55, 55);
            TabForeColor = Color.FromArgb(224, 224, 224);
            ActiveTabBackColor = Color.FromArgb(0, 150, 136);
            ActiveTabForeColor = Color.FromArgb(255, 255, 255);
            InactiveTabBackColor = Color.FromArgb(55, 55, 55);
            InactiveTabForeColor = Color.FromArgb(189, 189, 189);
            TabBorderColor = Color.FromArgb(97, 97, 97);
            TabHoverBackColor = Color.FromArgb(75, 75, 75);
            TabHoverForeColor = Color.FromArgb(224, 224, 224);
            TabSelectedBackColor = Color.FromArgb(0, 150, 136);
            TabSelectedForeColor = Color.FromArgb(255, 255, 255);
            TabSelectedBorderColor = Color.FromArgb(0, 150, 136);
            TabHoverBorderColor = Color.FromArgb(20, 170, 156);

            // Dialog Colors
            DialogBackColor = Color.FromArgb(55, 55, 55);
            DialogForeColor = Color.FromArgb(224, 224, 224);
            DialogYesButtonBackColor = Color.FromArgb(76, 175, 80);
            DialogYesButtonForeColor = Color.FromArgb(255, 255, 255);
            DialogYesButtonHoverBackColor = Color.FromArgb(96, 195, 100);
            DialogYesButtonHoverForeColor = Color.FromArgb(255, 255, 255);
            DialogYesButtonHoverBorderColor = Color.FromArgb(76, 175, 80);
            DialogCancelButtonBackColor = Color.FromArgb(97, 97, 97);
            DialogCancelButtonForeColor = Color.FromArgb(224, 224, 224);
            DialogCancelButtonHoverBackColor = Color.FromArgb(117, 117, 117);
            DialogCancelButtonHoverForeColor = Color.FromArgb(224, 224, 224);
            DialogCancelButtonHoverBorderColor = Color.FromArgb(97, 97, 97);
            DialogCloseButtonBackColor = Color.FromArgb(244, 67, 54);
            DialogCloseButtonForeColor = Color.FromArgb(255, 255, 255);
            DialogCloseButtonHoverBackColor = Color.FromArgb(255, 87, 74);
            DialogCloseButtonHoverForeColor = Color.FromArgb(255, 255, 255);
            DialogCloseButtonHoverBorderColor = Color.FromArgb(244, 67, 54);
            DialogHelpButtonBackColor = Color.FromArgb(0, 150, 136);
            DialogNoButtonBackColor = Color.FromArgb(97, 97, 97);
            DialogNoButtonForeColor = Color.FromArgb(224, 224, 224);
            DialogNoButtonHoverBackColor = Color.FromArgb(117, 117, 117);
            DialogNoButtonHoverForeColor = Color.FromArgb(224, 224, 224);
            DialogNoButtonHoverBorderColor = Color.FromArgb(97, 97, 97);
            DialogOkButtonBackColor = Color.FromArgb(0, 150, 136);
            DialogOkButtonForeColor = Color.FromArgb(255, 255, 255);
            DialogOkButtonHoverBackColor = Color.FromArgb(20, 170, 156);
            DialogOkButtonHoverForeColor = Color.FromArgb(255, 255, 255);
            DialogOkButtonHoverBorderColor = Color.FromArgb(0, 150, 136);
            DialogWarningButtonBackColor = Color.FromArgb(255, 152, 0);
            DialogWarningButtonForeColor = Color.FromArgb(255, 255, 255);
            DialogWarningButtonHoverBackColor = Color.FromArgb(255, 172, 20);
            DialogWarningButtonHoverForeColor = Color.FromArgb(255, 255, 255);
            DialogWarningButtonHoverBorderColor = Color.FromArgb(255, 152, 0);
            DialogErrorButtonBackColor = Color.FromArgb(244, 67, 54);
            DialogErrorButtonForeColor = Color.FromArgb(255, 255, 255);
            DialogErrorButtonHoverBackColor = Color.FromArgb(255, 87, 74);
            DialogErrorButtonHoverForeColor = Color.FromArgb(255, 255, 255);
            DialogErrorButtonHoverBorderColor = Color.FromArgb(244, 67, 54);
            DialogInformationButtonBackColor = Color.FromArgb(0, 150, 136);
            DialogInformationButtonForeColor = Color.FromArgb(255, 255, 255);
            DialogInformationButtonHoverBackColor = Color.FromArgb(20, 170, 156);
            DialogInformationButtonHoverForeColor = Color.FromArgb(255, 255, 255);
            DialogInformationButtonHoverBorderColor = Color.FromArgb(0, 150, 136);
            DialogQuestionButtonBackColor = Color.FromArgb(0, 150, 136);
            DialogQuestionButtonForeColor = Color.FromArgb(255, 255, 255);
            DialogQuestionButtonHoverBackColor = Color.FromArgb(20, 170, 156);
            DialogQuestionButtonHoverForeColor = Color.FromArgb(255, 255, 255);
            DialogQuestionButtonHoverBorderColor = Color.FromArgb(0, 150, 136);

            // Grid Colors
            GridBackColor = Color.FromArgb(55, 55, 55);
            GridForeColor = Color.FromArgb(224, 224, 224);
            GridHeaderBackColor = Color.FromArgb(0, 150, 136);
            GridHeaderForeColor = Color.FromArgb(255, 255, 255);
            GridHeaderBorderColor = Color.FromArgb(0, 130, 116);
            GridHeaderHoverBackColor = Color.FromArgb(20, 170, 156);
            GridHeaderHoverForeColor = Color.FromArgb(255, 255, 255);
            GridHeaderSelectedBackColor = Color.FromArgb(66, 66, 66);
            GridHeaderSelectedForeColor = Color.FromArgb(255, 255, 255);
            GridHeaderHoverBorderColor = Color.FromArgb(0, 150, 136);
            GridHeaderSelectedBorderColor = Color.FromArgb(66, 66, 66);
            GridRowHoverBackColor = Color.FromArgb(75, 75, 75);
            GridRowHoverForeColor = Color.FromArgb(224, 224, 224);
            GridRowSelectedBackColor = Color.FromArgb(0, 150, 136);
            GridRowSelectedForeColor = Color.FromArgb(255, 255, 255);
            GridRowHoverBorderColor = Color.FromArgb(20, 170, 156);
            GridRowSelectedBorderColor = Color.FromArgb(0, 150, 136);
            GridLineColor = Color.FromArgb(97, 97, 97);
            RowBackColor = Color.FromArgb(55, 55, 55);
            RowForeColor = Color.FromArgb(224, 224, 224);
            AltRowBackColor = Color.FromArgb(45, 45, 45);
            SelectedRowBackColor = Color.FromArgb(0, 150, 136);
            SelectedRowForeColor = Color.FromArgb(255, 255, 255);

            // Card Colors
            CardTextForeColor = Color.FromArgb(224, 224, 224);
            CardBackColor = Color.FromArgb(55, 55, 55);
            CardTitleForeColor = Color.FromArgb(0, 150, 136);
            CardSubTitleForeColor = Color.FromArgb(66, 66, 66);
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 18f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(224, 224, 224),
                LineHeight = 1.5f
            };
            CardSubTitleStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Medium,
                TextColor = Color.FromArgb(66, 66, 66),
                LineHeight = 1.2f
            };

            // Side Menu Colors
            SideMenuBackColor = Color.FromArgb(0, 150, 136);
            SideMenuHoverBackColor = Color.FromArgb(20, 170, 156);
            SideMenuSelectedBackColor = Color.FromArgb(66, 66, 66);
            SideMenuForeColor = Color.FromArgb(255, 255, 255);
            SideMenuSelectedForeColor = Color.FromArgb(255, 255, 255);
            SideMenuHoverForeColor = Color.FromArgb(255, 255, 255);
            SideMenuBorderColor = Color.FromArgb(0, 130, 116);
            SideMenuTitleTextColor = Color.FromArgb(255, 255, 255);
            SideMenuTitleBackColor = Color.FromArgb(0, 150, 136);
            SideMenuTitleStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 16f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 255),
                LineHeight = 1.2f
            };
            SideMenuSubTitleTextColor = Color.FromArgb(189, 189, 189);
            SideMenuSubTitleBackColor = Color.FromArgb(0, 150, 136);
            SideMenuSubTitleStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 10f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(189, 189, 189),
                LineHeight = 1.2f
            };

            // Dashboard Colors
            DashboardBackColor = Color.FromArgb(33, 33, 33);
            DashboardCardBackColor = Color.FromArgb(55, 55, 55);
            DashboardCardHoverBackColor = Color.FromArgb(75, 75, 75);
            DashboardTitleForeColor = Color.FromArgb(0, 150, 136);
            DashboardTitleBackColor = Color.FromArgb(33, 33, 33);
            DashboardTitleStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 24f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            DashboardSubTitleForeColor = Color.FromArgb(66, 66, 66);
            DashboardSubTitleBackColor = Color.FromArgb(33, 33, 33);
            DashboardSubTitleStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 16f,
                FontWeight = FontWeight.Medium,
                TextColor = Color.FromArgb(66, 66, 66),
                LineHeight = 1.2f
            };

            // Chart Colors
            ChartBackColor = Color.FromArgb(55, 55, 55);
            ChartLineColor = Color.FromArgb(0, 150, 136);
            ChartFillColor = Color.FromArgb(97, 97, 97);
            ChartAxisColor = Color.FromArgb(189, 189, 189);
            ChartTitleColor = Color.FromArgb(0, 150, 136);
            ChartTextColor = Color.FromArgb(224, 224, 224);
            ChartLegendBackColor = Color.FromArgb(33, 33, 33);
            ChartLegendTextColor = Color.FromArgb(0, 150, 136);
            ChartLegendShapeColor = Color.FromArgb(189, 189, 189);
            ChartGridLineColor = Color.FromArgb(97, 97, 97);
            ChartDefaultSeriesColors = new List<Color>
            {
                Color.FromArgb(0, 150, 136),
                Color.FromArgb(189, 189, 189),
                Color.FromArgb(76, 175, 80),
                Color.FromArgb(255, 152, 0)
            };

            // Navigation and Breadcrumbs Colors
            NavigationBackColor = Color.FromArgb(0, 150, 136);
            NavigationForeColor = Color.FromArgb(255, 255, 255);
            NavigationHoverBackColor = Color.FromArgb(20, 170, 156);
            NavigationHoverForeColor = Color.FromArgb(255, 255, 255);

            // Badge Colors
            BadgeBackColor = Color.FromArgb(0, 150, 136);
            BadgeForeColor = Color.FromArgb(255, 255, 255);
            HighlightBackColor = Color.FromArgb(189, 189, 189);

            // Menu Colors
            MenuBackColor = Color.FromArgb(0, 150, 136);
            MenuForeColor = Color.FromArgb(255, 255, 255);
            MenuBorderColor = Color.FromArgb(0, 130, 116);
            MenuMainItemForeColor = Color.FromArgb(255, 255, 255);
            MenuMainItemHoverForeColor = Color.FromArgb(255, 255, 255);
            MenuMainItemHoverBackColor = Color.FromArgb(20, 170, 156);
            MenuMainItemSelectedForeColor = Color.FromArgb(255, 255, 255);
            MenuMainItemSelectedBackColor = Color.FromArgb(66, 66, 66);
            MenuItemForeColor = Color.FromArgb(189, 189, 189);
            MenuItemHoverForeColor = Color.FromArgb(255, 255, 255);
            MenuItemHoverBackColor = Color.FromArgb(20, 170, 156);
            MenuItemSelectedForeColor = Color.FromArgb(255, 255, 255);
            MenuItemSelectedBackColor = Color.FromArgb(66, 66, 66);

            // Tree Colors
            TreeBackColor = Color.FromArgb(55, 55, 55);
            TreeForeColor = Color.FromArgb(224, 224, 224);
            TreeBorderColor = Color.FromArgb(97, 97, 97);
            TreeNodeForeColor = Color.FromArgb(0, 150, 136);
            TreeNodeHoverForeColor = Color.FromArgb(224, 224, 224);
            TreeNodeHoverBackColor = Color.FromArgb(75, 75, 75);
            TreeNodeSelectedForeColor = Color.FromArgb(255, 255, 255);
            TreeNodeSelectedBackColor = Color.FromArgb(0, 150, 136);
            TreeNodeCheckedBoxForeColor = Color.FromArgb(255, 255, 255);
            TreeNodeCheckedBoxBackColor = Color.FromArgb(0, 150, 136);

            // Calendar Colors
            CalendarBackColor = Color.FromArgb(55, 55, 55);
            CalendarForeColor = Color.FromArgb(224, 224, 224);
            CalendarTodayForeColor = Color.FromArgb(189, 189, 189);

            // List Colors
            ListBackColor = Color.FromArgb(55, 55, 55);
            ListForeColor = Color.FromArgb(224, 224, 224);
            ListBorderColor = Color.FromArgb(97, 97, 97);
            ListItemForeColor = Color.FromArgb(0, 150, 136);
            ListItemHoverForeColor = Color.FromArgb(224, 224, 224);
            ListItemHoverBackColor = Color.FromArgb(75, 75, 75);
            ListItemSelectedForeColor = Color.FromArgb(255, 255, 255);
            ListItemSelectedBackColor = Color.FromArgb(0, 150, 136);
            ListItemSelectedBorderColor = Color.FromArgb(0, 150, 136);
            ListItemBorderColor = Color.FromArgb(97, 97, 97);
            ListItemHoverBorderColor = Color.FromArgb(20, 170, 156);

            // Star Rating Colors
            StarRatingForeColor = Color.FromArgb(224, 224, 224);
            StarRatingBackColor = Color.FromArgb(55, 55, 55);
            StarRatingBorderColor = Color.FromArgb(97, 97, 97);
            StarRatingFillColor = Color.FromArgb(189, 189, 189);
            StarRatingHoverForeColor = Color.FromArgb(224, 224, 224);
            StarRatingHoverBackColor = Color.FromArgb(75, 75, 75);
            StarRatingHoverBorderColor = Color.FromArgb(0, 150, 136);
            StarRatingSelectedForeColor = Color.FromArgb(255, 255, 255);
            StarRatingSelectedBackColor = Color.FromArgb(189, 189, 189);
            StarRatingSelectedBorderColor = Color.FromArgb(0, 150, 136);

            // Stats Card Colors
            StatsCardBackColor = Color.FromArgb(55, 55, 55);
            StatsCardForeColor = Color.FromArgb(224, 224, 224);
            StatsCardBorderColor = Color.FromArgb(97, 97, 97);
            StatsCardTitleForeColor = Color.FromArgb(0, 150, 136);
            StatsCardTitleBackColor = Color.FromArgb(55, 55, 55);
            StatsCardTitleStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 18f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            StatsCardSubTitleForeColor = Color.FromArgb(66, 66, 66);
            StatsCardSubTitleBackColor = Color.FromArgb(55, 55, 55);
            StatsCardSubStyleStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Medium,
                TextColor = Color.FromArgb(66, 66, 66),
                LineHeight = 1.2f
            };
            StatsCardValueForeColor = Color.FromArgb(189, 189, 189);
            StatsCardValueBackColor = Color.FromArgb(55, 55, 55);
            StatsCardValueBorderColor = Color.FromArgb(97, 97, 97);
            StatsCardValueHoverForeColor = Color.FromArgb(209, 209, 209);
            StatsCardValueHoverBackColor = Color.FromArgb(75, 75, 75);
            StatsCardValueHoverBorderColor = Color.FromArgb(0, 150, 136);
            StatsCardValueStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 24f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(189, 189, 189),
                LineHeight = 1.2f
            };
            StatsCardInfoForeColor = Color.FromArgb(189, 189, 189);
            StatsCardInfoBackColor = Color.FromArgb(55, 55, 55);
            StatsCardInfoBorderColor = Color.FromArgb(97, 97, 97);
            StatsCardInfoStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 10f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(189, 189, 189),
                LineHeight = 1.2f
            };
            StatsCardTrendForeColor = Color.FromArgb(76, 175, 80);
            StatsCardTrendBackColor = Color.FromArgb(55, 55, 55);
            StatsCardTrendBorderColor = Color.FromArgb(97, 97, 97);
            StatsCardTrendStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Medium,
                TextColor = Color.FromArgb(76, 175, 80),
                LineHeight = 1.2f
            };

            // Switch Control Colors
            SwitchBackColor = Color.FromArgb(97, 97, 97);
            SwitchBorderColor = Color.FromArgb(0, 150, 136);
            SwitchForeColor = Color.FromArgb(224, 224, 224);
            SwitchSelectedBackColor = Color.FromArgb(0, 150, 136);
            SwitchSelectedBorderColor = Color.FromArgb(0, 150, 136);
            SwitchSelectedForeColor = Color.FromArgb(255, 255, 255);
            SwitchHoverBackColor = Color.FromArgb(117, 117, 117);
            SwitchHoverBorderColor = Color.FromArgb(20, 170, 156);
            SwitchHoverForeColor = Color.FromArgb(224, 224, 224);

            // Task Card Colors
            TaskCardBackColor = Color.FromArgb(55, 55, 55);
            TaskCardForeColor = Color.FromArgb(224, 224, 224);
            TaskCardBorderColor = Color.FromArgb(97, 97, 97);
            TaskCardTitleForeColor = Color.FromArgb(0, 150, 136);
            TaskCardTitleBackColor = Color.FromArgb(55, 55, 55);
            TaskCardTitleStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 18f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            TaskCardSubTitleForeColor = Color.FromArgb(66, 66, 66);
            TaskCardSubTitleBackColor = Color.FromArgb(55, 55, 55);
            TaskCardSubStyleStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Medium,
                TextColor = Color.FromArgb(66, 66, 66),
                LineHeight = 1.2f
            };
            TaskCardMetricTextForeColor = Color.FromArgb(189, 189, 189);
            TaskCardMetricTextBackColor = Color.FromArgb(55, 55, 55);
            TaskCardMetricTextBorderColor = Color.FromArgb(97, 97, 97);
            TaskCardMetricTextHoverForeColor = Color.FromArgb(209, 209, 209);
            TaskCardMetricTextHoverBackColor = Color.FromArgb(75, 75, 75);
            TaskCardMetricTextHoverBorderColor = Color.FromArgb(0, 150, 136);
            TaskCardMetricTextStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 16f,
                FontWeight = FontWeight.Medium,
                TextColor = Color.FromArgb(189, 189, 189),
                LineHeight = 1.2f
            };
            TaskCardProgressValueForeColor = Color.FromArgb(76, 175, 80);
            TaskCardProgressValueBackColor = Color.FromArgb(55, 55, 55);
            TaskCardProgressValueBorderColor = Color.FromArgb(97, 97, 97);
            TaskCardProgressValueStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Medium,
                TextColor = Color.FromArgb(76, 175, 80),
                LineHeight = 1.2f
            };

            // Testimony Colors
            TestimonialBackColor = Color.FromArgb(55, 55, 55);
            TestimonialTextColor = Color.FromArgb(224, 224, 224);
            TestimonialNameColor = Color.FromArgb(0, 150, 136);
            TestimonialDetailsColor = Color.FromArgb(66, 66, 66);
            TestimonialDateColor = Color.FromArgb(189, 189, 189);
            TestimonialRatingColor = Color.FromArgb(189, 189, 189);
            TestimonialStatusColor = Color.FromArgb(76, 175, 80);

            // Company Colors
            CompanyPopoverBackgroundColor = Color.FromArgb(55, 55, 55);
            CompanyTitleColor = Color.FromArgb(0, 150, 136);
            CompanySubtitleColor = Color.FromArgb(66, 66, 66);
            CompanyDescriptionColor = Color.FromArgb(224, 224, 224);
            CompanyLinkColor = Color.FromArgb(0, 150, 136);
            CompanyButtonBackgroundColor = Color.FromArgb(0, 150, 136);
            CompanyButtonTextColor = Color.FromArgb(255, 255, 255);
            CompanyDropdownBackgroundColor = Color.FromArgb(55, 55, 55);
            CompanyDropdownTextColor = Color.FromArgb(224, 224, 224);
            CompanyLogoBackgroundColor = Color.FromArgb(33, 33, 33);

            // Login Colors
            LoginPopoverBackgroundColor = Color.FromArgb(55, 55, 55);
            LoginTitleColor = Color.FromArgb(0, 150, 136);
            LoginSubtitleColor = Color.FromArgb(66, 66, 66);
            LoginDescriptionColor = Color.FromArgb(224, 224, 224);
            LoginLinkColor = Color.FromArgb(0, 150, 136);
            LoginButtonBackgroundColor = Color.FromArgb(0, 150, 136);
            LoginButtonTextColor = Color.FromArgb(255, 255, 255);
            LoginDropdownBackgroundColor = Color.FromArgb(55, 55, 55);
            LoginDropdownTextColor = Color.FromArgb(224, 224, 224);
            LoginLogoBackgroundColor = Color.FromArgb(33, 33, 33);

            // Typography
            Heading1 = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 32f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            Heading2 = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 28f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            Heading3 = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 24f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            Heading4 = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 20f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            Heading5 = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 18f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            Heading6 = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 16f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            Paragraph = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(224, 224, 224),
                LineHeight = 1.5f
            };
            Blockquote = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(66, 66, 66),
                LineHeight = 1.5f,
                FontStyle = FontStyle.Italic
            };
            BlockquoteBorderColor = Color.FromArgb(0, 150, 136);
            BlockquoteBorderWidth = 2f;
            BlockquotePadding = 10f;
            InlineCode = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(189, 189, 189),
                LineHeight = 1.2f
            };
            InlineCodeBackgroundColor = Color.FromArgb(75, 75, 75);
            InlineCodePadding = 2f;
            CodeBlock = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(224, 224, 224),
                LineHeight = 1.5f
            };
            CodeBlockBackgroundColor = Color.FromArgb(33, 33, 33);
            CodeBlockBorderColor = Color.FromArgb(97, 97, 97);
            CodeBlockBorderWidth = 1f;
            CodeBlockPadding = 10f;
            UnorderedList = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(224, 224, 224),
                LineHeight = 1.5f
            };
            OrderedList = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(224, 224, 224),
                LineHeight = 1.5f
            };
            ListItemSpacing = 5f;
            ListIndentation = 20f;
            Link = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(0, 150, 136),
                IsUnderlined = true,
                LineHeight = 1.2f
            };
            LinkHoverColor = Color.FromArgb(20, 170, 156);
            LinkIsUnderline = true;
            SmallText = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 10f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(189, 189, 189),
                LineHeight = 1.2f
            };
            StrongText = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(224, 224, 224),
                LineHeight = 1.5f
            };
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(224, 224, 224),
                FontStyle = FontStyle.Italic,
                LineHeight = 1.5f
            };
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 48f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.1f
            };
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 36f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.1f
            };
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 28f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.1f
            };
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 32f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 28f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 24f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 18f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 16f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 150, 136),
                LineHeight = 1.2f
            };
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 16f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(224, 224, 224),
                LineHeight = 1.5f
            };
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(224, 224, 224),
                LineHeight = 1.5f
            };
            BodySmall = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 10f,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(224, 224, 224),
                LineHeight = 1.5f
            };
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Medium,
                TextColor = Color.FromArgb(66, 66, 66),
                LineHeight = 1.2f
            };
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 10f,
                FontWeight = FontWeight.Medium,
                TextColor = Color.FromArgb(66, 66, 66),
                LineHeight = 1.2f
            };
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 11f,
                FontWeight = FontWeight.Medium,
                TextColor = Color.FromArgb(66, 66, 66),
                LineHeight = 1.2f
            };

            // Font Families and Styles
            FontSizeBlockHeader = 24f;
            FontSizeBlockText = 14f;
            FontSizeQuestion = 16f;
            FontSizeAnswer = 14f;
            FontSizeCaption = 12f;
            FontSizeButton = 14f;
            FontStyleRegular = FontStyle.Regular;
            FontStyleBold = FontStyle.Bold;
            FontStyleItalic = FontStyle.Italic;
            PrimaryTextColor = Color.FromArgb(224, 224, 224);
            SecondaryTextColor = Color.FromArgb(66, 66, 66);
            AccentTextColor = Color.FromArgb(189, 189, 189);

            // Additional Properties
            PaddingSmall = 8;
            PaddingMedium = 16;
            PaddingLarge = 24;
            BorderRadius = 6; // Slightly rounded for a sleek, modern touch
            BorderSize = 1;   // Thin borders for a clean look
            IconSet = "Material Icons";
            ApplyThemeToIcons = true;
            ShadowColor = Color.FromArgb(0, 0, 0);
            ShadowOpacity = 0.2f; // Subtle shadow for depth
            AnimationDurationShort = 0.2;
            AnimationDurationMedium = 0.3;
            AnimationDurationLong = 0.5;
            AnimationEasingFunction = "ease-in-out";
            HighContrastMode = false;
            FocusIndicatorColor = Color.FromArgb(0, 150, 136);
        }
    }
}