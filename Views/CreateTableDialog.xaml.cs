using System;
using System.Text.RegularExpressions;
using System.Windows;
using ResManager.Models;

namespace ResManager.Views
{
    public partial class CreateTableDialog : Window
    {
        public Table? CreatedTable { get; private set; }

        public CreateTableDialog()
        {
            InitializeComponent();
            CapacityTextBox.Focus();
            CapacityTextBox.SelectAll();
        }

        private void CapacityTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Only allow numeric input
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(CapacityTextBox.Text, out int capacity) && capacity > 0)
            {
                string location = LocationTextBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(location))
                {
                    MessageBox.Show("Please enter a location.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                CreatedTable = new Table
                {
                    Capacity = capacity,
                    Location = location,
                    Status = TableStatus.Available
                };

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid capacity (greater than 0).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

