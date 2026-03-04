# NuGet / Nugget Manager UI Enhancement Plan

**Date:** 2026-03-03  
**Location:** `TheTechIdea.Beep.Winform.Default.Views/NuggetsManage/`  
**Goal:** Replace the current single-panel `uc_NuggetsManage.cs` with a full-featured,  
tabbed NuGet package manager that can **search nuget.org**, **download**, **install**,  
**load/unload**, and **manage sources** — all using Beep controls only.  
Learned from: `BeepShell/Commands/NuGetShellCommands.cs` +  
`BeepShell/Infrastructure/NuGetPackageService.cs`.

---

## 1. Current State

| File | Role | Gap |
|------|------|-----|
| `uc_NuggetsManage.cs` | Basic scan/load/unload, file-system only | No nuget.org search, no download, no source management, no version picker, flat list |
| `NuggetsManageService.cs` | InstallFromPath, LoadNugget, UnloadNugget, ScanNuggets | No HTTP search, no download, no version query |
| `NuggetsManageModels.cs` | `NuggetItemState`, `NuggetOperationResult` | No `NuGetSearchResult`, no `NuGetSource` model |
| `NuggetStateStore.cs` | JSON persist/restore | No source list persistence |
| `NuggetManager.cs` (BeepDM) | Low-level .nupkg/DLL loader | Already complete — just wrap it better |

---

## 2. Target Architecture — 4-Tab Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│  [Installed] [Search & Download] [Sources] [Log]                    │
├─────────────────────────────────────────────────────────────────────┤
│  Tab 1: Installed (current uc_NuggetsManage, upgraded)              │
│  Tab 2: Search & Download (NEW — nuget.org + local)                 │
│  Tab 3: Sources  (NEW — manage feed URLs + local paths)             │
│  Tab 4: Log      (NEW — full operation log with severity icons)     │
└─────────────────────────────────────────────────────────────────────┘
```

The overall control becomes `uc_NuggetsManage` (same name, same AddinAttribute)  
with an internal `BeepTabControl` driving the four views.  
A shared `NuggetsManageService` (expanded) owns all async back-end calls.

---

## 3. New / Changed Files

| File | Change |
|------|--------|
| `uc_NuggetsManage.cs` | **Rewritten** — add `BeepTabControl`, split into 4 inner panels |
| `uc_NuggetsManage.Designer.cs` | **New** — designer for the tab shell |
| `uc_NuggetsManage_Installed.cs` | **New partial** — Tab 1 logic |
| `uc_NuggetsManage_Search.cs` | **New partial** — Tab 2 logic |
| `uc_NuggetsManage_Sources.cs` | **New partial** — Tab 3 logic |
| `uc_NuggetsManage_Log.cs` | **New partial** — Tab 4 logic |
| `NuggetsManageModels.cs` | **Extended** — add `NuGetSearchResult`, `NuGetSourceEntry`, `NuGetVersionInfo` |
| `NuggetsManageService.cs` | **Extended** — add `SearchAsync`, `GetVersionsAsync`, `DownloadAndInstallAsync` |
| `NuggetStateStore.cs` | **Extended** — also persist `NuGetSourceEntry` list |

---

## 4. Tab 1 — Installed Packages (upgraded current UI)

### Layout

```
┌── Scan Path ──────────────────────────────────────── [Browse] [Scan] ─┐
│ [Filter textbox] ─────────────────────── [Load] [Unload] [Remove] [↺] │
├────────────────────────────┬──────────────────────────────────────────┤
│  BeepSimpleGrid (list)     │  Detail card                             │
│  ✓ Name | Ver | State      │  Name:       SQLiteDriverCore            │
│  ✓ SQLiteDriverCore  1.3   │  Version:    1.3.0                       │
│  ✗ MySqlDriver       2.1   │  Path:       C:\...\ConnectionDrivers\   │
│    OracleDriver  missing   │  Loaded:     Yes                         │
│                            │  Startup:    Enabled  [toggle]           │
│                            │  Source:     nuget.org                   │
│                            │  Last Upd:   2026-03-01                  │
│                            │  [Load] [Unload] [Remove] [Update]       │
└────────────────────────────┴──────────────────────────────────────────┘
│ Status: 3 loaded  1 missing    [Install from file...]                  │
```

### New vs Old
| Feature | Old | New |
|---------|-----|-----|
| List control | `BeepListBox` (text only) | `BeepSimpleGrid` with ✓/✗ icon, name, version, state columns |
| Filter | none | `BeepTextBox` live filter |
| Detail pane | `BeepTextBox` (unformatted) | Structured label card |
| Startup toggle | `BeepCheckBoxBool` in list | Toggle in detail pane (per-item) |
| Remove | none | Remove button (deletes file + state) |
| Update | none | "Check for update" per item (queries source for newer version) |

### Controls (Beep)
- `BeepTextBox txtFilter` — live filter
- `BeepSimpleGrid nuggetGrid` — columns: Selected(bool), Name, Version, State (Loaded/Missing/Unloaded), Source
- `BeepLabel` × 6 — detail card fields
- `BeepCheckBoxBool chkEnableAtStartup`
- `BeepButton btnLoad, btnUnload, btnRemove, btnInstallFile, btnRefresh, btnCheckUpdate`
- `BeepLabel lblStatus`

---

## 5. Tab 2 — Search & Download

### Layout

```
┌── Source: [cmbSource ▼]  Search: [txtSearch ────────] [Search] ──────┐
├────────────────────────────┬──────────────────────────────────────────┤
│  BeepSimpleGrid results    │  Package Detail (right panel)            │
│  Name | Downloads | Desc   │  ID:          TheTechIdea.Beep.SQL       │
│  ─────────────────────     │  Latest:      3.2.1                      │
│  TheTechIdea.Beep.SQL 2.1M │  Downloads:   2,100,000                  │
│  Beep.MySql          890K  │  Description: Beep MySQL driver…         │
│  Beep.Oracle         450K  │  Authors:     TheTechIdea                │
│                            │  Versions: [cmbVersion ▼] ─ [Download ▼]│
│                            │  ┌─ Download options ──────────────────┐ │
│                            │  │ ○ Install & Load now                │ │
│                            │  │ ○ Download only (save to folder)    │ │
│                            │  └─────────────────────────────────────┘ │
│                            │  [Download + Install]                    │
├────────────────────────────┴──────────────────────────────────────────┤
│ Progress: [BeepProgressBar ──────────────────] 63%  "Downloading…"    │
└────────────────────────────────────────────────────────────────────────┘
```

### Behaviour
1. **Source** combo lists all sources from Tab 3 (nuget.org is always first).
2. **Search** calls `NuGetPackageService.SearchAsync(term, sourceUrl)` → HTTP GET  
   `https://azuresearch-usnc.nuget.org/query?q=…&take=25`  
   (same URL used in BeepShell).
