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
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<GetReportDTO>> GetReportAsync()
        {
            var reports = await _unitOfWork.ReportRepository
                .GetAllIncludeAsync(r => r.User); 

            return reports.Select(r => new GetReportDTO
            {
                ReportId = r.ReportId,
                SenderName = r.User.Username,
                ReportType = r.ReportType,
                CreatedDate = r.CreatedDate,
                ResolvedDate = r.ResolvedDate,
                Content = r.Content,
                Status = r.Status   
            }).ToList();
        }

        public async Task<GetReportDTO> GetReportByIdAsync(int reportid)
        {
            var reports = await _unitOfWork.ReportRepository
                .FindIncludeAsync(r => r.ReportId == reportid, r => r.User);

            var report = reports.FirstOrDefault();
            if (report == null) return null;

            return new GetReportDTO
            {
                ReportId = report.ReportId,
                SenderName = report.User.Username,
                ReportType = report.ReportType,
                CreatedDate = report.CreatedDate,
                ResolvedDate = report.ResolvedDate,
                Content = report.Content,
                Status = report.Status
            };
        }

        //public async Task<ReportDTO> CreateReportAsync(ReportDTO reportDTO)
        //{
        //    var report = new Report
        //    {
        //        UserId = reportDTO.UserId,
        //        OrderId = reportDTO.OrderId,
        //        ReportType = reportDTO.ReportType,
        //        CreatedDate = reportDTO.CreatedDate,
        //        ResolvedDate = reportDTO.ResolvedDate,
        //        Content = reportDTO.Content,
        //        Status = reportDTO.Status
        //    };

        //    await _unitOfWork.ReportRepository.AddAsync(report);
        //    await _unitOfWork.SaveAsync();

        //    var created = (await _unitOfWork.ReportRepository
        //        .FindIncludeAsync(r => r.ReportId == report.ReportId, r => r.User))
        //        .FirstOrDefault();

        //    return new ReportDTO
        //    {
        //        ReportId = created.ReportId,
        //        SenderName = created.User.Username,
        //        UserId = created.UserId,
        //        OrderId = created.OrderId,
        //        ReportType = created.ReportType,
        //        CreatedDate = created.CreatedDate,
        //        ResolvedDate = created.ResolvedDate,
        //        Content = created.Content,
        //        Status = created.Status,
        //    };
        //}

        public async Task<ReportDTO> UpdateReportAsync(int reportid, ReportDTO reportDTO)
        {
            var report = await _unitOfWork.ReportRepository.GetByIdAsync(reportid);
            if (report == null) return null;

            report.UserId = reportDTO.UserId;
            report.OrderId = reportDTO.OrderId;
            report.ReportType = reportDTO.ReportType;
            report.CreatedDate = reportDTO.CreatedDate;
            report.ResolvedDate = reportDTO.ResolvedDate;
            report.Content = reportDTO.Content;
            report.Status = reportDTO.Status;
            await _unitOfWork.ReportRepository.UpdateAsync(report);
            await _unitOfWork.SaveAsync();

            var updated = (await _unitOfWork.ReportRepository
                .FindIncludeAsync(r => r.ReportId == report.ReportId, r => r.User))
                .FirstOrDefault();

            return new ReportDTO
            {
                ReportId = updated.ReportId,
                SenderName = updated.User.Username,
                UserId = updated.UserId,
                OrderId = updated.OrderId,
                ReportType = updated.ReportType,
                CreatedDate = updated.CreatedDate,
                ResolvedDate = updated.ResolvedDate,
                Content = updated.Content,
                Status = updated.Status  
            };
        }

        public async Task<bool> DeleteReportAsync(int reportid)
        {
            var report = await _unitOfWork.ReportRepository.GetByIdAsync(reportid);
            if (report == null) return false;

            await _unitOfWork.ReportRepository.RemoveAsync(report);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
