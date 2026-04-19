using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Vis.Modules
{
    public static class ChartHelper
    {
         
        public static double GetXAsDouble(object X)
    {
        return X is double ? (double)X : 0;
    }

    // Method to retrieve Y value as double (for numeric calculations)
    public static double GetYAsDouble(object Y)
    {
        return Y is double ? (double)Y : 0;
    }

    // Method to retrieve X value as DateTime (for date-based calculations)
    public static DateTime? GetXAsDateTime(object X)
    {
        return X is DateTime date ? date : (DateTime?)null;
    }

    // Method to retrieve Y value as DateTime (for date-based calculations)
    public static DateTime? GetYAsDateTime(object Y)
    {
        return Y is DateTime date ? date : (DateTime?)null;
    }
    // Helper methods to get numeric values
    public static float? GetXAsFloat(object X)
    {
        return X is float x ? x : X is double d ? (float)d : X is int i ? (float)i : (float?)null;
    }

    public static float? GetYAsFloat(object Y)
    {
        return Y is float y ? y : Y is double d ? (float)d : Y is int i ? (float)i : (float?)null;
    }
    // Method to retrieve X and Y values as strings for text-based axes
    public static string GetXAsString(object X) => X?.ToString() ?? string.Empty;
    public static string GetYAsString(object Y) => Y?.ToString() ?? string.Empty;
    // Get X as a float, handling dates and numbers
    public static float? GetXAsFloat(object X,DateTime? minDate = null)
    {
        if (X is float x) return x;
        if (X is double d) return (float)d;
        if (X is int i) return i;
        if (X is DateTime dt && minDate.HasValue)
        {
            // Convert date to float by calculating days difference from a base date
            return (float)(dt - minDate.Value).TotalDays;
        }
        return null;
    }
}
}
