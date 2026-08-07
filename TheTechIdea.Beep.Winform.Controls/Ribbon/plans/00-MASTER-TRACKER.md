# Ribbon — review tracker

`BeepRibbonControl` is 18 partial files plus Accessibility, Backstage, Customization, Gallery,
Rendering, Search, Tokens and Tooltips subfolders. 42 files, ~11,300 lines after the deletions below.

## Done

### Two full backup copies of the main control were in the source tree

`BeepRibbonControl.cs.backup` and `BeepRibbonControl.cs.bk` — byte-identical, 5,410 lines each,
10,820 lines between them, against 11,292 lines of live code. Both were tracked by git and referenced
by nothing. Deleted: `CLAUDE.md` rule 2 says delete the old thing rather than keep it beside the new
one, and git already holds the history.

They were not harmless. Both still contained the pre-split implementation, so a search across the
folder returned two stale hits for every real one — which is how they were found: a scan for silent
catches reported 34 sites, 20 of them in files that are never compiled.

### 15 silent catches now report through `BeepLog`

Every one was a bare `catch { }` or a `catch` whose body was only a comment. By file:

| file | sites | what was being hidden |
|---|---|---|
| `.Events.cs` | 4 | theme subscribe/unsubscribe, theme application, design-time build |
| `.Customization.cs` | 4 | save/load customization, save/load theme tokens |
| `.Search.cs` | 4 | async search, provider failure, save/load history |
| `.QuickAccess.cs` | 2 | save/load the quick access toolbar |
| `.Backstage.cs` | 1 | a consumer-supplied timestamp formatter throwing |
| `RibbonTheme.cs` | 1 | reading the current Beep theme |

Chosen deliberately per site rather than mechanically:

- **`Failure`** where the operation is lost — a save that writes nothing, a theme that never applies.
- **`Fallback`** where a degraded path really does succeed: the design-time placeholder, the built-in
  timestamp format, the local search index standing in for a failed provider.
- **`FallbackOnce`** in `RibbonTheme.SyncFromBeepTheme`, which runs on every theme resolve. The
  fallback is legitimate during early startup before the theme manager has a current theme, so a
  per-call message would bury a real failure.

Three were worth more than a mechanical fix:

- **`UnsubscribeThemeManager`** was commented `// no-op`. `BeepThemesManager.ThemeChanged` is static,
  so a failed detach keeps the ribbon alive for the life of the process. That is a leak.
- **`TrySubscribeThemeManager`** was commented `// best effort only`. On failure `_subscribedToThemeManager`
  stays false, so every later call retries and fails identically, and the ribbon never follows a theme
  change again.
- **The search provider catch** already raised `providerFailed: true` on its event. A boolean says
  only *that* it failed; the provider is the consumer's own code, so it now gets the exception too.

Verified: the multiline pattern `catch\s*(\([^)]*\))?\s*\{\s*(//[^\n]*\s*)*\}` finds 0 in `Ribbon/`
and still finds 20 in `SideBar/`, so the zero is a result rather than a pattern that matches nothing.

A note on that pattern, because it cost a wrong conclusion first: a single-line
`grep "catch\s*{\s*}"` reported **0 silent catches in Ribbon** and was believed for several minutes.
Every catch in this folder puts its brace on the following line, so the pattern could not have matched
any of them. It had to be multiline.

## Not examined

Recorded so this does not read as a finished review:

- **54 literal colour references** (`Color.FromArgb`, `Color.White`…) across the folder. Not yet
  checked against rule 3 — some will be legitimate semantic or fallback colours, some may not be.
- **Composition vs hand-painting** (rule 4). `Rendering/BeepRibbonPainter.cs` and
  `Gallery/RibbonGalleryRenderer.cs` paint directly; whether that is warranted here is unassessed.
- **No behavioural probe has been written**, so nothing in this folder has been verified by running it.
  Everything above is a static finding plus a clean build. Layout, keyboard/KeyTips, backstage,
  contextual tabs, minimise/restore and the gallery are all unexercised.

## Standing constraints

Per `CLAUDE.md`: report every catch through `BeepLog`; no stubs or legacy paths; nothing assigns
colours; compose from Beep controls; a check must be able to fail for the reason it was written.
