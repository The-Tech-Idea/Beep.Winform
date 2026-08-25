# BeepCompanyProfile

A `BaseControl`-derived WinForms control that renders a company profile card in one of five
view variants. The control owns the data and the template child controls; painters own the
layout and drawing for each variant.

## File layout

| File | Responsibility |
|------|----------------|
| `BeepCompanyProfile.cs` | Core partial: fields, constructor, component initialization, layout orchestration, data properties, internal accessors. |
| `BeepCompanyProfile.Theme.cs` | `ApplyTheme()` — applies the `Company*` theme tokens to the template controls. |
| `BeepCompanyProfile.Drawing.cs` | `DrawContent` / `Draw` — fills the card background and delegates drawing to the active painter. |
| `BeepCompanyProfile.HitTesting.cs` | Hit-area registration and the click actions (open website, show dropdown). |
| `Models/CompanyProfileLayout.cs` | The computed layout rectangles shared by drawing and hit testing. |
| `Painters/ICompanyProfilePainter.cs` | Painter contract. |
| `Painters/CompanyProfilePainterBase.cs` | Shared drawing logic (standard and minimal views). |
| `Painters/*CompanyProfilePainter.cs` | One painter per `CompanyProfileViewType`. |

## View types

| View | Preferred size | Notes |
|------|----------------|-------|
| `Classic` | 400 × 350 | Default view. |
| `Compact` | 250 × 300 | Narrow card. |
| `Minimal` | 300 × 200 | Logo, centered name, profile button only. |
| `SocialCard` | 300 × 230 | Compact social-card layout with smaller buttons. |
| `Detailed` | 500 × 350 | Wider card; description area sized so all elements fit within bounds. |

## Design notes

- Template controls (`BeepLabel`, `BeepTextBox`, `BeepButton`, `BeepCircularButton`,
  `ContextMenuStrip`) are created in `InitializeComponents` but are never added to the
  `Controls` collection. Painters draw them via their `Draw(Graphics, Rectangle)` overloads
  and the control registers them as hit areas so hover/click mouse events are forwarded.
- Layout values are DPI-scaled through `DpiScalingHelper.ScaleValue`.
- Colors and fonts come exclusively from the `IBeepTheme` `Company*` tokens.
- The website and dropdown hit areas carry real actions; the logo/like/profile hit areas
  carry no action and rely on the forwarded component for hover feedback.
- `CompanyName` hides `Control.CompanyName` with the `new` keyword because it is a data
  property for this control.
