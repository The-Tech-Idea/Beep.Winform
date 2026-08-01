using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    internal sealed class BeepTabOverflowState
    {
        public static BeepTabOverflowState Empty { get; } = new BeepTabOverflowState();

        public BeepTabOverflowPolicy Policy { get; init; } = BeepTabOverflowPolicy.None;
        public int DesiredRunExtent { get; init; }
        public int AvailableRunExtent { get; init; }
        public int ReservedActionExtent { get; init; }
        public int VisibleItemCount { get; init; }
        public int OverflowItemCount { get; init; }
        public IReadOnlyList<int> VisibleIndices { get; init; } = Array.Empty<int>();
        public IReadOnlyList<int> OverflowIndices { get; init; } = Array.Empty<int>();

        public bool HasOverflow => OverflowItemCount > 0;
    }

    internal static class BeepTabOverflowCoordinator
    {
        public static BeepTabOverflowState Calculate(
            BeepTabs owner,
            Graphics graphics,
            IReadOnlyList<BeepTabHeaderAction> plannedActions,
            BeepTabOverflowPolicy policy)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            if (plannedActions == null)
            {
                throw new ArgumentNullException(nameof(plannedActions));
            }

            int itemCount = owner.GetHostedSourceItemCount();

            if (itemCount == 0 || policy == BeepTabOverflowPolicy.None)
            {
                int[] allIndices = Enumerable.Range(0, itemCount).ToArray();
                return new BeepTabOverflowState
                {
                    Policy = policy,
                    VisibleItemCount = itemCount,
                    VisibleIndices = allIndices,
                    OverflowIndices = Array.Empty<int>(),
                    AvailableRunExtent = GetAvailableRunExtent(owner, plannedActions),
                    DesiredRunExtent = (int)Math.Ceiling(owner.GetDesiredHeaderRunExtent(graphics)),
                    ReservedActionExtent = owner.GetHeaderActionReservedExtent(BeepTabLayoutHelper.GetHeaderBounds(owner), plannedActions)
                };
            }

            Rectangle headerBounds = BeepTabLayoutHelper.GetHeaderBounds(owner);
            int reservedActionExtent = owner.GetHeaderActionReservedExtent(headerBounds, plannedActions);
            int availableRunExtent = GetAvailableRunExtent(owner, plannedActions);
            float[] desiredSizes = owner.GetDesiredHeaderTabSizes(graphics);

            // Claim space in priority order — pinned first, then the selected tab, then the rest in
            // positional order — but render in positional order.
            //
            // This used to be a single left-to-right loop that stopped at the first tab that did not
            // fit. With ten tabs in a narrow header and the tenth selected, the selected tab was
            // pushed into overflow: the user clicked a tab and it vanished from the strip. Pinning a
            // tab did nothing to protect it either, which is most of what pinning is for. Every
            // reference product (VS, VS Code, Chrome, DevExpress, Telerik) guarantees both.
            int selectedIndex = owner.GetHostedSourceSelectedIndex();

            List<int> priority = new List<int>(desiredSizes.Length);
            for (int index = 0; index < desiredSizes.Length; index++)
                if (IsPinned(owner, index)) priority.Add(index);

            if (selectedIndex >= 0 && selectedIndex < desiredSizes.Length && !priority.Contains(selectedIndex))
                priority.Add(selectedIndex);

            for (int index = 0; index < desiredSizes.Length; index++)
                if (!priority.Contains(index)) priority.Add(index);

            HashSet<int> claimed = new HashSet<int>();
            float usedExtent = 0f;
            foreach (int index in priority)
            {
                float desiredSize = desiredSizes[index];
                if (usedExtent + desiredSize > availableRunExtent)
                {
                    // Keep scanning: a narrower tab later in priority order may still fit, and
                    // dropping out here is what made a wide tab hide everything after it.
                    continue;
                }

                usedExtent += desiredSize;
                claimed.Add(index);
            }

            List<int> visibleIndices = new List<int>(claimed.Count);
            List<int> overflowIndices = new List<int>(Math.Max(0, itemCount - claimed.Count));
            for (int index = 0; index < itemCount; index++)
            {
                if (claimed.Contains(index)) visibleIndices.Add(index);
                else overflowIndices.Add(index);
            }

            int visibleItemCount = visibleIndices.Count;
            int overflowItemCount = overflowIndices.Count;
            return new BeepTabOverflowState
            {
                Policy = policy,
                DesiredRunExtent = (int)Math.Ceiling(owner.GetDesiredHeaderRunExtent(graphics)),
                AvailableRunExtent = availableRunExtent,
                ReservedActionExtent = reservedActionExtent,
                VisibleItemCount = visibleItemCount,
                OverflowItemCount = overflowItemCount,
                VisibleIndices = visibleIndices,
                OverflowIndices = overflowIndices
            };
        }

        /// <summary>
        /// A pinned tab is exempt from overflow. Read from the page, which owns the document state.
        /// </summary>
        private static bool IsPinned(BeepTabs owner, int index)
        {
            return owner.GetHostedSourcePageAt(index)?.TabIsPinned == true;
        }

        private static int GetAvailableRunExtent(BeepTabs owner, IReadOnlyList<BeepTabHeaderAction> plannedActions)
        {
            Rectangle headerBounds = BeepTabLayoutHelper.GetHeaderBounds(owner);
            int reservedActionExtent = owner.GetHeaderActionReservedExtent(headerBounds, plannedActions);
            bool vertical = owner.HeaderPosition == TabHeaderPosition.Left || owner.HeaderPosition == TabHeaderPosition.Right;
            int totalExtent = vertical ? headerBounds.Height : headerBounds.Width;
            return Math.Max(0, totalExtent - reservedActionExtent);
        }
    }
}