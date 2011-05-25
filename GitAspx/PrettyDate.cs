using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace GitAspx
{
    class PrettyDateEntry
    {
        public DateElementType Element;
        public double Less;
        public string Format;
    }

    enum DateElementType
    {
        Second, Minute, Hour, Day, Week, Month, Year
    }

    static class PrettyDateCache
    {
        static Dictionary<string, List<PrettyDateEntry>> Cache = new Dictionary<string, List<PrettyDateEntry>>();
        static PrettyDateCache()
        {
            string lsPath = Path.Combine(HttpContext.Current.Server.MapPath("~"), "PrettyDate.xml");
            XmlDocument loXD = new XmlDocument();
            loXD.Load(lsPath);
            foreach (XmlElement loXE in loXD.DocumentElement.GetElementsByTagName("culture"))
            {
                var llstEntries = new List<PrettyDateEntry>();
                Cache.Add(loXE.GetAttribute("name"), llstEntries);
                foreach (XmlElement loXE2 in loXE.ChildNodes.Cast<XmlNode>().OfType<XmlElement>())
                {
                    try
                    {
                        llstEntries.Add(new PrettyDateEntry { 
                            Element = (DateElementType)Enum.Parse(typeof(DateElementType), loXE2.Name, true), 
                            Less = double.Parse(loXE2.GetAttribute("less")), Format = loXE2.GetAttribute("format") });
                    }
                    catch { }
                }
            }
        }

        public static string ToPrettyDateString(this TimeSpan aoTimeSpan)
        {
            List<PrettyDateEntry> llstEntries;
            if (!Cache.TryGetValue(CultureInfo.CurrentCulture.Name, out llstEntries))
                llstEntries = Cache[""];

            foreach (var item in llstEntries)
	        {
                if (item.Element == DateElementType.Second)
                {
                    if (aoTimeSpan.TotalSeconds < item.Less)
                        return item.Format.Replace("@", ((int)aoTimeSpan.TotalSeconds).ToString());
                }
                else if (item.Element == DateElementType.Minute)
                {
                    if (aoTimeSpan.TotalMinutes < item.Less)
                        return item.Format.Replace("@", ((int)aoTimeSpan.TotalMinutes).ToString());
                }
                else if (item.Element == DateElementType.Hour)
                {
                    if (aoTimeSpan.TotalHours < item.Less)
                        return item.Format.Replace("@", ((int)aoTimeSpan.TotalHours).ToString());
                }
                else if (item.Element == DateElementType.Day)
                {
                    if (aoTimeSpan.TotalDays < item.Less)
                        return item.Format.Replace("@", ((int)aoTimeSpan.TotalDays).ToString());
                }
                else if (item.Element == DateElementType.Week)
                {
                    if (aoTimeSpan.TotalDays / 7 < item.Less)
                        return item.Format.Replace("@", ((int)aoTimeSpan.TotalDays / 7).ToString());
                }
                else if (item.Element == DateElementType.Month)
                {
                    if (aoTimeSpan.TotalDays / 30 < item.Less)
                        return item.Format.Replace("@", ((int)aoTimeSpan.TotalDays / 30).ToString());
                }
                else if (item.Element == DateElementType.Year)
                {
                    if (aoTimeSpan.TotalDays / 365 < item.Less)
                        return item.Format.Replace("@", ((int)aoTimeSpan.TotalDays / 365).ToString());
                }
	        }

            return aoTimeSpan.ToString();
        }
    }
}