using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
        #region Mouse Handling

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Skip tab hover handling in Single mode
            if (_displayMode == ContainerDisplayMode.Single)
            {
                return;
            }

            var hitTab = GetTabAt(e.Location);
            
            if (hitTab != _hoveredTab)
            {
                // Update hover state
                if (_hoveredTab != null)
                {
                    _hoveredTab.IsCloseHovered = false;
                    StartAnimation(_hoveredTab, 0f);
                }

                _hoveredTab = hitTab;
                
                if (_hoveredTab != null)
                {
                    StartAnimation(_hoveredTab, 1f);
                }

                Invalidate();
            }

            // Check close button hover
            if (_hoveredTab != null && _showCloseButtons && _hoveredTab.CanClose)
            {
                var closeRect = GetCloseButtonRect(_hoveredTab.Bounds);
                bool isCloseHovered = closeRect.Contains(e.Location);
                
                if (isCloseHovered != _hoveredTab.IsCloseHovered)
                {
                    _hoveredTab.IsCloseHovered = isCloseHovered;
                    Cursor = isCloseHovered ? Cursors.Hand : Cursors.Default;
                    Invalidate();
                }
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveredTab != null)
            {
                _hoveredTab.IsCloseHovered = false;
                StartAnimation(_hoveredTab, 0f);
                _hoveredTab = null;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Skip tab click handling in Single mode
            if (_displayMode == ContainerDisplayMode.Single)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                var hitTab = GetTabAt(e.Location);
                if (hitTab != null)
                {
                    // Check if clicking close button
                    if (_showCloseButtons && hitTab.CanClose)
                    {
                        var closeRect = GetCloseButtonRect(hitTab.Bounds);
                        if (closeRect.Contains(e.Location))
                        {
                            RemoveTab(hitTab);
                            return;
                        }
                    }

                    // Activate tab
                    ActivateTab(hitTab);
                }
                else
                {
                    // Check scroll buttons
                    if (_needsScrolling)
                    {
                        if (_scrollLeftButton.Contains(e.Location))
                        {
                            ScrollTabs(-1);
                        }
                        else if (_scrollRightButton.Contains(e.Location))
                        {
                            ScrollTabs(1);
                        }
                        else if (_newTabButton.Contains(e.Location))
                        {
                            // Trigger new tab event
                            OnNewTabRequested();
                        }
                    }
                }
            }
        }

        private void ScrollTabs(int direction)
        {
            _scrollOffset += direction;
            _scrollOffset = Math.Max(0, Math.Min(_tabs.Count - 1, _scrollOffset));
            CalculateTabLayout();
            Invalidate();
        }

        private void OnNewTabRequested()
        {
            // Override in derived classes or handle via events
        }

        #endregion
    }
}

