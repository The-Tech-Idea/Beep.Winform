# Phase 7 — Rich List Types: ChatList, ContactList, ThreeLineList, NotificationList, ProfileCard

**Priority**: High  
**Status**: Not Started  
**Depends on**: Phase 2 (token-based alignment), Phase 6 (badge/sub-text/trailing helpers)

## Problem

Five `ListBoxType` enum values (35–39) are already declared in `ListBoxType.cs`, registered in `ListBoxVariantMetadataCatalog`, and wired in the `CreatePainter()` factory switch — but **no painter implementation files exist**. The project will not compile because the factory references `ChatListBoxPainter`, `ContactListBoxPainter`, `ThreeLineListBoxPainter`, `NotificationListBoxPainter`, and `ProfileCardListBoxPainter`, none of which are defined.

These five types are specifically designed to leverage `SimpleItem` properties that the existing 34 painters largely ignore:

| Property | SimpleItem Location | Current Painter Usage |
|----------|--------------------|-----------------------|
| `Description` | `SimpleItem.Description` | **None** — never rendered |
| `SubText` | `SimpleItem.SubText` | Only `AvatarListBoxPainter`, `BaseListBoxPainter` (preset-gated) |
| `SubText2` | `SimpleItem.SubText2` | **None** — never rendered |
| `SubText3` | `SimpleItem.SubText3` | **None** — never rendered |
| `ImagePath` | `SimpleItem.ImagePath` | Only `AvatarListBoxPainter`, `BaseListBoxPainter` (32px hardcoded) |
| `BadgeText` | `SimpleItem.BadgeText` / `BadgeBackColor` / `BadgeForeColor` | Only `AvatarListBoxPainter` (status dot) |
| `ShortcutText` | `SimpleItem.ShortcutText` | Only `CommandListBoxPainter` |

### Reference Spacing (from attached design specs)

**List Dialog Box** (MD3 spec):
- Dialog: 280px wide, 20px top padding, 8px bottom padding
- Item row: 48px height, 24px left padding, 12px right padding
- Radio/checkbox: 24px hit area, 20px gap between checkbox and text
- Text: Font 14px Regular, heading 20px Medium
- Touch target: 48px × full-width

**Chat List** (messaging app spec):
- Row: 52px height (per side, 72px with padding)
- Avatar: 52px circle, 16px left margin, 16px gap to text
- Name: Font 16px Bold
- Message preview: Font 14px Regular, secondary color
- Time: Font 14px Regular, right-aligned, 16px right margin
- Badge: 16px circle, right-aligned
- Inter-row gap: 8–20px

---

## Design

### Common Architecture

All five painters follow the same pattern:
1. Inherit from `BaseListBoxPainter`
2. Override `GetPreferredItemHeight()` — return scaled height per type
3. Override `DrawItem()` — implement the specific layout
4. Use existing base helpers: `DrawItemBackgroundEx()`, `DrawItemImage()`, `DrawItemText()`, `DrawSubText()`, `DrawBadgePill()`, `DrawAccentBar()`
5. Use `AvatarListBoxPainter` pattern for circular avatar: clip with `GraphicsPath.AddEllipse()`, draw image or initials fallback

### SimpleItem Property Mapping Per Type

```
┌──────────────────┬──────────┬────────┬─────────┬──────────┬──────────┬───────────┬──────────────┬───────────────┐
│ Type             │ ImagePath│ Text   │ SubText │ SubText2 │ SubText3 │ Description│ BadgeText    │ ShortcutText  │
├──────────────────┼──────────┼────────┼─────────┼──────────┼──────────┼───────────┼──────────────┼───────────────┤
│ ChatList (35)    │ Avatar   │ Name   │ Message │    —     │    —     │     —     │ Unread count │ Time stamp    │
│ ContactList (36) │ Avatar   │ Name   │ Role    │ Email    │    —     │ Notes     │     —        │     —         │
│ ThreeLineList(37)│ Thumb    │ Title  │ Line 1  │ Line 2   │ Line 3   │     —     │     —        │     —         │
│ NotificationList │ Icon     │ Title  │ Time    │    —     │    —     │ Body text │ Type badge   │     —         │
│ ProfileCard (39) │ Avatar   │ Name   │ Title   │    —     │    —     │ Bio       │     —        │     —         │
└──────────────────┴──────────┴────────┴─────────┴──────────┴──────────┴───────────┴──────────────┴───────────────┘
```

