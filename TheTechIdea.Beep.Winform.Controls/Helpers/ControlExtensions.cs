using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;
using TheTechIdea.Beep.ConfigUtil;

using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

using System.Linq.Expressions;
using System.Runtime.InteropServices;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using TheTechIdea.Beep.Winform.Controls.Numerics;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;



namespace TheTechIdea.Beep.Winform.Controls.Helpers;
public static partial class ControlExtensions
{
    public static void SetDoubleBuffered(Control control, bool enabled)
    {
        var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (doubleBufferPropertyInfo != null)
        {
            doubleBufferPropertyInfo.SetValue(control, enabled, null);
        }
    }

    private const int WM_SETREDRAW = 0x000B;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Disables the repainting of the control (and its children).
    /// </summary>
    public static void SuspendDrawing(this Control control)
    {
        if (control == null)
            throw new ArgumentNullException(nameof(control));

        SendMessage(control.Handle, WM_SETREDRAW, (IntPtr)0, IntPtr.Zero);
    }

    /// <summary>
    /// Re-enables repainting of the control and forces a refresh.
    /// </summary>
    public static void ResumeDrawing(this Control control)
    {
        if (control == null)
            throw new ArgumentNullException(nameof(control));

        SendMessage(control.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
        control.Refresh(); // Force a redraw.
    }
    public delegate T ObjectActivator<T>(params object[] args);
    public static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
    {
        Type type = ctor.DeclaringType;
        ParameterInfo[] paramsInfo = ctor.GetParameters();

        //create a single param of type object[]
        ParameterExpression param =
            Expression.Parameter(typeof(object[]), "args");

        Expression[] argsExp =
            new Expression[paramsInfo.Length];

        //pick each arg from the params array 
        //and create a typed expression of them
        for (int i = 0; i < paramsInfo.Length; i++)
        {
            Expression index = Expression.Constant(i);
            Type paramType = paramsInfo[i].ParameterType;

            Expression paramAccessorExp =
                Expression.ArrayIndex(param, index);

            Expression paramCastExp =
                Expression.Convert(paramAccessorExp, paramType);

            argsExp[i] = paramCastExp;
        }

        //make a NewExpression that calls the
        //ctor with the args we just created
        NewExpression newExp = Expression.New(ctor, argsExp);

        //create a lambda with the New
        //Expression as body and our param object[] as arg
        LambdaExpression lambda =
            Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

        //compile it
        ObjectActivator<T> compiled = (ObjectActivator<T>)lambda.Compile();
        return compiled;
    }
    public static void DoubleBuffered(this Control control, bool enable)
    {
        var doubleBufferProperty = control.GetType().GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (doubleBufferProperty != null)
        {
            doubleBufferProperty.SetValue(control, enable, null);
        }
    }
    public static string GetFormattedText(this Control control, string text, string maskFormat)
    {
        if (string.IsNullOrEmpty(maskFormat))
            return text;

        // Number formatting
        if (decimal.TryParse(text, out var number))
        {
            switch (maskFormat.ToLower())
            {
                case "currency":
                    return number.ToString("C", CultureInfo.CurrentCulture); // Currency format
                case "percentage":
                    return number.ToString("P", CultureInfo.CurrentCulture); // Percentage format
                case "fixedpoint":
                    return number.ToString("F2", CultureInfo.CurrentCulture); // Fixed-point format with 2 decimals
                case "scientific":
                    return number.ToString("E", CultureInfo.CurrentCulture); // Scientific notation
                case "number":
                    return number.ToString("N", CultureInfo.CurrentCulture); // Number with thousand separators
                case "hexadecimal":
                    return ((int)number).ToString("X"); // Hexadecimal format (integers only)
            }
        }

        // Date formatting
        if (DateTime.TryParse(text, out var date))
        {
            switch (maskFormat.ToLower())
            {
                case "shortdate":
                    return date.ToString("d", CultureInfo.CurrentCulture); // Short date
                case "longdate":
                    return date.ToString("D", CultureInfo.CurrentCulture); // Long date
                case "shorttime":
                    return date.ToString("t", CultureInfo.CurrentCulture); // Short time
                case "longtime":
                    return date.ToString("T", CultureInfo.CurrentCulture); // Long time
                case "monthday":
                    return date.ToString("M", CultureInfo.CurrentCulture); // Month day format
                case "yearmonth":
                    return date.ToString("Y", CultureInfo.CurrentCulture); // Year month format
                case "rfc1123":
                    return date.ToString("R", CultureInfo.InvariantCulture); // RFC 1123 format
                case "sortable":
                    return date.ToString("s", CultureInfo.InvariantCulture); // Sortable date/time format
                case "iso":
                    return date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture); // ISO 8601 format
            }
        }

        // Custom formatting (if users want to specify their own format pattern)
        try
        {
            // Assume the user-provided maskFormat is a valid .NET format string
            return string.Format(CultureInfo.CurrentCulture, "{0:" + maskFormat + "}", text);
        }
        catch
        {
            // Return unformatted text if the format string is invalid
            return text;
        }
    }

    // Special thanks to ReaLTaiizor for this code snippet

