using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Desktop.Common
{
    public static class MiscFunctions
    {
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

        public static Dictionary<string,object> ConvertPassedArgsToParameters(IPassedArgs passedArgs)
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
            parameters.Add("Progress", passedArgs.Progress  );
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
    }
}
