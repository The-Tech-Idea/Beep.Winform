# Wizards Enhancement — Master Todo Tracker

**Created:** 2026-07-08
**Target:** `TheTechIdea.Beep.Winform.Controls/Wizards/`
**Goal:** Elevate wizard framework to commercial-grade polish matching DevExpress WizardControl, Material Design 3 Stepper, and Ant Design Steps.

**Research basis:** Audited all 21 source files; surveyed Actipro WPF (10 animation types), MaterialDesignInXAML (7 transitions), Blazor.Wizard (lifecycle hooks), AeroWizard (OS-native theming), DevExpress XtraWizard, and 8+ open-source WinForms wizard projects.

---

## Phase Status Overview

| # | Phase | Tasks | Status | Key Deliverable |
|---|---|---|---|---|
| 1 | Smooth Transitions & Animation Engine | WZ-01–WZ-08 | `[ ]` | 12 easing functions, 4+ transition types |
| 2 | DPI Scaling & Responsive Layout | WZ-09–WZ-18 | `[ ]` | Zero hardcoded pixels, WizardFormBase |
| 3 | Progress & Navigation Enhancement | WZ-19–WZ-28 | `[ ]` | Progress bar, breadcrumb, clickable steps |
| 4 | Painter Expansion & Visual Polish | WZ-29–WZ-36 | `[ ]` | CardsPainter, animated connectors, icons |
| 5 | Design-Time Experience | WZ-37–WZ-43 | `[ ]` | Smart tags, collection editor, live preview |
| 6 | Accessibility & Keyboard UX | WZ-44–WZ-51 | `[ ]` | WCAG 2.2 AA, full keyboard nav, RTL |
| 7 | Advanced Features | WZ-52–WZ-63 | `[ ]` | Save/resume, branching, undo/redo, builder |

---

## All Tasks

### Phase 1: Animation Engine (8 tasks)
| ID | Task | Status | Complexity |
|---|---|---|---|
| WZ-01 | Expand WizardAnimationEngine — 12 easing functions | `[ ]` | Low |
| WZ-02 | Add TransitionType enum + TransitionDurationMs to WizardConfig | `[ ]` | Low |
| WZ-03 | Implement Fade transition (was a no-op stub) | `[ ]` | Medium |
| WZ-04 | Implement Zoom transition | `[ ]` | Medium |
| WZ-05 | Implement Flip/Wipe transitions | `[ ]` | Medium |
| WZ-06 | Route transition config through all 4 forms | `[ ]` | Low |
| WZ-07 | Add WizardManager.ReducedMotion static property | `[ ]` | Low |
| WZ-08 | Refactor animation dispatch into shared WizardTransitionEngine | `[ ]` | Medium |

### Phase 2: DPI & Layout (10 tasks)
| ID | Task | Status | Complexity |
|---|---|---|---|
| WZ-09 | DPI-scale VerticalStepperPainter (currently zero DPI scaling) | `[ ]` | Medium |
| WZ-10 | DPI-scale MinimalPainter (currently zero DPI scaling) | `[ ]` | Low |
| WZ-11 | DPI-scale WizardLayoutManagers (remove all const int, add DPI properties) | `[ ]` | Medium |
| WZ-12 | Wire forms to use layout managers (currently dead code) | `[ ]` | Medium |
| WZ-13 | Create WizardFormBase — extract ~300 lines shared across 4 forms | `[ ]` | High |
| WZ-14 | Add responsive threshold to layout managers | `[ ]` | Medium |
| WZ-15 | DPI-scale CardsWizardForm card painting | `[ ]` | Medium |
| WZ-16 | DPI-scale WizardStepTemplates (icons, fonts, padding) | `[ ]` | Medium |
| WZ-17 | Fix font disposal pattern in WizardStepTemplates | `[ ]` | Low |
| WZ-18 | Add OnResize recalculation to all forms | `[ ]` | Low |

