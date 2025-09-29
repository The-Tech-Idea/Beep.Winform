# BeepStatCard refactor plan

Goal: Convert `BeepStatCard` to match the methodology used by `BeepAppBar`.

Key decisions
- Use BaseControl as the base. Use `DrawingRect` as the drawing area.
- Convert `BeepStatCard` to partial class and use a helper/painter pattern.
- Create a painter interface and multiple concrete painters inspired by provided images.
- Use a registry to map enum -> painter and allow easy extension.
- Use `Dictionary<string, object>` for painter parameters. Keep simple model properties for convenience (HeaderText, ValueText, InfoText, Icon, IsTrendingUp, etc.).
- Apply theme from `_currentTheme` exactly like `BeepAppBar`, using Card colors/typography styles.
- Do not embed child controls; paint directly in `DrawContent(Graphics g)`.

Files to add
1) `StatusCards/StatCardPainterKind.cs` — enum selecting painter.
2) `StatusCards/Painters/IStatCardPainter.cs` — painter interface.
3) `StatusCards/Painters/BaseStatCardPainter.cs` — common helpers (text measuring, colors, fonts from theme).
4) `StatusCards/Painters/SimpleKpiPainter.cs` — minimal KPI card: header, main value, delta, small sparkline (3rd image set inspiration).
5) `StatusCards/Painters/HeartRatePainter.cs` — vertical rounded bars with small axis and labels (1st image, Heart Rate).
6) `StatusCards/Painters/EnergyActivityPainter.cs` — grouped horizontal bars (1st image, Avg. Energy Activity).
7) `StatusCards/Painters/PerformancePainter.cs` — center line/dot with dotted background and year ticks (2nd image middle inspiration). Simplified for now.
8) `StatusCards/BeepStatCard.PainterRegistry.cs` — registry that maps enum -> painter and allows custom registration.
9) `StatusCards/BeepStatCard.Core.cs` — main properties, theming, parameter bag, SelectedPainter property, overrides to paint via painter.

Backward compatibility
- Keep previous public properties: `HeaderText`, `PercentageText`, `ValueText`, `TrendText`, `InfoText`, `IsTrendingUp`, `TrendUpSvgPath`, `TrendDownSvgPath`, `Icon`.
- Bindable `Parameters` bag (Dictionary<string,object>).

Painting contract
- Painters receive: Graphics g, Rectangle bounds (owner.DrawingRect inset with Padding), IBeepTheme theme, BeepStatCard owner, IReadOnlyDictionary<string,object> parameters, and a small DTO of common strings.

Parameters (suggested common keys)
- "Series" -> IList<float/double> or float[]
- "Series2" -> IList<float/double>
- "Labels" -> string[]
- "Delta" -> float/double
- "Unit" -> string
- "Spark" -> IList<float/double> (for SimpleKpi)
- "Min","Max" -> double
- "PrimaryColor","SecondaryColor" -> Color (optional overrides)

Theme usage
- Card background/foreground: `_currentTheme.CardBackColor`, `_currentTheme.CardTextForeColor`, header font from `_currentTheme.CardHeaderStyle`, paragraph from `_currentTheme.CardparagraphStyle`, small text from `_currentTheme.SmallText`.
- Respect owner `UseGradientBackground` if set; otherwise painter draws content on top of already painted inner shape.

Extensibility
- New painters implement `IStatCardPainter` and register via `BeepStatCard.RegisterPainter(kind, painter)` or `RegisterPainter(string key, painter)` overload (optional).

Events
- None added in phase 1. Redraw on property set and size change.

Performance
- Use anti-aliasing and TextRenderer for crisp text. Avoid allocations in paint loop.

Testing
- Default painter: SimpleKpiPainter.
- Provide default demo parameters in control constructor for design-time view.
