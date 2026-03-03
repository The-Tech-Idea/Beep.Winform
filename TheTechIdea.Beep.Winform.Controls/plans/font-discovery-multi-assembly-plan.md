# Font Discovery — Multi-Assembly & Referenced Project Support

> **Created**: 2026-03-02  
> **Scope**: `FontManagement/BeepFontPaths.Core.cs`, `FontManagement/FontListHelper.cs`, `FontManagement/BeepFontManager.cs`  
> **Problem**: Font discovery is locked to `Assembly.GetExecutingAssembly()` and two hard-coded namespace roots. Consuming projects and plugin assemblies that embed fonts under their own namespaces are silently excluded, causing null fonts and invisible text.

---

## Root Causes

| # | File | Line | Issue |
|---|---|---|---|
| 1 | `BeepFontPaths.Core.cs` L19 | 19 | `ResourceAssembly` returns only `Assembly.GetExecutingAssembly()`. `GetAvailableFontResources()` and `LoadFontsFromBeepFontPaths()` only see the Controls assembly. |
| 2 | `BeepFontManager.Initialize()` ~L110 | ~110 | `EmbeddedNamespaces` is a compile-time constant array — no API to add namespaces at runtime. |
| 3 | `FontListHelper.GetFontResourcesFromEmbedded()` L343 | L343 | Although AppDomain + BFS-depth-2 referenced assemblies are scanned, the namespace filter excludes everything not matching the two hard-coded roots. |
| 4 | (all) | — | No public hook for an external assembly to say "include my fonts" at application startup. |
| 5 | `BeepFontPaths.Extensions.cs` `GetAllFamilies()` | — | Hard-coded to Controls-assembly paths only; family look-up always misses third-party fonts. |
| 6 | `BeepFontManager.GetOrCreateFont` (private) | ~L162 | Final `catch { return null; }` — can return null, propagating invisible-text bug silently. |

---

## Goals

1. **Zero breaking changes** — `GetFont`, `ToFont`, `IsFontAvailable`, `Initialize()` all work unchanged.
2. **Convention-based auto-discovery** — any assembly embedding fonts under a namespace ending in `.Fonts` is automatically included.
3. **Explicit registration API** — any external assembly (plugin, theme pack, app) can call `BeepFontManager.Register(assembly)` at startup.
4. **Incremental load** — registering after init triggers a lightweight per-assembly scan, not a full reload.
5. **Null-safe throughout** — `ToFont` and all `GetFont` paths never return null (failsafe font chain).

---

## Design

### A. New `FontManagement/BeepFontRegistry.cs`

New static class. Responsibilities:
- `_registeredAssemblies` (`HashSet<Assembly>`) and `_registeredNamespaces` (`HashSet<string>`) guarded by `ReaderWriterLockSlim`
- `PrimaryAssembly` property — optionally overrides what `BeepFontPaths.ResourceAssembly` returns
- `Register(Assembly)` — add assembly + auto-infer namespace from manifest resources (prefix of any `.ttf`/`.otf` entry)
- `Register(string namespacePrefix)` — explicit namespace root addition
- `RegisterFromAppDomain()` — walks `AppDomain.CurrentDomain.GetAssemblies()`, applies convention filter (`*.Fonts` suffix)
- `RegisterFromDirectory(string dir)` — registers a file-system path for `.ttf`/`.otf` scanning
- `GetRegisteredAssemblies()` / `GetRegisteredNamespaces()` — read-only views
- `Changed` event (`EventHandler`) — fires when new assemblies or namespaces are added

**Convention rule**: a resource is auto-included if its namespace prefix ends with `.Fonts` — opt-in by naming convention, no config required.

### B. Update `BeepFontPaths.Core.cs`

- `ResourceAssembly` property: check `BeepFontRegistry.PrimaryAssembly` first, fall back to `Assembly.GetExecutingAssembly()`
- `GetAvailableFontResources()`: iterate `BeepFontRegistry.GetRegisteredAssemblies()` + `ResourceAssembly`, collect matching `.ttf`/`.otf` resources from each, deduplicate, return merged array

### C. Update `BeepFontPaths.Extensions.cs` — Add `RegisterFamilyFromAssembly`

New method `RegisterFamilyFromAssembly(Assembly asm, string namespaceRoot)`:
- Reflects over `asm.GetManifestResourceNames()` matching `namespaceRoot + ".*.(ttf|otf)"`
- Extracts family name using `ExtractFontNameFromPath`
- Extracts style string from filename (Regular / Bold / Italic / BoldItalic)
- Inserts into runtime `_runtimeFamilies` dictionary that `GetAllFamilies()` merges

### D. Extend `FontScanOptions` in `FontListHelper.cs`

New fields (all backward-compatible with safe defaults):

| New Field | Type | Default | Purpose |
|---|---|---|---|
| `AdditionalAssemblies` | `IList<Assembly>` | `null` | Extra assemblies beyond AppDomain BFS |
| `AdditionalNamespaces` | `IList<string>` | `null` | Extra namespace roots beyond `EmbeddedNamespaces` |
| `UseConventionDiscovery` | `bool` | `true` | Auto-include assemblies with a `*.Fonts` resource namespace |

