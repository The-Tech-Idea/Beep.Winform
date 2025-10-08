# BeepTree PaintNode Revision Plan

## Objective
Make each painter's PaintNode() method truly distinct with unique visual characteristics that match its design style.

## Current Issue
Most painters use base.PaintNode() which gives them identical rendering. Each painter should override PaintNode() with style-specific rendering order, effects, and visual elements.

## Strategy for Each Painter

### 1. StandardTreePainter ✅
**Style**: Classic Windows Explorer
**Distinct Features**:
- Tree lines connecting nodes (dotted lines)
- Dotted focus rectangle for selection
- Simple rectangular selection
**Status**: Already implemented

### 2. Material3TreePainter
**Style**: Google Material Design 3
**Distinct Features**:
- Elevation shadows on selection
- Rounded corners (8px)
- State layers (semi-transparent overlays)
- Icon buttons with ripple effect
- Bold typography on selection
**Custom PaintNode**: Shadow → background → state layer → elements

### 3. AntDesignTreePainter
**Style**: Ant Design framework
**Distinct Features**:
- Clean rectangular selection with thin borders
- Caret icons (not chevrons)
- Flat, minimal design
- Subtle hover borders
**Custom PaintNode**: Flat background → border → caret → elements

### 4. Fluent2TreePainter
**Style**: Microsoft Fluent 2
**Distinct Features**:
- Acrylic tint overlays
- Rounded corners (4px)
- Reveal borders (accent color)
- Semi-transparent layers
**Custom PaintNode**: Acrylic base → tint → reveal border → elements

### 5. BootstrapTreePainter
**Style**: Bootstrap framework
**Distinct Features**:
- Card-based nodes with shadows
- Thicker borders (2px)
- Bootstrap primary colors
- Shadow on selection
**Custom PaintNode**: Shadow → rounded card → thick border → elements

### 6. ChakraUITreePainter
**Style**: Chakra UI (accessibility focused)
**Distinct Features**:
- Prominent focus rings (2px)
- Double ring on selection
- Inner shadow for depth
- Accessible color contrast
**Custom PaintNode**: Background → double focus ring → inner shadow → elements

### 7. MacOSBigSurTreePainter
**Style**: macOS Big Sur sidebar
**Distinct Features**:
- Translucent/vibrancy effects (70% opacity)
- Rounded corners (6px)
- Subtle highlights on top edge
- Sidebar-style selection
**Custom PaintNode**: Translucent fill → vibrancy → highlight → elements

### 8. iOS15TreePainter
**Style**: Apple iOS 15
**Distinct Features**:
- Rounded groups (10px corners)
- Drop shadows on selection
- SF Symbols style icons
- Smooth, fluid spacing
**Custom PaintNode**: Shadow → rounded fill → smooth elements

### 9. VercelCleanTreePainter
**Style**: Vercel design (ultra-minimal)
**Distinct Features**:
- Top accent border (2px) on selection
- Flat backgrounds
- Plus/minus toggles
- Monospace text feel
**Custom PaintNode**: Flat background → top accent → minimal elements

### 10. NotionMinimalTreePainter
**Style**: Notion (minimal)
**Distinct Features**:
- Bottom accent line (2px) on selection
- Left border hint on hover
- Simple arrow toggles
- Emoji-style icons
**Custom PaintNode**: Flat background → bottom accent → left hint → elements

### 11. InfrastructureTreePainter
**Style**: VMware vSphere
**Distinct Features**:
- Colored status pills
- Resource icons
- Hierarchical indicators
- Metric badges
- Chevron toggles with hover color
**Custom PaintNode**: Background → status pills → badges → elements

### 12. DevExpressTreePainter
**Style**: DevExpress professional
**Distinct Features**:
- Professional gradients (vertical)
- Plus/minus box toggles
- Focus rectangles
- Icon badges
**Custom PaintNode**: Gradient fill → focus border → box toggle → badges

### 13. SyncfusionTreePainter
**Style**: Syncfusion
**Distinct Features**:
- Left accent bars (4px)
- Flat design
- Clean checkboxes
**Custom PaintNode**: Background → left accent bar → flat elements

### 14. TelerikTreePainter
**Style**: Telerik
**Distinct Features**:
- Glass effects (gradients + transparency)
- Thick borders
- Shiny appearance
**Custom PaintNode**: Glass gradient → border → shiny elements

### 15. PillRailTreePainter
**Style**: Pill-shaped selections
**Distinct Features**:
- Pill-shaped backgrounds (high radius)
- Dot toggles instead of arrows
- Rounded everything
**Custom PaintNode**: Pill background → dot toggle → rounded elements

### 16. StripeDashboardTreePainter
**Style**: Stripe fintech
**Distinct Features**:
- Clean rounded backgrounds (6px)
- Subtle accent borders
- Professional spacing
- Metric badges
**Custom PaintNode**: Rounded background → border → metric badges

