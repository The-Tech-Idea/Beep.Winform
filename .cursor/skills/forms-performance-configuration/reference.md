# Forms Performance, Audit, Security, and Configuration Reference

## Scenario A: Bootstrap Configuration + Performance Policy

```csharp
public static FormsManager BuildConfiguredManager(IDMEEditor editor)
{
    var cfgManager = new ConfigurationManager();
    cfgManager.LoadConfiguration();
    if (!cfgManager.ValidateConfiguration())
        cfgManager.ResetToDefaults();

    cfgManager.Configuration.EnableLogging = true;
    cfgManager.Configuration.ValidateBeforeCommit = true;
    cfgManager.Configuration.ValidateBeforeNavigation = true;
    cfgManager.Configuration.ClearCacheOnFormClose = false;
    cfgManager.Configuration.MaxRecordsPerBlock = 2000;
    cfgManager.SaveConfiguration();

    return new FormsManager(editor, configurationManager: cfgManager);
}
```

## Scenario B: Paging Setup

```csharp
// After query completes, set total-record count then load first page
var queryResult = await forms.ExecuteQueryEnhancedAsync("ORDERS", filters);
forms.SetTotalRecordCount("ORDERS", forms.GetRecordCount("ORDERS"));
forms.SetBlockPageSize("ORDERS", 50);

var pageInfo = await forms.LoadPageAsync("ORDERS", pageNumber: 1);
Console.WriteLine($"Page {pageInfo.PageNumber} of {pageInfo.TotalPages} " +
                  $"({pageInfo.PageSize} records per page)");

// Navigate to next page
pageInfo = await forms.LoadPageAsync("ORDERS", pageInfo.PageNumber + 1);
```

## Scenario C: Cache Metrics and Tuning

```csharp
var stats = forms.PerformanceManager.GetPerformanceStatistics();
Console.WriteLine($"Hits={stats.CacheHits}, Misses={stats.CacheMisses}, Ratio={stats.CacheHitRatio:P2}");

var efficiency = forms.PerformanceManager.GetCacheEfficiencyMetrics();
Console.WriteLine($"Top blocks: {string.Join(", ", efficiency.TopAccessedBlocks)}");

if (stats.CacheHitRatio < 0.60)
    forms.PerformanceManager.OptimizeBlockAccess();

// Preload known-frequent blocks at startup
forms.PerformanceManager.PreloadFrequentBlocks(new[] { "CUSTOMERS", "ORDERS", "PRODUCTS" });
```

## Scenario D: Audit Trail Setup + Query

```csharp
forms.SetAuditUser(Environment.UserName);
forms.ConfigureAudit(cfg =>
{
    cfg.Enabled = true;
    cfg.FlushIntervalSeconds = 30;
    // Use file-backed store for session history
    cfg.Store = new FileAuditStore("./audit-session.json");
});

// After operations ...
var deletions = forms.GetAuditLog(
    blockName: "ORDERS",
    operation: AuditOperation.Delete,
    from: DateTime.Today);

foreach (var e in deletions)
    Console.WriteLine($"{e.UserName} deleted record {e.RecordKey} at {e.Timestamp}");

// Export and purge
await forms.ExportAuditToJsonAsync("./orders-audit.json", "ORDERS");
forms.PurgeAuditLog(before: DateTime.Today.AddDays(-30));
```

## Scenario E: Block and Field Security

```csharp
// Set security context at session start
forms.SetSecurityContext(new SecurityContext
{
    UserName = "carol",
    Roles = new[] { "manager", "sales" }
});

// Block-level rules
forms.SetBlockSecurity("ORDERS", new BlockSecurity
{
    AllowInsert = true,
    AllowUpdate = true,
    AllowDelete = false,          // only managers can delete
    RequiredRoles = new[] { "sales" }
});

// Field masking + read-only
forms.SetFieldSecurity("CUSTOMERS", "TaxId", new FieldSecurity
{
    Masked = true,
    ReadOnly = true
});

// Runtime check before attempting delete
if (!forms.IsBlockAllowed("ORDERS", SecurityPermission.Delete))
{
    await forms.ShowAlertAsync("Access Denied", "You do not have permission to delete orders.",
                               AlertStyle.Stop);
    return;
}

// Masked display value
var masked = forms.GetMaskedValue("CUSTOMERS", "TaxId", rawTaxId);
```

## Notes
- Tune with `GetCacheEfficiencyMetrics`; avoid blind cache invalidation.
- Set total-record count explicitly after each query when paging is active.
- `FileAuditStore` opens with `FileShare.Delete` to avoid lock conflicts during log rotation.
- `SecurityManager` pushes flags into `DataBlockInfo` and `ItemPropertyManager`; UI disabling is supplemental.
- `PurgeAuditLog` is a maintenance tool; avoid calling it inside transactional flows.

