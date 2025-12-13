using System.Windows;
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
    }
}
