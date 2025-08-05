
using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ITrees.BeepTreeView;
using TheTechIdea.Beep.Winform.Controls.Managers;

namespace TheTechIdea.Beep.Desktop.Common
{
    public  static partial class RegisterBeepWinformServices
    {
       
        public static IServiceCollection RegisterControlManager(this IServiceCollection services)
        {
         
            services.AddSingleton<IDialogManager, DialogManager>();
            return services;
        }
        public static IServiceCollection RegisterTreeControl(this IServiceCollection services)
        {

            services.AddSingleton<ITree, BeepAppTree>();
            return services;
        }
    }
}
