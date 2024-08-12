namespace DescriptionGenerator.Core
{
    public class NodeContainer : StructElement, IDataContainer
    {
        public List<StructElement> Properties { get; }

        public NodeContainer(string name, string type, string description, List<StructElement> properties = null) : base(name, type, description)
        {
            Properties = properties ?? new();
        }

        public string GetType()
        {
            return Type;
        }
    }
}
