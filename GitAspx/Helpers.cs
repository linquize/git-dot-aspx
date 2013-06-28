#region License

// Copyright 2010 Jeremy Skinner (http://www.jeremyskinner.co.uk)
//  
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://github.com/JeremySkinner/git-dot-aspx

#endregion

namespace GitAspx {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using GitAspx.Lib;

    public static class Helpers {
        static readonly string version, gitDllVersion, gitCoreDllVersion;

		static Helpers() {
			version = typeof(Helpers).Assembly.GetName().Version.ToString();
            gitDllVersion = typeof(GitSharp.Git).Assembly.GetName().Version.ToString();
            gitCoreDllVersion = typeof(GitSharp.Core.AnyObjectId).Assembly.GetName().Version.ToString();
		}

        #region Software Version
        public static string Version
        {
            get { return version; }
        }

        public static string GitDllVersion
        {
            get { return gitDllVersion; }
        }

        public static string GitCoreDllVersion
        {
            get { return gitCoreDllVersion; }
        }
        
        #endregion

        #region Get Url
        public static string GetGitUrl(this UrlHelper urlHelper, string project)
        {
            string[] lsaParts = project.Split('/');
            string lsCat = lsaParts.Length > 1 && lsaParts[0].Length > 0 ? lsaParts[0] : null;
            string lsSubCat = lsaParts.Length > 2 && lsaParts[1].Length > 0 ? lsaParts[1] : null;
            string lsProject = lsaParts.Length > 0 && lsaParts[lsaParts.Length - 1].Length > 0 ? lsaParts[lsaParts.Length - 1] : null;
            return urlHelper.RouteUrl("giturl", new RouteValueDictionary(new { cat = lsCat, subcat = lsSubCat, project = lsProject }),
                                      urlHelper.RequestContext.HttpContext.Request.Url.Scheme,
                                      urlHelper.RequestContext.HttpContext.Request.Url.Host);
        } 
        #endregion

        #region DateTime/DateTimeOffset
        public static string ToPrettyDateString(this DateTimeOffset d)
        {
            TimeSpan ts = DateTimeOffset.Now.Subtract(d);
            return PrettyDateCache.ToPrettyDateString(ts);
        }

        public static string ToPrettyDateString(this DateTime d)
        {
            TimeSpan ts = DateTime.Now.Subtract(d);
            return PrettyDateCache.ToPrettyDateString(ts);
        } 
        #endregion

		#region HttpReponseBase
		public static void WriteNoCache(this HttpResponseBase response) {
			response.AddHeader("Expires", "Fri, 01 Jan 1980 00:00:00 GMT");
			response.AddHeader("Pragma", "no-cache");
			response.AddHeader("Cache-Control", "no-cache, max-age=0, must-revalidate");
		}

		public static void PktWrite(this HttpResponseBase response, string input, params object[] args) {
			input = string.Format(input, args);
			var toWrite = (input.Length + 4).ToString("x").PadLeft(4, '0') + input;
			response.Write(toWrite);
		}

		public static void PktFlush(this HttpResponseBase response) {
			response.Write("0000");
		} 
	#endregion

        #region String
        public static string JoinLeft(this string[] values, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string value in values)
            {
                sb.Append(separator);
                sb.Append(value);
            }
            return sb.ToString();
        }

