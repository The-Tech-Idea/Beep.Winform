using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Services;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Extensions
{
    /// <summary>
    /// Composition for Beep WinForms: wires add-in command handlers and mirrors them into <see cref="SimpleItemFactory"/>.
    /// Call after <c>BeepDesktopServices.ConfigureServices(host)</c>. Keeps <c>TheTechIdea.Beep.Winform.Controls</c> free of desktop/add-in references.
    /// </summary>
    public static class BeepWinformUiBootstrap
    {
        /// <summary>Resolves <see cref="IBeepService"/> and <see cref="IAppManager"/> from the host and applies WinForms add-in UI wiring.</summary>
        public static void ConfigureBeepWinformAddInUi(this IHost host)
        {
            ArgumentNullException.ThrowIfNull(host);
            var beep = host.Services.GetRequiredService<IBeepService>();
            var app = host.Services.GetRequiredService<IAppManager>();
            ConfigureBeepWinformAddInUi(beep, app);
        }

        /// <summary>Runs <see cref="AddInCommandPipeline.Apply"/> and syncs <see cref="SimpleItemFactory"/> delegates for Beep WinForms controls.</summary>
        public static void ConfigureBeepWinformAddInUi(IBeepService beepService, IAppManager appManager)
        {
            ArgumentNullException.ThrowIfNull(beepService);
            ArgumentNullException.ThrowIfNull(appManager);

            AddInCommandPipeline.Apply(beepService, appManager);

            SimpleItemFactory.SetDelegates(
                HandlersFactory.GlobalMenuItemsProvider,
                HandlersFactory.RunFunctionHandler,
                HandlersFactory.RunFunctionWithTreeHandler,
                HandlersFactory.RunMethodFromObjectHandler,
                HandlersFactory.RunMethodFromExtensionWithTreeHandler,
                HandlersFactory.RunMethodFromExtensionHandler);
        }
    }
}
