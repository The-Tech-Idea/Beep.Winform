using Microsoft.VisualStudio.Extensibility.UI;
using TheTechIdea.Beep.Container.Services;

namespace  TheTechIdea.Beep.Desktop.Design.Extensions
{
    /// <summary>
    /// A remote user control to use as tool window UI content.
    /// </summary>
    internal class BeepDataToolWindowContent : RemoteUserControl
    {
        private readonly IBeepService _beepservice;

        /// <summary>
        /// Initializes a new instance of the <see cref="BeepDataToolWindowContent" /> class.
        /// </summary>
        public BeepDataToolWindowContent(IBeepService beepService)
            : base(dataContext: new BeepDataToolWindowData( beepService))
        {
            // Use this constructor to pass in any required data context.
            _beepservice = beepService;
        }
    }
}
