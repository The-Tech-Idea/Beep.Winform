
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Winform.Controls.Converters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDataConnection : Component
    {
        private IBeepService _beepService;

        public IBeepService BeepService => _beepService;
        public BeepDataConnection()
        {
            InitializeBeepService();
            DataConnections = new List<ConnectionProperties>();
            if (IsInDesignTime())
            {
                LoadDesignTimeConnections();
            }
        }

        [Browsable(true)]
        [Category("Connections")]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        public List<ConnectionProperties> DataConnections { get; set; }

        [Browsable(true)]
        [Category("Current Connection")]
        [TypeConverter(typeof(DataConnectionConverter))]
        public ConnectionProperties CurrentConnection { get; set; }

        /// <summary>
        /// Initializes the BeepService for design-time or runtime use.
        /// </summary>
        private void InitializeBeepService()
        {
            //if (IsInDesignTime())
            //{
            //    _beepService = BeepServiceLocator.GetDesignTimeService();
            //}
            //else
            //{
            //    _beepService = BeepServiceLocator.GetRuntimeService();
            //}
        }

        /// <summary>
        /// Loads design-time connections from the BeepService configuration.
        /// </summary>
        private void LoadDesignTimeConnections()
        {
            var connections = _beepService?.Config_editor?.DataConnections;
            if (connections != null)
            {
                DataConnections.AddRange(connections);
                if (DataConnections.Any())
                {
                    CurrentConnection = DataConnections.First();
                }
            }
        }

        /// <summary>
        /// Determines whether the current context is design-time.
        /// </summary>
        private static bool IsInDesignTime()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }
    }
}
