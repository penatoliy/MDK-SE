using System;
using System.Windows;
using System.Windows.Controls;

namespace Malware.MDKUI.Malformed.Controls
{
    public class FlexPanel : Panel
    {
        static readonly DependencyProperty MeasurementsProperty = DependencyProperty.RegisterAttached(
            "Measurements", typeof(Measurements), typeof(FlexPanel), new PropertyMetadata(default(Measurements)));

        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
            "Direction", typeof(FlexDirection), typeof(FlexPanel), new PropertyMetadata(default(FlexDirection), NotifyDirectionChanged));

        public static readonly DependencyProperty FlexProperty = DependencyProperty.RegisterAttached(
            "Flex", typeof(double?), typeof(FlexPanel), new PropertyMetadata(default(double?), NotifyFlexChanged));

        static void SetMeasurements(DependencyObject element, Measurements value)
        {
            element.SetValue(MeasurementsProperty, value);
        }

        static Measurements GetMeasurements(DependencyObject element)
        {
            return (Measurements)element.GetValue(MeasurementsProperty);
        }

        static void NotifyDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FlexPanel)d).OnDirectionChanged(e);
        }

        static void NotifyFlexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d.Up<FlexPanel>();
            panel?.OnChildFlexChanged(d, e);
        }

        public static void SetFlex(FrameworkElement element, double? value)
        {
            element.SetValue(FlexProperty, value);
        }

        public static double? GetFlex(FrameworkElement element)
        {
            return (double?)element.GetValue(FlexProperty);
        }

        static IMeasurer GetFetcher(FlexDirection direction) => direction == FlexDirection.Horizontal ? HorizontalMeasurer.Default : VerticalMeasurer.Default;

        public FlexDirection Direction
        {
            get => (FlexDirection)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        protected virtual void OnDirectionChanged(DependencyPropertyChangedEventArgs e)
        { }

        protected virtual void OnChildFlexChanged(DependencyObject child, DependencyPropertyChangedEventArgs e)
        {
            InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var measurer = GetFetcher(Direction);
            var fixedWidth = 0.0;
            var height = 0.0;
            var innerSize = measurer.Translate(availableSize);
            foreach (var child in Children)
            {
                var measurements = measurer.Measure(child, new Size(double.MaxValue, innerSize.Height));
                fixedWidth += Math.Max(measurements.MinWidth, measurements.Width);
                height = Math.Max(height, measurements.Height);
            }


            return measurer.Translate(new Size(Math.Max(fixedWidth, innerSize.Width), Math.Max(height, innerSize.Height)));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var measurer = GetFetcher(Direction);
            var fixedWidth = 0.0;
            var maxHeight = 0.0;
            var flex = 0.0;
            var highestFlexIndex = 0;
            var innerSize = measurer.Translate(finalSize);
            for (var index = 0; index < Children.Count; index++)
            {
                var child = Children[index];
                var measurements = measurer.GetMeasurements(child);
                if (measurements.Flex != null)
                    fixedWidth += measurements.MinWidth;
                else
                    fixedWidth += measurements.Width;
                maxHeight = Math.Max(Math.Max(maxHeight, measurements.Height), innerSize.Height);
                if (measurements.Flex != null)
                    highestFlexIndex = index;
                flex += measurements.Flex ?? 0;
            }
            var factor = 1.0 / flex;
            var halfMaxHeight = maxHeight / 2.0;

            var flexWidth = innerSize.Width - fixedWidth;
            var remainingWidth = flexWidth;
            var left = 0.0;
            var top = 0.0;
            for (var index = 0; index < Children.Count; index++)
            {
                var child = Children[index];
                var measurements = measurer.GetMeasurements(child);
                double width;
                if (measurements.Flex != null)
                {
                    if (highestFlexIndex == index)
                        width = remainingWidth;
                    else
                    {
                        width = Math.Floor(measurements.Flex.Value * factor * flexWidth);
                        width = Math.Min(measurements.MaxWidth, Math.Max(measurements.MinWidth, width));
                    }
                    remainingWidth -= width;
                }
                else
                    width = measurements.Width;

                double height;
                switch (measurements.VerticalAlignment)
                {
                    case FlexAlignment.Start:
                        top = 0;
                        height = measurements.Height;
                        break;

                    case FlexAlignment.Middle:
                        top = halfMaxHeight - (measurements.Height / 2.0);
                        height = measurements.Height;
                        break;

                    case FlexAlignment.End:
                        top = maxHeight - top;
                        height = measurements.Height;
                        break;

                    case FlexAlignment.Stretch:
                        top = 0;
                        height = Math.Max(measurements.Height, maxHeight);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                measurer.Arrange(child, new Rect(left, top, width, height));
                left += width;
            }

            return finalSize;
        }

        interface IMeasurer
        {
            Measurements Measure(object child, Size availableSize);
            Measurements GetMeasurements(object child);
            void Arrange(object child, Rect rect);
            Size Translate(Size size);
        }

        class HorizontalMeasurer : IMeasurer
        {
            public static readonly IMeasurer Default = new HorizontalMeasurer();

            public Measurements Measure(object child, Size availableSize)
            {
                var element = (FrameworkElement)child;
                element.Measure(availableSize);
                var desiredSize = element.DesiredSize;
                var measurements = new Measurements
                {
                    Width = desiredSize.Width,
                    MinWidth = element.MinWidth,
                    MaxWidth = element.MaxWidth,
                    Height = desiredSize.Height,
                    HorizontalAlignment = (FlexAlignment)element.HorizontalAlignment,
                    VerticalAlignment = (FlexAlignment)element.VerticalAlignment,
                    Flex = GetFlex(element)
                };
                SetMeasurements(element, measurements);
                return measurements;
            }

            public Measurements GetMeasurements(object child)
            {
                var element = (FrameworkElement)child;
                return FlexPanel.GetMeasurements(element);
            }

            public void Arrange(object child, Rect rect)
            {
                var element = (FrameworkElement)child;
                element.Arrange(rect);
            }

            public Size Translate(Size size) => size;
        }
        class VerticalMeasurer : IMeasurer
        {
            public static readonly IMeasurer Default = new VerticalMeasurer();

            public Measurements Measure(object child, Size availableSize)
            {
                var element = (FrameworkElement)child;
                element.Measure(availableSize);
                var desiredSize = new Size(element.DesiredSize.Height, element.DesiredSize.Width);
                var measurements = new Measurements
                {
                    Width = desiredSize.Height,
                    MaxWidth = element.MaxHeight,
                    Height = desiredSize.Width,
                    HorizontalAlignment = (FlexAlignment)element.VerticalAlignment,
                    VerticalAlignment = (FlexAlignment)element.HorizontalAlignment,
                    Flex = GetFlex(element)
                };
                SetMeasurements(element, measurements);
                return measurements;
            }

            public Measurements GetMeasurements(object child)
            {
                var element = (FrameworkElement)child;
                return FlexPanel.GetMeasurements(element);
            }

            public void Arrange(object child, Rect rect)
            {
                var element = (FrameworkElement)child;
                element.Arrange(new Rect(rect.Top, rect.Left, rect.Height, rect.Width));
            }

            public Size Translate(Size size) => new Size(size.Height, size.Width);
        }

        struct Measurements
        {
            public double Width;
            public double MinWidth;
            public double MaxWidth;
            public double Height;
            public double? Flex;
            public FlexAlignment HorizontalAlignment { get; set; }
            public FlexAlignment VerticalAlignment { get; set; }
        }
    }
}
