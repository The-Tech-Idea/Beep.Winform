# Phase 1 Validation Checklist

Status: Ready for execution
Scope: Foundation and core contracts

## Contract And Architecture

- [ ] CalendarTokens are the canonical source for layout constants.
- [ ] CalendarLayoutMetrics maps to CalendarTokens without behavior regressions.
- [ ] BeepCalendar command API routes toolbar and keyboard navigation paths.
- [ ] CommandInvoking cancellation prevents command execution.
- [ ] CommandInvoked fires after successful command execution.
- [ ] Render context exposes interaction state (hover/focus/visible-range).
- [ ] Month rendering uses shared day-cell state helper in both legacy and painter paths.

## Layout And Invalidation

- [ ] BeginVisualUpdate/EndVisualUpdate coalesces multiple layout/invalidate calls.
- [ ] DeferVisualUpdate scope flushes pending layout/redraw once on dispose.
- [ ] Resize path remains stable with no visible jitter.
- [ ] Theme apply path remains stable with no control overlap.

## Core Interaction

- [ ] Month view date select works by click.
- [ ] Keyboard navigation updates focused/selected date correctly.
- [ ] PageUp/PageDown uses command-driven period navigation.
- [ ] View switches (Month/Week/Day/List) keep layout consistent.

## Baseline Screenshot Matrix

Capture screenshots for each row and record file paths.

| Scenario | Required Shots | File Path | Status |
|---|---|---|---|
| Month view (Comfortable) | Default + selected date + today |  | Pending |
| Week view (Comfortable) | Empty grid + with events |  | Pending |
| Day view (Comfortable) | Time slots + event blocks |  | Pending |
| List view (Comfortable) | Multiple events list |  | Pending |
| Toolbar compact mode | Width < 720 behavior |  | Pending |
| Theme switch pass | Light/default theme + alternate theme |  | Pending |

## Notes

- Execute this checklist before starting Phase 2 implementation.
- For failed checks, link issue IDs in this file.
