using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Sample
{
    /// <summary>
    /// Sample implementations for BeepFinanceWidget with all finance styles
    /// </summary>
    public static class BeepFinanceWidgetSamples
    {
        /// <summary>
        /// Creates a portfolio card finance widget
        /// Uses PortfolioCardPainter.cs
        /// </summary>
        public static BeepFinanceWidget CreatePortfolioCardWidget()
        {
            return new BeepFinanceWidget
            {
                Style = FinanceWidgetStyle.PortfolioCard,
                Title = "Investment Portfolio",
                PrimaryValue = 231850.75m,
                Percentage = 12.5m,
                Currency = "USD",
                ShowCurrency = true,
                ShowPercentage = true,
                ShowTrend = true,
                Size = new Size(300, 200),
                AccentColor = Color.FromArgb(34, 139, 34)
            };
        }

        /// <summary>
        /// Creates a crypto widget finance widget
        /// Uses CryptoWidgetPainter.cs
        /// </summary>
        public static BeepFinanceWidget CreateCryptoWidget()
        {
            return new BeepFinanceWidget
            {
                Style = FinanceWidgetStyle.CryptoWidget,
                Title = "Bitcoin",
                PrimaryValue = 43250.80m,
                Percentage = 8.7m,
                Currency = "USD",
                ShowCurrency = true,
                ShowPercentage = true,
                ShowTrend = true,
                Size = new Size(280, 180),
                AccentColor = Color.FromArgb(255, 165, 0) // Bitcoin orange
            };
        }

        /// <summary>
        /// Creates a transaction card finance widget
        /// Uses TransactionCardPainter.cs
        /// </summary>
        public static BeepFinanceWidget CreateTransactionCardWidget()
        {
            return new BeepFinanceWidget
            {
                Style = FinanceWidgetStyle.TransactionCard,
                Title = "Recent Transaction",
                PrimaryValue = -87.50m,
                Currency = "USD",
                AccountNumber = "****1234",
                Size = new Size(320, 120),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates a balance card finance widget
        /// Uses BalanceCardPainter.cs
        /// </summary>
        public static BeepFinanceWidget CreateBalanceCardWidget()
        {
            return new BeepFinanceWidget
            {
                Style = FinanceWidgetStyle.BalanceCard,
                Title = "Checking Account",
                PrimaryValue = 12847.63m,
                Currency = "USD",
                AccountNumber = "****5678",
                CardType = "Checking",
                Size = new Size(300, 180),
                AccentColor = Color.FromArgb(70, 130, 180)
            };
        }

        /// <summary>
        /// Creates a financial chart finance widget
        /// Uses FinancialChartPainter.cs
        /// </summary>
        public static BeepFinanceWidget CreateFinancialChartWidget()
        {
            return new BeepFinanceWidget
            {
                Style = FinanceWidgetStyle.FinancialChart,
                Title = "Stock Performance",
                PrimaryValue = 156.75m,
                Percentage = 3.2m,
                Currency = "USD",
                ShowTrend = true,
                Size = new Size(350, 200),
                AccentColor = Color.FromArgb(76, 175, 80)
            };
        }

        /// <summary>
        /// Creates a payment card finance widget
        /// Uses PaymentCardPainter.cs
        /// </summary>
        public static BeepFinanceWidget CreatePaymentCardWidget()
        {
            return new BeepFinanceWidget
            {
                Style = FinanceWidgetStyle.PaymentCard,
                Title = "Credit Card",
                PrimaryValue = 2850.40m,
                SecondaryValue = 5000.00m, // Credit limit
                AccountNumber = "****9012",
                CardType = "Visa",
                Size = new Size(300, 190),
                AccentColor = Color.FromArgb(128, 0, 128)
            };
        }

        /// <summary>
        /// Creates an investment card finance widget
        /// Uses InvestmentCardPainter.cs
        /// </summary>
        public static BeepFinanceWidget CreateInvestmentCardWidget()
        {
            return new BeepFinanceWidget
            {
                Style = FinanceWidgetStyle.InvestmentCard,
                Title = "Growth Fund",
                PrimaryValue = 47830.25m,
                Percentage = 15.8m,
                Currency = "USD",
                ShowTrend = true,
                Size = new Size(280, 160),
                AccentColor = Color.FromArgb(255, 140, 0)
            };
        }

        /// <summary>
        /// Creates an expense card finance widget
        /// Uses ExpenseCardPainter.cs
        /// </summary>
        public static BeepFinanceWidget CreateExpenseCardWidget()
        {
            return new BeepFinanceWidget
            {
                Style = FinanceWidgetStyle.ExpenseCard,
                Title = "Monthly Expenses",
                PrimaryValue = 3247.89m,
                SecondaryValue = 3500.00m, // Budget
                Currency = "USD",
                Percentage = -7.2m, // Under budget
                Size = new Size(300, 150),
                AccentColor = Color.FromArgb(220, 20, 60)
            };
        }

        /// <summary>
        /// Creates a revenue card finance widget
        /// Uses RevenueCardPainter.cs
        /// </summary>
        public static BeepFinanceWidget CreateRevenueCardWidget()
        {
            return new BeepFinanceWidget
            {
                Style = FinanceWidgetStyle.RevenueCard,
                Title = "Monthly Revenue",
                PrimaryValue = 125760.00m,
                Percentage = 18.3m,
                Currency = "USD",
                ShowTrend = true,
                Size = new Size(300, 150),
                AccentColor = Color.FromArgb(34, 139, 34)
            };
        }

        /// <summary>
        /// Creates a budget widget finance widget
        /// Uses BudgetWidgetPainter.cs
        /// </summary>
        public static BeepFinanceWidget CreateBudgetWidget()
        {
            return new BeepFinanceWidget
            {
                Style = FinanceWidgetStyle.BudgetWidget,
                Title = "Budget Tracker",
                PrimaryValue = 6247.50m, // Spent
                SecondaryValue = 8000.00m, // Budget
                Percentage = 78.1m, // Percentage used
                Currency = "USD",
                Size = new Size(320, 180),
                AccentColor = Color.FromArgb(255, 193, 7)
            };
        }

        /// <summary>
        /// Gets all finance widget samples
        /// </summary>
        public static BeepFinanceWidget[] GetAllSamples()
        {
            return new BeepFinanceWidget[]
            {
                CreatePortfolioCardWidget(),
                CreateCryptoWidget(),
                CreateTransactionCardWidget(),
                CreateBalanceCardWidget(),
                CreateFinancialChartWidget(),
                CreatePaymentCardWidget(),
                CreateInvestmentCardWidget(),
                CreateExpenseCardWidget(),
                CreateRevenueCardWidget(),
                CreateBudgetWidget()
            };
        }
    }
}