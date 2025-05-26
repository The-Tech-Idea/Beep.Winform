//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TheTechIdea.Beep.Vis.Modules
//{
//    public class BeepComponentsJsonConverter : JsonConverter<BeepComponents>
//    {
//        public override BeepComponents ReadJson(JsonReader reader, Type objectType, BeepComponents existingValue, bool hasExistingValue, JsonSerializer serializer)
//        {
//            JObject obj = JObject.Load(reader);
//            var component = new BeepComponents
//            {
//                Name = obj["Name"]?.ToString(),
//                Description = obj["Description"]?.ToString(),
//                Assembly = obj["Assembly"]?.ToString(),
//                Namespace = obj["Namespace"]?.ToString(),
//                ClassName = obj["ClassName"]?.ToString(),
//                Id = obj["Id"]?.ToObject<int>() ?? 0,
//                GUID = obj["GUID"]?.ToString(),
//                ControlGuidID = obj["ControlGuidID"]?.ToString(),
//                ParentGUID = obj["ParentGUID"]?.ToString(),
//                ParentName = obj["ParentName"]?.ToString(),
//                ParentType = obj["ParentType"]?.ToString(),
//                ParentAssembly = obj["ParentAssembly"]?.ToString(),
//                ParentNamespace = obj["ParentNamespace"]?.ToString(),
//                ParentClassName = obj["ParentClassName"]?.ToString(),
//                Left = obj["Left"]?.ToObject<int>() ?? 0,
//                Top = obj["Top"]?.ToObject<int>() ?? 0,
//                Width = obj["Width"]?.ToObject<int>() ?? 0,
//                Height = obj["Height"]?.ToObject<int>() ?? 0,
//                BoundProperty = obj["BoundProperty"]?.ToString(),
//                DataSourceProperty = obj["DataSourceProperty"]?.ToString(),
//                LinkedProperty = obj["LinkedProperty"]?.ToString(),
//                TypeFullName = obj["Type"]?.ToString()
//            };

//            return component;
//        }

//        public override void WriteJson(JsonWriter writer, BeepComponents value, JsonSerializer serializer)
//        {
//            JObject obj = new JObject
//            {
//                { "Name", value.Name },
//                { "Description", value.Description },
//                { "Assembly", value.Assembly },
//                { "Namespace", value.Namespace },
//                { "ClassName", value.ClassName },
//                { "Id", value.Id },
//                { "GUID", value.GUID },
//                { "ControlGuidID", value.ControlGuidID },
//                { "ParentGUID", value.ParentGUID },
//                { "ParentName", value.ParentName },
//                { "ParentType", value.ParentType },
//                { "ParentAssembly", value.ParentAssembly },
//                { "ParentNamespace", value.ParentNamespace },
//                { "ParentClassName", value.ParentClassName },
//                { "Left", value.Left },
//                { "Top", value.Top },
//                { "Width", value.Width },
//                { "Height", value.Height },
//                { "BoundProperty", value.BoundProperty },
//                { "DataSourceProperty", value.DataSourceProperty },
//                { "LinkedProperty", value.LinkedProperty },
//                { "Type", value.Type?.FullName }
//            };
//            obj.WriteTo(writer);
//        }
//    }
//}
