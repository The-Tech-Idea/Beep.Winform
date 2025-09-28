using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Sample;

namespace TheTechIdea.Beep.Winform.Controls.Widgets
{
    /// <summary>
    /// Legacy compatibility layer for BeepWidget samples
    /// Redirects to new refactored sample classes in Helpers.Sample namespace
    /// 
    /// REFACTORED: Individual widget types now have their own sample classes:
    /// - BeepMetricWidgetSamples.cs
    /// - BeepChartWidgetSamples.cs
    /// - BeepListWidgetSamples.cs
    /// - BeepDashboardWidgetSamples.cs
    /// - BeepControlWidgetSamples.cs
    /// - BeepNotificationWidgetSamples.cs
    /// - BeepNavigationWidgetSamples.cs
    /// - BeepFinanceWidgetSamples.cs
    /// - BeepWidgetSampleCoordinator.cs (main coordinator)
    /// - BeepWidgetEventHandler.cs (centralized event handling)
    /// </summary>
    [Obsolete("Use individual widget sample classes in Helpers.Sample namespace for better organization")]
    public static class BeepWidgetSamples
    {
        #region Metric Widget Samples (Redirects)
        public static BeepMetricWidget CreateSimpleValueWidget() => BeepMetricWidgetSamples.CreateSimpleValueWidget();
        public static BeepMetricWidget CreateTrendWidget() => BeepMetricWidgetSamples.CreateTrendWidget();
        public static BeepMetricWidget CreateProgressWidget() => BeepMetricWidgetSamples.CreateProgressWidget();
        public static BeepMetricWidget CreateGaugeWidget() => BeepMetricWidgetSamples.CreateGaugeWidget();
        public static BeepMetricWidget CreateComparisonWidget() => BeepMetricWidgetSamples.CreateComparisonWidget();
        public static BeepMetricWidget CreateCardWidget() => BeepMetricWidgetSamples.CreateCardWidget();
        #endregion

        #region Chart Widget Samples (Redirects)
        public static BeepChartWidget CreateBarChartWidget() => BeepChartWidgetSamples.CreateBarChartWidget();
        public static BeepChartWidget CreateLineChartWidget() => BeepChartWidgetSamples.CreateLineChartWidget();
        public static BeepChartWidget CreatePieChartWidget() => BeepChartWidgetSamples.CreatePieChartWidget();
        public static BeepChartWidget CreateGaugeChartWidget() => BeepChartWidgetSamples.CreateGaugeChartWidget();
        public static BeepChartWidget CreateSparklineWidget() => BeepChartWidgetSamples.CreateSparklineWidget();
        public static BeepChartWidget CreateHeatmapWidget() => BeepChartWidgetSamples.CreateHeatmapWidget();
        public static BeepChartWidget CreateCombinationChartWidget() => BeepChartWidgetSamples.CreateCombinationChartWidget();
        #endregion

        #region List Widget Samples (Redirects)
        public static BeepListWidget CreateActivityFeedWidget() => BeepListWidgetSamples.CreateActivityFeedWidget();
        public static BeepListWidget CreateDataTableWidget() => BeepListWidgetSamples.CreateDataTableWidget();
        public static BeepListWidget CreateRankingListWidget() => BeepListWidgetSamples.CreateRankingListWidget();
        public static BeepListWidget CreateStatusListWidget() => BeepListWidgetSamples.CreateStatusListWidget();
        public static BeepListWidget CreateProfileListWidget() => BeepListWidgetSamples.CreateProfileListWidget();
        public static BeepListWidget CreateTaskListWidget() => BeepListWidgetSamples.CreateTaskListWidget();
        #endregion

        #region Dashboard Widget Samples (Redirects)
        public static BeepDashboardWidget CreateMultiMetricWidget() => BeepDashboardWidgetSamples.CreateMultiMetricWidget();
        public static BeepDashboardWidget CreateChartGridWidget() => BeepDashboardWidgetSamples.CreateChartGridWidget();
        public static BeepDashboardWidget CreateTimelineWidget() => BeepDashboardWidgetSamples.CreateTimelineWidget();
        public static BeepDashboardWidget CreateStatusOverviewWidget() => BeepDashboardWidgetSamples.CreateStatusOverviewWidget();
        public static BeepDashboardWidget CreateComparisonGridWidget() => BeepDashboardWidgetSamples.CreateComparisonGridWidget();
        public static BeepDashboardWidget CreateAnalyticsPanelWidget() => BeepDashboardWidgetSamples.CreateAnalyticsPanelWidget();
        #endregion

