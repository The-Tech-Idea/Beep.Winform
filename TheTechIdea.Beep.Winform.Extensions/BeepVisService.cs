using TheTechIdea.Beep.Vis.Modules;
using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Winform.Controls.Managers;
using TheTechIdea.Beep.Addin;

namespace TheTechIdea.Beep.Winform.Extensions
{
    public static class BeepVisService
    {
        private static IServiceCollection Services;
  
        public static IServiceCollection RegisterVisManager(this IServiceCollection services)
        {
            Services = services;
            Services.AddSingleton<IVisManager,VisManager>();
            return Services;
        }
        public static IVisManager SetBeepReference(this IVisManager ViewManager, IBeepService beepService)
        {
            beepService.vis = ViewManager;
            ViewManager.DMEEditor = beepService.DMEEditor;
            return ViewManager;
        }
        public static IVisManager SetMainDisplay(this IVisManager ViewManager, string mainform, string title, string iconname, string homePage = null, string homePageDescription = null, string logourl = null)
        {
            ViewManager.DMEEditor.ConfigEditor.Config.SystemEntryFormName = mainform;
            ViewManager.Title = title;
            ViewManager.IconUrl = iconname;
            ViewManager.LogoUrl = logourl;
            ViewManager.HomePageName = homePage;
            ViewManager.HomePageDescription = homePageDescription;
            return ViewManager;
        }
        public static IVisManager LoadAssemblies(this IVisManager ViewManager, IBeepService beepService, Progress<PassedArgs> progress)
        {
            // Create A parameter object for Wait Form

            // Load Assemblies
            beepService.LoadAssemblies(progress);
            beepService.Config_editor.LoadedAssemblies = beepService.LLoader.Assemblies.Select(c => c.DllLib).ToList();

            return ViewManager;
        }


    }
}
