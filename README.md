# ResManager

A Windows native desktop application for restaurant management built with C# and WPF.

## Features

- **Table Management**: Track table status (Available, Occupied, Reserved, Cleaning)
- **Menu Management**: Manage dishes with categories, prices, and availability
- **Order Processing**: Create and manage orders with multiple items
- **Order Tracking**: Monitor order status (Pending, InProgress, Ready, Served, Paid, Cancelled)
- **Reservation System**: Manage table reservations with customer information
- **Payment Processing**: Track payments for orders

## Project Structure

```
ResManager/
├── Models/              # Data models
│   ├── Table.cs        # Table entity with status tracking
│   ├── Dish.cs         # Menu item entity
│   ├── Order.cs        # Order entity
│   ├── OrderItem.cs    # Order line item
│   ├── Reservation.cs  # Table reservation
│   └── Payment.cs      # Payment record
├── Services/           # Business logic
│   └── RestaurantService.cs  # Main service for data management
├── ViewModels/         # MVVM ViewModels
│   └── MainViewModel.cs
├── Converters/         # Value converters
│   └── TotalConverter.cs
├── App.xaml            # Application definition
├── MainWindow.xaml     # Main UI window
├── Styles.xaml         # Application styles
└── ResManager.csproj   # Project file
```

## Requirements

- .NET 8.0 SDK or later
- Windows OS
- Visual Studio 2022 or Visual Studio Code (optional)

## Building the Project

### Using .NET CLI (Recommended)

```bash
dotnet restore
dotnet build
dotnet run
```

### Using Visual Studio

1. Open `ResManager.csproj` in Visual Studio
2. Restore NuGet packages
3. Build and run (F5)

### Using CMake

Note: CMake support for C# projects is limited. For full functionality, use the .NET CLI or Visual Studio.

```bash
mkdir build
cd build
cmake ..
cmake --build .
```

## Usage

1. **Select a Table**: Click on a table from the left panel
2. **Browse Menu**: View available dishes in the menu panel
3. **Add Items**: Select a dish and click "Add Selected Dish" to add it to the current order
4. **Create Order**: Click "Create Order" to finalize the order for the selected table
5. **Track Orders**: Monitor active orders in the orders panel
6. **Manage Status**: Update order status as items are prepared and served

## Data Models

### Table
- ID, Capacity, Location
- Status: Available, Occupied, Reserved, Cleaning

### Dish
- ID, Name, Description, Price
- Category: Appetizer, MainCourse, Dessert, Beverage, Side
- Availability status

### Order
- ID, Table ID, Order Time, Status
- Collection of OrderItems
- Total amount calculation

### OrderItem
- Dish ID, Dish Name, Quantity, Unit Price
- Special instructions
- Subtotal calculation

### Reservation
- ID, Table ID, Customer information
- Reservation time, Number of guests
- Special requests, Confirmation status

### Payment
- ID, Order ID, Amount, Payment Method
- Payment time, Transaction ID

## Technologies

- **C#** - Programming language
- **WPF** - Windows Presentation Foundation for UI
- **MVVM Pattern** - Model-View-ViewModel architecture
- **CommunityToolkit.Mvvm** - MVVM helpers and commands

## License

This project is provided as-is for educational and development purposes.