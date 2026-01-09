using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Charts;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers
{
    /// <summary>
    /// Helper for generating sample data for controls at design-time
    /// Improves design-time experience by populating controls with realistic sample data
    /// </summary>
    public static class DesignTimeDataHelper
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Populate a ListBox with sample items
        /// </summary>
        /// <param name="listBox">The ListBox to populate</param>
        /// <param name="count">Number of items to generate (default: 10)</param>
        public static void PopulateListBox(BeepListBox listBox, int count = 10)
        {
            if (listBox == null)
                throw new ArgumentNullException(nameof(listBox));

            listBox.Items.Clear();

            for (int i = 1; i <= count; i++)
            {
                listBox.Items.Add($"Item {i}");
            }
        }

        /// <summary>
        /// Populate a ListBox with sample items from a category
        /// </summary>
        /// <param name="listBox">The ListBox to populate</param>
        /// <param name="category">Category of items (e.g., "Names", "Cities", "Products")</param>
        /// <param name="count">Number of items to generate</param>
        public static void PopulateListBox(BeepListBox listBox, string category, int count = 10)
        {
            if (listBox == null)
                throw new ArgumentNullException(nameof(listBox));

            listBox.Items.Clear();

            var items = GetSampleItems(category, count);
            foreach (var item in items)
            {
                listBox.Items.Add(item);
            }
        }

        /// <summary>
        /// Populate a ComboBox with sample items
        /// </summary>
        /// <param name="comboBox">The ComboBox to populate</param>
        /// <param name="count">Number of items to generate (default: 10)</param>
        public static void PopulateComboBox(BeepComboBox comboBox, int count = 10)
        {
            if (comboBox == null)
                throw new ArgumentNullException(nameof(comboBox));

            comboBox.Items.Clear();

            for (int i = 1; i <= count; i++)
            {
                comboBox.Items.Add($"Option {i}");
            }
        }

        /// <summary>
        /// Populate a ComboBox with sample items from a category
        /// </summary>
        /// <param name="comboBox">The ComboBox to populate</param>
        /// <param name="category">Category of items</param>
        /// <param name="count">Number of items to generate</param>
        public static void PopulateComboBox(BeepComboBox comboBox, string category, int count = 10)
        {
            if (comboBox == null)
                throw new ArgumentNullException(nameof(comboBox));

            comboBox.Items.Clear();

            var items = GetSampleItems(category, count);
            foreach (var item in items)
            {
                comboBox.Items.Add(item);
            }
        }

        /// <summary>
        /// Generate sample chart data for a BeepChart
        /// </summary>
        /// <param name="chart">The chart to populate</param>
        /// <param name="seriesCount">Number of series to generate (default: 3)</param>
        /// <param name="pointsPerSeries">Number of data points per series (default: 10)</param>
        public static void GenerateChartData(BeepChart chart, int seriesCount = 3, int pointsPerSeries = 10)
        {
            if (chart == null)
                throw new ArgumentNullException(nameof(chart));

            // Note: This is a placeholder - actual implementation depends on BeepChart's API
            // The chart would need methods to add series and data points
            // This would typically be done through the chart's data source or series collection
        }

        /// <summary>
        /// Generate sample dates for date picker controls
        /// </summary>
        /// <param name="count">Number of dates to generate</param>
        /// <returns>List of sample dates</returns>
        public static List<DateTime> GenerateSampleDates(int count = 10)
        {
            var dates = new List<DateTime>();
            var startDate = DateTime.Today.AddDays(-count);

            for (int i = 0; i < count; i++)
            {
                dates.Add(startDate.AddDays(i));
            }

            return dates;
        }

        /// <summary>
        /// Generate sample times for time picker controls
        /// </summary>
        /// <param name="count">Number of times to generate</param>
        /// <param name="intervalMinutes">Interval between times in minutes (default: 30)</param>
        /// <returns>List of sample times</returns>
        public static List<TimeSpan> GenerateSampleTimes(int count = 10, int intervalMinutes = 30)
        {
            var times = new List<TimeSpan>();
            var startTime = new TimeSpan(9, 0, 0); // 9:00 AM

            for (int i = 0; i < count; i++)
            {
                times.Add(startTime.Add(TimeSpan.FromMinutes(i * intervalMinutes)));
            }

            return times;
        }

        /// <summary>
        /// Get sample items for a specific category
        /// </summary>
        /// <param name="category">Category name</param>
        /// <param name="count">Number of items to return</param>
        /// <returns>List of sample items</returns>
        public static List<string> GetSampleItems(string category, int count = 10)
        {
            var allItems = GetCategoryItems(category);
            var items = new List<string>();

            for (int i = 0; i < Math.Min(count, allItems.Count); i++)
            {
                items.Add(allItems[i]);
            }

            // If we need more items than available, generate numbered variants
            if (count > allItems.Count)
            {
                for (int i = allItems.Count; i < count; i++)
                {
                    items.Add($"{allItems[i % allItems.Count]} {i / allItems.Count + 1}");
                }
            }

            return items;
        }

        /// <summary>
        /// Get all items for a category
        /// </summary>
        private static List<string> GetCategoryItems(string category)
        {
            return category.ToLowerInvariant() switch
            {
                "names" => new List<string> { "John Doe", "Jane Smith", "Bob Johnson", "Alice Williams", "Charlie Brown", "Diana Prince", "Edward Norton", "Fiona Apple", "George Lucas", "Helen Keller" },
                "cities" => new List<string> { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose" },
                "products" => new List<string> { "Laptop", "Smartphone", "Tablet", "Headphones", "Keyboard", "Mouse", "Monitor", "Webcam", "Speaker", "Printer" },
                "countries" => new List<string> { "United States", "Canada", "United Kingdom", "Germany", "France", "Italy", "Spain", "Japan", "China", "Australia" },
                "colors" => new List<string> { "Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Pink", "Brown", "Black", "White" },
                "categories" => new List<string> { "Electronics", "Clothing", "Food", "Books", "Toys", "Sports", "Home", "Garden", "Automotive", "Health" },
                _ => new List<string> { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5", "Item 6", "Item 7", "Item 8", "Item 9", "Item 10" }
            };
        }

        /// <summary>
        /// Generate random numeric values for numeric controls
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <param name="count">Number of values to generate</param>
        /// <returns>List of random numeric values</returns>
        public static List<double> GenerateRandomNumbers(double min, double max, int count = 10)
        {
            var numbers = new List<double>();
            for (int i = 0; i < count; i++)
            {
                numbers.Add(_random.NextDouble() * (max - min) + min);
            }
            return numbers;
        }

        /// <summary>
        /// Generate sample email addresses
        /// </summary>
        /// <param name="count">Number of emails to generate</param>
        /// <returns>List of sample email addresses</returns>
        public static List<string> GenerateSampleEmails(int count = 10)
        {
            var emails = new List<string>();
            var domains = new[] { "example.com", "test.com", "demo.org", "sample.net" };

            for (int i = 1; i <= count; i++)
            {
                var domain = domains[i % domains.Length];
                emails.Add($"user{i}@{domain}");
            }

            return emails;
        }

        /// <summary>
        /// Generate sample phone numbers
        /// </summary>
        /// <param name="count">Number of phone numbers to generate</param>
        /// <returns>List of sample phone numbers</returns>
        public static List<string> GenerateSamplePhoneNumbers(int count = 10)
        {
            var phones = new List<string>();

            for (int i = 1; i <= count; i++)
            {
                var areaCode = _random.Next(200, 999);
                var exchange = _random.Next(200, 999);
                var number = _random.Next(1000, 9999);
                phones.Add($"({areaCode}) {exchange}-{number}");
            }

            return phones;
        }
    }
}
