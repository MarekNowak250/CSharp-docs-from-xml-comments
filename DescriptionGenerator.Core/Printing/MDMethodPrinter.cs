using DescriptionGenerator.Core.Interfaces;
using DescriptionGenerator.Core.Types;

namespace DescriptionGenerator.Core.Printing
{
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
}