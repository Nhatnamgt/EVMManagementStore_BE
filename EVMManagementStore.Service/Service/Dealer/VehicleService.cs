using EVMManagementStore.Repository.UnitOfWork;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Dealer
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehicleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<VehicleDTO>> GetAllVehicle()
        {
            var vehicles = await _unitOfWork.VehicleRepository.GetAllAsync();

            return vehicles.Select(v => new VehicleDTO
            {
                VehicleId = v.VehicleId,
                Type = v.Type,
                Model = v.Model,
                Version = v.Version,
                Color = v.Color,
                Price = v.Price,
                Image = v.Image,    
                Status = v.Status
            }).ToList();
        }

        public async Task<VehicleDTO?> GetVehicleById(int vehicleId)
        {
            var v = await _unitOfWork.VehicleRepository.GetByIdAsync(vehicleId);
            if (v == null) return null;

            return new VehicleDTO
            {
                VehicleId = v.VehicleId,
                Type = v.Type,
                Model = v.Model,
                Version = v.Version,
                Color = v.Color,
                Price = v.Price,
                Image = v.Image,
                Status = v.Status
            };
        }

        public async Task<VehicleComparisonDTO> CompareVehicles(int vehicleId1, int vehicleId2)
        {
            var vehicle1 = await _unitOfWork.VehicleRepository.GetByIdAsync(vehicleId1);
            var vehicle2 = await _unitOfWork.VehicleRepository.GetByIdAsync(vehicleId2);

            if (vehicle1 == null || vehicle2 == null)
                return null;

            var dto1 = new VehicleDTO
            {
                VehicleId = vehicle1.VehicleId,
                Type = vehicle1.Type,
                Model = vehicle1.Model,
                Version = vehicle1.Version,
                Color = vehicle1.Color,
                Price = vehicle1.Price,
                Status = vehicle1.Status
            };

            var dto2 = new VehicleDTO
            {
                VehicleId = vehicle2.VehicleId,
                Type = vehicle2.Type,
                Model = vehicle2.Model,
                Version = vehicle2.Version,
                Color = vehicle2.Color,
                Price = vehicle2.Price,
                Status = vehicle2.Status
            };

            return new VehicleComparisonDTO
            {
                Vehicle1 = dto1,
                Vehicle2 = dto2,
                PriceDifference = dto1.Price - dto2.Price,

                // So sánh chi tiết
                TypeComparison = dto1.Type == dto2.Type ? "Giống nhau" : $"{dto1.Type} vs {dto2.Type}",
                ModelComparison = dto1.Model == dto2.Model ? "Giống nhau" : $"{dto1.Model} vs {dto2.Model}",
                VersionComparison = dto1.Version == dto2.Version ? "Giống nhau" : $"{dto1.Version} vs {dto2.Version}",
                ColorComparison = dto1.Color == dto2.Color ? "Giống nhau" : $"{dto1.Color} vs {dto2.Color}",
                StatusComparison = dto1.Status == dto2.Status ? "Giống nhau" : $"{dto1.Status} vs {dto2.Status}"
            };
        }

    }
}