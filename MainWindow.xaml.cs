using System.Windows;
using System.Windows.Input;
using RestoManager.Models;
using RestoManager.ViewModels;

namespace RestoManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

