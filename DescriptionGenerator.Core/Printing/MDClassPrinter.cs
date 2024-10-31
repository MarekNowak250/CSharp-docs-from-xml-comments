using DescriptionGenerator.Core.Interfaces;
using DescriptionGenerator.Core.Types;

namespace DescriptionGenerator.Core.Printing
{
    internal class MDClassPrinter : MDContainerPrinter
    {
        public MDClassPrinter(IDataContainer container) : base(container)
        {
        }

        public override string GetTable()
        {
            var groupedProps = GroupProperties();

            string content = string.Empty;

            content += string.Format("\n| Parameter | Description | Type |");
            content += string.Format("\n| :- | :- | :- |\n");
            if (groupedProps.TryGetValue("props", out var props))
                content += string.Format("{0}", string.Join("\n", props.Select(x => x.GetPrinter(PrinterType.Markdown).Print())));
            if (groupedProps.TryGetValue("methods", out var methods))
                content += string.Format("{0}", string.Join("\n", methods.Select(x => x.GetPrinter(PrinterType.Markdown).Print())));
            return content;
        }

        public Dictionary<string, List<StructElement>> GroupProperties()
        {
            var methods = new List<StructElement>();
            var props = new List<StructElement>();
            foreach (var prop in container.Properties)
            {
                if (prop.Type == "method")
                    methods.Add(prop);
                else
                    props.Add(prop);
            }

            return new Dictionary<string, List<StructElement>> { { "methods", methods }, { "props", props } };
        }
    }
}