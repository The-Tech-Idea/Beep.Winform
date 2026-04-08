# Phase 11 — Optional: WPF and Blazor Adapters

**Repos:** Beep.Desktop (WPF) / MyWebSite or Beep.DeveloperAssistant (Blazor)  
**Scope:** New projects: `TheTechIdea.Beep.WPF.Controls.Integrated`, `TheTechIdea.Beep.Blazor.Integrated`.  
**Depends on:** Phases 01–09 complete.  
**Build check:** Each adapter project builds independently — zero errors.

---

## Purpose

Phases 01–09 attach all business logic to FormsManager via `IDataBlockController` and `IDataBlockNotifier`.  
Phase 11 demonstrates that a non-WinForms UI can satisfy those contracts and participate in the same FormsManager coordination with zero changes to BeepDM.

Both adapters only:
1. Implement `IDataBlockController` — wrap native UI state.
2. Register with `FormsManager.Blocks`.
3. Render the current record and respond to record-changed events.

No bridges. No translated types. No duplicated business logic.

---

## 11.1 — WPF Adapter

**Project:** `TheTechIdea.Beep.WPF.Controls.Integrated`  
**Namespace:** `TheTechIdea.Beep.WPF.Controls`  
**Base class:** `System.Windows.Controls.UserControl`

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor.Forms.Interfaces;
using TheTechIdea.Beep.Editor.Forms.Models;

namespace TheTechIdea.Beep.WPF.Controls
{
    /// <summary>
    /// WPF analogue of <c>BeepDataBlock</c>.
    /// Implements <see cref="IDataBlockController"/> so FormsManager coordinates
    /// this block identically to its WinForms counterpart.
    /// </summary>
    public partial class BeepWpfDataBlock : UserControl, IDataBlockController
    {
        // ── IDataBlockController identity ─────────────────────────────────────

        public string Name         { get; set; }
        public string FormName     { get; set; }
        public string EntityName   { get; set; }
        public string ConnectionName { get; set; }
        public IFormsManager FormsManager { get; set; }
        public IDataBlockNotifier Notifier { get; set; }

        // ── WPF-specific component registry ───────────────────────────────────

        /// <summary>Stores WPF data-bound controls keyed by item/field name.</summary>
        public Dictionary<string, FrameworkElement> UIElements { get; } = new();

        // ── IDataBlockController operations ───────────────────────────────────

        public void RegisterItem(string itemName, object metadata)
            => FormsManager.Items.RegisterItem(Name, itemName, (ItemInfo)metadata);

        public void SetItemEnabled(string itemName, bool enabled)
        {
            FormsManager.Items.SetItemProperty(Name, itemName, "Enabled", enabled);
            if (UIElements.TryGetValue(itemName, out var element))
                element.IsEnabled = enabled;
        }

