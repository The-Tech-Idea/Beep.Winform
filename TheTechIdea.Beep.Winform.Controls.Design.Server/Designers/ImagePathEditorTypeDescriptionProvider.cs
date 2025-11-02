using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal sealed class ImagePathEditorTypeDescriptionProvider : TypeDescriptionProvider
    {
        private readonly TypeDescriptionProvider _parent;

        public ImagePathEditorTypeDescriptionProvider(TypeDescriptionProvider parent)
            : base(parent)
        {
            _parent = parent;
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            var baseDescriptor = _parent?.GetTypeDescriptor(objectType, instance) ?? base.GetTypeDescriptor(objectType, instance);
            return new ImagePathTypeDescriptor(baseDescriptor);
        }

        private sealed class ImagePathTypeDescriptor : CustomTypeDescriptor
        {
            public ImagePathTypeDescriptor(ICustomTypeDescriptor parent) : base(parent)
            {
            }

            public override PropertyDescriptorCollection GetProperties()
                => GetProperties(null);

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                var original = base.GetProperties(attributes);
                var list = new List<PropertyDescriptor>(original.Count);

                foreach (PropertyDescriptor property in original)
                {
                    if (IsImagePath(property))
                    {
                        list.Add(new ImagePathPropertyDescriptor(property));
                    }
                    else
                    {
                        list.Add(property);
                    }
                }

                return new PropertyDescriptorCollection(list.ToArray(), true);
            }

            private static bool IsImagePath(PropertyDescriptor property)
            {
                if (property == null || property.IsReadOnly)
                {
                    return false;
                }

                if (property.PropertyType != typeof(string))
                {
                    return false;
                }

                return string.Equals(property.Name, "ImagePath", StringComparison.Ordinal) ||
                       string.Equals(property.Name, "EmbeddedImagePath", StringComparison.Ordinal);
            }
        }

        private sealed class ImagePathPropertyDescriptor : PropertyDescriptor
        {
            private readonly PropertyDescriptor _inner;
            private Attribute[] _attributes;

            public ImagePathPropertyDescriptor(PropertyDescriptor inner)
                : base(inner)
            {
                _inner = inner;
            }

            public override bool CanResetValue(object component) => _inner.CanResetValue(component);
            public override Type ComponentType => _inner.ComponentType;
            public override object GetValue(object component) => _inner.GetValue(component);
            public override bool IsReadOnly => _inner.IsReadOnly;
            public override Type PropertyType => _inner.PropertyType;
            public override void ResetValue(object component) => _inner.ResetValue(component);
            public override void SetValue(object component, object value) => _inner.SetValue(component, value);
            public override bool ShouldSerializeValue(object component) => _inner.ShouldSerializeValue(component);

            public override AttributeCollection Attributes
            {
                get
                {
                    if (_attributes == null)
                    {
                        var editorAttribute = new EditorAttribute(typeof(BeepImagePathEditor), typeof(UITypeEditor));
                        var list = new List<Attribute>(_inner.Attributes.Count + 1);

                        foreach (Attribute attribute in _inner.Attributes)
                        {
                            if (attribute is EditorAttribute)
                            {
                                continue;
                            }

                            list.Add(attribute);
                        }

                        list.Add(editorAttribute);
                        _attributes = list.ToArray();
                    }

                    return new AttributeCollection(_attributes);
                }
            }
        }
    }
}