---

## Steps

### Step 7.1 — Create `ChatListBoxPainter`

**File**: `Painters/ChatListBoxPainter.cs` (new)  
**Enum**: `ListBoxType.ChatList = 35`  
**Row height**: 72px (scaled)  
**Avatar**: 52px circle (left)  

**Layout diagram** (matching attached chat list spec):

```
┌──────────────────────────────────────────────────────────────────┐
│ 16px │ ┌──────┐ │ 16px │ Name (Bold 14px)         │ 3:16 pm   │
│      │ │Avatar│ │      │ Message preview (Reg 12px)│   [6]     │
│      │ │ 52px │ │      │ (secondary color, ellipsis)│  badge   │
│      │ └──────┘ │      │                           │   16px    │
└──────────────────────────────────────────────────────────────────┘
← 16px → ← 52px → ← 16px →          flex           ← 16px →
                                                     ↑ trailing
```

**SimpleItem mapping**:
- `ImagePath` → avatar (circular, initials fallback)
- `Text` → name (bold, primary color)
- `SubText` → message preview (regular, secondary color, single-line ellipsis)
- `BadgeText` → unread count pill (right side, uses `BadgeBackColor`)
- `ShortcutText` → timestamp (right-aligned, above badge, secondary color)

**Code sketch**:
```csharp
internal class ChatListBoxPainter : BaseListBoxPainter
{
    private const int AvatarSize = 52;
    private const int RowHeight = 72;
    private const int HMargin = 16;
    private const int AvatarTextGap = 16;
    private const int BadgeSize = 16;

    public override int GetPreferredItemHeight()
        => DpiScalingHelper.ScaleValue(RowHeight, _owner);

    protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item,
                                     bool isHovered, bool isSelected)
    {
        DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

        int hm   = Scale(HMargin);
        int avSz = Scale(AvatarSize);
        int gap  = Scale(AvatarTextGap);

        // ── Avatar (circular) ────────────────────────────────────────
        int avY = itemRect.Y + (itemRect.Height - avSz) / 2;
        var avRect = new Rectangle(itemRect.X + hm, avY, avSz, avSz);
        DrawCircularAvatar(g, avRect, item);           // reuse AvatarPainter pattern

        // ── Content zone ─────────────────────────────────────────────
        int contentX = avRect.Right + gap;
        int trailingW = Scale(64);                      // reserve for time + badge
        int contentW  = itemRect.Right - contentX - trailingW - hm;

        // Name (bold)
        var nameRect = new Rectangle(contentX, itemRect.Y + Scale(12), contentW, Scale(20));
        Color nameFg = isSelected ? _owner.GetSelectionTextColor() : (_theme?.ListItemForeColor ?? Color.Black);
        using var boldFont = BeepFontManager.GetFont(_owner.TextFont.Name, _owner.TextFont.Size, FontStyle.Bold);
        DrawItemText(g, nameRect, item.Text ?? "", nameFg, boldFont);

        // Message preview (SubText)
        if (!string.IsNullOrEmpty(item.SubText))
        {
            var msgRect = new Rectangle(contentX, nameRect.Bottom + Scale(2), contentW, Scale(18));
            Color msgFg = Color.FromArgb(ListBoxTokens.SubTextAlpha,
                          _theme?.ListItemForeColor ?? Color.Gray);
            using var regFont = BeepFontManager.GetFont(_owner.TextFont.Name,
                                    Math.Max(8f, _owner.TextFont.Size - 1.5f));
            DrawItemText(g, msgRect, item.SubText, msgFg, regFont);
        }

        // ── Trailing zone ────────────────────────────────────────────
        // Time (ShortcutText)
        if (!string.IsNullOrEmpty(item.ShortcutText))
        {
            var timeRect = new Rectangle(itemRect.Right - trailingW - hm,
                                         itemRect.Y + Scale(12), trailingW, Scale(16));
            // right-aligned, secondary color, small font
        }

        // Badge (BadgeText) — unread count pill
        if (!string.IsNullOrEmpty(item.BadgeText))
            DrawBadgePill(g, itemRect, item.BadgeText, item.BadgeBackColor);
    }

    public override bool SupportsCheckboxes() => false;
}
```

