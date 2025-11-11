# WidgetContext Property Refactoring Plan

## Overview
This document catalogs all `CustomData` dictionary usage across widget painters and maps them to strongly-typed properties that will be added to `WidgetContext`.

## Analysis Summary
After analyzing all painter files, the following CustomData keys have been identified and categorized:

---

## **Navigation Properties**

### Menu & Navigation Items
| CustomData Key | Type | Default | Usage | Proposed Property |
|---------------|------|---------|-------|-------------------|
| `Items` | `List<string>` or `List<NavigationItem>` | `new List<string>()` | Menu items, breadcrumb items, tab items, sidebar items | `NavigationItems` |
| `Groups` | `List<(string, List<string>)>` | null | Sidebar grouped navigation | `NavigationGroups` |
| `Actions` | `List<NavigationItem>` | `new List<NavigationItem>()` | Quick actions | `QuickActions` |
| `Processes` | `List<NavigationItem>` | `new List<NavigationItem>()` | Process flow items | `ProcessFlowItems` |

### Navigation State
| CustomData Key | Type | Default | Usage | Proposed Property |
|---------------|------|---------|-------|-------------------|
| `ActiveIndex` | `int` | `0` | Currently selected menu/tab index | `ActiveIndex` |
| `CurrentIndex` | `int` | `0` or `items.Count - 1` | Currently selected item index | `CurrentIndex` |
| `ActiveProcess` | `int` | `1` | Currently active process step | `ActiveProcessIndex` |
| `SidebarSelection` | `string` | null | Selected sidebar item ID | `SidebarSelectionId` |

### Pagination
| CustomData Key | Type | Default | Usage | Proposed Property |
|---------------|------|---------|-------|-------------------|
| `CurrentPage` | `int` | `1` | Current page number | `CurrentPage` |
| `TotalPages` | `int` | `10` | Total number of pages | `TotalPages` |
| `ShowPageInfo` | `bool` | `false` | Whether to show page info text | `ShowPageInfo` |

---

## **Social/Communication Properties**

### Chat & Messaging
| CustomData Key | Type | Default | Usage | Proposed Property |
|---------------|------|---------|-------|-------------------|
| `Messages` | `List<ChatMessage>` or `List<Dictionary<string, object>>` | `new List<>()` | Chat messages or message center messages | `ChatMessages` |
| `MessageCount` | `int` | `0` | Number of messages | `MessageCount` |
| `UnreadCount` | `int` | `0` | Number of unread messages | `UnreadCount` |
| `IsTyping` | `bool` | `false` | Someone is typing indicator | `IsTyping` |
| `Participants` | `List<ChatParticipant>` | `new List<>()` | Chat participants | `ChatParticipants` |
| `CurrentUserId` | `string` | `"current_user"` | ID of current user | `CurrentUserId` |
| `InputText` | `string` | `""` | Text input content | `InputText` |

### Comments & Activity
| CustomData Key | Type | Default | Usage | Proposed Property |
|---------------|------|---------|-------|-------------------|
| `Comments` | `List<Comment>` | `new List<>()` | Comment thread items | `Comments` |
| `CommentText` | `string` | `""` | Comment input text | `CommentText` |
| `HasNewComments` | `bool` | `false` | Whether there are new comments | `HasNewComments` |
| `Activities` | `List<ActivityItem>` | `new List<>()` | Activity stream items | `ActivityItems` |

### Social Profile/Contact
| CustomData Key | Type | Default | Usage | Proposed Property |
|---------------|------|---------|-------|-------------------|
| `UserName` | `string` | `"User"` | Display name of user | `UserName` |
| `UserRole` | `string` | `"Role"` | User's role/title | `UserRole` |
| `UserStatus` | `string` | `"Offline"` | User online status | `UserStatus` |
| `Contact` | `ContactInfo` | null | Contact information object | `ContactInfo` |
| `SocialItems` | `List<SocialItem>` | `new List<>()` | Social/team member items | `SocialItems` |
| `OnlineCount` | `int` | `0` | Number of online users | `OnlineCount` |
| `TotalCount` | `int` | `0` | Total number of users/members | `TotalCount` |
| `AvatarImage` | `Image` | null | Avatar image object | `AvatarImage` |
| `AvatarImagePath` | `string` | null | Path to avatar image | `AvatarImagePath` |
| `StatusColor` | `Color` | `Color.Gray` | Status indicator color | `StatusColor` |
| `VerticalLayout` | `bool` | `false` | Use vertical layout for contact card | `IsVerticalLayout` |

