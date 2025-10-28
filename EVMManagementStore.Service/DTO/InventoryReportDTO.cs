namespace EVMManagementStore.Service.DTO
{
    public class InventoryReportDTO
    {
        public int VehicleId { get; set; }
        public string Type { get; set; }               
        public string Model { get; set; }
        public string Version { get; set; }
        public string Color { get; set; }

        public int DealerId { get; set; }
        public string CompanyName { get; set; }        

        public int DispatchedQuantity { get; set; }
        public int RemainingInStock { get; set; }
        public string Status { get; set; }
    }
}
