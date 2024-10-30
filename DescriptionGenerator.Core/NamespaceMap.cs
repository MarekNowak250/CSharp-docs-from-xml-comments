using DescriptionGenerator.Core.Interfaces;

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
            foreach (var node in nodeContainers)
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

            if (string.IsNullOrEmpty(prevValue))
                return;

            foreach (var item in _namespaceMap)
            {
                var prevParts = prevValue.Split(".");
                var namespaces = item.Value.Split(".");
                if (prevParts.Length >= namespaces.Length)
                    continue;

                bool replace = true;
                int i = 0;
                for (; i < prevParts.Length; i++)
                {
                    if (namespaces[i] == prevParts[i])
                        continue;

                    replace = false;
                    break;
                }

                if (!replace)
                    continue;

                _namespaceMap[item.Key] = $"{value}.{string.Join(".", namespaces.Skip(i))}";
            }
        }
    }
}
