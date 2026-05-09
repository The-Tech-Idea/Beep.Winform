using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Adapters
{
    /// <summary>
    /// Bridges premium tab items into the existing DocumentHost document model.
    /// Preview-slot behavior remains owned by <see cref="BeepTabWorkspaceState"/>,
    /// but pinned, dirty, badge, active, and grouping metadata map directly onto
    /// the current DocumentHost surface.
    /// </summary>
    public sealed class BeepDocumentHostWorkspaceAdapter
    {
        public bool IsAvailable => true;

        public void EnsureAvailable()
        {
        }

        public BeepDocumentTab CreateDocumentTab(BeepTabItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return new BeepDocumentTab(GetDocumentId(item), item.Title)
            {
                IconPath = string.IsNullOrWhiteSpace(item.IconPath) ? null : item.IconPath,
                IsModified = item.IsDirty,
                CanClose = item.CanClose,
                IsPinned = item.IsPinned,
                IsActive = item.IsSelected,
                TooltipText = string.IsNullOrWhiteSpace(item.SubText) ? null : item.SubText,
                BadgeText = string.IsNullOrWhiteSpace(item.BadgeText) ? null : item.BadgeText,
                BadgeColor = GetBadgeColor(item.BadgeKind),
                DocumentCategory = string.IsNullOrWhiteSpace(item.GroupKey) ? null : item.GroupKey,
                Group = string.IsNullOrWhiteSpace(item.GroupKey) ? null : item.GroupKey
            };
        }

        public BeepTabItem CreateTabItem(BeepDocumentTab documentTab, int index = -1)
        {
            if (documentTab == null)
            {
                throw new ArgumentNullException(nameof(documentTab));
            }

            return new BeepTabItem
            {
                Index = index,
                Name = documentTab.Id,
                Title = documentTab.Title,
                IconPath = documentTab.IconPath ?? string.Empty,
                SubText = documentTab.TooltipText ?? string.Empty,
                BadgeText = documentTab.BadgeText ?? string.Empty,
                BadgeKind = InferBadgeKind(documentTab.BadgeText),
                CanClose = documentTab.CanClose,
                CloseVisible = documentTab.CanClose,
                IsSelected = documentTab.IsActive,
                WorkspaceState = new BeepTabWorkspaceState
                {
                    IsDirty = documentTab.IsModified,
                    IsPinned = documentTab.IsPinned,
                    GroupKey = documentTab.Group ?? documentTab.DocumentCategory ?? string.Empty
                }
            };
        }

        public BeepDocumentPanel CreateDocument(BeepDocumentHost host, BeepTabItem item, bool activate = false)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            string documentId = GetDocumentId(item);
            BeepDocumentPanel panel = host.AddDocument(
                documentId,
                item.Title,
                string.IsNullOrWhiteSpace(item.IconPath) ? null : item.IconPath,
                activate);

            panel.CanClose = item.CanClose;
            panel.IsModified = item.IsDirty;
            panel.DocumentCategory = string.IsNullOrWhiteSpace(item.GroupKey) ? null : item.GroupKey;

            ApplyVisualState(host, item);
            return panel;
        }

        public void ApplyVisualState(BeepDocumentHost host, BeepTabItem item)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            string documentId = GetDocumentId(item);

            host.PinDocument(documentId, item.IsPinned);

            if (string.IsNullOrWhiteSpace(item.BadgeText))
            {
                host.ClearBadge(documentId);
            }
            else
            {
                host.SetBadge(documentId, item.BadgeText, GetBadgeColor(item.BadgeKind));
            }

            if (item.IsSelected)
            {
                host.SetActiveDocument(documentId);
            }
        }

        private static string GetDocumentId(BeepTabItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.Name))
            {
                return item.Name;
            }

            return item.Index >= 0 ? $"tab-{item.Index}" : Guid.NewGuid().ToString();
        }

        private static BeepTabBadgeKind InferBadgeKind(string? badgeText)
        {
            if (string.IsNullOrWhiteSpace(badgeText))
            {
                return BeepTabBadgeKind.None;
            }

            return int.TryParse(badgeText, out _) ? BeepTabBadgeKind.Count : BeepTabBadgeKind.Info;
        }

        private static Color GetBadgeColor(BeepTabBadgeKind badgeKind)
        {
            return badgeKind switch
            {
                BeepTabBadgeKind.Error => Color.Crimson,
                BeepTabBadgeKind.Warning => Color.OrangeRed,
                BeepTabBadgeKind.Success => Color.SeaGreen,
                BeepTabBadgeKind.Info => Color.SteelBlue,
                BeepTabBadgeKind.Dot => Color.SteelBlue,
                _ => Color.Empty
            };
        }
    }
}