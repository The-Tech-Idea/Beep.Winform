using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Vis.Modules
{
    public class BeepThemesManager_v2
    {
        public List<BeepTheme> Themes { get; } = new List<BeepTheme>();

        public BeepThemesManager_v2()
        {
            var themeTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(BeepTheme)) && !t.IsAbstract);

            foreach (var type in themeTypes)
            {
                if (Activator.CreateInstance(type) is BeepTheme theme)
                    Themes.Add(theme);
            }
        }
    }
}
