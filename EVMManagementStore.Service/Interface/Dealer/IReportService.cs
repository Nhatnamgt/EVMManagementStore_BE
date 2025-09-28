using EVMManagementStore.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.Dealer
{
    public interface IReportService
    {
        Task<List<GetReportDTO>> GetReportAsync();
        Task<GetReportDTO> GetReportByIdAsync(int reportid);
        //Task<ReportDTO> CreateReportAsync(ReportDTO reportDTO);
        Task<ReportDTO> UpdateReportAsync(int reportid, ReportDTO reportDTO);
        Task<bool> DeleteReportAsync(int reportid);
    }
}
