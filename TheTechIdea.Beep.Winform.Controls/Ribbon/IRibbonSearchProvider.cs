using System.Collections.Generic;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Provides optional smart-search results for ribbon commands.
    /// </summary>
    public interface IRibbonSearchProvider
    {
        Task<IEnumerable<SimpleItem>?> SearchAsync(string query, IEnumerable<SimpleItem> candidates);
    }
}
