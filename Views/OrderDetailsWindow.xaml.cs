using System.Windows;
using System.Windows.Input;
using RestoManager.Models;

namespace RestoManager.Views
{
    public partial class OrderDetailsWindow : Window
    {
        public OrderDetailsWindow(Order order)
        {
            InitializeComponent();
            DataContext = order;
            
            // Set owner to center correctly
            if (Application.Current.MainWindow != null)
            {
                this.Owner = Application.Current.MainWindow;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