---

### Step 7.2 — Create `ContactListBoxPainter`

**File**: `Painters/ContactListBoxPainter.cs` (new)  
**Enum**: `ListBoxType.ContactList = 36`  
**Row height**: 72px (scaled)  
**Avatar**: 48px circle (left)  

**Layout diagram**:

```
┌──────────────────────────────────────────────────────────────────┐
│ 16px │ ┌──────┐ │ 12px │ Name (Bold 14px)                      │
│      │ │Avatar│ │      │ Role / Title (Reg 12px, secondary)     │
│      │ │ 48px │ │      │ email@example.com (Reg 11px, tertiary) │
│      │ └──────┘ │      │                                        │
└──────────────────────────────────────────────────────────────────┘
← 16px → ← 48px → ← 12px →                flex
```

**SimpleItem mapping**:
- `ImagePath` → avatar (circular, initials fallback)
- `Text` → contact name (bold)
- `SubText` → role / job title (secondary)
- `SubText2` → email or phone (tertiary, smaller)
- `Description` → notes (not displayed in list, used for detail/tooltip)

**Code sketch**:
```csharp
internal class ContactListBoxPainter : BaseListBoxPainter
{
    private const int AvatarSize = 48;
    private const int RowHeight = 72;

    public override int GetPreferredItemHeight()
        => DpiScalingHelper.ScaleValue(RowHeight, _owner);

    protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item,
                                     bool isHovered, bool isSelected)
    {
        DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

        int hm   = Scale(16);
        int avSz = Scale(AvatarSize);
        int gap  = Scale(12);

        // ── Avatar (circular) ────────────────────────────────────
        int avY = itemRect.Y + (itemRect.Height - avSz) / 2;
        var avRect = new Rectangle(itemRect.X + hm, avY, avSz, avSz);
        DrawCircularAvatar(g, avRect, item);

        // ── Text lines ──────────────────────────────────────────
        int textX = avRect.Right + gap;
        int textW = itemRect.Right - textX - hm;
        int lineH = Scale(18);
        int startY = itemRect.Y + Scale(8);

        // Line 1: Name (bold)
        var nameRect = new Rectangle(textX, startY, textW, lineH);
        using var boldFont = BeepFontManager.GetFont(..., FontStyle.Bold);
        DrawItemText(g, nameRect, item.Text ?? "", primaryColor, boldFont);

        // Line 2: SubText → role/title (secondary)
        if (!string.IsNullOrEmpty(item.SubText))
        {
            var roleRect = new Rectangle(textX, startY + lineH, textW, lineH);
            DrawSubText(g, roleRect, item.SubText, secondaryColor, ownerFont);
        }

        // Line 3: SubText2 → email/phone (tertiary, even smaller)
        if (!string.IsNullOrEmpty(item.SubText2))
        {
            var emailRect = new Rectangle(textX, startY + lineH * 2, textW, lineH);
            // use smaller font, tertiary alpha
        }
    }

    public override bool SupportsCheckboxes() => true;  // selectable contacts
}
```

---

### Step 7.3 — Create `ThreeLineListBoxPainter`

**File**: `Painters/ThreeLineListBoxPainter.cs` (new)  
**Enum**: `ListBoxType.ThreeLineList = 37`  
**Row height**: 88px (scaled) — MD3 canonical three-line height  
**Image**: 48px rounded square (left, optional)  

