---
name: forms-performance-configuration
description: Detailed guidance for FormsManager performance, paging, audit, security, and configuration in BeepDM. Use when tuning block caching/metrics, configuring paging and lazy loading, setting up audit trail capture, applying block/field security rules, or managing form and environment configuration defaults.
---

# Forms Performance, Audit, Security, and Configuration

Use this skill for performance-sensitive forms, runtime observability, access control, and environment-level defaults (Phases 5–8).

## File Locations

### Performance and paging
- `DataManagementEngineStandard/Editor/Forms/Helpers/PerformanceManager.cs`
- `DataManagementEngineStandard/Editor/Forms/FormsManager.Performance.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/PagingManager.cs`

### Audit trail
- `DataManagementEngineStandard/Editor/Forms/FormsManager.Audit.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/AuditManager.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/InMemoryAuditStore.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/FileAuditStore.cs`

### Security
- `DataManagementEngineStandard/Editor/Forms/FormsManager.Security.cs`
- `DataManagementEngineStandard/Editor/Forms/Helpers/SecurityManager.cs`

### Configuration
- `DataManagementEngineStandard/Editor/Forms/Configuration/ConfigurationManager.cs`
- `DataManagementEngineStandard/Editor/Forms/Configuration/UnitofWorksManagerConfiguration.cs`
- `DataManagementEngineStandard/Editor/Forms/Configuration/FormConfiguration.cs`
- `DataManagementEngineStandard/Editor/Forms/Configuration/NavigationConfiguration.cs`
- `DataManagementEngineStandard/Editor/Forms/Configuration/PerformanceConfiguration.cs`
- `DataManagementEngineStandard/Editor/Forms/Configuration/ValidationConfiguration.cs`

## Performance and Caching (PerformanceManager)
- `PreloadFrequentBlocks(blockNames[])` — warm block-info cache before first navigation
- `GetPerformanceStatistics()` → `CacheHits`, `CacheMisses`, `CacheHitRatio`
- `GetCacheEfficiencyMetrics()` → `TopAccessedBlocks`, block-level access frequencies
- `OptimizeBlockAccess()` — apply automated optimizations based on statistics
- `InvalidateCache(blockName)` / `InvalidateAllCaches()` — explicit cache invalidation
- `ClearCacheOnFormClose` config flag — default: `false`; enable only where stale-data risk is high

## Paging (PagingManager via FormsManager.Performance)
- `SetBlockPageSize(blockName, pageSize)` — activate paging; `0` = no paging
- `LoadPageAsync(blockName, pageNumber, ct)` → `PageInfo` — navigates UoW cursor to first record of page
- `GetPageInfo(blockName)` → `PageInfo` with `PageNumber`, `TotalPages`, `PageSize`, `Skip`
- `SetTotalRecordCount(blockName, count)` — required after each query to keep paging state accurate
- Paging is **state-only**: PagingManager does not execute datasource queries; FormsManager moves the UoW cursor

## Audit Trail (FormsManager.Audit + AuditManager)
- `SetAuditUser(userName)` — stamps all subsequent audit entries with the session user
- `ConfigureAudit(action<AuditConfiguration>)` — configure store, enabled fields, flush interval
- `GetAuditLog(blockName?, operation?, from?, to?)` → `IReadOnlyList<AuditEntry>`
- `GetFieldHistory(blockName, recordKey, fieldName)` → `IReadOnlyList<AuditFieldChange>`
- `ExportAuditToCsvAsync(filePath, blockName?)` / `ExportAuditToJsonAsync(...)` — export to file
- `PurgeAuditLog(before?)` — maintenance; purge entries older than date
- Audit capture is driven from `OnBlockFieldChanged`; bypassing FormsManager mutation paths also bypasses audit
- Two stores: `InMemoryAuditStore` (default) and `FileAuditStore` (JSON-backed); inject via `IAuditStore`

## Security (FormsManager.Security + SecurityManager)
- `SetSecurityContext(SecurityContext)` — set active user + roles; immediately re-applies all security flags
- `SecurityContext` property — current context
- `SetBlockSecurity(blockName, BlockSecurity)` — register or replace block-level rules
- `GetBlockSecurity(blockName)` → `BlockSecurity`
- `IsBlockAllowed(blockName, SecurityPermission)` → bool — runtime CRUD permission check
- `SetFieldSecurity(blockName, fieldName, FieldSecurity)` — field visibility, editability, masking
- `GetFieldSecurity(blockName, fieldName)` → `FieldSecurity`
- `GetMaskedValue(blockName, fieldName, value)` → masked string for display
- Security pushes effective flags into `DataBlockInfo` and `ItemPropertyManager`; UI-layer disabling is supplemental, not the enforcement boundary
- Violations are logged via `ErrorLog` through `SecurityManager.OnSecurityViolation`

## Configuration (ConfigurationManager)
- `LoadConfiguration()` / `SaveConfiguration()` / `ResetToDefaults()` / `ValidateConfiguration()`
- `Configuration` property → `UnitofWorksManagerConfiguration`
  - `EnableLogging` — structured log output
  - `ValidateBeforeCommit` — run form validation before every commit
  - `ValidateBeforeNavigation` — run record validation before navigation
  - `ClearCacheOnFormClose` — cache eviction policy on form close
  - `MaxRecordsPerBlock` — safeguard for runaway queries
- Constructor injection: `new FormsManager(editor, configurationManager: myConfig)` — override defaults

## Working Rules
1. Start from config defaults and validate before persisting.
2. Cache block info intentionally; do not pre-warm unless block access pattern justifies it.
3. Tune with telemetry (`GetCacheEfficiencyMetrics`), not guesses.
4. Set total-record count explicitly after every query when paging is active.
5. Always set a security context before the first form open when access control is required.
6. `AuditManager` does not automatically flush; use `ConfigureAudit` to set a flush interval, or call `PurgeAuditLog` in maintenance flows.
7. `FileAuditStore` supports concurrent readers with `FileShare.Delete` to avoid read-lock conflicts on rotation/replace.

## Related Skills
- [`forms`](../forms/SKILL.md)
- [`forms-helper-managers`](../forms-helper-managers/SKILL.md)

## Detailed Reference
Use [`reference.md`](./reference.md) for config patterns, metrics usage, audit setup, and security rule patterns.
