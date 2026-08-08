# Wizards — review & enhancement plan (2026-08)

23 files, ~7,300 lines. This IS the framework the repo's standing rule mandates ("no hand-rolled
stage machines"), so its own health matters doubly. `WizardManager` (static registry) →
`WizardInstance` (async navigation: `NavigateNextAsync/BackAsync/CompleteAsync`, events
StepChanging/Changed/Completed/Cancelled, auto-save) → 4 form hosts (Minimal, Cards,
Horizontal/VerticalStepper) + painters, transition engine, validators, templates.

Healthiest folder reviewed so far: 0 empty swallows, 10 literal colours, coherent async design.

## Findings

### F1 — 9 catches handle-but-never-report (0 BeepLog references in the folder)

| site | behaviour | fix |
|---|---|---|
| `WizardInstance:408` auto-save | swallowed "best-effort" — a failed auto-save silently loses user progress | `Failure` |
| `WizardInstance:426` resume | `return false` unreported | `Failure` |
| `WizardInstance:498` | NotImplementedException → sync validate fallback (designed protocol) | `FallbackOnce` per step |
| `WizardModels:371` GetValue<T> | `return defaultValue` on conversion failure | `WarnOnce` per key |
| `WizardModels:464,472` save | skip non-serializable values | `WarnOnce` per key |
| `WizardHelpers:134` anim timer | stop+dispose+return | `FailureOnce` |
| `TransitionEngine:77,138,207` | capture fails → dispose bitmaps → `onComplete()` = instant switch (good fallback) | `FallbackOnce` |

(The forms' MessageBox catch is fine — it reports to the user.)

### F2 — 10 literal colours (standing rule: theme slots only)

HorizontalStepperPainter ×4, WizardStepTemplates ×3, VerticalStepperPainter ×2, WizardHelpers ×1.

### F3 — CS8618 `_errorPanel` warnings in all four forms (from global build output)

Non-nullable fields not set in constructor — nullability smell to check.

### F4 — no probe

Planned: drive `WizardInstance` directly — config with 3 steps, `NavigateNextAsync` walks forward
and raises StepChanged, `StepChanging.Cancel` gates navigation, `NavigateBackAsync`, `CompleteAsync`
raises Completed, cancel path; plus one form host smoke test (Show non-modal, render, blank-guard).

## Order

F1+F2 (mechanical) → F4 probe → F3 if real → commit per batch on master.

## Batch 1 done - probe 11/11, sizing fixed

F1: all 12 unreported catches report (auto-save Failure - it silently lost user progress; transition
capture x4 FallbackOnce; typed-get/serialize WarnOnce per key; sync-validate FallbackOnce per type).
F2: 10 literals -> theme slots (OnPrimaryColor for on-fill ink, ShadowColor, ErrorColor, SuccessColor;
GetErrorColor collapsed to the always-a-theme rule). F4 probe: navigation fwd/back, StepChanging.Cancel
gates, Completed/Cancelled raised, registry round-trip, MinimalWizardForm hosted render + navigation -
11/11 first run; the framework core is the healthiest reviewed.

Sizing (user directive): the step title rendered clipped in half. Two stacked causes: the 60px header
band could not hold dots(top 25)+title(y40..65), AND the title's own 25px text rect clipped any title
font taller than 25 - DrawText clips to its rect, so growing the band alone changed nothing. Painter
title rect now sized from _titleFont.Height and anchored to the band bottom (fits any band height);
band 60->72. Verified by render: title fully visible.

Open: Cards/Horizontal/Vertical stepper forms not render-eyeballed (heights 70 look adequate, painters
share the font-height fix pattern only in Minimal); CS8618 _errorPanel nullability untouched.
