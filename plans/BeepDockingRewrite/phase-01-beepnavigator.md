# Phase 1 — BeepNavigator & BeepPage

> **CRITICAL**: Copy the EXACT code and logic from Krypton. Only change namespace/class/method names.
> Do NOT rewrite, simplify, stub, or reinterpret. Create EVERY class fully.
> No compatibility concerns. No legacy.

## Prerequisites

- Phase 0 COMPLETED (zero build errors)

## Goal

Create the tab control foundation: `BeepNavigator`, `BeepPage`, `BeepPageCollection`.

This is the KEY phase. Krypton's entire docking system is built on `KryptonNavigator` as the tab/page management engine. Once this phase is complete, BeepDockspace no longer needs manual `DrawHeader` tab rendering — tabs are handled by `BeepNavigator` natively.

## Krypton Source Files to Copy

### Task 1.1 — BeepPage

**Source**: `Standard-Toolkit-master/Source/Krypton Components/Krypton.Navigator/Page/KryptonPage.cs`

| # | Action | Detail |
|---|--------|--------|
| 1.1.1 | Create | `Controls/Docking/Navigator/BeepPage.cs` |
| 1.1.2 | Copy | Entire `KryptonPage.cs` content |
| 1.1.3 | Rename class | `KryptonPage` → `BeepPage` |
| 1.1.4 | Rename base | `VisualPanel` → `BeepVisualPanel` (from Phase 0) |
| 1.1.5 | Rename enum | `KryptonPageFlags` → `BeepPageFlags` |
| 1.1.6 | Rename event args | `KryptonPageFlagsEventArgs` → `BeepPageFlagsEventArgs` |
| 1.1.7 | Namespace | `TheTechIdea.Beep.Winform.Controls.Docking.Navigator` |

Also copy/create:
| # | Krypton Source | Beep File | 
|---|---------------|-----------|
| 1.1a | `KryptonPageFlags.cs` enum | `Navigator/BeepPageFlags.cs` |
| 1.1b | `KryptonPageFlagsEventArgs.cs` | `Navigator/BeepPageFlagsEventArgs.cs` |

### Task 1.2 — BeepPageCollection

**Source**: `Standard-Toolkit-master/Source/Krypton Components/Krypton.Navigator/Page/KryptonPageCollection.cs`

| # | Action | Detail |
|---|--------|--------|
| 1.2.1 | Create | `Controls/Docking/Navigator/BeepPageCollection.cs` |
| 1.2.2 | Copy | Entire content |
| 1.2.3 | Rename class | `KryptonPageCollection` → `BeepPageCollection` |
| 1.2.4 | Rename base | `TypedCollection<KryptonPage>` → `BeepTypedCollection<BeepPage>` |
| 1.2.5 | Namespace | `TheTechIdea.Beep.Winform.Controls.Docking.Navigator` |

### Task 1.3 — BeepNavigator

**Source**: `Standard-Toolkit-master/Source/Krypton Components/Krypton.Navigator/Controls Navigator/KryptonNavigator.cs`

This is the LARGEST file. ~5000+ lines.

| # | Action | Detail |
|---|--------|--------|
| 1.3.1 | Create | `Controls/Docking/Navigator/BeepNavigator.cs` |
| 1.3.2 | Copy | Entire `KryptonNavigator.cs` content |
| 1.3.3 | Rename class | `KryptonNavigator` → `BeepNavigator` |
| 1.3.4 | Rename base | `VisualControlContainment` → `BeepVisualControlContainment` |
| 1.3.5 | Rename all types | `KryptonPage` → `BeepPage`, `KryptonPageCollection` → `BeepPageCollection`, etc. |
| 1.3.6 | Rename events | Same names, just change EventArgs types |
| 1.3.7 | Namespace | `TheTechIdea.Beep.Winform.Controls.Docking.Navigator` |

### Task 1.4 — ALL Supporting Types

Search `KryptonNavigator.cs` for EVERY type reference and create its Beep equivalent:

