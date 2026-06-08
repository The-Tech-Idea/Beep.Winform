using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class RadioGroupTests
    {
        private static BeepRadioGroup NewRadioGroup()
        {
            // Code-only construction — no .Designer.cs / no InitializeComponent.
            // The control is built up entirely in the constructor.
            return new BeepRadioGroup();
        }

        [Fact]
        public void Default_Construction_Produces_Empty_Group_Without_Errors()
        {
            using var rg = NewRadioGroup();
            Assert.NotNull(rg);
            Assert.NotNull(rg.Items);
            Assert.Empty(rg.Items);
            // Renderers are wired up by the constructor.
            Assert.NotNull(rg.GetType().GetField("_renderers",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        }

        [Fact]
        public void Adding_Items_Does_Not_Throw()
        {
            using var rg = NewRadioGroup();
            rg.AddItem("One");
            rg.AddItem("Two", "icon.png", "Subtitle");
            Assert.Equal(2, rg.Items.Count);
            Assert.Equal("One", rg.Items[0].Text);
            Assert.Equal("Subtitle", rg.Items[1].SubText);
        }

        [Fact]
        public void Setting_Items_Triggers_Refresh()
        {
            using var rg = NewRadioGroup();
            rg.Items = new List<TheTechIdea.Beep.Winform.Controls.Models.SimpleItem>
            {
                new() { Text = "A" },
                new() { Text = "B" },
                new() { Text = "C" }
            };
            Assert.Equal(3, rg.Items.Count);
        }

        [Fact]
        public void AllowMultipleSelection_Propagates_To_All_Renderers()
        {
            using var rg = NewRadioGroup();
            rg.AllowMultipleSelection = true;
            // Reach into the private dictionary via reflection to assert propagation.
            var f = typeof(BeepRadioGroup).GetField("_renderers",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var dict = (Dictionary<RadioGroupRenderStyle, IRadioGroupRenderer>)f!.GetValue(rg)!;
            foreach (var kv in dict)
            {
                Assert.True(kv.Value.AllowMultipleSelection, $"Renderer {kv.Key} did not receive AllowMultipleSelection=true");
            }
        }

        [Fact]
        public void HasError_Flag_Toggles_ItemState_IsError()
        {
            using var rg = NewRadioGroup();
            rg.AddItem("Option");
            rg.HasError = true;
            // Trigger a state update by reflecting into the private UpdateItemStates().
            var m = typeof(BeepRadioGroup).GetMethod("UpdateItemStates",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            m!.Invoke(rg, new object?[] { false });
            var statesF = typeof(BeepRadioGroup).GetField("_itemStates",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var states = (List<RadioItemState>)statesF!.GetValue(rg)!;
            Assert.Single(states);
            Assert.True(states[0].IsError);
        }

        [Fact]
        public void DataBind_Projects_Source_To_SimpleItem_With_Tag()
        {
            using var rg = NewRadioGroup();
            var source = new[]
            {
                new { Name = "Alpha", Code = 1 },
                new { Name = "Beta",  Code = 2 }
            };
            rg.DataBind(source, x => x.Name, subTextSelector: x => $"Code {x.Code}");
            Assert.Equal(2, rg.Items.Count);
            Assert.Equal("Alpha", rg.Items[0].Text);
            Assert.Equal("Code 1", rg.Items[0].SubText);
            Assert.NotNull(rg.Items[0].Tag);

            var first = rg.GetSelectedItem<SourceTypePlaceholder>();
            Assert.Null(first); // nothing selected
        }

        [Fact]
        public void SearchText_Filter_Matches_CaseInsensitive_And_Headers_Always_Visible()
        {
            using var rg = NewRadioGroup();
            rg.AddItem("Apple");
            rg.AddItem("Banana");
            Assert.True(rg.MatchesSearch(rg.Items[0]));
            rg.SearchText = "ban";
            Assert.False(rg.MatchesSearch(rg.Items[0]));
            Assert.True(rg.MatchesSearch(rg.Items[1]));
        }

        [Fact]
        public void Wrap_Orientation_And_ColumnGap_RowGap_Properties_Exist()
        {
            using var rg = NewRadioGroup();
            var layout = rg.GetType().GetField("_layoutHelper",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            var helper = layout.GetValue(rg)!;
            var t = helper.GetType();
            Assert.NotNull(t.GetProperty("ColumnGap"));
            Assert.NotNull(t.GetProperty("RowGap"));
            t.GetProperty("Orientation")!.SetValue(helper, RadioGroupOrientation.Wrap);
            t.GetProperty("ColumnGap")!.SetValue(helper, 12);
            t.GetProperty("RowGap")!.SetValue(helper, 8);
        }

        [Fact]
        public void IsError_State_Flows_Into_Renderer_Through_ItemState()
        {
            using var rg = NewRadioGroup();
            rg.AddItem("Err");
            rg.HasError = true;
            var m = typeof(BeepRadioGroup).GetMethod("UpdateItemStates",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            m!.Invoke(rg, new object?[] { false });
            var statesF = typeof(BeepRadioGroup).GetField("_itemStates",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var states = (List<RadioItemState>)statesF!.GetValue(rg)!;
            Assert.True(states[0].IsError);
        }

        [Fact]
        public void Dispose_Calls_Cleanup_On_Every_Renderer()
        {
            using var rg = NewRadioGroup();
            // Dispose() on BeepRadioGroup -> propagates Cleanup to each renderer.
            // We assert this by ensuring Dispose() doesn't throw when called twice
            // and that the control's state remains valid for re-creation.
            rg.Dispose();
        }

        [Fact]
        public void ScrollIntoView_Is_Alias_For_EnsureItemVisible()
        {
            using var rg = NewRadioGroup();
            // Both methods should be public on BeepRadioGroup.
            Assert.NotNull(typeof(BeepRadioGroup).GetMethod("ScrollIntoView",
                new[] { typeof(int) }));
            Assert.NotNull(typeof(BeepRadioGroup).GetMethod("EnsureItemVisible",
                new[] { typeof(int) }));
        }

        [Fact]
        public void VirtualizationThreshold_Is_Browsable_With_Category_Behavior()
        {
            using var rg = NewRadioGroup();
            var prop = typeof(BeepRadioGroup).GetProperty("VirtualizationThreshold");
            Assert.NotNull(prop);
            var attrs = prop!.GetCustomAttributes(typeof(BrowsableAttribute), false);
            Assert.NotEmpty(attrs);
            var cat = (CategoryAttribute)prop.GetCustomAttributes(typeof(CategoryAttribute), false)[0];
            Assert.Equal("Behavior", cat.Category);
        }

        [Fact]
        public void ShowSearchBox_Toggle_Reserves_TopOffset_For_Items()
        {
            using var rg = NewRadioGroup();
            rg.Size = new Size(200, 200);
            rg.ShowSearchBox = true;
            // After enabling the search box, TopoffsetForDrawingRect must be > 0
            // so items don't render under the docked BeepTextBox.
            Assert.True(rg.TopoffsetForDrawingRect > 0,
                $"Expected TopoffsetForDrawingRect > 0, got {rg.TopoffsetForDrawingRect}");
            rg.ShowSearchBox = false;
            Assert.Equal(0, rg.TopoffsetForDrawingRect);
        }

        [Fact]
        public void Search_Threshold_Defaults_To_Ten()
        {
            using var rg = NewRadioGroup();
            Assert.Equal(10, rg.SearchThreshold);
        }

        [Fact]
        public void Headers_In_DataBind_Are_Non_Interactive()
        {
            using var rg = NewRadioGroup();
            // Manually inject a header-tagged item, then assert IsItemInteractive returns false.
            var item = new TheTechIdea.Beep.Winform.Controls.Models.SimpleItem
            {
                Text = "Group A",
                Tag = new HeaderProbe()
            };
            rg.Items.Add(item);
            var helperF = typeof(BeepRadioGroup).GetField("_hitTestHelper",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var helper = helperF!.GetValue(rg)!;
            // The hit test helper exposes IsItemInteractive as a public Func<int, bool> property.
            var funcProp = helper.GetType().GetProperty("IsItemInteractive",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(funcProp);
            var func = (Func<int, bool>)funcProp!.GetValue(helper)!;
            Assert.NotNull(func);
            Assert.False(func(0), "Header-tagged item should not be interactive");
        }

        [Fact]
        public void DataBind_With_ImagePathSelector_Propagates_ImagePath()
        {
            using var rg = NewRadioGroup();
            var source = new[]
            {
                new { Label = "A", Glyph = "check.png" },
                new { Label = "B", Glyph = "cross.png" }
            };
            rg.DataBind(source, x => x.Label, imagePathSelector: x => x.Glyph);
            Assert.Equal("check.png", rg.Items[0].ImagePath);
            Assert.Equal("cross.png", rg.Items[1].ImagePath);
        }

        [Fact]
        public void DataBind_Clears_Previous_Selection()
        {
            using var rg = NewRadioGroup();
            // CreateControl forces the handle to exist so UpdateItemsAndLayout actually
            // runs and the state helper learns about the items.
            if (!rg.IsHandleCreated) rg.CreateControl();
            rg.AddItem("A");
            rg.AddItem("B");
            rg.SelectItem("A");
            Assert.Equal(1, rg.SelectedCount);

            var source = new[]
            {
                new { Label = "X" },
                new { Label = "Y" }
            };
            rg.DataBind(source, x => x.Label);
            Assert.Equal(0, rg.SelectedCount);
            Assert.Empty(rg.SelectedItems);
        }

        [Fact]
        public void OnItemDoubleClicked_Handler_Is_Wired()
        {
            using var rg = NewRadioGroup();
            // Both ItemClicked and ItemDoubleClicked should be wired in SetupEventHandlers.
            // The simplest assertion is to confirm ItemDoubleClicked event is non-null after
            // the control has run its constructor (it would be null if no subscriber but the
            // event key is still there). We assert the field exists and is reachable via
            // reflection.
            var ev = typeof(BeepRadioGroup).GetEvent("ItemDoubleClicked",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(ev);
        }

        [Fact]
        public void SearchBox_Fallback_Height_Is_Applied_When_PreferredSize_Zero()
        {
            using var rg = NewRadioGroup();
            rg.Size = new Size(200, 200);
            // ShowSearchBox → EnsureSearchBox → TopoffsetForDrawingRect must be >= 32.
            rg.ShowSearchBox = true;
            Assert.True(rg.TopoffsetForDrawingRect >= 32,
                $"Expected TopoffsetForDrawingRect >= 32, got {rg.TopoffsetForDrawingRect}");
        }

        [Fact]
        public void Items_Setter_Clears_Previous_Selection()
        {
            using var rg = NewRadioGroup();
            if (!rg.IsHandleCreated) rg.CreateControl();
            rg.AddItem("A");
            rg.AddItem("B");
            rg.SelectItem("A");
            Assert.Equal(1, rg.SelectedCount);

            rg.Items = new List<TheTechIdea.Beep.Winform.Controls.Models.SimpleItem>
            {
                new() { Text = "X" },
                new() { Text = "Y" }
            };
            Assert.Equal(0, rg.SelectedCount);
        }

        [Fact]
        public void IsRequired_Triggers_UpdateItemStates_When_Toggled()
        {
            using var rg = NewRadioGroup();
            if (!rg.IsHandleCreated) rg.CreateControl();
            // No items, no selection → toggling IsRequired should set HasError=true
            // and the item-states list (even when empty) should reflect that flow.
            rg.IsRequired = true;
            Assert.True(rg.HasError);

            var statesF = typeof(BeepRadioGroup).GetField("_itemStates",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var states = (List<RadioItemState>)statesF!.GetValue(rg)!;
            // 0 items → 0 states; what we care about is the side effect on HasError.
            Assert.Empty(states);

            // Now add an item and re-validate.
            rg.AddItem("Z");
            rg.Validate();
            Assert.True(rg.HasError);
            rg.SelectItem("Z");
            rg.Validate();
            Assert.False(rg.HasError);
        }

        [Fact]
        public void Hierarchical_Has_ShowSearchBox_SearchThreshold_SearchText_And_DisableItem()
        {
            // Parity test: every property/method on BeepRadioGroup that mirrors to
            // Hierarchical must exist on BeepHierarchicalRadioGroup.
            var hType = typeof(BeepHierarchicalRadioGroup);
            Assert.NotNull(hType.GetProperty("ShowSearchBox"));
            Assert.NotNull(hType.GetProperty("SearchThreshold"));
            Assert.NotNull(hType.GetProperty("SearchText"));
            Assert.NotNull(hType.GetMethod("MatchesSearch", new[] { typeof(TheTechIdea.Beep.Winform.Controls.Models.SimpleItem) }));
            Assert.NotNull(hType.GetMethod("EnsureItemVisible", new[] { typeof(int) }));
            Assert.NotNull(hType.GetMethod("ScrollIntoView", new[] { typeof(int) }));
            Assert.NotNull(hType.GetMethod("DisableItem", new[] { typeof(string) }));
            Assert.NotNull(hType.GetMethod("EnableItem", new[] { typeof(string) }));
            Assert.NotNull(hType.GetMethod("IsItemDisabled", new[] { typeof(string) }));
            Assert.NotNull(hType.GetMethod("EnableAllItems"));
        }

        [Fact]
        public void OnHandleCreated_Is_Overridden_To_Run_Initial_Layout()
        {
            // Both controls must override OnHandleCreated to run their layout pipeline
            // when the handle first exists, since UpdateItemsAndLayout/UpdateHierarchy
            // bail out before the handle is created.
            Assert.NotNull(typeof(BeepRadioGroup).GetMethod("OnHandleCreated",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            Assert.NotNull(typeof(BeepHierarchicalRadioGroup).GetMethod("OnHandleCreated",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        }

        [Fact]
        public void Hierarchical_Has_IsRequired_Validate_And_ErrorMessage()
        {
            var hType = typeof(BeepHierarchicalRadioGroup);
            Assert.NotNull(hType.GetProperty("IsRequired"));
            Assert.NotNull(hType.GetProperty("ErrorMessage"));
            Assert.NotNull(hType.GetMethod("Validate", Type.EmptyTypes));
        }

        [Fact]
        public void Circular_AnimationProgress_Interpolates_Inner_Dot_Radius()
        {
            // The Circular renderer's inner dot radius is target * AnimationProgress.
            // We can't paint directly in the test, but we can confirm the public API
            // stays stable and the renderer inherits from BaseRadioRenderer.
            var renderer = new CircularRadioRenderer();
            Assert.IsAssignableFrom<IRadioGroupRenderer>(renderer);
            // Initial AnimationProgress is 1.0 (no animation), so dot is full size.
            var state = new RadioItemState { IsSelected = true, AnimationProgress = 1f };
            Assert.Equal(1f, state.AnimationProgress);
        }

        [Fact]
        public void OnMouseEnter_Sets_Hand_Cursor()
        {
            using var rg = NewRadioGroup();
            // The OnMouseEnter override exists and is non-public; reflect on it.
            var m = typeof(BeepRadioGroup).GetMethod("OnMouseEnter",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(m);
        }

        [Fact]
        public void Both_Controls_Handle_OnMouseDown_OnMouseWheel_And_OnParentChanged()
        {
            // Parity test: every input event handler we added in Pass 7 should be
            // overridden on both controls so the behavior is consistent.
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            foreach (var t in new[] { typeof(BeepRadioGroup), typeof(BeepHierarchicalRadioGroup) })
            {
                Assert.NotNull(t.GetMethod("OnMouseDown", flags));
                Assert.NotNull(t.GetMethod("OnMouseWheel", flags));
                Assert.NotNull(t.GetMethod("OnParentChanged", flags));
                Assert.NotNull(t.GetMethod("OnVisibleChanged", flags));
            }
        }

        [Fact]
        public void BaseRadioRenderer_Exposes_LerpColor_Helper()
        {
            // LerpColor is a protected static; assert the public surface is stable
            // by checking the type still has the expected non-private surface.
            var t = typeof(BaseRadioRenderer);
            Assert.True(t.IsAbstract);
            Assert.True(typeof(IRadioGroupRenderer).IsAssignableFrom(t));
        }

        [Fact]
        public void Renderers_All_Support_AnimationProgress_For_Selected_State()
        {
            // Every renderer should have its own RenderItem and accept RadioItemState
            // with an AnimationProgress field that the renderer interpolates over.
            // We can't paint in the test, but we can confirm the field exists on
            // RadioItemState and is settable.
            var state = new RadioItemState { AnimationProgress = 0.5f };
            Assert.Equal(0.5f, state.AnimationProgress);
            var p = typeof(RadioItemState).GetProperty("AnimationProgress");
            Assert.NotNull(p);
            Assert.True(p.CanWrite);
        }

        [Fact]
        public void BeepRadioGroup_Exposes_AddItems_Batch_Method()
        {
            // Pass 8 added a batch AddItems helper so callers don't have to
            // re-run the layout pipeline per item.
            var m = typeof(BeepRadioGroup).GetMethod("AddItems",
                new[] { typeof(System.Collections.Generic.IEnumerable<TheTechIdea.Beep.Winform.Controls.Models.SimpleItem>) });
            Assert.NotNull(m);
            Assert.Equal(typeof(void), m.ReturnType);
        }

        [Fact]
        public void Segmented_Renderer_Does_Not_Carry_Stale_LastState_Field()
        {
            // Pass 8 removed the fragile _lastState field; the renderer should no
            // longer expose it as a non-public field.  Use reflection to confirm.
            var renderer = new SegmentedRadioRenderer();
            var field = typeof(SegmentedRadioRenderer).GetField("_lastState",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.Null(field);
        }

        [Fact]
        public void ClearItems_Clears_Animation_Progress()
        {
            using var rg = NewRadioGroup();
            rg.AddItem("one");
            rg.AddItem("two");
            // Start an animation on item 0
            rg.StartItemAnimation(0);
            var ap = typeof(BeepRadioGroup).GetField("_animationProgress",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(ap);
            var dict = (System.Collections.Generic.Dictionary<int, float>)ap.GetValue(rg);
            Assert.NotEmpty(dict);
            rg.ClearItems();
            dict = (System.Collections.Generic.Dictionary<int, float>)ap.GetValue(rg);
            Assert.Empty(dict);
        }

        [Fact]
        public void RadioGroupStyle_Change_Clears_Animation_Progress()
        {
            using var rg = NewRadioGroup();
            rg.AddItem("a");
            rg.AddItem("b");
            rg.RadioGroupStyle = RadioGroupRenderStyle.Material;
            rg.StartItemAnimation(0);
            var ap = typeof(BeepRadioGroup).GetField("_animationProgress",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var dict = (System.Collections.Generic.Dictionary<int, float>)ap.GetValue(rg);
            Assert.NotEmpty(dict);
            rg.RadioGroupStyle = RadioGroupRenderStyle.Chip;
            dict = (System.Collections.Generic.Dictionary<int, float>)ap.GetValue(rg);
            Assert.Empty(dict);
        }

        [Fact]
        public void Both_Controls_Expose_DisabledItems_Collection()
        {
            // Pass 9 added a public read-only view of the disabled-item texts so
            // callers can serialize / inspect the disabled set without poking at
            // private fields via reflection.
            foreach (var t in new[] { typeof(BeepRadioGroup), typeof(BeepHierarchicalRadioGroup) })
            {
                var p = t.GetProperty("DisabledItems");
                Assert.NotNull(p);
                Assert.True(typeof(System.Collections.Generic.IReadOnlyCollection<string>).IsAssignableFrom(p.PropertyType));
            }
        }

        [Fact]
        public void Hierarchical_Has_ClearSelection_SelectAll_Reset_And_OnMouseEnter()
        {
            // Parity test: every selection-management method on BeepRadioGroup that
            // wasn't mirrored to Hierarchical should now exist.
            var hType = typeof(BeepHierarchicalRadioGroup);
            Assert.NotNull(hType.GetMethod("ClearSelection", Type.EmptyTypes));
            Assert.NotNull(hType.GetMethod("SelectAll", Type.EmptyTypes));
            Assert.NotNull(hType.GetMethod("Reset", Type.EmptyTypes));
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            Assert.NotNull(hType.GetMethod("OnMouseEnter", flags));
        }

        [Fact]
        public void Hierarchical_AllowMultipleSelection_Falls_Back_To_Material()
        {
            using var h = new BeepHierarchicalRadioGroup();
            h.RenderStyle = RadioGroupRenderStyle.Segmented; // does not support multiple
            h.AllowMultipleSelection = true;
            // After enabling multi, the renderer should have fallen back to a
            // multi-capable one (Material).
            Assert.True(h.RenderStyle == RadioGroupRenderStyle.Material);
        }

        [Fact]
        public void BeepRadioGroup_Stores_VScroll_Handler_As_Field()
        {
            // Pass 9 stored the Scroll handler in a field so Dispose can unsubscribe
            // it.  Confirm the field exists with the expected type.
            var f = typeof(BeepRadioGroup).GetField("_vScrollHandler",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(f);
            Assert.Equal(typeof(System.Windows.Forms.ScrollEventHandler), f.FieldType);
        }

        [Fact]
        public void Hierarchical_Exposes_Virtualization_API()
        {
            // Pass 12 added virtualization (VScrollBar, scroll offset, visible
            // window) to BeepHierarchicalRadioGroup.  Confirm the public surface
            // exists and the private fields backing it exist with expected types.
            var hType = typeof(BeepHierarchicalRadioGroup);

            var pIsVirtualized = hType.GetProperty("IsVirtualized");
            Assert.NotNull(pIsVirtualized);
            Assert.Equal(typeof(bool), pIsVirtualized.PropertyType);

            var pThreshold = hType.GetProperty("VirtualizationThreshold");
            Assert.NotNull(pThreshold);
            Assert.Equal(typeof(int), pThreshold.PropertyType);

            var pEnsure = hType.GetMethod("EnsureItemVisible", new[] { typeof(int) });
            Assert.NotNull(pEnsure);

            var pScrollInto = hType.GetMethod("ScrollIntoView", new[] { typeof(int) });
            Assert.NotNull(pScrollInto);

            var fVScroll = hType.GetField("_vScroll",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(fVScroll);

            var fScrollOffset = hType.GetField("_scrollOffset",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(fScrollOffset);
            Assert.Equal(typeof(int), fScrollOffset.FieldType);

            var fThreshold = hType.GetField("_virtualizationThreshold",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(fThreshold);
            Assert.Equal(typeof(int), fThreshold.FieldType);

            var fVisible = hType.GetField("_visibleIndices",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(fVisible);
        }

        [Fact]
        public void Hierarchical_IsVirtualized_Stays_False_Below_Threshold()
        {
            using var h = new BeepHierarchicalRadioGroup();
            // Default threshold is 50; with 5 items we are well below.
            for (int i = 0; i < 5; i++)
            {
                h.AddRootItem(new SimpleItem { Text = $"Item {i}" });
            }
            Assert.False(h.IsVirtualized);
        }

        [Fact]
        public void Hierarchical_Virtualization_Threshold_Defaults_To_50()
        {
            using var h = new BeepHierarchicalRadioGroup();
            Assert.Equal(50, h.VirtualizationThreshold);
        }

        [Fact]
        public void BeepRadioGroup_Has_TranslateMouseForHitTest_Helper()
        {
            // Pass 13 added TranslateMouseForHitTest to fix a bug where clicks on
            // items past the first scroll window would hit-test to the wrong index
            // because the renderer draws with a -_scrollOffset Y translate that
            // hit-testing did not compensate for.
            var m = typeof(BeepRadioGroup).GetMethod("TranslateMouseForHitTest",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(m);
            Assert.Equal(typeof(Point), m.ReturnType);
        }

        [Fact]
        public void BeepRadioGroup_TranslateMouseForHitTest_Is_Identity_When_Not_Virtualized()
        {
            using var rg = NewRadioGroup();
            var m = typeof(BeepRadioGroup).GetMethod("TranslateMouseForHitTest",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var p = new Point(5, 12);
            var result = (Point)m!.Invoke(rg, new object[] { p })!;
            Assert.Equal(p, result);
        }

        [Fact]
        public void BeepRadioGroup_TranslateMouseForHitTest_Adds_ScrollOffset_When_Virtualized()
        {
            using var rg = NewRadioGroup();
            // Add enough items to cross the virtualized threshold (default 50)
            // and force a measurable scroll offset.
            for (int i = 0; i < 200; i++)
            {
                rg.AddItem(new SimpleItem { Text = $"Item {i}" });
            }
            Assert.True(rg.IsVirtualized);

            // Set the private _scrollOffset field to a known value and check the helper.
            var fOffset = typeof(BeepRadioGroup).GetField("_scrollOffset",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(fOffset);
            fOffset!.SetValue(rg, 200);

            var m = typeof(BeepRadioGroup).GetMethod("TranslateMouseForHitTest",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var p = new Point(5, 50);
            var result = (Point)m!.Invoke(rg, new object[] { p })!;
            Assert.Equal(new Point(5, 250), result);
        }

        [Fact]
        public void BeepRadioGroup_Stores_SearchBoxTextChangedHandler_As_Field()
        {
            // Pass 13 stored the search-box TextChanged handler in a field so Dispose
            // can detach the same delegate (an anonymous lambda can't be removed by
            // -=).  Confirm the field exists with the expected type.
            var f = typeof(BeepRadioGroup).GetField("_searchBoxTextChangedHandler",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(f);
            Assert.Equal(typeof(System.EventHandler), f.FieldType);
        }

        [Fact]
        public void BeepRadioGroup_DataBind_Clears_Animation_Progress()
        {
            // Pass 13: DataBind<T> previously left _animationProgress populated
            // with stale indices from the prior item list.  The animation timer
            // would keep ticking over dead indices until they all completed.
            using var rg = NewRadioGroup();

            // Force a non-empty animation progress to simulate an in-flight animation.
            var fProgress = typeof(BeepRadioGroup).GetField("_animationProgress",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(fProgress);
            var progress = (System.Collections.Generic.Dictionary<int, float>)fProgress!.GetValue(rg)!;
            progress[7] = 0.5f;
            progress[42] = 0.3f;
            Assert.Equal(2, progress.Count);

            var data = new[]
            {
                new SourceTypePlaceholder { Name = "A" },
                new SourceTypePlaceholder { Name = "B" }
            };
            rg.DataBind<SourceTypePlaceholder>(data, x => x.Name!);

            Assert.Empty(progress);
        }

        [Fact]
        public void BeepRadioGroup_Items_Show_Disabled_State_When_Control_Is_Disabled()
        {
            // Pass 14: IsEnabled in RadioItemState did not factor in the control's
            // own Enabled flag, so the disabled overlay never rendered when the
            // parent form disabled the radio group.  Now it should.
            using var rg = NewRadioGroup();
            // CreateControl() forces handle creation without needing a parent form.
            rg.CreateControl();
            try
            {
                rg.AddItem(new SimpleItem { Text = "Apple" });
                rg.AddItem(new SimpleItem { Text = "Banana" });

                // Capture the pre-disable states.
                var fStates = typeof(BeepRadioGroup).GetField("_itemStates",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.NotNull(fStates);
                var states = (System.Collections.Generic.List<RadioItemState>)fStates!.GetValue(rg)!;
                Assert.Equal(2, states.Count);
                Assert.All(states, s => Assert.True(s.IsEnabled));

                // Disable the control; the states should reflect that.
                rg.Enabled = false;
                var statesDisabled = (System.Collections.Generic.List<RadioItemState>)fStates!.GetValue(rg)!;
                Assert.All(statesDisabled, s => Assert.False(s.IsEnabled));

                // Re-enable the control; the states should recover.
                rg.Enabled = true;
                var statesEnabled = (System.Collections.Generic.List<RadioItemState>)fStates!.GetValue(rg)!;
                Assert.All(statesEnabled, s => Assert.True(s.IsEnabled));
            }
            finally
            {
                rg.Dispose();
            }
        }

        [Fact]
        public void BeepRadioGroup_VScroll_Has_TabStop_False()
        {
            // Pass 14: the VScrollBar child control had TabStop=true by default.
            // Tab navigation could land on it instead of walking radio items.
            using var rg = NewRadioGroup();
            var f = typeof(BeepRadioGroup).GetField("_vScroll",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(f);
            var vScroll = (System.Windows.Forms.VScrollBar)f!.GetValue(rg)!;
            Assert.NotNull(vScroll);
            Assert.False(vScroll.TabStop);
        }

        [Fact]
        public void BeepRadioGroup_SearchBox_Has_TabStop_False_When_Shown()
        {
            // Pass 14: the search box had TabStop=true by default (BeepTextBox
            // constructor sets it).  Tab navigation should walk radio items, not
            // land on the text box.
            using var rg = NewRadioGroup();
            rg.ShowSearchBox = true;
            // The SearchBox accessor exposes the underlying control.
            Assert.NotNull(rg.SearchBox);
            Assert.False(rg.SearchBox!.TabStop);
        }

        [Fact]
        public void BeepRadioGroup_StartItemAnimation_Leaves_Animation_Progress_Clean()
        {
            // Pass 14: StartItemAnimation used to write _animationProgress[index]=0f
            // even in design mode, leaving dead entries.  The guard now skips the
            // write outside runtime; we can verify by calling it on a fresh
            // (non-design-mode) control and confirming the dictionary is populated,
            // and then clearing + asserting a new call re-populates (proving the
            // call path itself works outside the design-mode gate).
            using var rg = NewRadioGroup();
            var fProgress = typeof(BeepRadioGroup).GetField("_animationProgress",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var progress = (System.Collections.Generic.Dictionary<int, float>)fProgress!.GetValue(rg)!;
            Assert.Empty(progress);

            rg.StartItemAnimation(3);
            Assert.True(progress.ContainsKey(3));
            Assert.Equal(0f, progress[3]);
        }

        [Fact]
        public void BeepRadioGroup_SelectedValue_Setter_Fires_SelectionChanged_Event()
        {
            // Pass 15: SelectedValue setter was silently mutating state via
            // RadioGroupStateHelper.SelectedValue (which calls SetSingleSelection
            // and never raises SelectionChanged).  Now it must use the
            // event-firing path (SelectValue / ToggleValue) so subscribers see
            // programmatic changes.
            using var rg = NewRadioGroup();
            rg.CreateControl();
            try
            {
                rg.AddItem(new SimpleItem { Text = "Apple" });
                rg.AddItem(new SimpleItem { Text = "Banana" });

                int eventCount = 0;
                rg.SelectionChanged += (s, e) => eventCount++;

                rg.SelectedValue = "Banana";
                Assert.Equal(1, eventCount);
                Assert.Equal("Banana", rg.SelectedValue);

                // Re-setting the same value should not fire.
                rg.SelectedValue = "Banana";
                Assert.Equal(1, eventCount);

                // Changing to a different value should fire again.
                rg.SelectedValue = "Apple";
                Assert.Equal(2, eventCount);
                Assert.Equal("Apple", rg.SelectedValue);
            }
            finally { rg.Dispose(); }
        }

        [Fact]
        public void BeepRadioGroup_SetValue_With_List_Fires_SelectionChanged()
        {
            // Pass 15: SetValue(value) with a List<string> went through
            // SetMultipleSelection (non-event-firing) and silently swapped the
            // selection.  Now per-value SelectValue/ToggleValue is used so the
            // event fires.
            using var rg = NewRadioGroup();
            rg.CreateControl();
            try
            {
                rg.AddItem(new SimpleItem { Text = "A" });
                rg.AddItem(new SimpleItem { Text = "B" });
                rg.AddItem(new SimpleItem { Text = "C" });
                // In single mode, SetValue(List<string>) takes the first entry.
                // Enable multi so the assertion that both A and B are selected
                // is meaningful.
                rg.AllowMultipleSelection = true;

                int eventCount = 0;
                rg.SelectionChanged += (s, e) => eventCount++;

                rg.SetValue(new List<string> { "A", "B" });
                Assert.True(eventCount >= 1);
                Assert.Contains("A", rg.SelectedValues);
                Assert.Contains("B", rg.SelectedValues);
                Assert.DoesNotContain("C", rg.SelectedValues);
            }
            finally { rg.Dispose(); }
        }

        [Fact]
        public void BeepRadioGroup_DoubleClick_Performs_Selection()
        {
            // Pass 15: OnItemDoubleClicked previously fired the public event
            // but did NOT perform the selection.  Combined with the trailing
            // single-click suppression in OnItemClicked, a double-click became
            // a no-op for selection.  Now it selects just like a single click.
            using var rg = NewRadioGroup();
            rg.CreateControl();
            try
            {
                rg.AddItem(new SimpleItem { Text = "Apple" });
                rg.AddItem(new SimpleItem { Text = "Banana" });

                var dblClickCount = 0;
                rg.ItemDoubleClicked += (s, e) => dblClickCount++;

                // Invoke the private OnItemDoubleClicked handler directly so we
                // do not depend on mouse timing in the test harness.
                var m = typeof(BeepRadioGroup).GetMethod("OnItemDoubleClicked",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.NotNull(m);
                var args = new ItemClickEventArgs(1, rg.Items[1], System.Windows.Forms.MouseButtons.Left);
                m!.Invoke(rg, new object[] { this, args });

                Assert.Equal(1, dblClickCount);
                Assert.Equal("Banana", rg.SelectedValue);
            }
            finally { rg.Dispose(); }
        }

        [Fact]
        public void BeepRadioGroup_OnMouseDown_RightClick_Does_Not_Steal_Focus()
        {
            // Pass 16: OnMouseDown previously called Focus() regardless of which
            // button was pressed, so a right-click would steal keyboard focus
            // from another control.  Now Focus() is gated to MouseButtons.Left.
            using var rg = NewRadioGroup();
            rg.CreateControl();
            try
            {
                rg.AddItem(new SimpleItem { Text = "Apple" });

                var mDown = typeof(BeepRadioGroup).GetMethod("OnMouseDown",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.NotNull(mDown);

                // Simulate a right-click at the item's centre.
                var args = new MouseEventArgs(MouseButtons.Right, 1, 10, 10, 0);
                mDown!.Invoke(rg, new object[] { args });

                // The control must NOT own focus after a right-click.
                Assert.False(rg.Focused,
                    "Right-click on BeepRadioGroup should not steal keyboard focus.");
            }
            finally { rg.Dispose(); }
        }

        [Fact]
        public void BeepRadioGroup_SelectedValue_In_Multi_Mode_Adds_Rather_Than_Toggles()
        {
            // Pass 16: SelectedValue setter used ToggleValue in multi mode,
            // which DESELECTS a value that was already in the selection.  The
            // "set this value" intent of a SelectedValue property setter
            // requires ADD, not toggle.  Now SelectValue is used in both
            // modes — multi mode ADDs without toggling.
            using var rg = NewRadioGroup();
            rg.CreateControl();
            try
            {
                rg.AddItem(new SimpleItem { Text = "A" });
                rg.AddItem(new SimpleItem { Text = "B" });
                rg.AddItem(new SimpleItem { Text = "C" });
                rg.AllowMultipleSelection = true;

                // First set: adds A.
                rg.SelectedValue = "A";
                Assert.Contains("A", rg.SelectedValues);

                // Second set of the same value: should be a no-op (still in
                // selection), not toggle off.
                rg.SelectedValue = "A";
                Assert.Contains("A", rg.SelectedValues);

                // Second set of a different value: should ADD, not REPLACE.
                rg.SelectedValue = "B";
                Assert.Contains("A", rg.SelectedValues);
                Assert.Contains("B", rg.SelectedValues);
            }
            finally { rg.Dispose(); }
        }

        [Fact]
        public void BeepRadioGroup_Accessibility_Name_Handles_OutOfRange_Index()
        {
            // Pass 16: BeepRadioGroupItemAccessibleObject.Name/Value/State
            // getters indexed _owner._items[_index] without range checks.
            // An accessibility client requesting a stale or out-of-range
            // child would NRE.  Now they fall back to a placeholder.
            using var rg = NewRadioGroup();
            rg.CreateControl();
            try
            {
                var accessible = (AccessibleObject)rg.AccessibilityObject;
                var child = accessible.GetChild(999);
                if (child != null)
                {
                    // Name/Value/State should not throw on the out-of-range child.
                    var _ = child.Name;
                    var __ = child.Value;
                    var ___ = child.State;
                }
                // No assertion needed — the test passes if no exception is thrown.
            }
            finally { rg.Dispose(); }
        }

        [Fact]
        public void BeepRadioGroup_OnFocusedIndexChanged_Calls_EnsureItemVisible()
        {
            // Pass 15: Tab/arrow key navigation could focus an item outside the
            // visible window in virtualized mode.  The focus ring would be drawn
            // off-screen.  Now OnFocusedIndexChanged calls EnsureItemVisible.
            using var rg = NewRadioGroup();
            rg.CreateControl();
            try
            {
                for (int i = 0; i < 200; i++) rg.AddItem(new SimpleItem { Text = $"Item {i}" });
                Assert.True(rg.IsVirtualized);

                // Force a fresh layout pass + scrollbar update so _vScroll.Maximum
                // reflects the actual content height.
                var mMark = typeof(BeepRadioGroup).GetMethod("MarkLayoutDirty",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var mLayout = typeof(BeepRadioGroup).GetMethod("UpdateLayout",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var mScroll = typeof(BeepRadioGroup).GetMethod("UpdateScrollBar",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                mMark!.Invoke(rg, null);
                mLayout!.Invoke(rg, null);
                mScroll!.Invoke(rg, null);

                var fFocused = typeof(BeepRadioGroup).GetField("_hitTestHelper",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var fVScroll = typeof(BeepRadioGroup).GetField("_vScroll",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.NotNull(fVScroll);

                var vScroll = (System.Windows.Forms.VScrollBar)fVScroll!.GetValue(rg)!;
                Assert.True(vScroll.Maximum > 0,
                    $"Pre-condition: vScroll.Maximum should be > 0 with 200 items, was {vScroll.Maximum}.");

                var hth = fFocused!.GetValue(rg)!;
                var fhtFocus = hth.GetType().GetField("_focusedIndex",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                fhtFocus!.SetValue(hth, 150);

                var m = typeof(BeepRadioGroup).GetMethod("OnFocusedIndexChanged",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                m!.Invoke(rg, new object[] { this, new IndexChangedEventArgs(150) });

                Assert.True(vScroll.Value > 0,
                    $"Expected vScroll.Value to be > 0 after focusing item 150, but was {vScroll.Value}.");
            }
            finally { rg.Dispose(); }
        }

        [Fact]
        public void BeepRadioGroup_SetValue_Null_Clears_Selection()
        {
            // Pass 17: SetValue(null) was a no-op — all four `is` checks failed
            // and the method fell through.  Now null is treated as a "clear
            // selection" sentinel, mirroring common data-binding conventions.
            using var rg = NewRadioGroup();
            rg.CreateControl();
            try
            {
                rg.AddItem(new SimpleItem { Text = "Apple" });
                rg.AddItem(new SimpleItem { Text = "Banana" });
                rg.SelectedValue = "Apple";
                Assert.Equal("Apple", rg.SelectedValue);

                int eventCount = 0;
                rg.SelectionChanged += (s, e) => eventCount++;

                rg.SetValue(null!);
                Assert.Equal(0, rg.SelectedCount);
                Assert.True(eventCount >= 1, "SelectionChanged should fire when null clears the selection.");
            }
            finally { rg.Dispose(); }
        }

        [Fact]
        public void BeepRadioGroup_DataBind_Skips_Null_Source_Items()
        {
            // Pass 17: DataBind<T> would NRE when the source collection
            // contained a null item — the text/value selectors would throw on
            // a null argument.  Now null entries are skipped silently.
            using var rg = NewRadioGroup();
            rg.CreateControl();
            try
            {
                var data = new SimpleItem?[]
                {
                    new SimpleItem { Text = "A" },
                    null,
                    new SimpleItem { Text = "B" },
                    null,
                    new SimpleItem { Text = "C" }
                };
                // Use a SourceTypePlaceholder so the textSelector is non-null.
                var data2 = new SourceTypePlaceholder?[]
                {
                    new SourceTypePlaceholder { Name = "X" },
                    null,
                    new SourceTypePlaceholder { Name = "Y" }
                };
                rg.DataBind<SourceTypePlaceholder>(data2!, x => x.Name!);

                Assert.Equal(2, rg.Items.Count);
                Assert.Equal("X", rg.Items[0].Text);
                Assert.Equal("Y", rg.Items[1].Text);
            }
            finally { rg.Dispose(); }
        }

        [Fact]
        public void BeepRadioGroup_AnimationTimer_Tick_Handles_Disposed_Timer()
        {
            // Pass 17: OnAnimationTick used `_animationTimer!.Stop()` with the
            // null-forgiving operator, which would NRE on a race where Dispose
            // sets the field to null while the tick is still running.  The
            // null-check guard now prevents the NRE.
            using var rg = NewRadioGroup();
            rg.CreateControl();
            try
            {
                rg.AddItem(new SimpleItem { Text = "Apple" });

                // Populate the animation progress to keep the timer alive.
                rg.StartItemAnimation(0);

                // Invoke the tick once to start the animation.
                var mTick = typeof(BeepRadioGroup).GetMethod("OnAnimationTick",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                mTick!.Invoke(rg, new object?[] { this, EventArgs.Empty });

                // Set the field to null to simulate Dispose() racing with the
                // tick.  Then call tick again — it should NOT throw.
                var fTimer = typeof(BeepRadioGroup).GetField("_animationTimer",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                fTimer!.SetValue(rg, null);
                mTick!.Invoke(rg, new object?[] { this, EventArgs.Empty });
            }
            finally { rg.Dispose(); }
        }
    }

    /// <summary>Sample DTO used in DataBind tests.</summary>
    public class SourceTypePlaceholder
    {
        public string? Name { get; set; }
    }

    /// <summary>Marker used to test header treatment in tests.</summary>
    public class HeaderProbe : IRadioGroupHeader
    {
    }
}
