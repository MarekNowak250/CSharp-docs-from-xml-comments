using DescriptionGenerator.Core.Interfaces;

namespace DescriptionGenerator.Core.Printing
{
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
}