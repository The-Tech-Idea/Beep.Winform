# External Benchmark And UX Standards Notes

Purpose: capture externally researched scheduler features and translate them into concrete BeepCalendar expectations.

## Sources Reviewed

- FullCalendar docs: https://fullcalendar.io/docs
- React Big Calendar repository: https://github.com/jquense/react-big-calendar
- TOAST UI Calendar repository: https://github.com/nhn/tui.calendar
- DevExpress WinForms Scheduler page: https://www.devexpress.com/products/net/controls/winforms/scheduler/
- Telerik WinForms Scheduler page: https://www.telerik.com/products/winforms/scheduler.aspx
- Syncfusion WinForms Scheduler page: https://www.syncfusion.com/winforms-ui-controls/scheduler
- Material 3 Date Pickers guidance: https://m3.material.io/components/date-pickers/overview

Note: Fluent 2 calendar usage URL returned unavailable at fetch time.

## Feature Benchmarks To Mirror

### Core Views And Navigation

- Day, week, work-week, month, and list/agenda views are baseline expectations.
- Timeline views are expected for enterprise scheduling scenarios.
- Quick navigation patterns: today, previous/next, compact navigator patterns.

### Event Workflows

- Drag and resize interactions are standard.
- Recurrence support must include daily/weekly/monthly/yearly patterns.
- Editing a single occurrence in a recurrence series is expected.
- In-place quick edit plus full dialog edit is common in mature products.

### Data And Interop

- Data binding flexibility is expected (objects, data sources, custom fields).
- ICS import/export support is a high-value interoperability feature.
- External sync patterns (Outlook/M365/Google) are premium-level capabilities.

### Resources And Grouping

- Resource lanes (people, rooms, assets) and grouping by resource/date are common.
- Multiple schedules visible at once is expected in enterprise apps.

### UX And Theming

- Strong theming support and modern appointment style treatment are expected.
- Material 3 guidance emphasizes larger touch targets and reduced visual noise.
- Figma-style discipline implies reusable tokens, explicit states, and layout consistency.

### Accessibility And Globalization

- Keyboard navigation parity is expected across views.
- Localization and RTL support are standard commercial requirements.
- Timezone and business-hours policies are critical for global teams.

### Performance And Scale

- Large event set responsiveness is a core requirement.
- High-DPI fidelity and smooth scrolling are product differentiators.
- Virtualization and targeted invalidation are common implementation strategies.

## Translation To BeepCalendar Phases

- Phase 1: tokenization, contract consistency, command surface
- Phase 2: event model depth and interaction primitives
- Phase 3: views/resources/performance
- Phase 4: integrations/accessibility/localization/enterprise QA

## Non-Goals For Initial Commercial Cut

- Full parity with all premium scheduler features in first release.
- Provider-specific sync implementation for every external calendar vendor in phase 1.
- Heavy skin/template editors before core interaction and accessibility parity are complete.