3. Selecting a result populates the detail panel and fires `GetVersionsAsync` to fill `cmbVersion`.
4. **Download + Install** calls `NuGetPackageService.DownloadAndInstallAsync`:
   - Downloads `.nupkg` to `ConnectionDrivers/InstalledNuggets/`
   - Extracts DLLs for current TFM (same logic as `NuggetManager.ExtractBestFrameworkDlls`)
   - Calls `NuggetManager.LoadNugget(extractedPath)`
   - Appends to `_states` and saves via `NuggetStateStore`
5. Progress bar updates via `IProgress<(int Percent, string Message)>`.
6. **"Download only"** skips loading — saves `.nupkg` to folder the user picks via `FolderBrowserDialog`.

### Controls (Beep)
- `BeepComboBox cmbSource` — feed selector
- `BeepTextBox txtSearch` — search box
- `BeepButton btnSearch`
- `BeepSimpleGrid searchGrid` — columns: Name, Version, Downloads, Description (truncated)
- `BeepLabel` × 5 — package detail card
- `BeepComboBox cmbVersion` — version selector
- `BeepRadioButtonList` or two `BeepRadioButton` — install-now vs download-only
- `BeepButton btnDownloadInstall`
- `BeepProgressBar progressBar`
- `BeepLabel lblProgressStatus`

---

## 6. Tab 3 — Sources

### Layout

