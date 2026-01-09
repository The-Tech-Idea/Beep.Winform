using System;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// Transaction type enum
    /// </summary>
    public enum TransactionType
    {
        Income,
        Expense,
        Transfer,
        Investment
    }

    /// <summary>
    /// Financial transaction
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Transaction
    {
        [Category("Data")]
        [Description("Unique identifier for the transaction")]
        public string Id { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Transaction amount")]
        public decimal Amount { get; set; } = 0m;

        [Category("Data")]
        [Description("Transaction description")]
        public string Description { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Transaction date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Category("Data")]
        [Description("Transaction category")]
        public string Category { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Type of transaction")]
        public TransactionType Type { get; set; } = TransactionType.Expense;

        public override string ToString() => $"{Description}: {Amount:C}";
    }

    /// <summary>
    /// Portfolio item
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PortfolioItem
    {
        [Category("Data")]
        [Description("Stock symbol or identifier")]
        public string Symbol { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Number of shares")]
        public decimal Shares { get; set; } = 0m;

        [Category("Data")]
        [Description("Current price per share")]
        public decimal Price { get; set; } = 0m;

        [Category("Data")]
        [Description("Total value (shares * price)")]
        public decimal Value { get; set; } = 0m;

        [Category("Data")]
        [Description("Price change (positive or negative)")]
        public decimal Change { get; set; } = 0m;

        [Category("Data")]
        [Description("Percentage change")]
        public decimal ChangePercent { get; set; } = 0m;

        public override string ToString() => $"{Symbol}: {Value:C}";
    }
}
