# Stage 07 — Filtering and paging for real LOV sizes

**Kind:** enhancement · **Files:** `BeepLovPopup.cs`, `BeepListofValuesBox.cs` · **Status: partial.**

## Server-side search — done

`SearchChanged` existed with a comment calling it "useful for server-side filtering", and **nothing
subscribed to it**. Every search filtered `_allItems` in memory, which meant a LOV over a large table
had to load every row before it could narrow to three.

**`SearchLoader`** — `Func<string, CancellationToken, Task<List<SimpleItem>>>` — is now on both the
popup and the field. When set, typing queries the source and the results become the list; when not set,
the in-memory filter is unchanged.

Debounced at **250 ms** by default (`SearchDebounceMs`), so a typist does not launch a query per
keystroke, and each new search cancels the one before it.

The LOV now has three loaders answering three different questions, which is what a large one needs:

| | question |
|---|---|
| `ItemsLoader` | what is in the list |
| `SearchLoader` | what matches what I typed |
| `KeyResolver` | what is this one key |

Only the first existed.

### Verification

Opened with a **deliberately empty** list, so everything the grid shows must have come from the query.
Typing three characters fires **one** query carrying the full text, and the grid shows exactly what the
source returned.

## Rendering and a bounded result set — done

**Grid virtualization is on.** `EnableVirtualization = true` on the popup's grid, so only the visible
rows are materialised — a LOV result set is exactly what it is for.

**`MaxRows` (default 500) bounds what is bound at once**, and the count line *says so*:
`showing first 10 of 250 - narrow your search`. A loader is free to return everything and a LOV over a
large table often will; binding all of it froze the popup with no indication why. Silently showing
fewer rows than were found would have been worse than slow, so the cap is stated.

Checked in both directions: a capped set says it is capped, and an uncapped one reports a plain
`3 records` — without that guard the first check would pass on a label that always said "showing
first".

## True server-side paging — open, and deliberately not invented

`MaxRows` bounds the *client*. It does not stop a `SearchLoader` from fetching fifty thousand rows and
throwing 49,500 away, which is the query cost, not the render cost.

Fixing that properly means the loader takes a window and reports a total:

```csharp
Func<string search, int offset, int count, CancellationToken, Task<(List<SimpleItem> rows, int total)>>
```

That is a fourth loader signature on a control that now has three, and it changes who owns paging state
(scroll position, prefetch, the count line). **Worth doing, worth deciding deliberately** — it is a
design choice about the control's contract, not a defect to be quietly patched.
