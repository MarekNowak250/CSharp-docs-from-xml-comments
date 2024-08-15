namespace DescriptionGenerator.Core
{
    public class NamespaceMap
    {
        private readonly Dictionary<string, string> _namespaceMap;
        public Dictionary<string, string> Value => new(_namespaceMap);

        public NamespaceMap() 
        {
            _namespaceMap = new();
        }

        public void Generate(IDataContainer[] nodeContainers)
        {
            foreach(var node in nodeContainers) 
            {
                if (!_namespaceMap.ContainsKey(node.Namespace))
                    _namespaceMap.Add(node.Namespace, node.Namespace);
            }
        }

        public void Generate(Dictionary<string, string> namespaceMap) 
        {
            foreach (var item in namespaceMap)
            {
                if (!_namespaceMap.ContainsKey(item.Key))
                    _namespaceMap.Add(item.Key, item.Value);
            }
        }

        public bool TryGetValue(string key, out string value)
        {
            return _namespaceMap.TryGetValue(key, out value);
        }

        public void SetValue(string key, string value) 
        {
            if (!_namespaceMap.TryGetValue(key, out string prevValue))
                _namespaceMap.Add(key, value);
            else
                _namespaceMap[key] = value;

            foreach(var item in _namespaceMap)
            {
                var namespaces = item.Value.Split(".");
                if (string.IsNullOrEmpty(prevValue) || !namespaces.Contains(prevValue))
                    continue;

                // for custom partially map e.g core/classes -> core_classes
                // will map core/classes/item_class to core_classes/item_class
                _namespaceMap[item.Key] = item.Value.Replace(prevValue, value);
            }
        }
    }
}