**Layout diagram** (MD3 three-line list spec):

```
┌──────────────────────────────────────────────────────────────────┐
│ 16px │ ┌──────┐ │ 16px │ Title (Bold 14px)          │ trailing │
│      │ │Image │ │      │ SubText line 1 (Reg 12px)  │          │
│      │ │ 48px │ │      │ SubText2 line 2 (Reg 12px) │          │
│      │ │round │ │      │ SubText3 line 3 (Reg 11px) │          │
│      │ └──────┘ │      │                             │          │
└──────────────────────────────────────────────────────────────────┘
← 16px → ← 48px → ← 16px →           flex            ← 48px →
```

**SimpleItem mapping**:
- `ImagePath` → thumbnail (48px rounded-rect, 4px radius)
- `Text` → title (bold)
- `SubText` → description line 1 (secondary)
- `SubText2` → description line 2 (secondary)
- `SubText3` → description line 3 (tertiary, even dimmer)

**Code sketch**:
```csharp
internal class ThreeLineListBoxPainter : BaseListBoxPainter
{
    private const int ImageSize = 48;
    private const int RowHeight = 88;
    private const int ImageRadius = 4; // rounded square, not circle

    public override int GetPreferredItemHeight()
        => DpiScalingHelper.ScaleValue(RowHeight, _owner);

    protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item,
                                     bool isHovered, bool isSelected)
    {
        DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

        int hm   = Scale(16);
        int imgSz = Scale(ImageSize);
        int gap  = Scale(16);

        // ── Image (rounded square) ──────────────────────────────
        int textX;
        if (!string.IsNullOrEmpty(item.ImagePath))
        {
            int imgY = itemRect.Y + (itemRect.Height - imgSz) / 2;
            var imgRect = new Rectangle(itemRect.X + hm, imgY, imgSz, imgSz);
            // clip to rounded rect, then DrawItemImage
            DrawItemImage(g, imgRect, item.ImagePath);
            textX = imgRect.Right + gap;
        }
        else
        {
            textX = itemRect.X + hm;
        }

        int textW = itemRect.Right - textX - hm;
        int lineH = Scale(18);
        int startY = itemRect.Y + Scale(6);

        // Line 1: Title (bold)
        DrawItemText(g, new Rectangle(textX, startY, textW, lineH),
                     item.Text ?? "", primaryColor, boldFont);

        // Line 2: SubText (secondary)
        if (!string.IsNullOrEmpty(item.SubText))
            DrawSubText(g, new Rectangle(textX, startY + lineH, textW, lineH),
                        item.SubText, secondaryColor, ownerFont);

        // Line 3: SubText2 (secondary)
        if (!string.IsNullOrEmpty(item.SubText2))
            DrawSubText(g, new Rectangle(textX, startY + lineH * 2, textW, lineH),
                        item.SubText2, secondaryColor, ownerFont);

        // Line 4: SubText3 (tertiary, smaller)
        if (!string.IsNullOrEmpty(item.SubText3))
        {
            // smaller font, lower alpha
        }
    }

    public override bool SupportsCheckboxes() => true;
}
```

---

### Step 7.4 — Create `NotificationListBoxPainter`

**File**: `Painters/NotificationListBoxPainter.cs` (new)  
**Enum**: `ListBoxType.NotificationList = 38`  
**Row height**: 80px (scaled)  
**Icon**: 40px circle (left)  

**Layout diagram**:

```
┌──────────────────────────────────────────────────────────────────┐
│ 16px │ ┌──────┐ │ 12px │ Title (Bold 13px)       │ 2h ago     │
│      │ │ Icon │ │      │ Description body text    │            │
│      │ │ 40px │ │      │ (Reg 12px, 2 lines max,  │   [NEW]   │
│      │ └──────┘ │      │  ellipsis)               │   badge   │
└──────────────────────────────────────────────────────────────────┘
← 16px → ← 40px → ← 12px →           flex           ← 56px →
```

