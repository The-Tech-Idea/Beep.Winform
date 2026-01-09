using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.AccordionMenus.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus
{
    public partial class BeepAccordionMenu
    {
        private void InitializeAccordion()
        {
            // Set up the accordion properties
            BackColor = AccordionThemeHelpers.GetAccordionBackgroundColor(
                _currentTheme,
                UseThemeColors);

            // Subscribe to events
            items.ListChanged += Items_ListChanged;
        }

        private void Items_ListChanged(object sender, ListChangedEventArgs e)
        {
            InitializeMenuItemState();
            Invalidate();
        }

        private void InitializeMenu()
        {
            // Clear existing hit areas
            ClearHitList();

            // This method now just invalidates the control to redraw with painter
            Invalidate();
        }

        private void InitializeMenuItemState()
        {
            // Initialize expanded state for accordion headers
            foreach (var item in items)
            {
                // If it's not in the dictionary, add it (default to collapsed)
                if (!expandedState.ContainsKey(item))
                {
                    expandedState[item] = false;
                }

                // Also initialize any child items that have their own children
                foreach (var childItem in item.Children)
                {
                    if (childItem.Children.Count > 0 && !expandedState.ContainsKey((SimpleItem)childItem))
                    {
                        expandedState[(SimpleItem)childItem] = false;
                    }
                }
            }
        }

        private void StartAccordionAnimation()
        {
            if (!isInitialized) return;

            isAnimating = true;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (!isAnimating) return;

            int targetWidth = isCollapsed ? CollapsedWidth : ExpandedWidth;
            int currentWidth = Width;

            if (isCollapsed)
            {
                currentWidth -= AnimationStep;
                if (currentWidth <= targetWidth)
                {
                    currentWidth = targetWidth;
                    animationTimer.Stop();
                    isAnimating = false;
                }
            }
            else
            {
                currentWidth += AnimationStep;
                if (currentWidth >= targetWidth)
                {
                    currentWidth = targetWidth;
                    animationTimer.Stop();
                    isAnimating = false;
                }
            }

            Width = currentWidth;
            Invalidate();
        }

        private void UpdateItemsPanelSize()
        {
            if (itemsPanel == null || toggleButton == null || logo == null) return;

            itemsPanel.Location = new Point(5, toggleButton.Bottom + 5);
            itemsPanel.Size = new Size(Width - 10, Height - toggleButton.Bottom - logo.Height - 15);
            itemsPanel.PerformLayout();
        }

        // Override ApplyTheme to apply theme colors to the control
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            // Apply font theme based on ControlStyle
            AccordionFontHelpers.ApplyFontTheme(ControlStyle);

            if (_currentTheme != null)
            {
                // Use theme helpers for consistent color retrieval
                BackColor = AccordionThemeHelpers.GetAccordionBackgroundColor(
                    _currentTheme,
                    UseThemeColors);

                // Update header height based on style
                headerHeight = AccordionStyleHelpers.GetRecommendedHeaderHeight(_accordionStyle);
                itemHeight = AccordionStyleHelpers.GetRecommendedItemHeight(_accordionStyle);
                childItemHeight = AccordionStyleHelpers.GetRecommendedChildItemHeight(_accordionStyle);
                indentationWidth = AccordionStyleHelpers.GetRecommendedIndentation(_accordionStyle);
                spacing = AccordionStyleHelpers.GetRecommendedSpacing(_accordionStyle);
            }

            Invalidate();
        }
    }
}
