using System.Configuration;
using System.Data;
using System.Windows;

namespace DescriptorGenerator.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e) => FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata
        {
            DefaultValue = Current.FindResource(typeof(Window))
        });
    }

}
