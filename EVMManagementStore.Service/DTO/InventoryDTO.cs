namespace EVMManagementStore.Service.DTO
{
    public class InventoryDTO
    {
        public int InventoryId { get; set; }
        public int VehicleId { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public string Version { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public string Distance { get; set; }
        public string Timecharging { get; set; }
        public string Speed { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
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
