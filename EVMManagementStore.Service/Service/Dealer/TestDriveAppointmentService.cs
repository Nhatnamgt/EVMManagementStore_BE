using EVMManagementStore.Repository.Models;
using EVMManagementStore.Repository.UnitOfWork;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Service.Dealer
{
    public class TestDriveAppointmentService : ITestDriveAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TestDriveAppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TestDriveAppointmentDTO>> GetTestDriveAppointmenAsync()
        {
            var appointments = await _unitOfWork.TestDriveAppointmentRepository
                .GetAllIncludeAsync(a => a.User, a => a.Vehicle);

            return appointments.Select(MapToDTO).ToList();
        }

        public async Task<TestDriveAppointmentDTO> GetTestDriveAppointmentByIdAsync(int testdriveappointmentid)
        {
            var appointment = (await _unitOfWork.TestDriveAppointmentRepository
                .FindIncludeAsync(a => a.AppointmentId == testdriveappointmentid,
                                  a => a.User,
                                  a => a.Vehicle))
                .FirstOrDefault();

            return appointment == null ? null : MapToDTO(appointment);
        }

        public async Task<TestDriveAppointmentDTO> CreateTestDriveAppointmentAsync(TestDriveAppointmentDTO dto)
        {
            var entity = new TestDriveAppointment();
            MapToEntity(entity, dto);

            await _unitOfWork.TestDriveAppointmentRepository.AddAsync(entity);
            await _unitOfWork.SaveAsync();

            var created = (await _unitOfWork.TestDriveAppointmentRepository
                .FindIncludeAsync(a => a.AppointmentId == entity.AppointmentId,
                                  a => a.User,
                                  a => a.Vehicle))
                .FirstOrDefault();

            return MapToDTO(created);
        }

        public async Task<TestDriveAppointmentDTO> UpdateTestDriveAppointmentAsync(int testdriveappointmentid, TestDriveAppointmentDTO dto)
        {
            var entity = await _unitOfWork.TestDriveAppointmentRepository.GetByIdAsync(testdriveappointmentid);

            if (entity == null)
                return null;

            MapToEntity(entity, dto);
            await _unitOfWork.TestDriveAppointmentRepository.UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            var updated = (await _unitOfWork.TestDriveAppointmentRepository
                .FindIncludeAsync(a => a.AppointmentId == entity.AppointmentId,
                                  a => a.User,
                                  a => a.Vehicle))
                .FirstOrDefault();

            return MapToDTO(updated);
        }

        public async Task<bool> DeleteTestDriveAppointmentAsync(int testdriveappointmentid)
        {
            var entity = await _unitOfWork.TestDriveAppointmentRepository.GetByIdAsync(testdriveappointmentid);
            if (entity == null)
                return false;

            _unitOfWork.TestDriveAppointmentRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        // ================== Helper Mapping ==================
        private static TestDriveAppointmentDTO MapToDTO(TestDriveAppointment entity)
        {
            return new TestDriveAppointmentDTO
            {
                AppointmentId = entity.AppointmentId,
                AppointmentDate = entity.AppointmentDate,
                Status = entity.Status,
                UserId = entity.UserId,
                VehicleId = entity.VehicleId,
                Username = entity.User?.FullName,
                VehicleName = entity.Vehicle?.Model
            };
        }

        private static void MapToEntity(TestDriveAppointment entity, TestDriveAppointmentDTO dto)
        {
            entity.AppointmentDate = dto.AppointmentDate;
            entity.Status = dto.Status;
            entity.UserId = dto.UserId;
            entity.VehicleId = dto.VehicleId;
        }    
    }

}