### Phase 3: Progress & Navigation (10 tasks)
| ID | Task | Status | Complexity |
|---|---|---|---|
| WZ-19 | Implement progress bar rendering (ShowProgressBar currently ignored) | `[ ]` | Medium |
| WZ-20 | Animate progress bar fill on step transitions | `[ ]` | Low |
| WZ-21 | Add "Step X of Y" label to all forms | `[ ]` | Low |
| WZ-22 | Implement non-linear step navigation (click completed step to jump) | `[ ]` | Medium |
| WZ-23 | Add Breadcrumb wizard style + BreadcrumbWizardForm | `[ ]` | Medium |
| WZ-24 | Add loading state during async transitions (ShowLoading/HideLoading) | `[ ]` | Medium |
| WZ-25 | Expose IsNavigating as public read-only property | `[ ]` | Low |
| WZ-26 | Add tooltips to step indicators | `[ ]` | Low |
| WZ-27 | Configurable cancel confirmation (message + enable/disable) | `[ ]` | Low |
| WZ-28 | Add NavigationDirection to step event args | `[ ]` | Low |

### Phase 4: Painter Expansion (8 tasks)
| ID | Task | Status | Complexity |
|---|---|---|---|
| WZ-29 | Create CardsPainter — extract inline painting from CardsWizardForm | `[ ]` | High |
| WZ-30 | Add animated connector fill (lines animate grey→color on step complete) | `[ ]` | Medium |
| WZ-31 | Add error-state connector coloring | `[ ]` | Low |
| WZ-32 | Standardize IWizardPainter.PaintStepIndicators across all painters | `[ ]` | Medium |
| WZ-33 | Add radial progress mode for Minimal style | `[ ]` | Medium |
| WZ-34 | Add themed gradient + shadow to step circles | `[ ]` | Medium |
| WZ-35 | Implement step icon rendering from WizardStep.Icon (SVG) | `[ ]` | Medium |
| WZ-36 | Add step description rendering in HorizontalStepper | `[ ]` | Low |

### Phase 5: Design-Time (7 tasks)
| ID | Task | Status | Complexity |
|---|---|---|---|
| WZ-37 | Create WizardStepCollectionEditor (add/remove/reorder) | `[ ]` | Medium |
| WZ-38 | Create smart tag (DesignerActionList) for wizard forms | `[ ]` | Medium |
| WZ-39 | Enhance design-time sample data with realistic step names | `[ ]` | Low |
| WZ-40 | Live theme switching in designer | `[ ]` | Low |
| WZ-41 | Add WizardPage to toolbox with toolbox bitmap | `[ ]` | Low |
| WZ-42 | Design-time serialization of step content | `[ ]` | Low |
| WZ-43 | Design-time validation warning when Steps.Count == 0 | `[ ]` | Low |

### Phase 6: Accessibility (8 tasks)
| ID | Task | Status | Complexity |
|---|---|---|---|
| WZ-44 | Full keyboard shortcuts (Ctrl+N/B/Enter, Left/Right, Home/End, F1) | `[ ]` | Low |
| WZ-45 | Screen reader support (AccessibleName, step change announcements) | `[ ]` | Medium |
| WZ-46 | Focus ring on step indicators + arrow key navigation | `[ ]` | Medium |
| WZ-47 | High contrast mode (SystemInformation.HighContrast detection) | `[ ]` | Medium |
| WZ-48 | RTL support (RightToLeft property, mirror layout) | `[ ]` | High |
| WZ-49 | Minimum 44px touch targets on all step indicators | `[ ]` | Low |
| WZ-50 | Keyboard shortcut help overlay (F1) | `[ ]` | Low |
| WZ-51 | Focus trapping within modal wizard | `[ ]` | Low |

