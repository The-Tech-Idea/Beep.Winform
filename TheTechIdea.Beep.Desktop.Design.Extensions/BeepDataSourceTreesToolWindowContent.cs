using Microsoft.VisualStudio.Extensibility.UI;

namespace TheTechIdea.Beep.Desktop.Design.Extensions
{
    /// <summary>
    /// A remote user control to use as tool window UI content.
    /// </summary>
    internal class BeepDataSourceTreesToolWindowContent : RemoteUserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeepDataSourceTreesToolWindowContent" /> class.
        /// </summary>
        public BeepDataSourceTreesToolWindowContent()
            : base(dataContext: new BeepDataSourceTreesToolWindowData())
        {
        }
    }
}
