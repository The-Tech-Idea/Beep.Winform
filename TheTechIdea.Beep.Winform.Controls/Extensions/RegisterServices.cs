using Autofac;
using Autofac.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Desktop.Common.KeyManagement;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Managers;

namespace TheTechIdea.Beep.Desktop.Common
{
    public static class RegisterBeepWinformServices
    {
        public static ContainerBuilder RegisterControlManager(this ContainerBuilder builder)
        {

            builder.RegisterType<ControlManager>().As<IControlManager>().SingleInstance();
            return builder;
        }
        public static IServiceCollection RegisterControlManager(this IServiceCollection services)
        {
         
            services.AddSingleton<IControlManager, ControlManager>();
            return services;
        }
    }
}
