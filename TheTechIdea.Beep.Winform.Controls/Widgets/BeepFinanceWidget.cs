using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Finance;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls.Widgets
{
    public enum FinanceWidgetStyle
    {
        PortfolioCard,    // Investment portfolio display
        CryptoWidget,     // Cryptocurrency progress/stats
        TransactionCard,  // Financial transaction display
        BalanceCard,      // Account balance showcase
        FinancialChart,   // Specialized financial charts
        PaymentCard,      // Payment method display
        InvestmentCard,   // Investment tracking card
        ExpenseCard,      // Expense category display
        RevenueCard,      // Revenue tracking display
        BudgetWidget      // Budget progress tracking
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Finance Widget")]
    [Category("Beep Widgets")]
    [Description("Financial widget for portfolios, transactions, balances, investments, and financial tracking.")]
    public class BeepFinanceWidget : BaseControl
    {
        #region Fields
        private FinanceWidgetStyle _style = FinanceWidgetStyle.PortfolioCard;
        private IWidgetPainter _painter;
        private string _title = "Finance Widget";
        private string _subtitle = "Financial Data";
        private decimal _primaryValue = 0m;
        private decimal _secondaryValue = 0m;
        private decimal _percentage = 0m;
        private string _currency = "USD";
        private string _currencySymbol = "$";
        private Color _accentColor = Color.FromArgb(34, 139, 34); // Forest Green
        private Color _positiveColor = Color.FromArgb(34, 139, 34); // Green
        private Color _negativeColor = Color.FromArgb(220, 20, 60); // Crimson
        private Color _neutralColor = Color.FromArgb(128, 128, 128); // Gray
        private Color _cardBackColor = Color.White;
        private Color _surfaceColor = Color.FromArgb(250, 250, 250);
        private Color _panelBackColor = Color.FromArgb(250, 250, 250);
        private Color _titleForeColor = Color.Black;
        private Color _valueForeColor = Color.FromArgb(100, 100, 100);
        private Color _labelForeColor = Color.FromArgb(150, 150, 150);
        private Color _chartBackColor = Color.White;
        private Color _chartLineColor = Color.FromArgb(33, 150, 243);
        private Color _borderColor = Color.FromArgb(200, 200, 200);
        private Color _highlightBackColor = Color.FromArgb(240, 240, 240);
        private List<FinanceItem> _financeItems = new List<FinanceItem>();
        private bool _showCurrency = true;
        private bool _showPercentage = true;
        private bool _showTrend = true;
        private FinanceTrend _trend = FinanceTrend.Neutral;
        private string _accountNumber = "";
        private string _cardType = "";
        private DateTime _lastUpdated = DateTime.Now;

        // Events
        public event EventHandler<BeepEventDataArgs> ValueClicked;
        public event EventHandler<BeepEventDataArgs> TransactionClicked;
        public event EventHandler<BeepEventDataArgs> AccountClicked;
        public event EventHandler<BeepEventDataArgs> InvestmentClicked;
        public event EventHandler<BeepEventDataArgs> ActionClicked;
        #endregion

        #region Constructor
        public BeepFinanceWidget() : base()
        {
            IsChild = false;
            Padding = new Padding(5);
            this.Size = new Size(300, 200);
            ApplyThemeToChilds = false;
            InitializeSampleData();
            ApplyTheme();
            CanBeHovered = true;
            InitializePainter();
        }

        private void InitializeSampleData()
        {
            _financeItems.AddRange(new[]
            {
                new FinanceItem { Name = "Stocks", Value = 25000m, Percentage = 15.2m, Trend = FinanceTrend.Up, Category = "Investment" },
                new FinanceItem { Name = "Bonds", Value = 15000m, Percentage = -2.1m, Trend = FinanceTrend.Down, Category = "Investment" },
                new FinanceItem { Name = "Cash", Value = 8500m, Percentage = 0.5m, Trend = FinanceTrend.Up, Category = "Savings" },
                new FinanceItem { Name = "Crypto", Value = 3200m, Percentage = 25.8m, Trend = FinanceTrend.Up, Category = "Crypto"},
                new FinanceItem { Name = "Real Estate", Value = 180000m, Percentage = 8.3m, Trend = FinanceTrend.Up, Category = "Property" }
            });

            _primaryValue = _financeItems.Sum(x => x.Value);
            _percentage = _financeItems.Average(x => x.Percentage);
            _trend = _percentage > 0 ? FinanceTrend.Up : _percentage < 0 ? FinanceTrend.Down : FinanceTrend.Neutral;
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                case FinanceWidgetStyle.PortfolioCard:
                    _painter = new PortfolioCardPainter();
                    break;
                case FinanceWidgetStyle.CryptoWidget:
                    _painter = new CryptoWidgetPainter();
                    break;
                case FinanceWidgetStyle.TransactionCard:
                    _painter = new TransactionCardPainter();
                    break;
                case FinanceWidgetStyle.BalanceCard:
                    _painter = new BalanceCardPainter();
                    break;
                case FinanceWidgetStyle.FinancialChart:
                    _painter = new FinancialChartPainter();
                    break;
                case FinanceWidgetStyle.PaymentCard:
                    _painter = new PaymentCardPainter();
                    break;
                case FinanceWidgetStyle.InvestmentCard:
                    _painter = new InvestmentCardPainter();
                    break;
                case FinanceWidgetStyle.ExpenseCard:
                    _painter = new ExpenseCardPainter();
                    break;
                case FinanceWidgetStyle.RevenueCard:
                    _painter = new RevenueCardPainter();
                    break;
                case FinanceWidgetStyle.BudgetWidget:
                    _painter = new BudgetWidgetPainter();
                    break;
                default:
                    _painter = new PortfolioCardPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
        }
        #endregion

        #region Properties
        [Category("Finance")]
        [Description("Visual Style of the finance widget.")]
        public FinanceWidgetStyle Style
        {
            get => _style;
            set { _style = value; InitializePainter(); Invalidate(); }
        }

        [Category("Finance")]
        [Description("Title text for the finance widget.")]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        [Category("Finance")]
        [Description("Subtitle text for the finance widget.")]
        public string Subtitle
        {
            get => _subtitle;
            set { _subtitle = value; Invalidate(); }
        }

        [Category("Finance")]
        [Description("Primary financial value.")]
        public decimal PrimaryValue
        {
            get => _primaryValue;
            set { _primaryValue = value; Invalidate(); }
        }

        [Category("Finance")]
        [Description("Secondary financial value.")]
        public decimal SecondaryValue
        {
            get => _secondaryValue;
            set { _secondaryValue = value; Invalidate(); }
        }

        [Category("Finance")]
        [Description("Percentage change or ratio.")]
        public decimal Percentage
        {
            get => _percentage;
            set { _percentage = value; UpdateTrendFromPercentage(); Invalidate(); }
        }

        [Category("Finance")]
        [Description("Currency code (USD, EUR, BTC, etc.).")]
        public string Currency
        {
            get => _currency;
            set { _currency = value; UpdateCurrencySymbol(); Invalidate(); }
        }

        [Category("Finance")]
        [Description("Currency symbol ($, �, ?, etc.).")]
        public string CurrencySymbol
        {
            get => _currencySymbol;
            set { _currencySymbol = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Primary accent color for the finance widget.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color for positive financial values.")]
        public Color PositiveColor
        {
            get => _positiveColor;
            set { _positiveColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color for negative financial values.")]
        public Color NegativeColor
        {
            get => _negativeColor;
            set { _negativeColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color for neutral financial values.")]
        public Color NeutralColor
        {
            get => _neutralColor;
            set { _neutralColor = value; Invalidate(); }
        }

        [Category("Finance")]
        [Description("Whether to show currency symbols.")]
        public bool ShowCurrency
        {
            get => _showCurrency;
            set { _showCurrency = value; Invalidate(); }
        }

        [Category("Finance")]
        [Description("Whether to show percentage values.")]
        public bool ShowPercentage
        {
            get => _showPercentage;
            set { _showPercentage = value; Invalidate(); }
        }

        [Category("Finance")]
        [Description("Whether to show trend indicators.")]
        public bool ShowTrend
        {
            get => _showTrend;
            set { _showTrend = value; Invalidate(); }
        }

        [Category("Finance")]
        [Description("Financial trend direction.")]
        public FinanceTrend Trend
        {
            get => _trend;
            set { _trend = value; Invalidate(); }
        }

        [Category("Finance")]
        [Description("Account number for financial cards.")]
        public string AccountNumber
        {
            get => _accountNumber;
            set { _accountNumber = value; Invalidate(); }
        }

        [Category("Finance")]
        [Description("Card type (Credit, Debit, etc.).")]
        public string CardType
        {
            get => _cardType;
            set { _cardType = value; Invalidate(); }
        }

        [Category("Finance")]
        [Description("Last updated timestamp.")]
        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set { _lastUpdated = value; Invalidate(); }
        }

        [Category("Finance")]
        [Description("Collection of financial items for detailed displays.")]
        public List<FinanceItem> FinanceItems
        {
            get => _financeItems;
            set { _financeItems = value ?? new List<FinanceItem>(); UpdateTotals(); Invalidate(); }
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            var ctx = new WidgetContext
            {
                DrawingRect = DrawingRect,
                Title = _title,
                Value = _subtitle,
                AccentColor = _accentColor,
                ShowIcon = true,
                IsInteractive = true,
                CornerRadius = BorderRadius,
                
                // Finance-specific typed properties
                // Convert FinanceItem to appropriate typed collections based on widget style
                PortfolioItems = _style == FinanceWidgetStyle.PortfolioCard 
                    ? _financeItems.Select(f => new PortfolioItem 
                    { 
                        Symbol = f.Name, 
                        Value = f.Value, 
                        Change = f.Percentage, 
                        ChangePercent = f.Percentage 
                    }).ToList() 
                    : new List<PortfolioItem>(),
                Transactions = _style == FinanceWidgetStyle.TransactionCard || _style == FinanceWidgetStyle.FinancialChart || _style == FinanceWidgetStyle.ExpenseCard || _style == FinanceWidgetStyle.RevenueCard
                    ? _financeItems.Select(f => new Transaction 
                    { 
                        Description = f.Name, 
                        Amount = f.Value, 
                        Category = f.Category ?? "",
                        Date = DateTime.Now 
                    }).ToList() 
                    : new List<Transaction>(),
                PrimaryValue = _primaryValue,
                SecondaryValue = _secondaryValue,
                Percentage = _percentage,
                Currency = _currency,
                CurrencySymbol = _currencySymbol,
                ShowCurrency = _showCurrency,
                Trend = _trend.ToString()
            };

            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;

            _painter?.DrawBackground(g, ctx);
            _painter?.DrawContent(g, ctx);
            _painter?.DrawForegroundAccents(g, ctx);

            RefreshHitAreas(ctx);
            _painter?.UpdateHitAreas(this, ctx, (name, rect) => { });
        }

        private void RefreshHitAreas(WidgetContext ctx)
        {
            ClearHitList();

            if (!ctx.ContentRect.IsEmpty)
            {
                AddHitArea("Value", ctx.ContentRect, null, () =>
                {
                    ValueClicked?.Invoke(this, new BeepEventDataArgs("ValueClicked", this));
                });
            }

            if (!ctx.HeaderRect.IsEmpty)
            {
                AddHitArea("Account", ctx.HeaderRect, null, () =>
                {
                    AccountClicked?.Invoke(this, new BeepEventDataArgs("AccountClicked", this));
                });
            }

            if (!ctx.FooterRect.IsEmpty)
            {
                AddHitArea("Transaction", ctx.FooterRect, null, () =>
                {
                    TransactionClicked?.Invoke(this, new BeepEventDataArgs("TransactionClicked", this));
                });
            }

            // Add hit areas for individual finance items
            if (_style == FinanceWidgetStyle.PortfolioCard || _style == FinanceWidgetStyle.InvestmentCard)
            {
                for (int i = 0; i < _financeItems.Count && i < 5; i++) // Limit to 5 visible items
                {
                    int itemIndex = i; // Capture for closure
                    AddHitArea($"Item{i}", new Rectangle(), null, () =>
                    {
                        InvestmentClicked?.Invoke(this, new BeepEventDataArgs("InvestmentClicked", this) { EventData = _financeItems[itemIndex] });
                    });
                }
            }
        }

        private void UpdateCurrencySymbol()
        {
            _currencySymbol = _currency?.ToUpper() switch
            {
                "USD" => "$",
                "EUR" => "�",
                "GBP" => "�",
                "JPY" => "�",
                "BTC" => "?",
                "ETH" => "?",
                "CNY" => "�",
                "CAD" => "C$",
                "AUD" => "A$",
                _ => "$"
            };
        }

        private void UpdateTrendFromPercentage()
        {
            _trend = _percentage > 0 ? FinanceTrend.Up : _percentage < 0 ? FinanceTrend.Down : FinanceTrend.Neutral;
        }

        private void UpdateTotals()
        {
            if (_financeItems != null && _financeItems.Any())
            {
                _primaryValue = _financeItems.Sum(x => x.Value);
                _percentage = _financeItems.Average(x => x.Percentage);
                UpdateTrendFromPercentage();
            }
        }
        #endregion

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            // Apply finance-specific theme colors
            BackColor = _currentTheme.BackColor;
            ForeColor = _currentTheme.ForeColor;
            
            // Update financial status colors
            _positiveColor = _currentTheme.SuccessColor;      // Profits, gains
            _negativeColor = _currentTheme.ErrorColor;        // Losses, expenses
            _neutralColor = _currentTheme.WarningColor;       // Neutral values
            _accentColor = _currentTheme.AccentColor;         // Highlights
            
            // Update card and surface colors
            _cardBackColor = _currentTheme.CardBackColor;
            _surfaceColor = _currentTheme.SurfaceColor;
            _panelBackColor = _currentTheme.PanelBackColor;
            
            // Update text colors
            _titleForeColor = _currentTheme.CardTitleForeColor;
            _valueForeColor = _currentTheme.CardTextForeColor;
            _labelForeColor = _currentTheme.CardSubTitleForeColor;
            
            // Update chart colors if applicable
            _chartBackColor = _currentTheme.ChartBackColor;
            _chartLineColor = _currentTheme.ChartLineColor;
            
            // Update border and interactive colors
            _borderColor = _currentTheme.BorderColor;
            _highlightBackColor = _currentTheme.HighlightBackColor;
            
            InitializePainter();
            Invalidate();
        }
    }

    /// <summary>
    /// Financial trend direction enumeration
    /// </summary>
    public enum FinanceTrend
    {
        Up,       // Positive trend (green)
        Down,     // Negative trend (red)
        Neutral   // No significant change (gray)
    }

    /// <summary>
    /// Financial item data structure for detailed financial displays
    /// </summary>
    public class FinanceItem
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Value { get; set; } = 0m;
        public decimal Percentage { get; set; } = 0m;
        public FinanceTrend Trend { get; set; } = FinanceTrend.Neutral;
        public string Currency { get; set; } = "USD";
        public DateTime Date { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public Color ItemColor { get; set; } = Color.Blue;
        public object Tag { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}