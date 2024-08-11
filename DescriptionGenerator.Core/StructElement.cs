namespace DescriptionGenerator.Core
{
    public class StructElement
    {
        public StructElement(string name, string type, string description)
        {
            Name = name;
            Type = type;
            Description = description;
        }

        public string Name { get; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}