In `GetFontResourcesFromEmbedded(EmbeddedScanOptions)`:
1. Add `BeepFontRegistry.GetRegisteredAssemblies()` to the working assembly set.
2. Build effective namespace filter = `EmbeddedNamespaces` ∪ `BeepFontRegistry.GetRegisteredNamespaces()`.
3. If convention discovery enabled: any assembly with a `.ttf`/`.otf` resource whose namespace prefix ends with `.Fonts` → unconditionally include.

New public method `RegisterAndLoad(Assembly assembly)` — scoped scan of a single assembly, appends new `FontConfiguration` entries, evicts only related cache keys (prefix `"{fontName}|"`).

New public `EmbeddedScanOptions` fields mirroring the above.

### E. Update `BeepFontManager.cs`

1. Add `static void Register(Assembly assembly)` — delegates to `BeepFontRegistry.Register(assembly)` + `FontListHelper.RegisterAndLoad(assembly)`.
2. Add `static void Register(string namespacePrefix)` — delegates to `BeepFontRegistry.Register(namespacePrefix)`, triggers re-scan of already-loaded assemblies for the new namespace.
3. In `Initialize()`: merge `BeepFontRegistry.GetRegisteredNamespaces()` and `BeepFontRegistry.GetRegisteredAssemblies()` into `FontScanOptions`.
4. Subscribe to `BeepFontRegistry.Changed` at end of `Initialize()` for post-init registrations.

### F. Null-Safe Failsafe (prerequisite)

- `BeepFontManager.GetOrCreateFont` (private): final `catch { return null; }` → `catch { return SystemFonts.DefaultFont; }`
- `BeepFontManager.ToFont(TypographyStyle)`: add `fontSize <= 0` guard + `?? DefaultFont`
- `BeepThemesManager.ToFont(TypographyStyle, bool)`: add `if (fontSize <= 0) fontSize = 9.0f` + `?? BeepFontManager.DefaultFont`
- `BeepThemesManager.ToFontForControl`: same
- `BeepThemesManager.ToFont(string, float, FontWeight, FontStyle)`: same

---

## Implementation Steps

| # | File | Change |
|---|---|---|
| 1 | `BeepFontManager.cs` ~L162 | `GetOrCreateFont` final catch → `SystemFonts.DefaultFont` |
| 2 | `BeepFontManager.cs` ~L327 | `ToFont(TypographyStyle)` → guard + `?? DefaultFont` |
| 3 | `BeepThemesManager.cs` ~L452 | `ToFont(TypographyStyle, bool)` → fontSize guard + null-coalesce |
| 4 | `BeepThemesManager.cs` ~L485 | `ToFontForControl` → fontSize guard + null-coalesce |
| 5 | `BeepThemesManager.cs` ~L532 | `ToFont(string,float,FontWeight,FontStyle)` → guard + null-coalesce |
| 6 | **NEW** `BeepFontRegistry.cs` | Create full registration static class |
| 7 | `FontListHelper.cs` — `FontScanOptions` | Add `AdditionalAssemblies`, `AdditionalNamespaces`, `UseConventionDiscovery` |
| 8 | `FontListHelper.cs` — `EmbeddedScanOptions` | Add `AdditionalAssemblies`, `AdditionalNamespaces`, `UseConventionDiscovery` |
| 9 | `FontListHelper.cs` — `GetFontResourcesFromEmbedded` | Merge registry assemblies + namespaces; add convention branch |
| 10 | `FontListHelper.cs` | Add `RegisterAndLoad(Assembly)` public method |
| 11 | `BeepFontPaths.Core.cs` | `ResourceAssembly` check registry; `GetAvailableFontResources()` iterate all registered assemblies |
| 12 | `BeepFontPaths.Extensions.cs` | Add `_runtimeFamilies` dict + `RegisterFamilyFromAssembly`; merge into `GetAllFamilies()` |
| 13 | `BeepFontManager.cs` | Add `Register(Assembly)`, `Register(string)` overloads; update `Initialize()` |
| 14 | **NEW** `FontManagement/Readme.md` | Document registration API for consuming projects |

---

## Verification

- Embed a test font as `MyTestApp.Fonts.TestFont-Regular.ttf` in any assembly → call `BeepFontManager.Register(testAssembly)` → assert `IsFontAvailable("TestFont")` = `true` and `GetFont("TestFont", 12f, FontStyle.Regular)` is non-null.
- Set `ControlStyle = iOS15` on `BeepButton` → verify text renders (covers null-font + SF Pro Text fallback to Arial).
- Verify `Initialize()` without manual registration still works identically (regression).
- Verify convention discovery: embed font at namespace `SomePlugin.Fonts.X.ttf` in any loaded assembly → auto-discovered without `Register()`.

---

## Decisions

- **Convention (`*.Fonts`) over full-scan**: avoids penalizing startup by scanning all loaded assemblies blindly.
- **Incremental `RegisterAndLoad`**: new assemblies append to `FontConfigurations` and evict only related cache entries — no full reload.
- **`BeepFontRegistry` as static**: font loading is a pre-UI, pre-DI concern that must work before any IoC container is available.
- **`PrimaryAssembly`**: consuming projects can redirect `BeepFontPaths.ResourceAssembly` (e.g., a theme-pack assembly that ships its own font constants), replacing the Controls assembly as the primary source.
