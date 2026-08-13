import os, re, glob, json, sys
os.chdir(r"C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Forms\ModernForm\Painters")

def strip_comments(src):
    # block comments, then line comments, then string literals (so "//" inside a string
    # cannot masquerade as code either way round)
    src = re.sub(r"/\*.*?\*/", "", src, flags=re.S)
    out = []
    for line in src.split("\n"):
        # remove // ... unless inside a string; cheap approximation: cut at the first //
        # that has an even number of quotes before it
        i, n = 0, len(line)
        cut = None
        while i < n - 1:
            if line[i] == '"':
                # skip string
                i += 1
                while i < n and not (line[i] == '"' and line[i-1] != '\\'):
                    i += 1
            elif line[i] == '/' and line[i+1] == '/':
                cut = i; break
            i += 1
        out.append(line[:cut] if cut is not None else line)
    return "\n".join(out)

REGIONS = ["Close", "Maximize", "Minimize", "Theme", "Style", "CustomAction", "Profile", "Search", "Icon"]

rows = []
for p in sorted(glob.glob("*FormPainter.cs")):
    name = os.path.basename(p).replace("FormPainter.cs", "")
    raw = open(p, encoding="utf-8").read()
    code = strip_comments(raw)

    r = {"skin": name, "lines": raw.count("\n") + 1}
    r["helper"] = "FormPainterLayoutHelper" in code
    r["builtin"] = len(re.findall(r"PaintBuiltInCaptionElements", code))
    r["dpi"] = len(re.findall(r"DpiScalingHelper", code))
    r["capFromFont"] = bool(re.search(r"captionHeight\s*=\s*owner\.Font\.Height", code))
    r["capFromMetrics"] = bool(re.search(r"captionHeight\s*=\s*\w*\.?[Mm]etrics\.CaptionHeight|=\s*m\.CaptionHeight", code))

    # which regions does it LAY OUT (assign a rect) and which does it PAINT (invoke OnPaint)?
    laid, painted = [], []
    for g in REGIONS:
        if re.search(r"layout\.%sButtonRect\s*=|layout\.%sRect\s*=|layout\.%sBoxRect\s*=" % (g, g, g), code):
            laid.append(g)
        if re.search(r"%sButton\?\.OnPaint|%s\?\.OnPaint" % (g, g), code):
            painted.append(g)
    r["laid"] = laid
    r["painted"] = painted
    r["hits"] = sorted(set(re.findall(r"FormHitAreaNames\.(\w+)", code)) |
                       set(x.capitalize() for x in re.findall(r'RegisterHitArea\("(\w+)"', code)))
    rows.append(r)

# ── the report ──────────────────────────────────────────────────────────────
sys.stdout.write("skin           lines helper builtin dpi capH      laid-out                     painted\n")
for r in rows:
    cap = "font" if r["capFromFont"] else ("metrics" if r["capFromMetrics"] else "?")
    sys.stdout.write("%-14s %5d %-6s %-7d %-3d %-9s %-28s %s\n" % (
        r["skin"], r["lines"], "yes" if r["helper"] else "no", r["builtin"], r["dpi"], cap,
        ",".join(r["laid"]) or "-", ",".join(r["painted"]) or "-"))

sys.stdout.write("\n--- totals over %d painters ---\n" % len(rows))
sys.stdout.write("use layout helper:        %d\n" % sum(1 for r in rows if r["helper"]))
sys.stdout.write("call PaintBuiltIn (real): %d\n" % sum(1 for r in rows if r["builtin"]))
sys.stdout.write("call DpiScalingHelper:    %d\n" % sum(1 for r in rows if r["dpi"]))
sys.stdout.write("caption from font:        %d\n" % sum(1 for r in rows if r["capFromFont"]))
sys.stdout.write("caption from metrics:     %d\n" % sum(1 for r in rows if r["capFromMetrics"]))
sys.stdout.write("caption from neither:     %d\n" % sum(1 for r in rows if not r["capFromFont"] and not r["capFromMetrics"]))
for g in REGIONS:
    l = sum(1 for r in rows if g in r["laid"]); pn = sum(1 for r in rows if g in r["painted"])
    sys.stdout.write("  %-13s laid %2d   painted %2d   %s\n" % (g, l, pn, "<-- laid but not painted" if l > pn else ""))
json.dump(rows, open(r"C:\Users\f_ald\AppData\Local\Temp\claude\C--Users-f-ald-source-repos-The-Tech-Idea-Beep-Winform\ab910d27-0def-4974-83a0-5bf9c2b58dc2\scratchpad\audit.json", "w"), indent=1)
