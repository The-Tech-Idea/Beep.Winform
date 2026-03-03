// BeepDocumentHostExtensions.cs
// Fluent configuration helper for BeepDocumentHost + optional Microsoft.Extensions.DI
// registration pattern (Sprint 16.3).
//
// DI usage (app startup):
//
//   services.AddTransient<BeepDocumentHost>(sp =>
//       new BeepDocumentHost().Configure(options =>
//       {
//           options.DefaultTabStyle          = DocumentTabStyle.VSCode;
//           options.MaxGroups                = 3;
//           options.KeyboardShortcutsEnabled = true;
//       }));
//
//   // Or a custom extension in YOUR app:
//   public static IServiceCollection AddBeepDocumentHost(
//       this IServiceCollection services,
//       Action<BeepDocumentHostOptions> configure)
//   {
//       services.AddTransient<BeepDocumentHost>(_ =>
//           new BeepDocumentHost().Configure(configure));
//       services.AddTransient<IDocumentHostCommandService>(
//           sp => sp.GetRequiredService<BeepDocumentHost>().CommandService);
//       return services;
//   }
//
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Extension methods for <see cref="BeepDocumentHost"/> that apply
    /// <see cref="BeepDocumentHostOptions"/> in a fluent style.
    /// </summary>
    public static class BeepDocumentHostExtensions
    {
        /// <summary>
        /// Configures the <paramref name="host"/> using the provided
        /// <paramref name="configure"/> action and returns the same host (fluent).
        /// </summary>
        /// <example>
        /// <code>
        /// var host = new BeepDocumentHost().Configure(o =>
        /// {
        ///     o.DefaultTabStyle          = DocumentTabStyle.VSCode;
        ///     o.MaxGroups                = 3;
        ///     o.KeyboardShortcutsEnabled = true;
        /// });
        /// </code>
        /// </example>
        public static BeepDocumentHost Configure(
            this BeepDocumentHost host,
            Action<BeepDocumentHostOptions> configure)
        {
            if (host == null)      throw new ArgumentNullException(nameof(host));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = new BeepDocumentHostOptions();
            configure(options);
            options.ApplyTo(host);
            return host;
        }

        /// <summary>
        /// Applies a fully-populated <see cref="BeepDocumentHostOptions"/> instance
        /// to <paramref name="host"/>.
        /// </summary>
        public static BeepDocumentHost ApplyOptions(
            this BeepDocumentHost host,
            BeepDocumentHostOptions options)
        {
            if (host == null)    throw new ArgumentNullException(nameof(host));
            if (options == null) throw new ArgumentNullException(nameof(options));
            options.ApplyTo(host);
            return host;
        }
    }
}
