# Phase 5 — Floating & Auto-Hide

> **CRITICAL**: Copy the EXACT code and logic from Krypton. Only change namespace/class/method names.
> Do NOT rewrite, simplify, stub, or reinterpret. Create EVERY class fully.
> No compatibility concerns. No legacy.

## Prerequisites

- Phase 4 COMPLETED (zero build errors)

## Goal

Create floating window management and auto-hide panel infrastructure.

### Task 5.1 — BeepFloatingWindow

**Source**: `Krypton.Docking/Control Docking/KryptonFloatingWindow.cs`

| # | Action | Detail |
|---|--------|--------|
| 5.1.1 | Create | `Controls/Docking/Floating/BeepFloatingWindow.cs` |
| 5.1.2 | Copy entire content, rename class |
| 5.1.3 | Rename base | `KryptonForm` → `BeepVisualForm` (from Phase 0) |
| 5.1.4 | Namespace | `TheTechIdea.Beep.Winform.Controls.Docking.Floating` |

### Task 5.2 — Floating Element Adapters

| # | Source File | Beep File | Location |
|---|------------|-----------|----------|
| 5.2.1 | `KryptonDockingFloatingWindow.cs` | `BeepDockingFloatingWindow.cs` | `Elements/` |
| 5.2.2 | `KryptonDockingFloating.cs` | `BeepDockingFloating.cs` | `Elements/` |
| 5.2.3 | `KryptonDockingFloatspace.cs` | `BeepDockingFloatspace.cs` | `Elements/` |

Copy each file entirely, rename all classes/types with `Beep` prefix.

### Task 5.3 — Auto-Hide Controls

| # | Source File | Beep File | Location |
|---|------------|-----------|----------|
| 5.3.1 | `KryptonAutoHiddenGroup.cs` | `BeepAutoHiddenGroup.cs` | `AutoHidden/` |
| 5.3.2 | `KryptonAutoHiddenPanel.cs` | `BeepAutoHiddenPanel.cs` | `AutoHidden/` |
| 5.3.3 | `KryptonAutoHiddenSlidePanel.cs` | `BeepAutoHiddenSlidePanel.cs` | `AutoHidden/` |

### Task 5.4 — Auto-Hide Element Adapters

| # | Source File | Beep File | Location |
|---|------------|-----------|----------|
| 5.4.1 | `KryptonDockingEdgeAutoHidden.cs` | `BeepDockingEdgeAutoHidden.cs` | `Elements/` |
| 5.4.2 | `KryptonDockingAutoHiddenGroup.cs` | `BeepDockingAutoHiddenGroup.cs` | `Elements/` |

### Task 5.5 — BeepAutoHiddenProxyPage

Already created in Phase 3.6. If not done, create now:
- Copy `KryptonAutoHiddenProxyPage.cs`, rename to `BeepAutoHiddenProxyPage`

## Build Verification

```
dotnet build "TheTechIdea.Beep.Winform.Controls/TheTechIdea.Beep.Winform.Controls.csproj" -v:q /clp:ErrorsOnly
```
