using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace TheTechIdea.Beep.Winform.Controls.FontManagement
{
    /// <summary>
    /// Central registry for multi-assembly font discovery in Beep controls.
    /// Allows any consuming project, plugin, or theme pack to register its font-bearing assemblies
    /// and namespace roots SO that <see cref="FontListHelper"/> and <see cref="BeepFontPaths"/>
    /// include those fonts in all look-ups without requiring changes to the Controls library.
    ///
    /// <para><b>Convention-based auto-discovery</b>: Any assembly that has at least one embedded
    /// resource whose namespace prefix ends with <c>.Fonts</c> (e.g.
    /// <c>MyApp.Fonts.Roboto-Regular.ttf</c>) will be auto-included when
    /// <see cref="RegisterFromAppDomain"/> is called.</para>
    ///
    /// <para><b>Explicit registration</b>: Call
    /// <see cref="Register(System.Reflection.Assembly)"/> or
    /// <see cref="Register(string)"/> at application startup before showing any Beep controls.</para>
    ///
    /// <para>The easiest integration point is via <see cref="BeepFontManager.Register(Assembly)"/>
    /// which registers AND triggers an incremental scan in one call.</para>
    /// </summary>
    public static class BeepFontRegistry
    {
        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private static readonly HashSet<Assembly> _registeredAssemblies = new HashSet<Assembly>();
        private static readonly HashSet<string> _registeredNamespaces = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> _registeredDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Optional override for <see cref="BeepFontPaths.ResourceAssembly"/>.
        /// When set, <see cref="BeepFontPaths.GetAvailableFontResources"/> queries this assembly
        /// first (in addition to all other registered assemblies).
        /// Typical use: a theme-pack project that ships its own font constants and embedded fonts.
        /// </summary>
        public static Assembly PrimaryAssembly { get; set; }

        /// <summary>
        /// Fires whenever new assemblies, namespaces, or directories are added to the registry.
        /// <see cref="BeepFontManager"/> subscribes to this after <c>Initialize()</c> to trigger
        /// incremental loads for late registrations.
        /// </summary>
        public static event EventHandler Changed;

        // ───────────────────────────────────────────────────────────────────
        #region Register — Assemblies

        /// <summary>
        /// Registers an assembly for font scanning.
        /// The assembly's manifest resources are inspected and any namespace prefix found on a
        /// <c>.ttf</c> / <c>.otf</c> resource is also registered as a namespace root.
        /// </summary>
        /// <param name="assembly">Assembly to register. Null is silently ignored.</param>
        public static void Register(Assembly assembly)
        {
            if (assembly == null || assembly.IsDynamic) return;

            bool changed = false;
            _lock.EnterWriteLock();
            try
            {
                if (_registeredAssemblies.Add(assembly))
                {
                    changed = true;
                    // Auto-infer namespace roots from font resources in this assembly
                    InferNamespacesFromAssembly(assembly);
                }
            }
            finally { _lock.ExitWriteLock(); }

            if (changed) RaiseChanged();
        }

        // ───────────────────────────────────────────────────────────────────
        /// <summary>
        /// Registers multiple assemblies at once. Null entries are silently skipped.
        /// </summary>
        public static void Register(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null) return;
            bool changed = false;
            _lock.EnterWriteLock();
            try
            {
                foreach (var asm in assemblies)
                {
                    if (asm == null || asm.IsDynamic) continue;
                    if (_registeredAssemblies.Add(asm))
                    {
                        changed = true;
                        InferNamespacesFromAssembly(asm);
                    }
                }
            }
            finally { _lock.ExitWriteLock(); }

            if (changed) RaiseChanged();
        }

        #endregion

        // ───────────────────────────────────────────────────────────────────
        #region Register — Namespaces

        /// <summary>
        /// Registers a namespace prefix so that any embedded resource containing this string is
        /// treated as a font resource during scanning (regardless of which assembly it lives in).
        /// Example: <c>"MyCompany.MyTheme.Fonts"</c>
        /// </summary>
        /// <param name="namespacePrefix">Namespace root. Null / empty is silently ignored.</param>
        public static void Register(string namespacePrefix)
        {
            if (string.IsNullOrWhiteSpace(namespacePrefix)) return;

            bool changed = false;
            _lock.EnterWriteLock();
            try { changed = _registeredNamespaces.Add(namespacePrefix.Trim()); }
            finally { _lock.ExitWriteLock(); }

            if (changed) RaiseChanged();
        }

        #endregion

        // ───────────────────────────────────────────────────────────────────
        #region Register — File-System Directories

        /// <summary>
        /// Registers a file-system directory that will be scanned for <c>.ttf</c> / <c>.otf</c>
        /// font files. Directory must exist; non-existent paths are silently ignored.
        /// </summary>
        public static void RegisterFromDirectory(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath)) return;

            bool changed = false;
            _lock.EnterWriteLock();
            try { changed = _registeredDirectories.Add(directoryPath.Trim()); }
            finally { _lock.ExitWriteLock(); }

            if (changed) RaiseChanged();
        }

        #endregion

        // ───────────────────────────────────────────────────────────────────
        #region Convention Discovery

        /// <summary>
        /// Walks all assemblies currently loaded in <see cref="AppDomain.CurrentDomain"/> and
        /// registers those that satisfy the convention: at least one embedded resource whose
        /// namespace prefix ends with <c>.Fonts</c> (case-insensitive).
        /// Framework assemblies (System.*, Microsoft.*) are skipped.
        /// </summary>
        public static void RegisterFromAppDomain()
        {
            var candidates = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Where(a =>
                {
                    var fn = a.FullName ?? string.Empty;
                    return !fn.StartsWith("System", StringComparison.OrdinalIgnoreCase) &&
                           !fn.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase);
                })
                .Where(HasConventionFontResource)
                .ToList();

            Register(candidates);
        }

        /// <summary>
        /// Returns <c>true</c> if the assembly contains at least one embedded .ttf/.otf resource
        /// whose namespace prefix ends with <c>.Fonts</c>.
        /// </summary>
        public static bool HasConventionFontResource(Assembly assembly)
        {
            if (assembly == null || assembly.IsDynamic) return false;
            try
            {
                return assembly.GetManifestResourceNames()
                    .Any(r =>
                    {
                        var ext = Path.GetExtension(r)?.ToLowerInvariant();
                        if (ext != ".ttf" && ext != ".otf") return false;
                        // Namespace prefix = everything before the last dot + filename
                        int lastDot = r.LastIndexOf('.');           // extension dot
                        int prevDot = r.LastIndexOf('.', lastDot - 1); // separator before filename
                        if (prevDot < 0) return false;
                        string nsPrefix = r.Substring(0, prevDot);
                        return nsPrefix.EndsWith(".Fonts", StringComparison.OrdinalIgnoreCase)
                            || nsPrefix.EndsWith("Fonts", StringComparison.OrdinalIgnoreCase);
                    });
            }
            catch { return false; }
        }

        #endregion

        // ───────────────────────────────────────────────────────────────────
        #region Read-Only Views

        /// <summary>Returns a snapshot of all registered assemblies (thread-safe).</summary>
        public static IReadOnlyCollection<Assembly> GetRegisteredAssemblies()
        {
            _lock.EnterReadLock();
            try { return _registeredAssemblies.ToList(); }
            finally { _lock.ExitReadLock(); }
        }

        /// <summary>Returns a snapshot of all registered namespace roots (thread-safe).</summary>
        public static IReadOnlyCollection<string> GetRegisteredNamespaces()
        {
            _lock.EnterReadLock();
            try { return _registeredNamespaces.ToList(); }
            finally { _lock.ExitReadLock(); }
        }

        /// <summary>Returns a snapshot of all registered directories (thread-safe).</summary>
        public static IReadOnlyCollection<string> GetRegisteredDirectories()
        {
            _lock.EnterReadLock();
            try { return _registeredDirectories.ToList(); }
            finally { _lock.ExitReadLock(); }
        }

        #endregion

        // ───────────────────────────────────────────────────────────────────
        #region Helpers

        private static void InferNamespacesFromAssembly(Assembly assembly)
        {
            // Called inside write lock — no re-entrant lock needed
            try
            {
                foreach (var res in assembly.GetManifestResourceNames())
                {
                    var ext = Path.GetExtension(res)?.ToLowerInvariant();
                    if (ext != ".ttf" && ext != ".otf") continue;

                    // Namespace prefix = everything up to (but not including) the last "." segment
                    int lastDot = res.LastIndexOf('.');
                    if (lastDot <= 0) continue;
                    int prevDot = res.LastIndexOf('.', lastDot - 1);
                    string nsPrefix = prevDot >= 0 ? res.Substring(0, prevDot) : res.Substring(0, lastDot);
                    if (!string.IsNullOrWhiteSpace(nsPrefix))
                        _registeredNamespaces.Add(nsPrefix);
                }
            }
            catch { /* ignore reflection errors */ }
        }

        private static void RaiseChanged() =>
            Changed?.Invoke(null, EventArgs.Empty);

        #endregion
    }
}
