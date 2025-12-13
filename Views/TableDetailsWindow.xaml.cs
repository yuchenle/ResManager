using System.Windows;
using System.Windows.Input;
using ResManager.ViewModels;

namespace ResManager.Views
{
    public partial class TableDetailsWindow : Window
    {
        public TableDetailsWindow()
        {
            InitializeComponent();
        }

        private void OrderItemsDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && DataContext is TableDetailsViewModel viewModel)
            {
                if (viewModel.SelectedOrderItem != null && viewModel.DeleteOrderItemCommand.CanExecute(viewModel.SelectedOrderItem))
                {
                    viewModel.DeleteOrderItemCommand.Execute(viewModel.SelectedOrderItem);
                    e.Handled = true;
                }
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
            }
        }
    }
}