| # | Krypton Type | Beep Equivalent | Create In |
|---|-------------|-----------------|-----------|
| 1.4.1 | `NavigatorMode` enum | `BeepNavigatorMode.cs` | `Navigator/` |
| 1.4.2 | `BarMultiline` enum | `BeepBarMultiline.cs` | `Navigator/` |
| 1.4.3 | `MapKryptonPageText` enum | `BeepMapPageText.cs` | `Navigator/` |
| 1.4.4 | `MapKryptonPageImage` enum | `BeepMapPageImage.cs` | `Navigator/` |
| 1.4.5 | `CloseButtonAction` enum | `BeepCloseButtonAction.cs` | `Navigator/` |
| 1.4.6 | `PaletteNavContent` class | `BeepPaletteNavContent.cs` | `Navigator/` |
| 1.4.7 | `PaletteNavigator` class | `BeepPaletteNavigator.cs` | `Navigator/` |
| 1.4.8 | `PaletteNavigatorRedirect` class | `BeepPaletteNavigatorRedirect.cs` | `Navigator/` |
| 1.4.9 | `NavigatorOutlook` classes | `BeepNavigatorOutlook*.cs` | `Navigator/` |
| 1.4.10 | `DragFeedback` class | `BeepDragFeedback.cs` | `Navigator/` |
| 1.4.11 | `PageDragEventArgs` class | `BeepPageDragEventArgs.cs` | `Events/` |
| 1.4.12 | `PageDropEventArgs` class | `BeepPageDropEventArgs.cs` | `Events/` |
| 1.4.13 | `CtrlTabCancelEventArgs` class | `BeepCtrlTabCancelEventArgs.cs` | `Events/` |
| 1.4.14 | `TabPageEventArgs` class | `BeepTabPageEventArgs.cs` | `Events/` |
| 1.4.15 | `TabDragCancelEventArgs` class | `BeepTabDragCancelEventArgs.cs` | `Events/` |
| 1.4.16 | `ShowContextMenuArgs` class | `BeepShowContextMenuArgs.cs` | `Events/` |
| 1.4.17 | `KryptonContextMenu` class | `BeepContextMenu.cs` | `Navigator/` |
| 1.4.18 | `IDragPageNotify` interface | `IBeepDragPageNotify.cs` | `Interfaces/` |
| 1.4.19 | `IDragTargetProvider` interface | `IBeepDragTargetProvider.cs` | `Interfaces/` |
| 1.4.20 | `ButtonSpec` classes | `BeepButtonSpec.cs` etc. | `Navigator/` |
| 1.4.21 | `ButtonSpecNavigator` classes | `BeepButtonSpecNavigator.cs` etc. | `Navigator/` |

**Process**: Read `KryptonNavigator.cs` line by line. For every type that starts with `Krypton`, `Palette`, or is in `Krypton.Navigator` / `Krypton.Toolkit` namespace, find its source file in the Krypton codebase and copy it with Beep naming.

### Task 1.5 — Create ALL Event Args Classes

Read KryptonNavigator's events and copy every EventArgs type:

| # | Krypton | Beep |
|---|---------|------|
| 1.5.1 | `TabCountChangedEventArgs` | `BeepTabCountChangedEventArgs.cs` |
| 1.5.2 | `TextModifiedEventArgs` | `BeepTextModifiedEventArgs.cs` |
| 1.5.3 | `PopupPageEventArgs` | `BeepPopupPageEventArgs.cs` |

### Task 1.6 — ViewBuilder

**Source**: `Krypton.Navigator/ViewBuilder/` — this is likely a folder with multiple files

| # | Action | Detail |
|---|--------|--------|
| 1.6.1 | Copy ALL files in `ViewBuilder/` | `Controls/Docking/Navigator/ViewBuilder/` |
| 1.6.2 | Rename all | `ViewBuilder*` → `BeepViewBuilder*` |
| 1.6.3 | Rename all internal types | Same pattern |

## Build Verification

```
dotnet build "TheTechIdea.Beep.Winform.Controls/TheTechIdea.Beep.Winform.Controls.csproj" -v:q /clp:ErrorsOnly
```

Fix ALL errors. Common errors:
- Missing namespace `using`: add `TheTechIdea.Beep.Winform.Controls.Docking.*` equivalents
- Missing types: find the Krypton source and copy it with Beep naming
- Constructor mismatches: ensure all base class constructors exist from Phase 0
