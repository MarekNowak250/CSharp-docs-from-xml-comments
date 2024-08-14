using CommunityToolkit.Mvvm.ComponentModel;
using DescriptionGenerator.Core;

namespace DescriptorGenerator.Desktop
{
    public partial class ExtendedNode : ObservableObject, IDataContainer
    {
        [ObservableProperty]
        private bool selected;

        public ExtendedNode(NodeContainer nodeContainer)
        {
            Name = nodeContainer.Name;
            Type = nodeContainer.Type;
            Description = nodeContainer.Description;
            Properties = nodeContainer.Properties;
            Namespace = nodeContainer.Namespace;
            Selected = true;
        }

        public string Name { get; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Namespace { get; set; }

        public List<StructElement> Properties { get; set; }
    }
}
