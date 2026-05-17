// BeepDocumentManager.DisplayContainer.cs
// Phase 08 — IDisplayContainer integration for the MDI / document host stack.
//
// Why this exists:
//   The Beep ecosystem already has IDisplayContainer (in TheTechIdea.Beep.Vis.Modules),
//   implemented today by BeepDisplayContainer. Application code passes an
//   IDisplayContainer around to host addins (IDM_Addin) — adding/removing,
//   activating, popping up. Until now the only concrete implementation was
//   BeepDisplayContainer, which is a panel-based tab host.
//
//   This partial makes BeepDocumentManager itself an IDisplayContainer, so the
//   developer can choose any document-presentation mode the manager supports
//   — Tabbed Documents, Browser Tabs, or *Native MDI* — and still receive an
//   IDisplayContainer instance to pass around. In other words:
//
//        IDisplayContainer container = beepDocumentManager;   // ← drop-in
//        container.AddControl("Orders", new OrdersAddin(), ContainerTypeEnum.TabbedPanel);
//
//   The manager routes each AddControl/RemoveControl/ShowControl through the
//   active IBeepDocumentManagerView:
//       • BeepTabbedView     → existing BeepDocumentPanel pipeline
//       • BeepNativeMdiView  → DocumentFormCreated event → addin hosted as
//                              the MDI child Form's content
//
// Lifecycle invariants:
//   • Addin Initialize() runs before content is added.
//   • Addin Dispose()    runs after the panel/MDI form is removed.
//   • Each addin gets exactly one document id, tracked in _addinEntries.
//   • Events are bridged from the manager's own DocumentAdded/Removed/
//     ActiveDocumentChanged so IDisplayContainer subscribers get the
//     consolidated AddinAdded/AddinRemoved/AddinChanged surface.
//
// Threading:
//   IDisplayContainer is a WinForms UI surface — all calls assumed UI thread.
//   ShowPopup falls back to Invoke for cross-thread launches, mirroring
//   BeepDisplayContainer's behavior.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public sealed partial class BeepDocumentManager : IDisplayContainer
    {
        // ─────────────────────────────────────────────────────────────────
        // Bookkeeping
        // ─────────────────────────────────────────────────────────────────

        /// <summary>One entry per addin currently hosted by the manager.</summary>
        private sealed class AddinEntry
        {
            public string                  Title       { get; init; } = string.Empty;
            public IDM_Addin               Addin       { get; init; } = default!;
            public string                  DocumentId  { get; init; } = string.Empty;
            /// <summary>Set when the addin lives in an MDI child Form.</summary>
            public Form?                   MdiChild    { get; init; }
            /// <summary>Set when the addin lives inside a BeepDocumentPanel.</summary>
            public BeepDocumentPanel?      DocumentPanel { get; init; }
        }

        // Title → AddinEntry. Title is the IDisplayContainer key contract.
        private readonly Dictionary<string, AddinEntry> _addinEntries =
            new Dictionary<string, AddinEntry>(StringComparer.OrdinalIgnoreCase);

        // Used when AddControl is called in Native MDI mode: the manager's view
        // fires DocumentFormCreated synchronously inside AddDocument; this field
        // lets the handler attach the addin's content to the right form.
        [ThreadStatic] private static IDM_Addin? _pendingMdiAddin;

        // Single Native-MDI subscription, attached the first time we need it.
        private bool _mdiFormCreatedHooked;

        // Bridge subscription for view → IDisplayContainer events.
        private bool _displayContainerEventsBridged;

        // ─────────────────────────────────────────────────────────────────
        // IDisplayContainer — events
        // ─────────────────────────────────────────────────────────────────

        public event EventHandler<ContainerEvents>? AddinAdded;
        public event EventHandler<ContainerEvents>? AddinRemoved;
        public event EventHandler<ContainerEvents>? AddinMoved;
        public event EventHandler<ContainerEvents>? AddinChanging;
        public event EventHandler<ContainerEvents>? AddinChanged;
        public event EventHandler<IPassedArgs>?     PreCallModule;
        public event EventHandler<IPassedArgs>?     PreShowItem;
        public event EventHandler<KeyCombination>?  KeyPressed;

        // ─────────────────────────────────────────────────────────────────
        // IDisplayContainer — ContainerType property
        //
        // Maps the IDisplayContainer's tiny SinglePanel/TabbedPanel enum onto
        // the manager's far richer view system.
        //   • TabbedPanel — show the tab strip (default).
        //   • SinglePanel — hide the tab strip and let only the active document
        //                   stay visible (the user still gets all the manager
        //                   features programmatically). For Native MDI the
        //                   request is honored by maximising the active child.
        // The wizard / View.* still owns the *real* mode. This property is a
        // legacy compatibility shim.
        // ─────────────────────────────────────────────────────────────────

        private ContainerTypeEnum _idcContainerType = ContainerTypeEnum.TabbedPanel;

        [Browsable(false)] // Real mode lives in View / Setup Wizard.
        public ContainerTypeEnum ContainerType
        {
            get => _idcContainerType;
            set
            {
                if (_idcContainerType == value) return;
                _idcContainerType = value;
                ApplyContainerTypeHint();
            }
        }

        private void ApplyContainerTypeHint()
        {
            switch (_view)
            {
                case BeepTabbedView tabbed when tabbed.Host != null:
                    tabbed.Host.TabPosition = _idcContainerType == ContainerTypeEnum.SinglePanel
                        ? TabStripPosition.Hidden
                        : TabStripPosition.Top;
                    break;

                case BeepNativeMdiView mdi when mdi.ParentForm?.ActiveMdiChild != null
                                            && _idcContainerType == ContainerTypeEnum.SinglePanel:
                    try { mdi.ParentForm.ActiveMdiChild.WindowState = FormWindowState.Maximized; }
                    catch { /* non-fatal */ }
                    break;
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // IDisplayContainer — Add / Remove / Show
        // ─────────────────────────────────────────────────────────────────

        public bool AddControl(string TitleText, IDM_Addin control, ContainerTypeEnum pcontainerType)
        {
            if (control == null || string.IsNullOrWhiteSpace(TitleText)) return false;
            if (_view == null) return false;

            // Single-instance addin enforcement (matches BeepDisplayContainer semantics).
            var addinAttr = control.GetType()
                .GetCustomAttributes(typeof(AddinAttribute), true)
                .FirstOrDefault() as AddinAttribute;
            if (addinAttr != null &&
                addinAttr.ScopeCreateType == AddinScopeCreateType.Single &&
                _addinEntries.ContainsKey(TitleText))
            {
                return false;
            }

            try { control.Initialize(); } catch { return false; }

            EnsureDisplayContainerEventsBridged();

            AddinEntry? entry = _view switch
            {
                BeepNativeMdiView mdi   => HostAddinInMdi(TitleText, control, mdi),
                _                       => HostAddinInPanel(TitleText, control)
            };

            if (entry == null) return false;

            _addinEntries[TitleText] = entry;

            AddinAdded?.Invoke(this, new ContainerEvents
            {
                TitleText     = TitleText,
                Control       = control,
                ContainerType = pcontainerType,
                Guidid        = control.GuidID
            });

            return true;
        }

        public bool RemoveControl(string TitleText, IDM_Addin control)
        {
            if (string.IsNullOrWhiteSpace(TitleText)) return false;
            if (!_addinEntries.TryGetValue(TitleText, out var entry)) return false;
            if (entry.Addin != control) return false;

            // Close the corresponding document — the view raises
            // DocumentRemoved which our bridge translates into AddinRemoved.
            try
            {
                if (entry.MdiChild != null && !entry.MdiChild.IsDisposed)
                    entry.MdiChild.Close();
                else
                    RemoveDocument(entry.DocumentId);
            }
            catch { /* non-fatal */ }

            try { control.Dispose(); } catch { /* addin handles internally */ }

            // The bridge above may have already removed the entry. If not (e.g.
            // remove was rejected by the view), clean up here so the dictionary
            // never lingers with a stale title.
            _addinEntries.Remove(TitleText);

            AddinRemoved?.Invoke(this, new ContainerEvents
            {
                TitleText     = TitleText,
                Control       = control,
                ContainerType = _idcContainerType,
                Guidid        = control?.GuidID
            });

            return true;
        }

        public bool RemoveControlByGuidTag(string guidid)
        {
            if (string.IsNullOrEmpty(guidid)) return false;
            var entry = _addinEntries.Values.FirstOrDefault(e => e.Addin?.GuidID == guidid);
            return entry != null && RemoveControl(entry.Title, entry.Addin);
        }

        public bool RemoveControlByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            return _addinEntries.TryGetValue(name, out var entry) &&
                   RemoveControl(name, entry.Addin);
        }

        public bool ShowControl(string TitleText, IDM_Addin control)
        {
            if (!_addinEntries.TryGetValue(TitleText, out var entry)) return false;
            if (entry.Addin != control) return false;

            var args = new PassedArgs
            {
                ParameterString1 = TitleText,
                Addin            = control,
                EventType        = "ShowControl",
                Title            = TitleText,
                Timestamp        = DateTime.Now,
                AddinName        = control?.Details?.AddinName
            };
            PreShowItem?.Invoke(this, args);

            try { control?.Resume(); } catch { /* addin handles internally */ }

            if (entry.MdiChild != null && !entry.MdiChild.IsDisposed)
            {
                entry.MdiChild.Activate();
                if (_idcContainerType == ContainerTypeEnum.SinglePanel)
                    entry.MdiChild.WindowState = FormWindowState.Maximized;
            }
            else
            {
                ActivateDocument(entry.DocumentId);
            }

            return true;
        }

        public bool IsControlExit(IDM_Addin control)
            => control != null && _addinEntries.Values.Any(e => e.Addin == control);

        public void Clear()
        {
            // Snapshot so removal mutations don't invalidate the enumerator.
            foreach (var entry in _addinEntries.Values.ToList())
            {
                RemoveControl(entry.Title, entry.Addin);
            }
            _addinEntries.Clear();
            try { CloseAllDocuments(); } catch { /* non-fatal */ }
        }

        public IErrorsInfo PressKey(KeyCombination keyCombination)
        {
            try
            {
                KeyPressed?.Invoke(this, keyCombination);
                return new ErrorsInfo { Flag = Errors.Ok, Message = "Key processed successfully" };
            }
            catch (Exception ex)
            {
                return new ErrorsInfo { Flag = Errors.Failed, Message = $"Error processing key: {ex.Message}" };
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // IDisplayContainer — ShowPopup
        // Mirrors BeepDisplayContainer.ShowPopup so addin code that branches
        // on AddControl vs ShowPopup keeps working unchanged.
        // ─────────────────────────────────────────────────────────────────

        public async Task<bool> ShowPopup(IDM_Addin view)
        {
            try
            {
                if (view is Form form)
                {
                    if (form.IsDisposed) return false;

                    void ShowOnUi()
                    {
                        form.StartPosition = FormStartPosition.CenterParent;
                        form.Show();
                    }

                    if (form.InvokeRequired)
                        form.Invoke(new Action(ShowOnUi));
                    else
                        ShowOnUi();

                    await Task.CompletedTask;
                    return true;
                }

                if (view is Control control)
                {
                    var popupForm = new BeepiFormPro
                    {
                        StartPosition  = FormStartPosition.CenterParent,
                        AutoSize       = true,
                        ShowCaptionBar = true,
                        Text           = view.Details?.AddinName
                                          ?? view.Details?.ObjectName
                                          ?? "Beep"
                    };

                    popupForm.Controls.Add(control);
                    control.Dock = DockStyle.Fill;

                    popupForm.OnFormClose += (s, e) =>
                    {
                        try { popupForm.Controls.Remove(control); } catch { }
                    };

                    Form? owner = (_view as BeepNativeMdiView)?.ParentForm
                                  ?? Form.ActiveForm;
                    popupForm.Show(owner);

                    await Task.CompletedTask;
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Hosting helpers — Tabbed/Browser modes
        // ─────────────────────────────────────────────────────────────────

        private AddinEntry? HostAddinInPanel(string title, IDM_Addin control)
        {
            var panel = AddDocument(title, iconPath: null, activate: true);
            if (panel == null) return null;

            // BeepDocumentPanel.DocumentId is non-null; fall back to the title
            // only when the view returns a panel that hasn't been registered.
            string docId = string.IsNullOrEmpty(panel.DocumentId) ? title : panel.DocumentId;

            if (control is Form formAddin)
            {
                // Embed a Form addin inside the document panel.
                try
                {
                    formAddin.TopLevel        = false;
                    formAddin.FormBorderStyle = FormBorderStyle.None;
                    formAddin.Dock            = DockStyle.Fill;
                    formAddin.Visible         = true;
                    panel.Controls.Add(formAddin);
                }
                catch
                {
                    return null;
                }
            }
            else if (control is Control winControl)
            {
                winControl.Dock    = DockStyle.Fill;
                winControl.Visible = true;
                panel.Controls.Add(winControl);
            }
            else
            {
                // No visible surface — addin runs headless inside the tab.
            }

            return new AddinEntry
            {
                Title         = title,
                Addin         = control,
                DocumentId    = docId,
                DocumentPanel = panel
            };
        }

        // ─────────────────────────────────────────────────────────────────
        // Hosting helpers — Native MDI mode
        // ─────────────────────────────────────────────────────────────────

        private AddinEntry? HostAddinInMdi(string title, IDM_Addin control, BeepNativeMdiView mdi)
        {
            if (mdi.ParentForm == null) return null;
            EnsureMdiFormCreatedHook(mdi);

            string? capturedId = null;
            Form?    capturedForm = null;

            EventHandler<DocumentAddedEventArgs> captureAdded = (s, e) =>
            {
                capturedId   = e?.Descriptor?.Id;
                capturedForm = (s as BeepNativeMdiView)?.ParentForm?.ActiveMdiChild;
            };

            // For Form addins we don't want the view to create its own host
            // form — we'll reparent the addin directly. For Control addins we
            // let the view create the host and we attach the control via the
            // DocumentFormCreated hook (set by EnsureMdiFormCreatedHook).
            if (control is Form addinForm)
            {
                try
                {
                    string id = Guid.NewGuid().ToString("N");
                    addinForm.MdiParent     = mdi.ParentForm;
                    addinForm.Text          = string.IsNullOrEmpty(title)
                                                ? (addinForm.Text ?? "Document")
                                                : title;
                    addinForm.Show();
                    addinForm.Activate();

                    return new AddinEntry
                    {
                        Title      = title,
                        Addin      = control,
                        DocumentId = id,
                        MdiChild   = addinForm
                    };
                }
                catch
                {
                    return null;
                }
            }

            // Control addin path: stash and let the view create the form,
            // then DocumentFormCreated hook (below) reparents the control.
            _pendingMdiAddin = control;
            try
            {
                mdi.DocumentAdded += captureAdded;
                AddDocument(title);
            }
            finally
            {
                mdi.DocumentAdded -= captureAdded;
                _pendingMdiAddin   = null;
            }

            if (capturedId == null || capturedForm == null) return null;

            return new AddinEntry
            {
                Title      = title,
                Addin      = control,
                DocumentId = capturedId,
                MdiChild   = capturedForm
            };
        }

        private void EnsureMdiFormCreatedHook(BeepNativeMdiView mdi)
        {
            if (_mdiFormCreatedHooked) return;
            mdi.DocumentFormCreated += OnNativeMdiFormCreated;
            _mdiFormCreatedHooked = true;
        }

        private void OnNativeMdiFormCreated(object? sender, MdiDocumentEventArgs e)
        {
            // Only attach when an AddControl call is currently in flight. This
            // prevents accidental re-hosting when MDI children are added by
            // other code paths.
            var addin = _pendingMdiAddin;
            if (addin == null) return;
            if (e?.Form == null) return;

            try
            {
                if (addin is Control winControl && !(addin is Form))
                {
                    winControl.Dock    = DockStyle.Fill;
                    winControl.Visible = true;
                    e.Form.Controls.Add(winControl);
                    e.Form.ClientSize = new Size(
                        Math.Max(e.Form.ClientSize.Width,  winControl.PreferredSize.Width  + 32),
                        Math.Max(e.Form.ClientSize.Height, winControl.PreferredSize.Height + 32));
                }
            }
            catch { /* addin handles errors internally */ }
        }

        // ─────────────────────────────────────────────────────────────────
        // Event bridge — manager events → IDisplayContainer events
        // ─────────────────────────────────────────────────────────────────

        private void EnsureDisplayContainerEventsBridged()
        {
            if (_displayContainerEventsBridged) return;

            DocumentRemoved        += OnDisplayContainerDocumentRemoved;
            ActiveDocumentChanged  += OnDisplayContainerActiveDocumentChanged;

            _displayContainerEventsBridged = true;
        }

        private void OnDisplayContainerDocumentRemoved(object? sender, DocumentEventArgs e)
        {
            if (e == null || string.IsNullOrEmpty(e.DocumentId)) return;

            var match = _addinEntries.FirstOrDefault(kv => kv.Value.DocumentId == e.DocumentId);
            if (match.Key == null) return;

            _addinEntries.Remove(match.Key);

            AddinRemoved?.Invoke(this, new ContainerEvents
            {
                TitleText     = match.Key,
                Control       = match.Value.Addin,
                ContainerType = _idcContainerType,
                Guidid        = match.Value.Addin?.GuidID
            });
        }

        private void OnDisplayContainerActiveDocumentChanged(object? sender, DocumentEventArgs e)
        {
            if (e == null || string.IsNullOrEmpty(e.DocumentId)) return;

            var match = _addinEntries.FirstOrDefault(kv => kv.Value.DocumentId == e.DocumentId);
            if (match.Key == null) return;

            AddinChanged?.Invoke(this, new ContainerEvents
            {
                TitleText     = match.Key,
                Control       = match.Value.Addin,
                ContainerType = _idcContainerType,
                Guidid        = match.Value.Addin?.GuidID
            });
        }

        // ─────────────────────────────────────────────────────────────────
        // Cleanup — runs from Component.Dispose path
        // ─────────────────────────────────────────────────────────────────

        private void DisposeDisplayContainer()
        {
            try
            {
                if (_displayContainerEventsBridged)
                {
                    DocumentRemoved       -= OnDisplayContainerDocumentRemoved;
                    ActiveDocumentChanged -= OnDisplayContainerActiveDocumentChanged;
                    _displayContainerEventsBridged = false;
                }
                if (_mdiFormCreatedHooked && _view is BeepNativeMdiView mdi)
                {
                    mdi.DocumentFormCreated -= OnNativeMdiFormCreated;
                    _mdiFormCreatedHooked   = false;
                }
                foreach (var entry in _addinEntries.Values.ToList())
                {
                    try { entry.Addin?.Dispose(); } catch { }
                }
                _addinEntries.Clear();
            }
            catch { /* non-fatal */ }
        }
    }
}
