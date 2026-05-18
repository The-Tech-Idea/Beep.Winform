// BeepDocumentHostDesigner.LayoutPresets.cs
// Phase 03 — split out of BeepDocumentHostDesigner.cs.
//
// Layout-preset and Layout-Assistant logic for BeepDocumentHost. Each
// preset maps to one or more BeepDocumentLayoutTemplates (registered in
// BeepDocumentLayoutTemplateLibrary) and is applied inside the same
// DesignerTransaction surface used by every other design-time CRUD
// operation, so a Ctrl+Z undoes the entire preset application.
//
//   ApplyLayoutAssistant — applies the wizard's per-document specs
//   (titles + InitialContent) then runs the preset and re-selects the
//   active panel.
//
//   ApplyAssistantDocumentSpecifications — reconciles the
//   DesignTimeDocuments collection with the per-document specs from the
//   wizard (grows it, renames existing rows, archives extras into the
//   _designTimeClosedDocuments stack).
//
//   ApplyDesignTimeLayoutPresetCore — the actual preset → template
//   mapping (Single, SideBySide, Stacked, ThreeWay, FourUp, etc.).
//
//   GetLayoutPresetOrder / GetLayoutPresetDisplayName / GetAutoHideLabel
//   — small lookup helpers shared by the context menu and verbs.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Dialogs;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public partial class BeepDocumentHostDesigner
    {
        // ── Layout assistant ────────────────────────────────────────────────

        private void ApplyLayoutAssistant(BeepDocumentHost host,
                                          Collection<DocumentDescriptor> docs,
                                          LayoutPreset preset,
                                          IReadOnlyList<DocumentLayoutAssistantItem> documents)
        {
            ApplyAssistantDocumentSpecifications(host, docs, documents);
            SyncHostWithDesignTimeDocuments(host, docs);
            ApplyDesignTimeLayoutPresetCore(host, docs, preset, selectSurface: false);

            BeepDocumentPanel? panel = EnsureActiveDesignDocumentSurface(host, docs, selectSurface: true);
            SyncDesignerSelection((object?)panel ?? host);
        }

        private void ApplyAssistantDocumentSpecifications(BeepDocumentHost host,
                                                          Collection<DocumentDescriptor> docs,
                                                          IReadOnlyList<DocumentLayoutAssistantItem> documents)
        {
            int requiredCount = Math.Max(1, documents.Count);
            var coordinator = new DesignTimeDocumentCoordinator(this, host, docs);
            while (docs.Count < requiredCount)
            {
                if (coordinator.AddNewDocument(activate: false, selectSurface: false) == null)
                {
                    break;
                }
            }

            requiredCount = Math.Min(requiredCount, docs.Count);
            if (requiredCount == 0)
            {
                return;
            }

            for (int index = 0; index < requiredCount; index++)
            {
                DocumentLayoutAssistantItem specification = documents[index];
                DocumentDescriptor descriptor = docs[index];
                descriptor.Title = NormalizeDocumentTitle(specification.Title, index + 1);
                descriptor.InitialContent = specification.InitialContent;
            }

            for (int index = docs.Count - 1; index >= requiredCount; index--)
            {
                DocumentDescriptor descriptor = docs[index];
                if (coordinator.RemoveDocument(descriptor.Id, selectSurface: false, out DocumentDescriptor? snapshot)
                    && snapshot != null)
                {
                    _designTimeClosedDocuments.Push(snapshot);
                }
            }
        }

        // ── Preset → template mapping ───────────────────────────────────────

        private void ApplyDesignTimeLayoutPresetCore(BeepDocumentHost host,
                                                     Collection<DocumentDescriptor> docs,
                                                     LayoutPreset preset,
                                                     bool selectSurface)
        {
            switch (preset)
            {
                case LayoutPreset.Single:
                    if (!EnsureDesignDocumentCount(host, docs, 1)) return;
                    host.MergeAllGroups();
                    break;

                case LayoutPreset.SideBySide:
                    if (!EnsureDesignDocumentCount(host, docs, 2)) return;
                    host.TemplateLibrary.ApplyTemplate("side-by-side", host);
                    break;

                case LayoutPreset.Stacked:
                    if (!EnsureDesignDocumentCount(host, docs, 2)) return;
                    host.TemplateLibrary.ApplyTemplate("stacked", host);
                    break;

                case LayoutPreset.ThreeWay:
                    if (!EnsureDesignDocumentCount(host, docs, 3)) return;
                    host.TemplateLibrary.ApplyTemplate("three-way", host);
                    break;

                case LayoutPreset.FourUp:
                    if (!EnsureDesignDocumentCount(host, docs, 4)) return;
                    host.TemplateLibrary.ApplyTemplate("four-up", host);
                    break;

                case LayoutPreset.ThreeWayNested:
                    if (!EnsureDesignDocumentCount(host, docs, 2)) return;
                    host.TemplateLibrary.ApplyTemplate("side-by-side", host);
                    if (!EnsureDesignDocumentCount(host, docs, 3)) return;
                    if (CreateSplitDesignTimeDocumentInternal(host, docs, horizontal: false, selectSurface: false) == null) return;
                    break;

                case LayoutPreset.ThreeColumn:
                    if (!EnsureDesignDocumentCount(host, docs, 3)) return;
                    host.TemplateLibrary.ApplyTemplate("side-by-side", host);
                    if (CreateSplitDesignTimeDocumentInternal(host, docs, horizontal: true, selectSurface: false) == null) return;
                    break;

                case LayoutPreset.FiveWay:
                    if (!EnsureDesignDocumentCount(host, docs, 4)) return;
                    host.TemplateLibrary.ApplyTemplate("four-up", host);
                    if (!EnsureDesignDocumentCount(host, docs, 5)) return;
                    if (CreateSplitDesignTimeDocumentInternal(host, docs, horizontal: true, selectSurface: false) == null) return;
                    break;
            }

            BeepDocumentPanel? panel = EnsureActiveDesignDocumentSurface(host, docs, selectSurface: selectSurface);
            if (selectSurface)
            {
                SyncDesignerSelection((object?)panel ?? host);
            }
        }

        // ── Lookup helpers (context menu / verbs / status banners) ──────────

        private static IReadOnlyList<LayoutPreset> GetLayoutPresetOrder()
            => new[]
            {
                LayoutPreset.Single,
                LayoutPreset.SideBySide,
                LayoutPreset.Stacked,
                LayoutPreset.ThreeWay,
                LayoutPreset.ThreeWayNested,
                LayoutPreset.FourUp,
                LayoutPreset.ThreeColumn,
                LayoutPreset.FiveWay
            };

        private static string GetLayoutPresetDisplayName(LayoutPreset preset)
            => preset switch
            {
                LayoutPreset.SideBySide     => "Side-by-Side",
                LayoutPreset.ThreeWay       => "Three-Way",
                LayoutPreset.ThreeWayNested => "Three-Way Nested",
                LayoutPreset.FourUp         => "Four-Up",
                LayoutPreset.ThreeColumn    => "Three Column",
                LayoutPreset.FiveWay        => "Five-Way",
                _                           => preset.ToString()
            };

        private static string GetAutoHideLabel(AutoHideSide side)
            => side switch
            {
                AutoHideSide.Left   => "Left",
                AutoHideSide.Right  => "Right",
                AutoHideSide.Top    => "Top",
                AutoHideSide.Bottom => "Bottom",
                _                   => side.ToString()
            };
    }
}
