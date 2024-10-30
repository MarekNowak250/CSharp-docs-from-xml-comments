using System.Xml;

namespace DescriptionGenerator.Core.Tags
{
    internal static class SeeTagProcessor
    {
        public static void ProcessSeeTag(this XmlNode seeTag)
        {
            if (seeTag.Name != "see")
                throw new ArgumentException("Cannot process not see node!");

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
                    content = CrefProcessor.ProcessCref(content);
                    break;
                default:
                    return seeNode.InnerText;

            }

            if (seeNode.OuterXml.EndsWith("</see>"))
            {
                content = $"{content} {seeNode.InnerText}";
            }

            return content;
        }

        internal static class MemberProcessor
        {
            public static string ProcessCref(string crefString)
            {
                // cut cref=
                var content = crefString.Substring(5).TrimStart();
                if (crefString.EndsWith("/>"))
                    // cut />
                    content = content.Substring(0, content.Length - 2);

                return content;
            }
        }
    }
}
