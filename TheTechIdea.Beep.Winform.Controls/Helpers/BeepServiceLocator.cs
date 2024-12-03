using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    /// <summary>
    /// A static service locator for managing BeepService instances.
    /// </summary>
    public static class BeepServiceLocator
    {
        private static IBeepService _designTimeService;
        private static IBeepService _runtimeService;

        public static IBeepService GetDesignTimeService()
        {
            if (_designTimeService == null)
            {
                _designTimeService = new BeepService();
                _designTimeService.ConfigureForDesignTime();
            }
            return _designTimeService;
        }

        public static IBeepService GetRuntimeService()
        {
            if (_runtimeService == null)
            {
                _runtimeService = new IBeepService();
                _runtimeService.Configure(AppContext.BaseDirectory, "RuntimeContainer", BeepConfigType.DataConnector, true);
            }
            return _runtimeService;
        }
    }

}
