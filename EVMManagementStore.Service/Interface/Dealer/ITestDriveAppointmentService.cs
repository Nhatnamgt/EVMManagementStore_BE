using EVMManagementStore.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.Dealer
{
    public interface ITestDriveAppointmentService
    {
        Task<List<TestDriveAppointmentDTO>> GetTestDriveAppointmenAsync();
        Task<TestDriveAppointmentDTO> GetTestDriveAppointmentByIdAsync(int testdriveappointmenid);
        Task<TestDriveAppointmentDTO> CreateTestDriveAppointmentAsync(TestDriveAppointmentDTO testDriveAppointmentDTO);
        Task<TestDriveAppointmentDTO> UpdateTestDriveAppointmentAsync(int testdriveappointmenid, TestDriveAppointmentDTO testDriveAppointmentDTO);   
        Task<bool> DeleteTestDriveAppointmentAsync(int testdriveappointmenid);   
    }
}
