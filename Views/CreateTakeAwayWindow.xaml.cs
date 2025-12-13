using System.Windows;
using System.Windows.Input;
using RestoManager.ViewModels;

namespace RestoManager.Views
{
    public partial class CreateTakeAwayWindow : Window
    {
        public CreateTakeAwayWindow()
        {
            InitializeComponent();
        }

        private void MenuItemsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is CreateTakeAwayViewModel viewModel && viewModel.SelectedDish != null)
            {
                if (viewModel.AddDishCommand.CanExecute(viewModel.SelectedDish))
                {
                    viewModel.AddDishCommand.Execute(viewModel.SelectedDish);
                }
            }
        }
    }
}
