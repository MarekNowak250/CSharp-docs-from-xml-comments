using DescriptionGenerator.Core.Interfaces;

namespace DescriptionGenerator.Core.Printing
{
    public class MDPrinter
    {
        public string Print(IDataContainer handler)
        {
            var printer = handler.GetPrinter(PrinterType.Markdown);
            return printer.Print();
        }
    }
}