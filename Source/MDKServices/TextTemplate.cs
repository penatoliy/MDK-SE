using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Malware.MDKServices
{
    class TextTemplate
    {
        static readonly Regex MacroMatch = new Regex(@"<%\s?(?<key>(?:(?!%>).*?))\s*%>", RegexOptions.Compiled);
        readonly List<Segment> _segments = new List<Segment>();

        public TextTemplate(string source)
        {
            source = source ?? "";
            var matches = MacroMatch.Matches(source);
            var start = 0;
            foreach (Match match in matches)
            {
                var macroStart = match.Index;
                if (start < macroStart)
                    _segments.Add(new TextSegment(source.Substring(start, macroStart - start)));
                var startOfLineIndex = FindStartOfLine(source, macroStart);
                var indentLength = macroStart - startOfLineIndex;
                _segments.Add(new Macro(match.Groups["key"].Value, indentLength));
                start = match.Index + match.Length;
            }
            if (start < source.Length)
                _segments.Add(new TextSegment(source.Substring(start, source.Length - start)));
        }

        public string Apply(IDictionary<string, string> values)
        {
            return Apply(key =>
            {
                values.TryGetValue(key, out var value);
                return value;
            });
        }

        public string Apply(Func<string, string> resolveMacro)
        {
            var writer = new StringWriter();
            foreach (var segment in _segments)
                segment.Write(writer, resolveMacro);
            writer.Flush();
            return writer.ToString();
        }

        int FindStartOfLine(string source, int matchIndex)
        {
            for (var i = matchIndex; i >= 0; i--)
                if (source[i] == '\n')
                    return i + 1;
            return 0;
        }

        abstract class Segment
        {
            public abstract void Write(StringWriter writer, Func<string, string> resolveMacro);
        }

        class TextSegment : Segment
        {
            string _content;

            public TextSegment(string content)
            {
                _content = content;
            }

            public override void Write(StringWriter writer, Func<string, string> resolveMacro)
            {
                writer.Write(_content);
            }
        }

        class Macro : Segment
        {
            static readonly string[] Newlines = { "\r\n", "\n", "\r" };
            readonly string _key;
            string _indent;

            public Macro(string key, int indentLength)
            {
                _key = key;
                _indent = Environment.NewLine + (indentLength > 0 ? new string(' ', indentLength) : string.Empty);
            }

            public override void Write(StringWriter writer, Func<string, string> resolveMacro)
            {
                var value = resolveMacro(_key) ?? string.Empty;
                var newline = value.IndexOf('\n');
                if (newline >= 0)
                {
                    var lines = value.Split(Newlines, StringSplitOptions.None);
                    value = string.Join(_indent, lines);
                }
                writer.Write(value);
            }
        }
    }
}