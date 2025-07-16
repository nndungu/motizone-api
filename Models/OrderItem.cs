namespace MotiZone.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? VehicleId { get; set; }
        public int? PartId { get; set; }
        public int? CustomizationId { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }

        
        public Order Order { get; set; } = null!;
        public Vehicle? Vehicle { get; set; }
        public Part? Part { get; set; }
        public Customization Customization { get; set; } = null!;
    }
}