        public void SetItemVisible(string itemName, bool visible)
        {
            FormsManager.Items.SetItemProperty(Name, itemName, "Visible", visible);
            if (UIElements.TryGetValue(itemName, out var element))
                element.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public Task<bool> ExecuteQueryAsync(List<AppFilter> filters = null)
            => FormsManager.ExecuteQueryAsync(Name, filters);

        public Task<bool> SaveAsync()
            => FormsManager.SaveAsync(Name);

        public Task RollbackAsync()
            => FormsManager.RollbackAsync(Name);

        public bool NextRecord()  => FormsManager.Navigation.NextRecord(Name);
        public bool PreviousRecord() => FormsManager.Navigation.PreviousRecord(Name);
        public bool FirstRecord() => FormsManager.Navigation.FirstRecord(Name);
        public bool LastRecord()  => FormsManager.Navigation.LastRecord(Name);
    }
}
```

**Registration at startup:**
```csharp
var wpfBlock = new BeepWpfDataBlock
{
    Name           = "OrdersBlock",
    EntityName     = "Orders",
    FormName       = "MainOrdersForm",
    FormsManager   = formsManager,
    Notifier       = new DefaultDataBlockNotifier()
};

formsManager.Blocks.Register(wpfBlock);
await wpfBlock.ExecuteQueryAsync();
```

---

## 11.2 — Blazor Adapter

**Project:** `TheTechIdea.Beep.Blazor.Integrated`  
**Namespace:** `TheTechIdea.Beep.Blazor.Components`  
**Base:** `ComponentBase`

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor.Forms.Interfaces;
using TheTechIdea.Beep.Editor.Forms.Models;

namespace TheTechIdea.Beep.Blazor.Components
{
    /// <summary>
    /// Blazor analogue of <c>BeepDataBlock</c>.
    /// Implements <see cref="IDataBlockController"/> — FormsManager coordinates
    /// this block identically to any other implementation.
    /// </summary>
    public partial class BeepBlazorDataBlock : ComponentBase, IDataBlockController
    {
        // ── Injected services ─────────────────────────────────────────────────

        [Inject] public IFormsManager FormsManager  { get; set; }
        [Inject] public IDataBlockNotifier Notifier { get; set; }

        // ── IDataBlockController identity ─────────────────────────────────────

        [Parameter] public string Name          { get; set; }
        [Parameter] public string FormName      { get; set; }
        [Parameter] public string EntityName    { get; set; }
        [Parameter] public string ConnectionName { get; set; }

        // ── State observable by the Blazor render tree ────────────────────────

        protected List<Dictionary<string, object>> Records { get; private set; } = new();
        protected int CurrentRecordIndex => FormsManager.Navigation.GetCurrentIndex(Name);

        // ── Lifecycle ─────────────────────────────────────────────────────────

        protected override void OnInitialized()
        {
            FormsManager.Blocks.Register(this);
        }

        // ── IDataBlockController operations ───────────────────────────────────

        public async Task<bool> ExecuteQueryAsync(List<AppFilter> filters = null)
        {
            var ok = await FormsManager.ExecuteQueryAsync(Name, filters);
            await InvokeAsync(StateHasChanged);
            return ok;
        }

        public Task<bool> SaveAsync()
            => FormsManager.SaveAsync(Name);

        public Task RollbackAsync()
            => FormsManager.RollbackAsync(Name);

        public bool NextRecord()
        {
            var ok = FormsManager.Navigation.NextRecord(Name);
            InvokeAsync(StateHasChanged);
            return ok;
        }

        public bool PreviousRecord()
        {
            var ok = FormsManager.Navigation.PreviousRecord(Name);
            InvokeAsync(StateHasChanged);
            return ok;
        }

        public bool FirstRecord() => FormsManager.Navigation.FirstRecord(Name);
        public bool LastRecord()  => FormsManager.Navigation.LastRecord(Name);

        public void RegisterItem(string itemName, object metadata)
            => FormsManager.Items.RegisterItem(Name, itemName, (ItemInfo)metadata);

        public void SetItemEnabled(string itemName, bool enabled)
            => FormsManager.Items.SetItemProperty(Name, itemName, "Enabled", enabled);

        public void SetItemVisible(string itemName, bool visible)
            => FormsManager.Items.SetItemProperty(Name, itemName, "Visible", visible);
    }
}
```

**Razor template (`BeepBlazorDataBlock.razor.cs` or inline):**
```razor
@inherits ComponentBase

<div class="beep-data-block">
    @foreach (var record in Records)
    {
        <div class="beep-record">
            @foreach (var field in record)
            {
                <div class="beep-field">
                    <label>@field.Key</label>
                    <input value="@field.Value?.ToString()" readonly />
                </div>
            }
        </div>
    }
</div>
```

---

## What this phase proves

| Claim | Evidence |
|---|---|
| FormsManager is UI-agnostic | WPF and Blazor blocks register and execute queries identically to WinForms |
| `IDataBlockController` is the only contract | Neither adapter references WinForms types |
| Phases 01–09 are complete | No bridge classes, no `IsCoordinated` guards, no duplicate state |

---

## Checklist

- [ ] Create `TheTechIdea.Beep.WPF.Controls.Integrated` project
- [ ] Implement `BeepWpfDataBlock : UserControl, IDataBlockController`
- [ ] Create `TheTechIdea.Beep.Blazor.Integrated` project
- [ ] Implement `BeepBlazorDataBlock : ComponentBase, IDataBlockController`
- [ ] Register both blocks with a shared `FormsManager` in a spike test
- [ ] Verify that `ExecuteQueryAsync` on one block does not break the other
- [ ] `dotnet build` both projects — zero errors
