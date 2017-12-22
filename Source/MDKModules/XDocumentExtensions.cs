using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Malware.MDKModules
{
    /// <summary>
    /// Utility extensions for XDocument
    /// </summary>
    public static class XDocumentExtensions
    {
        /// <summary>
        /// Gets the element at the given subpath
        /// </summary>
        /// <param name="document"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XElement Element(this XDocument document, params XName[] path)
        {
            return document?.Root.Element(path);
        }

        /// <summary>
        /// Gets the element at the given subpath
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XElement Element(this XElement parent, params XName[] path)
        {
            if (parent == null)
                return null;
            for (var i = 0; i < path.Length; i++)
            {
                parent = parent.Element(path[i]);
                if (parent == null)
                    return null;
            }
            return parent;
        }

        /// <summary>
        /// Gets the elements at the given subpath
        /// </summary>
        /// <param name="document"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<XElement> Elements(this XDocument document, params XName[] path)
        {
            return document?.Root.Elements(path);
        }

        /// <summary>
        /// Gets the elements at the given subpath
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<XElement> Elements(this XElement parent, params XName[] path)
        {
            if (parent == null || path.Length == 0)
                return null;
            for (var i = 0; i < path.Length - 1; i++)
            {
                parent = parent.Element(path[i]);
                if (parent == null)
                    return null;
            }
            return parent.Elements(path.Last());
        }

        /// <summary>
        /// If an element exists, updates its value. If not, adds a new element.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XElement AddOrUpdateElement(this XElement parent, XName name, string value)
        {
            var element = parent.Element(name);
            if (element == null)
            {
                element = new XElement(name);
                parent.Add(name);
            }
            element.Value = value;
            return element;
        }

        /// <summary>
        /// If an attribute exists, updates its value. If not, adds a new attribute.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XAttribute AddOrUpdateAttribute(this XElement parent, XName name, string value)
        {
            var attribute = parent.Attribute(name);
            if (attribute == null)
            {
                attribute = new XAttribute(name, value);
                parent.Add(name);
                return attribute;
            }
            attribute.Value = value;
            return attribute;
        }

        /// <summary>
        /// Returns the value of the given element as an SByte
        /// </summary>
        /// <param name="element"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static sbyte? AsSByte(this XElement element, CultureInfo culture = null)
        {
            if (element == null)
                return null;
            if (!sbyte.TryParse(element.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// returns the value of the given attribute as an SByte
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static sbyte? AsSByte(this XAttribute attribute, CultureInfo culture = null)
        {
            if (attribute == null)
                return null;
            if (!sbyte.TryParse(attribute.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given element as a Byte
        /// </summary>
        /// <param name="element"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static byte? AsByte(this XElement element, CultureInfo culture = null)
        {
            if (element == null)
                return null;
            if (!byte.TryParse(element.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given attribute as a Byte
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static byte? AsByte(this XAttribute attribute, CultureInfo culture = null)
        {
            if (attribute == null)
                return null;
            if (!byte.TryParse(attribute.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given element as a UInt16
        /// </summary>
        /// <param name="element"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static ushort? AsUInt16(this XElement element, CultureInfo culture = null)
        {
            if (element == null)
                return null;
            if (!ushort.TryParse(element.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given attribute as a UInt16
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static ushort? AsUInt16(this XAttribute attribute, CultureInfo culture = null)
        {
            if (attribute == null)
                return null;
            if (!ushort.TryParse(attribute.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given element as an Int16
        /// </summary>
        /// <param name="element"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static short? AsInt16(this XElement element, CultureInfo culture = null)
        {
            if (element == null)
                return null;
            if (!short.TryParse(element.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given attribute as an Int16
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static short? AsInt16(this XAttribute attribute, CultureInfo culture = null)
        {
            if (attribute == null)
                return null;
            if (!short.TryParse(attribute.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given element as a UInt32
        /// </summary>
        /// <param name="element"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static uint? AsUInt32(this XElement element, CultureInfo culture = null)
        {
            if (element == null)
                return null;
            if (!uint.TryParse(element.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given attribute as a UInt32
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static uint? AsUInt32(this XAttribute attribute, CultureInfo culture = null)
        {
            if (attribute == null)
                return null;
            if (!uint.TryParse(attribute.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given element as an Int32
        /// </summary>
        /// <param name="element"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static int? AsInt32(this XElement element, CultureInfo culture = null)
        {
            if (element == null)
                return null;
            if (!int.TryParse(element.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given attribute as an Int32
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static int? AsInt32(this XAttribute attribute, CultureInfo culture = null)
        {
            if (attribute == null)
                return null;
            if (!int.TryParse(attribute.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given element as a UInt64
        /// </summary>
        /// <param name="element"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static ulong? AsUInt64(this XElement element, CultureInfo culture = null)
        {
            if (element == null)
                return null;
            if (!ulong.TryParse(element.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given attribute as a UInt64
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static ulong? AsUInt64(this XAttribute attribute, CultureInfo culture = null)
        {
            if (attribute == null)
                return null;
            if (!ulong.TryParse(attribute.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given element as an Int64
        /// </summary>
        /// <param name="element"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static long? AsInt64(this XElement element, CultureInfo culture = null)
        {
            if (element == null)
                return null;
            if (!long.TryParse(element.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given attribute as an Int64
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static long? AsInt64(this XAttribute attribute, CultureInfo culture = null)
        {
            if (attribute == null)
                return null;
            if (!long.TryParse(attribute.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given element as a Single
        /// </summary>
        /// <param name="element"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static float? AsSingle(this XElement element, CultureInfo culture = null)
        {
            if (element == null)
                return null;
            if (!float.TryParse(element.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given attribute as a Single
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static float? AsSingle(this XAttribute attribute, CultureInfo culture = null)
        {
            if (attribute == null)
                return null;
            if (!float.TryParse(attribute.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given element as a Double
        /// </summary>
        /// <param name="element"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static double? AsDouble(this XElement element, CultureInfo culture = null)
        {
            if (element == null)
                return null;
            if (!double.TryParse(element.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given attribute as a Double
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static double? AsDouble(this XAttribute attribute, CultureInfo culture = null)
        {
            if (attribute == null)
                return null;
            if (!double.TryParse(attribute.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given element as a Decimal
        /// </summary>
        /// <param name="element"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static decimal? AsDecimal(this XElement element, CultureInfo culture = null)
        {
            if (element == null)
                return null;
            if (!decimal.TryParse(element.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given attribute as a Decimal
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static decimal? AsDecimal(this XAttribute attribute, CultureInfo culture = null)
        {
            if (attribute == null)
                return null;
            if (!decimal.TryParse(attribute.Value, NumberStyles.Any, culture ?? CultureInfo.InvariantCulture, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given element as a Guid
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Guid? AsGuid(this XElement element)
        {
            if (element == null)
                return null;
            if (!Guid.TryParse(element.Value, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given attribute as a Guid
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static Guid? AsGuid(this XAttribute attribute)
        {
            if (attribute == null)
                return null;
            if (!Guid.TryParse(attribute.Value, out var value))
                return null;
            return value;
        }

        /// <summary>
        /// Returns the value of the given element as a string
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string AsString(this XElement element)
        {
            return element?.Value;
        }

        /// <summary>
        /// Returns the value of the given attribute as a string
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string AsString(this XAttribute attribute)
        {
            return attribute?.Value;
        }

        /// <summary>
        /// Returns the value of the given element as a Boolean
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool? AsBoolean(this XElement element)
        {
            var value = element?.Value;
            switch (value?.ToUpperInvariant().Trim())
            {
                case null:
                    return null;

                case "":
                case "FALSE":
                case "NO":
                case "0":
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Returns the value of the given attribute as a Boolean
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static bool? AsBoolean(this XAttribute attribute)
        {
            var value = attribute?.Value;
            switch (value?.ToUpperInvariant().Trim())
            {
                case null:
                    return null;

                case "":
                case "FALSE":
                case "NO":
                case "0":
                    return false;
                default:
                    return true;
            }
        }
    }
}