    public static Type GetDefaultControlType(DbFieldCategory category)
    {
        switch (category)
        {
            case DbFieldCategory.String:
                return typeof(BeepTextBox);
            case DbFieldCategory.Numeric:
                return typeof(BeepTextBox);
            case DbFieldCategory.Date:
                return typeof(BeepDatePicker);
            case DbFieldCategory.Boolean:
                return typeof(BeepCheckBoxBool);
            case DbFieldCategory.Binary:
                return typeof(BeepImage);
            case DbFieldCategory.Guid:
                return typeof(BeepTextBox);
            case DbFieldCategory.Json:
                return typeof(BeepTextBox);
            case DbFieldCategory.Xml:
                return typeof(BeepTextBox);
            case DbFieldCategory.Geography:
                return typeof(BeepTextBox);
            case DbFieldCategory.Currency:
                return typeof(BeepTextBox);
            case DbFieldCategory.Enum:
                return typeof(BeepComboBox);
            case DbFieldCategory.Timestamp:
                return typeof(BeepTextBox);
            case DbFieldCategory.Complex:
                return typeof(BeepTextBox);
            default:
                return typeof(BeepTextBox);
        }
    }


    private static Control GetControlCorrectPositiononForm(Control control)
    {
        if (control == null)
            throw new ArgumentNullException(nameof(control));

        // Ensure the control belongs to a form
        Form form = control.FindForm();
        if (form == null)
            throw new InvalidOperationException("The control does not belong to a form.");

        // Start with the control's position and iterate through all parent controls
        Point positionOnForm = control.Location;
        Control parent = control.Parent;

        while (parent != null && parent != form)
        {
            positionOnForm.Offset(parent.Location);
            parent = parent.Parent;
        }

        // At this point, positionOnForm contains the correct position on the form

        // Create a new control with the same size and correct location
        Control newControl = new Control
        {
            Size = control.Size,
            Location = positionOnForm
        };

        return newControl;
    }
    private static Point GetAdjustmentPoint(Control baseControl, Control targetControl)
    {
        if (baseControl == null)
            throw new ArgumentNullException(nameof(baseControl));
        if (targetControl == null)
            throw new ArgumentNullException(nameof(targetControl));

        // Ensure both controls belong to a form
        Form baseForm = baseControl.FindForm();
        Form targetForm = targetControl.FindForm();
        if (baseForm == null || targetForm == null || baseForm != targetForm)
            throw new InvalidOperationException("Both controls must belong to the same form.");

        // Get the positions of both controls relative to the form
        Point baseLocation = GetControlCorrectPointPositiononForm(baseControl);
        Point targetLocation = GetControlCorrectPointPositiononForm(targetControl);

        // Calculate the adjustment point
        return new Point(targetLocation.X - baseLocation.X, targetLocation.Y - baseLocation.Y);
    }
    public static IBeepUIComponent CreateFieldBasedOnCategory(string propertyName, Type propertyType)
    {
        DbFieldCategory category = MapTypeToDbFieldCategory(propertyType);

        switch (category)
        {
            case DbFieldCategory.String:
                return new BeepTextBox { BoundProperty = propertyName, ComponentName = propertyName };

            case DbFieldCategory.Numeric:
            case DbFieldCategory.Currency:
                return new BeepNumericUpDown { BoundProperty = propertyName, ComponentName = propertyName };

            case DbFieldCategory.Date:
            case DbFieldCategory.Timestamp:
                return new BeepDatePicker { BoundProperty = propertyName, ComponentName = propertyName };

            case DbFieldCategory.Boolean:
                return new BeepCheckBoxBool { BoundProperty = propertyName, ComponentName = propertyName };

            case DbFieldCategory.Guid:
                return new BeepTextBox { BoundProperty = propertyName, ComponentName = propertyName, ReadOnly = true };

            case DbFieldCategory.Json:
            case DbFieldCategory.Xml:
                return new BeepTextBox { BoundProperty = propertyName, ComponentName = propertyName };

            default:
                return new BeepTextBox { BoundProperty = propertyName, ComponentName = propertyName };
        }
    }
    public static DbFieldCategory MapTypeToDbFieldCategory(Type type)
    {
        if (type == typeof(string))
            return DbFieldCategory.String;
        if (type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(decimal))
            return DbFieldCategory.Numeric;
        if (type == typeof(DateTime))
            return DbFieldCategory.Date;
        if (type == typeof(bool))
            return DbFieldCategory.Boolean;
        if (type == typeof(Guid))
            return DbFieldCategory.Guid;
        if (type == typeof(byte[]))
            return DbFieldCategory.Binary;
        if (type == typeof(System.Xml.XmlDocument) || type == typeof(System.Xml.Linq.XElement))
            return DbFieldCategory.Xml;
        if (type == typeof(System.Text.Json.JsonDocument))
            return DbFieldCategory.Json;
        if (type.IsEnum)
            return DbFieldCategory.Enum;

        return DbFieldCategory.Complex;
    }
    private static Point GetControlCorrectPointPositiononForm(Control control)
    {
        if (control == null)
            throw new ArgumentNullException(nameof(control));

        // Ensure the control belongs to a form
        Form form = control.FindForm();
        if (form == null)
            throw new InvalidOperationException("The control does not belong to a form.");

        // Start with the control's position and iterate through all parent controls
        Point positionOnForm = control.Location;
        Control parent = control.Parent;

        while (parent != null && parent != form)
        {
            positionOnForm.Offset(parent.Location);
            parent = parent.Parent;
        }

        // Return the calculated position on the form
        return positionOnForm;
    }
}


