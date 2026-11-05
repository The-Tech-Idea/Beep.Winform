using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Binding
{
    internal sealed class GridDataField
    {
        public string Name { get; init; } = string.Empty;
        public string Caption { get; init; } = string.Empty;
        public Type Type { get; init; } = typeof(object);
        public bool IsReadOnly { get; init; }
        public int Index { get; init; }
    }

    internal sealed class GridDataBinding
    {
        private readonly Dictionary<Type, PropertyDescriptorCollection> _descriptors = new();
        private readonly Dictionary<(Type Type, string Name), PropertyInfo?> _properties = new();
        private readonly Dictionary<(Type Type, string Name), FieldInfo?> _fields = new();
        private readonly BeepGridPro? _grid;

        private GridDataBinding(
            object? originalSource,
            object? normalizedSource,
            IEnumerable items,
            bool isSingleObject,
            DataTable? schemaTable,
            Type? itemType,
            IReadOnlyList<GridDataField> fields,
            BeepGridPro? grid = null)
        {
            _grid = grid;
            OriginalSource = originalSource;
            NormalizedSource = normalizedSource;
            Items = items;
            IsSingleObject = isSingleObject;
            SchemaTable = schemaTable;
            ItemType = itemType;
            Fields = fields;
        }

        public object? OriginalSource { get; }
        public object? NormalizedSource { get; }
        public IEnumerable Items { get; }
        public bool IsSingleObject { get; }
        public DataTable? SchemaTable { get; }
        public Type? ItemType { get; }
        public IReadOnlyList<GridDataField> Fields { get; }

        public static GridDataBinding Create(object? source, string? dataMember, BeepGridPro? grid = null)
        {
            if (source == null)
                return new GridDataBinding(null, null, Array.Empty<object>(), false, null, null, Array.Empty<GridDataField>(), grid);

            bool single = false;
            string? member = dataMember;
            object? current = source;

            if (current is BindingSource bindingSource)
            {
                current = bindingSource.List;
                if (current == null)
                {
                    current = bindingSource.DataSource;
                    member = string.IsNullOrWhiteSpace(bindingSource.DataMember)
                        ? member
                        : bindingSource.DataMember;
                }
            }

            if (current is DataSet dataSet)
            {
                DataTable? table = null;
                if (!string.IsNullOrWhiteSpace(member) && dataSet.Tables.Contains(member))
                    table = dataSet.Tables[member];
                else if (dataSet.Tables.Count > 0)
                    table = dataSet.Tables[0];
                current = table?.DefaultView;
            }
            else if (current is DataViewManager viewManager)
            {
                var viewManagerSet = viewManager.DataSet;
                DataTable? table = null;
                if (viewManagerSet != null && !string.IsNullOrWhiteSpace(member) &&
                    viewManagerSet.Tables.Contains(member))
                    table = viewManagerSet.Tables[member];
                else if (viewManagerSet != null && viewManagerSet.Tables.Count > 0)
                    table = viewManagerSet.Tables[0];
                current = table?.DefaultView;
            }
            else if (current is DataTable dataTable)
            {
                current = dataTable.DefaultView;
            }
            else if (current is System.Data.DataView)
            {
            }
            else if (current is IListSource listSource)
            {
                current = listSource.GetList();
            }
            else if (current is IDataReader reader)
            {
                var table = new DataTable();
                table.Load(reader);
                current = table.DefaultView;
            }
            else if (current is IList)
            {
            }
            else if (current is not string && current is IEnumerable enumerable)
            {
                current = enumerable.Cast<object?>().ToList();
            }
            else if (current != null && IsAsyncEnumerable(current.GetType()))
            {
                current = MaterializeAsyncEnumerable(current, grid);
            }
            else if (current != null)
            {
                single = true;
                current = new List<object?> { current };
            }

            if (current == null)
                return new GridDataBinding(source, null, Array.Empty<object>(), false, null, null, Array.Empty<GridDataField>(), grid);

            var schemaTable = (current as System.Data.DataView)?.Table;
            var items = current as IEnumerable ?? new[] { current };
            var itemType = ResolveItemType(current, items);
            var fields = ResolveFields(current, items, schemaTable, grid);

            return new GridDataBinding(source, current, items, single, schemaTable, itemType, fields, grid);
        }

        public object? GetValue(object? item, string fieldName)
        {
            if (item == null || string.IsNullOrWhiteSpace(fieldName))
                return null;

            if (item is DataRowView rowView)
            {
                var rowTable = rowView.DataView?.Table;
                if (rowTable == null || !rowTable.Columns.Contains(fieldName))
                    return null;
                return NullIfDBNull(rowView[fieldName]);
            }

            if (item is DataRow row)
            {
                if (row.Table == null || !row.Table.Columns.Contains(fieldName))
                    return null;
                return NullIfDBNull(row[fieldName]);
            }

            if (item is IDictionary<string, object?> genericDictionary)
                return genericDictionary.TryGetValue(fieldName, out var genericValue)
                    ? NullIfDBNull(genericValue)
                    : null;

            if (item is IDictionary dictionary)
                return dictionary.Contains(fieldName) ? NullIfDBNull(dictionary[fieldName]) : null;

            var descriptor = GetDescriptor(item.GetType(), fieldName);
            if (descriptor != null)
                return descriptor.GetValue(item);

            var property = GetProperty(item.GetType(), fieldName);
            if (property?.CanRead == true)
                return property.GetValue(item);

            var field = GetField(item.GetType(), fieldName);
            if (field != null)
                return field.GetValue(item);

            return IsScalar(item.GetType()) && string.Equals(fieldName, "Value", StringComparison.OrdinalIgnoreCase)
                ? item
                : null;
        }

        public bool TryCreateAndAdd(out object? item, out Exception? error)
        {
            item = null;
            error = null;

            if (NormalizedSource is not IList list || list.IsReadOnly || list.IsFixedSize)
            {
                error = new InvalidOperationException("The source list does not support insert.");
                return false;
            }

            try
            {
                item = CreateNewItem();
                if (item == null)
                {
                    error = new InvalidOperationException("The source item type cannot be created.");
                    return false;
                }

                list.Add(item);
                return true;
            }
            catch (Exception ex)
            {
                error = ex;
                item = null;
                return false;
            }
        }

        private object? CreateNewItem()
        {
            if (ItemType == null)
                return null;

            if (IsScalar(ItemType))
                return Activator.CreateInstance(ItemType);

            if (typeof(IDictionary).IsAssignableFrom(ItemType))
            {
                var dictionary = (IDictionary)Activator.CreateInstance(ItemType)!;
                foreach (var field in Fields)
                {
                    if (dictionary.Contains(field.Name)) continue;
                    dictionary[field.Name] = field.Type.IsValueType
                        ? Activator.CreateInstance(field.Type)
                        : null;
                }
                return dictionary;
            }

            return Activator.CreateInstance(ItemType);
        }

        public bool TrySetValue(object? item, string fieldName, object? value, out object? appliedValue, out Exception? error, int itemIndex = -1)
        {
            appliedValue = value;
            error = null;

            if (item == null || string.IsNullOrWhiteSpace(fieldName))
            {
                error = new ArgumentException("A row item and field name are required.", nameof(item));
                return false;
            }

            try
            {
                if (item is DataRowView rowView)
                {
                    var rowTable = rowView.DataView?.Table;
                    if (rowTable == null || !rowTable.Columns.Contains(fieldName))
                    {
                        error = new ArgumentException($"Column '{fieldName}' does not exist.", nameof(fieldName));
                        return false;
                    }

                    var column = rowTable.Columns[fieldName]!;
                    appliedValue = value is null or DBNull ? null : ConvertValue(value, column.DataType);
                    rowView[fieldName] = appliedValue ?? DBNull.Value;
                    return true;
                }

                if (item is DataRow row)
                {
                    if (row.Table == null || !row.Table.Columns.Contains(fieldName))
                    {
                        error = new ArgumentException($"Column '{fieldName}' does not exist.", nameof(fieldName));
                        return false;
                    }

                    var column = row.Table.Columns[fieldName]!;
                    appliedValue = value is null or DBNull ? null : ConvertValue(value, column.DataType);
                    row[fieldName] = appliedValue ?? DBNull.Value;
                    return true;
                }

                if (item is IDictionary<string, object?> genericDictionary)
                {
                    genericDictionary[fieldName] = value is DBNull ? null : value;
                    RaiseResetItem(item);
                    return true;
                }

                if (item is IDictionary dictionary)
                {
                    dictionary[fieldName] = value is DBNull ? null : value;
                    RaiseResetItem(item);
                    return true;
                }

                if (IsScalar(item.GetType()) &&
                    string.Equals(fieldName, "Value", StringComparison.OrdinalIgnoreCase))
                {
                    if (NormalizedSource is not IList scalarList)
                    {
                        error = new InvalidOperationException("A scalar item can only be written through an IList source.");
                        return false;
                    }

                    int sourceIndex = itemIndex;
                    if (sourceIndex < 0 || sourceIndex >= scalarList.Count ||
                        !Equals(scalarList[sourceIndex], item))
                        sourceIndex = scalarList.IndexOf(item);
                    if (sourceIndex < 0)
                    {
                        error = new InvalidOperationException("The scalar item could not be located in the source list.");
                        return false;
                    }

                    appliedValue = ConvertValue(value, item.GetType());
                    scalarList[sourceIndex] = appliedValue;
                    RaiseResetItemAt(sourceIndex);
                    return true;
                }

                var descriptor = GetDescriptor(item.GetType(), fieldName);
                if (descriptor != null)
                {
                    if (descriptor.IsReadOnly)
                    {
                        error = new InvalidOperationException($"Field '{fieldName}' is read-only.");
                        return false;
                    }

                    appliedValue = ConvertValue(value, descriptor.PropertyType);
                    descriptor.SetValue(item, appliedValue);
                    RaiseResetItem(item);
                    return true;
                }

                var property = GetProperty(item.GetType(), fieldName);
                if (property != null)
                {
                    if (!property.CanWrite)
                    {
                        error = new InvalidOperationException($"Field '{fieldName}' is not writable.");
                        return false;
                    }

                    appliedValue = ConvertValue(value, property.PropertyType);
                    property.SetValue(item, appliedValue);
                    RaiseResetItem(item);
                    return true;
                }

                var field = GetField(item.GetType(), fieldName);
                if (field == null || field.IsInitOnly || field.IsLiteral)
                {
                    error = new InvalidOperationException($"Field '{fieldName}' is not writable.");
                    return false;
                }

                appliedValue = ConvertValue(value, field.FieldType);
                field.SetValue(item, appliedValue);
                RaiseResetItem(item);
                return true;
            }
            catch (Exception ex)
            {
                appliedValue = null;
                error = ex;
                return false;
            }
        }

        private void RaiseResetItemAt(int index)
        {
            if (NormalizedSource is not IBindingList list || index < 0)
                return;

            try
            {
                var resetItem = list.GetType().GetMethod("ResetItem", BindingFlags.Public | BindingFlags.Instance);
                if (resetItem != null)
                {
                    resetItem.Invoke(list, new object[] { index });
                    return;
                }

                var resetBindings = list.GetType().GetMethod("ResetBindings", BindingFlags.Public | BindingFlags.Instance);
                resetBindings?.Invoke(list, null);
            }
            catch (Exception ex)
            {
                _grid?.ReportOperationError("Binding.ResetItem", ex);
            }
        }

        private void RaiseResetItem(object item)
        {
            if (NormalizedSource is not IBindingList list)
                return;
            if (item is INotifyPropertyChanged)
                return;

            int index = list is IList items ? items.IndexOf(item) : -1;
            if (index < 0) return;

            var resetItem = list.GetType().GetMethod("ResetItem", BindingFlags.Public | BindingFlags.Instance);
            if (resetItem == null)
            {
                var resetBindings = list.GetType().GetMethod("ResetBindings", BindingFlags.Public | BindingFlags.Instance);
                resetBindings?.Invoke(list, null);
                return;
            }

            try { resetItem.Invoke(list, new object[] { index }); }
            catch (Exception ex)
            {
                _grid?.ReportOperationError("Binding.ResetItem", ex);
            }
        }

        private static Type? ResolveItemType(object source, IEnumerable items)
        {
            if (source is System.Data.DataView)
                return typeof(DataRowView);

            var sourceType = source.GetType();
            var genericList = sourceType.GetInterfaces()
                .Concat(new[] { sourceType })
                .FirstOrDefault(type => type.IsGenericType &&
                                        type.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            var genericItem = genericList?.GetGenericArguments().FirstOrDefault();
            if (genericItem != null && genericItem != typeof(object))
                return genericItem;

            foreach (var item in items)
            {
                if (item != null)
                    return item.GetType();
            }

            return null;
        }

        private static IReadOnlyList<GridDataField> ResolveFields(
            object source,
            IEnumerable items,
            DataTable? schemaTable,
            BeepGridPro? grid)
        {
            if (schemaTable != null)
                return FromDataTable(schemaTable);

            if (source is ITypedList typedList)
            {
                try
                {
                    var descriptors = typedList.GetItemProperties(null);
                    if (descriptors.Count > 0)
                        return FromDescriptors(descriptors);
                }
                catch (Exception ex) when (ex is NotSupportedException or InvalidOperationException)
                {
                    grid?.ReportOperationError("Binding.Schema", ex);
                }
            }

            object? first = null;
            foreach (var item in items)
            {
                if (item != null)
                {
                    first = item;
                    break;
                }
            }

            if (first is DataRowView rowView && rowView.DataView?.Table != null)
                return FromDataTable(rowView.DataView.Table);

            if (first is DataRow row && row.Table != null)
                return FromDataTable(row.Table);

            if (first is ICustomTypeDescriptor customDescriptor)
            {
                var descriptors = customDescriptor.GetProperties();
                if (descriptors.Count > 0)
                    return FromDescriptors(descriptors);
            }

            if (first is IDictionary<string, object?> || first is IDictionary)
                return FromDictionaries(items);

            Type? itemType = first?.GetType();
            if (itemType == null)
            {
                var sourceType = source.GetType();
                itemType = sourceType.GetInterfaces()
                    .Concat(new[] { sourceType })
                    .FirstOrDefault(type => type.IsGenericType &&
                                            type.GetGenericTypeDefinition() == typeof(IEnumerable<>))?
                    .GetGenericArguments().FirstOrDefault();
            }

            if (itemType == null)
                return Array.Empty<GridDataField>();

            if (!IsScalar(itemType))
                return FromTypes(items, itemType);

            return new[]
            {
                new GridDataField
                {
                    Name = "Value",
                    Caption = "Value",
                    Type = itemType,
                    IsReadOnly = false,
                    Index = 0
                }
            };
        }

        private static IReadOnlyList<GridDataField> FromTypes(IEnumerable items, Type fallbackType)
        {
            var fields = new List<GridDataField>();
            var indexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var typeFields = new Dictionary<Type, IReadOnlyList<GridDataField>>();

            foreach (var item in items)
            {
                if (item == null) continue;
                var itemType = item.GetType();
                if (!typeFields.TryGetValue(itemType, out var itemFields))
                {
                    itemFields = FromType(itemType);
                    typeFields[itemType] = itemFields;
                }

                foreach (var field in itemFields)
                {
                    if (!indexes.TryGetValue(field.Name, out int index))
                    {
                        indexes[field.Name] = fields.Count;
                        fields.Add(field);
                        continue;
                    }

                    var existing = fields[index];
                    bool typeConflict = existing.Type != field.Type &&
                        !existing.Type.IsAssignableFrom(field.Type) &&
                        !field.Type.IsAssignableFrom(existing.Type);
                    bool writable = !existing.IsReadOnly || !field.IsReadOnly;
                    if (typeConflict || writable != !existing.IsReadOnly)
                    {
                        fields[index] = new GridDataField
                        {
                            Name = existing.Name,
                            Caption = existing.Caption,
                            Type = typeConflict ? typeof(object) : existing.Type,
                            IsReadOnly = !writable,
                            Index = existing.Index
                        };
                    }
                }
            }

            return fields.Count > 0 ? fields : FromType(fallbackType);
        }

        private static IReadOnlyList<GridDataField> FromDataTable(DataTable table)
        {
            var fields = new List<GridDataField>(table.Columns.Count);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                var column = table.Columns[i];
                fields.Add(new GridDataField
                {
                    Name = column.ColumnName,
                    Caption = string.IsNullOrWhiteSpace(column.Caption) ? column.ColumnName : column.Caption,
                    Type = column.DataType,
                    IsReadOnly = column.ReadOnly,
                    Index = i
                });
            }
            return fields;
        }

        private static IReadOnlyList<GridDataField> FromDescriptors(PropertyDescriptorCollection descriptors)
        {
            var fields = new List<GridDataField>(descriptors.Count);
            for (int i = 0; i < descriptors.Count; i++)
            {
                var descriptor = descriptors[i];
                fields.Add(new GridDataField
                {
                    Name = descriptor.Name,
                    Caption = string.IsNullOrWhiteSpace(descriptor.DisplayName)
                        ? descriptor.Name
                        : descriptor.DisplayName,
                    Type = descriptor.PropertyType,
                    IsReadOnly = descriptor.IsReadOnly,
                    Index = i
                });
            }
            return fields;
        }

        private static IReadOnlyList<GridDataField> FromDictionaries(IEnumerable items)
        {
            var keys = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in items)
            {
                if (item is IDictionary<string, object?> generic)
                {
                    foreach (var key in generic.Keys)
                    {
                        if (!string.IsNullOrWhiteSpace(key) && seen.Add(key))
                            keys.Add(key);
                    }
                }
                else if (item is IDictionary nonGeneric)
                {
                    foreach (var rawKey in nonGeneric.Keys)
                    {
                        string key = Convert.ToString(rawKey, CultureInfo.InvariantCulture) ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(key) && seen.Add(key))
                            keys.Add(key);
                    }
                }
            }

            return keys.Select((key, index) => new GridDataField
                {
                    Name = key,
                    Caption = key,
                    Type = typeof(object),
                    Index = index
                })
                .ToList();
        }

        private static IReadOnlyList<GridDataField> FromType(Type itemType)
        {
            var fields = new List<GridDataField>();
            foreach (var property in itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.GetIndexParameters().Length > 0 || property.GetMethod == null)
                    continue;

                fields.Add(new GridDataField
                {
                    Name = property.Name,
                    Caption = property.Name,
                    Type = property.PropertyType,
                    IsReadOnly = !property.CanWrite,
                    Index = fields.Count
                });
            }

            foreach (var field in itemType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.IsStatic || fields.Any(existing =>
                        string.Equals(existing.Name, field.Name, StringComparison.OrdinalIgnoreCase)))
                    continue;

                fields.Add(new GridDataField
                {
                    Name = field.Name,
                    Caption = field.Name,
                    Type = field.FieldType,
                    IsReadOnly = field.IsInitOnly || field.IsLiteral,
                    Index = fields.Count
                });
            }

            return fields;
        }

        private PropertyDescriptor? GetDescriptor(Type type, string name)
        {
            if (!_descriptors.TryGetValue(type, out var descriptors))
            {
                descriptors = TypeDescriptor.GetProperties(type);
                _descriptors[type] = descriptors;
            }
            return descriptors.Find(name, ignoreCase: true);
        }

        private FieldInfo? GetField(Type type, string name)
        {
            var key = (type, name);
            if (_fields.TryGetValue(key, out var field))
                return field;

            field = type.GetField(name,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            _fields[key] = field;
            return field;
        }

        private PropertyInfo? GetProperty(Type type, string name)
        {
            var key = (type, name);
            if (_properties.TryGetValue(key, out var property))
                return property;

            property = type.GetProperty(name,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            _properties[key] = property;
            return property;
        }

        private static object MaterializeAsyncEnumerable(object source, BeepGridPro? grid)
        {
            var asyncInterface = source.GetType()
                .GetInterfaces()
                .Concat(new[] { source.GetType() })
                .FirstOrDefault(type => type.IsGenericType &&
                                        type.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>));

            if (asyncInterface == null)
                return Array.Empty<object>();

            var itemType = asyncInterface.GetGenericArguments()[0];
            var method = typeof(GridDataBinding).GetMethod(nameof(MaterializeAsyncCore),
                BindingFlags.NonPublic | BindingFlags.Static);
            if (method == null)
            {
                grid?.ReportOperationError("Binding.AsyncSource",
                    new InvalidOperationException("Async binding materializer is unavailable."));
                return Array.Empty<object>();
            }

            try
            {
                var task = (Task<List<object?>>)method.MakeGenericMethod(itemType)
                    .Invoke(null, new[] { source })!;
                return task.GetAwaiter().GetResult();
            }
            catch (Exception ex) when (ex is TargetInvocationException or InvalidOperationException
                                       or NotSupportedException or ArgumentException)
            {
                grid?.ReportOperationError("Binding.AsyncSource", ex);
                return Array.Empty<object>();
            }
        }

        private static async Task<List<object?>> MaterializeAsyncCore<T>(IAsyncEnumerable<T> source)
        {
            var items = new List<object?>();
            await foreach (var item in source.ConfigureAwait(false))
                items.Add(item);
            return items;
        }

        private static bool IsScalar(Type type)
        {
            var underlying = Nullable.GetUnderlyingType(type) ?? type;
            return underlying.IsPrimitive || underlying.IsEnum ||
                   underlying == typeof(string) || underlying == typeof(decimal) ||
                   underlying == typeof(DateTime) || underlying == typeof(DateTimeOffset) ||
                   underlying == typeof(TimeSpan) || underlying == typeof(Guid);
        }

        private static bool IsAsyncEnumerable(Type type)
            => type.GetInterfaces().Concat(new[] { type }).Any(candidate =>
                candidate.IsGenericType &&
                candidate.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>));

        private static object? NullIfDBNull(object? value)
            => value is DBNull ? null : value;

        private static object? ConvertValue(object? value, Type targetType)
        {
            if (value is DBNull)
                value = null;

            var nullableUnderlying = Nullable.GetUnderlyingType(targetType);
            var underlying = nullableUnderlying ?? targetType;
            if (value == null)
                return nullableUnderlying != null || !targetType.IsValueType
                    ? null
                    : Activator.CreateInstance(underlying);

            if (underlying == typeof(object) || underlying.IsInstanceOfType(value))
                return value;

            if (underlying == typeof(string))
                return Convert.ToString(value, CultureInfo.InvariantCulture);

            if (underlying.IsEnum)
                return value is string text
                    ? Enum.Parse(underlying, text, ignoreCase: true)
                    : Enum.ToObject(underlying, value);

            if (underlying == typeof(Guid))
                return value is Guid guid ? guid : Guid.Parse(Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty);

            var converter = TypeDescriptor.GetConverter(underlying);
            if (converter.CanConvertFrom(value.GetType()))
                return converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);

            return Convert.ChangeType(value, underlying, CultureInfo.InvariantCulture);
        }
    }
}
