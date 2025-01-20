using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Tools;
using TheTechIdea.Beep.Vis;

namespace TheTechIdea.Beep.Desktop.Common.KeyManagement
{
    public class KeyHandlingManager: IKeyHandlingManager
    {
        private  readonly Dictionary<Keys, BeepKeys> KeysMapping = new()
{
    { Keys.A, BeepKeys.A },
    { Keys.B, BeepKeys.B },
    { Keys.C, BeepKeys.C },
    { Keys.D, BeepKeys.D },
    { Keys.E, BeepKeys.E },
    { Keys.F, BeepKeys.F },
    { Keys.G, BeepKeys.G },
    { Keys.H, BeepKeys.H },
    { Keys.I, BeepKeys.I },
    { Keys.J, BeepKeys.J },
    { Keys.K, BeepKeys.K },
    { Keys.L, BeepKeys.L },
    { Keys.M, BeepKeys.M },
    { Keys.N, BeepKeys.N },
    { Keys.O, BeepKeys.O },
    { Keys.P, BeepKeys.P },
    { Keys.Q, BeepKeys.Q },
    { Keys.R, BeepKeys.R },
    { Keys.S, BeepKeys.S },
    { Keys.T, BeepKeys.T },
    { Keys.U, BeepKeys.U },
    { Keys.V, BeepKeys.V },
    { Keys.W, BeepKeys.W },
    { Keys.X, BeepKeys.X },
    { Keys.Y, BeepKeys.Y },
    { Keys.Z, BeepKeys.Z },
    { Keys.F1, BeepKeys.F1 },
    { Keys.F2, BeepKeys.F2 },
    { Keys.F3, BeepKeys.F3 },
    { Keys.F4, BeepKeys.F4 },
    { Keys.F5, BeepKeys.F5 },
    { Keys.F6, BeepKeys.F6 },
    { Keys.F7, BeepKeys.F7 },
    { Keys.F8, BeepKeys.F8 },
    { Keys.F9, BeepKeys.F9 },
    { Keys.F10, BeepKeys.F10 },
    { Keys.F11, BeepKeys.F11 },
    { Keys.F12, BeepKeys.F12 },
    { Keys.NumPad0, BeepKeys.NumPad0 },
    { Keys.NumPad1, BeepKeys.NumPad1 },
    { Keys.NumPad2, BeepKeys.NumPad2 },
    { Keys.NumPad3, BeepKeys.NumPad3 },
    { Keys.NumPad4, BeepKeys.NumPad4 },
    { Keys.NumPad5, BeepKeys.NumPad5 },
    { Keys.NumPad6, BeepKeys.NumPad6 },
    { Keys.NumPad7, BeepKeys.NumPad7 },
    { Keys.NumPad8, BeepKeys.NumPad8 },
    { Keys.NumPad9, BeepKeys.NumPad9 },
    { Keys.Multiply, BeepKeys.Multiply },
    { Keys.Add, BeepKeys.Add },
    { Keys.Separator, BeepKeys.Separator },
    { Keys.Subtract, BeepKeys.Subtract },
    { Keys.Decimal, BeepKeys.Decimal },
    { Keys.Divide, BeepKeys.Divide },
    { Keys.Enter, BeepKeys.Enter },
    { Keys.Shift, BeepKeys.Shift },
    { Keys.Control, BeepKeys.Control },
    { Keys.Alt, BeepKeys.Alt },
    { Keys.Pause, BeepKeys.Pause },
    { Keys.CapsLock, BeepKeys.CapsLock },
    { Keys.Escape, BeepKeys.Escape },
    { Keys.Space, BeepKeys.Space },
    { Keys.PageUp, BeepKeys.PageUp },
    { Keys.PageDown, BeepKeys.PageDown },
    { Keys.End, BeepKeys.End },
    { Keys.Home, BeepKeys.Home },
    { Keys.Left, BeepKeys.Left },
    { Keys.Up, BeepKeys.Up },
    { Keys.Right, BeepKeys.Right },
    { Keys.Down, BeepKeys.Down },
    { Keys.Back, BeepKeys.Back },
    { Keys.Tab, BeepKeys.Tab },
    { Keys.Clear, BeepKeys.Clear },
    { Keys.PrintScreen, BeepKeys.PrintScreen },
    { Keys.Insert, BeepKeys.Insert },
    { Keys.Delete, BeepKeys.Delete },
    { Keys.Help, BeepKeys.Help },
    { Keys.NumLock, BeepKeys.NumLock },
    { Keys.Scroll, BeepKeys.Scroll },
    { Keys.LWin, BeepKeys.LeftWindows },
    { Keys.RWin, BeepKeys.RightWindows },
    { Keys.LShiftKey, BeepKeys.LeftShift },
    { Keys.RShiftKey, BeepKeys.RightShift },
    { Keys.LControlKey, BeepKeys.LeftControl },
    { Keys.RControlKey, BeepKeys.RightControl },
    { Keys.LMenu, BeepKeys.LeftAlt },
    { Keys.RMenu, BeepKeys.RightAlt },
};

        public event EventHandler<BeepEventDataArgs> KeyPressed;

        public KeyHandlingManager()
        {
        }

        public KeyMapStorage KeyMapStorage { get ; set ; }
        public GlobalKeyHandler GlobalKeyHandler { get ; set ; }
        public ObservableBindingList<KeyCombination> KeyMapToFunction { get ; set ; }

        public  BeepKeys ConvertSystemKeysToBeepKeys(Keys key)
        {
            return KeysMapping.TryGetValue(key, out var beepKey) ? beepKey : BeepKeys.None;
        }

        public  Keys ConvertBeepKeysToSystemKeys(BeepKeys beepKey)
        {
            return KeysMapping.FirstOrDefault(kvp => kvp.Value == beepKey).Key;
        }

        public void LoadKeyMap()
        {
            throw new NotImplementedException();
        }

        public void SaveKeyMap()
        {
            throw new NotImplementedException();
        }

        public void UnregisterGlobalKeyHandler()
        {
            throw new NotImplementedException();
        }

        public void RegisterGlobalKeyHandler()
        {
            throw new NotImplementedException();
        }

        public AssemblyClassDefinition FindKeyMethod(KeyCombination combination)
        {
            throw new NotImplementedException();
        }

        public void RunFunctionFromKey(KeyCombination combination)
        {
            throw new NotImplementedException();
        }

        public bool IsValidKeyCombination(KeyCombination combination)
        {
            throw new NotImplementedException();
        }

        public bool AddKeyCombination(KeyCombination combination)
        {
            throw new NotImplementedException();
        }

       

        public void AddKeyCombination(string key, bool control, bool alt, bool shift, string className, string assemblyGuid, string functionName, string description)
        {
            throw new NotImplementedException();
        }

        public void AddKeyCombination( IAssemblyHandler assemblyHandler, string key, bool control, bool alt, bool shift, string className, string assemblyGuid, string functionName, string description)
        {
            throw new NotImplementedException();
        }

        public void RemoveKeyCombination(KeyCombination combination)
        {
            throw new NotImplementedException();
        }

        public void RemoveKeyCombination(string key, bool control, bool alt, bool shift)
        {
            throw new NotImplementedException();
        }

        public void RemoveKeyCombination(string key)
        {
            throw new NotImplementedException();
        }

        public void RemoveKeyCombination(BeepKeys key)
        {
            throw new NotImplementedException();
        }

        public void RemoveKeyCombination(Keys key)
        {
            throw new NotImplementedException();
        }
    }
}
