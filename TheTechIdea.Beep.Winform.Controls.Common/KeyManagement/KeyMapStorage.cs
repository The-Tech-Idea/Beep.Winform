using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis;

namespace TheTechIdea.Beep.Desktop.Common.KeyManagement
{
    public class KeyMapStorage
    {
        private readonly string _filePath;

        public KeyMapStorage(string filePath)
        {
            _filePath = filePath;
        }

        public ObservableBindingList<KeyCombination> Load()
        {
            if (!File.Exists(_filePath))
                return new ObservableBindingList<KeyCombination>();

            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<ObservableBindingList<KeyCombination>>(json);
        }

        public void Save(ObservableBindingList<KeyCombination> keyMap)
        {
            var json = JsonConvert.SerializeObject(keyMap, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }

}
