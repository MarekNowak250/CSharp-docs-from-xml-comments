using DescriptionGenerator.Core;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DescriptorGenerator.Desktop
{
    /// <summary>
    /// Interaction logic for NodesWindow.xaml
    /// </summary>
    public partial class NodesWindow : Window
    {
        List<ExtendedNode> _extendedNodes;
        NamespaceMap _namespaceMap;

        public NodesWindow(NodeContainer[] nodes)
        {
            InitializeComponent();
            _extendedNodes = new List<ExtendedNode>();
            foreach (var node in nodes) 
            {
                _extendedNodes.Add(new ExtendedNode(node));
            }
            ListView_Nodes.ItemsSource = _extendedNodes;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();

            var succeded = dialog.ShowDialog();

            if (!succeded ?? true)
                return;

            var config = Config.LoadConfig();

            MDPrinter printer = new MDPrinter();
            var nodesToProcess = _extendedNodes.Where( x=> x.Selected).ToArray();

            ISaver saver = null;
            if(config.NamespaceLikeStructure && _namespaceMap != null)
                saver = ISaver.GetSaver(dialog.FolderName, _namespaceMap, config.GenerateLinks, printer);
            else
                saver = ISaver.GetSaver(dialog.FolderName, config.NamespaceLikeStructure, config.GenerateLinks, printer);

            saver.Save(nodesToProcess);

            MessageBox.Show($"{nodesToProcess.Length} files were created at {dialog.FolderName}", "Files created");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var data = (sender as Button).DataContext as ExtendedNode;
            var window = new NodeDetailsWindow(data);
            window.Show();
            window.Closed += Window_Closed;
            this.IsEnabled = false;
        }

        private void Window_Closed(object? sender, EventArgs e)
        {
            IsEnabled = true;
            WindowState = WindowState.Normal;
            Focus();
            if(sender is NamespaceMapWindow namespaceMapWindow)
                _namespaceMap = namespaceMapWindow.GetNamespaceMap;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var window = new NamespaceMapWindow(_extendedNodes.ToArray(), _namespaceMap ?? new());
            window.Show();
            window.Closed += Window_Closed;
            this.IsEnabled = false;
        }
    }
}
