namespace DescriptionGenerator.Core
{
    public class MDPrinter
    {
        public string Print(IDataContainer handler)
        {
            var content = string.Format("# {0} {1}\n", handler.Name, handler.GetType());
            if (!string.IsNullOrWhiteSpace(handler.Description))
            {
                content += "\n## Description\n";
                content += string.Format("\n{0}\n", handler.Description);
            }

            if (handler.GetType() == "class")
                return content + PrintClass(handler);
            return content + PrintEnum(handler);
        }

        private string PrintClass(IDataContainer handler)
        {
            string content = string.Empty;

            content += string.Format("\n| Parameter | Description | Type |");
            content += string.Format("\n| :- | :- | :- |\n");
            content += string.Format("{0}", string.Join("\n", handler.Properties.Select(x => $"| {x.Name} | {x.Type} | {x.Description} |")));

            return content;
        }

        private string PrintEnum(IDataContainer handler)
        {
            string content = string.Empty;

            content += string.Format("\n| Value | Name | Description |");
            content += string.Format("\n| :- | :- | :- |\n");
            content += string.Format("{0}", string.Join("\n", handler.Properties.Select(x => $"| {x.Type} | {x.Name} | {x.Description} |")));

            return content;
        }
    }
}