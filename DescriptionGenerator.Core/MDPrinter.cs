using DescriptionGenerator.Core.Interfaces;
using DescriptionGenerator.Core.Types;

namespace DescriptionGenerator.Core
{
    public enum PrinterType
    {
        Markdown,
    }

    internal class MDStructElementPrinter : IPrinter
    {
        internal readonly StructElement element;

        public MDStructElementPrinter(StructElement element)
        {
            this.element = element;
        }

        public string Print()
        {
            return $"| {element.Name} | {element.Description} | {element.Type} |";
        }
    }

    internal abstract class MDContainerPrinter : IPrinter
    {
        internal readonly IDataContainer container;

        public MDContainerPrinter(IDataContainer container)
        {
            this.container = container;
        }

        public abstract string GetTable();

        public string Print()
        {
            var content = string.Format("# {0} {1}\n", container.Name, container.Type);
            if (!string.IsNullOrWhiteSpace(container.Description))
            {
                content += "\n## Description\n";
                content += string.Format("\n{0}\n", container.Description);
            }

            return content + GetTable();
        }
    }

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

    internal class MDMethodPrinter : IPrinter
    {
        private readonly MethodContainer container;

        public MDMethodPrinter(MethodContainer container)
        {
            this.container = container;
        }

        public string Print()
        {
            var content = string.Format("# {0} {1}\n", container.Name, container.Type);
            if (!string.IsNullOrWhiteSpace(container.Description))
            {
                content += "\n## Description\n";
                content += string.Format("\n{0}\n", container.Description);
            }

            content += GetParametersSection();
            content += GetExampleRequestSection();
            content += GetReturnsSection();
            content += GetExampleReturnSection();

            return content;
        }

        private string GetParametersSection()
        {
            var content = "\n## Parameters\n";
            content += string.Format("\n| Parameter | Description | Type |");
            content += string.Format("\n| :- | :- | :- |\n");
            content += string.Format("{0}", string.Join("\n", container.ArgumentsElements.Select(x => $"| {x.Name} | {x.Description} | {x.Type} |")));

            return content;
        }

        private string GetExampleRequestSection()
        {
            var content = "\n## Example request\n";
            content += "```json\n";
            content += container.GetInputJson();
            content += "```\n";

            return content;
        }

        private string GetReturnsSection()
        {
            var content = "\n## Returns\n";
            content += string.Format("\n| Parameter | Description | Type |");
            content += string.Format("\n| :- | :- | :- |\n");
            content += string.Format("{0}", $"| {container.OutputElement.Name} | {container.OutputElement.Description} | {container.OutputElement.Type} |");

            return content;
        }

        private string GetExampleReturnSection()
        {
            var content = "\n## Example response\n";
            content += "```json\n";
            content += container.GetOutputJson();
            content += "```\n";

            return content;
        }

    }

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
            if(groupedProps.TryGetValue("props", out var props))
                content += string.Format("{0}", string.Join("\n", props.Select(x => x.GetPrinter(PrinterType.Markdown).Print() )));
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

    public class MDPrinter
    {
        public string Print(IDataContainer handler)
        {
            var printer = handler.GetPrinter(PrinterType.Markdown);
            return printer.Print();
        }
    }
}