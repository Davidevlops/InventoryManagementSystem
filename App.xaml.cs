// App.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.ViewModels;
using System.Windows;
using InventoryManagementSystem.Views;

namespace InventoryManagementSystem
{
    public partial class App : Application
    {

        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
               {
                   services.AddDbContext<AppDbContext>();

                   services.AddSingleton<MainWindow>();

                   // Views (UserControls)
                   services.AddTransient<ProductView>();
                   services.AddTransient<SupplierView>();
                   services.AddTransient<AlertView>();

                   // ViewModels
                   services.AddTransient<ProductViewModel>();
                   services.AddTransient<SupplierViewModel>();
                   services.AddTransient<AlertViewModel>();
                   services.AddTransient<MainViewModel>();
               })
                .Build();
        }

        // protected override async void OnStartup(StartupEventArgs e)
        // {
        //     await _host.StartAsync();

        //     // Ensure database is created
        //     using (var scope = _host.Services.CreateScope())
        //     {
        //         var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        //         db.Database.EnsureCreated();
        //     }

        //     var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        //     mainWindow.DataContext = _host.Services.GetRequiredService<MainViewModel>();
        //     mainWindow.Show();

        //     base.OnStartup(e);
        // }
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                await _host.StartAsync();

                // Database seeding (kept exactly as before)
                using (var scope = _host.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var created = db.Database.EnsureCreated();

                    if (created)
                    {
                        Console.WriteLine("✅ Database created");
                        if (!db.Suppliers.Any())
                        {
                            db.Suppliers.Add(new Supplier
                            {
                                Name = "Rolex Supplier",
                                ContactName = "Raman Jago",
                                Phone = "+234708959433",
                                Email = "raman.jago@gmail.com"
                            });
                            await db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Suppliers: {db.Suppliers.Count()} | Products: {db.Products.Count()}");
                    }
                }

                // Create everything
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.DataContext = _host.Services.GetRequiredService<MainViewModel>();
                mainWindow.Show();

                Console.WriteLine("✅ App started successfully");
            }
            catch (Exception ex)
            {
                // This will show the REAL error that was previously hidden
                MessageBox.Show(
                    $"Startup crashed!\n\n{ex.Message}\n\nFull error:\n{ex}",
                    "Inventory App - Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // Also print to console (useful when running dotnet run)
                Console.WriteLine("=== CRASH ===");
                Console.WriteLine(ex.ToString());
            }

            base.OnStartup(e);
        }
        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }
}