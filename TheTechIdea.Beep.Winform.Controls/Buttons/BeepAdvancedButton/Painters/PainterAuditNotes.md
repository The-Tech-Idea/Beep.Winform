# BeepAdvancedButton Painter Audit Notes

## Applied normalization pass

- Introduced shared focus-visible primitive in `BaseButtonPainter` and applied it to refreshed core/specialized families.
- Added DPI-aware metrics hookup (`OwnerControl` + scaled `AdvancedButtonMetrics`) to reduce hardcoded pixel drift.
- Added `AdvancedButtonPaintContract` for shared token and layout slice normalization.
- Updated gradient painter to derive stop colors from token blend instead of fixed RGB subtraction.
- Updated toggle split icon geometry to use metrics-driven icon sizes/padding.
- Updated neon painter icon rendering to use the shared icon pipeline instead of internal primitive shape-only rendering.

## Remaining optional follow-up

- Continue replacing hardcoded colors in specialty/news variants with theme token routes.
- Collapse repeated geometry constants in specialty families into helper contracts.
- Expand per-variant visual snapshots to lock hover/pressed/focus parity.
