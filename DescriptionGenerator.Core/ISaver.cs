namespace DescriptionGenerator.Core
{
    public interface ISaver
    {
        void Save(IList<IDataContainer> nodesToProcess);

        public static ISaver GetSaver(string rootFolderPath, bool saveNamespaceLike, MDPrinter? printer = null) 
            => saveNamespaceLike ? new NamespaceLikeMDSaver(rootFolderPath, printer) : new SimpleMDSaver(rootFolderPath, printer);
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
        private readonly MDPrinter _printer;

        public NamespaceLikeMDSaver(string rootFolderPath, MDPrinter printer = null)
        {
            _rootFolderPath = rootFolderPath;
            _printer = printer ?? new MDPrinter();
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
                    else
                        folderPath = mapNamespaceToPath(node.Namespace);

                    namespacePathMap[node.Namespace] = folderPath;
                }

                var content = _printer.Print(node);
                
                folderPath = Path.Combine(_rootFolderPath, folderPath);
                Directory.CreateDirectory(folderPath);
                
                File.WriteAllText(Path.Combine(folderPath, $"{node.Name}.MD"), content);
            }
        }

        private string mapNamespaceToPath(string nodeNamespace)
        {
            return nodeNamespace.Replace(".", "/");
        }
    }
}
