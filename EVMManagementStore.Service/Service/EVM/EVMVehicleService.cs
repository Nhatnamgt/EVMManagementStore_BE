using EVMManagementStore.Repository.Models;
using EVMManagementStore.Repository.UnitOfWork;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.EVM;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Service.EVM
{
    public class EVMVehicleService : IEVMVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountService _discountService;

        public EVMVehicleService(IUnitOfWork unitOfWork, IDiscountService discountService)
        {
            _unitOfWork = unitOfWork;
            _discountService = discountService;
        }

        public async Task<IEnumerable<VehicleDTO>> GetAllVehiclesAsync()
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
                FinalPrice = v.DiscountId != null
                    ? _discountService.CalculateFinalPrice(v)
                    : v.Price,

                DiscountId = v.DiscountId,
                Distance = v.Distance,
                Timecharging = v.Timecharging,
                Speed = v.Speed,
                Image1 = v.Image1,
                Image2 = v.Image2,
                Image3 = v.Image3,
                Status = v.Status
            }).ToList();
        }

        public async Task<VehicleDTO?> GetVehicleByIdAsync(int id)
        {
            var v = await _unitOfWork.VehicleRepository.GetByIdAsync(id);
            if (v == null) return null;

            return new VehicleDTO
            {
                VehicleId = v.VehicleId,
                Type = v.Type,
                Model = v.Model,
                Version = v.Version,
                Color = v.Color,

                Price = v.Price,
                FinalPrice = v.DiscountId != null
                    ? _discountService.CalculateFinalPrice(v)
                    : v.Price,

                DiscountId = v.DiscountId,
                Distance = v.Distance,
                Timecharging = v.Timecharging,
                Speed = v.Speed,
                Image1 = v.Image1,
                Image2 = v.Image2,
                Image3 = v.Image3,
                Status = v.Status
            };
        }

        public async Task<VehicleDTO> CreateVehicleAsync(VehicleDTO vehicleDto)
        {
            var newVehicle = new Vehicle
            {
                Type = vehicleDto.Type,
                Model = vehicleDto.Model,
                Version = vehicleDto.Version,
                Color = vehicleDto.Color,
                Price = vehicleDto.Price,
                FinalPrice = vehicleDto.Price, 
                DiscountId = null,
                Distance = vehicleDto.Distance,
                Timecharging = vehicleDto.Timecharging,
                Speed = vehicleDto.Speed,
                Image1 = vehicleDto.Image1,
                Image2 = vehicleDto.Image2,
                Image3 = vehicleDto.Image3,
                Status = vehicleDto.Status
            };

            await _unitOfWork.VehicleRepository.AddAsync(newVehicle);
            await _unitOfWork.SaveAsync();

            await _unitOfWork.InventoryRepository.AddAsync(new Inventory { VehicleId = newVehicle.VehicleId, Quantity = 0 });
            await _unitOfWork.SaveAsync();

            vehicleDto.VehicleId = newVehicle.VehicleId;
            vehicleDto.FinalPrice = newVehicle.FinalPrice;
            return vehicleDto;
        }

        public async Task<VehicleDTO?> UpdateVehicleAsync(int id, VehicleDTO dto)
        {
            var existing = await _unitOfWork.VehicleRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Type = dto.Type;
            existing.Model = dto.Model;
            existing.Version = dto.Version;
            existing.Color = dto.Color;
            existing.Price = dto.Price;
            existing.Distance = dto.Distance;
            existing.Timecharging = dto.Timecharging;
            existing.Speed = dto.Speed;
            existing.Image1 = dto.Image1;
            existing.Image2 = dto.Image2;
            existing.Image3 = dto.Image3;
            existing.Status = dto.Status;

            if (existing.DiscountId != null)
                existing.FinalPrice = _discountService.CalculateFinalPrice(existing);
            else
                existing.FinalPrice = existing.Price;

            _unitOfWork.VehicleRepository.Update(existing);
            await _unitOfWork.SaveAsync();

            return dto;
        }

        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(id);
            if (vehicle == null) return false;

            _unitOfWork.VehicleRepository.Remove(vehicle);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