        public static string JoinRight(this string[] values, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string value in values)
            {
                sb.Append(value);
                sb.Append(separator);
            }
            return sb.ToString();
        }

        public static string Shorten(this string text, int maxLength)
        {
            if (text == null) return string.Empty;
            if (text.Length <= maxLength || maxLength <= 3) return text;
            return text.Substring(0, maxLength - 3) + "...";
        }

        public static IEnumerable<string> SplitLines(this string lines)
        {
            char b = '\0';
            int start = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                char c = lines[i];
                if (c == '\n')
                {
                    if (b == '\r')
                    {
                        start = i + 1;
                        continue;
                    }
                    else
                    {
                        yield return lines.Substring(start, i - start);
                        start = i + 1;
                    }
                }
                else if (c == '\r')
                {
                    yield return lines.Substring(start, i - start);
                    start = i + 1;
                }
                b = c;
            }

            yield return lines.Substring(start);
        }

        public static IEnumerable<string> SplitSlashes_OrEmpty(this string asText)
        {
            if (string.IsNullOrEmpty(asText))
                yield break;

            int i = 0, j = 0;
            if (asText[0] == '/')
                i = j = 1;
            while (i < asText.Length - 1)
            {
                if (asText[i] == '/')
                {
                    yield return asText.Substring(j, i - j);
                    j = i + 1;
                }
                i++;
            }

            if (asText[i] == '/')
                yield return asText.Substring(j, asText.Length - j - 1);
            else
                yield return asText.Substring(j);
        }

        public static string ToHtmlWithSpaces(this string asText)
        {
            return HttpContext.Current.Server.HtmlEncode(asText).Replace("  ", "&nbsp;&nbsp;").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
        }

        public static IEnumerable<TSource> OrderByStringNatural<TSource>(this IEnumerable<TSource> source, Func<TSource, string> keySelector)
        {
            return source.OrderBy(keySelector, StringNaturalComparer.Default);
        }

        class StringNaturalComparer : IComparer<string>
        {
            static StringNaturalComparer _default;
            public static StringNaturalComparer Default { get { return _default ?? (_default = new StringNaturalComparer()); } }
            public int Compare(string x, string y)
            {
                return CompareStringNatural(x, y);
            }
        }

        static int CompareStringNatural(string s1, string s2)
        {
            if (s1 == null && s2 == null) return 0;
            if (s1 == null) return -1;
            if (s2 == null) return 1;
            int x = 0;
            int y = 0;
            while (true)
            {
                if (x < s1.Length)
                {
                    if (y >= s2.Length)
                        return 1;
                }
                else if (y < s2.Length)
                    return -1;
                else
                    return 0;

                char a = s1[x];
                char b = s2[y];
                if (!(char.IsDigit(a) && char.IsDigit(b)))
                {
                    if (a > b)
                        return 1;
                    if (a < b)
                        return -1;
                    x++;
                    y++;
                }
                else
                {
                    bool zero = true;
                    int m = x;
                    while (m < s1.Length && char.IsDigit(s1[m]))
                    {
                        if (s1[m] == '0' && zero)
                            x++;
                        else
                            zero = false;
                        m++;
                    }

                    zero = true;
                    int n = y;
                    while (n < s2.Length && char.IsDigit(s2[n]))
                    {
                        if (s2[n] == '0' && zero)
                            y++;
                        else
                            zero = false;
                        n++;
                    }

                    if (m - x > n - y)
                        return 1;
                    else if (m - x < n - y)
                        return -1;

                    while (x < m)
                    {
                        if (s1[x] > s2[y])
                            return 1;
                        if (s1[x] < s2[y])
                            return -1;
                        x++;
                        y++;
                    }
                }
            }
        }
        #endregion

        #region Get Session
        public static WebBrowsingSettings GetWebBrowsingSettings(this Controller aoController)
        {
            WebBrowsingSettings loSettings = aoController.Session["WebBrowsingSettings"] as WebBrowsingSettings;
            if (loSettings == null)
                aoController.Session["WebBrowsingSettings"] = loSettings = new WebBrowsingSettings();
            return loSettings;
        }
        #endregion

        #region IEnumerable
        public static int CountWithLimit<T>(this IEnumerable<T> atqSource, int aiMaxCount)
        {
            int liCount = 0;
            foreach (var atItem in atqSource)
            {
                liCount++;
                if (liCount > aiMaxCount)
                    return liCount;
            }
            return liCount;
        }
        #endregion
	}
}