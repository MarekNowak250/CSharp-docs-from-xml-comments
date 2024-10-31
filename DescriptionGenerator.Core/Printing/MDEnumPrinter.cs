using DescriptionGenerator.Core.Interfaces;

namespace DescriptionGenerator.Core.Printing
{
    internal class MDEnumPrinter : MDContainerPrinter
    {
        public MDEnumPrinter(IDataContainer container) : base(container)
        {
        }

        public override string GetTable()
        {
            string content = string.Empty;

            content += string.Format("\n| Value | Name | Description |");
            content += string.Format("\n| :- | :- | :- |\n");
            content += string.Format("{0}", string.Join("\n", container.Properties.Select(x => $"| {x.Type} | {x.Name} | {x.Description} |")));

            return content;
        }
    }
}