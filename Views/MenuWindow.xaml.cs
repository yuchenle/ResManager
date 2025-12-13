using System.Windows;
using ResManager.ViewModels;

namespace ResManager.Views
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
