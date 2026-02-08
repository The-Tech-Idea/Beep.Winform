# BeepBreadCrumb Skill

## Overview
Breadcrumb navigation for hierarchical path display.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.BreadCrumbs;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `Items` | `List<BreadCrumbItem>` | Path items |
| `Separator` | `string` | Separator character |

## Events
| Event | Description |
|-------|-------------|
| `ItemClicked` | Breadcrumb item clicked |

## Usage
```csharp
var breadcrumb = new BeepBreadCrumb();
breadcrumb.Items.Add(new BreadCrumbItem { Text = "Home" });
breadcrumb.Items.Add(new BreadCrumbItem { Text = "Products" });
breadcrumb.Items.Add(new BreadCrumbItem { Text = "Electronics" });
breadcrumb.ItemClicked += (s, e) => NavigateTo(e.Item);
```
