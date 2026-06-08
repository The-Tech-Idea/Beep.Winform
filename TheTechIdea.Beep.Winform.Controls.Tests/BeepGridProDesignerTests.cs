using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TheTechIdea.Beep.Winform.Controls.GridX;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class BeepGridProDesignerTests
    {
        [Fact]
        public void DesignServerAssembly_CanBeLoaded()
        {
            var testAsm = Assembly.GetExecutingAssembly();
            var testDir = Path.GetDirectoryName(testAsm.Location);
            Assert.NotNull(testDir);

            var designServerPath = Path.Combine(testDir!, "TheTechIdea.Beep.Winform.Controls.Design.Server.dll");
            Assert.True(File.Exists(designServerPath), $"Design.Server assembly not found at {designServerPath}");

            // Just verify it can be loaded; don't enumerate types (they may fail to load)
            Assembly? asm = null;
            var ex = Record.Exception(() => asm = Assembly.LoadFrom(designServerPath));
            Assert.Null(ex);
            Assert.NotNull(asm);
        }

        [Fact]
        public void BeepGridPro_HasPropertyItemsForDesigner()
        {
            // Verify all properties the designer action list references exist on the grid
            var propNames = typeof(BeepGridPro).GetProperties()
                .Select(p => p.Name)
                .ToHashSet(StringComparer.Ordinal);

            // Properties referenced by BeepGridProActionList
            var designerProperties = new[]
            {
                "DataSource", "DataMember", "RowHeight", "ColumnHeaderHeight",
                "ShowColumnHeaders", "ShowNavigator", "AutoSizeColumnsMode",
                "AutoSizeTriggerMode", "AutoSizeDebounceMilliseconds", "AutoSizeRowsToContent",
                "RowAutoSizePadding", "UseDpiAwareRowHeights", "ReadOnly", "MultiSelect",
                "AllowUserToResizeColumns", "AllowColumnReorder", "GridStyle", "GridTitle",
                "NavigationStyle", "LayoutPreset", "ShowCheckBox", "ShowTopFilterPanel",
                "TopFilterPanelHeight", "SortIconVisibility", "FilterIconVisibility",
                "UseDedicatedFocusedRowStyle", "ShowFocusedCellFill", "ShowFocusedCellBorder",
                "FocusedCellBorderWidth"
            };

            foreach (var prop in designerProperties)
            {
                Assert.Contains(prop, propNames);
            }
        }

        [Fact]
        public void BeepGridPro_HasPublicSetterForEachDesignerProperty()
        {
            // Verify the designer properties are settable (have public setters)
            var designerProperties = new[]
            {
                "DataSource", "DataMember", "RowHeight", "ColumnHeaderHeight",
                "ShowColumnHeaders", "ShowNavigator", "AutoSizeColumnsMode",
                "AutoSizeTriggerMode", "AutoSizeDebounceMilliseconds", "AutoSizeRowsToContent",
                "ReadOnly", "MultiSelect", "AllowUserToResizeColumns", "AllowColumnReorder",
                "GridStyle", "GridTitle", "NavigationStyle", "LayoutPreset", "ShowCheckBox",
                "ShowTopFilterPanel", "SortIconVisibility", "FilterIconVisibility"
            };

            foreach (var propName in designerProperties)
            {
                var prop = typeof(BeepGridPro).GetProperty(propName);
                Assert.NotNull(prop);
                Assert.True(prop!.CanWrite, $"Property {propName} must be writable");
            }
        }
    }
}
