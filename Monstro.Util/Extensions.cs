using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Globalization;

namespace System
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Build an IEnumerable containing this
        /// </summary>
        public static IEnumerable<T> ToEnumerable<T>(this T o)
        {
            yield return o;
        }

        public static bool In<T>(this T o, params T[] values)
        {
            if(values == null)
                return false;
            return values.Contains(o);
        }
    }

    public static class CharExtension
    {
        /// <summary>
        /// true if the char is a simple lettre (without accent), lower or upper case.
        /// </summary>
        public static bool IsLetter(this char c)
        {
            return c > 64 && c < 91 || c > 96 && c < 123;
        }

        public static bool IsDigit(this char c)
        {
            return c > 47 && c < 58;
        }
    }

    public static class DateTimeExtension
    {
        public static bool IsBetween(this DateTime d, DateTimeOffset start, DateTimeOffset end)
        {
            return d >= start && d <= end;
        }

        public static int ToUnixTimestamp(this DateTime d)
        {
            return new DateTimeOffset(d).ToUnixTimestamp();
        }
    }

    public static class DateTimeOffsetExtention
    {
        public static DateTime? GetLocalDateTimeOrNull(this DateTimeOffset? o)
        {
            return o == null ? (DateTime?)null : o.Value.LocalDateTime;
        }

        public static int ToUnixTimestamp(this DateTimeOffset o)
        {
            return (int)(o - Ext.TimestampToDate(0)).TotalSeconds;
        }
    }

    public static class StringExtensions
    {
        /// <summary>
        /// Encode the input string in HTML, including single and double-quotes.
        /// </summary>
        public static String Html(this String s)
        {
            if (String.IsNullOrEmpty(s))
                return "";
            return HttpUtility.HtmlEncode(s).Replace("\"", "&quot;").Replace("'", "&#039;");
        }
        public static String Truncate(this String s, int maxLength) { return s.Truncate(maxLength, false); }
        public static String Truncate(this String s, int maxLength, bool addEllipsis)
        {
            if (s.Length <= maxLength)
                return s;
            return addEllipsis && maxLength >= 3 ?
                s.Substring(0, maxLength - 3) + "..." :
                s.Substring(0, maxLength);
        }

        /// <summary>
        /// Trim the string and return null if it is empty
        /// </summary>
        public static String TrimOrNull(this String s)
        {
            if(s == null)
                return null;
            s = s.Trim();
            return s == "" ? null : s;
        }

        public static String[] Split(this String s, char separator)
        {
            return s.Split(new[] { separator });
        }

        public static String[] Split(this String s, char separator, int count)
        {
            return s.Split(new[] { separator }, count);
        }

        /// <summary>
        /// Split a String joined by String.JoinEscaped Extension method
        /// </summary>
        public static IEnumerable<String> SplitEscaped(this String s)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '|')
                {
                    yield return sb.ToString();
                    sb = new StringBuilder();
                    continue;
                }
                if (s[i] == '\\')
                {
                    i++;
                    if (i >= s.Length)
                        throw new Exception("The input string is invalid");
                }
                sb.Append(s[i]);
            }
            yield return sb.ToString();
        }

        private static Regex UrlRegex;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="detectUrls">If true, urls are detected and converted to HTML A with nofollow</param>
        /// <returns></returns>
        public static String FormatToHtml(this String s, bool detectUrls)
        {
            if (String.IsNullOrEmpty(s))
                return String.Empty;
            if (!detectUrls)
                return BasicFormatToHtml(s);

            //Detect urls
            if (UrlRegex == null)
                UrlRegex = new Regex(@"\b(?:(?:https?|ftp|file)://|www\.|ftp\.)[-A-Z0-9+&@#/%=~_|$?!:,.]*[A-Z0-9+&@#/%=~_|$]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var sb = new StringBuilder();
            int lastIndex = 0;
            foreach (var m in UrlRegex.Matches(s).Cast<Match>())
            {
                sb.Append(BasicFormatToHtml(s.Substring(lastIndex, m.Index - lastIndex)));
                sb.AppendFormat("<a href='{0}' rel='nofollow'>{1}</a>", m.Value.Html(), BasicFormatToHtml(m.Value.Truncate(50, true)));
                lastIndex = m.Index + m.Length;
            }
            sb.Append(BasicFormatToHtml(s.Substring(lastIndex)));

            return sb.ToString();
        }

        public static String FormatToHtml(this String s)
        {
            return FormatToHtml(s, false);
        }

        private static String BasicFormatToHtml(String s)
        {
            if (String.IsNullOrEmpty(s))
                return String.Empty;

            var sb = new StringBuilder();
            const int maxNl = 2;
            const int maxSpace = 4;
            const int maxW = 20;//Max word length
            int cmptNl = 0;
            int cmptSpace = 0;
            int cmptW = 0;

            foreach (char c in s)
            {
                switch (c)
                {
                    case '\r':
                        continue;
                    case '\t':
                    case ' ':
                        cmptW = 0;
                        if (!(cmptSpace < maxSpace))
                            continue;
                        sb.Append((cmptSpace > 0) ? "&nbsp;" : " ");
                        cmptSpace++;
                        continue;
                    case '\n':
                        cmptW = 0;
                        if (!(cmptNl < maxNl))
                            continue;
                        sb.Append("<br/>");
                        cmptNl++;
                        cmptSpace = 0;
                        continue;
                    default:
                        cmptSpace = 0;
                        cmptNl = 0;
                        if (cmptW >= maxW)
                        {
                            sb.Append("&shy;");//<wbr/> is not supported by ie8. &shy; is not supported by FF2.
                            cmptW = 1;
                        }
                        else
                        {
                            cmptW++;
                        }
                        switch (c)
                        {
                            case '<': sb.Append("&lt;");
                                continue;
                            case '>': sb.Append("&gt;");
                                continue;
                            case '&': sb.Append("&amp;");
                                continue;
                            case '"': sb.Append("&quot;");
                                continue;
                        }
                        if (c >= '\x00a0' && c < 'Ā')
                        {
                            sb.Append("&#").Append((int)c).Append(";");
                            continue;
                        }
                        sb.Append(c);
                        continue;
                }
            }
            return sb.ToString();
        }

        public static String Js(this String s)
        {
            return s.Js(false);
        }

        /// <summary>
        /// Encode the input string as a Javascript literal, do not quote it.
        /// </summary>
        public static String Js(this String s, bool strict)
        {
            if (string.IsNullOrEmpty(s))
                return String.Empty;

            int len = s.Length;
            var sb = new StringBuilder((int)(len * 1.1));
            for (int i = 0; i < len; i += 1)
            {
                char c = s[i];
                if ((c == '\\' || c == '"' || c == '/')
                    || (!strict && (c == '\'' || c == '>')))
                {
                    sb.Append('\\');
                    sb.Append(c);
                }
                else if (c == '\b')
                    sb.Append("\\b");
                else if (c == '\t')
                    sb.Append("\\t");
                else if (c == '\n')
                    sb.Append("\\n");
                else if (c == '\f')
                    sb.Append("\\f");
                else if (c == '\r')
                    sb.Append("\\r");
                else
                {
                    if (c < ' ')
                    {
                        sb.Append(String.Format("\\u{0:x4}", (int)c));
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// URL encode the input string, replace spaces with "%20".
        /// </summary>
        public static String Url(this String s)
        {
            if (String.IsNullOrEmpty(s))
                return s;
            var w = new StringBuilder();
            for (; ; )
            {
                int pos = s.IndexOf(' ');
                if (pos < 0)
                {
                    w.Append(HttpUtility.UrlEncode(s));
                    break;
                }
                w.Append(HttpUtility.UrlEncode(s.Substring(0, pos)));
                w.Append("%20");
                s = s.Substring(pos + 1);
            }
            return w.ToString();
        }

        public static bool IsNullOrEmpty(this String s)
        {
            return String.IsNullOrEmpty(s);
        }

        public static bool ContainsIgnoreCase(this String s, String value)
        {
            if (s == null)
                return false;
            return s.IndexOf(value, StringComparison.OrdinalIgnoreCase) > -1;

        }

        public static String Format(this String s, params object[] args)
        {
            return String.Format(s, args);
        }
    }


    public static class ExtInt
    {
        /// <summary>
        /// Return true if this is between inclusive bounds
        /// </summary>
        public static bool IsBetween(this int i, int min, int max)
        {
            return i < min && i <= max;
        }

        /// <summary>
        /// Force this in the given inclusive bounds
        /// </summary>
        public static int MinMax(this int i, int? min, int? max)
        {
            if (min != null && max != null && min > max)
                throw new ArgumentException("min > max");
            if (min != null && i < min)
                return min ?? 0;
            if (max != null && i > max)
                return max ?? 0;
            return i;
        }
    }


    public static class ExtDouble
    {
        /// <summary>
        /// Return true if this is between inclusive bounds
        /// </summary>
        public static bool IsBetween(this double i, double min, double max)
        {
            return i < min && i <= max;
        }

        public static String ToInvariantString(this double i)
        {
            return i.ToString(CultureInfo.InvariantCulture);
        }
    }
}

namespace System.Collections.Generic
{
    public static class ExtDictionary
    {
        /// <summary>
        /// Return the value for the given key.
        /// If key does not exists, return the default value for TValue
        /// </summary>
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            return dict.GetOrDefault(key, default(TValue));
        }

        /// <summary>
        /// Return the value for the given key.
        /// If key does not exists, return the given default value
        /// </summary>
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            if (key == null)
                return defaultValue;
            return dict.ContainsKey(key) ? dict[key] : defaultValue;
        }

        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> original)
        {
            var r = new Dictionary<TKey, TValue>(original.Count, original.Comparer);
            foreach (KeyValuePair<TKey, TValue> entry in original)
                r.Add(entry.Key, entry.Value);
            return r;
        }

    }

    public static class ExtIEnumerable
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> e)
        {
            return new HashSet<T>(e);
        }

        public static IList<T> Shuffle<T>(this IEnumerable<T> e)
        {
            var r = new Random();
            var a = e.ToArray();
            for (int i = a.Length -1; i > 0; i--)
            {
                int j = r.Next(0, i + 1);
                T t = a[j];
                a[j] = a[i];
                a[i] = t;
            }
            return a;
        }

        public static IEnumerable<T> NoNull<T>(this IEnumerable<T> e)
        {
            return e ?? Enumerable.Empty<T>();
        }

        public static String Join(this IEnumerable<String> e, String separator)
        {
            return String.Join(separator, e.ToArray());
        }

        /// <summary>
        /// Join with the waranty that the split result give you back the original strings
        /// Use IEnumerable&lt;String&gt;.SplitEscaped Extension method to split the returned string
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static String JoinEscaped(this IEnumerable<String> e)
        {
            return e
                .Select(s => s == null ? s : s.Replace(@"\", @"\\").Replace("|", @"\|"))
                .Join("|");
        }

        public static IEnumerable<T> ConcatValue<T>(this IEnumerable<T> e, T value)
        {
            foreach (T t in e)
                yield return t;
            yield return value;
        }

        /// <summary>
        /// If only a part of the range is available, return it.
        /// Return an empty enumerable if no item is available for the given range
        /// </summary>
        public static IEnumerable<T> TryGetRange<T>(this IEnumerable<T> e, int start, int length)
        {
            int i = -1;
            foreach (T t in e)
            {
                i++;
                if (i < start)
                    continue;
                if (i >= start + length)
                    yield break;
                yield return t;
            }
        }

        /// <summary>
        /// Return the index of the first element that satisfy the predicat.
        /// Return null if not found.
        /// </summary>
        public static int? IndexOf<T>(this IEnumerable<T> e, Func<T, bool> f)
        {
            int i = 0;
            foreach (var t in e)
            {
                if (f(t))
                    return i;
                i++;
            }
            return null;
        }

        public static IEnumerable<TSource> Distinct<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.Distinct(keySelector, EqualityComparer<TKey>.Default);
        }

        public static IEnumerable<TSource> Distinct<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (comparer == null) throw new ArgumentNullException("comparer");
            var knownKeys = new HashSet<TKey>(comparer);
            return source.Where(element => knownKeys.Add(keySelector(element)));
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> e, T value)
        {
            return e.Prepend(value.ToEnumerable());
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> e, IEnumerable<T> values)
        {
            return values.Concat(e);
        }
    }

    public static class ExtIList
    {
        public static T GetOrDefault<T>(this IList<T> e, int index)
        {
            if (e != null && e.Count > index)
                return e[index];
            return default(T);
        }

        /// <summary>
        /// return a random element of the list or default if list is empty
        /// </summary>
        public static T AnyOrDefault<T>(this IList<T> e)
        {
            if (e.Count < 1)
                return default(T);
            if (e.Count == 1)
                return e[0];
            return e[new Random().Next(0, e.Count)];
        }
    }

    public static class ExtControlCollection
    {
        public static void AddLiteral(this ControlCollection e, String html)
        {
            e.Add(new LiteralControl(html));
        }

        public static void AddLiteral(this ControlCollection e, String htmlPatern, params Object[] args)
        {
            e.AddLiteral(String.Format(htmlPatern, args));
        }
    }

    public static class Ext
    {
        public static IEnumerable<int> Range(int from, int to, int step)
        {
            if(step > 0)
            {
                for(int r = from; r <= to; r += step)
                    yield return r;
            }
            else if(step < 0)
            {
                for (int r = from; r >= to; r += step)
                    yield return r;
            }
            else
            {
                throw new ArgumentException("Must be != 0", "step");
            }
        }

        public static IEnumerable<int> Range(int from, int to)
        {
            return Range(from, to, from < to ? 1 : -1);
        }

        public static DateTimeOffset TimestampToDate(double timestamp)
        {
            return new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero)
                .AddSeconds(timestamp);
        }
    }
}