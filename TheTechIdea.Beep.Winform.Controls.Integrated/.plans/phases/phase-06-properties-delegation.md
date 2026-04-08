# Phase 06 — Delegate `BeepDataBlock.Properties.*` to FormsManager

**Repo:** Beep.Winform  
**Scope:** `BeepDataBlock.Properties.cs`.  
**Depends on:** Phase 02 (`_formsManager` guaranteed non-null).  
**Build check:** `dotnet build WinFormsApp.sln` — zero errors before moving to Phase 07.

---

## What changes in this phase

| Item | Action |
|---|---|
| `private Dictionary<string, BeepDataBlockItem> _items` | Delete field |
| `private BeepDataBlockProperties _blockProperties` | Delete field |
| All item registration methods | Rewrite: delegate to `_formsManager.Items.*`, keep `UIComponents` dict for WinForms refs |
| All `GetItemProperty` / `SetItemProperty` methods | Rewrite as one-line delegates |
| `BeepDataBlockItem` | Thin wrapper only — adds `IBeepUIComponent Component` over `ItemInfo` |
| `Helpers/BeepDataBlockPropertyHelper.cs` | Delete file |

---

## Clean Code Rules

- `UIComponents` (the `Dictionary<string, IBeepUIComponent>`) **stays** — it holds WinForms control references. No cross-platform substitute.
- `_items` is gone — item metadata lives in `_formsManager.Items`.
- `RegisterItem` is Pattern B: delegate first, then store the `IBeepUIComponent` reference locally.
- Batch helpers (`SetAllItemsEnabled`, etc.) loop over `UIComponents.Keys` and call the delegate.

---

## Target: `BeepDataBlock.Properties.cs` (full rewrite)

```csharp
using System.Collections.Generic;
using TheTechIdea.Beep.Editor.Forms.Models;  // ItemInfo
using TheTechIdea.Beep.Vis.Modules;          // IBeepUIComponent

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial — Item &amp; Block Properties.
    /// Item metadata is owned by <see cref="FormsManager"/>.
    /// <see cref="UIComponents"/> holds WinForms control references (local-only).
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Registration

        /// <summary>
        /// Registers a UI component as an item of this block.
        /// Oracle Forms equivalent: items defined in the block's item list.
        /// </summary>
        public void RegisterItem(string itemName, IBeepUIComponent component)
        {
            itemName = NormalizeItemName(itemName, component);

            var info = new ItemInfo
            {
                ItemName       = itemName,
                BlockName      = Name,
                BoundProperty  = component.BoundProperty,
                Enabled        = true,
                Visible        = true,
                QueryAllowed   = true,
                InsertAllowed  = true,
                UpdateAllowed  = true
            };

            _formsManager.Items.RegisterItem(Name, itemName, info);
            UIComponents[itemName] = component;   // WinForms UI ref — stays local
        }

        /// <summary>Registers all child UI components automatically.</summary>
        public void RegisterAllItems()
        {
            foreach (var component in GetAllUIComponents())
            {
                var itemName = NormalizeItemName(component.Name, component);
                if (!string.IsNullOrEmpty(itemName))
                    RegisterItem(itemName, component);
            }
        }

        /// <summary>Removes an item from this block.</summary>
        public void UnregisterItem(string itemName)
        {
            _formsManager.Items.UnregisterItem(Name, itemName);
            UIComponents.Remove(itemName);
        }

        #endregion

        #region Item Properties

        /// <summary>
        /// Returns the metadata for the given item.
        /// Oracle Forms equivalent: GET_ITEM_PROPERTY built-in.
        /// </summary>
        public ItemInfo GetItemInfo(string itemName)
            => _formsManager.Items.GetItemInfo(Name, itemName);

        /// <summary>
        /// Sets a named property on the given item.
        /// Oracle Forms equivalent: SET_ITEM_PROPERTY built-in.
        /// </summary>
        public void SetItemProperty(string itemName, string propertyName, object value)
            => _formsManager.Items.SetItemProperty(Name, itemName, propertyName, value);

        /// <summary>
        /// Returns the value of a named property on the given item.
        /// Oracle Forms equivalent: GET_ITEM_PROPERTY built-in.
        /// </summary>
        public object GetItemProperty(string itemName, string propertyName)
            => _formsManager.Items.GetItemProperty(Name, itemName, propertyName);

        #endregion

        #region Convenience Setters

        /// <summary>Sets Enabled on the given item and syncs the WinForms component.</summary>
        public void SetItemEnabled(string itemName, bool enabled)
        {
            _formsManager.Items.SetItemProperty(Name, itemName, nameof(ItemInfo.Enabled), enabled);

            if (UIComponents.TryGetValue(itemName, out var component))
                component.Enabled = enabled;
        }

        /// <summary>Sets Visible on the given item and syncs the WinForms component.</summary>
        public void SetItemVisible(string itemName, bool visible)
        {
            _formsManager.Items.SetItemProperty(Name, itemName, nameof(ItemInfo.Visible), visible);

            if (UIComponents.TryGetValue(itemName, out var component))
                component.Visible = visible;
        }

        /// <summary>Applies Enabled to all registered items at once.</summary>
        public void SetAllItemsEnabled(bool enabled)
        {
            foreach (var itemName in UIComponents.Keys)
                SetItemEnabled(itemName, enabled);
        }

        /// <summary>Applies Visible to all registered items at once.</summary>
        public void SetAllItemsVisible(bool visible)
        {
            foreach (var itemName in UIComponents.Keys)
                SetItemVisible(itemName, visible);
        }

        #endregion

        #region Block Properties

        /// <summary>
        /// Updates block-level metadata in FormsManager.
        /// Called internally when block-level properties change.
        /// </summary>
        private void SyncBlockInfoToFormsManager()
        {
            var info = new DataBlockInfo
            {
                BlockName      = Name,
                FormName       = FormName,
                EntityName     = EntityName,
                ConnectionName = ConnectionName
            };
            _formsManager.UpdateBlockInfo(info);
        }

        #endregion
    }
}
```

---

## `BeepDataBlockItem` after this phase

`BeepDataBlockItem` is no longer used as a dictionary value inside `BeepDataBlock`. It exists only as a convenience type in Examples that compose an `IBeepUIComponent` with metadata. Slim it down:

```csharp
// DataBlocks/Models/BeepDataBlockItem.cs — slimmed wrapper
using TheTechIdea.Beep.Editor.Forms.Models;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    /// <summary>
    /// Combines FormsManager <see cref="ItemInfo"/> with a WinForms component reference.
    /// Used only in Examples and test scaffolding — not stored inside BeepDataBlock.
    /// </summary>
    public class BeepDataBlockItem : ItemInfo
    {
        /// <summary>The WinForms UI component that renders this item.</summary>
        public IBeepUIComponent Component { get; set; }
    }
}
```

---

## Files to delete

```
DataBlocks/Helpers/BeepDataBlockPropertyHelper.cs
```

Before deleting, grep:
```
grep -r "BeepDataBlockPropertyHelper\|_blockProperties" --include="*.cs"
```

---

## Checklist

- [ ] Rewrite `BeepDataBlock.Properties.cs` to delegation + UIComponents wiring
- [ ] Slim `BeepDataBlockItem` to extend `ItemInfo`
- [ ] Remove `_blockProperties` field → replace with `SyncBlockInfoToFormsManager()`
- [ ] Update `using` directives
- [ ] Grep: `BeepDataBlockPropertyHelper|_items\[|_blockProperties` — zero hits
- [ ] Delete `Helpers/BeepDataBlockPropertyHelper.cs`
- [ ] `dotnet build WinFormsApp.sln` — zero errors
