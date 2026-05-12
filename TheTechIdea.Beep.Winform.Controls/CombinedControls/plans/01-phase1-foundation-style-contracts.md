# Phase 1 - Foundation, Style, And Contracts

Priority: High
Status: Planning Ready
Depends on: CombinedControls overview gap matrix

## Goal

Create a stable product foundation for both BeepChipListBox and BeepRadioListBox so later enhancements can be implemented without duplicating style logic or selection rules.

## Why This Phase First

The current code already has the right pieces: partial classes, style coordination, selection sync, and a shared control base. This phase turns those pieces into an explicit contract for state, tokens, and defaults.

## Scope

- shared control tokens and recommended defaults
- style preset rules for chip/list combinations
- unified state terminology for selected, hovered, focused, disabled, empty, and loading
- consistent initialization and guardrails for child-control setup
- documentation of the intended control roles for chip and radio variants

## Planned Workstreams

### W1 - Shared Tokens And Defaults

- Define token guidance for spacing, radius, icon size, search height, chip height, divider thickness, and density tiers.
- Set recommended defaults for each combined control so commercial-friendly layouts are easy to reach without custom code.
- Document which properties are primary style drivers and which are compatibility shims.

### W2 - Style Contract Hardening

- Formalize the style-coordination model between ListBoxType and ChipStyle.
- Define when preset-based coordination should win over per-property overrides.
- Make the recommended combinations obvious for list, chip, checkbox, avatar, language, and compact variants.

### W3 - Initialization And Layout Discipline

- Normalize initialization order for child controls, sync helper wiring, and event subscription.
- Document layout rules for search, chips, divider, and list placement.
- Make the controls resilient when sections are hidden or reordered.

### W4 - UX Baseline

- Align the initial appearance with modern component systems:
  - clear hierarchy
  - readable spacing
  - strong focus visibility
  - calm borders and separators
- Ensure empty surfaces do not look broken when one of the child zones is hidden.

## Exit Criteria For Phase 1

- Shared style and variant rules are documented.
- Default combinations are decided and explained.
- Initialization and layout rules are stable enough to support Phase 2 behavior work.

## Definition Of Done

- Both controls have a documented foundation for variants, states, and layout rules.
- Style coordination behavior is understandable from the plan without reading implementation internals.
- The TODO tracker can be used to execute Phase 2 without reopening the foundation scope.
