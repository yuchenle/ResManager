using System;
using System.Collections.Generic;
using System.Globalization;

namespace RestoManager.Services
{
    public class LocalizationService
    {
        private static LocalizationService? _instance;
        private Dictionary<string, Dictionary<string, string>> _translations;
        private string _currentLanguage;

        public static LocalizationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalizationService();
                }
                return _instance;
            }
        }

        private LocalizationService()
        {
            _translations = new Dictionary<string, Dictionary<string, string>>();
            InitializeTranslations();
            // Set French as default
            _currentLanguage = "fr";
        }

        public string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value && _translations.ContainsKey(value))
                {
                    _currentLanguage = value;
                    CultureChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler? CultureChanged;

        private void InitializeTranslations()
        {
            // English translations
            _translations["en"] = new Dictionary<string, string>
            {
                { "RestaurantManagementSystem", "Restaurant Management System" },
                { "SeeOrders", "View All Orders" },
                { "Accounts", "View Accounts" },
                { "Orders", "Orders" },
                { "Checkout", "Checkout" },
                { "Seats", "Seats" },
                { "Client", "Client" },
                { "Phone", "Phone" },
                { "Pickup", "Pickup" },
                { "Language", "Language" },
                { "English", "English" },
                { "French", "French" },
                { "Chinese", "Chinese" },
                { "TableCheckout", "Table {0} - Checkout" },
                { "Location", "Location" },
                { "DishName", "Dish Name" },
                { "Quantity", "Quantity" },
                { "UnitPrice", "Unit Price" },
                { "Subtotal", "Subtotal" },
                { "Tax10Percent", "Tax (10%)" },
                { "Tax", "Tax (10%)" },
                { "Total", "Total" },
                { "Cancel", "Cancel" },
                { "PrintBill", "Print the bill" },
                { "AllWebOrders", "All Web Orders" },
                { "WebOrdersHistory", "Web Orders History" },
                { "Delete", "Delete" },
                { "AccountsComingSoon", "Accounts feature coming soon!" },
                { "AccountsTitle", "Accounts" },
                { "FirestoreNotInitialized", "Firestore service is not initialized yet." },
                { "ErrorTitle", "Error" },
                { "InitializationError", "Initialization Error" },
                { "NoValidOrdersSelected", "No valid orders selected for deletion." },
                { "DeleteOrders", "Delete Orders" },
                { "ConfirmDelete", "Confirm Delete" },
                { "AreYouSureDeleteOrder", "Are you sure you want to delete this order?" },
                { "AreYouSureDeleteOrders", "Are you sure you want to delete {0} orders?" },
                { "OrderDeletedSuccessfully", "Order deleted successfully." },
                { "OrdersDeletedSuccessfully", "{0} orders deleted successfully." }
            };

            // French translations
            _translations["fr"] = new Dictionary<string, string>
            {
                { "RestaurantManagementSystem", "Système de Gestion de Restaurant" },
                { "SeeOrders", "Voir toutes les commandes" },
                { "Accounts", "Voir les comptes" },
                { "Orders", "Commandes" },
                { "Checkout", "Encaissement" },
                { "Seats", "Places" },
                { "Client", "Client" },
                { "Phone", "Téléphone" },
                { "Pickup", "À emporter" },
                { "Language", "Langue" },
                { "English", "Anglais" },
                { "French", "Français" },
                { "Chinese", "Chinois" },
                { "TableCheckout", "Table {0} - Encaissement" },
                { "Location", "Emplacement" },
                { "DishName", "Nom du Plat" },
                { "Quantity", "Quantité" },
                { "UnitPrice", "Prix Unitaire" },
                { "Subtotal", "Sous-total" },
                { "Tax10Percent", "Taxe (10%)" },
                { "Tax", "Taxe (10%)" },
                { "Total", "Total" },
                { "Cancel", "Annuler" },
                { "PrintBill", "Imprimer la facture" },
                { "AllWebOrders", "Toutes les Commandes Web" },
                { "WebOrdersHistory", "Historique des Commandes Web" },
                { "Delete", "Supprimer" },
                { "AccountsComingSoon", "La fonctionnalité Comptes arrive bientôt !" },
                { "AccountsTitle", "Comptes" },
                { "FirestoreNotInitialized", "Le service Firestore n'est pas encore initialisé." },
                { "ErrorTitle", "Erreur" },
                { "InitializationError", "Erreur d'Initialisation" },
                { "NoValidOrdersSelected", "Aucune commande valide sélectionnée pour la suppression." },
                { "DeleteOrders", "Supprimer les Commandes" },
                { "ConfirmDelete", "Confirmer la Suppression" },
                { "AreYouSureDeleteOrder", "Êtes-vous sûr de vouloir supprimer cette commande ?" },
                { "AreYouSureDeleteOrders", "Êtes-vous sûr de vouloir supprimer {0} commandes ?" },
                { "OrderDeletedSuccessfully", "Commande supprimée avec succès." },
                { "OrdersDeletedSuccessfully", "{0} commandes supprimées avec succès." }
            };

            // Chinese translations
            _translations["zh"] = new Dictionary<string, string>
            {
                { "RestaurantManagementSystem", "餐厅管理系统" },
                { "SeeOrders", "查看所有订单" },
                { "Accounts", "查看账户" },
                { "Orders", "订单" },
                { "Checkout", "结账" },
                { "Seats", "座位" },
                { "Client", "客户" },
                { "Phone", "电话" },
                { "Pickup", "外带" },
                { "Language", "语言" },
                { "English", "英语" },
                { "French", "法语" },
                { "Chinese", "中文" },
                { "TableCheckout", "桌台 {0} - 结账" },
                { "Location", "位置" },
                { "DishName", "菜品名称" },
                { "Quantity", "数量" },
                { "UnitPrice", "单价" },
                { "Subtotal", "小计" },
                { "Tax10Percent", "税费 (10%)" },
                { "Tax", "税费 (10%)" },
                { "Total", "总计" },
                { "Cancel", "取消" },
                { "PrintBill", "打印账单" },
                { "AllWebOrders", "所有网络订单" },
                { "WebOrdersHistory", "网络订单历史" },
                { "Delete", "删除" },
                { "AccountsComingSoon", "账户功能即将推出！" },
                { "AccountsTitle", "账户" },
                { "FirestoreNotInitialized", "Firestore 服务尚未初始化。" },
                { "ErrorTitle", "错误" },
                { "InitializationError", "初始化错误" },
                { "NoValidOrdersSelected", "没有选择有效的订单进行删除。" },
                { "DeleteOrders", "删除订单" },
                { "ConfirmDelete", "确认删除" },
                { "AreYouSureDeleteOrder", "您确定要删除此订单吗？" },
                { "AreYouSureDeleteOrders", "您确定要删除 {0} 个订单吗？" },
                { "OrderDeletedSuccessfully", "订单删除成功。" },
                { "OrdersDeletedSuccessfully", "{0} 个订单删除成功。" }
            };
        }

        public string GetString(string key)
        {
            if (_translations.ContainsKey(_currentLanguage) && 
                _translations[_currentLanguage].ContainsKey(key))
            {
                return _translations[_currentLanguage][key];
            }
            
            // Fallback to English
            if (_translations.ContainsKey("en") && 
                _translations["en"].ContainsKey(key))
            {
                return _translations["en"][key];
            }
            
            return key;
        }

        public void SetLanguage(string languageCode)
        {
            if (_translations.ContainsKey(languageCode))
            {
                CurrentLanguage = languageCode;
            }
        }
    }
}
