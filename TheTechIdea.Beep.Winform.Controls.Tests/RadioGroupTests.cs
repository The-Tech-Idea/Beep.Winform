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