---

## **Notification/Alert Properties**

### Alert/Notification Display
| CustomData Key | Type | Default | Usage | Proposed Property |
|---------------|------|---------|-------|-------------------|
| `Message` | `string` | `""` | Message text content | `Message` |
| `Timestamp` | `string` | `""` | Timestamp string | `Timestamp` |
| `NotificationType` | `string` or `NotificationType` enum | `"info"` | Type of notification (info/warning/error/success) | `NotificationType` |
| `ValidationType` | `string` | `"error"` | Validation message type | `ValidationType` |
| `StatusType` | `string` | `"info"` | Status card type | `StatusType` |

### Alert Behavior
| CustomData Key | Type | Default | Usage | Proposed Property |
|---------------|------|---------|-------|-------------------|
| `Dismissible` | `bool` | `false` | Can be dismissed by user | `IsDismissible` |
| `IsDismissible` | `bool` | `false` | Toast can be dismissed | `IsDismissible` |
| `Collapsed` | `bool` | `false` | Panel is collapsed | `IsCollapsed` |
| `DisableCopy` | `bool` | `false` | Disable copy button | `DisableCopy` |
| `HideAction` | `bool` | `false` | Hide action button | `HideAction` |
| `ShowProgress` | `bool` | `false` | Show progress indicator | `ShowProgress` |
| `Progress` | `float` | `0.3f` | Progress value (0-100) | `Progress` |

---

## **Media/Display Properties**

| CustomData Key | Type | Default | Usage | Proposed Property |
|---------------|------|---------|-------|-------------------|
| `Image` | `Image` | null | Image object | `Image` |
| `ImagePath` | `string` | null | Path to image file | *(Already exists as ImagePath)* |
| `ShowOverlay` | `bool` | `false` | Show image overlay | `ShowImageOverlay` |
| `ShowBadge` | `bool` | `false` | Show badge indicator | `ShowBadge` |

---

## **Location/Map Properties**

| CustomData Key | Type | Default | Usage | Proposed Property |
|---------------|------|---------|-------|-------------------|
| `Address` | `string` | `"Unknown Location"` | Location address | `Address` |
| `Latitude` | `double` | `0.0` | Latitude coordinate | `Latitude` |
| `Longitude` | `double` | `0.0` | Longitude coordinate | `Longitude` |
| `LastUpdated` | `DateTime` | `DateTime.Now` | Last update timestamp | `LastUpdated` |
| `Locations` | `List<MapLocation>` | `new List<>()` | List of map locations | `MapLocations` |

---

## **Metric Properties**

| CustomData Key | Type | Default | Usage | Proposed Property |
|---------------|------|---------|-------|-------------------|
| `MetricType` | `string` | `""` | Type of metric for icon selection | `MetricType` |
| `ShowPercentage` | `bool` | `false` | Show percentage text | `ShowPercentage` |

---

## **List/Item Properties**

| CustomData Key | Type | Default | Usage | Proposed Property |
|---------------|------|---------|-------|-------------------|
| `Items` | `List<Dictionary<string, object>>` | `new List<>()` | Generic list items for ranking/profile lists | `ListItems` |

---

## **Interaction/State Properties (Event Results)**

These are set by painters when user interactions occur:

