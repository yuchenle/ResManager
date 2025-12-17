using System.ComponentModel;
using RestoManager.Services;

namespace RestoManager.Resources
{
    public class LocalizedStrings : INotifyPropertyChanged
    {
        private static LocalizationService _localizationService = LocalizationService.Instance;

        public static LocalizedStrings Instance { get; } = new LocalizedStrings();

        public LocalizedStrings()
        {
            _localizationService.CultureChanged += (s, e) =>
            {
                OnPropertyChanged(string.Empty); // Notify all properties changed
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Main Window
        public string RestaurantManagementSystem => _localizationService.GetString("RestaurantManagementSystem");
        public string SeeOrders => _localizationService.GetString("SeeOrders");
        public string Accounts => _localizationService.GetString("Accounts");
        public string Orders => _localizationService.GetString("Orders");
        public string Checkout => _localizationService.GetString("Checkout");
        public string Seats => _localizationService.GetString("Seats");
        public string Client => _localizationService.GetString("Client");
        public string Phone => _localizationService.GetString("Phone");
        public string Pickup => _localizationService.GetString("Pickup");
        public string Language => _localizationService.GetString("Language");
        public string English => _localizationService.GetString("English");
        public string French => _localizationService.GetString("French");
        public string Chinese => _localizationService.GetString("Chinese");

        // Checkout Window
        public string TableCheckout => _localizationService.GetString("TableCheckout");
        public string Location => _localizationService.GetString("Location");
        public string DishName => _localizationService.GetString("DishName");
        public string Quantity => _localizationService.GetString("Quantity");
        public string UnitPrice => _localizationService.GetString("UnitPrice");
        public string Subtotal => _localizationService.GetString("Subtotal");
        public string Tax10Percent => _localizationService.GetString("Tax10Percent");
        public string Tax => _localizationService.GetString("Tax");
        public string Total => _localizationService.GetString("Total");
        public string Cancel => _localizationService.GetString("Cancel");
        public string PrintBill => _localizationService.GetString("PrintBill");

        // All Orders Window
        public string AllWebOrders => _localizationService.GetString("AllWebOrders");
        public string WebOrdersHistory => _localizationService.GetString("WebOrdersHistory");
        public string Delete => _localizationService.GetString("Delete");

        // Messages
        public string AccountsComingSoon => _localizationService.GetString("AccountsComingSoon");
        public string AccountsTitle => _localizationService.GetString("AccountsTitle");
        public string FirestoreNotInitialized => _localizationService.GetString("FirestoreNotInitialized");
        public string ErrorTitle => _localizationService.GetString("ErrorTitle");
        public string InitializationError => _localizationService.GetString("InitializationError");
        public string NoValidOrdersSelected => _localizationService.GetString("NoValidOrdersSelected");
        public string DeleteOrders => _localizationService.GetString("DeleteOrders");
        public string ConfirmDelete => _localizationService.GetString("ConfirmDelete");
        public string AreYouSureDeleteOrder => _localizationService.GetString("AreYouSureDeleteOrder");
        public string AreYouSureDeleteOrders => _localizationService.GetString("AreYouSureDeleteOrders");
        public string OrderDeletedSuccessfully => _localizationService.GetString("OrderDeletedSuccessfully");
        public string OrdersDeletedSuccessfully => _localizationService.GetString("OrdersDeletedSuccessfully");
    }
}
