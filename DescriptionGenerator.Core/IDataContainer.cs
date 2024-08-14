namespace DescriptionGenerator.Core
{
    public interface IDataContainer
    {
        string Description { get; }
        string Name { get; }
        string Type { get; }
        string Namespace { get; }
        List<StructElement> Properties { get; }
    }
}