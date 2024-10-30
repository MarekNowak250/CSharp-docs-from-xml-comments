namespace DescriptionGenerator.Core.Tags
{
    internal static class CrefProcessor
    {
        public static string ProcessCref(string crefString)
        {
            // cut cref=
            var content = crefString.Substring(5).TrimStart();
            if (crefString.EndsWith("/>"))
                // cut />
                content = content.Substring(0, content.Length - 2);
            else
                content = $"({content.Split(">", 2)[0].Trim()})";

            return content;
        }
    }
}