### Phase 7: Advanced Features (12 tasks)
| ID | Task | Status | Complexity |
|---|---|---|---|
| WZ-52 | WizardContext JSON serialization (SaveState/RestoreState) | `[ ]` | Medium |
| WZ-53 | Auto-save on step transition | `[ ]` | Low |
| WZ-54 | Resume workflow (WizardManager.ResumeWizard) | `[ ]` | Medium |
| WZ-55 | Branching navigation: WizardStep.BranchCondition delegate | `[ ]` | Medium |
| WZ-56 | Branch step model: WizardBranchStep with Branches list | `[ ]` | High |
| WZ-57 | Undo last step (UndoLastStepAsync) | `[ ]` | Medium |
| WZ-58 | Redo last undone step | `[ ]` | Medium |
| WZ-59 | Breadcrumb navigation mode (click any step title to jump) | `[ ]` | Medium |
| WZ-60 | Data export/import (ExportData/ImportData on WizardContext) | `[ ]` | Low |
| WZ-61 | Conditional step visibility (VisibleCondition delegate) | `[ ]` | Medium |
| WZ-62 | Theme preference persistence across wizards | `[ ]` | Low |
| WZ-63 | Step completion history tracking (timestamps, validation errors) | `[ ]` | Medium |

---

## Dependency Graph

```
Phase 1 (Animation)
  └─→ Phase 2 (DPI/Layout, needs refactored forms from WZ-08)
        └─→ Phase 3 (Navigation/Progress)
              ├─→ Phase 6 (Accessibility, needs keyboard infra from WZ-44)
              └─→ Phase 7 (Advanced, needs navigation infra from WZ-22/28)
        └─→ Phase 4 (Painters)
              └─→ Phase 5 (Design-Time)
```

---

## File Creation/Modification Matrix

### New Files (10)
| File | Phase |
|---|---|
| `Forms/WizardFormBase.cs` | 2 |
| `Forms/BreadcrumbWizardForm.cs` | 3 |
| `Painters/CardsPainter.cs` | 4 |
| `Helpers/WizardTransitionEngine.cs` | 1 |
| `Helpers/WizardIconRenderer.cs` | 4 |
| `Helpers/WizardThemeHelper.cs` | 4 |
| `Design/WizardStepCollectionEditor.cs` | 5 |
| `Design/WizardActionList.cs` | 5 |
| `Design/WizardFormDesigner.cs` | 5 |
| `Core/WizardBuilder.cs` | 7 |

### Modified Files (17)
| File | Phases |
|---|---|
| `Helpers/WizardAnimationEngine.cs` | 1 |
| `Helpers/WizardHelpers.cs` | 1, 3, 6 |
| `Core/WizardModels.cs` | 1–7 |
| `Core/WizardEnums.cs` | 1, 3, 6 |
| `Core/WizardInstance.cs` | 3, 7 |
| `Core/WizardManager.cs` | 1, 7 |
| `Core/IWizardFormHost.cs` | 1, 3, 6 |
| `Core/WizardContext.cs` | 7 |
| `Core/WizardFormFactory.cs` | 3 |
| `Layout/WizardLayoutManagers.cs` | 2, 3, 6 |
| `Painters/HorizontalStepperPainter.cs` | 2, 4, 6 |
| `Painters/VerticalStepperPainter.cs` | 2, 4, 6 |
| `Painters/MinimalPainter.cs` | 2, 4, 6 |
| `Templates/WizardStepTemplates.cs` | 2 |
| `Forms/HorizontalStepperWizardForm.cs` | 1–6 |
| `Forms/VerticalStepperWizardForm.cs` | 1–6 |
| `Forms/MinimalWizardForm.cs` | 1–6 |
| `Forms/CardsWizardForm.cs` | 1–6 |
| `Forms/WizardPage.cs` | 2, 5 |

---

## Verification Checklist

- [ ] `dotnet build` — zero errors through all phases
- [ ] DPI test: 96/125/150/200% — all styles render correctly
- [ ] Animation test: slide, fade, zoom transitions at 60fps
- [ ] Keyboard test: all shortcuts work in all 4 styles
- [ ] Theme test: Light → Dark switch, all colors update
- [ ] High contrast: Windows HC Black theme renders correctly
- [ ] Save/resume: state round-trips through JSON
- [ ] Branching: conditional navigation routes correctly
- [ ] Touch targets: all step indicators ≥44px at 96 DPI
