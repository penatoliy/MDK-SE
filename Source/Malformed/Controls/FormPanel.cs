using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Malware.Malformed.Controls
{
    public class FormPanel : ItemsControl
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.RegisterAttached(
            "Label", typeof(object), typeof(FormPanel), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty LabelTemplateProperty = DependencyProperty.Register(
            nameof(LabelTemplate), typeof(DataTemplate), typeof(FormPanel), new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty ShowLabelProperty = DependencyProperty.RegisterAttached(
            "ShowLabel", typeof(bool), typeof(FormPanel), new PropertyMetadata(true));

        static FormPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormPanel), new FrameworkPropertyMetadata(typeof(FormPanel)));
        }

        public static void SetShowLabel(DependencyObject element, bool value)
        {
            element.SetValue(ShowLabelProperty, value);
        }

        public static bool GetShowLabel(DependencyObject element)
        {
            return (bool)element.GetValue(ShowLabelProperty);
        }

        public static void SetLabel(object element, object value)
        {
            ((DependencyObject)element).SetValue(LabelProperty, value);
        }

        public static object GetLabel(object element)
        {
            return ((DependencyObject)element).GetValue(LabelProperty);
        }

        public DataTemplate LabelTemplate
        {
            get => (DataTemplate)GetValue(LabelTemplateProperty);
            set => SetValue(LabelTemplateProperty, value);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var container = (FormPanelItem)element;
            var binding = new Binding
            {
                Source = item,
                Path = new PropertyPath(LabelProperty)
            };
            container.SetBinding(FormPanelItem.LabelProperty, binding);
            binding = new Binding
            {
                Source = item,
                Path = new PropertyPath(ShowLabelProperty)
            };
            container.SetBinding(FormPanelItem.ShowLabelProperty, binding);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            var container = (FormPanelItem)element;
            container.ClearValue(FormPanelItem.LabelProperty);
            container.ClearValue(FormPanelItem.ShowLabelProperty);
            base.ClearContainerForItemOverride(element, item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FormPanelItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item) => false;
    }
}
