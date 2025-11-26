import re
import math

MD = r"c:\\Users\\f_ald\\source\\repos\\The-Tech-Idea\\Beep.Winform\\TheTechIdea.Beep.Winform.Controls\\Themes\\plansfixtheme.md"

# WCAG relative luminance: https://www.w3.org/TR/WCAG20-TECHS/G17.html

def srgb_to_linear(c):
    c = c/255.0
    if c <= 0.03928:
        return c/12.92
    return ((c+0.055)/1.055) ** 2.4


def luminance(rgb):
    r,g,b = rgb
    r_lin = srgb_to_linear(r)
    g_lin = srgb_to_linear(g)
    b_lin = srgb_to_linear(b)
    return 0.2126*r_lin + 0.7152*g_lin + 0.0722*b_lin


def contrast_ratio(rgb1, rgb2):
    L1 = luminance(rgb1)
    L2 = luminance(rgb2)
    lighter = max(L1, L2)
    darker = min(L1, L2)
    return (lighter + 0.05) / (darker + 0.05)


def hex_from_rgb(rgb):
    return '#{0:02X}{1:02X}{2:02X}'.format(*rgb)


def rgb_from_tuple_text(text):
    m = re.match(r"\s*\(\s*(\d+),\s*(\d+),\s*(\d+)\s*\)", text)
    if not m:
        raise ValueError(f"can't parse rgb from '{text}'")
    return tuple(int(x) for x in m.groups())


def adjust_text_to_meet_contrast(fg, bg, target=4.5):
    # Attempt to darken or lighten fg to achieve contrast with bg.
    # For simplicity, darken/lighten in steps.
    fg_r, fg_g, fg_b = fg
    bg_lum = luminance(bg)
    fg_lum = luminance(fg)

    # Try darkening if bg is light, else lighten if bg is dark
    # Determine direction based on which side increases contrast
    # We'll try both directions and pick minimal delta achieving target
    def clamp(v):
        return max(0, min(255, int(round(v))))

    def change(factor):
        return (clamp(fg_r*factor), clamp(fg_g*factor), clamp(fg_b*factor))

    candidates = []
    # Darken - multiply by 0..1
    for s in [1.0 - i*0.02 for i in range(0, 51)]:
        new = change(s)
        ratio = contrast_ratio(new, bg)
        if ratio >= target:
            candidates.append((ratio, new, 'darken', s))
            break
    # Lighten - move towards white
    for t in [i*0.02 for i in range(0, 51)]:
        new = (clamp(fg_r + (255 - fg_r)*t), clamp(fg_g + (255 - fg_g)*t), clamp(fg_b + (255 - fg_b)*t))
        ratio = contrast_ratio(new, bg)
        if ratio >= target:
            candidates.append((ratio, new, 'lighten', t))
            break

    if not candidates:
        # Return best achieved between lighten/darken extremes
        dark = change(0.0)
        light = (255,255,255)
        dark_ratio = contrast_ratio(dark,bg)
        light_ratio = contrast_ratio(light,bg)
        if dark_ratio>light_ratio:
            return dark, dark_ratio, 'darken', 0.0
        else:
            return light, light_ratio, 'lighten', 1.0

    candidates.sort(key=lambda x: (x[0], -x[3]), reverse=True)
    best = candidates[0]
    return best[1], best[0], best[2], best[3]


# Extract theme sections and token values from plansfixtheme.md
with open(MD, 'r', encoding='utf-8') as f:
    text = f.read()

# Split into themes by heading '### ThemeName'
parts = re.split(r"^###\s+", text, flags=re.MULTILINE)
# First part is header; following are theme blocks: 'ThemeName\n- Description...' etc
themes = {}
for part in parts[1:]:
    lines = part.splitlines()
    theme_name = lines[0].strip()
    themes[theme_name] = {}
    # find RGB lines like '| ForeColor | (17, 24, 39) | #111827 |'
    for m in re.finditer(r"\|\s*(?P<token>[A-Za-z0-9_]+)\s*\|\s*\((?P<r>\d+),\s*(?P<g>\d+),\s*(?P<b>\d+)\)\s*\|\s*(?P<hex>#[0-9A-Fa-f]+)\s*\|", part):
        token = m.group('token')
        rgb = (int(m.group('r')), int(m.group('g')), int(m.group('b')))
        hexv = m.group('hex')
        themes[theme_name][token] = {'rgb': rgb, 'hex': hexv}

