using Autofac;
using Autofac.Core;
using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Beep.Vis.Modules;
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
        public static IServiceCollection RegisterControlManager(this IServiceCollection services)
        {
         
            services.AddSingleton<IDialogManager, DialogManager>();
            return services;
        }
    }
}
