# Phase 06 — Property-Grid Editors & InitializeComponent Codegen

**Status overview.** This phase splits into three independently
shippable sub-phases. The two lower-risk slices land first; the
codegen contract is deferred to a dedicated iteration because a
broken `CodeDomSerializer` can corrupt user source.

| Sub-phase | Scope                                                          | Status         |
| --------- | -------------------------------------------------------------- | -------------- |
| 06A       | Property metadata audit + Properties-grid editor attributes    | ✅ **Shipped** |
| 06B       | `DocumentHostLayoutEditor` (UITypeEditor wrapping layout-tree dialog) | ✅ **Shipped** |
| 06C       | `CodeDomSerializer` + runtime `RegisterDesignTimePanelContent` | ⏳ Pending — own iteration |

---

## 06A — Property metadata audit & Properties-grid editor attributes

### Goal
Every public, designable property on `BeepDocumentHost`,
`BeepDocumentManager`, and `BeepDocumentPanel` carries `[Category]`,
`[Description]`, `[DefaultValue]`, and (when read-only / non-
serialisable) `[Browsable(false)]` + `[DesignerSerializationVisibility
(Hidden)]`. The `DesignTimeDocuments` collections expose our custom
editor through the property grid's `[…]` button, not just through the
smart-tag verbs.

### Findings (audit)
A `Select-String` pass over the three runtime files counted:

| File                                              | Public decls | `[Category]` | `[Description]` | `[DefaultValue]` | `[Browsable(false)]` | `[DSV(Hidden)]` |
| ------------------------------------------------- | ------------ | ------------ | ---------------- | ----------------- | -------------------- | --------------- |
| `BeepDocumentHost.Properties.cs`                  | 72           | 38           | 38               | 38                | 20                   | 20              |
| `BeepDocumentManager.cs`                          | 20           | 21           | 25               | 15                | 3                    | 3               |
| `BeepDocumentPanel.cs`                            | 8            | **0**        | 7                | 6                 | 2                    | 2               |

The two host files were already well-attributed; the remaining gaps
were narrow:

1. **`BeepDocumentPanel`** had `[Description]` + `[DefaultValue]` on
   every visible setter but no `[Category]`. Every property fell into
   the "(unsorted)" bucket of the Properties window.
2. **`BeepDocumentHost.DesignTimeDocuments`** lacked an
   `[Editor]` attribute, so the property grid fell back to the
   framework's generic collection editor instead of the custom
   `DocumentDescriptorEditorForm` the smart-tag uses.
3. **`BeepDocumentManager.DesignTimeDocuments`** had the same gap.

### Fixes
- `BeepDocumentPanel`: added `[Category("Document")]` to `DocumentTitle`,
  `IsModified`, `CanClose`, `IconPath`, `DocumentCategory`,
  `ShowStatusBar`.
- `BeepDocumentHost.DesignTimeDocuments`: added
  ```csharp
  [Editor(
      "TheTechIdea.Beep.Winform.Controls.Design.Server.Editors.DesignTimeDocumentsEditor, TheTechIdea.Beep.Winform.Controls.Design.Server",
      "System.Drawing.Design.UITypeEditor, System.Drawing")]
  ```
  Uses the string-based form because the runtime project intentionally
  does not reference the design-server assembly (loaded by the
  designer at design time only).
- `BeepDocumentManager.DesignTimeDocuments`: same `[Editor]` wiring.

### Acceptance
- `dotnet build TheTechIdea.Beep.Winform.Controls.csproj` → 0 errors.
- `dotnet build TheTechIdea.Beep.Winform.Controls.Design.Server.csproj`
  → 0 errors.
- Linter clean on every changed file.
- Manual: in VS, click a `BeepDocumentPanel` in the host's tab strip
  → the Properties window now groups `DocumentTitle`, `CanClose`,
  `IconPath`, etc. under a **Document** category header.
- Manual: click the `[…]` button on `DesignTimeDocuments` in either
  host or manager → the custom `DocumentDescriptorEditorForm` opens
  (Phase 04's `CommitEditedCollection` keeps it transactional).

---

## 06B — `DocumentHostLayoutEditor`

### Goal
Provide a `UITypeEditor` that opens the layout-tree viewer from the
Properties grid (in addition to the existing
**View Layout Tree…** smart-tag verb). Lets power users inspect the
host's serialised layout in one click without leaving the grid.

### Files
- `Editors/DocumentHostLayoutEditor.cs` *(new)* — `UITypeEditor`
  subclass that resolves the parent `BeepDocumentHost` via
  `ITypeDescriptorContext.Instance` and opens the existing
  `Dialogs/LayoutTreeDialog` (extracted in Phase 03).

### Design notes
- **Read-only on purpose.** `LayoutTreeDialog` is a passive viewer; a
  future iteration can swap in a drag-to-rearrange tree without
  changing the editor contract.
- **Doesn't require unhiding `DesignTimeLayoutJson`.** The JSON
  property stays `Browsable(false)` so it doesn't clutter the grid
  with raw string noise. The editor is wired as a generic capability;
  the verb path remains the primary entry point.
- **Generic surface.** The editor depends only on the property's
  `Instance` being a `BeepDocumentHost`. It can be applied to *any*
  string property that represents a host's serialised layout (for
  example, a future workspace-snapshot string).

