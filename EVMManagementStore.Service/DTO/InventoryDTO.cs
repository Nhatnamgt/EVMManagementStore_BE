namespace EVMManagementStore.Service.DTO
{
    public class InventoryDTO
    {
        public int InventoryId { get; set; }
        public int VehicleId { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
    }

    public class DispatchRequest
    {
        public int VehicleId { get; set; }
        public int Quantity { get; set; }
        public int DealerId { get; set; } 
    }
}