        #region Control Widget Samples (Redirects)
        public static BeepControlWidget CreateToggleSwitchWidget() => BeepControlWidgetSamples.CreateToggleSwitchWidget();
        public static BeepControlWidget CreateSliderWidget() => BeepControlWidgetSamples.CreateSliderWidget();
        public static BeepControlWidget CreateDropdownWidget() => BeepControlWidgetSamples.CreateDropdownWidget();
        public static BeepControlWidget CreateDatePickerControlWidget() => BeepControlWidgetSamples.CreateDatePickerWidget();
        public static BeepControlWidget CreateSearchBoxWidget() => BeepControlWidgetSamples.CreateSearchBoxWidget();
        #endregion

        #region Notification Widget Samples (Redirects)
        public static BeepNotificationWidget CreateToastNotificationWidget() => BeepNotificationWidgetSamples.CreateToastNotificationWidget();
        public static BeepNotificationWidget CreateAlertBannerWidget() => BeepNotificationWidgetSamples.CreateAlertBannerWidget();
        public static BeepNotificationWidget CreateProgressAlertWidget() => BeepNotificationWidgetSamples.CreateProgressAlertWidget();
        public static BeepNotificationWidget CreateStatusCardWidget() => BeepNotificationWidgetSamples.CreateStatusCardWidget();
        #endregion

        #region Navigation Widget Samples (Redirects)
        public static BeepNavigationWidget CreateBreadcrumbWidget() => BeepNavigationWidgetSamples.CreateBreadcrumbWidget();
        public static BeepNavigationWidget CreateStepIndicatorWidget() => BeepNavigationWidgetSamples.CreateStepIndicatorWidget();
        public static BeepNavigationWidget CreateTabContainerWidget() => BeepNavigationWidgetSamples.CreateTabContainerWidget();
        public static BeepNavigationWidget CreatePaginationWidget() => BeepNavigationWidgetSamples.CreatePaginationWidget();
        #endregion

        #region Finance Widget Samples (Redirects)
        public static BeepFinanceWidget CreatePortfolioCardWidget() => BeepFinanceWidgetSamples.CreatePortfolioCardWidget();
        public static BeepFinanceWidget CreateCryptoWidget() => BeepFinanceWidgetSamples.CreateCryptoWidget();
        public static BeepFinanceWidget CreateTransactionCardWidget() => BeepFinanceWidgetSamples.CreateTransactionCardWidget();
        public static BeepFinanceWidget CreateBalanceCardWidget() => BeepFinanceWidgetSamples.CreateBalanceCardWidget();
        public static BeepFinanceWidget CreateFinancialChartWidget() => BeepFinanceWidgetSamples.CreateFinancialChartWidget();
        public static BeepFinanceWidget CreatePaymentCardWidget() => BeepFinanceWidgetSamples.CreatePaymentCardWidget();
        public static BeepFinanceWidget CreateInvestmentCardWidget() => BeepFinanceWidgetSamples.CreateInvestmentCardWidget();
        public static BeepFinanceWidget CreateExpenseCardWidget() => BeepFinanceWidgetSamples.CreateExpenseCardWidget();
        public static BeepFinanceWidget CreateRevenueCardWidget() => BeepFinanceWidgetSamples.CreateRevenueCardWidget();
        public static BeepFinanceWidget CreateBudgetWidget() => BeepFinanceWidgetSamples.CreateBudgetWidget();
        #endregion

        #region Map Widget Samples (Redirects)
        public static BeepMapWidget CreateLiveTrackingWidget() => BeepMapWidgetSamples.CreateLiveTrackingWidget();
        public static BeepMapWidget CreateRouteDisplayWidget() => BeepMapWidgetSamples.CreateRouteDisplayWidget();
        public static BeepMapWidget CreateLocationCardWidget() => BeepMapWidgetSamples.CreateLocationCardWidget();
        public static BeepMapWidget CreateGeographicHeatmapWidget() => BeepMapWidgetSamples.CreateGeographicHeatmapWidget();
        public static BeepMapWidget CreateAddressCardWidget() => BeepMapWidgetSamples.CreateAddressCardWidget();
        public static BeepMapWidget CreateMapPreviewWidget() => BeepMapWidgetSamples.CreateMapPreviewWidget();
        public static BeepMapWidget CreateLocationListWidget() => BeepMapWidgetSamples.CreateLocationListWidget();
        public static BeepMapWidget CreateTravelCardWidget() => BeepMapWidgetSamples.CreateTravelCardWidget();
        public static BeepMapWidget CreateRegionMapWidget() => BeepMapWidgetSamples.CreateRegionMapWidget();
        public static BeepMapWidget CreatePlaceCardWidget() => BeepMapWidgetSamples.CreatePlaceCardWidget();
        #endregion

