namespace DescriptionGenerator.Core
{
    public interface IDataContainer
    {
        string Description { get; }
        string Name { get; }
        string GetType();
        List<StructElement> Properties { get; }
    }
}