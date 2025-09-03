using System.ComponentModel;
using System.Text.Json;

namespace TheTechIdea.Beep.Winform.Controls
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BeepFormStylePresets
    {
        public Dictionary<string, BeepFormStyleMetrics> Presets { get; set; } = new();

        public void LoadFromJson(string json)
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, BeepFormStyleMetrics>>(json);
            if (dict == null) return;
            Presets = dict;
        }

        public string SaveToJson() => JsonSerializer.Serialize(Presets, new JsonSerializerOptions{ WriteIndented = true });

        public bool TryGet(string key, out BeepFormStyleMetrics metrics) => Presets.TryGetValue(key, out metrics!);
    }
}
