using System.Collections.ObjectModel;
using InventoryManagementSystem.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.ViewModels
{
    public partial class ProductViewModel : ObservableObject
    {
        private readonly AppDbContext _context;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        private ObservableCollection<Product> products = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        private ObservableCollection<Supplier> suppliers = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteProductCommand))]
        private Product? selectedProduct;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        private string productName = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        private string? description;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        private decimal price;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        private int stockQuantity;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        private int reorderLevel;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        private Supplier? selectedSupplier;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteProductCommand))]
        private bool isEditMode;

        public IAsyncRelayCommand LoadDataCommand { get; }
        public IRelayCommand NewProductCommand { get; }
        public IAsyncRelayCommand SaveProductCommand { get; }
        public IAsyncRelayCommand DeleteProductCommand { get; }
        public IRelayCommand CancelEditCommand { get; }

        public ProductViewModel(AppDbContext context)
        {
            _context = context;

            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            NewProductCommand = new RelayCommand(NewProduct);
            SaveProductCommand = new AsyncRelayCommand(SaveProductAsync, CanSaveProduct);
            DeleteProductCommand = new AsyncRelayCommand(DeleteProductAsync, CanDeleteProduct);
            CancelEditCommand = new RelayCommand(CancelEdit);

            // Auto-load on creation (now consistent with SupplierViewModel)
            _ = LoadDataCommand.ExecuteAsync(null);
        }

        public async Task LoadDataAsync()
        {
            Suppliers = new ObservableCollection<Supplier>(
                await _context.Suppliers.ToListAsync());

            Products = new ObservableCollection<Product>(
                await _context.Products
                    .Include(p => p.Supplier)
                    .ToListAsync());
        }

        private void NewProduct()
        {
            SelectedProduct = new Product();
            IsEditMode = true;
            ClearInputFields();
        }

        private void CancelEdit()
        {
            SelectedProduct = null;
            IsEditMode = false;
            ClearInputFields();
        }

        private void ClearInputFields()
        {
            ProductName = string.Empty;
            Description = null;
            Price = 0;
            StockQuantity = 0;
            ReorderLevel = 0;
            SelectedSupplier = null;
        }

        private bool CanSaveProduct() =>
            IsEditMode &&
            !string.IsNullOrWhiteSpace(ProductName) &&
            Price >= 0 &&
            StockQuantity >= 0 &&
            ReorderLevel >= 0 &&
            SelectedSupplier != null;

        private async Task SaveProductAsync()
        {
            if (SelectedProduct == null) return;

            // Update entity
            SelectedProduct.Name = ProductName;
            SelectedProduct.Description = Description;
            SelectedProduct.Price = Price;
            SelectedProduct.StockQuantity = StockQuantity;
            SelectedProduct.ReorderLevel = ReorderLevel;
            SelectedProduct.SupplierId = SelectedSupplier!.Id;
            // Navigation property is set for UI only – EF will use FK
            SelectedProduct.Supplier = SelectedSupplier;

            if (SelectedProduct.Id == 0)
                await _context.Products.AddAsync(SelectedProduct);

            await _context.SaveChangesAsync();
            await LoadDataAsync();

            // Reset form
            IsEditMode = false;
            SelectedProduct = null;
            ClearInputFields();
        }

        private bool CanDeleteProduct() => SelectedProduct != null && !IsEditMode;

        private async Task DeleteProductAsync()
        {
            if (SelectedProduct == null) return;

            _context.Products.Remove(SelectedProduct);
            await _context.SaveChangesAsync();
            await LoadDataAsync();
        }

        partial void OnSelectedProductChanged(Product? value)
        {
            if (value != null)
            {
                IsEditMode = true;
                ProductName = value.Name;
                Description = value.Description;
                Price = value.Price;
                StockQuantity = value.StockQuantity;
                ReorderLevel = value.ReorderLevel;
                SelectedSupplier = value.Supplier;
            }
        }
    }
}