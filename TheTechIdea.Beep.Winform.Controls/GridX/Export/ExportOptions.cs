using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Export
{
    /// <summary>
    /// Options controlling how grid data is exported.
    /// </summary>
    public sealed class ExportOptions
    {
        /// <summary>
        /// Export only visible rows (honors filters). Default: true.
        /// </summary>
        public bool VisibleRowsOnly { get; set; } = true;

        /// <summary>
        /// Export only visible columns. Default: true.
        /// </summary>
        public bool VisibleColumnsOnly { get; set; } = true;

        /// <summary>
        /// Respect current column display order. Default: true.
        /// </summary>
        public bool RespectColumnDisplayOrder { get; set; } = true;

        /// <summary>
        /// Include the column header row. Default: true.
        /// </summary>
        public bool IncludeHeaders { get; set; } = true;

        /// <summary>
        /// Exclude system columns (checkbox, row number, row ID). Default: true.
        /// </summary>
        public bool ExcludeSystemColumns { get; set; } = true;

        /// <summary>
        /// Culture-aware formatting string for DateTime values. Default: "o" (ISO 8601).
        /// </summary>
        public string DateTimeFormat { get; set; } = "o";

        /// <summary>
        /// Culture-aware formatting string for numeric values. Default: null (invariant).
        /// </summary>
        public string? NumberFormat { get; set; } = null;

        /// <summary>
        /// Optional: only export these specific column names. If null, all eligible columns are exported.
        /// </summary>
        public List<string>? ColumnWhitelist { get; set; } = null;

        /// <summary>
        /// Optional: skip these column names.
        /// </summary>
        public List<string>? ColumnBlacklist { get; set; } = null;

        /// <summary>
        /// CSV-specific: delimiter character. Default: ','.
        /// </summary>
        public char CsvDelimiter { get; set; } = ',';

        /// <summary>
        /// CSV-specific: quote character. Default: '"'.
        /// </summary>
        public char CsvQuote { get; set; } = '"';

        /// <summary>
        /// HTML-specific: CSS class name for the table element. Default: "beep-grid-export".
        /// </summary>
        public string HtmlTableClass { get; set; } = "beep-grid-export";

        /// <summary>
        /// HTML-specific: inline CSS styles to inject in a &lt;style&gt; block. Default: null.
        /// </summary>
        public string? HtmlInlineStyles { get; set; } = null;

        /// <summary>
        /// JSON-specific: pretty-print with indentation. Default: true.
        /// </summary>
        public bool JsonIndented { get; set; } = true;

        /// <summary>
        /// Clone the options (useful when a caller wants to tweak a preset without mutating it).
        /// </summary>
        public ExportOptions Clone()
        {
            return new ExportOptions
            {
                VisibleRowsOnly = this.VisibleRowsOnly,
                VisibleColumnsOnly = this.VisibleColumnsOnly,
                RespectColumnDisplayOrder = this.RespectColumnDisplayOrder,
                IncludeHeaders = this.IncludeHeaders,
                ExcludeSystemColumns = this.ExcludeSystemColumns,
                DateTimeFormat = this.DateTimeFormat,
                NumberFormat = this.NumberFormat,
                ColumnWhitelist = this.ColumnWhitelist != null ? new List<string>(this.ColumnWhitelist) : null,
                ColumnBlacklist = this.ColumnBlacklist != null ? new List<string>(this.ColumnBlacklist) : null,
                CsvDelimiter = this.CsvDelimiter,
                CsvQuote = this.CsvQuote,
                HtmlTableClass = this.HtmlTableClass,
                HtmlInlineStyles = this.HtmlInlineStyles,
                JsonIndented = this.JsonIndented
            };
        }
    }
}
