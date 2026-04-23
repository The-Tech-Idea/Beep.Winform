using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Grouping
{
    /// <summary>
    /// Contract for grouping strategies in BeepGridPro.
    /// Implementations must be stateless; all mutable state lives in <see cref="GridGroupEngine"/>.
    /// </summary>
    public interface IGridGrouper
    {
        /// <summary>
        /// Compute the group key for a single row value.
        /// </summary>
        /// <param name="value">The raw cell value from the grouping column.</param>
        /// <param name="descriptor">The group descriptor (contains format hints).</param>
        /// <returns>A string key used to bucket rows into groups.</returns>
        string GetGroupKey(object? value, GroupDescriptor descriptor);

        /// <summary>
        /// Compute the human-readable label for a group key.
        /// </summary>
        /// <param name="key">The group key produced by <see cref="GetGroupKey"/>.</param>
        /// <param name="descriptor">The group descriptor.</param>
        /// <returns>A display label shown in the group header row.</returns>
        string GetGroupLabel(string key, GroupDescriptor descriptor);
    }

    /// <summary>
    /// Default grouper: uses string equality on formatted values.
    /// Supports format strings for dates and numbers.
    /// </summary>
    public sealed class DefaultGridGrouper : IGridGrouper
    {
        public static readonly DefaultGridGrouper Instance = new();

        private DefaultGridGrouper() { }

        public string GetGroupKey(object? value, GroupDescriptor descriptor)
        {
            if (value == null) return "(null)";

            if (!string.IsNullOrEmpty(descriptor.ValueFormat) && value is System.IFormattable fmt)
            {
                try
                {
                    return fmt.ToString(descriptor.ValueFormat, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch
                {
                    // Fallback to ToString if formatting fails
                }
            }

            return value.ToString() ?? "(empty)";
        }

        public string GetGroupLabel(string key, GroupDescriptor descriptor)
        {
            if (string.IsNullOrEmpty(descriptor.ValueFormat)) return key;

            // If a format was applied, the key is already formatted
            return key;
        }
    }
}
