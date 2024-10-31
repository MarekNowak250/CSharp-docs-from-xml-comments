namespace DescriptionGenerator.Core.Tags
{
    internal static class SubElementsProcessor
    {
        public static string Process(string data, string element)
        {
            // cut element name  + = char e.g. cref=
            var content = data.Substring(element.Length +1).TrimStart();
            if (data.EndsWith("/>"))
                // cut />
                content = content.Substring(0, content.Length - 2);
            else
                content = content.Split(">", 2)[0];

            return content.Trim();
        }
    }
}
