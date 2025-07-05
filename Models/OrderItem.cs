namespace MotiZone.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public int? PartId { get; set; }
        public Part? Part { get; set; }

        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}