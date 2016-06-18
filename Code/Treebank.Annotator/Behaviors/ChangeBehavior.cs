namespace Treebank.Annotator.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public static class ChangeBehavior
    {
        public static DependencyProperty IsActiveProperty;
        public static DependencyProperty IsChangedProperty;
        public static DependencyProperty OriginalValueProperty;
        private static readonly Dictionary<Type, DependencyProperty> DefaultProperties;

        static ChangeBehavior()
        {
            IsActiveProperty = DependencyProperty.RegisterAttached("IsActive", typeof(bool), typeof(ChangeBehavior),
                new PropertyMetadata(false, OnIsActivePropertyChanged));
            IsChangedProperty = DependencyProperty.RegisterAttached("IsChanged", typeof(bool), typeof(ChangeBehavior),
                new PropertyMetadata(false));
            OriginalValueProperty = DependencyProperty.RegisterAttached("OriginalValue", typeof(object),
                typeof(ChangeBehavior), new PropertyMetadata(null));

            DefaultProperties = new Dictionary<Type, DependencyProperty>
            {
                {typeof(TextBox), TextBox.TextProperty}
            };
        }

        private static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DefaultProperties.ContainsKey(d.GetType()))
            {
                var defaultProperty = DefaultProperties[d.GetType()];
                if ((bool) e.NewValue)
                {
                    var binding = BindingOperations.GetBinding(d, defaultProperty);
                    if (binding != null)
                    {
                        var bindingPath = binding.Path.Path;
                        BindingOperations.SetBinding(d, IsChangedProperty, new Binding(bindingPath + "IsChanged"));
                        BindingOperations.SetBinding(d, OriginalValueProperty,
                            new Binding(bindingPath + "OriginalValue"));
                    }
                }
                else
                {
                    BindingOperations.ClearBinding(d, IsChangedProperty);
                    BindingOperations.ClearBinding(d, OriginalValueProperty);
                }
            }
        }

        public static bool GetIsActive(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsActiveProperty);
        }

        public static void SetIsActive(DependencyObject obj, bool value)
        {
            obj.SetValue(IsActiveProperty, value);
        }

        public static bool GetIsChanged(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsChangedProperty);
        }

        public static void SetIsChanged(DependencyObject obj, bool value)
        {
            obj.SetValue(IsChangedProperty, value);
        }

        public static bool GetOriginalValue(DependencyObject obj)
        {
            return (bool) obj.GetValue(OriginalValueProperty);
        }

        public static void SetOriginalValue(DependencyObject obj, object value)
        {
            obj.SetValue(OriginalValueProperty, value);
        }
    }
}