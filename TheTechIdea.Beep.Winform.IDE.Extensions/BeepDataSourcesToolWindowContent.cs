using Microsoft.VisualStudio.Extensibility.UI;

namespace TheTechIdea.Beep.Winform.IDE.Extensions
{
    /// <summary>
    /// A remote user control to use as tool window UI content.
    /// </summary>
    internal class BeepDataSourcesToolWindowContent : RemoteUserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeepDataSourcesToolWindowContent" /> class.
        /// </summary>
        public BeepDataSourcesToolWindowContent()
            : base(dataContext: new BeepDataSourcesToolWindowData())
        {
        }
    }
}
