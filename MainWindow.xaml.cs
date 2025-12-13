using System.Windows;
using System.Windows.Input;
using RestoManager.Models;
using RestoManager.ViewModels;
using RestoManager.Services;
using System;

namespace RestoManager
{
    public partial class MainWindow : Window
    {
        private FirestoreListenerService _firestoreService;

        public MainWindow()
        {
            InitializeComponent();

            if (DataContext is MainViewModel vm)
            {
                _firestoreService = new FirestoreListenerService(vm.Service);
                
                // Construct path safely
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string jsonPath = System.IO.Path.Combine(baseDir, "firestore-project.json");

                // "restoauthen" is the project ID provided by the user
                _ = _firestoreService.StartListeningAsync("restoauthen", jsonPath);
            }
        }

        private void TableBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.Border border && border.DataContext is Table table)
            {
                if (DataContext is MainViewModel viewModel)
                {
                    viewModel.ShowTableDetailsCommand.Execute(table);
                }
            }
        }
    }
}

