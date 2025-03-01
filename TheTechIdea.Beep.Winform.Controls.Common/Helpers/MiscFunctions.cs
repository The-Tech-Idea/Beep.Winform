using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Desktop.Common.Helpers
{
    public static class MiscFunctions
    { /// <summary>
      /// Helper that tries to load an assembly by path (LoadFrom) or 
      /// by full name (Load) depending on whether the string is a valid file path.
      /// </summary>
        public static Assembly LoadAssembly(string assemblyFullNameOrPath)
        {
            try
            {
                if (File.Exists(assemblyFullNameOrPath))
                {
                    // It's a file path
                    return Assembly.LoadFrom(assemblyFullNameOrPath);
                }
                else
                {
                    // Assume it's an assembly full name
                    return Assembly.Load(assemblyFullNameOrPath);
                }
            }
            catch
            {
                return null;
            }
        }
        public static string GetAssemblyName(string assemblyFullNameOrPath)
        {
            try
            {
                if (File.Exists(assemblyFullNameOrPath))
                {
                    // It's a file path
                    return AssemblyName.GetAssemblyName(assemblyFullNameOrPath).Name;
                }
                else
                {
                    // Assume it's an assembly full name
                    return Assembly.Load(assemblyFullNameOrPath).GetName().Name;
                }
            }
            catch
            {
                return null;
            }
        }
        public static string GetRandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GetRandomString(int length, string chars)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GetRandomString(int length, string chars, Random random)
        {
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GetRandomString(int length, Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GetRandomString(int length, string chars, Random random, int seed)
        {
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static BeepMouseEventArgs GetMouseEventArgs(string eventname, MouseEventArgs e)

        {
            BeepMouseEventArgs args = new BeepMouseEventArgs();
            args.EventName = eventname;
            args.Button = (BeepMouseEventArgs.BeepMouseButtons)e.Button;
            args.Clicks = e.Clicks;
            args.X = e.X;
            args.Y = e.Y;
            args.Delta = e.Delta;
            args.Handled = false;
            args.Data = e;
            return args;

        }
        public static void SetThemePropertyinControlifexist(Control control, EnumBeepThemes theme)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control), "The control parameter cannot be null.");
            }

            try
            {
                // Check if the control itself has a "Theme" property
                var themeProperty = control.GetType().GetProperty("Theme");
                if (themeProperty != null && themeProperty.PropertyType == typeof(EnumBeepThemes))
                {
                    // Set the "Theme" property on the control
                    themeProperty.SetValue(control, theme);
                    Debug.WriteLine($"Theme property set on control: {control.Name}");
                    return; // Exit after setting the property
                }



                Debug.WriteLine("No 'Theme' property found on the control or its components.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting theme property: {ex.Message}");
            }
        }
        public static Dictionary<string, object> ConvertPassedArgsToParameters(IPassedArgs passedArgs)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("AddinName", passedArgs.AddinName);
            parameters.Add("AddinType", passedArgs.AddinType);
            parameters.Add("Flag", passedArgs.Flag);
            parameters.Add("Cancel", passedArgs.Cancel);
            parameters.Add("SentData", passedArgs.SentData);
            parameters.Add("SentType", passedArgs.SentType);
            parameters.Add("ReturnData", passedArgs.ReturnData);
            parameters.Add("ReturnType", passedArgs.ReturnType);
            parameters.Add("Category", passedArgs.Category);
            parameters.Add("CurrentEntity", passedArgs.CurrentEntity);
            parameters.Add("DatasourceName", passedArgs.DatasourceName);
            parameters.Add("ViewName", passedArgs.DMView.ViewName);
            parameters.Add("Entities", passedArgs.Entities);
            parameters.Add("EntitiesNames", passedArgs.EntitiesNames);
            parameters.Add("ErrorCode", passedArgs.ErrorCode);
            parameters.Add("ErrorObject", passedArgs.ErrorObject);
            parameters.Add("EventType", passedArgs.EventType);
            parameters.Add("Id", passedArgs.Id);
            parameters.Add("Messege", passedArgs.Messege);
            parameters.Add("ImagePath", passedArgs.ImagePath);
            parameters.Add("IsError", passedArgs.IsError);
            parameters.Add("ObjectName", passedArgs.ObjectName);
            parameters.Add("ObjectType", passedArgs.ObjectType);
            parameters.Add("Objects", passedArgs.Objects);
            parameters.Add("ParameterDate1", passedArgs.ParameterDate1);
            parameters.Add("ParameterDate2", passedArgs.ParameterDate2);
            parameters.Add("ParameterDate3", passedArgs.ParameterDate3);
            parameters.Add("ParameterInt1", passedArgs.ParameterInt1);
            parameters.Add("ParameterInt2", passedArgs.ParameterInt2);
            parameters.Add("ParameterInt3", passedArgs.ParameterInt3);
            parameters.Add("ParameterString1", passedArgs.ParameterString1);
            parameters.Add("ParameterString2", passedArgs.ParameterString2);
            parameters.Add("ParameterString3", passedArgs.ParameterString3);
            parameters.Add("Parameterdouble1", passedArgs.Parameterdouble1);
            parameters.Add("Parameterdouble2", passedArgs.Parameterdouble2);
            parameters.Add("Parameterdouble3", passedArgs.Parameterdouble3);
            parameters.Add("Progress", passedArgs.Progress);
            parameters.Add("Timestamp", passedArgs.Timestamp);
            parameters.Add("Title", passedArgs.Title);
            return parameters;
        }
        public static IPassedArgs ConvertParametersToPassArgs(Dictionary<string, object> parameters)
        {
            IPassedArgs passedArgs = new PassedArgs();
            passedArgs.AddinName = parameters["AddinName"].ToString();
            passedArgs.AddinType = parameters["AddinType"].ToString();
            passedArgs.Flag = Enum.Parse<Errors>(parameters["Flag"].ToString());
            passedArgs.Cancel = (bool)parameters["Cancel"];
            passedArgs.SentData = parameters["SentData"];
            passedArgs.SentType = (Type)parameters["SentType"];
            passedArgs.ReturnData = parameters["ReturnData"];
            passedArgs.ReturnType = (Type)parameters["ReturnType"];
            passedArgs.Category = parameters["Category"].ToString();
            passedArgs.CurrentEntity = parameters["CurrentEntity"].ToString();
            passedArgs.DatasourceName = parameters["DatasourceName"].ToString();
            passedArgs.DMView.ViewName = parameters["ViewName"].ToString();
            passedArgs.Entities = (List<EntityStructure>)parameters["Entities"];
            passedArgs.EntitiesNames = (List<string>)parameters["EntitiesNames"];
            passedArgs.ErrorCode = parameters["ErrorCode"].ToString();
            passedArgs.ErrorObject = (IErrorsInfo)parameters["ErrorObject"];
            passedArgs.EventType = parameters["EventType"].ToString();
            passedArgs.Id = (int)parameters["Id"];
            passedArgs.Messege = parameters["Messege"].ToString();
            passedArgs.ImagePath = parameters["ImagePath"].ToString();
            passedArgs.IsError = (bool)parameters["IsError"];
            passedArgs.ObjectName = parameters["ObjectName"].ToString();
            passedArgs.ObjectType = parameters["ObjectType"].ToString();
            passedArgs.Objects = (List<ObjectItem>)parameters["Objects"];
            passedArgs.ParameterDate1 = (DateTime)parameters["ParameterDate1"];
            passedArgs.ParameterDate2 = (DateTime)parameters["ParameterDate2"];
            passedArgs.ParameterDate3 = (DateTime)parameters["ParameterDate3"];
            passedArgs.ParameterInt1 = (int)parameters["ParameterInt1"];
            passedArgs.ParameterInt2 = (int)parameters["ParameterInt2"];
            passedArgs.ParameterInt3 = (int)parameters["ParameterInt3"];
            passedArgs.ParameterString1 = parameters["ParameterString1"].ToString();
            passedArgs.ParameterString2 = parameters["ParameterString2"].ToString();
            passedArgs.ParameterString3 = parameters["ParameterString3"].ToString();
            passedArgs.Parameterdouble1 = (double)parameters["Parameterdouble1"];
            passedArgs.Parameterdouble2 = (double)parameters["Parameterdouble2"];
            passedArgs.Parameterdouble3 = (double)parameters["Parameterdouble3"];
            passedArgs.Progress = (int)parameters["Progress"];
            passedArgs.Timestamp = (DateTime)parameters["Timestamp"];
            passedArgs.Title = parameters["Title"].ToString();
            return passedArgs;

        }

        // A set of known acronyms (uppercase).
        // If a token fully matches (case-insensitive), we'll keep it uppercase 
        // rather than splitting or spacing it out.
        private static readonly HashSet<string> knownAcronyms = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "AC",       // Air Conditioning or Alternating Current
        "AJAX",
        "AMD",
        "API",
        "ASCII",
        "ASP",
        "BIOS",
        "CSV",
        "CPU",
        "DB",
        "DHCP",
        "DOB",
        "DNS",
        "DOC",
        "EXIF",
        "FTP",
        "FHIR",
        "GPU",
        "GUID",
        "HEX",
        "HR",
        "HTML",
        "HTTP",
        "HTTPS",
        "HUD",
        "I/O",
        "ID",
        "IP",
        "ISO",
        "JSON",
        "JDBC",
        "JPEG",
        "JPG",
        "JWT",
        "KPI",
        "LAN",
        "MAC",
        "MVC",
        "MVP",
        "MVVM",
        "NET",      // Representing ".NET" (cannot include a dot in a token easily)
        "ODBC",
        "OLE",
        "OS",
        "PDF",
        "PNG",
        "RAM",
        "REST",
        "RPC",
        "SHA",
        "SMTP",
        "SNMP",
        "SOAP",
        "SQL",
        "SSAS",
        "SSIS",
        "SSN",
        "SSRS",
        "SSL",
        "SSH",
        "SSO",
        "TLS",
        "TFS",
        "UID",
        "UI",
        "URI",
        "URL",
        "USB",
        "UTF8",
        "UX",
        "VPN",
        "WCF",
        "XML"
        // Add more as needed...
    };

        /// <summary>
        /// Creates a more user-friendly caption from a raw field name.
        /// Examples:
        ///   "FirstName" -> "First Name", 
        ///   "Contact_Phone_Number" -> "Contact Phone Number",
        ///   "DOB" -> "DOB", 
        ///   "SQLVersion" -> "SQL Version"
        /// </summary>
        public static string CreateCaptionFromFieldName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            // 1) Replace underscores with spaces
            fieldName = fieldName.Replace('_', ' ');

            // 2) Split on spaces so we can handle each "word" or "token" individually.
            //    E.g. "Contact Phone DOB" -> ["Contact", "Phone", "DOB"]
            string[] tokens = fieldName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];

                // Convert the token to uppercase for checking against knownAcronyms
                string upperToken = token.ToUpperInvariant();

                // 3) If the entire token is a known acronym, keep it uppercase
                //    (avoiding splitting it into letters).
                if (knownAcronyms.Contains(upperToken))
                {
                    tokens[i] = upperToken;
                }
                else
                {
                    // 4) Otherwise, insert spaces before uppercase letters to handle CamelCase/PascalCase.
                    //    e.g., "FirstName" -> "First Name".
                    tokens[i] = InsertSpacesBeforeUpper(token);
                }
            }

            // 5) Re-join tokens with a space and return the final caption.
            return string.Join(" ", tokens).Trim();
        }

        /// <summary>
        /// Inserts a space before uppercase letters in a token,
        /// unless the previous letter is also uppercase.
        /// e.g., "FirstName" -> "First Name", "FullAddress" -> "Full Address"
        /// </summary>
        private static string InsertSpacesBeforeUpper(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var sb = new StringBuilder();
            sb.Append(str[0]); // Start with the first character.

            for (int i = 1; i < str.Length; i++)
            {
                char currentChar = str[i];
                char previousChar = sb[sb.Length - 1];

                // If current char is uppercase, and the previous is not uppercase/space, insert a space.
                // So "VersionNumber" -> "Version Number". 
                // "DOB" remains "D O B" unless recognized as an acronym above.
                if (char.IsUpper(currentChar)
                    && !char.IsWhiteSpace(previousChar)
                    && !char.IsUpper(previousChar))
                {
                    sb.Append(' ');
                }
                sb.Append(currentChar);
            }

            return sb.ToString();
        }
        /// <summary>
        /// Converts a value to the expected property type.
        /// Handles cases where numbers need to be converted between types safely.
        /// </summary>
        public static object ConvertValueToPropertyType(Type targetType, object value)
        {
            if (value == null || value == DBNull.Value)
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

            try
            {
                if (value == null)
                {
                    return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
                    // For bool: returns false
                    // For char: returns '\0'
                    // For string: returns null
                }

                if (targetType == typeof(bool))
                {
                    if (bool.TryParse(value.ToString(), out bool result))
                        return result;
                    return false; // Default for invalid bool
                }
                if (targetType == typeof(char))
                {
                    if (value is char c)
                        return c;
                    if (!string.IsNullOrEmpty(value.ToString()) && value.ToString().Length > 0)
                        return value.ToString()[0];
                    return '\0'; // Default for invalid char
                }
                if (targetType == typeof(string))
                {
                    return value.ToString();
                }
                if (targetType == typeof(int))
                    return Convert.ToInt32(value);  // 🔹 Converts decimals/floats safely to int
                if (targetType == typeof(long))
                    return Convert.ToInt64(value);
                if (targetType == typeof(float))
                    return Convert.ToSingle(value);
                if (targetType == typeof(double))
                    return Convert.ToDouble(value);
                if (targetType == typeof(decimal))
                    return Convert.ToDecimal(value);
                if (targetType == typeof(bool))
                    return Convert.ToBoolean(value);
                if (targetType == typeof(string))
                    return value.ToString();
                if (targetType == typeof(DateTime))
                    return Convert.ToDateTime(value);
                if (targetType.IsEnum)
                    return Enum.Parse(targetType, value.ToString());

                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }
        }
    }
}
