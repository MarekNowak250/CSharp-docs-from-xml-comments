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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            var configuration = Config.LoadConfig();
            CheckBox_PropertiesSummary.IsChecked = configuration.IncludePropertiesSummary;
            CheckBox_ContainersSummary.IsChecked = configuration.IncludeContainersSummary;
            CheckBox_NamespaceStructure.IsChecked = configuration.NamespaceLikeStructure;
            CheckBox_NestedDependencies.IsChecked = configuration.IncludeNested;
            CheckBox_GenerateLinks.IsChecked = configuration.GenerateLinks;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            new Config()
            {
                IncludePropertiesSummary = CheckBox_PropertiesSummary.IsChecked ?? true,
                IncludeContainersSummary = CheckBox_ContainersSummary.IsChecked ?? true,
                NamespaceLikeStructure = CheckBox_NamespaceStructure.IsChecked ?? false,
                IncludeNested = CheckBox_NestedDependencies.IsChecked ?? true,
                GenerateLinks = CheckBox_GenerateLinks.IsChecked ?? true,
            }.SaveConfig();

            MessageBox.Show("Configuration has been saved", "Configuration saved");
        }
    }
}