### 17. ComponentTreePainter
**Style**: Figma/VS Code
**Distinct Features**:
- Drag handles (3 horizontal lines)
- Left accent stripe
- Triangle toggles
- Layer hierarchy feel
**Custom PaintNode**: Rounded background → left stripe → drag handles → triangle

### 18. DocumentTreePainter
**Style**: Document management
**Distinct Features**:
- Card elevation with shadows
- Rounded corners (4px)
- Metadata display
- Thumbnail previews
**Custom PaintNode**: Shadow → card → border → thumbnail → metadata

### 19. ActivityLogTreePainter
**Style**: Timeline/events
**Distinct Features**:
- Timeline dots with glow
- Plus/minus toggles
- Chronological indicators
**Custom PaintNode**: Background → timeline dot with glow → timestamp

### 20. DiscordTreePainter
**Style**: Discord server/channel
**Distinct Features**:
- Left indicator pill (4px width)
- Rounded selections (4px)
- Channel-style layout
**Custom PaintNode**: Rounded background → left indicator pill → elements

### 21. FigmaCardTreePainter
**Style**: Figma Layers panel
**Distinct Features**:
- Card with accent border (2px)
- Drag handles visible on hover
- Triangle toggles
**Custom PaintNode**: Card → accent border → drag handles → triangle

### 22. FileBrowserTreePainter
**Style**: File browser (compact)
**Distinct Features**:
- Left border accent (2px)
- Type-specific icons
- Compact spacing
- Small triangle toggles
**Custom PaintNode**: Background → left border → small triangle → type icons

### 23. FileManagerTreePainter
**Style**: Google Drive/OneDrive
**Distinct Features**:
- Rounded selection (6px)
- Subtle shadows
- Modern chevrons
- Colorful folder icons
**Custom PaintNode**: Rounded background → subtle shadow → border → colorful icons

### 24. PortfolioTreePainter
**Style**: Jira/Atlassian
**Distinct Features**:
- Left accent bar (3px)
- Progress indicators
- Triangle toggles
- Epic/story hierarchy
**Custom PaintNode**: Rounded background → left accent bar → progress → triangle

### 25. TailwindCardTreePainter
**Style**: Tailwind CSS
**Distinct Features**:
- Card with shadow layers
- Ring borders (2px)
- Shadow offsets (lg, md)
- Rounded corners (8px)
**Custom PaintNode**: Shadow layers → card → ring border → elements

## Implementation Order

1. ✅ StandardTreePainter - Already done
2. ✅ Material3TreePainter - Complex shadows and state layers
3. ✅ AntDesignTreePainter - Simple but clean
4. ✅ Fluent2TreePainter - Acrylic effects
5. ✅ BootstrapTreePainter - Card shadows
6. ✅ ChakraUITreePainter - Focus rings
7. ✅ VercelCleanTreePainter - Top accent
8. ✅ NotionMinimalTreePainter - Bottom accent
9. ✅ MacOSBigSurTreePainter - Translucency
10. ✅ iOS15TreePainter - Drop shadows
11. ✅ TailwindCardTreePainter - Shadow layers
12. ✅ DiscordTreePainter - Left pill
13. ✅ FileBrowserTreePainter - Compact
14. ✅ FileManagerTreePainter - Rounded modern
15. ✅ InfrastructureTreePainter - Status pills
16. ✅ DevExpressTreePainter - Gradients
17. ✅ SyncfusionTreePainter - Accent bars
18. ✅ TelerikTreePainter - Glass effects
19. ✅ PillRailTreePainter - Pills and dots
20. ✅ StripeDashboardTreePainter - Clean fintech
21. ComponentTreePainter - Drag handles
22. DocumentTreePainter - Document cards
23. ActivityLogTreePainter - Timeline
24. FigmaCardTreePainter - Layer cards
25. PortfolioTreePainter - Project management
11. TailwindCardTreePainter - Shadow layers
12. DiscordTreePainter - Left pill
13. FileBrowserTreePainter - Compact
14. FileManagerTreePainter - Rounded modern
15. InfrastructureTreePainter - Status pills
16. DevExpressTreePainter - Gradients
17. SyncfusionTreePainter - Accent bars
18. TelerikTreePainter - Glass effects
19. PillRailTreePainter - Pills and dots
20. StripeDashboardTreePainter - Clean fintech
21. ComponentTreePainter - Drag handles
22. DocumentTreePainter - Document cards
23. ActivityLogTreePainter - Timeline
24. FigmaCardTreePainter - Layer cards
25. PortfolioTreePainter - Project management

## Testing After Each Update
- Verify distinct visual appearance
- Check hover states
- Test selection states
- Verify toggle appearance
- Ensure no coordinate issues
