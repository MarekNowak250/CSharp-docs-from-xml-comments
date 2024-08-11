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
        List<ExtendedNode> extendedNodes;
        public NodesWindow(NodeContainer[] nodes)
        {
            InitializeComponent();
            extendedNodes = new List<ExtendedNode>();
            foreach (var node in nodes) 
            {
                extendedNodes.Add(new ExtendedNode(node));
            }
            Test.ItemsSource = extendedNodes;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();

            var succeded = dialog.ShowDialog();

            if (!succeded ?? true)
                return;

            MDPrinter printer = new MDPrinter();
            var nodesToProcess = extendedNodes.Where( x=> x.Selected).ToArray();

            for( int i=0; i< nodesToProcess.Length; i++ )
            {
                var node = new NodeContainer(
                    nodesToProcess[i].Name,
                    nodesToProcess[i].Type,
                    nodesToProcess[i].Description,
                    nodesToProcess[i].Properties);

                var content = printer.Print(node);
                File.WriteAllText(Path.Combine(dialog.FolderName, $"{node.Name}.MD"), content);
            }

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
            WindowState = System.Windows.WindowState.Normal;
            Focus();
        }
    }
}
