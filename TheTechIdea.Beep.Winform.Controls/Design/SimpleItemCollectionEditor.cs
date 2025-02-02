using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Desktop.Common;

namespace TheTechIdea.Beep.Winform.Controls.Design
{
    public class SimpleItemCollectionEditor : CollectionEditor
    {
        public SimpleItemCollectionEditor(Type type) : base(type) { }

        protected override object SetItems(object editValue, object[] value)
        {
            if (editValue is FullySuppressedObservableCollection<SimpleItem> existingList)
            {
                bool isDesignMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime;
                if (!isDesignMode)
                    existingList.BeginUpdate();
                var newItems = value.OfType<SimpleItem>().ToList();
                bool hadChanges = false;

                // ✅ Update existing items
                foreach (var newItem in newItems)
                {
                    var existingItem = existingList.FirstOrDefault(i => i.GuidId == newItem.GuidId);
                    if (existingItem != null)
                    {
                        if (!existingItem.Equals(newItem))
                        {
                            UpdateSimpleItem(existingItem, newItem);
                            hadChanges = true;
                        }
                    }
                    else
                    {
                        existingList.Add(newItem);
                        hadChanges = true;
                    }
                }

                // ✅ Remove items that no longer exist
                for (int i = 0; i < existingList.Count; i++)
                {
                    if (!newItems.Any(n => n.GuidId == existingList[i].GuidId))
                    {
                        existingList.RemoveAt(i);
                        i--;
                        hadChanges = true;
                    }
                }

                if (!isDesignMode)
                    existingList.EndUpdate();
                else
                {
                    // In design mode, ensure notifications are fired for each change.
                    // This may already happen if you're adding items individually.
                }
                return existingList;
            }

            return base.SetItems(editValue, value);
        }

        /// <summary>
        /// Updates an existing SimpleItem with new values
        /// </summary>
        private void UpdateSimpleItem(SimpleItem existingItem, SimpleItem newItem)
        {
            existingItem.Text = newItem.Text;
            existingItem.Name = newItem.Name;
            existingItem.MenuName = newItem.MenuName;
            existingItem.ImagePath = newItem.ImagePath;
            existingItem.DisplayField = newItem.DisplayField;
            existingItem.ValueField = newItem.ValueField;
            existingItem.ParentItem = newItem.ParentItem;
            existingItem.Value = newItem.Value;
            existingItem.MenuID = newItem.MenuID;
            existingItem.ActionID = newItem.ActionID;
            existingItem.ReferenceID = newItem.ReferenceID;
            existingItem.ParentID = newItem.ParentID;
            existingItem.OwnerReferenceID = newItem.OwnerReferenceID;
            existingItem.OtherReferenceID = newItem.OtherReferenceID;
            existingItem.PointType = newItem.PointType;
            existingItem.ObjectType = newItem.ObjectType;
            existingItem.BranchClass = newItem.BranchClass;
            existingItem.BranchName = newItem.BranchName;
            existingItem.BranchType = newItem.BranchType;
            existingItem.MethodName = newItem.MethodName;
            existingItem.ItemType = newItem.ItemType;
            existingItem.Category = newItem.Category;
            existingItem.Uri = newItem.Uri;
            existingItem.KeyCombination = newItem.KeyCombination;
            existingItem.AssemblyClassDefinitionID = newItem.AssemblyClassDefinitionID;
            existingItem.ClassDefinitionID = newItem.ClassDefinitionID;
            existingItem.PackageName = newItem.PackageName;
            existingItem.BranchID = newItem.BranchID;
            existingItem.X = newItem.X;
            existingItem.Y = newItem.Y;
            existingItem.Width = newItem.Width;
            existingItem.Height = newItem.Height;
            existingItem.ContainerGuidID = newItem.ContainerGuidID;
            existingItem.ContainerID = newItem.ContainerID;
            existingItem.RootContainerGuidID = newItem.RootContainerGuidID;
            existingItem.RootContainerID = newItem.RootContainerID;
            existingItem.IsDrawn = newItem.IsDrawn;
            existingItem.ComposedID = newItem.ComposedID;

            // ✅ Update Children recursively
            existingItem.Children.Clear();
            foreach (var child in newItem.Children)
            {
                existingItem.Children.Add(child);
            }
        }

        protected override object CreateInstance(Type itemType)
        {
            return new SimpleItem
            {
                GuidId = Guid.NewGuid().ToString(),
                Text = "New Item"
            };
        }
    }
}
