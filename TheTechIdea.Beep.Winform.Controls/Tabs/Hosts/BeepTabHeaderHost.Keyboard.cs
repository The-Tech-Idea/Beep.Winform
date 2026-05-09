using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Hosts
{
    public partial class BeepTabHeaderHost
    {
        public bool TryGetNextSelectableTabIndex(int currentIndex, bool forward, bool wrap, out int tabIndex)
        {
            tabIndex = -1;
            if (LayoutSnapshot == null || LayoutSnapshot.Items.Count == 0)
            {
                return false;
            }

            int currentPosition = FindItemPosition(currentIndex);
            if (currentPosition < 0)
            {
                return TryGetBoundarySelectableTabIndex(!forward, out tabIndex);
            }

            int direction = forward ? 1 : -1;
            for (int step = 1; step <= LayoutSnapshot.Items.Count; step++)
            {
                int candidatePosition = currentPosition + (direction * step);
                if (wrap)
                {
                    candidatePosition = (candidatePosition % LayoutSnapshot.Items.Count + LayoutSnapshot.Items.Count) % LayoutSnapshot.Items.Count;
                }
                else if (candidatePosition < 0 || candidatePosition >= LayoutSnapshot.Items.Count)
                {
                    break;
                }

                BeepTabHeaderItemLayout candidate = LayoutSnapshot.Items[candidatePosition];
                if (candidate.Item.CanSelect && candidate.Item.IsVisible)
                {
                    tabIndex = candidate.Item.Index;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetBoundarySelectableTabIndex(bool last, out int tabIndex)
        {
            tabIndex = -1;
            if (LayoutSnapshot == null || LayoutSnapshot.Items.Count == 0)
            {
                return false;
            }

            int start = last ? LayoutSnapshot.Items.Count - 1 : 0;
            int end = last ? -1 : LayoutSnapshot.Items.Count;
            int step = last ? -1 : 1;

            for (int position = start; position != end; position += step)
            {
                BeepTabHeaderItemLayout candidate = LayoutSnapshot.Items[position];
                if (candidate.Item.CanSelect && candidate.Item.IsVisible)
                {
                    tabIndex = candidate.Item.Index;
                    return true;
                }
            }

            return false;
        }

        public bool TryResolveArrowNavigationTarget(Keys keyData, int currentIndex, out int tabIndex)
        {
            tabIndex = -1;
            if (LayoutSnapshot == null || LayoutSnapshot.Items.Count == 0)
            {
                return false;
            }

            bool isHorizontal = LayoutSnapshot.HeaderPosition == TabHeaderPosition.Top || LayoutSnapshot.HeaderPosition == TabHeaderPosition.Bottom;
            Keys keyCode = keyData & Keys.KeyCode;
            if (isHorizontal)
            {
                return keyCode switch
                {
                    Keys.Left => TryGetNextSelectableTabIndex(currentIndex, false, false, out tabIndex),
                    Keys.Right => TryGetNextSelectableTabIndex(currentIndex, true, false, out tabIndex),
                    _ => false
                };
            }

            return keyCode switch
            {
                Keys.Up => TryGetNextSelectableTabIndex(currentIndex, false, false, out tabIndex),
                Keys.Down => TryGetNextSelectableTabIndex(currentIndex, true, false, out tabIndex),
                _ => false
            };
        }

        private int FindItemPosition(int tabIndex)
        {
            if (LayoutSnapshot == null)
            {
                return -1;
            }

            for (int index = 0; index < LayoutSnapshot.Items.Count; index++)
            {
                if (LayoutSnapshot.Items[index].Item.Index == tabIndex)
                {
                    return index;
                }
            }

            return -1;
        }
    }
}