# Docking Code Dictionary

Per-file responsibility inventory for `Docking/`, current as of **Phase 08** (all phases
landed). One row per file. Keep this in sync as the code evolves.

> History: Phase 00 removed the dead native-MDI subsystem and stub splitter classes;
> Phases 01–07 added the renderer framework, drag-and-drop, the layout engine + live
> splitters, auto-hide/float/animation, tabbed-group behavior, designer.cs persistence,
> and the design-time experience. The unified model has **no** separate `Document`
> type/`Kind` flag — the `Fill` group is the document well.

## Top level — `Docking/`
| File | Role |
|------|------|
| `BeepDockingManager.cs` | Orchestrator: panel registry, layout tree, WinForms hosting, theming, panel ops, events, batch update, single `ApplyLayout()` pass. |
| `BeepDockingManager.DragDrop.cs` | Partial: drag-drop commit helpers + cancelable docking events. |
| `BeepDockingManager.Persistence.cs` | Partial: `LayoutDefinition` `[Content]` property over a stable backing instance; `CaptureDefinition`/`FillDefinition`/`MaterializeFromDefinition`; `SaveLayout`/`LoadLayout`. |
| `BeepDockspace.cs` | `Panel` that hosts `DockPanel`s as tab pages; design-time tab cell surface. |
| `BeepDockingStrings.cs` | Localizable UI strings (`INotifyPropertyChanged`). |
| `BeepDockingUpdate.cs` | Disposable batch-update scope (`BeginUpdate`/`EndUpdate`). |
| `DockingThemeColors.cs` | `IBeepTheme` → docking palette resolver with WCAG contrast safeguards. |

## Models — `Docking/Models/`
| File | Role |
|------|------|
| `DockPanel.cs` | Primary content unit; externally-rendered caption/tabs; caption drag, in-strip reorder, middle-click close, overflow menu; `ShowCaption`, `MinWidth/MinHeight`. |
| `DockGroup.cs` | Group of panels in one dock cell; tab order, active panel, 0.1–0.9 split ratio, `MinWidth/MinHeight`, `MovePanelToIndex`. |
| `DockLayoutTree.cs` | Hierarchical layout model (recursion, registration, lookup by id). |
| `DockLayoutDefinition.cs` | Designer-serializable layout graph: `DockGroupDefinition`, `FloatingPanelInfo`, `AutoHiddenPanelInfo` (get-only `[Content]` collections). |
| `DockingEnums.cs` | `DockPosition`, `DockPanelState`, `TabStyle`, `AllowedAreas`, etc. |
| `DockingEventArgs.cs` | Cancelable request + lifecycle event args. |
| `PanelSerializationInfo.cs` | Legacy per-panel snapshot; superseded by `DockLayoutDefinition` (retained). |

## Layout — `Docking/Layout/`
| File | Role |
|------|------|
| `DockingLayoutController.cs` | Canonical dock-site edge layout engine; cached `DockLayoutResult`, `ContainerBounds`, `InvalidateLayout`, splitter hit-test + `DragSplitter`. |
| `DockLayoutResult.cs` | Immutable layout output: panel/group bounds + `DockSplitterHit` list. |
| `LayoutCalculator.cs` | Pure layout math (clamping, ratio-from-delta). |
| `LayoutValidator.cs` | Layout sanity checks. |

## Painters — `Docking/Painters/`
| File | Role |
|------|------|
| `IDockingElementRenderer.cs` | Element-renderer contract (paints from a context; stateless). |
| `DockingPainterContext.cs` | Resolved palette/style/state passed to renderers. |
| `DockingRendererSet.cs` | Bundle of element renderers for the active style/theme. |
| `DockingPainterFactory.cs` | Returns a `DockingRendererSet` by `BeepControlStyle`/theme. |
| `IDockingPainter.cs` | Painter contract consumed by the layout controller. |
| `DockingPainterAdapter.cs` | Bridges the renderer set onto the Beep painter framework. |
| `NullDockingPainter.cs` | No-op painter (default / design-time safe). |
| `Caption/CaptionRenderer.cs` | Paints caption background/border, title, tabs, caption buttons, overflow chevron. |
| `Splitter/SplitterRenderer.cs` | Paints splitter strips with hover/drag states. |
| `AutoHide/AutoHideStripRenderer.cs` | Paints auto-hide strip tabs/icons. |