**SimpleItem mapping**:
- `ImagePath` → notification icon (40px circle, color bg fallback)
- `Text` → notification title (bold)
- `Description` → notification body (secondary, max 2 lines with ellipsis)
- `SubText` → time ago string (trailing, secondary, small font)
- `BadgeText` → type badge / "NEW" pill (trailing, below time)

**Code sketch**:
```csharp
internal class NotificationListBoxPainter : BaseListBoxPainter
{
    private const int IconSize = 40;
    private const int RowHeight = 80;

    public override int GetPreferredItemHeight()
        => DpiScalingHelper.ScaleValue(RowHeight, _owner);

    protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item,
                                     bool isHovered, bool isSelected)
    {
        DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

        int hm   = Scale(16);
        int icSz = Scale(IconSize);
        int gap  = Scale(12);

        // ── Icon (circular with tinted bg) ───────────────────────
        int icY = itemRect.Y + Scale(12);
        var icRect = new Rectangle(itemRect.X + hm, icY, icSz, icSz);
        // draw circular bg + icon image
        DrawCircularAvatar(g, icRect, item);

        // ── Trailing zone ────────────────────────────────────────
        int trailingW = Scale(56);
        int trailingX = itemRect.Right - trailingW - hm;

        // Time (SubText)
        if (!string.IsNullOrEmpty(item.SubText))
        {
            var timeRect = new Rectangle(trailingX, itemRect.Y + Scale(10), trailingW, Scale(16));
            // right-aligned, small font, secondary color
        }

        // Badge (BadgeText)
        if (!string.IsNullOrEmpty(item.BadgeText))
            DrawBadgePill(g, itemRect, item.BadgeText, item.BadgeBackColor);

        // ── Content zone ─────────────────────────────────────────
        int textX = icRect.Right + gap;
        int textW = trailingX - textX - Scale(8);

        // Title (bold)
        var titleRect = new Rectangle(textX, itemRect.Y + Scale(10), textW, Scale(18));
        DrawItemText(g, titleRect, item.Text ?? "", primaryColor, boldFont);

        // Description (2 lines, secondary)
        if (!string.IsNullOrEmpty(item.Description))
        {
            var descRect = new Rectangle(textX, titleRect.Bottom + Scale(2), textW, Scale(34));
            // multi-line with WordBreak + EndEllipsis
        }
    }

    public override bool SupportsSearch() => true;
    public override bool SupportsCheckboxes() => false;
}
```

---

### Step 7.5 — Create `ProfileCardListBoxPainter`

**File**: `Painters/ProfileCardListBoxPainter.cs` (new)  
**Enum**: `ListBoxType.ProfileCard = 39`  
**Row height**: 120px (variable, scaled)  
**Avatar**: 64px circle (centered)  

**Layout diagram**:

```
┌──────────────────────────────────────────────────────────────────┐
│                        ┌──────┐                                  │
│                        │Avatar│                                  │
│                        │ 64px │                                  │
│                        └──────┘                                  │
│                     Name (Bold 16px)                             │
│                     Title (Reg 13px)                             │
│               Bio / Description (Reg 12px)                       │
│               (secondary color, 2 lines max)                     │
└──────────────────────────────────────────────────────────────────┘
                    all text center-aligned
```

**SimpleItem mapping**:
- `ImagePath` → avatar (64px circle, centered, initials fallback)
- `Text` → display name (bold, centered)
- `SubText` → title/role (secondary, centered)
- `Description` → bio text (tertiary, centered, max 2 lines, ellipsis)

