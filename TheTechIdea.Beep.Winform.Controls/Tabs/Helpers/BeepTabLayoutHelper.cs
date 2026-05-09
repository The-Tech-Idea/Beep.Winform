using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    /// <summary>
    /// Canonical layout helper for the custom header host migration.
    /// </summary>
    public static class BeepTabLayoutHelper
    {
        public static BeepTabRenderContext CreateRenderContext(BeepTabs owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            return new BeepTabRenderContext
            {
                HeaderBounds = GetHeaderBounds(owner),
                ContentBounds = owner.DisplayRectangle,
                HeaderPosition = owner.HeaderPosition,
                TextFont = owner.TextFont,
                Theme = owner.CurrentTheme,
                ShowCloseButtons = owner.ShowCloseButtons,
                MinTouchTargetWidth = owner.MinTouchTargetWidth
            };
        }

        public static BeepTabHeaderLayoutSnapshot CreateSnapshot(
            BeepTabs owner,
            IEnumerable<BeepTabItem> items)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            BeepTabRenderContext renderContext = CreateRenderContext(owner);
            Font textFont = TabFontHelpers.ResolveSafeFont(renderContext.TextFont ?? owner.Font, owner);
            bool isHorizontal = renderContext.HeaderPosition == TabHeaderPosition.Top ||
                renderContext.HeaderPosition == TabHeaderPosition.Bottom;
            BeepTabHeaderLayoutSnapshot snapshot = new BeepTabHeaderLayoutSnapshot
            {
                HeaderBounds = renderContext.HeaderBounds,
                ContentBounds = renderContext.ContentBounds,
                HeaderPosition = renderContext.HeaderPosition,
                SelectedIndex = owner.GetHostedSourceSelectedIndex()
            };

            List<RectangleF> tabRects = owner.GetCurrentHeaderTabRects();

            // Pre-size the items list to avoid multiple backing-array reallocations.
            // tabRects.Count is the maximum number of visible tabs.
            if (tabRects.Count > 0)
                snapshot.Items.Capacity = tabRects.Count;

            // Avoid LINQ enumerator allocation — plain foreach + guard is allocation-free.
            foreach (BeepTabItem item in items)
            {
                if (!item.IsVisible) continue;

                Rectangle tabBounds = item.Index >= 0 && item.Index < tabRects.Count
                    ? Rectangle.Ceiling(tabRects[item.Index])
                    : Rectangle.Empty;
                item.Bounds = tabBounds;

                BeepTabHeaderItemLayout itemLayout = new BeepTabHeaderItemLayout
                {
                    Item = item,
                    Bounds = tabBounds,
                    HasCloseButton = ResolveCloseButtonVisibility(item, renderContext.ShowCloseButtons)
                };

                BeepTabAdornmentLayoutHelper.Calculate(itemLayout, textFont, itemLayout.HasCloseButton, isHorizontal);
                snapshot.Items.Add(itemLayout);
            }

            return snapshot;
        }

        private static bool ResolveCloseButtonVisibility(BeepTabItem item, bool showCloseButtons)
        {
            if (item == null || !item.CanClose)
            {
                return false;
            }

            if (item.CloseVisible.HasValue)
            {
                return item.CloseVisible.Value;
            }

            return showCloseButtons;
        }

        public static Rectangle GetHeaderBounds(BeepTabs owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            Rectangle contentBounds = owner.DisplayRectangle;
            return owner.HeaderPosition switch
            {
                TabHeaderPosition.Top => Rectangle.FromLTRB(0, 0, owner.ClientSize.Width, contentBounds.Top),
                TabHeaderPosition.Bottom => Rectangle.FromLTRB(0, contentBounds.Bottom, owner.ClientSize.Width, owner.ClientSize.Height),
                TabHeaderPosition.Left => Rectangle.FromLTRB(0, 0, contentBounds.Left, owner.ClientSize.Height),
                TabHeaderPosition.Right => Rectangle.FromLTRB(contentBounds.Right, 0, owner.ClientSize.Width, owner.ClientSize.Height),
                _ => Rectangle.Empty
            };
        }
    }
}