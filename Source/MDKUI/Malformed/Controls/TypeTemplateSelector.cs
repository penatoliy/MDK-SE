using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Malware.MDKUI.Malformed.Controls
{
    [ContentProperty("Selections")]
    public class TypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Default { get; set; }

        public ObservableCollection<DataTemplate> Selections { get; } = new ObservableCollection<DataTemplate>();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var type = item?.GetType();
            return Selections.FirstOrDefault(s => (s.DataType as Type)?.IsAssignableFrom(type) ?? false) ?? Default;
        }
    }
}
