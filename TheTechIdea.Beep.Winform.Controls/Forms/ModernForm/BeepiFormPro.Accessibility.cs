using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Diagnostics;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    /// <summary>
    /// The caption bar as a real UI Automation subtree: one accessible child per visible caption
    /// button, each nameable, focusable and invokable.
    /// </summary>
    /// <remarks>
    /// The vocabulary was already here - names, roles, descriptions and default-action text, with
    /// live variation ("Caption restore button" vs "Caption maximize button"). What was missing was
    /// anything to attach it to. There were **no AccessibleObjects at all**; the vocabulary was
    /// delivered by overwriting the FORM's own AccessibleName/Role/Description as keyboard focus
    /// moved, stashing the originals to put back later.
    ///
    /// Three things a commercial accessibility audit fails on, all measured:
    ///
    /// - `AccessibilityObject.GetChildCount()` returned **-1**. A screen reader could not enumerate
    ///   the caption buttons, list them, or navigate to one; it only heard the window rename itself.
    /// - Automation had nothing to click. No child means no InvokePattern, so WinAppDriver / FlaUI /
    ///   Coded UI cannot press Close.
    /// - The form's identity was clobbered while a caption region had focus - measured as
    ///   AccessibleName becoming "Caption close button" on a form named "My Application Window".
    ///
    /// The existing GetCaptionAccessibility* methods are now the children's data source rather than
    /// being copied onto the form, so the tooltip, the screen reader and the automation name cannot
    /// drift apart - there is one source for all three.
    /// </remarks>
    public partial class BeepiFormPro
    {
        /// <summary>
        /// The caption regions a screen reader should see, in registration order.
        /// </summary>
        /// <remarks>
        /// "caption", "title" and "icon" are excluded: the first two are the drag surface, not
        /// controls, and exposing them as buttons would tell a screen-reader user there are three
        /// more things to press than there are.
        ///
        /// Order must be stable, because a child's index IS its automation identity for the run.
        /// Registration order is the painter's left-to-right order, which is also the order F6
        /// cycles - so keyboard order and UIA order agree without a second sort.
        /// </remarks>
        internal List<HitArea> GetAccessibleCaptionRegions()
        {
            var result = new List<HitArea>();
            if (_hits?.Areas == null) return result;

            foreach (var area in _hits.Areas)
            {
                if (string.IsNullOrWhiteSpace(area?.Name)) continue;

                string key = BeepiFormProHitAreaManager.NormalizeName(area.Name);
                if (key == FormHitAreaNames.Caption ||
                    key == FormHitAreaNames.Title ||
                    key == FormHitAreaNames.Icon)
                    continue;

                if (result.Any(a => BeepiFormProHitAreaManager.NormalizeName(a.Name) == key))
                    continue;

                result.Add(area);
            }
            return result;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
            => new BeepiFormProAccessibleObject(this);

        /// <summary>Tells assistive tech that focus moved to a caption child.</summary>
        private void NotifyCaptionChildFocus(string areaName)
        {
            try
            {
                var regions = GetAccessibleCaptionRegions();
                string key = BeepiFormProHitAreaManager.NormalizeName(areaName);
                int index = regions.FindIndex(a => BeepiFormProHitAreaManager.NormalizeName(a.Name) == key);
                if (index < 0) return;

                // MSAA child IDs are 1-based; 0 means "the form itself".
                AccessibilityNotifyClients(AccessibleEvents.Focus, index + 1);
            }
            catch (Exception ex)
            {
                BeepLog.FailureOnce("form.a11y.focusnotify", this,
                    $"notify assistive technology that caption focus moved to '{areaName}'", ex);
            }
        }

        internal sealed class BeepiFormProAccessibleObject : Control.ControlAccessibleObject
        {
            private readonly BeepiFormPro _form;

            public BeepiFormProAccessibleObject(BeepiFormPro form) : base(form) => _form = form;

            public override AccessibleRole Role =>
                _form.AccessibleRole != AccessibleRole.Default ? _form.AccessibleRole : AccessibleRole.Window;

            public override int GetChildCount() => _form.GetAccessibleCaptionRegions().Count;

            public override AccessibleObject GetChild(int index)
            {
                var regions = _form.GetAccessibleCaptionRegions();
                return index >= 0 && index < regions.Count
                    ? new CaptionRegionAccessibleObject(_form, this, regions[index].Name)
                    : null;
            }

            public override AccessibleObject HitTest(int x, int y)
            {
                var client = _form.PointToClient(new Point(x, y));
                var regions = _form.GetAccessibleCaptionRegions();
                for (int i = 0; i < regions.Count; i++)
                {
                    if (regions[i].Bounds.Contains(client))
                        return GetChild(i);
                }
                return base.HitTest(x, y);
            }

            public override AccessibleObject GetFocused()
            {
                string focused = _form.KeyboardFocusedCaptionArea;
                if (string.IsNullOrEmpty(focused)) return base.GetFocused();

                var regions = _form.GetAccessibleCaptionRegions();
                string key = BeepiFormProHitAreaManager.NormalizeName(focused);
                int index = regions.FindIndex(a => BeepiFormProHitAreaManager.NormalizeName(a.Name) == key);
                return index >= 0 ? GetChild(index) : base.GetFocused();
            }
        }

        /// <summary>One caption button, as automation sees it.</summary>
        internal sealed class CaptionRegionAccessibleObject : AccessibleObject
        {
            private readonly BeepiFormPro _form;
            private readonly AccessibleObject _parent;
            private readonly string _areaName;

            public CaptionRegionAccessibleObject(BeepiFormPro form, AccessibleObject parent, string areaName)
            {
                _form = form;
                _parent = parent;
                _areaName = areaName;
            }

            public override AccessibleObject Parent => _parent;

            // All four come from the methods that already existed. Copying them onto the form was
            // the bug; reading them from here is the fix.
            public override string Name => _form.GetCaptionAccessibilityName(_areaName);
            public override string Description => _form.GetCaptionAccessibilityDescription(_areaName);
            public override string DefaultAction => _form.GetCaptionAccessibilityDefaultActionDescription(_areaName);
            public override AccessibleRole Role => _form.GetCaptionAccessibilityRole(_areaName);

            public override Rectangle Bounds
            {
                get
                {
                    var area = _form._hits?.GetHitArea(_areaName);
                    if (area == null || area.Bounds.IsEmpty) return Rectangle.Empty;

                    // Screen coordinates: a client rect would put a screen reader's highlight in
                    // the wrong place on every monitor but the primary at (0,0).
                    return _form.RectangleToScreen(area.Bounds);
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    var state = AccessibleStates.Focusable;

                    var area = _form._hits?.GetHitArea(_areaName);
                    if (area == null || area.Bounds.IsEmpty)
                        state |= AccessibleStates.Invisible;

                    string focused = _form.KeyboardFocusedCaptionArea;
                    if (!string.IsNullOrEmpty(focused) &&
                        BeepiFormProHitAreaManager.NormalizeName(focused) ==
                        BeepiFormProHitAreaManager.NormalizeName(_areaName))
                        state |= AccessibleStates.Focused;

                    return state;
                }
            }

            /// <summary>
            /// Invoked by automation - the same path a mouse click and the Enter key take.
            /// </summary>
            public override void DoDefaultAction()
            {
                var area = _form._hits?.GetHitArea(_areaName)
                           ?? new HitArea { Name = _areaName, Bounds = Rectangle.Empty };
                _form.OnRegionClicked(area);
            }

            public override void Select(AccessibleSelection flags)
            {
                if ((flags & (AccessibleSelection.TakeFocus | AccessibleSelection.TakeSelection)) != 0)
                    _form.SetKeyboardFocusedCaptionArea(_areaName, focusForm: true);
            }
        }
    }
}
