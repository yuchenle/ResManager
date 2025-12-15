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

        private void OrderItemsDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && DataContext is CreateTakeAwayViewModel viewModel)
            {
                if (viewModel.SelectedOrderItem != null && viewModel.RemoveItemCommand.CanExecute(viewModel.SelectedOrderItem))
                {
                    viewModel.RemoveItemCommand.Execute(viewModel.SelectedOrderItem);
                    e.Handled = true;
                }
            }
        }
    }
}
