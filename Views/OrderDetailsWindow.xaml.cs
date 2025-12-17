using System.Windows;
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
    }
}
