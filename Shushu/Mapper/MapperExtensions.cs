using System.Collections.Generic;

namespace Shushu
{
    public static class MapperExtensions
    {
        static readonly List<string> _tokens = new List<string>
            {
                "\\",
                "+",
                "-",
                "&&",
                "||",
                "!",
                "(",
                ")",
                "{",
                "}",
                "[",
                "]",
                "^",
                "\"",
                "~",
                "*",
                "?",
                ":",                
                "/"
            };

        public static string ToEscapedSearchString(this string text)
        {
            if (text == null)
                return string.Empty;

            foreach (var token in _tokens)
                text = text.Replace(token, @"\" + token);

            return text;
        }
    }
}