# Key token pairs to check for each theme
pairs = [
    ('ForeColor','Background'),
    ('OnPrimaryColor','PrimaryColor'),
    ('AppBarTitleForeColor','AppBarBackColor'),
    ('BorderColor','Background'),
    ('LinkColor','Background')
]

# If token absent, skip
report_lines = []
report_lines.append('# Contrast Report\n')
report_lines.append('Generated from `plansfixtheme.md`\n')
report_lines.append('Target contrast for normal text: 4.5:1 (WCAG AA).\n')
report_lines.append('\n---\n')

for theme, tokens in themes.items():
    report_lines.append(f'## {theme}\n')
    if 'Background' in tokens:
        bg = tokens['Background']['rgb']
    else:
        # fallback panel or surface
        bg = tokens.get('PanelBackColor', tokens.get('SurfaceColor', {'rgb': (255,255,255)}))['rgb']
    report_lines.append(f'- Background `rgb{bg}` {hex_from_rgb(bg)}\n')

    failures = []
    for (fg_token,bg_token) in pairs:
        # For pairs where fg token is second in tuple, adjust accordingly
        if fg_token in tokens and bg_token in tokens:
            fg = tokens[fg_token]['rgb']
            bg2 = tokens[bg_token]['rgb']
            ratio = contrast_ratio(fg,bg2)
            status = 'PASS' if ratio >= 4.5 else 'FAIL'
            report_lines.append(f'  - {fg_token} on {bg_token}: {hex_from_rgb(fg)} on {hex_from_rgb(bg2)} -> ratio {ratio:.2f} : {status}\n')
            if status == 'FAIL':
                # propose fix - change fg to meet contrast
                new, newratio, method, amt = adjust_text_to_meet_contrast(fg,bg2, 4.5)
                report_lines.append(f'    - Suggested {method} for {fg_token} to {hex_from_rgb(new)} (ratio {newratio:.2f})\n')
                failures.append((fg_token,bg_token,ratio, hex_from_rgb(new)))
        else:
            # If tokens missing, try useful fallback combinations
            # For AppBar: fallback to AppBarTitleForeColor and AppBarBackColor if defined
            if fg_token == 'AppBarTitleForeColor' and ('AppBarTitleForeColor' in tokens and 'AppBarBackColor' in tokens):
                fg = tokens['AppBarTitleForeColor']['rgb']
                bg2 = tokens['AppBarBackColor']['rgb']
                ratio = contrast_ratio(fg,bg2)
                status = 'PASS' if ratio >= 4.5 else 'FAIL'
                report_lines.append(f'  - {fg_token} on {bg_token}: {hex_from_rgb(fg)} on {hex_from_rgb(bg2)} -> ratio {ratio:.2f} : {status}\n')
                if status == 'FAIL':
                    new, newratio, method, amt = adjust_text_to_meet_contrast(fg,bg2, 4.5)
                    report_lines.append(f'    - Suggested {method} for {fg_token} to {hex_from_rgb(new)} (ratio {newratio:.2f})\n')
                    failures.append((fg_token,bg_token,ratio, hex_from_rgb(new)))
            # Else skip
    if not failures:
        report_lines.append('\n')
    else:
        report_lines.append('\n')
    report_lines.append('---\n')

# Write report
OUT = r"c:\\Users\\f_ald\\source\\repos\\The-Tech-Idea\\Beep.Winform\\TheTechIdea.Beep.Winform.Controls\\Themes\\plansfixtheme_contrast_report.md"
with open(OUT,'w',encoding='utf-8') as out:
    out.write('\n'.join(report_lines))

print('Report generated at', OUT)
