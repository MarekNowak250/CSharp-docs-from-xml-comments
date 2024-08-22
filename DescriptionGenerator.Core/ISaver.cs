using System.Diagnostics;

namespace DescriptionGenerator.Core
{
    public interface ISaver
    {
        void Save(IList<IDataContainer> nodesToProcess);

        public static ISaver GetSaver(string rootFolderPath, bool saveNamespaceLike, bool generateLinks, MDPrinter? printer = null)
    => saveNamespaceLike ? new NamespaceLikeMDSaver(rootFolderPath, false, printer) : new SimpleMDSaver(rootFolderPath, printer);

        public static ISaver GetSaver(string rootFolderPath, NamespaceMap namespaceMap, bool generateLinks, MDPrinter? printer = null)
            => new NamespaceLikeMDSaver(rootFolderPath, generateLinks, printer, namespaceMap);
    }

    internal class SimpleMDSaver : ISaver
    {
        private readonly string _rootFolderPath;
        private readonly MDPrinter _printer;

        public SimpleMDSaver(string rootFolderPath, MDPrinter printer = null)
        {
            _rootFolderPath = rootFolderPath;
            _printer = printer ?? new MDPrinter();
        }

        public void Save(IList<IDataContainer> nodesToProcess)
        {
            for (int i = 0; i < nodesToProcess.Count(); i++)
            {
                var node = nodesToProcess[i];
                var content = _printer.Print(node);
                File.WriteAllText(Path.Combine(_rootFolderPath, $"{node.Name}.MD"), content);
            }
        }
    }

    internal class NamespaceLikeMDSaver : ISaver
    {
        private readonly string _rootFolderPath;
        private readonly bool _generateLinks;
        private readonly MDPrinter _printer;
        private readonly Dictionary<string, string> _namespaceMap;

        public NamespaceLikeMDSaver(string rootFolderPath, bool generateLinks, MDPrinter printer = null, NamespaceMap namespaceMap = null)
        {
            _rootFolderPath = rootFolderPath;
            _generateLinks = generateLinks;
            _printer = printer ?? new MDPrinter();
            _namespaceMap = namespaceMap?.Value ?? new();
        }

        public void Save(IList<IDataContainer> nodesToProcess)
        {
            Dictionary<string, string> namespacePathMap = new();

            for (int i = 0; i < nodesToProcess.Count(); i++)
            {
                var node = nodesToProcess[i];

                if(!namespacePathMap.TryGetValue(node.Namespace, out string folderPath))
                {
                    if(string.IsNullOrEmpty(node.Namespace))
                        folderPath = "";
                    else if(_namespaceMap.TryGetValue(node.Namespace, out string namespaceMapped))
                        folderPath = mapNamespaceToPath(namespaceMapped);
                    else
                        folderPath = mapNamespaceToPath(node.Namespace);

                    namespacePathMap[node.Namespace] = folderPath;
                }

                folderPath = Path.Combine(_rootFolderPath, folderPath);
                Directory.CreateDirectory(folderPath);
                
                node.Namespace = folderPath;
            }

            foreach(var node in nodesToProcess)
            {
                if (_generateLinks)
                {
                    var linker = new Linker(nodesToProcess);
                    linker.LinkDependencies(node);
                }

                var content = _printer.Print(node);

                File.WriteAllText(Path.Combine(node.Namespace, $"{node.Name}.MD"), content);
            }
        }

        private string mapNamespaceToPath(string nodeNamespace)
        {
            return nodeNamespace.Replace(".", "/");
        }
    }

    internal class Linker
    {
        private readonly IEnumerable<IDataContainer> dataContainers;

        public Linker(IEnumerable<IDataContainer> dataContainers)
        {
            this.dataContainers = dataContainers;
        }

        public void LinkDependencies(IDataContainer dataContainer)
        {
            foreach(var prop in dataContainer.Properties)
            {
                var corespondingNode = dataContainers.SingleOrDefault(x => x.Name == prop.Type);
                if (corespondingNode is null)
                    continue;

                Uri baseUri = new Uri(dataContainer.Namespace);
                Uri dependencyUri = new Uri(corespondingNode.Namespace);
                Uri relativeUri = baseUri.MakeRelativeUri(dependencyUri);

                // not sure why it's always one level too deep
                prop.Type = $"[{prop.Type}](../{relativeUri.ToString()}/{prop.Type}.MD)";
            }
        }
    }
}
