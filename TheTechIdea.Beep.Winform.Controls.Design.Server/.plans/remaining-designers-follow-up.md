# Remaining Designer Follow-Up

High-value next targets after this pass:
- Keep reusable smart-tag infrastructure in `ActionLists/` when more than one designer consumes it; keep action-list classes inline beside a designer only when they are truly control-specific.
- The widget preset sweep is complete for the current widget designer set; only revisit this area when new widget controls land or existing runtime controls gain new meaningful scalar properties.
- Audit other custom container designers for design-surface selection handoff only when they own child creation or destruction behavior similar to `BeepTabsDesigner`; `BeepDocumentHostDesigner` already touches the selection service and does not currently need the same follow-up.
- When a designer exposes custom verbs, prefer explicit helper methods plus `override Verbs` instead of invoking `_designer.Verbs[index]` from smart-tag actions; the toggle, switch, and extended-button designers were corrected to use that pattern.
- Keep icon-picking designers aligned to the runtime property contract instead of placeholder property names, and prefer preset helpers that set any explicit helper-backed companion sizing values together with the style itself.
- For toggle-family controls, icon presets should also activate the icon-capable runtime style when the control requires one, rather than only setting icon paths.
- For controls whose runtime style setter already applies recommended metrics, keep designer presets thin and route them through that setter instead of duplicating hardcoded size or spacing tables; `BeepDockDesigner` now follows that rule and only layers layout choices on top.
- For rich painter-driven controls like `BeepCard`, prefer smart-tag properties and presets that target the fields each painter actually reads (`SubtitleText`, status, rating, badges, secondary button state, accent color) rather than only exposing the shared header/paragraph/button trio.
- Remove or implement placeholder smart-tag commands when they stop reflecting reality; `BeepGridProDesigner.GenerateSampleData` is now a real reversible preview action instead of a stub comment.