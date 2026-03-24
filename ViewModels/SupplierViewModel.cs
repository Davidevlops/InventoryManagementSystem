using System.Collections.ObjectModel;
using InventoryManagementSystem.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.ViewModels
{
    public partial class SupplierViewModel : ObservableObject
    {
        private readonly AppDbContext _context;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DeleteSupplierCommand))]
        private ObservableCollection<Supplier> suppliers = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSupplierCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteSupplierCommand))]
        private Supplier? selectedSupplier;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSupplierCommand))]
        private string name = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSupplierCommand))]
        private string? contactName;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSupplierCommand))]
        private string? phone;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSupplierCommand))]
        private string? email;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSupplierCommand))]
        private string? address;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSupplierCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteSupplierCommand))]
        private bool isEditMode;

        public IAsyncRelayCommand LoadDataCommand { get; }
        public IRelayCommand NewSupplierCommand { get; }
        public IAsyncRelayCommand SaveSupplierCommand { get; }
        public IAsyncRelayCommand DeleteSupplierCommand { get; }
        public IRelayCommand CancelEditCommand { get; }

        public SupplierViewModel(AppDbContext context)
        {
            _context = context;

            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            NewSupplierCommand = new RelayCommand(NewSupplier);
            SaveSupplierCommand = new AsyncRelayCommand(SaveSupplierAsync, CanSaveSupplier);
            DeleteSupplierCommand = new AsyncRelayCommand(DeleteSupplierAsync, CanDeleteSupplier);
            CancelEditCommand = new RelayCommand(CancelEdit);

            // Auto-load on creation
            _ = LoadDataCommand.ExecuteAsync(null);
        }

        private async Task LoadDataAsync()
        {
            Suppliers = new ObservableCollection<Supplier>(
                await _context.Suppliers
                    .Include(s => s.Products)
                    .ToListAsync());
        }

        private void NewSupplier()
        {
            SelectedSupplier = null;
            Name = string.Empty;
            ContactName = null;
            Phone = null;
            Email = null;
            Address = string.Empty;
            IsEditMode = true;
        }

        private bool CanSaveSupplier() =>
            IsEditMode && !string.IsNullOrWhiteSpace(Name);

        private async Task SaveSupplierAsync()
        {
            if (IsEditMode && SelectedSupplier != null)
            {
                // Update existing
                SelectedSupplier.Name = Name;
                SelectedSupplier.ContactName = ContactName;
                SelectedSupplier.Phone = Phone;
                SelectedSupplier.Email = Email;
                SelectedSupplier.Address = Address;
            }
            else
            {
                // Create new
                var supplier = new Supplier
                {
                    Name = Name,
                    ContactName = ContactName,
                    Phone = Phone,
                    Email = Email,
                    Address = Address
                };
                await _context.Suppliers.AddAsync(supplier);
            }

            await _context.SaveChangesAsync();
            await LoadDataAsync();
            CancelEdit();
        }

        private bool CanDeleteSupplier() => SelectedSupplier != null && !IsEditMode;

        private async Task DeleteSupplierAsync()
        {
            if (SelectedSupplier == null) return;

            _context.Suppliers.Remove(SelectedSupplier);
            await _context.SaveChangesAsync();
            await LoadDataAsync();
            CancelEdit();
        }

        private void CancelEdit()
        {
            SelectedSupplier = null;
            Name = string.Empty;
            ContactName = null;
            Phone = null;
            Email = null;
            Address = string.Empty;
            IsEditMode = false;
        }

        partial void OnSelectedSupplierChanged(Supplier? value)
        {
            if (value != null)
            {
                IsEditMode = true;
                Name = value.Name;
                ContactName = value.ContactName;
                Phone = value.Phone;
                Email = value.Email;
                Address = value.Address;
            }
        }
    }
}