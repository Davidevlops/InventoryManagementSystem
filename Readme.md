# Inventory Management System

A complete desktop application for managing products, suppliers, and stock levels. Built with **WPF**, **MVVM**, **Entity Framework Core**, and **SQLite**.

---

## 📌 Overview

This application helps small to medium businesses track inventory, manage supplier information, and receive alerts when stock levels fall below defined thresholds. It provides a clean, intuitive interface with three main modules:

- **Products** – Add, edit, delete, and view products with supplier details.
- **Suppliers** – Manage company and contact information.
- **Stock Alerts** – Automatically display products that need reordering.

The system uses a local SQLite database, making it portable and easy to deploy.

---

## ✨ Features

### Products

- Create, read, update, and delete products.
- Each product has:
  - Name, description, price
  - Current stock quantity
  - Reorder level (minimum stock before alert)
  - Associated supplier (dropdown selection)

### Suppliers

- Manage supplier records (company name, contact person, phone, email, address).
- Suppliers can be deleted only after confirmation; if they have products, a warning is shown.

### Stock Alerts

- Real-time list of products where `StockQuantity ≤ ReorderLevel`.
- Visual indicators (colors, urgency levels) for quick action.
- Displays supplier information for easy reordering.

### Data Persistence

- All data stored in a single SQLite database file (`inventory.db`).
- Database created automatically on first run with Entity Framework Core.

### User Interface

- Responsive layout with a main navigation bar.
- Split view for products and suppliers: data grid on left, edit form on right.
- Status messages and loading indicators.
- Error handling with user-friendly message boxes.

---

## 🛠️ Technologies Used

| Technology                                   | Purpose                                          |
| -------------------------------------------- | ------------------------------------------------ |
| **.NET 6 / 8**                               | Framework                                        |
| **WPF**                                      | UI framework                                     |
| **MVVM**                                     | Design pattern (via CommunityToolkit.Mvvm)       |
| **Entity Framework Core**                    | Object-relational mapping for database access    |
| **SQLite**                                   | Embedded database engine                         |
| **CommunityToolkit.Mvvm**                    | MVVM helpers: `ObservableObject`, `RelayCommand` |
| **Microsoft.Extensions.DependencyInjection** | Dependency injection container                   |

---

## 📁 Project Structure

InventoryManagementSystem/
│
├── Models/ # Entity classes
│ ├── Product.cs
│ └── Supplier.cs
│
├── Data/ # Database context
│ └── AppDbContext.cs
│
├── ViewModels/ # MVVM ViewModels
│ ├── MainViewModel.cs
│ ├── ProductViewModel.cs
│ ├── SupplierViewModel.cs
│ └── AlertViewModel.cs
│
├── Views/ # XAML UserControls and Windows
│ ├── MainWindow.xaml
│ ├── ProductView.xaml
│ ├── SupplierView.xaml
│ └── AlertView.xaml
│
├── Converters/ (Optional) Value converters for UI
│ ├── StockToStatusConverter.cs
│ └── UrgencyConverter.cs
│
├── App.xaml # Application resources
└── App.xaml.cs # Startup and DI configuration

---

## 🚀 Setup Instructions

### Prerequisites

- Visual Studio 2022 (or later) with **.NET Desktop Development** workload.
- .NET 8 SDK.

### Step 1: Clone or create the project

```bash
git clone <repository-url>
# or create a new WPF project in Visual Studio
```

### Step 2: Install NuGet packages

In Package Manager Console:
Install-Package Microsoft.EntityFrameworkCore.Sqlite
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.Extensions.DependencyInjection
Install-Package CommunityToolkit.Mvvm

### Step 3: Build the solution

Press Ctrl+Shift+B to build. The database will be created automatically when the application runs (via EnsureCreated() or migrations).

### Step 4: Run the application

Press F5 to start. The main window appears with three buttons. You can now:

Add suppliers (required before adding products).

Add products with stock and reorder levels.

View stock alerts.

## 🗄️ Database Schema

The SQLite database consists of two tables:

Suppliers

