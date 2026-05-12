# Phase 3 - File-By-File Implementation Matrix

This matrix converts the Phase 3 checklist into file-specific visual, accessibility, and performance work.

## Execution Order

1. [ChipListBoxStyleCoordinator.cs](../ChipListBoxStyleCoordinator.cs)
2. [BeepChipListBox.Drawing.cs](../BeepChipListBox.Drawing.cs)
3. [BeepChipListBox.Properties.cs](../BeepChipListBox.Properties.cs)
4. [BeepRadioListBox.Properties.cs](../BeepRadioListBox.Properties.cs)
5. [BeepChipListBox.Core.cs](../BeepChipListBox.Core.cs)
6. [BeepRadioListBox.Core.cs](../BeepRadioListBox.Core.cs)

## Matrix

| File | Phase 3 responsibility | Specific work | Notes |
|---|---|---|---|
| [ChipListBoxStyleCoordinator.cs](../ChipListBoxStyleCoordinator.cs) | Visual variant system | Document which mappings are the official commercial presets and which are niche styles. | This is the shared visual language authority for the control family. |
| [BeepChipListBox.Drawing.cs](../BeepChipListBox.Drawing.cs) | Focus, hover, and visual polish | Review focus handling, section effects, and visual state update behavior for commercial-grade polish. | This is the main place to document visual feedback quality for the chip control. |
| [BeepChipListBox.Properties.cs](../BeepChipListBox.Properties.cs) | Accessibility-facing property clarity | Review style, layout, selection, and visibility properties so the public surface reads clearly for users and assistive-tech documentation. | This file contains the public-facing knobs that shape the visual story. |
| [BeepRadioListBox.Properties.cs](../BeepRadioListBox.Properties.cs) | Accessibility and variant clarity | Review radio style, orientation, layout, and visibility properties so the control reads like a polished preset-driven selector. | This is the property contract most likely to surface in docs and samples. |
| [BeepChipListBox.Core.cs](../BeepChipListBox.Core.cs) | Performance and layout efficiency | Confirm the child-control wiring, layout sequence, and update flow do not create unnecessary relayout or repaint churn. | Foundation matters here because visual polish should not cost stability. |
| [BeepRadioListBox.Core.cs](../BeepRadioListBox.Core.cs) | Performance and layout efficiency | Confirm the radio/list composition remains stable under dense content and frequent selection changes. | The core file owns the structural path for layout and update behavior. |

## Phase 3 Deliverables By File

- `ChipListBoxStyleCoordinator.cs`: documented visual preset authority.
- `BeepChipListBox.Drawing.cs`: focus/hover/section polish notes.
- `BeepChipListBox.Properties.cs`: accessibility-friendly public contract notes.
- `BeepRadioListBox.Properties.cs`: polished variant and layout contract notes.
- `BeepChipListBox.Core.cs`: update and layout efficiency notes.
- `BeepRadioListBox.Core.cs`: dense-state stability notes.
