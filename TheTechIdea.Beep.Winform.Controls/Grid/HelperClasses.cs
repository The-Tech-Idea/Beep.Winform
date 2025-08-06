using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    #region Helper Classes

    public class GridHitTestInfo
    {
        public HitTestType Type { get; }
        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public string Name { get; set; }
        public GridHitTestInfo(HitTestType type, int rowIndex, int columnIndex)
        {
            Type = type;
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
        }
    }

    public enum HitTestType
    {
        None,
        Cell,
        ColumnHeader,
        RowHeader,
        NavigationButton,
        SelectionCheckbox
    }

    public class HitTestArea
    {
        public Rectangle Bounds { get; set; }
        public string Name { get; set; }
        public Action Action { get; set; }
    }

    #endregion
    #region Event Argument Classes

    public class DataUpdateEventArgs : EventArgs
    {
        public object DataItem { get; }
        public string PropertyName { get; }
        public object NewValue { get; }
        public bool Cancel { get; set; }

        public DataUpdateEventArgs(object dataItem, string propertyName, object newValue)
        {
            DataItem = dataItem;
            PropertyName = propertyName;
            NewValue = newValue;
        }
    }

    public class DataUpdateFailedEventArgs : EventArgs
    {
        public object DataItem { get; }
        public string PropertyName { get; }
        public object Value { get; }
        public Exception Exception { get; }

        public DataUpdateFailedEventArgs(object dataItem, string propertyName, object value, Exception exception)
        {
            DataItem = dataItem;
            PropertyName = propertyName;
            Value = value;
            Exception = exception;
        }
    }

    public class ValidationFailedEventArgs : EventArgs
    {
        public string Message { get; }
        public string PropertyName { get; }
        public object Value { get; }

        public ValidationFailedEventArgs(string message, string propertyName, object value)
        {
            Message = message;
            PropertyName = propertyName;
            Value = value;
        }
    }

    #endregion
    // Add helper class for color operations
    public static class ColorHelper
    {
        public static Color Lighten(Color color, float amount)
        {
            return Color.FromArgb(
                color.A,
                Math.Min(255, (int)(color.R + (255 - color.R) * amount)),
                Math.Min(255, (int)(color.G + (255 - color.G) * amount)),
                Math.Min(255, (int)(color.B + (255 - color.B) * amount))
            );
        }

        public static Color Darken(Color color, float amount)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, (int)(color.R * (1 - amount))),
                Math.Max(0, (int)(color.G * (1 - amount))),
                Math.Max(0, (int)(color.B * (1 - amount)))
            );
        }
    }
}
