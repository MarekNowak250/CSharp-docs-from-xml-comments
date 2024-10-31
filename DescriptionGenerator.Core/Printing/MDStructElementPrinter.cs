using DescriptionGenerator.Core.Interfaces;
using DescriptionGenerator.Core.Types;

namespace DescriptionGenerator.Core.Printing
{
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
}