| CustomData Key | Type | Default | Usage | Proposed Property |
|---------------|------|---------|-------|-------------------|
| `HeaderIconClicked` | `bool` | - | Header icon was clicked | `HeaderIconClicked` |
| `SelectedMessageIndex` | `int` | - | Index of selected message | `SelectedMessageIndex` |
| `SelectedMessageIsMine` | `bool` | - | Selected message belongs to current user | `SelectedMessageIsMine` |
| `InputFocused` | `bool` | - | Input field is focused | `InputFocused` |
| `SendClicked` | `bool` | - | Send button was clicked | `SendClicked` |
| `BannerClicked` | `bool` | - | Banner was clicked | `BannerClicked` |
| `IconClicked` | `bool` | - | Icon was clicked | `IconClicked` |
| `Dismissed` | `bool` | - | Item was dismissed | `Dismissed` |
| `SuccessDismissed` | `bool` | - | Success banner dismissed | `SuccessDismissed` |
| `StatusCardClicked` | `bool` | - | Status card clicked | `StatusCardClicked` |
| `StatusCardDismissed` | `bool` | - | Status card dismissed | `StatusCardDismissed` |
| `ValidationDismissed` | `bool` | - | Validation dismissed | `ValidationDismissed` |
| `ValidationCopyRequested` | `object` | - | Validation copy requested | `ValidationCopyRequested` |
| `BreadcrumbHomeClicked` | `bool` | - | Breadcrumb home clicked | `BreadcrumbHomeClicked` |
| `BreadcrumbIndex` | `int` | - | Breadcrumb item clicked index | `BreadcrumbIndex` |
| `SelectedRankIndex` | `int` | - | Selected rank item index | `SelectedRankIndex` |
| `SelectedProfileAvatarIndex` | `int` | - | Selected profile avatar index | `SelectedProfileAvatarIndex` |
| `SelectedProfileNameIndex` | `int` | - | Selected profile name index | `SelectedProfileNameIndex` |
| `IconCardClicked` | `bool` | - | Icon card clicked | `IconCardClicked` |
| `IconCardBadgeClicked` | `bool` | - | Icon card badge clicked | `IconCardBadgeClicked` |
| `ShowSeparators` | `bool` | `false` | Show tab separators | `ShowTabSeparators` |

---

## Property Consolidation

Some CustomData keys appear with slight variations but represent the same concept:
- `Dismissible` and `IsDismissible` → Use `IsDismissible`
- `Items` (various types) → Split into specific typed lists:
  - `NavigationItems` for navigation-related lists
  - `ListItems` for generic dictionary-based lists
  - `ChatMessages` for messages
  - etc.

---

## Implementation Strategy

1. **Phase 1**: Add all properties to `WidgetContext` class
2. **Phase 2**: Refactor painters category by category:
   - Navigation painters
   - Social painters
   - Notification painters
   - Media painters
   - Map painters
   - Metric painters
   - List painters
   - Other painters
3. **Phase 3**: Remove `CustomData` dictionary property after all migrations complete

---

## Files to Update

### Core File
- `IWidgetPainter.cs` - Add all properties to `WidgetContext` class

### Painter Files (by category)

**Navigation (16 painters)**
- BreadcrumbPainter.cs
- MenuBarPainter.cs
- MenuPainter.cs
- PaginationPainter.cs
- ProcessFlowPainter.cs
- QuickActionsPainter.cs
- SidebarNavPainter.cs
- SidebarPainter.cs
- TabContainerPainter.cs

**Social (7 painters)**
- ActivityStreamPainter.cs
- ChatWidgetPainter.cs
- CommentThreadPainter.cs
- ContactCardPainter.cs
- MessageCardPainter.cs
- SocialProfileCardPainter.cs
- TeamMembersPainter.cs

**Notification (10 painters)**
- AlertBannerPainter.cs
- InfoPanelPainter.cs
- MessageCenterPainter.cs
- StatusCardPainter.cs
- SuccessBannerPainter.cs
- SystemAlertPainter.cs
- ToastNotificationPainter.cs
- ValidationMessagePainter.cs
- WarningBadgePainter.cs

**Media (2 painters)**
- IconCardPainter.cs
- ImageCardPainter.cs

**Map (2 painters)**
- LiveTrackingPainter.cs
- LocationCardPainter.cs

**Metric (2 painters)**
- CardMetricPainter.cs
- ProgressMetricPainter.cs

**List (2 painters)**
- ProfileListPainter.cs
- RankingListPainter.cs

---

## Total Counts
- **Total unique CustomData keys**: ~70+
- **Total properties to add**: ~65 (after consolidation)
- **Total painter files to update**: ~40+

