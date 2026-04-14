# BeepListBoxDesigner Plan

Status: partially completed in this pass.

Implemented:
- Fixed the smart-tag search toggle to bind to the real runtime property `ShowSearch` instead of the nonexistent `EnableSearch` name.
- Expanded the smart-tag surface to include list style, selection mode, grouping, hierarchy, density, loading state, search placeholder, and real `DataSource` / `DisplayMember` / `ValueMember` properties.
- Added presets for searchable, navigation, and grouped list configurations.

Remaining follow-up:
- Shared `DataControlActionList` cleanup is complete for the current surface: the shared action list now only carries binding properties plus `Clear Data Binding`, while sample-data and richer configuration flows stay on the individual designers that actually implement them.