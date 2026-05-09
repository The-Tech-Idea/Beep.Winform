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

            List<int> visibleIndices = new List<int>(desiredSizes.Length);
            int visibleItemCount = 0;
            float usedExtent = 0f;
            for (int index = 0; index < desiredSizes.Length; index++)
            {
                float desiredSize = desiredSizes[index];
                if (usedExtent + desiredSize > availableRunExtent)
                {
                    break;
                }

                usedExtent += desiredSize;
                visibleItemCount++;
                visibleIndices.Add(index);
            }

            List<int> overflowIndices = new List<int>(Math.Max(0, itemCount - visibleItemCount));
            for (int index = visibleItemCount; index < itemCount; index++)
            {
                overflowIndices.Add(index);
            }

            int overflowItemCount = Math.Max(0, itemCount - visibleItemCount);
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