Column Type Description
Id INTEGER Primary key, auto-increment
Name TEXT Company name (required)
ContactName TEXT Contact person
Phone TEXT Phone number
Email TEXT Email address
Address TEXT Physical address
Products

Column Type Description
Id INTEGER Primary key, auto-increment
Name TEXT Product name (required)
Description TEXT Optional description
Price REAL Unit price
StockQuantity INTEGER Current stock
ReorderLevel INTEGER Threshold for low stock alert
SupplierId INTEGER Foreign key to Suppliers.Id
Relationship: One-to-Many – a supplier can have many products; a product belongs to one supplier.

## 🧩 Key Code Explanations

App.xaml.cs – Dependency Injection
csharp
services.AddDbContext<AppDbContext>();
services.AddSingleton<MainWindow>();
services.AddTransient<ProductViewModel>();
services.AddTransient<SupplierViewModel>();
services.AddTransient<AlertViewModel>();
services.AddSingleton<MainViewModel>();
AddDbContext registers the EF Core context.

AddTransient creates a new instance each time the ViewModel is requested (used for child views).

AddSingleton ensures one instance of MainViewModel and MainWindow for the application lifetime.

ProductViewModel – CRUD Operations
LoadDataAsync: Loads suppliers and products from the database using Include to eagerly load supplier navigation properties.

SaveProductAsync: Adds or updates a product; validation ensures required fields are filled.

DeleteProductAsync: Shows confirmation before removing a product.

AlertViewModel – Low Stock Filtering
private async Task LoadAlertsAsync()
{
await \_context.Products.Include(p => p.Supplier)
.Where(p => p.StockQuantity <= p.ReorderLevel)
.LoadAsync();
LowStockProducts = new ObservableCollection<Product>(\_context.Products.Local);
}

The query filters products where current stock is less than or equal to the reorder level.

MainWindow.xaml – Navigation
<StackPanel Orientation="Horizontal" Margin="10" Grid.Row="0">
<Button Content="Products" Command="{Binding ShowProductsCommand}" />
<Button Content="Suppliers" Command="{Binding ShowSuppliersCommand}" />
<Button Content="Stock Alerts" Command="{Binding ShowAlertsCommand}" />
</StackPanel>
<ContentControl Grid.Row="1" Content="{Binding CurrentView}" />

📖 Usage Guide
Adding a Supplier
Click Suppliers.

Click New, fill in the form, and click Save.

Adding a Product
Click Products.

Ensure at least one supplier exists.

Click New, enter product details, select a supplier from the dropdown, and click Save.

Updating Stock
In the Products tab, click on any product in the grid.

Modify the Stock Quantity field and click Save.

Viewing Alerts
Click Stock Alerts.

A list of products with low stock appears. Use the supplier column to contact them.

Deleting a Supplier
Select the supplier in the grid.

Click Delete. If the supplier has products, a warning will appear.

## 🔮 Future Enhancements

Search / Filter – Quickly find products by name or category.

Sales Transactions – Record sales to automatically deduct stock.

Purchase Orders – Generate reorder lists and print PDFs.

Reports – Export inventory lists to Excel or CSV.

User Authentication – Secure access with login.

Barcode Scanning – Fast product lookup.

Categories – Group products for better organization.

## 🧪 Troubleshooting

The database is not created
Check the output folder (bin\Debug\net6.0) for inventory.db.

Ensure AppDbContext.EnsureCreated() is called in App.xaml.cs startup.

If using migrations, run Update-Database in Package Manager Console.

Data grid shows blank
Verify that the ViewModel's LoadDataCommand is executed (check OnStartup and constructor).

Check the Output window (Debug) for binding errors.

Ensure Products or Suppliers collection is populated before binding.

Buttons are disabled
Confirm that CanSaveProduct() and other commands return true based on validation.

Check IsLoading flag; operations are disabled during loading.

## 📄 License

This project is open-source and available under the MIT License.

## 👥 Contributors

David – Initial design and implementation.

## 🙏 Acknowledgements

Microsoft for WPF and EF Core.

CommunityToolkit.Mvvm for simplifying MVVM.

SQLite for a lightweight database engine.
