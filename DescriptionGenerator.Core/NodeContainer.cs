namespace DescriptionGenerator.Core
{
    public class NodeContainer : StructElement, IDataContainer
    {
        public string Namespace { get; }
        public List<StructElement> Properties { get; }

        public NodeContainer(string name, string type, string description, string nodeNamespace, List<StructElement> properties = null) : base(name, type, description)
        {
            Namespace = nodeNamespace;
            Properties = properties ?? new();
        }
    }
}
