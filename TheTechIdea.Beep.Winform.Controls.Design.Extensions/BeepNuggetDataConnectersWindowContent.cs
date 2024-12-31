using Microsoft.VisualStudio.Extensibility.UI;
using TheTechIdea.Beep.Container.Services;

namespace TheTechIdea.Beep.Desktop.Design.Extensions
{
    /// <summary>
    /// A remote user control to use as tool window UI content.
    /// </summary>
    internal class BeepNuggetDataConnectersWindowContent : RemoteUserControl
    {
        private readonly IBeepService _beepservice;

        /// <summary>
        /// Initializes a new instance of the <see cref="BeepNuggetDataConnectersWindowContent" /> class.
        /// </summary>
        public BeepNuggetDataConnectersWindowContent(IBeepService beepService)
            : base(dataContext: new BeepNuggetDataConnectersWindowData(beepService))
        {
            _beepservice = beepService;
        }
    }
}
