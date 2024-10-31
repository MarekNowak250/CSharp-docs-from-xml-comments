﻿using System.Xml;

namespace DescriptionGenerator.Core.Tags
{
    internal static class SeeTagProcessor
    {
        public static void ProcessSeeTag(this XmlNode seeTag)
        {
            if (seeTag.Name != "see")
                throw new ArgumentException("Cannot process - not a see node!");

            seeTag.InnerText = GetInnerText(seeTag);
        }

        private static string GetInnerText(XmlNode seeNode)
        {
            var xmlString = seeNode.OuterXml;
            // cut <see
            var content = xmlString.Substring(4).TrimStart();
            switch (content)
            {
                case string s when s.StartsWith("cref"):
                    content = SubElementsProcessor.Process(content, "cref");
                    break;
                case string s when s.StartsWith("href"):
                    content = SubElementsProcessor.Process(content, "href");
                    break;
                case string s when s.StartsWith("langword"):
                    return SubElementsProcessor.Process(content, "langword") + " ";
                    break;
                default:
                    return seeNode.InnerText;

            }

            if (!seeNode.OuterXml.EndsWith("</see>") && string.IsNullOrEmpty(seeNode.InnerText))
                seeNode.InnerText = content;

            return $"({content})[{seeNode.InnerText}] ";
        }
    }
}
