using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    /// <summary>
    /// TypeConverter for DataMember property that provides dropdown options
    /// for bindable properties from various data source types
    /// </summary>
    public class DataMemberConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false; // Allow manual entry as well
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var values = new List<string>();

            if (context?.Instance == null)
                return new StandardValuesCollection(values);

            // Get the DataSource property value from the control
            var dataSourceProperty = context.Instance.GetType().GetProperty("DataSource");
            if (dataSourceProperty == null)
                return new StandardValuesCollection(values);

            var dataSource = dataSourceProperty.GetValue(context.Instance);
            if (dataSource == null)
                return new StandardValuesCollection(values);

            // Inspect the data source and add appropriate DataMember options
            InspectDataSource(dataSource, values);

            return new StandardValuesCollection(values.OrderBy(v => v).ToList());
        }

        private void InspectDataSource(object dataSource, List<string> values)
        {
            Type dataSourceType = dataSource.GetType();

            // Handle DataSet - add table names
            if (dataSource is DataSet dataSet)
            {
                foreach (DataTable table in dataSet.Tables)
                {
                    values.Add(table.TableName);
                }
                return;
            }

            // Handle DataTable - no DataMember needed, but add empty for consistency
            if (dataSource is DataTable)
            {
                values.Add(string.Empty);
                return;
            }

            // Handle DataView - no DataMember needed
            if (dataSource is DataViewManager)
            {
                values.Add(string.Empty);
                return;
            }

            // Handle arrays
            if (dataSourceType.IsArray)
            {
                values.Add(string.Empty);
                return;
            }

            // Handle generic collections (List<T>, ObservableCollection<T>, etc.)
            if (dataSourceType.IsGenericType)
            {
                var genericTypeDef = dataSourceType.GetGenericTypeDefinition();
                if (genericTypeDef == typeof(List<>) ||
                    genericTypeDef == typeof(IList<>) ||
                    genericTypeDef == typeof(ICollection<>) ||
                    genericTypeDef == typeof(IEnumerable<>) ||
                    genericTypeDef == typeof(ObservableCollection<>) ||
                    genericTypeDef == typeof(BindingList<>))
                {
                    values.Add(string.Empty);
                    return;
                }
            }

            // Handle non-generic collections that implement IList
            if (typeof(IList).IsAssignableFrom(dataSourceType))
            {
                values.Add(string.Empty);
                return;
            }

            // Handle IEnumerable (but not string, which is also IEnumerable)
            if (typeof(IEnumerable).IsAssignableFrom(dataSourceType) && dataSourceType != typeof(string))
            {
                values.Add(string.Empty);
                return;
            }

            // Handle single objects - inspect their properties for collections
            InspectObjectProperties(dataSource, dataSourceType, values);
        }

        private void InspectObjectProperties(object dataSource, Type dataSourceType, List<string> values)
        {
            // Get all public instance properties
            var properties = dataSourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                try
                {
                    // Skip indexer properties
                    if (prop.GetIndexParameters().Length > 0)
                        continue;

                    // Skip properties that can't be read
                    if (!prop.CanRead)
                        continue;

                    // Get the property value
                    var propValue = prop.GetValue(dataSource);
                    if (propValue == null)
                        continue;

                    Type propType = prop.PropertyType;

                    // Check if property is a collection type
                    if (IsCollectionType(propType))
                    {
                        values.Add(prop.Name);
                    }
                }
                catch
                {
                    // Skip properties that throw exceptions when accessed
                    continue;
                }
            }

            // If no collection properties found, add empty string
            if (values.Count == 0)
            {
                values.Add(string.Empty);
            }
        }

        private bool IsCollectionType(Type type)
        {
            // Handle arrays
            if (type.IsArray)
                return true;

            // Handle DataSet, DataTable, DataView
            if (type == typeof(DataSet) || type == typeof(DataTable) || type == typeof(DataViewManager))
                return true;

            // Handle generic collections
            if (type.IsGenericType)
            {
                var genericTypeDef = type.GetGenericTypeDefinition();
                if (genericTypeDef == typeof(List<>) ||
                    genericTypeDef == typeof(IList<>) ||
                    genericTypeDef == typeof(ICollection<>) ||
                    genericTypeDef == typeof(IEnumerable<>) ||
                    genericTypeDef == typeof(ObservableCollection<>) ||
                    genericTypeDef == typeof(BindingList<>))
                {
                    return true;
                }
            }

            // Handle non-generic collections that implement IList or IEnumerable
            if (typeof(IList).IsAssignableFrom(type) || typeof(IEnumerable).IsAssignableFrom(type))
            {
                // Exclude strings since they're IEnumerable<char> but not typically used as data sources
                if (type != typeof(string))
                    return true;
            }

            return false;
        }
    }
}
