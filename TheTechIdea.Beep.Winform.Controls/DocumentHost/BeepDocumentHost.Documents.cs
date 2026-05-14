using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentHost
    {
        // ─────────────────────────────────────────────────────────────────
        // Batch-Add API (performance — 100+ documents)
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Suspends layout recalculation and tab-strip repaints during a bulk
        /// <see cref="AddDocument"/> loop.  Call <see cref="EndBatchAddDocuments"/>
        /// when done to flush with a single layout pass.
        /// </summary>
        public void BeginBatchAddDocuments()
        {
            _batchAdding    = true;
            _layoutSuspended = true;
            foreach (var grp in _groups)
                grp.TabStrip.BeginBatchAdd();
        }

        /// <summary>
        /// Ends a batch started by <see cref="BeginBatchAddDocuments"/>.
        /// Calls one <see cref="RecalculateLayout"/> and one tab-strip flush.
        /// </summary>
        public void EndBatchAddDocuments()
        {
            _batchAdding     = false;
            _layoutSuspended = false;
            foreach (var grp in _groups)
                grp.TabStrip.EndBatchAdd();
            RecalculateLayout();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Batch-Close API (performance — 5.6)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Begins a batch close operation.  Individual <see cref="CloseDocument"/> calls
        /// are queued rather than executed immediately; call <see cref="EndBatchCloseDocuments"/>
        /// to flush the queue with a single layout pass.
        /// </summary>
        public void BeginBatchCloseDocuments()
        {
            _batchClosing = true;
            _pendingCloseIds.Clear();
        }

        /// <summary>
        /// Ends a batch close started by <see cref="BeginBatchCloseDocuments"/> and
        /// closes all queued documents.
        /// </summary>
        public void EndBatchCloseDocuments()
        {
            _batchClosing = false;
            var toClose = new System.Collections.Generic.List<string>(_pendingCloseIds);
            _pendingCloseIds.Clear();
            foreach (var id in toClose)
                CloseDocument(id);
        }

        /// <summary>
        /// Moves <paramref name="documentId"/> from its current group to
        /// <paramref name="targetGroupId"/> with a single layout pass.
        /// Returns <see langword="false"/> if either the document or target group is not found.
        /// </summary>
        public bool BatchMoveDocument(string documentId, string targetGroupId)
        {
            var coordinator = new DocumentHostTreeMutationCoordinator(this);
            bool moved = false;
            coordinator.Execute(DocumentHostOperationNames.BatchMoveDocument, () =>
            {
                moved = MoveDocumentToGroupCore(documentId, targetGroupId, recalcLayout: false);
            });
            return moved;
        }

        private bool ContainsOpenDocument(string documentId)
            => _panels.ContainsKey(documentId) || _floatWindows.ContainsKey(documentId);

        /// <summary>
        /// Returns the current dock placement for the specified document.
        /// </summary>
        public DocumentDockState GetDocumentDockState(string documentId)
            => TryGetDocumentDockState(documentId, out var state) ? state : DocumentDockState.None;

        /// <summary>
        /// Returns <see langword="true"/> when the document is tracked by this host and
        /// populates its current dock placement.
        /// </summary>
        public bool TryGetDocumentDockState(string documentId, out DocumentDockState state)
        {
            if (_floatWindows.ContainsKey(documentId))
            {
                state = DocumentDockState.Floating;
                return true;
            }

            if (_autoHideMap.ContainsKey(documentId))
            {
                state = DocumentDockState.AutoHide;
                return true;
            }

            if (_panels.ContainsKey(documentId))
            {
                state = DocumentDockState.Docked;
                return true;
            }

            state = DocumentDockState.None;
            return false;
        }

        private void OnDocumentDockStateChanged(string documentId,
                                                string title,
                                                DocumentDockState oldState,
                                                DocumentDockState newState,
                                                AutoHideSide? side = null)
        {
            if (oldState == newState)
                return;

            _docGroupMap.TryGetValue(documentId, out var groupId);
            DocumentDockStateChanged?.Invoke(
                this,
                new DocumentDockStateChangedEventArgs(documentId,
                                                      title,
                                                      oldState,
                                                      newState,
                                                      groupId,
                                                      side));
        }

        private bool TryGetDocumentTab(string documentId,
                                       out BeepDocumentTabStrip tabStrip,
                                       out BeepDocumentTab? tab)
        {
            tabStrip = GetGroupForDocument(documentId).TabStrip;
            tab = tabStrip.FindTabById(documentId);
            return tab != null;
        }

        private string ResolveOpenTargetGroupId(DocumentOpenOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            return options.Target switch
            {
                DocumentOpenTarget.PrimaryGroup => _primaryGroup.GroupId,
                DocumentOpenTarget.SpecificGroup
                    when !string.IsNullOrWhiteSpace(options.TargetGroupId)
                         && _groupById.ContainsKey(options.TargetGroupId)
                    => options.TargetGroupId,
                _ => _activeGroup?.GroupId ?? _primaryGroup.GroupId
            };
        }

        private bool TryActivateOpenDocument(string documentId)
        {
            if (_panels.ContainsKey(documentId))
                return SetActiveDocument(documentId);

            if (_floatWindows.TryGetValue(documentId, out var floatWindow))
            {
                if (floatWindow.WindowState == FormWindowState.Minimized)
                    floatWindow.WindowState = FormWindowState.Normal;

                floatWindow.BringToFront();
                floatWindow.Activate();
                return true;
            }

            return false;
        }

        private BeepDocumentPanel OpenDocumentCore(DocumentDescriptor descriptor,
                                                   DocumentOpenOptions? options,
                                                   bool attachDescriptorChanges,
                                                   bool raiseDocumentAddedEvent)
        {
            ArgumentNullException.ThrowIfNull(descriptor);

            if (ContainsOpenDocument(descriptor.Id))
                throw new InvalidOperationException($"Document '{descriptor.Id}' already exists.");

            options ??= new DocumentOpenOptions();

            var panel = AddDocument(descriptor.Id, descriptor.Title, descriptor.IconPath, activate: false);
            string targetGroupId = ResolveOpenTargetGroupId(options);

            if (!string.Equals(targetGroupId, _primaryGroup.GroupId, StringComparison.Ordinal))
                MoveDocumentToGroup(descriptor.Id, targetGroupId);

            ApplyDescriptorToOpenDocument(descriptor, panel, raiseDocumentAddedEvent);

            if (attachDescriptorChanges)
                AttachDescriptorPropertyChanged(descriptor);

            if (options.Activate)
                SetActiveDocument(descriptor.Id);

            return panel;
        }
        // ─────────────────────────────────────────────────────────────────────
        // Add
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Adds a new empty <see cref="BeepDocumentPanel"/> with an auto-generated id.
        /// Returns it so the caller can populate the content.
        /// </summary>
        public BeepDocumentPanel AddDocument(string title,
                                             string? iconPath = null,
                                             bool activate = true)
            => AddDocument(Guid.NewGuid().ToString(), title, iconPath, activate);

        /// <summary>Adds a new empty panel with an explicit document id.</summary>
        public BeepDocumentPanel AddDocument(string documentId,
                                             string title,
                                             string? iconPath = null,
                                             bool activate = true)
        {
            if (ContainsOpenDocument(documentId))
                throw new InvalidOperationException($"Document '{documentId}' already exists.");

            var panel = new BeepDocumentPanel(documentId, title)
            {
                Bounds    = _contentArea.ClientRectangle,
                Theme = ThemeName,
                IconPath  = iconPath,
                Visible   = false
            };

            _contentArea.Controls.Add(panel);
            _panels[documentId] = panel;

            // Register to primary group
            _docGroupMap[documentId] = _primaryGroup.GroupId;
            _primaryGroup.DocumentIds.Add(documentId);

            _tabStrip.AddTab(documentId, title, iconPath, activate: false);

            if (activate || _panels.Count == 1)
                SetActiveDocument(documentId);

            panel.ModifiedChanged += (s, _) =>
                _tabStrip.SetTabModified(documentId, panel.IsModified);

            // Auto-show the mini toolbar when the mouse enters the document panel
            WireMiniToolbarToPanel(panel, documentId);

            // Skip the expensive layout pass when batching — EndBatchAddDocuments will flush
            if (!_batchAdding)
                RecalculateLayout();

            return panel;
        }

        /// <summary>
        /// Opens a descriptor-backed document in the requested group.
        /// Throws when the document id is already open in this host.
        /// </summary>
        public BeepDocumentPanel OpenDocument(DocumentDescriptor descriptor,
                                              DocumentOpenOptions? options = null)
            => OpenDocumentCore(descriptor,
                                options,
                                attachDescriptorChanges: true,
                                raiseDocumentAddedEvent: true);

        /// <summary>
        /// Opens a document with the given identity and title in the requested group.
        /// Throws when the document id is already open in this host.
        /// </summary>
        public BeepDocumentPanel OpenDocument(string documentId,
                                              string title,
                                              string? iconPath = null,
                                              DocumentOpenOptions? options = null)
            => OpenDocumentCore(DocumentDescriptor.Create(documentId, title, iconPath),
                                options,
                                attachDescriptorChanges: false,
                                raiseDocumentAddedEvent: false);

        /// <summary>
        /// Opens the descriptor-backed document when missing, or activates the existing
        /// docked or floated document when it is already open.
        /// Returns true when a new document was created; false when an existing one was activated.
        /// </summary>
        public bool OpenOrActivate(DocumentDescriptor descriptor,
                                   DocumentOpenOptions? options = null)
        {
            ArgumentNullException.ThrowIfNull(descriptor);

            if (ContainsOpenDocument(descriptor.Id))
            {
                if (_panels.TryGetValue(descriptor.Id, out _))
                    ApplyDescriptorState(descriptor);

                if ((options?.Activate).GetValueOrDefault(true))
                    TryActivateOpenDocument(descriptor.Id);

                return false;
            }

            OpenDocumentCore(descriptor,
                             options,
                             attachDescriptorChanges: true,
                             raiseDocumentAddedEvent: true);
            return true;
        }

        /// <summary>
        /// Opens the document when missing, or activates the existing one when it is already open.
        /// Returns true when a new document was created; false when an existing one was activated.
        /// </summary>
        public bool OpenOrActivate(string documentId,
                                   string title,
                                   string? iconPath = null,
                                   DocumentOpenOptions? options = null)
        {
            if (ContainsOpenDocument(documentId))
            {
                if ((options?.Activate).GetValueOrDefault(true))
                    TryActivateOpenDocument(documentId);

                return false;
            }

            OpenDocumentCore(DocumentDescriptor.Create(documentId, title, iconPath),
                             options,
                             attachDescriptorChanges: false,
                             raiseDocumentAddedEvent: false);
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Activate
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Activates the document with the given id.  Returns false if not found.</summary>
        public bool SetActiveDocument(string documentId)
        {
            if (!_panels.TryGetValue(documentId, out var panel)) return false;

            // Hide the previously active panel (and capture preview if enabled)
            if (_activeDocumentId != null
                && _panels.TryGetValue(_activeDocumentId, out var prev)
                && prev != panel)
            {
                // Lazy deferred snapshot — schedule via BeginInvoke so the tab
                // switch itself is not blocked by DrawToBitmap.
                if (_tabPreviewEnabled)
                {
                    var captureId = _activeDocumentId;  // capture local copy for lambda
                    if (IsHandleCreated)
                        BeginInvoke(new Action(() => CaptureSnapshot(captureId)));
                    else
                        CaptureSnapshot(captureId);
                }
                prev.Visible = false;
            }

            // Show the new panel — use the group's content area, not always _contentArea
            var activeGrp = GetGroupForDocument(documentId);
            panel.Bounds  = activeGrp.ContentArea.ClientRectangle;
            panel.Anchor  = AnchorStyles.Top | AnchorStyles.Bottom
                          | AnchorStyles.Left | AnchorStyles.Right;
            // 5.2: Ensure deferred content is loaded the first time the panel is activated
            panel.EnsureContentLoaded();
            panel.Visible = true;
            panel.BringToFront();

            _activeDocumentId = documentId;
            _activeGroup      = activeGrp;
            PushMru(documentId);
            activeGrp.TabStrip.ActivateTabById(documentId);
            AccessibleName = panel.DocumentTitle;
            AccessibleDescription = $"Active document {panel.DocumentTitle}";

            ActiveDocumentChanged?.Invoke(this,
                new DocumentEventArgs(documentId, panel.DocumentTitle));
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Close
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Closes and removes the document.  Returns false if not found.</summary>
        public bool CloseDocument(string documentId)
        {
            // 5.6: If a batch-close is in progress, queue instead of closing immediately
            if (_batchClosing)
            {
                if (_panels.ContainsKey(documentId) && !_pendingCloseIds.Contains(documentId))
                    _pendingCloseIds.Add(documentId);
                return true;
            }

            if (!_panels.TryGetValue(documentId, out var panel)) return false;

            string title = panel.DocumentTitle;
            var coordinator = new DocumentHostTreeMutationCoordinator(this);

            coordinator.Execute(DocumentHostOperationNames.CloseDocument, () =>
            {
                // Push to closed-tab history BEFORE destroying the panel
                PushClosedTab(new ClosedTabRecord(documentId, title, panel.IconPath));
                RemoveMru(documentId);

                // Remove from whichever group owns this document
                if (_docGroupMap.TryGetValue(documentId, out var closingGroupId))
                {
                    _docGroupMap.Remove(documentId);
                    var ownerGrp = _groups.Find(g => g.GroupId == closingGroupId);
                    if (ownerGrp != null)
                    {
                        ownerGrp.DocumentIds.Remove(documentId);
                        int gTabIdx = ownerGrp.TabStrip.Tabs.ToList().FindIndex(t => t.Id == documentId);
                        if (gTabIdx >= 0) ownerGrp.TabStrip.RemoveTabAt(gTabIdx);

                        if (ownerGrp.IsEmpty && !ownerGrp.IsPrimary)
                            CollapseEmptyGroup(ownerGrp);
                    }
                }
                else
                {
                    // Fallback: remove from primary strip
                    int tabIdx = _tabStrip.Tabs.ToList().FindIndex(t => t.Id == documentId);
                    if (tabIdx >= 0) _tabStrip.RemoveTabAt(tabIdx);
                }

                _panels.Remove(documentId);
                panel.Parent?.Controls.Remove(panel);
                panel.Dispose();

                if (_activeDocumentId == documentId)
                {
                    _activeDocumentId = null;
                    if (_panels.Count > 0)
                        SetActiveDocument(_panels.Keys.Last());
                }
            });

            // 5.7: Free cached thumbnail for the closed document
            InvalidatePreview(documentId);

            DocumentClosed?.Invoke(this, new DocumentEventArgs(documentId, title));
            RemoveDescriptorForId(documentId);          // keep Documents list in sync
            return true;
        }

        /// <summary>
        /// Closes every open document that is allowed to close (respects <see cref="BeepDocumentPanel.CanClose"/>
        /// and the pinned state).  Returns <see langword="true"/> when at least one document was closed.
        /// </summary>
        public bool CloseAllDocuments()
        {
            var ids = _panels.Keys.Where(CanCloseDocument).ToList();
            if (ids.Count == 0) return false;
            foreach (var id in ids)
                CloseDocument(id);
            return true;
        }

        /// <summary>
        /// Returns <see langword="true"/> when the document identified by
        /// <paramref name="documentId"/> is allowed to close (i.e. not pinned and
        /// not otherwise marked non-closable).
        /// </summary>
        private bool CanCloseDocument(string documentId)
        {
            if (_panels.TryGetValue(documentId, out var p)) return p.CanClose;
            if (_floatWindows.TryGetValue(documentId, out var fw)) return fw.HostedPanel?.CanClose != false;
            return true;
        }

        /// <summary>
        /// Closes all <em>closable</em> documents in the specified group, respecting the
        /// per-document <c>CanClose</c> flag (pinned tabs are skipped).
        /// Returns <see langword="true"/> when at least one document was closed.
        /// </summary>
        public bool CloseGroupDocuments(string groupId)
        {
            var group = _groups?.Find(g => g.GroupId == groupId);
            if (group == null) return false;
            var ids = group.DocumentIds.Where(CanCloseDocument).ToList();
            if (ids.Count == 0) return false;
            foreach (var id in ids)
                CloseDocument(id);
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Mini toolbar — hover-triggered contextual action bar
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Wires <see cref="Features.BeepDocumentMiniToolbar"/> hover show/hide to
        /// <paramref name="panel"/>.  The toolbar fades in 150 ms after <c>MouseEnter</c>
        /// and fades out when the pointer leaves both the panel and the toolbar.
        /// </summary>
        private void WireMiniToolbarToPanel(BeepDocumentPanel panel, string documentId)
        {
            System.Windows.Forms.Timer? leaveTimer = null;

            panel.MouseEnter += (_, _) =>
            {
                leaveTimer?.Stop();
                var tb = MiniToolbar;
                tb.SetActions(BuildPanelMiniToolbarActions(documentId));
                tb.ShowAt(panel);
            };

            panel.MouseLeave += (_, _) =>
            {
                if (leaveTimer == null)
                {
                    leaveTimer = new System.Windows.Forms.Timer { Interval = 150 };
                    leaveTimer.Tick += (__, ___) =>
                    {
                        leaveTimer.Stop();
                        // Stay visible if the pointer moved onto the toolbar itself
                        if (!MiniToolbar.Bounds.Contains(System.Windows.Forms.Cursor.Position))
                            MiniToolbar.FadeOut();
                    };
                }
                leaveTimer.Start();
            };
        }

        /// <summary>
        /// Builds the default set of mini-toolbar actions for a document panel:
        /// Float, Pin, Maximize / Restore, Close.
        /// </summary>
        private Features.MiniToolbarAction[] BuildPanelMiniToolbarActions(string documentId)
        {
            return new Features.MiniToolbarAction[]
            {
                new Features.MiniToolbarAction
                {
                    Glyph       = "\uD83D\uDDD7",   // 🗗 restore / float
                    ToolTipText = "Float",
                    CommandId   = "document.float",
                    Enabled     = CanFloatNow(),
                    Execute     = () => FloatDocument(documentId),
                },
                new Features.MiniToolbarAction
                {
                    Glyph       = "\uD83D\uDCCC",   // 📌 pin
                    ToolTipText = "Pin",
                    CommandId   = "document.pin",
                    Enabled     = CanPinNow(),
                    Execute     = () => PinDocument(documentId),
                },
                new Features.MiniToolbarAction
                {
                    Glyph       = "\u2610",          // ☐ maximize / restore
                    ToolTipText = "Maximize",
                    CommandId   = "document.maximize",
                    Enabled     = true,
                    Execute     = ToggleMaximizeActiveDocument,
                },
                new Features.MiniToolbarAction
                {
                    Glyph       = "\u2715",          // ✕ close
                    ToolTipText = "Close",
                    CommandId   = "document.close",
                    Enabled     = true,
                    Execute     = () => CloseDocument(documentId),
                },
            };
        }

        // ─────────────────────────────────────────────────────────────────────
        // Float / Dock
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Floats the document panel into its own <see cref="BeepDocumentFloatWindow"/>.</summary>
        public bool FloatDocument(string documentId)
        {
            if (!_panels.TryGetValue(documentId, out var panel)) return false;
            if (_floatWindows.ContainsKey(documentId)) return false;   // already floated
            if (!CanFloatNow()) return false;                          // policy check
            PushUndoState();

            string title = panel.DocumentTitle;
            var previousState = GetDocumentDockState(documentId);
            var ownerGroup = GetGroupForDocument(documentId);

            // Record the source group so DockBack can re-target intelligently (P4-002)
            var floatDesc = _documents?.FirstOrDefault(d => d.Id == documentId);
            if (floatDesc != null && ownerGroup != null)
                floatDesc.PreviousGroupId = ownerGroup.GroupId;

            var coordinator = new DocumentHostTreeMutationCoordinator(this);
            coordinator.Execute(DocumentHostOperationNames.FloatDocument, () =>
            {
                ownerGroup.ContentArea.Controls.Remove(panel);
                _panels.Remove(documentId);

                int tabIdx = ownerGroup.TabStrip.Tabs.ToList().FindIndex(t => t.Id == documentId);
                if (tabIdx >= 0) ownerGroup.TabStrip.RemoveTabAt(tabIdx);

                if (_activeDocumentId == documentId)
                {
                    _activeDocumentId = null;
                    if (_panels.Count > 0) SetActiveDocument(_panels.Keys.Last());
                }
            });

            var fw = new BeepDocumentFloatWindow(panel, ThemeName);
            fw.DockBack       += (s, ea) => DockBackDocument(documentId);
            fw.WindowMoved    += (s, screenPt) => OnFloatWindowMoved(documentId, screenPt);
            fw.TitleBarMouseUp += (s, ea) => OnFloatWindowDropped(documentId);
            // Block close when the document is pinned / non-closable (unless we are
            // intentionally docking it back, in which case DockBackDocument has already
            // added the id to _dockBackClosingIds before calling fw.Close()).
            fw.FormClosing += (s, e) =>
            {
                if (!e.Cancel && !_dockBackClosingIds.Contains(documentId) && !CanCloseDocument(documentId))
                    e.Cancel = true;
            };
            fw.FormClosed  += (s, _) =>
            {
                if (_isDisposingHost)
                    return;

                try
                {
                    _dockOverlay?.HideOverlay();
                }
                catch
                {
                    // Ignore overlay teardown errors during fast host/window transitions.
                }

                _floatWindows.Remove(documentId);

                // Do not report DocumentClosed when this close is part of docking-back.
                if (_dockBackClosingIds.Remove(documentId))
                    return;

                DocumentClosed?.Invoke(this, new DocumentEventArgs(documentId, title));
            };
            _floatWindows[documentId] = fw;
            fw.Show();

            DocumentFloated?.Invoke(this, new DocumentEventArgs(documentId, title));
            OnDocumentDockStateChanged(documentId, title, previousState, DocumentDockState.Floating);
            return true;
        }

        /// <summary>Docks a previously floated document back into this host.</summary>
        public bool DockBackDocument(string documentId)
            => DockBackDocumentInternal(documentId, useCoordinator: true);

        private bool DockBackDocumentInternal(string documentId, bool useCoordinator)
        {
            if (!_floatWindows.TryGetValue(documentId, out var fw)) return false;
            PushUndoState();

            var previousState = GetDocumentDockState(documentId);

            var panel = fw.DetachPanel();
            _floatWindows.Remove(documentId);
            _dockBackClosingIds.Add(documentId);
            fw.Close();

            string title = panel.DocumentTitle;
            var targetGroup = GetGroupForDocument(documentId);
            Action mutation = () =>
            {
                _panels[documentId] = panel;
                targetGroup.ContentArea.Controls.Add(panel);
                targetGroup.TabStrip.AddTab(documentId, title, panel.IconPath, activate: false);
                SetActiveDocument(documentId);
            };

            if (useCoordinator)
            {
                var coordinator = new DocumentHostTreeMutationCoordinator(this);
                coordinator.Execute(DocumentHostOperationNames.DockBackDocument, mutation);
            }
            else
            {
                mutation();
            }

            DocumentDocked?.Invoke(this, new DocumentEventArgs(documentId, title));
            OnDocumentDockStateChanged(documentId, title, previousState, DocumentDockState.Docked);
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Dock overlay helpers  (private)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Called by a float window's WindowMoved event.
        /// Shows (or updates) the dock overlay when the window is dragged over this host.
        /// </summary>
        private void OnFloatWindowMoved(string documentId, System.Drawing.Point floatWindowScreenPt)
        {
            Rectangle hostScreen = RectangleToScreen(ClientRectangle);

            if (hostScreen.Contains(floatWindowScreenPt))
            {
                if (_dockOverlay == null)
                    _dockOverlay = new BeepDocumentDockOverlay();

                if (_currentTheme != null)
                    _dockOverlay.ApplyTheme(_currentTheme);

                if (!_dockOverlay.Visible)
                    _dockOverlay.ShowOverlay(hostScreen);
                else
                    _dockOverlay.Bounds = hostScreen;

                _dockOverlay.UpdateHighlight(floatWindowScreenPt);
            }
            else
            {
                _dockOverlay?.HideOverlay();
            }

            // Broadcast position to all other registered hosts (Feature 4.5)
            if (_allowDragBetweenHosts)
                BeepDocumentDragManager.BroadcastFloatWindowMoved(
                    floatWindowScreenPt, this, documentId);
        }

        /// <summary>
        /// Called when the user releases the left mouse button on the float window's title bar.
        /// If a dock zone is highlighted, docks the document back.
        /// </summary>
        private void OnFloatWindowDropped(string documentId)
        {
            var cursor = System.Windows.Forms.Cursor.Position;

            // ── Cross-host drop (Feature 4.5) ─────────────────────────────────
            if (_allowDragBetweenHosts)
            {
                bool handled = BeepDocumentDragManager.BroadcastFloatWindowDropped(
                    cursor, this, documentId);
                if (handled)
                {
                    _dockOverlay?.HideOverlay();
                    return;
                }
            }

            // ── Local drop ────────────────────────────────────────────────────
            if (_dockOverlay == null || !_dockOverlay.Visible) return;

            DockZone zone = _dockOverlay.HitTest(cursor);
            _dockOverlay.HideOverlay();

            // Enforce dock constraints (Feature 4.7)
            if (!_dockConstraints.IsZoneAllowed(zone)) return;

            var coordinator = new DocumentHostTreeMutationCoordinator(this);
            switch (zone)
            {
                case DockZone.Centre:
                    coordinator.Execute(DocumentHostOperationNames.DropDockCentre, () =>
                    {
                        DockBackDocumentInternal(documentId, useCoordinator: false);
                    });
                    break;

                case DockZone.Right:
                    coordinator.Execute(DocumentHostOperationNames.DropDockRight, () =>
                    {
                        if (!DockBackDocumentInternal(documentId, useCoordinator: false)) return;
                        SplitInternal(documentId, horizontal: true, useCoordinator: false);
                    });
                    break;

                case DockZone.Bottom:
                    coordinator.Execute(DocumentHostOperationNames.DropDockBottom, () =>
                    {
                        if (!DockBackDocumentInternal(documentId, useCoordinator: false)) return;
                        SplitInternal(documentId, horizontal: false, useCoordinator: false);
                    });
                    break;

                case DockZone.Left:
                    coordinator.Execute(DocumentHostOperationNames.DropDockLeft, () =>
                    {
                        if (!DockBackDocumentInternal(documentId, useCoordinator: false)) return;
                        var leftOther = _primaryGroup.DocumentIds
                            .Find(id => id != documentId);
                        if (leftOther != null)
                            SplitInternal(leftOther, horizontal: true, useCoordinator: false);
                    });
                    break;

                case DockZone.Top:
                    coordinator.Execute(DocumentHostOperationNames.DropDockTop, () =>
                    {
                        if (!DockBackDocumentInternal(documentId, useCoordinator: false)) return;
                        var topOther = _primaryGroup.DocumentIds
                            .Find(id => id != documentId);
                        if (topOther != null)
                            SplitInternal(topOther, horizontal: false, useCoordinator: false);
                    });
                    break;

                // DockZone.None → user released outside any zone; do nothing
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // External drag overlay callbacks (Feature 4.5)
        // Called by BeepDocumentDragManager on non-source hosts during a drag.
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Shows or hides this host's dock overlay while a foreign float window is moving.</summary>
        internal void HandleExternalDragPosition(
            Point screenPt, BeepDocumentHost source, string documentId)
        {
            if (!_allowDragBetweenHosts) return;
            if (!_dockConstraints.AllowIncomingTransfer) return;

            Rectangle hostScreen = RectangleToScreen(ClientRectangle);
            if (hostScreen.Contains(screenPt))
            {
                if (_dockOverlay == null)
                    _dockOverlay = new BeepDocumentDockOverlay();

                if (_currentTheme != null)
                    _dockOverlay.ApplyTheme(_currentTheme);

                if (!_dockOverlay.Visible)
                    _dockOverlay.ShowOverlay(hostScreen);
                else
                    _dockOverlay.Bounds = hostScreen;

                _dockOverlay.UpdateHighlight(screenPt);
            }
            else
            {
                _dockOverlay?.HideOverlay();
            }
        }

        /// <summary>Hides the dock overlay without acting on any zone selection.</summary>
        internal void HideExternalDragOverlay()
            => _dockOverlay?.HideOverlay();

        /// <summary>
        /// Accepts a document dropped from another host at the currently highlighted zone.
        /// Returns true if the drop was accepted.
        /// </summary>
        internal bool AcceptExternalDrop(
            BeepDocumentHost source, string documentId, Point screenPt)
        {
            if (!_allowDragBetweenHosts) return false;
            if (!_dockConstraints.AllowIncomingTransfer) return false;
            if (_dockOverlay == null || !_dockOverlay.Visible) return false;

            DockZone zone = _dockOverlay.HitTest(screenPt);
            _dockOverlay.HideOverlay();

            if (zone == DockZone.None) return false;
            if (!_dockConstraints.IsZoneAllowed(zone)) return false;

            // Raise transfer event on source so consumer can cancel
            var args = new DocumentTransferEventArgs(documentId, this);
            source.DocumentDetaching?.Invoke(source, args);
            if (args.Cancel) return false;

            // Transfer document from source to this host
            AttachExternalDocument(source, documentId);

            var coordinator = new DocumentHostTreeMutationCoordinator(this);
            // Apply dock zone split after transfer
            switch (zone)
            {
                case DockZone.Right:
                    coordinator.Execute(DocumentHostOperationNames.ExternalDropRight, () =>
                    {
                        SplitInternal(documentId, horizontal: true, useCoordinator: false);
                    });
                    break;
                case DockZone.Bottom:
                    coordinator.Execute(DocumentHostOperationNames.ExternalDropBottom, () =>
                    {
                        SplitInternal(documentId, horizontal: false, useCoordinator: false);
                    });
                    break;
                case DockZone.Left:
                    coordinator.Execute(DocumentHostOperationNames.ExternalDropLeft, () =>
                    {
                        var leftOther = _primaryGroup.DocumentIds.Find(id => id != documentId);
                        if (leftOther != null)
                            SplitInternal(leftOther, horizontal: true, useCoordinator: false);
                    });
                    break;
                case DockZone.Top:
                    coordinator.Execute(DocumentHostOperationNames.ExternalDropTop, () =>
                    {
                        var topOther = _primaryGroup.DocumentIds.Find(id => id != documentId);
                        if (topOther != null)
                            SplitInternal(topOther, horizontal: false, useCoordinator: false);
                    });
                    break;
                // DockZone.Centre → just tab merge (already done by AttachExternalDocument)
            }

            return true;
        }

        /// <summary>Returns the panel for a given document id, or null.</summary>
        public BeepDocumentPanel? GetPanel(string documentId)
            => _panels.TryGetValue(documentId, out var p) ? p : null;

        internal bool IsDocumentFloating(string documentId)
            => _floatWindows.ContainsKey(documentId);

        // ─────────────────────────────────────────────────────────────────────
        // Reopen Closed Tab
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Reopens the most recently closed document.
        /// Raises <see cref="ReopenDocumentRequested"/> so the consumer can rebuild
        /// the panel and restore state from <see cref="ClosedTabRecord.RestoreData"/>.
        /// </summary>
        /// <returns>False if there is nothing to reopen.</returns>
        public bool ReopenLastClosed()
        {
            if (_closedStack.Count == 0) return false;
            var record = _closedStack.Pop();
            ReopenDocumentRequested?.Invoke(this, record);
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // MRU + Closed-stack helpers  (private)
        // ─────────────────────────────────────────────────────────────────────

        private void PushMru(string documentId)
        {
            _mruList.Remove(documentId);
            _mruList.AddFirst(documentId);
            while (_mruList.Count > _maxRecentHistory)
                _mruList.RemoveLast();
        }

        private void RemoveMru(string documentId) => _mruList.Remove(documentId);

        private void PushClosedTab(ClosedTabRecord record)
        {
            if (_maxClosedHistory <= 0) return;
            _closedStack.Push(record);

            // Trim to max: Stack is LIFO — rebuild from array to discard oldest entries
            if (_closedStack.Count > _maxClosedHistory)
            {
                var items = _closedStack.ToArray();
                _closedStack.Clear();
                for (int i = Math.Min(_maxClosedHistory, items.Length) - 1; i >= 0; i--)
                    _closedStack.Push(items[i]);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Quick-Switch Popup
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Opens the Quick-Switch popup with documents sorted in MRU order and the
        /// second-most-recent entry pre-selected (VS Code / VS 2022 Ctrl+Tab behaviour).
        /// While the popup is open, Tab / Ctrl+Tab advances the selection;
        /// Shift+Tab / Ctrl+Shift+Tab reverses it.  Enter commits; Escape cancels.
        /// </summary>
        /// <param name="reverse">
        /// When <c>true</c>, the last item in the MRU list is pre-selected (Ctrl+Shift+Tab).
        /// </param>
        internal void ShowQuickSwitchMru(bool reverse)
        {
            var allTabs = GetAllTabs();
            if (allTabs.Count <= 1) return;

            // Build MRU-sorted list: _mruList order first, then any tab not yet in the list.
            var tabById = allTabs.ToDictionary(t => t.Id, t => t);
            var sorted  = _mruList
                .Where(id  => tabById.ContainsKey(id))
                .Select(id => tabById[id])
                .ToList();

            // Append any open tabs that have never been explicitly activated (new windows).
            foreach (var t in allTabs)
                if (!sorted.Contains(t)) sorted.Add(t);

            // Pre-select: index 1 for forward (skip the current active at index 0),
            // last item for reverse.
            int startIndex = reverse
                ? sorted.Count - 1
                : Math.Min(1, sorted.Count - 1);

            var stripPt = _tabStrip.PointToScreen(Point.Empty);
            int popW    = 420, popH = 340;
            int x       = stripPt.X + (_tabStrip.Width  - popW) / 2;
            int y       = stripPt.Y;
            var screen  = Screen.FromControl(this).WorkingArea;
            x = Math.Max(screen.Left, Math.Min(x, screen.Right  - popW));
            y = Math.Max(screen.Top,  Math.Min(y, screen.Bottom - popH));

            using var popup = new BeepDocumentQuickSwitch(
                sorted,
                activeDocumentId: null,
                _currentTheme,
                new Point(x, y),
                initialIndex: startIndex);

            popup.ShowDialog(this);

            if (!string.IsNullOrEmpty(popup.SelectedDocumentId))
                SetActiveDocument(popup.SelectedDocumentId);
        }

        /// <summary>
        /// Opens the Quick-Switch document picker above the tab strip.
        /// The user can type a filter, navigate with arrow keys, and press Enter
        /// to switch to the selected document.
        /// </summary>
        public void ShowQuickSwitch()
        {
            // Collect tabs from every split group so multi-group layouts are covered.
            var tabs = GetAllTabs().ToList();
            if (tabs.Count == 0) return;

            // Position the popup centred over the tab strip
            var stripScreenPt = _tabStrip.PointToScreen(Point.Empty);
            int popW = 420;
            int popH = 340;
            int x    = stripScreenPt.X + (_tabStrip.Width  - popW) / 2;
            int y    = stripScreenPt.Y;

            // Keep on screen
            var screen = Screen.FromControl(this).WorkingArea;
            x = Math.Max(screen.Left, Math.Min(x, screen.Right  - popW));
            y = Math.Max(screen.Top,  Math.Min(y, screen.Bottom - popH));

            var popup = new BeepDocumentQuickSwitch(
                tabs,
                _activeDocumentId,
                _currentTheme,
                new System.Drawing.Point(x, y));

            popup.ShowDialog(this);

            if (!string.IsNullOrEmpty(popup.SelectedDocumentId))
                SetActiveDocument(popup.SelectedDocumentId);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Pin / Unpin
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Pins or unpins the document.  A pinned document collapses to an icon-only tab
        /// and cannot be closed by the user until unpinned.
        /// </summary>
        /// <param name="documentId">Id of the document to pin or unpin.</param>
        /// <param name="pin">True to pin, false to unpin.</param>
        /// <returns>False if the document was not found.</returns>
        public bool PinDocument(string documentId, bool pin = true)
        {
            if (!_panels.TryGetValue(documentId, out var panel)) return false;

            if (!TryGetDocumentTab(documentId, out var tabStrip, out var tab)) return false;

            if (tab.IsPinned == pin) return true;   // already in requested state

            // Find current index and tell the strip to toggle
            int index = tabStrip.Tabs.ToList().IndexOf(tab);
            if (index >= 0)
                tabStrip.TogglePin(index);

            // Sync CanClose on the panel
            panel.CanClose = !pin;
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Badge API (7.4)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sets or clears the notification badge on the tab for the given document.
        /// </summary>
        /// <param name="documentId">Target document id.</param>
        /// <param name="badgeText">Short label (e.g. "3", "!"). Null/empty clears the badge.</param>
        /// <param name="badgeColor">Badge colour. <see cref="System.Drawing.Color.Empty"/> = use theme ErrorColor.</param>
        public void SetBadge(string documentId, string? badgeText,
                              System.Drawing.Color badgeColor = default)
        {
            if (TryGetDocumentTab(documentId, out var tabStrip, out _))
                tabStrip.SetBadge(documentId, badgeText, badgeColor);
        }

        /// <summary>Removes the notification badge from the given document's tab.</summary>
        public void ClearBadge(string documentId)
        {
            if (TryGetDocumentTab(documentId, out var tabStrip, out _))
                tabStrip.ClearBadge(documentId);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Cross-host document transfer
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Transfers a document from <paramref name="sourceHost"/> into this host.
        /// Called by <see cref="BeepDocumentDragManager"/> routing in <c>Events.cs</c>.
        /// </summary>
        internal void AttachExternalDocument(BeepDocumentHost sourceHost, string documentId)
        {
            if (!sourceHost._panels.TryGetValue(documentId, out var panel)) return;

            string title    = panel.DocumentTitle;
            string? iconPath = panel.IconPath;
            var sourceGroup = sourceHost.GetGroupForDocument(documentId);

            var sourceCoordinator = new DocumentHostTreeMutationCoordinator(sourceHost);
            sourceCoordinator.Execute(DocumentHostOperationNames.DetachExternalDocument, () =>
            {
                int tabIdx = sourceGroup.TabStrip.Tabs
                    .ToList()
                    .FindIndex(t => t.Id == documentId);
                if (tabIdx >= 0) sourceGroup.TabStrip.RemoveTabAt(tabIdx);
                sourceGroup.ContentArea.Controls.Remove(panel);
                sourceHost._panels.Remove(documentId);
                if (sourceHost._activeDocumentId == documentId)
                {
                    sourceHost._activeDocumentId = null;
                    if (sourceHost._panels.Count > 0)
                        sourceHost.SetActiveDocument(sourceHost._panels.Keys.Last());
                }
            });

            var targetCoordinator = new DocumentHostTreeMutationCoordinator(this);
            targetCoordinator.Execute(DocumentHostOperationNames.AttachExternalDocument, () =>
            {
                panel.Parent = _contentArea;
                _panels[documentId] = panel;
                _docGroupMap[documentId] = _primaryGroup.GroupId;
                _primaryGroup.DocumentIds.Add(documentId);
                _tabStrip.AddTab(documentId, title, iconPath, activate: false);
                SetActiveDocument(documentId);
            });

            DocumentAttached?.Invoke(this, new DocumentEventArgs(documentId, title));
        }

        // ─────────────────────────────────────────────────────────────────────
        // Document Groups — Split View (Feature 2.1)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Moves <paramref name="documentId"/> into a new side-by-side pane
        /// (horizontal split — one pane to the left, one to the right).
        /// </summary>
        public bool SplitDocumentHorizontal(string documentId)
            => SplitInternal(documentId, horizontal: true, useCoordinator: true);

        /// <summary>
        /// Moves <paramref name="documentId"/> into a new stacked pane
        /// (vertical split — one pane above, one below).
        /// </summary>
        public bool SplitDocumentVertical(string documentId)
            => SplitInternal(documentId, horizontal: false, useCoordinator: true);

        private bool SplitInternal(string documentId, bool horizontal, bool useCoordinator)
        {
            if (!_panels.ContainsKey(documentId)) return false;
            if (_groups.Count >= _maxGroups) return false;

            PushUndoState();
            if (_trackLayoutHistory) PushLayoutVersion($"Before split ({(horizontal ? "H" : "V")})");
            Action mutation = () =>
            {
                var grp = new BeepDocumentGroup(
                    Guid.NewGuid().ToString(),
                    ThemeName,
                    _showAddButton,
                    _closeMode,
                    _tabStyle,
                    _currentTheme)
                {
                    TabPosition = _tabPosition
                };

                Controls.Add(grp.ContentArea);
                Controls.Add(grp.TabStrip);

                WireSecondaryGroup(grp);

                _groups.Add(grp);
                _groupById[grp.GroupId] = grp;
                _splitHorizontal = horizontal;

                MoveDocumentToGroupCore(documentId, grp.GroupId, recalcLayout: false);
                SetActiveDocument(documentId);
            };

            if (useCoordinator)
            {
                var coordinator = new DocumentHostTreeMutationCoordinator(this);
                return coordinator.Execute(DocumentHostOperationNames.SplitDocument, mutation);
            }

            mutation();
            return true;
        }

        /// <summary>
        /// Moves a document from its current group to the group identified by
        /// <paramref name="targetGroupId"/>.  Returns false if either id is unknown.
        /// </summary>
        public bool MoveDocumentToGroup(string documentId, string targetGroupId)
        {
            var coordinator = new DocumentHostTreeMutationCoordinator(this);
            bool moved = false;
            coordinator.Execute(DocumentHostOperationNames.MoveDocumentToGroup, () =>
            {
                moved = MoveDocumentToGroupCore(documentId, targetGroupId, recalcLayout: false);
            });
            return moved;
        }

        private bool MoveDocumentToGroupCore(string documentId, string targetGroupId, bool recalcLayout)
        {
            if (!_panels.TryGetValue(documentId, out var panel)) return false;
            var targetGroup = _groups.Find(g => g.GroupId == targetGroupId);
            if (targetGroup == null) return false;

            // Remove from current group
            if (_docGroupMap.TryGetValue(documentId, out var currentGroupId))
            {
                var currentGroup = _groups.Find(g => g.GroupId == currentGroupId);
                if (currentGroup != null && currentGroup != targetGroup)
                {
                    // Record the previous group for intelligent re-dock hints (P4-002)
                    var desc = _documents?.FirstOrDefault(d => d.Id == documentId);
                    if (desc != null) desc.PreviousGroupId = currentGroupId;

                    currentGroup.DocumentIds.Remove(documentId);
                    int idx = currentGroup.TabStrip.Tabs.ToList()
                                          .FindIndex(t => t.Id == documentId);
                    if (idx >= 0) currentGroup.TabStrip.RemoveTabAt(idx);

                    if (currentGroup.IsEmpty && !currentGroup.IsPrimary)
                        CollapseEmptyGroup(currentGroup);
                }
            }

            // Reparent the panel to the target group's content area
            panel.Parent?.Controls.Remove(panel);
            targetGroup.ContentArea.Controls.Add(panel);

            // Add to the target group's tab strip
            targetGroup.TabStrip.AddTab(
                documentId, panel.DocumentTitle, panel.IconPath, activate: false);
            targetGroup.DocumentIds.Add(documentId);
            _docGroupMap[documentId] = targetGroupId;

            panel.ModifiedChanged += (s, _) =>
                targetGroup.TabStrip.SetTabModified(documentId, panel.IsModified);

            if (recalcLayout)
                RecalculateLayout();
            return true;
        }

        /// <summary>Wires a secondary group's forwarded events to this host's handlers.</summary>
        private void WireSecondaryGroup(BeepDocumentGroup grp)
        {
            grp.TabSelected       += (s, e) => OnTabSelected(grp.TabStrip, e);
            grp.TabCloseRequested += (s, e) => OnTabCloseRequested(grp.TabStrip, e);
            grp.TabClosing        += (s, e) => OnTabClosing(grp.TabStrip, e);
            grp.TabFloatRequested += (s, e) => OnTabFloatRequested(grp.TabStrip, e);
            grp.TabPinToggled     += (s, e) => OnTabPinToggled(grp.TabStrip, e);
            grp.TabReordered      += (s, e) => OnTabReordered(grp.TabStrip, e);
            grp.AddButtonClicked  += (s, e) => OnAddButtonClicked(grp.TabStrip, e);
            grp.GroupSplitHorizontalRequested += (s, e) =>
            {
                var docId = grp.DocumentIds.FirstOrDefault() ?? _activeDocumentId;
                if (!string.IsNullOrEmpty(docId))
                    SplitDocumentHorizontal(docId);
            };
            grp.GroupSplitVerticalRequested += (s, e) =>
            {
                var docId = grp.DocumentIds.FirstOrDefault() ?? _activeDocumentId;
                if (!string.IsNullOrEmpty(docId))
                    SplitDocumentVertical(docId);
            };
            grp.GroupCloseAllRequested += (s, e) => CloseGroupDocuments(grp.GroupId);
        }

        /// <summary>
        /// Removes a now-empty secondary group, unregisters its controls from
        /// this host, disposes it, and triggers a layout recalculation.
        /// Uses animated collapse when a splitter is visible; immediate otherwise.
        /// </summary>
        private void CollapseEmptyGroup(BeepDocumentGroup grp)
        {
            CollapseEmptyGroupAnimated(grp);
        }
    }
}
