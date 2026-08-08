using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTabs
    {
        // ── Error reporting ────────────────────────────────────────────────────

        /// <summary>Last error message to render on the control surface. Cleared on next successful operation.</summary>
        private string? _lastError;

        /// <summary>
        /// Raised when a tab operation fails, in every build configuration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a diagnostic channel, not error handling: the operation that failed still
        /// throws, so a host that does not subscribe still learns about the failure. Subscribe to
        /// route failures to a log or telemetry sink in a shipped application, where the
        /// <c>BeepLog</c> report below does not exist.
        /// </para>
        /// <para>
        /// Handlers must not throw. This event is raised from inside a <c>catch</c> block that is
        /// about to rethrow, so an exception from a handler would replace the original failure and
        /// destroy the diagnostic it was subscribed to receive.
        /// </para>
        /// </remarks>
        [Category("Behavior")]
        [Description("Raised when a tab operation fails. The operation still throws; this reports it.")]
        public event EventHandler<BeepTabErrorEventArgs>? TabError;

        /// <summary>
        /// Records an error, publishes it on <see cref="TabError"/>, writes it to the debug output,
        /// and repaints the control so the message is visible on its surface.
        /// </summary>
        /// <remarks>
        /// This reports a failure; it does not handle one. Every caller rethrows after calling it —
        /// reporting and continuing was how a failed <c>AddPage</c> used to return normally, which
        /// told the caller the page had been added when it had not.
        /// </remarks>
        private void ReportError(string context, Exception? ex)
        {
            string message = ex == null
                ? context
                : $"{context}\n{ex.GetType().Name}: {ex.Message}";

            _lastError = message;

            if (ex != null)
                Diagnostics.BeepLog.Failure(this, context, ex);
            else
                Diagnostics.BeepLog.Error(this, context);

            TabError?.Invoke(this, new BeepTabErrorEventArgs(context, ex));

            Invalidate();
        }

        /// <summary>Clears the displayed error message (call after a successful operation).</summary>
        private void ClearError()
        {
            if (_lastError != null)
            {
                _lastError = null;
                Invalidate();
            }
        }

        private bool _showHeaderCloseCurrentAction;
        private BeepTabOverflowPolicy _headerOverflowPolicy = BeepTabOverflowPolicy.OverflowMenu;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Controls how the header responds when the visible tab run exceeds available header space.")]
        [DefaultValue(BeepTabOverflowPolicy.OverflowMenu)]
        public BeepTabOverflowPolicy HeaderOverflowPolicy
        {
            get => _headerOverflowPolicy;
            set
            {
                if (_headerOverflowPolicy == value)
                {
                    return;
                }

                _headerOverflowPolicy = value;
                CloseChildPopup();
                RefreshHeaderLayoutState(updateItemSize: false);
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Shows a header action slot that closes the currently selected tab.")]
        [DefaultValue(false)]
        public bool ShowHeaderCloseCurrentAction
        {
            get => _showHeaderCloseCurrentAction;
            set
            {
                if (_showHeaderCloseCurrentAction == value)
                {
                    return;
                }

                _showHeaderCloseCurrentAction = value;
                RefreshHeaderLayoutState(updateItemSize: false);
            }
        }

        private BeepTabHeaderAction[] CreateHeaderActionSlots(bool includeOverflowAction)
        {
            List<BeepTabHeaderAction> actions = new List<BeepTabHeaderAction>();
            int selectedIndex = GetHostedSourceSelectedIndex();

            if (_showHeaderCloseCurrentAction && ShowCloseButtons && selectedIndex >= 0)
            {
                actions.Add(
                    BeepTabHeaderAction.CreateActionSlot(
                        BeepTabHeaderActionKind.CloseCurrent,
                        "tab.closeCurrent",
                        "X",
                        order: 100));
            }

            if (includeOverflowAction)
            {
                actions.Add(
                    BeepTabHeaderAction.CreateActionSlot(
                        BeepTabHeaderActionKind.Overflow,
                        "tab.overflow",
                        "...",
                        order: 200));
            }

            return actions.ToArray();
        }

        internal BeepTabHeaderAction[] GetHeaderActionSlots()
        {
            if (!IsHandleCreated)
            {
                return Array.Empty<BeepTabHeaderAction>();
            }

            using Graphics graphics = CreateGraphics();
            return GetHeaderActionSlots(graphics);
        }

        internal BeepTabHeaderAction[] GetHeaderActionSlots(Graphics graphics)
        {
            BeepTabHeaderAction[] baseActions = CreateHeaderActionSlots(includeOverflowAction: false);
            if (HeaderOverflowPolicy == BeepTabOverflowPolicy.OverflowMenu)
            {
                BeepTabOverflowState overflowState = GetHeaderOverflowState(graphics, baseActions);
                if (overflowState.HasOverflow)
                {
                    return CreateHeaderActionSlots(includeOverflowAction: true);
                }
            }

            return baseActions;
        }

        internal int GetHeaderActionSlotGap()
        {
            return 4;
        }

        internal int GetHeaderActionSlotSize(Rectangle headerBounds)
        {
            int referenceSize = HeaderPosition == TabHeaderPosition.Top || HeaderPosition == TabHeaderPosition.Bottom
                ? headerBounds.Height
                : headerBounds.Width;

            return Math.Max(14, referenceSize - GetHeaderActionSlotGap() * 2);
        }

        internal int GetHeaderActionReservedExtent(Rectangle headerBounds)
        {
            BeepTabHeaderAction[] actions = GetHeaderActionSlots();
            return GetHeaderActionReservedExtent(headerBounds, actions);
        }

        internal int GetHeaderActionReservedExtent(Rectangle headerBounds, IReadOnlyList<BeepTabHeaderAction> actions)
        {
            int visibleActionCount = 0;

            foreach (BeepTabHeaderAction action in actions)
            {
                if (action.IsActionSlot && action.IsVisible)
                {
                    visibleActionCount++;
                }
            }

            if (visibleActionCount == 0 || headerBounds.IsEmpty)
            {
                return 0;
            }

            int slotGap = GetHeaderActionSlotGap();
            int slotSize = GetHeaderActionSlotSize(headerBounds);
            return (visibleActionCount * slotSize) + ((visibleActionCount + 1) * slotGap);
        }

        internal BeepTabOverflowState GetHeaderOverflowState()
        {
            if (!IsHandleCreated)
            {
                return BeepTabOverflowState.Empty;
            }

            using Graphics graphics = CreateGraphics();
            return GetHeaderOverflowState(graphics, null);
        }

        internal BeepTabOverflowState GetHeaderOverflowState(Graphics graphics, IReadOnlyList<BeepTabHeaderAction>? plannedActions)
        {
            IReadOnlyList<BeepTabHeaderAction> actions = plannedActions ?? GetHeaderActionSlots(graphics);
            return BeepTabOverflowCoordinator.Calculate(this, graphics, actions, HeaderOverflowPolicy);
        }

        internal BeepTabItem[] GetOverflowedItems()
        {
            BeepTabOverflowState overflowState = GetHeaderOverflowState();
            if (!overflowState.HasOverflow)
            {
                return Array.Empty<BeepTabItem>();
            }

            IReadOnlyList<BeepTabItem> items = GetHostedSourceItemsSnapshot();

            // Overflowed tabs, most-recently-used first. Positional order is the wrong ordering for
            // this menu: the tabs in it are precisely the ones that did not fit, so the one the user
            // wants is far more likely to be the one they were last in than the one that happens to
            // sit leftmost. Every reference product orders this list by recency.
            // BeepTabWorkspaceMruTracker already tracked it and nothing here consulted it.
            List<BeepTabPage> overflowPages = new List<BeepTabPage>(overflowState.OverflowIndices.Count);
            foreach (int index in overflowState.OverflowIndices)
            {
                BeepTabPage? page = GetHostedSourcePageAt(index);
                if (page != null) overflowPages.Add(page);
            }

            IReadOnlyList<BeepTabPage> ordered = ModeCapabilities.SupportsMruOrdering
                ? _workspaceMruTracker.GetMruOrderedPages(overflowPages, GetHostedSourceSelectedPage())
                : overflowPages;

            List<BeepTabItem> overflowItems = new List<BeepTabItem>(ordered.Count);
            foreach (BeepTabPage page in ordered)
            {
                int index = _hostedPages.IndexOf(page);
                if (index >= 0 && index < items.Count) overflowItems.Add(items[index]);
            }

            return overflowItems.ToArray();
        }

        internal bool TryShowHeaderOverflow()
        {
            if (HeaderOverflowPolicy != BeepTabOverflowPolicy.OverflowMenu)
            {
                return false;
            }

            BeepTabOverflowState overflowState = GetHeaderOverflowState();
            if (!overflowState.HasOverflow)
            {
                return false;
            }

            if (IsPopupOpen)
            {
                CloseChildPopup();
            }
            else
            {
                OnPopupOpened();
            }

            return true;
        }

        internal bool TryCloseCurrentHeaderTab()
        {
            return TryCloseHeaderTab(GetHostedSourceSelectedIndex());
        }

        /// <summary>
        /// Shows the MRU quick-switch popup (Ctrl+P) for Documents / Workspace modes.
        /// Does nothing and returns <c>false</c> in Navigation mode or when fewer than
        /// two tabs are open.
        /// </summary>
        public bool ShowQuickSwitch()
        {
            return TryShowWorkspaceQuickSwitch();
        }

        /// <summary>
        /// Reopens the most recently closed tab (Ctrl+Shift+T).
        /// Only operates in Documents or Workspace mode.
        /// </summary>
        public bool ReopenLastClosedTab()
        {
            return TryReopenLastClosedTab();
        }
    }
}