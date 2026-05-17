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
            while (docs.Count < requiredCount)
            {
                docs.Add(CreateNextDesignTimeDocumentDescriptor(host, docs));
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
                _designTimeClosedDocuments.Push(CloneDescriptor(descriptor));
                CloseDesignTimeDocument(host, descriptor.Id);
                docs.RemoveAt(index);
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
                    EnsureDesignDocumentCount(host, docs, 1);
                    host.MergeAllGroups();
                    break;

                case LayoutPreset.SideBySide:
                    EnsureDesignDocumentCount(host, docs, 2);
                    host.TemplateLibrary.ApplyTemplate("side-by-side", host);
                    break;

                case LayoutPreset.Stacked:
                    EnsureDesignDocumentCount(host, docs, 2);
                    host.TemplateLibrary.ApplyTemplate("stacked", host);
                    break;

                case LayoutPreset.ThreeWay:
                    EnsureDesignDocumentCount(host, docs, 3);
                    host.TemplateLibrary.ApplyTemplate("three-way", host);
                    break;

                case LayoutPreset.FourUp:
                    EnsureDesignDocumentCount(host, docs, 4);
                    host.TemplateLibrary.ApplyTemplate("four-up", host);
                    break;

                case LayoutPreset.ThreeWayNested:
                    EnsureDesignDocumentCount(host, docs, 2);
                    host.TemplateLibrary.ApplyTemplate("side-by-side", host);
                    EnsureDesignDocumentCount(host, docs, 3);
                    CreateSplitDesignTimeDocumentInternal(host, docs, horizontal: false, selectSurface: false);
                    break;

                case LayoutPreset.ThreeColumn:
                    EnsureDesignDocumentCount(host, docs, 3);
                    host.TemplateLibrary.ApplyTemplate("side-by-side", host);
                    CreateSplitDesignTimeDocumentInternal(host, docs, horizontal: true, selectSurface: false);
                    break;

                case LayoutPreset.FiveWay:
                    EnsureDesignDocumentCount(host, docs, 4);
                    host.TemplateLibrary.ApplyTemplate("four-up", host);
                    EnsureDesignDocumentCount(host, docs, 5);
                    CreateSplitDesignTimeDocumentInternal(host, docs, horizontal: true, selectSurface: false);
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
