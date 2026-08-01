# 12 — Verification Harness

**Priority P0. Phase 1. Build this first.**

## Why first

`DialogsManagers/` has 9,829 lines and no harness. Every finding in this program was reached by
reading code, which is the weakest method available — and in the four preceding programs, reading
consistently missed what one render or one counter exposed immediately:

- **`BeepGridPro`** — a toolbar bug was "fixed" three times against rectangle coordinates before a
  PNG showed a null-brush exception silently aborting half the paint.
- **`BeepTree`** — 25 painters clipped every label; a contact sheet found it at a glance.
- **`ToolTips`** — a manual audit found three never-read properties; a reflection check found six.
- **`BeepTabs`** — the contact sheet reported 21 green renders while a painter was throwing, because
  `DrawToBitmap` swallows paint exceptions. Pixel assertions cannot see a paint that aborted.

Dialogs add a failure mode the previous programs did not have: **they are modal**. A probe that opens
a modal dialog and waits for input hangs forever. The harness must drive them without blocking —
`Shown` handlers that close the dialog, or `IMessageFilter` — and that plumbing has to exist before
any behavioural check can be written.

## What it must do

### 1. Enforce the ground rules mechanically

| Check | Fails when |
|---|---|
| Bare catch | any `catch { }` or `catch { return …; }` exists under `DialogsManagers/` |
| Stub | a method body is empty without an explicit `// intentionally empty: <reason>` |
| Legacy | any `[Obsolete]` member remains |
| Orphan resource | a `.resx` has no matching `.cs` |
| Shadowed type | a type here shares a name with a `System.Windows.Forms` type |
| Designer geometry | `new GraphicsPath()` appears in any `.Designer.cs` |
| Absolute layout | `Location = new` appears in any form |
| Designer control flow | `if`/`for`/`foreach`/`while`/`switch` inside `InitializeComponent` |

### 2. Prove the pipeline is one path

The check that would have caught `ShowInfo`: subscribe to `DialogOpened`, call **every** public show
method, and assert the event fired for each. A method that bypasses the pipeline cannot fire it.

### 3. Measure what reading cannot settle

- **GDI handle count** across 500 construct/dispose cycles per dialog — the only honest proof that
  [06](06-designer-serialization.md) is fixed.
- **Placement agreement** between the 17 hand-rolled sites and `DialogPlacementEngine`, recorded
  before either is changed.
- **Render at 100/150/200% DPI** and with captions 3× the design length, asserting nothing clips —
  the localisation case absolute layout always fails.

### 4. Lessons carried forward from `BeepTabs`

Four checks in that harness passed while the defect they were written for was live. All four had the
same cause and the same fix:

- **A visual check needs a controlled baseline.** Counting ink "with the feature on" proves nothing
  unless something else is held identical. Comparing against a render with only that feature disabled
  is what works.
- **Do not assert what the measurement cannot determine.** Text matching cannot distinguish a
  declaration from an override from a call; deadness is proven by deleting and compiling.
- **Self-test every text detector** against the real before/after strings. One regex there would have
  missed the exact defect it was written for, because the offenders were named constants, not literals.
- **Look at the output.** An assertion that failed all seven painters at once was a wrong assumption,
  not seven bugs — visible in two seconds from the rendered sheet.

## Verification

- ⬜ Harness runs headless and drives modal dialogs without blocking.
- ⬜ Every ground-rule check above is green, or its failure is an open item in a feature doc.
- ⬜ Each text-matching detector carries a self-test over known-bad and known-good input.
