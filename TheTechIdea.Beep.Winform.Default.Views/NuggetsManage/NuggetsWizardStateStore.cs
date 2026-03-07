using System;
using System.IO;
using System.Text.Json;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    public sealed class NuggetsWizardStateStore
    {
        private readonly IDMEEditor _editor;
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true
        };

        public NuggetsWizardStateStore(IDMEEditor editor)
        {
            _editor = editor ?? throw new ArgumentNullException(nameof(editor));
        }

        public NuggetsWizardPersistedState Load()
        {
            var statePath = GetStatePath();
            if (!File.Exists(statePath))
            {
                return new NuggetsWizardPersistedState();
            }

            try
            {
                var json = File.ReadAllText(statePath);
                return JsonSerializer.Deserialize<NuggetsWizardPersistedState>(json, _serializerOptions)
                    ?? new NuggetsWizardPersistedState();
            }
            catch
            {
                return new NuggetsWizardPersistedState();
            }
        }

        public void Save(NuggetsWizardPersistedState state)
        {
            var statePath = GetStatePath();
            var dir = Path.GetDirectoryName(statePath);
            if (!string.IsNullOrWhiteSpace(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var json = JsonSerializer.Serialize(state ?? new NuggetsWizardPersistedState(), _serializerOptions);
            File.WriteAllText(statePath, json);
        }

        private string GetStatePath()
        {
            var configPath = _editor.ConfigEditor?.ConfigPath;
            if (string.IsNullOrWhiteSpace(configPath))
            {
                configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
            }

            return Path.Combine(configPath, "NuggetsWizardState.json");
        }
    }
}
