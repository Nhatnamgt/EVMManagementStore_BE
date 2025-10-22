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

        public EVMVehicleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

            vehicleDto.VehicleId = newVehicle.VehicleId;
            return vehicleDto;
        }

        public async Task<VehicleDTO?> UpdateVehicleAsync(int id, VehicleDTO vehicleDto)
        {
            var existing = await _unitOfWork.VehicleRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Type = vehicleDto.Type;
            existing.Model = vehicleDto.Model;
            existing.Version = vehicleDto.Version;
            existing.Color = vehicleDto.Color;
            existing.Price = vehicleDto.Price;
            existing.Distance = vehicleDto.Distance;
            existing.Timecharging = vehicleDto.Timecharging;
            existing.Speed = vehicleDto.Speed;
            existing.Image1 = vehicleDto.Image1;
            existing.Image2 = vehicleDto.Image2;
            existing.Image3 = vehicleDto.Image3;
            existing.Status = vehicleDto.Status;

            _unitOfWork.VehicleRepository.Update(existing);
            await _unitOfWork.SaveAsync();

            return vehicleDto;
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
