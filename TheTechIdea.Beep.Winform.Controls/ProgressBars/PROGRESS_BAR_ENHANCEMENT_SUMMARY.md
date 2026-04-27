## 2026 ProgressBar Enhancement Pass

### New Features
- **ProgressState enum**: `Normal`, `Indeterminate`, `Paused`, `Error` states with `ProgressState` property and `IsIndeterminate` convenience property
- **Indeterminate animation**: Spinning/wandering animation for Ring, DottedRing, RadialSegmented, RingCenterImage, and Linear painters when `ProgressState.Indeterminate`
- **Milestone events**: `MilestoneReached` event fires at configurable percentage thresholds (default: 25, 50, 75, 100). Configurable via `MilestoneThresholds` list or `SetMilestoneThresholds()`
- **StateChanged event**: Fires when `ProgressState` transitions between states
- **Step convenience API**: `SetSteps(int, string[])`, `SetCurrentStep(int)`, `SetStepLabels(string[])` for cleaner step-based painter configuration
- **ResetMilestones()**: Clears reached milestone tracking

### Bug Fixes
- **ArrowHeadAnimatedPainter state isolation**: Moved per-control animation state (timer, phase, loop position) to a handle-keyed dictionary so multiple progress bars sharing the same painter instance don't corrupt each other's animation state
- **ChevronStepsPainter last chevron**: Changed from triangle to flat-right rectangle for consistent visual termination
- **LinearBadgePainter/LinearTrackerIconPainter**: Reuse static `LinearProgressPainter` instance instead of allocating new painter on every Paint call

### Code Quality
- **DotsLoaderPainter**: Removed dead `registerLabel` no-op method
- **ProgressPainterState**: Added `State` (ProgressState) and `IndeterminateOffset` fields
- **ProgressPainterRegistry**: Marked Ring, DottedRing, RadialSegmented, RingCenterImage as supporting animation for indeterminate mode

### Painter Changes
- **LinearProgressPainter**: Renders 30%-width wandering bar for indeterminate mode
- **RingProgressPainter**: Spinning arc with pulsing sweep for indeterminate, shows ellipsis center text
- **DottedRingProgressPainter**: Rotating active dot cluster for indeterminate
- **RadialSegmentedPainter**: Rotating active segment cluster for indeterminate, ellipsis center text
- **RingCenterImagePainter**: Inherits indeterminate support from RingProgressPainter pattern