**Code sketch**:
```csharp
internal class ProfileCardListBoxPainter : BaseListBoxPainter
{
    private const int AvatarSize = 64;
    private const int RowHeight = 120;

    public override int GetPreferredItemHeight()
        => DpiScalingHelper.ScaleValue(RowHeight, _owner);

    protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item,
                                     bool isHovered, bool isSelected)
    {
        DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

        int avSz = Scale(AvatarSize);
        int centerX = itemRect.X + (itemRect.Width - avSz) / 2;

        // ── Avatar (centered, circular) ──────────────────────────
        var avRect = new Rectangle(centerX, itemRect.Y + Scale(8), avSz, avSz);
        DrawCircularAvatar(g, avRect, item);

        int textY = avRect.Bottom + Scale(6);
        int textW = itemRect.Width - Scale(32);
        int textX = itemRect.X + Scale(16);

        // Name (bold, centered)
        var nameRect = new Rectangle(textX, textY, textW, Scale(20));
        // StringFormat with Alignment = Center

        // Title (SubText, centered, secondary)
        if (!string.IsNullOrEmpty(item.SubText))
        {
            var titleRect = new Rectangle(textX, textY + Scale(20), textW, Scale(16));
            // centered, secondary color
        }

        // Bio (Description, centered, tertiary, 2 lines)
        if (!string.IsNullOrEmpty(item.Description))
        {
            var bioRect = new Rectangle(textX, textY + Scale(36), textW, Scale(28));
            // centered, smaller font, WordBreak, EndEllipsis
        }
    }

    public override bool SupportsSearch() => false;
    public override bool SupportsCheckboxes() => false;
}
```

---

### Step 7.6 — Add shared `DrawCircularAvatar` helper to BaseListBoxPainter

**File**: `Painters/BaseListBoxPainter.cs`  
**Location**: After `DrawBadgePill()` (around line 680)

Currently the circular avatar drawing logic (clip to ellipse → draw image or initials → draw border) is **duplicated** in `AvatarListBoxPainter`. All five new painters need the same pattern, so extract it to a shared base helper.

**Add**:
```csharp
/// <summary>
/// Draws a circular avatar from ImagePath, or initials fallback.
/// Reuses AvatarListBoxPainter pattern as a shared helper.
/// </summary>
protected void DrawCircularAvatar(Graphics g, Rectangle avatarRect, SimpleItem item)
{
    using var path = new GraphicsPath();
    path.AddEllipse(avatarRect);
    var state = g.Save();
    g.SetClip(path);

    if (!string.IsNullOrEmpty(item.ImagePath))
    {
        DrawItemImage(g, avatarRect, item.ImagePath);
    }
    else
    {
        // Initials fallback: hash name → pick color → draw initials
        DrawInitialsFallback(g, avatarRect, item.Text ?? item.DisplayField);
    }
    g.Restore(state);

    // Border ring
    using var pen = new Pen(Color.FromArgb(40, 0, 0, 0), 1.5f);
    g.DrawEllipse(pen, avatarRect);
}

/// <summary>
/// Draws a colored circle with 1–2 letter initials (reusable fallback).
/// </summary>
protected void DrawInitialsFallback(Graphics g, Rectangle rect, string name)
{
    Color[] palette = { /* 7 avatar colors from AvatarListBoxPainter */ };
    int idx = Math.Abs((name ?? "").GetHashCode()) % palette.Length;
    using var bg = new SolidBrush(palette[idx]);
    g.FillEllipse(bg, rect);
    
    string initials = GetInitials(name);
    using var font = BeepFontManager.GetFont(_owner.TextFont.Name,
                         Math.Max(10f, rect.Height * 0.35f), FontStyle.Bold);
    using var brush = new SolidBrush(Color.White);
    using var sf = new StringFormat { Alignment = StringAlignment.Center,
                                      LineAlignment = StringAlignment.Center };
    g.DrawString(initials, font, brush, rect, sf);
}
```

Also add the `Scale()` convenience method if not already present:
```csharp
/// <summary>Shorthand for DPI scaling.</summary>
protected int Scale(int value) => DpiScalingHelper.ScaleValue(value, _owner);
```

---

### Step 7.7 — Add tokens for new list types

**File**: `Tokens/ListBoxTokens.cs`

