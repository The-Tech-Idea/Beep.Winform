using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.Vis.Modules
{
    public interface ICrud<T>
    {
        Task<IErrorsInfo> Delete(int id);
        Task<T> Get(int id);
        Task<IEnumerable<T>> Get();
        Task<IEnumerable<T>> GetByFilter(string Filter);
        Task<IEnumerable<T>> GetByFilter(List<AppFilter> Filter);
        Task<IErrorsInfo> Insert(T doc);
        Task<IErrorsInfo> Update(T doc);

    }
}