### Acceptance
- `dotnet build TheTechIdea.Beep.Winform.Controls.Design.Server.csproj`
  → 0 errors.
- Linter clean.
- Manual: smart-tag → **View Layout Tree…** still opens the dialog.
- Manual: when a future iteration exposes a string property tagged
  `[Editor(typeof(DocumentHostLayoutEditor), typeof(UITypeEditor))]`,
  clicking the `[…]` button opens the dialog for the owning host.

---

## 06C — `CodeDomSerializer` + runtime `RegisterDesignTimePanelContent` *(pending)*

### Why this is deferred
A custom `CodeDomSerializer` writes directly into the user's source
file. Mistakes here can corrupt projects and are hard to recover from
in a code-review loop. The scope is also substantial — it requires:

1. A runtime contract change on `BeepDocumentHost`
   (`RegisterDesignTimePanelContent(string docId, Action<BeepDocumentPanel> builder)`)
   that survives all currently supported target frameworks.
2. A `DesignerSerializationVisibility.Hidden` overlay on every sited
   `BeepDocumentPanel` via the design-server's
   `TypeDescriptionProvider` pipeline.
3. A round-trip regression test against at least three real-world
   form scenarios.

### Plan when scheduled
1. **Runtime API** — add
   `BeepDocumentHost.RegisterDesignTimePanelContent(docId, builder)`
   plus a private deferred-queue keyed by document id. The host's
   existing `ApplyDesignTimeDocuments` flushes the queue after each
   panel is created.
2. **`DesignerSerializationVisibility.Hidden` overlay** — applied via
   the panel siting flow in
   `BeepDocumentHostDesigner.PanelSiting.cs` (Phase 02). Inject
   through a `CustomTypeDescriptor` registered on the panel's
   `ISite` so the standard `CodeDomSerializer` skips the panel
   declaration but still emits its child controls.
3. **Custom `CodeDomSerializer`** — `BeepDocumentHostCodeDomSerializer`
   subclasses `CodeDomSerializer` and:
   - Calls the base for the host itself (so all the host's properties
     are emitted normally).
   - Walks the host's sited child panels.
   - For each panel, emits a `host.RegisterDesignTimePanelContent("id",
     p => { p.Controls.Add(this.beepButton1); … });` block in lieu of
     `host.Controls.Add(panel)`.
   - Suppresses the panel's own `new BeepDocumentPanel()` line.
4. **Round-trip test matrix**:
   - 1 host, 1 panel, 0 child controls.
   - 1 host, 3 panels, mixed child controls.
   - 1 host, 5 panels, nested splits.
   - 2 hosts on one form.
   - Manager + tabbed view + 3 panels.
5. **Feature flag.** Gate behind
   `BeepDocumentManager.EnableCodeDomCodegen` (default `false`)
   during early testing so the existing string-serialised
   `DesignTimeDocuments + DesignTimeLayoutJson` pipeline keeps
   working unchanged.

### Risks
- Designer / runtime version skew between consumer projects and
  upgraded `Beep.Winform.Controls` versions. The contract for
  `RegisterDesignTimePanelContent` must be additive-only after first
  ship.
- Theme-aware property defaults that differ from declared
  `[DefaultValue]` would cause undo to appear no-op. Audit during the
  06C iteration.
- `BeepDocumentManager` codegen still needs to round-trip the `View`
  reference through the existing component-tray serialiser even when
  06C is shipped for the host.

---

## Files touched (this iteration — 06A + 06B)

**New:**
- `Editors/DocumentHostLayoutEditor.cs`

**Edited (runtime):**
- `DocumentHost/BeepDocumentHost.Properties.cs`
  — `[Editor]` on `DesignTimeDocuments`.
- `DocumentHost/BeepDocumentManager.cs`
  — `[Editor]` on `DesignTimeDocuments`.
- `DocumentHost/BeepDocumentPanel.cs`
  — `[Category("Document")]` on the six visible setters.

**Edited (plans):**
- `.plans/documenthost-designtime-Phase-06-property-grid-editors.md`
  — this document, restructured into 06A / 06B / 06C.
- `.plans/MASTER-TODO-TRACKER.md` — Phase 06 split, statuses updated.
