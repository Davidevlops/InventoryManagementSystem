// Models/Product.cs
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int ReorderLevel { get; set; }   // minimum stock before alert

    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
}
