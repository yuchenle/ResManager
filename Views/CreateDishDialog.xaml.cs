using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using RestoManager.Models;

namespace RestoManager.Views
{
    public partial class CreateDishDialog : Window
    {
        public Dish? CreatedDish { get; private set; }

        public CreateDishDialog()
        {
            InitializeComponent();
            NameTextBox.Focus();
            CategoryComboBox.SelectedIndex = 0;
        }

        private void PriceTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Allow numeric input and decimal point
            TextBox textBox = sender as TextBox ?? throw new InvalidOperationException();
            string newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            Regex regex = new Regex(@"^\d*\.?\d*$");
            e.Handled = !regex.IsMatch(newText);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter a name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, NumberStyles.Currency | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal price) || price < 0)
            {
                MessageBox.Show("Please enter a valid price (greater than or equal to 0).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CategoryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CreatedDish = new Dish
            {
                Name = name,
                Price = price,
                Category = (DishCategory)CategoryComboBox.SelectedItem,
                Description = string.Empty,
                IsAvailable = true
            };

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