## Layout managers — `Docking/Layoutmanagers/`
| File | Role |
|------|------|
| `CaptionLayoutManager.cs` | Caption/tab geometry + hit-testing; overflow-aware tab layout + chevron. |
| `AutoHideStripLayoutManager.cs` | Auto-hide strip tab geometry + hit-testing. |

## Runtime — `Docking/Runtime/`
| File | Role |
|------|------|
| `BeepDockSplitter.cs` | Live edge splitter; positioned by the engine, drags via `SplitterMoved`. |
| `AutoHideStrip.cs` | Per-edge auto-hide tab strip; hover-peek, delayed slide-back, pin/unpin. |
| `AutoHideSlidePanel.cs` | Animated slide-in panel for a peeked auto-hidden panel. |
| `FloatWindow.cs` | Borderless themed floating tool-window; custom caption, resize borders, edge snap, drag-to-redock. |
| `DockingGuideOverlay.cs` | 5-diamond drag-to-dock guide (SVG icons via renderers). |
| `DockingDropTarget.cs` | Hit-tests the guide overlay against the cursor. |
| `TabInteractionHandler.cs` | Tab click/close/double-click/reorder state (decoupled via activate callback). |
| `Animation/Easing.cs` | Easing functions. |
| `Animation/AnimationTrack.cs` | A single from→to tween (duration, onTick, onComplete). |
| `Animation/DockAnimator.cs` | Shared timer running multiple tracks. |
| `DragDrop/IDockDragHost.cs` | Host contract the drag controller drives. |
| `DragDrop/DockDragController.cs` | Start/update/commit/cancel of a dock drag. |
| `DragDrop/DockDragSession.cs` | Mutable per-drag state. |
| `DragDrop/DockDropResult.cs` | Resolved drop outcome (target + position). |
| `DragDrop/DockTargetResolver.cs` | Cursor → `DockDropResult`. |
| `DragDrop/DockGuideController.cs` | Drives the guide overlay during a drag. |
| `DragDrop/DockDragGhost.cs` | Translucent layered drag-preview window. |

## Helpers — `Docking/Helpers/`
| File | Role |
|------|------|
| `DockingCaptionPainter.cs` | Static caption/icon paint utility (`StyledImagePainter` + `SvgsUIcons`, vector fallback). |

## Design-time (in `...Design.Server/Docking/`)
| File | Role |
|------|------|
| `Designers/BeepDockingManagerDesigner.cs` | `ComponentDesigner`: verbs + action list + design-time child creation. |
| `Designers/DockPanelDesigner.cs` | `ControlDesigner` for a panel/tab page. |
| `Designers/BeepDockspaceDesigner.cs` | `ControlDesigner` for a docked tab cell. |
| `ActionLists/BeepDockingManagerActionList.cs` | Smart-tag items: add/move/stack panels, layout, Appearance (Style/Theme/UseThemeColors). |
| `ActionLists/DockPanelActionList.cs` | Per-panel smart-tag items. |
| `Infrastructure/BeepDockingDesignerWiring.cs` | `IDesignerHost.CreateComponent` + `IComponentChangeService` mutations → `*.Designer.cs`. |
| `Infrastructure/BeepDockingTypeRoutingProvider.cs` | Registers the docking designers with the design-tools host. |

---

## Removed in Phase 00 (do not recreate)
`Runtime/MdiPanelPositioner.cs`, `Runtime/PanelWindowManager.cs`, `Runtime/ContentHosting.cs`,
`Runtime/EventInterceptor.cs` (+ `DockingMouseEventHandler`), `Runtime/PainterIntegration.cs`,
`Runtime/WindowChrome.cs`, `Runtime/Phase4RuntimeExample.cs`, `Runtime/SplitterDragHandler.cs`,
`Runtime/PositioningUtilities.cs`, `Layout/SplitterManager.cs`, `Interop/MdiNativeApi.cs`,
`Interop/MdiConstants.cs`, `Interop/WindowBatchUpdater.cs`. The `Docking/Interop/` folder is empty.