        #region Calendar Widget Samples (Redirects)
        public static BeepCalendarWidget CreateDateGridWidget() => BeepCalendarWidgetSamples.CreateDateGridWidget();
        public static BeepCalendarWidget CreateTimeSlotsWidget() => BeepCalendarWidgetSamples.CreateTimeSlotsWidget();
        public static BeepCalendarWidget CreateEventCardWidget() => BeepCalendarWidgetSamples.CreateEventCardWidget();
        public static BeepCalendarWidget CreateCalendarViewWidget() => BeepCalendarWidgetSamples.CreateCalendarViewWidget();
        public static BeepCalendarWidget CreateScheduleCardWidget() => BeepCalendarWidgetSamples.CreateScheduleCardWidget();
        public static BeepCalendarWidget CreateDatePickerWidget() => BeepCalendarWidgetSamples.CreateDatePickerWidget();
        public static BeepCalendarWidget CreateTimelineViewWidget() => BeepCalendarWidgetSamples.CreateTimelineViewWidget();
        public static BeepCalendarWidget CreateWeekViewWidget() => BeepCalendarWidgetSamples.CreateWeekViewWidget();
        public static BeepCalendarWidget CreateEventListWidget() => BeepCalendarWidgetSamples.CreateEventListWidget();
        public static BeepCalendarWidget CreateAvailabilityGridWidget() => BeepCalendarWidgetSamples.CreateAvailabilityGridWidget();
        #endregion

        #region Social Widget Samples (Redirects)
        public static BeepSocialWidget CreateProfileCardWidget() => BeepSocialWidgetSamples.CreateProfileCardWidget();
        public static BeepSocialWidget CreateTeamMembersWidget() => BeepSocialWidgetSamples.CreateTeamMembersWidget();
        public static BeepSocialWidget CreateMessageCardWidget() => BeepSocialWidgetSamples.CreateMessageCardWidget();
        public static BeepSocialWidget CreateActivityStreamWidget() => BeepSocialWidgetSamples.CreateActivityStreamWidget();
        public static BeepSocialWidget CreateUserListWidget() => BeepSocialWidgetSamples.CreateUserListWidget();
        public static BeepSocialWidget CreateChatWidget() => BeepSocialWidgetSamples.CreateChatWidget();
        public static BeepSocialWidget CreateCommentThreadWidget() => BeepSocialWidgetSamples.CreateCommentThreadWidget();
        public static BeepSocialWidget CreateSocialFeedWidget() => BeepSocialWidgetSamples.CreateSocialFeedWidget();
        public static BeepSocialWidget CreateUserStatsWidget() => BeepSocialWidgetSamples.CreateUserStatsWidget();
        public static BeepSocialWidget CreateContactCardWidget() => BeepSocialWidgetSamples.CreateContactCardWidget();
        #endregion

        #region Form Widget Samples (Redirects)
        public static BeepFormWidget CreateFieldGroupWidget() => BeepFormWidgetSamples.CreateFieldGroupWidget();
        public static BeepFormWidget CreateValidationPanelWidget() => BeepFormWidgetSamples.CreateValidationPanelWidget();
        public static BeepFormWidget CreateFormSectionWidget() => BeepFormWidgetSamples.CreateFormSectionWidget();
        public static BeepFormWidget CreateInputCardWidget() => BeepFormWidgetSamples.CreateInputCardWidget();
        public static BeepFormWidget CreateFormStepWidget() => BeepFormWidgetSamples.CreateFormStepWidget();
        public static BeepFormWidget CreateFieldSetWidget() => BeepFormWidgetSamples.CreateFieldSetWidget();
        public static BeepFormWidget CreateInlineFormWidget() => BeepFormWidgetSamples.CreateInlineFormWidget();
        public static BeepFormWidget CreateCompactFormWidget() => BeepFormWidgetSamples.CreateCompactFormWidget();
        public static BeepFormWidget CreateValidatedInputWidget() => BeepFormWidgetSamples.CreateValidatedInputWidget();
        public static BeepFormWidget CreateFormSummaryWidget() => BeepFormWidgetSamples.CreateFormSummaryWidget();
        #endregion

        #region Event Handling (Redirect)
        /// <summary>
        /// Demonstrates how to handle widget events using BaseControl's hit area system
        /// </summary>
        public static void SetupWidgetEvents(BaseControl widget) => BeepWidgetEventHandler.SetupWidgetEvents(widget);
        #endregion

        #region Demo Forms (Redirects)
        /// <summary>
        /// Creates a comprehensive form with ALL widget types
        /// </summary>
        public static Form CreateAllWidgetsDemo() => BeepWidgetSampleCoordinator.CreateAllWidgetsDemo();

        /// <summary>
        /// Creates a dashboard-style form showcasing complex layouts
        /// </summary>
        public static Form CreateDashboardDemo() => BeepWidgetSampleCoordinator.CreateDashboardDemo();
        #endregion
    }
}