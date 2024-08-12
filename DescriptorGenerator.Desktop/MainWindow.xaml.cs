using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DescriptionGenerator.Core;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace DescriptorGenerator.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AssemblyLoaderContext loaderContext = new AssemblyLoaderContext();
        ProcessAssemblyContext processContext = new ProcessAssemblyContext();

        public MainWindow()
        {
            InitializeComponent();
            LoadPanel.DataContext = loaderContext;
            loaderContext.PropertyChanged += LoaderContext_PropertyChanged;
            ProcessPanel.DataContext = processContext;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewTypes.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Namespace");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void LoaderContext_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!e.PropertyName?.Equals("AssemblyName") ?? true)
                return;

            processContext.LoadTypes(loaderContext.Assembly);
        }

        private void ShowSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();

            settingsWindow.Closed += SettingsWindow_Closed;
            IsEnabled = false;
        }

        private void SettingsWindow_Closed(object? sender, EventArgs e)
        {
            IsEnabled = true;
            WindowState = WindowState.Normal;
            Focus();
        }
    }

    public partial class ProcessAssemblyContext : ObservableObject
    {
        public ObservableCollection<Type> AccessibleTypes { get; set; } = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
        private string searchValue;
        
        private Type[] allTypes;
        private bool isLoading = false;
        private Assembly currentAssembly;

        [RelayCommand]
        private void Process(Type type)
        {
            if (!CanProcess())
                return;

            isLoading = true;
            var result = MessageBox.Show($"Are you sure you want to generate data for {type.FullName}", "Confirmation window", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                var config = Config.LoadConfig();
                AssemblyProcessor assemblyProcessor = new AssemblyProcessor(currentAssembly, config);

                // node window where you can uncheck which items you want to save?
                var output = assemblyProcessor.ProcessContainer(type);
                new NodesWindow(output).Show();
            }

            isLoading = false;
        }

        [RelayCommand(CanExecute = nameof(CanSearch))]
        private void Search()
        {
            isLoading = true;
            AccessibleTypes.Clear();
            if(SearchValue.Length == 0)
            {
                foreach (var item in allTypes)
                {
                    AccessibleTypes.Add(item);
                }
            }
            else
            {
                foreach (var item in allTypes.Where(x => x.Name.Contains(SearchValue, StringComparison.InvariantCultureIgnoreCase)))
                {
                    AccessibleTypes.Add(item);
                }
            }

            isLoading = false;
        }

        public void LoadTypes(Assembly assembly)
        {
            try
            {
                isLoading = true;
                currentAssembly = assembly;
                AccessibleTypes.Clear();
                allTypes = assembly.GetTypes().Where(x => x.IsClass || x.IsInterface || x.IsEnum).Where( x=> !x.Name.StartsWith('<')).ToArray();
                foreach (var item in allTypes)
                {
                    AccessibleTypes.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cannot load assembly types");
            }
            finally
            {
                isLoading = false;
            }
        }

        private bool CanProcess()
        {
            return !isLoading && currentAssembly is not null; 
        }

        private bool CanSearch()
        {
            return !isLoading && (SearchValue?.Length > 3 || SearchValue?.Length == 0);
        }
    }

    public partial class AssemblyLoaderContext: ObservableObject
    {
        [ObservableProperty]
        private string assemblyName;

        [ObservableProperty]
        public string path;

        public Assembly Assembly { get; private set; }

        private bool isLoading = false;

        [RelayCommand(CanExecute = nameof(CanLoad))]
        private void Load()
        {
            try
            {
                isLoading = true;
                var dialog = new OpenFileDialog();
                dialog.Filter = dialog.Filter = "DLL files|*.dll";
                var succeded = dialog.ShowDialog();

                if (!succeded ?? true)
                    return;

                Path = dialog.FileName;
                Assembly = new AssemblyReader(Path).Read();
                AssemblyName = Assembly.GetName().Name;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Cannot load assembly");
            }
            finally
            {
                isLoading = false;
            }
        }


        private bool CanLoad()
        {
            return !isLoading;
        }
    }
}