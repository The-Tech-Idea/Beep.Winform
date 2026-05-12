# CombinedControls External Benchmark And UX Standards

Priority: High
Status: Planning Approved

## Benchmark Notes

### DevExpress-inspired expectations

- Strong state clarity for selected, focused, and disabled items.
- Dense but readable editors that still feel polished at small sizes.
- Explicit preset behavior instead of implicit visual drift.
- Reliable DPI behavior and consistent spacing.

### GitHub and open-source control patterns

- Separate state, layout, and paint responsibilities.
- Keep interaction logic predictable and inspectable.
- Prefer composition and small helpers over monolithic event handlers.

### Figma-style UX standards

- Use tokens for spacing, radius, and typography.
- Keep variant names meaningful and limited.
- Design empty, loading, and error states deliberately.
- Make component states visible in the design system, not only in code.

## CombinedControls UX Rules

- The control family should always show a clear hierarchy: container, search, chips or radio affordance, and list surface.
- Selection should be obvious without becoming visually heavy.
- Styling should favor calm borders, readable spacing, and consistent hit targets.
- Control variants should be easy to understand from their names and defaults.
