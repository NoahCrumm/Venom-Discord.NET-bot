using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VenomBot.Services
{
    public static class ListExtentions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static bool IsFirst<T>(this List<T> items, int index)
        {
            return index == 0;
        }

        public static bool IsLast<T>(this List<T> items, int index)
        {
            return items.Count == index + 1;
        }

        public static string PadRight(this int i, int padding)
        {
            return i.ToString().PadRight(padding);
        }

        public static string DisplayName(this SocketUser u)
        {
            var user = u as SocketGuildUser;
            var name = (!string.IsNullOrEmpty(user.Nickname)) ? user.Nickname : user.Username;
            return name.FirstLetterToUpperCase().Sanitize();
        }

        public static string DisplayTime(this DateTime datetime)
        {
            var past = DateTime.Now - datetime;

            if (past.TotalSeconds < 1)
            {
                return "Now";
            }
            else if (past.TotalMinutes < 1)
            {
                return $"{past.Seconds} sec ago";
            }
            else if (past.TotalHours < 1)
            {
                return $"{past.Minutes} min {past.Seconds} sec ago";
            }
            else if (past.TotalDays < 1)
            {
                return $"{past.Hours} hr {past.Minutes} min {past.Seconds} sec ago";
            }
            else if (past.TotalDays < 365)
            {
                return $"{(int)past.TotalDays} d {past.Hours} hr {past.Minutes} min {past.Seconds} sec ago";
            }
            else if (past.TotalDays > 5000)
            {
                return $"Never";
            }
            else
            {
                return $"> 1 year ago";
            }
        }

        private static string p(int i)
        {
            return (i == 1) ? "" : "s";
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;

            if (value.Length <= maxLength)
            {
                return value;
            }
            else
            {
                var newstring = value.Substring(0, maxLength);
                return newstring + "...";
            }
        }
        public static string FirstLetterToUpperCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("There is no first letter");

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static List<string> SplitString(this string str, int chunkSize)
        {
            List<string> chunks = new List<string>();
            while (str.Length > chunkSize)
            {
                chunks.Add(str.Substring(0, chunkSize));
                str = str.Substring(chunkSize);
            }
            chunks.Add(str);
            return chunks;
        }

        public static string Sanitize(this string s)
        {
            s = s.Replace("'", "")
                .RemoveSpecialChars();

            return s;
        }

        public static string RemoveSpecialChars(this string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static IEnumerable<string> ChunksUpto(this string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        public static Color Random(this Color color)
        {
            Random rnd = new Random();
            return new Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
        }
    }
}
