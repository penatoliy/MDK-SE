using System;
using System.Windows;
using System.Windows.Media;

namespace Malware.MDKUI.Malformed
{
    public static class WpfExtensions
    {
        public static T Up<T>(this DependencyObject item) where T: DependencyObject
        {
            while (true)
            {
                if (item == null)
                    return null;

                //get parent item
                var parentObject = VisualTreeHelper.GetParent(item);

                //we've reached the end of the tree
                switch (parentObject)
                {
                    case null:
                        return null;
                    case T parent:
                        return parent;
                }

                //check if the parent matches the type we're looking for
                item = parentObject;
            }
        }

        public static DependencyObject Up(this DependencyObject item, Func<DependencyObject, bool> predicate)
        {
            while (true)
            {
                //we've reached the end of the tree
                if (item == null || predicate(item))
                    return item;

                //get parent item
                var parentObject = VisualTreeHelper.GetParent(item);

                //check if the parent matches the type we're looking for
                item = parentObject;
            }
        }
    }
}
