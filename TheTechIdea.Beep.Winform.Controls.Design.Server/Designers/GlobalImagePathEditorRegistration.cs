using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Global TypeDescriptionProvider that automatically applies the
    /// <see cref="BeepImagePathEditor"/> to ANY property named "ImagePath"
    /// of type <see cref="string"/> that doesn't already have its own
    /// <see cref="EditorAttribute"/>.
    /// 
    /// Registered at design time via ModuleInitializer — no code changes
    /// needed in Vis.Modules2.0, Controls, or any downstream project.
    /// 
    /// Covers:
    ///   - <see cref="SimpleItem"/>.ImagePath
    ///   - All <see cref="BaseControl"/> subclasses with ImagePath
    ///   - Any other type that exposes a string ImagePath property
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal static class GlobalImagePathEditorRegistration
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        internal static void Initialize()
        {
            var provider = new ImagePathTypeDescriptionProvider();

            // Register for SimpleItem (used in tree nodes, menus, list items, etc.)
            TypeDescriptor.AddProvider(provider, typeof(SimpleItem));

            // Register for BaseControl and all Control subclasses
            // (BeepImage, BeepButton, BeepLabel, etc.)
            TypeDescriptor.AddProvider(provider, typeof(TheTechIdea.Beep.Winform.Controls.Base.BaseControl));
        }
    }

    /// <summary>
    /// TypeDescriptionProvider that wraps properties named "ImagePath"
    /// of type string with the BeepImagePathEditor, unless the property
    /// already has an explicit [Editor] attribute.
    /// </summary>
    internal sealed class ImagePathTypeDescriptionProvider : TypeDescriptionProvider
    {
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            var parent = base.GetTypeDescriptor(objectType, instance);
            return new ImagePathTypeDescriptor(parent);
        }

        private sealed class ImagePathTypeDescriptor : CustomTypeDescriptor
        {
            public ImagePathTypeDescriptor(ICustomTypeDescriptor parent)
                : base(parent) { }

            public override PropertyDescriptorCollection GetProperties()
            {
                return GetProperties(null);
            }

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                var props = base.GetProperties(attributes);
                bool modified = false;
                var list = new List<PropertyDescriptor>(props.Count);

                foreach (PropertyDescriptor pd in props)
                {
                    if (ShouldEnhance(pd))
                    {
                        modified = true;
                        list.Add(new ImagePathPropertyDescriptor(pd));
                    }
                    else
                    {
                        list.Add(pd);
                    }
                }

                return modified
                    ? new PropertyDescriptorCollection(list.ToArray())
                    : props;
            }

            /// <summary>
            /// Returns true if the property is a string named "ImagePath"
            /// that does NOT already have an EditorAttribute.
            /// </summary>
            private static bool ShouldEnhance(PropertyDescriptor pd)
            {
                if (pd.PropertyType != typeof(string))
                    return false;

                if (!string.Equals(pd.Name, "ImagePath", StringComparison.Ordinal))
                    return false;

                // If already has an [Editor] attribute, don't override
                var existingEditor = pd.Attributes[typeof(EditorAttribute)];
                if (existingEditor != null)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Wraps an "ImagePath" property descriptor to add the
        /// BeepImagePathEditor UITypeEditor while preserving all
        /// original attributes.
        /// </summary>
        private sealed class ImagePathPropertyDescriptor : PropertyDescriptor
        {
            private readonly PropertyDescriptor _inner;

            public ImagePathPropertyDescriptor(PropertyDescriptor inner)
                : base(inner)
            {
                _inner = inner;
            }

            public override Type ComponentType => _inner.ComponentType;
            public override bool IsReadOnly => _inner.IsReadOnly;
            public override Type PropertyType => _inner.PropertyType;

            public override bool CanResetValue(object component)
                => _inner.CanResetValue(component);

            public override object GetValue(object component)
                => _inner.GetValue(component);

            public override void ResetValue(object component)
                => _inner.ResetValue(component);

            public override void SetValue(object component, object value)
                => _inner.SetValue(component, value);

            public override bool ShouldSerializeValue(object component)
                => _inner.ShouldSerializeValue(component);

            public override object GetEditor(Type editorBaseType)
            {
                if (editorBaseType == typeof(UITypeEditor))
                    return new BeepImagePathEditor();

                return _inner.GetEditor(editorBaseType);
            }

            protected override void FillAttributes(IList list)
            {
                // Copy all existing attributes from the inner descriptor
                foreach (Attribute attr in _inner.Attributes)
                    list.Add(attr);
            }
        }
    }
}
