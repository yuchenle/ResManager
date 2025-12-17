using System.Windows;
using System.Windows.Input;
using RestoManager.Models;
using RestoManager.Resources;
using RestoManager.ViewModels;
using RestoManager.Services;
using System;

namespace RestoManager
{
    public partial class MainWindow : Window
    {
        private FirestoreListenerService _firestoreService;
        private LocalizedStrings _localizedStrings = LocalizedStrings.Instance;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                if (DataContext is MainViewModel vm)
                {
                    _firestoreService = new FirestoreListenerService(vm.Service);
                    vm.FirestoreService = _firestoreService;
                    
                    // Construct path safely
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string jsonPath = System.IO.Path.Combine(baseDir, "firestore-project.json");

                    // "restoauthen" is the project ID provided by the user
                    _ = _firestoreService.StartListeningAsync("restoauthen", jsonPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error initializing MainWindow:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    _localizedStrings.InitializationError,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw; // Re-throw to let App.xaml.cs handle it
            }
        }

    }
}