```
┌──────────────────────────────────────────────────────────────────────┐
│  BeepSimpleGrid (sources)                    [Add] [Edit] [Remove]   │
│  ✓ Name             | Type | URL/Path         | Enabled              │
│  ✓ nuget.org        | HTTP | https://api…     | Yes                  │
│  ✓ Local Drivers    | Dir  | C:\Drivers\      | Yes                  │
│  ✗ Internal Feed    | HTTP | https://pkgs…    | No                   │
│                                                                       │
│  ── Edit / Add ──────────────────────────────────────────────────── │
│  Name:    [BeepTextBox ─────────────────────]                        │
│  Type:    [BeepComboBox: NuGet HTTP | Local Directory]               │
│  URL/Path:[BeepTextBox ───────────────────────────] [Browse]         │
│  Enabled: [BeepCheckBoxBool]                                         │
│           [Test Connection]  [Save]  [Cancel]                        │
└──────────────────────────────────────────────────────────────────────┘
```

### Behaviour
- **Test Connection**: async `HttpClient.GetAsync(url/v3/index.json)` for HTTP sources;  
  `Directory.Exists` for local path sources. Shows ✓/✗ in a `BeepLabel`.
- Sources serialized to `Config/NuGetSources.json` via `NuggetStateStore` (extended).
- Default "nuget.org" source added on first run if list is empty.
- Sources populate `cmbSource` in Tab 2.

### Controls (Beep)
- `BeepSimpleGrid sourcesGrid`
- `BeepButton btnAddSource, btnEditSource, btnRemoveSource`
- `BeepTextBox txtSourceName, txtSourceUrl`
- `BeepComboBox cmbSourceType` — "NuGet HTTP", "Local Directory"
- `BeepCheckBoxBool chkSourceEnabled`
- `BeepButton btnTestSource, btnSaveSource, btnCancelSource`
- `BeepLabel lblTestResult`

---

## 7. Tab 4 — Log

```
┌──────────────────────────────────────────────────────┐
│  [All ▼] [Clear] [Copy to Clipboard] [Export…]       │
│  ┌────────────────────────────────────────────────┐  │
│  │ 10:42:01  ℹ  Scan completed. Found 4 nuggets. │  │
│  │ 10:42:03  ✓  Loaded SQLiteDriverCore           │  │
│  │ 10:43:11  ⚠  MySqlDriver: path not found       │  │
│  │ 10:44:00  ✗  Download failed: timeout          │  │
│  └────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────┘
```

- `BeepComboBox cmbLogFilter` — All / Info / Warn / Error
- `BeepButton btnClearLog, btnCopyLog, btnExportLog`
- `RichTextBox` or `BeepSimpleGrid` — log entries with colour coding:
  - Info → default text
  - Warn → orange
  - Error → red

---

## 8. New Model Classes (`NuggetsManageModels.cs` additions)

```csharp
internal sealed class NuGetSearchResult
{
    public string Id          { get; set; } = string.Empty;
    public string Version     { get; set; } = string.Empty;   // latest
    public string Description { get; set; } = string.Empty;
    public string Authors     { get; set; } = string.Empty;
    public long   Downloads   { get; set; }
}

internal sealed class NuGetVersionInfo
{
    public string Version   { get; set; } = string.Empty;
    public bool   IsLatest  { get; set; }
    public bool   IsStable  { get; set; }
}

internal sealed class NuGetSourceEntry
{
    public string  Name      { get; set; } = string.Empty;
    public string  SourceUrl { get; set; } = string.Empty;
    public string  Type      { get; set; } = "NuGet HTTP";   // or "Local Directory"
    public bool    IsEnabled { get; set; } = true;
    public bool    IsDefault { get; set; }
}

internal sealed class NuggetDownloadOptions
{
    public bool   LoadAfterInstall  { get; set; } = true;
    public string DestinationFolder { get; set; } = string.Empty;
}
```

---

## 9. Service Layer Additions (`NuggetsManageService.cs`)

```csharp
// Search nuget.org (or local source)
Task<List<NuGetSearchResult>> SearchAsync(string term, string sourceUrl, CancellationToken ct);

// Get all versions for a package
Task<List<NuGetVersionInfo>> GetVersionsAsync(string packageId, string sourceUrl, CancellationToken ct);

// Download .nupkg, extract best-TFM DLLs, optionally load
Task<NuggetOperationResult> DownloadAndInstallAsync(
    string packageId, string version, string sourceUrl,
    NuggetDownloadOptions options, IProgress<(int Percent, string Message)> progress,
    CancellationToken ct);

// Test a source URL or directory
Task<NuggetOperationResult> TestSourceAsync(NuGetSourceEntry source, CancellationToken ct);

// CRUD for sources
List<NuGetSourceEntry> LoadSources();
void SaveSources(IEnumerable<NuGetSourceEntry> sources);
```

