using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using Newtonsoft.Json;

using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Tools;



namespace TheTechIdea.Beep.Winform.Controls.KeyManagement
{
    public static class KeyManager
    {

        public static GlobalKeyHandler globalKeyHandler ;
        public static IDMEEditor Editor;

        public static IVisManager Vis { get; private set; }

        public static ObservableBindingList<KeyCombination> keyMapToFunction = new ObservableBindingList<KeyCombination>();
        public static void RegisterGlobalKeyHandler(IDMEEditor dMEEditor,IVisManager vis)
        {
            // Registering global key handler
            Createfolder("keyconfig");
            LoadKeyMap();
            Editor=dMEEditor;
            Vis=vis;
            globalKeyHandler = new GlobalKeyHandler();
            Application.AddMessageFilter(globalKeyHandler);
            globalKeyHandler.KeyDown += GlobalKeyDown;
           
        }
        private static void GlobalKeyDown(object sender, KeyEventArgs e)
        {
            var combination = new KeyCombination(ConvertSystemKeysToBeepKeys(e.KeyCode), e.Control, e.Alt, e.Shift);

            if (FindKeyMethod(Editor,combination)!=null)
            {
                Console.WriteLine($"{combination.MappedFunction.Name} triggered");
                // Implement your function call logic based on functionName
                Vis.PressKey(combination);
                e.Handled = true;
            }
        }
        public static void UnregisterGlobalKeyHandler()
        {
            // Unregistering global key handler
           
            if (globalKeyHandler != null)
            {
                Application.RemoveMessageFilter(globalKeyHandler);
            }
        }
        public static AssemblyClassDefinition FindKeyMethod(IDMEEditor dMEEditor, KeyCombination combination)
        {
            return  dMEEditor.ConfigEditor.AppComponents.FirstOrDefault(p=>p.className==combination.ClassName &&  p.GuidID== combination.AssemblyGuid );
        }
        public static bool IsValidKeyCombination(KeyCombination combination)
        {
            // Checks if the combination already exists in the dictionary
            if(keyMapToFunction==null)
            {
                return false;
            }
            if (keyMapToFunction.Count==0)
            {
                return false;
            }
            return !keyMapToFunction.Any(p=>p.Key==combination.Key && p.Control==combination.Control && p.Alt==combination.Alt && p.Shift==combination.Shift);
        }
        public static bool AddKeyCombination(KeyCombination combination)
        {
            if (IsValidKeyCombination(combination))
            {
                keyMapToFunction?.Add(combination);
                return true;
            }

            Console.WriteLine($"Key combination for {combination.Key} with Control: {combination.Control}, Alt: {combination.Alt}, and Shift: {combination.Shift} already exists.");
            return false;
        }
        public static BeepKeys ConvertSystemKeysToBeepKeys(System.Windows.Forms.Keys key)
        {
            return key switch
            {
                Keys.A => BeepKeys.A,
                Keys.B => BeepKeys.B,
                Keys.C => BeepKeys.C,
                Keys.D => BeepKeys.D,
                Keys.E => BeepKeys.E,
                Keys.F => BeepKeys.F,
                Keys.G => BeepKeys.G,
                Keys.H => BeepKeys.H,
                Keys.I => BeepKeys.I,
                Keys.J => BeepKeys.J,
                Keys.K => BeepKeys.K,
                Keys.L => BeepKeys.L,
                Keys.M => BeepKeys.M,
                Keys.N => BeepKeys.N,
                Keys.O => BeepKeys.O,
                Keys.P => BeepKeys.P,
                Keys.Q => BeepKeys.Q,
                Keys.R => BeepKeys.R,
                Keys.S => BeepKeys.S,
                Keys.T => BeepKeys.T,
                Keys.U => BeepKeys.U,
                Keys.V => BeepKeys.V,
                Keys.W => BeepKeys.W,
                Keys.X => BeepKeys.X,
                Keys.Y => BeepKeys.Y,
                Keys.Z => BeepKeys.Z,
                Keys.F1 => BeepKeys.F1,
                Keys.F2 => BeepKeys.F2,
                Keys.F3 => BeepKeys.F3,
                Keys.F4 => BeepKeys.F4,
                Keys.F5 => BeepKeys.F5,
                Keys.F6 => BeepKeys.F6,
                Keys.F7 => BeepKeys.F7,
                Keys.F8 => BeepKeys.F8,
                Keys.F9 => BeepKeys.F9,
                Keys.F10 => BeepKeys.F10,
                Keys.F11 => BeepKeys.F11,
                Keys.F12 => BeepKeys.F12,
                Keys.NumPad0 => BeepKeys.NumPad0,
                Keys.NumPad1 => BeepKeys.NumPad1,
                Keys.NumPad2 => BeepKeys.NumPad2,
                Keys.NumPad3 => BeepKeys.NumPad3,
                Keys.NumPad4 => BeepKeys.NumPad4,
                Keys.NumPad5 => BeepKeys.NumPad5,
                Keys.NumPad6 => BeepKeys.NumPad6,
                Keys.NumPad7 => BeepKeys.NumPad7,
                Keys.NumPad8 => BeepKeys.NumPad8,
                Keys.NumPad9 => BeepKeys.NumPad9,
                Keys.Multiply => BeepKeys.Multiply,
                Keys.Add => BeepKeys.Add,
                Keys.Separator => BeepKeys.Separator,
                Keys.Subtract => BeepKeys.Subtract,
                Keys.Decimal => BeepKeys.Decimal,
                Keys.Divide => BeepKeys.Divide,
                Keys.Enter => BeepKeys.Enter,
                Keys.Shift => BeepKeys.Shift,
                Keys.Control => BeepKeys.Control,
                Keys.Alt => BeepKeys.Alt,
                Keys.Pause => BeepKeys.Pause,
                Keys.CapsLock => BeepKeys.CapsLock,
                Keys.Escape => BeepKeys.Escape,
                Keys.Space => BeepKeys.Space,
                Keys.PageUp => BeepKeys.PageUp,
                Keys.PageDown => BeepKeys.PageDown,
                Keys.End => BeepKeys.End,
                Keys.Home => BeepKeys.Home,
                Keys.Left => BeepKeys.Left,
                Keys.Up => BeepKeys.Up,
                Keys.Right => BeepKeys.Right,
                Keys.Down => BeepKeys.Down,
                Keys.Select => BeepKeys.Select,
                Keys.Print => BeepKeys.Print,
                Keys.Execute => BeepKeys.Execute,
                Keys.PrintScreen => BeepKeys.PrintScreen,
                Keys.Insert => BeepKeys.Insert,
                Keys.Delete => BeepKeys.Delete,
                Keys.Help => BeepKeys.Help,
                Keys.D0 => BeepKeys.D0,
                Keys.D1 => BeepKeys.D1,
                Keys.D2 => BeepKeys.D2,
                Keys.D3 => BeepKeys.D3,
                Keys.D4 => BeepKeys.D4,
                Keys.D5 => BeepKeys.D5,
                Keys.D6 => BeepKeys.D6,
                Keys.D7 => BeepKeys.D7,
                Keys.D8 => BeepKeys.D8,
                Keys.D9 => BeepKeys.D9,
                Keys.Back => BeepKeys.Back,
                Keys.Tab => BeepKeys.Tab,
                Keys.Clear => BeepKeys.Clear,
                Keys.ShiftKey => BeepKeys.Shift,
                Keys.ControlKey => BeepKeys.Control,
                Keys.Menu => BeepKeys.Alt,
                Keys.LWin => BeepKeys.LeftWindows,
                Keys.RWin => BeepKeys.RightWindows,
                Keys.Apps => BeepKeys.Applications,
                Keys.Sleep => BeepKeys.Sleep,
                Keys.NumLock => BeepKeys.NumLock,
                Keys.Scroll => BeepKeys.Scroll,
                Keys.LShiftKey => BeepKeys.LeftShift,
                Keys.RShiftKey => BeepKeys.RightShift,
                Keys.LControlKey => BeepKeys.LeftControl,
                Keys.RControlKey => BeepKeys.RightControl,
                Keys.LMenu => BeepKeys.LeftAlt,
                Keys.RMenu => BeepKeys.RightAlt,
                // Continue mapping other keys...
                // Note: Some keys may not have a direct mapping and can be set to BeepKeys.None or appropriately handled.
                _ => BeepKeys.None,
            };
        }
        public static System.Windows.Forms.Keys ConvertBeepKeysToSystemKeys(BeepKeys beepKey)
        {
            return beepKey switch
            {
                BeepKeys.A => Keys.A,
                BeepKeys.B => Keys.B,
                BeepKeys.C => Keys.C,
                BeepKeys.D => Keys.D,
                BeepKeys.E => Keys.E,
                BeepKeys.F => Keys.F,
                BeepKeys.G => Keys.G,
                BeepKeys.H => Keys.H,
                BeepKeys.I => Keys.I,
                BeepKeys.J => Keys.J,
                BeepKeys.K => Keys.K,
                BeepKeys.L => Keys.L,
                BeepKeys.M => Keys.M,
                BeepKeys.N => Keys.N,
                BeepKeys.O => Keys.O,
                BeepKeys.P => Keys.P,
                BeepKeys.Q => Keys.Q,
                BeepKeys.R => Keys.R,
                BeepKeys.S => Keys.S,
                BeepKeys.T => Keys.T,
                BeepKeys.U => Keys.U,
                BeepKeys.V => Keys.V,
                BeepKeys.W => Keys.W,
                BeepKeys.X => Keys.X,
                BeepKeys.Y => Keys.Y,
                BeepKeys.Z => Keys.Z,
                BeepKeys.F1 => Keys.F1,
                BeepKeys.F2 => Keys.F2,
                BeepKeys.F3 => Keys.F3,
                BeepKeys.F4 => Keys.F4,
                BeepKeys.F5 => Keys.F5,
                BeepKeys.F6 => Keys.F6,
                BeepKeys.F7 => Keys.F7,
                BeepKeys.F8 => Keys.F8,
                BeepKeys.F9 => Keys.F9,
                BeepKeys.F10 => Keys.F10,
                BeepKeys.F11 => Keys.F11,
                BeepKeys.F12 => Keys.F12,
                BeepKeys.NumPad0 => Keys.NumPad0,
                BeepKeys.NumPad1 => Keys.NumPad1,
                BeepKeys.NumPad2 => Keys.NumPad2,
                BeepKeys.NumPad3 => Keys.NumPad3,
                BeepKeys.NumPad4 => Keys.NumPad4,
                BeepKeys.NumPad5 => Keys.NumPad5,
                BeepKeys.NumPad6 => Keys.NumPad6,
                BeepKeys.NumPad7 => Keys.NumPad7,
                BeepKeys.NumPad8 => Keys.NumPad8,
                BeepKeys.NumPad9 => Keys.NumPad9,
                BeepKeys.Multiply => Keys.Multiply,
                BeepKeys.Add => Keys.Add,
                BeepKeys.Separator => Keys.Separator,
                BeepKeys.Subtract => Keys.Subtract,
                BeepKeys.Decimal => Keys.Decimal,
                BeepKeys.Divide => Keys.Divide,
                BeepKeys.Enter => Keys.Enter,
                BeepKeys.Shift => Keys.Shift,
                BeepKeys.Control => Keys.Control,
                BeepKeys.Alt => Keys.Alt,
                BeepKeys.Pause => Keys.Pause,
                BeepKeys.CapsLock => Keys.CapsLock,
                BeepKeys.Escape => Keys.Escape,
                BeepKeys.Space => Keys.Space,
                BeepKeys.PageUp => Keys.PageUp,
                BeepKeys.PageDown => Keys.PageDown,
                BeepKeys.End => Keys.End,
                BeepKeys.Home => Keys.Home,
                BeepKeys.Left => Keys.Left,
                BeepKeys.Up => Keys.Up,
                BeepKeys.Right => Keys.Right,
                BeepKeys.Down => Keys.Down,
                BeepKeys.Select => Keys.Select,
                BeepKeys.Print => Keys.Print,
                BeepKeys.Execute => Keys.Execute,
                BeepKeys.PrintScreen => Keys.PrintScreen,
                BeepKeys.Insert => Keys.Insert,
                BeepKeys.Delete => Keys.Delete,
                BeepKeys.Help => Keys.Help,
                BeepKeys.D0 => Keys.D0,
                BeepKeys.D1 => Keys.D1,
                BeepKeys.D2 => Keys.D2,
                BeepKeys.D3 => Keys.D3,
                BeepKeys.D4 => Keys.D4,
                BeepKeys.D5 => Keys.D5,
                BeepKeys.D6 => Keys.D6,
                BeepKeys.D7 => Keys.D7,
                BeepKeys.D8 => Keys.D8,
                BeepKeys.D9 => Keys.D9,
                BeepKeys.Back => Keys.Back,
                BeepKeys.Tab => Keys.Tab,
                BeepKeys.Clear => Keys.Clear,
                BeepKeys.Sleep => Keys.Sleep,
                BeepKeys.NumLock => Keys.NumLock,
                BeepKeys.Scroll => Keys.Scroll,
                _ => Keys.None,  // Default case for unmapped or unknown BeepKeys values
            };
        }

