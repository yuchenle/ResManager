using System.Windows;
using RestoManager.ViewModels;

namespace RestoManager.Views
{
    public partial class MenuWindow : Window
    {
        public MenuWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