Port the HTTP search logic from `NuGetPackageService.SearchAndSelectPackageAsync` and  
`SelectPackageVersionAsync` in BeepShell — strip the Spectre.Console UI layer,  
return plain `List<T>` so the WinForms control drives the UI.

---

## 10. Implementation Phases

### Phase 1 — Models + Service (no UI)
1. Extend `NuggetsManageModels.cs` with 4 new model classes
2. Extend `NuggetStateStore.cs` to persist `NuGetSourceEntry` list  
   (`Config/NuGetSources.json`, default nuget.org entry on first run)
3. Add `SearchAsync`, `GetVersionsAsync`, `DownloadAndInstallAsync`, `TestSourceAsync`,  
   `LoadSources`, `SaveSources` to `NuggetsManageService.cs`  
   (port HTTP logic from BeepShell, remove Spectre dependency)

### Phase 2 — Tab 1: Installed (upgrade existing)
4. Swap `BeepListBox` → `BeepSimpleGrid` with icon/color state column
5. Add `BeepTextBox` live filter
6. Add structured detail card (labels, not textbox)
7. Add `btnRemove` (delete file + state) and `btnCheckUpdate`
8. Move `BeepTabControl` shell into `uc_NuggetsManage.cs` with Tab 1 as first tab

### Phase 3 — Tab 2: Search & Download
9. Build Tab 2 panel with `BeepSimpleGrid`, detail card, `cmbVersion`, progress bar
10. Wire `btnSearch` → `SearchAsync` → populate grid
11. Wire grid selection → `GetVersionsAsync` → populate `cmbVersion`
12. Wire `btnDownloadInstall` → `DownloadAndInstallAsync` with progress bar updates
13. Disable controls during async operations; re-enable on completion

### Phase 4 — Tab 3: Sources
14. Build Tab 3 panel with `BeepSimpleGrid` + add/edit/remove + test
15. Default "nuget.org" source on first run
16. Persist/restore via `NuggetStateStore` extension
17. Populate `cmbSource` in Tab 2 from sources list

### Phase 5 — Tab 4: Log
18. Move all `AppendLog` calls to a shared `LogBus` (simple `List<NuggetOperationLogEntry>`)
19. Build Tab 4 with filter combo, copy/export buttons
20. Colour-code log rows (Info/Warn/Error)

---

## 11. NuGet Download Logic (ported from BeepShell)

```
SearchAsync(term, source)
  → HTTP GET azuresearch-usnc.nuget.org/query?q={term}&take=25
  → parse JSON data[] → return List<NuGetSearchResult>

GetVersionsAsync(id, source)
  → NuGet SDK:  Repository.Factory.GetCoreV3(source)
               .GetResourceAsync<FindPackageByIdResource>()
               .GetAllVersionsAsync(id)
  → sort descending, return List<NuGetVersionInfo>

DownloadAndInstallAsync(id, version, source, options, progress)
  → NuGet SDK:  FindPackageByIdResource.CopyNupkgToStreamAsync(id, version, stream)
  → save .nupkg to  ConnectionDrivers/InstalledNuggets/{id}.{version}.nupkg
  → ZipFile.OpenRead → find lib/{tfm}/*.dll (prefer net8.0, netstandard2.0, etc.)
  → copy best DLLs to  ConnectionDrivers/InstalledNuggets/{id}/
  → NuggetManager.LoadNugget(extractedPath)
  → upsert NuggetItemState → SaveStates
```

NuGet SDK packages needed in project:
```xml
<PackageReference Include="NuGet.Protocol" Version="6.*" />
<PackageReference Include="NuGet.Common"   Version="6.*" />
```

---

## 12. UX Principles

| Principle | Implementation |
|-----------|----------------|
| Progressive disclosure | Tabs hide complexity; Installed tab is first and simplest |
| Visibility of status | Progress bar + status label on every async operation |
| Error recovery | All async paths wrapped in try/catch; errors appear in Log tab AND status label |
| Prevent mistakes | "Remove" button asks for confirmation via `BeepDialogBox` |
| Efficiency for repeat users | Source combo remembers last-used; search box remembers last 5 terms (stored in state) |
| Consistency with framework | All controls are Beep controls; no raw WinForms except `SplitContainer` layout panels |
