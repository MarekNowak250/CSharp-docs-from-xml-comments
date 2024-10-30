using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DescriptionGenerator.Core;
using DescriptionGenerator.Core.Interfaces;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace DescriptorGenerator.Desktop
{
    /// <summary>
    /// Interaction logic for NamespaceMapWindow.xaml
    /// </summary>
    public partial class NamespaceMapWindow : Window
    {
        NamespaceMapContext _context;

        public NamespaceMapWindow(IDataContainer[] nodes, NamespaceMap currentMap)
        {
            InitializeComponent();
            _context = new NamespaceMapContext(nodes, currentMap);
            ListView_NamespaceMap.ItemsSource = _context.NamespaceMapItems;
            this.DataContext = _context;
        }

        public NamespaceMap GetNamespaceMap => _context.NamespaceMap;
    }

    internal partial class NamespaceMapContext
    {
        public ObservableCollection<NamespaceMapItem> NamespaceMapItems { get; private set; }
        NamespaceMap _map;

        public NamespaceMapContext(IDataContainer[] nodes, NamespaceMap currentMap)
        {
            NamespaceMapItems = new();
            _map = new NamespaceMap();
            _map.Generate(nodes);

            foreach (var item in currentMap.Value)
            {
                _map.SetValue(item.Key, item.Value);
            }

            Generate();
        }

        public NamespaceMap NamespaceMap => _map;

        private void Generate()
        {
            NamespaceMapItems.Clear();
            foreach (var item in _map.Value)
            {
                var namespaceMapItem = new NamespaceMapItem() { Name = item.Key, Map = item.Value };
                NamespaceMapItems.Add(namespaceMapItem);
            }
        }

        [RelayCommand]
        public void UpdateItem(NamespaceMapItem source)
        {
            _map.SetValue(source.Name, source.Map);
            Generate();
        }

        [RelayCommand]
        public void Save()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "node map files|*.node_map";
            dialog.DefaultExt = ".node_map";

            var succeded = dialog.ShowDialog();

            if (!succeded ?? true)
                return;

            File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(_map?.Value));
        }

        [RelayCommand]
        public void Load()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "node map files|*.node_map";

            var succeded = dialog.ShowDialog();

            if (!succeded ?? true)
                return;

            var fileContent = File.ReadAllText(dialog.FileName);
            var savedMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent);

            _map = new NamespaceMap();
            foreach (var item in savedMap)
            {
                _map.SetValue(item.Key, item.Value);
            }

            Generate();
        }
    }

    internal partial class NamespaceMapItem : ObservableObject
    {
        public NamespaceMapItem()
        {
        }

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string map;
    }
}
