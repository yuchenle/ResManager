using System.Windows;
using ResManager.ViewModels;

namespace ResManager.Views
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
