using DescriptionGenerator.Core.Printing;
using DescriptionGenerator.Core.Types;

namespace DescriptionGenerator.Core.Interfaces
{
    public interface IDataContainer
    {
        string Description { get; }
        string Name { get; }
        string Type { get; }
        string Namespace { get; set; }
        List<StructElement> Properties { get; }
        IPrinter GetPrinter(PrinterType printerType);
    }
}