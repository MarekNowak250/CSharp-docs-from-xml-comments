using DescriptionGenerator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace DescriptorGenerator.Desktop
{
    /// <summary>
    /// Interaction logic for NodeDetailsWindow.xaml
    /// </summary>
    public partial class NodeDetailsWindow : Window
    {
        public NodeDetailsWindow(ExtendedNode node)
        {
            InitializeComponent();
            DataContext = node;
        }
    }
}
