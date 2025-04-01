using Autofac;
using Autofac.Core;
using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ITrees.BeepTreeView;
using TheTechIdea.Beep.Winform.Controls.Managers;

namespace TheTechIdea.Beep.Desktop.Common
{
    public static class RegisterBeepWinformServices
    {
        public static ContainerBuilder RegisterControlManager(this ContainerBuilder builder)
        {

            builder.RegisterType<DialogManager>().As<IDialogManager>().SingleInstance();
            return builder;
        }
        public static ContainerBuilder RegisterTreeControl(this ContainerBuilder builder)
        {

            builder.RegisterType<BeepAppTree>().As<ITree>().SingleInstance();
            return builder;
        }
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
