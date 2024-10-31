using DescriptionGenerator.Core.Interfaces;
using DescriptionGenerator.Core.Printing;

namespace DescriptionGenerator.Core.Types
{
    public class NodeContainer : StructElement, IDataContainer
    {
        public string Namespace { get; set; }
        public List<StructElement> Properties { get; }

        public NodeContainer(string name, string type, string description, string nodeNamespace, List<StructElement> properties = null) : base(name, type, description)
        {
            Namespace = nodeNamespace;
            Properties = properties ?? new();
        }

        public override IPrinter GetPrinter(PrinterType printerType)
        {
            switch (printerType)
            {
                case PrinterType.Markdown:
                    if (Type == "class")
                        return new MDClassPrinter(this);
                    else
                        return new MDEnumPrinter(this);
                default:
                    throw new NotImplementedException($"Type {printerType} is not supported for node container");
            }
        }
    }
}