Add constants used by the 5 new painters:
```csharp
// ── Rich list type tokens ─────────────────────────────────────────────

/// <summary>Chat list row height (px). Matches messaging app 72 px spec.</summary>
public const int ChatRowHeight = 72;

/// <summary>Chat avatar size (px). 52 px per attached spec.</summary>
public const int ChatAvatarSize = 52;

/// <summary>Contact list row height (px).</summary>
public const int ContactRowHeight = 72;

/// <summary>Contact avatar size (px).</summary>
public const int ContactAvatarSize = 48;

/// <summary>Three-line list row height (px). MD3 canonical 88 px.</summary>
public const int ThreeLineRowHeight = 88;

/// <summary>Three-line list thumbnail size (px). 48 px rounded square.</summary>
public const int ThreeLineImageSize = 48;

/// <summary>Three-line image corner radius (px).</summary>
public const int ThreeLineImageRadius = 4;

/// <summary>Notification list row height (px).</summary>
public const int NotificationRowHeight = 80;

/// <summary>Notification icon size (px).</summary>
public const int NotificationIconSize = 40;

/// <summary>Profile card row height (px).</summary>
public const int ProfileCardRowHeight = 120;

/// <summary>Profile card avatar size (px). Larger centered avatar.</summary>
public const int ProfileCardAvatarSize = 64;

/// <summary>Standard horizontal margin for rich list types (px).</summary>
public const int RichListHMargin = 16;

/// <summary>Gap between avatar/image and text content (px).</summary>
public const int AvatarTextGap = 16;

/// <summary>Trailing zone width for time + badge area (px).</summary>
public const int ChatTrailingWidth = 64;

/// <summary>Notification trailing zone width (px).</summary>
public const int NotificationTrailingWidth = 56;
```

---

## Files Created / Modified

| File | Action | Description |
|------|--------|-------------|
| `Painters/ChatListBoxPainter.cs` | **Create** | 72px row, 52px circular avatar + name + message preview + time + badge |
| `Painters/ContactListBoxPainter.cs` | **Create** | 72px row, 48px circular avatar + name + role + email |
| `Painters/ThreeLineListBoxPainter.cs` | **Create** | 88px row, 48px rounded thumbnail + title + 3 sub-text lines |
| `Painters/NotificationListBoxPainter.cs` | **Create** | 80px row, 40px icon + title + description + time + badge |
| `Painters/ProfileCardListBoxPainter.cs` | **Create** | 120px row, 64px centered avatar + name + title + bio |
| `Painters/BaseListBoxPainter.cs` | **Modify** | Add `DrawCircularAvatar()`, `DrawInitialsFallback()`, `Scale()` helpers |
| `Tokens/ListBoxTokens.cs` | **Modify** | Add height/size tokens for all 5 types |

**Already done** (no changes needed):
- `ListBoxType.cs` — enum values 35–39 already declared
- `ListBoxVariantMetadata.cs` — catalog entries already registered
- `BeepListBox.Drawing.cs` — factory switch cases already wired

## Verification Checklist

- [ ] `ListBoxType = ChatList` → 72px rows, circular 52px avatar, bold name, message preview, right-aligned time, unread badge pill
- [ ] `ListBoxType = ContactList` → 72px rows, 48px avatar, name + role + email on 3 lines
- [ ] `ListBoxType = ThreeLineList` → 88px rows, 48px rounded-rect image, title + 3 sub-text lines
- [ ] `ListBoxType = NotificationList` → 80px rows, 40px icon, title + description body, trailing time + badge
- [ ] `ListBoxType = ProfileCard` → 120px rows, large centered avatar, centered name + title + bio
- [ ] All 5 types: DPI scaling works at 100%, 125%, 150%
- [ ] All 5 types: theme change updates colors correctly
- [ ] Missing `ImagePath` → initials fallback avatar renders correctly
- [ ] Empty `SubText`/`SubText2`/`SubText3` → lines gracefully collapse (no blank gaps)
- [ ] `BadgeText` with custom `BadgeBackColor` → colored pill renders
- [ ] Selection + hover states work on all 5 types
- [ ] All 5 types compile and render without errors
- [ ] Existing 34 painters unaffected
