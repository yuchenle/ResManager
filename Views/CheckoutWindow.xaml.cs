using System.Windows;
using System.Windows.Input;
using RestoManager.ViewModels;

namespace RestoManager.Views
{
    public partial class CheckoutWindow : Window
    {
        public CheckoutWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PrintBillButton_Click(object sender, RoutedEventArgs e)
        {
            // Command binding on the button already executed the PrintBillCommand.
            // This handler only closes the window after printing.
            DialogResult = true;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