        public static bool SaveKeyMap()
        {
            Createfolder("keyconfig");
            //save key map to a file or database
            var json =  JsonConvert.SerializeObject(keyMapToFunction, Formatting.Indented);
            System.IO.File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheTechIdea", "Beep", "keyconfig", "keymap.json"), json);
            // Implement your logic to save the key map to a file or database
            return true;
        }
        public static bool LoadKeyMap()
        {
            //load key map from a file or database
            //check if the file exists
            if(!System.IO.File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheTechIdea", "Beep", "keyconfig", "keymap.json")))
            {
                return false;
            }
            var json = System.IO.File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheTechIdea", "Beep", "keyconfig", "keymap.json"));
            // Implement your logic to load the key map from a file or database
            return true;
        }
        private static void Createfolder(string foldername)
        {
          
                if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheTechIdea", "Beep")))
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheTechIdea", "Beep"));

                }
            string    BeepDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheTechIdea", "Beep");
            
            if (!string.IsNullOrEmpty(foldername))
            {
                if (!Directory.Exists(Path.Combine(BeepDataPath, foldername)))
                {
                    Directory.CreateDirectory(Path.Combine(BeepDataPath, foldername));

                }
            }
        }
        public static void Dispose()
        {
            UnregisterGlobalKeyHandler();
        }
        public static void AddKeyCombination(string key, bool control, bool alt, bool shift, string className, string assemblyGuid, string functionName, string description)
        {
            var combination = new KeyCombination((BeepKeys)Enum.Parse(typeof(BeepKeys), key), control, alt, shift)
            {
                ClassName = className,
                AssemblyGuid = assemblyGuid,
                MappedFunction = new MethodsClass { Name = functionName },
                Description = description
            };
            AddKeyCombination(combination);
        }
        public static void AddKeyCombination(this IAssemblyHandler assemblyHandler, string key, bool control, bool alt, bool shift, string className, string assemblyGuid, string functionName, string description)
        {
            var combination = new KeyCombination((BeepKeys)Enum.Parse(typeof(BeepKeys), key), control, alt, shift)
            {
                ClassName = className,
                AssemblyGuid = assemblyGuid,
                MappedFunction = new MethodsClass { Name = functionName },
                Description = description
            };
            AddKeyCombination(combination);
            
        }
    }
}
