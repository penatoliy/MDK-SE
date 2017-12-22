using System;
using System.Windows;

namespace Malware.Malformed.Controls
{
    public class TemplateSelection : DependencyObject
    {
        public Type Type { get; set; }
        public DataTemplate DataTemplate { get; set; }
    }
}