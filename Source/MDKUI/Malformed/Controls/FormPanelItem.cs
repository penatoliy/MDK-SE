using System.Windows;
using System.Windows.Controls;

namespace Malware.MDKUI.Malformed.Controls
{
    public class FormPanelItem : ContentControl
    {
        public static readonly DependencyProperty LabelWidthProperty = DependencyProperty.Register(
            nameof(LabelWidth), typeof(GridLength), typeof(FormPanelItem), new PropertyMetadata(GridLength.Auto));

        public static readonly DependencyProperty ShowLabelProperty = DependencyProperty.Register(
            nameof(ShowLabel), typeof(bool), typeof(FormPanelItem), new PropertyMetadata(true));

        static FormPanelItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormPanelItem), new FrameworkPropertyMetadata(typeof(FormPanelItem)));
        }

        public static readonly DependencyProperty LabelTemplateProperty = DependencyProperty.Register(
            "LabelTemplate", typeof(DataTemplate), typeof(FormPanelItem), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate LabelTemplate
        {
            get => (DataTemplate)GetValue(LabelTemplateProperty);
            set => SetValue(LabelTemplateProperty, value);
        }

        public GridLength LabelWidth
        {
            get => (GridLength)GetValue(LabelWidthProperty);
            set => SetValue(LabelWidthProperty, value);
        }

        public bool ShowLabel
        {
            get => (bool)GetValue(ShowLabelProperty);
            set => SetValue(ShowLabelProperty, value);
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(object), typeof(FormPanelItem), new PropertyMetadata(default(object)));

        public object Label
        {
            get => (object)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }
    }
}
