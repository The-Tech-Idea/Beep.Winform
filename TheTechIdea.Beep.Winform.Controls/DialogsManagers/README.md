# Beep Dialog Manager (DialogsManagers)

Rich, theme-aware dialogs, drawers, sheets, progress, and toasts for BeepiForm-based WinForms apps. All APIs are async-friendly, support stacking, and integrate with Beep theme typography/colors.

Features
- Async modal dialogs with overlay and smooth animations
- Placements: `Centered`, `TopSheet`, `LeftDrawer`, `RightDrawer`
- Helpers: `MessageAsync`, `ConfirmAsync`, progress wrapper
- Toast notifications with queuing and auto-hide
- Keyboard UX: `Esc` to close, `Enter` triggers default button
- Stacks multiple dialogs safely; resizes with host form
- Theme-aware (auto-updates via ThemeAccess)

Requirements
- Host form must be a `BeepiForm` (implements `IBeepModernFormHost`)
- .NET 8+, C# 12

Architecture overview
- `BeepDialogManager`: entry point scoped to a host `BeepiForm`. One manager per form is cached.
- `_DialogCard`: visual container for a dialog (title, body, footer buttons, validation area).
- `_OverlayPanel`: dimming overlay; animated opacity; click-to-dismiss (optionally).
- `BeepToastHost` + `BeepToastCard`: toast queue + toast cards; manages layout and animations.
- `ThemeAccess`: resolves themed fonts/colors for labels and buttons.

File inventory
- `BeepDialogManager.cs`: core show/close logic, animations, stacking, keyboard handling.
- `BeepDialogManager.Theme.cs`: live re-theme wiring between form and dialogs/toasts.
- `BeepDialogManager.Toast.cs`: toast host integration and API (`ShowToast`).
- `BeepDialogManager.Progress.cs`: progress dialog helper (`RunWithProgressAsync`).
- `_DialogCard.cs`: builds header/body/footer, buttons, validation, paints the card.
- `_OverlayPanel.cs`: semi-transparent overlay with alpha.
- `BeepDialogOptions.cs`: dialog configuration (kind, padding, radius, animation, etc.).
- `BeepDialogContent.cs`: simple content contract (title, message, buttons, optional custom body, optional validator).
- `BeepDialogButton.cs`: text/result/default/cancel flags.
- `BeepToastHost.cs`: manages active toasts and layout.
- `BeepToastCard.cs`: material-like toast card with title/body/action.
- `BeepToastOptions.cs`: toast configuration (kind, message, duration, action).
- `Enums.cs`: `BeepDialogKind`, `BeepDialogResult`, `BeepToastKind`.
- `ValidationState.cs`: validator result (`IsValid`, `Errors`).

Key types and APIs
- `BeepDialogManager.For(Control control)`: get/create a manager for the owning `BeepiForm`.
- `Task<BeepDialogResult> ShowAsync(BeepDialogContent content, BeepDialogOptions options, CancellationToken token = default)`
- `Task<BeepDialogResult> MessageAsync(string title, string message, string okText = "OK", BeepDialogOptions? options = null, CancellationToken token = default)`
- `Task<BeepDialogResult> ConfirmAsync(string title, string message, string okText = "OK", string cancelText = "Cancel", BeepDialogOptions? options = null, CancellationToken token = default)`
- `Task<bool> RunWithProgressAsync(string title, Func<IProgress<int>, CancellationToken, Task> operation, string cancelText = "Cancel", BeepDialogOptions? options = null, CancellationToken token = default)`
- `void ShowToast(BeepToastOptions options)`

Options
- `BeepDialogOptions`
  - `Kind`: `Centered` | `TopSheet` | `LeftDrawer` | `RightDrawer`
  - `DismissOnOverlayClick` (bool, default false)
  - `EscToClose` (bool, default true)
  - `BorderRadius` (int), `BorderThickness` (int)
  - `AnimationMs` (int, default 220)
  - `MaxSize` (Size), `ContentPadding` (Padding)
- `BeepToastOptions`
  - `Title` (string?)
  - `Message` (string)
  - `Kind`: `Info` | `Success` | `Warning` | `Error`
  - `DurationMs` (default 3500)
  - `ActionText` (string?), `Action` (Action?)
  - `MaxWidth` (default 360), `Margin` (Padding)

Dialog lifecycle
1) Create `_OverlayPanel` and `_DialogCard` and add to the host `BeepiForm`.
2) Compute start/end bounds based on `BeepDialogKind`.
3) Animate in (position/opacity), focus-trap into `_DialogCard` first control.
4) Await a `TaskCompletionSource` that resolves on close.
5) Animate out, dispose and pop from stack.

Keyboard and focus
- `Esc` closes when `EscToClose` is true.
- `Enter` triggers the default button via `_DialogCard.TryTriggerDefault()`.
- Focus starts on the first tabbable child in the card.

Toasts
- Enqueue via `BeepDialogManager.ShowToast(options)`.
- Up to 3 cards visible; queue overflow is processed when a slot frees.
- Auto-hide after `DurationMs`.

Theming
- Typography and colors flow from `ThemeAccess` using the host form theme.
- `ApplyThemeFrom` on cards and hosts updates fonts and colors live.

Notes
- Always obtain the manager via `BeepDialogManager.For(this)` from a control hosted by a `BeepiForm`.
- Use `CancellationToken` for long operations (progress helper cancels the task if dialog is canceled).
