using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using Newtonsoft.Json;

using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Tools;

namespace TheTechIdea.Beep.Desktop.Common.KeyManagement
{
    public interface IKeyHandlingManager
    {
        KeyMapStorage KeyMapStorage { get; set; }
        GlobalKeyHandler GlobalKeyHandler { get; set; }
        void LoadKeyMap();
        void SaveKeyMap();
        event EventHandler<BeepEventDataArgs> KeyPressed;
        ObservableBindingList<KeyCombination> KeyMapToFunction { get; set; }
        void UnregisterGlobalKeyHandler();
        void RegisterGlobalKeyHandler();
        AssemblyClassDefinition FindKeyMethod(KeyCombination combination);
        void RunFunctionFromKey(KeyCombination combination);
        void GlobalKeyDown(object sender, KeyEventArgs e);
        bool IsValidKeyCombination(KeyCombination combination);
        bool AddKeyCombination(KeyCombination combination);
        BeepKeys ConvertSystemKeysToBeepKeys(Keys key);
        Keys ConvertBeepKeysToSystemKeys(BeepKeys beepKey);
        void AddKeyCombination(string key, bool control, bool alt, bool shift, string className, string assemblyGuid, string functionName, string description);
        void AddKeyCombination(IAssemblyHandler assemblyHandler, string key, bool control, bool alt, bool shift, string className, string assemblyGuid, string functionName, string description);
        void RemoveKeyCombination(KeyCombination combination);
        void RemoveKeyCombination(string key, bool control, bool alt, bool shift);
        void RemoveKeyCombination(string key);
        void RemoveKeyCombination(BeepKeys key);
        void RemoveKeyCombination(Keys key);

    }
}
