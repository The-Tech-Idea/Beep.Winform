# BeepComboBoxDesigner Plan

Status: partially completed in this pass.

Implemented:
- Fixed the smart-tag multi-select binding to the actual runtime property `AllowMultipleSelection`.
- Expanded the smart-tag surface to include `ComboBoxType`, placeholder text, search, auto-complete, free-text, loading state, and presets for searchable picker, chip multi-select, and command-menu layouts.

Remaining follow-up:
- If the runtime control later gains true `DataSource` / `DisplayMember` / `ValueMember` support, add a shared data-control action list rather than another one-off combo-box expansion.