namespace DescriptionGenerator.Core
{
    public interface IDataContainer
    {
        string Description { get; }
        string Name { get; }
        string Type { get; }
        string Namespace { get; set;  }
        List<StructElement> Properties { get; }
    }
}